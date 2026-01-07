using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RitiksJournalApp.Models
{
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty; // Stores Markdown/HTML

        public DateTime Date { get; set; } = DateTime.Today; // One per day check happens in Service

        // Timestamps (System Generated)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Mood Tracking
        public string PrimaryMood { get; set; } = "Neutral";
        public string SecondaryMoods { get; set; } = string.Empty; // Store as "Excited,Grateful"

        // Tags
        public string Tags { get; set; } = string.Empty; // Store as "Work,Travel"
    }
}
