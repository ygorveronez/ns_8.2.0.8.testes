using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaFichaClienteLancamento
    {
        public double CpfCnpjPessoa { get; set; }

        public DateTime DataLancamento { get; set; }

        public decimal ValorLancamento { get; set; }

        public Dominio.Enumeradores.TipoMovimento TipoLancamento { get; set; }

    }
}

