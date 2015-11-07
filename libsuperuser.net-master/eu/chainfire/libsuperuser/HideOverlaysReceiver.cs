using System;
using Android.Content;

namespace libsuperuser_net
{/**
 * <p>
 * Base receiver to extend to catch notifications when overlays should be
 * hidden.
 * </p>
 * <p>
 * Tapjacking protection in SuperSU prevents some dialogs from receiving user
 * input when overlays are present. For security reasons this notification is
 * only sent to apps that have previously been granted root access, so even if
 * your app does not <em>require</em> root, you still need to <em>request</em>
 * it, and the user must grant it.
 * </p>
 * <p>
 * Note that the word overlay as used here should be interpreted as "any view or
 * window possibly obscuring SuperSU dialogs".
 * </p>
 */
	public abstract class HideOverlaysReceiver:BroadcastReceiver
	{
		public const String ACTION_HIDE_OVERLAYS = "eu.chainfire.supersu.action.HIDE_OVERLAYS";
		public const String CATEGORY_HIDE_OVERLAYS = Intent.CategoryInfo;
		public const String EXTRA_HIDE_OVERLAYS = "eu.chainfire.supersu.extra.HIDE";


		public override void OnReceive (Context context, Intent intent)
		{
			if (intent.HasExtra (EXTRA_HIDE_OVERLAYS)) {
				OnHideOverlays (intent.GetBooleanExtra (EXTRA_HIDE_OVERLAYS, false));
			}
		}

		public abstract void OnHideOverlays (bool hide);
	}
}

