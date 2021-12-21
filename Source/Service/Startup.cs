using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service
{
	public class Startup
	{
		#region Constructors

		public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
		{
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
		}

		#endregion

		#region Properties

		protected internal virtual IConfiguration Configuration { get; }
		protected internal virtual IHostEnvironment HostEnvironment { get; }

		#endregion

		#region Methods

		public virtual void Configure(IApplicationBuilder applicationBuilder)
		{
			if(applicationBuilder == null)
				throw new ArgumentNullException(nameof(applicationBuilder));

			applicationBuilder
				.UseDeveloperExceptionPage()
				.UseRouting()
				.UseAuthentication()
				.UseAuthorization()
				.UseEndpoints(endpoints =>
				{
					endpoints.MapGrpcService<Service>().RequireAuthorization();

					endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909").ConfigureAwait(false); });
				});
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					//options.Audience = "";
					options.Authority = "https://localhost:5001";
					//options.MapInboundClaims = false;
					options.TokenValidationParameters.ValidateAudience = false;
					options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
				});

			services.AddAuthorization();

			services.AddGrpc();
		}

		#endregion
	}
}