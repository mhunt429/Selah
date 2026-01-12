using System.Text.Json;
using Domain.Events;
using Domain.Models.Plaid;

namespace Infrastructure.Extensions;

public static class PlaidHttpExtensions
{
    public static void ParseErrorResponse(
        this ConnectorDataSyncEvent syncEvent,
        string errorMessage)
    {
        syncEvent.Error = new PlaidApiErrorResponse();
        try
        {
            var rsp = JsonSerializer.Deserialize<PlaidApiErrorResponse>(errorMessage);

            if (rsp is null)
                return;

            syncEvent.Error.ErrorCode = rsp.ErrorCode;
            syncEvent.Error.ErrorMessage = rsp.ErrorMessage;
        }
        catch (Exception e)
        {
            syncEvent.Error.ErrorMessage = e.ToString();
        }
    }
}