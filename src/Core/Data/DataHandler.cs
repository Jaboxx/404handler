﻿using System;
using System.Collections.Generic;
using System.Data;

namespace BVNetwork.NotFound.Core.Data
{
    public class DataHandler
    {
        public static string UknownReferer = "Uknown referers";

        public static Dictionary<string, int> GetRedirects()
        {
            var keyCounts = new Dictionary<string, int>();
            var dabe = DataAccessBaseEx.GetWorker();
            var allkeys = dabe.GetAllClientRequestCount();

            foreach (DataTable table in allkeys.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    var oldUrl = row[0].ToString();
                    keyCounts.Add(oldUrl, Convert.ToInt32(row[1]));
                }
            }

            return keyCounts;
        }

        public static Dictionary<string, int> GetReferers(string url)
        {
            var dataAccess = DataAccessBaseEx.GetWorker();
            var referersDs = dataAccess.GetRequestReferers(url);

            var referers = new Dictionary<string, int>();

            if (referersDs.Tables[0] == null) return referers;

            var unknownReferers = 0;
            for (var i = 0; i < referersDs.Tables[0].Rows.Count; i++)
            {

                var referer = referersDs.Tables[0].Rows[i][0].ToString();
                var count = Convert.ToInt32(referersDs.Tables[0].Rows[i][1].ToString());
                if (referer.Trim() != string.Empty
                    && !referer.Contains("(null)"))
                {
                    if (!referer.Contains("://")) referer = referer.Insert(0, "/");
                    referers.Add(referer, count);
                }
                else
                {
                    unknownReferers += count;
                }

            }
            if (unknownReferers > 0)
            {
                referers.Add(UknownReferer, unknownReferers);
            }
            return referers;
        }

        public static int GetTotalSuggestionCount()
        {
            var dataAccess = DataAccessBaseEx.GetWorker();
            var totalSuggestionCountDs = dataAccess.GetTotalNumberOfSuggestions();
            if (totalSuggestionCountDs?.Tables != null
                && totalSuggestionCountDs.Tables.Count > 0)
            {
                return Convert.ToInt32(totalSuggestionCountDs.Tables[0].Rows[0][0]);
            }
            return 0;
        }
    }
}