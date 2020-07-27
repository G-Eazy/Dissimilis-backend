﻿namespace Dissimilis.WebAPI.Database.Models
{
    public interface IBar : IBaseEntity
    {
        ushort BarNumber { get; }
        byte? House { get; }
        int Id { get; }
        Part Part { get; }
        int PartId { get; }
        bool RepAfter { get; }
        bool RepBefore { get; }
    }
}