using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FullCoreProject.Models
{
    public static class ClaimStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Roles","Create Roles"),
            new Claim("Edit Roles","Edit Roles"),
            new Claim("Delete Roles","Delete Roles")
        };
    }
}
