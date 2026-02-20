using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class FiltroPesquisaPedidoDadosTransporteMaritimo
    {
        #region Propriedades

        public string  NumeroCargaEmbarcador { get; set; }

        public int Origem { get; set; }

        public int Destino { get; set; }

        public int Filial { get; set; }

        public string numeroEXP { get; set; }

        public string numeroBooking { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFim { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo? status { get; set; }

        #endregion Propriedades

    }
}
