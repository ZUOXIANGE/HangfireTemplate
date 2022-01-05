using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MySql;
using HangfireTemplate.Extensions;
using HangfireTemplate.Services;
using HangfireTemplate.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HangfireTemplate
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var taskConn = Configuration["TaskConnectionString"];
            services.AddHangfire(x =>
                x.UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseStorage(new MySqlStorage(taskConn, new MySqlStorageOptions
                    {
                        TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 50000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire"
                    })
                    ));
            services.AddHangfireServer();
            services.Configure<MyTaskOptions>(Configuration.GetSection("MyTaskOptions"));

            services.AddTasks();

            services.AddScoped<TestService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            var myTaskOptions = Configuration.GetSection("MyTaskOptions").Get<MyTaskOptions>();
            //使用hangfire面板
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(
                        new BasicAuthAuthorizationFilterOptions
                        {
                            SslRedirect = false,
                            RequireSsl = false,
                            LoginCaseSensitive = false,
                            Users = new[]
                            {
                                new BasicAuthAuthorizationUser
                                {
                                    Login = myTaskOptions.UserName,
                                    PasswordClear = myTaskOptions.Password
                                }
                            }
                        })
                }
            });

            //批量启动任务
            var maTasks = app.ApplicationServices.GetService<IEnumerable<IMyTask>>();
            if (maTasks == null) return;
            foreach (var maTask in maTasks)
            {
                var taskOption = myTaskOptions.Tasks.FirstOrDefault(x => x.TaskName == maTask.GetTaskName());
                if (taskOption != null && !string.IsNullOrWhiteSpace(taskOption.Cron))
                {
                    maTask.Start(taskOption.Cron);
                }
            }
        }
    }
}