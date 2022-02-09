using OnlineLibrary.Common.DBEntities;
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
using OnlineLibrary.API.Model;
using OnlineLibrary.API.Validator;
using AutoMapper;
using OnlineLibrary.API.Mapper;
using OnlineLibrary.DAL.Repositories.Dapper;

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
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
                
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            //services.AddAutoMapper(typeof(MappingProfile));

            services.AddControllers()
                .AddFluentValidation()
                .AddNewtonsoftJson(OptionsBuilderConfigurationExtensions =>
            {
                OptionsBuilderConfigurationExtensions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddDbContext<BookContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BookContext"), b => b.MigrationsAssembly("OnlineLibrary.API")));
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IAuthorService, AuthorService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IDataExportService, DataExportService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IReservationService, ReservationService>();

            services.AddTransient<IUnitOfWork, DapperUnitOfWork>(serviceProvider =>
            {
                return new DapperUnitOfWork(Configuration.GetConnectionString("BookContext"));
            });

            // Validators.
            services.AddTransient<IValidator<CreateBook>, CreateBookValidator>();
            services.AddTransient<IValidator<ReservationModel>, CreateReservationValidator>();
            services.AddTransient<IValidator<Book>, BookValidator>();
            services.AddTransient<IValidator<Author>, AuthorValidator>();
            services.AddTransient<IValidator<Tag>, TagValidator>();
            services.AddTransient<IValidator<User>, UserValidator>();
            services.AddTransient<IValidator<CreateUser>, CreateUserValidator>();

            // Swagger.
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Online Library API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
