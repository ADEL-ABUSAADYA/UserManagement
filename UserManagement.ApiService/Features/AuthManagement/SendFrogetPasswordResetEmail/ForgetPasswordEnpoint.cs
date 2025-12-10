using Hangfire;
using Microsoft.AspNetCore.Mvc;
using UserManagement.ApiService.Common.BaseEndpoints;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Features.AuthManagement.SendFrogetPasswordResetEmail.Commands;
using UserManagement.ApiService.src.Helpers;

namespace UserManagement.ApiService.Features.AuthManagement.SendFrogetPasswordResetEmail
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendFrogetPasswordResstEmailEnpoint : BaseEndpoint<ForgetPasswordViewModel, bool>
    {
        public SendFrogetPasswordResstEmailEnpoint(BaseEndpointParameters<ForgetPasswordViewModel> parameters) : base(parameters)
        {
        }

        [HttpGet]
        public async Task<EndpointResponse<bool>> SendFrogetPasswordResstEmail(ForgetPasswordViewModel model)
        {
            var validationResult = ValidateRequest(model);
            if (!validationResult.isSuccess)
                return EndpointResponse<bool>.Failure(ErrorCode.InvalidInput);
            
            var response = await _mediator.Send(new SendForgetPasswordResetEmailCommand(model.email));

            if(!response.isSuccess)
                return EndpointResponse<bool>.Failure(response.errorCode , response.message);

            return EndpointResponse<bool>.Success(true);

        }


    }
}
