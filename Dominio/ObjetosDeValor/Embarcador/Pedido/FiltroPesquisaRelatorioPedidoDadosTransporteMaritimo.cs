using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo
    {
        #region Propriedades

        public string NumeroCargaEmbarcador { get; set; }

        public int Origem { get; set; }

        public int Destino { get; set; }

        public int Filial { get; set; }

        public string NumeroEXP { get; set; }

        public string NumeroBooking { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFim { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo? Status { get; set; }

        #endregion Propriedades
    }
}
