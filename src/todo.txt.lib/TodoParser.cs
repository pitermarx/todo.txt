using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace todo.txt.lib
{
    public sealed class TodoParser
    {
        public IEnumerable<Todo> ParseFile(string path) => File.ReadAllLines(path).Select(ParseLine);

        public Todo ParseLine(string line)
        {
            var position = 0;
            bool isCompleted = false;
            char? priority = null;

            // Assume it's really just one line
            var parts = Split(line?.Trim(), ' ');

            // Ensure it's not empty
            if (parts is null or { Length: 0 })
            {
                return null;
            }

            // if completed, starts with lowercase x
            if (Get(position) == "x")
            {
                position += 1;
                isCompleted = true;
            }

            // If priority is set, is right after x (when completed) or at beginning (when incomplete)
            // Priority is an upercase A-Z char enclosed in parentheses
            var maybePrio = Get(position);
            if (maybePrio.Length == 3 &&
                maybePrio[0] == '(' && maybePrio[2] == ')' &&
                maybePrio[1] >= 'A' && maybePrio[1] <= 'Z')
            {
                position += 1;
                priority = maybePrio[1];
            }

            // If completion date is set, there are 2 dates defined (completion and creation)
            // Creation date may be set without completion date
            var date1 = Get(position).ParseDate();
            var date2 = Get(position + 1).ParseDate();
            var createdAt = date2 == null ? date1 : date2;
            var completedAt = date2 == null ? null : date1;
            position += date1 == null ? 0 :
                        date2 == null ? 1 : 2;

            // TODO: Optimize this part without Linq

            // the rest is the description
            // any word that starts with + is a project
            // any word that starts with @ is a context
            var description = parts.Skip(position);
            var projects = description.Where(p => p.StartsWith('+')).Select(p => p[1..]).ToArray();
            var contexts = description.Where(p => p.StartsWith('@')).Select(p => p[1..]).ToArray();

            // extra metadata is defined by the key:value syntax
            //    - should it be a dictionary or a lookup?
            //    - can i define 2 times the same key?
            //       - currently get the last value
            var metadata = description
                .Select(p => Split(p, ':'))
                .Where(m => m.Length == 2)
                .GroupBy(m => m[0])
                .ToDictionary(m => m.Key, m => m.Last()[1]);

            return new Todo(
                isCompleted,
                priority,
                completedAt,
                createdAt,
                string.Join(' ', description),
                projects,
                contexts,
                metadata);

            static string[] Split(string src, char split) => src?.Split(split, StringSplitOptions.RemoveEmptyEntries);
            string Get(int pos) => parts.Length <= pos ? null : parts[pos];
        }
    }
}
