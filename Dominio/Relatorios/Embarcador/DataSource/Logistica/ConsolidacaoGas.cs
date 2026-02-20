using System;
using System.Text;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class ConsolidacaoGas
    {
        public int Codigo { get; set; }
        public string ClienteBase { get; set; }
        public string ClienteBaseCodigoIntegracao { get; set; }
        public string Produto { get; set; }
        public decimal Capacidade { get; set; }
        public decimal Lastro { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public decimal EstoqueMaximo { get; set; }
        public DateTime DataMedicao { get; set; }
        public decimal PorcentagemAbertura { get; set; }
        public decimal Abertura { get; set; }
        public decimal PrevisaoBombeio { get; set; }
        public decimal PrevisaoTransferenciaRecebida { get; set; }
        public decimal PrevisaoDemandaDomiciliar { get; set; }
        public decimal PrevisaoDemandaEmpresarial { get; set; }
        public decimal EstoqueUltrasystem { get; set; }
        public decimal PrevisaoTransferenciaEnviada { get; set; }
        public decimal PrevisaoFechamento { get; set; }
        public decimal VolumeRodoviarioCarregamentoProximoDia { get; set; }
        public decimal PrevisaoBombeioProximoDia { get; set; }
        public decimal DisponibilidadeTransferenciaProximoDia { get; set; }
        public decimal DensidadeAberturaDia { get; set; }
        public decimal AdicionalVolumeRodoviarioCarregamentoProximoDia { get; set; }
        public decimal AdicionalDisponibilidadeTransferenciaProximoDia { get; set; }
        public decimal SaldoDisponibilidadeTransferencia { get; set; }
        public int VeiculosPlanejados { get; set; }
        public decimal DemandaPlanejada { get; set; }
        public decimal DemandaPlanejar { get; set; }
        public string ClienteSupridor { get; set; }
        public string ClienteSupridorCodigoIntegracao { get; set; }
        public string TipoDeCarga { get; set; }
        public string TipoOperacao { get; set; }
        public string ModeloVeicular { get; set; }
        public string Observacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public DateTime DataAdicaoQuantidade { get; set; }
        public string Usuario { get; set; }
        public string UsuarioAdicaoQuantidade { get; set; }

        public string ClienteBaseDescricao
        {
            get
            {
                return new StringBuilder()
                    .Append($"{ClienteBaseCodigoIntegracao} - ", !string.IsNullOrWhiteSpace(ClienteBaseCodigoIntegracao))
                    .Append(ClienteBase)
                    .ToString();
            }
        }

        public string ClienteSupridorDescricao
        {
            get
            {
                return new StringBuilder()
                    .Append($"{ClienteSupridorCodigoIntegracao} - ", !string.IsNullOrWhiteSpace(ClienteSupridorCodigoIntegracao))
                    .Append(ClienteSupridor)
                    .ToString();
            }
        }

        public string DataMedicaoFormatada
        {
            get
            {
                return DataMedicao != DateTime.MinValue ? DataMedicao.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string DataUltimaAlteracaoFormatada
        {
            get
            {
                return DataUltimaAlteracao != DateTime.MinValue ? DataUltimaAlteracao.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            }
        }

        public string DataAdicaoQuantidadeFormatada
        {
            get
            {
                return DataAdicaoQuantidade != DateTime.MinValue ? DataAdicaoQuantidade.ToString("dd/MM/yyyy HH:mm") : string.Empty;
            }
        }
    }
}
