using Machina.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Machina.Service
{
    public static class CognitiveService
    {
        private static readonly string API_KEY = "b06828fa1b4144ee91b974816a3a03a4";
        private static readonly string ENDPOINT = "https://kwandjeen-machina.cognitiveservices.azure.com/face/v1.0/";
        public static async Task<FaceDetectModels> FaceDetect(Stream imageStream)
        {
            if(imageStream == null)
            {
                return null;
            }
            try
            {
                var url = ENDPOINT + "detect?";
                var client = new HttpClient();
                var queryString = HttpUtility.ParseQueryString(string.Empty);

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", API_KEY);

                // Request parameters
                //queryString["returnFaceId"] = "true";
                //queryString["returnFaceLandmarks"] = "false";
                queryString["returnFaceAttributes"] = "age,gender";
                //queryString["recognitionModel"] = "recognition_01";
                //queryString["returnRecognitionModel"] = "false";
                //queryString["detectionModel"] = "detection_01";
                var uri = url + queryString;

                HttpResponseMessage response;

                // Request body
                byte[] byteData = StreamToByte(imageStream);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);
                    if(response == null && !response.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    string json = response.Content.ReadAsStringAsync().Result;
                    FaceDetectModels[] faceResult = Newtonsoft.Json.JsonConvert.DeserializeObject<FaceDetectModels[]>(json);
                    if(faceResult.Length > 0)
                    {
                        return faceResult[0];
                    }
                }
            } catch (Exception ex)
            {
                Console.WriteLine("erreur http : " + ex.Message);
            }
            return null;
        }
        private static byte[] StreamToByte(Stream stream)
        {
            BinaryReader b = new BinaryReader(stream);
            byte[] data = b.ReadBytes((int)stream.Length);
            return data;
        }
    }
}
