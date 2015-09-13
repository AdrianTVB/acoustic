using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp_QuadTree.Src.Resources
{
    public abstract class Resource
    {
        private string resourceName;
        private string resourceCode;

        public string ResourceName
        {
            get
            {
                return resourceName;
            }

            set
            {
                resourceName = value;
            }
        }
        public string ResourceCode
        {
            get
            {
                return resourceCode;
            }

            set
            {
                resourceCode = value;
            }
        }

    }
}
