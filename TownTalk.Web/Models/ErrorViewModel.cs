namespace TownTalk.Web.Models;

/// <summary>
/// ViewModel for representing error information in the application.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the unique identifier for the request.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the RequestId should be shown.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
