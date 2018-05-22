using System;
using System.ComponentModel.DataAnnotations;

namespace Cdc.Web.Models
{
    public class AddResponseViewModel
    {
        [Required]
        public string Author { get; set; }
        [Required]
        public string SubjectName { get; set; }
        [Required]
        public string Content { get; set; }
    }
}