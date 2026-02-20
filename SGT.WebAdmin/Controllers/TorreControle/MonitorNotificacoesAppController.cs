using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.TorreControle
{
    [CustomAuthorize("TorreControle/MonitorNotificacoesApp", "Logistica/FilaCarregamento")]
    public class MonitorNotificacoesAppController : BaseController
    {
        #region Construtores

        public MonitorNotificacoesAppController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repIntegracoesSuperApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp integracaoSuperApp = repIntegracoesSuperApp.BuscarPorCodigo(codigo, auditavel: true);
                if (integracaoSuperApp == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                if (integracaoSuperApp.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Apenas integrações com problema de processamento podem ser reenviadas.");


                integracaoSuperApp.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                repIntegracoesSuperApp.Atualizar(integracaoSuperApp);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivosIntegracaoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repositorioIntegracaoNotificacoesSuperApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp integracaoNotificacoesSuperApp = repositorioIntegracaoNotificacoesSuperApp.BuscarPorCodigo(codigo, true);

                if (integracaoNotificacoesSuperApp == null)
                    return new JsonpResult(false, "Histórico de integração não encontrado.");

                if (integracaoNotificacoesSuperApp.ArquivoRequisicao == null && integracaoNotificacoesSuperApp.ArquivoResposta == null)
                    return new JsonpResult(false, true, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracaoNotificacoesSuperApp.ArquivoRequisicao, integracaoNotificacoesSuperApp.ArquivoResposta });
                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos de notificação da Carga {integracaoNotificacoesSuperApp.Carga?.CodigoCargaEmbarcador ?? string.Empty}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download dos arquivos da integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarNotificacaoApp()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<dynamic> objetosNotificacao = Request.GetListParam<dynamic>("objetosNotificacao");
                if (objetosNotificacao.Count == 0) return new JsonpResult(false, "Sem motoristas para notificar.");

                string titulo = Request.GetStringParam("titulo");
                string mensagem = Request.GetStringParam("mensagem");

                Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp servicoIntegracaoNotificacaoApp = new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                Repositorio.Embarcador.SuperApp.NotificacaoApp repositorioNotificacaoApp = new Repositorio.Embarcador.SuperApp.NotificacaoApp(unitOfWork);
                Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp notificacaoApp = new Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp()
                {
                    Tipo = TipoNotificacaoApp.Custom,
                    Titulo = titulo,
                    Mensagem = mensagem
                };

                unitOfWork.Start();
                repositorioNotificacaoApp.Inserir(notificacaoApp);

                foreach (dynamic objeto in objetosNotificacao)
                {
                    List<string> CPFs = new List<string>(objeto.CPFMotoristas.ToString().Split(','));
                    foreach (string cpf in CPFs)
                    {
                        Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarMotoristaPorCPF(cpf);
                        if (motorista != null)
                            servicoIntegracaoNotificacaoApp.GerarIntegracaoNotificacaoCustom(motorista.Codigo, notificacaoApp.Codigo, (int)objeto.CodigoCarga);
                    }
                }
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu falha ao salvar notificação!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de notificação", "TipoNotificacaoDescricao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nº Carga", "CodigoCargaEmbarcador", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Chamado", "Chamado", 7, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Integradora", "Integradora", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data do Envio", "DataEnvioFormatada", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "SituacaoIntegracaoDescricao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 7, Models.Grid.Align.left, true);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaMonitorNotificacaoApp filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repNotificacaoApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(unitOfWork);

            int totalRegistros = repNotificacaoApp.ContarConsulta(filtroPesquisa, parametrosConsulta);
            IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaMonitoramentoNotificacoesApp> lista = totalRegistros > 0 ? repNotificacaoApp.Consultar(filtroPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaMonitoramentoNotificacoesApp>();

            grid.AdicionaRows(lista);

            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaMonitorNotificacaoApp ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaMonitorNotificacaoApp()
            {
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao"),
                TipoNotificacaoApp = Request.GetNullableEnumParam<TipoNotificacaoApp>("TipoNotificacaoApp"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataInicioEnvio = Request.GetDateTimeParam("DataInicioEnvio"),
                DataFimEnvio = Request.GetDateTimeParam("DataFimEnvio"),
                CodigoChamado = Request.GetIntParam("Chamado")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.EndsWith("Descricao"))
                return propriedadeOrdenar.Replace("Descricao", "");

            if (propriedadeOrdenar.EndsWith("Formatada"))
                return propriedadeOrdenar.Replace("Formatada", "");

            if (propriedadeOrdenar == "Motorista")
                return "NomeMotorista";

            if (propriedadeOrdenar == "Transportador")
                return "DescricaoTransportador";

            return propriedadeOrdenar;
        }

        #endregion
    }
}