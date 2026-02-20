using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ParametroOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia>
    {
        #region Construtores

        public ParametroOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaParametroOcorrencia filtrosPesquisa)
        {
            var consultaParametroOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaParametroOcorrencia = consultaParametroOcorrencia.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.FiltrarParametrosPeriodo)
                consultaParametroOcorrencia = consultaParametroOcorrencia.Where(o => o.TipoParametro != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Periodo);

            if (filtrosPesquisa.Tipo.HasValue)
                consultaParametroOcorrencia = consultaParametroOcorrencia.Where(o => o.TipoParametro == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaParametroOcorrencia = consultaParametroOcorrencia.Where(o => o.Ativo == true);
            else if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaParametroOcorrencia = consultaParametroOcorrencia.Where(o => o.Ativo == false);

            return consultaParametroOcorrencia;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia BuscarPorCodigo(int codigo)
        {
            var consultaParametroOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia>()
                .Where(o => o.Codigo == codigo);

            return consultaParametroOcorrencia.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia tipo)
        {
            var consultaParametroOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia>()
                .Where(o => o.TipoParametro == tipo);

            return consultaParametroOcorrencia.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia> BuscarTodosAtivos()
        {
            var consultaParametroOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia>();

            consultaParametroOcorrencia = consultaParametroOcorrencia.Where(o => o.Ativo == true);

            return consultaParametroOcorrencia.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaParametroOcorrencia filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaParametroOcorrencia = Consultar(filtrosPesquisa);

            return ObterLista(consultaParametroOcorrencia, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaParametroOcorrencia filtrosPesquisa)
        {
            var consultaParametroOcorrencia = Consultar(filtrosPesquisa);

            return consultaParametroOcorrencia.Count();
        }

        #endregion
    }
}
