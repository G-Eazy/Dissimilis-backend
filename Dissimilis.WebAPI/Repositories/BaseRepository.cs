using System;

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

            //THIS NEEDS TO BE SET TO FALSE IN LATER VERSION
            /*
             At the moment all users can edit, delete and create new songs. This is
            because we want there to technically be "one" user, but for future use, this needs 
            to be changed to reflect some sort of authorisation
             */
            return true;
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
