using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.DbContext.PagedExtensions;
using Microsoft.EntityFrameworkCore;

namespace Dissimilis.Core.Collections
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query, IPageSelectDto pageSelectDto, CancellationToken cancellationToken) where T : class
        {
            var queryResultCount = await query.CountAsync(cancellationToken);
            var result = new PagedResult<T> { CurrentPage = pageSelectDto.Page, PageSize = pageSelectDto.PageSize, RowCount = queryResultCount };

            if (pageSelectDto.PageSize > 0)
            {
                var pageCount = (double)result.RowCount / pageSelectDto.PageSize;
                result.PageCount = (int)Math.Ceiling(pageCount);

                var skip = (pageSelectDto.Page - 1) * pageSelectDto.PageSize;
                result.Results = await query.Skip(skip).Take(pageSelectDto.PageSize).ToListAsync(cancellationToken);
            }
            else
            {
                result.PageCount = 1;
                result.Results = await query.ToListAsync(cancellationToken);
            }


            return result;
        }


        public static PagedResult<T> GetPaged<T>(this T[] list, IPageSelectDto pageSelectDto) where T : class
        {
            var result = new PagedResult<T> { CurrentPage = pageSelectDto.Page, PageSize = pageSelectDto.PageSize, RowCount = list.ToArray().Count() };

            var pageCount = (double)result.RowCount / pageSelectDto.PageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (pageSelectDto.Page - 1) * pageSelectDto.PageSize;
            result.Results = list.ToList().Skip(skip).Take(pageSelectDto.PageSize).ToList();

            return result;
        }

        public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query, IPageSelectDto pageSelectDto, int rowCount, CancellationToken cancellationToken) where T : class
        {
            var result = new PagedResult<T> { CurrentPage = pageSelectDto.Page, PageSize = pageSelectDto.PageSize, RowCount = rowCount };

            var pageCount = (double)result.RowCount / pageSelectDto.PageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (pageSelectDto.Page - 1) * pageSelectDto.PageSize;
            result.Results = await query.Skip(skip).Take(pageSelectDto.PageSize).ToListAsync(cancellationToken);

            return result;
        }

        public static IQueryable<T> TakePage<T>(this IQueryable<T> query, IPageSelectDto pageSelectDto)
        {
            var skip = (pageSelectDto.Page - 1) * pageSelectDto.PageSize;
            return query.Skip(skip).Take(pageSelectDto.PageSize);
        }

        public static async Task<PagedResult<T>> ToPage<T>(this IQueryable<T> query, IPageSelectDto pageSelectDto, int rowCount, CancellationToken cancellationToken) where T : class
        {
            var result = new PagedResult<T> { CurrentPage = pageSelectDto.Page, PageSize = pageSelectDto.PageSize, RowCount = rowCount };

            var pageCount = (double)result.RowCount / pageSelectDto.PageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            result.Results = await query
                .TakePage(pageSelectDto)
                .ToListAsync(cancellationToken);

            return result;
        }


    }
}
