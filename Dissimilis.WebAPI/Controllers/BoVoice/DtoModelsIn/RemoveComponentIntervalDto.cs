namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class RemoveComponentIntervalDto
    {
        public bool DeleteChordsOnLastIntervalRemoved { get; set; } = false;
        public int IntervalPosition { get; set; }
    }
}
