using System;

namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public sealed class ObterAgendamentos
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public Embarcador.Filial.Filial Filial { get; set; }
    }
}
