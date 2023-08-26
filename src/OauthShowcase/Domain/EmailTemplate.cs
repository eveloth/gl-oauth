namespace OauthShowcase.Domain;

public sealed record EmailTemplate(string Name, string Template)
{
    public int Id { get; set; }
    public string Name { get; set; } = Name;
    public string Template { get; set; } = Template;
}