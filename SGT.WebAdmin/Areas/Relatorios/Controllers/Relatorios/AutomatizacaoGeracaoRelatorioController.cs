using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Relatorios
{
    [Area("Relatorios")]
    public abstract class AutomatizacaoGeracaoRelatorioController<TFiltroPesquisa> : BaseController
        where TFiltroPesquisa : class
    {
        #region Construtores

        public AutomatizacaoGeracaoRelatorioController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarAutomatizacaoGeracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio repositorioConfiguracaoRelatorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = repositorioConfiguracaoRelatorio.BuscarConfiguracaoPadrao();

                if (!(configuracaoRelatorio?.ServicoGeracaoRelatorioHabilitado ?? false) || !(configuracaoRelatorio?.UtilizaAutomacaoEnvioRelatorio ?? false))
                    throw new ControllerException("Este recurso não está disponível.");

                Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio repositorioAutomatizacaoGeracao = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio(unitOfWork);
                int quantidadeAutomatizacaoCadastrada = repositorioAutomatizacaoGeracao.ContarTodos();

                if (quantidadeAutomatizacaoCadastrada >= configuracaoRelatorio.QuantidadeAutomacaoEnvioRelatorio)
                    throw new ControllerException($"Quantidade de automatizações disponível ({configuracaoRelatorio.QuantidadeAutomacaoEnvioRelatorio}) excedida. Não é possível registrar novas automatizações.");

                Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio = new Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio();

                await unitOfWork.StartAsync(cancellationToken);

                PreencherAutomatizacaoGeracao(automatizacaoGeracaoRelatorio, unitOfWork);
                await PreencherAutomatizacaoGeracaoDadosConsulta(automatizacaoGeracaoRelatorio, unitOfWork, cancellationToken);

                repositorioAutomatizacaoGeracao.Inserir(automatizacaoGeracaoRelatorio, Auditado);

                PreencherAutomatizacaoGeracaoConfiguracaoFTP(automatizacaoGeracaoRelatorio, historicoAlteracaoAutomatizacao: null, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a automatização de geração do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AtualizarAutomatizacaoGeracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic automatizacaoDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Automatizacao"));
                int codigo = ((string)automatizacaoDinamica.Codigo).ToInt();
                Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio repositorioAutomatizacaoGeracao = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio repConfiguracaoRelatorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = repConfiguracaoRelatorio.BuscarConfiguracaoPadrao();

                if (!(configuracaoRelatorio?.ServicoGeracaoRelatorioHabilitado ?? false) ||
                    !(configuracaoRelatorio?.UtilizaAutomacaoEnvioRelatorio ?? false))
                    throw new ControllerException("Este recurso não está disponível.");

                Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio = repositorioAutomatizacaoGeracao.BuscarPorCodigo(codigo, auditavel: true);

                if (automatizacaoGeracaoRelatorio == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                await unitOfWork.StartAsync(cancellationToken);

                PreencherAutomatizacaoGeracao(automatizacaoGeracaoRelatorio, unitOfWork);

                Dominio.Entidades.Auditoria.HistoricoObjeto historicoAlteracaoAutomatizacao = repositorioAutomatizacaoGeracao.Atualizar(automatizacaoGeracaoRelatorio, Auditado);

                PreencherAutomatizacaoGeracaoConfiguracaoFTP(automatizacaoGeracaoRelatorio, historicoAlteracaoAutomatizacao, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a automatização de geração do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirAutomatizacaoGeracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio repositorioAutomatizacaoGeracao = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio(unitOfWork);
                Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP repositorioConfiguracaoFTP = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP(unitOfWork);
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repositorioRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio = repositorioAutomatizacaoGeracao.BuscarPorCodigo(codigo, auditavel: true);

                if (automatizacaoGeracaoRelatorio == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                await unitOfWork.StartAsync(cancellationToken);

                repositorioRelatorioControleGeracao.DeletarPorAutomatizacao(automatizacaoGeracaoRelatorio.Codigo);
                repositorioConfiguracaoFTP.DeletarPorAutomatizacao(automatizacaoGeracaoRelatorio.Codigo);
                repositorioAutomatizacaoGeracao.Deletar(automatizacaoGeracaoRelatorio, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir a automatização de geração do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAutomatizacaoGeracaoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio repositorioAutomatizacaoGeracao = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio(unitOfWork);
                Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP repositorioConfiguracaoFTP = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio = repositorioAutomatizacaoGeracao.BuscarPorCodigo(codigo, auditavel: false);
                Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP configuracaoFTP = repositorioConfiguracaoFTP.BuscarPorAutomatizacao(automatizacaoGeracaoRelatorio.Codigo);

                return new JsonpResult(new
                {
                    automatizacaoGeracaoRelatorio.Codigo,
                    automatizacaoGeracaoRelatorio.Descricao,
                    automatizacaoGeracaoRelatorio.DiaGeracao,
                    automatizacaoGeracaoRelatorio.Email,
                    automatizacaoGeracaoRelatorio.EnviarParaFTP,
                    automatizacaoGeracaoRelatorio.EnviarPorEmail,
                    automatizacaoGeracaoRelatorio.OcorrenciaGeracao,
                    automatizacaoGeracaoRelatorio.TipoArquivo,
                    automatizacaoGeracaoRelatorio.GerarSegunda,
                    automatizacaoGeracaoRelatorio.GerarTerca,
                    automatizacaoGeracaoRelatorio.GerarQuarta,
                    automatizacaoGeracaoRelatorio.GerarQuinta,
                    automatizacaoGeracaoRelatorio.GerarSexta,
                    automatizacaoGeracaoRelatorio.GerarSabado,
                    automatizacaoGeracaoRelatorio.GerarDomingo,
                    automatizacaoGeracaoRelatorio.GerarSomenteEmDiaUtil,
                    HoraGeracao = automatizacaoGeracaoRelatorio.HoraGeracao.ToTimeString(),
                    TipoRelatorio = new { Codigo = automatizacaoGeracaoRelatorio.Relatorio.Codigo, Descricao = automatizacaoGeracaoRelatorio.Relatorio.Descricao },
                    Diretorio = configuracaoFTP?.Diretorio ?? "",
                    EnderecoFTP = configuracaoFTP?.EnderecoFTP ?? "",
                    Nomenclatura = configuracaoFTP?.Nomenclatura ?? "",
                    Passivo = configuracaoFTP?.Passivo ?? false,
                    Porta = configuracaoFTP?.Porta ?? "",
                    Senha = configuracaoFTP?.Senha ?? "",
                    SSL = configuracaoFTP?.SSL ?? false,
                    Usuario = configuracaoFTP?.Usuario ?? "",
                    UtilizarSFTP = configuracaoFTP?.UtilizarSFTP ?? false,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a automatização de geração do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAutomatizacaoGeracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo do Relatório", "TipoRelatorio", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo do Arquivo", "TipoArquivo", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Hora", "HoraGeracao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Ocorrência", "OcorrenciaGeracao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Próxima Geração", "ProximaGeracao", 12, Models.Grid.Align.center, false);

                Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorios = Request.GetEnumParam<Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios>("CodigoControleRelatorios");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio repositorioAutomatizacaoGeracao = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio(unitOfWork);
                int totalRegistros = repositorioAutomatizacaoGeracao.ContarConsulta(codigoControleRelatorios);
                List<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio> listaAutomatizacao = (totalRegistros > 0) ? repositorioAutomatizacaoGeracao.Consultar(codigoControleRelatorios, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio>();

                var listaAutomatizacaoRetornar = (
                    from automatizacao in listaAutomatizacao
                    select new
                    {
                        automatizacao.Codigo,
                        automatizacao.Descricao,
                        TipoRelatorio = automatizacao.Relatorio.Descricao,
                        TipoArquivo = automatizacao.TipoArquivo.ObterDescricao(),
                        HoraGeracao = automatizacao.HoraGeracao.ToTimeString(),
                        OcorrenciaGeracao = automatizacao.OcorrenciaGeracao.ObterDescricao(),
                        ProximaGeracao = automatizacao.DataProximaGeracao.ToString("dd/MM/yy HH:mm")
                    }
                ).ToList();

                grid.AdicionaRows(listaAutomatizacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as automatizações de geração do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion Globais

        #region Métodos Protegidos Abstratos

        protected abstract TFiltroPesquisa ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork);

        protected abstract Task<TFiltroPesquisa> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken);

        #endregion Métodos Protegidos Abstratos

        #region Métodos Privados

        private void PreencherAutomatizacaoGeracao(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Relatorios.Automatizacao servicoAutomatizacao = new Servicos.Embarcador.Relatorios.Automatizacao(unitOfWork, TipoServicoMultisoftware, Cliente);
            dynamic automatizacaoDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Automatizacao"));

            automatizacaoGeracaoRelatorio.Descricao = (string)automatizacaoDinamica.Descricao;
            automatizacaoGeracaoRelatorio.DiaGeracao = ((string)automatizacaoDinamica.DiaGeracao).ToInt();
            automatizacaoGeracaoRelatorio.Email = (string)automatizacaoDinamica.Email;
            automatizacaoGeracaoRelatorio.GerarSegunda = ((string)automatizacaoDinamica.GerarSegunda).ToBool();
            automatizacaoGeracaoRelatorio.GerarTerca = ((string)automatizacaoDinamica.GerarTerca).ToBool();
            automatizacaoGeracaoRelatorio.GerarQuarta = ((string)automatizacaoDinamica.GerarQuarta).ToBool();
            automatizacaoGeracaoRelatorio.GerarQuinta = ((string)automatizacaoDinamica.GerarQuinta).ToBool();
            automatizacaoGeracaoRelatorio.GerarSexta = ((string)automatizacaoDinamica.GerarSexta).ToBool();
            automatizacaoGeracaoRelatorio.GerarSabado = ((string)automatizacaoDinamica.GerarSabado).ToBool();
            automatizacaoGeracaoRelatorio.GerarDomingo = ((string)automatizacaoDinamica.GerarDomingo).ToBool();
            automatizacaoGeracaoRelatorio.GerarSomenteEmDiaUtil = ((string)automatizacaoDinamica.GerarSomenteEmDiaUtil).ToBool();
            automatizacaoGeracaoRelatorio.HoraGeracao = ((string)automatizacaoDinamica.HoraGeracao).ToTime();
            automatizacaoGeracaoRelatorio.OcorrenciaGeracao = ((string)automatizacaoDinamica.OcorrenciaGeracao).ToEnum<OcorrenciaGeracaoRelatorio>();
            automatizacaoGeracaoRelatorio.TipoArquivo = ((string)automatizacaoDinamica.TipoArquivo).ToEnum<TipoArquivoGeracaoRelatorio>();
            automatizacaoGeracaoRelatorio.Usuario = Usuario;
            automatizacaoGeracaoRelatorio.EnviarParaFTP = ((string)automatizacaoDinamica.EnviarParaFTP).ToBool();
            automatizacaoGeracaoRelatorio.EnviarPorEmail = ((string)automatizacaoDinamica.EnviarPorEmail).ToBool();
            automatizacaoGeracaoRelatorio.DataProximaGeracao = servicoAutomatizacao.ObterDataProximaGeracao(automatizacaoGeracaoRelatorio);

            if (automatizacaoGeracaoRelatorio.DataBase == DateTime.MinValue)
                automatizacaoGeracaoRelatorio.DataBase = automatizacaoGeracaoRelatorio.DataProximaGeracao;
        }

        private void PreencherAutomatizacaoGeracaoConfiguracaoFTP(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, Dominio.Entidades.Auditoria.HistoricoObjeto historicoAlteracaoAutomatizacao, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic automatizacaoDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Automatizacao"));
            Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP repositorioConfiguracaoFTP = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP(unitOfWork);
            Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP configuracaoFTP = repositorioConfiguracaoFTP.BuscarPorAutomatizacao(automatizacaoGeracaoRelatorio.Codigo);

            if (configuracaoFTP == null)
                configuracaoFTP = new Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioConfiguracaoFTP()
                {
                    AutomatizacaoGeracaoRelatorio = automatizacaoGeracaoRelatorio
                };

            configuracaoFTP.Diretorio = (string)automatizacaoDinamica.Diretorio;
            configuracaoFTP.EnderecoFTP = (string)automatizacaoDinamica.EnderecoFTP;
            configuracaoFTP.Nomenclatura = (string)automatizacaoDinamica.Nomenclatura;
            configuracaoFTP.Passivo = ((string)automatizacaoDinamica.Passivo).ToBool();
            configuracaoFTP.Porta = (string)automatizacaoDinamica.Porta;
            configuracaoFTP.Senha = (string)automatizacaoDinamica.Senha;
            configuracaoFTP.SSL = ((string)automatizacaoDinamica.SSL).ToBool();
            configuracaoFTP.Usuario = (string)automatizacaoDinamica.Usuario;
            configuracaoFTP.UtilizarSFTP = ((string)automatizacaoDinamica.UtilizarSFTP).ToBool();

            if (configuracaoFTP.Codigo > 0)
                repositorioConfiguracaoFTP.Atualizar(configuracaoFTP, Auditado, historicoAlteracaoAutomatizacao);
            else
                repositorioConfiguracaoFTP.Inserir(configuracaoFTP, Auditado, historicoAlteracaoAutomatizacao);
        }

        private async Task PreencherAutomatizacaoGeracaoDadosConsulta(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioDadosConsulta repositorioAutomatizacaoGeracaoRelatorioDadosConsulta = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioDadosConsulta(unitOfWork);
            Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Newtonsoft.Json.JsonSerializerSettings configuracoesSerializacao = new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };
            Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio relatorioDinamico = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
            Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(relatorioDinamico.Grid);
            Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

            automatizacaoGeracaoRelatorio.Relatorio = await repositorioRelatorio.BuscarPorCodigoAsync(relatorioDinamico.Codigo, cancellationToken);

            Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(relatorioDinamico, automatizacaoGeracaoRelatorio.Relatorio, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, parametrosConsulta.PropriedadeAgrupar);
            TFiltroPesquisa filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            automatizacaoGeracaoRelatorio.DadosConsulta = new Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorioDadosConsulta();
            automatizacaoGeracaoRelatorio.DadosConsulta.FiltrosPesquisa = Newtonsoft.Json.JsonConvert.SerializeObject(filtrosPesquisa, configuracoesSerializacao);
            automatizacaoGeracaoRelatorio.DadosConsulta.ParametrosConsulta = Newtonsoft.Json.JsonConvert.SerializeObject(parametrosConsulta, configuracoesSerializacao);
            automatizacaoGeracaoRelatorio.DadosConsulta.Propriedades = Newtonsoft.Json.JsonConvert.SerializeObject(propriedades, configuracoesSerializacao);
            automatizacaoGeracaoRelatorio.DadosConsulta.RelatorioTemporario = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoRelatorio, configuracoesSerializacao);

            repositorioAutomatizacaoGeracaoRelatorioDadosConsulta.Inserir(automatizacaoGeracaoRelatorio.DadosConsulta);
        }

        #endregion Métodos Privados
    }
}
