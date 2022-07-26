using BookStore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BookStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public object Identity { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // ������� ����� ��������� �������� ��������������
        protected void Application_AuthenticateRequest()
        {
            // ��������� ����������� �� ������������
            if(User == null)
            {
                return;
            }

            // �������� ��� ������������
            string userName = Context.User.Identity.Name;

            // ��������� ������ �����
            string[] roles = null;

            using(Db db = new Db())
            {
                UserDTO userDTO = db.Users.FirstOrDefault(x => x.Username == userName);

                if(userDTO == null)
                {
                    return;
                }

                // ��������� ������ ������
                roles = db.UserRoles.Where(x => x.UserId == userDTO.Id).Select(x => x.Roles.Name).ToArray();
            }


            // ������� ������ ���������� IPrincipal
            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            // ��������� � �������������� ������� Context.User
            Context.User = newUserObj;
        }
    }
}
