using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using UnityEngine;

namespace Assets.Misc
{
    public static class MathUtil
    {
        private static readonly List<Vector3> BasisVectors = new List<Vector3> { Vector3.right, Vector3.up, Vector3.forward };
        public static Vector3 Snap(this Vector3 v)
        {
            var best = BasisVectors.MaxBy(a => Math.Abs(Vector3.Dot(v, a)));
            if (Vector3.Dot(v, best) < 0)
                best *= -1;
            return best;
        }
    }
}
