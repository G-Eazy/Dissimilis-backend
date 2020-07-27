namespace Dissimilis.WebAPI.Database.Models
{
    public interface INote : IBaseEntity
    {
        Bar Bar { get; }
        int BarId { get; }
        int Id { get; }
        byte Length { get; }
        ushort NoteNumber { get; }
        string[] NoteValues { get; }
    }
}