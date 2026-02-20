using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frota
{
    public class DimensaoPneu : RepositorioBase<Dominio.Entidades.Embarcador.Frota.DimensaoPneu>
    {
        #region Construtores

        public DimensaoPneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.DimensaoPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDimensaoPneu filtrosPesquisa)
        {
            var consultaDimensaoPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.DimensaoPneu>();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaDimensaoPneu = consultaDimensaoPneu.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Aplicacao))
                consultaDimensaoPneu = consultaDimensaoPneu.Where(o => o.Aplicacao.Contains(filtrosPesquisa.Aplicacao));

            if (filtrosPesquisa.SituacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaDimensaoPneu = consultaDimensaoPneu.Where(o => o.Ativo == ativo);
            }

            return consultaDimensaoPneu;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.DimensaoPneu BuscarPorCodigo(int codigo)
        {
            var dimensaoPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.DimensaoPneu>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return dimensaoPneu;
        }

        public List<Dominio.Entidades.Embarcador.Frota.DimensaoPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDimensaoPneu filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDimensaoPneu = Consultar(filtrosPesquisa);

            return ObterLista(consultaDimensaoPneu, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDimensaoPneu filtrosPesquisa)
        {
            var consultaDimensaoPneu = Consultar(filtrosPesquisa);

            return consultaDimensaoPneu.Count();
        }

        #endregion
    }
}
