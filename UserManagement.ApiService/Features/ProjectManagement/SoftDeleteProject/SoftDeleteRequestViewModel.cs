using FluentValidation;
using UserManagement.ApiService.Features.ProjectManagement.AddProject;

namespace UserManagement.ApiService.Features.ProjectManagement.SoftDeleteProject
{
    public record SoftDeleteRequestViewModel(Guid ProjectID);
    public class RequestEndPointModelValidator : AbstractValidator<SoftDeleteRequestViewModel>
    {
        public RequestEndPointModelValidator()
        {

        }
    }
}