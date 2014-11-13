﻿using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.Model.Comparisons
{
    public class BestSplitTimesComparisonGenerator : IComparisonGenerator
    {
        public IRun Run { get; set; }
        public const String ComparisonName = "Best Split Times";
        public String Name { get { return ComparisonName; } }

        public BestSplitTimesComparisonGenerator(IRun run)
        {
            Run = run;
        }

        public void Generate(TimingMethod method)
        {
            for (var y = Run.GetMinSegmentHistoryIndex() + 1; y <= Run.RunHistory.Count; y++)
            {
                var time = TimeSpan.Zero;

                foreach (var segment in Run)
                {
                    var segmentHistoryElement = segment.SegmentHistory.Where(x => x.Index == y).FirstOrDefault();
                    if (segmentHistoryElement != null)
                    {
                        var segmentTime = segmentHistoryElement.Time[method];
                        if (segmentTime != null)
                        {
                            time += segmentTime.Value;

                            if (segment.Comparisons[Name][method] == null || time < segment.Comparisons[Name][method])
                            {
                                var newTime = new Time(segment.Comparisons[Name]);
                                newTime[method] = time;
                                segment.Comparisons[Name] = newTime;
                            }
                        }
                    }
                    else break;
                }
            }
        }

        public void Generate(ISettings settings)
        {
            foreach (var segment in Run)
            {
                if (Run.IndexOf(segment) > 0)
                    segment.Comparisons[Name] = segment.Comparisons[LiveSplit.Model.Run.PersonalBestComparisonName];
                else
                    segment.Comparisons[Name] = segment.BestSegmentTime;
            }
            Generate(TimingMethod.RealTime);
            Generate(TimingMethod.GameTime);
        }
    }
}