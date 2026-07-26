using System.Collections.Generic;

namespace AfroEvent.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public ICollection<EventEntity> Events { get; set; } = new List<EventEntity>();
    }
}
