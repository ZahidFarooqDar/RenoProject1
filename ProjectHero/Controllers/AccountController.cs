using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectHero.Repository;
using ProjectHero.ServiceModals;

namespace ProjectHero.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Controller for Authentication purpose
    public class AccountController : ControllerBase  
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
        {
            var result = await _accountRepository.SignUpAsync(signUpModel);
            if (result.Succeeded)
            {
                return Ok("Sign Up Successful....");
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "UserAlreadyExists")
                {
                    // User already exists, return a custom message
                    return BadRequest(new { Message = error.Description, Action = "Sign In Instead" });
                }
            }

            // Handle other validation errors
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }
    }
}
