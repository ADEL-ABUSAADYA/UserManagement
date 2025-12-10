using FluentValidation;
using UserManagement.ApiService.Features.ProjectManagement.GetAllProject;

namespace UserManagement.ApiService.Features.ProjectManagement.GetProjectDetailes
{
    public record ProjectDetailsRequest(int id);

    public class ProjectDetailsRequestValidator : AbstractValidator<ProjectDetailsRequest>
    {
        public ProjectDetailsRequestValidator()
        {

        }
    }


}