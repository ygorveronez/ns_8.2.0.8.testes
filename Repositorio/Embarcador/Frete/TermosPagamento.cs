using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class TermosPagamento : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TermosPagamento>
    {
        public TermosPagamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.TermosPagamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TermosPagamento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TermosPagamento BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TermosPagamento>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TermosPagamento> Consultar(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(codigoIntegracao, descricao);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(string codigoIntegracao, string descricao)
        {
            var result = Consultar(codigoIntegracao, descricao);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TermosPagamento> Consultar(string codigoIntegracao, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TermosPagamento>();
            query = from obj in query select obj;

            if (!string.IsNullOrEmpty(codigoIntegracao))
                query = query.Where(obj => obj.CodigoIntegracao.Equals(codigoIntegracao));

            if (!string.IsNullOrEmpty(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            return query;
        }
    }
}
