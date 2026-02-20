using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class MotivoRejeicaoColeta : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta>
    {
        public MotivoRejeicaoColeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta> query = ObterQueryConsulta(descricao, status);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            return ObterQueryConsulta(descricao, status).Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta> BuscarAtivos()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta>();

            query = query.Where(o => o.Ativo);

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta> ObterQueryConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query;
        }

        #endregion
    }
}
