using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.UserManagement.ChangeStatus.Command
{
    public record ChangeStatusCommand(Guid id) : IRequest<RequestResult<bool>>;

    public class BlockUserCommandHandler : BaseRequestHandler<ChangeStatusCommand, RequestResult<bool>>
    {
        private readonly IRepository<User> _repository;
        public BlockUserCommandHandler(BaseRequestHandlerParameters parameters, IRepository<User> repository) : base(parameters)
        {
            _repository = repository;
        }

        public override async Task<RequestResult<bool>> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
        {
            var checkActivtion = await  _repository 
               .Get(c => c.ID == request.id)
               .Select(c=> new  { ID = c.ID ,  IsActive =c.IsActive })
               .FirstOrDefaultAsync();

            if (checkActivtion == null) return RequestResult<bool>.Failure(ErrorCode.NoUsersFound, "this user not found");

         

            var changeStatus = !checkActivtion.IsActive;   
                 

            var user = new User { ID = checkActivtion.ID  , IsActive = changeStatus };


          await  _repository.SaveIncludeAsync(user , nameof(user.IsActive));

          await  _repository.SaveChangesAsync();

         return RequestResult<bool>.Success(true);  
        }
    }



}
