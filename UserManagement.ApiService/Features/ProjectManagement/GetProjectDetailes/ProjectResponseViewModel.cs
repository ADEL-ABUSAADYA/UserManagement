using UserManagement.ApiService.Features.ProjectManagement.GetAllProject;

namespace UserManagement.ApiService.Features.ProjectManagement.GetProjectDetailes
{
    public class ProjectDetailsResponseViewModel
    {

        public string title { get; set; }

        public string description { get; set; }

        public int NumUSers { get; set; }

        public int NumTask { get; set; }

        public DateTime CreatedDate { get; set; }



    }
}