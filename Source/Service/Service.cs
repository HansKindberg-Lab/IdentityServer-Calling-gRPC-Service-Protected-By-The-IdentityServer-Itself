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

			var identity = context.GetHttpContext().User.Identity;

			var response = new Response
			{
				Value = identity == null || !identity.IsAuthenticated ? "Anonymous" : identity.Name ?? "No name"
			};

			return await Task.FromResult(response);
		}

		#endregion
	}
}