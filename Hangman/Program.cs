using System;
using System.Text;
using System.Linq;

namespace Hangman
{
    class Program
    {
        static void Main(string[] args)
        {
            HangmanGame hangmanGame = new HangmanGame("wordlist.txt");
            hangmanGame.Run();
        }
    }
}
