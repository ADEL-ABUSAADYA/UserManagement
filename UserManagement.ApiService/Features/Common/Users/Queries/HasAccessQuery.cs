using MediatR;
using UserManagement.ApiService.Common.BaseHandlers;
using UserManagement.ApiService.Common.Data.Enums;
using UserManagement.ApiService.Data.Repositories;
using UserManagement.ApiService.Models;

namespace UserManagement.ApiService.Features.Common.Users.Queries
{
    public record HasAccessQuery(Guid ID, Feature Feature) : IRequest<bool>;

    public class HasAccessQueryHandler : BaseRequestHandler<HasAccessQuery, bool>
    {
        private readonly IRepository<UserFeature> _repository;
        public HasAccessQueryHandler(BaseRequestHandlerParameters parameters, IRepository<UserFeature> repository) : base(parameters)
        {
            _repository = repository;
        }
        public override async Task<bool> Handle(HasAccessQuery request, CancellationToken cancellationToken)
        {
            var hasFeature = await _repository.AnyAsync(
                uf => uf.UserID == request.ID && uf.Feature == request.Feature);
            return hasFeature;
        }
    }
}