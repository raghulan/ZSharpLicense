using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;

namespace ZSharpLicHelper.Helper
{
    public class ApiIntegration
    {
        public static APIResult callGetAPI(string apiUrl, List<APIParameters> apiParameters)
        {
            APIResult _apiResult = new APIResult();
            try
            {
                RestClient client = new RestClient(apiUrl);
                RestRequest request = new RestRequest(new Uri(String.Format("{0}", apiUrl)), Method.GET);
                if (apiParameters.Count > 0)
                {
                    foreach (var para in apiParameters)
                    {
                        if (apiParameters.Any(x => x.Type == APIParameterType.HttpHeader))
                        {
                            request.AddParameter(para.Key, para.Value.ToString(), ParameterType.HttpHeader);
                        }
                        else
                        {
                            request.AddBody(para.Value);
                        }
                    }
                }
                var apiResponse = client.Execute(request);
                _apiResult.Content = apiResponse.Content;
                _apiResult.ResponseStatusCode = apiResponse.StatusCode;
            }
            catch (Exception ex)
            {
                _apiResult.Exception = ex.ToString();
            }
            return _apiResult;
        }
        public static APIResult callPostAPI(string apiUrl, List<APIParameters> apiParameters)
        {
            APIResult _apiResult = new APIResult();
            try
            {
                RestClient client = new RestClient(apiUrl);
                RestRequest request = new RestRequest(new Uri(String.Format("{0}", apiUrl)), Method.POST);
                request.RequestFormat = DataFormat.Json;
                if (apiParameters.Count > 0)
                {
                    foreach (var para in apiParameters)
                    {
                        if (apiParameters.Any(x => x.Type == APIParameterType.HttpHeader))
                        {
                            request.AddParameter(para.Key, para.Value.ToString(), ParameterType.HttpHeader);
                        }
                        else
                        {
                            request.AddParameter(para.Key, para.Value.ToString(), ParameterType.GetOrPost);
                        }
                    }
                }
                var apiResponse = client.Execute(request);
                _apiResult.Content = apiResponse.Content;
                _apiResult.ResponseStatusCode = apiResponse.StatusCode;
            }
            catch (Exception ex)
            {
                _apiResult.Exception = ex.ToString();
            }
            return _apiResult;
        }
    }
    public class APIResult
    {
        public string Content { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string Exception { get; set; }
    }

    public class APIParameters
    {
        public string Key { get; set; }
        public object Value { get; set; }
        // public object Data { get; set; }
        public APIParameterType Type { get; set; }
    }
    public enum APIParameterType : int
    {
        HttpHeader = 1,
        Body = 2,
        //QueryString = 3
    }
}
