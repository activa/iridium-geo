using System.Collections.Generic;

namespace Iridium.Geo
{
    public interface IMultiGeometry<out T> : IEnumerable<T>, IGeometry where T:IGeometry
    {
        
    }
}