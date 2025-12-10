using MediatR;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.Common.Users.DTOs;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.AuthManagement.RegisterUser.Queries;

public record IsUserExistQuery (string email) : IRequest<RequestResult<bool>>;


public class IsUserExistQueryHandler : BaseRequestHandler<IsUserExistQuery, RequestResult<bool>>
{
    private readonly IRepository<User> _repository;
    public IsUserExistQueryHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<bool>> Handle(IsUserExistQuery request, CancellationToken cancellationToken)
    {
        var result= await _repository.AnyAsync(u => u.Email == request.email);
        if (!result)
        {
            return RequestResult<bool>.Failure(ErrorCode.UserNotFound);
        }

        return RequestResult<bool>.Success(result);
    }
}