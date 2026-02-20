namespace Dominio.ObjetosDeValor.WebService.Rest
{
    public class AtualizarDatasCarga
    {
        public int ProtocoloCarga { get; set; }
        public string NumeroCarga { get; set; }
        public string DataInicioCarregamento { get; set; }
        public string DataTerminoCarregamento { get; set; }
        public string DataPrevisaoInicioViagem { get; set; }
        public string DataLoger { get; set; }
        public string StatusLoger { get; set; }
    }
}
