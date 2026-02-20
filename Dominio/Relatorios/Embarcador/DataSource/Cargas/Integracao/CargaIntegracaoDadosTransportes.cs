using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao
{
    public class CargaIntegracaoDadosTransportes
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public string GrupoPessoas { get; set; }
        public string Veiculos { get; set; }
        public string NumeroRastreadorVeiculos { get; set; }
        public string TecnologiaRastreadorVeiculos { get; set; }
        public string CPFMotoristas { get; set; }
        public string Motoristas { get; set; }
        public string TipoOperacao { get; set; }
        private DateTime DataCarga { get; set; }
        public string TipoIntegracao { get; set; }
        public int Tentativas { get; set; }
        public DateTime DataIntegracao { get; set; }
        public string Mensagem { get; set; }
        public SituacaoIntegracao Situacao { get; set; }
        public string Protocolo { get; set; }
        public SituacaoCarga SituacaoCarga { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        private string CNPJFilial { get; set; }
        public string Filial { get; set; }
        public string TipoCarga { get; set; }
        public string ModeloVeicular { get; set; }
        public string Transportador { get; set; }
        public DateTime DataEncerramento { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataCargaFormatada
        {
            get { return DataCarga != DateTime.MinValue ? DataCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataEncerramentoFormatada
        {
            get { return DataEncerramento != DateTime.MinValue ? DataEncerramento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoSituacaoCarga
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }

        public string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string CNPJFilialFormatado
        {
            get { return CNPJFilial.ObterCnpjFormatado(); }
        }

        #endregion
    }
}
