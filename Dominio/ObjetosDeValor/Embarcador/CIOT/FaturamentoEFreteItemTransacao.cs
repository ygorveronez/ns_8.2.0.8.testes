using System;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT
{
    public class FaturamentoEFreteItemTransacao
    {
        public string Id { get; set; }

        public int Recibo { get; set; }

        public DateTime Data { get; set; }

        public string DocumentoViagem { get; set; }

        public decimal? QuantidadeDaMercadoriaNoDesembarque { get; set; }

        public decimal? ValorDiferencaDeFrete { get; set; }

        public decimal? ValorQuebraDeFrete { get; set; }
    }
}
