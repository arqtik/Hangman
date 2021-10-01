using System;
using System.Text;

namespace Hangman
{
    class HangmanDrawer
    {
        /// <summary>
        /// Height of the drawing area in characters
        /// </summary>
        private int height;
        
        /// <summary>
        /// Width of the drawing are in characters
        /// </summary>
        private int width;
        
        /// <summary>
        /// Width of the hanging man in characters
        /// </summary>
        private int manWidth;
        
        /// <summary>
        /// Padding toward the left for the "post"
        /// Essentially how many characters from the left is the middle of the base string
        /// </summary>
        private int postLeftPadding;


        /// <summary>
        /// Creates a new HangmanDrawer that can draw a hangman to console
        /// </summary>
        public HangmanDrawer()
        {
            height = 8;
            width = 10;
            manWidth = 3;
            postLeftPadding = 2;
        }

        /// <summary>
        /// Draws a hangman to console based on how many wrong guesses
        /// </summary>
        /// <param name="wrongGuesses">How many wrong guesses have been made, stops making a difference after 10</param>
        public void Draw(int wrongGuesses)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Drawing loop, Height
            for (int h = 0; h < height; h++)
            {
                // If last line, draw base
                if (h == height - 1 && wrongGuesses > 0)
                {
                    stringBuilder.Append(@"(---)");
                }
                // Otherwise draw the hangman guy
                else
                {
                    // Drawing loop, width
                    for (int w = 0; w < width; w++)
                    {
                        // If this is first line of the drawing
                        if (h == 0)
                        {
                            // If we are between the padding and middle of the hanging man
                            // and we have more than 2 wrong guesses, draw the top post
                            if (w > postLeftPadding && w < width - (manWidth / 2) && wrongGuesses > 2)
                            {
                                stringBuilder.Append('_');
                            }
                            else
                            {
                                stringBuilder.Append(" ");
                            }
                        }
                        else
                        {
                            // If we are between left border
                            // and where the hangman should be
                            if (w < (width - 1) - manWidth)
                            {
                                // If w == padding draw the post
                                if (w == postLeftPadding && wrongGuesses > 1)
                                {
                                    stringBuilder.Append('|');
                                }
                                // Otherwise just add empty space
                                else
                                {
                                    stringBuilder.Append(' ');
                                }
                            }
                            // Build the body
                            else
                            {
                                switch (h)
                                {
                                    // Rope height
                                    case 1:
                                        // Print the rope of the hanging man if we are in position to do so
                                        // and we have the right amount of wrong guesses
                                        if (w == width - manWidth + (manWidth / 2) && wrongGuesses > 3)
                                        {
                                            stringBuilder.Append("|");
                                        }
                                        else
                                        {
                                            stringBuilder.Append(" ");
                                        }
                                        break;
                                    // Head height
                                    case 2:
                                        // Print the head of the hanging man if we are in position to do so
                                        // and we have the right amount of wrong guesses
                                        if (w == width - manWidth + (manWidth / 2) && wrongGuesses > 4)
                                        {
                                            stringBuilder.Append("o");
                                        }
                                        else
                                        {
                                            stringBuilder.Append(" ");
                                        }
                                        break;
                                    // Chest / arms height
                                    case 3:
                                        // Print the left arm, chest and right arm of the hanging man if we are in position to do so
                                        // and we have the right amount of wrong guesses
                                        
                                        // Left arm
                                        if (w == width - manWidth && wrongGuesses > 6)
                                        {
                                            stringBuilder.Append(@"/");
                                        }
                                        // Chest
                                        else if (w == width - manWidth + (manWidth / 2) && wrongGuesses > 5)
                                        {
                                            stringBuilder.Append(@"|");
                                        }
                                        // Right arm
                                        else if (w == width - 1 && wrongGuesses > 7)
                                        {
                                            stringBuilder.Append(@"\");
                                        }
                                        else
                                        {
                                            stringBuilder.Append(" ");
                                        }
                                        break;
                                    // Leg height
                                    case 4:
                                        // Print the left and right leg of the hanging man if we are in position to do so
                                        // and we have the right amount of wrong guesses
                                        
                                        // Left leg
                                        if (w == width - manWidth && wrongGuesses > 8)
                                        {
                                            stringBuilder.Append(@"/");
                                        }
                                        // Right leg
                                        else if (w == width - 1 && wrongGuesses > 9)
                                        {
                                            stringBuilder.Append(@"\");
                                        }
                                        else
                                        {
                                            stringBuilder.Append(" ");
                                        }
                                        break;
                                    default:
                                        stringBuilder.Append(" ");
                                        break;
                                }
                            }
                        }
                    }
                }

                // Write out our line that we built
                Console.WriteLine(stringBuilder.ToString());
                
                // Clear StringBuilder for the next line of characters
                stringBuilder.Clear();
            }
            Console.WriteLine("#######################");
        }
    }
}
