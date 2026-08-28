using System.Text.Json.Nodes;

namespace AiQaLab.AI.Schemas
{
    public static class TestAnalysisSchema
    {
        public static JsonNode CreateSchema()
        {
            return JsonNode.Parse("""
            {
                "type": "object",
                "properties": {
                    "suggestions": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "description": {
                                    "type": "string"
                                },
                                "testLevel": {
                                    "type": "string",
                                    "enum": [
                                        "Unit",
                                        "Integration",
                                        "EndToEnd"
                                    ]
                                },
                                "reason": {
                                    "type": "string"
                                }
                            },
                            "required": [
                                "description",
                                "testLevel",
                                "reason"
                            ]
                        },
                        "minItems": 1
                    }
                },
                "required": [
                    "suggestions"
                ]
            }
            """)!;
        }
    }
}
