using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class JanelaDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento>
    {
        #region Construtores

        public JanelaDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaDescarregamento filtrosPesquisa)
        {
            var consultaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento>();

            if (filtrosPesquisa.CodigoCentroDescarregamento > 0)
                consultaJanelaDescarregamento = consultaJanelaDescarregamento.Where(o => o.Escala.CentroDescarregamento.Codigo == filtrosPesquisa.CodigoCentroDescarregamento);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaJanelaDescarregamento = consultaJanelaDescarregamento.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaJanelaDescarregamento = consultaJanelaDescarregamento.Where(o => o.PrevisaoChegada.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaJanelaDescarregamento = consultaJanelaDescarregamento.Where(o => o.PrevisaoChegada.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaJanelaDescarregamento;
        }

        #endregion

        #region Métodos Públicos
        
        public Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.JanelaDescarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaDescarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaJanelaDescarregamento = Consultar(filtrosPesquisa);

            return ObterLista(consultaJanelaDescarregamento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaDescarregamento filtrosPesquisa)
        {
            var consultaJanelaDescarregamento = Consultar(filtrosPesquisa);

            return consultaJanelaDescarregamento.Count();
        }

        #endregion
    }
}
