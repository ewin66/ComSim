﻿using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Values;
using Waher.Content.Xml;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Defines an argument in an action.
	/// </summary>
	public class Argument : SimulationNodeChildren, IValueRecipient
	{
		private IValue value = null;
		private string name;

		/// <summary>
		/// Defines an argument in an action.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Argument(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Argument";

		/// <summary>
		/// Name of variable within the scope of the event.
		/// </summary>
		public string Name => this.name;

		/// <summary>
		/// Value node.
		/// </summary>
		public IValue Value => this.value;

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Argument(Parent);
		}

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.name = XML.Attribute(Definition, "name");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Registers a value for the argument.
		/// </summary>
		/// <param name="Value">Value node</param>
		public void Register(IValue Value)
		{
			if (this.value is null)
				this.value = Value;
			else
				throw new Exception("Argument already has a value defined.");
		}

	}
}
