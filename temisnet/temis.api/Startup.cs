using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using temis.Core.Interfaces;
using temis.Core.Models;
using temis.Core.Services.Interfaces;
using temis.Core.Services.Service;
using temis.data.Data;
using temis.Data.Repositories;
using System.Reflection;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using temis.Api.AutoMapper;
using AutoMapper;
using temis.Api.Models.DTO;
using temis.Api.Models.DTO.MemberDto;
using temis.Api.AutoMapper.Mapper.MemberMapper;
using temis.api.Requests;

namespace temis.api
{
    public class Startup
    {
        public static string Secret = "fedaf7d8863b48e197b9287d492b708e";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["MySQLConnection:MySQLConnectionString"];
            
            services.AddStackExchangeRedisCache(options => options.Configuration = this.Configuration.GetConnectionString("redisServerUrl"));
            
            services.AddDbContext<TemisContext>((options) => options.UseMySql(connectionString));

            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IMemberService, MemberService>();

            services.AddScoped<IJudgmentRepository, JudgmentRepository>();
            services.AddScoped<IJudgmentService, JudgmentService>();

            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<IProcessService, ProcessService>();

            services.AddControllers();
            
            services.AddCors(options =>
                {
                    options.AddPolicy("AnyOrigin", builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod();
                    });
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "temis.api", 
                    Version = "v1" ,
                    Description = "Coder Trainee Training with ASP.NET 3.1",
                    
                    Contact = new OpenApiContact
                    {
                        Name = "Coder Trainee Training with ASP.NET 3.1 - Repository",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/PaschoalOliveira/temis/tree/feature/dotnet"),
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference 
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" 
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>() }
                 });

            });

            var key = Encoding.ASCII.GetBytes(Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


          /*   var config = new MapperConfiguration(cfg => {
                 cfg.AddMaps( new Assembly[] { typeof(AutoMapperProfile).GetTypeInfo().Assembly } );
             });
             IMapper mapper = config.CreateMapper(); */

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Member, MemberDto>();
                cfg.CreateMap<Judgment, JudgmentDto>();
                cfg.CreateMap<Process, ProcessDto>();
                cfg.CreateMap<Process, CreateProcessRequest>();
                cfg.CreateMap<CreateProcessRequest, Process>();
                cfg.CreateMap<PageResponse<Process>, PageProcessDto>();
                
            });

            IMapper mapper = config.CreateMapper();

            services.AddSingleton(mapper);

            services.AddMemoryCache();

            services.AddControllers().AddXmlDataContractSerializerFormatters();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "temis.api v1"));
            }

            app.UseCors("AnyOrigin");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
