using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.Common.Users.Queries;

public record GetUserIDByEmailQuery(string Email) : IRequest<RequestResult<Guid>>;

public class GetUserIDByEmailQueryHandler : BaseRequestHandler<GetUserIDByEmailQuery, RequestResult<Guid>>
{
    private readonly IRepository<User> _repository;
    public GetUserIDByEmailQueryHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<Guid>> Handle(GetUserIDByEmailQuery request, CancellationToken cancellationToken)
    {
        var userID = await _repository.Get(U => U.Email == request.Email).Select(u => u.ID).FirstOrDefaultAsync();

        if(userID == Guid.Empty)
            return RequestResult<Guid>.Failure(ErrorCode.UserNotFound, "User does not exist");
        
        return RequestResult<Guid>.Success(userID);
    }
}