using DAL.Models;
using System.Diagnostics.CodeAnalysis;

namespace BL.Services
{
    public class SectionComparer : IEqualityComparer<Section>
    {
        public bool Equals(Section? x, Section? y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            else
            {
                return x.Id == y.Id;
            }
        }

        public int GetHashCode([DisallowNull] Section obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
