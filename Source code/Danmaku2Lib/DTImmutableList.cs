
namespace Danmaku2Lib
{
	using System.Collections.Generic;

	public class DTImmutableList<T>
	{
		private List<T> list;
		private int count;

		public static DTImmutableList<T> AsImmutableList(List<T> l)
		{
			var immutableList = new DTImmutableList<T>();
			immutableList.list = l;
			immutableList.count = l.Count;
			return immutableList;
		}
		
		private DTImmutableList()
		{
		}
		
		public DTImmutableList(List<T> list)
		{
			this.list = new List<T>(capacity: list.Count);
			foreach (T item in list)
			{
				this.list.Add(item);
			}
			this.count = list.Count;
		}

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}
	}
}
