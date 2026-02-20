
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.ICMS
{
    public class RegraICMS
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Modal { get; set; }
        public string EstadoEmitente { get; set; }
        public string EstadoEmitenteDiferente { get; set; }
        public string EstadoOrigem { get; set; }
        public string EstadoOrigemDiferente { get; set; }
        public string EstadoDestino { get; set; }
        public string EstadoDestinoDiferente { get; set; }
        public string EstadoTomador { get; set; }
        public string EstadoTomadorDiferente { get; set; }
        public string Empresa { get; set; }
        public string Setor { get; set; }
        public string Remetente { get; set; }
        public string GrupoRemetente { get; set; }
        public string AtividadeRemetente { get; set; }
        public string Destinatario { get; set; }
        public string GrupoDestinatario { get; set; }
        public string AtividadeDestinatario { get; set; }
        public string Tomador { get; set; }
        public string GrupoTomador { get; set; }
        public string AtividadedoTomadorDiferenteDe { get; set; }
        public string AtividadeTomador { get; set; }
        public string TipoServico { get; set; }
        public string TipoPagamento { get; set; }
        public string NumeroProposta { get; set; }
        public string TipoOperacao { get; set; }
        public string Produtos { get; set; }
        public string CST { get; set; }
        public int CFOP { get; set; }
        public decimal Aliquota { get; set; }
        public string Situacao { get; set; }
        public decimal PercentualReducaoBaseCalculo { get; set; }
        public decimal PercentualCreditoPresumido { get; set; }
        public string Descricao { get; set; }

        private DateTime DataVigenciaInicio { get; set; }
        private DateTime DataVigenciaFim { get; set; }
        private bool UFdeOrigemIgualaUFTomador { get; set; }
        private bool ImprimirLei { get; set; }
        private bool ZerarBaseCalculo { get; set; }
        private bool NaoReduzirRetencao { get; set; }
        private bool NaoImprimirImpostos { get; set; }
        public string RegimeTomadorDiferente { get; set; }
        private bool SomenteOptanteSimplesNacional { get; set; }
        private bool DescontarICMSSTQuandoICMSNaoIncluso { get; set; }
        private bool IncluirPISeCOFINSnaBC { get; set; }
        private bool NaoIncluirPISeCOFINSnaBCparaComplementos { get; set; }

        #endregion

        #region Propriedades com Regras 

        public string NaoIncluirPISeCOFINSnaBCparaComplementosFormatada
        {
            get { return NaoIncluirPISeCOFINSnaBCparaComplementos ? "Sim" : "Não"; }
        }
        public string IncluirPISeCOFINSnaBCFormatada
        {
            get { return IncluirPISeCOFINSnaBC ? "Sim" : "Não"; }
        }
        public string DescontarICMSSTQuandoICMSNaoInclusoFormatada
        {
            get { return DescontarICMSSTQuandoICMSNaoIncluso ? "Sim" : "Não"; }
        }
        public string SomenteOptanteSimplesNacionalFormatada
        {
            get { return SomenteOptanteSimplesNacional ? "Sim" : "Não"; }
        }
        public string UFdeOrigemIgualaUFTomadorFormatada
        {
            get { return UFdeOrigemIgualaUFTomador ? "Sim" : "Não"; }
        }
        public string NaoReduzirRetencaoFormatada
        {
            get { return NaoReduzirRetencao ? "Sim" : "Não"; }
        }
        public string ZerarBaseCalculoFormatada
        {
            get { return ZerarBaseCalculo ? "Sim" : "Não"; }
        }
        public string ImprimirLeiFormatada
        {
            get { return ImprimirLei ? "Sim" : "Não"; }
        }
        public string DataVigenciaInicioFormatada
        {
            get { return DataVigenciaInicio != DateTime.MinValue ? DataVigenciaInicio.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataVigenciaFimFormatada
        {
            get { return DataVigenciaFim != DateTime.MinValue ? DataVigenciaFim.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string NaoImprimirImpostosFormatada
        {
            get { return NaoImprimirImpostos ? "Sim" : "Não"; }
        }

        #endregion
    }
}
