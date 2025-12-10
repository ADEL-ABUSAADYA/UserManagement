using MediatR;
using Microsoft.AspNetCore.Identity;
using OtpNet;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.Common.Users.Queries;
using UserManagement.ApiService.Models;


public record UpdateUser2FASecretKeyCommand(string User2FASecretKey) : IRequest<RequestResult<bool>>;

public class UpdateUser2FASecretKeyCommandHandler : BaseRequestHandler<UpdateUser2FASecretKeyCommand, RequestResult<bool>>
{
    private readonly IRepository<User> _repository;
    public UpdateUser2FASecretKeyCommandHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<bool>> Handle(UpdateUser2FASecretKeyCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            ID = _userInfo.ID,
            TwoFactorAuthsecretKey = request.User2FASecretKey,
            TwoFactorAuthEnabled = true,
        };

        try
        {

            await _repository.SaveIncludeAsync(user, nameof(user.TwoFactorAuthsecretKey), nameof(user.TwoFactorAuthEnabled));
            await _repository.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return RequestResult<bool>.Failure(ErrorCode.UnKnownError, e.Message);
        }

        return RequestResult<bool>.Success(true);
    }
}