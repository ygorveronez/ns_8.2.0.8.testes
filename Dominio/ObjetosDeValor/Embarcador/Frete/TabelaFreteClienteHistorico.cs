using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class TabelaFreteClienteHistorico
    {
        #region Propriedades

        public int CodigoTabelaFrete { get; set; }
        public Dictionary<int, List<int>> CodigosTabelasFreteClienteHistorico { get; set; }

        #endregion
    }
}
