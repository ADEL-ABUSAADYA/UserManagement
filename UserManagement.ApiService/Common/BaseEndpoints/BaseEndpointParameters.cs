using FluentValidation;
using MediatR;
using UserManagement.ApiService.Common.Views;

namespace UserManagement.ApiService.Common.BaseEndpoints;
public class BaseEndpointParameters<TRequest>
{
    readonly IMediator _mediator;
    readonly IValidator<TRequest> _validator;
    readonly UserInfo _userInfo;

    
    public IMediator Mediator => _mediator;
    public IValidator<TRequest> Validator => _validator;
    public UserInfo UserInfo => _userInfo;
   
    public BaseEndpointParameters(IMediator mediator, IValidator<TRequest> validator, UserInfoProvider userInfoProvider)
    {
        _mediator = mediator;
        _validator = validator;
        _userInfo = userInfoProvider.UserInfo;
        
    }
}

