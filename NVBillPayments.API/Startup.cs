using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NVBillPayments.Core;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.PaymentProviders.Beyonic;
using NVBillPayments.PaymentProviders.DPO;
using NVBillPayments.PaymentProviders.Flutterwave;
using NVBillPayments.PaymentProviders.Interswitch;
using NVBillPayments.PaymentProviders.Pegasus;
using NVBillPayments.PaymentProviders.Pesapal;
using NVBillPayments.ServiceProviders;
using NVBillPayments.ServiceProviders.AIRTELUG;
using NVBillPayments.ServiceProviders.MTNUG;
using NVBillPayments.ServiceProviders.NewVision;
using NVBillPayments.ServiceProviders.Quickteller;
using NVBillPayments.Services;
using NVBillPayments.Shared;
using NVBillPayments.WorkerService;

namespace NVBillPayments.API
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
            services.AddControllers();
            services.AddSwaggerGen();

            var dbOptions = new DbContextOptionsBuilder<NVTransactionsDbContext>()
                .UseSqlServer(ConfigurationConstants.DBCONNECTION, providerOptions => providerOptions.EnableRetryOnFailure());

            services.AddDbContext<NVTransactionsDbContext>(options => options = dbOptions);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddSingleton<IQRCodeService, QRCodeService>();
            services.AddHostedService<TransactionsWorker>();
            //services.AddHostedService<TransactionsLogWorker>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITransactionLogService, TransactionLogService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IServiceProviderService, ServiceProviderService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICachingService, CachingService>();
            //third party services
            services.AddSingleton<IPesapalService, PesapalService>();
            services.AddSingleton<IPegasusService, PegasusService>();
            services.AddSingleton<IBeyonicService, BeyonicService>();
            services.AddSingleton<IDPOService, DPOService>();
            services.AddSingleton<IFlutterwaveService, FlutterwaveService>();
            services.AddSingleton<IInterswitchService, InterswitchService>();
            services.AddSingleton<IQuicktellerService, QuicktellerService>();
            services.AddSingleton<IMTNService, MTNService>();
            services.AddSingleton<IAirtelService, AirtelService>();
            services.AddSingleton<INewVisionService, NewVisionService>();

            services.AddSingleton<IEventTicketsManagementService, EventTicketsManagementService>();

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = ConfigurationConstants.REDIS_URI;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, NVTransactionsDbContext dbContext)
        {
            // migrate any database changes on startup (includes initial db creation)
            //dbContext.Database.Migrate();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/StaticFiles",
                EnableDefaultFiles = true
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
