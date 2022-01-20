using UnityEngine;

[CreateAssetMenu(menuName = "Animation Curve", order = 400)]
public class AnimationCurveAsset : ScriptableObject
{
	public AnimationCurve data = AnimationCurve.Linear(0, 0, 1, 1);

	public static implicit operator AnimationCurve(AnimationCurveAsset me)
	{
		return me.data;
	}

	public static implicit operator AnimationCurveAsset(AnimationCurve curve)
	{
		AnimationCurveAsset so = CreateInstance<AnimationCurveAsset>();
		so.data = curve;
		return so;
	}
}