using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class TipoSinistro : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro>
    {
        #region Construtores

        public TipoSinistro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro BuscarPrimeiro(string descricao, bool status)
        {
            var consultaTipoSinistro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro>()
                .Where(obj =>
                    obj.Descricao == descricao &&
                    obj.Status == status
                );

            return consultaTipoSinistro.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoSinistro filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTipoSinistro = Consultar(filtrosPesquisa);

            return ObterLista(consultaTipoSinistro, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoSinistro filtrosPesquisa)
        {
            return Consultar(filtrosPesquisa).Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoSinistro filtrosPesquisa)
        {
            var consultaTipoSinistro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaTipoSinistro = consultaTipoSinistro.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaTipoSinistro = consultaTipoSinistro.Where(obj => obj.Status == true);

                else
                    consultaTipoSinistro = consultaTipoSinistro.Where(obj => obj.Status == false);
            }

            return consultaTipoSinistro;
        }

        #endregion
    }
}
