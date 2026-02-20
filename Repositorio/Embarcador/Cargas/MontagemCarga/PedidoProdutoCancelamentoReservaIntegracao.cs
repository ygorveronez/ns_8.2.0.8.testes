using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class PedidoProdutoCancelamentoReservaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao>
    {
        #region Construtores

        public PedidoProdutoCancelamentoReservaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao> BuscarPorPedidoIntegracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.PedidoProdutoCancelamentoReservaIntegracao>();

            var result = from obj in query where obj.PedidoCancelamentoReservaIntegracao.Codigo == codigo select obj;

            return result.ToList();
        }
    }
}
