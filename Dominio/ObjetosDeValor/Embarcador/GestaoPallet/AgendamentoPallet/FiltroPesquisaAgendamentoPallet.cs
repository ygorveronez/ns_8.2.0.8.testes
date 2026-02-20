using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet
{
    public class FiltroPesquisaAgendamentoPallet
    {
        public string NumeroCarga { get; set; }

        public string Senha { get; set; }

        public DateTime DataAgendamento { get; set; }

        public long CodigoDestinatario { get; set; }

        public int CodigoTransportador { get; set; }

        public long CodigoCliente { get; set; }

        public SituacaoCargaJanelaCarregamento? SituacaoCargaJanelaCarregamento { get; set; }
    }
}
