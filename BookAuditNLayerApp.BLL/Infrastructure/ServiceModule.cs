using BookAuditNLayerApp.DAL.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookAuditNLayerApp.BLL.Infrastructure
{
    class ServiceModule 
       // : NinjectModule
    {
        private DbContextOptions<BookContext> options;
        public ServiceModule(DbContextOptions<BookContext> options)
        {
            this.options = options;
        }

        //public override void Load()
        //{
        //    Bind<IUnitOfWork>().To<EFUnitOfWork>().WithConstructorArgument(options);
        //}
    }
}
