using BizNewsAppDB1.Models;

namespace BizNewsAppDB1.ViewsCategoryVM
{
    public class ArticleCategory
    {
        public Article Articles { get; set; }
        public Article PopularPost { get; set; }
        public Article RecentPost { get; set; }
    }
}
