namespace VDiary.Models
{
    public class SubjectUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int BelongsTo { get; set; }
    }
}