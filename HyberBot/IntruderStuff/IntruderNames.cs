using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class IntruderNames 
{

    public string[] names;
	public bool jumbledNames;
	public bool fetchNamesFromWebserver;


    public IntruderNames()
    {
		fetchNamesFromWebserver = true;
		jumbledNames = true;
        names = Array.Empty<string>();
    }
}