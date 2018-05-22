using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Cdc.Web.Models;

namespace Cdc.Web.BLL
{
    public class CdcManagerService
    {
        private readonly CdcContext context = new CdcContext();

        public void RegisterChild(RegisterChildViewModel childForm)
        {
            Child child = new Child()
            {
                BirthDate = childForm.BirthDate,
                Discount = 0,
                FirstName = childForm.FirstName,
                LastName = childForm.LastName
            };

            var user=context
                .AspNetUsers
                .Where(u => u.Email == childForm.Email)
                .First();

            if (user == null)
            {
                throw new ArgumentException("this email is not registred");
            }

            child.ChildId = user.Id;
            context.Children.Add(child);
            AspNetRole role = 
                context.AspNetRoles.Where(r => r.Name == "parent").First();
            user.AspNetRoles.Add(role);
            context.SaveChanges();
        }

        public void RegisterTeacher(RegisterTeacherViewModel teacherForm)
        {
            Teacher teacher = new Teacher()
            {
                FirstName = teacherForm.FirstName,
                LastName = teacherForm.LastName
            };

            var user = context
                .AspNetUsers
                .Where(u => u.Email == teacherForm.Email)
                .First();

            if (user == null)
            {
                throw new ArgumentException("this email is not registred");
            }

            teacher.TeacherId = user.Id;
            context.Teachers.Add(teacher);
            AspNetRole role =
                context.AspNetRoles.Where(r => r.Name == "teacher").First();
            user.AspNetRoles.Add(role);
            context.SaveChanges();
        }

        public void AddSubject(AddSubjectViewModel subjectForm)
        {
            var user = context
                .AspNetUsers
                .Where(u => u.Email == subjectForm.TeacherEmail)
                .First();

            if (user == null||user.Teacher==null)
            {
                throw new ArgumentException("teacher not found");
            }

            Teacher teacher = user.Teacher;
            TeachingSubject subject = new TeachingSubject()
            {
                TeacherId = teacher.TeacherId,
                TeachingSubjectName = subjectForm.Name,
                Price = subjectForm.Price,
                MinAge = subjectForm.MinAge,
                MaxAge = subjectForm.MaxAge
            };

            context.TeachingSubjects.Add(subject);
            context.SaveChanges();
        }

        public void AddLesson(AddLessonViewModel lessonForm)
        {
            TeachingSubject subject =
                context
                .TeachingSubjects
                .Where(l => l.TeachingSubjectName == lessonForm.SubjectName)
                .First();
            if(subject==null)
            {
                throw new ArgumentException("subject not found");
            }

            Lesson lesson = new Lesson()
            {
                TeachingSubjectId = subject.TeachingSubjectId,
                LessonTime = lessonForm.Date
            };
            context.Lessons.Add(lesson);
            context.SaveChanges();
        }

        public void RecordChildForLesson(RecordChildForLessonViewModel form)
        {
            var user = context
                .AspNetUsers
                .Where(u => u.Email == form.Email)
                .First();
            if (user == null || user.Child == null)
            {
                throw new ArgumentException("child not found");
            }

            Child child = context.Children.Find(user.Child.ChildId);
            Lesson lesson = context
                .Lessons
                .Where(l =>
                l.TeachingSubject.TeachingSubjectName == form.SubjectName &&
                l.LessonTime == form.Date)
                .First();

            if (lesson == null)
            {
                throw new ArgumentException("lesson not found");
            }

            lesson.Children.Add(child);
            
            context.SaveChanges();
        }

        public void SetDiscount(string email, float discount)
        {
            var user = context
                .AspNetUsers
                .Where(u => u.Email == email)
                .First();

            if (user == null || user.Child == null)
            {
                throw new ArgumentException("child not found");
            }

            Child child = user.Child;
            child.Discount = discount;
            context.Entry(child).State = EntityState.Modified;
            context.SaveChanges();
        }

        public decimal GetIncome(DateTime from, DateTime to)
        {
            decimal? income=context.GetCdcIncome(from, to).First();
            if (income == null) return 0;
            else return income.Value;
        }

        public void PublishNews(string subjectName,string content)
        {
            TeachingSubject subject =
                context
                .TeachingSubjects
                .Where(t => t.TeachingSubjectName == subjectName)
                .First();

            if(subject==null)
            {
                throw new ArgumentException("subject not found");
            }

            News news = new News()
            {
                TeachingSubjectId = subject.TeachingSubjectId,
                NewsTime = DateTime.Now,
                Content = content
            };
            context.News.Add(news);
            context.SaveChanges();
        }
    }
}