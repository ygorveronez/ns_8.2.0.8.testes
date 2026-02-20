namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestInformarChegada
    {
        public int clienteMultisoftware { get; set; }
        public int codigoCargaEntrega { get; set; }
        public string IdTrizy { get; set; }
        public long dataConfirmacaoChegada { get; set; }
        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenadaConfirmacaoChegada { get; set; }
    }
}
