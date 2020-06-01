﻿/*
 * this middleware will set the cookie of antiforgery when the client sent request to the server
 * and make the anti forgery to header named X-XSRF-TOKEN
 * the antiforgery source code: https://github.com/myfor/AspNetCore/tree/master/src/Antiforgery
 */

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace League_Of_Programmers.Middlewares
{
    public class Antiforgery
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;

        private Microsoft.Extensions.Primitives.StringValues acceptValue;

        public Antiforgery(RequestDelegate _next, IAntiforgery _antiforgery)
        {
            this._next = _next;
            this._antiforgery = _antiforgery;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-XSRF-TOKEN", out Microsoft.Extensions.Primitives.StringValues token))
                token = "";

            //  如果请求的是页面或图片，就提供防伪字段
            if (context.Request.Headers.TryGetValue("Accept", out acceptValue) && string.IsNullOrWhiteSpace(token))
            {
                var value = acceptValue.ToString().Split(new char[2] { ';', ',' });

                if (value.Contains("text/html") || value.Contains("application/xhtml+xml") || value.Contains("application/xml") )
                {
                    // The request token can be sent as a JavaScript-readable cookie, 
                    // and Angular uses it by default.
                    //  angular 会自动将 cookie 中 XSRF-TOKEN 的防伪内容在请求时自动加在 X-XSRF-TOKEN 请求头上
                    //  服务器接受到请求时会进行防伪检查，在 LOPController 的特性 [AutoValidateAntiforgeryToken] 中指定
                    //  若不需要进行防伪检查，可在不需要检查的控制器上添加特性 [IgnoreAntiforgeryToken]
                    var tokens = _antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                        new CookieOptions() { HttpOnly = false });
                }
            }

            await _next(context);

        }
    }
}
