using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using todo.txt.lib;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace todo.txt.tests;

[UsesVerify]
public class TodoParserTests
{
    private static VerifySettings Settings(Action<VerifySettings> configure = null)
    {
        var s = new VerifySettings();
        s.UseDirectory("snapshots");
        s.DisableDiff();
        s.ModifySerialization(_ => _.DontScrubDateTimes());
        configure?.Invoke(s);
        return s;
    }

    [Fact]
    public Task TestRecurrence()
    {
        var todos = new TodoParser()
            .ParseFile(@"data\recurrences.txt")
            .Select(t => t.Recurrence())
            .Select(r => new {rec=r, next=r?.Next()});
        return Verifier.Verify(todos, Settings());
    }

    [Theory]
    [InlineData(@"data\todo.txt")]
    [InlineData(@"data\todo_large.txt")]
    public Task TestFile(string file)
    {
        var todos = new TodoParser().ParseFile(file);
        return Verifier.Verify(todos, Settings(s => s.UseParameters(file)));
    }

    [Theory]
    [InlineData(@"data\todo.txt")]
    [InlineData(@"data\todo_large.txt")]
    public void TestFileLines(string file)
    {
        var parser = new TodoParser();
        foreach (var line in File.ReadAllLines(file))
        {
            Assert.Equal(line, parser.ParseLine(line).ToString());
        }
    }
}