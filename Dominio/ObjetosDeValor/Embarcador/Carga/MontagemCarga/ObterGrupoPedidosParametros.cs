using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class ObterGrupoPedidosParametros
    {
        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> CentrosCarregamento { get; set; }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> DisponibilidadeDia { get; set; }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> GruposPedidos { get; set; }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> SessaoRoteirizadorPedidosSituacao { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador SessaoRoteirizador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SessaoRoteirizadorParametros SessaoRoteirizadorParametros { get; set; }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> DisponibilidadeDiaUtilizar { get; set; }

        public List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> LinhasSeparacaoAgrupa { get; set; }
    }
}
