﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcMovie
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<MvcMovie.Models.CreateOrEditMovieVM, MvcMovie.Models.Movie>();
                    cfg.CreateMap<MvcMovie.Models.Movie, MvcMovie.Models.CreateOrEditMovieVM>();
                }
            );



        }
    }
}
