using Money2Word.Models;
using Money2Word.Services;
using Money2Word.Services.Interfaces;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

namespace Money2Word.App_Start
{
    public class SimpleInjectorConfig
    {
        public static void RegisterComponents()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            RegisterTypes(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static void RegisterTypes(Container container)
        {
            container.Register<IMoney2WordConvertor, Money2WordConvertor>(Lifestyle.Transient);
            container.Register<INameValidator, NameValidator>(Lifestyle.Transient);         
            container.Register<IService,Service>(Lifestyle.Transient);
            container.Register<IResponseModel, ResponseModel>(Lifestyle.Transient);
            container.Register<IInputModel, InputModel>(Lifestyle.Transient);
        }
    }
}