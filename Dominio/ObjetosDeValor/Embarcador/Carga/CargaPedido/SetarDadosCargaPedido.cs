using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Pedidos;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class SetarDadosCargaPedido
    {
        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }
        public Dominio.Entidades.Cliente Tomador { get; set; }
        public TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS Configuracao { get; set; }
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ConfiguracaoGeralCarga { get; set; }
        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> ClientesDescarga { get; set; }
        public bool? PossuiRegraTomador { get; set; }
        public bool? UtilizarDistribuidorPorRegiaoNaRegiaoDestino { get; set; }
        public List<Dominio.Entidades.Embarcador.Filiais.Filial> Filiais { get; set; }
        public List<RegraTomador> RegrasTomadores { get; set; }
        public List<RegraTomador> RegrasTomadoresSemTomador { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.DadosCrtMicPedido> DadosCrtMicPedido { get; set; }
    }
}
