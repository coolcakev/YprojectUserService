namespace YprojectUserService.Razor.Models;

public class EmailButton
{
    public string Text { get; set; }
    public string Link { get; set;  }
}
public class EmailModel
{
    public string Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Body { get; set; }
    public EmailButton? EmailButton { get; set; } 
    public string? Code { get; set; }
    public string SupportEmail { get; } = "support@y-project.com";
    public string TermsOfUseUrl { get; } = "http://example.com";
    public string PrivacyPolicyUrl { get; } = "http://example.com";
}