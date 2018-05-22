using Cdc.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cdc.Web.BLL
{
    public class CdcUserService
    {
        private readonly CdcContext context = new CdcContext();

        public List<News> GetNews() => context.News.ToList();

        public List<Lesson> GetShedule() => context.Lessons.ToList();

        public List<TeachingSubject> GetSubjects() => context.TeachingSubjects.ToList();

        public List<Teacher> GetTeachers() => context.Teachers.ToList();

        public List<Response> GetResponses() => context.Responses.ToList();

        public void AddResponse(AddResponseViewModel responseForm)
        {
            TeachingSubject subject =
                context
                .TeachingSubjects
                .Where(t => t.TeachingSubjectName == responseForm.SubjectName)
                .First();
            
            if(subject==null)
            {
                throw new ArgumentException("subject not found");
            }

            subject.Responses.Add(
                new Response()
                {
                    Author = responseForm.Author,
                    Content = responseForm.Content,
                    TeachingSubjectId = subject.TeachingSubjectId
                });

            context.SaveChanges();
        }
    }
}