﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FastFood.Models
{
    public partial class User
    {
        public User()
        {
            Roles = new HashSet<Role>();
        }

        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}