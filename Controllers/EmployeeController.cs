using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ExcelService _excelService;
        public ILogger<EmployeeController> _logger;
        public EmployeeController(ExcelService excelService,
            ILogger<EmployeeController> logger)
        {
            _excelService = excelService;
            _logger = logger;
        }
        [HttpPost("savefile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveFile(CancellationToken cancellationToken)
        {
            try
            {
                if (Request.Form != null && Request.Form.Files != null && Request.Form.Files[0] != null)
                    return Ok(new { result = await _excelService.ProcessExcel(Request.Form.Files[0], cancellationToken) });
                else
                    throw new ArgumentNullException("File not found.");
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(ex.HResult), ex, "{Message}", ex.Message);
                return BadRequest();
            }
            
        }
    }
}
