﻿namespace Dissimilis.WebAPI.Database.Models
{
    public interface IPart : IBaseEntity
    {
        int Id { get; }
        Instrument Instrument { get; }
        int InstrumentId { get; }
        byte PartNumber { get; }
        Song Song { get; }
        int SongId { get; }
    }
}