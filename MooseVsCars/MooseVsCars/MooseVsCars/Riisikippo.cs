using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;
/// <summary>
/// Tämä luokka on tehty "tapettavaa" riisikippo autoa varten
/// </summary>
public class Riisikippo : PhysicsObject
{

    public Color[] Varit;

    public Riisikippo(double leveys, double korkeus,
        Color[] varit) : base(leveys, korkeus)
    {
        Varit = varit;
    }
    /// <summary>
    /// Tässä määritellään törmäyksen vaikutus objektiin
    /// </summary>
    public void TormaaHirveen()
    {
        this.Destroy();
    }
}
