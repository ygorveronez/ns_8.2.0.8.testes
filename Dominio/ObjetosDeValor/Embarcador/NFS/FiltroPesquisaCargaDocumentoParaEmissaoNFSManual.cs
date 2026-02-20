using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NFS
{
    public sealed class FiltroPesquisaCargaDocumentoParaEmissaoNFSManual
    {
        #region Propriedades

        public string CodigoCargaEmbarcador { get; set; }

        public double CodigoDestinatario { get; set; }

        public int CodigoFilial { get; set; }

        /// <summary>
        /// Quando o código for maior que zero, retorna apenas os documentos do lançamento de NFS Manual
        /// </summary>
        public int CodigoLancamentoNFSManual { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public bool RetornarFiliaisTransportador { get; set; }

        public List<int> Codigosfilial { get; set; }

        public double CpfCnpjTomador { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public bool LancamentoNFSManualCancelado { get; set; }

        public Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        public int NumeroFinal { get; set; }

        public int NumeroInicial { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public bool Residuais { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.NFSManualTipoComplemento ComplementoOcorrencia { get; set; }

        public List<int> CodigosDocumentos { get; set; }

        #endregion

        #region Propriedades com Regras

        public List<Enumeradores.SituacaoCarga> SituacoesCargasPermitidas
        {
            get
            {
                return new List<Enumeradores.SituacaoCarga>() {
                    Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                    Enumeradores.SituacaoCarga.AgIntegracao,
                    Enumeradores.SituacaoCarga.EmTransporte,
                    Enumeradores.SituacaoCarga.Encerrada,
                    Enumeradores.SituacaoCarga.LiberadoPagamento
                };
            }
        }

        public List<Enumeradores.SituacaoOcorrencia> SituacoesOcorrenciasPermitidas
        {
            get
            {
                return new List<Enumeradores.SituacaoOcorrencia>() {
                    Enumeradores.SituacaoOcorrencia.Finalizada,
                    Enumeradores.SituacaoOcorrencia.AgIntegracao,
                    Enumeradores.SituacaoOcorrencia.FalhaIntegracao
                };
            }
        }

        #endregion
    }
}
