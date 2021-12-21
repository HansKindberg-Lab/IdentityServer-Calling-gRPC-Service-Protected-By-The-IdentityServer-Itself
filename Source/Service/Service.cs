using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Services;

namespace Service
{
	public class Service : Grpc.Services.Service.ServiceBase
	{
		#region Methods

		public override async Task<Response> Get(Request request, ServerCallContext context)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));

			//var identity = context.GetHttpContext().User.Identity;
			//var value = identity == null || !identity.IsAuthenticated ? "Anonymous" : identity.Name ?? "No name";

			var user = context.GetHttpContext().User;
			var values = new List<string>();

			foreach(var claim in user.Claims)
			{
				values.Add($"{claim.Type} = {claim.Value}");
			}

			var value = string.Join(", ", values);

			var response = new Response
			{
				Value = value
			};

			return await Task.FromResult(response);
		}

		#endregion
	}
}