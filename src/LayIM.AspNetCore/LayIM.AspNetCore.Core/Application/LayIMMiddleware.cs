﻿using LayIM.AspNetCore.Core.Dispatcher;
using LayIM.AspNetCore.Core.Routes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LayIM.AspNetCore.Core.Application
{
    public class LayIMMiddleware
    {
        private readonly RequestDelegate next;
        private readonly LayIMOptions options;
        private readonly IServiceProvider serviceProvider;

        public LayIMMiddleware(RequestDelegate next, LayIMOptions options, IServiceProvider serviceProvider)
        {
            this.next = next;
            this.options = options;
            this.serviceProvider = serviceProvider;
            LayIMServiceLocator.SetServiceProvider(this.serviceProvider);
        }


        public async Task Invoke(HttpContext context)
        {
            if (context.IsLayIMRequest(options) == false)
            {
                await next?.Invoke(context);
                return;
            }

            string path = context.GetRoutePath(options);

            var dispatcher = LayIMRoutes.Routes.FindDispatcher(path);

            if (dispatcher != null)
            {
                await DispatcherWrapper.Wrapper.Dispatch(dispatcher, context);
            }
            else
            {
                await LayIMRoutes.ResourceDispatcher.Dispatch(context, options, next);
            }
        }
    }
}
