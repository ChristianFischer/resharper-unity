using UnityEngine;

namespace ContextActionsTests
{
    public class ContextActionsTests : MonoBehaviour
    {
        private void Update()
        {
            GetComponent<int>();
            PlainExpensiveMethod();
            CommentExpensiveMethod();
            TransitiveExpensiveeMethod();
        } 

        private void TransitiveExpensiveeMethod()
        {
            PlainExpensiveMethod();
            CommentExpensi{caret}veMethod();
        }

        private void PlainExpensiveMethod()
        {
            GetComponent<int>();
        }

        // ReSharper restore Unity.ExpensiveCode
        private void CommentExpensiveMethod()
        {
        }
    }
}