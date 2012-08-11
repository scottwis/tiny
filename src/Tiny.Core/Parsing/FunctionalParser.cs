namespace Tiny.Parsing
{
    delegate IParseState<TInput, TResultState> ParseFunc<TInput, in TSourceState, out TResultState>(
        IParseState<TInput, TSourceState> state
    );

    class FunctionalParser
    {
        public static FunctionalParser<TInput, TSourceState, TResultState> Create<TInput, TSourceState, TResultState>(
            ParseFunc<TInput, TSourceState, TResultState> func,
            TInput lookAhead
        )
        {
            return new FunctionalParser<TInput, TSourceState, TResultState>(func, lookAhead);
        }
    }

    class FunctionalParser<TInput, TSourceState, TResultState> : IParser<TInput, TSourceState, TResultState>
    {
        readonly ParseFunc<TInput, TSourceState, TResultState> m_func;
        readonly TInput m_lookAhead;

        public FunctionalParser(ParseFunc<TInput, TSourceState, TResultState> func, TInput lookAhead)
        {
            m_lookAhead = lookAhead;
            m_func = func;
        }

        public TInput LookAhead
        {
            get { return m_lookAhead; }
        }

        public IParseState<TInput, TResultState> Parse(IParseState<TInput, TSourceState> state)
        {
            return m_func(state);
        }
    }
}
