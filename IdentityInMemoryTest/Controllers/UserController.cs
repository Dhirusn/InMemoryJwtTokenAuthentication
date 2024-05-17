using IdentityInMemoryTest.Models;
using IdentityInMemoryTest.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityInMemoryTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly SignInManager<AppUser> _signInManager;
        public readonly UserManager<AppUser> _userManager;
        public readonly IJwtTokenHandler _jwtTokenHandler;


        public UserController(SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager,
            IJwtTokenHandler jwtTokenHandler)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtTokenHandler = jwtTokenHandler;
        }


        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync([FromBody] AppUser user){
            if(user != null)
            {
                var checkUser = await _userManager.FindByNameAsync(user.Name!);
                if(checkUser == null)
                {
                    await _userManager.CreateAsync(user);
                    return Ok();
                }
            }
            return NotFound();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (login != null)
            {
                var user =await _userManager.FindByNameAsync(login.UserName);
                if (user != null)
                {
                    var canUserSignIn = await _signInManager.CanSignInAsync(user);
                    if (canUserSignIn)
                    {
                        return Ok(_jwtTokenHandler.GenerateJSONWebToken(user));

                    }
                }
            }
            return NotFound();
        }

    }
}
