using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class PlanejamentoPedidoTMS
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int CodigoCarga { get; set; }
        public SituacaoPlanejamentoPedidoTMS SituacaoPlanejamentoPedidoTMS { get; set; }
        public string SituacaoPlanejamentoPedidoTMSFormatada { get { return this.SituacaoPlanejamentoPedidoTMS.ObterDescricao(); } }
        public bool PossuiVeiculo { get; set; }
        public bool PossuiMotorista { get; set; }
        public bool PossuiModeloVeicular { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public DateTime? DataCarregamentoPedido { get; set; }
        public string NumeroCelularMotorista { get; set; }
        public bool MotoristaCiente { get; set; }
        public string MotoristaCienteFormatado { get { return this.MotoristaCiente == true ? "Sim" : "Não"; } }
        public string CodigoCargaEmbarcador { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string UF { get; set; }
        public string Estado { get; set; }
        public string OrigemDescricao { get; set; }
        public string PaisAbreviacao { get; set; }
        public string NomePais { get; set; }
        public int CodigoIBGE { get; set; }
        public int SituacaoCarga { get; set; }
        public int TipoPessoa { get; set; }
        public TipoTomador TipoTomador { get; set; }
        public int CodigoPais { get; set; }
        public string Destino { get; set; }
        public string EstadoDestinoSigla { get; set; }
        public string DestinoFormatado { get { return this.Destino + " - " + this.EstadoDestinoSigla; }  }
        public string DestinoRecebedor { get; set; }
        public string ModeloVeicularCarga { get; set; }
        public string ObservacaoInterna { get; set; }
        public string NumeroRota { get; set; }
        public string Motoristas { get; set; }
        public string Proprietario { get; set; }
        public string ProprietarioVeiculoTracao { get; set; }
        public DateTime HoraColeta { get; set; }
        public DateTime DataColeta { get; set; }
        public bool Tratativa { get; set; }
        public string TratativaFormatada { get { return this.Tratativa == true ? "Sim" : "Não"; } }
        public DateTime? DataPrevisaoSaida { get; set; }
        public DateTime? PrevisaoEntrega { get; set; }
        public string CentroResultado { get; set; }
        public string TipoDeCarga { get; set; }
        public string Tomador { get; set; }
        public decimal Peso { get; set; }
        public string LocalColeta { get; set; }
        public string Expedidores { get; set; }
        public string Remetentes { get; set; }
        public string RemetenteCodigoIntegracao { get; set; }
        public string Destinatarios { get; set; }
        public string DestinatarioCodigoIntegracao { get; set; }
        public string Expedidor { get; set; }
        public string ExpedidorCodigoIntegracao { get; set; }
        public string RecebedorNome { get; set; }
        public string RecebedorCodigoIntegracao { get; set; }
        public string LocalColetaFormatado { get { return !string.IsNullOrWhiteSpace(this.Expedidor) ? this.Expedidor : this.Remetente; } }
        public string LocalEntrega { get; set; }
        public string Recebedores { get; set; }
        public string LocalEntregaFormatado { get { return !string.IsNullOrWhiteSpace(this.Recebedores) ? this.Recebedores : this.Destinatarios; } }
        public string TipoOperacao { get; set; }
        public int TipoOperacaoCodigo { get; set; }
        public string TipoOperacaoObservacao { get; set; }
        public string TipoPropriedadeVeiculo { get; set; }
        public string CodigoIntegracaoFronteira { get; set; }
        public string FronteiraNome { get; set; }
        public string TipoFronteira { get; set; }
        public string FronteiraNomeFantasia { get; set; }
        public double  CPFCNPJFronteira { get; set; }
        public bool PontoTransbordo { get; set; }
        public string Remetente { get; set; }
        public string Gestor { get; set; }
        public IndicativoColetaEntrega IndicativoColetaEntrega { get; set; }
        public string IndicativoColetaEntregaDescricao { get { return this.IndicativoColetaEntrega.ObterDescricao(); } }
        public string Destinatario { get; set; }
        public double DestinatarioCPF { get; set; }
        public double ExpedidorCpf { get; set; }
        public string ExpedidorCpfFormatado { get { return !string.IsNullOrWhiteSpace(ExpedidorCodigoIntegracao) ? $"{ExpedidorCodigoIntegracao} - {this.Expedidor} ({this.ExpedidorCpf.ToString().ObterCpfOuCnpjFormatado()})" : $"{this.Expedidor} ({this.ExpedidorCpf.ToString().ObterCpfOuCnpjFormatado()})"; } }

        public double RecebedorNomeCPF { get; set; }
        public string RecebedorNomeFormatado { get { return !string.IsNullOrWhiteSpace(RecebedorCodigoIntegracao) ? $"{RecebedorCodigoIntegracao} - {this.RecebedorNome} ({this.RecebedorNomeCPF.ToString().ObterCpfOuCnpjFormatado()})" : $"{this.RecebedorNome} ({this.RecebedorNomeCPF.ToString().ObterCpfOuCnpjFormatado()})"; } }

        public double TomadorCPF { get; set; }
        public string TomadorCodigoIntegracao { get; set; }
        public string TomadorFormatado { get { return !string.IsNullOrWhiteSpace(TomadorCodigoIntegracao) ? $"{TomadorCodigoIntegracao} - {this.Tomador} ({this.TomadorCPF.ToString().ObterCpfOuCnpjFormatado()})" : $"{this.Tomador} ({this.TomadorCPF.ToString().ObterCpfOuCnpjFormatado()})"; } }

        public string DestinatarioFormatado { get { return !string.IsNullOrWhiteSpace(DestinatarioCodigoIntegracao) ? $"{DestinatarioCodigoIntegracao} - {this.Destinatario} ({this.DestinatarioCPF.ToString().ObterCpfOuCnpjFormatado()})" : $"{this.Destinatario} ({this.DestinatarioCPF.ToString().ObterCpfOuCnpjFormatado()})"; } }
        public double RemetenteCPF { get; set; }
        public string RemetenteCPFFormatado { get { return !string.IsNullOrWhiteSpace(RemetenteCodigoIntegracao) ? $"{RemetenteCodigoIntegracao} - {this.Remetente} ({this.RemetenteCPF.ToString().ObterCpfOuCnpjFormatado()})" : $"{RemetenteCodigoIntegracao} - {this.Remetente} ({this.RemetenteCPF.ToString().ObterCpfOuCnpjFormatado()})"; } }

        public decimal NumeroPaletesFracionado { get; set; }
        public decimal PalletSaldoRestante { get; set; }
        public decimal SomaPallet { get; set; }
        public SituacaoIntegracao Integracao { get; set; }
        public string SituacaoIntegracao { get { return this.Integracao.ObterDescricao(); } }
        public SituacaoPedido SituacaoPedido { get; set; }
        public string DT_RowColor { get { return this.SituacaoPedido == SituacaoPedido.Cancelado ? CorGrid.Red : this.SituacaoPlanejamentoPedidoTMS.ObterCorLinha(); } }
        public string TaraContainer { get; set; }
        public string LacreContainerUm { get; set; }
        public string LacreContainerDois { get; set; }
        public string LacreContainerTres { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroNavio { get; set; }
        public decimal ValorTotalNotas { get; set; }
        public string Container { get; set; }
        public string TipoContainerDescricao { get; set; }
        public CategoriaOS CategoriaOS { get; set; }
        public TipoOSConvertido TipoOSConvertido { get; set; }

        #endregion

        public string Origem
        {
            get
            {
                if (this.CodigoIBGE != 9999999 || this.CodigoPais > 0)
                    return this.OrigemDescricao + " - " + this.UF ?? "";
                else
                {
                    if (!string.IsNullOrWhiteSpace(this.PaisAbreviacao))
                        return this.OrigemDescricao + " - " + this.PaisAbreviacao;
                    else
                        return this.OrigemDescricao + " - " + this.NomePais;
                }
            }
        }

        public virtual string CPF_CNPJ_Formatado
        {
            get
            {
                if (this.TipoFronteira.Equals("E"))
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return this.TipoFronteira.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJFronteira) : String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJFronteira);
                }
            }
        }
        public virtual string DescricaoFronteira
        {
            get
            {
                string descricao = "";

                string nome = this.FronteiraNome;
                if (this.PontoTransbordo)
                    nome = this.FronteiraNomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.CodigoIntegracaoFronteira))
                    descricao += this.CodigoIntegracaoFronteira + " - ";
                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;
                if (!string.IsNullOrWhiteSpace(this.TipoFronteira))
                    descricao += " (" + this.CPF_CNPJ_Formatado + ")";

                return descricao;
            }
        }
        public string DT_FontColor
        {
            get
            {
                return this.SituacaoPlanejamentoPedidoTMS == SituacaoPlanejamentoPedidoTMS.CargaGerouDocumentacao ||
                         this.SituacaoPlanejamentoPedidoTMS == SituacaoPlanejamentoPedidoTMS.CargaCanceladaAnulada ||
                         this.SituacaoPlanejamentoPedidoTMS == SituacaoPlanejamentoPedidoTMS.AvisoAoMotorista ||
                         this.SituacaoPlanejamentoPedidoTMS == SituacaoPlanejamentoPedidoTMS.MotoristaCiente ||
                         this.SituacaoPedido == SituacaoPedido.Cancelado ? CorGrid.Branco : CorGrid.Black;
            }
        }

        public string ObterTomador
        {
            get
            {
                if (TipoTomador == TipoTomador.Remetente)
                    return this.RemetenteCPFFormatado;

                if (this.TipoTomador == TipoTomador.Expedidor)
                    return this.ExpedidorCpfFormatado;

                else if (TipoTomador == TipoTomador.Recebedor)
                    return this.RecebedorNomeFormatado;

                else if (TipoTomador == TipoTomador.Destinatario)
                    return this.DestinatarioFormatado;

                else if (TipoTomador == TipoTomador.Tomador)
                    return this.TomadorFormatado;
                else
                    return null;
            }
        }

        public string CategoriaOSDescricao
        {
            get { return CategoriaOS.ObterDescricao(); }
        }

        public string TipoOSConvertidoDescricao
        {
            get { return TipoOSConvertido.ObterDescricao(); }
        }
    }
}