using SIL.Collections;

namespace SIL.Cog
{
	public interface IConditionalProbabilityDistribution<TCondition, TSample>
	{
		IReadOnlyCollection<TCondition> Conditions { get; }
		IProbabilityDistribution<TSample> this[TCondition condition] { get; }
		bool TryGetProbabilityDistribution(TCondition condition, out IProbabilityDistribution<TSample> probDist);
	}
}
