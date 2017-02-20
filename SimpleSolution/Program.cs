using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleSolution
{
	public static class TwoDStringArrayExtensions
	{
		//EXTENSION METHOD TO EASILY ACCESS DATA BY KEY
		public static string[] Get(this string[][] array, string key)
		{
			var keyData = (array == null || array.Length == 0) ? Array.Empty<string>() : array.FirstOrDefault(x => x[0] == key);
			return keyData == null ? Array.Empty<string>() : keyData.Skip(1).ToArray();
		}
	}

	//CLASS MADE PUBLIC FOR TESTABILITY
	public class Program
	{
		//METHOD MADE PUBLIC FOR TESTABILITY
		public static void Main(string[] args)
		{
			//LOAD DATA INTO READABLE FORMAT ONCE
			string[][] data = null;

			//ENSURE DATA IS LOADABLE AS REQUIRED
			if (!LoadCSVFile(Properties.Resources.data, out data)) return;

			//ENSURE DATA IS IN A FORMAT SUITABLE FOR THIS EXAMPLE
			if (!ContainsColumns(data, "FirstName", "LastName")) return;

			//SET FILENAMES
			string nameDataFileName = "NameData.txt";
			string addressDataFileName = "AddressData.txt";

			//EXTRACT NAME DATA ORDERED BY FREQUENCY THEN ALPHABETICALLY
			ExtractNameData(data, nameDataFileName);

			//EXTRACT ADDRESS DATA ORDERED ALPHABETICALLY
			ExtractAddressData(data, addressDataFileName);

			//OPEN FILES FOR DISPLAY PURPOSES
			System.Diagnostics.Process.Start(nameDataFileName);
			System.Diagnostics.Process.Start(addressDataFileName);
		}

		//A METHOD TO CONFIRM INPUT DATA HOLDS WHAT IS IN INTEREST
		public static bool ContainsColumns(string[][] data, params string[] columns)
		{
			return (columns != null && columns.All(x => data.Get(x).Length > 0));
		}

		//METHOD MADE PUBLIC FOR TESTABILITY
		public static void ExtractNameData(string[][] data, string fileName)
		{
			//GET ARRAY OF FIRSTNAMES AND LASTNAMES
			var values = data.Get("FirstName").Concat(data.Get("LastName"));

			//CREATE A STRING BUILDER FOR EASE OF STRING BUILDING AND MEMORY CONSUMPTION
			StringBuilder builder = new StringBuilder();

			//CYCLE THROUGH STRING LIST, GROUP, COUNT AND ORDER AND SELECT DESIRABLE OBJECT TYPE
			foreach (var result in (from val in values
									group val by val into g
									select new { Value = g.Key, Frequency = g.Count() }).OrderByDescending(x => x.Frequency).ThenBy(x => x.Value))
			{
				//APPEND LINE USING DESIRABLE OBJECT TYPE
				builder.AppendLine(string.Format("{0}, {1}", result.Value, result.Frequency));
			}

			//WRITE RESULTS TO FILE
			System.IO.File.WriteAllText(fileName, builder.ToString());
		}

		//METHOD MADE PUBLIC FOR TESTABILITY
		public static void ExtractAddressData(string[][] data, string fileName)
		{
			//GET ARRAY OF ADDRESSES
			var values = data.Get("Address");

			//CREATE A STRING BUILDER FOR EASE OF STRING BUILDING AND MEMORY CONSUMPTION
			StringBuilder builder = new StringBuilder();

			//CYCLE THROUGH STRING LIST, ORDER USING FIRST ALPHABETIC WORD AND SELECT RESULTS
			foreach (var result in (from val in values
									orderby Regex.Match(val, "[A-Za-z]+").Value
									select val))
			{
				//APPEND LINE USING ITEREE
				builder.AppendLine(result);
			}

			//WRITE RESULTS TO FILE
			System.IO.File.WriteAllText(fileName, builder.ToString());
		}

		//LOAD THE DATA 
		//ASSUMING DATA IS NOT EMPTY
		//USE LIGHTWEIGHT RETURN TYPE AS VESSEL
		//METHOD MADE PUBLIC FOR TESTABILITY
		public static bool LoadCSVFile(byte[] bytes, out string[][] results)
		{
			//ENSURE THERE IS DATA TO BE READ
			bool isValid = bytes != null && bytes.Length > 0;

			//ASSIGN DEFAULT OUTPUT
			results = new string[][] { };

			//DATA IS NOT NULL OR EMPTY
			if (isValid)
			{
				//PREPARE A LIST FOR EXTRACTED LINES
				List<string> lines = new List<string>();

				using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
				{
					using (System.IO.TextReader reader = new System.IO.StreamReader(ms))
					{
						//INITIATE THE READ PROCESS
						string header = reader.ReadLine();
						for (string line = header; !string.IsNullOrEmpty(line); line = reader.ReadLine())
						{
							//APPEND THE REST OF THE LINES
							lines.Add(line);
						}
					}
					//DONE WITH READER
				}
				//DONE WITH MEMORY STREAM

				//ENSURE READABLE DATA (HAS A HEADER ROW + MORE)
				isValid = lines.Count > 1;

				//DATA IS MADE UP OF LINES
				if (isValid)
				{
					//ENSURE DATA IS USABLE
					isValid = lines.All(x => x.IndexOf(',') > -1);

					//DATA IS IN CSV FORMAT USING COMMA AS DELIMETER
					if (isValid)
					{
						//GET THE HEADER LINE
						string headerLine = lines.First();

						//GET THE COLUMNS INTO A LIST FOR MINIMIZED OVERHEAD OF INDEXOF
						List<string> headerColumns = headerLine.Split(',').ToList();

						//GET ACTUAL DATA
						IEnumerable<string> data = lines.Skip(1);

						//RETURN DATA
						results = (from header in headerColumns
								   let headerIndex = headerColumns.IndexOf(header)
								   select new string[1] { header }.Concat(data.Select(x => x.Split(',')[headerIndex])).ToArray()).ToArray();
					}
				}
			}
			return isValid;
		}
	}
}
