using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaFluxoEncerramentoCarga
    {
        public string NumeroCarga { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public SituacaoEncerramentoCarga? Situacao { get; set; }
    }
}
