using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaHistoricoVinculoKmReboque
    {
        public DateTime? DataCriacaoInicial { get; set; }
        public DateTime? DataCriacaoFinal { get; set; }
        public DateTime? DataAlteracaoInicial { get; set; }
        public DateTime? DataAlteracaoFinal { get; set; }
        public int Veiculo { get; set; }
        public int Reboque { get; set; }                        
        public TipoMovimentoKmReboque TipoMovimentoKmReboque { get; set; }       
    }
}
