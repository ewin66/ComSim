﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Abstract base class for activity nodes
	/// </summary>
	public abstract class ActivityNode : SimulationNodeChildren, IActivityNode
	{
		private LinkedList<IActivityNode> activityNodes = null;
		private int count;
		private string id;

		/// <summary>
		/// Abstract base class for activity nodes
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public ActivityNode(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// ID of activity node.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			if (this.Parent is IActivity Activity)
				Activity.Register(Model, this);
			else if (this.Parent is IActivityNode ActivityNode)
				ActivityNode.Register(Model, this);

			return base.Initialize(Model);
		}

		/// <summary>
		/// Registers a child activity node.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		/// <param name="Node">Activity node.</param>
		public void Register(Model Model, IActivityNode Node)
		{
			if (this.activityNodes is null)
				this.activityNodes = new LinkedList<IActivityNode>();

			Model.Register(this.activityNodes.AddLast(Node));
			this.count++;
		}

		/// <summary>
		/// First child activity node.
		/// </summary>
		protected LinkedListNode<IActivityNode> FirstNode => this.activityNodes?.First;

		/// <summary>
		/// Number of registered activity nodes.
		/// </summary>
		protected int Count => this.count;

		/// <summary>
		/// Executes a node.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <returns>Next node of execution, if different from the default, otherwise null (for default).</returns>
		public abstract Task<LinkedListNode<IActivityNode>> Execute(Model Model, Variables Variables);

		/// <summary>
		/// Adds indentation to the current row.
		/// </summary>
		/// <param name="Output">Output.</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		protected static void Indent(StreamWriter Output, int Indentation)
		{
			if (Indentation > 0)
				Output.Write(new string('\t', Indentation));
		}

		/// <summary>
		/// Exports PlantUML
		/// </summary>
		/// <param name="Output">Output node</param>
		/// <param name="Indentation">Number of tabs to indent.</param>
		public virtual void ExportPlantUml(StreamWriter Output, int Indentation)
		{
			foreach (IActivityNode Node in this.activityNodes)
				Node.ExportPlantUml(Output, Indentation);
		}
	}
}
