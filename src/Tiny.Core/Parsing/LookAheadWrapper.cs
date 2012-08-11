using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tiny.Parsing
{
    //# Defines a wrapper around a parser that specifies a look ahead string with a size > 1.
    class LookAheadWrapper<TInput, TSourceState, TResultState> : IParser<TInput, TSourceState, TResultState>
    {
        [NotNull] readonly IReadOnlyList<TInput> m_inputs;

        public LookAheadWrapper(IReadOnlyList<TInput> inputs)
        {
            if (inputs.CheckNotNull("realParser").Count < 1) {
                throw new ArgumentException("Inputs should contain at least one element.");
            }
            m_inputs = inputs;
        }

        public TInput LookAhead
        {
            get { return m_inputs[0]; }
        }

        public IReadOnlyList<TInput> Inputs
        {
            get { return m_inputs; }
        }

        public IParseState<TInput, TResultState> Parse(IParseState<TInput, TSourceState> state)
        {
            throw new NotSupportedException("All instances of LookAheadWrapper should be erased prior to parsing.");
        }

        public LookAheadWrapper<TInput, TSourceState, TResultState> SubList(int count)
        {
            return new LookAheadWrapper<TInput, TSourceState, TResultState>(m_inputs.SubList(count));
        }
    }
}
