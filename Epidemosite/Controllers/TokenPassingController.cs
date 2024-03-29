﻿using Epidemosite.Business;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Epidemosite.Controllers
{
    public class TokenPassingController : Controller
    {
        // GET: TokenPassing
        [Authorize]
        public ActionResult Index()
        {
            var cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            var ticket = cookie.Value;

            //Handle encoding
            ticket = ticket.Replace('-', '+').Replace('_', '/');
            var padding = 3 - ((ticket.Length + 3) % 4);
            if (padding != 0) { ticket = ticket + new string('=', padding); }

            var machineKeyProtector = new MachineKeyProtector();
            var ticketData = new TicketDataFormat(machineKeyProtector);


            //Set the purpose for decrypting the cookie based ticket
            machineKeyProtector.Purpose = new string[]
            {
                typeof(CookieAuthenticationMiddleware).FullName,
                        "ApplicationCookie",
                        "v1"
            };
            var decryptedTicket = ticketData.Unprotect(ticket);

            //Change the purpose for creating an encrypted bearer token
            machineKeyProtector.Purpose = new string[]
            {
                typeof(OAuthAuthorizationServerMiddleware).Namespace,
                        "Access_Token",
                        "v1"
            };
            var encryptedTicket = ticketData.Protect(decryptedTicket);

            string bearerToken = $"Bearer {encryptedTicket}";

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:58719/");
            client.DefaultRequestHeaders.Add("Authorization", bearerToken);
            var result = client.GetAsync("/api/ResourceData").Result.Content.ReadAsStringAsync().Result;
            ViewBag.ResultData = result;
            ViewBag.BearerToken = bearerToken;
            return View();
        }
    }
}