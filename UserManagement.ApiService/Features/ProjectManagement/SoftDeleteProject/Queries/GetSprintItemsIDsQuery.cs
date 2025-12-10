using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.ProjectManagement.SoftDeleteProject.Queries;

public record GetSprintItemsIDsQuery(Guid ProjectID) : IRequest<RequestResult<List<Guid>>>;

public class GetSprintItemsIDsQueryHandler : BaseRequestHandler<GetSprintItemsIDsQuery, RequestResult<List<Guid>>>
{
    private readonly IRepository<SprintItem> _repository;
    public GetSprintItemsIDsQueryHandler(BaseRequestHandlerParameters parameters, IRepository<SprintItem> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<List<Guid>>> Handle(GetSprintItemsIDsQuery request, CancellationToken cancellationToken)
    {
        var sprintItemsIDs = await _repository.Get(si => si.ProjectID == request.ProjectID && si.Deleted == false).Select(si => si.ID).ToListAsync();
        if (sprintItemsIDs.Count <= 0)
            return RequestResult<List<Guid>>.Failure(ErrorCode.NoSprintItems, "No sprint items found");
        
        return RequestResult<List<Guid>>.Success(sprintItemsIDs);
    }
}