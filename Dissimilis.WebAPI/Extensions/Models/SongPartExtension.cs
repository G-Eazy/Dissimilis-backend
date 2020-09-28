using System.Linq;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.WebAPI.Extensions.Models
{
    public static class SongPartExtension
    {
        public static void SortBars(this SongVoice songVoice)
        {
            var number = 1;
            var orderedList = songVoice.SongBars.OrderBy(b => b.BarNumber);
            foreach (var songBar in orderedList)
            {
                songBar.BarNumber = number++;
            }
        }
    }
}
