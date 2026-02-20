using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoAgregado
{
    public class PagamentoAgregadoAdiantamento : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento>
    {
        public PagamentoAgregadoAdiantamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento> BuscarPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigosPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return result.Select(obj => obj.Codigo).ToList();
        }
    }
}
