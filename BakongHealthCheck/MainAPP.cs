using System.Data;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using BakongHealthCheck.Configures;
using BakongHealthCheck.Entities;
using BakongHealthCheck.Repository;
using BakongHealthCheck.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Serilog;

namespace BakongHealthCheck
{
    public class MainAPP : IHostedService , IDisposable
    {
        private Timer? _timer;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IConfigureBakong configure;
        private CancellationTokenSource? _currentApiCts;

        public MainAPP(IServiceScopeFactory scopeFactory, IConfigureBakong configure)
        {
            this.scopeFactory = scopeFactory;
            this.configure = configure;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _currentApiCts = new CancellationTokenSource();
            int schduleTime = Convert.ToInt32(configure.BakongTimeService);

            XDocument doc = XDocument.Load(Environment.CurrentDirectory + "" + configure.BakongSchedule);
            var scheduleTime = doc.Descendants("schedule").Descendants("processtime")
                               .Select(e => new
                               {
                                   StartTime = e.Element("start")?.Value,
                                   EndTime = e.Element("end")?.Value,
                                   Service = e.Element("service")?.Value
                               }).FirstOrDefault();

            DateTime startTime = Convert.ToDateTime(scheduleTime.StartTime);
            DateTime endTime = Convert.ToDateTime(scheduleTime.EndTime);

            if (startTime <= DateTime.Now && DateTime.Now <= endTime && configure.BakongButton == "ON") 
            {
                _timer = new Timer(Process, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(schduleTime));
            }
            else if (configure.BakongButton == "ON" && scheduleTime.Service != "UP") // let service process until 
            {
                _timer = new Timer(Process, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(schduleTime));
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _currentApiCts?.Cancel();
            _currentApiCts?.Dispose();
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Process(object? state)
        {
            //  Cancel the previous API call if it's still running create a NEW CancellationTokenSource for the upcoming call
            var oldCts = Interlocked.Exchange(ref _currentApiCts, new CancellationTokenSource());
            oldCts?.Cancel();
            oldCts?.Dispose();

            var newCancellationToken = _currentApiCts!.Token;

            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    var service = scope.ServiceProvider.GetRequiredService<IBCService>();
                    _ = service.BakongHealthCheck(newCancellationToken);
                }
                catch (TaskCanceledException ex)
                {
                    if (newCancellationToken.IsCancellationRequested) Log.Error("Background Service> main app was cancelled by a new schedule. | " + ex.Message);
                    else Log.Error("Background Service> main app timed out or was cancelled for another reason. | " + ex.Message);
                }
                catch (Exception ex)
                {
                    Log.Error("Background Service> main app failed | " + ex.Message);;
                }
            }
        }

        public void Dispose()
        {
            _currentApiCts?.Cancel();
            _currentApiCts?.Dispose();
            _timer?.Dispose();
            _timer = null;
        }
    }
}
