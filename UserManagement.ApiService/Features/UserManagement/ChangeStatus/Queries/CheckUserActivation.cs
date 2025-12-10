using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.Common.Pagination;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.UserManagement.ChangeStatus.Queries;

public record CheckUserActivation(int PageNumber , int PageSize) : IRequest<RequestResult<bool>>;

public class CheckUserActivationHandler : BaseRequestHandler<CheckUserActivation, RequestResult<bool>>
{
    private readonly IRepository<User> _repository;
    public CheckUserActivationHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override Task<RequestResult<bool>> Handle(CheckUserActivation request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}