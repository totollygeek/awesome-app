using System;
using Figgle;

namespace TOTOllyGeek.Awesome.Lib
{
    /// <summary>
    /// A class for wrapping text and Figgle library to produce a FIGlet.
    /// </summary>
    public class FigMe
    {
        private readonly string _text;
        
        /// <summary>
        /// Create an instance of a text that will be turned into a Figlet.
        /// </summary>
        /// <param name="text">Text that you want to convert</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="text"/> is null.</exception>
        public FigMe(string text)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Generate figlet for the text passed in the constructor
        /// </summary>
        /// <returns>Figlet text</returns>
        public override string ToString()
        {
            return FiggleFonts.Standard.Render(_text);
        }
    }
}