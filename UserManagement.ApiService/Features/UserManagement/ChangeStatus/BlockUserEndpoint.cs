
using Microsoft.AspNetCore.Mvc;
using UserManagement.ApiService.Common.BaseEndpoints;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Features.UserManagement.ChangeStatus.Command;


namespace UserManagement.ApiService.Features.UserManagement.ChangeStatus
{
  
    public class BlockUserEndPoint : BaseEndpoint<RequestUserActivtaionStatus,bool >
    {
        public BlockUserEndPoint(BaseEndpointParameters<RequestUserActivtaionStatus> parameters) : base(parameters)
        {
        }

        [HttpPut]
        public async Task<EndpointResponse<bool>> GetUSerDetails([FromQuery] RequestUserActivtaionStatus request)
        {
            var validationResult = ValidateRequest(request);
            if (!validationResult.isSuccess)
                return EndpointResponse<bool>.Failure(ErrorCode.InvalidInput);
            
            var response = await _mediator.Send(new ChangeStatusCommand(request.id));

            if (!response.isSuccess)
                return EndpointResponse<bool>.Failure(response.errorCode , response.message);


            return EndpointResponse<bool>.Success(response.isSuccess, "the user has deactivate ");

        }


    }
}
