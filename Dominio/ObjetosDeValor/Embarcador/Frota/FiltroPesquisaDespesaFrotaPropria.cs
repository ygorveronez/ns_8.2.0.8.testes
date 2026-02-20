using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaDespesaFrotaPropria
    {
        public int CodigoFilial { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }

    }
}