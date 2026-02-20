using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.PagamentoAgregado
{
    public class PagamentoAgregadoInfracaoParcela : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela>
    {
        public PagamentoAgregadoInfracaoParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemInfracaoPagamento(int codigoPagamento, int codigoInfracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigoPagamento && obj.InfracaoParcela.Codigo == codigoInfracao select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela> BuscarPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigosPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return result.Select(obj => obj.Codigo).ToList();
        }
    }
}
