﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Waher.Content.Xml;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Activities
{
	/// <summary>
	/// Represents an activity that can be executed as the result of triggered events.
	/// </summary>
	public class Activity : SimulationNodeChildren, IActivity
	{
		private LinkedList<IActivityNode> activityNodes = null;
		private string id;

		/// <summary>
		/// Represents an activity that can be executed as the result of triggered events.
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Activity(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// ID of activity.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// Local name of XML element defining contents of class.
		/// </summary>
		public override string LocalName => "Activity";

		/// <summary>
		/// Creates a new instance of the node.
		/// </summary>
		/// <param name="Parent">Parent node.</param>
		/// <returns>New instance</returns>
		public override ISimulationNode Create(ISimulationNode Parent)
		{
			return new Activity(Parent);
		}

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
			Model.Register(this);
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
		}

		/// <summary>
		/// Executes the activity.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		public virtual async Task ExecuteTask(Model Model, Variables Variables)
		{
			if (!(this.activityNodes is null))
			{
				try
				{
					await ExecuteActivity(Model, Variables, this.activityNodes.First);
				}
				catch (FinishedException)
				{
					// Execution finished.
				}
			}
		}

		/// <summary>
		/// Executes an activity by executing a possibly branching sequence of nodes.
		/// </summary>
		/// <param name="Model">Current model</param>
		/// <param name="Variables">Set of variables for the activity.</param>
		/// <param name="Start">Node to start execution with.</param>
		public static async Task ExecuteActivity(Model Model, Variables Variables, LinkedListNode<IActivityNode> Start)
		{
			LinkedListNode<IActivityNode> Next;

			while (!(Start is null))
			{
				Next = await Start.Value.Execute(Model, Variables);
				if (Next is null)
					Next = Start.Next;

				Start = Next;
			}
		}
	}
}
