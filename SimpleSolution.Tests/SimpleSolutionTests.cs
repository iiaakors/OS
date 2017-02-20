using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleSolution.Tests
{
	[TestClass]
	public class SimpleSolutionTests
	{
		private const string testData = @"FirstName,LastName,Address,PhoneNumber\n
										Jimmy,Smith,102 Long Lane,29384857\n
										Clive,Owen,65 Ambling Way,31214788\n
										James,Brown,82 Stewart St,32114566\n
										Graham,Howe,12 Howard St,8766556\n
										John,Howe,78 Short Lane,29384857\n
										Clive,Smith,49 Sutherland St,31214788\n
										James,Owen,8 Crimson Rd,32114566\n
										Graham,Brown,94 Roland St,8766556\n";

		//ENSURE ARRAY EXTENSION GET METHOD RETURNS EMPTY IF NULL OR KEY NOT FOUND
		[TestMethod]
		public void TestArrayExtensionReturnsEmptyIfKeyNotFoundOrParameterNull()
		{
			string[][] candidate = null;
			Assert.IsTrue(SimpleSolution.TwoDStringArrayExtensions.Get(candidate, "asdasd").Length == 0);

			candidate = new string[][] { new string[1] { "MyKey" } };
			Assert.IsTrue(SimpleSolution.TwoDStringArrayExtensions.Get(candidate, "asdasd").Length == 0);
		}

		//ENSURE ARRAY EXTENSION GET METHOD RETURNS RESULT IF KEY FOUND
		[TestMethod]
		public void TestArrayExtensionReturnsResultWhenKeyFound()
		{
			string[][] candidate = null;
			
			candidate = new string[][] { new string[2] { "MyKey", "SomeData" } };
			
			Assert.IsTrue(SimpleSolution.TwoDStringArrayExtensions.Get(candidate, "MyKey").Length >0);
		}


		//ENSURE LoadCSVFile RETURNS FALSE ON ATTEMPT LOAD INVALID DATA
		[TestMethod]
		public void TestLoadCSVFileReturnsFalseWhenInvalid()
		{
			byte[] data = null;
			string[][] output = null;
			Assert.IsFalse(SimpleSolution.Program.LoadCSVFile(data, out output));

			string someInputString = "Some Non Comma Delimited Text";
			data = System.Text.Encoding.ASCII.GetBytes(someInputString);
			Assert.IsFalse(SimpleSolution.Program.LoadCSVFile(data, out output));

			someInputString = "Some Comma, Separated\nText Without All Lines\nBeing Comma, Separated";
			data = System.Text.Encoding.ASCII.GetBytes(someInputString);
			Assert.IsFalse(SimpleSolution.Program.LoadCSVFile(data, out output));
		}

		//ENSURE LoadCSVFile RETURNS TRUE AND RESULTS HAVE VALUE ON ATTEMPT LOAD VALID DATA
		[TestMethod]
		public void TestLoadCSVFileReturnsTrueAndOutPutHasValueWhenValid()
		{
			byte[] data = null;
			string[][] output = null;
			string someInputString = testData;
			data = System.Text.Encoding.ASCII.GetBytes(someInputString);

			Assert.IsTrue(SimpleSolution.Program.LoadCSVFile(data, out output));
			Assert.IsTrue(output != null);
			Assert.IsTrue(output.Length > 0);
			Assert.IsTrue(output[0].Length > 0);
		}

		//ENSURE COLUMN TESTING METHOD RETURNS FALSE WHEN NOT ALL INPUTS MATCH DATA
		[TestMethod]
		public void TestContainsColumnsReturnsFalseWhenInValid()
		{
			byte[] data = System.Text.Encoding.ASCII.GetBytes(testData);
			string[][] output = null;
			SimpleSolution.Program.LoadCSVFile(data, out output);
			
			Assert.IsFalse(SimpleSolution.Program.ContainsColumns(output, "SomeNonExistantColumn"));
			Assert.IsFalse(SimpleSolution.Program.ContainsColumns(output, "FirstName", "SomeNonExistantColumn"));
		}

		//ENSURE COLUMN TESTING METHOD RETURNS TRUE WHEN ALL INPUTS MATCH DATA
		[TestMethod]
		public void TestContainsColumnsReturnsTrueWhenValid()
		{
			byte[] data = System.Text.Encoding.ASCII.GetBytes(testData);
			string[][] output = null;
			SimpleSolution.Program.LoadCSVFile(data, out output);
			Assert.IsTrue(SimpleSolution.Program.ContainsColumns(output, "FirstName"));
			Assert.IsTrue(SimpleSolution.Program.ContainsColumns(output, "FirstName", "LastName"));
		}
	}
}
