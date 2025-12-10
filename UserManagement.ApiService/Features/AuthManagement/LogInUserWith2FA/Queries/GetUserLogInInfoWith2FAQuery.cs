using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.AuthManagement.LogInUserWith2FA;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.AuthManagement.LogInUserWith2FA.Queries;
public record GetUserLogInInfoWith2FAQuery(string email) : IRequest<RequestResult<LogInInfoWith2FADTO>>;

public class GetUserLogInInfoWith2FAQueryHandler : BaseRequestHandler<GetUserLogInInfoWith2FAQuery, RequestResult<LogInInfoWith2FADTO>>
{
    private readonly IRepository<User> _repository;
    public GetUserLogInInfoWith2FAQueryHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<LogInInfoWith2FADTO>> Handle(GetUserLogInInfoWith2FAQuery request, CancellationToken cancellationToken)
    {
        var userData  = await _repository.Get(u => u.Email == request.email)
            .Select(u => new LogInInfoWith2FADTO(u.ID, u.TwoFactorAuthEnabled, u.TwoFactorAuthsecretKey)).FirstOrDefaultAsync();
        
        if (userData.ID == Guid.Empty)
        {
            return RequestResult<LogInInfoWith2FADTO>.Failure(ErrorCode.UserNotFound, "please check your email address.");
        }
        return RequestResult<LogInInfoWith2FADTO>.Success(userData);
    }
}