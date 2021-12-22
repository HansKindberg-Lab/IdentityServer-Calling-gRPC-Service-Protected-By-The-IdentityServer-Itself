// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServerHost.Quickstart.UI
{
	[SecurityHeaders]
	public class HomeController : Controller
	{
		#region Fields

		private readonly IWebHostEnvironment _environment;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly ILogger _logger;
		private readonly IdentityServerTools _tools;

		#endregion

		#region Constructors

		public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment, ILogger<HomeController> logger, IdentityServerTools tools)
		{
			this._interaction = interaction;
			this._environment = environment;
			this._logger = logger;
			this._tools = tools;
		}

		#endregion

		#region Methods

		[Authorize]
		//[AllowAnonymous]
		public async Task<IActionResult> CallService()
		{
			// If the authorize attribute would be removed.
			if(!this.User.IsAuthenticated())
			{
				this.ViewBag.Warning = "Not authenticated.";
				return this.View();
			}

			try
			{
				using(var channel = GrpcChannel.ForAddress("https://localhost:6001"))
				{
					//var token = await this._tools.IssueJwtAsync(10, this.HttpContext.User.Claims);
					var token = await this._tools.IssueClientJwtAsync(this.HttpContext.GetIdentityServerIssuerUri(), 10, additionalClaims: this.HttpContext.User.Claims);

					var client = new Service.ServiceClient(channel);

					var headers = new Metadata
					{
						{ "Authorization", $"Bearer {token}" }
					};

					var response = await client.GetAsync(new Request(), headers);

					this.ViewBag.Result = response.Values.ToArray();
				}
			}
			catch(Exception exception)
			{
				this.ViewBag.Error = exception.Message;
			}

			return this.View();
		}

		/// <summary>
		/// Shows the error page
		/// </summary>
		[AllowAnonymous]
		public async Task<IActionResult> Error(string errorId)
		{
			var vm = new ErrorViewModel();

			// retrieve error details from identityserver
			var message = await this._interaction.GetErrorContextAsync(errorId);
			if(message != null)
			{
				vm.Error = message;

				if(!this._environment.IsDevelopment())
				{
					// only show in development
					message.ErrorDescription = null;
				}
			}

			return this.View("Error", vm);
		}

		[AllowAnonymous]
		public IActionResult Index()
		{
			if(this._environment.IsDevelopment())
			{
				// only show in development
				return this.View();
			}

			this._logger.LogInformation("Homepage is disabled in production. Returning 404.");
			return this.NotFound();
		}

		#endregion
	}
}