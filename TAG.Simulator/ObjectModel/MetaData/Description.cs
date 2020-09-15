﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace TAG.Simulator.ObjectModel.MetaData
{
	/// <summary>
	/// Description of model
	/// </summary>
	public class Description : SimulationNode
	{
		private string description;

		/// <summary>
		/// Description of model
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Description(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Description";

		/// <summary>
		/// Description string
		/// </summary>
		public string DescriptionString => this.description;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Description(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.description = Definition.InnerText;

			return Task.CompletedTask;
		}

		/// <summary>
		/// Exports Markdown
		/// </summary>
		/// <param name="Output">Output node</param>
		public override Task ExportMarkdown(StreamWriter Output)
		{
			Output.WriteLine(this.description);
			Output.WriteLine();

			return base.ExportMarkdown(Output);
		}

		/// <summary>
		/// Exports XML
		/// </summary>
		/// <param name="Output">Output node</param>
		public override Task ExportXml(XmlWriter Output)
		{
			Output.WriteElementString("Description", this.description);

			return base.ExportXml(Output);
		}
	}
}