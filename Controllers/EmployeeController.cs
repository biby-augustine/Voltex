using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        public ILogger<EmployeeController> _logger;
        public EmployeeController(EmployeeService employeeService,
            ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
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
                    return Ok(new { result = await _employeeService.ProcessExcel(Request.Form.Files[0], cancellationToken) });
                else
                    throw new ArgumentNullException("File not found.");
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(ex.HResult), ex, "{Message}", ex.Message);
                return BadRequest();
            }
            
        }
        [HttpGet("getall")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(int page,int pageSize,CancellationToken cancellationToken)
        {
            return Ok(await _employeeService.GetEmployees(page, pageSize, cancellationToken));
        }
    }
}
