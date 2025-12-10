using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtpNet;
using UserManagement.ApiService.Common.BaseEndpoints;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Features.AuthManagement.ActivateUser2FA.Commands;
using UserManagement.ApiService.Features.AuthManagement.LogInUser;
using UserManagement.ApiService.Filters;

namespace UserManagement.ApiService.Features.AuthManagement.ActivateUser2FA;


public class Activate2FAQRCodeEndpoint : BaseEndpoint<EmptyRequestViewModel, string>
{
    public Activate2FAQRCodeEndpoint(BaseEndpointParameters<EmptyRequestViewModel> parameters) : base(parameters)
    {
    }
    
    [HttpPost]
    [Authorize]
    [TypeFilter(typeof(CustomizeAuthorizeAttribute), Arguments =new object[] {Feature.ActivateUser2FA})]
    public async Task<EndpointResponse<string>> ActivateUser2FA()
    {
        var activateCommand = await _mediator.Send(new ActivateUser2FAOrchestrator());
        if (!activateCommand.isSuccess)
            return EndpointResponse<string>.Failure(activateCommand.errorCode, activateCommand.message);
        
        return EndpointResponse<string>.Success(activateCommand.data);
    }
}