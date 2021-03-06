﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Infrastructure;
using Trady.Core;

namespace Trady.Analysis.Indicator
{
    public class ExponentialMovingAverageOscillator<TInput, TOutput> : AnalyzableBase<TInput, decimal, decimal?, TOutput>
    {
        readonly ExponentialMovingAverageByTuple _ema2;
        readonly ExponentialMovingAverageByTuple _ema1;

        public ExponentialMovingAverageOscillator(IEnumerable<TInput> inputs, Func<TInput, decimal> inputMapper, Func<TInput, decimal?, TOutput> outputMapper, int periodCount1, int periodCount2) : base(inputs, inputMapper, outputMapper)
        {
			_ema1 = new ExponentialMovingAverageByTuple(inputs.Select(inputMapper), periodCount1);
			_ema2 = new ExponentialMovingAverageByTuple(inputs.Select(inputMapper), periodCount2);

			PeriodCount1 = periodCount1;
			PeriodCount2 = periodCount2;
        }

        public int PeriodCount1 { get; }

        public int PeriodCount2 { get; }

        protected override decimal? ComputeByIndexImpl(IEnumerable<decimal> mappedInputs, int index) => _ema1[index] - _ema2[index];
    }

    public class ExponentialMovingAverageOscillatorByTuple : ExponentialMovingAverageOscillator<decimal, decimal?>
    {
        public ExponentialMovingAverageOscillatorByTuple(IEnumerable<decimal> inputs, int periodCount1, int periodCount2) 
            : base(inputs, i => i, (i, otm) => otm, periodCount1, periodCount2)
        {
        }
    }

    public class ExponentialMovingAverageOscillator : ExponentialMovingAverageOscillator<Candle, AnalyzableTick<decimal?>>
    {
        public ExponentialMovingAverageOscillator(IEnumerable<Candle> inputs, int periodCount1, int periodCount2) 
            : base(inputs, i => i.Close, (i, otm) => new AnalyzableTick<decimal?>(i.DateTime, otm), periodCount1, periodCount2)
        {
        }
    }
}