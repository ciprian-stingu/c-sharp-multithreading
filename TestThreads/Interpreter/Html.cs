using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace TestThreads.Interpreter
{
    class Html : Interface.Interpreter, Interface.Named
    {
        private string[] validExtensions =
        {
            ".htm",
            ".html",
            ".css",
            ".js",
            ".jpg",
            ".jpeg",
            ".png"
        };

        private enum RequestType : int { GET, POST }; 
        public byte[] Process(string data)
        {
            string requestData = data.Substring(0, data.IndexOf("\n")).Trim();
            RequestType requestType = GetRequestType(requestData);
            Factory.Logger.Get().Log(Name() + " -> Request type: " + requestType);
            string filePathAndParams = GetFilePathAndParams(requestData);
            Factory.Logger.Get().Log(Name() + " -> File path & params: " + filePathAndParams);
            string fileName = GetFileName(filePathAndParams);
            Factory.Logger.Get().Log(Name() + " -> File name: " + fileName);
            List<string> parameters = GetParams(filePathAndParams);
            Factory.Logger.Get().Log(Name() + " -> Params: " + string.Join(", ", parameters));
            string currentFolder = Util.Utilities.GetCurrentFolder();
            if(currentFolder.IndexOf(@"bin\Debug\netcoreapp3.0") >= 0) {
                currentFolder = currentFolder.Substring(0, currentFolder.IndexOf(@"bin\Debug\netcoreapp3.0"));
            }
            Factory.Logger.Get().Log(Name() + " -> Html folder: " + currentFolder + @"HtmlTest\");
            string realFileName = currentFolder + @"HtmlTest\" + (fileName == "/" ? "index.html" : fileName);
            Factory.Logger.Get().Log(Name() + " -> Real file name: " + realFileName);
            if (!File.Exists(realFileName))
            {
                Factory.Logger.Get().Log(Name() + " -> Real file name doesn't exists!");
                return Get404Page();
            }

            byte[] fileData = null;
            try
            {
                fileData = File.ReadAllBytes(realFileName);
            }
            catch(Exception e)
            {
                Factory.Logger.Get().Log(Name() + " -> " + e.Message);
                return Get404Page();
            }

            return GetOK(fileData, realFileName);
        }

        private RequestType GetRequestType(string data)
        {
            if(data.IndexOf("POST") == 0)
            {
                return RequestType.POST;
            }
            return RequestType.GET;
        }

        private string GetFilePathAndParams(string data)
        {
            string path = data.Substring(data.IndexOf(" ") + 1);
            path = path.Substring(0, path.IndexOf(" "));
            return path;
        }

        public string Name()
        {
            return "[" + Thread.CurrentThread.GetHashCode() + "] Interpreter.Html";
        }

        private string GetFileName(string data)
        {
            if(data.IndexOf("?") >= 0) {
                data = data.Substring(0, data.IndexOf("?"));
            }
            if(Array.IndexOf(validExtensions, Path.GetExtension(data)) < 0) {
                return "/";
            }
            return data;
        }

        private List<string> GetParams(string data)
        {
            List<string> parameters = new List<string>(); //params is a keyword...
            if(data.IndexOf("?") >= 0)
            {
                data = data.Substring(data.IndexOf("?") + 1);
                parameters.AddRange(data.Split("&"));
            }
            return parameters;
        }

        private byte[] Get404Page()
        {
            string errorPage = "HTTP/1.0 404 Not Found \nContent-Type: text/html\n\n<html><body><center><h1>Error 404: It's not here...</h1></center><p>Try the <a href=\"/\">root page</a>.</p></body></html>";
            return Encoding.ASCII.GetBytes(errorPage);
        }

        private byte[] GetOK(byte[] data, string fileName)
        {
            StringBuilder header = new StringBuilder();
            header.Append("HTTP/1.0 200 OK \n");

            string extension = Path.GetExtension(fileName);
            if(extension == ".html" || extension == "htm") {
                header.Append("Content-Type: text/html\n");
            }
            else if(extension == ".css") {
                header.Append("Content-Type: text/css\n");
            }
            else if(extension == ".js") {
                header.Append("Content-Type: text/javascript\n");
            }
            else if(extension == ".jpg" || extension == ".jpeg") {
                header.Append("Content-Type: image/jpeg\n");
            }
            else if (extension == ".png") {
                header.Append("Content-Type: image/png\n");
            }
            header.Append("\n");

            byte[] headerData = Encoding.ASCII.GetBytes(header.ToString());
            byte[] result = new byte[headerData.Length + data.Length];
            Array.Copy(headerData, 0, result, 0, headerData.Length);
            Array.Copy(data, 0, result, headerData.Length, data.Length);

            return result;
        }
    }
}
