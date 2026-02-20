using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Pessoas
{
    public class Cargo : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.Cargo>
    {
        #region Construtores

        public Cargo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region MétodosPrivados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.Cargo> Consultar(string descricao, SituacaoAtivoPesquisa? ativo)
        {
            var consultaCargo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.Cargo>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaCargo = consultaCargo.Where(o => o.Descricao.Contains(descricao));
            
            if (ativo.HasValue)
            {
                if (ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    consultaCargo = consultaCargo.Where(obj => obj.Ativo);
                else if (ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    consultaCargo = consultaCargo.Where(obj => !obj.Ativo);
            }
            return consultaCargo;
        }

        #endregion

        #region Métodos Públicos


        public Dominio.Entidades.Embarcador.Pessoas.Cargo BuscarPorCodigo(int codigo)
        {
            var consultaCargo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.Cargo>()
                .Where(o => o.Codigo == codigo);         

            return consultaCargo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.Cargo> Consultar(string descricao, SituacaoAtivoPesquisa? ativo,  Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargo = Consultar(descricao, ativo);

            return ObterLista(consultaCargo, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa? ativo)
        {
            var consultaCargo = Consultar(descricao, ativo);

            return consultaCargo.Count();
        }

        public Dominio.Entidades.Embarcador.Pessoas.Cargo BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.Cargo>()
                   .Where(x => x.Descricao.ToLower().Equals(descricao));

            return query.FirstOrDefault();
        }

        #endregion
    }
}
