using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StackTracer.Enums
{
    public enum Roles
    {
        Admin,
        [Display(Name = "Project Manager")]
        ProjectManager,
        Developer,
        Submitter,
        NewUser,
        Demo
    }
}
