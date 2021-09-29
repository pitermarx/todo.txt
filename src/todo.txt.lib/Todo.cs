using System;
using System.Collections.Generic;
using System.Text;

namespace todo.txt.lib
{
    public record Todo(
        bool IsCompleted,
        char? Priority,
        DateTime? CompletedAt,
        DateTime? CreatedAt,
        string Description,
        string[] Projects,
        string[] Contexts,
        IReadOnlyDictionary<string, string> Metadata)
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (IsCompleted) sb.Append("x ");
            if (Priority.HasValue) sb.AppendFormat("({0}) ", Priority.Value);
            if (CompletedAt.HasValue) sb.AppendFormat("{0:yyyy-MM-dd} ", CompletedAt.Value);
            if (CreatedAt.HasValue) sb.AppendFormat("{0:yyyy-MM-dd} ", CreatedAt.Value);
            return sb.Append(Description).ToString();
        }
    }
}
