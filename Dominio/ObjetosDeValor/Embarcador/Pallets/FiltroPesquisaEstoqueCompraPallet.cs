using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaEstoqueCompraPallet
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public IList<int> ListaCodigoFilial { get; set; }
        public IList<int> ListaCodigoTransportador { get; set; }
        public IList<double> ListaCpfCnpjFornecedor { get; set; }
        public int NumeroNfe { get; set; }
    }
}
