using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoIntegracaoPacotes : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes>
    {
        public CargaPedidoIntegracaoPacotes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes> BuscarPorCargaPedidoIntegracaoPacotes(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao /*|| (obj.NumeroTentativas <= 3 &&  obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)*/ select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes> BuscarPorCargaPedidoIntegracaoPacotesPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes>();
            var result = from obj in query where obj.CargaPedido.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes>();
            var result = from obj in query where obj.CargaPedido.Codigo == codigoCargaPedido select obj;
            return result.ToList();
        }
    }
}
