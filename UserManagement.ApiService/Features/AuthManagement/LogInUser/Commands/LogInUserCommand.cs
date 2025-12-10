using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.AuthManagement.LogInUser.Queries;
using UserManagement.ApiService.Features.Common.Users.DTOs;
using UserManagement.ApiService.Models;
using UserManagement.ApiService.src.Helpers;

namespace UserManagement.ApiService.Features.AuthManagement.LogInUser.Commands;

public record LogInUserCommand(string Email, string Password) : IRequest<RequestResult<TokenDTO>>;

public class LogInUserCommandHandler : BaseRequestHandler<LogInUserCommand, RequestResult<TokenDTO>>
{
    private readonly IRepository<User> _repository;
    public LogInUserCommandHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }
    
    public override async Task<RequestResult<TokenDTO>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        
        var userInfo = await _mediator.Send(new GetUserLogInInfoQuery(request.Email));
        if (!userInfo.isSuccess)
        {
            return RequestResult<TokenDTO>.Failure(userInfo.errorCode, userInfo.message);
        }
        var isPasswordMached = CheckPassword(request.Password, userInfo.data.hashedPassword);
        if (!isPasswordMached)
            return RequestResult<TokenDTO>.Failure(userInfo.errorCode, "password is incorrect.");
        if (!userInfo.data.IsEmailConfirmed)
        {
            return RequestResult<TokenDTO>.Failure(userInfo.errorCode, "email is not confirmed.");
        }
        if (userInfo.data.ID != Guid.Empty && !userInfo.data.Is2FAEnabled)
        {
            var token = _tokenHelper.GenerateToken(userInfo.data.ID);
            return RequestResult<TokenDTO>.Success(new TokenDTO(LogInToken: token, TokenWith2FA: ""));
        }
        if (userInfo.data.ID != Guid.Empty && userInfo.data.Is2FAEnabled)
        {
            var token2fa = _tokenHelper.Generate2FALoginToken(userInfo.data.ID);
            return RequestResult<TokenDTO>.Success(new TokenDTO(LogInToken: "", TokenWith2FA: token2fa));
        }
        return RequestResult<TokenDTO>.Failure(userInfo.errorCode, userInfo.message);
    }

    private bool CheckPassword(string requestPassword, string databasePassword)
    {
        PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, databasePassword, requestPassword);
        if (passwordVerificationResult== PasswordVerificationResult.Success)
        return true;
        
        return false;
    }
}