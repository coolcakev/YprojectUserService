namespace YprojectUserService.Razor.Models;

// TODO check razor as tamplate
public class EmailModel
{
    public string Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Body { get; set; }
    public string? Code { get; set; }
    public string SupportEmail { get; } = "support@y-project.com";
}