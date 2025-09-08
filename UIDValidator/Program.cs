using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace UIDValidator
{
    public class Program
    {
        private static readonly HashSet<string> ApprovedFacilities = new() { "ADDRESS", "FACILITY_2" };
        static async Task Main(string[] args)
        {
            string schemaPath = "uid_schema.json";
            string jsonPath = "label.json";

            // Load and validate JSON schema
            var schema = await JsonSchema.FromFileAsync(schemaPath);
            var jsonData = await File.ReadAllTextAsync(jsonPath);
            var errors = schema.Validate(jsonData);
            if (errors.Count > 0)
            {
                Console.WriteLine("Schema validation failed:");
                errors.ToList().ForEach(e => Console.WriteLine($" - {e.Path}: {e.Kind}"));
                /*foreach (var e in errors)
                {
                    Console.WriteLine($" - {e.Path}: {e.Kind}");
                }*/
                return;
            }

            // Parse JSON data
            var data = JObject.Parse(jsonData);
            var payload = data["payload"];
            var vendorFacility = payload["vendorProcessingFacilityId"]?.ToString();

            if (!ApprovedFacilities.Contains(vendorFacility))
            {
                Console.WriteLine($"Invalid Vendor Facility: {vendorFacility}");
                return;
            }
            var metadata = new VendorMetadata
            {
                LabelVendorID = payload["vendorId"]?.ToString(),
                VendorFacilityID = vendorFacility,
                ShipToDate = payload["shipDate"]?.ToString(),
                ShipToLocation = payload["shipToLocation"]?.ToString(),
                ProductSKU = payload["lot"]?["productSKU"]?.ToString(),
                LotNumber = payload["lot"]?["lotId"]?.ToString(),
            };

            foreach (var caseItem in payload["lot"]["cases"])
            {
                foreach (var roll in caseItem["rolls"])
                {
                    metadata.RollNumbers.Add(roll["rollId"]?.ToString());
                }
            }
            Console.WriteLine("Metadata extracted successfully:");
            Console.WriteLine($"Vendor ID: {metadata.LabelVendorID}");
            Console.WriteLine($"Facility: {metadata.VendorFacilityID}");
            Console.WriteLine($"Lot: {metadata.LotNumber}");
            Console.WriteLine($"Rolls: {string.Join(", ", metadata.RollNumbers)}");
        }
    }
}
