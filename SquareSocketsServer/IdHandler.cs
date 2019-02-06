using System;
using System.Collections.Generic;
using System.Text;

namespace SquareSocketsServer {
    internal static class IdHandler {
        private static List<int> Ids { get; set; }

        static IdHandler() {
            Ids = new List<int>();
        }

        public static int NewId() {
            int id;

            id = Ids.Count;
            Ids.Add(id);

            return id;
        }
    }
}
