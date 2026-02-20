using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaJanelaDescarregamento
    {
        public int CodigoCentroDescarregamento { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public Enumeradores.SituacaoJanelaDescarregamento? Situacao { get; set; }
    }
}
