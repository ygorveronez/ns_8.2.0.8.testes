using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class FiltroPesquisaReprocessarAbastecimento
    {
        public List<int> CodigosVeiculos { get; set; }
        public List<int> CodigosEquipamentos { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string SituacaoAbastecimento { get; set; }
    }
}
