using FluentValidation;
using UserManagement.ApiService.Features.AuthManagement.RegisterUser;

namespace UserManagement.ApiService.Features.AuthManagement.SendFrogetPasswordResetEmail
{
    public record ForgetPasswordViewModel(string email); 


     public class ForgetPasswordViewModelValidator : AbstractValidator<ForgetPasswordViewModel>
    {
        public ForgetPasswordViewModelValidator ()
        {
         
        }
    }

}