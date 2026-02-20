using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios
{
    public abstract class RelatorioBase<TFiltroPesquisa, TDataSource>
        where TFiltroPesquisa : class
        where TDataSource : class
    {
        #region Atributos

        protected readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        protected readonly Repositorio.UnitOfWork _unitOfWork;
        protected readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;

        #endregion Atributos

        #region Construtores

        public RelatorioBase(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            _clienteMultisoftware = clienteMultisoftware;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Relatorios.ParametrosGeracaoRelatorio GerarRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio, TFiltroPesquisa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Relatorio servicoRelatorio = new Relatorio(_unitOfWork);

            try
            {
                int totalRegistros = ContarRegistros(filtrosPesquisa, propriedadesAgrupamento);
                int limiteRegistrosRelatorio = ObterLimiteRegistrosRelatorio();

                if ((limiteRegistrosRelatorio > 0) && (totalRegistros > limiteRegistrosRelatorio))
                {
                    Log.TratarErro($"Registros selecionados ({relatorioControleGeracao.Relatorio?.Codigo} - {relatorioControleGeracao.Relatorio?.Titulo}) excedem o limite de registros configurado ({totalRegistros}/{limiteRegistrosRelatorio}).");
                    servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, _unitOfWork, $"A quantidade de registros selecionada ({totalRegistros}) excedeu o limite máximo para a geração do relatório ({limiteRegistrosRelatorio}).");

                    return null;
                }

                if (totalRegistros == 0)
                {
                    Log.TratarErro($"Nenhum registro selecionado ({relatorioControleGeracao.Relatorio?.Codigo} - {relatorioControleGeracao.Relatorio?.Titulo}).");
                    servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, _unitOfWork, $"Não é possível gerar um relatório sem registros.");

                    return null;
                }

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = ObterParametros(filtrosPesquisa, parametrosConsulta);
                List<TDataSource> listaRegistros = ConsultarRegistros(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);

                Log.TratarErro($"Gerando relatório ({relatorioControleGeracao.Relatorio?.Codigo} - {relatorioControleGeracao.Relatorio?.Titulo}). {listaRegistros.Count} registros selecionados.");

                //CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio = servicoRelatorio.CriarRelatorio(relatorioControleGeracao, configuracaoRelatorio, listaRegistros, _unitOfWork, null, ObterSubReportDataSources(filtrosPesquisa), true, _tipoServicoMultisoftware, relatorioControleGeracao.Empresa?.CaminhoLogoDacte);

                //servicoRelatorio.PreecherParamentrosFiltro(relatorio, relatorioControleGeracao, configuracaoRelatorio, parametros);
                //servicoRelatorio.GerarRelatorio(relatorio, relatorioControleGeracao, ObterCaminhoPaginaRelatorio(), _unitOfWork);

                return PreencherParametrosGeracao(configuracaoRelatorio, listaRegistros.ToList<dynamic>(), filtrosPesquisa, parametrosConsulta, parametros);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, _unitOfWork, excecao);
                return null;
            }
            finally
            {
                if (!relatorioControleGeracao?.GerarPorServico ?? true)
                    _unitOfWork.Dispose();
            }

            //API Reports
        }

        private Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao ObterRelatorioControleGeracao(int codigoRelatorioControleGeracao)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repositorioRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = repositorioRelatorioControleGeracao.BuscarPorCodigo(codigoRelatorioControleGeracao);

            return relatorioControleGeracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Relatorios.ParametrosGeracaoRelatorio PreencherParametrosGeracao(Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio, List<dynamic> listaRegistros, TFiltroPesquisa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros)
        {
            Dominio.ObjetosDeValor.Embarcador.Relatorios.ParametrosGeracaoRelatorio parametrosGeracaoRelatorio = new Dominio.ObjetosDeValor.Embarcador.Relatorios.ParametrosGeracaoRelatorio()
            {
                ConfiguracaoRelatorio = configuracaoRelatorio,
                ListaRegistros = listaRegistros,
                FiltrosPesquisa = filtrosPesquisa,
                CaminhoRelatorio = ObterCaminhoPaginaRelatorio(),
                ParametrosConsulta = parametrosConsulta,
                Parametros = parametros,
                SubReportDataSources = ObterSubReportDataSources(filtrosPesquisa),
            };

            return parametrosGeracaoRelatorio;
        }

        #endregion Métodos Privados

        #region Métodos Protegidos

        protected void AtualizarPropriedadesOrdenacaoEAgrupamento(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            parametrosConsulta.PropriedadeAgrupar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeAgrupar);
            parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeOrdenar);
        }

        protected virtual int ObterLimiteRegistrosRelatorio()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            return repositorioConfiguracaoTMS.ObterLimiteRegistrosRelatorio();
        }

        protected virtual List<KeyValuePair<string, dynamic>> ObterSubReportDataSources(TFiltroPesquisa filtrosPesquisa)
        {
            return null;
        }

        #endregion Métodos Protegidos

        #region Métodos Protegidos Abstratos

        protected abstract List<TDataSource> ConsultarRegistros(TFiltroPesquisa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta);

        protected abstract int ContarRegistros(TFiltroPesquisa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento);

        protected abstract string ObterCaminhoPaginaRelatorio();

        protected abstract List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(TFiltroPesquisa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta);

        protected abstract string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar);

        #endregion Métodos Protegidos Abstratos

        #region Métodos Públicos

        //TODO: Ajustar para async
        public virtual void ExecutarPesquisa(out List<TDataSource> listaRegistros, out int totalRegistros, TFiltroPesquisa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            AtualizarPropriedadesOrdenacaoEAgrupamento(parametrosConsulta);

            totalRegistros = ContarRegistros(filtrosPesquisa, propriedadesAgrupamento);
            listaRegistros = (totalRegistros > 0) ? ConsultarRegistros(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta) : new List<TDataSource>();
        }

        public Dominio.ObjetosDeValor.Embarcador.Relatorios.ParametrosGeracaoRelatorio GerarRelatorioPorServico(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio;
            TFiltroPesquisa filtrosPesquisa;
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento;
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta;

            if (relatorioControleGeracao.AutomatizacaoGeracaoRelatorio != null)
            {
                configuracaoRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>(relatorioControleGeracao.AutomatizacaoGeracaoRelatorio.DadosConsulta.RelatorioTemporario);
                propriedadesAgrupamento = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>>(relatorioControleGeracao.AutomatizacaoGeracaoRelatorio.DadosConsulta.Propriedades);
                parametrosConsulta = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta>(relatorioControleGeracao.AutomatizacaoGeracaoRelatorio.DadosConsulta.ParametrosConsulta);
                filtrosPesquisa = Newtonsoft.Json.JsonConvert.DeserializeObject<TFiltroPesquisa>(relatorioControleGeracao.AutomatizacaoGeracaoRelatorio.DadosConsulta.FiltrosPesquisa);

                Servicos.Embarcador.Relatorios.Automatizacao svcAutomatizacao = new Automatizacao(_unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware);

                svcAutomatizacao.AtualizarFiltrosPesquisaAutomatizacao(filtrosPesquisa, relatorioControleGeracao.AutomatizacaoGeracaoRelatorio);
            }
            else
            {
                configuracaoRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>(relatorioControleGeracao.DadosConsulta.RelatorioTemporario);
                filtrosPesquisa = Newtonsoft.Json.JsonConvert.DeserializeObject<TFiltroPesquisa>(relatorioControleGeracao.DadosConsulta.FiltrosPesquisa);
                propriedadesAgrupamento = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>>(relatorioControleGeracao.DadosConsulta.Propriedades);
                parametrosConsulta = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta>(relatorioControleGeracao.DadosConsulta.ParametrosConsulta);
            }

            AtualizarPropriedadesOrdenacaoEAgrupamento(parametrosConsulta);
            return GerarRelatorio(relatorioControleGeracao, configuracaoRelatorio, filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }

        #endregion Métodos Públicos
    }
}