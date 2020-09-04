﻿using System;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Structure
{
	/// <summary>
	/// Assembly reference.
	/// </summary>
	public class Assembly : SimulationNodeChildren
	{
		private string fileName;

		/// <summary>
		/// Assembly reference.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Assembly(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Assembly";

		/// <summary>
		/// Filename of assembly.
		/// </summary>
		public string FileName => this.fileName;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Assembly(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.fileName = XML.Attribute(Definition, "fileName");

			return base.FromXml(Definition);
		}
	}
}
