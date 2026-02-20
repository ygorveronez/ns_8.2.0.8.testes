using System;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class ParadasCarga
    {
        public int Codigo { get; set; }

        public string Filial { get; set; }

        public string Carga { get; set; }

        public string TipoOperacao { get; set; }

        public DateTime DataCriacaoCarga { get; set; }

        public string Transportador { get; set; }

        public string Motoristas { get; set; }

        public string Placas { get; set; }

        public int SituacaoEntrega { get; set; }

        public int OrdemPrevista { get; set; }

        public int OrdemExecutada { get; set; }

        public int Aderencia { get; set; }

        public double CPFCNPJCliente { get; set; }

        public string TipoCliente { get; set; }

        public string Cliente { get; set; }

        public string Endereco { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string CEP { get; set; }

        public DateTime DataChegadaCliente { get; set; }

        public DateTime DataSaidaCliente { get; set; }

        public DateTime DataEntrega { get; set; }

        public string NotasFiscais { get; set; }

        public string Pedidos { get; set; }

        public decimal PesoBruto { get; set; }

        public decimal Quantidades { get; set; }

        public decimal Volumes { get; set; }

        private DateTime DataHoraAvaliacao { get; set; }

        public int ResultadoAvaliacao { get; set; }

        public string MotivoAvaliacao { get; set; }

        public string ObservacaoAvaliacao { get; set; }

        private Embarcador.Enumeradores.TipoArea TipoArea { get; set; }
        private int RaioCliente { get; set; }
        private string LatitudeCliente { get; set; }
        private string LongitudeCliente { get; set; }
        private decimal LatitudeEntregaFinalizada { get; set; }
        private decimal LongitudeEntregaFinalizada { get; set; }
        public bool TipoParada { get; set; }
        public decimal ValorTotalNotas { get; set; }
        public string CodigoIntegracaoCliente { get; set; }

        public decimal KMPlanejado { get; set; }

        public decimal KMRealizado { get; set; }

        private DateTime DataInicioViagem { get; set; }

        private DateTime DataConfirmacaoChegada { get; set; }

        private DateTime DataInicioCarregamento { get; set; }

        private DateTime DataTerminoCarregamento { get; set; }

        private DateTime DataInicioDescarga { get; set; }

        private DateTime DataTerminoDescarga { get; set; }

        private DateTime DataFimViagem { get; set; }
        private DateTime DataCarregamento { get; set; }

        public string ConfirmacaoViaApp { get; set; }
        private bool EncerramentoManualViagem { get; set; }

        public string ModeloVeicular { get; set; }
        public int ProtocoloIntegracaoCarga { get; set; }
        public int ProtocoloPedido { get; set; }
        public DateTime? PrevisaoChegada { get; set; }
        public DateTime? PrevisaoChegadaRecalculadaETA { get; set; }
    }
}