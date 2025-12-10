using UserManagement.ApiService.Common.Data.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using UserManagement.ApiService.Features.Common.Users.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace UserManagement.ApiService.Filters
{
    public class CustomizeAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        
        Feature _feature;
        

        public CustomizeAuthorizeAttribute(Feature feature)
        {
            _feature = feature;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var claims = context.HttpContext.User;
             
            var userID = claims.FindFirst("ID");

            if (userID is null || string.IsNullOrEmpty(userID.Value))
            {
                throw new UnauthorizedAccessException();
            }

            var user = Guid.Parse(userID.Value);

            var _mediator = context.HttpContext.RequestServices.GetService<IMediator>();
            if (_mediator is null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }
            var hasAccess = await _mediator.Send(new HasAccessQuery(user, _feature));

            if (!hasAccess)
            {
                context.Result = new ForbidResult(new[] {JwtBearerDefaults.AuthenticationScheme});
                return;
            }
        }
    }
}
