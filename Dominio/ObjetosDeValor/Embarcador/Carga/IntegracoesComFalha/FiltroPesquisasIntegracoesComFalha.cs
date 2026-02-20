using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.IntegracoesComFalha
{
    public class FiltroPesquisaIntegracoesComFalha
    {
        public DateTime? DataInicial { get; set; }

        public DateTime? DataFim { get; set; }

        public List<int> CodigosCarga { get; set; }

        public Enumeradores.TipoIntegracao? TipoIntegracao { get; set; }

        public Enumeradores.SituacaoCarga? EtapaCarga { get; set; }
    }
}
