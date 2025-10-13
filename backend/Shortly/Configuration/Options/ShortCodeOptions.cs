using System.ComponentModel.DataAnnotations;

using NanoidDotNet;

namespace Shortly.Configuration.Options;

public class ShortCodeOptions
{
    [StringLength(maximumLength: 256, MinimumLength = 6)]
    public string Alphabet { get; set; } = Nanoid.Alphabets.LowercaseLettersAndDigits;

    [Range(6, 32)]
    public int Length { get; set; } = 8;
}
