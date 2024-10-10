using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Assigment3.models;

namespace Assigment3
{
    public static class ClientHandler
    {
        public static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            // Read data from client
            byte[] buffer = new byte[2048];
            int bytesRead = 0;
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading from client: " + e.Message);
                client.Close();
                return;
            }

            if (bytesRead == 0)
            {
                // Client did not send any data
                client.Close();
                return;
            }

            string requestString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received request: " + requestString);

            // Parse request
            Request request = null;
            List<string> errors = new List<string>();
            try
            {
                request = JsonSerializer.Deserialize<Request>(requestString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                // Return error
                Response errorResponse = new Response { Status = "4 Bad Request" };
                SendResponse(stream, errorResponse);
                client.Close();
                return;
            }

            // Validate request
            errors = RequestValidator.ValidateRequest(request);

            if (errors.Count > 0)
            {
                // Return error
                Response errorResponse = new Response { Status = "4 " + string.Join(", ", errors) };
                SendResponse(stream, errorResponse);
                client.Close();
                return;
            }

            // Process request
            Response response = RequestProcessor.ProcessRequest(request);

            // Send response
            SendResponse(stream, response);

            client.Close();
        }

        private static void SendResponse(NetworkStream stream, Response response)
        {
            string responseString = JsonSerializer.Serialize(response);
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);
            stream.Write(responseBytes, 0, responseBytes.Length);
            Console.WriteLine("Sent response: " + responseString);
        }
    }
}
