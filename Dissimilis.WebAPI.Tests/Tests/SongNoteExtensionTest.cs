using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Dissimilis.WebAPI.Extensions.Models;

namespace Dissimilis.WebAPI.xUnit.Tests
{
    [Collection("Serial")]
    [CollectionDefinition("Serial", DisableParallelization = true)]
    public class SongNoteExtensionTest
    {
        [Fact]
        public void TestGetNoteValues()
        {
            string[] chordNamesToTest = new string[]
            {
                "C",
                "Cmaj7",
                "D",
                "E",
                "F",
                "Dm",
                "Faug7",
            };
            List<List<string>> expectedNoteValues = new List<List<string>>()
            {
                new List<string> { "C", "E", "G" },
                new List<string> { "C", "E", "G", "H" },
                new List<string> { "D", "F#", "A" },
                new List<string> { "E", "G#", "H" },
                new List<string> { "F", "A", "C" },
                new List<string> { "D", "F", "A" },
                new List<string> { "F", "A", "C#", "D#" },
            };

            for (int index = 0; index < chordNamesToTest.Length; index++)
            {
                SongNoteExtension.GetNoteValues(chordNamesToTest[index]).ShouldBeEquivalentTo(expectedNoteValues[index], $"Note values were not correct for chord {chordNamesToTest[index]}");
            }
        }
    }
}
