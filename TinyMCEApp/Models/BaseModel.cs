namespace TinyMCEApp.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
