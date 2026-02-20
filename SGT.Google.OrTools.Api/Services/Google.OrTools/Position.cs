using System.Collections.Generic;
using System.Linq;

namespace Google.OrTools.Api.Services.GoogleOrTools
{
    /// <summary>
    /// Uma posição no mapa com coordenadas (x, y).
    /// </summary>
    public class Position
    {
        public Position()
        {
            this.PedidosConfig = new List<Pedido>();
        }

        public Position(int codigo, double latitude, double longitude)
        {
            this.Codigo = codigo;
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.PedidosConfig = new List<Pedido>();
        }

        public override string ToString()
        {
            return (this.Deposito ? "Deposito - " : this.Codigo.ToString()) + " - " + this.TipoPonto.ToString() + " - " + this.PesoTotal.ToString() + " [ " + this.Latitude.ToString() + " " + this.Longitude.ToString() + "]" +
                (this.Janela != null ? " [ " + this.Janela.start.ToString() + " - " + this.Janela.end.ToString() : "") + "]" + ((this.VeiculosRestritos?.Count ?? 0) > 0 ? " Restrição veiculos:" + string.Join(",", this.VeiculosRestritos) : "");
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
        public Models.EnumTipoPonto TipoPonto { get; set; }
        public List<Pedido> PedidosConfig { get; set; }
        /// <summary>
        /// Utilizado para "Priorizar" determinados pedidos...
        /// </summary>
        public int Prioridade
        {
            get
            {
                int prioridade = 0;
                if ((PedidosConfig?.Count ?? 0) > 0)
                    prioridade = PedidosConfig.Min(x => x.Prioridade);
                return (prioridade > 0 ? prioridade : 99);
            }
        }
    }
}