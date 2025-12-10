using FluentValidation;
using MediatR;
using UserManagement.ApiService.Common.Views;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Helpers;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Common.BaseHandlers
{
    public class BaseRequestHandlerParameters
    {
        private readonly IMediator _mediator;
        private readonly TokenHelper _tokenHelper;
        private readonly UserInfo _userInfo;
        private readonly CancellationToken _cancellationToken;

        public IMediator Mediator => _mediator;

        public TokenHelper TokenHelper => _tokenHelper;
        public UserInfo UserInfo => _userInfo;
        public CancellationToken CancellationToken => _cancellationToken;
        

        // Constructor accepts the generic repository type for flexibility
        public BaseRequestHandlerParameters(IMediator mediator, UserInfoProvider userInfoProvider, TokenHelper tokenHelper, CancellationTokenProvider cancellationTokenProvider)
        {
            _mediator = mediator;
            _userInfo = userInfoProvider.UserInfo;
            _tokenHelper = tokenHelper;
            _cancellationToken = cancellationTokenProvider.CancellationToken;
            
        }
    }
}
