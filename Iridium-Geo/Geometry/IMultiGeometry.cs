using System.Collections.Generic;

namespace Iridium.Geo
{
    public interface IMultiGeometry : IEnumerable<IGeometry> , IGeometry
    {
        
    }
}