﻿using System;
using System.Collections.Generic;
using System.Linq;
using GodelTech.Microservices.Core.Collaborators;
using GodelTech.Microservices.Core.HealthChecks;
using GodelTech.Microservices.Core.Mvc;
using GodelTech.Microservices.Core.Mvc.Security;
using GodelTech.Microservices.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GodelTech.Microservices.Core
{
    public abstract class MicroserviceStartup
    {
        private IMicroserviceInitializer[] _initializers;

        private IEnumerable<IMicroserviceInitializer> Initializers => _initializers ?? (_initializers = CreateInitializers().ToArray());

        public IConfiguration Configuration { get; }

        protected MicroserviceStartup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected virtual IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            var securityInfoProvider = CreateSecurityInfoProvider();

            yield return new CommonServicesInitializer(Configuration);
            yield return new CollaboratorsInitializer(Configuration);

            yield return new SwaggerInitializer(Configuration, securityInfoProvider);

            yield return new CommonMiddlewareInitializer(Configuration);

            yield return new RoutingInitializer(Configuration);
            yield return new ApiSecurityInitializer(Configuration, securityInfoProvider);
            yield return new HealthCheckInitializer(Configuration);
            yield return new MvcInitializer(Configuration);
        }

        protected virtual ISecurityInfoProvider CreateSecurityInfoProvider()
        {
            return new NullSecurityInfoProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            foreach (var initializer in Initializers)
            {
                initializer.Configure(app, env);
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var hostConfig = Configuration.GetSection("Host").Get<HostConfig>();
            if (hostConfig != null)
                services.AddSingleton<IHostConfig>(hostConfig);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IBearerTokenStorage, BearerTokenStorage>();

            foreach (var initializer in Initializers)
            {
                initializer.ConfigureServices(services);
            }
        }
    }
}