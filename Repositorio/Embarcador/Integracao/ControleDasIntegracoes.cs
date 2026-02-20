
using System.Linq;
using System;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Integracao
{
    public class ControleDasIntegracoes : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes>
    {
        #region Construtores
        public ControleDasIntegracoes(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos publicos

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltrosPesquisaControleDasIntegracoes filtrosPesquisa)
        {
            return Consulta(filtrosPesquisa).Count();
        }
        public List<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes> ObterResumoIntegracoes(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltrosPesquisaControleDasIntegracoes filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return ObterLista(Consulta(filtrosPesquisa), parametrosConsulta);
        }
        public Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes BuscarPorCodigo(long codigo)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes>();
            consulta = consulta.Where(c => c.Codigo == codigo);
            return consulta.FirstOrDefault();
        }

        public List<long> BuscarCodigoIntegracaoesVencidos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes>();
            query = from obj in query where obj.DataRequisicao <= DateTime.Now.AddDays(-2) && !obj.Expiou select obj;
            return query.Select(i => i.Codigo).ToList();

        }
        #endregion

        #region Metodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes> Consulta(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltrosPesquisaControleDasIntegracoes filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes>();
            var consultaWebService = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.MetodosRest>();

            consulta = consulta.Where(ce => ce.Situacao == filtrosPesquisa.Sitaucao);

            if (filtrosPesquisa.CodigoIntegradora > 0)
                consulta = consulta.Where(ce => ce.Integradora.Codigo == filtrosPesquisa.CodigoIntegradora);

            if (filtrosPesquisa.CodigoMetodo > 0)
            {
                var existeMetodo = consultaWebService.Where(c => c.Codigo == filtrosPesquisa.CodigoMetodo).FirstOrDefault();
                if (existeMetodo != null)
                {
                    consulta = consulta.Where(ce => ce.NomeMetodo == existeMetodo.NomeMetodo);
                }
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                consulta = consulta.Where(ce => ce.DataRequisicao <= filtrosPesquisa.DataFinal);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                consulta = consulta.Where(ce => ce.DataRequisicao >= filtrosPesquisa.DataInicial);


            return consulta;
        }
        #endregion
    }
}
