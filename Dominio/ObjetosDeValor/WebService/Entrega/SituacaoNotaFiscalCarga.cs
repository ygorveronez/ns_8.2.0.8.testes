using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Entrega
{
    public class SituacaoNotaFiscalCarga
    {
        public int CargaProtocoloCarga { get; set; }
        public string CargaCodigoCargaEmbarcador { get; set; }
        public string CargaDescricaoOperacao { get; set; }
        public decimal CargaPeso { get; set; }
        public string CargaDescricaoRota { get; set; }
        public DateTime CargaDataCriacao { get; set; }
        public DateTime? CargaDataCarregamento { get; set; }
        public DateTime? CargaDataInicioViagem { get; set; }
        public DateTime? CargaDataFimViagem { get; set; }
        public DateTime? CargaDataPrevisaoChegadaDestinatario { get; set; }

        // dados VEICULO TRACAO
        public string VeiculoPlaca { get; set; }
        public string VeiculoTransportadora { get; set; }

        public int VeiculoTaraKg { get; set; }
        public int VeiculoCapacidadeKG { get; set; }

        // dados MOTORISTA
        public string MotoristaNome { get; set; }
        public string MotoristaCPF { get; set; }
        public string MotoristaTelefone { get; set; }

        // dados OPERADOR
        public string OperadorNome { get; set; }
        public string OperadorLogin { get; set; }
        public string OperadorEmail { get; set; }

        // dados POSICAO
        public DateTime? PosicaoData { get; set; }
        public double PosicaoLongitude { get; set; }
        public double PosicaoLatitude { get; set; }
        public List<SituacaoNotaFiscalNotasCarga> Notas { get; set; }
        public List<OcorrenciaDetalhes> Ocorrencias { get; set; }

        public Embarcador.Pessoas.Pessoa Remetente { get; set; }
        public Embarcador.Pessoas.Pessoa Destinatario { get; set; }

        //dados RASTREIO
        public string LinkRastreio { get; set; }

    }
}
