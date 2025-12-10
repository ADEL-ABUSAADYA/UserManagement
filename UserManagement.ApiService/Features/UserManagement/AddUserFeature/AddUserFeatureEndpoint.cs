using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.ApiService.Common.BaseEndpoints;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Features.AuthManagement.LogInUser.Commands;
using UserManagement.ApiService.Filters;

namespace UserManagement.ApiService.Features.UserManagement.AddUserFeature;

public class AddUserFeatureEndpoint : BaseEndpoint<AddUserFeatureRequestViewModel, bool>
{
   public AddUserFeatureEndpoint(BaseEndpointParameters<AddUserFeatureRequestViewModel> parameters) : base(parameters)
   {
   }

   [HttpPost]
   [Authorize]
   [TypeFilter(typeof(CustomizeAuthorizeAttribute), Arguments =new object[] {Feature.AddUserFeature})]
   public async Task<EndpointResponse<bool>>  AddUserFeature(AddUserFeatureRequestViewModel viewmodel)
   {
      var validationResult =  ValidateRequest(viewmodel);
      if (!validationResult.isSuccess)
         return validationResult;
      
      var loginCommand = new LogInUserCommand(viewmodel.Email, viewmodel.Password);
      var logInToken = await _mediator.Send(loginCommand);
      if (!logInToken.isSuccess)
         return EndpointResponse<bool>.Failure(logInToken.errorCode, logInToken.message);
      
      return EndpointResponse<bool>.Success(true);
   }
}
