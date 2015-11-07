package md55a05def10ad178c7b6d4d61352f47736;


public class core
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("com.northstar.core, com.northstar, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", core.class, __md_methods);
	}


	public core () throws java.lang.Throwable
	{
		super ();
		if (getClass () == core.class)
			mono.android.TypeManager.Activate ("com.northstar.core, com.northstar, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
