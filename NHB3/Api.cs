﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NHB3
{
    class Api
    {
        private string urlRoot;
        private string orgId;
        private string apiKey;
        private string apiSecret;

        public string time;

        public Api(string urlRoot, string orgId, string apiKey, string apiSecret)
        {
            this.urlRoot = urlRoot;
            this.orgId = orgId;
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
        }

        private static string HashBySegments(string key, string apiKey, string time, string nonce, string orgId, string method, string encodedPath, string query, string bodyStr)
        {
            List<string> segments = new List<string>();
            segments.Add(apiKey);
            segments.Add(time);
            segments.Add(nonce);
            segments.Add(null);
            segments.Add(orgId);
            segments.Add(null);
            segments.Add(method);
            segments.Add(encodedPath == null ? null : encodedPath);
            segments.Add(query == null ? null : query);

            if (bodyStr != null && bodyStr.Length > 0)
            {
                segments.Add(bodyStr);
            }
            return Api.CalcHMACSHA256Hash(Api.JoinSegments(segments), key);
        }
        private static string getPath(string url)
        {
            var arrSplit = url.Split('?');
            return arrSplit[0];
        }
        private static string getQuery(string url)
        {
            var arrSplit = url.Split('?');

            if (arrSplit.Length == 1)
            {
                return null;
            }
            else
            {
                return arrSplit[1];
            }
        }

        private static string JoinSegments(List<string> segments)
        {
            var sb = new System.Text.StringBuilder();
            bool first = true;
            foreach (var segment in segments)
            {
                if (!first)
                {
                    sb.Append("\x00");
                }
                else
                {
                    first = false;
                }

                if (segment != null)
                {
                    sb.Append(segment);
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Out.WriteLine("req: [" + sb.ToString() + "]");
            return sb.ToString();
        }

        private static string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new System.Security.Cryptography.HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }

        private string srvrTime()
        {
            string timeResponse = this.get("/api/v2/time", false);
            JObject timeObject = JsonConvert.DeserializeObject<JObject>(timeResponse);

            if (timeObject["error_id"] == null)
            {
                this.time = "" + timeObject["serverTime"];
                return this.time;
            }
            return "0";
        }

        public string get(string url, bool auth)
        {
            var client = new RestSharp.RestClient(this.urlRoot);
            var request = new RestSharp.RestRequest(url);

            if (auth)
            {
                string time = this.srvrTime();
                string nonce = Guid.NewGuid().ToString();
                string digest = Api.HashBySegments(this.apiSecret, this.apiKey, time, nonce, this.orgId, "GET", getPath(url), getQuery(url), null);

                request.AddHeader("X-Time", time);
                request.AddHeader("X-Nonce", nonce);
                request.AddHeader("X-Auth", this.apiKey + ":" + digest);
                request.AddHeader("X-Organization-Id", this.orgId);
            }

            var response = client.Execute(request, RestSharp.Method.GET);
            Console.Out.WriteLine("res: [" + response.Content + "]");

            if (response.StatusCode != HttpStatusCode.OK) 
            {
                return "{error_id: -1}";
            }
            return response.Content;
        }

        public string post(string url, string payload, bool requestId)
        {
            string time = this.srvrTime();
            var client = new RestSharp.RestClient(this.urlRoot);
            var request = new RestSharp.RestRequest(url);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-type", "application/json");

            string nonce = Guid.NewGuid().ToString();
            string digest = Api.HashBySegments(this.apiSecret, this.apiKey, time, nonce, this.orgId, "POST", getPath(url), getQuery(url), payload);

            if (payload != null)
            {
                request.AddJsonBody(payload);
            }

            request.AddHeader("X-Time", time);
            request.AddHeader("X-Nonce", nonce);
            request.AddHeader("X-Auth", this.apiKey + ":" + digest);
            request.AddHeader("X-Organization-Id", this.orgId);

            if (requestId)
            {
                request.AddHeader("X-Request-Id", Guid.NewGuid().ToString());
            }

            var response = client.Execute(request, RestSharp.Method.POST);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Out.WriteLine("res: [" + response.Content + "]");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "{error_id: -1}";
            }
            return response.Content;
        }

        public string delete(string url, bool requestId)
        {
            string time = this.srvrTime();
            var client = new RestSharp.RestClient(this.urlRoot);
            var request = new RestSharp.RestRequest(url);

            string nonce = Guid.NewGuid().ToString();
            string digest = Api.HashBySegments(this.apiSecret, this.apiKey, time, nonce, this.orgId, "DELETE", getPath(url), getQuery(url), null);

            request.AddHeader("X-Time", time);
            request.AddHeader("X-Nonce", nonce);
            request.AddHeader("X-Auth", this.apiKey + ":" + digest);
            request.AddHeader("X-Organization-Id", this.orgId);

            if (requestId)
            {
                request.AddHeader("X-Request-Id", Guid.NewGuid().ToString());
            }

            var response = client.Execute(request, RestSharp.Method.DELETE);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Out.WriteLine("res: [" + response.Content + "]");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "{error_id: -1}";
            }
            return response.Content;
        }
    }
}