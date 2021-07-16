namespace LT.DigitalOffice.RightsService.Models.Dto.Responses
{
    public record RightResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}