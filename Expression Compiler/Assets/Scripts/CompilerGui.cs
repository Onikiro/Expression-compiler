using UnityEngine;
using UnityEngine.UI;

public class CompilerGui : MonoBehaviour
{

    [SerializeField] private Text _inputText;
    [SerializeField] private Text _resultText;

    public void OnClickGetResult()
    {
        _resultText.text = "Result: " + ExpressionCompiler.GetResult(_inputText.text);
    }
}