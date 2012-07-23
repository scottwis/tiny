namespace Tiny.Parsing
{
    //# Defines a wrapper around a parser that specifies a look ahead string with a size > 1.
    class LookAheadWrapper<TInput, TSourceState, TResultState> : IParser<TInput, TSourceState, TResultState>
    {
        readonly IParser<TInput, TSourceState, TResultState> m_realParser;
        readonly TInput[] m_inputs;

        public LookAheadWrapper(IParser<TInput, TSourceState, TResultState> realParser, TInput[] inputs)
        {
            m_realParser = realParser.CheckNotNull("realParser");
            m_inputs = inputs.CheckNotNull("realParser");
        }

        public TInput LookAhead
        {
            get { return m_inputs[0]; }
        }

        public IParseState<TInput, TResultState> Parse(IParseState<TInput, TSourceState> state)
        {
            return m_realParser.Parse(state);
        }
    }
}
