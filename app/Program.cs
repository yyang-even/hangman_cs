﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace hangman_cs {

public class HangmanGame {
    private static readonly IDictionary<string, string[]> WORDS_DICTIONARY =
    new Dictionary<string, string[]>() {
        {"a fruit", new[] {"apple", "apricot", "avocado", "banana", "blackberry", "blackcurrant", "blueberry", "boysenberry", "cherry", "coconut", "fig", "grape", "grapefruit", "kiwifruit", "lemon", "lime", "lychee", "mandarin", "mango", "melon", "nectarine", "orange", "papaya", "passion fruit", "peach", "pear", "pineapple", "plum", "pomegranate", "quince", "raspberry", "strawberry", "watermelon"}
        },
        {"a C# keyword", new[] {"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set"}},
        {"an animal", new[] {"alligator", "ant", "bear", "bee", "bird", "camel", "cat", "cheetah", "chicken", "chimpanzee", "cow", "crocodile", "deer", "dog", "dolphin", "duck", "eagle", "elephant", "fish", "fly", "fox", "frog", "giraffe", "goat", "goldfish", "hamster", "hippopotamus", "horse", "kangaroo", "kitten", "lion", "lobster", "monkey", "octopus", "owl", "panda", "pig", "puppy", "rabbit", "rat", "scorpion", "seal", "shark", "sheep", "snail", "snake", "spider", "squirrel", "tiger", "turtle", "wolf", "zebra", "alligator", "ant", "bear", "bee", "bird", "camel", "cat", "cheetah", "chicken", "chimpanzee", "cow", "crocodile", "deer", "dog", "dolphin", "duck", "eagle", "elephant", "fish", "fly", "fox", "frog", "giraffe", "goat", "goldfish", "hamster", "hippopotamus", "horse", "kangaroo", "kitten", "lion", "lobster", "monkey", "octopus", "owl", "panda", "pig", "puppy", "rabbit", "rat", "scorpion", "seal", "shark", "sheep", "snail", "snake", "spider", "squirrel", "tiger", "turtle", "wolf", "zebra"}}
    };


    public string SelectedWord {
        get;
        private set;
    }
    public string SelectedCetegory {
        get;
        private set;
    }


    private static readonly Random random = new Random();
    private const char MASK_CHAR = '_';

    private void newGame() {
        selectRandomWord();

        guessed_chars.Clear();

        var masked_word = new string(MASK_CHAR, SelectedWord.Length);
        display_word = String.Join(' ', masked_word.ToCharArray()).ToCharArray();
    }

    private void selectRandomWord() {
        var word_length_prefix_sums = new int[WORDS_DICTIONARY.Count];
        int prefix_sum = 0;
        for (int i = 0; i < WORDS_DICTIONARY.Count; ++i) {
            prefix_sum += WORDS_DICTIONARY.ElementAt(i).Value.Length;
            word_length_prefix_sums[i] = prefix_sum;
        }

        int selected_word_index = random.Next(word_length_prefix_sums.Last());
        int cetegory_index = 0;
        for (; cetegory_index < word_length_prefix_sums.Length &&
             word_length_prefix_sums[cetegory_index] <= selected_word_index;
             ++cetegory_index);

        int offset =
            cetegory_index == 0 ?
            selected_word_index :
            (selected_word_index - word_length_prefix_sums[cetegory_index - 1]);

        SelectedWord = WORDS_DICTIONARY.ElementAt(cetegory_index).Value[offset];
        SelectedCetegory = WORDS_DICTIONARY.ElementAt(cetegory_index).Key;
    }


    private HashSet<char> guessed_chars = new HashSet<char>();

    private char? validateInput(in string input) {
        if (string.IsNullOrEmpty(input)) {
            Console.WriteLine("Empty input. Please enter a letter.");
            return null;
        }

        if (input.Length > 1) {
            Console.WriteLine("More than one characters. Please enter one letter only.");
            return null;
        }

        var input_lower_char = Char.ToLower(input.Last());

        if (input_lower_char < 'a' || input_lower_char > 'z') {
            Console.WriteLine("Input character is not a letter.");
            return null;
        }

        if (guessed_chars.Contains(input_lower_char)) {
            Console.WriteLine("Guessed before. Please try another letter.");
            return null;
        }
        guessed_chars.Add(input_lower_char);

        return input_lower_char;
    }

    private bool updateDisplay(in char guess) {
        Debug.Assert(SelectedWord.Length * 2 - 1 == display_word.Length);

        bool found = false;
        for (int i = 0; i < SelectedWord.Length; ++i) {
            if (guess == SelectedWord[i]) {
                found = true;
                display_word[i * 2] = guess;
            }
        }

        return found;
    }

    private const int MAX_RETRY = 10;
    private char[] display_word;

    public void Play() {
        Console.WriteLine("Hangman Game with Computer.");
        Console.WriteLine("Guess only one letter at a time. Please press 'Enter' key after each guess.");

        newGame();
        Console.WriteLine($"Word Length: {SelectedWord.Length}; Hint: {SelectedCetegory}.");
#if DEBUG
        Console.WriteLine($"Selected Word: {SelectedWord}.");
#endif

        bool win = false;
        for (int retry = MAX_RETRY; retry > 0;) {
            Console.WriteLine("\nGuess a letter:");
            var guess = validateInput(Console.ReadLine());
            if (guess != null) {
                --retry;

                if (updateDisplay(guess.Value)) {
                    if (!display_word.Contains(MASK_CHAR)) {
                        win = true;
                        break;
                    }
                }
            }
            Console.WriteLine($"Remaining guess: {retry}.");
            Console.WriteLine($"Letters history: {String.Join(' ', guessed_chars)}.");
            Console.WriteLine($"{new string(display_word)}");
        }

        Console.WriteLine($"Answer: {SelectedWord}.");
        if (win) {
            Console.WriteLine("You Win!!!");
        } else {
            Console.WriteLine("You lose!");
        }
    }
}

class Program {
    static void Main(string[] args) {
        var game = new HangmanGame();
        game.Play();
    }
}

}
