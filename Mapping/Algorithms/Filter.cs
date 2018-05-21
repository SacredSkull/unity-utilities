using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityUtilities.Mapping.Algorithms
{
    public abstract class Filter
    {
        private HashSet<Vector2> vectors;
        
        public Filter(params Vector2[] filtervectors) {
            vectors = new HashSet<Vector2>(filtervectors);
        }

        public HashSet<Vector2> GetVectors() => vectors;

        public void AddVectors(params Vector2[] additional)
        {
            vectors = new HashSet<Vector2>(vectors.Concat(additional));
        }

        public void RemoveVectors(params Vector2[] removal)
        {
            vectors = new HashSet<Vector2>(vectors.Except(removal));
        }
    }

    public class PassingFilter : Filter
    {
        public PassingFilter(params Vector2[] filtervectors) : base(filtervectors) { }
    }

    public class BlockingFilter : Filter
    {
        public BlockingFilter(params Vector2[] filtervectors) : base(filtervectors) { }
    }
}