using DotNetCore.CAP;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserManagement.ApiService.Common.BaseEndpoints;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Features.AuthManagement.RegisterUser;
using UserManagement.ApiService.Features.AuthManagement.RegisterUser.Commands;
using UserManagement.ApiService.Features.UserManagement.GetAllUsers;

public class UserDetailsEndpoint : BaseEndpoint<EmptyRequestViewModel, bool>
{
    private readonly ICapPublisher _capPublisher;
    public UserDetailsEndpoint(BaseEndpointParameters<EmptyRequestViewModel> parameters, ICapPublisher capPublisher) : base(parameters)
    {
        _capPublisher = capPublisher;
    }

    [HttpPut]
    public async Task<EndpointResponse<bool>> UserDetails(EmptyRequestViewModel viewmodel)
    {
        var validationResult =  ValidateRequest(viewmodel);
        if (!validationResult.isSuccess)
            return validationResult;
      
        await _capPublisher.PublishAsync("test.message", new { Message = "Hello CAP" });

      
        return EndpointResponse<bool>.Success(true);
    }
}
