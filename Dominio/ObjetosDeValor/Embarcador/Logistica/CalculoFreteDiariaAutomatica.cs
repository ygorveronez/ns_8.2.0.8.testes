using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class CalculoFreteDiariaAutomatica
    {
        public bool ValorCalculadoPorTabelaFrete { get; set; }

        public decimal ValorDiariaAutomatica { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete> ListaComposicaoFrete;
    }
}
