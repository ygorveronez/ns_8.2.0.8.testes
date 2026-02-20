namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class NotaFiscalServico
    {
        public int NumeroProtocolo { get; set; }
        
        public string Chave { get; set; }
        
        public int Numero { get; set; }

        public string CNPJCPFDestinatario { get; set; }

        public string CNPJCPFRemetente { get; set; }

        public decimal ValorAReceber { get; set; }

        public int LocalidadePrestacao { get; set; }

    }
}
