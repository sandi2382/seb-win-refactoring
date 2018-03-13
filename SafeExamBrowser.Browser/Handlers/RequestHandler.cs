﻿/*
 * Copyright (c) 2018 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Handler;

namespace SafeExamBrowser.Browser.Handlers
{
	internal delegate void ConfigurationDetectedEventHandler(string url, CancelEventArgs args);

	/// <remarks>
	/// See https://cefsharp.github.io/api/63.0.0/html/T_CefSharp_Handler_DefaultRequestHandler.htm.
	/// </remarks>
	internal class RequestHandler : DefaultRequestHandler
	{
		internal event ConfigurationDetectedEventHandler ConfigurationDetected;

		public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
		{
			Task.Run(() =>
			{
				var allow = true;
				var uri = new Uri(request.Url);

				if (uri.Scheme == "seb" || uri.Scheme == "sebs" || Path.HasExtension("seb"))
				{
					var args = new CancelEventArgs();

					ConfigurationDetected?.Invoke(request.Url, args);

					allow = !args.Cancel;
				}

				using (callback)
				{
					callback.Continue(allow);
				}
			});

			return CefReturnValue.ContinueAsync;
		}
	}
}
