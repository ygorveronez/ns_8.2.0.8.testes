using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class FiltroPesquisaControleEntradaSaidaPallet
    {
        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public List<int> ListaCodigoFilial { get; set; }

        public List<int> ListaCodigoTransportador { get; set; }

        public List<double> ListaCpfCnpjCliente { get; set; }

        public Enumeradores.NaturezaMovimentacaoEstoquePallet? NaturezaMovimentacao { get; set; }
    }
}
