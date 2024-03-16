using Core;

using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu(fileName = "Essential services", menuName = "Custom/Architecture/CreateServicesNode")]
	public sealed class CreateEssentialServicesElement : BootstrapElement
	{
		public override void Init()
		{
			ServiceLocator.ClearAll(this);
			ServiceLocator.Register<GlobalDeathNotificator>(new GlobalDeathNotificator());
		}
	}
}
