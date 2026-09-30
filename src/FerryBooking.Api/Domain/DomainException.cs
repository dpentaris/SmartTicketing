namespace FerryBooking.Api.Domain;

/// <summary>
/// Thrown when an operation would violate a business rule.
/// Translated to an HTTP 422 response by <see cref="Common.DomainExceptionHandler"/>.
/// </summary>
public sealed class DomainException(string message) : Exception(message);
