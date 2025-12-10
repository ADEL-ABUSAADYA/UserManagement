using FluentValidation;
using UserManagement.ApiService.Features.UserManagement.GetAllUsers;

namespace UserManagement.ApiService.Features.UserManagement.ChangeStatus
{
    public record RequestUserActivtaionStatus(Guid id);

    public class RequestUserActivtaionStatusValidator : AbstractValidator<RequestUserActivtaionStatus>
    {
        public RequestUserActivtaionStatusValidator()
        {
          
        }
    }


}