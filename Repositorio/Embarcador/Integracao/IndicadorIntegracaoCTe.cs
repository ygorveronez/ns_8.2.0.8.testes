using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Integracao
{
    public class IndicadorIntegracaoCTe : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>
    {
        #region Construtores
        public IndicadorIntegracaoCTe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public IndicadorIntegracaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe> Consultar(IQueryable<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe> consultaIndicadorIntegracaoCTe, Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa)
        {
            if (filtrosPesquisa.CodigoFilial > 0)
                consultaIndicadorIntegracaoCTe = consultaIndicadorIntegracaoCTe.Where(o => o.CargaCTe.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaIndicadorIntegracaoCTe = consultaIndicadorIntegracaoCTe.Where(o => o.CargaCTe.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataEmissaoInicio.HasValue)
                consultaIndicadorIntegracaoCTe = consultaIndicadorIntegracaoCTe.Where(o => o.CargaCTe.CTe.DataEmissao >= filtrosPesquisa.DataEmissaoInicio.Value.Date);

            if (filtrosPesquisa.DataEmissaoLimite.HasValue)
                consultaIndicadorIntegracaoCTe = consultaIndicadorIntegracaoCTe.Where(o => o.CargaCTe.CTe.DataEmissao <= filtrosPesquisa.DataEmissaoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaIndicadorIntegracaoCTe;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarCTeEmitido(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa)
        {
            var consultaIndicadorIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>();

            consultaIndicadorIntegracaoCTe = Consultar(consultaIndicadorIntegracaoCTe, filtrosPesquisa);

            return consultaIndicadorIntegracaoCTe.Select(o => o.CargaCTe).Distinct();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe> ConsultarPorIntegracaoAutomatica(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa)
        {
            var consultaIndicadorIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>()
                .Where(o => o.DataIntegracao.HasValue);

            consultaIndicadorIntegracaoCTe = Consultar(consultaIndicadorIntegracaoCTe, filtrosPesquisa);

            if (filtrosPesquisa.DataIntegracaoInicio.HasValue)
                consultaIndicadorIntegracaoCTe = consultaIndicadorIntegracaoCTe.Where(o => o.DataIntegracao >= filtrosPesquisa.DataIntegracaoInicio.Value.Date);

            if (filtrosPesquisa.DataIntegracaoLimite.HasValue)
                consultaIndicadorIntegracaoCTe = consultaIndicadorIntegracaoCTe.Where(o => o.DataIntegracao <= filtrosPesquisa.DataIntegracaoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaIndicadorIntegracaoCTe;
        }

        #endregion

        #region Métodos Públicos

        public void Atualizar(string xmlCTes, int codigoIntegradora)
        {
            try
            {
                var consultaVincularCteArquivoEDI = this.SessionNHiBernate.CreateSQLQuery("exec AtualizarIndicadorIntegracaoCTe @XML_CTES = :xmlCtes, @CODIGO_INTEGRADORA = :codigoIntegradora");

                consultaVincularCteArquivoEDI.SetParameter("xmlCtes", xmlCTes, NHibernate.NHibernateUtil.StringClob);
                consultaVincularCteArquivoEDI.SetInt32("codigoIntegradora", codigoIntegradora);
                consultaVincularCteArquivoEDI.SetTimeout(300).ExecuteUpdate();
            }
            catch (System.Exception excecao)
            {
                if (excecao.InnerException != null)
                    throw excecao.InnerException;

                throw;
            }
        }

        public Task<List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>> BuscarPorConsultaIntegracaoPendenteAsync(int limiteRegistros)
        {
            var consultaIndicadorIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>()
                .Where(o => o.Integradora.TipoIndicadorIntegracao == TipoIndicadorIntegracao.WebService && o.DataUltimaConsultaIntegracao == null);

            return consultaIndicadorIntegracaoCTe.Take(limiteRegistros).ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>> BuscarPorIntegracaoPendenteAsync(int limiteRegistros)
        {
            var consultaIndicadorIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>()
                .Where(o => o.Integradora.TipoIndicadorIntegracao == TipoIndicadorIntegracao.WebService && o.DataUltimaConsultaIntegracao != null && o.DataIntegracao == null);

            return consultaIndicadorIntegracaoCTe.OrderBy(o => o.DataUltimaConsultaIntegracao).Take(limiteRegistros).ToListAsync(CancellationToken);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIndicadorIntegracaoCTe = this.SessionNHiBernate.CreateSQLQuery(new ConsultaIndicadorIntegracaoCTe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta));

            consultaIndicadorIntegracaoCTe.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe)));

            return consultaIndicadorIntegracaoCTe.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Integracao.IndicadorIntegracaoCTe> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIndicadorIntegracaoCTe = new ConsultaRelatorioIndicadorIntegracaoCTe().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaIndicadorIntegracaoCTe.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Integracao.IndicadorIntegracaoCTe)));

            return consultaIndicadorIntegracaoCTe.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Integracao.IndicadorIntegracaoCTe>();
        }

        public dynamic ConsultaGraficoIntegracaoAutomatica(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa)
        {
            var consultaIndicadorIntegracaoCTe = ConsultarPorIntegracaoAutomatica(filtrosPesquisa);
            int totalRegistros = consultaIndicadorIntegracaoCTe.Count();

            if (totalRegistros == 0)
                return null;

            var consultaCTeEmitido = ConsultarCTeEmitido(filtrosPesquisa);
            int totalEmitidos = consultaCTeEmitido.Count();

            var dados = consultaIndicadorIntegracaoCTe
                .GroupBy(o => o.Integradora.Descricao)
                .Select(o => new
                {
                    label = o.Key,
                    value = o.Count()
                })
                .ToList();

            return new
            {
                Informacoes = new
                {
                    TotalEmitidos = totalEmitidos.ToString("n0")
                },
                Dados = dados
            };
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa)
        {
            var consultaIndicadorIntegracaoCTe = this.SessionNHiBernate.CreateSQLQuery(new ConsultaIndicadorIntegracaoCTe().ObterSqlContarPesquisa(filtrosPesquisa));

            return consultaIndicadorIntegracaoCTe.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaIndicadorIntegracaoCTe = new ConsultaRelatorioIndicadorIntegracaoCTe().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaIndicadorIntegracaoCTe.SetTimeout(600).UniqueResult<int>();
        }

        public bool VerificarRegistroExistente(int codigoCargaCTe, int codigoIntegradora)
        {
            var consultaIndicadorIntegracaoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IndicadorIntegracaoCTe>()
                .Where(o => o.CargaCTe.Codigo == codigoCargaCTe && o.Integradora.Codigo == codigoIntegradora);

            return consultaIndicadorIntegracaoCTe.Count() > 0;
        }

        #endregion
    }
}
