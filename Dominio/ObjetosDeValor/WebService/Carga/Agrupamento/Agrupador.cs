using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga.Agrupamento
{
    public class Agrupador
    {
        public string CodigoAgrupador { get; set; }

        public string TipoViagem { get; set; }

        public Embarcador.Frota.Veiculo Veiculo { get; set; }

        public Embarcador.Pessoas.Empresa Transportadora { get; set; }

        public List<Roteirizacao> Roteirizacao { get; set; }

        public List<PedidoAgrupamento> Pedidos { get; set; }

        public string DataPrevisaoInicioViagem { get; set; }
    }
}
