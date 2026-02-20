using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaControleValePallet
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataLimite { get; set; }
        public IList<int> ListaCodigoFilial { get; set; }
        public IList<double> ListaCpfCnpjCliente { get; set; }
        public int NumeroNfe { get; set; }
        public Enumeradores.SituacaoValePallet? Situacao { get; set; }
    }
}
