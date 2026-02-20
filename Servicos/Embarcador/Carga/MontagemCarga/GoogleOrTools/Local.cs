using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    public class Local
    {
        public Local() { }
        public Local(long codigo, double latitude, double longitude)
        {
            this.Codigo = codigo;
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.VeiculosRestritos = new List<int>();
        }
        public long Codigo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double PesoTotal { get; set; }
        public bool Deposito { get; set; }
        public TimeWindow Janela { get; set; }
        public long CodigoAux { get; set; }
        public List<int> VeiculosRestritos { get; set; }
        public List<int> Pedidos { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem TipoPonto { get; set; }
        public List<Pedido> PedidosConfig { get; set; }
    }
}
