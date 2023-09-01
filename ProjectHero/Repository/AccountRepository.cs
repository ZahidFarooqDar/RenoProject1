using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using ProjectHero.DomainModal;
using ProjectHero.ServiceModals;

namespace ProjectHero.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<HeroUser> _userManager;
       /* Constructor typically in other repository we used DbContext here
        Here we are using UserManager-> To deal with user (CRUD) we use managers provided by IdentityCore
        HeroUser is actually our Model controlled by Identity Core */

        public AccountRepository(UserManager<HeroUser> userManager) 
        {
            _userManager = userManager;
        }
        public async Task<IdentityResult> SignUpAsync(SignUpModel signUpModel)
        {
            //We need to create a new user , using userManager
            //Remember usermanager will only work with HeroUser
            //Here we need to create new instance of HeroUser
            /*var isExisting = _userManager.FindByEmailAsync(signUpModel.Email);

           public async Task<IdentityResult> SignUpAsync(SignUpModel signUpModel)
        {
            //We need to create a new user , using userManager
            //Remember usermanager will only work with HeroUser
            //Here we need to create new instance of HeroUser
            /*var isExisting = _userManager.FindByEmailAsync(signUpModel.Email);

            if (isExisting != null)
            {
                
            }*/
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

            var user = new HeroUser()
            {
                FirstName = signUpModel.FirstName,
                LastName = signUpModel.LastName,
                Email = signUpModel.Email,
                UserName = signUpModel.FirstName //UserName property is not available in SignUpModel, We can create at their
                                                //But here we created it and assigned to the FirstName given
               
            };
            string password = signUpModel.Password;
            //Note we did'nt assign password above as it is because needs to pass separately and separated parameter
            return await _userManager.CreateAsync(user, password);
        }
    }
}
