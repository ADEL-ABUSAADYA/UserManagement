using Hangfire;
using MailKit.Net.Smtp;
using MediatR;
using MimeKit;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.AuthManagement.ReSendRegistrationEmail.Queries;
using UserManagement.ApiService.Features.AuthManagement.SendFrogetPasswordResetEmail.Queries;
using UserManagement.ApiService.Models;
using UserManagement.ApiService.src.Helpers;


namespace UserManagement.ApiService.Features.AuthManagement.SendFrogetPasswordResetEmail.Commands;

public record SendForgetPasswordResetEmailCommand(string email) : IRequest<RequestResult<bool>>;

public class SendForgetPasswordResetEmailCommandHandler : BaseRequestHandler<SendForgetPasswordResetEmailCommand, RequestResult<bool>>
{
    private readonly IRepository<User> _repository;
    public SendForgetPasswordResetEmailCommandHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<bool>> Handle(SendForgetPasswordResetEmailCommand request,
        CancellationToken cancellationToken)
    {
        var passwordResetData = await _mediator.Send(new GetForgetPasswordInfoQuery(request.email));

        if (!passwordResetData.isSuccess)
            return RequestResult<bool>.Failure(passwordResetData.errorCode, passwordResetData.message);
        
        var passwordResetCode =Guid.NewGuid().ToString().Substring(0, 6);

        var user = new User
        {
            ID = passwordResetData.data.UserID,
            ResetPasswordToken = passwordResetCode
        };

        await _repository.SaveIncludeAsync(user, nameof(User.ResetPasswordToken)); 
             
        await _repository.SaveChangesAsync();

      

        BackgroundJob.Enqueue(() => EmailHelper.SendEmail(user.Email, user.ResetPasswordToken, null));
        return RequestResult<bool>.Success(true);
    }
    
   }

