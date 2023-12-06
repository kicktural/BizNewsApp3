namespace BizNewsAppDB1.Models
{
    public class Tag : BaseEntity
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
    }
}
