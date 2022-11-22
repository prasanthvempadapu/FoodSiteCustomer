using System.ComponentModel.DataAnnotations;

namespace Practice_CoreApp_04_08.Models
{
    public class Login
    {
        [Required(ErrorMessage ="Username is required")]
        [Display(Name ="Username")]
        public string UserName { get; set; }
        [Display(Name ="Password")]
        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; }
        public Login()
        {

        }
        public Login(string username,string password)
        {
            UserName = username;
            Password = password;
        }
    }
}
