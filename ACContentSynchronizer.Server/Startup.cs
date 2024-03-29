using ACContentSynchronizer.Server.Hubs;
using ACContentSynchronizer.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Serilog;

namespace ACContentSynchronizer.Server {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddSignalR();
      services.AddHttpContextAccessor();
      services.AddScoped<SignalRService>();
      services.AddScoped<ContentService>();
      services.AddScoped<ServerConfigurationService>();

      services.AddControllers().AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
      });

      services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new() { Title = "ACContentSynchronizer.Server", Version = "v1" });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ACContentSynchronizer.Server v1"));

      app.UseSerilogRequestLogging();
      app.UseRouting();

      app.UseAuthorization();

      var counter = Metrics.CreateCounter("api_path_counter", "Counts requests to the API endpoints",
        new CounterConfiguration {
          LabelNames = new[] { "method", "endpoint" },
        });

      app.Use((context, next) => {
        counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
        return next();
      });

      app.UseMetricServer();
      app.UseHttpMetrics();

      app.UseEndpoints(endpoints => {
        endpoints.MapHub<NotificationHub>(Constants.HubEndpoint);
        endpoints.MapControllers();
      });
    }
  }
}
