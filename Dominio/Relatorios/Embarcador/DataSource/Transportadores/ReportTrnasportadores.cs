using System;
using System.Collections.Generic;

namespace Dominio.Relatorios.Embarcador.DataSource.Transportadores
{
    public class ReportTrnasportadores
    {
        #region Propriedades

        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string NumeroUltimoContrato { get; set; }
        public string TipoUltimoContrato { get; set; }
        public string CodigoIntegracao { get; set; }
        public string InscricaoEstadual { get; set; }
        public string RNTRC { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Endereco { get; set; }
        public bool SituacaoContrato { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Telefone { get; set; }
        public string CNPJTransportadorSemFormato { get; set; }
        public string Email { get; set; }
        public string CEP { get; set; }
        public string Status { get; set; }
        public bool EmiteEmbarcador { get; set; }
        public bool CertificadoVencido { get; set; }
        public string DataVencimentoCertificado { get; set; }
        public bool LiberacaoParaPagamentoAutomatico { get; set; }
        public string OptanteSimplesNacional { get; set; }
        public bool ConfiguracaoNFSe { get; set; }
        public string Bloqueado { get; set; }
        public string MotivoBloqueio { get; set; }
        public int SerieCTeDentro { get; set; }
        public int SerieCTeFora { get; set; }
        public int SerieMDFe { get; set; }
        public string UsuarioCadastro { get; set; }
        public string UsuarioAtualizacao { get; set; }
        public string DataCadastro { get; set; }
        public string DataAtualizacao { get; set; }
        public string OperadoraValePedagio { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CNPJ
        {
            get
            {
                if (!string.IsNullOrEmpty(CNPJTransportadorSemFormato))
                {
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJTransportadorSemFormato));
                }
                else
                {
                    return "";
                }
            }
        }

        public string DescricaoEmiteEmbarcador
        {
            get { return EmiteEmbarcador ? "Não" : "Sim"; }
        }

        public string DescricaoCertificadoVencido
        {
            get { return CertificadoVencido ? "Sim" : "Não"; }
        }

        public string DescricaoLiberacaoParaPagamentoAutomatico
        {
            get { return LiberacaoParaPagamentoAutomatico ? "Sim" : "Não"; }
        }

        public string DescricaoConfiguracaoNFSe
        {
            get { return ConfiguracaoNFSe ? "Sim" : "Não"; }
        }

        public string Situacao
        {
            get
            {
                if (this.Status == "A")
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public string DescricaoSituacaoContrato
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.NumeroUltimoContrato))
                {
                    if (this.SituacaoContrato)
                        return "Ativo";
                    else
                        return "Inativo";
                }
                else
                {
                    return "";
                }
            }
        }

        public string Vigencia
        {
            get
            {
                List<string> vigencia = new List<string>();

                if (this.DataInicial != DateTime.MinValue)
                    vigencia.Add(this.DataInicial.ToString("dd/MM/yyyy"));

                if (this.DataFinal != DateTime.MinValue)
                    vigencia.Add(this.DataFinal.ToString("dd/MM/yyyy"));

                return String.Join(" - ", vigencia);
            }
        }

        #endregion
    }
}
