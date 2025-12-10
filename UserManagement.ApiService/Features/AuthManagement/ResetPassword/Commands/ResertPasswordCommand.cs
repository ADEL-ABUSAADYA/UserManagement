using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.AuthManagement.ResetPassword.Queries;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.AuthManagement.ResetPassword.Commands
{
    public record ResetPasswordCommand(string ResetToken, string email, string NewPassword) : IRequest<RequestResult<bool>>;


    public class ResetPasswordCommandHandeler : BaseRequestHandler<ResetPasswordCommand, RequestResult<bool>>
    {
        private readonly IRepository<User> _repository;
        public ResetPasswordCommandHandeler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
        {
            _repository = repository;
        }

        public override async Task<RequestResult<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
           var userExist = await  _mediator.Send(new GetUserIDIfPasswordTokenMatchQuery(request.email, request.ResetToken), cancellationToken);
           
           if (!userExist.isSuccess)
               return RequestResult<bool>.Failure(userExist.errorCode, userExist.message);
           
           var hasher = new PasswordHasher<string>();
           var Newpassword = hasher.HashPassword(null, request.NewPassword);
           
           var user = new User()
           {
                ID = userExist.data,
                Password = Newpassword,
           };
           
           await  _repository.SaveIncludeAsync(user, nameof(User.Password));
           
           await _repository.SaveChangesAsync(); 

           return RequestResult<bool>.Success(true);

         }
    }

}
