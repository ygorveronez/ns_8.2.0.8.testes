using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class GravidadeSinistro : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro>
    {
        #region Construtores

        public GravidadeSinistro(UnitOfWork unitWork) : base(unitWork) { }
        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro BuscarPrimeiro(string descricao, bool status)
        {
            var consultaGravidadeSinistro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro>()
                .Where(obj =>
                    obj.Descricao == descricao &&
                    obj.Status == status
                );

            return consultaGravidadeSinistro.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGravidadeSinistro filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaGravidadeSinistro = Consultar(filtrosPesquisa);

            return ObterLista(consultaGravidadeSinistro, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGravidadeSinistro filtrosPesquisa)
        {
            return Consultar(filtrosPesquisa).Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaGravidadeSinistro filtrosPesquisa)
        {
            var consultaGravidadeSinistro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaGravidadeSinistro = consultaGravidadeSinistro.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    consultaGravidadeSinistro = consultaGravidadeSinistro.Where(obj => obj.Status == true);

                else
                    consultaGravidadeSinistro = consultaGravidadeSinistro.Where(obj => obj.Status == false);
            }

            return consultaGravidadeSinistro;
        }

        #endregion
    }
}
