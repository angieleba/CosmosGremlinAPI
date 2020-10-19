using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel.Graph
{
    public class BaseVertex
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }

    public abstract class BaseVertexProperties : IVertexProperties
    {
        protected BaseVertexProperties()
        {

        }

        public string PartKey { get; set; }  
    }
}
