namespace Common.Api.Responses;

public sealed record ErrorResponse(string Message, string[]? Path = null);