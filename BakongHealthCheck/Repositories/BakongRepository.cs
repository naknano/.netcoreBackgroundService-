using BakongHealthCheck.Entities;
using BakongHealthCheck.Repositories;
using BakongHealthCheck.Data;
using BakongHealthCheck.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure;
using Serilog;
using BakongHealthCheck.Dto.Bakong;
using System;
using BakongHealthCheck.Services.Bakong;
using AutoMapper;
using BakongHealthCheck.Dto.MBService;
using BakongHealthCheck.Dto;
using BakongHealthCheck.Util;
using System.Threading;
using BakongHealthCheck.Services;
using System.Xml;
using System.Xml.Linq;
using BakongHealthCheck.Configures;

namespace BakongHealthCheck.Repositories
{
    public class BakongRepository : IBakongRepository
    {
        private readonly AppDBContext appDbContext;
        private readonly IBakongService bakongService;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IConfigureBakong configure;
        private readonly IMapper mapper;


        public BakongRepository(AppDBContext appDbContext, IBakongService bakongService, 
            IServiceScopeFactory scopeFactory, IMapper mapper, IConfigureBakong configure)
        {
            this.appDbContext = appDbContext;
            this.bakongService = bakongService;
            this.scopeFactory = scopeFactory;
            this.mapper = mapper;
            this.configure = configure;
        }

        public async Task<ResponseV1DTO> updateMBService(string status, int regId, string remark)
        {
            var result = new ResponseV1DTO();
            try
            {
                // for open service again ( httprequest already close connect by singleton )
                var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                var mbService = await service.mbService.FirstOrDefaultAsync(b => b.recID == regId 
                                                                            && b.serviceID == BKBC.serviceCode 
                                                                            && b.status == BKBC.userCodeError);
                // Update to down
                mbService.endDate = DateTime.Now;
                if (status == "DOWN")
                {
                    mbService.remark = remark;
                    mbService.status = "DENY";
                }
                else
                {
                    mbService.remark = remark;
                    mbService.status = "ALL";
                }
                service.Update(mbService);
                await service.SaveChangesAsync();


                // Update xml
                XDocument xmlDoc = XDocument.Load(Environment.CurrentDirectory + configure.BakongSchedule);
                foreach (var processtime in xmlDoc.Descendants("processtime"))
                {
                    var serviceElement = processtime.Element("service");
                    if (serviceElement != null)
                    {
                        serviceElement.Value = mbService.status == "ALL" ? "UP" : "DOWN";
                    }
                }
                xmlDoc.Save(Environment.CurrentDirectory + configure.BakongSchedule);

             
                return result = new ResponseV1DTO()
                {
                    responseCode = "2000",
                    responseMessage = "Updated was successfully!!"
                };
            }
            catch (Exception ex)
            {
                Log.Information("BakongHealthCheck > Update MBService catch : " + ex.Message);
                return new ResponseV1DTO()
                {
                    responseCode = BKBC.code4000,
                    responseMessage = ex.Message
                };
            }
        }

        public async Task<ResponseV1DTO> getBakongHealth()
        {
            var result = new ResponseV1DTO();
            try
            {
                // for open service again ( httprequest already close connect by singleton )
                var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                var response = await service.mbService.FirstOrDefaultAsync(b => b.serviceID == BKBC.serviceCode);
                if (response == null)
                {
                    Log.Information("BakongHealthCheck > MBService from DB |Data response " + response);
                    result = new ResponseV1DTO()
                    {
                        responseCode = BKBC.code4000,
                        responseMessage = BKBC.message4000,
                        responseMBServiceDTO = null
                    };
                }
                else
                {
                    Log.Information("BakongHealthCheck > MBService from DB |Data response " + response);
                    result = new ResponseV1DTO()
                    {
                        responseCode = BKBC.code2000,
                        responseMessage = BKBC.message4001,
                        responseMBServiceDTO = new ResponseMBServiceDTO()
                        {
                            recID = response.recID,
                            blackListVersion = response.blackListVersion,
                            endDate = response.endDate,
                            remark = response.remark,
                            serviceID = response.serviceID,
                            startDate = response.startDate,
                            status = response.status,
                            userID = response.userID
                        }
                    };
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Information("BakongHealthCheck > MBService from DB |Catch error : " + ex.Message);
                return new ResponseV1DTO()
                {
                    responseCode = BKBC.code4000,
                    responseMessage = ex.Message
                };
            }
        }

        public async Task<ResponseV1DTO> createBakongHealthCheck(CancellationToken cancellationToken)
        {
            var result = new ResponseV1DTO();
            var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            try
            {
                ResponseBakongHealthDTO bakongHealth = await bakongService.GetBakongHealth(cancellationToken);
                ResponseV1DTO DBMBService = await getBakongHealth();
                if (bakongHealth == null || bakongHealth.code != "000")
                {
                    // MbService found data
                    if (DBMBService.responseCode == "2000" && DBMBService.responseMBServiceDTO.status == "DENY")
                    {
                        string remark = bakongHealth == null ? "Service response null" : "Status : " + bakongHealth.result.status + " - Message : " + bakongHealth.message;
                        await updateMBService("DOWN", DBMBService.responseMBServiceDTO.recID, remark);
                    }
                    else // MbService not found data
                    {
                        RequestMBServiceDTO request = new RequestMBServiceDTO()
                        {
                            userID = BKBC.userCodeError,
                            serviceID = BKBC.serviceCode,
                            startDate = DateTime.Now,
                            endDate = DateTime.Now,
                            status = BKBC.userCodeError,
                            remark = "Status : MB_Serivce response null from DB!!",
                            blackListVersion = ""
                        };

                        var requestData = mapper.Map<MBService>(request);
                        await service.mbService.AddAsync(requestData);
                        await service.SaveChangesAsync();
                    }

                    Log.Information("BakongHealthCheck > createBakongHealthCheck > got status from bakong api null.");
                    result = new ResponseV1DTO()
                    {
                        responseCode = BKBC.code4000,
                        responseMessage = BKBC.message4000
                    };
                }

                else if(bakongHealth.code == "000") // SERVICE UP
                {
                    // Prevent update many times
                    if (DBMBService.responseMBServiceDTO.status == "DENY")
                    {
                        string remark = "Status : " + bakongHealth.result.status + " - Message : " + bakongHealth.message;
                        await updateMBService("UP", DBMBService.responseMBServiceDTO.recID, remark);
                    }

                    Log.Information("BakongHealthCheck > createBakongHealthCheck > getBakongHealthV2 : " + bakongHealth.result.status);
                    result = new ResponseV1DTO()
                    {
                        responseCode = BKBC.code4000,
                        responseMessage = BKBC.message4000
                    };
                }
                return result;
                     
            }
            catch (TaskCanceledException ex)
            {
                // No need add log
                throw new TaskCanceledException(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Information("BakongHealthCheck > createBakongHealthCheck > Catch Error " + ex.Message);
                return new ResponseV1DTO()
                {
                    responseCode = BKBC.code4000,
                    responseMessage = ex.Message
                };
            }
        }
    }
}
