using MediatR;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.ProjectManagement.AddProject.Queries
{
    public record IsProjectExistQuery(string Title) : IRequest<RequestResult<bool>>;


    public class IsProjectExistQueryHandler : BaseRequestHandler<IsProjectExistQuery, RequestResult<bool>>
    {
        private readonly IRepository<Project> _repository;
        public IsProjectExistQueryHandler(BaseRequestHandlerParameters parameters, IRepository<Project> repository) : base(parameters)
        {
            _repository = repository;
        }

        public override async Task<RequestResult<bool>> Handle(IsProjectExistQuery request, CancellationToken cancellationToken)
        {
            var projectExist = await _repository.AnyAsync(p=> p.Title == request.Title);

            if (projectExist) return RequestResult<bool>.Failure(ErrorCode.ProjectAlreadyExists, "this project is already exist"); 

           
            return RequestResult<bool>.Success(projectExist);   

        }
    }
}
