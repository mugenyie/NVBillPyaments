using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NVBillPayments.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class ProjectHealth : ControllerBase
    {
        private readonly ICachingService _cachingService;
        public ProjectHealth(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Bill Payments Project Healthy and Running!!");
        }

        [HttpGet]
        [Route("TestCache")]
        public async Task<IActionResult> GetKeyAsync(string key)
        {
            var result = await _cachingService.Get<object>(key);
            return Ok(result);
        }

        [HttpPost]
        [Route("TestCache")]
        public async Task<IActionResult> SetKeyAsync(string key, string value, int ttl)
        {
            await _cachingService.Set(key, value, ttl);
            return Ok();
        }
    }
}
