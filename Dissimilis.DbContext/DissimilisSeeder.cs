using System.Linq;
using Dissimilis.DbContext.Models;
using Dissimilis.DbContext.Models.Song;

namespace Dissimilis.DbContext
{
    public static class DissimilisSeeder
    {
        public static void SeedBasicData(DissimilisDbContext context)
        {
            context.Database.EnsureCreated();

            var user = context.Users.SingleOrDefault(x => x.Name == "AdminUser");
            if (user is null)
            {
                context.Users.Add(new User() { Name = "AdminUser", Email = "admin@support.no" });
            }

            context.SaveChanges();


            var organisation = context.Organisations.SingleOrDefault(x => x.Name == "Dissimilis Norge");
            if (organisation is null)
            {
                context.Organisations.Add(new Organisation("Ukjent"));
                context.Organisations.Add(new Organisation("Dissimilis Norge"));
                context.Organisations.Add(new Organisation("Dissimilis Kultursenter"));
            }

            var countryNorway = context.Countries.SingleOrDefault(x => x.Name == "Norge");
            if (countryNorway is null)
            {
                context.Countries.Add(new Country("Norge"));
                context.Countries.Add(new Country("Sverige"));
            }

            var instrument = context.Instruments.SingleOrDefault(x => x.Name == "Piano");
            if (instrument is null)
            {
                context.Instruments.Add(new Instrument("Piano"));
                context.Instruments.Add(new Instrument("Gitar"));
                context.Instruments.Add(new Instrument("Bass"));
                context.Instruments.Add(new Instrument("Partitur"));
            }


            context.SaveChanges();

            var userAdmin = context.Users.FirstOrDefault(x => x.Name == "AdminUser");
            if (userAdmin?.OrganisationId is null)
            {
                userAdmin.OrganisationId = 1;
                userAdmin.CountryId = 1;
            }


            context.SaveChanges();


            var firstSong = context.Songs.FirstOrDefault(x => x.Title == "Lisa Gikk Til Skolen");
            if (firstSong is null)
            {
                context.Songs.Add(new Song() { Title = "Lisa Gikk Til Skolen", Composer = "Unknown", ArrangerId = 1, Numerator = 4, Denominator = 4 });
                context.Songs.Add(new Song() { Title = "Fade To Black", Composer = "Metallica", ArrangerId = 1, Numerator = 4, Denominator = 4 });
                context.Songs.Add(new Song() { Title = "Be Yourself", Composer = "Audioslave", ArrangerId = 1, Numerator = 4, Denominator = 4 });

                context.SaveChanges();

                context.SongVoices.Add(new SongVoice() { InstrumentId = 4, SongId = 1, VoiceNumber = 1 });
                context.SongVoices.Add(new SongVoice() { InstrumentId = 4, SongId = 2, VoiceNumber = 1 });
                context.SongVoices.Add(new SongVoice() { InstrumentId = 4, SongId = 3, VoiceNumber = 1 });
            }

            context.SaveChanges();

        }
    }
}
