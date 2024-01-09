using System.Collections.Generic;

namespace Common
{
    public class FontPackModel
    {
	    public string Name { get; set; }
		public FontModel FallbackCharacter { get; set; }
		public Dictionary<char, FontModel> CharMap { get; set; }
    }
}
