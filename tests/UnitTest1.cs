using Microsoft.VisualStudio.TestTools.UnitTesting;
using hangman_cs;
using System.Reflection;
using System;
using System.Linq;

namespace tests {

[TestClass]
public class UnitTest1 {

    static internal MethodInfo GetPrivateMethod<InstanceType>(
        in InstanceType instance, in string methodName) {
        var type = instance.GetType();
        var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        return type.GetMethod(methodName, bindingAttr);
    }

    static internal void AssertPrivateMethod<InstanceType, ReturnType>(
        in ReturnType expected,
        in InstanceType instance, in string methodName, params object[] parameters) {
        var method = GetPrivateMethod(instance, methodName);
        var actual = (ReturnType)method.Invoke(instance, parameters);

        Assert.AreEqual(expected, actual);
    }

    static internal void CallVoidPrivateMethod<InstanceType>(
        InstanceType instance, in string methodName, params object[] parameters) {
        GetPrivateMethod(instance, methodName).Invoke(instance, parameters);
    }


    [TestMethod]
    public void AfterSelectRandomWordSelectedMembersShouldNotBeEmpty() {
        var game = new HangmanGame();
        CallVoidPrivateMethod(game, "selectRandomWord");

        Assert.IsFalse(string.IsNullOrEmpty(game.SelectedWord));
        Assert.IsFalse(string.IsNullOrEmpty(game.SelectedCetegory));
    }


    [DataTestMethod]
    [DataRow("")]
    [DataRow(null)]
    [DataRow("ab")]
    [DataRow("8")]
    public void ValidateInputReturn0(in string input) {
        AssertPrivateMethod(new Nullable<int>(), new HangmanGame(), "validateInput", input);
    }

    [DataTestMethod]
    [DataRow("a")]
    [DataRow("Z")]
    public void ValidateInputReturnLowerCase(in string input) {
        var expected = Char.ToLower(input.Last());
        AssertPrivateMethod(expected, new HangmanGame(), "validateInput", input);
    }

    [TestMethod]
    public void ValidateInputReturn0IfGuessedBefore() {
        string input = "y";
        var game = new HangmanGame();
        AssertPrivateMethod(input.Last(), game, "validateInput", input);

        AssertPrivateMethod(new Nullable<int>(), game, "validateInput", input);
    }


    [TestMethod]
    public void UpdateDisplayReturnTrueIfInSelectedWord() {
        var game = new HangmanGame();
        CallVoidPrivateMethod(game, "newGame");

        foreach (char c in game.SelectedWord) {
            AssertPrivateMethod(true, game, "updateDisplay", c);
        }
    }

    [TestMethod]
    public void UpdateDisplayReturnFalseIfNotInSelectedWord() {
        var game = new HangmanGame();
        CallVoidPrivateMethod(game, "newGame");

        for (char c = 'a'; c <= 'z'; ++c) {
            if (!game.SelectedWord.Contains(c)) {
                AssertPrivateMethod(false, game, "updateDisplay", c);
            }
        }
    }

}

}
