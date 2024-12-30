using InterServiceCenter_Core.Services;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/metrics")]
public class MetricController : ControllerBase
{
    private readonly JwtToken _token;
    private readonly MetricService _metricService;
    
    public MetricController(MetricService metricService, JwtToken token)
    {
        _metricService = metricService;
        _token = token;
    }

    [HttpGet("post/monthly")]
    public IActionResult GetTotalPostsTwelveMonths()
    {
        var response = _metricService.GetTotalPostsTwelveMonths();
        return StatusCode(response.Result.StatusCode, new { todaysYear = response.Result.TodaysYear, lastYear = response.Result.LastYear });
    }
}