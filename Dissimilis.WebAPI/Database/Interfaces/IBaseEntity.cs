using System;

namespace Dissimilis.WebAPI.Database.Models
{
    public interface IBaseEntity
    {
        User CreatedBy { get; set; }
        int? CreatedById { get; set; }
        DateTime? CreatedOn { get; set; }
        User UpdatedBy { get; set; }
        int? UpdatedById { get; set; }
        DateTime? UpdatedOn { get; set; }
    }
}