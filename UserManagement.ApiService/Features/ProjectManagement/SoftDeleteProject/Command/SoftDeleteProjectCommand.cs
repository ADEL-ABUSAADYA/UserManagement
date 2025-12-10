using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Models;


namespace UserManagement.ApiService.Features.ProjectManagement.SoftDeleteProject.Command
{
    public record SoftDeleteProjectCommand(Guid ProjectID) : IRequest<RequestResult<bool>>;

    public class DeleteProjectCommandHandler : BaseRequestHandler<SoftDeleteProjectCommand, RequestResult<bool>>
    {
        private readonly IRepository<Project> _repository;
        public DeleteProjectCommandHandler(BaseRequestHandlerParameters parameters, IRepository<Project> repository) : base(parameters)
        {
            _repository = repository;
        }

        public override async Task<RequestResult<bool>> Handle(SoftDeleteProjectCommand request, CancellationToken cancellationToken)
        {
            // var project = await _repository.Get(c => c.ID == softDeleteRequest.ProjectId && !c.Deleted).FirstOrDefaultAsync(); 

          

            //foreach (var item in project.SprintItems)
            //{

            //    item.Deleted = true;

            //}

            
            // //await _repository.SaveIncludeAsync(project , nameof(project.SprintItems));
            // await _repository.Delete(project);
            // await _repository.SaveChangesAsync();
            return RequestResult<bool>.Success(true, "project deleted succssfully ");
        }
    }

}
