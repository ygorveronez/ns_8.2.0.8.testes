using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class Path
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Center
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Bounds
    {
        public double south { get; set; }
        public double west { get; set; }
        public double north { get; set; }
        public double east { get; set; }
    }

    public class LocalArea
    {
        public string type { get; set; }
        public int zIndex { get; set; }
        public string fillColor { get; set; }
        public List<Path> paths { get; set; }
        public double radius { get; set; }
        public Center center { get; set; }
        public Bounds bounds { get; set; }
        public Path position { get; set; }
    }
}
