namespace LT.DigitalOffice.RightsService.Models.Dto.Models
{
  public record RightInfo
  {
    public int RightId { get; set; }
    public string Locale { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
  }
}
