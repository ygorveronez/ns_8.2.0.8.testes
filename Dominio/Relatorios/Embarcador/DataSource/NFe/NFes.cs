using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Linq;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class NFes
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Fatura { get; set; }
        public string CTe { get; set; }
        public string SerieCTe { get; set; }
        public string TipoCTe { get; set; }
        public string CFOP { get; set; }
        public decimal Frete { get; set; }
        public decimal Peso { get; set; }
        public decimal FreteTotalCTe { get; set; }
        public decimal ValorMercadoriaTotalCTe { get; set; }
        public decimal NumeroPallet { get; set; }
        public decimal ValorNFe { get; set; }
        public decimal FreteTotalLiquidoCTe { get; set; }
        public decimal FreteEmbarcador { get; set; }
        public decimal FreteTabelaFrete { get; set; }
        public decimal ValorICMS { get; set; }
        public string ChaveNFe { get; set; }
        public int NotasFiscais { get; set; }
        public string NaturezaNF { get; set; }
        public string Produto { get; set; }
        public string CodigoProduto { get; set; }
        public string CodigocEAN { get; set; }
        public string UnidadeComercial { get; set; }
        public decimal QuantidadeProduto { get; set; }
        public decimal QuantidadeTotalProduto { get; set; }
        public string SerieNotaFiscal { get; set; }
        public string CodigoDestinatario { get; set; }
        public string Destinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string TelefoneDestinatario { get; set; }
        public string EmailDestinatario { get; set; }
        public string TipoPessoaDestinatario { get; set; }
        public double CNPJDestinatarioSemFormato { get; set; }
        public string GrupoDestinatario { get; set; }
        public string CategoriaDestinatario { get; set; }
        public string Destino { get; set; }
        public string Origem { get; set; }
        public string CodigoRemetente { get; set; }
        public string Remetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoCarga { get; set; }
        public string TipoPessoaRemetente { get; set; }
        public double CNPJRemetenteSemFormato { get; set; }
        public string GrupoRemetente { get; set; }
        public string CategoriaRemetente { get; set; }
        public string CodigoTomador { get; set; }
        public string Tomador { get; set; }
        public string TipoPessoaTomador { get; set; }
        public double CNPJTomadorSemFormato { get; set; }
        public decimal ValorComponentes { get; set; }
        public decimal _ICMS { get; set; }
        public decimal _ValorISS { get; set; }
        public decimal ValorISSRetido { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal AliquotaISS { get; set; }
        public string CST { get; set; }
        public string CSTIBSCBS { get; set; }
        public string ClassificacaoTributariaIBSCBS { get; set; }
        public decimal BaseCalculoIBSCBS { get; set; }
        public decimal AliquotaIBSEstadual { get; set; }
        public decimal PercentualReducaoIBSEstadual { get; set; }
        public decimal ValorReducaoIBSEstadual { get; set; }
        public decimal ValorIBSEstadual { get; set; }
        public decimal AliquotaIBSMunicipal { get; set; }
        public decimal PercentualReducaoIBSMunicipal { get; set; }
        public decimal ValorReducaoIBSMunicipal { get; set; }
        public decimal ValorIBSMunicipal { get; set; }
        public decimal AliquotaCBS { get; set; }
        public decimal PercentualReducaoCBS { get; set; }
        public decimal ValorReducaoCBS { get; set; }
        public decimal ValorCBS { get; set; }
        public string CodigoEmpresa { get; set; }
        public string CNPJEmpresaSemFormato { get; set; }
        public string Empresa { get; set; }
        public string Filial { get; set; }
        public DateTime DataNotaFiscal { get; set; }
        public DateTime DataDigitalizacaoCanhoto { get; set; }
        public SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataCarga { get; set; }
        public string Pedido { get; set; }
        public DateTime DataHoraPedido { get; set; }
        public decimal PesoPedido { get; set; }
        public decimal ValorPedido { get; set; }
        public int Volumes { get; set; }
        public string NumeroCarga { get; set; }
        public string Motorista { get; set; }
        public string Itinerario { get; set; }
        public string RotaCarga { get; set; }
        public string Placa { get; set; }
        public double CapacidadeVeiculo { get; set; }
        public decimal MetroCubico { get; set; }
        public decimal MetroCubicoNFe { get; set; }
        public string Restricoes { get; set; }
        public string GrupoPessoa { get; set; }
        public string CEPDestinatario { get; set; }
        public string PaisDestinatario { get; set; }
        public DateTime PrevisaoEntregaPedido { get; set; }
        public DateTime DataRealizadaEntrega { get; set; }
        public string Atendimentos { get; set; }
        public string SituacaoNotaFiscal { get; set; }
        public SituacaoNotaFiscal SituacaoNotaFiscalEntrega { get; set; }
        public SituacaoNotaFiscal UltimaSituacaoEntregaDevolucao { get; set; }
        public SituacaoEntrega SituacaoEntrega { get; set; }
        public int SLADocumento { get; set; }
        public string ModeloVeicular { get; set; }
        public string NomeVendedor { get; set; }
        public string TelefoneVendedor { get; set; }
        public decimal KMFilialCliente { get; set; }
        public int MesPrevisaoEntregaPedido { get; set; }
        public int SemanaPrevisaoEntregaPedido { get; set; }
        public string NumeroRemessaEspelho { get; set; }
        public int QuantidadeVolumes { get; set; }
        public ClassificacaoNFe ClassificacaoNFe { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public DateTime DataInicioViagem { get; set; }
        public SituacaoCarga Situacao { get; set; }
        public DateTime DataSituacaoNotaFiscalEntrega { get; set; }
        public string NomeFantasiaExpedidor { get; set; }
        public string CodigoSapExpedidor { get; set; }
        public string Recebedor { get; set; }
        public string MotivosAtendimentos { get; set; }
        private int QuantidadeCaixa { get; set; }
        public string ObservacaoPedido { get; set; }
        public string IdAgrupador { get; set; }
        public string CodigoSAPDestinatario { get; set; }
        public string NumeroDestino { get; set; }
        public string UFDestino { get; set; }
        public string UFOrigem { get; set; }
        private DateTime DataEntrega { get; set; }
        private DateTime DataPrevisaoCargaEntrega { get; set; }
        public string NaturezaOP { get; set; }
        public string RecebedorCarga { get; set; }
        public string ExpedidorCarga { get; set; }
        public string Transbordo { get; set; }
        public string NumeroPedidoNFe { get; set; }

        #endregion

        #region Propriedades com Regras

        public string ClassificacaoNFeDescricao
        {
            get
            {
                return ClassificacaoNFe.ObterDescricao();
            }
        }

        public string CNPJDestinatario
        {
            get
            {
                if (TipoPessoaDestinatario == "F")
                    return String.Format(@"{0:000\.000\.000\-00}", CNPJDestinatarioSemFormato);
                else
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJDestinatarioSemFormato);
            }
        }

        public string CNPJEmpresa
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJEmpresaSemFormato);
            }
        }

        public string CNPJRemetente
        {
            get
            {
                if (TipoPessoaRemetente == "F")
                    return String.Format(@"{0:000\.000\.000\-00}", CNPJRemetenteSemFormato);
                else
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJRemetenteSemFormato);
            }
        }

        public string CNPJTomador
        {
            get
            {
                if (TipoPessoaTomador == "F")
                    return String.Format(@"{0:000\.000\.000\-00}", CNPJTomadorSemFormato);
                else
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJTomadorSemFormato);
            }
        }

        public string DataInicioViagemFormatada
        {
            get { return DataInicioViagem != DateTime.MinValue ? DataInicioViagem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPedido
        {
            get
            {
                return DataHoraPedido != DateTime.MinValue ? DataHoraPedido.ToString("dd/MM/yyyy") : "";
            }
        }

        public string DataSituacaoNotaFiscalEntregaFormatada
        {
            get { return DataSituacaoNotaFiscalEntrega != DateTime.MinValue ? DataSituacaoNotaFiscalEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

        public string DescricaoSituacaoEntrega
        {
            get { return this.SituacaoEntrega.ObterDescricao(); }
        }

        public string DescricaoSituacaoNotaFiscalEntrega
        {
            get { return UltimaSituacaoEntregaDevolucao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNotaFiscal.Nenhum ? UltimaSituacaoEntregaDevolucao.ObterDescricao() : SituacaoNotaFiscalEntrega.ObterDescricao(); }
        }

        public string DescricaoSituacaoDigitalizacaoCanhoto
        {
            get
            {
                return this.SituacaoDigitalizacaoCanhoto.ObterDescricao();
            }
        }

        public string DataDigitalizacaoCanhotoFormatada
        {
            get
            {
                return this.DataDigitalizacaoCanhoto != DateTime.MinValue ? this.DataDigitalizacaoCanhoto.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public decimal FreteTotalSemImposto
        {
            get
            {
                return ValorComponentes + Frete;
            }
        }

        public string HoraPedido
        {
            get
            {
                return DataHoraPedido != DateTime.MinValue ? DataHoraPedido.ToString("HH:mm") : "";
            }
        }

        public decimal ICMS
        {
            get
            {
                return CST != "60" ? _ICMS : 0;
            }
        }

        public string TipoCTeFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TipoCTe))
                    return TipoCTe;

                string[] listatipos = TipoCTe.Split(',');

                return string.Join(", ", (from tipo in listatipos select TipoCTeHelper.ObterDescricao((TipoCTE)tipo.ToInt())));
            }
        }

        public decimal TotalReceber
        {
            get
            {
                return ValorComponentes + Frete + ICMS + ValorISS;
            }
        }

        public decimal ValorICMSST
        {
            get
            {
                return CST == "60" ? _ICMS : 0;
            }
        }

        public decimal ValorISS
        {
            get
            {
                return _ValorISS - ValorISSRetido;
            }
        }

        public decimal QuantidadePecas
        {
            get { return QuantidadeProduto * QuantidadeCaixa; }
        }

        public string DataEntregaFormatada
        {
            get { return DataEntrega != DateTime.MinValue ? DataEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoCargaEntregaFormatada
        {
            get { return DataPrevisaoCargaEntrega != DateTime.MinValue ? DataPrevisaoCargaEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        public string CodigoInteragracaoMigo { get; set; }
        public DateTime DataMigo { get; set; }
        public string DataMigoFormatado { get { return this.DataMigo != DateTime.MinValue ? this.DataMigo.ToString("dd/MM/yyyy hh:mm:ss") : string.Empty; } }
        public string CodigoInteragracaoMiro { get; set; }
        public DateTime DataMiro { get; set; }
        public string DataMiroFormatado { get { return this.DataMiro != DateTime.MinValue ? this.DataMiro.ToString("dd/MM/yyyy hh:mm:ss") : string.Empty; } }
        public decimal PesoLiquido { get; set; }

        #endregion

        #region Propriedades de Componentes

        public decimal ValorComponente1 { get; set; }
        public decimal ValorComponente2 { get; set; }
        public decimal ValorComponente3 { get; set; }
        public decimal ValorComponente4 { get; set; }
        public decimal ValorComponente5 { get; set; }
        public decimal ValorComponente6 { get; set; }
        public decimal ValorComponente7 { get; set; }
        public decimal ValorComponente8 { get; set; }
        public decimal ValorComponente9 { get; set; }
        public decimal ValorComponente10 { get; set; }
        public decimal ValorComponente11 { get; set; }
        public decimal ValorComponente12 { get; set; }
        public decimal ValorComponente13 { get; set; }
        public decimal ValorComponente14 { get; set; }
        public decimal ValorComponente15 { get; set; }
        public decimal ValorComponente16 { get; set; }
        public decimal ValorComponente17 { get; set; }
        public decimal ValorComponente18 { get; set; }
        public decimal ValorComponente19 { get; set; }
        public decimal ValorComponente20 { get; set; }
        public decimal ValorComponente21 { get; set; }
        public decimal ValorComponente22 { get; set; }
        public decimal ValorComponente23 { get; set; }
        public decimal ValorComponente24 { get; set; }
        public decimal ValorComponente25 { get; set; }
        public decimal ValorComponente26 { get; set; }
        public decimal ValorComponente27 { get; set; }
        public decimal ValorComponente28 { get; set; }
        public decimal ValorComponente29 { get; set; }
        public decimal ValorComponente30 { get; set; }

        #endregion
    }
}
