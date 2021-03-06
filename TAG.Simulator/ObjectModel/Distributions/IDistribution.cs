﻿using System;
using System.Text;

namespace TAG.Simulator.ObjectModel.Distributions
{
	/// <summary>
	/// Interface for distributions
	/// </summary>
	public interface IDistribution : ISimulationNode
	{
		/// <summary>
		/// ID of distribution.
		/// </summary>
		string Id
		{
			get;
		}

		/// <summary>
		/// Check if distribution has a sample within the time period.
		/// </summary>
		/// <param name="t1">Starting time of period.</param>
		/// <param name="t2">Ending time of period.</param>
		/// <param name="NrCycles">Number of time cycles completed.</param>
		/// <returns>How many times samples were found in time period.</returns>
		int CheckTrigger(double t1, double t2, int NrCycles);

		/// <summary>
		/// Exports the PDF function, if not already exported.
		/// </summary>
		/// <param name="Output">Export output</param>
		void ExportPdfOnceOnly(StringBuilder Output);
	}
}
