using Dominio.Entidades.Embarcador.ISS;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Relatorios.Embarcador.DataSource.PreCTes
{
    public class PreCTe
    {
        public int Codigo { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoDeCarga { get; set; }
        public int NumeroCTe { get; set; }
        public string ModeloVeicular { get; set; }
        public string ModeloVeiculoCarga { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CNPJFilial {  get; set; }
        public string FilialDescricao { get; set; }
        public TipoTomador TipoTomador { get; set; }
        private Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }
        public string NumeroCarga { get; set; }
        public decimal ValorPrestacaoServico { get; set; }
        public string TipoOcorrencia { get; set; }
        public string Ocorrencia { get; set; }
        public decimal ValorFrete { get; set; }
        public string CST { get; set; }
        public decimal PesoKg { get; set; }
        public decimal PesoLiquidoKg { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal AliquiotaISS {  get; set; }
        public decimal ValorISS { get; set; }
        public decimal AliquotaPIS {  get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public decimal ValorMercadoria { get; set; }
        public string TabelaFrete { get; set; }
        public string CodigoTabelaFreteCliente { get; set; }
        public string TabelaFreteCliente { get; set; }
        public string ProdutoPredominante { get; set; }
        public string NomeFantasiaTransportador { get; set; }
        public string RazaoSocialTransportador { get; set; }
        private string CNPJTransportador { get; set; }
        public string UFTransportador { get; set; }
        public string CPFMotorista { get; set; }
        public string Motorista { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string ChaveNotaFiscal { get; set; }
        public int CFOP { get; set; }

        public string LocalidadeRemetente { get; set; }
        public string LocalidadeTomador { get; set; }
        public string LocalidadeRecebedor { get; set; }
        public string LocalidadeExpedidor { get; set; }
        public string LocalidadeDestinatario { get; set; }

        public string UFExpedidor { get; set; }
        public string UFRecebedor { get; set; }
        public string UFRemetente { get; set; }
        public string UFDestinatario { get; set; }
        public string UFTomador { get; set; }

        public string CodigoRemetente { get; set; }
        public string CodigoRecebedor { get; set; }
        public string CodigoDestinatario { get; set; }
        public string CodigoTomador { get; set; }
        public string CodigoExpedidor { get; set; }

        public string CPFCNPJRemetente { get; set; }
        public string CPFCNPJRecebedor{ get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string CPFCNPJTomador { get; set; }
        public string CPFCNPJExpedidor { get; set; }


        public string IERemetente { get; set; }
        public string IERecebedor{ get; set; }
        public string IEDestinatario { get; set; }
        public string IEExpedidor { get; set; }
        public string IETomador { get; set; }

        public string Expedidor { get; set; }
        public string Remetente { get; set; }
        public string Recebedor { get; set; }
        public string Destinatario { get; set; }
        public string Tomador { get; set; }

        public string CodigoDocumentoExpedidor { get; set; }
        public string CodigoDocumentoRecebedor { get; set; }
        public string CodigoDocumentoRemetente { get; set; }
        public string CodigoDocumentoTomador { get; set; }
        public string CodigoDocumentoDestinatario { get; set; }

        public string VeiculoTracao { get; set; }
        public string VeiculoReboque { get; set; }

        public int IBGEFimPrestacao { get; set; }
        public string FimPrestacao { get; set; }
        public string UFFimPrestacao { get; set; }

        public int IBGEInicioPrestacao { get; set; }
        public string InicioPrestacao { get; set; }
        public string UFInicioPrestacao { get; set; }

        public decimal PesoPedido { get; set; }
        public string NumeroFolha { get; set; }
        public string DataFolha { get; set; }
        public string FolhaCalculada { get; set; }
        public string FolhaAtribuida { get; set; }
        public string FolhaTransferida { get; set; }
        public string FolhaCancelada { get; set; }
        public string FolhaInconsistente { get; set; }
        public string InconsistenciaFolha { get; set; }
        public SituacaoRelatorioPreCTe SituacaoPreCTe { get; set; }

        public string LocalidadeFimPrestacao { get; set; }

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
        public decimal ValorComponente31 { get; set; }
        public decimal ValorComponente32 { get; set; }
        public decimal ValorComponente33 { get; set; }
        public decimal ValorComponente34 { get; set; }
        public decimal ValorComponente35 { get; set; }
        public decimal ValorComponente36 { get; set; }
        public decimal ValorComponente37 { get; set; }
        public decimal ValorComponente38 { get; set; }
        public decimal ValorComponente39 { get; set; }
        public decimal ValorComponente40 { get; set; }
        public decimal ValorComponente41 { get; set; }
        public decimal ValorComponente42 { get; set; }
        public decimal ValorComponente43 { get; set; }
        public decimal ValorComponente44 { get; set; }
        public decimal ValorComponente45 { get; set; }
        public decimal ValorComponente46 { get; set; }
        public decimal ValorComponente47 { get; set; }
        public decimal ValorComponente48 { get; set; }
        public decimal ValorComponente49 { get; set; }
        public decimal ValorComponente50 { get; set; }
        public decimal ValorComponente51 { get; set; }
        public decimal ValorComponente52 { get; set; }
        public decimal ValorComponente53 { get; set; }
        public decimal ValorComponente54 { get; set; }
        public decimal ValorComponente55 { get; set; }
        public decimal ValorComponente56 { get; set; }
        public decimal ValorComponente57 { get; set; }
        public decimal ValorComponente58 { get; set; }
        public decimal ValorComponente59 { get; set; }
        public decimal ValorComponente60 { get; set; }




        public string CSTFormatada
        {
            get { return TipoICMSHelper.ObterTipoDescricao(CST); }
        }

        public string CNPJTransportadorFormatado
        {
            get { return CNPJTransportador != null ? CNPJTransportador.ObterCpfOuCnpjFormatado() : string.Empty; }
        }


        public decimal ValorCOFINS
        {
            get { return Math.Round(BaseCalculoICMS * (AliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public decimal ValorPIS
        {
            get { return Math.Round(BaseCalculoICMS * (AliquotaPIS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public string DescricaoTipoTomador
        {
            get
            {
                switch (this.TipoTomador)
                {
                    case Dominio.Enumeradores.TipoTomador.Remetente:
                        return "Remetente";
                    case Dominio.Enumeradores.TipoTomador.Expedidor:
                        return "Expedidor";
                    case Dominio.Enumeradores.TipoTomador.Recebedor:
                        return "Recebedor";
                    case Dominio.Enumeradores.TipoTomador.Destinatario:
                        return "Destinatário";
                    case Dominio.Enumeradores.TipoTomador.Outros:
                        return "Outro";
                    default:
                        return "";
                }
            }
        }

        public string DescricaoTipoPagamento
        {
            get { return TipoPagamento.ObterDescricao(); }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SituacaoPreCTeDescricao
        {
            get { return this.SituacaoPreCTe.ObterDescricao(); }
        }
    }
}
