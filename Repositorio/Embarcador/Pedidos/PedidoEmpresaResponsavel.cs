using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoEmpresaResponsavel : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel>
    {
        public PedidoEmpresaResponsavel(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel BuscarPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel>();
            var result = from obj in query where obj.CodigoIntegracao == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel> Consultar(string descricao, string codigoIntegracao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(descricao, codigoIntegracao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string codigoIntegracao)
        {
            var result = Consultar(descricao, codigoIntegracao);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel> Consultar(string descricao, string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(codigoIntegracao));

            return result;
        }
    }
}
