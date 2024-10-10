using System;
using System.Collections.Generic;
using System.Text.Json;
using Assigment3.models;

namespace Assigment3

{
    public static class RequestValidator
    {
        public static List<string> ValidateRequest(Request request)
        {
            List<string> errors = new List<string>();

            // method is mandatory
            if (string.IsNullOrEmpty(request.method))
            {
                errors.Add("missing method");
            }
            else
            {
                // Check if method is one of the allowed methods
                string[] allowedMethods = { "create", "read", "update", "delete", "echo" };
                if (Array.IndexOf(allowedMethods, request.method.ToLower()) == -1)
                {
                    errors.Add("illegal method");
                }
            }

            // date is mandatory
            if (string.IsNullOrEmpty(request.date))
            {
                errors.Add("missing date");
            }
            else
            {
                // Check if date is valid unix timestamp
                if (!long.TryParse(request.date, out long unixTime))
                {
                    errors.Add("illegal date");
                }
            }

            // For methods other than echo, path is mandatory
            if (request.method != null && request.method.ToLower() != "echo")
            {
                if (string.IsNullOrEmpty(request.path))
                {
                    errors.Add("missing resource");
                }
            }

            // For methods that require body
            if (request.method != null)
            {
                string methodLower = request.method.ToLower();
                if ((methodLower == "create" || methodLower == "update" || methodLower == "echo"))
                {
                    if (string.IsNullOrEmpty(request.body))
                    {
                        errors.Add("missing body");
                    }
                    else
                    {
                        // For update and create, body must be JSON
                        if (methodLower == "create" || methodLower == "update")
                        {
                            try
                            {
                                JsonDocument.Parse(request.body);
                            }
                            catch (JsonException)
                            {
                                errors.Add("illegal body");
                            }
                        }
                    }
                }
            }

            return errors;
        }
    }
}
