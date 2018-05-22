using Cdc.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace Cdc.Web.BLL
{
    public class CdcParentService
    {
        private readonly CdcContext context = new CdcContext();

        public decimal GetBalance(string email)
        {
            var user = context
                .AspNetUsers
                .Where(u => u.Email == email)
                .First();

            if (user == null || user.Child == null)
            {
                throw new ArgumentException("child not found");
            }

            decimal? balance = 
                context.GetChildBalance(user.Child.ChildId).First();
            if (balance == null)
                return 0;
            else return balance.Value;
        }

        public List<Lesson> GetSchedule(string email)
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
            List<Lesson> schedule = new List<Lesson>();
            foreach(Lesson l in context.Lessons)
            {
                if (l.Children.Contains(child))
                    schedule.Add(l);
            }

            return schedule;
        }

        public void MakePayment(string email, decimal sum)
        {
            var user = context
                .AspNetUsers
                .Where(u => u.Email == email)
                .First();

            if (user == null || user.Child == null)
            {
                throw new ArgumentException("child not found");
            }

            user.Child.Payments.Add(
                new Payment()
                {
                    ChildId = user.Id,
                    PaymentSum = sum,
                    PaymentTime = DateTime.Now
                });
            context.SaveChanges();
        }
    }
}