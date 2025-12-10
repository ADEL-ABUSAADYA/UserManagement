using DotNetCore.CAP;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using UserManagement.ApiService.Common.BaseEndpoints;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Features.AuthManagement.RegisterUser;
using UserManagement.ApiService.Features.AuthManagement.ReSendRegistrationEmail.Queries;
using UserManagement.ApiService.src.Helpers;
using System.Text.Json;

namespace UserManagement.ApiService.Features.AuthManagement.ResendRegistrationEmail;

public class ReSendRegistrationEmailEndpoint : BaseEndpoint<ReSendRegistrationEmailRequestViewModel, bool>
{
   private readonly ICapPublisher _capPublisher;
   public ReSendRegistrationEmailEndpoint(BaseEndpointParameters<ReSendRegistrationEmailRequestViewModel> parameters, ICapPublisher capPublisher) : base(parameters)
   {
      _capPublisher = capPublisher;
   }

   [HttpPut]
   public async Task<EndpointResponse<bool>> ReSendEmail(ReSendRegistrationEmailRequestViewModel viewmodel)
   {
      var validationResult =  ValidateRequest(viewmodel);
      if (!validationResult.isSuccess)
         return validationResult;
      
      var registrationInfoQuery = new GetUserRegistrationInfoQuery(viewmodel.Email);
      
      var regisRequestInfo = await _mediator.Send(registrationInfoQuery);
      if (!regisRequestInfo.isSuccess)
         return EndpointResponse<bool>.Failure(regisRequestInfo.errorCode, regisRequestInfo.message);

        BackgroundJob.Enqueue(() => EmailHelper.SendEmail("adelabusaadya@icloud.com", "gfhgfh", null));
        //var message = new UserRegisteredEvent(regisRequestInfo.data.Email, regisRequestInfo.data.Name, $"{regisRequestInfo.data.ConfirmationToken}", DateTime.UtcNow);
        //var messageJson = JsonSerializer.Serialize(message);
        //await _capPublisher.PublishAsync("user.registered", messageJson);

        return EndpointResponse<bool>.Success(true);
   }
}
