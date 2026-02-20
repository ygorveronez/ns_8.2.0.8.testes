using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class PedidoImportacaoOcorrencia
    {
        public string NumeroPedido { get; set; }

        public string CodigoOcorrencia { get; set; }

        public DateTime? DataHoraOcorrencia { get; set; }

        public string Observacao { get; set; }

        public string NomeArquivoGerador { get; set; }

        public string GuidArquivoGerador { get; set; }
    }
}