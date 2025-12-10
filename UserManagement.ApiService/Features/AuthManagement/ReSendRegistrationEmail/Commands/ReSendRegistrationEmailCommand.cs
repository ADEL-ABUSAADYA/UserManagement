using Hangfire;
using MailKit.Net.Smtp;
using MediatR;
using MimeKit;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.AuthManagement.ReSendRegistrationEmail.Queries;
using UserManagement.ApiService.Models;
using UserManagement.ApiService.src.Helpers;


namespace UserManagement.ApiService.Features.AuthManagement.ReSendRegistrationEmail.Commands;

public record ReSendRegistrationEmailCommand(string Email) : IRequest<RequestResult<bool>>;

public class ReSendRegistrationEmailCommandHandler : BaseRequestHandler<ReSendRegistrationEmailCommand, RequestResult<bool>>
{
    private readonly IRepository<User> _repository;
    public ReSendRegistrationEmailCommandHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<bool>> Handle(ReSendRegistrationEmailCommand request,
        CancellationToken cancellationToken)
    {
        var RegisterationData = await _mediator.Send(new GetUserRegistrationInfoQuery(request.Email));

        if (!RegisterationData.isSuccess)
            return RequestResult<bool>.Failure(RegisterationData.errorCode, RegisterationData.message);

        if (RegisterationData.data.IsEmailConfirmed)
            return RequestResult<bool>.Failure(ErrorCode.UserAlreadyRegistered, "user already registered please login");

        var confirmationLink = $"{RegisterationData.data.ConfirmationToken}";

        BackgroundJob.Schedule(() => EmailHelper.SendEmail(request.Email, confirmationLink, RegisterationData.data.Name), TimeSpan.FromSeconds(5));

        return RequestResult<bool>.Success(true);
    }
}

