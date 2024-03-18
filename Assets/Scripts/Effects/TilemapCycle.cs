using UnityEngine;

namespace Effects
{
	public sealed class TilemapCycle : MonoBehaviour
	{
		[SerializeField] private float _cycleTime;
		[SerializeField] private Gradient _shadowColor;
		[SerializeField] private Gradient _tileMapColor;
		[SerializeField] private float _maxShadowLength;
		[SerializeField] private float _maxAffinity;
		[SerializeField] private AnimationCurve _shadowGrowthCurve;
		[SerializeField] private AnimationCurve _affinityGrowthCurve;
		[SerializeField] private Material _material;
		private float _elapsed;

		private int _shadowColorID;
		private int _mapColorID;
		private int _shadowAngleID;
		private int _shadowLengthID;
		private int _affinityID;

		private void Start()
		{
			_shadowColorID = Shader.PropertyToID("_ShadowColor");
			_mapColorID = Shader.PropertyToID("_MapColor");
			_shadowAngleID = Shader.PropertyToID("_Angle");
			_shadowLengthID = Shader.PropertyToID("_Length");
			_affinityID = Shader.PropertyToID("_Affinity");
		}
		private void Update()
		{
			_elapsed += Time.deltaTime;
			float delta = _elapsed / _cycleTime;

			if(delta > 1)
			{
				_elapsed -= _cycleTime;
				delta -= 1;
			}

			float angle = delta * Mathf.PI * 2;
			float length = _shadowGrowthCurve.Evaluate(delta) * _maxShadowLength;
			float affinity = _affinityGrowthCurve.Evaluate(delta) * _maxAffinity;
			Color mapColor = _tileMapColor.Evaluate(delta);
			Color shadowColor = _shadowColor.Evaluate(delta);

			_material.SetColor(_mapColorID, mapColor);
			_material.SetColor(_shadowColorID, shadowColor);
			_material.SetFloat(_shadowAngleID, angle);
			_material.SetFloat(_affinityID, affinity);
			_material.SetFloat(_shadowLengthID, length);
		}
	}
}