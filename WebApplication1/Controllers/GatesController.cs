using Microsoft.AspNetCore.Mvc;
namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/gates")]
    public class GatesController : ControllerBase
    {
        private readonly IAccessCodeValidationService _validationService;

        public GatesController(IAccessCodeValidationService validationService)
        {
            _validationService = validationService;
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] AccessCodeRequest? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AccessCode))
            {
                var result = new ValidationResult
                {
                    IsValid = false,
                    Errors = new List<string> { "Код доступа отсутствует или пустой." },
                    Message = "Код отклонён."
                };
                return BadRequest(result);
            }

            var validationResult = _validationService.Validate(request.AccessCode);

            if (validationResult.IsValid)
            {
                return Ok(validationResult);
            }

            return BadRequest(validationResult);
        }
    }
}
