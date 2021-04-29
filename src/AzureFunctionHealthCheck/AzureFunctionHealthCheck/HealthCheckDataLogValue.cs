using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaleLearnCode.AzureFunctionHealthCheck
{

	/// <summary>
	/// Represents value of a health check data log.
	/// </summary>
	/// <seealso cref="IReadOnlyList{KeyValuePair{string, object}}" />
	internal class HealthCheckDataLogValue : IReadOnlyList<KeyValuePair<string, object>>
	{
		private readonly string _name;
		private readonly List<KeyValuePair<string, object>> _values;
		private string _formatted;

		/// <summary>
		/// Initializes a new instance of the <see cref="HealthCheckDataLogValue"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="values">The values.</param>
		public HealthCheckDataLogValue(string name, IReadOnlyDictionary<string, object> values)
		{
			_name = name;
			_values = values.ToList();
			_values.Add(new KeyValuePair<string, object>("HealthCheckName", name));
		}

		/// <summary>
		/// Gets the <see cref="KeyValuePair{string, Object}"/> at the specified index.
		/// </summary>
		/// <value>
		/// The <see cref="KeyValuePair{String, Object}"/>.
		/// </value>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException">index</exception>
		public KeyValuePair<string, object> this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
					throw new IndexOutOfRangeException(nameof(index));
				return _values[index];
			}
		}

		public int Count => _values.Count;

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An enumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		/// <summary>
		/// Converts the <see cref="HealthCheckDataLogValue"/> to a string.
		/// </summary>
		/// <returns>
		/// A <see cref="String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if (_formatted == null)
			{
				var builder = new StringBuilder();
				builder.AppendLine($"Health check data for {_name}:");

				var values = _values;
				for (var i = 0; i < values.Count; i++)
				{
					var kvp = values[i];
					builder.Append("    ");
					builder.Append(kvp.Key);
					builder.Append(": ");

					builder.AppendLine(kvp.Value?.ToString());
				}

				_formatted = builder.ToString();
			}

			return _formatted;
		}

	}

}