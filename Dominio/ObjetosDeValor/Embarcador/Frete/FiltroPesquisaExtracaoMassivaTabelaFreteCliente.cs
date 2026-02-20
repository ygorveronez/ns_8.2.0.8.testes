using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaExtracaoMassivaTabelaFreteCliente
    {
        #region Propriedades

        public List<int> CodigosTabelasFrete { get; set; }
        public List<TabelaFreteClienteHistorico> TabelasFreteClienteHistorico { get; set; }
        public int CodigoTabelaFreteClientePesquisaParametrosAnteriores { get; set; }
        public DateTime? DataInicialAlteracao { get; set; }
        public DateTime? DataFinalAlteracao { get; set; }

        #endregion
    }
}
