using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaAcompanhamentoChecklist
    {
        public int Filial { get; set; }

        public int TipoOperacao { get; set; }

        public int Transportador { get; set; }

        public bool? Situacao { get; set; }

        public DateTime DataCarregamento { get; set; }

        public List<int> CodigosCarga { get; set; }
    }
}
