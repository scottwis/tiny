using Tiny.Collections;

namespace Tiny.Parsing
{
    class ParseState
    {
        public static ParseState<TInput, TResult> Create<TInput, TResult>(
            ISubList<TInput> consumedInput,
            ISubList<TInput> remainingInput,
            TResult result
        )
        {
            return new ParseState<TInput, TResult>(consumedInput, remainingInput, result);
        }
    }
    class ParseState<TInput, TResult> : IParseState<TInput, TResult>
    {
        readonly ISubList<TInput> m_consumedInput;
        readonly ISubList<TInput> m_remainingInput;
        readonly TResult m_result;

        public ParseState(ISubList<TInput> consumedInput, ISubList<TInput> remainingInput, TResult result)
        {
            m_consumedInput = consumedInput;
            m_remainingInput = remainingInput;
            m_result = result;
        }

        public ISubList<TInput> ConsumedInput
        {
            get { return m_consumedInput; }
        }

        public ISubList<TInput> RemainingInput
        {
            get { return m_remainingInput;  }
        }

        public TResult Result
        {
            get { return m_result; }
        }
    }
}