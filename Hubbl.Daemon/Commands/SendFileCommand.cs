﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Hubbl.Core.Messages;
using Hubbl.Core.Service;
using Hubbl.Daemon.Properties;
using Module.MessageRouter.Abstractions.Network.Interfaces;

namespace Hubbl.Daemon.Commands
{
	internal class SendFileCommand : ICommand
	{
		private readonly INetworkMessageRouter _router;
		private readonly ISession _session;

		public SendFileCommand(INetworkMessageRouter router, ISession session)
		{
			_router = router;
			_session = session;
			Shortcuts = new[] { "sendfile" };
			Description = Resources.SendFileCommand;
		}

		public bool Execute(params string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Parameters are incorrect. Please, try again.");
				return false;
			}


			var ids = args[0].Split(',');
			

			var file_location = args[1];

			var fileName = Path.GetFileName(file_location);

			var exists = File.Exists(file_location);

			if (!exists)
				Console.WriteLine(Resources.SendFileCommand_Execute_File_not_found);
			Stream stream;
			try
			{
				stream = File.Open(file_location, FileMode.Open);
			}
			catch (IOException e)
			{
				Console.WriteLine("IOException catched! \n {0}", e.Message);
				return false;	
				throw;
			}
			

			
			var tasks = _router.PublishFor(ids, new SendFileMessage(fileName, (ulong)stream.Length, stream));


			var tasksForAll = _router.Publish(new SendFileMessage(fileName, (ulong)stream.Length, stream));

			//TODO: put this tasks to task-container (https://github.com/mesenev/hubbl/issues/3)

			return false;
		}

		public IEnumerable<string> Shortcuts { get; private set; }
		public string Description { get; private set; }
	}
}
