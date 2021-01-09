using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcVillage.Engine;
using Microsoft.Extensions.Logging;

namespace GrpcVillage
{
    public class CreaterService : Creater.CreaterBase
    {
        private readonly ILogger<CreaterService> _logger;
        public CreaterService(ILogger<CreaterService> logger)
        {
            _logger = logger;
        }

        public override async Task StartVillage(VillageStartup villageStartup, IServerStreamWriter<VillageStatus> responseStream, ServerCallContext context)
        {
            var village = new Village(villageStartup, context);
            await village.Run(responseStream);
        }
    }
}
