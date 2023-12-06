using BizNewsAppDB1.Models;

namespace BizNewsAppDB1.ViewsModels
{
    public class HomeVM
    {
        public List<Article> FeaturedArticles { get; set; }
        public List<Article> LatestArticles { get; set; }
        public IEnumerable<Article> ActiveLatestArticles { get; set; }
        public List<Article> PopulaeArticle { get; set; }
        public List<Article> recentPost { get; set; }
    }
}
