using Microsoft.AspNetCore.Http;
using StackTracer.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Project Description")]
        public string Description { get; set; }

        [Display(Name = "Project Manager")]
        public string ProjectManagerId { get; set; }

        public bool IsArchived { get; set; }

        [Display(Name = "Project Image")]
        [NotMapped]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize((1024 * 1024 * 5))]
        [MinFileSize(1024)]
        public IFormFile FormFile { get; set; }

        public string ImagePath { get; set; }

        public byte[] ImageData { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        public virtual ICollection<AppUser> Users { get; set; } = new HashSet<AppUser>();
    }
}
