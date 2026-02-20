using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoAgregado
{
    public class PagamentoAgregadoAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto>
    {
        public PagamentoAgregadoAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> BuscarPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigoPagamento select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> BuscarPorPagamentoETipo(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa tipoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigoPagamento && obj.Justificativa.TipoJustificativa == tipoJustificativa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto> BuscarPorPagamento(int codigoPagamento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigoPagamento select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigoPagamento select obj;
            return result.Count();
        }
    }
}
