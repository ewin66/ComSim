﻿using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator;
using Waher.Content.Xml;
using Waher.Content.Xsl;
using Waher.Events;
using Waher.Events.Console;
using Waher.Persistence;
using Waher.Persistence.Files;
using Waher.Runtime.Inventory;
using Waher.Runtime.Inventory.Loader;

namespace ComSim
{
	/// <summary>
	/// The TAG Network Communication Simulator (or TAG ComSim) is a white-label console
	/// utility application written in C# provided by Trust Anchor Group (TAG for short).
	/// It can be used to simulate network communication traffic in large-scale networks.
	/// 
	/// Command-line arguments:
	/// 
	/// -i FILENAME           Specifies the filename of the model to use during simulation.
	///                       The file must be an XML file that conforms to the
	///                       http://trustanchorgroup.com/Schema/ComSim.xsd namespace.
	///                       Schema is available at Schema/ComSim.xsd in repository.
	/// -d APP_DATA_FOLDER    Points to the application data folder. Required if storage
	///                       of data in a local database is necessary for the 
	///                       simulation. (Storage can include generated user credentials
	///                       so that the same user identities can be used across
	///                       simulations.)
	/// -e                    If encryption is used by the database. Default=no encryption.
	/// -bs BLOCK_SIZE        Block size, in bytes. Default=8192.
	/// -bbs BLOB_BLOCK_SIZE  BLOB block size, in bytes. Default=8192.
	/// -enc ENCODING         Text encoding. Default=UTF-8
	/// -?                    Displays command-line help.
	/// </summary>
	class Program
	{
		static int Main(string[] args)
		{
			try
			{
				XmlDocument Model = null;
				Encoding Encoding = Encoding.UTF8;
				string ProgramDataFolder = null;
				int i = 0;
				int c = args.Length;
				int BlockSize = 8192;
				int BlobBlockSize = 8192;
				bool Encryption = false;
				string s;
				bool Help = args.Length == 0;

				while (i < c)
				{
					s = args[i++].ToLower();

					if (s.StartsWith("/"))
						s = "-" + s.Substring(1);

					switch (s)
					{
						case "-i":
							if (i >= c)
								throw new Exception("Expected model filename.");

							s = args[i++];
							if (!File.Exists(s))
								throw new Exception("File not found: " + s);

							Model = new XmlDocument();
							Model.Load(s);
							break;

						case "-d":
							if (i >= c)
								throw new Exception("Missing program data folder.");

							if (string.IsNullOrEmpty(ProgramDataFolder))
								ProgramDataFolder = args[i++];
							else
								throw new Exception("Only one program data folder allowed.");
							break;

						case "-bs":
							if (i >= c)
								throw new Exception("Block size missing.");

							if (!int.TryParse(args[i++], out BlockSize))
								throw new Exception("Invalid block size");

							break;

						case "-bbs":
							if (i >= c)
								throw new Exception("Blob Block size missing.");

							if (!int.TryParse(args[i++], out BlobBlockSize))
								throw new Exception("Invalid blob block size");

							break;

						case "-enc":
							if (i >= c)
								throw new Exception("Text encoding missing.");

							Encoding = Encoding.GetEncoding(args[i++]);
							break;

						case "-e":
							Encryption = true;
							break;

						case "-?":
							Help = true;
							break;

						default:
							throw new Exception("Unrecognized switch: " + s);
					}
				}

				if (Help)
				{
					Console.Out.WriteLine("The TAG Network Communication Simulator (or TAG ComSim) is a white-label console");
					Console.Out.WriteLine("utility application written in C# provided by Trust Anchor Group (TAG for short).");
					Console.Out.WriteLine("It can be used to simulate network communication traffic in large-scale networks.");
					Console.Out.WriteLine();
					Console.Out.WriteLine("Command-line arguments:");
					Console.Out.WriteLine();
					Console.Out.WriteLine("-i FILENAME           Specifies the filename of the model to use during simulation.");
					Console.Out.WriteLine("                      The file must be an XML file that conforms to the");
					Console.Out.WriteLine("                      http://trustanchorgroup.com/Schema/ComSim.xsd namespace.");
					Console.Out.WriteLine("                      Schema is available at Schema/ComSim.xsd in the repository.");
					Console.Out.WriteLine("-d APP_DATA_FOLDER    Points to the application data folder. Required if storage");
					Console.Out.WriteLine("                      of data in a local database is necessary for the");
					Console.Out.WriteLine("                      simulation. (Storage can include generated user credentials");
					Console.Out.WriteLine("                      so that the same user identities can be used across");
					Console.Out.WriteLine("                      simulations.)");
					Console.Out.WriteLine("-e                    If encryption is used by the database. Default=no encryption.");
					Console.Out.WriteLine("-bs BLOCK_SIZE        Block size, in bytes. Default=8192.");
					Console.Out.WriteLine("-bbs BLOB_BLOCK_SIZE  BLOB block size, in bytes. Default=8192.");
					Console.Out.WriteLine("-enc ENCODING         Text encoding. Default=UTF-8");
					Console.Out.WriteLine("-?                    Displays command-line help.");
					Console.Out.WriteLine();

					if (args.Length <= 1)
						return 1;
				}

				if (Model is null)
					throw new Exception("No simulation model specified.");

				if (string.IsNullOrEmpty(ProgramDataFolder))
					throw new Exception("No program data folder set");

				Console.Out.WriteLine("Validating model.");

				XSL.Validate("Model", Model, "Model", TAG.Simulator.Model.ComSimNamespace,
					XSL.LoadSchema("ComSim.Schema.ComSim.xsd", typeof(Program).Assembly));

				foreach (XmlNode N in Model.DocumentElement.ChildNodes)
				{
					if (N is XmlElement E && E.LocalName == "Assemblies")
					{
						foreach (XmlNode N2 in E.ChildNodes)
						{
							if (N2 is XmlElement E2 && E2.LocalName == "Assembly")
							{
								string FileName = XML.Attribute(E2, "fileName");

								if (!File.Exists(FileName))
									throw new Exception("File not found: " + FileName);

								Console.Out.WriteLine("Loading " + FileName);

								AssemblyName AN = AssemblyName.GetAssemblyName(FileName);
								AppDomain.CurrentDomain.Load(AN);
							}
						}
					}
				}

				if (!Directory.Exists(ProgramDataFolder))
					Directory.CreateDirectory(ProgramDataFolder);

				Console.Out.WriteLine("Initializing runtime inventory.");
				TypesLoader.Initialize();

				Log.Register(new ConsoleEventSink());

				Console.Out.WriteLine("Initializing database.");

				using (FilesProvider FilesProvider = new FilesProvider(ProgramDataFolder, "Default", BlockSize, 10000, BlobBlockSize, Encoding, 3600000, Encryption, false))
				{
					Run(Model, FilesProvider).Wait();
				}

				Console.Out.WriteLine("Simulation completed.");
			}
			catch (AggregateException ex)
			{
				foreach (Exception ex2 in ex.InnerExceptions)
					WriteLine(ex2.Message, ConsoleColor.White, ConsoleColor.Red);

				return 1;
			}
			catch (Exception ex)
			{
				WriteLine(ex.Message, ConsoleColor.White, ConsoleColor.Red);
				return 1;
			}

			return 0;
		}

		private static async Task Run(XmlDocument ModelXml, FilesProvider DB)
		{
			try
			{
				Console.Out.WriteLine("Starting database...");
				Database.Register(DB);
				await DB.RepairIfInproperShutdown(null);

				Console.Out.WriteLine("Starting modules...");
				await Types.StartAllModules(60000);

				Console.Out.WriteLine("Running simulation...");
				Model Model = (Model)await Factory.Create(ModelXml.DocumentElement, null);
				await Model.Run();
			}
			finally
			{
				Console.Out.WriteLine("Stopping modules...");
				await Types.StopAllModules();
				await DB.Flush();
				Log.Terminate();
			}
		}

		private static void WriteLine(string Row, ConsoleColor ForegroundColor, ConsoleColor BackgrounColor)
		{
			ConsoleColor ForegroundColorBak = Console.ForegroundColor;
			ConsoleColor BackgroundColorBak = Console.BackgroundColor;

			Console.ForegroundColor = ForegroundColor;
			Console.BackgroundColor = BackgrounColor;

			Console.Out.WriteLine(Row);

			Console.ForegroundColor = ForegroundColorBak;
			Console.BackgroundColor = BackgroundColorBak;
		}

	}
}
