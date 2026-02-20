namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    /// <summary>
    /// Uma posição no mapa com coordenadas (x, y).
    /// </summary>
    public class Position
    {
        public Position() { }
        public Position(long id, double latitude, double longitude)
        {
            this.id = id;
            this.X = longitude;
            this.Y = latitude;
        }
        public long id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
