using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using ProjectHero.DomainModal;
using ProjectHero.ServiceModals;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Xml;

namespace ProjectHero.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<HeroUser> _userManager;
        /*
          The SignInManager class in ASP.NET Identity is a fundamental component for managing user sign-in operations in an ASP.NET Core application. 
          It provides a convenient and secure way to handle user authentication and related tasks. 
        */
        private readonly SignInManager<HeroUser> _signInManager;
        private readonly IConfiguration _configuration;  //For reading settings from appsetting.json

        /* Constructor typically in other repository we used DbContext here
           Here we are using UserManager-> To deal with user (CRUD) we use managers provided by IdentityCore
           HeroUser is actually our Model controlled by Identity Core */

        public AccountRepository(UserManager<HeroUser> userManager, SignInManager<HeroUser> signInManager , IConfiguration configuration) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        public async Task<IdentityResult> SignUpAsync(SignUpModel signUpModel)
        {
          
            // Check if a user with the same email address already exists
            var existingUser = await _userManager.FindByEmailAsync(signUpModel.Email);
            if (existingUser != null)
            {
                // A user with the same email already exists, return a custom IdentityResult
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserAlreadyExists",
                    Description = "A user with this email address already exists. Please sign in instead."
                });
            }
            //We need to create a new user , using userManager
            //Remember usermanager will only work with HeroUser
            //Here we need to create new instance of HeroUser

            var user = new HeroUser()
            {
                FirstName = signUpModel.FirstName,
                LastName = signUpModel.LastName,
                Email = signUpModel.Email,
                UserName = signUpModel.Email //UserName property is not available in SignUpModel, We can create at their
                                                //But here we created it and assigned to the FirstName given
               
            };
            string password = signUpModel.Password;
            //Note we did'nt assign password above as it is because needs to pass separately and separated parameter
            return await _userManager.CreateAsync(user, password);
        }
        public async Task<string> LoginAsync (SignInModel signInModel)
        {
            var result = await _signInManager.PasswordSignInAsync(signInModel.Email, signInModel.Password, false, false); //Token
            /*3rd parameter is actually isPersistent => whether u want automatic login or not means if login once, can be login in automatically another time
              4th parameter is lockOutFailure => Means if we want to lock the account after some wrong attempts.
              The third parameter, false, means that you don't want to enable automatic login (persistent cookie).
              The fourth parameter, false, means that you don't want to lock the user's account due to too many failed login attempts.
            */
            if (!result.Succeeded)
            {
                return null;
            }
            //Add Claims
                /*
                Here, you create a list of claims that will be added to the JWT token. 
                Claims are pieces of information about the user, such as their name, email, or roles. 
                In this case, you're adding the user's email as the name claim and a unique identifier(Jti) generated using Guid.NewGuid().
                */
            var authclaims = new List<Claim> {
                new Claim(ClaimTypes.Name, signInModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
                //Add New Authentication Key
            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
                //Generate new Token
            var token = new JwtSecurityToken(
                //Here we need to pass some settings
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),   //Setting validity for Particular Token
                claims: authclaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)  
                //This specifies the key and the signing algorithm (HMACSHA256) used to sign the token.
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
