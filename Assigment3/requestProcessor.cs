using System;
using System.Text.Json;
using Assigment3.models;


namespace Assigment3
{
    public static class RequestProcessor
    {
        public static Response ProcessRequest(Request request)
        {
            string method = request.method.ToLower();

            switch (method)
            {
                case "echo":
                    return new Response { Status = "1 Ok", Body = request.body };
                case "read":
                    return HandleRead(request);
                case "create":
                    return HandleCreate(request);
                case "update":
                    return HandleUpdate(request);
                case "delete":
                    return HandleDelete(request);
                default:
                    return new Response { Status = "6 Error" };
            }
        }

        private static Response HandleRead(Request request)
        {
            if (request.path == "/api/categories")
            {
                // Return all categories
                string body = JsonSerializer.Serialize(DataStore.Categories);
                return new Response { Status = "1 Ok", Body = body };
            }
            else if (request.path.StartsWith("/api/categories/"))
            {
                // Get category by id
                string[] parts = request.path.Split('/');
                if (parts.Length != 4)
                {
                    return new Response { Status = "4 Bad Request" };
                }
                else
                {
                    string idStr = parts[3];
                    if (int.TryParse(idStr, out int id))
                    {
                        Category category = DataStore.Categories.Find(c => c.Id == id);
                        if (category != null)
                        {
                            string body = JsonSerializer.Serialize(category);
                            return new Response { Status = "1 Ok", Body = body };
                        }
                        else
                        {
                            return new Response { Status = "5 Not Found" };
                        }
                    }
                    else
                    {
                        return new Response { Status = "4 Bad Request" };
                    }
                }
            }
            else
            {
                // Invalid path
                return new Response { Status = "4 Bad Request" };
            }
        }

        private static Response HandleCreate(Request request)
        {
            if (request.path == "/api/categories")
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    Category newCategory = JsonSerializer.Deserialize<Category>(request.body, options);
                    newCategory.Id = DataStore.GetNextId();
                    DataStore.Categories.Add(newCategory);
                    string body = JsonSerializer.Serialize(newCategory);
                    return new Response { Status = "2 Created", Body = body };
                }
                catch (JsonException)
                {
                    return new Response { Status = "4 Bad Request" };
                }
            }
            else
            {
                // Create with path including id is invalid
                return new Response { Status = "4 Bad Request" };
            }
        }

        private static Response HandleUpdate(Request request)
        {
            if (request.path.StartsWith("/api/categories/"))
            {
                string[] parts = request.path.Split('/');
                if (parts.Length != 4)
                {
                    return new Response { Status = "4 Bad Request" };
                }
                else
                {
                    string idStr = parts[3];
                    if (int.TryParse(idStr, out int id))
                    {
                        Category category = DataStore.Categories.Find(c => c.Id == id);
                        if (category != null)
                        {
                            try
                            {
                                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                                Category updatedCategory = JsonSerializer.Deserialize<Category>(request.body, options);
                                category.Name = updatedCategory.Name;
                                return new Response { Status = "3 Updated" };
                            }
                            catch (JsonException)
                            {
                                return new Response { Status = "4 Bad Request" };
                            }
                        }
                        else
                        {
                            return new Response { Status = "5 Not Found" };
                        }
                    }
                    else
                    {
                        return new Response { Status = "4 Bad Request" };
                    }
                }
            }
            else
            {
                // Update without id in path is invalid
                return new Response { Status = "4 Bad Request" };
            }
        }

        private static Response HandleDelete(Request request)
        {
            if (request.path.StartsWith("/api/categories/"))
            {
                string[] parts = request.path.Split('/');
                if (parts.Length != 4)
                {
                    return new Response { Status = "4 Bad Request" };
                }
                else
                {
                    string idStr = parts[3];
                    if (int.TryParse(idStr, out int id))
                    {
                        Category category = DataStore.Categories.Find(c => c.Id == id);
                        if (category != null)
                        {
                            DataStore.Categories.Remove(category);
                            return new Response { Status = "1 Ok" };
                        }
                        else
                        {
                            return new Response { Status = "5 Not Found" };
                        }
                    }
                    else
                    {
                        return new Response { Status = "4 Bad Request" };
                    }
                }
            }
            else
            {
                // Delete without id in path is invalid
                return new Response { Status = "4 Bad Request" };
            }
        }
    }
}
