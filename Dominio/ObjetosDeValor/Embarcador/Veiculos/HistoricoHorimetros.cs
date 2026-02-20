using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public sealed class HistoricoHorimetros
    {
        public int? Codigo { get; set; }

        public string DataAlteracao { get; set; }

        public string HorimetroAtual { get; set; }

        public string Observacao { get; set; }


    }
}
