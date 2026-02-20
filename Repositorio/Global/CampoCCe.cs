using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.Enumeradores;

namespace Repositorio
{
    public class CampoCCe : RepositorioBase<Dominio.Entidades.CampoCCe>, Dominio.Interfaces.Repositorios.CampoCCe
    {
        public CampoCCe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CampoCCe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.CampoCCe BuscarPorTipoCampoCCeAutomatico(TipoCampoCCeAutomatico tipoCampo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();
            var result = from obj in query where obj.TipoCampoCCeAutomatico == tipoCampo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.CampoCCe BuscarPorNomeCampoGrupo(string nomeCampo, string numeroGrupo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();
            var result = from obj in query where obj.NomeCampo == nomeCampo && obj.GrupoCampo == numeroGrupo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.CampoCCe BuscarPorNomeCampo(string nomeCampo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();
            var result = from obj in query where obj.NomeCampo == nomeCampo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.CampoCCe> Consultar(string descricao, string nomeCampo, string grupoCampo, string status, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(nomeCampo))
                result = result.Where(o => o.NomeCampo.Contains(nomeCampo));

            if (!string.IsNullOrWhiteSpace(grupoCampo))
                result = result.Where(o => o.GrupoCampo.Contains(grupoCampo));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string nomeCampo, string grupoCampo, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(nomeCampo))
                result = result.Where(o => o.NomeCampo.Contains(nomeCampo));

            if (!string.IsNullOrWhiteSpace(grupoCampo))
                result = result.Where(o => o.GrupoCampo.Contains(grupoCampo));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.Entidades.CampoCCe> ConsultarParaEmissao(string descricao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();

            var result = from obj in query where obj.Status.Equals("A") select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.OrderBy(o => o.Descricao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaParaEmissao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();

            var result = from obj in query where obj.Status.Equals("A") select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.Count();
        }

        #region Consulta 

        public List<Dominio.Entidades.CampoCCe> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.CampoCCe> query = ObterConsulta(descricao, status);

            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.CampoCCe> query = ObterConsulta(descricao, status);

            return query.Count();
        }

        public IQueryable<Dominio.Entidades.CampoCCe> ObterConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.CampoCCe> query = this.SessionNHiBernate.Query<Dominio.Entidades.CampoCCe>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Status == "A");
            else if(status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => o.Status == "I");

            return query;
        }

        #endregion
    }
}
