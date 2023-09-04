using Microsoft.AspNetCore.Identity;
using ProjectHero.ServiceModals;

namespace ProjectHero.Repository
{
    public interface IAccountRepository
    {
         Task<IdentityResult> SignUpAsync(SignUpModel signUpModel);
        Task<string> LoginAsync(SignInModel signInModel);
    }
    
}
