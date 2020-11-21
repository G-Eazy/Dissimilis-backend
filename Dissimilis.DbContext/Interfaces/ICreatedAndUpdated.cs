using System;
using Dissimilis.DbContext.Models;

namespace Dissimilis.DbContext.Interfaces
{
    public interface ICreatedAndUpdated
    {
        User CreatedBy { get; set; }
        int? CreatedById { get; set; }
        DateTimeOffset? CreatedOn { get; set; }
        User UpdatedBy { get; set; }
        int? UpdatedById { get; set; }
        DateTimeOffset? UpdatedOn { get; set; }
    }
}