using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NFS
{
    public sealed class FiltroPesquisaCargaDescontoParaEmissaoNFSManual
    {
        #region Propriedades

        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public List<int> Codigosfilial { get; set; }

        public double CpfCnpjTomador { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

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

        #endregion
    }
}
