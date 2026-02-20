using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class MotivoRetificacaoColeta : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta>
    {
        public MotivoRetificacaoColeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> BuscarMotivosRetificacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta>();
            var result = from obj in query where obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoTipoOperacao, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> query = ObterQueryConsulta(descricao, status, codigoTipoOperacao);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoTipoOperacao)
        {
            return ObterQueryConsulta(descricao, status, codigoTipoOperacao).Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> BuscarAtivos(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega aplicacaoColetaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta>();

            query = query.Where(obj => obj.TipoAplicacaoColetaEntrega == aplicacaoColetaEntrega);

            query = query.Where(o => o.Ativo);

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> ObterQueryConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (codigoTipoOperacao > 0)
                query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return query;
        }

        #endregion
    }
}
