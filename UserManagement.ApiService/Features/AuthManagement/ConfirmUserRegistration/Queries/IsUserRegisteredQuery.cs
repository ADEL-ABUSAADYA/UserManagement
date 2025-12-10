using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.AuthManagement.ConfirmUserRegistration.Queries;

public record IsUserRegisteredQuery(string email, string token) : IRequest<RequestResult<Guid>>;

public class IsUserRegisteredQueryHandler : BaseRequestHandler<IsUserRegisteredQuery, RequestResult<Guid>>
{
    private readonly IRepository<User> _repository;
    public IsUserRegisteredQueryHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<Guid>> Handle(IsUserRegisteredQuery request, CancellationToken cancellationToken)
    {
        var result= await _repository.Get(u => u.Email == request.email && u.ConfirmationToken == request.token).Select(u => u.ID).FirstOrDefaultAsync();
        if (result == Guid.Empty)
        {
            return RequestResult<Guid>.Failure(ErrorCode.UserNotFound);
        }
        return RequestResult<Guid>.Success(result);
    }
}