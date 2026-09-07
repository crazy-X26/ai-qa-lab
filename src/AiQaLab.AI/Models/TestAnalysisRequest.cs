using Anthropic.Models.Beta.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiQaLab.AI.Models
{
    public class TestAnalysisRequest
    {
        public required string Requirement { get; init; }
        public IReadOnlyList<CodeContextItem> CodeContext { get; init; } = [];
    }

    public class CodeContextItem
    {
        public required string Path { get; init; }
        public required CodeContextKind Kind { get; init; }
        public required string Content { get; init; }
    }

    public enum  CodeContextKind
    {
        SourceCode,
        Test
    }
}
