    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BookStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "ShowPages" }, new[] { "BookStore.Controllers" });

            //routes.MapRoute("Default", "", new { controller = "Pages", action = "ShowPages" }, new[] { "BookStore.Controllers" });

            routes.MapRoute("Home", "Home/{action}/{name}", new { controller = "Home", action = "Index", name = UrlParameter.Optional }, new[] { "BookStore.Controllers" });

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional }, new[] { "BookStore.Controllers" });

            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional }, new[] { "BookStore.Controllers" });



            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
