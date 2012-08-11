using System.Collections.Generic;
using System.Linq;

namespace Tiny.Parsing
{
    class TableParser
    {
        public static TableParser<TInput, TSourceState, TResultState> Create<TInput, TSourceState, TResultState>(
            TInput lookAhead,
            IEnumerable<IParser<TInput, TSourceState, TResultState>> parsers
        )
        {
            return new TableParser<TInput, TSourceState, TResultState>(lookAhead, parsers);
        }
    }

    class TableParser<TInput, TSourceState, TResultState> : IParser<TInput, TSourceState, TResultState>
    {
        readonly TInput m_lookahead;
        readonly IReadOnlyDictionary<TInput, IParser<TInput, TSourceState, TResultState>> m_table;

        public TableParser(TInput lookAhead, IEnumerable<IParser<TInput, TSourceState, TResultState>> parsers)
        {
            m_lookahead = lookAhead;
            m_table = parsers.ToDictionary(x => x.LookAhead, x => x).AsReadOnly();
        }

        public TInput LookAhead
        {
            get { return m_lookahead; }
        }

        public IParseState<TInput, TResultState> Parse(IParseState<TInput, TSourceState> state)
        {
            if (state.RemainingInput.Count > 0) {
                IParser<TInput, TSourceState, TResultState> parser;
                if (m_table.TryGetValue(state.RemainingInput[0], out parser)) {
                    return parser.Parse(state);
                }
            }
            return null;
        }
    }
}
