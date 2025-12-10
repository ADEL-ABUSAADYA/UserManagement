// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Infrastructure;
// using UserManagement.ApiService.Common;
// using UserManagement.ApiService.Common.BaseEndpoints;
// using UserManagement.ApiService.Common.Data.Enums;
// using UserManagement.ApiService.Common.Views;
// using UserManagement.ApiService.Features.UserManagement.GetUserDetalies;
//
//
//
// namespace UserManagement.ApiService.Features.UserManagement.GetUserDetails
// {
//   
//     public class UserDetaileEndpoint : BaseEndpoint<RequestUserDetailesEndpoint ,ResponseUserDetailsEndpoint >
//     {
//         public UserDetaileEndpoint(BaseEndpointParameters<RequestUserDetailesEndpoint> parameters) : base(parameters)
//         {
//         }
//
//         [HttpGet]
//         public async Task<EndpointResponse<ResponseUserDetailsEndpoint>> GetUSerDetails([FromQuery] RequestUserDetailesEndpoint request)
//         {
//             var validationResult = ValidateRequest(request);
//             if (!validationResult.isSuccess)
//                 return EndpointResponse<ResponseUserDetailsEndpoint>.Failure(ErrorCode.InvalidInput);
//             
//     
//
//
//
//        
//
//             return EndpointResponse<ResponseUserDetailsEndpoint>.Success(default);
//
//         }
//
//
//     }
// }
