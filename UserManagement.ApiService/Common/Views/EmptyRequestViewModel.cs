using FluentValidation;

namespace UserManagement.ApiService.Common.Views;

public record EmptyRequestViewModel();

public class LogInUserRequestViewModelValidator : AbstractValidator<EmptyRequestViewModel>
{
    public LogInUserRequestViewModelValidator()
    {
        
    }
}