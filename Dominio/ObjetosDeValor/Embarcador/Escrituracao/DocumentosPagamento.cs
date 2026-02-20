using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class DocumentosPagamento
    {
        public Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao { get; set; }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual { get; set; }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        public int? Fechamento { get; set; }

        public int? Carga { get; set; }

        public int? Ocorrencia { get; set; }


        public virtual string CST
        {
            get
            {
                if (this.Ocorrencia != null && this.cargaDocumentoParaEmissaoNFSManual != null)
                    return this.cargaDocumentoParaEmissaoNFSManual.CTe?.CST ?? "";
                else if(this.pedidoXMLNotaFiscal != null)
                    return this.pedidoXMLNotaFiscal.CST;
                else if (this.pedidoCTeParaSubContratacao != null)
                    return this.pedidoCTeParaSubContratacao.CST;
                else if (this.CTe != null)
                    return this.CTe.CST;
                else
                    return this.cargaDocumentoParaEmissaoNFSManual.CTe?.CST ?? "";
            }
        }

        public virtual decimal ValorICMS
        {
            get
            {
                if (this.pedidoXMLNotaFiscal != null)
                    return this.pedidoXMLNotaFiscal.ValorICMS;
                else if (this.pedidoCTeParaSubContratacao != null)
                    return this.pedidoCTeParaSubContratacao.ValorICMS;
                else if (this.CTe != null)
                    return this.CTe.ValorICMS;
                else
                    return 0;
            }
        }

        public virtual decimal ValorISS
        {
            get
            {
                if (this.Ocorrencia != null && this.cargaDocumentoParaEmissaoNFSManual != null)
                    return this.cargaDocumentoParaEmissaoNFSManual?.ValorISS ?? 0;
                else if (this.pedidoXMLNotaFiscal != null)
                    return this.pedidoXMLNotaFiscal.ValorISS;
                else if (this.pedidoCTeParaSubContratacao != null)
                    return this.pedidoCTeParaSubContratacao.ValorISS;
                else if (this.CTe != null)
                    return this.CTe.ValorISS;
                else
                    return this.cargaDocumentoParaEmissaoNFSManual?.ValorISS ?? 0;
            }
        }

        public virtual decimal AliquotaISS
        {
            get
            {
                if (this.Ocorrencia != null && this.cargaDocumentoParaEmissaoNFSManual != null)
                    return this.cargaDocumentoParaEmissaoNFSManual?.PercentualAliquotaISS ?? 0;
                else if (this.pedidoXMLNotaFiscal != null)
                    return this.pedidoXMLNotaFiscal.PercentualAliquotaISS;
                else if (this.pedidoCTeParaSubContratacao != null)
                    return this.pedidoCTeParaSubContratacao.PercentualAliquotaISS;
                else if (this.CTe != null)
                    return this.CTe.AliquotaISS;
                else
                    return this.cargaDocumentoParaEmissaoNFSManual?.PercentualAliquotaISS ?? 0;
            }
        }

        public virtual decimal Valor
        {
            get
            {
                if (this.Ocorrencia != null && this.cargaDocumentoParaEmissaoNFSManual != null)
                    return this.cargaDocumentoParaEmissaoNFSManual?.ValorFrete ?? 0;
                else if (this.pedidoXMLNotaFiscal != null)
                    return this.pedidoXMLNotaFiscal.ValorFrete + this.pedidoXMLNotaFiscal.ValorTotalComponentes;
                else if (this.pedidoCTeParaSubContratacao != null)
                    return this.pedidoCTeParaSubContratacao.ValorFrete + this.pedidoCTeParaSubContratacao.ValorTotalComponentes;
                else if (this.CTe != null)
                {
                    return this.CTe.ValorFrete + ((from obj in CTe.ComponentesPrestacao where obj.ComponenteFrete != null && obj.ComponenteFrete.TipoComponenteFrete != Enumeradores.TipoComponenteFrete.ICMS && obj.ComponenteFrete.TipoComponenteFrete != Enumeradores.TipoComponenteFrete.ISS && obj.ComponenteFrete.TipoComponenteFrete != Enumeradores.TipoComponenteFrete.PISCONFIS select obj.Valor).Sum());
                }
                else
                    return this.cargaDocumentoParaEmissaoNFSManual?.ValorFrete ?? 0;
            }
        }

        public virtual decimal ValorRetencaoISS
        {
            get
            {
                if (this.Ocorrencia != null && this.cargaDocumentoParaEmissaoNFSManual != null)
                    return this.cargaDocumentoParaEmissaoNFSManual?.ValorRetencaoISS ?? 0;
                else if (this.pedidoXMLNotaFiscal != null)
                    return this.pedidoXMLNotaFiscal.ValorRetencaoISS;
                else if (this.pedidoCTeParaSubContratacao != null)
                    return this.pedidoCTeParaSubContratacao.ValorRetencaoISS;
                else if (this.CTe != null)
                    return this.CTe.ValorISSRetido;
                else
                    return this.cargaDocumentoParaEmissaoNFSManual?.ValorRetencaoISS ?? 0;
            }
        }

        public virtual bool ISSInclusoBC
        {
            get
            {
                if (this.Ocorrencia != null && this.cargaDocumentoParaEmissaoNFSManual != null)
                    return this.cargaDocumentoParaEmissaoNFSManual.CTe.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
                else if (this.pedidoXMLNotaFiscal != null)
                    return this.pedidoXMLNotaFiscal.IncluirISSBaseCalculo;
                else if (this.pedidoCTeParaSubContratacao != null)
                    return this.pedidoCTeParaSubContratacao.IncluirISSBaseCalculo;
                else if (this.CTe != null)
                    return CTe.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
                else
                {
                    if (this.cargaDocumentoParaEmissaoNFSManual.CTe != null)
                        return this.cargaDocumentoParaEmissaoNFSManual.CTe.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
                    else
                        return false;
                }

            }
        }

        public virtual bool ICMSInclusoBC
        {
            get
            {
                if (this.Ocorrencia != null && this.cargaDocumentoParaEmissaoNFSManual != null)
                    return this.cargaDocumentoParaEmissaoNFSManual.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
                else if(this.pedidoXMLNotaFiscal != null)
                    return this.pedidoXMLNotaFiscal.IncluirICMSBaseCalculo;
                else if (this.pedidoCTeParaSubContratacao != null)
                    return this.pedidoCTeParaSubContratacao.IncluirICMSBaseCalculo;
                else if (this.CTe != null)
                    return CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
                else
                {
                    if (this.cargaDocumentoParaEmissaoNFSManual.CTe != null)
                        return this.cargaDocumentoParaEmissaoNFSManual.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim;
                    else
                        return false;
                }
            }
        }

    }
}
