using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Utilities
{
    /// <summary>
    /// This is a crazy-simple scraper / parser for the Locast EPG JSON.
    /// <br /><br />
    /// I use this to simply find unique values for things like EntityType, Genres, and ShowType. That's literally all this is for.
    /// </summary>
    public static class EPGScrapeParser
    {
        public static async Task Parse()
        {
            var filePath = "../../../SampleData/DMA506-Locast-EPG-Scrape.json";
            JsonElement jsonDataRoot;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                jsonDataRoot = await JsonSerializer.DeserializeAsync<JsonElement>(fs);
            }

            FindUniqueValuesForListingProperty("entityType", jsonDataRoot);
            
            FindUniqueValuesForListingProperty("genres", jsonDataRoot, true);

            FindUniqueValuesForListingProperty("showType", jsonDataRoot);
        }

        private static void FindUniqueValuesForListingProperty(string propertyName, JsonElement jsonDataRoot, bool? containsMultipleValues = false)
        {
            Console.WriteLine($"================= Finding listing values for '{propertyName}'");
            
            var uniqueValues = new HashSet<string>();

            foreach (var channel in jsonDataRoot.EnumerateArray())
            {
                var listings = channel.GetProperty("listings");
   
                foreach (var listing in listings.EnumerateArray())
                {
                    try
                    {
                        var theString = listing.GetProperty(propertyName).GetString();
                        if (!string.IsNullOrWhiteSpace(theString))
                        {
                            if (containsMultipleValues.HasValue && containsMultipleValues.Value)
                            {
                                var multipleValues = theString.Split(',');
                                foreach (var multiVal in multipleValues)
                                {
                                    uniqueValues.Add(multiVal.Trim());
                                }
                            }
                            else
                            {
                                uniqueValues.Add(theString);
                            }
                        }
                    }
                    catch { }
                }

            }

            if (uniqueValues.Any())
            {
                foreach (var uniqueValue in uniqueValues.OrderBy(_ => _))
                {
                    Console.WriteLine(uniqueValue);
                }
            }

            Console.WriteLine($"==================================================================");
        }
    }
}
