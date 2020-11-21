using System;
using System.Collections.Generic;

namespace Dissimilis.Core.Collections
{
	public abstract class PagedResultBaseDto
	{
		public int CurrentPage { get; set; }
		public int PageCount { get; set; }
		public int PageSize { get; set; }
		public int RowCount { get; set; }

		public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;

        public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);
    }

	public class PagedResult<T> : PagedResultBaseDto where T : class
	{
		public IList<T> Results { get; set; } = new List<T>();
    }
}
