using Google.GenAI.Types;

namespace AiQaLab.AI.Schemas
{
    public static class TestAnalysisSchema
    {
        public static Schema Create()
        {
            var suggestionSchema = new Schema
            {
                Type = Google.GenAI.Types.Type.Object,
                Properties = new Dictionary<string, Schema>
                {
                    ["description"] = new Schema
                    {
                        Type = Google.GenAI.Types.Type.String
                    },
                    ["testLevel"] = new Schema
                    {
                        Type = Google.GenAI.Types.Type.String,
                        Enum = ["Unit", "Integration", "EndToEnd"]
                    },
                    ["reason"] = new Schema
                    {
                        Type = Google.GenAI.Types.Type.String
                    }
                },
                Required = ["description", "testLevel", "reason"],
                PropertyOrdering = ["description", "testLevel", "reason"]
            };

            return new Schema
            {
                Type = Google.GenAI.Types.Type.Object,
                Properties = new Dictionary<string, Schema>
                {
                    ["suggestions"] = new Schema
                    {
                        Type = Google.GenAI.Types.Type.Array,
                        Items = suggestionSchema,
                        MinItems = 1
                    }
                },
                Required = ["suggestions"],
                PropertyOrdering = ["suggestions"]
            };
        }
    }
}
