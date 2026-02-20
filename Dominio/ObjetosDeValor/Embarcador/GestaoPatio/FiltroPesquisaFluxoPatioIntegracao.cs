using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class FiltroPesquisaFluxoPatioIntegracao
    {
        public string NumeroCarga { get; set; }
        public SituacaoIntegracao? SituacaoIntegracao { get; set; }
        public int CodigoIntegradora { get; set; }
        public DateTime? Data { get; set; }
        public EtapaFluxoGestaoPatio EtapaFluxo { get; set; }
    }
}
