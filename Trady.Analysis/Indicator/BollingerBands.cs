﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Infrastructure;
using Trady.Core;

namespace Trady.Analysis.Indicator
{
    public class BollingerBands<TInput, TOutput> : AnalyzableBase<TInput, decimal, (decimal? LowerBand, decimal? MiddleBand, decimal? UpperBand), TOutput>
    {
        readonly SimpleMovingAverageByTuple _sma;
        readonly StandardDeviationByTuple _sd;

        public BollingerBands(IEnumerable<TInput> inputs, Func<TInput, decimal> inputMapper, Func<TInput, (decimal? LowerBand, decimal? MiddleBand, decimal? UpperBand), TOutput> outputMapper, int periodCount, decimal sdCount) : base(inputs, inputMapper, outputMapper)
        {
			_sma = new SimpleMovingAverageByTuple(inputs.Select(inputMapper), periodCount);
			_sd = new StandardDeviationByTuple(inputs.Select(inputMapper), periodCount);

			PeriodCount = periodCount;
			SdCount = sdCount;
        }

        public int PeriodCount { get; }

        public decimal SdCount { get; }

        protected override (decimal? LowerBand, decimal? MiddleBand, decimal? UpperBand) ComputeByIndexImpl(IEnumerable<decimal> mappedInputs, int index)
        {
			decimal? middleBand = _sma[index];
			decimal? sd = _sd[index];
			return (middleBand - SdCount * sd, middleBand, middleBand + SdCount * sd);
        }
    }

    public class BollingerBandsByTuple : BollingerBands<decimal, (decimal? LowerBand, decimal? MiddleBand, decimal? UpperBand)>
    {
        public BollingerBandsByTuple(IEnumerable<decimal> inputs, int periodCount, decimal sdCount) 
            : base(inputs, i => i, (i, otm) => otm, periodCount, sdCount)
        {
        }
    }

    public class BollingerBands : BollingerBands<Candle, AnalyzableTick<(decimal? LowerBand, decimal? MiddleBand, decimal? UpperBand)>>
    {
        public BollingerBands(IEnumerable<Candle> inputs, int periodCount, decimal sdCount) 
            : base(inputs, i => i.Close, (i, otm) => new AnalyzableTick<(decimal? LowerBand, decimal? MiddleBand, decimal? UpperBand)>(i.DateTime, otm), periodCount, sdCount)
        {
        }
    }
}