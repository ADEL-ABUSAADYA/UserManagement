using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.AuthManagement.ChangePassword.Queries;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.AuthManagement.ChangePassword.Commands;
public record ChangePasswordCommand(string CurrentPassword,string newPassword) : IRequest<RequestResult<bool>>;
public class ChangePasswordCommandHandler : BaseRequestHandler<ChangePasswordCommand, RequestResult<bool>>
{
    private readonly IRepository<User> _repository;
    public ChangePasswordCommandHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }
    
    
    public async override Task<RequestResult<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var CurrentDatabasePassword = await _mediator.Send(new GetPasswordByIDQuery());

        if (!CurrentDatabasePassword.isSuccess)
            return RequestResult<bool>.Failure(ErrorCode.UserNotFound);

        PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
        var newPassword = passwordHasher.HashPassword(null, request.newPassword);
        

        var isOldPasswordCorrect = CheckPassword(request.CurrentPassword, CurrentDatabasePassword.data);
        
        if (!isOldPasswordCorrect)
            return RequestResult<bool>.Failure(ErrorCode.InvalidInput);

        
        var user = new User()
        {
            ID = _userInfo.ID,
            Password = request.newPassword,
        };
           
        await  _repository.SaveIncludeAsync(user, nameof(User.Password));
           
        await _repository.SaveChangesAsync(); 

        return RequestResult<bool>.Success(true);

    }

    private bool CheckPassword(string requestCurrentPassword, string databasePassword)
    {
        var passwordHasher = new PasswordHasher<string>();
        return passwordHasher.VerifyHashedPassword(null, databasePassword, requestCurrentPassword) != PasswordVerificationResult.Failed;
    }

}
