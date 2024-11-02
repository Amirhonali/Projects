using Infrastructure;
using Application;
using Microsoft.OpenApi.Models;

namespace BookCatalogApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",   
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {{
                    new OpenApiSecurityScheme()
                    {
                       Reference=new OpenApiReference()
                       {
                           Id="Bearer",
                           Type=ReferenceType.SecurityScheme
                       }
                    },
                    new List<string>()
                } });
            });

            builder.Services.AddMemoryCache();  //In-Memory cache
            
            builder.Services.AddStackExchangeRedisCache(setupAction =>
            {
                setupAction.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
            });


            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("PolicyForPDP",
                policy =>
                {
                    policy.WithOrigins("https://online.pdp.uz")
                    .WithHeaders("TestHeader")
                    .WithMethods("GET", "Put");
                });

                opt.AddPolicy("PolicyForMicrosoft",
                policy =>
                {
                    policy.WithOrigins("https://www.microsoft.com")
                    .WithHeaders("Net-Header")
                    .WithMethods("GET", "Post");
                });

                opt.AddDefaultPolicy(policyOpt =>
                {
                    policyOpt.WithOrigins("https://www.google.com");

                });
            });

            builder.Services.AddResponseCaching();

            builder.Services.AddOutputCache();

            builder.Services.AddControllers();

            //builder.Services.AddRateLimiter(opt =>
            //{
            //    opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            //    opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            //        RateLimitPartition.GetConcurrencyLimiter(
            //             partitionKey: "ConcurrencyLimiter",
            //             factory: x => new ConcurrencyLimiterOptions
            //             {
            //                 PermitLimit = 4,
            //                 QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            //                 QueueLimit = 2,
            //             }));
            //});

            ////AddFixedWindowLimiter
            //builder.Services.AddRateLimiter(options =>
            //{
            //    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            //    options.AddFixedWindowLimiter("FixedWindow", x =>
            //    {
            //        x.PermitLimit = 3;
            //        x.QueueLimit = 0;
            //        x.Window = TimeSpan.FromSeconds(20);
            //        x.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            //        // x.AutoReplenishment = true;
            //    });
            //});

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //app.UseRateLimiter();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.Use((context, next) =>
            {
                if (context.Request.Method.Equals("OPTIONS"))
                {
                    Console.WriteLine(context.Request.Method);
                    foreach (var item in context.Request.Headers)
                    {
                        Console.WriteLine(item.Key + ":" + item.Value);
                    }
                    next();

                    Console.WriteLine("\nThis is Response header \n");
                    foreach (var item in context.Response.Headers)
                    {
                        Console.WriteLine(item.Key + ":" + item.Value);
                    }
                   
                }
                else
                {
                    next();
                }
                return Task.CompletedTask;
            });
            app.UseCors(opt =>
            {
                opt.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            app.UseCors("PolicyForPDP");
            app.UseCors("PolicyForMicrosoft");
            //app.UseCors(opt =>
            //{
            //    opt.AllowAnyMethod()
            //    .AllowAnyOrigin()
            //    .AllowAnyHeader();
            //    //.WithOrigins("https://online.pdp.uz")
            //    //.WithHeaders("my-header", "Test-Header","Other-Header")
            //    //.WithMethods("GET")
            //    //.AllowCredentials();
            //});
            app.UseResponseCaching();

            app.UseOutputCache();
            //app.UseETagger();
            app.MapControllers();

            app.Run();
        }
    }
}