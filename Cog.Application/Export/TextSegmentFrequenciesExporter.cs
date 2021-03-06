﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using SIL.Cog.Application.ViewModels;
using SIL.Cog.Domain;
using SIL.Machine.FeatureModel;

namespace SIL.Cog.Application.Export
{
	public class TextSegmentFrequenciesExporter : ISegmentFrequenciesExporter
	{
		private readonly static Dictionary<string, int> SortOrderLookup = new Dictionary<string, int>
			{
				{"bilabial", 0},
				{"labiodental", 0},
				{"dental", 1},
				{"alveolar", 1},
				{"retroflex", 1},
				{"palato-alveolar", 1},
				{"alveolo-palatal", 1},
				{"palatal", 2},
				{"velar", 2},
				{"uvular", 2},
				{"pharyngeal", 3},
				{"epiglottal", 3},
				{"glottal", 3},

				{"close-vowel", 0},
				{"mid-vowel", 1},
				{"open-vowel", 2}
			};

		public void Export(Stream stream, CogProject project, SyllablePosition syllablePosition)
		{
			FeatureSymbol domainSyllablePosition = null;
			switch (syllablePosition)
			{
				case SyllablePosition.Onset:
					domainSyllablePosition = CogFeatureSystem.Onset;
					break;
				case SyllablePosition.Nucleus:
					domainSyllablePosition = CogFeatureSystem.Nucleus;
					break;
				case SyllablePosition.Coda:
					domainSyllablePosition = CogFeatureSystem.Coda;
					break;
			}

			var comparer = new SegmentComparer();
			Segment[] segments = project.Varieties
				.SelectMany(v => v.SyllablePositionSegmentFrequencyDistributions[domainSyllablePosition].ObservedSamples)
				.Distinct().OrderBy(GetSortOrder).ThenBy(s => s, comparer).ToArray();

			using (var writer = new StreamWriter(new NonClosingStreamWrapper(stream)))
			{
				foreach (Segment seg in segments)
				{
					writer.Write("\t");
					writer.Write(seg.StrRep);
				}
				writer.WriteLine();

				foreach (Variety variety in project.Varieties)
				{
					writer.Write(variety.Name);
					foreach (Segment seg in segments)
					{
						writer.Write("\t");
						writer.Write(variety.SyllablePositionSegmentFrequencyDistributions[domainSyllablePosition][seg]);
					}
					writer.WriteLine();
				}
			}
		}

		private int GetSortOrder(Segment segment)
		{
			FeatureStruct fs = segment.FeatureStruct;
			if (segment.IsComplex)
				fs = segment.FeatureStruct.GetValue(CogFeatureSystem.First);

			return segment.Type == CogFeatureSystem.VowelType ? SortOrderLookup[((FeatureSymbol) fs.GetValue<SymbolicFeatureValue>("manner")).ID]
				: SortOrderLookup[((FeatureSymbol) fs.GetValue<SymbolicFeatureValue>("place")).ID];
		}
	}
}
