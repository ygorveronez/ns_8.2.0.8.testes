using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "BuscarOcorrenciasMobile" }, "Ocorrencias/TipoOcorrencia", "Ocorrencias/Ocorrencia")]
    public class TipoOcorrenciaController : BaseController
    {
        #region Construtores

        public TipoOcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoDeOcorrenciaDeCTe filtrosPesquisa = ObterFiltrosPesquisa();

                filtrosPesquisa.Finalidades = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoOcorrencia?>>(Request.Params("Finalidades") ?? "[]");
                if (filtrosPesquisa.Finalidades.Count == 1 && filtrosPesquisa.Finalidades[0] == null)
                    filtrosPesquisa.Finalidades = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoOcorrencia?>();

                filtrosPesquisa.ValidarSomenteDisponiveisParaCarga = false;

                if (filtrosPesquisa.CodigoCarga > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga);
                    filtrosPesquisa.ValidarSomenteDisponiveisParaCarga = true;
                    filtrosPesquisa.CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? filtrosPesquisa.CodigoTipoOperacao;

                    if (carga.Pedidos.Count > 0)
                    {
                        Dominio.Entidades.Cliente tomador = carga.Pedidos.First().ObterTomador();
                        if (tomador != null)
                        {
                            filtrosPesquisa.CpfCnpjPessoa = tomador.CPF_CNPJ;

                            if (tomador.GrupoPessoas != null)
                                filtrosPesquisa.CodigoGrupoPessoas = tomador.GrupoPessoas.Codigo;
                        }
                        else if (carga.GrupoPessoaPrincipal != null)
                            filtrosPesquisa.CodigoGrupoPessoas = carga.GrupoPessoaPrincipal.Codigo;
                    }
                }

                filtrosPesquisa.Transportador = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    Dominio.Entidades.Empresa empresaTerceiro = repEmpresa.BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);
                    filtrosPesquisa.Transportador = empresaTerceiro?.Codigo ?? 0;
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    filtrosPesquisa.TipoDocumentoCreditoDebito = TipoDocumentoCreditoDebito.Credito;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("OcorrenciaPorPeriodo", false);
                grid.AdicionarCabecalho("OrigemOcorrencia", false);
                grid.AdicionarCabecalho("TipoEmissaoIntramunicipal", false);
                grid.AdicionarCabecalho("TipoEmissaoDocumentoOcorrencia", false);
                grid.AdicionarCabecalho("TipoOcorrenciaMotivoRejeicao", false);
                grid.AdicionarCabecalho("PeriodoOcorrencia", false);
                grid.AdicionarCabecalho("PermiteInformarSobras", false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.TipoOcorrencia.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.TipoOcorrencia.CodigoProceda, "CodigoProceda", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.TipoOcorrencia.Tipo, "DescricaoTipo", 15, Models.Grid.Align.left, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.TipoOcorrencia.CreditoDebito, "TipoDocumentoCreditoDebito", 15, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("TipoDocumentoCreditoDebito", false);
                if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.TipoOcorrencia.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("CodigoComponenteFrete", false);
                grid.AdicionarCabecalho("DescricaoComponenteFrete", false);
                grid.AdicionarCabecalho("TipoComponenteFrete", false);
                grid.AdicionarCabecalho("OcorrenciaPorNotaFiscal", false);
                grid.AdicionarCabecalho("OcorrenciaParaQuebraRegraPallet", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "DescricaoTipo")
                    propOrdena = "Tipo";
                if (propOrdena == "TipoDocumentoCreditoDebito")
                    propOrdena = "ModeloDocumentoFiscal.TipoDocumentoCreditoDebito";


                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> listaTipoDeOcorrenciaDeCTe = repTipoDeOcorrenciaDeCTe.Consultar(filtrosPesquisa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoDeOcorrenciaDeCTe.ContarConsulta(filtrosPesquisa));
                var lista = (from p in listaTipoDeOcorrenciaDeCTe
                             select new
                             {
                                 p.Codigo,
                                 OcorrenciaPorPeriodo = p.OrigemOcorrencia == OrigemOcorrencia.PorPeriodo,
                                 p.OrigemOcorrencia,
                                 p.PeriodoOcorrencia,
                                 p.TipoEmissaoIntramunicipal,
                                 p.TipoEmissaoDocumentoOcorrencia,
                                 p.Descricao,
                                 p.CodigoProceda,
                                 p.DescricaoAtivo,
                                 p.DescricaoTipo,
                                 TipoDocumentoCreditoDebito = p.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebitoDescricao ?? (p.PermiteInformarValor || p.OcorrenciaPorQuantidade ? "Crédito" : ""),
                                 CodigoComponenteFrete = p.ComponenteFrete != null ? p.ComponenteFrete.Codigo : 0,
                                 DescricaoComponenteFrete = p.ComponenteFrete != null ? p.ComponenteFrete.Descricao : string.Empty,
                                 p.ComponenteFrete?.TipoComponenteFrete,
                                 TipoRateio = p.TipoRateio.HasValue ? p.TipoRateio : ParametroRateioFormula.todos,
                                 p.FiltrarOcorrenciasPeriodoPorFilial,
                                 p.NaoGerarDocumento,
                                 p.GerarApenasUmComplemento,
                                 TipoOcorrenciaMotivoRejeicao = p.UsadoParaMotivoRejeicaoColetaEntrega,
                                 p.PermiteInformarSobras,
                                 p.OcorrenciaPorNotaFiscal,
                                 p.OcorrenciaParaQuebraRegraPallet
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCausasTipoOcorrencia()
        {
            
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas filtrosPesquisa = ObterFiltrosPesquisaCausas();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
               
                Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas repositorioTipoOcorrenciaCausas = new Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> tipoOcorrenciaCausas = repositorioTipoOcorrenciaCausas.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioTipoOcorrenciaCausas.ContarConsulta(filtrosPesquisa));

                var lista = (from p in tipoOcorrenciaCausas
                             select new
                                {
                                    p.Codigo,
                                    p.Descricao,
                                }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarOcorrenciasMobile()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega>("TipoAplicacaoColetaEntrega");

                bool UsadoParaMotivoRejeicaoColetaEntrega = Request.GetBoolParam("UsadoParaMotivoRejeicaoColetaEntrega");
                int codigoCargaEntrega = Request.GetIntParam("Codigo");

                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tipoDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarOcorrenciasEntrega(tipoAplicacaoColetaEntrega, UsadoParaMotivoRejeicaoColetaEntrega, codigoCargaEntrega, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? true : false);

                dynamic retorno = (from obj in tipoDeOcorrenciaDeCTe
                                   select new
                                   {
                                       obj.Codigo,
                                       Descricao = obj.DescricaoCompleta
                                   }).ToList().OrderBy(o => o.Descricao);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.TipoOcorrencia.OcorreuUmaFalhaAoBuscarTiposOcorrencia);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);


                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe = new Dominio.Entidades.TipoDeOcorrenciaDeCTe();

                PreencheEntidade(tipoDeOcorrenciaDeCTe, unitOfWork);

                List<int> codigosCanais = Request.GetListParam<int>("Canais");

                if (tipoDeOcorrenciaDeCTe.CanaisDeEntrega == null)
                    tipoDeOcorrenciaDeCTe.CanaisDeEntrega = new List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();

                tipoDeOcorrenciaDeCTe.CanaisDeEntrega.Clear();

                foreach (int codigoCanal in codigosCanais)
                    tipoDeOcorrenciaDeCTe.CanaisDeEntrega.Add(repCanalEntrega.BuscarPorCodigo(codigoCanal));

                repositorioTipoDeOcorrenciaDeCTe.Inserir(tipoDeOcorrenciaDeCTe, Auditado);

                SalvarTiposIntegracao(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarListaParametros(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarPermissoes(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarFiltrosCarga(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarGatilho(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarCausasTipoOcorrencia(tipoDeOcorrenciaDeCTe, unitOfWork);

                if (tipoDeOcorrenciaDeCTe.UsarMobile)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
                    if (configuracaoIntegracaoTrizy != null && configuracaoIntegracaoTrizy.VersaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoIntegracaoTrizy.Versao3)
                    {
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarTipoOcorrencia(tipoDeOcorrenciaDeCTe, unitOfWork);
                    }
                }

                repositorioTipoDeOcorrenciaDeCTe.Atualizar(tipoDeOcorrenciaDeCTe);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
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

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigo, true);

                if (tipoDeOcorrenciaDeCTe == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencheEntidade(tipoDeOcorrenciaDeCTe, unitOfWork);

                List<int> codigosCanais = Request.GetListParam<int>("Canais");

                if (tipoDeOcorrenciaDeCTe.CanaisDeEntrega == null)
                    tipoDeOcorrenciaDeCTe.CanaisDeEntrega = new List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();

                tipoDeOcorrenciaDeCTe.CanaisDeEntrega.Clear();

                foreach (int codigoCanal in codigosCanais)
                    tipoDeOcorrenciaDeCTe.CanaisDeEntrega.Add(repCanalEntrega.BuscarPorCodigo(codigoCanal));

                repositorioTipoDeOcorrenciaDeCTe.Atualizar(tipoDeOcorrenciaDeCTe, Auditado);

                SalvarTiposIntegracao(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarListaParametros(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarPermissoes(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarFiltrosCarga(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarGatilho(tipoDeOcorrenciaDeCTe, unitOfWork);
                SalvarCausasTipoOcorrencia(tipoDeOcorrenciaDeCTe, unitOfWork);

                if (tipoDeOcorrenciaDeCTe.UsarMobile) 
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repIntegracaoTrizy.BuscarPrimeiroRegistro();
                    if (configuracaoIntegracaoTrizy != null && configuracaoIntegracaoTrizy.VersaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VersaoIntegracaoTrizy.Versao3)
                    {
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarTipoOcorrencia(tipoDeOcorrenciaDeCTe, unitOfWork);
                    }
                }

                repositorioTipoDeOcorrenciaDeCTe.Atualizar(tipoDeOcorrenciaDeCTe);

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
                return new JsonpResult(false, Localization.Resources.Ocorrencias.TipoOcorrencia.OcorreuUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTePerfilAcesso repositorioTipoDeOcorrenciaDeCTePerfilAcesso = new Repositorio.TipoDeOcorrenciaDeCTePerfilAcesso(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.FiltroPeriodo repositorioFiltroPeriodo = new Repositorio.Embarcador.Ocorrencias.FiltroPeriodo(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repositorioOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unitOfWork);

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigo);

                if (tipoDeOcorrenciaDeCTe == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso> perfisAcesso = repositorioTipoDeOcorrenciaDeCTePerfilAcesso.BuscarPorTipoDeOcorrenciaDeCTe(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo> filtrosPeriodos = repositorioFiltroPeriodo.BuscarPorTipoOcorrencia(codigo);
                List<TipoIntegracao> tiposIntegracao = repositorioOcorrenciaTipoIntegracao.BuscarTiposIntegracaoPorTipoOcorrencia(codigo);

                var tipoOcorrenciaRetornar = new
                {
                    tipoDeOcorrenciaDeCTe.Codigo,
                    tipoDeOcorrenciaDeCTe.CodigoProceda,
                    tipoDeOcorrenciaDeCTe.CodigoObservacao,
                    tipoDeOcorrenciaDeCTe.Descricao,
                    tipoDeOcorrenciaDeCTe.PrefixoFaturamentoOutrosModelos,
                    tipoDeOcorrenciaDeCTe.TipoConhecimentoProceda,
                    tipoDeOcorrenciaDeCTe.DescricaoPortal,
                    tipoDeOcorrenciaDeCTe.Tipo,
                    tipoDeOcorrenciaDeCTe.Ativo,
                    tipoDeOcorrenciaDeCTe.TipoEmissaoIntramunicipal,
                    tipoDeOcorrenciaDeCTe.AnexoObrigatorio,
                    tipoDeOcorrenciaDeCTe.OcorrenciaExclusivaParaIntegracao,
                    tipoDeOcorrenciaDeCTe.CalculaValorPorTabelaFrete,
                    tipoDeOcorrenciaDeCTe.BloqueiaOcorrenciaDuplicada,
                    tipoDeOcorrenciaDeCTe.BuscarCSTQuandoDocumentoOrigemIsento,
                    tipoDeOcorrenciaDeCTe.TodosCTesSelecionados,
                    tipoDeOcorrenciaDeCTe.NaoPermiteSelecionarTodosCTes,
                    tipoDeOcorrenciaDeCTe.BloquearOcorrenciaDuplicadaCargaMesmoMDFe,
                    tipoDeOcorrenciaDeCTe.BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado,
                    tipoDeOcorrenciaDeCTe.PermiteInformarAprovadorResponsavel,
                    tipoDeOcorrenciaDeCTe.FinalidadeTipoOcorrencia,
                    tipoDeOcorrenciaDeCTe.Valor,
                    tipoDeOcorrenciaDeCTe.PercentualDoFrete,
                    tipoDeOcorrenciaDeCTe.TipoEmissaoDocumentoOcorrencia,
                    tipoDeOcorrenciaDeCTe.TipoClassificacaoOcorrencia,
                    tipoDeOcorrenciaDeCTe.TipoInclusaoImpostoComplemento,
                    CanaisDeEntrega = tipoDeOcorrenciaDeCTe.CanaisDeEntrega == null ? null : tipoDeOcorrenciaDeCTe.CanaisDeEntrega.Select(x => new { Codigo = x.Codigo, Descricao = x.Descricao }).ToList(),
                    Tomador = tipoDeOcorrenciaDeCTe.TomadorTipoOcorrencia,
                    Emitente = tipoDeOcorrenciaDeCTe.EmitenteTipoOcorrencia,
                    OutroTomador = tipoDeOcorrenciaDeCTe.OutroTomador == null ? null : new { Codigo = tipoDeOcorrenciaDeCTe.OutroTomador.CPF_CNPJ, Descricao = tipoDeOcorrenciaDeCTe.OutroTomador.Nome },
                    OutroEmitente = tipoDeOcorrenciaDeCTe.OutroEmitente == null ? null : new { tipoDeOcorrenciaDeCTe.OutroEmitente.Codigo, Descricao = tipoDeOcorrenciaDeCTe.OutroEmitente.RazaoSocial },
                    ComponenteFrete = tipoDeOcorrenciaDeCTe.ComponenteFrete != null ? new { tipoDeOcorrenciaDeCTe.ComponenteFrete.Codigo, tipoDeOcorrenciaDeCTe.ComponenteFrete.Descricao } : new { Codigo = 0, Descricao = "" },
                    GrupoOcorrencia = tipoDeOcorrenciaDeCTe.GrupoTipoDeOcorrenciaDeCTe != null ? new { tipoDeOcorrenciaDeCTe.GrupoTipoDeOcorrenciaDeCTe.Codigo, tipoDeOcorrenciaDeCTe.GrupoTipoDeOcorrenciaDeCTe.Descricao } : new { Codigo = 0, Descricao = "" },
                    tipoDeOcorrenciaDeCTe.PermiteInformarValor,
                    tipoDeOcorrenciaDeCTe.FiltrarCargasPeriodo,
                    tipoDeOcorrenciaDeCTe.PermiteSelecionarTomador,
                    tipoDeOcorrenciaDeCTe.OrigemOcorrencia,
                    tipoDeOcorrenciaDeCTe.PeriodoOcorrencia,
                    tipoDeOcorrenciaDeCTe.TipoPessoa,
                    tipoDeOcorrenciaDeCTe.TipoCTeIntegracao,
                    tipoDeOcorrenciaDeCTe.OcorrenciaComplementoValorFreteCarga,
                    tipoDeOcorrenciaDeCTe.OcorrenciaComVeiculo,
                    BloquearVisualizacaoTipoOcorrenciaTransportador = tipoDeOcorrenciaDeCTe.BloquearVisualizacaoTipoOcorrenciaTransportador.HasValue ? tipoDeOcorrenciaDeCTe.BloquearVisualizacaoTipoOcorrenciaTransportador : false,
                    tipoDeOcorrenciaDeCTe.UsarMobile,
                    tipoDeOcorrenciaDeCTe.NaoGerarIntegracao,
                    tipoDeOcorrenciaDeCTe.OcorrenciaDestinadaFranquias,
                    tipoDeOcorrenciaDeCTe.OcorrenciaPorQuantidade,
                    tipoDeOcorrenciaDeCTe.OcorrenciaPorPercentualDoFrete,
                    tipoDeOcorrenciaDeCTe.CaracteristicaAdicionalCTe,
                    tipoDeOcorrenciaDeCTe.EntregaRealizada,
                    MotivoChamado = new { Codigo = tipoDeOcorrenciaDeCTe.MotivoChamado?.Codigo ?? 0, Descricao = tipoDeOcorrenciaDeCTe.MotivoChamado?.Descricao ?? "" },
                    tipoDeOcorrenciaDeCTe.TipoOcorrenciaControleEntrega,
                    tipoDeOcorrenciaDeCTe.NaoIndicarAoCliente,
                    tipoDeOcorrenciaDeCTe.NaoAlterarSituacaoColetaEntrega,
                    tipoDeOcorrenciaDeCTe.UsadoParaMotivoRejeicaoColetaEntrega,
                    tipoDeOcorrenciaDeCTe.TipoAplicacaoColetaEntrega,
                    tipoDeOcorrenciaDeCTe.ExigirInformarObservacao,
                    tipoDeOcorrenciaDeCTe.ExigirChamadoParaAbrirOcorrencia,
                    TipoIntegracao = tiposIntegracao.ToList(),
                    TipoRateio = tipoDeOcorrenciaDeCTe.TipoRateio.HasValue ? tipoDeOcorrenciaDeCTe.TipoRateio : ParametroRateioFormula.todos,
                    tipoDeOcorrenciaDeCTe.AdicionarPISCOFINS,
                    tipoDeOcorrenciaDeCTe.AdicionarPISCOFINSBaseCalculoICMS,
                    tipoDeOcorrenciaDeCTe.SomenteCargasFinalizadas,
                    tipoDeOcorrenciaDeCTe.ExigirMotivoDeDevolucao,
                    tipoDeOcorrenciaDeCTe.OcorrenciaPorNotaFiscal,
                    tipoDeOcorrenciaDeCTe.DisponibilizarDocumentosParaNFsManual,
                    tipoDeOcorrenciaDeCTe.PermiteAlterarNumeroDocumentoOcorrencia,
                    tipoDeOcorrenciaDeCTe.ExibirParcelasNaAprovacao,
                    tipoDeOcorrenciaDeCTe.OcorrenciaPorAjudante,
                    tipoDeOcorrenciaDeCTe.UtilizarParcelamentoAutomatico,
                    tipoDeOcorrenciaDeCTe.FiltrarOcorrenciasPeriodoPorFilial,
                    tipoDeOcorrenciaDeCTe.NaoGerarDocumento,
                    tipoDeOcorrenciaDeCTe.DataComplementoIgualDataOcorrencia,
                    tipoDeOcorrenciaDeCTe.GerarApenasUmComplemento,
                    tipoDeOcorrenciaDeCTe.OcorrenciaExclusivaParaCanhotosDigitalizados,
                    tipoDeOcorrenciaDeCTe.QuantidadeReenvioIntegracao,
                    tipoDeOcorrenciaDeCTe.GerarEventoIntegracaoCargaEntregaPorCarga,
                    tipoDeOcorrenciaDeCTe.RecalcularPrevisaoEntregaPendentes,
                    tipoDeOcorrenciaDeCTe.OcorrenciaFinalizaViagem,
                    tipoDeOcorrenciaDeCTe.DescricaoTipoPrevisao,
                    tipoDeOcorrenciaDeCTe.CodigoEventoOcorrenciaPrimeiro,
                    tipoDeOcorrenciaDeCTe.CodigoEventoOcorrenciaSegundo,
                    tipoDeOcorrenciaDeCTe.DataOcorrenciaIgualDataCTeComplementado,
                    tipoDeOcorrenciaDeCTe.BloquearAberturaAtendimentoParaVeiculoEmContratoFrete,
                    tipoDeOcorrenciaDeCTe.NaoPermiteInformarValorDaOcorrenciaAoSelecionarAtendimentos,
                    tipoDeOcorrenciaDeCTe.GerarNFSeParaComplementosTomadorIgualDestinatario,
                    tipoDeOcorrenciaDeCTe.DiasAprovacaoAutomatica,
                    tipoDeOcorrenciaDeCTe.HorasToleranciaEntradaSaidaRaio,
                    tipoDeOcorrenciaDeCTe.InformarMotivoNaAprovacao,
                    tipoDeOcorrenciaDeCTe.GerarOcorrenciaComMesmoValorCTesAnteriores,
                    tipoDeOcorrenciaDeCTe.GerarOcorrenciaComValorGrossPedido,
                    tipoDeOcorrenciaDeCTe.OcorrenciaDiferencaValorFechamento,
                    tipoDeOcorrenciaDeCTe.GerarPedidoDevolucaoAutomaticamente,
                    tipoDeOcorrenciaDeCTe.DisponibilizarPedidoParaNovaIntegracao,
                    tipoDeOcorrenciaDeCTe.PermiteInformarSobras,
                    tipoDeOcorrenciaDeCTe.ApresentarValorPesoDaCarga,
                    tipoDeOcorrenciaDeCTe.ExigirCodigoParaAprovacao,
                    tipoDeOcorrenciaDeCTe.CalcularDistanciaPorCTe,
                    tipoDeOcorrenciaDeCTe.CopiarObservacoesDoCTeDeOrigemAoGerarCTeComplementar,
                    tipoDeOcorrenciaDeCTe.GerarAtendimentoAutomaticamente,
                    tipoDeOcorrenciaDeCTe.GerarOcorrenciaAutomaticamenteRejeicaoMobile,
                    MotivoChamadoGeracaoAutomatica = new { Codigo = tipoDeOcorrenciaDeCTe.MotivoChamadoGeracaoAutomatica?.Codigo ?? 0, Descricao = tipoDeOcorrenciaDeCTe.MotivoChamadoGeracaoAutomatica?.Descricao ?? "" },
                    tipoDeOcorrenciaDeCTe.DebitaFreeTimeCalculoValorOcorrencia,
                    tipoDeOcorrenciaDeCTe.NovaOcorrenciaAguardandoInformacoes,
                    tipoDeOcorrenciaDeCTe.PrazoSolicitacaoOcorrencia,
                    tipoDeOcorrenciaDeCTe.ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes,
                    tipoDeOcorrenciaDeCTe.HorasSemFranquia,
                    tipoDeOcorrenciaDeCTe.NaoPermitirQueTransportadorSelecioneTipoOcorrencia,
                    tipoDeOcorrenciaDeCTe.PermitirConsultarCTesComEsseTipoDeOcorrencia,
                    tipoDeOcorrenciaDeCTe.DescricaoAuxiliar,
                    tipoDeOcorrenciaDeCTe.CodigoIntegracaoAuxiliar,
                    tipoDeOcorrenciaDeCTe.FreeTime,
                    tipoDeOcorrenciaDeCTe.TipoReceita,
                    tipoDeOcorrenciaDeCTe.TipoProposta,
                    tipoDeOcorrenciaDeCTe.TipoEnvio,
                    tipoDeOcorrenciaDeCTe.AtivarEnvioAutomaticoTipoOcorrencia,
                    tipoDeOcorrenciaDeCTe.EssaOcorrenciaGeraOutraOcorrenciaIntegracao,
                    tipoDeOcorrenciaDeCTe.PermitirTransportadorInformarDataInicioFimRaioCarga,
                    tipoDeOcorrenciaDeCTe.TipoIntegracaoComprovei,
                    tipoDeOcorrenciaDeCTe.AtivarIntegracaoComprovei,
                    tipoDeOcorrenciaDeCTe.NaoPermitirValorSuplementoMaiorQueDocumento,
                    tipoDeOcorrenciaDeCTe.CSTFilialEmissora,
                    tipoDeOcorrenciaDeCTe.CSTSubContratada,
                    tipoDeOcorrenciaDeCTe.EmitirDocumentoParaFilialEmissoraComPreCTe,
                    tipoDeOcorrenciaDeCTe.ExibirMotivoUltimaAprovacaoPortalTransportador,
                    tipoDeOcorrenciaDeCTe.SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias,
                    tipoDeOcorrenciaDeCTe.EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao,
                    tipoDeOcorrenciaDeCTe.PermitirSelecionarEssaOcorrenciaNoPortalDoCliente,
                    tipoDeOcorrenciaDeCTe.OcorrenciaParaCobrancaDePedagio,
                    tipoDeOcorrenciaDeCTe.OcorrenciaTerceiros,
                    tipoDeOcorrenciaDeCTe.OcorrenciaParaQuebraRegraPallet,
                    tipoDeOcorrenciaDeCTe.PermitirReabrirOcorrenciaEmCasoDeRejeicao,
                    tipoDeOcorrenciaDeCTe.EfetuarOControleQuilometragem,
                    tipoDeOcorrenciaDeCTe.PermiteInformarCausadorOcorrencia,
                    tipoDeOcorrenciaDeCTe.PermitirInformarGrupoOcorrencia,
                    tipoDeOcorrenciaDeCTe.PermitirEnviarOcorrenciaSemAprovacaoPreCTe,
                    tipoDeOcorrenciaDeCTe.NaoCalcularValorOcorrenciaAutomaticamente,
                    PermitirAlterarDataPrevisaoEntrega = tipoDeOcorrenciaDeCTe?.PermitirAlterarDataPrevisaoEntrega ?? false,
                    InformarProdutoLancamentoOcorrencia = tipoDeOcorrenciaDeCTe?.InformarProdutoLancamentoOcorrencia ?? false,
                    CalcularValorCTEComplementarPeloValorCTESemImposto = tipoDeOcorrenciaDeCTe?.CalcularValorCTEComplementarPeloValorCTESemImposto ?? false,
                    TipoOcorrenciaIntegracao = new
                    {
                        Codigo = tipoDeOcorrenciaDeCTe.TipoOcorrenciaIntegracao?.Codigo ?? 0,
                        Descricao = tipoDeOcorrenciaDeCTe.TipoOcorrenciaIntegracao?.Descricao ?? string.Empty,
                    },
                    JustificativaPadraoAprovacao = new
                    {
                        Codigo = tipoDeOcorrenciaDeCTe.JustificativaPadraoAprovacao?.Codigo ?? 0,
                        Descricao = tipoDeOcorrenciaDeCTe.JustificativaPadraoAprovacao?.Descricao ?? string.Empty,
                    },
                    tipoDeOcorrenciaDeCTe.OcorrenciaProvisionada,
                    tipoDeOcorrenciaDeCTe.UtilizarEntradaSaidaDoRaioCargaEntrega,
                    ModeloDocumentoFiscal = new
                    {
                        Codigo = tipoDeOcorrenciaDeCTe.ModeloDocumentoFiscal?.Codigo ?? 0,
                        Descricao = tipoDeOcorrenciaDeCTe.ModeloDocumentoFiscal?.Descricao ?? string.Empty,
                        TipoDocumentoCreditoDebito = tipoDeOcorrenciaDeCTe.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito ?? TipoDocumentoCreditoDebito.Credito,
                        Abreviacao = tipoDeOcorrenciaDeCTe.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                        TipoDocumentoEmissao = tipoDeOcorrenciaDeCTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao ?? Dominio.Enumeradores.TipoDocumento.Nenhum
                    },
                    Pessoa = new { Codigo = tipoDeOcorrenciaDeCTe.Pessoa?.CPF_CNPJ_SemFormato ?? string.Empty, Descricao = tipoDeOcorrenciaDeCTe.Pessoa != null ? tipoDeOcorrenciaDeCTe.Pessoa.Descricao : string.Empty },
                    GrupoPessoas = new { Codigo = tipoDeOcorrenciaDeCTe.GrupoPessoas?.Codigo ?? 0, Descricao = tipoDeOcorrenciaDeCTe.GrupoPessoas?.Descricao ?? string.Empty },
                    Configuracao = new
                    {
                        tipoDeOcorrenciaDeCTe.EnviarEmailGeracaoOcorrencia,
                        tipoDeOcorrenciaDeCTe.EmailGeracaoOcorrencia,
                    },
                    TipoOperacaoColeta = new
                    {
                        Codigo = tipoDeOcorrenciaDeCTe.TipoOperacaoColeta?.Codigo ?? 0,
                        Descricao = tipoDeOcorrenciaDeCTe.TipoOperacaoColeta?.Descricao ?? string.Empty
                    },
                    TipoOperacaoDevolucao = new
                    {
                        Codigo = tipoDeOcorrenciaDeCTe.TipoOperacaoDevolucao?.Codigo ?? 0,
                        Descricao = tipoDeOcorrenciaDeCTe.TipoOperacaoDevolucao?.Descricao ?? string.Empty
                    },
                    FiltrosPeriodo = (
                        from obj in filtrosPeriodos
                        select new
                        {
                            DescricaoRemetente = obj.Remetente.Descricao,
                            Remetente = obj.Remetente.CPF_CNPJ_SemFormato,
                            DescricaoDestinatario = obj.Destinatario.Descricao,
                            Destinatario = obj.Destinatario.CPF_CNPJ_SemFormato
                        }
                    ).ToList(),
                    Parametros = (
                        from obj in tipoDeOcorrenciaDeCTe.ParametrosOcorrencia
                        orderby obj.Descricao
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.DescricaoParametro,
                            obj.DescricaoParametroFinal,
                            obj.TipoParametro
                        }
                    ).ToList(),
                    PerfisAcesso = (
                        from obj in perfisAcesso
                        select new
                        {
                            obj.PerfilAcesso.Codigo,
                            obj.PerfilAcesso.Descricao
                        }
                    ).ToList(),
                    Gatilho = ObterGatilho(tipoDeOcorrenciaDeCTe, unitOfWork),
                    Notificacao = new
                    {
                        NotificarTransportadorPorEmail = tipoDeOcorrenciaDeCTe.NotificarTransportadorPorEmail,
                        NotificarEmail = tipoDeOcorrenciaDeCTe.NotificarPorEmail,
                        NotificarClientePorEmail = tipoDeOcorrenciaDeCTe.NotificarClientePorEmail,
                        AssuntoEmail = tipoDeOcorrenciaDeCTe.AssuntoEmailNotificacao ?? string.Empty,
                        CorpoEmail = tipoDeOcorrenciaDeCTe.CorpoEmailNotificacao ?? string.Empty,
                        ListaEmail = (
                            from obj in tipoDeOcorrenciaDeCTe.EmailsNotificacao
                            select new
                            {
                                Codigo = obj,
                                Email = obj
                            }
                            ).ToList(),
                    },
                    Causas = ObterCausas(tipoDeOcorrenciaDeCTe, unitOfWork),
                    tipoDeOcorrenciaDeCTe.RemarkSped,
                    tipoDeOcorrenciaDeCTe.IdSuperApp,
                    ChecklistSuperApp = new
                    {
                        Codigo = tipoDeOcorrenciaDeCTe.ChecklistSuperApp?.Codigo ?? 0,
                        Descricao = tipoDeOcorrenciaDeCTe.ChecklistSuperApp?.Descricao ?? string.Empty
                    },

                };

                return new JsonpResult(tipoOcorrenciaRetornar);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigo, true);

                if (tipoDeOcorrenciaDeCTe == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                //Repositorio.TipoDeOcorrenciaDeCTeCanalEntrega repoOcorrenciaCanalEntrega = new Repositorio.TipoDeOcorrenciaDeCTeCanalEntrega(unitOfWork);
                //repoOcorrenciaCanalEntrega.DeletarCanaisDoTipoOcorrencia(tipoDeOcorrenciaDeCTe);

                repTipoDeOcorrenciaDeCTe.Deletar(tipoDeOcorrenciaDeCTe, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.TipoOcorrencia.NaoFoiPossivelExlcuirRegistroPoisMesmoJaPossuiVinculo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.TipoOcorrencia.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarOcorrenciasUtilizadasControleEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarTodasOcorrenciasUltilizadasControleEntregaAtivas();

                var result = (from r in tiposDeOcorrenciaDeCTe
                              select new
                              {
                                  value = r.Codigo,
                                  text = r.Descricao
                              }).ToList();

                return new JsonpResult(new
                {
                    TiposOcorrencia = result
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as ocorrências de controle de entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoDeOcorrenciaDeCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoDeOcorrenciaDeCTe()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo"),
                CodigoGrupoPessoasIgual = Request.GetIntParam("GrupoPessoas"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CpfCnpjPessoaIgual = Request.GetDoubleParam("Pessoa"),
                FiltrarContratoFrete = Request.GetBoolParam("FiltrarContratoFrete"),
                OrigemOcorrencia = Request.GetNullableEnumParam<OrigemOcorrencia>("OrigemOcorrencia"),
                TipoAplicacaoColetaEntrega = Request.GetEnumParam<TipoAplicacaoColetaEntrega>("TipoAplicacaoColetaEntrega"),
                TipoOcorrenciaControleEntrega = Request.GetBoolParam("TipoOcorrenciaControleEntrega"),
                AcessoTerceiro = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros,
                BloquearVisualizacaoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? true : false,
                NaoPermitirQueTransportadorSelecioneTipoOcorrencia = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Request.GetBoolParam("NaoPermitirQueTransportadorSelecioneTipoOcorrencia") : false,
                UsuarioAdministrador = this.Usuario.UsuarioAdministrador,
                CodigoPerfilAcesso = this.Usuario?.PerfilAcesso?.Codigo ?? 0,
                CodigosTipoOperacaoColeta = Request.GetListParam<int>("TipoOperacao"),
                CodigosMotivoChamado = Request.GetListParam<int>("MotivoAtendimento"),
                SomenteOcorrenciaUtilizadaControleEntrega = Request.GetBoolParam("SomenteTiposDeOcorrenciaUtilizadosControleEntrega"),
                SomenteOcorrenciasQueNaoUtilizamControleEntrega = ConfiguracaoEmbarcador.PedidoOcorrenciaColetaEntregaIntegracaoNova,
                NaoUtilizarFlagsControleEntrega = Request.GetBoolParam("NaoUtilizarFlagsControleEntrega"),
                FiltrarApenasOcorrenciasPermitidasNoPortalDoCliente = Request.GetBoolParam("FiltrarApenasOcorrenciasPermitidasNoPortalDoCliente"),
            };
        }

        private dynamic ObterGatilho(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repositorioGatilho = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho = repositorioGatilho.BuscarPorTipoOcorrencia(tipoDeOcorrenciaDeCTe.Codigo);

            if (gatilho == null)
                return new
                {
                    UtilizarGatilhoGeracaoOcorrencia = false
                };

            return new
            {
                gatilho.DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega,
                gatilho.GerarAutomaticamente,
                gatilho.GatilhoFinalFluxoPatio,
                gatilho.GatilhoInicialFluxoPatio,
                gatilho.GatilhoFinalTraking,
                gatilho.GatilhoInicialTraking,
                gatilho.TipoAplicacaoGatilhoTracking,
                gatilho.UtilizarTempoCarregamentoComoHoraMinima,
                gatilho.HorasMinimas,
                Parametro = gatilho.Parametro == null ? null : new
                {
                    gatilho.Parametro.Codigo,
                    gatilho.Parametro.Descricao,
                    gatilho.Parametro.TipoParametro
                },
                gatilho.Tipo,
                UtilizarGatilhoGeracaoOcorrencia = true,
                TipoOperacao = (
                    from obj in gatilho.TiposOperacoes
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }
                ).ToList(),
                Filial = (
                    from obj in gatilho.Filiais
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }
                ).ToList(),
                Transportador = (
                    from obj in gatilho.Transportadores
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }
                ).ToList(),
                gatilho.TipoCobrancaMultimodal,
                gatilho.ValidarDataAgendadaEntrega,
                gatilho.TipoDataAlteracaoGatilho,
                gatilho.NaoPermiteDuplicarOcorrencia,
                gatilho.AtribuirDataOcorrenciaNaDataAgendamentoTransportador
            };
        }

        private dynamic ObterCausas(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas repositorioTipoOcorrenciaCausas = new Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas(unitOfWork);
            List<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> listaTipoOcorrenciaCausas = repositorioTipoOcorrenciaCausas.BuscarPorTipoOcorrencia(tipoDeOcorrenciaDeCTe.Codigo);

            return new
            {
                ListaCausas = (
                            from obj in listaTipoOcorrenciaCausas
                            select new
                            {
                                Codigo = obj.Codigo,
                                Descricao = obj.Descricao,
                                Ativo = (obj.Ativo == true ? "Sim" : "Não"),
                            }
                            ).ToList(),
            };
        }

        private void PreencheEntidade(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repositorioGrupoOcorrencia = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.SuperApp.ChecklistSuperApp repChecklistSuperApp = new Repositorio.Embarcador.SuperApp.ChecklistSuperApp(unitOfWork);

            TipoPessoa tipoPessoa = Request.GetEnumParam<TipoPessoa>("TipoPessoa");
            TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega = Request.GetEnumParam<TipoAplicacaoColetaEntrega>("TipoAplicacaoColetaEntrega");
            TipoEmissaoIntramunicipal tipoEmissaoIntramunicipal = Request.GetEnumParam<TipoEmissaoIntramunicipal>("TipoEmissaoIntramunicipal");
            TomadorTipoOcorrencia tomadorTipoOcorrencia = Request.GetEnumParam<TomadorTipoOcorrencia>("Tomador");
            EmitenteTipoOcorrencia emitenteTipoOcorrencia = Request.GetEnumParam<EmitenteTipoOcorrencia>("Emitente");
            OrigemOcorrencia origemOcorrencia = Request.GetEnumParam<OrigemOcorrencia>("OrigemOcorrencia");
            ParametroRateioFormula tipoRateio = Request.GetEnumParam<ParametroRateioFormula>("TipoRateio");
            FinalidadeTipoOcorrencia? finalidadeTipoOcorrencia = Request.GetNullableEnumParam<FinalidadeTipoOcorrencia>("FinalidadeTipoOcorrencia");
            PeriodoAcordoContratoFreteTransportador periodoOcorrencia = Request.GetEnumParam("PeriodoOcorrencia", PeriodoAcordoContratoFreteTransportador.Quinzenal);
            TipoEnvioIntegracaoNeokohm? tipoEnvio = Request.GetNullableEnumParam<TipoEnvioIntegracaoNeokohm>("TipoEnvio");
            TipoIntegracaoComprovei? tipoIntegracaoComprovei = Request.GetNullableEnumParam<TipoIntegracaoComprovei>("TipoIntegracaoComprovei");
            TipoPropostaOcorrencia? tipoProposta = Request.GetNullableEnumParam<TipoPropostaOcorrencia>("TipoProposta");
            TipoReceita? tipoReceita = Request.GetNullableEnumParam<TipoReceita>("TipoReceita");

            double cpfCnpjCliente = Request.GetDoubleParam("Pessoa");
            double outroTomador = Request.GetDoubleParam("OutroTomador");

            int outroEmitente = Request.GetIntParam("OutroEmitente");
            int componenteFrete = Request.GetIntParam("ComponenteFrete");
            int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");
            int modeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal");
            int motivoChamado = Request.GetIntParam("MotivoChamado");
            int motivoChamadoGeracaoAutomatica = Request.GetIntParam("MotivoChamadoGeracaoAutomatica");
            int codigoGrupoOcorrencia = Request.GetIntParam("GrupoOcorrencia");
            int codigoTipoOcorrenciaIntegracao = Request.GetIntParam("TipoOcorrenciaIntegracao");

            bool ativo = Request.GetBoolParam("Ativo");
            bool permiteInformarValor = Request.GetBoolParam("PermiteInformarValor");
            bool anexoObrigatorio = Request.GetBoolParam("AnexoObrigatorio");
            bool ocorrenciaExclusivaParaIntegracao = Request.GetBoolParam("OcorrenciaExclusivaParaIntegracao");
            bool calculaValorPorTabelaFrete = Request.GetBoolParam("CalculaValorPorTabelaFrete");
            bool filtrarCargasPeriodo = Request.GetBoolParam("FiltrarCargasPeriodo");
            bool permiteSelecionarTomador = Request.GetBoolParam("PermiteSelecionarTomador");
            bool bloqueiaOcorrenciaDuplicada = Request.GetBoolParam("BloqueiaOcorrenciaDuplicada");

            bool buscarCSTQuandoDocumentoOrigemIsento = Request.GetBoolParam("BuscarCSTQuandoDocumentoOrigemIsento");
            bool todosCTesSelecionados = Request.GetBoolParam("TodosCTesSelecionados");
            bool naoPermiteSelecionarTodosCTes = Request.GetBoolParam("NaoPermiteSelecionarTodosCTes");
            bool ocorrenciaComplementoValorFreteCarga = Request.GetBoolParam("OcorrenciaComplementoValorFreteCarga");
            bool ocorrenciaComVeiculo = Request.GetBoolParam("OcorrenciaComVeiculo");
            bool usarMobile = Request.GetBoolParam("UsarMobile");
            bool naoGerarIntegracao = Request.GetBoolParam("NaoGerarIntegracao");
            bool ocorrenciaDestinadaFranquias = Request.GetBoolParam("OcorrenciaDestinadaFranquias");
            bool exigirInformarObservacao = Request.GetBoolParam("ExigirInformarObservacao");
            bool exibirParcelasNaAprovacao = Request.GetBoolParam("ExibirParcelasNaAprovacao");
            bool ocorrenciaPorAjudante = Request.GetBoolParam("OcorrenciaPorAjudante");
            bool utilizarParcelamentoAutomatico = Request.GetBoolParam("UtilizarParcelamentoAutomatico");
            bool filtrarOcorrenciasPeriodoPorFilial = Request.GetBoolParam("FiltrarOcorrenciasPeriodoPorFilial");
            bool naoGerarDocumento = Request.GetBoolParam("NaoGerarDocumento");
            bool gerarApenasUmComplemento = Request.GetBoolParam("GerarApenasUmComplemento");
            bool bloquearOcorrenciaDupliacadaMesmoMDFe = Request.GetBoolParam("BloquearOcorrenciaDuplicadaCargaMesmoMDFe");
            bool bloquearOcorrenciaCargaCanhotoDigitalizadoAprovado = Request.GetBoolParam("BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado");
            bool ocorrenciaExclusivaParaCanhotosDigitalizados = Request.GetBoolParam("OcorrenciaExclusivaParaCanhotosDigitalizados");
            bool entregaRealizada = Request.GetBoolParam("EntregaRealizada");
            bool permiteInformarAprovadorResponsavel = Request.GetBoolParam("PermiteInformarAprovadorResponsavel");
            bool TipoOcorrenciaControleEntrega = Request.GetBoolParam("TipoOcorrenciaControleEntrega");
            bool NaoIndicarAoCliente = Request.GetBoolParam("NaoIndicarAoCliente");
            bool NaoAlterarSituacaoColetaEntrega = Request.GetBoolParam("NaoAlterarSituacaoColetaEntrega");
            bool UsadoParaMotivoRejeicaoColetaEntrega = Request.GetBoolParam("UsadoParaMotivoRejeicaoColetaEntrega");
            bool gerarPedidoDevolucaoAutomaticamente = Request.GetBoolParam("GerarPedidoDevolucaoAutomaticamente");
            bool disponibilizarPedidoParaNovaIntegracao = Request.GetBoolParam("DisponibilizarPedidoParaNovaIntegracao");
            bool permiteInformarSobras = Request.GetBoolParam("PermiteInformarSobras");
            bool ApresentarValorPesoDaCarga = Request.GetBoolParam("ApresentarValorPesoDaCarga");
            bool exigirCodigoParaAprovacao = Request.GetBoolParam("ExigirCodigoParaAprovacao");
            bool DebitaFreeTimeCalculoValorOcorrencia = Request.GetBoolParam("DebitaFreeTimeCalculoValorOcorrencia");
            bool novaOcorrenciaAguardandoInformacoes = Request.GetBoolParam("NovaOcorrenciaAguardandoInformacoes");
            bool ativarIntegracaoComprovei = Request.GetBoolParam("AtivarIntegracaoComprovei");
            bool calcularDistanciaPorCTe = Request.GetBoolParam("CalcularDistanciaPorCTe");
            bool naoPermitirValorSuplementoMaiorQueDocumento = Request.GetBoolParam("NaoPermitirValorSuplementoMaiorQueDocumento");
            bool informarProdutoLancamentoOcorrencia = Request.GetBoolParam("InformarProdutoLancamentoOcorrencia");
            bool permitirReabrirOcorrenciaEmCasoDeRejeicao = Request.GetBoolParam("PermitirReabrirOcorrenciaEmCasoDeRejeicao");

            string tipoCTeIntegracao = Request.Params("TipoCTeIntegracao");
            string caracteristicaAdicionalCTe = Request.Params("CaracteristicaAdicionalCTe");
            string descricaoPortal = Request.GetStringParam("DescricaoPortal");

            int checklistSuperAppId = Request.GetIntParam("ChecklistSuperApp");

            tipoDeOcorrenciaDeCTe.TipoOcorrenciaControleEntrega = TipoOcorrenciaControleEntrega;
            tipoDeOcorrenciaDeCTe.NaoIndicarAoCliente = NaoIndicarAoCliente;
            tipoDeOcorrenciaDeCTe.NaoAlterarSituacaoColetaEntrega = NaoAlterarSituacaoColetaEntrega;
            tipoDeOcorrenciaDeCTe.UsadoParaMotivoRejeicaoColetaEntrega = UsadoParaMotivoRejeicaoColetaEntrega;
            tipoDeOcorrenciaDeCTe.TipoAplicacaoColetaEntrega = tipoAplicacaoColetaEntrega;
            tipoDeOcorrenciaDeCTe.MotivoChamado = motivoChamado > 0 ? repMotivoChamado.BuscarPorCodigo(motivoChamado) : null;
            tipoDeOcorrenciaDeCTe.EntregaRealizada = entregaRealizada;
            tipoDeOcorrenciaDeCTe.Ativo = ativo;
            tipoDeOcorrenciaDeCTe.TipoEmissaoIntramunicipal = tipoEmissaoIntramunicipal;
            tipoDeOcorrenciaDeCTe.TomadorTipoOcorrencia = tomadorTipoOcorrencia;
            tipoDeOcorrenciaDeCTe.EmitenteTipoOcorrencia = emitenteTipoOcorrencia;
            tipoDeOcorrenciaDeCTe.FinalidadeTipoOcorrencia = finalidadeTipoOcorrencia;
            tipoDeOcorrenciaDeCTe.OutroTomador = (tomadorTipoOcorrencia == TomadorTipoOcorrencia.Outros && outroTomador > 0d) ? repCliente.BuscarPorCPFCNPJ(outroTomador) : null;
            tipoDeOcorrenciaDeCTe.OutroEmitente = (emitenteTipoOcorrencia == EmitenteTipoOcorrencia.Outros && outroEmitente > 0) ? repEmpresa.BuscarPorCodigo(outroEmitente) : null;
            tipoDeOcorrenciaDeCTe.Descricao = Request.Params("Descricao") ?? string.Empty;
            tipoDeOcorrenciaDeCTe.DescricaoPortal = descricaoPortal.Substring(0, Math.Min(descricaoPortal.Length, 150));
            tipoDeOcorrenciaDeCTe.CodigoProceda = Request.Params("CodigoProceda") ?? string.Empty;
            tipoDeOcorrenciaDeCTe.TipoEmissaoDocumentoOcorrencia = Request.GetEnumParam<TipoEmissaoDocumentoOcorrencia>("TipoEmissaoDocumentoOcorrencia");
            tipoDeOcorrenciaDeCTe.TipoInclusaoImpostoComplemento = Request.GetEnumParam<TipoInclusaoImpostoComplemento>("TipoInclusaoImpostoComplemento");
            tipoDeOcorrenciaDeCTe.TipoClassificacaoOcorrencia = Request.GetEnumParam<ClassificacaoOcorrencia>("TipoClassificacaoOcorrencia");
            tipoDeOcorrenciaDeCTe.GerarPedidoDevolucaoAutomaticamente = gerarPedidoDevolucaoAutomaticamente;
            tipoDeOcorrenciaDeCTe.DisponibilizarPedidoParaNovaIntegracao = disponibilizarPedidoParaNovaIntegracao;
            tipoDeOcorrenciaDeCTe.PermiteInformarSobras = permiteInformarSobras;
            tipoDeOcorrenciaDeCTe.ApresentarValorPesoDaCarga = ApresentarValorPesoDaCarga;
            tipoDeOcorrenciaDeCTe.DebitaFreeTimeCalculoValorOcorrencia = DebitaFreeTimeCalculoValorOcorrencia;
            tipoDeOcorrenciaDeCTe.NovaOcorrenciaAguardandoInformacoes = novaOcorrenciaAguardandoInformacoes;
            tipoDeOcorrenciaDeCTe.AtivarIntegracaoComprovei = ativarIntegracaoComprovei;
            tipoDeOcorrenciaDeCTe.ExigirCodigoParaAprovacao = exigirCodigoParaAprovacao;
            tipoDeOcorrenciaDeCTe.CalcularDistanciaPorCTe = calcularDistanciaPorCTe;
            tipoDeOcorrenciaDeCTe.NaoPermitirValorSuplementoMaiorQueDocumento = naoPermitirValorSuplementoMaiorQueDocumento;
            tipoDeOcorrenciaDeCTe.InformarProdutoLancamentoOcorrencia = informarProdutoLancamentoOcorrencia;
            tipoDeOcorrenciaDeCTe.CopiarObservacoesDoCTeDeOrigemAoGerarCTeComplementar = Request.GetBoolParam("CopiarObservacoesDoCTeDeOrigemAoGerarCTeComplementar");
            tipoDeOcorrenciaDeCTe.GerarAtendimentoAutomaticamente = Request.GetBoolParam("GerarAtendimentoAutomaticamente");
            tipoDeOcorrenciaDeCTe.MotivoChamadoGeracaoAutomatica = motivoChamadoGeracaoAutomatica > 0 ? repMotivoChamado.BuscarPorCodigo(motivoChamadoGeracaoAutomatica) : null;
            tipoDeOcorrenciaDeCTe.PrefixoFaturamentoOutrosModelos = Request.GetStringParam("PrefixoFaturamentoOutrosModelos");
            tipoDeOcorrenciaDeCTe.TipoConhecimentoProceda = Request.GetStringParam("TipoConhecimentoProceda");
            tipoDeOcorrenciaDeCTe.GerarOcorrenciaAutomaticamenteRejeicaoMobile = Request.GetBoolParam("GerarOcorrenciaAutomaticamenteRejeicaoMobile");
            tipoDeOcorrenciaDeCTe.PermitirAlterarDataPrevisaoEntrega = Request.GetBoolParam("PermitirAlterarDataPrevisaoEntrega");
            tipoDeOcorrenciaDeCTe.EmitirDocumentoParaFilialEmissoraComPreCTe = Request.GetBoolParam("EmitirDocumentoParaFilialEmissoraComPreCTe");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                tipoDeOcorrenciaDeCTe.CodigoIntegracao = tipoDeOcorrenciaDeCTe.CodigoProceda;

            tipoDeOcorrenciaDeCTe.Tipo = Request.Params("Tipo") ?? string.Empty;
            tipoDeOcorrenciaDeCTe.PermiteInformarValor = permiteInformarValor;
            tipoDeOcorrenciaDeCTe.AnexoObrigatorio = anexoObrigatorio;
            tipoDeOcorrenciaDeCTe.OcorrenciaExclusivaParaIntegracao = ocorrenciaExclusivaParaIntegracao;
            tipoDeOcorrenciaDeCTe.CalculaValorPorTabelaFrete = calculaValorPorTabelaFrete;
            tipoDeOcorrenciaDeCTe.BloqueiaOcorrenciaDuplicada = bloqueiaOcorrenciaDuplicada;
            tipoDeOcorrenciaDeCTe.BuscarCSTQuandoDocumentoOrigemIsento = buscarCSTQuandoDocumentoOrigemIsento;
            tipoDeOcorrenciaDeCTe.TodosCTesSelecionados = todosCTesSelecionados;
            tipoDeOcorrenciaDeCTe.NaoPermiteSelecionarTodosCTes = naoPermiteSelecionarTodosCTes;
            tipoDeOcorrenciaDeCTe.BloquearOcorrenciaDuplicadaCargaMesmoMDFe = bloquearOcorrenciaDupliacadaMesmoMDFe;
            tipoDeOcorrenciaDeCTe.BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado = bloquearOcorrenciaCargaCanhotoDigitalizadoAprovado;
            tipoDeOcorrenciaDeCTe.FiltrarCargasPeriodo = filtrarCargasPeriodo;
            tipoDeOcorrenciaDeCTe.OrigemOcorrencia = origemOcorrencia;
            tipoDeOcorrenciaDeCTe.PeriodoOcorrencia = periodoOcorrencia;
            tipoDeOcorrenciaDeCTe.PermiteSelecionarTomador = permiteSelecionarTomador;
            tipoDeOcorrenciaDeCTe.GrupoPessoas = tipoPessoa == TipoPessoa.GrupoPessoa && codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null;
            tipoDeOcorrenciaDeCTe.Pessoa = tipoPessoa == TipoPessoa.Pessoa && cpfCnpjCliente > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente) : null;
            tipoDeOcorrenciaDeCTe.TipoPessoa = tipoPessoa;
            tipoDeOcorrenciaDeCTe.TipoCTeIntegracao = tipoCTeIntegracao;
            tipoDeOcorrenciaDeCTe.OcorrenciaComplementoValorFreteCarga = ocorrenciaComplementoValorFreteCarga;
            tipoDeOcorrenciaDeCTe.OcorrenciaComVeiculo = ocorrenciaComVeiculo;
            tipoDeOcorrenciaDeCTe.UsarMobile = usarMobile;
            tipoDeOcorrenciaDeCTe.Valor = Request.GetDecimalParam("Valor");
            tipoDeOcorrenciaDeCTe.PercentualDoFrete = Request.GetDecimalParam("PercentualDoFrete");
            tipoDeOcorrenciaDeCTe.NaoGerarIntegracao = naoGerarIntegracao;
            tipoDeOcorrenciaDeCTe.OcorrenciaDestinadaFranquias = ocorrenciaDestinadaFranquias;
            tipoDeOcorrenciaDeCTe.OcorrenciaPorQuantidade = Request.GetBoolParam("OcorrenciaPorQuantidade");
            tipoDeOcorrenciaDeCTe.OcorrenciaPorPercentualDoFrete = Request.GetBoolParam("OcorrenciaPorPercentualDoFrete");
            tipoDeOcorrenciaDeCTe.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(modeloDocumentoFiscal);
            tipoDeOcorrenciaDeCTe.CaracteristicaAdicionalCTe = caracteristicaAdicionalCTe;
            tipoDeOcorrenciaDeCTe.PermiteInformarAprovadorResponsavel = permiteInformarAprovadorResponsavel;
            tipoDeOcorrenciaDeCTe.ExigirInformarObservacao = exigirInformarObservacao;
            tipoDeOcorrenciaDeCTe.EnviarEmailGeracaoOcorrencia = Request.GetBoolParam("EnviarEmailGeracaoOcorrencia");
            tipoDeOcorrenciaDeCTe.EmailGeracaoOcorrencia = Request.GetStringParam("EmailGeracaoOcorrencia");
            tipoDeOcorrenciaDeCTe.CodigoObservacao = Request.GetStringParam("CodigoObservacao");
            tipoDeOcorrenciaDeCTe.ExigirChamadoParaAbrirOcorrencia = Request.GetBoolParam("ExigirChamadoParaAbrirOcorrencia");
            tipoDeOcorrenciaDeCTe.AdicionarPISCOFINS = Request.GetBoolParam("AdicionarPISCOFINS");
            tipoDeOcorrenciaDeCTe.AdicionarPISCOFINSBaseCalculoICMS = Request.GetBoolParam("AdicionarPISCOFINSBaseCalculoICMS");
            tipoDeOcorrenciaDeCTe.SomenteCargasFinalizadas = Request.GetBoolParam("SomenteCargasFinalizadas");
            tipoDeOcorrenciaDeCTe.ExigirMotivoDeDevolucao = Request.GetBoolParam("ExigirMotivoDeDevolucao");
            tipoDeOcorrenciaDeCTe.OcorrenciaTerceiros = Request.GetBoolParam("OcorrenciaTerceiros");
            tipoDeOcorrenciaDeCTe.OcorrenciaPorNotaFiscal = Request.GetBoolParam("OcorrenciaPorNotaFiscal");
            tipoDeOcorrenciaDeCTe.DisponibilizarDocumentosParaNFsManual = Request.GetBoolParam("DisponibilizarDocumentosParaNFsManual");
            tipoDeOcorrenciaDeCTe.PermiteAlterarNumeroDocumentoOcorrencia = Request.GetBoolParam("PermiteAlterarNumeroDocumentoOcorrencia");
            tipoDeOcorrenciaDeCTe.QuantidadeReenvioIntegracao = Request.GetIntParam("QuantidadeReenvioIntegracao");
            tipoDeOcorrenciaDeCTe.GerarEventoIntegracaoCargaEntregaPorCarga = Request.GetBoolParam("GerarEventoIntegracaoCargaEntregaPorCarga");
            tipoDeOcorrenciaDeCTe.RecalcularPrevisaoEntregaPendentes = Request.GetBoolParam("RecalcularPrevisaoEntregaPendentes");
            tipoDeOcorrenciaDeCTe.OcorrenciaFinalizaViagem = Request.GetBoolParam("OcorrenciaFinalizaViagem");
            tipoDeOcorrenciaDeCTe.DescricaoTipoPrevisao = Request.GetStringParam("DescricaoTipoPrevisao");
            tipoDeOcorrenciaDeCTe.CodigoEventoOcorrenciaPrimeiro = Request.GetStringParam("CodigoEventoOcorrenciaPrimeiro");
            tipoDeOcorrenciaDeCTe.CodigoEventoOcorrenciaSegundo = Request.GetStringParam("CodigoEventoOcorrenciaSegundo");
            tipoDeOcorrenciaDeCTe.TipoRateio = tipoRateio;
            tipoDeOcorrenciaDeCTe.ExibirParcelasNaAprovacao = exibirParcelasNaAprovacao;
            tipoDeOcorrenciaDeCTe.OcorrenciaPorAjudante = ocorrenciaPorAjudante;
            tipoDeOcorrenciaDeCTe.UtilizarParcelamentoAutomatico = utilizarParcelamentoAutomatico;
            tipoDeOcorrenciaDeCTe.FiltrarOcorrenciasPeriodoPorFilial = filtrarOcorrenciasPeriodoPorFilial;
            tipoDeOcorrenciaDeCTe.NaoGerarDocumento = naoGerarDocumento;
            tipoDeOcorrenciaDeCTe.DataComplementoIgualDataOcorrencia = Request.GetBoolParam("DataComplementoIgualDataOcorrencia");
            tipoDeOcorrenciaDeCTe.GerarApenasUmComplemento = gerarApenasUmComplemento;
            tipoDeOcorrenciaDeCTe.OcorrenciaExclusivaParaCanhotosDigitalizados = ocorrenciaExclusivaParaCanhotosDigitalizados;
            tipoDeOcorrenciaDeCTe.TipoOperacaoColeta = TipoOcorrenciaControleEntrega ? repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoColeta")) : null;
            tipoDeOcorrenciaDeCTe.DataOcorrenciaIgualDataCTeComplementado = Request.GetBoolParam("DataOcorrenciaIgualDataCTeComplementado");
            tipoDeOcorrenciaDeCTe.BloquearAberturaAtendimentoParaVeiculoEmContratoFrete = Request.GetBoolParam("BloquearAberturaAtendimentoParaVeiculoEmContratoFrete");
            tipoDeOcorrenciaDeCTe.NaoPermiteInformarValorDaOcorrenciaAoSelecionarAtendimentos = Request.GetBoolParam("NaoPermiteInformarValorDaOcorrenciaAoSelecionarAtendimentos");
            tipoDeOcorrenciaDeCTe.GerarNFSeParaComplementosTomadorIgualDestinatario = Request.GetBoolParam("GerarNFSeParaComplementosTomadorIgualDestinatario");
            tipoDeOcorrenciaDeCTe.DiasAprovacaoAutomatica = Request.GetIntParam("DiasAprovacaoAutomatica");
            tipoDeOcorrenciaDeCTe.HorasToleranciaEntradaSaidaRaio = Request.GetIntParam("HorasToleranciaEntradaSaidaRaio");
            tipoDeOcorrenciaDeCTe.InformarMotivoNaAprovacao = Request.GetBoolParam("InformarMotivoNaAprovacao");
            tipoDeOcorrenciaDeCTe.OcorrenciaProvisionada = Request.GetBoolParam("OcorrenciaProvisionada");
            tipoDeOcorrenciaDeCTe.UtilizarEntradaSaidaDoRaioCargaEntrega = Request.GetBoolParam("UtilizarEntradaSaidaDoRaioCargaEntrega");
            tipoDeOcorrenciaDeCTe.TipoOperacaoDevolucao = gerarPedidoDevolucaoAutomaticamente && TipoOcorrenciaControleEntrega && motivoChamado > 0 ? repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoDevolucao")) : null;
            tipoDeOcorrenciaDeCTe.JustificativaPadraoAprovacao = tipoDeOcorrenciaDeCTe.InformarMotivoNaAprovacao ? repMotivoRejeicaoOcorrencia.BuscarPorCodigo(Request.GetIntParam("JustificativaPadraoAprovacao")) : null;
            tipoDeOcorrenciaDeCTe.ComponenteFrete = componenteFrete > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFrete) : null;
            tipoDeOcorrenciaDeCTe.GerarOcorrenciaComMesmoValorCTesAnteriores = Request.GetBoolParam("GerarOcorrenciaComMesmoValorCTesAnteriores");
            tipoDeOcorrenciaDeCTe.GerarOcorrenciaComValorGrossPedido = Request.GetBoolParam("GerarOcorrenciaComValorGrossPedido");
            tipoDeOcorrenciaDeCTe.OcorrenciaDiferencaValorFechamento = Request.GetBoolParam("OcorrenciaDiferencaValorFechamento");
            tipoDeOcorrenciaDeCTe.ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes = Request.GetBoolParam("ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes");
            tipoDeOcorrenciaDeCTe.PrazoSolicitacaoOcorrencia = Request.GetIntParam("PrazoSolicitacaoOcorrencia");
            tipoDeOcorrenciaDeCTe.GrupoTipoDeOcorrenciaDeCTe = codigoGrupoOcorrencia > 0 ? repositorioGrupoOcorrencia.BuscarPorCodigo(codigoGrupoOcorrencia, false) : null;
            tipoDeOcorrenciaDeCTe.HorasSemFranquia = Request.GetIntParam("HorasSemFranquia");
            tipoDeOcorrenciaDeCTe.DescricaoAuxiliar = Request.GetStringParam("DescricaoAuxiliar");
            tipoDeOcorrenciaDeCTe.CodigoIntegracaoAuxiliar = Request.GetStringParam("CodigoIntegracaoAuxiliar");
            tipoDeOcorrenciaDeCTe.FreeTime = Request.GetIntParam("FreeTime");
            tipoDeOcorrenciaDeCTe.TipoProposta = tipoProposta;
            tipoDeOcorrenciaDeCTe.TipoReceita = tipoReceita;
            tipoDeOcorrenciaDeCTe.TipoEnvio = tipoEnvio;
            tipoDeOcorrenciaDeCTe.TipoIntegracaoComprovei = tipoIntegracaoComprovei;
            tipoDeOcorrenciaDeCTe.AtivarEnvioAutomaticoTipoOcorrencia = Request.GetBoolParam("AtivarEnvioAutomaticoTipoOcorrencia");
            tipoDeOcorrenciaDeCTe.EssaOcorrenciaGeraOutraOcorrenciaIntegracao = Request.GetBoolParam("EssaOcorrenciaGeraOutraOcorrenciaIntegracao");
            tipoDeOcorrenciaDeCTe.ExibirMotivoUltimaAprovacaoPortalTransportador = Request.GetBoolParam("ExibirMotivoUltimaAprovacaoPortalTransportador");
            tipoDeOcorrenciaDeCTe.EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao = Request.GetBoolParam("EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao");
            tipoDeOcorrenciaDeCTe.SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias = Request.GetBoolParam("SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias");
            tipoDeOcorrenciaDeCTe.EfetuarOControleQuilometragem = Request.GetBoolParam("EfetuarOControleQuilometragem");
            tipoDeOcorrenciaDeCTe.PermitirSelecionarEssaOcorrenciaNoPortalDoCliente = Request.GetBoolParam("PermitirSelecionarEssaOcorrenciaNoPortalDoCliente");
            tipoDeOcorrenciaDeCTe.OcorrenciaParaCobrancaDePedagio = Request.GetBoolParam("OcorrenciaParaCobrancaDePedagio");
            tipoDeOcorrenciaDeCTe.OcorrenciaParaQuebraRegraPallet = Request.GetBoolParam("OcorrenciaParaQuebraRegraPallet");
            tipoDeOcorrenciaDeCTe.TipoOcorrenciaIntegracao = codigoTipoOcorrenciaIntegracao > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrenciaIntegracao) : null;
            tipoDeOcorrenciaDeCTe.TipoOcorrenciaIntegracao = codigoTipoOcorrenciaIntegracao > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoTipoOcorrenciaIntegracao) : null;
            tipoDeOcorrenciaDeCTe.CSTFilialEmissora = Request.GetStringParam("CSTFilialEmissora");
            tipoDeOcorrenciaDeCTe.CSTSubContratada = Request.GetStringParam("CSTSubContratada");
            tipoDeOcorrenciaDeCTe.PermitirReabrirOcorrenciaEmCasoDeRejeicao = permitirReabrirOcorrenciaEmCasoDeRejeicao;
            tipoDeOcorrenciaDeCTe.PermiteInformarCausadorOcorrencia = Request.GetBoolParam("PermiteInformarCausadorOcorrencia");
            tipoDeOcorrenciaDeCTe.PermitirInformarGrupoOcorrencia = Request.GetBoolParam("PermitirInformarGrupoOcorrencia");
            tipoDeOcorrenciaDeCTe.PermitirEnviarOcorrenciaSemAprovacaoPreCTe = Request.GetBoolParam("PermitirEnviarOcorrenciaSemAprovacaoPreCTe");
            tipoDeOcorrenciaDeCTe.NaoCalcularValorOcorrenciaAutomaticamente = Request.GetBoolParam("NaoCalcularValorOcorrenciaAutomaticamente");
            tipoDeOcorrenciaDeCTe.CalcularValorCTEComplementarPeloValorCTESemImposto = Request.GetBoolParam("CalcularValorCTEComplementarPeloValorCTESemImposto");
            tipoDeOcorrenciaDeCTe.RemarkSped = Request.GetEnumParam<RemarkSped>("RemarkSped", RemarkSped.OutrosServicos);
            tipoDeOcorrenciaDeCTe.ChecklistSuperApp = checklistSuperAppId > 0 ? repChecklistSuperApp.BuscarPorCodigo(checklistSuperAppId, false) : null;

            SalvarNotificacao(tipoDeOcorrenciaDeCTe, unitOfWork);
        }

        private void SalvarFiltrosCarga(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.FiltroPeriodo repFiltroPeriodo = new Repositorio.Embarcador.Ocorrencias.FiltroPeriodo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo> listaFiltroPeriodo = repFiltroPeriodo.BuscarPorTipoOcorrencia(tipoDeOcorrenciaDeCTe.Codigo);
            foreach (Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo excluirFiltro in listaFiltroPeriodo)
                repFiltroPeriodo.Deletar(excluirFiltro);

            dynamic filtrosPeriodo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FiltrosPeriodo"));
            if (filtrosPeriodo != null)
            {
                foreach (dynamic filtro in filtrosPeriodo)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo filtroPeriodo = new Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo()
                    {
                        Remetente = repCliente.BuscarPorCPFCNPJ((double)filtro.Remetente),
                        Destinatario = repCliente.BuscarPorCPFCNPJ((double)filtro.Destinatario),
                        TipoOcorrencia = tipoDeOcorrenciaDeCTe
                    };
                    repFiltroPeriodo.Inserir(filtroPeriodo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoDeOcorrenciaDeCTe, null, string.Format(Localization.Resources.Ocorrencias.TipoOcorrencia.AdicionouFiltro, filtroPeriodo.Remetente.Nome, filtroPeriodo.Destinatario.Nome), unitOfWork);
                }
            }
        }

        private void SalvarGatilho(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repositorioGatilho = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho = repositorioGatilho.BuscarPorTipoOcorrencia(tipoDeOcorrenciaDeCTe.Codigo);
            dynamic gatilhoSalvar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Gatilho"));
            bool utilizarGatilhoGeracaoOcorrencia = ((string)gatilhoSalvar.UtilizarGatilhoGeracaoOcorrencia).ToBool();

            if (utilizarGatilhoGeracaoOcorrencia)
            {
                Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repositorioParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);

                if (gatilho == null)
                    gatilho = new Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia()
                    {
                        TipoOcorrencia = tipoDeOcorrenciaDeCTe
                    };

                gatilho.DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega = ((string)gatilhoSalvar.DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega).ToBool();
                gatilho.UtilizarTempoCarregamentoComoHoraMinima = ((string)gatilhoSalvar.UtilizarTempoCarregamentoComoHoraMinima).ToBool();
                gatilho.GerarAutomaticamente = ((string)gatilhoSalvar.GerarAutomaticamente).ToBool();
                gatilho.HorasMinimas = ((string)gatilhoSalvar.HorasMinimas).ToInt();
                gatilho.Parametro = repositorioParametroOcorrencia.BuscarPorCodigo(((string)gatilhoSalvar.Parametro).ToInt());
                gatilho.Tipo = ((string)gatilhoSalvar.Tipo).ToEnum<TipoGatilhoOcorrencia>();
                gatilho.TipoCobrancaMultimodal = ((string)gatilhoSalvar.TipoCobrancaMultimodal).ToEnum<TipoCobrancaMultimodal>();
                gatilho.NaoPermiteDuplicarOcorrencia = ((string)gatilhoSalvar.NaoPermiteDuplicarOcorrencia).ToBool();
                gatilho.AtribuirDataOcorrenciaNaDataAgendamentoTransportador = ((string)gatilhoSalvar.AtribuirDataOcorrenciaNaDataAgendamentoTransportador).ToBool();

                gatilho.GatilhoFinalFluxoPatio = null;
                gatilho.GatilhoInicialFluxoPatio = null;
                gatilho.GatilhoFinalTraking = null;
                gatilho.GatilhoInicialTraking = null;
                gatilho.ValidarDataAgendadaEntrega = false;


                if (gatilho.Tipo == TipoGatilhoOcorrencia.FluxoPatio)
                {
                    gatilho.GatilhoFinalFluxoPatio = ((string)gatilhoSalvar.GatilhoFinalFluxoPatio).ToEnum<EtapaFluxoGestaoPatio>();
                    gatilho.GatilhoInicialFluxoPatio = ((string)gatilhoSalvar.GatilhoInicialFluxoPatio).ToEnum<TipoGatilhoOcorrenciaInicialFluxoPatio>();
                }
                else if (gatilho.Tipo == TipoGatilhoOcorrencia.Tracking)
                {
                    gatilho.GatilhoFinalTraking = ((string)gatilhoSalvar.GatilhoFinalTraking).ToEnum<GatilhoFinalTraking>();
                    gatilho.GatilhoInicialTraking = ((string)gatilhoSalvar.GatilhoInicialTraking).ToEnum<GatilhoInicialTraking>();
                    gatilho.TipoAplicacaoGatilhoTracking = ((string)gatilhoSalvar.TipoAplicacaoGatilhoTracking).ToEnum<TipoAplicacaoGatilhoTracking>();

                    if ((gatilho.TipoAplicacaoGatilhoTracking is TipoAplicacaoGatilhoTracking.Entrega or TipoAplicacaoGatilhoTracking.Coleta) && gatilho.GatilhoInicialTraking == GatilhoInicialTraking.EntradaCliente)
                        gatilho.ValidarDataAgendadaEntrega = ((string)gatilhoSalvar.ValidarDataAgendadaEntrega).ToBool();
                }
                else if (gatilho.Tipo == TipoGatilhoOcorrencia.AlteracaoData)
                    gatilho.TipoDataAlteracaoGatilho = ((string)gatilhoSalvar.TipoDataAlteracaoGatilho).ToEnum<TipoDataAlteracaoGatilho>();

                if (gatilho.Codigo > 0)
                    repositorioGatilho.Atualizar(gatilho);
                else
                    repositorioGatilho.Inserir(gatilho);

                SalvarTipoOperacaoGatilho(gatilho, Request.GetListParam<int>("GatilhoTipoOperacao"), unitOfWork);
                SalvarFilialGatilho(gatilho, Request.GetListParam<int>("GatilhoFilial"), unitOfWork);
                SalvarTransportadorGatilho(gatilho, Request.GetListParam<int>("GatilhoTransportador"), unitOfWork);
            }
            else if (gatilho != null)
                repositorioGatilho.Deletar(gatilho);
        }

        private void SalvarTransportadorGatilho(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho, List<int> codigosTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            if (gatilho.Transportadores == null) gatilho.Transportadores = new List<Dominio.Entidades.Empresa>();
            gatilho.Transportadores.Clear();

            foreach (int codigo in codigosTransportador)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);
                if (empresa == null)
                    continue;

                gatilho.Transportadores.Add(empresa);
            }
        }

        private void SalvarFilialGatilho(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho, List<int> codigosFilial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            if (gatilho.Filiais == null) gatilho.Filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            gatilho.Filiais.Clear();

            foreach (int codigo in codigosFilial)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigo);
                if (filial == null)
                    continue;

                gatilho.Filiais.Add(filial);
            }
        }

        private void SalvarTipoOperacaoGatilho(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho, List<int> codigosTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            if (gatilho.TiposOperacoes == null) gatilho.TiposOperacoes = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            gatilho.TiposOperacoes.Clear();

            foreach (int codigo in codigosTipoOperacao)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigo);
                if (tipoOperacao == null)
                    continue;

                gatilho.TiposOperacoes.Add(tipoOperacao);
            }
        }

        private void SalvarListaParametros(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoDeOcorrenciaDeCTe.Codigo);
            if (tipoOcorrencia != null && tipoOcorrencia.ParametrosOcorrencia != null && tipoOcorrencia.ParametrosOcorrencia.Count() > 0)
                tipoOcorrencia.ParametrosOcorrencia.Clear();

            dynamic listaParametros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametros"));
            if (listaParametros != null)
            {
                if (tipoDeOcorrenciaDeCTe.ParametrosOcorrencia == null)
                    tipoDeOcorrenciaDeCTe.ParametrosOcorrencia = new List<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia>();

                foreach (dynamic parametro in listaParametros)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo((int)parametro.Codigo);
                    if (parametroOcorrencia != null)
                    {
                        tipoDeOcorrenciaDeCTe.ParametrosOcorrencia.Add(parametroOcorrencia);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoDeOcorrenciaDeCTe, null, string.Format(Localization.Resources.Ocorrencias.TipoOcorrencia.AdicionouParametro, parametroOcorrencia.Descricao), unitOfWork);
                    }
                }
            }
        }

        private void SalvarPermissoes(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoOcorrenciaCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcesso repositorioPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTePerfilAcesso repositorioTipoDeOcorrenciaDeCTePerfilAcesso = new Repositorio.TipoDeOcorrenciaDeCTePerfilAcesso(unitOfWork);

            tipoDeOcorrenciaDeCTe.BloquearVisualizacaoTipoOcorrenciaTransportador = Request.GetNullableBoolParam("BloquearVisualizacaoTipoOcorrenciaTransportador");
            tipoDeOcorrenciaDeCTe.NaoPermitirQueTransportadorSelecioneTipoOcorrencia = Request.GetBoolParam("NaoPermitirQueTransportadorSelecioneTipoOcorrencia");
            tipoDeOcorrenciaDeCTe.PermitirTransportadorInformarDataInicioFimRaioCarga = Request.GetBoolParam("PermitirTransportadorInformarDataInicioFimRaioCarga");
            tipoDeOcorrenciaDeCTe.PermitirConsultarCTesComEsseTipoDeOcorrencia = Request.GetBoolParam("PermitirConsultarCTesComEsseTipoDeOcorrencia");
            tipoDeOcorrenciaDeCTe.PermitirTransportadorInformarDataInicioFimRaioCarga = Request.GetBoolParam("PermitirTransportadorInformarDataInicioFimRaioCarga");

            repositorioTipoOcorrenciaCTe.Atualizar(tipoDeOcorrenciaDeCTe);

            if (tipoDeOcorrenciaDeCTe.Codigo > 0)
                repositorioTipoDeOcorrenciaDeCTePerfilAcesso.DeletarTodosPorTipoDeOcorrenciaDeCTe(tipoDeOcorrenciaDeCTe.Codigo);

            dynamic listaPerfilAcesso = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PerfilAcesso"));

            if (listaPerfilAcesso != null)
            {
                foreach (dynamic perfilAcesso in listaPerfilAcesso)
                {
                    Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso perfilAcessoAdicionar = new Dominio.Entidades.TipoDeOcorrenciaDeCTePerfilAcesso()
                    {
                        PerfilAcesso = repositorioPerfilAcesso.BuscarPorCodigo(((string)perfilAcesso.Codigo).ToInt()),
                        TipoDeOcorrenciaDeCTe = tipoDeOcorrenciaDeCTe
                    };

                    repositorioTipoDeOcorrenciaDeCTePerfilAcesso.Inserir(perfilAcessoAdicionar);
                }
            }
        }

        private void SalvarTiposIntegracao(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<TipoIntegracao> tiposIntegracao = Request.GetListEnumParam<TipoIntegracao>("TipoIntegracao");

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao> ocorrenciaTiposIntegracao = repOcorrenciaTipoIntegracao.BuscarPorTipoOcorrencia(tipoDeOcorrenciaDeCTe.Codigo);

            if (ocorrenciaTiposIntegracao.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao> deletar = (from obj in ocorrenciaTiposIntegracao where !tiposIntegracao.Contains(obj.TipoIntegracao.Tipo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao ocorrenciaTipoIntegracao in deletar)
                {
                    TipoIntegracao tipoIntegracaoDeletar = ocorrenciaTipoIntegracao.TipoIntegracao.Tipo;
                    repOcorrenciaTipoIntegracao.Deletar(ocorrenciaTipoIntegracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoDeOcorrenciaDeCTe, string.Format(Localization.Resources.Ocorrencias.TipoOcorrencia.ExcluiuIntegracao, tipoIntegracaoDeletar.ObterDescricao()), unitOfWork);
                }
            }

            foreach (TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao ocorrenciaTipoIntegracao = repOcorrenciaTipoIntegracao.BuscarPorTipoOcorrenciaETipoIntegracao(tipoDeOcorrenciaDeCTe.Codigo, tipoIntegracao);

                if (ocorrenciaTipoIntegracao == null)
                {
                    ocorrenciaTipoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao();
                    ocorrenciaTipoIntegracao.TipoOcorrencia = tipoDeOcorrenciaDeCTe;
                    ocorrenciaTipoIntegracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);
                    repOcorrenciaTipoIntegracao.Inserir(ocorrenciaTipoIntegracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoDeOcorrenciaDeCTe, string.Format(Localization.Resources.Ocorrencias.TipoOcorrencia.AdicionouIntegracao, tipoIntegracao.ObterDescricao()), unitOfWork);
                }
            }
        }

        private void SalvarNotificacao(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic dynNotificacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Notificacao"));
            bool notificarTransportadorPorEmail = ((string)dynNotificacao.NotificarTransportadorPorEmail).ToBool();
            bool notificarPorEmail = ((string)dynNotificacao.NotificarEmail).ToBool();
            bool notificarClientePorEmail = ((string)dynNotificacao.NotificarClientePorEmail).ToBool();


            if (tipoDeOcorrenciaDeCTe.TipoOcorrenciaControleEntrega && notificarTransportadorPorEmail)
                notificarTransportadorPorEmail = false;

            tipoDeOcorrenciaDeCTe.NotificarTransportadorPorEmail = notificarTransportadorPorEmail;
            tipoDeOcorrenciaDeCTe.NotificarPorEmail = notificarPorEmail;
            tipoDeOcorrenciaDeCTe.NotificarClientePorEmail = notificarClientePorEmail;
            tipoDeOcorrenciaDeCTe.AssuntoEmailNotificacao = (string)dynNotificacao.AssuntoEmail;
            tipoDeOcorrenciaDeCTe.CorpoEmailNotificacao = (string)dynNotificacao.CorpoEmail;


            dynamic emailsNotificacao = dynNotificacao.ListaEmail; //Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OutrosCodigosIntegracao"));

            if (tipoDeOcorrenciaDeCTe.EmailsNotificacao == null)
                tipoDeOcorrenciaDeCTe.EmailsNotificacao = new List<string>();

            tipoDeOcorrenciaDeCTe.EmailsNotificacao.Clear();

            foreach (dynamic email in emailsNotificacao)
            {
                //if (!string.IsNullOrEmpty((string)outroCodigo.CodigoIntegracao) && repCliente.ValidarCodigoExiste((string)outroCodigo.CodigoIntegracao, pessoa.Codigo))
                //    throw new ControllerException(string.Format(Localization.Resources.Pessoas.Pessoa.JaExistePessoaComCodigoIntegracaoCadastrado, (string)outroCodigo.CodigoIntegracao));

                tipoDeOcorrenciaDeCTe.EmailsNotificacao.Add((string)email.Email);
            }

        }

        private void SalvarCausasTipoOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas repositorioTipoOcorrenciaCausas = new Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas(unitOfWork);

            List<dynamic> listaTipoOcorrenciaCausas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Causas"));

            ExcuirCausasTipoOcorrencia(tipoDeOcorrenciaDeCTe,listaTipoOcorrenciaCausas, unitOfWork);

            foreach (dynamic tipoOcorrenciaCausa in listaTipoOcorrenciaCausas)
            {
                Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas Causa = repositorioTipoOcorrenciaCausas.BuscarPorCodigo((int)tipoOcorrenciaCausa.Codigo);

                if(Causa != null)
                {
                    Causa.Descricao = (string)tipoOcorrenciaCausa.Descricao;
                    Causa.Ativo = ((string)tipoOcorrenciaCausa.Ativo == "Sim" ? true : false);
                    Causa.TipoOcorrencia = tipoDeOcorrenciaDeCTe;

                    repositorioTipoOcorrenciaCausas.Atualizar(Causa);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoDeOcorrenciaDeCTe, string.Format("Atualizou a Causa ", Causa.Codigo), unitOfWork);
                }
                else
                {
                    Causa = new Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas();
                    Causa.Descricao = (string)tipoOcorrenciaCausa.Descricao;
                    Causa.Ativo = ((string)tipoOcorrenciaCausa.Ativo == "Sim" ? true : false);
                    Causa.TipoOcorrencia = tipoDeOcorrenciaDeCTe;

                    repositorioTipoOcorrenciaCausas.Inserir(Causa);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoDeOcorrenciaDeCTe, string.Format("Adicionou a Causa ", Causa.Descricao), unitOfWork);
                }
                
            }

        }

        private void ExcuirCausasTipoOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe, List<dynamic> listaTipoOcorrenciaCausas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas repositorioTipoOcorrenciaCausas = new Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas(unitOfWork);
            List<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> listaTipoOcorrenciaCausaExcluir = repositorioTipoOcorrenciaCausas.BuscarPorTipoOcorrencia(tipoDeOcorrenciaDeCTe.Codigo);

            if (listaTipoOcorrenciaCausaExcluir.Count > 0 && (int)listaTipoOcorrenciaCausas.Count == 0)
            {
                //Exclui todas as causas
                repositorioTipoOcorrenciaCausas.Deletar(listaTipoOcorrenciaCausaExcluir);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoDeOcorrenciaDeCTe, string.Format("Excluído as Causas do Tipo de Ocorrencia ", tipoDeOcorrenciaDeCTe.Codigo), unitOfWork);
            }else
            {
                if (listaTipoOcorrenciaCausaExcluir.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas> deletar = (from obj in listaTipoOcorrenciaCausaExcluir where !listaTipoOcorrenciaCausas.Contains(obj.Codigo) select obj).ToList();

                    foreach (Dominio.Entidades.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas causa in deletar)
                    {
                        //Exclui a causa removida da tela
                        repositorioTipoOcorrenciaCausas.Deletar(causa);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoDeOcorrenciaDeCTe, string.Format("Excluiu a Causa ", causa.Descricao), unitOfWork);
                    }
                }
            }

        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas ObterFiltrosPesquisaCausas()
        {

            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaCausas()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoTipoOcorrencia = Request.GetIntParam("CodigoTipoOcorrencia"),
                BuscarTodasCausasDesconsiderandoTipoOcorrencia = Request.GetBoolParam("BuscarTodasCausasDesconsiderandoTipoOcorrencia"),
            };
        }

        #endregion
    }
}
