using Microsoft.VisualStudio.TestTools.UnitTesting;
using hangman_cs;
using System.Reflection;
using System;
using System.Linq;

namespace tests {

[TestClass]
public class UnitTest1 {

    static internal MethodInfo GetPrivateMethod<InstanceType>(
        InstanceType instance, in string methodName) {
        var type = instance.GetType();
        var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        return type.GetMethod(methodName, bindingAttr);
    }

    static internal ReturnType CallPrivateMethod<InstanceType, ReturnType>(
        InstanceType instance, in string methodName, params object[] parameters) {
        var method = GetPrivateMethod(instance, methodName);

        return (ReturnType)method.Invoke(instance, parameters);
    }

    static internal void CallVoidPrivateMethod<InstanceType>(
        InstanceType instance, in string methodName, params object[] parameters) {
        GetPrivateMethod(instance, methodName).Invoke(instance, parameters);
    }


    [TestMethod]
    public void AfterSelectRandomWordSelectedMembersShouldNotBeEmpty() {
        var game = new HangmanGame();
        CallVoidPrivateMethod(game, "selectRandomWord", null);

        Assert.IsFalse(string.IsNullOrEmpty(game.SelectedWord));
        Assert.IsFalse(string.IsNullOrEmpty(game.SelectedCetegory));
    }


    [DataTestMethod]
    [DataRow("")]
    [DataRow(null)]
    [DataRow("ab")]
    [DataRow("8")]
    public void ValidateInputReturn0(in string input) {
        var actual = CallPrivateMethod<HangmanGame, char>(new HangmanGame(), "validateInput", input);

        Assert.AreEqual('\0', actual);
    }

    [DataTestMethod]
    [DataRow("a")]
    [DataRow("Z")]
    public void ValidateInputReturnLowerCase(in string input) {
        var actual = CallPrivateMethod<HangmanGame, char>(new HangmanGame(), "validateInput", input);
        var expected = Char.ToLower(input.Last());

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ValidateInputReturn0IfGuessedBefore() {
        string input = "y";
        var game = new HangmanGame();
        var actual = CallPrivateMethod<HangmanGame, char>(game, "validateInput", input);

        Assert.AreEqual(input.Last(), actual);

        actual = CallPrivateMethod<HangmanGame, char>(game, "validateInput", input);

        Assert.AreEqual('\0', actual);
    }


    [TestMethod]
    public void UpdateDisplayReturnTrueIfInSelectedWord() {
        var game = new HangmanGame();
        CallVoidPrivateMethod(game, "newGame", null);

        foreach (char c in game.SelectedWord) {
            Assert.IsTrue(CallPrivateMethod<HangmanGame, bool>(game, "updateDisplay", c));
        }
    }

    [TestMethod]
    public void UpdateDisplayReturnFalseIfNotInSelectedWord() {
        var game = new HangmanGame();
        CallVoidPrivateMethod(game, "newGame", null);

        for (char c = 'a'; c <= 'z'; ++c) {
            if (!game.SelectedWord.Contains(c)) {
                Assert.IsFalse(CallPrivateMethod<HangmanGame, bool>(game, "updateDisplay", c));
            }
        }
    }

}

}
