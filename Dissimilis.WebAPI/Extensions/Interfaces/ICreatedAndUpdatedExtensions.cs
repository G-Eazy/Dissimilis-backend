using System;
using Dissimilis.DbContext.Interfaces;
using Dissimilis.DbContext.Models;

namespace Dissimilis.WebAPI.Extensions.Interfaces
{
    public static class CreatedAndUpdatedExtensions
    {
        public static void SetCreated(this ICreatedAndUpdated item, int? userId)
        {
            item.CreatedById = userId;
            item.CreatedOn = DateTimeOffset.Now;

            item.UpdatedById = userId;
            item.UpdatedOn = DateTimeOffset.Now;
        }

        public static void SetUpdated(this ICreatedAndUpdated item, int? userId)
        {
            item.UpdatedById = userId;
            item.UpdatedOn = DateTimeOffset.Now;
       }

        public static void SetUpdated(this ICreatedAndUpdated item, User user)
        {
            item.SetUpdated(user.Id);
        }
    }
}
