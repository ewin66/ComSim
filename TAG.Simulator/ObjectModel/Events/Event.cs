﻿using System;
using System.Threading.Tasks;
using System.Xml;
using TAG.Simulator.ObjectModel.Activities;
using Waher.Content.Xml;
using Waher.Events;
using Waher.Script;

namespace TAG.Simulator.ObjectModel.Events
{
	/// <summary>
	/// Abstract base class for events
	/// </summary>
	public abstract class Event : SimulationNodeChildren, IEvent
	{
		private Model model;
		private IActivity activity;
		private string activityId;
		private string id;

		/// <summary>
		/// Abstract base class for events
		/// </summary>
		/// <param name="Parent">Parent node</param>
		public Event(ISimulationNode Parent)
			: base(Parent)
		{
		}

		/// <summary>
		/// ID of event.
		/// </summary>
		public string Id => this.id;

		/// <summary>
		/// ID of Activity to execute when event is triggered.
		/// </summary>
		public string ActivityId => this.activityId;

		/// <summary>
		/// Activity to execute when event is triggered.
		/// </summary>
		public IActivity Activity => this.activity;

		/// <summary>
		/// Sets properties and attributes of class in accordance with XML definition.
		/// </summary>
		/// <param name="Definition">XML definition</param>
		public override Task FromXml(XmlElement Definition)
		{
			this.id = XML.Attribute(Definition, "id");
			this.activityId = XML.Attribute(Definition, "activity");

			return base.FromXml(Definition);
		}

		/// <summary>
		/// Initialized the node before simulation.
		/// </summary>
		/// <param name="Model">Model being executed.</param>
		public override Task Initialize(Model Model)
		{
			this.model = Model;
			this.model.Register(this);

			return base.Initialize(Model);
		}

		/// <summary>
		/// Starts the node.
		/// </summary>
		public override Task Start()
		{
			if (!this.model.TryGetActivity(this.activityId, out this.activity))
				throw new Exception("Activity not found: " + this.activityId);

			return base.Start();
		}

		/// <summary>
		/// Triggers the event.
		/// </summary>
		/// <param name="Variables">Event variables</param>
		public async void Trigger(Variables Variables)
		{
			try
			{
				// TODO: Populate and set variables

				this.model.IncActivityStartCount(this.activityId, this.id);
				await this.activity.ExecuteTask(this.model, Variables);
				this.model.IncActivityFinishedCount(this.activityId, this.id);
			}
			catch (Exception ex)
			{
				this.model.IncActivityErrorCount(this.activityId, this.id, ex.Message);
			}
		}

	}
}
