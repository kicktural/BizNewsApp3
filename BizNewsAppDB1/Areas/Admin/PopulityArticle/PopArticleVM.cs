using BizNewsAppDB1.Models;

namespace BizNewsAppDB1.Areas.Admin.PopulityArticle
{
    public class PopArticleVM
    {
        public List<Article> Article { get; set; }
        public List<Article> PopArticle { get; set; }
        public List<User> user { get; set; }
        public List<Article> IsFeatured { get; set; }
        public List<Article> RecentProduct { get; set; }
    }
}
