using FluentValidation;
using UserManagement.ApiService.Features.AuthManagement.RegisterUser;
using UserManagement.ApiService.Features.AuthManagement.RegisterUser.Commands;

namespace UserManagement.ApiService.Features.ProjectManagement.AddProject
{
    public class RequestAddProjectModel
    {
        public string Title { get;  set; }
        public string Descrbition { get;  set; }
        
        public DateTime EndDate { get;  set; }
    }

    public class RequestAddProjectModelValidator : AbstractValidator<RequestAddProjectModel>
    {
        public RequestAddProjectModelValidator()
        {
  
        }
    }
}