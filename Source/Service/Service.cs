using System;
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

			var response = new Response();
			var user = context.GetHttpContext().User;
			response.Values.Add($"IsAuthenticated = {user.Identity.IsAuthenticated}");
			foreach(var claim in user.Claims)
			{
				response.Values.Add($"{claim.Type} = {claim.Value}");
			}

			return await Task.FromResult(response);
		}

		#endregion
	}
}