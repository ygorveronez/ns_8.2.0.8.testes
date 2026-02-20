using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.Cancelamento
{
    public class JustificativaCancelamentoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga>
    {
        public JustificativaCancelamentoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.Count();
        }



    }
}
