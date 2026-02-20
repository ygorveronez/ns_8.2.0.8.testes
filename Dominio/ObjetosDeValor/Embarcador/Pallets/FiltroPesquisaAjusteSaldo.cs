using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public class FiltroPesquisaAjusteSaldo
    {
        #region Propriedades

        public List<int> CodigosTransportador { get; set; }

        public int NumeroDocumento { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoFilial { get; set; }

        public DateTime? DataMovimentoInicial { get; set; }

        public DateTime? DataMovimentoFinal { get; set; }

        public TipoMovimentacaoEstoquePallet? TipoMovimento { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public int CodigoTransportador {
            get
            {
                return ((CodigosTransportador != null) && (CodigosTransportador.Count == 1)) ? CodigosTransportador[0] : 0;
            }
            set
            {
                if (value > 0)
                    CodigosTransportador = new List<int>() { value };
            }
        }

        #endregion Propriedades com Regras
    }
}
