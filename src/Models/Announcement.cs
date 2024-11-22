using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pawsome.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<byte[]> Photos { get; set; } = new List<byte[]>();
       


    }
}
