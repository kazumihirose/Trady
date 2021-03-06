﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Infrastructure;
using Trady.Core;

namespace Trady.Analysis.Indicator
{
    public class PlusDirectionalIndicator<TInput, TOutput> : AnalyzableBase<TInput, (decimal High, decimal Low, decimal Close), decimal?, TOutput>
    {
        PlusDirectionalMovementByTuple _pdm;
        MinusDirectionalMovementByTuple _mdm;
        readonly GenericExponentialMovingAverage _tpdmEma;
        readonly AverageTrueRangeByTuple _atr;

        public PlusDirectionalIndicator(IEnumerable<TInput> inputs, Func<TInput, (decimal High, decimal Low, decimal Close)> inputMapper, Func<TInput, decimal?, TOutput> outputMapper, int periodCount) : base(inputs, inputMapper, outputMapper)
        {
			_pdm = new PlusDirectionalMovementByTuple(inputs.Select(i => inputMapper(i).High));
			_mdm = new MinusDirectionalMovementByTuple(inputs.Select(i => inputMapper(i).Low));

			Func<int, decimal?> tpdm = i => (_pdm[i] > 0 && _pdm[i] > _mdm[i]) ? _pdm[i] : 0;

            _tpdmEma = new GenericExponentialMovingAverage(
                periodCount,
                i => Enumerable.Range(i - periodCount + 1, periodCount).Select(j => tpdm(j)).Average(),
                i => tpdm(i),
                i => 1.0m / periodCount,
                inputs.Count());

			_atr = new AverageTrueRangeByTuple(inputs.Select(inputMapper), periodCount);

			PeriodCount = periodCount;
        }

        public int PeriodCount { get; }

        protected override decimal? ComputeByIndexImpl(IEnumerable<(decimal High, decimal Low, decimal Close)> mappedInputs, int index)
			=> _tpdmEma[index] / _atr[index] * 100;
	}

    public class PlusDirectionalIndicatorByTuple : PlusDirectionalIndicator<(decimal High, decimal Low, decimal Close), decimal?>
    {
        public PlusDirectionalIndicatorByTuple(IEnumerable<(decimal High, decimal Low, decimal Close)> inputs, int periodCount) 
            : base(inputs, i => i, (i, otm) => otm, periodCount)
        {
        }
    }

    public class PlusDirectionalIndicator : PlusDirectionalIndicator<Candle, AnalyzableTick<decimal?>>
    {
        public PlusDirectionalIndicator(IEnumerable<Candle> inputs, int periodCount) 
            : base(inputs, i => (i.High, i.Low, i.Close), (i, otm) => new AnalyzableTick<decimal?>(i.DateTime, otm), periodCount)
        {
        }
    }
}