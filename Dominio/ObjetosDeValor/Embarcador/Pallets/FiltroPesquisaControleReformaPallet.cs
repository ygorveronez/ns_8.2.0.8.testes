using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaControleReformaPallet
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public DateTime? DataRetornoInicio { get; set; }
        public DateTime? DataRetornoLimite { get; set; }
        public bool ExibirNfs { get; set; }
        public IList<int> ListaCodigoFilial { get; set; }
        public IList<int> ListaCodigoTransportador { get; set; }
        public IList<double> ListaCpfCnpjFornecedor { get; set; }
        public int NumeroNfe { get; set; }
        public int NumeroNfeRetorno { get; set; }
    }
}
