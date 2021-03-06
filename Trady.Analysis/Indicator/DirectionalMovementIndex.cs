﻿using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Infrastructure;
using Trady.Core;

namespace Trady.Analysis.Indicator
{
    public class DirectionalMovementIndex<TInput, TOutput> : AnalyzableBase<TInput, (decimal High, decimal Low, decimal Close), decimal?, TOutput>
    {
        readonly PlusDirectionalIndicatorByTuple _pdi;
        readonly MinusDirectionalIndicatorByTuple _mdi;

        public DirectionalMovementIndex(IEnumerable<TInput> inputs, Func<TInput, (decimal High, decimal Low, decimal Close)> inputMapper, Func<TInput, decimal?, TOutput> outputMapper, int periodCount) : base(inputs, inputMapper, outputMapper)
        {
			_pdi = new PlusDirectionalIndicatorByTuple(inputs.Select(inputMapper), periodCount);
			_mdi = new MinusDirectionalIndicatorByTuple(inputs.Select(inputMapper), periodCount);
			PeriodCount = periodCount;
        }

        public int PeriodCount { get; }

        protected override decimal? ComputeByIndexImpl(IEnumerable<(decimal High, decimal Low, decimal Close)> mappedInputs, int index)
        {
			var value = (_pdi[index] - _mdi[index]) / (_pdi[index] + _mdi[index]);
			return value.HasValue ? Math.Abs(value.Value) * 100 : (decimal?)null;
        }
    }

    public class DirectionalMovementIndexByTuple : DirectionalMovementIndex<(decimal High, decimal Low, decimal Close), decimal?>
    {
        public DirectionalMovementIndexByTuple(IEnumerable<(decimal High, decimal Low, decimal Close)> inputs, int periodCount)
            : base(inputs, i => i, (i, otm) => otm, periodCount)
        {
        }
    }

    public class DirectionalMovementIndex : DirectionalMovementIndex<Candle, AnalyzableTick<decimal?>>
    {
        public DirectionalMovementIndex(IEnumerable<Candle> inputs, int periodCount) 
            : base(inputs, i => (i.High, i.Low, i.Close), (i, otm) => new AnalyzableTick<decimal?>(i.DateTime, otm), periodCount)
        {
        }
    }
}