﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    public class MicroserviceInitializerBase : IMicroserviceInitializer
    {
        protected IConfiguration Configuration { get; }

        public MicroserviceInitializerBase(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }
    }
}