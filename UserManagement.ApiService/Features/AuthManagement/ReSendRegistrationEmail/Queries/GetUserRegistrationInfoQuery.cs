using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.AuthManagement.ResendRegistrationEmail;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.AuthManagement.ReSendRegistrationEmail.Queries;

public record GetUserRegistrationInfoQuery(string email) : IRequest<RequestResult<RegistrationInfoDTO>>;

public class GetUserRegistrationInfoQueryHandler : BaseRequestHandler<GetUserRegistrationInfoQuery, RequestResult<RegistrationInfoDTO>>
{
    private readonly IRepository<User> _repository;
    public GetUserRegistrationInfoQueryHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }


    public override async Task<RequestResult<RegistrationInfoDTO>> Handle(GetUserRegistrationInfoQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.Get(u => u.Email == request.email).Select(u => new RegistrationInfoDTO
        {
            Name = u.Name,
            ConfirmationToken = u.ConfirmationToken,
            Email = u.Email,
            IsEmailConfirmed = u.IsEmailConfirmed
        }).FirstOrDefaultAsync();
        
        if (result == null)
            return RequestResult<RegistrationInfoDTO>.Failure(ErrorCode.UserNotFound, "please check your email address or register your email address.");
        
        if (result.IsEmailConfirmed)
            return RequestResult<RegistrationInfoDTO>.Failure(ErrorCode.UserAlreadyRegistered, "user already registered please login");
        
        return RequestResult<RegistrationInfoDTO>.Success(result);
    }
}