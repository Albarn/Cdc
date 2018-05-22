using Cdc.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cdc.Web.BLL
{
    public class CdcSystemService
    {
        private readonly CdcContext context = new CdcContext();

        public Child GetChild(string email)
        {
            var user = context.AspNetUsers.Where(u => u.Email == email).First();
            if (user == null || user.Child==null)
            {
                throw new ArgumentException("child not found");
            }

            return user.Child;
        }

        public Teacher GetTeacher(string email)
        {
            var user = context.AspNetUsers.Where(u => u.Email == email).First();
            if (user == null || user.Teacher == null)
            {
                throw new ArgumentException("teacher not found");
            }

            return user.Teacher;
        }
    }
}