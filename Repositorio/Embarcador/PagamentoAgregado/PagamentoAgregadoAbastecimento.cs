using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.PagamentoAgregado
{
    public class PagamentoAgregadoAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento>
    {
        public PagamentoAgregadoAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento> BuscarPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return result.ToList();
        }

        public bool ContemAbastecimentoPagamento(int codigoPagamento, int codigoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigoPagamento && obj.Abastecimento.Codigo == codigoAbastecimento select obj;
            return result.Any();
        }

        public List<int> BuscarCodigosPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento>();
            var result = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return result.Select(obj => obj.Codigo).ToList();
        }
    }
}

