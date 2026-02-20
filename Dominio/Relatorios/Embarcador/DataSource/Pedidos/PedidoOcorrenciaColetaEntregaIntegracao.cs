using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PedidoOcorrenciaColetaEntregaIntegracao
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int LayoutEDI { get; set; }
        public int CodigoSituacaoIntegracao { get; set; }
        public int CodigoTipoDeOcorrencia { get; set; }
        public int CodigoTipoIntegracao { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string DescricaoTipoDeOcorrencia { get; set; }
        public string DescricaoTipoIntegracao { get; set; }
        public string DataIntegracaoFormatada { get { return DataIntegracao.ToString("dd/MM/yyyy HH:mm"); } }
        public int NumeroTentativas { get; set; }
        public string ProblemaIntegracao { get; set; }
        public string SituacaoIntegracaoFormatada { get { return this.SituacaoIntegracao.ObterDescricao(); } }
        

        #endregion


        #region Propriedades Auxiliares 
        public SituacaoIntegracao SituacaoIntegracao { get; set; }

        #region Proproedades Auxiliares Transportador
        public int CargaCodigoIBGE { get; set; }
        public int CargaCodigoPais { get; set; }
        public string CargaLocalidadeDescricao { get; set; }
        public string CargaEstadoSigla { get; set; }
        public string CargaPaisAbreviacao { get; set; }
        public string CargaPaisNome { get; set; }


        public int PedidoCodigoIBGE { get; set; }
        public int PedidoCodigoPais { get; set; }
        public string PedidoLocalidadeDescricao { get; set; }
        public string PedidoEstadoSigla { get; set; }
        public string PedidoPaisAbreviacao { get; set; }
        public string PedidoPaisNome { get; set; }


        public string CargaCodigoEmpresa { get; set; }
        public string CargaRazaoSocial { get; set; }
        public int CargaLocalidade { get; set; }


        public string PedidoCodigoEmpresa { get; set; }
        public string PedidoRazaoSocial { get; set; }
        public int PedidoLocalidade { get; set; }


        public int CargaCodigo { get; set; }
        public int CargaEmpresaCodigo { get; set; }

        public int PedidoEmpresaCodigo { get; set; }

        public string CargaDescricaoCidadeEstado
        {
            get
            {
                if (this.CargaCodigoIBGE != 9999999 || this.CargaCodigoPais > 0)
                    return this.CargaLocalidadeDescricao + " - " + this.CargaEstadoSigla ?? "";
                else
                {
                    if (!string.IsNullOrWhiteSpace(this.CargaPaisAbreviacao))
                        return this.CargaLocalidadeDescricao + " - " + this.CargaPaisAbreviacao;
                    else
                        return this.CargaLocalidadeDescricao + " - " + this.CargaPaisNome;
                }
            }
        }

        public string PedidoDescricaoCidadeEstado
        {
            get
            {
                if (this.PedidoCodigoIBGE != 9999999 || this.PedidoCodigoPais > 0)
                    return this.PedidoLocalidadeDescricao + " - " + this.PedidoEstadoSigla ?? "";
                else
                {
                    if (!string.IsNullOrWhiteSpace(this.PedidoPaisAbreviacao))
                        return this.PedidoLocalidadeDescricao + " - " + this.PedidoPaisAbreviacao;
                    else
                        return this.PedidoLocalidadeDescricao + " - " + this.PedidoPaisNome;
                }
            }
        }

        #endregion

        #region Propriedades Auxiliares Tomador

        public TipoTomador TipoTomador { get; set; }

        public string ClienteRemetenteNome { get; set; }
        public string ClienteRemetenteNomeFantasia { get; set; }
        public string ClienteRemetenteCodigoIntegracao { get; set; }
        public string ClienteRemetenteTipo { get; set; }
        public bool ClienteRemetentePontoTransbordo { get; set; }
        public double ClienteRemetenteCPF_CNPJ { get; set; }


        public string ClienteDestinatarioNome { get; set; }
        public string ClienteDestinatarioNomeFantasia { get; set; }
        public string ClienteDestinatarioCodigoIntegracao { get; set; }
        public string ClienteDestinatarioTipo { get; set; }
        public bool ClienteDestinatarioPontoTransbordo { get; set; }
        public double ClienteDestinatarioCPF_CNPJ { get; set; }


        public string ClienteTomadorNome { get; set; }
        public string ClienteTomadorNomeFantasia { get; set; }
        public string ClienteTomadorCodigoIntegracao { get; set; }
        public string ClienteTomadorTipo { get; set; }
        public bool ClienteTomadorPontoTransbordo { get; set; }
        public double ClienteTomadorCPF_CNPJ { get; set; }


        public string ClienteRecebedorNome { get; set; }
        public string ClienteRecebedorNomeFantasia { get; set; }
        public string ClienteRecebedorCodigoIntegracao { get; set; }
        public string ClienteRecebedorTipo { get; set; }
        public bool ClienteRecebedorPontoTransbordo { get; set; }
        public double ClienteRecebedorCPF_CNPJ { get; set; }


        public string ClienteExpedidorNome { get; set; }
        public string ClienteExpedidorNomeFantasia { get; set; }
        public string ClienteExpedidorCodigoIntegracao { get; set; }
        public string ClienteExpedidorTipo { get; set; }
        public bool ClienteExpedidorPontoTransbordo { get; set; }
        public double ClienteExpedidorCPF_CNPJ { get; set; }

        #endregion

        #endregion

        #region Propriedades com regras

        public string TomadorFormatado
        {
            get
            {
                switch (this.TipoTomador)
                {
                    case TipoTomador.Remetente:
                        return this.Remetente;
                    case TipoTomador.Expedidor:
                        return this.Expedidor;
                    case TipoTomador.Recebedor:
                        return this.Recebedor;
                    case TipoTomador.Destinatario:
                        return this.Destinatario;
                    case TipoTomador.Outros:
                        return this.Tomador;
                    default:
                        return null;
                }
            }
        }
        public string TransportadorFormatado
        {
            get
            {
                if (this.CargaEmpresaCodigo > 0)
                    return $"{(string.IsNullOrWhiteSpace(this.CargaCodigoEmpresa) ? "" : this.CargaCodigoEmpresa + " ")}{(string.IsNullOrWhiteSpace(this.CargaRazaoSocial) ? "" : this.CargaRazaoSocial + " ")}{(this.CargaLocalidade > 0 ? $"({ObterDescricaoCidadeEstado(this.CargaEmpresaCodigo)})" : string.Empty)}";
                else
                    return $"{(string.IsNullOrWhiteSpace(this.PedidoCodigoEmpresa) ? "" : this.PedidoCodigoEmpresa + " ")}{(string.IsNullOrWhiteSpace(this.PedidoRazaoSocial) ? "" : this.PedidoRazaoSocial + " ")}{(this.PedidoLocalidade > 0 ? $"({ObterDescricaoCidadeEstado(this.CargaEmpresaCodigo)})" : string.Empty)}";
            }
        }
        public string DescricaoSituacaoIntegracao { get { return SituacaoIntegracao.ObterDescricao(); } }
        public string DT_RowColor { get { return SituacaoIntegracao.ObterCorLinha(); } }
        public string DT_FontColor { get { return SituacaoIntegracao.ObterCorFonte(); } }
        public string Remetente
        {
            get
            {
                string descricao = "";

                string nome = this.ClienteRemetenteNome;
                if (this.ClienteRemetentePontoTransbordo)
                    nome = this.ClienteRemetenteNomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.ClienteRemetenteCodigoIntegracao))
                    descricao += this.ClienteRemetenteCodigoIntegracao + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;
                if (!string.IsNullOrWhiteSpace(this.ClienteRemetenteTipo))
                    descricao += " (" + this.ClienteRemetenteCPF_CNPJ.ToString().ObterCpfOuCnpjFormatado(this.ClienteRemetenteTipo) + ")";

                return descricao;
            }
        }
        public string Expedidor
        {
            get
            {
                string descricao = "";

                string nome = this.ClienteExpedidorNome;
                if (this.ClienteExpedidorPontoTransbordo)
                    nome = this.ClienteExpedidorNomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.ClienteExpedidorCodigoIntegracao))
                    descricao += this.ClienteExpedidorCodigoIntegracao + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;
                if (!string.IsNullOrWhiteSpace(this.ClienteExpedidorTipo))
                    descricao += " (" + this.ClienteExpedidorCPF_CNPJ.ToString().ObterCpfOuCnpjFormatado(this.ClienteExpedidorTipo) + ")";

                return descricao;
            }
        }
        public string Recebedor
        {
            get
            {
                string descricao = "";

                string nome = this.ClienteRecebedorNome;
                if (this.ClienteRecebedorPontoTransbordo)
                    nome = this.ClienteRecebedorNomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.ClienteRecebedorCodigoIntegracao))
                    descricao += this.ClienteRecebedorCodigoIntegracao + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;
                if (!string.IsNullOrWhiteSpace(this.ClienteRecebedorTipo))
                    descricao += " (" + this.ClienteRecebedorCPF_CNPJ.ToString().ObterCpfOuCnpjFormatado(this.ClienteRecebedorTipo) + ")";

                return descricao;
            }
        }
        public string Destinatario
        {
            get
            {
                string descricao = "";

                string nome = this.ClienteDestinatarioNome;
                if (this.ClienteDestinatarioPontoTransbordo)
                    nome = this.ClienteDestinatarioNomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.ClienteDestinatarioCodigoIntegracao))
                    descricao += this.ClienteDestinatarioCodigoIntegracao + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;
                if (!string.IsNullOrWhiteSpace(this.ClienteDestinatarioTipo))
                    descricao += " (" + this.ClienteDestinatarioCPF_CNPJ.ToString().ObterCpfOuCnpjFormatado(this.ClienteDestinatarioTipo) + ")";

                return descricao;
            }
        }
        public string Tomador
        {
            get
            {
                string descricao = "";

                string nome = this.ClienteTomadorNome;
                if (this.ClienteTomadorPontoTransbordo)
                    nome = this.ClienteTomadorNomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.ClienteTomadorCodigoIntegracao))
                    descricao += this.ClienteTomadorCodigoIntegracao + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;
                if (!string.IsNullOrWhiteSpace(this.ClienteTomadorTipo))
                    descricao += " (" + this.ClienteTomadorCPF_CNPJ.ToString().ObterCpfOuCnpjFormatado(this.ClienteTomadorTipo) + ")";

                return descricao;
            }
        }
        public DateTime DataIntegracao { get; set; }


        #endregion


        #region Metodos

        private string ObterDescricaoCidadeEstado(int cargaEmpresaCodigo)
        {
            if (cargaEmpresaCodigo > 0)
                return this.CargaDescricaoCidadeEstado;
            return this.PedidoDescricaoCidadeEstado;
        }

        #endregion
    }
}