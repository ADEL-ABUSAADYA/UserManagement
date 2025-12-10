using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.Common.Users.DTOs;
using UserManagement.ApiService.Features.AuthManagement.LogInUser;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.AuthManagement.ChangePassword.Queries;

public record GetPasswordByIDQuery () : IRequest<RequestResult<string>>;

public class GetPasswordByIDQueryHandler : BaseRequestHandler<GetPasswordByIDQuery, RequestResult<string>>
{
    private readonly IRepository<User> _repository;
    public GetPasswordByIDQueryHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
    {
        _repository = repository;
    }

    public override async Task<RequestResult<string>> Handle(GetPasswordByIDQuery request, CancellationToken cancellationToken)
    {
        var password = await _repository.Get(u => u.ID == _userInfo.ID).Select(u => u.Password).FirstOrDefaultAsync();
        
        if (string.IsNullOrWhiteSpace(password))
            return RequestResult<string>.Failure(ErrorCode.PasswordTokenNotMatch, "Password Token Not Match");

        return RequestResult<string>.Success(password);


    }
}

