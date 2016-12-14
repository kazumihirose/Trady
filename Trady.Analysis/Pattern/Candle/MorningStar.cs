﻿using System;
using Trady.Core;

namespace Trady.Analysis.Pattern.Candle
{
    /// <summary>
    /// Reference: http://stockcharts.com/school/doku.php?id=chart_school:chart_analysis:candlestick_pattern_dictionary
    /// </summary>
    public class MorningStar : AnalyticBase<IsMatchedResult>
    {
        public MorningStar(Equity equity) : base(equity)
        {
        }

        public override IsMatchedResult ComputeByIndex(int index)
        {
            throw new NotImplementedException();
        }
    }
}