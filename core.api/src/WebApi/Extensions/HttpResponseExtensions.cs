using System.Net;
using Domain.ApiContracts;

namespace WebApi.Extensions;

public static class HttpResponseExtensions
{
    public static BaseHttpResponse<T> ToBaseHttpResponse<T>(this T data, HttpStatusCode httpStatusCode,
        List<string>? errors = null)
    {
        return new BaseHttpResponse<T>
        {
            StatusCode = (int)httpStatusCode,
            Data = data,
            Errors = errors
        };
    }
}