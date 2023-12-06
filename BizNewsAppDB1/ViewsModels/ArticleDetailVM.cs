using BizNewsAppDB1.Models;

namespace BizNewsAppDB1.ViewsModels
{
    public class ArticleDetailVM
    {
        public Article Article { get; set; }    
        public List<Article> SimilarArticles { get; set; }
        public List<Article> SimilarArticleTake2 { get; set; }
        public List<Article> PopularPosts { get; set; }
        public List<ArticleComment>? ArticleComments { get; set; }
        public List<Article> PopularCategory { get; set; }
        public List<Article> RecentPost { get; set; }
        public List<ArticleComment> ArtCommentPublishdate { get; set; }
        public List<Article> ArticlesStatus { get; set; }
    }
}
