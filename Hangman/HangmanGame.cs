using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hangman
{
    class HangmanGame
    {
        /// <summary>
        /// Reference on max guesses
        /// </summary>
        private const int MaxGuesses = 10;

        /// <summary>
        /// Used as our random generator to choose random words
        /// </summary>
        private Random rng;

        /* Game Data */
        /// <summary>
        /// How many guesses the player has left
        /// </summary>
        private int guessesLeft;

        /// <summary>
        /// Array of correctly guesses characters based on the secret word
        /// </summary>
        private char[] correctChars;

        /// <summary>
        /// StringBuilder object that creates our string of incorrectly guessed characters
        /// </summary>
        private StringBuilder incorrectCharsSB;

        /// <summary>
        /// Our HangmanDrawer object that draws our hangman based on guesses
        /// </summary>
        private HangmanDrawer hmDrawer;
        /* End of Game Data */

        /// <summary>
        /// Where our file that contains list of words is
        /// </summary>
        private string wordlistPath;

        /// <summary>
        /// All of the words that get loaded from file
        /// </summary>
        private string[] words;

        /// <summary>
        /// Should the game be run in debug mode?
        /// If true shows the secret word to user
        /// </summary>
        private bool debug;

        /// <summary>
        /// Create a new hangman console game object
        /// </summary>
        /// <param name="wordlistPath">Path to wordlist. If only the name and extension are provided e.g. (e.g. "wordlist.txt")
        /// it will look for it in the same folder as the program</param>
        /// <param name="debug">Should this game be run in debug mode? If true shows the secret word</param>
        public HangmanGame(string wordlistPath, bool debug = false)
        {
            rng = new Random();
            this.wordlistPath = wordlistPath;
            hmDrawer = new HangmanDrawer();
            incorrectCharsSB = new StringBuilder();
            
            // Gets all the possible words that we can use from file
            words = GetWordsFromFile(wordlistPath);
            
            this.debug = debug;
        }

        /// <summary>
        /// Main Run method of the game, call this to start the game
        /// </summary>
        public void Run()
        {
            // Check if we got any words, if not write error and end game
            if (words.Length == 0)
            {
                Console.WriteLine("Could not get any words from file, " +
                                  "make sure there are words in the file and that they are comma separated.");
                Console.WriteLine("\n Press any key to continue");
                Console.ReadKey();
                return;
            }
            
            bool wantsToPlay = true;

            // While player wants to play, keep replaying game
            while (wantsToPlay)
            {
                // Get secret word from wordlist
                string secretWord = words[rng.Next(0, words.Length)].ToUpper();

                // Make sure game data is reset, this lets us play again
                correctChars = new char[secretWord.Length];
                guessesLeft = MaxGuesses;
                incorrectCharsSB.Clear();
                
                // Gameplay Loop
                while (!WordIsGuessed(correctChars))
                {
                    // If we don't have guesses left, break gameplay loop
                    if (guessesLeft <= 0)
                        break;
                    
                    UpdateGame(secretWord);

                    Console.Write("\nGuess: ");
                    // Note: always makes input uppercase
                    string input = Console.ReadLine().ToUpper();

                    // User guessed one character
                    if (input.Length == 1)
                    {
                        if (char.TryParse(input, out char charInput) && char.IsLetter(charInput))
                        {
                            // Check if secret word contains char
                            if (secretWord.Contains(charInput))
                            {
                                PlaceGuessedLetters(ref correctChars, secretWord, charInput);
                            }
                            // Check if we have already guessed the char
                            // and if not, add to incorrect guesses
                            else if (!incorrectCharsSB.ToString().Contains(charInput))
                            {
                                incorrectCharsSB.Append(charInput);
                                guessesLeft--;
                            }
                        }
                    }
                    // User tried to guess the whole word
                    else if (input.Length == secretWord.Length)
                    {
                        if (input.ToUpper() == secretWord)
                        {
                            // Game won
                            correctChars = input.ToArray();
                        }
                        else
                        {
                            guessesLeft--;
                        }
                    }
                } // Word is guessed or player is out of guesses

                // Update our game data on screen
                UpdateGame(secretWord);

                // Check if we guessed our word or if we lost
                if (WordIsGuessed(correctChars))
                {
                    Console.WriteLine("Congratulations! You have won!");
                }
                else
                {
                    Console.WriteLine("Unlucky, you lost the game! ):");
                    Console.WriteLine("The secret word was: " + secretWord);
                }

                // Ask if the player wants to play again
                char choice;
                do
                {
                    Console.Write("Would you like to play again? (Y/N): ");
                } while (!char.TryParse(Console.ReadLine().ToUpper(), out choice));

                if (choice != 'Y')
                    wantsToPlay = false;
            }
        }

        /// <summary>
        /// Updates the game by clearing console and re-printing
        /// game data, including the hangman
        /// </summary>
        private void UpdateGame(string secretWord)
        {
            Console.Clear();

            // Draw our hangman
            hmDrawer.Draw(MaxGuesses - guessesLeft);

            if (debug)
                Console.WriteLine("Secret word is " + secretWord);
            
            Console.Write("Guesses left: " + guessesLeft + "\n\n");

            // Write incorrect characters
            Console.WriteLine("Incorrect characters: ");
            Console.WriteLine(incorrectCharsSB.ToString().ToUpper() + "\n\n");

            // Print out correctly guessed characters
            for (int i = 0; i < correctChars.Length; i++)
            {
                if (char.IsLetter(correctChars[i]))
                {
                    Console.Write($"{correctChars[i]} ");
                }
                else
                {
                    Console.Write("_ ");
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Gets comma separated strings from file
        /// </summary>
        /// <param name="pathToFile">Path to the file we want to try and get words from</param>
        /// <returns></returns>
        private string[] GetWordsFromFile(string pathToFile)
        {
            using (StreamReader sr = new StreamReader(pathToFile))
            {
                try
                {
                    string allWords = sr.ReadToEnd();

                    if (allWords == string.Empty)
                        return Array.Empty<string>();
                    else
                        return allWords.Split(',');
                }
                catch (OutOfMemoryException e)
                {
                    Console.WriteLine(e);
                }
                catch (IOException e)
                {
                    Console.WriteLine("File might not exist at: " + pathToFile);
                    Console.WriteLine(e);
                }
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Places a char into char array based on indexes in a referenced string
        /// </summary>
        /// <param name="charArray">Array that the char is going be placed in</param>
        /// <param name="stringWord">Reference string for indexes</param>
        /// <param name="charToPlace">Char to we want to use to compare against stringWord and place into the array</param>
        private void PlaceGuessedLetters(ref char[] charArray, string stringWord, char charToPlace)
        {
            for (int i = 0; i < stringWord.Length; i++)
            {
                if (stringWord[i] == charToPlace)
                {
                    charArray[i] = charToPlace;
                }
            }
        }

        /// <summary>
        /// Checks if every char in the array is a letter
        /// </summary>
        /// <param name="charArray">Char array that we want to check for letters</param>
        /// <returns>Returns true if every element in the array is a letter</returns>
        private bool WordIsGuessed(char[] charArray)
        {
            for (int i = 0; i < charArray.Length; i++)
            {
                if (!char.IsLetter(charArray[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}