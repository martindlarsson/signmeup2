namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using SignMeUp2.Data;
    using SignMeUp2.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SignMeUp2.Data.SignMeUpDataModel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "SignMeUp2.Data.SignMeUpDataModel";
        }

        protected override void Seed(SignMeUp2.Data.SignMeUpDataModel context)
        {
            var adminOrg = context.Organisationer.SingleOrDefault(o => o.Namn == "SignMeUp");

            // Då har vi redan kört seed tidigare
            if (adminOrg != null)
            {
                return;
            }

            var org = new Organisation
            {
                Namn = "SignMeUp",
                Epost = "martin.d.andersson@gmail.com",
                Adress = "Kätterud"
            };

            context.Organisationer.AddOrUpdate(org);
            context.SaveChanges();
            context.Entry(org).GetDatabaseValues();
                
            string userId = AddUserAndRole(context, org.Id);

            if (!string.IsNullOrEmpty(userId))
            {
                org.AnvändareId = userId;
                context.SaveChanges();
            }
        }

        private string AddUserAndRole(SignMeUp2.Data.SignMeUpDataModel context, int orgId)
        {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var admin = um.Users.FirstOrDefault(u => u.UserName == "martin.d.andersson@gmail.com");

            if (admin == null)
            {
                IdentityResult ir;
                var rm = new RoleManager<IdentityRole>
                    (new RoleStore<IdentityRole>(context));
                ir = rm.Create(new IdentityRole("admin"));

                var user = new ApplicationUser()
                {
                    UserName = "martin.d.andersson@gmail.com",
                    OrganisationsId = orgId,
                    Email = "martin.d.andersson@gmail.com",
                    EmailConfirmed = true
                };
                ir = um.Create(user, "a0oa9ia8uAA");
                if (ir.Succeeded == false)
                    return null;
                ir = um.AddToRole(user.Id, "admin");
                return user.Id;
            }

            return null;
        }
    }
}
