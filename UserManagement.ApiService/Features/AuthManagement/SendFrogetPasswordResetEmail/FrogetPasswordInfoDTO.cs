namespace UserManagement.ApiService.Features.AuthManagement.SendFrogetPasswordResetEmail;

public record FrogetPasswordInfoDTO(Guid UserID, bool IsEmailConfirmed);