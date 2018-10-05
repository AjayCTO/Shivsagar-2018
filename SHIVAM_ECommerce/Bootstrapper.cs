using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using SHIVAM_ECommerce.Repository;
using SHIVAM_ECommerce.Models;
using SHIVAM_ECommerce.Controllers;
using Microsoft.AspNet.Identity.EntityFramework;
namespace SHIVAM_ECommerce
{
  public static class Bootstrapper
  {
    public static IUnityContainer Initialise()
    {
      var container = BuildUnityContainer();

      DependencyResolver.SetResolver(new UnityDependencyResolver(container));

      return container;
    }

    private static IUnityContainer BuildUnityContainer()
    {
      var container = new UnityContainer();

      // register all your components with the container here
      // it is NOT necessary to register your controllers

      // e.g. container.RegisterType<ITestService, TestService>();   
      container.RegisterType<IRepository<Category>, Repository<Category>>();
      container.RegisterType<IRepository<AllProductImages>, Repository<AllProductImages>>();
      container.RegisterType<IRepository<Product>, Repository<Product>>();
      container.RegisterType<IRepository<ProductAttributes>, Repository<ProductAttributes>>();
      container.RegisterType<IRepository<UnitOfMeasures>, Repository<UnitOfMeasures>>();
      container.RegisterType<IRepository<ProductImages>, Repository<ProductImages>>();
      container.RegisterType<IRepository<ProductAttributesRelation>, Repository<ProductAttributesRelation>>();
      container.RegisterType<IRepository<ApplicationUser>, Repository<ApplicationUser>>();
      container.RegisterType<IRepository<Claims>, Repository<Claims>>();
      container.RegisterType<IRepository<IdentityUserClaim>, Repository<IdentityUserClaim>>();
      container.RegisterType<IRepository<ProductAttributeWithQuantity>, Repository<ProductAttributeWithQuantity>>();
      container.RegisterType<IRepository<Customer>, Repository<Customer>>();
      container.RegisterType<AccountController>(new InjectionConstructor());
      RegisterTypes(container);

      return container;
    }

    public static void RegisterTypes(IUnityContainer container)
    {
    
    }
  }
}