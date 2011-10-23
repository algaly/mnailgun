﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using Typesafe.Mailgun.Extensions.Json;
using Typesafe.Mailgun.Routing;
using Typesafe.Mailgun.Statistics;

namespace Typesafe.Mailgun
{
	/// <summary>
	/// Provides access to the Mailgun REST API.
	/// </summary>
	public class MailgunClient : IMailgunAccountInfo
	{
		/// <summary>
		/// Initializes a new client for the specified domain and api key.
		/// </summary>
		public MailgunClient(string domain, string apiKey)
		{
			DomainBaseUrl = new Uri("https://api.mailgun.net/v2/" + domain + "/");
			ApiKey = apiKey;
		}

		public Uri DomainBaseUrl { get; private set; }

		public string ApiKey { get; private set; }

		public IEnumerable<MailgunStatEntry> GetStats(out int count)
		{
			return GetStats(0, 100, MailgunEventTypes.Sent, out count);
		}

		public IEnumerable<MailgunStatEntry> GetStats(int skip, int take, MailgunEventTypes eventTypes, out int count)
		{
			return new MailgunStatsQuery(this, eventTypes).Execute(skip, take, out count);
		}

		public IEnumerable<Route> GetRoutes(int skip, int take, out int count)
		{
			return new MailgunRouteQuery(this).Execute(skip, take, out count);
		}

		public Route CreateRoute(int priority, string description, RouteFilter expression, params RouteAction[] actions)
		{
			return new CreateRouteCommand(this, priority, description, expression, actions).Invoke().Route;
		}

		public CommandResult SendMail(MailMessage mailMessage)
		{
			return new SendMailCommand(this, mailMessage).Invoke();
		}
	}
}