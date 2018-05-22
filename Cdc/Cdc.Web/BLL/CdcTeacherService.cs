using Cdc.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cdc.Web.BLL
{
    public class CdcTeacherService
    {
        private readonly CdcContext context = new CdcContext();

        public List<Lesson> GetSchedule(string email)
        {
            var user = context
                .AspNetUsers
                .Where(u => u.Email == email)
                .First();

            if (user == null || user.Teacher == null)
            {
                throw new ArgumentException("teacher not found");
            }
            Teacher teacher = user.Teacher;
            List<Lesson> schedule = new List<Lesson>();
            foreach (TeachingSubject t in teacher.TeachingSubjects)
            {
                foreach (Lesson l in context.Lessons)
                {
                    if (l.TeachingSubjectId == t.TeachingSubjectId)
                        schedule.Add(l);
                }
            }

            return schedule;
        }
    }
}