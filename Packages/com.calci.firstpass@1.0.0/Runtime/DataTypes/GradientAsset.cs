using UnityEngine;

[CreateAssetMenu(menuName = "Gradient", order = 401)]
public class GradientAsset : ScriptableObject
{
	public Gradient data = new Gradient();

	public static implicit operator Gradient(GradientAsset me)
	{
		return me.data;
	}

	public static implicit operator GradientAsset(Gradient curve)
	{
		GradientAsset so = CreateInstance<GradientAsset>();
		so.data = curve;

		return so;
	}
}