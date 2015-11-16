//********************************************************
//*                                                      *
//*   Copyright (C) Microsoft. All rights reserved.      *
//*                                                      *
//********************************************************
//////////////////////////////////////////////////////////////////////////////
//    Command Line Argument Parser
//    ----------------------------
//
//    Author: peterhal@microsoft.com
//
//    Shared Source License for Command Line Parser Library
//
//    This license governs use of the accompanying software ('Software'), and your
//    use of the Software constitutes acceptance of this license.
//
//    You may use the Software for any commercial or noncommercial purpose,
//    including distributing derivative works.
//
//    In return, we simply require that you agree:
//
//    1. Not to remove any copyright or other notices from the Software. 
//    2. That if you distribute the Software in source code form you do so only
//    under this license (i.e. you must include a complete copy of this license
//    with your distribution), and if you distribute the Software solely in
//    object form you only do so under a license that complies with this
//    license.
//    3. That the Software comes "as is", with no warranties. None whatsoever.
//    This means no express, implied or statutory warranty, including without
//    limitation, warranties of merchantability or fitness for a particular
//    purpose or any warranty of title or non-infringement. Also, you must pass
//    this disclaimer on whenever you distribute the Software or derivative
//    works.
//    4. That no contributor to the Software will be liable for any of those types
//    of damages known as indirect, special, consequential, or incidental
//    related to the Software or this license, to the maximum extent the law
//    permits, no matter what legal theory it’s based on. Also, you must pass
//    this limitation of liability on whenever you distribute the Software or
//    derivative works.
//    5. That if you sue anyone over patents that you think may apply to the
//    Software for a person's use of the Software, your license to the Software
//    ends automatically.
//    6. That the patent rights, if any, granted in this license only apply to the
//    Software, not to any derivative works you make.
//    7. That the Software is subject to U.S. export jurisdiction at the time it
//    is licensed to you, and it may be subject to additional export or import
//    laws in other places.  You agree to comply with all such laws and
//    regulations that may apply to the Software after delivery of the software
//    to you.
//    8. That if you are an agency of the U.S. Government, (i) Software provided
//    pursuant to a solicitation issued on or after December 1, 1995, is
//    provided with the commercial license rights set forth in this license,
//    and (ii) Software provided pursuant to a solicitation issued prior to
//    December 1, 1995, is provided with “Restricted Rights” as set forth in
//    FAR, 48 C.F.R. 52.227-14 (June 1987) or DFAR, 48 C.F.R. 252.227-7013
//    (Oct 1988), as applicable.
//    9. That your rights under this License end automatically if you breach it in
//    any way.
//    10.That all rights not expressly granted to you in this license are reserved.
//
//    Usage
//    -----
//
//    Parsing command line arguments to a console application is a common problem. 
//    This library handles the common task of reading arguments from a command line 
//    and filling in the values in a type.
//
//    To use this library, define a class whose fields represent the data that your 
//    application wants to receive from arguments on the command line. Then call 
//    CommandLine.ParseArguments() to fill the object with the data 
//    from the command line. Each field in the class defines a command line argument. 
//    The type of the field is used to validate the data read from the command line. 
//    The name of the field defines the name of the command line option.
//
//    The parser can handle fields of the following types:
//
//    - string
//    - int
//    - uint
//    - bool
//    - enum
//    - array of the above type
//
//    For example, suppose you want to read in the argument list for wc (word count). 
//    wc takes three optional boolean arguments: -l, -w, and -c and a list of files.
//
//    You could parse these arguments using the following code:
//
//    class WCArguments
//    {
//        public bool lines;
//        public bool words;
//        public bool chars;
//        public string[] files;
//    }
//
//    class WC
//    {
//        static void Main(string[] args)
//        {
//            if (CommandLine.ParseArgumentsWithUsage(args, parsedArgs))
//            {
//            //     insert application code here
//            }
//        }
//    }
//
//    So you could call this aplication with the following command line to count 
//    lines in the foo and bar files:
//
//        wc.exe /lines /files:foo /files:bar
//
//    The program will display the following usage message when bad command line 
//    arguments are used:
//
//        wc.exe -x
//
//    Unrecognized command line argument '-x'
//        /lines[+|-]                         short form /l
//        /words[+|-]                         short form /w
//        /chars[+|-]                         short form /c
//        /files:<string>                     short form /f
//        @<file>                             Read response file for more options
//
//    That was pretty easy. However, you realy want to omit the "/files:" for the 
//    list of files. The details of field parsing can be controled using custom 
//    attributes. The attributes which control parsing behaviour are:
//
//    ArgumentAttribute 
//        - controls short name, long name, required, allow duplicates, default value
//        and help text
//    DefaultArgumentAttribute 
//        - allows omition of the "/name".
//        - This attribute is allowed on only one field in the argument class.
//
//    So for the wc.exe program we want this:
//
//    using System;
//    using Utilities;
//
//    class WCArguments
//    {
//        [Argument(ArgumentTypes.AtMostOnce, HelpText="Count number of lines in the input text.")]
//        public bool lines;
//        [Argument(ArgumentTypes.AtMostOnce, HelpText="Count number of words in the input text.")]
//        public bool words;
//        [Argument(ArgumentTypes.AtMostOnce, HelpText="Count number of chars in the input text.")]
//        public bool chars;
//        [DefaultArgument(ArgumentTypes.MultipleUnique, HelpText="Input files to count.")]
//        public string[] files;
//    }
//
//    class WC
//    {
//        static void Main(string[] args)
//        {
//            WCArguments parsedArgs = new WCArguments();
//            if (CommandLine.ParseArgumentsWithUsage(args, parsedArgs))
//            {
//            //     insert application code here
//            }
//        }
//    }
//
//
//
//    So now we have the command line we want:
//
//        wc.exe /lines foo bar
//
//    This will set lines to true and will set files to an array containing the 
//    strings "foo" and "bar".
//
//    The new usage message becomes:
//
//        wc.exe -x
//
//    Unrecognized command line argument '-x'
//    /lines[+|-]  Count number of lines in the input text. (short form /l)
//    /words[+|-]  Count number of words in the input text. (short form /w)
//    /chars[+|-]  Count number of chars in the input text. (short form /c)
//    @<file>      Read response file for more options
//    <files>      Input files to count. (short form /f)
//
//    If you want more control over how error messages are reported, how /help is 
//    dealt with, etc you can instantiate the CommandLine.Parser class.
//
//
//
//    Cheers,
//    Peter Hallam
//    C# Compiler Developer
//    Microsoft Corp.
//
//
//
//
//    Release Notes
//    -------------
//
//    10/02/2002 Initial Release
//    10/14/2002 Bug Fix
//    01/08/2003 Bug Fix in @ include files
//    10/23/2004 Added user specified help text, formatting of help text to 
//            screen width. Added ParseHelp for /?.
//    11/23/2004 Added support for default values.
//    07/24/2011 Added support for bool default values.
//////////////////////////////////////////////////////////////////////////////
namespace CommandLineParser
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Collections;
	using System.IO;
	using System.Text;
	using System.Runtime.InteropServices;
	using System.Threading;

	/// <summary>
	/// Used to control parsing of command line arguments.
	/// </summary>
	[Flags]    
	public enum ArgumentTypes
	{
		/// <summary>
		/// Indicates that this field is required. An error will be displayed
		/// if it is not present when parsing arguments.
		/// </summary>
		Required    = 0x01,
		/// <summary>
		/// Only valid in conjunction with Multiple.
		/// Duplicate values will result in an error.
		/// </summary>
		Unique      = 0x02,
		/// <summary>
		/// Inidicates that the argument may be specified more than once.
		/// Only valid if the argument is a collection
		/// </summary>
		Multiple    = 0x04,

		/// <summary>
		/// The default type for non-collection arguments.
		/// The argument is not required, but an error will be reported if it is specified more than once.
		/// </summary>
		AtMostOnce  = 0x08,

		/// <summary>
		/// For non-collection arguments, when the argument is specified more than
		/// once no error is reported and the value of the argument is the last
		/// value which occurs in the argument list.
		/// </summary>
		LastOccurrenceWins = Multiple,

		/// <summary>
		/// The default type for collection arguments.
		/// The argument is permitted to occur multiple times, but duplicate 
		/// values will cause an error to be reported.
		/// </summary>
		MultipleUnique  = Multiple | Unique,
	}

	/// <summary>
	/// Allows control of command line parsing.
	/// Attach this attribute to instance fields of types used
	/// as the destination of command line argument parsing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813", Justification="Need DefaultArgumentAttribute to derive from ArgumentAttibute and it cannot derive from a sealed class")]
	public class ArgumentAttribute : Attribute
	{
		/// <summary>
		/// Allows control of command line parsing.
		/// </summary>
		/// <param name="type"> Specifies the error checking to be done on the argument. </param>
		public ArgumentAttribute(ArgumentTypes type)
		{
			this.type = type;
		}

		/// <summary>
		/// The error checking to be done on the argument.
		/// </summary>
		public ArgumentTypes Type
		{
			get { return this.type; }
		}
		/// <summary>
		/// Returns true if the argument did not have an explicit short name specified.
		/// </summary>
		public bool DefaultShortName    { get { return null == this.shortName; } }

		/// <summary>
		/// The short name of the argument.
		/// Set to null means use the default short name if it does not
		/// conflict with any other parameter name.
		/// Set to String.Empty for no short name.
		/// This property should not be set for DefaultArgumentAttributes.
		/// </summary>
		public string ShortName
		{
			get { return this.shortName; }
			set { Debug.Assert(value == null || !(this is DefaultArgumentAttribute)); this.shortName = value; }
		}

		/// <summary>
		/// Returns true if the argument did not have an explicit long name specified.
		/// </summary>
		public bool DefaultLongName     { get { return null == this.longName; } }

		/// <summary>
		/// The long name of the argument.
		/// Set to null means use the default long name.
		/// The long name for every argument must be unique.
		/// It is an error to specify a long name of String.Empty.
		/// </summary>
		public string LongName
		{
			get { Debug.Assert(!this.DefaultLongName); return this.longName; }
			set { Debug.Assert(!string.IsNullOrEmpty(value)); this.longName = value; }
		}

		/// <summary>
		/// The default value of the argument.
		/// </summary>
		public object DefaultValue
		{
			get 
			{
				if ( this.defaultValue.GetType() == typeof( string ) )
				{
					return Environment.ExpandEnvironmentVariables( this.defaultValue.ToString() );
				}

				return this.defaultValue; 
			}
			set
			{
				this.defaultValue = value;
			}
		}

		/// <summary>
		/// Returns true if the argument has a default value.
		/// </summary>
		public bool HasDefaultValue     { get { return null != this.defaultValue; } }

		/// <summary>
		/// Returns true if the argument has help text specified.
		/// </summary>
		public bool HasHelpText         { get { return null != this.helpText; } }

		/// <summary>
		/// The help text for the argument.
		/// </summary>
		public string HelpText
		{
			get { return this.helpText; }
			set { this.helpText = value; }
		}

		private string shortName;
		private string longName;
		private string helpText;
		private object defaultValue;
		private ArgumentTypes type;
	}

	/// <summary>
	/// Indicates that this argument is the default argument.
	/// '/' or '-' prefix only the argument value is specified.
	/// The ShortName property should not be set for DefaultArgumentAttribute
	/// instances. The LongName property is used for usage text only and
	/// does not affect the usage of the argument.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	sealed internal class DefaultArgumentAttribute : ArgumentAttribute
	{
		/// <summary>
		/// Indicates that this argument is the default argument.
		/// </summary>
		/// <param name="type"> Specifies the error checking to be done on the argument. </param>
		public DefaultArgumentAttribute(ArgumentTypes type)
			: base (type)
		{
		}
	}

	/// <summary>
	/// A delegate used in error reporting.
	/// </summary>
	internal delegate void ErrorReporter(string message);

	static class NativeMethods
	{
		[DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", EntryPoint = "GetConsoleScreenBufferInfo", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);
	}

	struct CONSOLE_SCREEN_BUFFER_INFO
	{
		internal COORD dwSize;
		internal COORD dwCursorPosition;
		internal Int16 wAttributes;
		internal SMALL_RECT srWindow;
		internal COORD dwMaximumWindowSize;
	}

	struct COORD
	{
		internal Int16 x;
		internal Int16 y;
	}

	struct SMALL_RECT
	{
		internal Int16 Left;
		internal Int16 Top;
		internal Int16 Right;
		internal Int16 Bottom;
	}

	/// <summary>
	/// Parser for command line arguments.
	///
	/// The parser specification is infered from the instance fields of the object
	/// specified as the destination of the parse.
	/// Valid argument types are: int, uint, string, bool, enums, dateTime
	/// Also argument types of Array of the above types are also valid.
	/// 
	/// Error checking options can be controlled by adding a ArgumentAttribute
	/// to the instance fields of the destination object.
	///
	/// At most one field may be marked with the DefaultArgumentAttribute
	/// indicating that arguments without a '-' or '/' prefix will be parsed as that argument.
	///
	/// If not specified then the parser will infer default options for parsing each
	/// instance field. The default long name of the argument is the field name. The
	/// default short name is the first character of the long name. Long names and explicitly
	/// specified short names must be unique. Default short names will be used provided that
	/// the default short name does not conflict with a long name or an explicitly
	/// specified short name.
	///
	/// Arguments which are array types are collection arguments. Collection
	/// arguments can be specified multiple times.
	/// </summary>
	/// <example>Using CommandLine Parser
	/// <code lang="C#">
	/// 
	/// //Add a description for the exe to be displayed in the usage
	/// [assembly: AssemblyDescription("Tests the ultra cool feature foobaz")]
	/// namespace myNamespace
	/// {
	///   //Create a class that will hold your parameters
	///   class MyParameters
	///   {
	///     //Add an attribute to each variable to mark it as an input parameter
	///     [ArgumentAttribute(ArgumentTypes.AtMostOnce, HelpText="File Name to write log to")]
	///     public string LogName = null;
	///   
	///     [ArgumentAttribute(ArgumentTypes.Required, HelpText="Test case ID to execute")]
	///     public int TestCaseID=1; //Specify any default value using 1 in this case
	/// 
	///     [ArgumentAttribute(ArgumentTypes.Required | ArgumentTypes.Multiple, HelpText="List of input files to read from")]
	///     public string[] InputFile = null;
	///   }
	/// 
	///   class MyTest
	///   {
	///     int Main(string[] args)
	///     {
	///       MyParmeters params = new MyParameters();
	///       if( !Parser.ParseArgumentsWithUsage(args, params) )
	///       {
	///         //ParseArgumentsWithUsage returned false, so either there was an error or we just displayed the usage
	///         return 1;
	///       }
	/// 
	///       //Once here our parameters are ready for use
	///       Log.Open(params.LogFile);
	///       Log.Info("Running test case: " + params.TestCaseID);
	/// 
	///       //If you want to display the inputs to a test you can just call the following
	///       Parser.DisplayInputParameters(params);
	/// 
	///       //Rest of test code
	///       ...
	/// 
	///       return 0;
	///     }
	///   }
	/// }
	/// </code>
	/// </example>
	/// 
	public sealed class Parser
	{
		/// <summary>
		/// The System Defined new line string.
		/// </summary>
		private const string NewLine = "\r\n";

		/// <summary>
		/// Don't ever call this.
		/// </summary>
		private Parser() { }

		/// <summary>
		/// Parses Command Line Arguments. Displays usage message to Console.Out
		/// if /?, /help or invalid arguments are encounterd.
		/// Errors are output on Console.Error.
		/// Use ArgumentAttributes to control parsing behaviour.
		/// </summary>
		/// <param name="arguments"> The actual arguments. </param>
		/// <param name="destination"> The resulting parsed arguments. </param>
		/// <returns> true if no errors were detected. </returns>
		public static bool ParseArgumentsWithUsage(string [] arguments, object destination)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			if (Parser.ParseHelp(arguments) || !Parser.ParseArguments(arguments, destination))
			{
				// error encountered in arguments. Display usage message
				System.Console.Write(Parser.ArgumentsUsage(destination.GetType()));
				return false;
			}

			return true;
		}

		/// <summary>
		/// Parses Command Line Arguments. 
		/// Errors are output on Console.Error.
		/// Use ArgumentAttributes to control parsing behaviour.
		/// </summary>
		/// <param name="arguments"> The actual arguments. </param>
		/// <param name="destination"> The resulting parsed arguments. </param>
		/// <returns> true if no errors were detected. </returns>
		public static bool ParseArguments(string [] arguments, object destination)
		{
			return Parser.ParseArguments(arguments, destination, new ErrorReporter(ConsoleReporter.WriterError));
		}

		/// <summary>
		/// Parses Command Line Arguments. 
		/// Use ArgumentAttributes to control parsing behaviour.
		/// </summary>
		/// <param name="arguments"> The actual arguments. </param>
		/// <param name="destination"> The resulting parsed arguments. </param>
		/// <param name="reporter"> The destination for parse errors. </param>
		/// <returns> true if no errors were detected. </returns>
		private static bool ParseArguments(string[] arguments, object destination, ErrorReporter reporter)
		{
			Parser parser = new Parser(destination.GetType(), reporter);
			return parser.Parse(arguments, destination);
		}

		private static void NullErrorReporter(string message) 
		{ 
		}

		private class HelpArgument 
		{
			[ArgumentAttribute(ArgumentTypes.AtMostOnce, ShortName = "?")]
			public bool help = false;

			//Note: This constructor is only implemented so that we can suppress the FxCop violation
			/// <summary>
			/// HelpArgument constructor 
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Scope = "member", Target = "Microsoft.IdentityManagement.Test.Framework.TestUtilities.CommandLine.Parser+HelpArgument..ctor()", Justification = "Need to suppress this since we have to specify a default value, otherwise we get a build error.")]
			public HelpArgument()
			{
			}
		} 

		/// <summary>
		/// Checks if a set of arguments asks for help.
		/// </summary>
		/// <param name="args"> Args to check for help. </param>
		/// <returns> Returns true if args contains /? or /help. </returns>
		public static bool ParseHelp(string[] args)
		{
			Parser helpParser = new Parser(typeof(HelpArgument), new ErrorReporter(NullErrorReporter));
			HelpArgument helpArgument = new HelpArgument();
			helpParser.Parse(args, helpArgument);
			return helpArgument.help;
		}


		/// <summary>
		/// Returns a Usage string for command line argument parsing.
		/// Use ArgumentAttributes to control parsing behaviour.
		/// </summary>
		/// <param name="argumentType"> The type of the arguments to display usage for. </param>
		/// <returns> Printable string containing a user friendly description of command line arguments. </returns>
		public static string ArgumentsUsage(Type argumentType)
		{
			return (new Parser(argumentType, null)).UsageString;
		}

		private const int STD_OUTPUT_HANDLE  = -11;

		/// <summary>
		/// Returns the number of columns in the current console window
		/// </summary>
		/// <returns>Returns the number of columns in the current console window</returns>
		private static int GetConsoleWindowWidth()
		{
			int screenWidth;
			CONSOLE_SCREEN_BUFFER_INFO csbi = new CONSOLE_SCREEN_BUFFER_INFO();

			NativeMethods.GetConsoleScreenBufferInfo(NativeMethods.GetStdHandle(STD_OUTPUT_HANDLE), ref csbi);
			screenWidth = csbi.dwSize.x;
			return screenWidth;
		}

		private const int spaceBeforeParam = 2;

		/// <summary>
		/// Displays the values of the input parameters to the console
		/// </summary>
		/// <param name="destination">Class that contains the parameter arguments</param>
		public static void DisplayInputParameters(object destination)
		{
			ArrayList paramNameList = new ArrayList();
			ArrayList paramValList = new ArrayList();
			Console.WriteLine( "----------------------------------------" );
			Console.WriteLine( "Input parameters" );
			Console.WriteLine( "----------------------------------------" );

			if (destination == null)
				throw new ArgumentNullException("destination");

			//Build lists of parameter names and their values
			foreach (FieldInfo field in destination.GetType().GetFields())
			{
				if (!field.IsStatic && !field.IsInitOnly && !field.IsLiteral)
				{
					if (field.GetValue(destination) != null)
					{
						//If it is a collection then we need to retrieve each value from the collection
						if (IsCollectionType(field.GetValue(destination).GetType()))
						{
							//Create an arrayList from the collection
							ArrayList valList = new ArrayList((ICollection)field.GetValue(destination));
							bool firstArg = true;

							//We only display the parameter if a value is specified
							if (valList.Count != 0)
								paramNameList.Add(field.Name);

							foreach (object val in valList)
							{
								//To get each value on a separate line we add a blank paramName to the list
								if (!firstArg)
									paramNameList.Add("");
								firstArg = false;
								paramValList.Add(val.ToString());
							}
						}
						else
						{
							paramNameList.Add(field.Name);
							paramValList.Add(field.GetValue(destination).ToString());
						}
					}
				}
			}

			Console.WriteLine(PrettyPrintTwoColumn(paramNameList, paramValList));
			Console.WriteLine("----------------------------------------");
		}

		/// <summary>
		/// Prints information in well formatted two columns
		/// </summary>
		/// <param name="col1">Values for first column</param>
		/// <param name="col2">Values for seconds column</param>
		/// <returns>string formatted into two columns</returns>
		private static string PrettyPrintTwoColumn(ArrayList col1, ArrayList col2)
		{
			StringBuilder builder = new StringBuilder();
			int maxColumnOneLen = 0;
			int screenWidth = GetConsoleWindowWidth();
			int columnTwoIndex; //index where column two should start
			const int minimumNumberOfColumnTwoChars = 10;
			const int minimumColumnTwoIndex = 5;
			const int minimumScreenWidth = minimumColumnTwoIndex + minimumNumberOfColumnTwoChars;

			//Find the max length of the values in column 1
			foreach (string col1Val in col1)
				maxColumnOneLen = Math.Max(maxColumnOneLen, col1Val.Length);

			//Calculate the screenwidth and then the index where the second column should start
			int idealMinimumColumnTwo = maxColumnOneLen + spaceBeforeParam;
			screenWidth = Math.Max(screenWidth, minimumScreenWidth);
			if (screenWidth < (idealMinimumColumnTwo + minimumNumberOfColumnTwoChars))
				columnTwoIndex = minimumColumnTwoIndex;
			else
				columnTwoIndex = idealMinimumColumnTwo;
			int charsPerLine = screenWidth - columnTwoIndex;

			//Loop though each of the values in column 1 and append them plus format and append column 2
			for (int i = 0; i < col1.Count; i++)
			{
				int col1Length = col1[i].ToString().Length;
				builder.Append(col1[i]);

				int curCol = col1Length;
				if (col1Length >= columnTwoIndex)
				{
					builder.Append(NewLine);
					curCol = 0;
				}

				int index = 0;
				//Start appending column 2 text, starting a new line and lining it up as appropriate
				while (index < col2[i].ToString().Length)
				{
					builder.Append(' ', columnTwoIndex - curCol);
					curCol = columnTwoIndex;

					//determine how many chars to display on this line
					int endIndex = index + charsPerLine;
					if (endIndex >= col2[i].ToString().Length)
					{
						//rest of text fits on this line
						endIndex = col2[i].ToString().Length;
					}
					else
					{
						endIndex = col2[i].ToString().LastIndexOf(' ', endIndex - 1, Math.Min(endIndex - index, charsPerLine));
						if (endIndex <= index)
						{
							//no spaces on this line, append full set of chars
							endIndex = index + charsPerLine;
						}
					}

					//add chars
					builder.Append(col2[i].ToString(), index, endIndex - index);
					index = endIndex;

					//Add new line except for last line
					if( i+1< col1.Count || endIndex != col2[i].ToString().Length )
						builder.Append(NewLine);
					curCol = 0;

					//don't start a new line with spaces
					while (index < col2[i].ToString().Length && col2[i].ToString()[index] == ' ')
						index++;
				}
			}
			return builder.ToString();
		}

		/// <summary>
		/// Creates a new command line argument parser.
		/// </summary>
		/// <param name="argumentSpecification"> The type of object to  parse. </param>
		/// <param name="reporter"> The destination for parse errors. </param>
		private Parser(Type argumentSpecification, ErrorReporter reporter)
		{
			this.reporter = reporter;
			this.arguments = new ArrayList();
			this.argumentMap = new Hashtable();

			foreach (FieldInfo field in argumentSpecification.GetFields())
			{
				if (!field.IsStatic && !field.IsInitOnly && !field.IsLiteral)
				{
					ArgumentAttribute attribute = GetAttribute(field);
					if (attribute is DefaultArgumentAttribute)
					{
						Debug.Assert(this.defaultArgument == null);
						this.defaultArgument = new Argument(attribute, field, reporter);
					}
					else
					{
						this.arguments.Add(new Argument(attribute, field, reporter));
					}
				}
			}

			// add explicit names to map
			foreach (Argument argument in this.arguments)
			{
				Debug.Assert(!argumentMap.ContainsKey(argument.LongName));
				this.argumentMap[argument.LongName] = argument;
				if (argument.ExplicitShortName)
				{
					if (argument.ShortName != null && argument.ShortName.Length > 0)
					{
						Debug.Assert(!argumentMap.ContainsKey(argument.ShortName));
						this.argumentMap[argument.ShortName] = argument;
					}
					else
					{
						argument.ClearShortName();
					}
				}
			}

			// add implicit names which don't collide to map
			foreach (Argument argument in this.arguments)
			{
				if (!argument.ExplicitShortName)
				{
					if (argument.ShortName != null && argument.ShortName.Length > 0 && !argumentMap.ContainsKey(argument.ShortName))
						this.argumentMap[argument.ShortName] = argument;
					else
						argument.ClearShortName();
				}
			}
		}

		private static ArgumentAttribute GetAttribute(FieldInfo field)
		{
			object[] attributes = field.GetCustomAttributes(typeof(ArgumentAttribute), false);
			if (attributes.Length == 1)
				return (ArgumentAttribute) attributes[0];

			Debug.Assert(attributes.Length == 0);
			return null;
		}

		private void ReportUnrecognizedArgument(string argument)
		{
			this.reporter(string.Format(Thread.CurrentThread.CurrentCulture, "Unrecognized command line argument '{0}'", argument));
		}

		/// <summary>
		/// Parses an argument list into an object
		/// </summary>
		/// <param name="args"></param>
		/// <param name="destination"></param>
		/// <returns> true if an error occurred </returns>
		private bool ParseArgumentList(string[] args, object destination)
		{
			bool hadError = false;
			if (args != null)
			{
				foreach (string argument in args)
				{
					if (argument.Length > 0)
					{
						switch (argument[0])
						{
						case '-':
						case '/':
							int endIndex = argument.IndexOfAny(new char[] {':', '+', '-'}, 1);
							string option = argument.Substring(1, endIndex == -1 ? argument.Length - 1 : endIndex - 1);
							string optionArgument;
							if (option.Length + 1 == argument.Length)
							{
								optionArgument = null;
							}
							else if (argument.Length > 1 + option.Length && argument[1 + option.Length] == ':')
							{
								optionArgument = argument.Substring(option.Length + 2);
							}
							else
							{
								optionArgument = argument.Substring(option.Length + 1);
							}

							if (optionArgument != null)
							{
								optionArgument = Environment.ExpandEnvironmentVariables(optionArgument);
							}

							Argument arg = (Argument) this.argumentMap[option];
							if (arg == null)
							{
								ReportUnrecognizedArgument(argument);
								hadError = true;
							}
							else
							{
								hadError |= !arg.SetValue(optionArgument, destination);
							}
							break;
						case '@':
							string[] nestedArguments;
							hadError |= LexFileArguments(argument.Substring(1), out nestedArguments);
							hadError |= ParseArgumentList(nestedArguments, destination);
							break;
						default:
							if (this.defaultArgument != null)
							{
								hadError |= !this.defaultArgument.SetValue(argument, destination);
							}
							else
							{
								ReportUnrecognizedArgument(argument);
								hadError = true;
							}
							break;
						}
					}
				}
			}

			return hadError;
		}

		/// <summary>
		/// Parses an argument list.
		/// </summary>
		/// <param name="args"> The arguments to parse. </param>
		/// <param name="destination"> The destination of the parsed arguments. </param>
		/// <returns> true if no parse errors were encountered. </returns>
		public bool Parse(string[] args, object destination)
		{
			bool hadError = ParseArgumentList(args, destination);

			// check for missing required arguments
			foreach (Argument arg in this.arguments)
			{
				hadError |= arg.Finish(destination);
			}
			if (this.defaultArgument != null)
			{
				hadError |= this.defaultArgument.Finish(destination);
			}

			return !hadError;
		}

		private struct ArgumentHelpStrings
		{
			public ArgumentHelpStrings(string syntax, string help)
			{
				this.syntax = syntax;
				this.help = help;
			}

			public string syntax;
			public string help;
		}

		/// <summary>
		/// A user friendly usage string describing the command line argument syntax.
		/// </summary>
		public string UsageString
		{
			get
			{
				ArgumentHelpStrings[] strings = GetAllHelpStrings();
				ArrayList paramSyntax = new ArrayList();
				ArrayList paramHelp = new ArrayList();
				Assembly assembly = Assembly.GetEntryAssembly();
				AssemblyDescriptionAttribute desc = (AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute));
				string header = assembly.GetName().Name;
				if (desc != null)
				{
					//Need to create these arrays just for the pretty print method
					ArrayList headList = new ArrayList();
					headList.Add(header.Trim() + " -");
					ArrayList descList = new ArrayList();
					descList.Add(desc.Description.Trim());
					//Format the header
					header = PrettyPrintTwoColumn(headList, descList);
				}

				//Build the arrays of the parameters and their help text for prettyprint
				foreach (ArgumentHelpStrings helpStrings in strings)
				{
					// add syntax string
					paramSyntax.Add("  " + helpStrings.syntax);
					paramHelp.Add(helpStrings.help);
				}

				return "\r\n" + header + "\r\nSwitches:\r\n" + PrettyPrintTwoColumn(paramSyntax, paramHelp);
			}
		}

		private ArgumentHelpStrings[] GetAllHelpStrings()
		{
			ArgumentHelpStrings[] strings = new ArgumentHelpStrings[NumberOfParametersToDisplay()];

			int index = 0;
			foreach (Argument arg in this.arguments)
			{
				strings[index] = GetHelpStrings(arg);
				index++;
			}
			strings[index++] = new ArgumentHelpStrings("@<file>", "Read response file for more options");
			if (this.defaultArgument != null)
				strings[index++] = GetHelpStrings(this.defaultArgument);

			return strings;
		}

		private static ArgumentHelpStrings GetHelpStrings(Argument arg)
		{
			return new ArgumentHelpStrings(arg.SyntaxHelp, arg.FullHelpText);
		}

		private int NumberOfParametersToDisplay()
		{
			int numberOfParameters = this.arguments.Count + 1;
			if (HasDefaultArgument)
				numberOfParameters += 1;
			return numberOfParameters;
		}

		/// <summary>
		/// Does this parser have a default argument.
		/// </summary>
		/// <value> Does this parser have a default argument. </value>
		private bool HasDefaultArgument
		{
			get { return this.defaultArgument != null; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="Need to catch multiple exceptions that could occur when opening the filestream")]
		private bool LexFileArguments(string fileName, out string[] lexArguments)
		{
			string args  = null;

			try
			{
				using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					StreamReader sr = new StreamReader(file);
					args = sr.ReadToEnd();
					sr.Dispose();
				}
			}
			catch (Exception e)
			{
				this.reporter(string.Format(Thread.CurrentThread.CurrentCulture, "Error: Can't open command line argument file '{0}' : '{1}'", fileName, e.Message));
				lexArguments = null;
				return false;
			}

			bool hadError = false;                    
			ArrayList argArray = new ArrayList();
			StringBuilder currentArg = new StringBuilder();
			bool inQuotes = false;
			int index = 0;

			// while (index < args.Length)
			try
			{
				while (true)
				{
					// skip whitespace
					while (char.IsWhiteSpace(args[index]))
					{
						index += 1;
					}

					// # - comment to end of line
					if (args[index] == '#')
					{
						index += 1;
						while (args[index] != '\n')
						{
							index += 1;
						}
						continue;
					}

					// do one argument
					do
					{
						if (args[index] == '\\')
						{
							int cSlashes = 1;
							index += 1;
							while (index == args.Length && args[index] == '\\')
							{
								cSlashes += 1;
							}

							if (index == args.Length || args[index] != '"')
							{
								currentArg.Append('\\', cSlashes);
							}
							else
							{
								currentArg.Append('\\', (cSlashes >> 1));
								if (0 != (cSlashes & 1))
								{
									currentArg.Append('"');
								}
								else
								{
									inQuotes = !inQuotes;
								}
							}
						}
						else if (args[index] == '"')
						{
							inQuotes = !inQuotes;
							index += 1;
						}
						else
						{
							currentArg.Append(args[index]);
							index += 1;
						}
					} while (!char.IsWhiteSpace(args[index]) || inQuotes);
					argArray.Add(currentArg.ToString());
					currentArg.Length = 0;
				}
			}
			catch (System.IndexOutOfRangeException)
			{
				// got EOF 
				if (inQuotes)
				{
					this.reporter(string.Format(Thread.CurrentThread.CurrentCulture, "Error: Unbalanced '\"' in command line argument file '{0}'", fileName));
					hadError = true;
				}
				else if (currentArg.Length > 0)
				{
					// valid argument can be terminated by EOF
					argArray.Add(currentArg.ToString());
				}
			}

			lexArguments = (string[])argArray.ToArray(typeof(string));
			return hadError;
		}

		private static string LongName(ArgumentAttribute attribute, FieldInfo field)
		{
			return (attribute == null || attribute.DefaultLongName) ? field.Name : attribute.LongName;
		}

		private static string ShortName(ArgumentAttribute attribute, FieldInfo field)
		{
			if (attribute is DefaultArgumentAttribute)
				return null;
			if (!ExplicitShortName(attribute))
				return LongName(attribute, field).Substring(0,1);
			return attribute.ShortName;
		}

		private static string HelpText(ArgumentAttribute attribute)
		{
			if (attribute == null)
				return null;
			else
				return attribute.HelpText;
		}

		private static bool HasHelpText(ArgumentAttribute attribute)
		{
			return (attribute != null && attribute.HasHelpText);
		}

		private static bool ExplicitShortName(ArgumentAttribute attribute)
		{
			return (attribute != null && !attribute.DefaultShortName);
		}

		private static object DefaultValue(ArgumentAttribute attribute)
		{
			return (attribute == null || !attribute.HasDefaultValue) ? null : attribute.DefaultValue;
		}

		private static Type ElementType(FieldInfo field)
		{
			if (IsCollectionType(field.FieldType))
				return field.FieldType.GetElementType();
			else
				return null;
		}

		private static ArgumentTypes Flags(ArgumentAttribute attribute, FieldInfo field)
		{
			if (attribute != null)
				return attribute.Type;
			else if (IsCollectionType(field.FieldType))
				return ArgumentTypes.MultipleUnique;
			else
				return ArgumentTypes.AtMostOnce;
		}

		private static bool IsCollectionType(Type type)
		{
			return type.IsArray;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="7/11/07 - DarrylRu - This appears as uncalled in retail builds because it is only used in Debug.Assert calls")]
		private static bool IsValidElementType(Type type)
		{
			return type != null && (
				type == typeof(int) ||
				type == typeof(uint) ||
				type == typeof(string) ||
				type == typeof(bool) ||
				type == typeof(DateTime) ||
				type.IsEnum);
		}

		private class Argument
		{
			public Argument(ArgumentAttribute attribute, FieldInfo field, ErrorReporter reporter)
			{
				this.longName = Parser.LongName(attribute, field);
				this.explicitShortName = Parser.ExplicitShortName(attribute);
				this.shortName = Parser.ShortName(attribute, field);
				this.hasHelpText = Parser.HasHelpText(attribute);
				this.helpText = Parser.HelpText(attribute);
				this.defaultValue = Parser.DefaultValue(attribute);
				this.elementType = ElementType(field);
				this.flags = Flags(attribute, field);
				this.field = field;
				this.reporter = reporter;
				this.isDefault = attribute != null && attribute is DefaultArgumentAttribute;

				if (IsCollection)
				{
					this.collectionValues = new ArrayList();
				}

				Debug.Assert(!string.IsNullOrEmpty(longName));
				Debug.Assert(!this.isDefault || !this.ExplicitShortName);
				Debug.Assert(!IsCollection || AllowMultiple, "Collection arguments must have allow multiple");
				Debug.Assert(!Unique || IsCollection, "Unique only applicable to collection arguments");
				Debug.Assert(IsValidElementType(Type) ||
					IsCollectionType(Type));
				Debug.Assert((IsCollection && IsValidElementType(elementType)) ||
					(!IsCollection && elementType == null));
				Debug.Assert(!(this.IsRequired && this.HasDefaultValue), "Required arguments cannot have default value");
				Debug.Assert(!this.HasDefaultValue || (this.defaultValue.GetType() == field.FieldType), "Type of default value must match field type");
			}

			public bool Finish(object destination)
			{
				if (!this.SeenValue && this.HasDefaultValue)
				{
					this.field.SetValue(destination, this.DefaultValue);
				}
				if (this.IsCollection)
				{
					this.field.SetValue(destination, this.collectionValues.ToArray(this.elementType));
				}

				return ReportMissingRequiredArgument();
			}

			private bool ReportMissingRequiredArgument()
			{
				if (this.IsRequired && !this.SeenValue)
				{
					if (this.IsDefault)
						reporter(string.Format(Thread.CurrentThread.CurrentCulture, "Missing required argument '<{0}>'.", this.LongName));
					else
						reporter(string.Format(Thread.CurrentThread.CurrentCulture, "Missing required argument '/{0}'.", this.LongName));
					return true;
				}
				return false;
			}

			private void ReportDuplicateArgumentValue(string value)
			{
				this.reporter(string.Format(Thread.CurrentThread.CurrentCulture, "Duplicate '{0}' argument '{1}'", this.LongName, value));
			}

			internal bool SetValue(string value, object destination)
			{
				if (SeenValue && !AllowMultiple)
				{
					this.reporter(string.Format(Thread.CurrentThread.CurrentCulture, "Duplicate '{0}' argument", this.LongName));
					return false;
				}
				this.seenValue = true;

				object newValue;
				if (!ParseValue(this.ValueType, value, out newValue))
					return false;
				if (this.IsCollection)
				{
					if (this.Unique && this.collectionValues.Contains(newValue))
					{
						ReportDuplicateArgumentValue(value);
						return false;
					}
					else
					{
						this.collectionValues.Add(newValue);
					}
				}
				else
				{
					this.field.SetValue(destination, newValue);
				}

				return true;
			}

			public Type ValueType
			{
				get { return this.IsCollection ? this.elementType : this.Type; }
			}

			private void ReportBadArgumentValue(string value)
			{
				this.reporter(string.Format(Thread.CurrentThread.CurrentCulture, "'{0}' is not a valid value for the '{1}' command line option", value, this.LongName));
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch multiple exceptions that could occur when parsing different value types")]
			private bool ParseValue(Type type, string stringData, out object value)
			{
				// null is only valid for bool variables
				// empty string is never valid
				if ((stringData != null || type == typeof(bool)) && (stringData == null || stringData.Length > 0))
				{
					try
					{
						if (type == typeof(string))
						{
							value = stringData;
							return true;
						}
						else if (type == typeof(bool))
						{
							if (stringData == null || stringData == "+" || stringData.Equals("true", StringComparison.OrdinalIgnoreCase) || stringData.Equals("1", StringComparison.OrdinalIgnoreCase))
							{
								value = true;
								return true;
							}
							else if (stringData == "-" || stringData.Equals("false", StringComparison.OrdinalIgnoreCase) || stringData.Equals("0", StringComparison.OrdinalIgnoreCase))
							{
								value = false;
								return true;
							}
						}
						else if (type == typeof(int))
						{
							value = int.Parse(stringData, Thread.CurrentThread.CurrentCulture);
							return true;
						}
						else if (type == typeof(uint))
						{
							value = int.Parse(stringData, Thread.CurrentThread.CurrentCulture);
							return true;
						}
						else if (type == typeof(DateTime))
						{
							value = DateTime.Parse(stringData, Thread.CurrentThread.CurrentCulture);
							return true;
						}
						else
						{
							Debug.Assert(type.IsEnum);
							value = Enum.Parse(type, stringData, true);
							return true;
						}
					}
					catch(Exception)
					{
						// catch parse errors
					}
				}

				ReportBadArgumentValue(stringData);
				value = null;
				return false;
			}

			private void AppendValue(StringBuilder builder, object value)
			{
				if (value is string || value is int || value is uint || value.GetType().IsEnum)
				{
					builder.Append(value.ToString());
				}
				else if (value is bool)
				{
					builder.Append((bool) value ? "true" : "false");
				}
				else
				{
					bool first = true;
					foreach (object o in (System.Array) value)
					{
						if (!first)
						{
							builder.Append(", ");
						}
						AppendValue(builder, o);
						first = false;
					}
				}
			}

			public string LongName
			{
				get { return this.longName; }
			}

			public bool ExplicitShortName
			{
				get { return this.explicitShortName; }
			}

			public string ShortName
			{
				get { return this.shortName; }
			}

			public bool HasShortName
			{
				get { return this.shortName != null; }
			}

			public void ClearShortName()
			{
				this.shortName = null;
			}

			public bool HasHelpText
			{
				get { return this.hasHelpText; }
			}

			public string HelpText
			{
				get { return this.helpText; }
			}

			public object DefaultValue
			{
				get { return this.defaultValue; }
			}

			public bool HasDefaultValue
			{
				get { return null != this.defaultValue; }
			}

			public string FullHelpText
			{
				get {
					StringBuilder builder = new StringBuilder();
					if (this.HasHelpText)
					{
						builder.Append(this.HelpText);
					}
					if (this.HasDefaultValue)
					{
						if (builder.Length > 0)
							builder.Append(" ");
						builder.Append("Default value:'");
						AppendValue(builder, this.DefaultValue);
						builder.Append('\'');
					}
					if (this.HasShortName)
					{
						if (builder.Length > 0)
							builder.Append(" ");
						builder.Append("(short form /");
						builder.Append(this.ShortName);
						builder.Append(")");
					}
					return builder.ToString();
				}
			}

			public string SyntaxHelp
			{
				get
				{
					StringBuilder builder = new StringBuilder();

					if (this.IsDefault)
					{
						builder.Append("<");
						builder.Append(this.LongName);
						builder.Append(">");
					}
					else
					{
						builder.Append("/");
						builder.Append(this.LongName);
						Type valueType = this.ValueType;
						if (valueType == typeof(int))
						{
							builder.Append(":<int>");
						}
						else if (valueType == typeof(uint))
						{
							builder.Append(":<uint>");
						}
						else if (valueType == typeof(bool))
						{
							builder.Append("[true|false]");
						}
						else if (valueType == typeof(string))
						{
							builder.Append(":<string>");
						}
						else if (valueType == typeof(DateTime))
							builder.Append(":<DateTime>");
						else
						{
							Debug.Assert(valueType.IsEnum);

							builder.Append(":{");
							bool first = true;
							foreach (FieldInfo myField in valueType.GetFields())
							{
								if (myField.IsStatic)
								{
									if (first)
										first = false;
									else
										builder.Append('|');
									builder.Append(myField.Name);
								}
							}
							builder.Append('}');
						}
					}

					return builder.ToString();
				}
			}

			public bool IsRequired
			{
				get { return 0 != (this.flags & ArgumentTypes.Required); }
			}

			public bool SeenValue
			{
				get { return this.seenValue; }
			}

			public bool AllowMultiple
			{
				get { return 0 != (this.flags & ArgumentTypes.Multiple); }
			}

			public bool Unique
			{
				get { return 0 != (this.flags & ArgumentTypes.Unique); }
			}

			public Type Type
			{
				get { return field.FieldType; }
			}

			public bool IsCollection
			{
				get { return IsCollectionType(Type); }
			}

			public bool IsDefault
			{
				get { return this.isDefault; }
			}

			private string longName;
			private string shortName;
			private string helpText;
			private bool hasHelpText;
			private bool explicitShortName;
			private object defaultValue;
			private bool seenValue;
			private FieldInfo field;
			private Type elementType;
			private ArgumentTypes flags;
			private ArrayList collectionValues;
			private ErrorReporter reporter;
			private bool isDefault;
		}

		private ArrayList arguments;
		private Hashtable argumentMap;
		private Argument defaultArgument;
		private ErrorReporter reporter;
	}

	static class ConsoleReporter
	{
		static public void WriterError(string error)
		{
			ConsoleColor curColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine(error);
			Console.ForegroundColor = curColor;
		}
	}
}