using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class MotivoReajuste : RepositorioBase<Dominio.Entidades.Embarcador.Frete.MotivoReajuste>
    {
        public MotivoReajuste(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.MotivoReajuste BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.MotivoReajuste>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public IQueryable<Dominio.Entidades.Embarcador.Frete.MotivoReajuste> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.MotivoReajuste>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Equals(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                result = result.Where(o => o.Ativo == ativo);

            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frete.MotivoReajuste> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var result = _Consultar(descricao, ativo);

            if (inicio > 0)
                result = result.Skip(inicio);

            if (limite > 0)
                result = result.Take(limite);

            result = result.OrderBy(propOrdenar + " " + dirOrdenar);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var result = _Consultar(descricao, ativo);

            return result.Count();
        }
    }
}
