using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FreteCargaPedido
    {
        public FreteCargaPedido()
        {
            ID = Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete { get; set; }
        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente { get; set; }
        public bool ultimoPedido { get; set; }
    }
}
