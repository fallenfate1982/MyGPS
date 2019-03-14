using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertService
{
  public class ServiceHelper
  {

    public static bool   IsPointInPolygon(List<Loc> poly, Loc point)
    {
      int i, j;
      bool c = false;
      for (i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
      {
        if ((((poly[i].Lt <= point.Lt) && (point.Lt < poly[j].Lt)) ||
            ((poly[j].Lt <= point.Lt) && (point.Lt < poly[i].Lt))) &&
            (point.Lg < (poly[j].Lg - poly[i].Lg) * (point.Lt - poly[i].Lt) / (poly[j].Lt - poly[i].Lt) + poly[i].Lg))
          c = !c;
      }
      return c;
    }
  }

  public class Loc
  {
    public double Lt{get;set;}
    public double Lg { get; set; }
  }
}
