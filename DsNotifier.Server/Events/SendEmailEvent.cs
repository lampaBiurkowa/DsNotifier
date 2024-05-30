class SendEmailEvent
{
    public required string BodyHtml { get; set; }
    public required string RecipentEmail { get; set; }
}