namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestMotoristaACaminho
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public bool aCaminho { get; set; }
        public string IdTrizy { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
