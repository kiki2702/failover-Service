using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FailoverScheduler.Factories;
using FailoverScheduler.Interfaces;
using FailoverScheduler.Jobs;
using FailoverScheduler.Models;
using FailoverScheduler.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserAuthorization;

namespace FailoverScheduler
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<IKafkaSendMessageService, KafkaSendMessageService>();
            services.AddDbContext<FailoverDbContext>(options =>
                     options.UseSqlServer(Configuration.GetConnectionString("DefaultDatabase")));
            services.AddTransient<ISchedulerService, SchedulerService>();
            services.AddTransient<JobFactory>();
            services.AddScoped<FailoverProducerJob>();
            services.AddScoped<FailoverConsumerJob>();
            services.AddTransient<KafkaReceiveMessageService>();
            services.AddScoped<IHandlingDBService, HandlingDBService>();
            services.AddTransient<IReceiveMethodService, ReceiveMethodService>();
            //services.AddTransient<IKafkaNotificationService,KafkaNotificationService>();
            services.AddTransient<IKafkaReceiveMessageService, KafkaReceiveMessageService>();
            services.AddTransient<IEmailNotificationService, EmailNotificationService>();
            

          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            var schedulerService = app.ApplicationServices.GetRequiredService<ISchedulerService>();
            schedulerService.Init();
            schedulerService.Start();

            //var masterDataService = app.ApplicationServices.GetRequiredService<KafkaReceiveMessageService>();
            //masterDataService.StartConsumeData();
        }
    }
}
