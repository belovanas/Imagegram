using System.Collections.Generic;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Internal;
using Amazon.Runtime;
using Amazon.S3;
using Dynamo.Abstractions;
using Dynamo.Repositories;
using Imagegram.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using S3;

namespace Imagegram;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private IConfiguration _configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            {
                Description = "Basic auth added to authorization header",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "basic",
                Type = SecuritySchemeType.Http
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Basic"}
                    },
                    new List<string>()
                }
            });
        });
        
        services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        services.AddHttpContextAccessor();
 
        services.AddAuthorization();

        ConfigureAwsInfrastructure(services);
        ConfigurePersistence(services);
    }

    private void ConfigureAwsInfrastructure(IServiceCollection services)
    {
        AWSCredentials credentials = new BasicAWSCredentials("", ");
        services.AddScoped<IAmazonDynamoDB>(_ => 
            new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1));
        services.AddScoped<IAmazonS3>(_ => new AmazonS3Client(credentials, RegionEndpoint.USEast1));
    }

    private void ConfigurePersistence(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IImageUploader, ImageUploader>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });

        app.UseSwagger();
        app.UseSwaggerUI();
    }
}