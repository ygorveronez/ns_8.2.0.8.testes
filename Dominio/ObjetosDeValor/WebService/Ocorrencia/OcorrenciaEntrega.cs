using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Ocorrencia
{
    public class OcorrenciaEntrega
    {
        public int ProtocoloPedido { get; set; }
        public string NumeroPedido { get; set; }
        public string CodigoOcorrencia { get; set; }
        public string DescricaoOcorrencia { get; set; }
        public string Observacao { get; set; }
        public string Cliente { get; set; }
        public string DataOcorrencia { get; set; }


        public string IdPedidoVTEX { get; set; }
        public string IdPedidoEMillennium { get; set; }
        public string CodigoVolume { get; set; }
        public string Transportadora { get; set; }
        public bool OcorrenciaVisivelAoCliente { get; set; }
        public int QuantidadeVolumes { get; set; }
        public bool OcorrenciaFinalizadora { get; set; }
        public List<NotaFiscal> NotasFiscais { get; set; }
    }
}
