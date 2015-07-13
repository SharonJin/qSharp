﻿//
//   Copyright (c) 2011-2014 Exxeleron GmbH
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//

using System;

namespace qSharp.Sample
{
    internal class TickSubscriber
    {
        private static void Main(string[] args)
        {
            var q = new QCallbackConnection((args.Length >= 1) ? args[0] : "localhost",
                (args.Length >= 2) ? int.Parse(args[1]) : 17010);
            try
            {
                q.DataReceived += OnData;
                q.ErrorOccured += OnError;
                q.Open();
                Console.WriteLine("conn: " + q + "  protocol: " + q.ProtocolVersion);
                Console.WriteLine("Press <ENTER> to close application");

                var response = q.Sync(".u.sub", "trade", ""); // subscribe to tick
                var model = (QTable) ((object[]) response)[1]; // get table model

                q.StartListener();
                Console.ReadLine();
                q.StopListener();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error occured: " + e);
                Console.ReadLine();
            }
            finally
            {
                q.Close();
            }
        }

        private static void OnData(object sender, QMessageEvent message)
        {
            var data = message.Message.Data;
            if (data is object[])
            {
                // unpack upd message
                var args = ((object[]) data);
                if (args.Length == 3 && args[0].Equals("upd") && args[2] is QTable)
                {
                    var table = (QTable) args[2];
                    foreach (QTable.Row row in table)
                    {
                        Console.WriteLine(row);
                    }
                }
            }
        }

        private static void OnError(object sender, QErrorEvent error)
        {
            Console.Error.WriteLine("Error received via callback: " + error.Cause.Message);
        }
    }
}