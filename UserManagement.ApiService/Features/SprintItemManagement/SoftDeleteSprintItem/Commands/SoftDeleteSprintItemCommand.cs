// using MediatR;
// using UserManagement.ApiService.Common.BaseHandlers;
// using UserManagement.ApiService.Common.Views;
//
// namespace UserManagement.ApiService.Features.SprintItemManagement.SoftDeleteSprintItem.Commands;
//
// public record SoftDeleteSprintItemCommand(int ProjectID) : IRequest<RequestResult<bool>>;
//
// public class SoftDeleteSprintItemCommandHandler : BaseRequestHandler<SoftDeleteSprintItemCommand, RequestResult<bool>>
// {
//     public SoftDeleteSprintItemCommandHandler(BaseRequestHandlerParameters parameters) : base(parameters)
//     {
//     }
//
//     public override Task<RequestResult<bool>> Handle(SoftDeleteSprintItemCommand request, CancellationToken cancellationToken)
//     {
//         
//     }
// }