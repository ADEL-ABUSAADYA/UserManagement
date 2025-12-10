using MediatR;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums; 
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Features.Common.Users.Queries;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.UserManagement.AddUserFeature.Commands;

public record AddUserFeatureCommand(string Email, Feature feature) : IRequest<RequestResult<bool>>;

public class AddUserFeatureCommandHandler : BaseRequestHandler<AddUserFeatureCommand, RequestResult<bool>>
{
    private readonly IRepository<UserFeature> _repository;
    public AddUserFeatureCommandHandler(BaseRequestHandlerParameters parameters, IRepository<UserFeature> repository) :
        base(parameters)
    {
        _repository = repository;
    }

    public async override Task<RequestResult<bool>> Handle(AddUserFeatureCommand request, CancellationToken cancellationToken)
    {
        
        var userID = await _mediator.Send(new GetUserIDByEmailQuery(request.Email));
        if (!userID.isSuccess)
            return RequestResult<bool>.Failure(userID.errorCode, userID.message);

        var hasAccess = await _mediator.Send(new HasAccessQuery(userID.data, request.feature));
        if (hasAccess)
            return RequestResult<bool>.Success(true);
        
        await _repository.AddAsync(new UserFeature
            {
                Feature = request.feature,
                UserID = userID.data,
            });
        await _repository.SaveChangesAsync();

        return RequestResult<bool>.Success(true);
    }
}
