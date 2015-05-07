namespace SignMeUp2.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using SignMeUp2.DataModel;
    using SignMeUp2.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SignMeUp2.DataModel.SignMeUpDataModel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "SignMeUp2.DataModel.SignMeUpDataModel";
        }

        protected override void Seed(SignMeUp2.DataModel.SignMeUpDataModel context)
        {
            var org = new Organisation
            {
                Namn = "SignMeUp",
                Epost = "martin.d.andersson@gmail.com",
                Adress = "Kätterud"
            };

            context.Organisationer.Add(org);
            context.SaveChanges();
            context.Entry(org).GetDatabaseValues();

            string userId = AddUserAndRole(context, org.ID);

            if (!string.IsNullOrEmpty(userId))
            {
                org.AnvändareId = userId;
                context.SaveChanges();
            }
        }

        private string AddUserAndRole(SignMeUp2.DataModel.SignMeUpDataModel context, int orgId)
        {
            IdentityResult ir;
            var rm = new RoleManager<IdentityRole>
                (new RoleStore<IdentityRole>(context));
            ir = rm.Create(new IdentityRole("admin"));
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
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
    }
}
