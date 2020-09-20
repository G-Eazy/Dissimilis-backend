using System;

namespace Dissimilis.WebAPI.DTOs
{
    public class BarDTO : NewBarDTO
    {
        public int Id { get; set; }

        public BarDTO() { }

        public BarDTO(IBar bar)
        {
            if (bar is null)
                throw new ArgumentNullException(nameof(bar));

            this.Id = bar.Id;
            base.PartId = bar.PartId;
            base.BarNumber = bar.BarNumber;
            base.RepBefore = bar.RepBefore;
            base.RepAfter = bar.RepAfter;
            base.House = bar.House;
        }
    }
}
