﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ZeroMQ;

namespace ZeroMQ.Test
{
	static partial class Program
	{
		public static void MSReader(IDictionary<string, string> dict, string[] args)
		{
			//
			// Reading from multiple sockets
			// This version uses a simple recv loop
			//
			// Authors: Pieter Hintjens, Uli Riehm
			//

			using (var context = ZContext.Create())
			using (var receiver = ZSocket.Create(context, ZSocketType.PULL))
			using (var subscriber = ZSocket.Create(context, ZSocketType.SUB))
			{
				// Connect to task ventilator
				receiver.Connect("tcp://127.0.0.1:5557");

				// Connect to weather server
				subscriber.Connect("tcp://127.0.0.1:5556");
				subscriber.SetOption(ZSocketOption.SUBSCRIBE, "10001 ");

				// Process messages from both sockets
				// We prioritize traffic from the task ventilator
				ZError error;
				ZFrame frame;
				while (true)
				{
					if (null != (frame = receiver.ReceiveFrame(ZSocketFlags.DontWait, out error)))
					{
						// Process task
					}
					else
					{
						if (error == ZError.ETERM)
							return;	// Interrupted
						if (error != ZError.EAGAIN)
							throw new ZException(error);
					}

					if (null != (frame = subscriber.ReceiveFrame(ZSocketFlags.DontWait, out error)))
					{
						// Process weather update
					}
					else
					{
						if (error == ZError.ETERM)
							return;	// Interrupted
						if (error != ZError.EAGAIN)
							throw new ZException(error);
					}

					// No activity, so sleep for 1 msec
					Thread.Sleep(1);
				}
			}
		}
	}
}