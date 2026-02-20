using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escalas
{
    public sealed class FiltroPesquisaGeracaoEscala
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoProduto { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public int NumeroEscala { get; set; }

        public Enumeradores.SituacaoEscala? SituacaoEscala { get; set; }
    }
}
