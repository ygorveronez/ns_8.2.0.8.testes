using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frota
{
    public class ModeloPneu : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ModeloPneu>
    {
        #region Construtores

        public ModeloPneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.ModeloPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaModeloPneu filtrosPesquisa)
        {
            var consultaModeloPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ModeloPneu>();

            if (filtrosPesquisa.CodigoDimensao > 0)
                consultaModeloPneu = consultaModeloPneu.Where(o => o.Dimensao.Codigo == filtrosPesquisa.CodigoDimensao);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaModeloPneu = consultaModeloPneu.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoMarca > 0)
                consultaModeloPneu = consultaModeloPneu.Where(o => o.Marca.Codigo == filtrosPesquisa.CodigoMarca);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaModeloPneu = consultaModeloPneu.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaModeloPneu = consultaModeloPneu.Where(o => o.Ativo == ativo);
            }

            return consultaModeloPneu;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.ModeloPneu BuscarPorCodigo(int codigo)
        {
            var modeloPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ModeloPneu>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return modeloPneu;
        }

        public List<Dominio.Entidades.Embarcador.Frota.ModeloPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaModeloPneu filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaModeloPneu = Consultar(filtrosPesquisa);

            return ObterLista(consultaModeloPneu, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaModeloPneu filtrosPesquisa)
        {
            var consultaModeloPneu = Consultar(filtrosPesquisa);

            return consultaModeloPneu.Count();
        }

        #endregion
    }
}
