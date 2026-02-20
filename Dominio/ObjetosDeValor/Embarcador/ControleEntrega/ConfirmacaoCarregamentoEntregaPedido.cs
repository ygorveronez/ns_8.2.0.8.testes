using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.ControleEntrega
{
    public class ConfirmacaoCarregamentoEntregaPedido
    {
        public string IdCliente { get; set; }
        public string IdTransportadora { get; set; }
        public int TempoJanela { get; set; }
        public string IdLocal { get; set; }
        public DateTime DataAgenda { get; set; }
        public string PaletizacaoEspecial { get; set; }
        public string CargaRefrigerada { get; set; }
        public string Observacao { get; set; }
        public List<ListaMs> ListaMs { get; set; }

    }
}
