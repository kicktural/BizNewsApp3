using System.ComponentModel.DataAnnotations;

namespace BizNewsAppDB1.Models
{
    public class Article : BaseEntity
    {
        public int Id { get; set; }
        [MaxLength(15)]
        [MinLength(3)]
        public string Title { get; set; }
        [MinLength(5)]
        public string Content { get; set; } 
        public string PhotoUrl { get; set; }
        public int ViewCount { get; set; }
        public string SeoUrl { get; set; }
        public bool Status { get; set; }
        public bool IsFeatured { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<ArticleComment> ArticleComments { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
    }
}
