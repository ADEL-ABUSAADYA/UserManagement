using System.Text.Json;
using DotNetCore.CAP;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.AuthManagement.RegisterUser.Queries;
using UserManagement.ApiService.Models;
using UserManagement.ApiService.src.Helpers;

namespace UserManagement.ApiService.Features.AuthManagement.RegisterUser.Commands;

public record RegisterUserCommand(string email, string password, string name, string phoneNo, string country) : IRequest<RequestResult<bool>>;

public class RegisterUserCommandHandler : BaseRequestHandler<RegisterUserCommand, RequestResult<bool>>
{
    private readonly IRepository<User> _repository;
    private readonly ICapPublisher _capPublisher;

    public RegisterUserCommandHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository,
        ICapPublisher capPublisher) : base(parameters)
    {
        _repository = repository;
        _capPublisher = capPublisher;
    }

    public async override Task<RequestResult<bool>> Handle(RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var reponse = await _mediator.Send(new IsUserExistQuery(request.email));
        if (reponse.isSuccess)
            return RequestResult<bool>.Failure(ErrorCode.UserAlreadyExist);

        PasswordHasher<string> passwordHasher = new PasswordHasher<string>();
        var password = passwordHasher.HashPassword(null, request.password);

        var user = new User
        {
            Email = request.email,
            Password = password,
            Name = request.name,
            PhoneNo = request.phoneNo,
            Country = request.country,
            RoleID = new Guid("f5beeceb-e61b-4cd9-bf6c-a41d5e7b6f4b"),
            IsActive = true,
            ConfirmationToken = Guid.NewGuid().ToString()
        };


        var userID = await _repository.AddAsync(user, _cancellationToken);
        await _repository.SaveChangesAsync(_cancellationToken);

        if (userID == Guid.Empty)
            return RequestResult<bool>.Failure(ErrorCode.UnKnownError);

        //var message = new UserRegisteredEvent(user.Email, user.Name, $"{user.ConfirmationToken}", DateTime.UtcNow);
        //var messageJson = JsonSerializer.Serialize(message);
        //await _capPublisher.PublishAsync("user.registered", messageJson);

        BackgroundJob.Enqueue(() => EmailHelper.SendEmail(user.Email, $"{user.ConfirmationToken}", user.Name));


        return RequestResult<bool>.Success(true);
    }
}