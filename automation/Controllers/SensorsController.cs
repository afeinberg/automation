using automation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace automation.Controllers
{

    [Route("/sensors")]
    public class SensorsController : Controller
    {
        private readonly TelemetryDbContext _context;
        private readonly ILogger<SensorsController> _logger;

        public SensorsController(TelemetryDbContext context, ILogger<SensorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Route("add_reading")]
        [HttpPost()]
        public IActionResult AddTelemetryData([FromBody] TelemetryData data)
        {
            _logger.LogDebug("Received telemetry data {}", data);
            if (!data.Timestamp.HasValue)
            {
                data.Timestamp = DateTime.Now;
            }

            _context.TelemetryData.Add(data);
            _context.SaveChanges();
            return StatusCode(200, data);
        }

        [Route("last/{seconds?}")]
        [HttpGet]
        public ICollection<DecoratedTelemetryData> GetLast(long? seconds)
        {
            return _context.TelemetryData
                .FromSql("SELECT * FROM \"TelemetryData\" where \"Timestamp\" > NOW() - {0}",
                    TimeSpan.FromSeconds(seconds ?? 3600 * 24))
                .ToList()
                .Select(td => new DecoratedTelemetryData(td,
                    DateTimeOffset.FromFileTime((td.Timestamp ?? DateTime.Now).ToFileTime()).ToUnixTimeMilliseconds()))
                .ToList();
        }
    }
}