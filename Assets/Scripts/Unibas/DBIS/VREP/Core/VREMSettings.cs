using System;

namespace Unibas.DBIS.VREP.Core
{
    [Serializable]
    public class VREMSettings
    {
        /// <summary>
        /// The Address of the VREM server instance, inclusive port
        /// Usually this is 127.0.0.1:4567
        /// Default: 127.0.0.1:4567
        /// </summary>
        public string Address = "http://127.0.0.1:4567";
        
        /// <summary>
        /// Whether the server will be queried for exhibitions
        /// Default: False
        /// </summary>
        public bool RequestExhibitions = false;

        /// <summary>
        /// Whether the list of exhibitions is treated as names (otherwise as IDs)
        /// </summary>
        public bool LoadByName = true;
        
        /// <summary>
        /// A list of exhibitions, which shall be loaded
        /// Default: Empty
        /// </summary>
        public string[] Exhibitions = new string[0];

        public VREMSettings()
        {
            // empty default constructor, as default values are already set
        }
        
        public void SanitizeHost()
        {
            if (!Address.EndsWith("/"))
            {
                Address += "/";
            }

            if (!Address.StartsWith("http://"))
            {
                Address = "http://" + Address;
            }
        }
    }
}