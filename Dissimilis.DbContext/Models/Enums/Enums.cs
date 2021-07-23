using System.ComponentModel;

namespace Dissimilis.DbContext.Models.Enums
{
    /// <summary>
    /// The enum containing the different roles in the system, used in GroupUser and OrganisationUser
    /// </summary>
    public enum Role
    {
        Member =10,
        Admin = 20
    }

    public enum ProtectionLevels
    {
        [Description("Private")]
        Private = 10,
        [Description("Public")]
        Public = 20
    }

    public enum Operation
    {
        [Description("Create")]
        Create = 0,

        [Description("Modify")]
        Modify = 10,

        [Description("Delete")]
        Delete = 20,

        [Description("Invite")]
        Invite = 30,

        [Description("Kick")]
        Kick = 40,

        [Description("Get")]
        Get = 50
    }

    public enum DefinedInstruments
    {
        [Description("Accordion")]
        Accordion = 0,

        [Description("Acoustic Guitar")]
        Acoustic_Guitar = 10,

        [Description("Bagpipes")]
        Bagpipes = 20,

        [Description("Banjo")]
        Banjo = 30,

        [Description("Bass Guitar")]
        Bass_Guitar = 40,

        [Description("Bongo Drums")]
        Bongo_Drums = 50,

        [Description("Bugle")]
        Bugle = 60,

        [Description("Cello")]
        Cello = 70,

        [Description("Clarinet")]
        Clarinet = 80,

        [Description("Cymbals")]
        Cymbals = 90,

        [Description("Drums")]
        Drums = 100,

        [Description("Electric Guitar")]
        Electric_Guitar = 110,

        [Description("Flute")]
        Flute = 120,

        [Description("French Horn")]
        French_Horn = 130,

        [Description("Harmonica")]
        Harmonica = 140,

        [Description("Keyboard")]
        Keyboard = 150,

        [Description("Maracas")]
        Maracas = 160,

        [Description("Organ")]
        Organ = 170,

        [Description("Pan Flute")]
        Pan_Flute = 180,

        [Description("Piano")]
        Piano = 190,

        [Description("Recorder")]
        Recorder = 200,

        [Description("Saxophone")]
        Saxophone = 210,

        [Description("Tambourine")]
        Tambourine = 220,

        [Description("Triangle")]
        Triangle = 230,

        [Description("Trombone")]
        Trombone = 240,

        [Description("Trumpet")]
        Trumpet = 250,

        [Description("Tuba")]
        Tuba = 260,

        [Description("Ukulele")]
        Ukulele = 270,

        [Description("Violin")]
        Violin = 280,

        [Description("Xylophone")]
        Xylophone = 290
    }
}
