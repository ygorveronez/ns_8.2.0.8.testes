using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class FaixaTemperatura : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>
    {
        #region Construtores

        public FaixaTemperatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFaixaTemperatura filtrosPesquisa)
        {
            var consultaFaixaTemperatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaFaixaTemperatura = consultaFaixaTemperatura.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                consultaFaixaTemperatura = consultaFaixaTemperatura.Where(o => o.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.ProcedimentoEmbarque > 0)
                consultaFaixaTemperatura = consultaFaixaTemperatura.Where(o => o.ProcedimentoEmbarque.Codigo == filtrosPesquisa.ProcedimentoEmbarque);
            else if (filtrosPesquisa.TipoOperacao > 0)
                consultaFaixaTemperatura = consultaFaixaTemperatura.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.TipoOperacao);

            return consultaFaixaTemperatura;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> BuscarPorCodigos(List<int> codigos)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>()
                .Where(o => codigos.Contains(o.Codigo))
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> BuscarPorCodigosIntegracao(List<string> codigosIntegracao)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>()
                .Where(o => codigosIntegracao.Contains(o.CodigoIntegracao))
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura BuscarPorDescricao(string descricao, int filial, int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

            query = query.Where(obj => obj.Descricao == descricao);

            if (filial > 0)
                query = query.Where(obj => obj.ProcedimentoEmbarque == null || obj.ProcedimentoEmbarque.Filial.Codigo == filial);

            //if (tipoOperacao > 0)
            query = query.Where(obj => obj.ProcedimentoEmbarque == null || obj.ProcedimentoEmbarque.TipoOperacao.Codigo == tipoOperacao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura BuscarPorDescricaoEProcedimento(string descricao, int procedimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

            query = query.Where(obj => obj.Descricao == descricao && obj.ProcedimentoEmbarque.Codigo == procedimento);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> BuscarTodos()
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>().ToList();
        }

        //public Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura BuscarPorDescricaoeTipoOperacao(string Descricao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

        //    var result = from obj in query select obj;
        //    result = result.Where(ent => ent.Descricao.Contains(Descricao) && ent.TipoOperacao.Codigo == tipoOperacao.Codigo);

        //    return result.FirstOrDefault();
        //}

        public List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> BuscarPorTipoOperacao(int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.TipoOperacao.Codigo == tipoOperacao);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> BuscarPorProcedimento(int procedimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.ProcedimentoEmbarque.Codigo == procedimento);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFaixaTemperatura filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFaixaTemperatura = Consultar(filtrosPesquisa);

            if (parametrosConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                    consultaFaixaTemperatura = consultaFaixaTemperatura.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametrosConsulta.InicioRegistros > 0)
                    consultaFaixaTemperatura = consultaFaixaTemperatura.Skip(parametrosConsulta.InicioRegistros);

                if (parametrosConsulta.LimiteRegistros > 0)
                    consultaFaixaTemperatura = consultaFaixaTemperatura.Take(parametrosConsulta.LimiteRegistros);
            }

            return consultaFaixaTemperatura
                .Fetch(obj => obj.ProcedimentoEmbarque)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.ProcedimentoEmbarque)
                .ThenFetch(obj => obj.TipoOperacao)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFaixaTemperatura filtrosPesquisa)
        {
            var consultaFaixaTemperatura = Consultar(filtrosPesquisa);

            return consultaFaixaTemperatura.Count();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.FaixaTemperatura.FaixaTemperaturaMonitoramento> BuscarPorCodigosMonitoramentos(List<int> codigosMonitoramento)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarFaixaTemperaturaMonitoramento(codigosMonitoramento));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.FaixaTemperatura.FaixaTemperaturaMonitoramento)));

            return (List<Dominio.ObjetosDeValor.Embarcador.FaixaTemperatura.FaixaTemperaturaMonitoramento>)consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.FaixaTemperatura.FaixaTemperaturaMonitoramento>();
        }

        public string QueryConsultarFaixaTemperaturaMonitoramento(List<int> codigosMonitoramento)
        {
            string sql;

            sql = $@"select FaixaTemperatura.FTE_FAIXA_INICIAL TemperaturaFaixaInicial, FaixaTemperatura.FTE_FAIXA_FINAL TemperaturaFaixaFinal, T_MONITORAMENTO.mon_codigo CodigoMonitoramento  from T_MONITORAMENTO 
                    inner join T_CARGA 
                          on T_CARGA.CAR_CODIGO = T_MONITORAMENTO.CAR_CODIGO 
                    left join T_TIPO_DE_CARGA as TipoCarga 
                          on TipoCarga.TCG_CODIGO = T_CARGA.TCG_CODIGO
                    left join T_FAIXA_TEMPERATURA as FaixaTemperatura 
                          on TipoCarga.FTE_CODIGO = FaixaTemperatura.FTE_CODIGO where MON_CODIGO in ({string.Join(",", codigosMonitoramento)}) ";
            return sql;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> BuscarPorRemetente(double CPF_CNPJ)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>()
                .Where(obj => obj.Remetente.CPF_CNPJ == CPF_CNPJ);

            return query
                .ToList();
        }

        #endregion
    }
}
