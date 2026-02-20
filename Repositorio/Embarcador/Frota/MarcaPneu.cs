using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frota
{
    public class MarcaPneu : RepositorioBase<Dominio.Entidades.Embarcador.Frota.MarcaPneu>
    {
        #region Construtores

        public MarcaPneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.MarcaPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMarcaPneu filtrosPesquisa)
        {
            var consultaMarcaPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.MarcaPneu>();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaMarcaPneu = consultaMarcaPneu.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaMarcaPneu = consultaMarcaPneu.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaMarcaPneu = consultaMarcaPneu.Where(o => o.Ativo == ativo);
            }

            return consultaMarcaPneu;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.MarcaPneu BuscarPorCodigo(int codigo)
        {
            var marcaPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.MarcaPneu>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return marcaPneu;
        }

        public List<Dominio.Entidades.Embarcador.Frota.MarcaPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMarcaPneu filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMarcaPneu = Consultar(filtrosPesquisa);

            return ObterLista(consultaMarcaPneu, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMarcaPneu filtrosPesquisa)
        {
            var consultaMarcaPneu = Consultar(filtrosPesquisa);

            return consultaMarcaPneu.Count();
        }

        #endregion
    }
}
