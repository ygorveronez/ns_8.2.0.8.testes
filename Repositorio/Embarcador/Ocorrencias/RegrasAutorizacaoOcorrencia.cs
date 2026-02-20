using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class RegrasAutorizacaoOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>
    {
        #region Construtores

        public RegrasAutorizacaoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> Consultar(int tipoOcorrencia, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia? etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (tipoOcorrencia > 0)
                result = result.Where(o => o.RegrasTipoOcorrencia.Any(ocorrencia => ocorrencia.TipoDeOcorrencia.Codigo == tipoOcorrencia));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (etapaAutorizacaoOcorrencia.HasValue)
                result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia.Value);

            return result;
        }

        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorComponenteFrete(int codigoComponenteFrete, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoOcorrencia.Ativo) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.ComponenteFrete.Codigo == codigoComponenteFrete ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.ComponenteFrete.Codigo != codigoComponenteFrete)
                         select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorComponenteFrete == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorExpedidor(double cnpjCpfExpedidor, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoOcorrencia.Ativo) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.Expedidor.CPF_CNPJ == cnpjCpfExpedidor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.Expedidor.CPF_CNPJ != cnpjCpfExpedidor)
                         select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorExpedidor == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorFilial(int codigoFilial, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoOcorrencia.Ativo) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.Filial.Codigo == codigoFilial ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.Filial.Codigo != codigoFilial)
                         select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorFilialEmissao == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorTomador(double cnpjCpfTomador, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoOcorrencia.Ativo) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.Tomador.CPF_CNPJ == cnpjCpfTomador ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.Tomador.CPF_CNPJ != cnpjCpfTomador)
                         select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorTomadorOcorrencia == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorValor(DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia>();

            var result = from obj in query where obj.RegrasAutorizacaoOcorrencia.Ativo select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorValorOcorrencia == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorTipoOcorrencia(int codigoTipoOcorrencia, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoOcorrencia.Ativo) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.TipoDeOcorrencia.Codigo != codigoTipoOcorrencia)
                         select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorTipoOcorrencia == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorTipoOperacao(int codigoTipoOperacao, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoOcorrencia.Ativo) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.TipoOperacao.Codigo == codigoTipoOperacao ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.TipoOperacao.Codigo != codigoTipoOperacao)
                         select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorTipoOperacao == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorTipoCarga(int codigoTipoCarga, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoOcorrencia.Ativo) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.TipoDeCarga.Codigo == codigoTipoCarga ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.TipoDeCarga.Codigo != codigoTipoCarga)
                         select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorTipoCarga == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorCanalEntrega(int codigoCanalEntrega, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega>();
            var result = from obj in query
                         where
                            (obj.RegrasAutorizacaoOcorrencia.Ativo) &&
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && obj.CanalEntrega.Codigo == codigoCanalEntrega ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && obj.CanalEntrega.Codigo != codigoCanalEntrega)
                         select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorCanalEntrega == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> BuscarRegraPorDiasAbertura(DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura>();

            var result = from obj in query where obj.RegrasAutorizacaoOcorrencia.Ativo select obj.RegrasAutorizacaoOcorrencia;

            result = result.Where(o => o.RegraPorDiasAbertura == true && (o.Vigencia >= data || o.Vigencia == null) && o.Ativo);
            result = result.Where(o => o.EtapaAutorizacaoOcorrencia == etapaAutorizacaoOcorrencia);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> ConsultarRegras(int tipoOcorrencia, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia? etapaAutorizacaoOcorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(tipoOcorrencia, dataInicio, dataFim, aprovador, descricao, ativo, etapaAutorizacaoOcorrencia);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrencia> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRegrasAutorizacaoOcorrencia = new ConsultaRegrasAutorizacaoOcorrencia().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaRegrasAutorizacaoOcorrencia.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrencia)));

            return consultaRegrasAutorizacaoOcorrencia.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrencia>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrenciaAlcada> ConsultarRelatorioAlcada(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa)
        {
            var consultaRegrasAutorizacaoOcorrenciaAlcada = this.SessionNHiBernate.CreateSQLQuery(new ConsultaRegrasAutorizacaoOcorrencia().ObterSqlPesquisaAlcada(filtrosPesquisa));

            consultaRegrasAutorizacaoOcorrenciaAlcada.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrenciaAlcada)));

            return consultaRegrasAutorizacaoOcorrenciaAlcada.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrenciaAlcada>();
        }

        public int ContarConsultaRegras(int codigoOcorrencia, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia? etapaAutorizacaoOcorrencia)
        {
            var result = Consultar(codigoOcorrencia, dataInicio, dataFim, aprovador, descricao, ativo, etapaAutorizacaoOcorrencia);

            return result.Count();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaRegrasAutorizacaoOcorrencia = new ConsultaRegrasAutorizacaoOcorrencia().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaRegrasAutorizacaoOcorrencia.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
