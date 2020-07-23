using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http.Validation;

namespace Dissimilis.WebAPI.Repositories
{
    public class BaseRepository
    {
        /// <summary>
        /// Check if the user belongs to the entity it is trying to access/edit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="entity"></param>
        /// <returns></returns>

        public bool ValidateUser(uint userId, BaseEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (userId == entity.CreatedById)
                return true;
            return false;
        }

        /// <summary>
        /// Generic function that has multiple implementations
        /// Checks for default and invalid incoming DTO property values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValidator"></typeparam>
        /// <param name="dto"></param>
        /// <returns>bool</returns>
        public static bool IsValidDTO<T, TValidator>(T dto)
            where TValidator : class, IValidator<T>, new()
        {
            return new TValidator().IsValid(dto);
        }
    }

}
