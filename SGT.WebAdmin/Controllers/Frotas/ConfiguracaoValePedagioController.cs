using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/ConfiguracaoValePedagio")]
    public class ConfiguracaoValePedagioController : BaseController
    {
        #region Construtores

        public ConfiguracaoValePedagioController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio = new Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio();

                PreencherEntidade(configuracaoValePedagio, unitOfWork);
                ValidarEntidade(configuracaoValePedagio, unitOfWork);
                SalvarConfiguracoesIntegracao(configuracaoValePedagio, unitOfWork);

                repConfiguracaoValePedagio.Inserir(configuracaoValePedagio, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio = repConfiguracaoValePedagio.BuscarPorCodigo(codigo, true);

                if (configuracaoValePedagio == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                PreencherEntidade(configuracaoValePedagio, unitOfWork);
                ValidarEntidade(configuracaoValePedagio, unitOfWork);
                SalvarConfiguracoesIntegracao(configuracaoValePedagio, unitOfWork);

                repConfiguracaoValePedagio.Atualizar(configuracaoValePedagio, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoTarget repIntegracaoTarget = new Repositorio.Embarcador.Configuracoes.IntegracaoTarget(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoRepom repIntegracaoRepom = new Repositorio.Embarcador.Configuracoes.IntegracaoRepom(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoPagbem repIntegracaoPagbem = new Repositorio.Embarcador.Configuracoes.IntegracaoPagbem(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoDBTrans repIntegracaoDBTrans = new Repositorio.Embarcador.Configuracoes.IntegracaoDBTrans(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoPamcard repIntegracaoPamcard = new Repositorio.Embarcador.Configuracoes.IntegracaoPamcard(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoQualP repIntegracaoQualP = new Repositorio.Embarcador.Configuracoes.IntegracaoQualP(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoEFrete repositorioIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoEFrete(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio repositorioIntegracaoExtratta = new Repositorio.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio repositorioIntegracaoAmbipar = new Repositorio.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoNDDCargo repositorioIntegracaoNDDCargo = new Repositorio.Embarcador.Configuracoes.IntegracaoNDDCargo(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio = repConfiguracaoValePedagio.BuscarPorCodigo(codigo, true);

                if (configuracaoValePedagio == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                if (configuracaoValePedagio.IntegracaoSemParar != null)
                {
                    repIntegracaoSemParar.Deletar(configuracaoValePedagio.IntegracaoSemParar, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração do Sem Parar", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoTarget != null)
                {
                    repIntegracaoTarget.Deletar(configuracaoValePedagio.IntegracaoTarget, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração do Target", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoRepom != null)
                {
                    repIntegracaoRepom.Deletar(configuracaoValePedagio.IntegracaoRepom, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração do Repom", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoPagbem != null)
                {
                    repIntegracaoPagbem.Deletar(configuracaoValePedagio.IntegracaoPagbem, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da Pagbem", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoDBTrans != null)
                {
                    repIntegracaoDBTrans.Deletar(configuracaoValePedagio.IntegracaoDBTrans, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da DBTrans", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoPamcard != null)
                {
                    repIntegracaoPamcard.Deletar(configuracaoValePedagio.IntegracaoPamcard, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da Pamcard", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoQualP != null)
                {
                    repIntegracaoQualP.Deletar(configuracaoValePedagio.IntegracaoQualP, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da QualP", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoEFrete != null)
                {
                    repositorioIntegracaoEFrete.Deletar(configuracaoValePedagio.IntegracaoEFrete, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da e-Frete", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoExtratta != null)
                {
                    repositorioIntegracaoExtratta.Deletar(configuracaoValePedagio.IntegracaoExtratta, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da Extratta", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoAmbipar != null)
                {
                    repositorioIntegracaoAmbipar.Deletar(configuracaoValePedagio.IntegracaoAmbipar, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da Ambipar", unitOfWork);
                }

                if (configuracaoValePedagio.IntegracaoNDDCargo != null)
                {
                    repositorioIntegracaoNDDCargo.Deletar(configuracaoValePedagio.IntegracaoNDDCargo, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da NDD Cargo", unitOfWork);
                }

                repConfiguracaoValePedagio.Deletar(configuracaoValePedagio, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio = repConfiguracaoValePedagio.BuscarPorCodigo(codigo, false);

                if (configuracaoValePedagio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                var dynConfiguracaoValePedagio = new
                {
                    configuracaoValePedagio.Codigo,
                    configuracaoValePedagio.Situacao,
                    configuracaoValePedagio.TipoIntegracao,
                    TipoOperacao = new { Codigo = configuracaoValePedagio.TipoOperacao?.Codigo ?? 0, Descricao = configuracaoValePedagio.TipoOperacao?.Descricao ?? string.Empty },
                    Filial = new { Codigo = configuracaoValePedagio.Filial?.Codigo ?? 0, Descricao = configuracaoValePedagio.Filial?.Descricao ?? string.Empty },
                    GrupoPessoas = new { Codigo = configuracaoValePedagio.GrupoPessoas?.Codigo ?? 0, Descricao = configuracaoValePedagio.GrupoPessoas?.Descricao ?? string.Empty },
                    ConfiguracaoIntegracao = ObterObjetoConfiguracoesIntegracaoValePedagio(configuracaoValePedagio),
                    configuracaoValePedagio.ConsultarValorPedagioAntesAutorizarEmissao,
                    configuracaoValePedagio.PermitirGerarValePedagioVeiculoProprio
                };

                return new JsonpResult(dynConfiguracaoValePedagio);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaConfiguracaoValePedagio filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo Integração", "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 35, Models.Grid.Align.left, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 35, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("Filial", "Filial", 35, Models.Grid.Align.left, true);
                if (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> listaConfiguracores = repConfiguracaoValePedagio.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repConfiguracaoValePedagio.ContarConsulta(filtrosPesquisa);

                var retorno = (from configuracao in listaConfiguracores
                               select new
                               {
                                   configuracao.Codigo,
                                   TipoIntegracao = configuracao.DescricaoTipoIntegracao,
                                   Filial = configuracao.Filial?.Descricao ?? string.Empty,
                                   TipoOperacao = configuracao.TipoOperacao?.Descricao ?? string.Empty,
                                   GrupoPessoas = configuracao.GrupoPessoas?.Descricao ?? string.Empty,
                                   Situacao = configuracao.DescricaoSituacao
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaConfiguracaoValePedagio ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaConfiguracaoValePedagio()
            {
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao"),
                TipoIntegracao = Request.GetNullableEnumParam<TipoIntegracao>("TipoIntegracao"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

            configuracaoValePedagio.TipoIntegracao = Request.GetEnumParam<TipoIntegracao>("TipoIntegracao");
            configuracaoValePedagio.Situacao = Request.GetBoolParam("Situacao");
            configuracaoValePedagio.Filial = codigoFilial > 0 ? repFilial.BuscarPorCodigo(codigoFilial) : null;
            configuracaoValePedagio.TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            configuracaoValePedagio.GrupoPessoas = codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null;
            configuracaoValePedagio.ConsultarValorPedagioAntesAutorizarEmissao = Request.GetBoolParam("ConsultarValorPedagioAntesAutorizarEmissao");
            configuracaoValePedagio.PermitirGerarValePedagioVeiculoProprio = Request.GetBoolParam("PermitirGerarValePedagioVeiculoProprio");
        }

        private void ValidarEntidade(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);

            List<TipoIntegracao> tiposIntegracaoDisponiveis = new List<TipoIntegracao>() {
                TipoIntegracao.NaoInformada, TipoIntegracao.SemParar, TipoIntegracao.Target, TipoIntegracao.Repom, TipoIntegracao.PagBem, TipoIntegracao.DBTrans, TipoIntegracao.QualP,
                TipoIntegracao.Pamcard, TipoIntegracao.EFrete, TipoIntegracao.Extratta, TipoIntegracao.DigitalCom, TipoIntegracao.Ambipar, TipoIntegracao.NDDCargo
            };

            if (!tiposIntegracaoDisponiveis.Contains(configuracaoValePedagio.TipoIntegracao))
                throw new ControllerException("Tipo de integração inválido");

            Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configDuplicada = repConfiguracaoValePedagio.BuscarConfiguracaoDuplicada(configuracaoValePedagio);
            if (configuracaoValePedagio.Situacao && configDuplicada != null)
                throw new ControllerException("Já existe uma configuração de vale pedágio com os dados informados.");
        }

        #endregion

        #region Métodos Privados - Salvar Integrações

        private void SalvarConfiguracoesIntegracao(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            SalvarConfiguracoesIntegracaoSemParar(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoTarget(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoRepom(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoPagbem(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoDBTrans(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoPamcard(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoQualP(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoEFrete(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoExtratta(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoDigitalCom(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoAmbipar(configuracaoValePedagio, unitOfWork);
            SalvarConfiguracoesIntegracaoNDDCargo(configuracaoValePedagio, unitOfWork);
        }

        private void SalvarConfiguracoesIntegracaoQualP(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoQualP repIntegracaoQualP = new Repositorio.Embarcador.Configuracoes.IntegracaoQualP(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.QualP)
            {
                if (configuracaoValePedagio.IntegracaoQualP == null)
                    return;

                repIntegracaoQualP.Deletar(configuracaoValePedagio.IntegracaoQualP, Auditado);
                configuracaoValePedagio.IntegracaoQualP = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da QualP", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoQualP = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoQualP == null)
                throw new ControllerException("Dados da integração QualP inválido");

            int codigo = ((string)dadosIntegracaoQualP.Codigo).ToInt();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP integracaoQualP = repIntegracaoQualP.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP();

            integracaoQualP.DistanciaMinimaQuadrante = ((string)dadosIntegracaoQualP.DistanciaMinimaQuadrante).ToInt();
            integracaoQualP.Observacao1 = (string)dadosIntegracaoQualP.Observacao;
            integracaoQualP.UrlIntegracao = (string)dadosIntegracaoQualP.URL;
            integracaoQualP.Token = (string)dadosIntegracaoQualP.Token;

            if (string.IsNullOrWhiteSpace(integracaoQualP.UrlIntegracao))
                throw new ControllerException("URL é obrigatório");

            if (string.IsNullOrWhiteSpace(integracaoQualP.Token))
                throw new ControllerException("Token é obrigatório");

            if (integracaoQualP.Codigo > 0)
                repIntegracaoQualP.Atualizar(integracaoQualP);
            else
                repIntegracaoQualP.Inserir(integracaoQualP);

            configuracaoValePedagio.IntegracaoQualP = integracaoQualP;
            configuracaoValePedagio.SetExternalChanges(integracaoQualP.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoSemParar(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.SemParar)
            {
                if (configuracaoValePedagio.IntegracaoSemParar == null)
                    return;

                repIntegracaoSemParar.Deletar(configuracaoValePedagio.IntegracaoSemParar, Auditado);
                configuracaoValePedagio.IntegracaoSemParar = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração do Sem Parar", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoSemParar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoSemParar == null)
                throw new ControllerException("Dados da integração Sem Parar inválido");

            int codigo = ((string)dadosIntegracaoSemParar.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoSemParar.FornecedorValePedagio).ToDouble();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = repIntegracaoSemParar.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar();

            integracaoSemParar.Usuario = (string)dadosIntegracaoSemParar.Usuario;
            integracaoSemParar.Senha = (string)dadosIntegracaoSemParar.Senha;
            integracaoSemParar.CNPJ = Utilidades.String.OnlyNumbers((string)dadosIntegracaoSemParar.CNPJ);
            integracaoSemParar.DiasPrazo = ((string)dadosIntegracaoSemParar.DiasPrazo).ToInt();
            integracaoSemParar.NomeRpt = (string)dadosIntegracaoSemParar.NomeRpt;
            integracaoSemParar.Observacao1 = (string)dadosIntegracaoSemParar.Observacao1;
            integracaoSemParar.Observacao2 = (string)dadosIntegracaoSemParar.Observacao2;
            integracaoSemParar.Observacao3 = (string)dadosIntegracaoSemParar.Observacao3;
            integracaoSemParar.Observacao4 = (string)dadosIntegracaoSemParar.Observacao4;
            integracaoSemParar.Observacao5 = (string)dadosIntegracaoSemParar.Observacao5;
            integracaoSemParar.Observacao6 = (string)dadosIntegracaoSemParar.Observacao6;
            integracaoSemParar.FornecedorValePedagio = cnpjFornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoSemParar.TipoRota = ((string)dadosIntegracaoSemParar.TipoRota).ToEnum<TipoRotaSemParar>();
            integracaoSemParar.TipoConsultaRota = ((string)dadosIntegracaoSemParar.TipoConsultaRota).ToEnum<TipoConsultaRota>();
            integracaoSemParar.UtilizarModeoVeicularCarga = (bool)dadosIntegracaoSemParar.UtilizarModeoVeicularCarga;
            integracaoSemParar.BuscarPracasNaGeracaoDaCarga = (bool)dadosIntegracaoSemParar.BuscarPracasNaGeracaoDaCarga;
            integracaoSemParar.ComprarRetornoVazioSeparado = (bool)dadosIntegracaoSemParar.ComprarRetornoVazioSeparado;
            integracaoSemParar.NaoComprarValePedagioVeiculoSemTag = (bool)dadosIntegracaoSemParar.NaoComprarValePedagioVeiculoSemTag;
            integracaoSemParar.ConsultarValorPedagioParaRota = (bool)dadosIntegracaoSemParar.ConsultarValorPedagioParaRota;
            integracaoSemParar.ConsultarEComprarPedagioFreteEmbarcador = (bool)dadosIntegracaoSemParar.ConsultarEComprarPedagioFreteEmbarcador;
            integracaoSemParar.ConsultarSeVeiculoPossuiCadastro = (bool)dadosIntegracaoSemParar.ConsultarSeVeiculoPossuiCadastro;
            integracaoSemParar.NotificarTransportadorPorEmail = ((string)dadosIntegracaoSemParar.NotificarTransportadorPorEmail).ToBool();
            integracaoSemParar.GerarRegistroMesmoSeRotaNaoPossuirPracaPedagio = ((string)dadosIntegracaoSemParar.GerarRegistroMesmoSeRotaNaoPossuirPracaPedagio).ToBool();
            integracaoSemParar.ComprarSomenteNoMesVigente = ((string)dadosIntegracaoSemParar.ComprarSomenteNoMesVigente).ToBool();
            integracaoSemParar.ConsultarExtrato = ((string)dadosIntegracaoSemParar.ConsultarExtrato).ToBool();
            integracaoSemParar.QuantidadeDiasConsultarExtrato = ((string)dadosIntegracaoSemParar.QuantidadeDiasConsultarExtrato).ToInt();
            integracaoSemParar.TipoBuscarPracasNaGeracaoDaCarga = ((string)dadosIntegracaoSemParar.TipoBuscarPracasNaGeracaoDaCarga).ToEnum<TipoBuscarPracasNaGeracaoDaCarga>();
            integracaoSemParar.UrlIntegracaoRest = (string)dadosIntegracaoSemParar.UrlIntegracaoRest;

            if (string.IsNullOrWhiteSpace(integracaoSemParar.Usuario))
                throw new ControllerException("Usuário do Sem Parar é obrigatório");

            if (string.IsNullOrWhiteSpace(integracaoSemParar.Senha))
                throw new ControllerException("Senha do Sem Parar é obrigatório");

            if (string.IsNullOrWhiteSpace(integracaoSemParar.CNPJ))
                throw new ControllerException("CNPJ do Sem Parar é obrigatório");

            if (string.IsNullOrWhiteSpace(integracaoSemParar.UrlIntegracaoRest))
                throw new ControllerException("URL integração Rest Sem Parar é obrigatório");

            if (integracaoSemParar.Codigo > 0)
                repIntegracaoSemParar.Atualizar(integracaoSemParar);
            else
                repIntegracaoSemParar.Inserir(integracaoSemParar);

            configuracaoValePedagio.IntegracaoSemParar = integracaoSemParar;
            configuracaoValePedagio.SetExternalChanges(integracaoSemParar.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoTarget(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTarget repIntegracaoTarget = new Repositorio.Embarcador.Configuracoes.IntegracaoTarget(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.Target)
            {
                if (configuracaoValePedagio.IntegracaoTarget == null)
                    return;

                repIntegracaoTarget.Deletar(configuracaoValePedagio.IntegracaoTarget, Auditado);
                configuracaoValePedagio.IntegracaoTarget = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração do Target", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoTarget = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoTarget == null)
                throw new ControllerException("Dados da integração Target inválido");

            int codigo = ((string)dadosIntegracaoTarget.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoTarget.FornecedorValePedagio).ToDouble();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = repIntegracaoTarget.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget();

            integracaoTarget.Usuario = (string)dadosIntegracaoTarget.Usuario;
            integracaoTarget.Senha = (string)dadosIntegracaoTarget.Senha;
            integracaoTarget.DiasPrazo = ((string)dadosIntegracaoTarget.DiasPrazo).ToInt();
            integracaoTarget.Token = (string)dadosIntegracaoTarget.Token;
            integracaoTarget.CodigoCentroCusto = ((string)dadosIntegracaoTarget.CodigoCentroCusto).ToInt();
            integracaoTarget.FornecedorValePedagio = cnpjFornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoTarget.CadastrarRotaPorIBGE = ((string)dadosIntegracaoTarget.CadastrarRotaPorIBGE).ToBool();
            integracaoTarget.CadastrarRotaPorCoordenadas = ((string)dadosIntegracaoTarget.CadastrarRotaPorCoordenadas).ToBool();
            integracaoTarget.NaoBuscarCartaoMotoristaTarget = ((string)dadosIntegracaoTarget.NaoBuscarCartaoMotoristaTarget).ToBool();
            integracaoTarget.PreencherLatLongDaRotaIntegracao = ((string)dadosIntegracaoTarget.PreencherLatLongDaRotaIntegracao).ToBool();
            integracaoTarget.PreencherPontosPassagemModificadoCliente = ((string)dadosIntegracaoTarget.PreencherPontosPassagemModificadoCliente).ToBool();
            integracaoTarget.NotificarTransportadorPorEmail = ((string)dadosIntegracaoTarget.NotificarTransportadorPorEmail).ToBool();

            if (string.IsNullOrWhiteSpace(integracaoTarget.Usuario))
                throw new ControllerException("Usuário do Target é obrigatório");

            if (string.IsNullOrWhiteSpace(integracaoTarget.Senha))
                throw new ControllerException("Senha do Target é obrigatório");

            if (integracaoTarget.Codigo > 0)
                repIntegracaoTarget.Atualizar(integracaoTarget);
            else
                repIntegracaoTarget.Inserir(integracaoTarget);

            configuracaoValePedagio.IntegracaoTarget = integracaoTarget;
            configuracaoValePedagio.SetExternalChanges(integracaoTarget.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoRepom(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoRepom repIntegracaoRepom = new Repositorio.Embarcador.Configuracoes.IntegracaoRepom(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.Repom)
            {
                if (configuracaoValePedagio.IntegracaoRepom == null)
                    return;

                repIntegracaoRepom.Deletar(configuracaoValePedagio.IntegracaoRepom, Auditado);
                configuracaoValePedagio.IntegracaoRepom = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração do Repom", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoRepom = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoRepom == null)
                throw new ControllerException("Dados da integração Repom inválido");

            int codigo = ((string)dadosIntegracaoRepom.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoRepom.FornecedorValePedagio).ToDouble();
            int codigoFilial = ((string)dadosIntegracaoRepom.FilialCompra).ToInt();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom integracaoRepom = repIntegracaoRepom.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom();

            integracaoRepom.FornecedorValePedagio = cnpjFornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoRepom.TipoIntegracaoRepom = (TipoIntegracaoRepom)dadosIntegracaoRepom.TipoIntegracaoRepom;
            integracaoRepom.TipoRotaFreteRepom = (TipoRotaFreteRepom)dadosIntegracaoRepom.TipoRotaFreteRepom;
            integracaoRepom.FilialCompra = codigoFilial > 0 ? new Repositorio.Embarcador.Filiais.Filial(unitOfWork).BuscarPorCodigo(codigoFilial) : null;

            integracaoRepom.CodigoCliente = (string)dadosIntegracaoRepom.CodigoCliente;
            integracaoRepom.CodigoFilial = (string)dadosIntegracaoRepom.CodigoFilial;
            integracaoRepom.AssinaturaDigital = (string)dadosIntegracaoRepom.AssinaturaDigital;

            integracaoRepom.URLAutenticacaoRota = (string)dadosIntegracaoRepom.URLAutenticacaoRota;
            integracaoRepom.URLRota = (string)dadosIntegracaoRepom.URLRota;
            integracaoRepom.ClientID = (string)dadosIntegracaoRepom.ClientID;
            integracaoRepom.ClientSecret = (string)dadosIntegracaoRepom.ClientSecret;
            integracaoRepom.URLViagem = (string)dadosIntegracaoRepom.URLViagem;
            integracaoRepom.Usuario = (string)dadosIntegracaoRepom.Usuario;
            integracaoRepom.Senha = (string)dadosIntegracaoRepom.Senha;
            integracaoRepom.ConsiderarEixosSuspensosNaConsultaDoValePedagio = (bool)dadosIntegracaoRepom.ConsiderarEixosSuspensosNaConsultaDoValePedagio;
            integracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio = (bool)dadosIntegracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio;

            if (integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.SOAP)
            {
                if (string.IsNullOrWhiteSpace(integracaoRepom.CodigoCliente))
                    throw new ControllerException("Código cliente Repom é obrigatório");

                if (string.IsNullOrWhiteSpace(integracaoRepom.AssinaturaDigital))
                    throw new ControllerException("Assinatura digital Repom é obrigatório");
            }

            if (integracaoRepom.Codigo > 0)
                repIntegracaoRepom.Atualizar(integracaoRepom);
            else
                repIntegracaoRepom.Inserir(integracaoRepom);

            configuracaoValePedagio.IntegracaoRepom = integracaoRepom;
            configuracaoValePedagio.SetExternalChanges(integracaoRepom.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoPagbem(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoPagbem repIntegracaoPagbem = new Repositorio.Embarcador.Configuracoes.IntegracaoPagbem(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.PagBem)
            {
                if (configuracaoValePedagio.IntegracaoPagbem == null)
                    return;

                repIntegracaoPagbem.Deletar(configuracaoValePedagio.IntegracaoPagbem, Auditado);
                configuracaoValePedagio.IntegracaoPagbem = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração do Pagbem", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoPagbem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoPagbem == null)
                throw new ControllerException("Dados da integração Pagbem inválido");

            int codigo = ((string)dadosIntegracaoPagbem.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoPagbem.FornecedorValePedagio).ToDouble();
            int quantidadeEixosPadraoValePedagio = ((string)dadosIntegracaoPagbem.QuantidadeEixosPadraoValePedagio).ToInt();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem = repIntegracaoPagbem.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem();

            integracaoPagbem.CNPJEmpresaContratante = (string)dadosIntegracaoPagbem.CNPJEmpresaContratante;
            integracaoPagbem.SenhaPagbem = (string)dadosIntegracaoPagbem.SenhaPagbem;
            integracaoPagbem.URLPagbem = (string)dadosIntegracaoPagbem.URLPagbem;
            integracaoPagbem.UsuarioPagbem = (string)dadosIntegracaoPagbem.UsuarioPagbem;
            integracaoPagbem.FornecedorValePedagio = cnpjFornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoPagbem.IntegrarNumeroRPSNFSE = ((bool)dadosIntegracaoPagbem.IntegrarNumeroRPSNFSE);
            integracaoPagbem.LiberarViagemManualmente = (bool)dadosIntegracaoPagbem.LiberarViagemManualmente;
            integracaoPagbem.ConsultarVeiculoSemParar = (bool)dadosIntegracaoPagbem.ConsultarVeiculoSemParar;
            integracaoPagbem.QuantidadeEixosPadraoValePedagio = quantidadeEixosPadraoValePedagio;

            if (string.IsNullOrWhiteSpace(integracaoPagbem.URLPagbem))
                throw new ControllerException("URL da PagBem é obrigatório");

            if (string.IsNullOrWhiteSpace(integracaoPagbem.SenhaPagbem))
                throw new ControllerException("Senha da PagBem é obrigatório");

            if (integracaoPagbem.Codigo > 0)
                repIntegracaoPagbem.Atualizar(integracaoPagbem);
            else
                repIntegracaoPagbem.Inserir(integracaoPagbem);

            configuracaoValePedagio.IntegracaoPagbem = integracaoPagbem;
            configuracaoValePedagio.SetExternalChanges(integracaoPagbem.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoDBTrans(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDBTrans repIntegracaoDBTrans = new Repositorio.Embarcador.Configuracoes.IntegracaoDBTrans(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.DBTrans)
            {
                if (configuracaoValePedagio.IntegracaoDBTrans == null)
                    return;

                repIntegracaoDBTrans.Deletar(configuracaoValePedagio.IntegracaoDBTrans, Auditado);
                configuracaoValePedagio.IntegracaoDBTrans = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da DBTrans", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoDBTrans = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoDBTrans == null)
                throw new ControllerException("Dados da integração DBTrans inválido");

            int codigo = ((string)dadosIntegracaoDBTrans.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoDBTrans.FornecedorValePedagio).ToDouble();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans = repIntegracaoDBTrans.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans();

            integracaoDBTrans.FornecedorValePedagio = cnpjFornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;

            integracaoDBTrans.CodigoCliente = (string)dadosIntegracaoDBTrans.CodigoCliente;
            integracaoDBTrans.URL = (string)dadosIntegracaoDBTrans.URL;
            integracaoDBTrans.Usuario = (string)dadosIntegracaoDBTrans.Usuario;
            integracaoDBTrans.Senha = (string)dadosIntegracaoDBTrans.Senha;
            integracaoDBTrans.IdLocalImpressao = ((string)dadosIntegracaoDBTrans.IdLocalImpressao).ToInt();
            integracaoDBTrans.TipoRota = ((string)dadosIntegracaoDBTrans.TipoRota).ToEnum<TipoRotaDBTrans>();
            integracaoDBTrans.TipoRotaFrete = ((string)dadosIntegracaoDBTrans.TipoRotaFrete).ToEnum<TipoRotaFreteDBTrans>();
            integracaoDBTrans.MeioPagamento = ((string)dadosIntegracaoDBTrans.MeioPagamento).ToEnum<MeioPagamentoDBTrans>();
            integracaoDBTrans.NaoEnviarTransportadorNaIntegracao = ((string)dadosIntegracaoDBTrans.NaoEnviarTransportadorNaIntegracao).ToBool();
            integracaoDBTrans.NaoEnviarMotoristaNaIntegracao = ((string)dadosIntegracaoDBTrans.NaoEnviarMotoristaNaIntegracao).ToBool();
            integracaoDBTrans.TipoTomador = ((string)dadosIntegracaoDBTrans.TipoTomador).ToNullableEnum<Dominio.Enumeradores.TipoTomador>();
            integracaoDBTrans.VerificarVeiculoCompraPorTag = ((string)dadosIntegracaoDBTrans.VerificarVeiculoCompraPorTag).ToBool();
            integracaoDBTrans.ConsultarValorPedagioParaRota = ((string)dadosIntegracaoDBTrans.ConsultarValorPedagioParaRota).ToBool();
            integracaoDBTrans.CadastrarTransportadorAntesDaCompra = ((string)dadosIntegracaoDBTrans.CadastrarTransportadorAntesDaCompra).ToBool();
            integracaoDBTrans.CadastrarMotoristaAntesDaCompra = ((string)dadosIntegracaoDBTrans.CadastrarMotoristaAntesDaCompra).ToBool();
            integracaoDBTrans.CadastrarVeiculoAntesDaCompra = ((string)dadosIntegracaoDBTrans.CadastrarVeiculoAntesDaCompra).ToBool();
            integracaoDBTrans.CadastrarDocumentoTransportadorAntesDaCompra = ((string)dadosIntegracaoDBTrans.CadastrarDocumentoTransportadorAntesDaCompra).ToBool();

            if (integracaoDBTrans.Codigo > 0)
                repIntegracaoDBTrans.Atualizar(integracaoDBTrans);
            else
                repIntegracaoDBTrans.Inserir(integracaoDBTrans);

            configuracaoValePedagio.IntegracaoDBTrans = integracaoDBTrans;
            configuracaoValePedagio.SetExternalChanges(integracaoDBTrans.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoPamcard(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoPamcard repIntegracaoPamcard = new Repositorio.Embarcador.Configuracoes.IntegracaoPamcard(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.Pamcard)
            {
                if (configuracaoValePedagio.IntegracaoPamcard == null)
                    return;

                repIntegracaoPamcard.Deletar(configuracaoValePedagio.IntegracaoPamcard, Auditado);
                configuracaoValePedagio.IntegracaoPamcard = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da Pamcard", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoPamcard = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoPamcard == null)
                throw new ControllerException("Dados da integração Pamcard inválido");

            int codigo = ((string)dadosIntegracaoPamcard.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoPamcard.FornecedorValePedagio).ToDouble();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard integracaoPamcard = repIntegracaoPamcard.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard();

            integracaoPamcard.FornecedorValePedagio = cnpjFornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoPamcard.URL = (string)dadosIntegracaoPamcard.URL;
            integracaoPamcard.TipoRota = ((string)dadosIntegracaoPamcard.TipoRota).ToEnum<TipoRotaPamcard>();
            integracaoPamcard.AcoesPamcard = ((string)dadosIntegracaoPamcard.AcoesPamcard).ToEnum<TipoAcaoPamcard>();
            integracaoPamcard.AdicionarValorConsultadoComoComponentePedagioCarga = (bool)dadosIntegracaoPamcard.AdicionarValorConsultadoComoComponentePedagioCarga;
            integracaoPamcard.UtilizarCertificadoFilialMatrizCompraValePedagio = (bool)dadosIntegracaoPamcard.UtilizarCertificadoFilialMatrizCompraValePedagio;
            integracaoPamcard.EnviarCEPsNaIntegracao = (bool)dadosIntegracaoPamcard.EnviarCEPsNaIntegracao;
            integracaoPamcard.SomarEixosSuspensosValePedagio = (bool)dadosIntegracaoPamcard.SomarEixosSuspensosValePedagio;
            integracaoPamcard.NaoEnviarIdaVoltaValePedagio = (bool)dadosIntegracaoPamcard.NaoEnviarIdaVoltaValePedagio;
            integracaoPamcard.ConsiderarRotaFreteDaCargaNoValePedagio = (bool)dadosIntegracaoPamcard.ConsiderarRotaFreteDaCargaNoValePedagio;

            if (integracaoPamcard.Codigo > 0)
                repIntegracaoPamcard.Atualizar(integracaoPamcard);
            else
                repIntegracaoPamcard.Inserir(integracaoPamcard);

            configuracaoValePedagio.IntegracaoPamcard = integracaoPamcard;
            configuracaoValePedagio.SetExternalChanges(integracaoPamcard.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoEFrete(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoEFrete repositorioIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoEFrete(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.EFrete)
            {
                if (configuracaoValePedagio.IntegracaoEFrete == null)
                    return;

                repositorioIntegracaoEFrete.Deletar(configuracaoValePedagio.IntegracaoEFrete, Auditado);
                configuracaoValePedagio.IntegracaoEFrete = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da e-Frete", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoEFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoEFrete == null)
                throw new ControllerException("Dados da integração e-Frete inválido");

            int codigo = ((string)dadosIntegracaoEFrete.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoEFrete.FornecedorValePedagio).ToDouble();
            double cnpjCliente = ((string)dadosIntegracaoEFrete.Cliente).ToDouble();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete = repositorioIntegracaoEFrete.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete();

            integracaoEFrete.FornecedorValePedagio = cnpjFornecedor > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoEFrete.Cliente = cnpjCliente > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cnpjCliente) : null;
            integracaoEFrete.URL = (string)dadosIntegracaoEFrete.URL;
            integracaoEFrete.CodigoIntegrador = (string)dadosIntegracaoEFrete.CodigoIntegrador;
            integracaoEFrete.Usuario = (string)dadosIntegracaoEFrete.Usuario;
            integracaoEFrete.Senha = (string)dadosIntegracaoEFrete.Senha;
            integracaoEFrete.DiasPrazo = ((string)dadosIntegracaoEFrete.DiasPrazo).ToInt();
            integracaoEFrete.NotificarTransportadorPorEmail = ((string)dadosIntegracaoEFrete.NotificarTransportadorPorEmail).ToBool();
            integracaoEFrete.EnviarPontosPassagemRotaFrete = ((string)dadosIntegracaoEFrete.EnviarPontosPassagemRotaFrete).ToBool();
            integracaoEFrete.EnviarPolilinhaRoteirizacaoCarga = ((string)dadosIntegracaoEFrete.EnviarPolilinhaRoteirizacaoCarga).ToBool();
            integracaoEFrete.EnviarTipoVeiculoNaIntegracao = ((string)dadosIntegracaoEFrete.EnviarTipoVeiculoNaIntegracao).ToBool();

            if (integracaoEFrete.Codigo > 0)
                repositorioIntegracaoEFrete.Atualizar(integracaoEFrete);
            else
                repositorioIntegracaoEFrete.Inserir(integracaoEFrete);

            configuracaoValePedagio.IntegracaoEFrete = integracaoEFrete;
            configuracaoValePedagio.SetExternalChanges(integracaoEFrete.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoExtratta(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio repositorioIntegracaoExtratta = new Repositorio.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.Extratta)
            {
                if (configuracaoValePedagio.IntegracaoExtratta == null)
                    return;

                repositorioIntegracaoExtratta.Deletar(configuracaoValePedagio.IntegracaoExtratta, Auditado);
                configuracaoValePedagio.IntegracaoExtratta = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da Extratta", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoExtratta = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoExtratta == null)
                throw new ControllerException("Dados da integração Extratta inválido");

            int codigo = ((string)dadosIntegracaoExtratta.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoExtratta.FornecedorValePedagio).ToDouble();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio integracaoExtratta = repositorioIntegracaoExtratta.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio();

            integracaoExtratta.FornecedorValePedagio = cnpjFornecedor > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoExtratta.URL = (string)dadosIntegracaoExtratta.URL;
            integracaoExtratta.Token = (string)dadosIntegracaoExtratta.Token;
            integracaoExtratta.CNPJAplicacao = ((string)dadosIntegracaoExtratta.CNPJAplicacao).ObterSomenteNumeros();
            integracaoExtratta.TipoRota = ((string)dadosIntegracaoExtratta.TipoRota).ToEnum<TipoRotaExtratta>();
            integracaoExtratta.FornecedorParceiro = ((string)dadosIntegracaoExtratta.FornecedorParceiro).ToEnum<FornecedorPedagioExtratta>();

            if (integracaoExtratta.Codigo > 0)
                repositorioIntegracaoExtratta.Atualizar(integracaoExtratta);
            else
                repositorioIntegracaoExtratta.Inserir(integracaoExtratta);

            configuracaoValePedagio.IntegracaoExtratta = integracaoExtratta;
            configuracaoValePedagio.SetExternalChanges(integracaoExtratta.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoDigitalCom(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDigitalComValePedagio repositorioIntegracaoDigitalCom = new Repositorio.Embarcador.Configuracoes.IntegracaoDigitalComValePedagio(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.DigitalCom)
            {
                if (configuracaoValePedagio.IntegracaoDigitalCom == null)
                    return;

                repositorioIntegracaoDigitalCom.Deletar(configuracaoValePedagio.IntegracaoDigitalCom, Auditado);
                configuracaoValePedagio.IntegracaoDigitalCom = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da DigitalCom", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoDigitalCom = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoDigitalCom == null)
                throw new ControllerException("Dados da integração Extratta inválido");

            int codigo = ((string)dadosIntegracaoDigitalCom.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoDigitalCom.FornecedorValePedagio).ToDouble();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalComValePedagio integracaoDigitalCom = repositorioIntegracaoDigitalCom.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalComValePedagio();

            integracaoDigitalCom.FornecedorValePedagio = cnpjFornecedor > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoDigitalCom.NotificarTransportadorPorEmail = ((string)dadosIntegracaoDigitalCom.NotificarTransportadorPorEmail).ToBool();
            integracaoDigitalCom.EnviarNumeroCargaNoCampoDocumentoTransporte = ((string)dadosIntegracaoDigitalCom.EnviarNumeroCargaNoCampoDocumentoTransporte).ToBool();

            if (integracaoDigitalCom.Codigo > 0)
                repositorioIntegracaoDigitalCom.Atualizar(integracaoDigitalCom);
            else
                repositorioIntegracaoDigitalCom.Inserir(integracaoDigitalCom);

            configuracaoValePedagio.IntegracaoDigitalCom = integracaoDigitalCom;
            configuracaoValePedagio.SetExternalChanges(integracaoDigitalCom.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoAmbipar(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio repositorioIntegracaoAmbipar = new Repositorio.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.Ambipar)
            {
                if (configuracaoValePedagio.IntegracaoAmbipar == null)
                    return;

                repositorioIntegracaoAmbipar.Deletar(configuracaoValePedagio.IntegracaoAmbipar, Auditado);
                configuracaoValePedagio.IntegracaoAmbipar = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da Ambipar", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoAmbipar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoAmbipar == null)
                throw new ControllerException("Dados da integração Ambipar inválido");

            int codigo = ((string)dadosIntegracaoAmbipar.Codigo).ToInt();
            double cnpjFornecedor = ((string)dadosIntegracaoAmbipar.FornecedorValePedagio).ToDouble();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio integracaoAmbipar = repositorioIntegracaoAmbipar.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio();

            integracaoAmbipar.FornecedorValePedagio = cnpjFornecedor > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cnpjFornecedor) : null;
            integracaoAmbipar.URL = (string)dadosIntegracaoAmbipar.URL;
            integracaoAmbipar.Usuario = (string)dadosIntegracaoAmbipar.Usuario;
            integracaoAmbipar.Senha = (string)dadosIntegracaoAmbipar.Senha;

            if (integracaoAmbipar.Codigo > 0)
                repositorioIntegracaoAmbipar.Atualizar(integracaoAmbipar);
            else
                repositorioIntegracaoAmbipar.Inserir(integracaoAmbipar);

            configuracaoValePedagio.IntegracaoAmbipar = integracaoAmbipar;
            configuracaoValePedagio.SetExternalChanges(integracaoAmbipar.GetCurrentChanges());
        }

        private void SalvarConfiguracoesIntegracaoNDDCargo(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoNDDCargo repositorioIntegracaoNDDCargo = new Repositorio.Embarcador.Configuracoes.IntegracaoNDDCargo(unitOfWork);

            if (configuracaoValePedagio.TipoIntegracao != TipoIntegracao.NDDCargo)
            {
                if (configuracaoValePedagio.IntegracaoNDDCargo == null)
                    return;

                repositorioIntegracaoNDDCargo.Deletar(configuracaoValePedagio.IntegracaoNDDCargo, Auditado);
                configuracaoValePedagio.IntegracaoNDDCargo = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, configuracaoValePedagio, "Removeu configurações de Integração da NDD Cargo", unitOfWork);
                return;
            }

            dynamic dadosIntegracaoNDDCargo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoIntegracao"));
            if (dadosIntegracaoNDDCargo == null)
                throw new ControllerException("Dados da integração NDD Cargo inválido");

            int codigo = ((string)dadosIntegracaoNDDCargo.Codigo).ToInt();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo integracaoNDDCargo = repositorioIntegracaoNDDCargo.BuscarPorCodigo(codigo, true) ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo();

            integracaoNDDCargo.URL = (string)dadosIntegracaoNDDCargo.URL;
            integracaoNDDCargo.EnterpriseId = (string)dadosIntegracaoNDDCargo.EnterpriseId;
            integracaoNDDCargo.Token = (string)dadosIntegracaoNDDCargo.Token;
            integracaoNDDCargo.Versao = (string)dadosIntegracaoNDDCargo.Versao;

            if (integracaoNDDCargo.Codigo > 0)
                repositorioIntegracaoNDDCargo.Atualizar(integracaoNDDCargo);
            else
                repositorioIntegracaoNDDCargo.Inserir(integracaoNDDCargo);

            configuracaoValePedagio.IntegracaoNDDCargo = integracaoNDDCargo;
            configuracaoValePedagio.SetExternalChanges(integracaoNDDCargo.GetCurrentChanges());
        }

        #endregion

        #region Métodos Privados - Obter Integrações

        private dynamic ObterObjetoConfiguracoesIntegracaoValePedagio(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio configuracaoValePedagio)
        {
            if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.SemParar)
                return RetornaDynIntegracaoSemParar(configuracaoValePedagio.IntegracaoSemParar);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.Target)
                return RetornaDynIntegracaoTarget(configuracaoValePedagio.IntegracaoTarget);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.Repom)
                return RetornaDynIntegracaoRepom(configuracaoValePedagio.IntegracaoRepom);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.PagBem)
                return RetornaDynIntegracaoPagbem(configuracaoValePedagio.IntegracaoPagbem);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.DBTrans)
                return RetornaDynIntegracaoDBTrans(configuracaoValePedagio.IntegracaoDBTrans);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.Pamcard)
                return RetornaDynIntegracaoPamcard(configuracaoValePedagio.IntegracaoPamcard);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.QualP)
                return RetornaDynIntegracaoQualP(configuracaoValePedagio.IntegracaoQualP);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.EFrete)
                return RetornaDynIntegracaoEFrete(configuracaoValePedagio.IntegracaoEFrete);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.Extratta)
                return RetornaDynIntegracaoExtratta(configuracaoValePedagio.IntegracaoExtratta);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.DigitalCom)
                return RetornaDynIntegracaoDigitalCom(configuracaoValePedagio.IntegracaoDigitalCom);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.Ambipar)
                return RetornaDynIntegracaoAmbipar(configuracaoValePedagio.IntegracaoAmbipar);
            else if (configuracaoValePedagio.TipoIntegracao == TipoIntegracao.NDDCargo)
                return RetornaDynIntegracaoNDDCargo(configuracaoValePedagio.IntegracaoNDDCargo);
            else
                return null;
        }

        private dynamic RetornaDynIntegracaoSemParar(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar)
        {
            if (integracaoSemParar == null)
                return null;

            return new
            {
                integracaoSemParar.Codigo,
                integracaoSemParar.Usuario,
                integracaoSemParar.Senha,
                FornecedorValePedagio = integracaoSemParar.FornecedorValePedagio != null ? new { integracaoSemParar.FornecedorValePedagio.Codigo, integracaoSemParar.FornecedorValePedagio.Descricao } : null,
                integracaoSemParar.TipoRota,
                integracaoSemParar.TipoConsultaRota,
                integracaoSemParar.CNPJ,
                integracaoSemParar.DiasPrazo,
                integracaoSemParar.Observacao1,
                integracaoSemParar.Observacao2,
                integracaoSemParar.Observacao3,
                integracaoSemParar.Observacao4,
                integracaoSemParar.Observacao5,
                integracaoSemParar.Observacao6,
                integracaoSemParar.NomeRpt,
                integracaoSemParar.UtilizarModeoVeicularCarga,
                integracaoSemParar.BuscarPracasNaGeracaoDaCarga,
                integracaoSemParar.ComprarRetornoVazioSeparado,
                integracaoSemParar.NaoComprarValePedagioVeiculoSemTag,
                integracaoSemParar.ConsultarValorPedagioParaRota,
                integracaoSemParar.ConsultarEComprarPedagioFreteEmbarcador,
                integracaoSemParar.ConsultarSeVeiculoPossuiCadastro,
                integracaoSemParar.NotificarTransportadorPorEmail,
                integracaoSemParar.GerarRegistroMesmoSeRotaNaoPossuirPracaPedagio,
                integracaoSemParar.ComprarSomenteNoMesVigente,
                integracaoSemParar.ConsultarExtrato,
                integracaoSemParar.QuantidadeDiasConsultarExtrato,
                integracaoSemParar.TipoBuscarPracasNaGeracaoDaCarga,
                integracaoSemParar.UrlIntegracaoRest,
            };
        }

        private dynamic RetornaDynIntegracaoTarget(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget)
        {
            if (integracaoTarget == null)
                return null;

            return new
            {
                integracaoTarget.Codigo,
                integracaoTarget.Usuario,
                integracaoTarget.Senha,
                integracaoTarget.Token,
                FornecedorValePedagio = integracaoTarget.FornecedorValePedagio != null ? new { integracaoTarget.FornecedorValePedagio.Codigo, integracaoTarget.FornecedorValePedagio.Descricao } : null,
                integracaoTarget.DiasPrazo,
                integracaoTarget.CodigoCentroCusto,
                integracaoTarget.CadastrarRotaPorIBGE,
                integracaoTarget.CadastrarRotaPorCoordenadas,
                integracaoTarget.NaoBuscarCartaoMotoristaTarget,
                integracaoTarget.PreencherLatLongDaRotaIntegracao,
                integracaoTarget.PreencherPontosPassagemModificadoCliente,
                integracaoTarget.NotificarTransportadorPorEmail
            };
        }

        private dynamic RetornaDynIntegracaoRepom(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom integracaoRepom)
        {
            if (integracaoRepom == null)
                return null;

            return new
            {
                integracaoRepom.Codigo,
                integracaoRepom.CodigoCliente,
                integracaoRepom.TipoIntegracaoRepom,
                integracaoRepom.CodigoFilial,
                integracaoRepom.AssinaturaDigital,
                FornecedorValePedagio = integracaoRepom.FornecedorValePedagio != null ? new { integracaoRepom.FornecedorValePedagio.Codigo, integracaoRepom.FornecedorValePedagio.Descricao } : null,
                integracaoRepom.URLAutenticacaoRota,
                integracaoRepom.URLRota,
                integracaoRepom.ClientID,
                integracaoRepom.ClientSecret,
                integracaoRepom.URLViagem,
                integracaoRepom.Usuario,
                integracaoRepom.Senha,
                integracaoRepom.TipoRotaFreteRepom,
                FilialCompra = integracaoRepom.FilialCompra != null ? new { integracaoRepom.FilialCompra.Codigo, integracaoRepom.FilialCompra.Descricao } : null,
                integracaoRepom.ConsiderarEixosSuspensosNaConsultaDoValePedagio,
                integracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio,
            };
        }

        private dynamic RetornaDynIntegracaoPagbem(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem)
        {
            if (integracaoPagbem == null)
                return null;

            return new
            {
                integracaoPagbem.Codigo,
                integracaoPagbem.CNPJEmpresaContratante,
                integracaoPagbem.SenhaPagbem,
                integracaoPagbem.URLPagbem,
                integracaoPagbem.UsuarioPagbem,
                integracaoPagbem.IntegrarNumeroRPSNFSE,
                integracaoPagbem.LiberarViagemManualmente,
                FornecedorValePedagio = integracaoPagbem.FornecedorValePedagio != null ? new { integracaoPagbem.FornecedorValePedagio.Codigo, integracaoPagbem.FornecedorValePedagio.Descricao } : null,
                integracaoPagbem.QuantidadeEixosPadraoValePedagio,
                integracaoPagbem.ConsultarVeiculoSemParar
            };
        }

        private dynamic RetornaDynIntegracaoDBTrans(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans)
        {
            if (integracaoDBTrans == null)
                return null;

            return new
            {
                integracaoDBTrans.Codigo,
                FornecedorValePedagio = integracaoDBTrans.FornecedorValePedagio != null ? new { integracaoDBTrans.FornecedorValePedagio.Codigo, integracaoDBTrans.FornecedorValePedagio.Descricao } : null,
                integracaoDBTrans.CodigoCliente,
                integracaoDBTrans.URL,
                integracaoDBTrans.Usuario,
                integracaoDBTrans.Senha,
                integracaoDBTrans.IdLocalImpressao,
                integracaoDBTrans.TipoRota,
                integracaoDBTrans.TipoRotaFrete,
                integracaoDBTrans.MeioPagamento,
                integracaoDBTrans.NaoEnviarTransportadorNaIntegracao,
                integracaoDBTrans.NaoEnviarMotoristaNaIntegracao,
                TipoTomador = integracaoDBTrans.TipoTomador?.ToString("d") ?? string.Empty,
                integracaoDBTrans.VerificarVeiculoCompraPorTag,
                integracaoDBTrans.ConsultarValorPedagioParaRota,
                integracaoDBTrans.CadastrarTransportadorAntesDaCompra,
                integracaoDBTrans.CadastrarDocumentoTransportadorAntesDaCompra,
                integracaoDBTrans.CadastrarMotoristaAntesDaCompra,
                integracaoDBTrans.CadastrarVeiculoAntesDaCompra
            };
        }

        private dynamic RetornaDynIntegracaoPamcard(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard integracaoPamcard)
        {
            if (integracaoPamcard == null)
                return null;

            return new
            {
                integracaoPamcard.Codigo,
                FornecedorValePedagio = integracaoPamcard.FornecedorValePedagio != null ? new { integracaoPamcard.FornecedorValePedagio.Codigo, integracaoPamcard.FornecedorValePedagio.Descricao } : null,
                integracaoPamcard.URL,
                integracaoPamcard.TipoRota,
                integracaoPamcard.AcoesPamcard,
                integracaoPamcard.AdicionarValorConsultadoComoComponentePedagioCarga,
                integracaoPamcard.UtilizarCertificadoFilialMatrizCompraValePedagio,
                integracaoPamcard.EnviarCEPsNaIntegracao,
                integracaoPamcard.SomarEixosSuspensosValePedagio,
                integracaoPamcard.NaoEnviarIdaVoltaValePedagio,
                integracaoPamcard.ConsiderarRotaFreteDaCargaNoValePedagio,
            };
        }

        private dynamic RetornaDynIntegracaoQualP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP integracaoQualP)
        {
            if (integracaoQualP == null)
                return null;

            return new
            {
                integracaoQualP.Codigo,
                URL = integracaoQualP.UrlIntegracao,
                integracaoQualP.DistanciaMinimaQuadrante,
                integracaoQualP.Token,
                Observacao = integracaoQualP.Observacao1
            };
        }

        private dynamic RetornaDynIntegracaoEFrete(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete)
        {
            if (integracaoEFrete == null)
                return null;

            return new
            {
                integracaoEFrete.Codigo,
                FornecedorValePedagio = integracaoEFrete.FornecedorValePedagio != null ? new { integracaoEFrete.FornecedorValePedagio.Codigo, integracaoEFrete.FornecedorValePedagio.Descricao } : null,
                Cliente = integracaoEFrete.Cliente != null ? new { integracaoEFrete.Cliente.Codigo, integracaoEFrete.Cliente.Descricao } : null,
                integracaoEFrete.URL,
                integracaoEFrete.CodigoIntegrador,
                integracaoEFrete.Usuario,
                integracaoEFrete.Senha,
                integracaoEFrete.DiasPrazo,
                integracaoEFrete.NotificarTransportadorPorEmail,
                integracaoEFrete.EnviarPontosPassagemRotaFrete,
                integracaoEFrete.EnviarPolilinhaRoteirizacaoCarga,
                integracaoEFrete.EnviarTipoVeiculoNaIntegracao,
            };
        }

        private dynamic RetornaDynIntegracaoExtratta(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio integracaoExtratta)
        {
            if (integracaoExtratta == null)
                return null;

            return new
            {
                integracaoExtratta.Codigo,
                FornecedorValePedagio = integracaoExtratta.FornecedorValePedagio != null ? new { integracaoExtratta.FornecedorValePedagio.Codigo, integracaoExtratta.FornecedorValePedagio.Descricao } : null,
                integracaoExtratta.URL,
                integracaoExtratta.Token,
                CNPJAplicacao = integracaoExtratta.CNPJAplicacao.ObterCnpjFormatado(),
                integracaoExtratta.TipoRota,
                integracaoExtratta.FornecedorParceiro
            };
        }

        private dynamic RetornaDynIntegracaoDigitalCom(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalComValePedagio integracaoDigitalCom)
        {
            if (integracaoDigitalCom == null)
                return null;

            return new
            {
                integracaoDigitalCom.Codigo,
                FornecedorValePedagio = integracaoDigitalCom.FornecedorValePedagio != null ? new { integracaoDigitalCom.FornecedorValePedagio.Codigo, integracaoDigitalCom.FornecedorValePedagio.Descricao } : null,
                integracaoDigitalCom.NotificarTransportadorPorEmail,
                integracaoDigitalCom.EnviarNumeroCargaNoCampoDocumentoTransporte
            };
        }

        private dynamic RetornaDynIntegracaoAmbipar(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio integracaoAmbipar)
        {
            if (integracaoAmbipar == null)
                return null;

            return new
            {
                integracaoAmbipar.Codigo,
                FornecedorValePedagio = integracaoAmbipar.FornecedorValePedagio != null ? new { integracaoAmbipar.FornecedorValePedagio.Codigo, integracaoAmbipar.FornecedorValePedagio.Descricao } : null,
                integracaoAmbipar.URL,
                integracaoAmbipar.Usuario,
                integracaoAmbipar.Senha
            };
        }

        private dynamic RetornaDynIntegracaoNDDCargo(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo integracaoNDDCargo)
        {
            if (integracaoNDDCargo == null)
                return null;

            return new
            {
                integracaoNDDCargo.Codigo,
                integracaoNDDCargo.URL,
                integracaoNDDCargo.EnterpriseId,
                integracaoNDDCargo.Token,
                integracaoNDDCargo.Versao
            };
        }

        #endregion
    }
}
