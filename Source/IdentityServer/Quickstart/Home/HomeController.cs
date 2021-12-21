// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Grpc.Net.Client;
using Grpc.Services;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IdentityServerTools _tools;

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment, ILogger<HomeController> logger, IdentityServerTools tools)
        {
            _interaction = interaction;
            _environment = environment;
            _logger = logger;
            _tools = tools;
        }

        public IActionResult Index()
        {
            if (_environment.IsDevelopment())
            {
                // only show in development
                return View();
            }

            _logger.LogInformation("Homepage is disabled in production. Returning 404.");
            return NotFound();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return View("Error", vm);
        }

        public async Task<IActionResult> ServiceResult()
        {
	        using (var channel = GrpcChannel.ForAddress("https://localhost:6001"))
	        {
		        var client = new Service.ServiceClient(channel);
		        var response = await client.GetAsync(new Request());

		        ViewBag.ServiceResult = response.Value;

		        return View();
	        }
        }
    }
}