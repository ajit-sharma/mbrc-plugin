using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBeePlugin.Rest.ServiceModel.Const;
using NServiceKit.ServiceHost;

namespace MusicBeePlugin.Rest.ServiceModel
{
	/// <summary>
	/// Used to Retrieve the debug log.
	/// </summary>
	[Route("/debug/log", Verbs.Get, Summary = @"Retrieves the debug log.")]
	public class GetDebugLog
	{
	}

	[Route("/debug/test", Verbs.Get, Summary = @"Used for testing purposes")]
	public class GetTest
	{
	}
}
