using BizNewsAppDB1.Models;

namespace BizNewsAppDB1.ViewModelVM
{
    public class RoleModelVM
    {
        public User User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
