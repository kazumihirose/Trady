﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Indicator;
using Trady.Analysis.Infrastructure;
using Trady.Analysis.Pattern.State;
using Trady.Core;

namespace Trady.Analysis.Pattern.Indicator
{
    public partial class StochasticsOscillatorTrend
    {
        public class Fast<TInput, TOutput> : IndicatorBase<TInput, TOutput>
        {
            public Fast(
                IEnumerable<TInput> inputs, 
                Func<TInput, (decimal High, decimal Low, decimal Close)> inputMapper, 
                Func<TInput, Trend?, TOutput> outputMapper, 
                int periodCount, 
                int smaPeriodCount) 
                : base(inputs, inputMapper, outputMapper, new Stochastics.FastByTuple(inputs.Select(inputMapper), periodCount, smaPeriodCount))
            {
            }
        }

        public class FastByTuple : Fast<(decimal High, decimal Low, decimal Close), Trend?>
        {
            public FastByTuple(IEnumerable<(decimal High, decimal Low, decimal Close)> inputs, int periodCount, int smaPeriodCount) 
                : base(inputs, i => i, (i, otm) => otm, periodCount, smaPeriodCount)
            {
            }
        }

        public class Fast : Fast<Candle, AnalyzableTick<Trend?>>
        {
            public Fast(IEnumerable<Candle> inputs, int periodCount, int smaPeriodCount) 
                : base(inputs, i => (i.High, i.Low, i.Close), (i, otm) => new AnalyzableTick<Trend?>(i.DateTime, otm), periodCount, smaPeriodCount)
            {
            }
        }
    }
}