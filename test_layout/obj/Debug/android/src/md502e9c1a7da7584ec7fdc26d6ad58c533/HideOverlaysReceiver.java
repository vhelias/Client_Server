package md502e9c1a7da7584ec7fdc26d6ad58c533;


public abstract class HideOverlaysReceiver
	extends android.content.BroadcastReceiver
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onReceive:(Landroid/content/Context;Landroid/content/Intent;)V:GetOnReceive_Landroid_content_Context_Landroid_content_Intent_Handler\n" +
			"";
		mono.android.Runtime.register ("libsuperuser_net.HideOverlaysReceiver, libsuperuser.net, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", HideOverlaysReceiver.class, __md_methods);
	}


	public HideOverlaysReceiver () throws java.lang.Throwable
	{
		super ();
		if (getClass () == HideOverlaysReceiver.class)
			mono.android.TypeManager.Activate ("libsuperuser_net.HideOverlaysReceiver, libsuperuser.net, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onReceive (android.content.Context p0, android.content.Intent p1)
	{
		n_onReceive (p0, p1);
	}

	private native void n_onReceive (android.content.Context p0, android.content.Intent p1);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
