using System;

namespace Dominio.ObjetosDeValor.EDI.Carregamento
{
    public class Pedido
    {
        public string Identificador { get; set; }
        public string NumeroRomaneio { get; set; }
        public string NumeroPedido { get; set; }
        public string SeriePedido { get; set; }
        public decimal Volumes { get; set; }
        public decimal Peso { get; set; }
        public decimal Cubagem { get; set; }
        public string Situacao { get; set; }

        public string CNPJEmbarcador { get; set; }
        public string NumeroCarga { get; set; }
        public string CodigoItem { get; set; }
        public string OrdemCarregamento { get; set; }
        public string CodigoDestinatario { get; set; }
        public string Destinatario { get; set; }
        public DateTime DataPrevistaPartida { get; set; }
        public DateTime DataPrevistaEntrega { get; set; }        
    }
}
