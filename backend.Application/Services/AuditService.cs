using backend.Domain.Entities;
using backend.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;

        public AuditService(ILogger<AuditService> logger)
        {
            _logger = logger;
        }

        public Task LogAsync(AuditLog log)
        {
            _logger.LogInformation(
                "AUDIT | Event={Event} User={UserId} IP={IP} Device={Device} Metadata={Metadata}",
                log.Event,
                log.UserId,
                log.Ip,
                log.DeviceId,
                log.MetadataJson);

            return Task.CompletedTask;
        }
    }
}

