using System.Diagnostics;
using System.Xml.Linq;
using SIL.Cog.Components;
using SIL.Machine;

namespace SIL.Cog.Config.Components
{
	public class BlairCognateIdentifierConfig : IComponentConfig<IProcessor<VarietyPair>>
	{
		public IProcessor<VarietyPair> Load(SpanFactory<ShapeNode> spanFactory, CogProject project, XElement elem)
		{
			XElement alignerElem = elem.Element(ConfigManager.Cog + "ApplicableAligner");
			Debug.Assert(alignerElem != null);
			var alignerID = (string) alignerElem.Attribute("ref");
			var ignoreRegularInsertionDeletionStr = (string) elem.Element(ConfigManager.Cog + "IgnoreRegularInsertionDeletion");
			var regularConsEqualStr = (string) elem.Element(ConfigManager.Cog + "RegularConsonantsAreEqual");

			var ignoredMappings = ConfigManager.LoadComponent<ISegmentMappings>(spanFactory, project, elem.Element(ConfigManager.Cog + "IgnoredCorrespondences"));
			var similarSegments = ConfigManager.LoadComponent<ISegmentMappings>(spanFactory, project, elem.Element(ConfigManager.Cog + "SimilarSegments"));

			return new BlairCognateIdentifier(project, ignoreRegularInsertionDeletionStr != null && bool.Parse(ignoreRegularInsertionDeletionStr),
				regularConsEqualStr != null && bool.Parse(regularConsEqualStr), alignerID, ignoredMappings, similarSegments);
		}

		public void Save(IProcessor<VarietyPair> component, XElement elem)
		{
			var blair = (BlairCognateIdentifier) component;
			elem.Add(new XElement(ConfigManager.Cog + "ApplicableAligner", new XAttribute("ref", blair.AlignerID)));
			elem.Add(new XElement(ConfigManager.Cog + "IgnoreRegularInsertionDeletion", blair.IgnoreRegularInsertionDeletion));
			elem.Add(new XElement(ConfigManager.Cog + "RegularConsonantsAreEqual", blair.RegularConsonantEqual));
			elem.Add(ConfigManager.SaveComponent("IgnoredCorrespondences", blair.IgnoredMappings));
			elem.Add(ConfigManager.SaveComponent("SimilarSegments", blair.SimilarSegments));
		}
	}
}