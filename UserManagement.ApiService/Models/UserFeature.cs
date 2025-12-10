using UserManagement.ApiService.Common.Data.Enums;

namespace UserManagement.ApiService.Models
{
    public class UserFeature : BaseModel
    { 
        public Guid UserID { get; set; }
        public User user { get; set; }
        public Feature Feature { get; set; }
    }
}
