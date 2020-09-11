﻿using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using TAG.Simulator.ObjectModel.Events;

namespace TAG.Simulator.ObjectModel.Actors
{
	/// <summary>
	/// Basic interface for simulator nodes. Implementing this interface allows classes with default contructors to be used
	/// in simulator models.
	/// </summary>
	public interface IActor : ISimulationNode
	{
		/// <summary>
		/// ID of actor.
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// Registers an external event on the actor.
		/// </summary>
		/// <param name="ExternalEvent">External event</param>
		void Register(ExternalEvent ExternalEvent);

		/// <summary>
		/// Tries to get an external event, given its name.
		/// </summary>
		/// <param name="Name">Name of external event.</param>
		/// <param name="ExternalEvent">External event object.</param>
		/// <returns>If an external event with the corresponding name was found.</returns>
		bool TryGetExternalEvent(string Name, out ExternalEvent ExternalEvent);

		/// <summary>
		/// Number of individuals in population that are free.
		/// </summary>
		int FreeCount
		{
			get;
		}

		/// <summary>
		/// Gets a free individual instance from the population.
		/// </summary>
		/// <param name="Index">Zero-based index of individual to get.</param>
		/// <param name="Exclusive">If individual is for exclusive use (i.e. will not be free once gotten, until returned).</param>
		/// <returns>Individual instance returned.</returns>
		IActor GetFreeIndividual(int Index, bool Exclusive);

		/// <summary>
		/// Returns an individual to the population, once free again.
		/// </summary>
		/// <param name="Individual">Individual to return.</param>
		void ReturnIndividual(IActor Individual);

		/// <summary>
		/// Returns the object that will be used by the actor for actions during an activity.
		/// </summary>
		object ActivityObject
		{
			get;
		}
	}
}
