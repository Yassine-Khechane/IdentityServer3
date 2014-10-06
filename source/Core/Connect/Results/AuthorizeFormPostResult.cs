﻿/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.IdentityServer.Core.Connect.Models;
using Thinktecture.IdentityServer.Core.Extensions;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Views.Embedded.Assets;

namespace Thinktecture.IdentityServer.Core.Connect.Results
{
    public class AuthorizeFormPostResult : IHttpActionResult
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();

        private AuthorizeResponse _response;
        private HttpRequestMessage _request;

        public AuthorizeFormPostResult(AuthorizeResponse response, HttpRequestMessage request)
        {
            _response = response;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        HttpResponseMessage Execute()
        {
            // TODO : cleanup using embedded assets helpers
            var root = _request.GetRequestContext().VirtualPathRoot;
            if (root.EndsWith("/")) root = root.Substring(0, root.Length - 1);
            string form = AssetManager.LoadResourceString("Thinktecture.IdentityServer.Core.Views.Embedded.Assets.app.FormPostResponse.html", new { rootUrl = root });
            form = form.Replace("{{redirect_uri}}", _response.RedirectUri.AbsoluteUri);

            var fields = _response.ToNameValueCollection();
            form = form.Replace("{{fields}}", fields.ToFormPost());

            var content = new StringContent(form, Encoding.UTF8, "text/html");
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };

            Logger.Info("Posting to " + _response.RedirectUri.AbsoluteUri);
            return message;
        }
    }
}