using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class FiltroPesquisaMontagemCargaPatio
    {
        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public string NumeroCarga { get; set; }

        public Enumeradores.SituacaoMontagemCargaPatio? Situacao { get; set; }
    }
}
