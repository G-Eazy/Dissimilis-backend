using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dissimilis.DbContext.PagedExtensions
{
    public interface IPageSelectDto
    {
        [Range(1, 1000)]
        public int PageSize { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; }
    }
}
