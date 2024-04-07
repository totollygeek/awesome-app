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
        private readonly FiggleFont _font;
        
        /// <summary>
        /// Create an instance of a text that will be turned into a Figlet.
        /// </summary>
        /// <param name="text">Text that you want to convert</param>
        /// <param name="font">Font that you want to use</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="text"/> is null.</exception>
        public FigMe(string text, FiggleFont font = FiggleFont.Standard)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            _font = font;
        }

        /// <summary>
        /// Generate figlet for the text passed in the constructor
        /// </summary>
        /// <returns>Figlet text</returns> 
        /// <exception cref="ArgumentOutOfRangeException">Throws if font supplied in constructor is not supported</exception>
        public override string ToString()
        {
            return _font switch
            {
                FiggleFont.Standard => FiggleFonts.Standard.Render(_text),
                FiggleFont.Graffiti => FiggleFonts.Graffiti.Render(_text),
                FiggleFont.ThreePoint => FiggleFonts.ThreePoint.Render(_text),
                FiggleFont.Ogre => FiggleFonts.Ogre.Render(_text),
                FiggleFont.Rectangles => FiggleFonts.Rectangles.Render(_text),
                FiggleFont.Slant => FiggleFonts.Slant.Render(_text),
                FiggleFont.ThreeDDiagonal => FiggleFonts.ThreeDDiagonal.Render(_text),
                FiggleFont.Alpha => FiggleFonts.Alpha.Render(_text),
                _ => throw new ArgumentOutOfRangeException($"Font type \"{_font}\" is not supported")
            };
        }
    }
}