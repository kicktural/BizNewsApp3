using BizNewsAppDB1.AuthDTO;
using BizNewsAppDB1.Models;

namespace BizNewsAppDB1.Areas.Admin.ProfileViews
{
    public class ProfileView
    {
      
        public Article Article { get; set; }
        public List<User> User { get; set; }
    }
}
