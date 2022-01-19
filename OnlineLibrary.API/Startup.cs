using OnlineLibrary.Common.Entities;
using OnlineLibrary.BLL.Interfaces;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.DAL.EF;
using OnlineLibrary.DAL.Interfaces;
using OnlineLibrary.DAL.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using FluentValidation.AspNetCore;
using FluentValidation;
using OnlineLibrary.Common.Validators;

namespace OnlineLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation()
                .AddNewtonsoftJson(OptionsBuilderConfigurationExtensions =>
            {
                OptionsBuilderConfigurationExtensions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddDbContext<BookContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BookContext")));
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IAuthorService, AuthorService>();
            services.AddTransient<IDataExportService, DataExportService>();
            services.AddTransient<IUnitOfWork, EFUnitOfWork>(setviceProvider =>
            {
                var context = setviceProvider.GetRequiredService<BookContext>();
                return new EFUnitOfWork(context);
            });

            // Validators.
            services.AddTransient<IValidator<Book>, BookValidator>();
            services.AddTransient<IValidator<Author>, AuthorValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
