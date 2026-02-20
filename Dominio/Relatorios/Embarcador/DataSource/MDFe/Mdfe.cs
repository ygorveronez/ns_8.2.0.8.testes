using Dominio.Enumeradores;
using System;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.MDFe
{
    public sealed class Mdfe
    {
        public int Codigo { get; set; }

        public string ChaveAcesso { get; set; }

        private string _cnpjEmpresa;
        public string CnpjEmpresa
        {
            get { return _cnpjEmpresa.ObterCnpjFormatado(); }
            set { _cnpjEmpresa = value; }
        }

        public DateTime DataAutorizacao { get; set; }
        public string DataAutorizacaoFormatada
        {
            get { return DataAutorizacao != DateTime.MinValue ? DataAutorizacao.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public DateTime DataCancelamento { get; set; }
        public string DataCancelamentoFormatada
        {
            get { return DataCancelamento != DateTime.MinValue ? DataCancelamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public DateTime DataEmissao { get; set; }
        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public DateTime DataEncerramento { get; set; }
        public string DataEncerramentoFormatada
        {
            get { return DataEncerramento != DateTime.MinValue ? DataEncerramento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string JustificativaCancelamento { get; set; }

        public string MensagemRetornoSefaz { get; set; }

        public string Motoristas { get; set; }

        public int Numero { get; set; }

        public decimal PesoBrutoMercadoria { get; set; }
        public string PesoBrutoMercadoriaFormatado
        {
            get { return $"{PesoBrutoMercadoria.ToString("n4")} {UnidadeMedidaMercadoria.ObterDescricao()}"; }
        }

        public string ProtocoloAutorizacao { get; set; }

        public string ProtocoloCancelamento { get; set; }

        public string ProtocoloEncerramento { get; set; }

        public string RazaoSocialEmpresa { get; set; }

        public int Serie { get; set; }

        public StatusMDFe StatusMdfe { get; set; }
        public string StatusMdfeDescricao
        {
            get { return StatusMdfe.ObterDescricao(); }
        }

        public string UfCarregamento { get; set; }

        public string UfDescarregamento { get; set; }

        public UnidadeMedidaMDFe UnidadeMedidaMercadoria { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public string Veiculos { get; set; }

        public string NumeroCarga { get; set; }

        public string TipoOperacao { get; set; }

        public string CTes { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorReceber { get; set; }

        public string MunicipioDescarregamento { get; set; }

        private string CPFMotorista { get; set; }

        public string CPFMotoristaFormatado
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CPFMotorista))
                    return string.Join(", ", CPFMotorista.Split(',').Select(o => o.ObterCpfFormatado()));
                else
                    return string.Empty;
            }
        }

        public string Tomador { get; set; }

        public string CPFCNPJTomador { get; set; }

        public string CPFCNPJTomadorFormatado
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CPFCNPJTomador))
                    return string.Join(", ", CPFCNPJTomador.Split(',').Select(o => o.ObterCpfOuCnpjFormatado()));
                else
                    return string.Empty;
            }
        }
    }
}
