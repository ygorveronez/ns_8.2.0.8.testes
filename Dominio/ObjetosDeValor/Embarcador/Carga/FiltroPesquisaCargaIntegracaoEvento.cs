using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaIntegracaoEvento
    {
        public string NumeroCarga { get; set; }

        public SituacaoIntegracao? SituacaoIntegracao { get; set; }

        public DateTime DataIntegracaoInicial { get; set; }

        public DateTime DataIntegracaoFinal { get; set; }

    }
}
