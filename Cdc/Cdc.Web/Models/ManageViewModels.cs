using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Cdc.Web.Models
{
    public class IndexViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public double Discount { get; set; }
        public double Balance { get; set; }
        public string Role { get; set; }
    }

    public class RegisterChildViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }

    public class RegisterTeacherViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }

    public class AddSubjectViewModel
    {
        [Required]
        [EmailAddress]
        public string TeacherEmail { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required]
        [Range(0, 6)]
        public int MinAge { get; set; }
        [Required]
        [Range(0,6)]
        public int MaxAge { get; set; }
    }

    public class AddLessonViewModel
    {
        [Required]
        public string SubjectName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }

    public class RecordChildForLessonViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string SubjectName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }

    public class SetDiscountViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public float Discount { get; set; }
    }

    public class StatisticsViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime From { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime To { get; set; }

        public decimal Income { get; set; }
    }

    public class PublishNewsViewModel
    {
        [Required]
        public string SubjectName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [MaxLength(200)]
        public string Content { get; set; }
    }
}