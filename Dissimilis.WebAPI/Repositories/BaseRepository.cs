using Dissimilis.WebAPI.Database.Models;
using Dissimilis.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dissimilis.WebAPI.Repositories
{
    public class BaseRepository
    {
        /// <summary>
        /// Check if the user belongs to the entity it is trying to access/edit
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="song"></param>
        /// <returns></returns>

        public bool ValidateUser(uint userId, BaseEntity entity)
        {
            try
            {
                if (userId == entity.CreatedById)
                    return true;
                return false;
            }
            catch
            {
                throw new ArgumentException("The user is not allowed to edit on this song");
            }
        }
        public bool CheckProperties(SuperDTO obj)
        {
            Type t = obj.GetType();
            var properties = t.GetProperties();

            foreach(PropertyInfo p in properties)
            {
                Console.WriteLine(p.GetValue(obj));
                if (p.PropertyType == typeof(string))
                {
                    if (string.IsNullOrWhiteSpace((string)p.GetValue(obj)))
                        return false;
                }
                else if(p.PropertyType == typeof(int))
                {
                    if ((int)p.GetValue(obj) <= 0)
                        return false;
                }
                else if(p.PropertyType == typeof(byte))
                {
                    if ((byte)p.GetValue(obj) <= 0 && p.Name.ToLower() != "house")
                        return false;
                }
                else if (p.PropertyType.IsArray)
                {
                    if (string.IsNullOrWhiteSpace(p.GetValue(obj).ToString()))
                        return false;
                }
            }
            
            return true;
        } 
    }
}
