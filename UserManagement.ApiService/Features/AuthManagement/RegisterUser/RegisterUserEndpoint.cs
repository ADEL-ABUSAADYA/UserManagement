using Microsoft.AspNetCore.Mvc;
using UserManagement.ApiService.Common.BaseEndpoints;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Features.AuthManagement.RegisterUser.Commands;

namespace UserManagement.ApiService.Features.AuthManagement.RegisterUser;

public class RegisterUserEndpoint : BaseEndpoint<RegisterUserRequestViewModel, bool>
{
   public RegisterUserEndpoint(BaseEndpointParameters<RegisterUserRequestViewModel> parameters) : base(parameters)
   {
   }

   [HttpPut]
   public async Task<EndpointResponse<bool>> RegisterUser(RegisterUserRequestViewModel viewmodel)
   {
      var validationResult =  ValidateRequest(viewmodel);
      if (!validationResult.isSuccess)
         return validationResult;
      
      var regisetrCommand = new RegisterUserCommand(viewmodel.Email, viewmodel.Password, viewmodel.Name, viewmodel.PhoneNo, viewmodel.Country);
      var isRegistered = await _mediator.Send(regisetrCommand);
      if (!isRegistered.isSuccess)
         return EndpointResponse<bool>.Failure(isRegistered.errorCode, isRegistered.message);
      
      return EndpointResponse<bool>.Success(true);
   }
}
