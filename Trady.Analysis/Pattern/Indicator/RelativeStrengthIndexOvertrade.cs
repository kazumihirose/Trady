﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Indicator;
using Trady.Analysis.Infrastructure;
using Trady.Analysis.Pattern.State;
using Trady.Core;

namespace Trady.Analysis.Pattern.Indicator
{
    public class RelativeStrengthIndexOvertrade<TInput, TOutput> : AnalyzableBase<TInput, decimal, Overtrade?, TOutput>
    {
        readonly RelativeStrengthIndexByTuple _rsi;

        public RelativeStrengthIndexOvertrade(IEnumerable<TInput> inputs, Func<TInput, decimal> inputMapper, Func<TInput, Overtrade?, TOutput> outputMapper, int periodCount) : base(inputs, inputMapper, outputMapper)
        {
			_rsi = new RelativeStrengthIndexByTuple(inputs.Select(inputMapper), periodCount);
		}

        protected override Overtrade? ComputeByIndexImpl(IEnumerable<decimal> mappedInputs, int index)
			=> StateHelper.IsOvertrade(_rsi[index]);
	}

    public class RelativeStrengthIndexOvertradeByTuple : RelativeStrengthIndexOvertrade<decimal, Overtrade?>
    {
        public RelativeStrengthIndexOvertradeByTuple(IEnumerable<decimal> inputs, int periodCount) 
            : base(inputs, i => i, (i, otm) => otm, periodCount)
        {
        }
    }

    public class RelativeStrengthIndexOvertrade : RelativeStrengthIndexOvertrade<Candle, AnalyzableTick<Overtrade?>>
    {
        public RelativeStrengthIndexOvertrade(IEnumerable<Candle> inputs, int periodCount) 
            : base(inputs, i => i.Close, (i, otm) => new AnalyzableTick<Overtrade?>(i.DateTime, otm), periodCount)
        {
        }
    }
}