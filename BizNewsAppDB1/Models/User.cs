using Microsoft.AspNetCore.Identity;

namespace BizNewsAppDB1.Models
{
    public class User : IdentityUser
    {       
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string AboutAuthor { get; set; }
        public string PhotoUrl { get; set; }   
        public List<ArticleComment> ArticleComments { get; set; }
    }
}
