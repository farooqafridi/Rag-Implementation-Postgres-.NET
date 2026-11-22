using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pgvector;

namespace Infrastructure.Converters
{
    public class VectorConverter : ValueConverter<float[], Vector>
    {
        public VectorConverter()
            : base(
                value => new Vector(value),
                value => value.ToArray())
        {
        }
    }
}
