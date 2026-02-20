using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize(new string[] { "BuscarClientesPorRaiz" }, "Pessoas/GrupoPessoas")]
    public class GrupoPessoasController : BaseController
    {
        #region Construtores

        public GrupoPessoasController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaGrupoPessoas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Bloqueado", false);
                grid.AdicionarCabecalho("MotivoBloqueio", false);
                grid.AdicionarCabecalho("ObservacaoEmissaoCarga", false);
                grid.AdicionarCabecalho("GerarPedidoColeta", false);
                grid.AdicionarCabecalho("DisponibilizarDocumentosParaLoteEscrituracao", false);
                grid.AdicionarCabecalho("DisponibilizarDocumentosParaLoteEscrituracaoCancelamento", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TempoCarregamento", false);
                grid.AdicionarCabecalho("TempoDescarregamento", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 70, Models.Grid.Align.left, true);
                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = repGrupoPessoas.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repGrupoPessoas.ContarConsulta(filtrosPesquisa));

                var lista = (from obj in gruposPessoas
                             select new
                             {
                                 obj.Bloqueado,
                                 obj.DisponibilizarDocumentosParaLoteEscrituracao,
                                 obj.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento,
                                 obj.MotivoBloqueio,
                                 obj.ObservacaoEmissaoCarga,
                                 obj.Codigo,
                                 obj.Descricao,
                                 obj.DescricaoAtivo,
                                 obj.GerarPedidoColeta,
                                 TempoCarregamento = $"{(int)obj.TempoCarregamento.TotalHours:d3}:{obj.TempoCarregamento:mm}",
                                 TempoDescarregamento = $"{(int)obj.TempoDescarregamento.TotalHours:d3}:{obj.TempoDescarregamento:mm}"
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
        public async Task<IActionResult> ObterTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool opcaoSemGrupo = Request.GetBoolParam("OpcaoSemGrupo");
                bool apenasClientes = Request.GetBoolParam("ApenasClientes");

                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaGrupoPessoas filtrosPesquisa = ObterFiltrosPesquisa(unidadeDeTrabalho);

                filtrosPesquisa.TipoGrupoPessoas = apenasClientes ? TipoGrupoPessoas.Clientes : TipoGrupoPessoas.Ambos;

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta
                {
                    PropriedadeOrdenar = "Descricao",
                    DirecaoOrdenar = "asc",
                    InicioRegistros = 0,
                    LimiteRegistros = 999
                };

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = repGrupoPessoas.Consultar(filtrosPesquisa, parametrosConsulta);

                var retorno = (from obj in gruposPessoas select new { value = obj.Codigo, text = obj.Descricao }).ToList();

                if (opcaoSemGrupo)
                    retorno.Insert(0, new { value = -1, text = "Sem Grupo de Pessoas" });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/GrupoPessoas");

                if (!Usuario.UsuarioAdministrador && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Criar))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                unitOfWork.Start();

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas();

                PreencherGrupoPessoas(grupoPessoas, unitOfWork);
                PreencherDadosAdicionaisGrupoPessoas(grupoPessoas, unitOfWork);

                bool novo = true;
                if (!string.IsNullOrWhiteSpace(grupoPessoas.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasExiste = repGrupoPessoas.BuscarPorCodigoIntegracao(grupoPessoas.CodigoIntegracao);
                    if (grupoPessoasExiste != null)
                        novo = false;
                }
                else
                    grupoPessoas.CodigoIntegracao = Guid.NewGuid().ToString().Replace("-", "");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    grupoPessoas.Empresa = this.Usuario.Empresa;

                SalvarPerfilChamado(grupoPessoas, unitOfWork);
                PreencherDadosFornecedor(grupoPessoas, unitOfWork);

                if (novo)
                    repGrupoPessoas.Inserir(grupoPessoas, Auditado);
                else
                    throw new ControllerException("Já existe um grupo de pessoas com esse código de integração informado");

                IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosFaturaCanhoto = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo>();
                dynamic configuracaoFatura = SalvarConfiguracaoFatura(grupoPessoas, permissoesPersonalizadas, unitOfWork, ref anexosFaturaCanhoto);
                SalvarConfiguracaoFaturaListasAuditarManual(grupoPessoas, unitOfWork);

                SalvarPessoasDoGrupo(grupoPessoas, unitOfWork);

                string retornoListaRaiz = SalvarListaRaizCNPJ(grupoPessoas, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retornoListaRaiz))
                    throw new ControllerException("Já existe um grupo de pessoas com a raíz " + retornoListaRaiz + " cadastrado");

                List<int> codigosLayoutEDIsPorAnexo = new List<int>();
                IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosLayoutEDI = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo>();
                SalvarLayoutsEDI(grupoPessoas, unitOfWork, ref anexosLayoutEDI);

                dynamic configuracao = SalvarConfiguracaoEmissaoCTe(grupoPessoas, permissoesPersonalizadas, unitOfWork);
                SalvarModelosReboque(ref grupoPessoas, unitOfWork);
                SalvarOcorrencias(grupoPessoas, unitOfWork);
                SalvarEmails(grupoPessoas, unitOfWork);
                SalvarLeituraDinamicaXML(grupoPessoas, unitOfWork);
                SalvarContatos(grupoPessoas, unitOfWork);
                SalvarVendedores(grupoPessoas, unitOfWork);
                SalvarMensagemAlerta(grupoPessoas, unitOfWork);
                SalvarAutorizadosDownloadDFe(grupoPessoas, unitOfWork);
                SalvarFormulasObservacaoNfe(grupoPessoas, unitOfWork);
                SalvarTabelaValores(grupoPessoas, unitOfWork);
                SalvarObservacoesCTe(grupoPessoas, unitOfWork);
                SalvarTabelasFornecedor(grupoPessoas, unitOfWork);
                SalvarNCMPalletsNFe(grupoPessoas, unitOfWork);
                SalvarGrupoPessoasIntegracao(grupoPessoas, unitOfWork);
                SalvarTipoComprovantes(grupoPessoas, unitOfWork);
                AtualizarConfiguracoesEmissao(grupoPessoas, configuracao, null, unitOfWork);
                AtualizarConfiguracoesFatura(grupoPessoas, configuracaoFatura, null, unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> servicoAnexoLayoutEDI = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>(unitOfWork);
                foreach (Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo anexoLayoutEDI in anexosLayoutEDI)
                {
                    IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexos = null;
                    anexos.Add(anexoLayoutEDI);
                    servicoAnexoLayoutEDI.AnexarArquivos(anexoLayoutEDI.CodigoEntidade, anexos);
                }

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> servicoAnexoFaturaCanhoto = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(unitOfWork);
                foreach (Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo anexoFaturaCanhoto in anexosFaturaCanhoto)
                {
                    IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosg = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo>();
                    anexosg.Add(anexoFaturaCanhoto);
                    servicoAnexoFaturaCanhoto.AnexarArquivos(anexoFaturaCanhoto.CodigoEntidade, anexosg);
                }

                return new JsonpResult(new { grupoPessoas.Codigo });
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/GrupoPessoas");

                unitOfWork.Start();

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (grupoPessoas == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherGrupoPessoas(grupoPessoas, unitOfWork);
                PreencherDadosAdicionaisGrupoPessoas(grupoPessoas, unitOfWork);

                bool novo = true;
                if (!string.IsNullOrWhiteSpace(grupoPessoas.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasExiste = repGrupoPessoas.BuscarPorCodigoIntegracao(grupoPessoas.CodigoIntegracao);
                    if (grupoPessoasExiste != null && grupoPessoasExiste.Codigo != grupoPessoas.Codigo)
                        novo = false;
                }
                else
                    grupoPessoas.CodigoIntegracao = Guid.NewGuid().ToString().Replace("-", "");

                IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosFaturaCanhoto = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo>();
                dynamic configuracaoFatura = SalvarConfiguracaoFatura(grupoPessoas, permissoesPersonalizadas, unitOfWork, ref anexosFaturaCanhoto);

                if (!novo)
                    throw new ControllerException("Já existe um grupo de pessoas com esse código de integração informado");

                SalvarPessoasDoGrupo(grupoPessoas, unitOfWork);

                string retornoListaRaiz = SalvarListaRaizCNPJ(grupoPessoas, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retornoListaRaiz))
                    throw new ControllerException("Já existe um grupo de pessoas com a raíz " + retornoListaRaiz + " cadastrado");

                List<int> codigosLayoutEDIsPorAnexo = new List<int>();
                IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosLayoutEDI = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo>();
                SalvarLayoutsEDI(grupoPessoas, unitOfWork, ref anexosLayoutEDI);

                dynamic configuracao = SalvarConfiguracaoEmissaoCTe(grupoPessoas, permissoesPersonalizadas, unitOfWork);
                SalvarModelosReboque(ref grupoPessoas, unitOfWork);
                SalvarOcorrencias(grupoPessoas, unitOfWork);
                SalvarEmails(grupoPessoas, unitOfWork);
                SalvarLeituraDinamicaXML(grupoPessoas, unitOfWork);
                SalvarContatos(grupoPessoas, unitOfWork);
                SalvarVendedores(grupoPessoas, unitOfWork);
                SalvarMensagemAlerta(grupoPessoas, unitOfWork);
                SalvarAutorizadosDownloadDFe(grupoPessoas, unitOfWork);
                SalvarFormulasObservacaoNfe(grupoPessoas, unitOfWork);
                SalvarPerfilChamado(grupoPessoas, unitOfWork);
                SalvarTabelaValores(grupoPessoas, unitOfWork);
                SalvarObservacoesCTe(grupoPessoas, unitOfWork);
                PreencherDadosFornecedor(grupoPessoas, unitOfWork);
                SalvarTabelasFornecedor(grupoPessoas, unitOfWork);
                SalvarNCMPalletsNFe(grupoPessoas, unitOfWork);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repGrupoPessoas.Atualizar(grupoPessoas, Auditado);

                SalvarConfiguracaoFaturaListasAuditarManual(grupoPessoas, unitOfWork);
                SalvarGrupoPessoasIntegracao(grupoPessoas, unitOfWork);
                SalvarTipoComprovantes(grupoPessoas, unitOfWork);

                AtualizarConfiguracoesEmissao(grupoPessoas, configuracao, historico, unitOfWork);
                AtualizarConfiguracoesFatura(grupoPessoas, configuracaoFatura, historico, unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> servicoAnexoLayoutEDI = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>(unitOfWork);
                foreach (Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo anexoLayoutEDI in anexosLayoutEDI)
                {
                    IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexos = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo>();
                    anexos.Add(anexoLayoutEDI);
                    servicoAnexoLayoutEDI.AnexarArquivos(anexoLayoutEDI.CodigoEntidade, anexos);
                }

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> servicoAnexoFaturaCanhoto = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(unitOfWork);
                foreach (Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo anexoFaturaCanhoto in anexosFaturaCanhoto)
                {
                    IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosg = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo>();
                    anexosg.Add(anexoFaturaCanhoto);
                    servicoAnexoFaturaCanhoto.AnexarArquivos(anexoFaturaCanhoto.CodigoEntidade, anexosg);
                }

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarClientesPorRaiz()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoGrupo = int.Parse(Request.Params("CodigoGrupo"));

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                List<Dominio.Entidades.Cliente> listaClienteFinal = new List<Dominio.Entidades.Cliente>();
                dynamic listaRaiz = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaRaizCNPJ"));
                if (listaRaiz != null)
                {
                    foreach (dynamic raizCNPJ in listaRaiz)
                    {
                        List<Dominio.Entidades.Cliente> listaCliente = repCliente.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers((string)raizCNPJ.RaizCNPJ), codigoGrupo);
                        for (int i = 0; i < listaCliente.Count; i++)
                        {
                            listaClienteFinal.Add(listaCliente[i]);
                        }
                    }
                }
                dynamic retorno = new
                {
                    Pessoas = from p in listaClienteFinal
                              select new
                              {
                                  Codigo = p.CPF_CNPJ,
                                  CPF_CNPJ = p.CPF_CNPJ_Formatado,
                                  Nome = p.Nome,
                                  DescricaoCidadeEstado = p.Localidade.DescricaoCidadeEstado
                              }
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por Raiz do CNPJ.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP repGrupoPessoasIntegracaoFTP = new Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoasObservacaoCTe repGrupoPessoasObservacaoCTe = new Repositorio.Embarcador.Pessoas.GrupoPessoasObservacaoCTe(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia repGrupoPessoaOcorrencia = new Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoasEmailDocumento repPessoaEmailDocumento = new Repositorio.Embarcador.Pessoas.GrupoPessoasEmailDocumento(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml repGrupoPessoasLeituraDinamicaXml = new Repositorio.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaVencimento repGrupoPessoasFaturaVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaVencimento(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto repGrupoPessoasFaturaCanhoto = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repositorioGrupoPessoasIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unidadeDeTrabalho);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> repAnexoGrupoPessoasLayoutEDI = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>(unidadeDeTrabalho);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> repAnexoGrupoPessoasFaturaCanhoto = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(unidadeDeTrabalho);
                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> servicoAnexoLayoutEDI = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>(unidadeDeTrabalho);
                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> servicoAnexoFaturaCanhoto = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(unidadeDeTrabalho);

                Servicos.Embarcador.Pessoa.GrupoPessoa servicoGrupoPessoa = new Servicos.Embarcador.Pessoa.GrupoPessoa();

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigo);

                if (grupoPessoas == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP grupoPessoasIntegracaoFTP = repGrupoPessoasIntegracaoFTP.BuscarPorGrupoPessoasAsync(grupoPessoas.Codigo, default).Result;
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoCTe> observacoesCTe = repGrupoPessoasObservacaoCTe.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia> ocorrencias = repGrupoPessoaOcorrencia.BuscarTodosPorEntidade(grupoPessoas);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail> modelosDocumentosEmail = repPessoaEmailDocumento.BuscarTodosPorEntidade(grupoPessoas);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> leiturasDinamicaXml = repGrupoPessoasLeituraDinamicaXml.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento> grupoPessoasFaturaVencimentos = repGrupoPessoasFaturaVencimento.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> grupoPessoasFaturaCanhoto = repGrupoPessoasFaturaCanhoto.BuscarPorGrupoPessoasAsync(grupoPessoas.Codigo, default).Result;
                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repositorioGrupoPessoasIntegracao.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
                List<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI> listaAnexoLayoutEDI = repAnexoGrupoPessoasLayoutEDI.BuscarPorEntidades(grupoPessoas.LayoutsEDI.Select(o => o.Codigo).ToList());
                List<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto> listaAnexoFaturaCanhoto = repAnexoGrupoPessoasFaturaCanhoto.BuscarPorEntidade(grupoPessoasFaturaCanhoto.Select(o => o.Codigo).FirstOrDefault());


                var dynGrupoPessoas = new
                {
                    grupoPessoas.NomeNomenclaturaArquivosDownloadCTe,
                    grupoPessoas.Codigo,
                    grupoPessoas.Descricao,
                    grupoPessoas.Bloqueado,
                    grupoPessoas.MotivoBloqueio,
                    grupoPessoas.UtilizaMetaEmissao,
                    grupoPessoas.LerPDFNotaFiscalRecebidaPorEmail,
                    grupoPessoas.MetaEmissaoMensal,
                    grupoPessoas.MetaEmissaoAnual,
                    grupoPessoas.TipoGrupoPessoas,
                    grupoPessoas.Prioridade,
                    GrupoPessoas = new
                    {
                        grupoPessoas.Codigo,
                        grupoPessoas.Descricao
                    },
                    grupoPessoas.Ativo,
                    grupoPessoas.CodigoIntegracao,
                    NaoImportarDocumentosDestinadosTransporte = grupoPessoas.NaoImportarDocumentoDestinadoTransporte,
                    grupoPessoas.ValidaPlacaNFe,
                    grupoPessoas.ValidaOrigemNFe,
                    grupoPessoas.ArmazenaProdutosXMLNFE,
                    grupoPessoas.ValidaDestinoNFe,
                    grupoPessoas.ValidaEmitenteNFe,
                    grupoPessoas.LerNumeroPedidoDaObservacaoDaNota,
                    grupoPessoas.ControlaPagamentos,
                    grupoPessoas.Email,
                    grupoPessoas.Contato,
                    grupoPessoas.TelefoneContato,
                    grupoPessoas.EnviarXMLCTePorEmail,
                    grupoPessoas.EnviarNovoVeiculoEmail,
                    grupoPessoas.EmailEnvioNovoVeiculo,
                    grupoPessoas.ExigirRotaParaEmissaoDocumentos,
                    grupoPessoas.ExigirNumeroControleCliente,
                    grupoPessoas.ExigirNumeroNumeroReferenciaCliente,
                    grupoPessoas.ReplicarNumeroReferenciaTodasNotasCarga,
                    grupoPessoas.ReplicarNumeroControleCliente,
                    grupoPessoas.GerarPedidoColeta,
                    grupoPessoas.GerarNumeroConhecimentoNoBoleto,
                    grupoPessoas.GerarNumeroFaturaNoBoleto,
                    grupoPessoas.GerarOcorrenciaControleEntrega,
                    grupoPessoas.PermitirConsultarOcorrenciaControleEntregaWebService,
                    grupoPessoas.ParquearDocumentosAutomaticamenteParaCNPJDesteGrupo,
                    grupoPessoas.ExigirRotaCalculoFreteParaEmissaoDocumentos,
                    grupoPessoas.ProdutoPredominante,
                    grupoPessoas.LerPlacaDaObservacaoDaNota,
                    grupoPessoas.LerPlacaDaObservacaoDaNotaInicio,
                    grupoPessoas.LerPlacaDaObservacaoDaNotaFim,
                    grupoPessoas.LerPlacaDaObservacaoContribuinteDaNota,
                    grupoPessoas.LerPlacaDaObservacaoContribuinteDaNotaIdentificacao,
                    grupoPessoas.VincularNotaFiscalEmailNaCarga,
                    grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento,
                    grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial,
                    grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoFinal,
                    grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoInicial,
                    grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoFinal,
                    grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial,
                    grupoPessoas.LerKMObservacaoNotaParaAbastecimentoFinal,
                    grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial,
                    grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoFinal,
                    grupoPessoas.LerNumeroPedidoDaObservacaoDaNotaInicio,
                    grupoPessoas.LerNumeroPedidoDaObservacaoDaNotaFim,
                    grupoPessoas.ExpressaoBooking,
                    grupoPessoas.ExpressaoContainer,
                    grupoPessoas.TokenIntegracaoMultiEmbarcador,
                    grupoPessoas.URLIntegracaoMultiEmbarcador,
                    grupoPessoas.SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe,
                    grupoPessoas.LerNumeroPedidoObservacaoCTe,
                    grupoPessoas.LerNumeroPedidoObservacaoMDFe,
                    grupoPessoas.RegexNumeroPedidoObservacaoCTe,
                    grupoPessoas.RegexNumeroPedidoObservacaoMDFe,
                    grupoPessoas.VincularCTePeloNumeroPedido,
                    grupoPessoas.VincularMDFePeloNumeroPedido,
                    grupoPessoas.LerNumeroPedidoObservacaoCTeSubcontratacao,
                    grupoPessoas.VincularCTeSubcontratacaoPeloNumeroPedido,
                    grupoPessoas.RegexNumeroPedidoObservacaoCTeSubcontratacao,
                    HabilitarIntegracaoVeiculoMultiEmbarcador = grupoPessoas.HabilitarIntegracaoVeiculoMultiEmbarcador ?? false,
                    HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador = grupoPessoas.HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador ?? false,
                    HabilitarIntegracaoOcorrenciasMultiEmbarcador = grupoPessoas.HabilitarIntegracaoOcorrenciasMultiEmbarcador ?? false,
                    HabilitarIntegracaoXmlCteMultiEmbarcador = grupoPessoas.HabilitarIntegracaoXmlCteMultiEmbarcador ?? false,
                    HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador = grupoPessoas.HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador ?? false,
                    DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador = grupoPessoas.DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador?.ToString("dd/MM/yyyy") ?? string.Empty,
                    grupoPessoas.NaoGerarOcorreciaApenasDocumentos,
                    TipoEmissaoCTeDocumentosExclusivo = grupoPessoas.TipoEmissaoCTeDocumentosExclusivo.HasValue ? grupoPessoas.TipoEmissaoCTeDocumentosExclusivo.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado,
                    RegraCotacaoFeeder = grupoPessoas.RegraCotacaoFeeder.HasValue ? grupoPessoas.RegraCotacaoFeeder.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraCotacaoFeeder.Nenhuma,
                    grupoPessoas.LerNumeroCargaObservacaoCTeSubcontratacao,
                    grupoPessoas.RegexNumeroCargaObservacaoCTeSubcontratacao,
                    grupoPessoas.NaoAdicionarNotaNCMPalletCarga,
                    grupoPessoas.ZerarPesoNotaNCMPalletCarga,
                    grupoPessoas.NaoAlterarDocumentoIntegracao,
                    TipoIntegracoes = (from o in integracaoGrupoPessoas
                                       select new
                                       {
                                           Codigo = o.Codigo,
                                           Descricao = o.TipoIntegracao.Descricao,
                                       }).ToList(),
                    RateioFormulaExclusivo = new
                    {
                        Codigo = grupoPessoas.RateioFormulaExclusivo?.Codigo ?? 0,
                        Descricao = grupoPessoas.RateioFormulaExclusivo?.Descricao ?? string.Empty
                    },
                    Classificacao = new
                    {
                        Codigo = grupoPessoas.Classificacao?.Codigo ?? 0,
                        Descricao = grupoPessoas.Classificacao?.Descricao ?? ""
                    },
                    InformarProdutoPredominante = !string.IsNullOrWhiteSpace(grupoPessoas.ProdutoPredominante),
                    Transportador = grupoPessoas.Empresa != null ? new { grupoPessoas.Empresa.Codigo, grupoPessoas.Empresa.Descricao } : null,
                    DadosAdicionais = new
                    {
                        PedidoTipoPagamento = grupoPessoas.PedidoTipoPagamento != null ? new { grupoPessoas.PedidoTipoPagamento.Codigo, grupoPessoas.PedidoTipoPagamento.Descricao } : null,
                        grupoPessoas.SituacaoFinanceira,
                        grupoPessoas.ClassificacaoEmpresa,
                        grupoPessoas.CobrancaDiaria,
                        grupoPessoas.CobrancaDescarga,
                        grupoPessoas.CobrancaCarregamento,
                        CobrancaDiariaObservacao = grupoPessoas.CobrancaDiariaObservacao ?? string.Empty,
                        CobrancaDescargaObservacao = grupoPessoas.CobrancaDescargaObservacao ?? string.Empty,
                        CobrancaCarregamentoObservacao = grupoPessoas.CobrancaCarregamentoObservacao ?? string.Empty,
                        grupoPessoas.EnviarAutomaticamenteDocumentacaoCarga,
                        grupoPessoas.NaoGerarArquivoVgm,
                        grupoPessoas.ControlaPallets,
                        grupoPessoas.GerarImpressaoOrdemColetaExclusiva,
                        grupoPessoas.TipoImpressaoOrdemColetaExclusiva,
                        grupoPessoas.NaoEnviarParaDocsys,
                        Despachante = grupoPessoas.Despachante != null ? new { grupoPessoas.Despachante.Codigo, grupoPessoas.Despachante.Descricao } : null,
                        grupoPessoas.EmailDespachante,
                        grupoPessoas.AdicionarDespachanteComoConsignatario,
                        grupoPessoas.NaoEnviarXMLCteSubcontratacaoOuRedespachoPorEmail
                    },
                    ModelosReboque = (from obj in grupoPessoas.ModelosReboque
                                      orderby obj.Descricao
                                      select new
                                      {
                                          Modelo = new
                                          {
                                              Codigo = obj.Codigo,
                                              Descricao = obj.Descricao
                                          }
                                      }).ToList(),
                    Ocorrencias = (from obj in ocorrencias
                                   select new
                                   {
                                       Codigo = obj.Codigo,
                                       TipoOcorrencia = obj.TipoOcorrencia.Descricao,
                                       CodigoIntegracao = obj.CodigoIntegracao,
                                       CodigoTipoOcorrencia = obj.TipoOcorrencia.Codigo
                                   }).ToList(),
                    EmailDocumentos = (from obj in modelosDocumentosEmail
                                       select new
                                       {
                                           obj.Codigo,
                                           CodigoModeloDocumento = obj.ModeloDocumentoFiscal.Codigo,
                                           obj.Emails,
                                           ModeloDocumentoFiscal = obj.ModeloDocumentoFiscal.Descricao
                                       }).ToList(),
                    LeituraDinamicaXML = (from obj in leiturasDinamicaXml
                                          select new
                                          {
                                              obj.Codigo,
                                              TipoDocumento = obj.TipoDocumento,
                                              Descricao = obj.Descricao,
                                              CodigoLeituraDinamicaXmlOrigem = obj.LeituraDinamicaXmlOrigem.Codigo,
                                              LeituraDinamicaXmlOrigem = obj.LeituraDinamicaXmlOrigem.Descricao,
                                              CodigoLeituraDinamicaXmlOrigemTagFilha = obj.LeituraDinamicaXmlOrigemTagFilha?.Codigo,
                                              LeituraDinamicaXmlOrigemTagFilha = obj.LeituraDinamicaXmlOrigemTagFilha?.Descricao,
                                              CodigoLeituraDinamicaXmlDestino = obj.LeituraDinamicaXmlDestino.Codigo,
                                              LeituraDinamicaXmlDestino = obj.LeituraDinamicaXmlDestino.Descricao,
                                              FiltrarTag = obj.FiltrarTag,
                                              FiltrarPrimeiroDisponivel = obj.FiltrarPrimeiroDisponivel,
                                              HabilitarFiltrarConteudo = obj.HabilitarFiltrarConteudo,
                                              FiltrarConteudoTextoInicial = obj.FiltrarConteudoTextoInicial,
                                              FiltrarConteudoTextoFinal = obj.FiltrarConteudoTextoFinal,
                                              TipoFiltrarConteudo = obj.TipoFiltrarConteudo,
                                              RemoverCaracteres = obj.RemoverCaracteres,
                                              SubstituirVirgulaPorPonto = obj.SubstituirVirgulaPorPonto
                                          }).ToList(),
                    AutorizadosDownloadDFe = (from obj in grupoPessoas.AutorizadosDownloadDFe
                                              orderby obj.Nome
                                              select new
                                              {
                                                  Pessoa = new
                                                  {
                                                      obj.Codigo,
                                                      obj.Descricao
                                                  }
                                              }).ToList(),
                    Clientes = (from obj in grupoPessoas.Clientes
                                select new
                                {
                                    Codigo = obj.CPF_CNPJ,
                                    CPF_CNPJ = obj.CPF_CNPJ_Formatado,
                                    obj.Nome,
                                    Localidade = obj.Localidade.DescricaoCidadeEstado
                                }).ToList(),
                    RecebedorColeta = new
                    {
                        Codigo = grupoPessoas.RecebedorColeta?.Codigo,
                        Descricao = grupoPessoas.RecebedorColeta?.Descricao
                    },
                    TipoOperacaoColeta = new
                    {
                        Codigo = grupoPessoas.TipoOperacaoColeta?.Codigo,
                        Descricao = grupoPessoas.TipoOperacaoColeta?.Descricao
                    },
                    ConfiguracaoLayoutEDI = (from obj in grupoPessoas.LayoutsEDI
                                             orderby obj.LayoutEDI.Descricao
                                             select selectConfiguracaoLayoutEDI(obj, servicoAnexoLayoutEDI, listaAnexoLayoutEDI, unidadeDeTrabalho)).ToList(),
                    ConfiguracaoEmissaoCTe = new
                    {
                        TipoIntegracaoMercadoLivre = grupoPessoas.TipoIntegracaoMercadoLivre ?? TipoIntegracaoMercadoLivre.HandlingUnit,
                        grupoPessoas.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente,
                        grupoPessoas.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente,
                        grupoPessoas.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida,
                        TempoAcrescimoDecrescimoDataPrevisaoSaida = $"{(int)grupoPessoas.TempoAcrescimoDecrescimoDataPrevisaoSaida.TotalHours:d3}:{grupoPessoas.TempoAcrescimoDecrescimoDataPrevisaoSaida:mm}",
                        grupoPessoas.DisponibilizarDocumentosParaNFsManual,
                        grupoPessoas.ValorFreteLiquidoDeveSerValorAReceberSemICMS,
                        grupoPessoas.ValorFreteLiquidoDeveSerValorAReceber,
                        grupoPessoas.GerarCIOTParaTodasAsCargas,
                        ValorMaximoEmissaoPendentePagamento = (grupoPessoas.ValorMaximoEmissaoPendentePagamento ?? 0m).ToString("n2"),
                        ValorLimiteFaturamento = (grupoPessoas.ValorLimiteFaturamento ?? 0m).ToString("n2"),
                        DiasEmAbertoAposVencimento = (grupoPessoas.DiasEmAbertoAposVencimento ?? 0).ToString("n0"),
                        TipoEnvioEmail = grupoPessoas.TipoEnvioEmail ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe.Normal,
                        grupoPessoas.ObservacaoEmissaoCarga,
                        grupoPessoas.DescricaoItemPesoCTeSubcontratacao,
                        grupoPessoas.CaracteristicaTransporteCTe,
                        grupoPessoas.ImportarRedespachoIntermediario,
                        EmitenteImportacaoRedespachoIntermediario = new
                        {
                            Codigo = grupoPessoas.EmitenteImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                            Descricao = grupoPessoas.EmitenteImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                        },
                        ExpedidorImportacaoRedespachoIntermediario = new
                        {
                            Codigo = grupoPessoas.ExpedidorImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                            Descricao = grupoPessoas.ExpedidorImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                        },
                        RecebedorImportacaoRedespachoIntermediario = new
                        {
                            Codigo = grupoPessoas.RecebedorImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                            Descricao = grupoPessoas.RecebedorImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                        },
                        grupoPessoas.GerarMDFeTransbordoSemConsiderarOrigem,
                        grupoPessoas.BloquearDiferencaValorFreteEmbarcador,
                        PercentualBloquearDiferencaValorFreteEmbarcador = grupoPessoas.PercentualBloquearDiferencaValorFreteEmbarcador.ToString("n2"),
                        grupoPessoas.EmitirComplementoDiferencaFreteEmbarcador,
                        TipoOcorrenciaComplementoDiferencaFreteEmbarcador = new
                        {
                            Descricao = grupoPessoas.TipoOcorrenciaComplementoDiferencaFreteEmbarcador?.Descricao ?? string.Empty,
                            Codigo = grupoPessoas.TipoOcorrenciaComplementoDiferencaFreteEmbarcador?.Codigo ?? 0
                        },
                        grupoPessoas.GerarOcorrenciaSemTabelaFrete,
                        TipoOcorrenciaSemTabelaFrete = new
                        {
                            Descricao = grupoPessoas.TipoOcorrenciaSemTabelaFrete?.Descricao ?? string.Empty,
                            Codigo = grupoPessoas.TipoOcorrenciaSemTabelaFrete?.Codigo ?? 0
                        },
                        TipoOcorrenciaCTeEmitidoEmbarcador = new
                        {
                            Descricao = grupoPessoas.TipoOcorrenciaCTeEmitidoEmbarcador?.Descricao ?? string.Empty,
                            Codigo = grupoPessoas.TipoOcorrenciaCTeEmitidoEmbarcador?.Codigo ?? 0
                        },
                        NaoValidarNotaFiscalExistente = grupoPessoas.NaoValidarNotaFiscalExistente,
                        AgruparMovimentoFinanceiroPorPedido = grupoPessoas.AgruparMovimentoFinanceiroPorPedido,
                        NaoValidarNotasFiscaisComDiferentesPortos = grupoPessoas.NaoValidarNotasFiscaisComDiferentesPortos,
                        grupoPessoas.ValePedagioObrigatorio,
                        Diretorio = grupoPessoasIntegracaoFTP?.Diretorio ?? string.Empty,
                        EnderecoFTP = grupoPessoasIntegracaoFTP?.EnderecoFTP ?? string.Empty,
                        Passivo = grupoPessoasIntegracaoFTP?.Passivo ?? false,
                        UtilizarSFTP = grupoPessoasIntegracaoFTP?.UtilizarSFTP ?? false,
                        SSL = grupoPessoasIntegracaoFTP?.SSL ?? false,
                        Porta = grupoPessoasIntegracaoFTP?.Porta ?? string.Empty,
                        Senha = grupoPessoasIntegracaoFTP?.Senha ?? string.Empty,
                        NomenclaturaArquivo = grupoPessoasIntegracaoFTP?.NomenclaturaArquivo ?? string.Empty,
                        Usuario = grupoPessoasIntegracaoFTP?.Usuario ?? string.Empty,
                        ModeloDocumentoFiscal = grupoPessoas.ModeloDocumentoFiscal != null ? new { grupoPessoas.ModeloDocumentoFiscal.Codigo, grupoPessoas.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                        EmpresaEmissora = grupoPessoas.EmpresaEmissora != null ? new { grupoPessoas.EmpresaEmissora.Codigo, Descricao = grupoPessoas.EmpresaEmissora.RazaoSocial + " (" + grupoPessoas.EmpresaEmissora.Localidade.DescricaoCidadeEstado + ")" } : new { Codigo = 0, Descricao = "" },
                        EmitirEmpresaFixa = grupoPessoas.EmpresaEmissora != null ? true : false,
                        CobrarOutroDocumento = grupoPessoas.ModeloDocumentoFiscal != null ? true : false,
                        CTeEmitidoNoEmbarcador = grupoPessoas.CTeEmitidoNoEmbarcador,
                        ExigirNumeroPedido = grupoPessoas.ExigirNumeroPedido,
                        RegexValidacaoNumeroPedidoEmbarcador = grupoPessoas.RegexValidacaoNumeroPedidoEmbarcador,
                        TipoRateioDocumentos = grupoPessoas.TipoEmissaoCTeDocumentos,
                        TipoEmissaoCTeParticipantes = grupoPessoas.TipoEmissaoCTeParticipantes,
                        TipoEmissaoIntramunicipal = grupoPessoas.TipoEmissaoIntramunicipal,
                        TipoIntegracao = grupoPessoas.TipoIntegracao?.Tipo ?? TipoIntegracao.NaoInformada,
                        grupoPessoas.DescricaoComponenteFreteEmbarcador,
                        TempoCarregamento = $"{(int)grupoPessoas.TempoCarregamento.TotalHours:d3}:{grupoPessoas.TempoCarregamento:mm}",
                        TempoDescarregamento = $"{(int)grupoPessoas.TempoDescarregamento.TotalHours:d3}:{grupoPessoas.TempoDescarregamento:mm}",
                        grupoPessoas.UtilizarOutroModeloDocumentoEmissaoMunicipal,
                        grupoPessoas.NaoEmitirMDFe,
                        grupoPessoas.ProvisionarDocumentos,
                        grupoPessoas.GerarSomenteUmaProvisaoCadaCargaCompleta,
                        grupoPessoas.EscriturarSomenteDocumentosEmitidosParaNFe,
                        grupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao,
                        grupoPessoas.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento,
                        grupoPessoas.DisponibilizarDocumentosParaPagamento,
                        grupoPessoas.QuitarDocumentoAutomaticamenteAoGerarLote,
                        grupoPessoas.TornarPedidosPrioritarios,
                        grupoPessoas.GerarOcorrenciaComplementoSubcontratacao,
                        grupoPessoas.ObrigatorioInformarMDFeEmitidoPeloEmbarcador,
                        TipoOcorrenciaComplementoSubcontratacao = new
                        {
                            Codigo = grupoPessoas.TipoOcorrenciaComplementoSubcontratacao?.Codigo ?? 0,
                            Descricao = grupoPessoas.TipoOcorrenciaComplementoSubcontratacao?.Descricao
                        },
                        ModeloDocumentoFiscalEmissaoMunicipal = new
                        {
                            Codigo = grupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal?.Codigo ?? 0,
                            Descricao = grupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal?.Descricao ?? string.Empty
                        },
                        ArquivoImportacaoNotasFiscais = new
                        {
                            Codigo = grupoPessoas.ArquivoImportacaoNotaFiscal?.Codigo ?? 0,
                            Descricao = grupoPessoas.ArquivoImportacaoNotaFiscal?.Descricao ?? string.Empty
                        },
                        FormulaRateioFrete = new
                        {
                            Codigo = grupoPessoas.RateioFormula?.Codigo ?? 0,
                            Descricao = grupoPessoas.RateioFormula?.Descricao ?? string.Empty
                        },
                        ApolicesSeguro = (from obj in grupoPessoas.ApolicesSeguro
                                          select new
                                          {
                                              obj.Codigo,
                                              Seguradora = obj.Seguradora.Nome,
                                              obj.NumeroApolice,
                                              obj.NumeroAverbacao,
                                              Responsavel = obj.DescricaoResponsavel,
                                              Vigencia = obj.InicioVigencia.ToString("dd/MM/yyyy") + " até " + obj.FimVigencia.ToString("dd/MM/yyyy")
                                          }).ToList(),
                        ComponentesFrete = (from obj in grupoPessoas.GrupoPessoasConfiguracaoComponentesFretes
                                            select new
                                            {
                                                Codigo = obj.ComponenteFrete.Codigo,
                                                ComponenteFrete = new { obj.ComponenteFrete.Codigo, obj.ComponenteFrete.Descricao },
                                                CobrarOutroDocumento = obj.ModeloDocumentoFiscal != null ? true : false,
                                                ModeloDocumentoFiscal = obj.ModeloDocumentoFiscal != null ? new { obj.ModeloDocumentoFiscal.Codigo, obj.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                                                ImprimirOutraDescricaoCTe = !string.IsNullOrEmpty(obj.OutraDescricaoCTe) ? true : false,
                                                DescricaoCTe = obj.OutraDescricaoCTe,
                                                UsarOutraFormulaRateio = obj.RateioFormula != null ? true : false,
                                                FormulaRateioFrete = obj.RateioFormula != null ? new { obj.RateioFormula.Codigo, obj.RateioFormula.Descricao } : new { Codigo = 0, Descricao = "" },
                                                obj.IncluirICMS,
                                                obj.IncluirIntegralmenteContratoFreteTerceiro
                                            }).ToList(),
                        ClientesBloqueados = (from obj in grupoPessoas.ClientesBloquearEmissaoDosDestinatario
                                              select new
                                              {
                                                  obj.Codigo,
                                                  CPF_CNPJ = obj.CPF_CNPJ,
                                                  obj.Nome
                                              }).ToList(),
                        grupoPessoas.TipoPropostaMultimodal,
                        grupoPessoas.TipoServicoMultimodal,
                        grupoPessoas.ModalPropostaMultimodal,
                        grupoPessoas.TipoCobrancaMultimodal,
                        grupoPessoas.BloquearEmissaoDeEntidadeSemCadastro,
                        grupoPessoas.BloquearEmissaoDosDestinatario,
                        Observacao = grupoPessoas.ObservacaoCTe,
                        ObservacaoTerceiro = grupoPessoas.ObservacaoCTeTerceiro,
                        grupoPessoas.NaoPermitirVincularCTeComplementarEmCarga,
                        UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao = grupoPessoas.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false
                    },
                    ListaRaizCNPJ = grupoPessoas.RaizesCNPJ != null ? (from obj in grupoPessoas.RaizesCNPJ
                                                                       orderby obj.RaizCNPJ
                                                                       select new
                                                                       {
                                                                           Codigo = obj.Codigo,
                                                                           AdicionarPessoasMesmaRaiz = obj.AdicionarPessoasMesmaRaiz,
                                                                           RaizCNPJ = obj.RaizCNPJ
                                                                       }).ToList() : null,
                    ConfiguracaoFatura = new
                    {
                        PermiteFinalSemana = grupoPessoas.PermiteFinalDeSemana,
                        TipoPrazoFaturamento = grupoPessoas.TipoPrazoFaturamento,
                        FormaGeracaoTituloFatura = grupoPessoas.FormaGeracaoTituloFatura,
                        DiasDePrazoFatura = grupoPessoas.DiasDePrazoFatura,
                        ExigeCanhotoFisico = grupoPessoas.ExigeCanhotoFisico ?? false,
                        ArmazenaCanhotoFisicoCTe = grupoPessoas.ArmazenaCanhotoFisicoCTe ?? false,
                        SomenteOcorrenciasFinalizadoras = grupoPessoas.SomenteOcorrenciasFinalizadoras,
                        FaturarSomenteOcorrenciasFinalizadoras = grupoPessoas.FaturarSomenteOcorrenciasFinalizadoras,
                        NaoGerarFaturaAteReceberCanhotos = grupoPessoas.NaoGerarFaturaAteReceberCanhotos,
                        Banco = grupoPessoas.Banco != null ? new { grupoPessoas.Banco.Codigo, grupoPessoas.Banco.Descricao } : new { Codigo = 0, Descricao = "" },
                        Agencia = grupoPessoas.Agencia,
                        Digito = grupoPessoas.DigitoAgencia,
                        NumeroConta = grupoPessoas.NumeroConta,
                        TipoConta = grupoPessoas.TipoContaBanco,
                        TomadorFatura = grupoPessoas.ClienteTomadorFatura != null ? new { Codigo = grupoPessoas.ClienteTomadorFatura.CPF_CNPJ, Descricao = grupoPessoas.ClienteTomadorFatura.Nome } : new { Codigo = 0.0, Descricao = "" },
                        ObservacaoFatura = grupoPessoas.ObservacaoFatura,
                        FormaPagamento = new
                        {
                            Codigo = grupoPessoas.FormaPagamento?.Codigo ?? 0,
                            Descricao = grupoPessoas.FormaPagamento?.Descricao ?? string.Empty
                        },
                        GerarTituloPorDocumentoFiscal = grupoPessoas.GerarTituloPorDocumentoFiscal,
                        BoletoConfiguracao = new
                        {
                            Codigo = grupoPessoas.BoletoConfiguracao?.Codigo ?? 0,
                            Descricao = grupoPessoas.BoletoConfiguracao?.Descricao ?? string.Empty
                        },
                        EnviarBoletoPorEmailAutomaticamente = grupoPessoas.EnviarBoletoPorEmailAutomaticamente,
                        EnviarDocumentacaoFaturamentoCTe = grupoPessoas.EnviarDocumentacaoFaturamentoCTe,
                        NaoValidarPossuiAcordoFaturamentoAvancoCarga = grupoPessoas.NaoValidarPossuiAcordoFaturamentoAvancoCarga,
                        grupoPessoas.GerarTituloAutomaticamente,
                        grupoPessoas.GerarFaturaAutomaticaCte,
                        grupoPessoas.GerarFaturamentoAVista,
                        grupoPessoas.AssuntoEmailFatura,
                        grupoPessoas.CorpoEmailFatura,
                        grupoPessoas.GerarBoletoAutomaticamente,
                        grupoPessoas.EnviarArquivosDescompactados,
                        grupoPessoas.NaoEnviarEmailFaturaAutomaticamente,
                        grupoPessoas.GerarFaturaPorCte,
                        grupoPessoas.TipoEnvioFatura,
                        grupoPessoas.TipoAgrupamentoFatura,
                        grupoPessoas.FormaTitulo,
                        DiasSemanaFatura = grupoPessoas.DiasSemanaFatura.Select(o => o).ToList(),
                        DiasMesFatura = grupoPessoas.DiasMesFatura.Select(o => o).ToList(),
                        grupoPessoas.EmailEnvioDocumentacao,
                        grupoPessoas.TipoAgrupamentoEnvioDocumentacao,
                        grupoPessoas.AssuntoDocumentacao,
                        grupoPessoas.CorpoEmailDocumentacao,
                        grupoPessoas.EmailFatura,
                        grupoPessoas.HabilitarPeriodoVencimentoEspecifico,

                        InformarEmailEnvioDocumentacao = !string.IsNullOrWhiteSpace(grupoPessoas.EmailEnvioDocumentacao),
                        grupoPessoas.FormaEnvioDocumentacao,
                        grupoPessoas.EmailEnvioDocumentacaoPorta,
                        grupoPessoas.TipoAgrupamentoEnvioDocumentacaoPorta,
                        grupoPessoas.AssuntoDocumentacaoPorta,
                        grupoPessoas.CorpoEmailDocumentacaoPorta,
                        grupoPessoas.GerarFaturamentoMultiplaParcela,
                        grupoPessoas.QuantidadeParcelasFaturamento,
                        grupoPessoas.AvisoVencimetoHabilitarConfiguracaoPersonalizada,
                        grupoPessoas.AvisoVencimetoQunatidadeDias,
                        grupoPessoas.AvisoVencimetoEnviarDiariamente,
                        grupoPessoas.CobrancaHabilitarConfiguracaoPersonalizada,
                        grupoPessoas.CobrancaQunatidadeDias,
                        grupoPessoas.AvisoVencimetoNaoEnviarEmail,
                        grupoPessoas.CobrancaNaoEnviarEmail,
                        InformarEmailEnvioDocumentacaoPorta = !string.IsNullOrWhiteSpace(grupoPessoas.EmailEnvioDocumentacaoPorta),
                        grupoPessoas.FormaEnvioDocumentacaoPorta,

                        FaturaVencimentos = (from vencimento in grupoPessoasFaturaVencimentos
                                             select new
                                             {
                                                 vencimento.Codigo,
                                                 vencimento.DiaInicial,
                                                 vencimento.DiaFinal,
                                                 vencimento.DiaVencimento
                                             }).ToList(),

                        GerarTituloAutomaticamenteComAdiantamentoSaldo = grupoPessoas.ConfiguracaoFatura?.GerarTituloAutomaticamenteComAdiantamentoSaldo ?? false,
                        PercentualAdiantamentoTituloAutomatico = grupoPessoas.ConfiguracaoFatura?.PercentualAdiantamentoTituloAutomatico.ToString("n2") ?? string.Empty,
                        PrazoAdiantamentoEmDiasTituloAutomatico = grupoPessoas.ConfiguracaoFatura?.PrazoAdiantamentoEmDiasTituloAutomatico.ToString("n0") ?? string.Empty,
                        PercentualSaldoTituloAutomatico = grupoPessoas.ConfiguracaoFatura?.PercentualSaldoTituloAutomatico.ToString("n2") ?? string.Empty,
                        PrazoSaldoEmDiasTituloAutomatico = grupoPessoas.ConfiguracaoFatura?.PrazoSaldoEmDiasTituloAutomatico.ToString("n0") ?? string.Empty,
                        EfetuarImpressaoDaTaxaDeMoedaEstrangeira = grupoPessoas.ConfiguracaoFatura?.EfetuarImpressaoDaTaxaDeMoedaEstrangeira ?? false,
                        EnvioCanhoto = (from canhoto in grupoPessoasFaturaCanhoto
                                        select selectFaturaCanhoto(canhoto, servicoAnexoFaturaCanhoto, listaAnexoFaturaCanhoto, unidadeDeTrabalho)).ToList(),
                        ContasBancarias = (from obj in grupoPessoas.ContasBancarias
                                           select new
                                           {
                                               Codigo = obj.Codigo,
                                               Descricao = (obj.Banco?.Descricao ?? "") + " / " + (obj.NumeroConta ?? "")
                                           }).ToList(),
                        grupoPessoas.UtilizarCadastroContaBancaria

                    },
                    Contatos = (from obj in grupoPessoas.Contatos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Contato,
                                    obj.Email,
                                    Situacao = obj.Ativo,
                                    obj.Telefone,
                                    TipoContato = obj.TiposContato.Select(o => o.Codigo).ToList(),
                                    obj.DescricaoTipoContato,
                                    obj.DescricaoSituacao
                                }).ToList(),
                    ListaVendedores = (from obj in grupoPessoas.Vendedores
                                       where TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ?
                                        obj.Funcionario.Empresa.Codigo == this.Usuario.Empresa.Codigo : true
                                       select new
                                       {
                                           obj.Codigo,
                                           CodigoFuncionario = obj.Funcionario.Codigo,
                                           Funcionario = obj.Funcionario.Nome,
                                           PercentualComissao = obj.PercentualComissao.ToString("n5"),
                                           DataInicioVigencia = obj.DataInicioVigencia.HasValue ? obj.DataInicioVigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                                           DataFimVigencia = obj.DataFimVigencia.HasValue ? obj.DataFimVigencia.Value.ToString("dd/MM/yyyy") : string.Empty
                                       }).ToList(),
                    ListaMensagemAlerta = (from obj in grupoPessoas.MensagemAlerta
                                           select new
                                           {
                                               obj.Codigo,
                                               Tag = obj.Tag,
                                               MensagemAlerta = obj.MensagemAlerta
                                           }).ToList(),
                    grupoPessoas.ObservacaoNfe,
                    FormulasObservacaoNfe = (
                        from formula in grupoPessoas.FormulasObservacaoNfe
                        select new
                        {
                            formula.Codigo,
                            formula.Descricao,
                            formula.IdentificadorFim,
                            formula.IdentificadorInicio,
                            formula.QtdMaximoDigitos,
                            formula.QtdMinimoDigitos,
                            formula.NumeroPedido,
                            formula.NumeroContainer,
                            formula.TaraContainer,
                            formula.LacreContainerUm,
                            formula.LacreContainerDois,
                            formula.LacreContainerTres,
                            formula.NumeroControleCliente,
                            formula.NumeroControlePedido,
                            formula.NumeroReferenciaEDI,
                            SubstituicaoTributaria = false,
                            formula.Tag
                        }
                    ).ToList(),
                    UtilizaMultiEmbarcador = grupoPessoas.UtilizaMultiEmbarcador ?? false,
                    PerfilChamado = new
                    {
                        grupoPessoas.ClienteNaoNecessitaAutorizacaoAtendimento,
                        grupoPessoas.GeraNumeroOcorrenciaAutorizacao,
                        grupoPessoas.DiaPrazoCobrancaChamado,
                        grupoPessoas.QuantidadeMaximaDiasDataReciboAbertura,
                        ValorMaximoDiferencaRecibo = grupoPessoas.ValorMaximoDiferencaRecibo.ToString("n2"),
                        ValorLimiteFaturamento = (grupoPessoas.ValorLimiteFaturamento ?? 0m).ToString("n2"),
                        DiasEmAbertoAposVencimento = (grupoPessoas.DiasEmAbertoAposVencimento ?? 0).ToString("n0"),
                        grupoPessoas.AssuntoEmailChamado,
                        grupoPessoas.CorpoEmailChamado,
                        grupoPessoas.MensagemPadraoOrientacaoMotorista,
                        grupoPessoas.FormaValorDescarga,
                        grupoPessoas.FormaAberturaOcorrencia,
                        grupoPessoas.TipoPrazoCobrancaChamado,
                        grupoPessoas.NivelToleranciaValorCliente,
                        grupoPessoas.NivelToleranciaValorMotorista
                    },
                    TabelaValores = (from obj in grupoPessoas.TabelaValores
                                     select new
                                     {
                                         obj.Codigo,
                                         Valor = obj.Valor.ToString("n2"),
                                         ModeloVeicularCarga = new
                                         {
                                             Codigo = obj.ModeloVeicularCarga?.Codigo ?? 0,
                                             Descricao = obj.ModeloVeicularCarga?.Descricao ?? string.Empty
                                         }
                                     }).ToList(),
                    ObservacoesCTes = observacoesCTe.Select(o => new
                    {
                        o.Codigo,
                        o.Identificador,
                        o.Tipo,
                        o.Texto,
                        DescricaoTipo = o.Tipo.ObterDescricao()
                    }).ToList(),
                    Anexos = (
                        from anexo in grupoPessoas.Anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList(),
                    Fornecedor = new
                    {
                        grupoPessoas.GerarDuplicataNotaEntrada,
                        grupoPessoas.UtilizarParametrizacaoDeHorariosNoAgendamento,
                        grupoPessoas.DiaPadraoDuplicataNotaEntrada,
                        grupoPessoas.ParcelasDuplicataNotaEntrada,
                        grupoPessoas.IntervaloDiasDuplicataNotaEntrada,
                        grupoPessoas.IgnorarDuplicataRecebidaXMLNotaEntrada,
                        grupoPessoas.PermitirMultiplosVencimentos,
                        grupoPessoas.FormaTituloFornecedor,
                        TabelaMultiplosVencimentos = (
                                from vencimento in grupoPessoas.Vencimentos
                                select new
                                {
                                    DataEmissao = vencimento.DataEmissao.ToString(),
                                    Codigo = vencimento.Codigo.ToString(),
                                    Vencimento = vencimento.Vencimento.ToString(),
                                    DiaEmissaoFinal = vencimento.DiaEmissaoFinal.ToString(),
                                    DiaEmissaoInicial = vencimento.DiaEmissaoInicial.ToString()
                                }
                            ).ToList()
                    },
                    NCMPalletsNFe = (
                        from ncmPallet in grupoPessoas.NCMsPallet
                        select new
                        {
                            ncmPallet.Codigo,
                            ncmPallet.NCM
                        }
                    ).ToList(),
                    LogoGrupoPessoas = servicoGrupoPessoa.ObterLogoBase64(codigo, unidadeDeTrabalho),
                    ExigirComprovantesLiberacaoPagamentoContratoFrete = grupoPessoas.ExigirComprovantesLiberacaoPagamentoContratoFrete,
                    Comprovantes = (
                        from obj in grupoPessoas.TiposComprovante
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao
                        }
                    ).ToList(),

                };

                return new JsonpResult(dynGrupoPessoas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Descrição", Propriedade = "Descricao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "*Código Integração", Propriedade = "CodigoIntegracao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Raiz CNPJ", Propriedade = "RaizCnpj", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Parametrização de Horários", Propriedade = "ParametrizacaoDeHorarios", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Classificação", Propriedade = "Classificacao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Lista de Vendedores", Propriedade = "ListaVendedores", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });


            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                Servicos.Embarcador.Pessoa.GrupoPessoa servicoGrupoPessoa = new Servicos.Embarcador.Pessoa.GrupoPessoa(unitOfWork, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoGrupoPessoa.Importar(linhas);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
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

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasEmissao repConfiguracaoGrupoPessoasEmissao = new Repositorio.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasEmissao(unitOfWork);
                Repositorio.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasFatura repConfiguracaoGrupoPessoasFatura = new Repositorio.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasFatura(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto repGrupoPessoasFaturaCanhoto = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> grupoPessoasCanhotos = repGrupoPessoasFaturaCanhoto.BuscarPorGrupoPessoasAsync(codigo, default).Result;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigo, true);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> repAnexoGrupoPessoasFaturaCanhoto = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto> listaAnexoCanhoto = repAnexoGrupoPessoasFaturaCanhoto.BuscarPorEntidades(grupoPessoasCanhotos.Select(o => o.Codigo).ToList());


                if (grupoPessoas == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                foreach (Dominio.Entidades.Cliente cliente in grupoPessoas.Clientes)
                {
                    cliente.GrupoPessoas = null;
                    cliente.DataUltimaAtualizacao = DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Atualizar(cliente);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, "Removeu do Grupo de Pessoa " + grupoPessoas.Descricao + ".", unitOfWork);
                }
                grupoPessoas.ApolicesSeguro.Clear();
                grupoPessoas.LayoutsEDI = null;
                if (grupoPessoas.ConfiguracaoEmissao != null)
                    repConfiguracaoGrupoPessoasEmissao.Deletar(grupoPessoas.ConfiguracaoEmissao);
                if (grupoPessoas.ConfiguracaoFatura != null)
                    repConfiguracaoGrupoPessoasFatura.Deletar(grupoPessoas.ConfiguracaoFatura);
                foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto faturaCanhoto in grupoPessoasCanhotos)
                {
                    foreach (Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto anexoCanhoto in listaAnexoCanhoto)
                        repAnexoGrupoPessoasFaturaCanhoto.Deletar(anexoCanhoto);
                    repGrupoPessoasFaturaCanhoto.Deletar(faturaCanhoto);
                }

                repGrupoPessoas.Deletar(grupoPessoas, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Bloquear()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo) || motivo.Length < 20)
                    return new JsonpResult(false, true, "O motivo do bloqueio deve ter no mínimo 20 caracteres.");

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigo, true);

                unitOfWork.Start();

                grupoPessoas.Bloqueado = true;
                grupoPessoas.MotivoBloqueio = motivo;
                grupoPessoas.DataBloqueio = DateTime.Now;

                repGrupoPessoas.Atualizar(grupoPessoas, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao desbloquear o grupo de pessoas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Desbloquear()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigo, true);

                unitOfWork.Start();

                grupoPessoas.Bloqueado = false;
                grupoPessoas.MotivoBloqueio = null;
                grupoPessoas.DataBloqueio = null;

                repGrupoPessoas.Atualizar(grupoPessoas, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao desbloquear o grupo de pessoas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarLogo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                Servicos.Embarcador.Pessoa.GrupoPessoa servicoGrupoPessoa = new Servicos.Embarcador.Pessoa.GrupoPessoa();

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigo);

                if (grupoPessoas == null)
                    return new JsonpResult(false, true, "O grupo de pessoas não foi encontrado.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoLogo");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhuma logo selecionada para adicionar.");

                Servicos.DTO.CustomFile file = arquivos.FirstOrDefault();

                long byteCount = file.Length;
                if ((byteCount / 1024) > 60)
                    return new JsonpResult(false, true, "Tamanho máximo permitido para a logo é de 60 KB.");

                string extensaoArquivo = Path.GetExtension(file.FileName).ToLowerInvariant();
                string caminho = ObterCaminhoArquivoLogo(unitOfWork);
                string nomeArquivoFotoExistente = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(nomeArquivoFotoExistente))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivoFotoExistente);

                file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{codigo}{extensaoArquivo}"));

                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, $"Alterou a logo do grupo de pessoas.", unitOfWork);

                return new JsonpResult(new { LogoGrupoPessoas = servicoGrupoPessoa.ObterLogoBase64(codigo, unitOfWork) });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a logo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirLogo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigo);

                if (grupoPessoas == null)
                    return new JsonpResult(false, true, "O grupo de pessoas não foi encontrado.");

                string caminho = ObterCaminhoArquivoLogo(unitOfWork);
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                    return new JsonpResult(false, true, "Logo não encontrada.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Removeu a logo do grupo de pessoas.", unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover a logo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherGrupoPessoas(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaClassificacao repositorioPessoaClassificacao = new Repositorio.Embarcador.Pessoas.PessoaClassificacao(unitOfWork);

            bool? utilizaMultiEmbarcador = Request.GetNullableBoolParam("UtilizaMultiEmbarcador");
            bool informarProdutoPredominante = Request.GetBoolParam("InformarProdutoPredominante");
            string produtoPredominante = Request.Params("ProdutoPredominante");

            bool naoImportarDocumentosDestinadosTransporte, validaEmitenteNFe, validaPlacaNFe, validaDestinoNFe, validaOrigemNFe, enviarXMLCTePorEmail, armazenaProdutosXMLNFE;
            bool.TryParse(Request.Params("NaoImportarDocumentosDestinadosTransporte"), out naoImportarDocumentosDestinadosTransporte);
            bool.TryParse(Request.Params("ValidaPlacaNFe"), out validaPlacaNFe);
            bool.TryParse(Request.Params("ValidaOrigemNFe"), out validaOrigemNFe);

            bool.TryParse(Request.Params("ValidaDestinoNFe"), out validaDestinoNFe);
            bool.TryParse(Request.Params("ValidaEmitenteNFe"), out validaEmitenteNFe);
            bool.TryParse(Request.Params("ArmazenaProdutosXMLNFE"), out armazenaProdutosXMLNFE);
            bool.TryParse(Request.Params("EnviarXMLCTePorEmail"), out enviarXMLCTePorEmail);
            bool.TryParse(Request.Params("ExigirRotaParaEmissaoDocumentos"), out bool exigirRotaParaEmissaoDocumentos);
            bool.TryParse(Request.Params("ExigirNumeroControleCliente"), out bool exigirNumeroControleCliente);
            bool.TryParse(Request.Params("ExigirNumeroNumeroReferenciaCliente"), out bool exigirNumeroNumeroReferenciaCliente);
            bool.TryParse(Request.Params("ReplicarNumeroReferenciaTodasNotasCarga"), out bool replicarNumeroReferenciaTodasNotasCarga);
            bool.TryParse(Request.Params("ReplicarNumeroControleCliente"), out bool ReplicarNumeroControleCliente);
            bool.TryParse(Request.Params("GerarPedidoColeta"), out bool gerarPedidoColeta);
            bool.TryParse(Request.Params("ExigirComprovantesLiberacaoPagamentoContratoFrete"), out bool ExigirComprovantesLiberacaoPagamentoContratoFrete);


            bool gerarOcorrenciaControleEntrega = Request.GetBoolParam("GerarOcorrenciaControleEntrega");

            int codigoRecebedorColeta = Request.GetIntParam("RecebedorColeta");
            int codigoTipoOperacaoColeta = Request.GetIntParam("TipoOperacaoColeta");
            int codigoClassificacao = Request.GetIntParam("Classificacao");
            int transportador = Request.GetIntParam("transportador");

            Dominio.Entidades.Cliente recebedorColeta = codigoRecebedorColeta > 0 ? repCliente.BuscarPorCPFCNPJ(codigoRecebedorColeta) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoColeta = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacaoColeta);

            bool.TryParse(Request.Params("ExigirRotaCalculoFreteParaEmissaoDocumentos"), out bool exigirRotaCalculoFreteParaEmissaoDocumentos);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoPessoas tipoGrupoPessoas;
            Enum.TryParse(Request.Params("TipoGrupoPessoas"), out tipoGrupoPessoas);

            string email = Request.Params("Email");
            string erro = string.Empty;

            if (!string.IsNullOrWhiteSpace(email) && !Utilidades.Email.ValidarEmails(out erro, email, ';'))
                throw new ControllerException(erro);
            grupoPessoas.LerNumeroPedidoObservacaoCTe = Request.GetBoolParam("LerNumeroPedidoObservacaoCTe");
            grupoPessoas.LerNumeroPedidoObservacaoMDFe = Request.GetBoolParam("LerNumeroPedidoObservacaoMDFe");
            grupoPessoas.SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe = Request.GetBoolParam("SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe");
            grupoPessoas.LerNumeroCargaObservacaoCTeSubcontratacao = Request.GetBoolParam("LerNumeroCargaObservacaoCTeSubcontratacao");
            grupoPessoas.RegexNumeroCargaObservacaoCTeSubcontratacao = Request.GetStringParam("RegexNumeroCargaObservacaoCTeSubcontratacao");
            grupoPessoas.RegexNumeroPedidoObservacaoCTe = Request.GetStringParam("RegexNumeroPedidoObservacaoCTe");
            grupoPessoas.RegexNumeroPedidoObservacaoMDFe = Request.GetStringParam("RegexNumeroPedidoObservacaoMDFe");
            grupoPessoas.VincularCTePeloNumeroPedido = Request.GetBoolParam("VincularCTePeloNumeroPedido");
            grupoPessoas.VincularMDFePeloNumeroPedido = Request.GetBoolParam("VincularMDFePeloNumeroPedido");
            grupoPessoas.LerNumeroPedidoObservacaoCTeSubcontratacao = Request.GetBoolParam("LerNumeroPedidoObservacaoCTeSubcontratacao");
            grupoPessoas.VincularCTeSubcontratacaoPeloNumeroPedido = Request.GetBoolParam("VincularCTeSubcontratacaoPeloNumeroPedido");
            grupoPessoas.RegexNumeroPedidoObservacaoCTeSubcontratacao = Request.GetStringParam("RegexNumeroPedidoObservacaoCTeSubcontratacao");
            grupoPessoas.UtilizaMetaEmissao = Request.GetBoolParam("UtilizaMetaEmissao");
            grupoPessoas.LerPDFNotaFiscalRecebidaPorEmail = Request.GetBoolParam("LerPDFNotaFiscalRecebidaPorEmail");
            grupoPessoas.MetaEmissaoMensal = Request.GetNullableDecimalParam("MetaEmissaoMensal");
            grupoPessoas.MetaEmissaoAnual = Request.GetNullableDecimalParam("MetaEmissaoAnual");
            grupoPessoas.UtilizaMultiEmbarcador = utilizaMultiEmbarcador;
            grupoPessoas.ExigirRotaCalculoFreteParaEmissaoDocumentos = exigirRotaCalculoFreteParaEmissaoDocumentos;
            grupoPessoas.ExigirRotaParaEmissaoDocumentos = exigirRotaParaEmissaoDocumentos;
            grupoPessoas.ExigirNumeroControleCliente = exigirNumeroControleCliente;
            grupoPessoas.ReplicarNumeroControleCliente = ReplicarNumeroControleCliente;
            grupoPessoas.ExigirNumeroNumeroReferenciaCliente = exigirNumeroNumeroReferenciaCliente;
            grupoPessoas.ReplicarNumeroReferenciaTodasNotasCarga = replicarNumeroReferenciaTodasNotasCarga;

            grupoPessoas.GerarPedidoColeta = gerarPedidoColeta;
            grupoPessoas.GerarNumeroConhecimentoNoBoleto = Request.GetBoolParam("GerarNumeroConhecimentoNoBoleto");
            grupoPessoas.GerarNumeroFaturaNoBoleto = Request.GetBoolParam("GerarNumeroFaturaNoBoleto");
            grupoPessoas.GerarOcorrenciaControleEntrega = gerarOcorrenciaControleEntrega;
            grupoPessoas.PermitirConsultarOcorrenciaControleEntregaWebService = Request.GetBoolParam("PermitirConsultarOcorrenciaControleEntregaWebService");
            grupoPessoas.ParquearDocumentosAutomaticamenteParaCNPJDesteGrupo = Request.GetBoolParam("ParquearDocumentosAutomaticamenteParaCNPJDesteGrupo");
            grupoPessoas.Empresa = repEmpresa.BuscarPorCodigo(transportador);
            grupoPessoas.Ativo = bool.Parse(Request.Params("Ativo"));
            grupoPessoas.Descricao = Request.Params("Descricao");
            grupoPessoas.NaoImportarDocumentoDestinadoTransporte = naoImportarDocumentosDestinadosTransporte;
            grupoPessoas.ValidaPlacaNFe = validaPlacaNFe;
            grupoPessoas.ValidaDestinoNFe = validaDestinoNFe;
            grupoPessoas.ValidaOrigemNFe = validaOrigemNFe;
            grupoPessoas.ArmazenaProdutosXMLNFE = armazenaProdutosXMLNFE;
            grupoPessoas.ValidaEmitenteNFe = validaEmitenteNFe;
            grupoPessoas.NaoAdicionarNotaNCMPalletCarga = Request.GetBoolParam("NaoAdicionarNotaNCMPalletCarga");
            grupoPessoas.ZerarPesoNotaNCMPalletCarga = Request.GetBoolParam("ZerarPesoNotaNCMPalletCarga");
            grupoPessoas.NaoAlterarDocumentoIntegracao = Request.GetBoolParam("NaoAlterarDocumentoIntegracao");

            grupoPessoas.EnviarXMLCTePorEmail = enviarXMLCTePorEmail;
            grupoPessoas.Email = email;
            grupoPessoas.EnviarNovoVeiculoEmail = Request.GetBoolParam("EnviarNovoVeiculoEmail");
            grupoPessoas.EmailEnvioNovoVeiculo = Request.GetStringParam("EmailEnvioNovoVeiculo");
            grupoPessoas.TipoGrupoPessoas = tipoGrupoPessoas;
            grupoPessoas.CodigoIntegracao = Request.Params("CodigoIntegracao");
            grupoPessoas.ObservacaoNfe = Request.GetStringParam("ObservacaoNfe");
            grupoPessoas.RecebedorColeta = recebedorColeta;
            grupoPessoas.TipoOperacaoColeta = tipoOperacaoColeta;
            grupoPessoas.Contato = Request.GetStringParam("Contato");
            grupoPessoas.TelefoneContato = Request.GetStringParam("TelefoneContato");

            grupoPessoas.LerPlacaDaObservacaoDaNota = Request.GetBoolParam("LerPlacaDaObservacaoDaNota");
            grupoPessoas.LerPlacaDaObservacaoDaNotaInicio = Request.GetStringParam("LerPlacaDaObservacaoDaNotaInicio");
            grupoPessoas.LerPlacaDaObservacaoDaNotaFim = Request.GetStringParam("LerPlacaDaObservacaoDaNotaFim");
            grupoPessoas.LerPlacaDaObservacaoContribuinteDaNota = Request.GetBoolParam("LerPlacaDaObservacaoContribuinteDaNota");
            grupoPessoas.LerPlacaDaObservacaoContribuinteDaNotaIdentificacao = Request.GetStringParam("LerPlacaDaObservacaoContribuinteDaNotaIdentificacao");
            grupoPessoas.VincularNotaFiscalEmailNaCarga = Request.GetBoolParam("VincularNotaFiscalEmailNaCarga");

            grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento = Request.GetBoolParam("LerVeiculoObservacaoNotaParaAbastecimento");
            grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial = Request.Params("LerPlacaObservacaoNotaParaAbastecimentoInicial");
            grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoFinal = Request.Params("LerPlacaObservacaoNotaParaAbastecimentoFinal");
            grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial = Request.Params("LerKMObservacaoNotaParaAbastecimentoInicial");
            grupoPessoas.LerKMObservacaoNotaParaAbastecimentoFinal = Request.Params("LerKMObservacaoNotaParaAbastecimentoFinal");

            grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial = Request.Params("LerHorimetroObservacaoNotaParaAbastecimentoInicial");
            grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoFinal = Request.Params("LerHorimetroObservacaoNotaParaAbastecimentoFinal");
            grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoInicial = Request.GetStringParam("LerChassiObservacaoNotaParaAbastecimentoInicial");
            grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoFinal = Request.GetStringParam("LerChassiObservacaoNotaParaAbastecimentoFinal");

            grupoPessoas.LerNumeroPedidoDaObservacaoDaNota = Request.GetBoolParam("LerNumeroPedidoDaObservacaoDaNota");
            grupoPessoas.LerNumeroPedidoDaObservacaoDaNotaInicio = Request.Params("LerNumeroPedidoDaObservacaoDaNotaInicio");
            grupoPessoas.LerNumeroPedidoDaObservacaoDaNotaFim = Request.Params("LerNumeroPedidoDaObservacaoDaNotaFim");

            grupoPessoas.ControlaPagamentos = Request.GetBoolParam("ControlaPagamentos");

            grupoPessoas.ExpressaoBooking = Request.Params("ExpressaoBooking");
            grupoPessoas.ExpressaoContainer = Request.Params("ExpressaoContainer");

            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && grupoPessoas.Codigo == 0)
            {
                grupoPessoas.VincularNotaFiscalEmailNaCarga = true;
                if (string.IsNullOrWhiteSpace(grupoPessoas.ExpressaoBooking))
                    grupoPessoas.ExpressaoBooking = @"\d[a-zA-Z]{3}(AK|AM|([a-zA-Z]{2}))\d{0,5}[a-zA-Z]{0,4}\w";
                if (string.IsNullOrWhiteSpace(grupoPessoas.ExpressaoContainer))
                    grupoPessoas.ExpressaoContainer = @"(?<=\s)*([a-z]{3})u[- /\.]*((\d{7}|\d{6}[- /\.]*\d)|(\d{3}[- /\.]*\d{3}[- /\.]*\d))(?=\s)*";
            }

            grupoPessoas.HabilitarIntegracaoVeiculoMultiEmbarcador = Request.GetNullableBoolParam("HabilitarIntegracaoVeiculoMultiEmbarcador");
            grupoPessoas.HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador = Request.GetNullableBoolParam("HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador");
            grupoPessoas.HabilitarIntegracaoOcorrenciasMultiEmbarcador = Request.GetNullableBoolParam("HabilitarIntegracaoOcorrenciasMultiEmbarcador");
            grupoPessoas.HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador = Request.GetNullableBoolParam("HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador");
            grupoPessoas.HabilitarIntegracaoXmlCteMultiEmbarcador = Request.GetNullableBoolParam("HabilitarIntegracaoXmlCteMultiEmbarcador");
            grupoPessoas.DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador = Request.GetNullableDateTimeParam("DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador");
            grupoPessoas.NaoGerarOcorreciaApenasDocumentos = Request.GetBoolParam("NaoGerarOcorreciaApenasDocumentos");
            grupoPessoas.TokenIntegracaoMultiEmbarcador = Request.GetStringParam("TokenIntegracaoMultiEmbarcador");
            grupoPessoas.URLIntegracaoMultiEmbarcador = Request.GetStringParam("URLIntegracaoMultiEmbarcador");

            grupoPessoas.RateioFormulaExclusivo = repFormulaRateio.BuscarPorCodigo(Request.GetIntParam("RateioFormulaExclusivo"));
            grupoPessoas.TipoEmissaoCTeDocumentosExclusivo = Request.GetEnumParam("TipoEmissaoCTeDocumentosExclusivo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado);
            grupoPessoas.RegraCotacaoFeeder = Request.GetEnumParam("RegraCotacaoFeeder", Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraCotacaoFeeder.Nenhuma);
            grupoPessoas.Classificacao = (codigoClassificacao > 0) ? repositorioPessoaClassificacao.BuscarPorCodigo(codigoClassificacao, auditavel: false) : null;
            grupoPessoas.NomeNomenclaturaArquivosDownloadCTe = Request.GetStringParam("NomeNomenclaturaArquivosDownloadCTe");
            grupoPessoas.ProdutoPredominante = informarProdutoPredominante ? produtoPredominante : null;
            grupoPessoas.ExigirComprovantesLiberacaoPagamentoContratoFrete = ExigirComprovantesLiberacaoPagamentoContratoFrete;
            grupoPessoas.Prioridade = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeGrupoPessoas>("Prioridade");
        }

        private void PreencherDadosAdicionaisGrupoPessoas(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            dynamic dynDadosAdicionais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DadosAdicionais"));
            if (dynDadosAdicionais != null)
            {
                int.TryParse((string)dynDadosAdicionais.PedidoTipoPagamento, out int codigoPedidoTipoPagamento);

                grupoPessoas.PedidoTipoPagamento = codigoPedidoTipoPagamento > 0 ? repPedidoTipoPagamento.BuscarPorCodigo(codigoPedidoTipoPagamento) : null;

                grupoPessoas.ControlaPallets = ((string)dynDadosAdicionais.ControlaPallets).ToBool();
                grupoPessoas.CobrancaDiaria = ((string)dynDadosAdicionais.CobrancaDiaria).ToBool();
                grupoPessoas.CobrancaDescarga = ((string)dynDadosAdicionais.CobrancaDescarga).ToBool();
                grupoPessoas.CobrancaCarregamento = ((string)dynDadosAdicionais.CobrancaCarregamento).ToBool();
                grupoPessoas.EnviarAutomaticamenteDocumentacaoCarga = ((string)dynDadosAdicionais.EnviarAutomaticamenteDocumentacaoCarga).ToBool();
                grupoPessoas.CobrancaDiariaObservacao = dynDadosAdicionais.CobrancaDiariaObservacao;
                grupoPessoas.CobrancaDescargaObservacao = dynDadosAdicionais.CobrancaDescargaObservacao;
                grupoPessoas.CobrancaCarregamentoObservacao = dynDadosAdicionais.CobrancaCarregamentoObservacao;
                grupoPessoas.NaoGerarArquivoVgm = ((string)dynDadosAdicionais.NaoGerarArquivoVgm).ToBool();
                grupoPessoas.GerarImpressaoOrdemColetaExclusiva = ((string)dynDadosAdicionais.GerarImpressaoOrdemColetaExclusiva).ToBool();
                grupoPessoas.NaoEnviarParaDocsys = ((string)dynDadosAdicionais.NaoEnviarParaDocsys).ToBool();
                grupoPessoas.TipoImpressaoOrdemColetaExclusiva = ((string)dynDadosAdicionais.TipoImpressaoOrdemColetaExclusiva).ToEnum<TipoImpressaoOrdemColetaExclusiva>();
                grupoPessoas.EmailDespachante = ((string)dynDadosAdicionais.EmailDespachante);
                double codigoDespachante = ((string)dynDadosAdicionais.Despachante).ToDouble();
                grupoPessoas.Despachante = codigoDespachante > 0 ? repositorioCliente.BuscarPorCPFCNPJ(codigoDespachante) : null;
                grupoPessoas.AdicionarDespachanteComoConsignatario = ((string)dynDadosAdicionais.AdicionarDespachanteComoConsignatario).ToBool();
                grupoPessoas.NaoEnviarXMLCteSubcontratacaoOuRedespachoPorEmail = ((string)dynDadosAdicionais.NaoEnviarXMLCteSubcontratacaoOuRedespachoPorEmail).ToBool();


                SituacaoFinanceira? situacaoFinanceiraAnterior = grupoPessoas.SituacaoFinanceira;
                grupoPessoas.SituacaoFinanceira = ((string)dynDadosAdicionais.SituacaoFinanceira).ToNullableEnum<SituacaoFinanceira>();
                if (grupoPessoas.SituacaoFinanceira.HasValue && grupoPessoas.SituacaoFinanceira != situacaoFinanceiraAnterior)
                    grupoPessoas.DataAlteracaoSituacaoFinanceira = DateTime.Now;

                grupoPessoas.ClassificacaoEmpresa = dynDadosAdicionais.ClassificacaoEmpresa;
            }
        }

        private void SalvarOcorrencias(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic ocorrencias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Ocorrencias"));
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia tipoOcorrencia;
            Repositorio.TipoDeOcorrenciaDeCTe repOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia repGrupoPessoaOcorrencia = new Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia> listaOcorrencias = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia>();
            listaOcorrencias = repGrupoPessoaOcorrencia.BuscarTodosPorEntidade(grupoPessoas);

            foreach (dynamic ocorrenciaExistente in listaOcorrencias)
            {
                repGrupoPessoaOcorrencia.Deletar(ocorrenciaExistente);
            }

            foreach (dynamic ocorrencia in ocorrencias)
            {
                tipoOcorrencia = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia();
                tipoOcorrencia.CodigoIntegracao = ((string)ocorrencia.CodigoIntegracao).ToString();
                tipoOcorrencia.GrupoPessoa = grupoPessoas;
                tipoOcorrencia.TipoOcorrencia = repOcorrencia.BuscarPorCodigo((int)ocorrencia.CodigoTipoOcorrencia);
                repGrupoPessoaOcorrencia.Inserir(tipoOcorrencia);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Adicionou o tipo de ocorrencia " + ocorrencia.CodigoIntegracao + ".", unitOfWork);
            }
        }

        private void SalvarEmails(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic emailsDocumentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EmailsDocumentos"));
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail modeloDocumentoEmail;
            Repositorio.ModeloDocumentoFiscal repModeloDocFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasEmailDocumento repGrupoPessoaEmailDocumento = new Repositorio.Embarcador.Pessoas.GrupoPessoasEmailDocumento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail> listaEmailDocumentos = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail>();
            listaEmailDocumentos = repGrupoPessoaEmailDocumento.BuscarTodosPorEntidade(grupoPessoas);

            foreach (dynamic emailDocumentoExistente in listaEmailDocumentos)
            {
                repGrupoPessoaEmailDocumento.Deletar(emailDocumentoExistente);
            }

            foreach (dynamic documentoEmail in emailsDocumentos)
            {
                modeloDocumentoEmail = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail();
                modeloDocumentoEmail.Emails = ((string)documentoEmail.Emails).ToString();
                modeloDocumentoEmail.GrupoPessoas = grupoPessoas;
                modeloDocumentoEmail.ModeloDocumentoFiscal = repModeloDocFiscal.BuscarPorId((int)documentoEmail.CodigoModeloDocumento);
                repGrupoPessoaEmailDocumento.Inserir(modeloDocumentoEmail);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Adicionou o Tipo Documento " + documentoEmail.ModeloDocumentoFiscal + ".", unitOfWork);
            }
        }

        private void SalvarLeituraDinamicaXML(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic leiturasDinamicaXML = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LeituraDinamicaXML"));

            Repositorio.Embarcador.Pessoas.LeituraDinamicaXmlOrigem repLeituraDinamicaXmlOrigem = new Repositorio.Embarcador.Pessoas.LeituraDinamicaXmlOrigem(unitOfWork);
            Repositorio.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha repLeituraDinamicaXmlOrigemTagFilha = new Repositorio.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha(unitOfWork);
            Repositorio.Embarcador.Pessoas.LeituraDinamicaXmlDestino repLeituraDinamicaXmlDestino = new Repositorio.Embarcador.Pessoas.LeituraDinamicaXmlDestino(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml repGrupoPessoasLeituraDinamicaXml = new Repositorio.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml> listaGrupoPessoasLeituraDinamicaXml = repGrupoPessoasLeituraDinamicaXml.BuscarPorGrupoPessoas(grupoPessoas.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml grupoPessoasLeituraDinamicaXml in listaGrupoPessoasLeituraDinamicaXml)
            {
                repGrupoPessoasLeituraDinamicaXml.Deletar(grupoPessoasLeituraDinamicaXml);
            }

            foreach (dynamic leituraDinamicaXML in leiturasDinamicaXML)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml grupoPessoasLeituraDinamicaXml = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLeituraDinamicaXml();
                grupoPessoasLeituraDinamicaXml.GrupoPessoas = grupoPessoas;
                grupoPessoasLeituraDinamicaXml.TipoDocumento = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento)leituraDinamicaXML.TipoDocumento;
                grupoPessoasLeituraDinamicaXml.Descricao = ((string)leituraDinamicaXML.Descricao).ToString();
                grupoPessoasLeituraDinamicaXml.LeituraDinamicaXmlOrigem = repLeituraDinamicaXmlOrigem.BuscarPorCodigo((int)leituraDinamicaXML.CodigoLeituraDinamicaXmlOrigem);
                grupoPessoasLeituraDinamicaXml.LeituraDinamicaXmlOrigemTagFilha = repLeituraDinamicaXmlOrigemTagFilha.BuscarPorCodigo(((int?)leituraDinamicaXML.CodigoLeituraDinamicaXmlOrigemTagFilha) ?? 0);
                grupoPessoasLeituraDinamicaXml.LeituraDinamicaXmlDestino = repLeituraDinamicaXmlDestino.BuscarPorCodigo((int)leituraDinamicaXML.CodigoLeituraDinamicaXmlDestino);
                grupoPessoasLeituraDinamicaXml.FiltrarTag = ((string)leituraDinamicaXML.FiltrarTag).ToString();
                grupoPessoasLeituraDinamicaXml.FiltrarPrimeiroDisponivel = bool.Parse((string)leituraDinamicaXML.FiltrarPrimeiroDisponivel);
                grupoPessoasLeituraDinamicaXml.HabilitarFiltrarConteudo = bool.Parse((string)leituraDinamicaXML.HabilitarFiltrarConteudo);
                grupoPessoasLeituraDinamicaXml.FiltrarConteudoTextoInicial = ((string)leituraDinamicaXML.FiltrarConteudoTextoInicial).ToString();
                grupoPessoasLeituraDinamicaXml.FiltrarConteudoTextoFinal = ((string)leituraDinamicaXML.FiltrarConteudoTextoFinal).ToString();
                grupoPessoasLeituraDinamicaXml.TipoFiltrarConteudo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFiltrarConteudo)leituraDinamicaXML.TipoFiltrarConteudo;
                grupoPessoasLeituraDinamicaXml.RemoverCaracteres = ((string)leituraDinamicaXML.RemoverCaracteres).ToString();
                grupoPessoasLeituraDinamicaXml.SubstituirVirgulaPorPonto = bool.Parse((string)leituraDinamicaXML.SubstituirVirgulaPorPonto);
                repGrupoPessoasLeituraDinamicaXml.Inserir(grupoPessoasLeituraDinamicaXml);
            }
        }

        private void SalvarModelosReboque(ref Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            dynamic modelosReboque = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosReboque"));

            List<int> codigos = new List<int>();


            foreach (dynamic modelo in modelosReboque)
                codigos.Add((int)modelo.Codigo);

            if (grupoPessoas.ModelosReboque == null)
                grupoPessoas.ModelosReboque = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosDeletar = grupoPessoas.ModelosReboque.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloDeletar in modelosDeletar)
                {
                    grupoPessoas.ModelosReboque.Remove(modeloDeletar);

                    //Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Removeu o modelo veicular " + modeloDeletar.Descricao + ".", unitOfWork);
                }
            }

            foreach (int codigoModelo in codigos)
            {
                if (!grupoPessoas.ModelosReboque.Any(o => o.Codigo == codigoModelo))
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(codigoModelo);

                    grupoPessoas.ModelosReboque.Add(modeloVeicularCarga);

                    //Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Adicionou o modelo veicular " + modeloVeicularCarga.Descricao + ".", unitOfWork);
                }
            }

            repGrupoPessoas.Atualizar(grupoPessoas);
        }

        private void SalvarAutorizadosDownloadDFe(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic autorizadosDownloadDFe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("AutorizadosDownloadDFe"));

            List<double> cpfCnpjs = new List<double>();

            foreach (dynamic autorizado in autorizadosDownloadDFe)
                cpfCnpjs.Add((double)autorizado.Pessoa.Codigo);

            if (grupoPessoas.AutorizadosDownloadDFe == null)
                grupoPessoas.AutorizadosDownloadDFe = new List<Dominio.Entidades.Cliente>();
            else
            {
                List<Dominio.Entidades.Cliente> autorizadosDeletar = grupoPessoas.AutorizadosDownloadDFe.Where(o => !cpfCnpjs.Contains(o.CPF_CNPJ)).ToList();

                foreach (Dominio.Entidades.Cliente autorizadoDeletar in autorizadosDeletar)
                {
                    grupoPessoas.AutorizadosDownloadDFe.Remove(autorizadoDeletar);

                    //Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Removeu o autorizado para download do DF-e " + autorizadoDeletar.Descricao + ".", unitOfWork);
                }
            }

            foreach (double cpfCnpj in cpfCnpjs)
            {
                if (!grupoPessoas.AutorizadosDownloadDFe.Any(o => o.CPF_CNPJ == cpfCnpj))
                {
                    Dominio.Entidades.Cliente autorizadoDownload = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                    grupoPessoas.AutorizadosDownloadDFe.Add(autorizadoDownload);

                    //Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Adicionou o autorizado para download do DF-e " + autorizadoDownload.Descricao + ".", unitOfWork);
                }
            }

            repGrupoPessoas.Atualizar(grupoPessoas);
        }

        private string SalvarListaRaizCNPJ(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ repGrupoPessoasRaizCNPJ = new Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ> listaRaizCNPJ = repGrupoPessoasRaizCNPJ.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
            for (int i = 0; i < listaRaizCNPJ.Count(); i++)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Raíz CNPJ " + listaRaizCNPJ[i].Descricao + " removido do grupo.", unidadeDeTrabalho);
                repGrupoPessoasRaizCNPJ.Deletar(listaRaizCNPJ[i], Auditado);
            }

            dynamic listaRaiz = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaRaizCNPJ"));
            if (listaRaiz != null)
            {
                foreach (dynamic raizCNPJ in listaRaiz)
                {

                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasExiste = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers((string)raizCNPJ.RaizCNPJ));
                    if (grupoPessoasExiste == null || string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers((string)raizCNPJ.RaizCNPJ)) || (grupoPessoasExiste.Codigo == grupoPessoas.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ raiz = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ();
                        raiz.RaizCNPJ = Utilidades.String.OnlyNumbers((string)raizCNPJ.RaizCNPJ);
                        raiz.AdicionarPessoasMesmaRaiz = bool.Parse((string)raizCNPJ.AdicionarPessoasMesmaRaiz);
                        raiz.GrupoPessoas = grupoPessoas;
                        repGrupoPessoasRaizCNPJ.Inserir(raiz, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Raíz CNPJ " + raiz.Descricao + " adicionada ao grupo.", unidadeDeTrabalho);

                        if (raiz.AdicionarPessoasMesmaRaiz && int.Parse((string)raizCNPJ.Codigo) == 0)
                            AdicionarPessoasDaRaiz(grupoPessoas, unidadeDeTrabalho, raiz.RaizCNPJ);
                    }
                    else
                    {
                        return Utilidades.String.OnlyNumbers((string)raizCNPJ.RaizCNPJ); ;
                    }
                }
            }
            return "";
        }

        private void AdicionarPessoasDaRaiz(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unidadeDeTrabalho, string raizCNPJ)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            List<Dominio.Entidades.Cliente> listaClientes = repCliente.BuscarPorRaizCNPJ(raizCNPJ, grupoPessoas.Codigo);

            if (grupoPessoas.Clientes == null)
                grupoPessoas.Clientes = new List<Dominio.Entidades.Cliente>();

            for (int i = 0; i < listaClientes.Count(); i++)
            {
                if (!grupoPessoas.Clientes.Contains(listaClientes[i]))
                {
                    Dominio.Entidades.Cliente cliente = listaClientes[i];
                    if (cliente.GrupoPessoas == null)
                    {
                        cliente.GrupoPessoas = grupoPessoas;
                        cliente.DataUltimaAtualizacao = DateTime.Now;
                        cliente.Integrado = false;
                        repCliente.Atualizar(cliente);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cliente, null, "Adicionou ao Grupo de Pessoa " + grupoPessoas.Descricao + ".", unidadeDeTrabalho);
                    }
                }
            }
        }

        private dynamic SalvarConfiguracaoEmissaoCTe(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoEmissao))
                return null;

            if (grupoPessoas.ApolicesSeguro == null)
                grupoPessoas.ApolicesSeguro = new List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro>();
            if (grupoPessoas.ClientesBloquearEmissaoDosDestinatario == null)
                grupoPessoas.ClientesBloquearEmissaoDosDestinatario = new List<Dominio.Entidades.Cliente>();

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal repArquivoImportacaoNotaFiscal = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP repGrupoPessoasIntegracaoFTP = new Repositorio.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP(unidadeDeTrabalho);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP grupoPessoasIntegracaoFTP = repGrupoPessoasIntegracaoFTP.BuscarPorGrupoPessoasAsync(grupoPessoas.Codigo, default).Result;

            dynamic configuracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoEmissaoCTe"));

            int codigoTipoOcorrenciaComplementoSubcontratacao = (int)configuracao.TipoOcorrenciaComplementoSubcontratacao;

            grupoPessoas.DisponibilizarDocumentosParaNFsManual = (bool)configuracao.DisponibilizarDocumentosParaNFsManual;
            grupoPessoas.TipoOcorrenciaComplementoSubcontratacao = codigoTipoOcorrenciaComplementoSubcontratacao > 0 ? repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrenciaComplementoSubcontratacao) : null;
            grupoPessoas.GerarOcorrenciaComplementoSubcontratacao = (bool)configuracao.GerarOcorrenciaComplementoSubcontratacao;
            grupoPessoas.ValorFreteLiquidoDeveSerValorAReceber = (bool)configuracao.ValorFreteLiquidoDeveSerValorAReceber;
            grupoPessoas.ValorFreteLiquidoDeveSerValorAReceberSemICMS = (bool)configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS;
            grupoPessoas.ValorMaximoEmissaoPendentePagamento = Utilidades.Decimal.Converter((string)configuracao.ValorMaximoEmissaoPendentePagamento);
            grupoPessoas.ValorLimiteFaturamento = Utilidades.Decimal.Converter((string)configuracao.ValorLimiteFaturamento);
            int.TryParse((string)configuracao.DiasEmAbertoAposVencimento, out int diasEmAbertoAposVencimento);
            grupoPessoas.DiasEmAbertoAposVencimento = diasEmAbertoAposVencimento;
            grupoPessoas.TipoEnvioEmail = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioEmailCTe)configuracao.TipoEnvioEmail;
            grupoPessoas.ObservacaoEmissaoCarga = (string)configuracao.ObservacaoEmissaoCarga;
            grupoPessoas.GerarMDFeTransbordoSemConsiderarOrigem = (bool)configuracao.GerarMDFeTransbordoSemConsiderarOrigem;
            grupoPessoas.NaoValidarNotaFiscalExistente = (bool)configuracao.NaoValidarNotaFiscalExistente;
            grupoPessoas.NaoValidarNotasFiscaisComDiferentesPortos = (bool)configuracao.NaoValidarNotasFiscaisComDiferentesPortos;
            grupoPessoas.AgruparMovimentoFinanceiroPorPedido = (bool)configuracao.AgruparMovimentoFinanceiroPorPedido;
            grupoPessoas.ValePedagioObrigatorio = (bool)configuracao.ValePedagioObrigatorio;
            grupoPessoas.NaoEmitirMDFe = (bool)configuracao.NaoEmitirMDFe;
            grupoPessoas.ProvisionarDocumentos = (bool)configuracao.ProvisionarDocumentos;
            grupoPessoas.GerarSomenteUmaProvisaoCadaCargaCompleta = (bool)configuracao.GerarSomenteUmaProvisaoCadaCargaCompleta;
            grupoPessoas.DisponibilizarDocumentosParaLoteEscrituracao = (bool)configuracao.DisponibilizarDocumentosParaLoteEscrituracao;
            grupoPessoas.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento = (bool)configuracao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento;
            grupoPessoas.DisponibilizarDocumentosParaPagamento = (bool)configuracao.DisponibilizarDocumentosParaPagamento;
            grupoPessoas.QuitarDocumentoAutomaticamenteAoGerarLote = ((string)configuracao.QuitarDocumentoAutomaticamenteAoGerarLote).ToBool();
            grupoPessoas.EscriturarSomenteDocumentosEmitidosParaNFe = (bool)configuracao.EscriturarSomenteDocumentosEmitidosParaNFe;
            grupoPessoas.TornarPedidosPrioritarios = (bool)configuracao.TornarPedidosPrioritarios;
            grupoPessoas.CTeEmitidoNoEmbarcador = (bool)configuracao.CTeEmitidoNoEmbarcador;
            grupoPessoas.ExigirNumeroPedido = (bool)configuracao.ExigirNumeroPedido;
            grupoPessoas.RegexValidacaoNumeroPedidoEmbarcador = (string)configuracao.RegexValidacaoNumeroPedidoEmbarcador;
            grupoPessoas.TipoEmissaoCTeDocumentos = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos)configuracao.TipoRateioDocumentos;
            grupoPessoas.TipoEmissaoCTeParticipantes = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes)configuracao.TipoEmissaoCTeParticipantes;
            grupoPessoas.TipoEmissaoIntramunicipal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal)configuracao.TipoEmissaoIntramunicipal;
            grupoPessoas.RateioFormula = repFormulaRateio.BuscarPorCodigo((int)configuracao.FormulaRateioFrete);
            grupoPessoas.TipoIntegracao = repTipoIntegracao.BuscarPorTipo((Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao)(int)configuracao.TipoIntegracao);
            grupoPessoas.ArquivoImportacaoNotaFiscal = repArquivoImportacaoNotaFiscal.BuscarPorCodigo((int)configuracao.ArquivoImportacaoNotasFiscais);
            grupoPessoas.DescricaoComponenteFreteEmbarcador = (string)configuracao.DescricaoComponenteFreteEmbarcador;
            grupoPessoas.TempoCarregamento = RetornarTimeSpan((string)configuracao.TempoCarregamento);
            grupoPessoas.TempoDescarregamento = RetornarTimeSpan((string)configuracao.TempoDescarregamento);
            grupoPessoas.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId((int)configuracao.ModeloDocumentoFiscal);
            grupoPessoas.EmpresaEmissora = repEmpresa.BuscarPorCodigo((int)configuracao.EmpresaEmissora);
            grupoPessoas.BloquearDiferencaValorFreteEmbarcador = (bool)configuracao.BloquearDiferencaValorFreteEmbarcador;
            grupoPessoas.EmitirComplementoDiferencaFreteEmbarcador = (bool)configuracao.EmitirComplementoDiferencaFreteEmbarcador;
            grupoPessoas.GerarOcorrenciaSemTabelaFrete = (bool)configuracao.GerarOcorrenciaSemTabelaFrete;
            grupoPessoas.TipoOcorrenciaSemTabelaFrete = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaSemTabelaFrete);
            grupoPessoas.PercentualBloquearDiferencaValorFreteEmbarcador = (decimal)configuracao.PercentualBloquearDiferencaValorFreteEmbarcador;
            grupoPessoas.TipoOcorrenciaComplementoDiferencaFreteEmbarcador = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador);
            grupoPessoas.TipoOcorrenciaCTeEmitidoEmbarcador = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaCTeEmitidoEmbarcador);
            grupoPessoas.ObrigatorioInformarMDFeEmitidoPeloEmbarcador = grupoPessoas.CTeEmitidoNoEmbarcador ? (bool)configuracao.ObrigatorioInformarMDFeEmitidoPeloEmbarcador : false;

            double.TryParse((string)configuracao.EmitenteImportacaoRedespachoIntermediario, out double cpfCnpjEmitenteImportacaoRedespachoIntermediario);
            double.TryParse((string)configuracao.ExpedidorImportacaoRedespachoIntermediario, out double cpfCnpjExpedidorImportacaoRedespachoIntermediario);
            double.TryParse((string)configuracao.RecebedorImportacaoRedespachoIntermediario, out double cpfCnpjRecebedorImportacaoRedespachoIntermediario);

            grupoPessoas.ImportarRedespachoIntermediario = (bool)configuracao.ImportarRedespachoIntermediario;
            grupoPessoas.EmitenteImportacaoRedespachoIntermediario = cpfCnpjEmitenteImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjEmitenteImportacaoRedespachoIntermediario) : null;
            grupoPessoas.ExpedidorImportacaoRedespachoIntermediario = cpfCnpjExpedidorImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidorImportacaoRedespachoIntermediario) : null;
            grupoPessoas.RecebedorImportacaoRedespachoIntermediario = cpfCnpjRecebedorImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedorImportacaoRedespachoIntermediario) : null;

            grupoPessoas.DescricaoItemPesoCTeSubcontratacao = (string)configuracao.DescricaoItemPesoCTeSubcontratacao;
            grupoPessoas.CaracteristicaTransporteCTe = (string)configuracao.CaracteristicaTransporteCTe;
            grupoPessoas.ObservacaoCTe = (string)configuracao.Observacao;
            grupoPessoas.ObservacaoCTeTerceiro = (string)configuracao.ObservacaoTerceiro;
            grupoPessoas.GerarCIOTParaTodasAsCargas = (bool)configuracao.GerarCIOTParaTodasAsCargas;
            grupoPessoas.NaoPermitirVincularCTeComplementarEmCarga = (bool)configuracao.NaoPermitirVincularCTeComplementarEmCarga;

            if (grupoPessoas.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual)
            {
                grupoPessoas.UtilizarOutroModeloDocumentoEmissaoMunicipal = (bool)configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal;

                if (grupoPessoas.UtilizarOutroModeloDocumentoEmissaoMunicipal)
                    grupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal = repModeloDocumentoFiscal.BuscarPorId((int)configuracao.ModeloDocumentoFiscalEmissaoMunicipal);
                else
                    grupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal = null;
            }
            else
            {
                grupoPessoas.UtilizarOutroModeloDocumentoEmissaoMunicipal = false;
                grupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal = null;
            }

            List<int> codigosApolicesSeguro = new List<int>();

            for (int i = 0; i < configuracao.ApolicesSeguro.Count; i++)
                codigosApolicesSeguro.Add((int)configuracao.ApolicesSeguro[i]);

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesRemover = grupoPessoas.ApolicesSeguro.Where(o => !codigosApolicesSeguro.Contains(o.Codigo)).ToList();

            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceRemover in apolicesRemover)
            {
                grupoPessoas.ApolicesSeguro.Remove(apoliceRemover);

                //Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Removeu a apólice de seguro " + apoliceRemover.Descricao + ".", unidadeDeTrabalho);
            }

            foreach (int codigoApoliceSeguro in codigosApolicesSeguro)
            {
                if (!grupoPessoas.ApolicesSeguro.Any(o => o.Codigo == codigoApoliceSeguro))
                {
                    Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repApoliceSeguro.BuscarPorCodigo(codigoApoliceSeguro);

                    grupoPessoas.ApolicesSeguro.Add(apoliceSeguro);

                    //Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, "Adicionou a apólice de seguro " + apoliceSeguro.Descricao + ".", unidadeDeTrabalho);
                }
            }

            if (grupoPessoas.TipoIntegracao != null && grupoPessoas.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
            {
                if (grupoPessoasIntegracaoFTP == null)
                    grupoPessoasIntegracaoFTP = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasIntegracaoFTP();
                else
                    grupoPessoasIntegracaoFTP.Initialize();

                grupoPessoasIntegracaoFTP.GrupoPessoas = grupoPessoas;
                grupoPessoasIntegracaoFTP.Diretorio = (string)configuracao.Diretorio;
                grupoPessoasIntegracaoFTP.EnderecoFTP = (string)configuracao.EnderecoFTP;
                grupoPessoasIntegracaoFTP.Passivo = (bool)configuracao.Passivo;
                grupoPessoasIntegracaoFTP.UtilizarSFTP = (bool)configuracao.UtilizarSFTP;
                grupoPessoasIntegracaoFTP.SSL = (bool)configuracao.SSL;
                grupoPessoasIntegracaoFTP.Porta = (string)configuracao.Porta;
                grupoPessoasIntegracaoFTP.Senha = (string)configuracao.Senha;
                grupoPessoasIntegracaoFTP.Usuario = (string)configuracao.Usuario;
                grupoPessoasIntegracaoFTP.NomenclaturaArquivo = (string)configuracao.NomenclaturaArquivo;

                if (grupoPessoasIntegracaoFTP.Codigo > 0)
                {
                    repGrupoPessoasIntegracaoFTP.Atualizar(grupoPessoasIntegracaoFTP, Auditado);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = grupoPessoasIntegracaoFTP.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, alteracoes, "FTP " + grupoPessoasIntegracaoFTP.Descricao + " alterado.", unidadeDeTrabalho);
                }
                else
                {
                    repGrupoPessoasIntegracaoFTP.Inserir(grupoPessoasIntegracaoFTP, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "FTP " + grupoPessoasIntegracaoFTP.Descricao + " adicionado ao grupo.", unidadeDeTrabalho);
                }
            }
            else if (grupoPessoasIntegracaoFTP != null)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "FTP " + grupoPessoasIntegracaoFTP.Descricao + " removido do grupo.", unidadeDeTrabalho);
                repGrupoPessoasIntegracaoFTP.Deletar(grupoPessoasIntegracaoFTP, Auditado);
            }

            grupoPessoas.TipoPropostaMultimodal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal)configuracao.TipoPropostaMultimodal;
            grupoPessoas.TipoServicoMultimodal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal)configuracao.TipoServicoMultimodal;
            grupoPessoas.ModalPropostaMultimodal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal)configuracao.ModalPropostaMultimodal;
            grupoPessoas.TipoCobrancaMultimodal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal)configuracao.TipoCobrancaMultimodal;

            bool.TryParse((string)configuracao.BloquearEmissaoDeEntidadeSemCadastro, out bool bloquearEmissaoDeEntidadeSemCadastro);
            bool.TryParse((string)configuracao.BloquearEmissaoDosDestinatario, out bool bloquearEmissaoDosDestinatario);
            grupoPessoas.BloquearEmissaoDeEntidadeSemCadastro = bloquearEmissaoDeEntidadeSemCadastro;
            grupoPessoas.BloquearEmissaoDosDestinatario = bloquearEmissaoDosDestinatario;

            grupoPessoas.TipoIntegracaoMercadoLivre = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre)configuracao.TipoIntegracaoMercadoLivre;
            grupoPessoas.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente = (bool)configuracao.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente;
            grupoPessoas.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente = (bool)configuracao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente;
            grupoPessoas.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida)configuracao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida;
            grupoPessoas.TempoAcrescimoDecrescimoDataPrevisaoSaida = RetornarTimeSpan((string)configuracao.TempoAcrescimoDecrescimoDataPrevisaoSaida);

            List<double> codigosClientesBloqueio = new List<double>();

            for (int i = 0; i < configuracao.ClientesBloqueados.Count; i++)
                codigosClientesBloqueio.Add((double)configuracao.ClientesBloqueados[i]);

            List<Dominio.Entidades.Cliente> clientesRemover = grupoPessoas.ClientesBloquearEmissaoDosDestinatario.Where(o => !codigosClientesBloqueio.Contains(o.CPF_CNPJ)).ToList();

            foreach (Dominio.Entidades.Cliente clienteRemover in clientesRemover)
            {
                grupoPessoas.ClientesBloquearEmissaoDosDestinatario.Remove(clienteRemover);
            }

            foreach (double codigoClientesBloquei in codigosClientesBloqueio)
            {
                if (!grupoPessoas.ClientesBloquearEmissaoDosDestinatario.Any(o => o.Codigo == codigoClientesBloquei))
                {
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(codigoClientesBloquei);

                    grupoPessoas.ClientesBloquearEmissaoDosDestinatario.Add(cliente);
                }
            }

            SalvarConfiguracaoComponentesFrete(grupoPessoas, configuracao.ComponentesFrete, unidadeDeTrabalho);

            repGrupoPessoas.Atualizar(grupoPessoas);

            return configuracao;
        }

        private void SalvarConfiguracaoComponentesFrete(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, dynamic configuracaoComponentes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (configuracaoComponentes != null)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete repGrupoPessoasConfiguracaoComponentesFrete = new Repositorio.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete(unidadeDeTrabalho);
                Repositorio.Embarcador.Rateio.RateioFormula repRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete> grupoPessoasConfiguracaesComponentesFreteExcluir = repGrupoPessoasConfiguracaoComponentesFrete.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete grupoPessoasConfiguracaoComponentesFreteExcluir in grupoPessoasConfiguracaesComponentesFreteExcluir)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Configuração do componente " + grupoPessoasConfiguracaoComponentesFreteExcluir.Descricao + " removido do grupo.", unidadeDeTrabalho);
                    repGrupoPessoasConfiguracaoComponentesFrete.Deletar(grupoPessoasConfiguracaoComponentesFreteExcluir, Auditado);
                }

                for (int i = 0; i < configuracaoComponentes.Count; i++)
                {
                    dynamic dynConfiguracaoComponentes = configuracaoComponentes[i];

                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete grupoPessoasConfiguracaoComponentesFrete = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete();

                    grupoPessoasConfiguracaoComponentesFrete.GrupoPessoas = grupoPessoas;
                    grupoPessoasConfiguracaoComponentesFrete.ComponenteFrete = repComponenteFrete.BuscarPorCodigo((int)dynConfiguracaoComponentes.ComponenteFrete.Codigo);
                    if ((int)dynConfiguracaoComponentes.ModeloDocumentoFiscal.Codigo > 0)
                        grupoPessoasConfiguracaoComponentesFrete.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId((int)dynConfiguracaoComponentes.ModeloDocumentoFiscal.Codigo);

                    if ((int)dynConfiguracaoComponentes.FormulaRateioFrete.Codigo > 0)
                        grupoPessoasConfiguracaoComponentesFrete.RateioFormula = repRateioFormula.BuscarPorCodigo((int)dynConfiguracaoComponentes.FormulaRateioFrete.Codigo);

                    grupoPessoasConfiguracaoComponentesFrete.OutraDescricaoCTe = (string)dynConfiguracaoComponentes.DescricaoCTe;
                    grupoPessoasConfiguracaoComponentesFrete.IncluirICMS = (bool)dynConfiguracaoComponentes.IncluirICMS;
                    grupoPessoasConfiguracaoComponentesFrete.IncluirIntegralmenteContratoFreteTerceiro = (bool)dynConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;

                    repGrupoPessoasConfiguracaoComponentesFrete.Inserir(grupoPessoasConfiguracaoComponentesFrete, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Configuração do componente  " + grupoPessoasConfiguracaoComponentesFrete.Descricao + " adicionado ao grupo.", unidadeDeTrabalho);
                }
            }
        }

        private void AtualizarConfiguracoesEmissao(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, dynamic configuracao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (configuracao == null)
                return;

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasEmissao repConfiguracaoGrupoPessoasEmissao = new Repositorio.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasEmissao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasEmissao configuracaoGrupoPessoasEmissao = grupoPessoas.ConfiguracaoEmissao ?? new Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasEmissao();

            if (configuracaoGrupoPessoasEmissao.Codigo > 0)
                configuracaoGrupoPessoasEmissao.Initialize();

            configuracaoGrupoPessoasEmissao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao = ((string)configuracao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao).ToBool();

            if (configuracaoGrupoPessoasEmissao.Codigo == 0)
                repConfiguracaoGrupoPessoasEmissao.Inserir(configuracaoGrupoPessoasEmissao);
            else
                repConfiguracaoGrupoPessoasEmissao.Atualizar(configuracaoGrupoPessoasEmissao, Auditado, historico);

            grupoPessoas.ConfiguracaoEmissao = configuracaoGrupoPessoasEmissao;

            repGrupoPessoas.Atualizar(grupoPessoas);
        }

        private void SalvarLayoutsEDI(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unidadeDeTrabalho, ref IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosLayoutEDI)
        {
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoasLayoutEDI repGrupoPessoasLayoutEDI = new Repositorio.Embarcador.Pessoas.GrupoPessoasLayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> repAnexoGrupoPessoasLayoutEDI = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);

            dynamic layoutsEDI = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoLayoutEDI"));
            List<int> codigosExistentes = new List<int>();
            int codigo = 0;

            for (int i = 0; i < layoutsEDI.Count; i++)
                if (int.TryParse((string)layoutsEDI[i].Codigo, out codigo))
                    codigosExistentes.Add(codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsExistentes = repGrupoPessoasLayoutEDI.BuscarPorGrupoPessoas(grupoPessoas.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layoutsDeletar = (from obj in layoutsExistentes where !codigosExistentes.Contains(obj.Codigo) select obj).ToList();

            List<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI> listaAnexoLayoutEDI = repAnexoGrupoPessoasLayoutEDI.BuscarPorEntidades(layoutsDeletar.Select(o => o.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutDeletar in layoutsDeletar)
            {
                foreach (Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI anexoLayoutEDI in listaAnexoLayoutEDI.FindAll(x => x.EntidadeAnexo.Codigo == layoutDeletar.Codigo))
                    repAnexoGrupoPessoasLayoutEDI.Deletar(anexoLayoutEDI);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, layoutDeletar.GrupoPessoas, null, "Layout " + layoutDeletar.Descricao + " deletado do grupo.", unidadeDeTrabalho);
                layoutsExistentes.Remove(layoutDeletar);
                repGrupoPessoasLayoutEDI.Deletar(layoutDeletar);
            }

            for (int i = 0; i < layoutsEDI.Count; i++)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI grupoPessoasLayoutEDI = null;

                if (int.TryParse((string)layoutsEDI[i].Codigo, out codigo))
                    grupoPessoasLayoutEDI = (from obj in layoutsExistentes where obj.Codigo == codigo select obj).FirstOrDefault();

                if (grupoPessoasLayoutEDI == null)
                    grupoPessoasLayoutEDI = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI();
                else
                    grupoPessoasLayoutEDI.Initialize();

                grupoPessoasLayoutEDI.GrupoPessoas = grupoPessoas;
                grupoPessoasLayoutEDI.LayoutEDI = repLayoutEDI.Buscar((int)layoutsEDI[i].CodigoLayoutEDI);
                grupoPessoasLayoutEDI.TipoIntegracao = repTipoIntegracao.BuscarPorTipo((Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao)(int)layoutsEDI[i].TipoIntegracao);

                grupoPessoasLayoutEDI.EnderecoFTP = null;
                grupoPessoasLayoutEDI.Diretorio = null;
                grupoPessoasLayoutEDI.Passivo = false;
                grupoPessoasLayoutEDI.UtilizarSFTP = false;
                grupoPessoasLayoutEDI.SSL = false;
                grupoPessoasLayoutEDI.UtilizarLeituraArquivos = false;
                grupoPessoasLayoutEDI.AdicionarEDIFilaProcessamento = false;
                grupoPessoasLayoutEDI.CriarComNomeTemporaraio = false;
                grupoPessoasLayoutEDI.Porta = null;
                grupoPessoasLayoutEDI.Senha = null;
                grupoPessoasLayoutEDI.Usuario = null;
                grupoPessoasLayoutEDI.Emails = null;
                grupoPessoasLayoutEDI.EmailsAlertaLeituraEDI = null;

                if (grupoPessoasLayoutEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP)
                {
                    grupoPessoasLayoutEDI.EnderecoFTP = (string)layoutsEDI[i].EnderecoFTP;
                    grupoPessoasLayoutEDI.Diretorio = (string)layoutsEDI[i].Diretorio;
                    grupoPessoasLayoutEDI.Passivo = (bool)layoutsEDI[i].Passivo;
                    grupoPessoasLayoutEDI.UtilizarSFTP = (bool)layoutsEDI[i].UtilizarSFTP;
                    grupoPessoasLayoutEDI.SSL = (bool)layoutsEDI[i].SSL;
                    grupoPessoasLayoutEDI.UtilizarLeituraArquivos = (bool)layoutsEDI[i].UtilizarLeituraArquivos;
                    grupoPessoasLayoutEDI.AdicionarEDIFilaProcessamento = (bool)layoutsEDI[i].AdicionarEDIFilaProcessamento;
                    grupoPessoasLayoutEDI.CriarComNomeTemporaraio = (bool)layoutsEDI[i].CriarComNomeTemporaraio;
                    grupoPessoasLayoutEDI.Porta = (string)layoutsEDI[i].Porta;
                    grupoPessoasLayoutEDI.Senha = (string)layoutsEDI[i].Senha;
                    grupoPessoasLayoutEDI.Usuario = (string)layoutsEDI[i].Usuario;
                    grupoPessoasLayoutEDI.EmailsAlertaLeituraEDI = (string)layoutsEDI[i].EmailsAlertaLeituraEDI;
                }
                else if (grupoPessoasLayoutEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email)
                {
                    grupoPessoasLayoutEDI.Emails = (string)layoutsEDI[i].Emails;
                }

                if (grupoPessoasLayoutEDI.Codigo > 0)
                {
                    repGrupoPessoasLayoutEDI.Atualizar(grupoPessoasLayoutEDI, Auditado);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesLayout = grupoPessoasLayoutEDI.GetChanges();
                    if (alteracoesLayout.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoasLayoutEDI.GrupoPessoas, alteracoesLayout, "Alterou o Layout " + grupoPessoasLayoutEDI.Descricao + ".", unidadeDeTrabalho);
                }
                else
                {
                    repGrupoPessoasLayoutEDI.Inserir(grupoPessoasLayoutEDI, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoasLayoutEDI.GrupoPessoas, null, "Adicionou o Layout " + grupoPessoasLayoutEDI.Descricao + ".", unidadeDeTrabalho);
                }

                //Se houver Certificado de Chave Privada, cria o anexo baseado no base64 do arquivo selecionado.
                if (!string.IsNullOrEmpty((string)layoutsEDI[i].CertificadoChavePrivada))
                {
                    anexosLayoutEDI.Add(new Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo()
                    {
                        CodigoEntidade = grupoPessoasLayoutEDI.Codigo,
                        Descricao = "Certificado Chave Privada",
                        Arquivo = Convert.FromBase64String((string)layoutsEDI[i].CertificadoChavePrivadaBase64),
                        Nome = (string)layoutsEDI[i].CertificadoChavePrivada,
                        Extensao = ""
                    });
                }
            }
        }

        private dynamic SalvarConfiguracaoFatura(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas, Repositorio.UnitOfWork unidadeDeTrabalho, ref IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosFaturaCanhoto)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return null;

            if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.GrupoPessoas_PermiteAlterarConfiguracaoFatura))
                return null;

            if (string.IsNullOrWhiteSpace(Request.Params("ConfiguracaoFatura")))
                return null;

            dynamic configuracaoFatura = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoFatura"));
            if (configuracaoFatura == null)
                return null;

            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unidadeDeTrabalho);
            Repositorio.Banco repBanco = new Repositorio.Banco(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            TipoContaBanco tipoConta;
            TipoPrazoFaturamento tipoPrazoFaturamento;
            TipoAgrupamentoEnvioDocumentacao tipoAgrupamentoEnvioDocumentacao;
            TipoAgrupamentoEnvioDocumentacao tipoAgrupamentoEnvioDocumentacaoPorta;
            FormaEnvioDocumentacao formaEnvioDocumentacao;
            FormaEnvioDocumentacao formaEnvioDocumentacaoPorta;
            FormaGeracaoTituloFatura formaGeracaoTituloFatura;

            grupoPessoas.TipoPrazoFaturamento = null;
            if (configuracaoFatura.TipoPrazoFaturamento != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoPrazoFaturamento, out tipoPrazoFaturamento);
                grupoPessoas.TipoPrazoFaturamento = tipoPrazoFaturamento;
            }

            grupoPessoas.FormaGeracaoTituloFatura = null;
            if (configuracaoFatura.FormaGeracaoTituloFatura != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaGeracaoTituloFatura, out formaGeracaoTituloFatura);
                grupoPessoas.FormaGeracaoTituloFatura = formaGeracaoTituloFatura;
            }

            grupoPessoas.DiasDePrazoFatura = 0;
            if (configuracaoFatura.DiasDePrazoFatura != null)
            {
                int diasPrazo = 0;
                int.TryParse((string)configuracaoFatura.DiasDePrazoFatura, out diasPrazo);
                grupoPessoas.DiasDePrazoFatura = diasPrazo;
            }

            grupoPessoas.PermiteFinalDeSemana = false;
            if (configuracaoFatura.PermiteFinalSemana != null)
                grupoPessoas.PermiteFinalDeSemana = (bool)configuracaoFatura.PermiteFinalSemana;

            grupoPessoas.ExigeCanhotoFisico = false;
            if (configuracaoFatura.ExigeCanhotoFisico != null)
                grupoPessoas.ExigeCanhotoFisico = (bool)configuracaoFatura.ExigeCanhotoFisico;

            grupoPessoas.ArmazenaCanhotoFisicoCTe = false;
            if (configuracaoFatura.ArmazenaCanhotoFisicoCTe != null)
                grupoPessoas.ArmazenaCanhotoFisicoCTe = (bool)configuracaoFatura.ArmazenaCanhotoFisicoCTe;

            grupoPessoas.SomenteOcorrenciasFinalizadoras = false;
            if (configuracaoFatura.SomenteOcorrenciasFinalizadoras != null)
                grupoPessoas.SomenteOcorrenciasFinalizadoras = (bool)configuracaoFatura.SomenteOcorrenciasFinalizadoras;

            grupoPessoas.FaturarSomenteOcorrenciasFinalizadoras = false;
            if (configuracaoFatura.FaturarSomenteOcorrenciasFinalizadoras != null)
                grupoPessoas.FaturarSomenteOcorrenciasFinalizadoras = (bool)configuracaoFatura.FaturarSomenteOcorrenciasFinalizadoras;

            grupoPessoas.NaoGerarFaturaAteReceberCanhotos = false;
            if (configuracaoFatura.NaoGerarFaturaAteReceberCanhotos != null)
                grupoPessoas.NaoGerarFaturaAteReceberCanhotos = (bool)configuracaoFatura.NaoGerarFaturaAteReceberCanhotos;

            grupoPessoas.GerarFaturamentoMultiplaParcela = false;
            if (configuracaoFatura.GerarFaturamentoMultiplaParcela != null)
                grupoPessoas.GerarFaturamentoMultiplaParcela = (bool)configuracaoFatura.GerarFaturamentoMultiplaParcela;

            grupoPessoas.AvisoVencimetoHabilitarConfiguracaoPersonalizada = false;
            if (configuracaoFatura.AvisoVencimetoHabilitarConfiguracaoPersonalizada != null)
                grupoPessoas.AvisoVencimetoHabilitarConfiguracaoPersonalizada = (bool)configuracaoFatura.AvisoVencimetoHabilitarConfiguracaoPersonalizada;

            grupoPessoas.AvisoVencimetoNaoEnviarEmail = false;
            if (configuracaoFatura.AvisoVencimetoNaoEnviarEmail != null)
                grupoPessoas.AvisoVencimetoNaoEnviarEmail = (bool)configuracaoFatura.AvisoVencimetoNaoEnviarEmail;

            grupoPessoas.CobrancaNaoEnviarEmail = false;
            if (configuracaoFatura.CobrancaNaoEnviarEmail != null)
                grupoPessoas.CobrancaNaoEnviarEmail = (bool)configuracaoFatura.CobrancaNaoEnviarEmail;

            grupoPessoas.AvisoVencimetoEnviarDiariamente = false;
            if (configuracaoFatura.AvisoVencimetoEnviarDiariamente != null)
                grupoPessoas.AvisoVencimetoEnviarDiariamente = (bool)configuracaoFatura.AvisoVencimetoEnviarDiariamente;

            grupoPessoas.CobrancaHabilitarConfiguracaoPersonalizada = false;
            if (configuracaoFatura.CobrancaHabilitarConfiguracaoPersonalizada != null)
                grupoPessoas.CobrancaHabilitarConfiguracaoPersonalizada = (bool)configuracaoFatura.CobrancaHabilitarConfiguracaoPersonalizada;

            grupoPessoas.AvisoVencimetoQunatidadeDias = 0;
            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AvisoVencimetoQunatidadeDias))
                grupoPessoas.AvisoVencimetoQunatidadeDias = (int)configuracaoFatura.AvisoVencimetoQunatidadeDias;

            grupoPessoas.CobrancaQunatidadeDias = 0;
            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CobrancaQunatidadeDias))
                grupoPessoas.CobrancaQunatidadeDias = (int)configuracaoFatura.CobrancaQunatidadeDias;

            if (grupoPessoas.ExigeCanhotoFisico.Value == false)
                grupoPessoas.NaoGerarFaturaAteReceberCanhotos = false;

            if (configuracaoFatura.Banco != null && (int)configuracaoFatura.Banco > 0)
                grupoPessoas.Banco = repBanco.BuscarPorCodigo((int)configuracaoFatura.Banco);
            else
                grupoPessoas.Banco = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.Agencia))
                grupoPessoas.Agencia = (string)configuracaoFatura.Agencia;
            else
                grupoPessoas.Agencia = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.Digito))
                grupoPessoas.DigitoAgencia = (string)configuracaoFatura.Digito;
            else
                grupoPessoas.DigitoAgencia = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.NumeroConta))
                grupoPessoas.NumeroConta = (string)configuracaoFatura.NumeroConta;
            else
                grupoPessoas.NumeroConta = null;

            if (configuracaoFatura.TipoConta != null && (int)configuracaoFatura.TipoConta > 0)
            {
                Enum.TryParse((string)configuracaoFatura.TipoConta, out tipoConta);
                grupoPessoas.TipoContaBanco = tipoConta;
            }
            else
                grupoPessoas.TipoContaBanco = null;

            if (configuracaoFatura.TomadorFatura != null && (double)configuracaoFatura.TomadorFatura > 0)
                grupoPessoas.ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ((double)configuracaoFatura.TomadorFatura);
            else
                grupoPessoas.ClienteTomadorFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.ObservacaoFatura))
                grupoPessoas.ObservacaoFatura = (string)configuracaoFatura.ObservacaoFatura;
            else
                grupoPessoas.ObservacaoFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailFatura))
                grupoPessoas.EmailFatura = (string)configuracaoFatura.EmailFatura;
            else
                grupoPessoas.EmailFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoEmailFatura))
                grupoPessoas.AssuntoEmailFatura = (string)configuracaoFatura.AssuntoEmailFatura;
            else
                grupoPessoas.AssuntoEmailFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailFatura))
                grupoPessoas.CorpoEmailFatura = (string)configuracaoFatura.CorpoEmailFatura;
            else
                grupoPessoas.CorpoEmailFatura = null;

            bool informarEmailEnvioDocumentacao = (bool)configuracaoFatura.InformarEmailEnvioDocumentacao;
            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailEnvioDocumentacao) && informarEmailEnvioDocumentacao)
                grupoPessoas.EmailEnvioDocumentacao = (string)configuracaoFatura.EmailEnvioDocumentacao;
            else
                grupoPessoas.EmailEnvioDocumentacao = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoDocumentacao))
                grupoPessoas.AssuntoDocumentacao = (string)configuracaoFatura.AssuntoDocumentacao;
            else
                grupoPessoas.AssuntoDocumentacao = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailDocumentacao))
                grupoPessoas.CorpoEmailDocumentacao = (string)configuracaoFatura.CorpoEmailDocumentacao;
            else
                grupoPessoas.CorpoEmailDocumentacao = null;

            grupoPessoas.TipoAgrupamentoEnvioDocumentacao = null;
            if (configuracaoFatura.TipoAgrupamentoEnvioDocumentacao != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoEnvioDocumentacao, out tipoAgrupamentoEnvioDocumentacao);
                grupoPessoas.TipoAgrupamentoEnvioDocumentacao = tipoAgrupamentoEnvioDocumentacao;
            }

            grupoPessoas.FormaEnvioDocumentacao = null;
            if (configuracaoFatura.FormaEnvioDocumentacao != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaEnvioDocumentacao, out formaEnvioDocumentacao);
                grupoPessoas.FormaEnvioDocumentacao = formaEnvioDocumentacao;
            }

            bool informarEmailEnvioDocumentacaoPorta = (bool)configuracaoFatura.InformarEmailEnvioDocumentacaoPorta;
            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailEnvioDocumentacaoPorta) && informarEmailEnvioDocumentacaoPorta)
                grupoPessoas.EmailEnvioDocumentacaoPorta = (string)configuracaoFatura.EmailEnvioDocumentacaoPorta;
            else
                grupoPessoas.EmailEnvioDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoDocumentacaoPorta))
                grupoPessoas.AssuntoDocumentacaoPorta = (string)configuracaoFatura.AssuntoDocumentacaoPorta;
            else
                grupoPessoas.AssuntoDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailDocumentacaoPorta))
                grupoPessoas.CorpoEmailDocumentacaoPorta = (string)configuracaoFatura.CorpoEmailDocumentacaoPorta;
            else
                grupoPessoas.CorpoEmailDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.QuantidadeParcelasFaturamento))
                grupoPessoas.QuantidadeParcelasFaturamento = (string)configuracaoFatura.QuantidadeParcelasFaturamento;
            else
                grupoPessoas.QuantidadeParcelasFaturamento = null;

            grupoPessoas.TipoAgrupamentoEnvioDocumentacaoPorta = null;
            if (configuracaoFatura.TipoAgrupamentoEnvioDocumentacaoPorta != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoEnvioDocumentacaoPorta, out tipoAgrupamentoEnvioDocumentacaoPorta);
                grupoPessoas.TipoAgrupamentoEnvioDocumentacaoPorta = tipoAgrupamentoEnvioDocumentacaoPorta;
            }

            grupoPessoas.FormaEnvioDocumentacaoPorta = null;
            if (configuracaoFatura.FormaEnvioDocumentacaoPorta != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaEnvioDocumentacaoPorta, out formaEnvioDocumentacaoPorta);
                grupoPessoas.FormaEnvioDocumentacaoPorta = formaEnvioDocumentacaoPorta;
            }

            if (configuracaoFatura.BoletoConfiguracao != null && !string.IsNullOrWhiteSpace((string)configuracaoFatura.BoletoConfiguracao))
            {
                int.TryParse((string)configuracaoFatura.BoletoConfiguracao, out int codigoBoletoConfiguracao);
                if (codigoBoletoConfiguracao > 0)
                    grupoPessoas.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                else
                    grupoPessoas.BoletoConfiguracao = null;

            }
            else
                grupoPessoas.BoletoConfiguracao = null;

            grupoPessoas.EnviarBoletoPorEmailAutomaticamente = (bool)configuracaoFatura.EnviarBoletoPorEmailAutomaticamente;
            grupoPessoas.EnviarDocumentacaoFaturamentoCTe = (bool)configuracaoFatura.EnviarDocumentacaoFaturamentoCTe;
            grupoPessoas.NaoValidarPossuiAcordoFaturamentoAvancoCarga = (bool)configuracaoFatura.NaoValidarPossuiAcordoFaturamentoAvancoCarga;
            grupoPessoas.FormaPagamento = repTipoPagamentoRecebimento.BuscarPorCodigo((int)configuracaoFatura.FormaPagamento);
            grupoPessoas.GerarTituloPorDocumentoFiscal = (bool)configuracaoFatura.GerarTituloPorDocumentoFiscal;
            grupoPessoas.GerarTituloAutomaticamente = (bool)configuracaoFatura.GerarTituloAutomaticamente;
            grupoPessoas.GerarFaturaAutomaticaCte = (bool)configuracaoFatura.GerarFaturaAutomaticaCte;
            grupoPessoas.GerarFaturamentoAVista = (bool)configuracaoFatura.GerarFaturamentoAVista;

            grupoPessoas.GerarBoletoAutomaticamente = (bool)configuracaoFatura.GerarBoletoAutomaticamente;
            grupoPessoas.EnviarArquivosDescompactados = (bool)configuracaoFatura.EnviarArquivosDescompactados;
            grupoPessoas.NaoEnviarEmailFaturaAutomaticamente = (bool)configuracaoFatura.NaoEnviarEmailFaturaAutomaticamente;
            grupoPessoas.GerarFaturaPorCte = (bool)configuracaoFatura.GerarFaturaPorCte;
            grupoPessoas.HabilitarPeriodoVencimentoEspecifico = ((string)configuracaoFatura.HabilitarPeriodoVencimentoEspecifico).ToBool();

            if (configuracaoFatura.TipoEnvioFatura != null && (int)configuracaoFatura.TipoEnvioFatura > 0)
            {
                Enum.TryParse((string)configuracaoFatura.TipoEnvioFatura, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura tipoEnvioFatura);
                grupoPessoas.TipoEnvioFatura = tipoEnvioFatura;
            }
            else
                grupoPessoas.TipoEnvioFatura = null;

            if (configuracaoFatura.TipoAgrupamentoFatura != null && (int)configuracaoFatura.TipoAgrupamentoFatura > 0)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoFatura, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura tipoAgrupamentoFatura);
                grupoPessoas.TipoAgrupamentoFatura = tipoAgrupamentoFatura;
            }
            else
                grupoPessoas.TipoAgrupamentoFatura = null;

            if (configuracaoFatura.FormaTitulo != null && (int)configuracaoFatura.FormaTitulo > 0)
            {
                Enum.TryParse((string)configuracaoFatura.FormaTitulo, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo);
                grupoPessoas.FormaTitulo = formaTitulo;
            }
            else
                grupoPessoas.FormaTitulo = null;

            grupoPessoas.UtilizarCadastroContaBancaria = false;
            if (configuracaoFatura.UtilizarCadastroContaBancaria != null)
                grupoPessoas.UtilizarCadastroContaBancaria = (bool)configuracaoFatura.UtilizarCadastroContaBancaria;

            SalvarConfiguracaoFaturaVencimentos(grupoPessoas, configuracaoFatura.FaturaVencimentos, unidadeDeTrabalho);
            SalvarConfiguracaoFaturaEnvioCanhoto(grupoPessoas, configuracaoFatura, unidadeDeTrabalho, ref anexosFaturaCanhoto);
            SalvarContasBancarias(ref grupoPessoas, configuracaoFatura.ContasBancarias, unidadeDeTrabalho);

            return configuracaoFatura;
        }
        private void SalvarContasBancarias(ref Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, dynamic dynConfiguracaoContasBancarias, Repositorio.UnitOfWork unitOfWork)
        {
            if (!grupoPessoas.UtilizarCadastroContaBancaria)
                return;

            Repositorio.Embarcador.Financeiro.ContaBancaria repositorioContaBancaria = new Repositorio.Embarcador.Financeiro.ContaBancaria(unitOfWork);
            Repositorio.Cliente repUsuario = new Repositorio.Cliente(unitOfWork);

            string jsonString = JsonConvert.SerializeObject(dynConfiguracaoContasBancarias);

            dynamic contasBancarias = JsonConvert.DeserializeObject<dynamic>((string)jsonString);

            if (grupoPessoas.ContasBancarias != null && grupoPessoas.ContasBancarias.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic contaBancaria in contasBancarias)
                    codigos.Add((int)contaBancaria.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria> tiposDeletar = grupoPessoas.ContasBancarias.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Financeiro.ContaBancaria contasBancariasDeletar in tiposDeletar)
                    grupoPessoas.ContasBancarias.Remove(contasBancariasDeletar);
            }
            else
                grupoPessoas.ContasBancarias = new List<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria>();

            if (contasBancarias != null && contasBancarias.Count > 0)
            {
                foreach (dynamic contaBancaria in contasBancarias)
                {
                    int.TryParse((string)contaBancaria.Codigo, out int codigoContaBancaria);
                    Dominio.Entidades.Embarcador.Financeiro.ContaBancaria existeContasBancarias = repositorioContaBancaria.BuscarPorCodigo(codigoContaBancaria, false);

                    if (existeContasBancarias == null)
                        continue;

                    bool existeContaBancaria = grupoPessoas.ContasBancarias.Any(o => o.Codigo == existeContasBancarias.Codigo);

                    if (!existeContaBancaria)
                        grupoPessoas.ContasBancarias.Add(existeContasBancarias);
                }
            }
        }
        private void SalvarConfiguracaoFaturaEnvioCanhoto(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, dynamic dynConfiguracaoEnvioCanhoto, Repositorio.UnitOfWork unidadeDeTrabalho, ref IList<Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo> anexosFaturaCanhoto)
        {
            if (!dynConfiguracaoEnvioCanhoto.ContainsKey("CanhotoHabilitarEnvioCanhoto"))
                return;

            Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto repGrupoPessoasFaturaCanhoto = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> grupoPessoasFaturaCanhotos = repGrupoPessoasFaturaCanhoto.BuscarPorGrupoPessoasAsync(grupoPessoas.Codigo, default).Result;
            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> repAnexoGrupoPessoasFaturaCanhoto = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto> listaAnexoCanhoto = repAnexoGrupoPessoasFaturaCanhoto.BuscarPorEntidades(grupoPessoasFaturaCanhotos.Select(o => o.Codigo).ToList());

            if (grupoPessoasFaturaCanhotos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto gr in grupoPessoasFaturaCanhotos)
                {
                    foreach (Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto anexoCanhoto in listaAnexoCanhoto)
                        repAnexoGrupoPessoasFaturaCanhoto.Deletar(anexoCanhoto);
                    repGrupoPessoasFaturaCanhoto.Deletar(gr);
                }
            }


            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto grupoPessoasFaturaCanhoto = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto();
            grupoPessoasFaturaCanhoto.GrupoPessoas = grupoPessoas;
            grupoPessoasFaturaCanhoto.HabilitarEnvioCanhoto = ((bool)dynConfiguracaoEnvioCanhoto.CanhotoHabilitarEnvioCanhoto);

            if (dynConfiguracaoEnvioCanhoto.TipoIntegracaoCanhoto != null && (int)dynConfiguracaoEnvioCanhoto.TipoIntegracaoCanhoto > 0)
            {
                Enum.TryParse((string)dynConfiguracaoEnvioCanhoto.TipoIntegracaoCanhoto, out Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoCanhoto tipoIntegracao);
                grupoPessoasFaturaCanhoto.TipoIntegracaoCanhoto = tipoIntegracao;
            }
            else
                grupoPessoasFaturaCanhoto.TipoIntegracaoCanhoto = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoCanhoto.Nenhum;

            grupoPessoasFaturaCanhoto.EnderecoFTP = ((string)dynConfiguracaoEnvioCanhoto.EnderecoFTP);
            grupoPessoasFaturaCanhoto.Usuario = ((string)dynConfiguracaoEnvioCanhoto.Usuario);
            grupoPessoasFaturaCanhoto.Senha = ((string)dynConfiguracaoEnvioCanhoto.Senha);
            grupoPessoasFaturaCanhoto.Porta = ((string)dynConfiguracaoEnvioCanhoto.Porta);
            grupoPessoasFaturaCanhoto.Diretorio = ((string)dynConfiguracaoEnvioCanhoto.Diretorio);
            grupoPessoasFaturaCanhoto.Passivo = ((bool)dynConfiguracaoEnvioCanhoto.Passivo);
            grupoPessoasFaturaCanhoto.UtilizarSFTP = ((bool)dynConfiguracaoEnvioCanhoto.UtilizarSFTP);
            grupoPessoasFaturaCanhoto.SSL = ((bool)dynConfiguracaoEnvioCanhoto.SSL);
            grupoPessoasFaturaCanhoto.Nomenclatura = ((string)dynConfiguracaoEnvioCanhoto.Nomenclatura);
            grupoPessoasFaturaCanhoto.ExtensaoArquivo = ((string)dynConfiguracaoEnvioCanhoto.ExtensaoArquivo);

            repGrupoPessoasFaturaCanhoto.Inserir(grupoPessoasFaturaCanhoto);

            //Se houver Certificado de Chave Privada, cria o anexo baseado no base64 do arquivo selecionado.
            if (!string.IsNullOrEmpty((string)dynConfiguracaoEnvioCanhoto.CertificadoChavePrivada))
            {
                anexosFaturaCanhoto.Add(new Dominio.ObjetosDeValor.Embarcador.Pessoas.Anexo()
                {
                    CodigoEntidade = grupoPessoasFaturaCanhoto.Codigo,
                    Descricao = "Certificado Chave Privada",
                    Arquivo = Convert.FromBase64String((string)dynConfiguracaoEnvioCanhoto.CertificadoChavePrivadaBase64),
                    Nome = (string)dynConfiguracaoEnvioCanhoto.CertificadoChavePrivada,
                    Extensao = ""
                });
            }
        }

        private void SalvarConfiguracaoFaturaVencimentos(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, dynamic dynConfiguracaoFaturaVencimentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (dynConfiguracaoFaturaVencimentos == null)
                return;

            Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaVencimento repGrupoPessoasFaturaVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaVencimento(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento> grupoPessoasFaturaVencimentos = repGrupoPessoasFaturaVencimento.BuscarPorGrupoPessoas(grupoPessoas.Codigo);

            if (grupoPessoasFaturaVencimentos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic configuracaoFaturaVencimentos in dynConfiguracaoFaturaVencimentos)
                {
                    int codigo = ((string)configuracaoFaturaVencimentos.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento> deletar = (from obj in grupoPessoasFaturaVencimentos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < deletar.Count; i++)
                    repGrupoPessoasFaturaVencimento.Deletar(deletar[i]);
            }

            foreach (dynamic configuracaoFaturaVencimentos in dynConfiguracaoFaturaVencimentos)
            {
                int codigo = ((string)configuracaoFaturaVencimentos.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento grupoPessoasFaturaVencimento = codigo > 0 ? repGrupoPessoasFaturaVencimento.BuscarPorCodigo(codigo, false) : null;

                if (grupoPessoasFaturaVencimento == null)
                {
                    grupoPessoasFaturaVencimento = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento();
                    grupoPessoasFaturaVencimento.GrupoPessoas = grupoPessoas;
                }

                grupoPessoasFaturaVencimento.DiaInicial = ((string)configuracaoFaturaVencimentos.DiaInicial).ToInt();
                grupoPessoasFaturaVencimento.DiaFinal = ((string)configuracaoFaturaVencimentos.DiaFinal).ToInt();
                grupoPessoasFaturaVencimento.DiaVencimento = ((string)configuracaoFaturaVencimentos.DiaVencimento).ToInt();

                if (grupoPessoasFaturaVencimento.Codigo > 0)
                    repGrupoPessoasFaturaVencimento.Atualizar(grupoPessoasFaturaVencimento);
                else
                    repGrupoPessoasFaturaVencimento.Inserir(grupoPessoasFaturaVencimento);
            }
        }

        private void SalvarConfiguracaoFaturaListasAuditarManual(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            if (!string.IsNullOrWhiteSpace(Request.Params("ConfiguracaoFatura")))
            {
                dynamic configuracaoFatura = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoFatura"));
                if (configuracaoFatura != null)
                {
                    string diasSemanaAnterior = "", diasMesAnterior = "";

                    if (grupoPessoas.DiasSemanaFatura == null)
                        grupoPessoas.DiasSemanaFatura = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();
                    else
                    {
                        diasSemanaAnterior = string.Join(", ", grupoPessoas.DiasSemanaFatura);
                        grupoPessoas.DiasSemanaFatura.Clear();
                    }

                    if (configuracaoFatura.DiasSemanaFatura != null)
                    {
                        foreach (dynamic dia in configuracaoFatura.DiasSemanaFatura)
                        {
                            Enum.TryParse((string)dia, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemanaFatura);
                            grupoPessoas.DiasSemanaFatura.Add(diaSemanaFatura);
                        }
                    }

                    if (grupoPessoas.DiasMesFatura == null)
                        grupoPessoas.DiasMesFatura = new List<int>();
                    else
                    {
                        diasMesAnterior = string.Join(", ", grupoPessoas.DiasMesFatura);
                        grupoPessoas.DiasMesFatura.Clear();
                    }

                    if (configuracaoFatura.DiasMesFatura != null)
                    {
                        foreach (dynamic diaMesFatura in configuracaoFatura.DiasMesFatura)
                            grupoPessoas.DiasMesFatura.Add((int)diaMesFatura);
                    }

                    if (grupoPessoas.Codigo > 0)
                    {
                        string diasSemanaAtuais = string.Join(", ", grupoPessoas.DiasSemanaFatura);
                        if (!diasSemanaAnterior.Equals(diasSemanaAtuais))
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Alterou dias da semana de '" + diasSemanaAnterior + "' para '" + diasSemanaAtuais + "'.", unidadeDeTrabalho);

                        string diasMesAtuais = string.Join(", ", grupoPessoas.DiasMesFatura);
                        if (!diasMesAnterior.Equals(diasMesAtuais))
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Alterou dias do mês de '" + diasMesAnterior + "' para '" + diasMesAtuais + "'.", unidadeDeTrabalho);
                    }

                    if (grupoPessoas.PermiteFinalDeSemana == false)
                    {
                        foreach (DiaSemana diaSemana in grupoPessoas.DiasSemanaFatura)
                        {
                            if (diaSemana == DiaSemana.Sabado || diaSemana == DiaSemana.Domingo)
                                throw new ControllerException("Foi selecionado um dia do final de semana para a configuração da fatura.");
                        }
                    }
                }
            }
        }

        private void AtualizarConfiguracoesFatura(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, dynamic configuracao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (configuracao == null)
                return;

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasFatura repConfiguracaoGrupoPessoasFatura = new Repositorio.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasFatura(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasFatura configuracaoGrupoPessoasFatura = grupoPessoas.ConfiguracaoFatura ?? new Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoGrupoPessoas.ConfiguracaoGrupoPessoasFatura();

            if (configuracaoGrupoPessoasFatura.Codigo > 0)
                configuracaoGrupoPessoasFatura.Initialize();

            configuracaoGrupoPessoasFatura.GerarTituloAutomaticamenteComAdiantamentoSaldo = ((string)configuracao.GerarTituloAutomaticamenteComAdiantamentoSaldo).ToBool();
            configuracaoGrupoPessoasFatura.PercentualAdiantamentoTituloAutomatico = ((string)configuracao.PercentualAdiantamentoTituloAutomatico).ToDecimal();
            configuracaoGrupoPessoasFatura.PrazoAdiantamentoEmDiasTituloAutomatico = ((string)configuracao.PrazoAdiantamentoEmDiasTituloAutomatico).ToInt();
            configuracaoGrupoPessoasFatura.PercentualSaldoTituloAutomatico = ((string)configuracao.PercentualSaldoTituloAutomatico).ToDecimal();
            configuracaoGrupoPessoasFatura.PrazoSaldoEmDiasTituloAutomatico = ((string)configuracao.PrazoSaldoEmDiasTituloAutomatico).ToInt();
            configuracaoGrupoPessoasFatura.EfetuarImpressaoDaTaxaDeMoedaEstrangeira = ((string)configuracao.EfetuarImpressaoDaTaxaDeMoedaEstrangeira).ToNullableBool();

            decimal somaPercentual = configuracaoGrupoPessoasFatura.PercentualAdiantamentoTituloAutomatico + configuracaoGrupoPessoasFatura.PercentualSaldoTituloAutomatico;
            if (configuracaoGrupoPessoasFatura.GerarTituloAutomaticamenteComAdiantamentoSaldo && somaPercentual != 100)
                throw new ControllerException("A soma dos percentuais de Adiantamento e Saldo devem somar 100% para geração de título automático!");

            if (configuracaoGrupoPessoasFatura.Codigo == 0)
                repConfiguracaoGrupoPessoasFatura.Inserir(configuracaoGrupoPessoasFatura);
            else
                repConfiguracaoGrupoPessoasFatura.Atualizar(configuracaoGrupoPessoasFatura, Auditado, historico);

            grupoPessoas.ConfiguracaoFatura = configuracaoGrupoPessoasFatura;

            repGrupoPessoas.Atualizar(grupoPessoas);
        }

        private void SalvarContatos(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Contatos.PessoaContato repPessoaContato = new Repositorio.Embarcador.Contatos.PessoaContato(unidadeTrabalho);
            Repositorio.Embarcador.Contatos.TipoContato repTipoContato = new Repositorio.Embarcador.Contatos.TipoContato(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Contatos.PessoaContato> contatosGrupoPessoas = repPessoaContato.BuscarPorGrupoPessoas(grupoPessoas.Codigo);

            dynamic contatos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaContatos"));

            if (contatosGrupoPessoas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic contato in contatos)
                    if (int.TryParse((string)contato.Codigo, out int codigoContato))
                        codigos.Add(codigoContato);

                List<Dominio.Entidades.Embarcador.Contatos.PessoaContato> contatosDeletar = (from obj in contatosGrupoPessoas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < contatosDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Removeu o contato " + contatosDeletar[i].Descricao + ".", unidadeTrabalho);
                    repPessoaContato.Deletar(contatosDeletar[i], Auditado);
                }
            }

            foreach (dynamic contato in contatos)
            {
                Dominio.Entidades.Embarcador.Contatos.PessoaContato contatoGrupoPessoas = null;

                if (int.TryParse((string)contato.Codigo, out int codigoContato))
                    contatoGrupoPessoas = repPessoaContato.BuscarPorCodigo(codigoContato, false);

                if (contatoGrupoPessoas == null)
                    contatoGrupoPessoas = new Dominio.Entidades.Embarcador.Contatos.PessoaContato();
                else
                    contatoGrupoPessoas.Initialize();

                contatoGrupoPessoas.GrupoPessoas = grupoPessoas;
                contatoGrupoPessoas.Ativo = (bool)contato.Situacao;
                contatoGrupoPessoas.Contato = (string)contato.Contato;
                contatoGrupoPessoas.Email = (string)contato.Email;
                contatoGrupoPessoas.Telefone = (string)contato.Telefone;

                if (contatoGrupoPessoas.TiposContato == null)
                    contatoGrupoPessoas.TiposContato = new List<Dominio.Entidades.Embarcador.Contatos.TipoContato>();
                else
                    contatoGrupoPessoas.TiposContato.Clear();

                if (contato.TipoContato != null)
                {
                    foreach (dynamic codigoTipoContato in contato.TipoContato)
                        contatoGrupoPessoas.TiposContato.Add(repTipoContato.BuscarPorCodigo((int)codigoTipoContato));
                }

                if (contatoGrupoPessoas.Codigo > 0)
                {
                    //repPessoaContato.Atualizar(contatoGrupoPessoas, Auditado);
                    repPessoaContato.Atualizar(contatoGrupoPessoas, Auditado);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesLayout = contatoGrupoPessoas.GetChanges();
                    if (alteracoesLayout.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, alteracoesLayout, "Alterou o contato " + contatoGrupoPessoas.Descricao + ".", unidadeTrabalho);
                }
                else
                {
                    repPessoaContato.Inserir(contatoGrupoPessoas, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Adicionou o contato " + contatoGrupoPessoas.Descricao + ".", unidadeTrabalho);
                }
            }
        }

        private void SalvarMensagemAlerta(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoaMensagemAlerta repGrupoPessoaMensagemAlerta = new Repositorio.Embarcador.Pessoas.GrupoPessoaMensagemAlerta(unitOfWork);

            repGrupoPessoaMensagemAlerta.DeletarPorGrupoPessoa(grupoPessoas.Codigo);
            dynamic dynMensagemAlerta = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaMensagemAlerta"));
            if (dynMensagemAlerta != null && dynMensagemAlerta.Count > 0)
            {
                foreach (dynamic dynAlerta in dynMensagemAlerta)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta alerta = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta();

                    alerta.Tag = (string)dynAlerta.Tag;
                    alerta.MensagemAlerta = (string)dynAlerta.MensagemAlerta;
                    alerta.GrupoPessoas = grupoPessoas;

                    repGrupoPessoaMensagemAlerta.Inserir(alerta);
                }
            }
        }

        private void SalvarVendedores(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.PessoaFuncionario repPessoaFuncionario = new Repositorio.Embarcador.Pessoas.PessoaFuncionario(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFuncionario repGrupoPessoasFuncionario = new Repositorio.Embarcador.Pessoas.GrupoPessoasFuncionario(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            repGrupoPessoasFuncionario.DeletarPorGrupoPessoaEmpresa(grupoPessoas.Codigo, codigoEmpresa);
            dynamic dynVendedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaVendedores"));
            if (dynVendedores.Count > 0)
            {
                foreach (dynamic dynVendedor in dynVendedores)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFuncionario vendedor = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFuncionario();

                    int codigoFuncionario = int.Parse((string)dynVendedor.CodigoFuncionario);
                    decimal percentualComissao;
                    decimal.TryParse((string)dynVendedor.PercentualComissao, out percentualComissao);
                    DateTime.TryParse((string)dynVendedor.DataInicioVigencia, out DateTime dataInicioVigencia);
                    DateTime.TryParse((string)dynVendedor.DataFimVigencia, out DateTime dataFimVigencia);

                    vendedor.PercentualComissao = percentualComissao;
                    if (dataInicioVigencia > DateTime.MinValue)
                        vendedor.DataInicioVigencia = dataInicioVigencia;
                    if (dataFimVigencia > DateTime.MinValue)
                        vendedor.DataFimVigencia = dataFimVigencia;
                    if (codigoFuncionario > 0)
                        vendedor.Funcionario = repFuncionario.BuscarPorCodigo(codigoFuncionario);
                    vendedor.GrupoPessoas = grupoPessoas;

                    repGrupoPessoasFuncionario.Inserir(vendedor);
                }
            }
        }

        private void SalvarFormulasObservacaoNfe(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic formulasObservacaoNfe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FormulasObservacaoNfe"));
            List<int> codigosFormulasExistentes = new List<int>();

            foreach (dynamic formula in formulasObservacaoNfe)
            {
                int? codigo = ((string)formula.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    codigosFormulasExistentes.Add(codigo.Value);
            }

            Repositorio.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula repositorioObservacaoNfeFormula = new Repositorio.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula> formulasExistentes = repositorioObservacaoNfeFormula.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula> formulasDeletar = (from formula in formulasExistentes where !codigosFormulasExistentes.Contains(formula.Codigo) select formula).ToList();

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formulaDeletar in formulasDeletar)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, $"Fórmula da Observação da NF-e {formulaDeletar.Descricao} deletada do grupo.", unitOfWork);
                formulasExistentes.Remove(formulaDeletar);
                repositorioObservacaoNfeFormula.Deletar(formulaDeletar);
            }

            foreach (dynamic formula in formulasObservacaoNfe)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula formulaAdicionar = null;

                int? codigo = ((string)formula.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    formulaAdicionar = (from obj in formulasExistentes where obj.Codigo == codigo.Value select obj).FirstOrDefault();

                if (formulaAdicionar == null)
                    formulaAdicionar = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula();
                else
                    formulaAdicionar.Initialize();

                formulaAdicionar.Descricao = (string)formula.Descricao;
                formulaAdicionar.GrupoPessoas = grupoPessoas;
                formulaAdicionar.IdentificadorFim = (string)formula.IdentificadorFim;
                formulaAdicionar.IdentificadorInicio = (string)formula.IdentificadorInicio;
                formulaAdicionar.QtdMaximoDigitos = ((string)formula.QtdMaximoDigitos).ToInt();
                formulaAdicionar.QtdMinimoDigitos = ((string)formula.QtdMinimoDigitos).ToInt();
                formulaAdicionar.NumeroPedido = ((string)formula.NumeroPedido).ToBool();
                formulaAdicionar.NumeroContainer = ((string)formula.NumeroContainer).ToBool();
                formulaAdicionar.TaraContainer = ((string)formula.TaraContainer).ToBool();
                formulaAdicionar.LacreContainerUm = ((string)formula.LacreContainerUm).ToBool();
                formulaAdicionar.LacreContainerDois = ((string)formula.LacreContainerDois).ToBool();
                formulaAdicionar.LacreContainerTres = ((string)formula.LacreContainerTres).ToBool();
                formulaAdicionar.NumeroControleCliente = ((string)formula.NumeroControleCliente).ToBool();
                formulaAdicionar.NumeroControlePedido = ((string)formula.NumeroControlePedido).ToBool();
                formulaAdicionar.NumeroReferenciaEDI = ((string)formula.NumeroReferenciaEDI).ToBool();
                formulaAdicionar.Tag = (string)formula.Tag;

                if (string.IsNullOrWhiteSpace(formulaAdicionar.IdentificadorFim) && formulaAdicionar.QtdMaximoDigitos <= 0)
                    formulaAdicionar.IdentificadorFim = " ";

                if (formulaAdicionar.Codigo > 0)
                {
                    repositorioObservacaoNfeFormula.Atualizar(formulaAdicionar, Auditado);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = formulaAdicionar.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, alteracoes, $"Alterou a fórmula da Observação da NF-e {formulaAdicionar.Descricao}.", unitOfWork);
                }
                else
                {
                    repositorioObservacaoNfeFormula.Inserir(formulaAdicionar, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, $"Adicionou a fórmula da Observação da NF-e {formulaAdicionar.Descricao}.", unitOfWork);
                }
            }
        }

        private void SalvarPerfilChamado(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic perfilChamado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PerfilChamado"));

            grupoPessoas.ClienteNaoNecessitaAutorizacaoAtendimento = ((string)perfilChamado.ClienteNaoNecessitaAutorizacaoAtendimento).ToBool();
            grupoPessoas.GeraNumeroOcorrenciaAutorizacao = ((string)perfilChamado.GeraNumeroOcorrenciaAutorizacao).ToBool();

            grupoPessoas.DiaPrazoCobrancaChamado = ((string)perfilChamado.DiaPrazoCobrancaChamado).ToInt();
            grupoPessoas.QuantidadeMaximaDiasDataReciboAbertura = ((string)perfilChamado.QuantidadeMaximaDiasDataReciboAbertura).ToInt();

            grupoPessoas.ValorMaximoDiferencaRecibo = ((string)perfilChamado.ValorMaximoDiferencaRecibo).ToDecimal();

            grupoPessoas.AssuntoEmailChamado = (string)perfilChamado.AssuntoEmailChamado;
            grupoPessoas.CorpoEmailChamado = (string)perfilChamado.CorpoEmailChamado;
            grupoPessoas.MensagemPadraoOrientacaoMotorista = (string)perfilChamado.MensagemPadraoOrientacaoMotorista;

            grupoPessoas.FormaValorDescarga = ((string)perfilChamado.AssuntoEmailChamado).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaValorDescarga>();
            grupoPessoas.FormaAberturaOcorrencia = ((string)perfilChamado.FormaAberturaOcorrencia).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaAberturaOcorrencia>();
            grupoPessoas.TipoPrazoCobrancaChamado = ((string)perfilChamado.TipoPrazoCobrancaChamado).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoCobrancaChamado>();
            grupoPessoas.NivelToleranciaValorCliente = ((string)perfilChamado.NivelToleranciaValorCliente).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelToleranciaValor>();
            grupoPessoas.NivelToleranciaValorMotorista = ((string)perfilChamado.NivelToleranciaValorMotorista).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelToleranciaValor>();
        }

        private void SalvarTabelaValores(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor repositorioPerfilTabelaValor = new Repositorio.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            dynamic tabelaValores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TabelaValores"));
            List<int> codigosTabelaValores = new List<int>();

            foreach (dynamic tabelaValor in tabelaValores)
            {
                int? codigo = ((string)tabelaValor.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    codigosTabelaValores.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor> tabelaValoresExistentes = repositorioPerfilTabelaValor.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor> tabelaValoresDeletar = (from tabelaValor in tabelaValoresExistentes where !codigosTabelaValores.Contains(tabelaValor.Codigo) select tabelaValor).ToList();

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor tabelaValorDeletar in tabelaValoresDeletar)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, $"Tabela de valor do Perfil Chamado {tabelaValorDeletar.Descricao} deletada do grupo.", unitOfWork);
                tabelaValoresExistentes.Remove(tabelaValorDeletar);
                repositorioPerfilTabelaValor.Deletar(tabelaValorDeletar);
            }

            foreach (dynamic tabelaValor in tabelaValores)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor tabelaValorAdicionar = null;

                int? codigo = ((string)tabelaValor.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    tabelaValorAdicionar = (from obj in tabelaValoresExistentes where obj.Codigo == codigo.Value select obj).FirstOrDefault();

                if (tabelaValorAdicionar == null)
                    tabelaValorAdicionar = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasPerfilTabelaValor();
                else
                    tabelaValorAdicionar.Initialize();

                int modeloVeicular = ((string)tabelaValor.ModeloVeicularCarga.Codigo).ToInt();

                tabelaValorAdicionar.GrupoPessoa = grupoPessoas;
                tabelaValorAdicionar.Valor = Utilidades.Decimal.Converter((string)tabelaValor.Valor);
                tabelaValorAdicionar.ModeloVeicularCarga = modeloVeicular > 0 ? repModeloVeicularCarga.BuscarPorCodigo(modeloVeicular) : null;

                if (tabelaValorAdicionar.Codigo > 0)
                {
                    repositorioPerfilTabelaValor.Atualizar(tabelaValorAdicionar, Auditado);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = tabelaValorAdicionar.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, alteracoes, $"Alterou a tabela de valor do Perfil Chamado {tabelaValorAdicionar.Descricao}.", unitOfWork);
                }
                else
                {
                    repositorioPerfilTabelaValor.Inserir(tabelaValorAdicionar, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, $"Adicionou a tabela de valor do Perfil Chamado {tabelaValorAdicionar.Descricao}.", unitOfWork);
                }
            }
        }

        private void SalvarObservacoesCTe(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasObservacaoCTe repGrupoPessoasObservacaoCTe = new Repositorio.Embarcador.Pessoas.GrupoPessoasObservacaoCTe(unitOfWork);

            dynamic observacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ObservacoesCTes"));

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoCTe> observacoesCTes = repGrupoPessoasObservacaoCTe.BuscarPorGrupoPessoas(grupoPessoas.Codigo);

            if (observacoesCTes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic observacao in observacoes)
                    if (observacao.Codigo != null)
                        codigos.Add((int)observacao.Codigo);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoCTe> observacoesDeletar = observacoesCTes.Where(obj => !codigos.Contains(obj.Codigo)).ToList();

                for (int i = 0; i < observacoesDeletar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoCTe observacaoDeletar = observacoesDeletar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, $"Removeu a observação para CT-e {observacaoDeletar.Descricao}.", unitOfWork);

                    repGrupoPessoasObservacaoCTe.Deletar(observacaoDeletar);
                }
            }

            foreach (dynamic observacao in observacoes)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoCTe obs = null;

                int codigo = 0;
                if (observacao.Codigo != null && int.TryParse((string)observacao.Codigo, out codigo))
                    obs = repGrupoPessoasObservacaoCTe.BuscarPorCodigo(codigo, true);

                if (obs == null)
                    obs = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoCTe();

                obs.GrupoPessoas = grupoPessoas;
                obs.Identificador = (string)observacao.Identificador;
                obs.Texto = (string)observacao.Texto;
                obs.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe)observacao.Tipo;

                if (obs.Codigo > 0)
                {
                    repGrupoPessoasObservacaoCTe.Atualizar(obs);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = obs.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, alteracoes, $"Alterou a observação para CT-e {obs.Descricao}.", unitOfWork);
                }
                else
                {
                    repGrupoPessoasObservacaoCTe.Inserir(obs);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, $"Adicionou a observação para CT-e {obs.Descricao}.", unitOfWork);
                }
            }
        }

        private void PreencherDadosFornecedor(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic dynFornecedor = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Fornecedor"));
            if (dynFornecedor.Fornecedor == null)
                return;

            grupoPessoas.GerarDuplicataNotaEntrada = ((string)dynFornecedor.Fornecedor.GerarDuplicataNotaEntrada).ToBool();
            grupoPessoas.UtilizarParametrizacaoDeHorariosNoAgendamento = ((string)dynFornecedor.Fornecedor.UtilizarParametrizacaoDeHorariosNoAgendamento).ToBool();
            grupoPessoas.DiaPadraoDuplicataNotaEntrada = ((string)dynFornecedor.Fornecedor.DiaPadraoDuplicataNotaEntrada).ToInt();
            grupoPessoas.IntervaloDiasDuplicataNotaEntrada = (string)dynFornecedor.Fornecedor.IntervaloDiasDuplicataNotaEntrada;
            grupoPessoas.ParcelasDuplicataNotaEntrada = ((string)dynFornecedor.Fornecedor.ParcelasDuplicataNotaEntrada).ToInt();
            grupoPessoas.IgnorarDuplicataRecebidaXMLNotaEntrada = ((string)dynFornecedor.Fornecedor.IgnorarDuplicataRecebidaXMLNotaEntrada).ToBool();
            grupoPessoas.PermitirMultiplosVencimentos = ((string)dynFornecedor.Fornecedor.PermitirMultiplosVencimentos).ToBool();

            grupoPessoas.FormaTituloFornecedor = ((string)dynFornecedor.Fornecedor.FormaTituloFornecedor).ToInt() > 0 ? ((string)dynFornecedor.Fornecedor.FormaTituloFornecedor).ToNullableEnum<FormaTitulo>() : null;
        }

        private void SalvarTabelasFornecedor(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento repFornecedorVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento(unitOfWork);

            dynamic dynFornecedor = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Fornecedor"));
            if (dynFornecedor.Fornecedor == null)
                return;

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento> listaVencimentos = repFornecedorVencimento.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento in listaVencimentos)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Deletou vencimento " + vencimento.DataEmissao + ".", unitOfWork);
                repFornecedorVencimento.Deletar(vencimento);
            }

            foreach (dynamic dynTabelaMultiplosVencimentos in dynFornecedor.TabelaMultiplosVencimentos)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento();
                vencimento.GrupoPessoas = grupoPessoas;
                vencimento.DiaEmissaoInicial = ((string)dynTabelaMultiplosVencimentos.DiaEmissaoInicial.val).ToInt();
                vencimento.DiaEmissaoFinal = ((string)dynTabelaMultiplosVencimentos.DiaEmissaoFinal.val).ToInt();
                vencimento.Vencimento = ((string)dynTabelaMultiplosVencimentos.Vencimento.val).ToInt();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoPessoas, null, "Adicionou o vencimento " + vencimento.Vencimento + ".", unitOfWork);
                repFornecedorVencimento.Inserir(vencimento);
            }
        }

        private void SalvarNCMPalletsNFe(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasNCMPallet repGrupoPessoasNCMPallet = new Repositorio.Embarcador.Pessoas.GrupoPessoasNCMPallet(unitOfWork);

            dynamic ncmsPallets = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NCMPalletsNFe"));

            if (grupoPessoas.NCMsPallet?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic ncm in ncmsPallets)
                {
                    int? codigo = ((string)ncm.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasNCMPallet> listaDeletar = (from obj in grupoPessoas.NCMsPallet where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < listaDeletar.Count; i++)
                    repGrupoPessoasNCMPallet.Deletar(listaDeletar[i]);
            }
            else
                grupoPessoas.NCMsPallet = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasNCMPallet>();

            foreach (dynamic ncm in ncmsPallets)
            {
                int codigoNCMPallet = ((string)ncm.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasNCMPallet ncmPallet = codigoNCMPallet > 0 ? repGrupoPessoasNCMPallet.BuscarPorCodigo(codigoNCMPallet, true) : null;

                if (ncmPallet == null)
                    ncmPallet = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasNCMPallet();

                ncmPallet.GrupoPessoas = grupoPessoas;
                ncmPallet.NCM = (string)ncm.NCM;

                if (ncmPallet.Codigo > 0)
                    repGrupoPessoasNCMPallet.Atualizar(ncmPallet);
                else
                    repGrupoPessoasNCMPallet.Inserir(ncmPallet);
            }
        }

        private TimeSpan RetornarTimeSpan(string strTempo)
        {
            if (strTempo != string.Empty)
            {
                string[] HrMin = strTempo.Split(':');
                double hr = HrMin[0].ToDouble();
                double min = HrMin[1].ToDouble();
                TimeSpan tempo = TimeSpan.FromHours(hr) + TimeSpan.FromMinutes(min);

                return tempo;
            }
            else
                return TimeSpan.Zero;
        }

        private string ObterCaminhoArquivoLogo(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Logo", "GrupoPessoas" });
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaGrupoPessoas ObterFiltrosPesquisa(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaGrupoPessoas filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaGrupoPessoas()
            {
                Descricao = Request.GetStringParam("Descricao"),
                RaizCNPJ = Utilidades.String.OnlyNumbers(Request.GetStringParam("RaizCNPJ")),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                TipoGrupoPessoas = Request.GetEnumParam<TipoGrupoPessoas>("TipoGrupoPessoas"),
                CnpjCpfCliente = Request.GetDoubleParam("Cliente"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0
            };

            if (Request.GetBoolParam("FiltrarPorConfiguracaoOperadorLogistica", valorPadrao: true))
                filtrosPesquisa.ListaCodigoGrupoPessoasPermitidos = ObterListaCodigoGrupoPessoasPermitidosOperadorLogistica(unidadeDeTrabalho);

            return filtrosPesquisa;
        }

        private void SalvarGrupoPessoasIntegracao(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repositorioGrupoPessoasIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            dynamic dynGrupoPessoasIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoIntegracoes"));

            if (dynGrupoPessoasIntegracao == null)
                return;

            List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repositorioGrupoPessoasIntegracao.BuscarPorGrupoPessoas(grupoPessoas.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (integracaoGrupoPessoas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic grupoPessoasIntegracao in dynGrupoPessoasIntegracao)
                {
                    int codigo = ((string)grupoPessoasIntegracao.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> listaDeletar = (from obj in integracaoGrupoPessoas where !codigos.Contains(obj.TipoIntegracao.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa deletar in listaDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "TipoIntegracao",
                        De = $"{deletar.TipoIntegracao.Descricao}",
                        Para = ""
                    });

                    repositorioGrupoPessoasIntegracao.Deletar(deletar);
                }
            }

            foreach (dynamic grupoPessoasIntegracao in dynGrupoPessoasIntegracao)
            {
                int codigo = ((string)grupoPessoasIntegracao.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa tipoGrupoPessoas = codigo > 0 ? repositorioGrupoPessoasIntegracao.BuscarPorGrupoPessoasETipoIntegracao(grupoPessoas.Codigo, codigo) : null;

                if (tipoGrupoPessoas == null)
                    tipoGrupoPessoas = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa();

                int codigoTipoIntegracao = ((string)grupoPessoasIntegracao.Codigo).ToInt();

                tipoGrupoPessoas.GrupoPessoas = grupoPessoas;
                tipoGrupoPessoas.TipoIntegracao = repositorioTipoIntegracao.BuscarPorCodigo(codigoTipoIntegracao);
                tipoGrupoPessoas.Habilitado = true;

                if (tipoGrupoPessoas.TipoIntegracao != null)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "TipoIntegracao",
                        De = "",
                        Para = $"{tipoGrupoPessoas.TipoIntegracao.Descricao}"
                    });

                    if (tipoGrupoPessoas.Codigo > 0)
                        repositorioGrupoPessoasIntegracao.Atualizar(tipoGrupoPessoas);
                    else
                        repositorioGrupoPessoasIntegracao.Inserir(tipoGrupoPessoas);
                }
            }

            grupoPessoas.SetExternalChanges(alteracoes);
        }

        private void SalvarPessoasDoGrupo(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/GrupoPessoas");

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            dynamic dynClientes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Clientes"));

            if (dynClientes == null)
                return;

            List<Dominio.Entidades.Cliente> listaCliente = repositorioCliente.BuscarPorGrupoPessoa(grupoPessoas.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            List<double> cpfCNPJ = new List<double>();
            List<Dominio.Entidades.Cliente> listaDeletar = new List<Dominio.Entidades.Cliente>();

            foreach (dynamic cliente in dynClientes)
            {
                double codigo = ((string)cliente.Cliente.Codigo).ToDouble();
                if (codigo > 0)
                    cpfCNPJ.Add(codigo);
            }

            if (listaCliente.Count > 0)
            {
                listaDeletar = (from obj in listaCliente where !cpfCNPJ.Contains(obj.CPF_CNPJ) select obj).ToList();

                foreach (Dominio.Entidades.Cliente deletar in listaDeletar)
                {
                    if (deletar.GrupoPessoas != null || deletar.GrupoPessoas.Codigo != grupoPessoas.Codigo)
                    {
                        deletar.GrupoPessoas = null;
                        deletar.Integrado = false;
                        deletar.DataUltimaAtualizacao = DateTime.Now;

                        repositorioCliente.Atualizar(deletar);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, deletar, null, "Removeu do Grupo de Pessoa " + grupoPessoas.Descricao + ".", unitOfWork);

                        alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                        {
                            Propriedade = "Pessoas do Grupo",
                            De = "Removido " + deletar.Descricao + ".",
                            Para = ""
                        });
                    }
                }
            }

            List<double> listaCPFCNPJAtualizarCliente = (from obj in cpfCNPJ where !(listaCliente.Select(o => o.CPF_CNPJ).ToList()).Contains(obj) select obj).ToList();

            foreach (double cpfCNPJCliente in listaCPFCNPJAtualizarCliente)
            {
                Dominio.Entidades.Cliente clienteAtualizacao = repositorioCliente.BuscarPorCPFCNPJ(cpfCNPJCliente);

                if (clienteAtualizacao != null)
                {
                    clienteAtualizacao.Initialize();
                    clienteAtualizacao.GrupoPessoas = grupoPessoas;
                    clienteAtualizacao.DataUltimaAtualizacao = DateTime.Now;
                    clienteAtualizacao.Integrado = false;
                    repositorioCliente.Atualizar(clienteAtualizacao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, clienteAtualizacao, null, "Adicionou ao Grupo de Pessoa " + grupoPessoas.Descricao + ".", unitOfWork);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Pessoas do Grupo",
                        De = "",
                        Para = "Adicionado " + clienteAtualizacao.Descricao + "."
                    });
                }
            }

            if (!Usuario.UsuarioAdministrador && (listaCPFCNPJAtualizarCliente.Count > 0 || listaDeletar.Count > 0) && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.GrupoPessoas_PermitirAdicionarClienteAoGrupoCliente))
                throw new ControllerException("Usuário não possui permissão para adicionar clientes ao grupo de clientes.");

            grupoPessoas.SetExternalChanges(alteracoes);
        }
        private void SalvarTipoComprovantes(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unidadeDeTrabalho);
            dynamic tipoComprovantes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Comprovantes"));

            if (grupoPessoas.TiposComprovante == null)
                grupoPessoas.TiposComprovante = new List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoComprovante in tipoComprovantes)
                    codigos.Add((int)tipoComprovante.Tipo.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> tipoComprovantesDeletar = grupoPessoas.TiposComprovante.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipoComprovanteDeletar in tipoComprovantesDeletar)
                    grupoPessoas.TiposComprovante.Remove(tipoComprovanteDeletar);
            }

            foreach (dynamic tipoComprovante in tipoComprovantes)
            {
                if (grupoPessoas.TiposComprovante.Any(o => o.Codigo == (int)tipoComprovante.Tipo.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipoComprovanteObj = repTipoComprovante.BuscarPorCodigo((int)tipoComprovante.Tipo.Codigo);
                grupoPessoas.TiposComprovante.Add(tipoComprovanteObj);
            }
        }

        private dynamic selectFaturaCanhoto(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto canhoto, Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> servicoAnexoFaturaCanhoto, List<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto> listaAnexoFaturaCanhoto, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.Pessoa.GrupoPessoasFaturaCanhoto servicoGrupoPessoaFaturaCanhoto = new Servicos.Embarcador.Pessoa.GrupoPessoasFaturaCanhoto(unitOfWork);
            string CertificadoChavePrivada = string.Empty,
                   CertificadoChavePrivadaBase64 = string.Empty;


            Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasFaturaCanhoto anexoCanhoto = listaAnexoFaturaCanhoto.Find(x => x.EntidadeAnexo.Codigo == canhoto.Codigo);
            if (anexoCanhoto != null)
            {
                CertificadoChavePrivada = servicoGrupoPessoaFaturaCanhoto.ObtemCertificadoChavePrivada(canhoto);
                CertificadoChavePrivadaBase64 = servicoGrupoPessoaFaturaCanhoto.ObtemCertificadoChavePrivadaBase64(canhoto);
            }

            return new
            {
                canhoto.Codigo,
                CanhotoHabilitarEnvioCanhoto = canhoto.HabilitarEnvioCanhoto,
                canhoto.TipoIntegracaoCanhoto,
                canhoto.EnderecoFTP,
                canhoto.Usuario,
                canhoto.Senha,
                canhoto.Porta,
                canhoto.Diretorio,
                canhoto.Passivo,
                canhoto.UtilizarSFTP,
                canhoto.SSL,
                canhoto.Nomenclatura,
                canhoto.ExtensaoArquivo,
                CertificadoChavePrivada,
                CertificadoChavePrivadaBase64
            };
        }

        private dynamic selectConfiguracaoLayoutEDI(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI obj, Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> servicoAnexoLayoutEDI, List<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI> listaAnexoLayoutEDI, Repositorio.UnitOfWork unitOfWork)
        {
            string CertificadoChavePrivada = string.Empty;
            string CertificadoChavePrivadaBase64 = string.Empty;

            Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI anexoEDI = listaAnexoLayoutEDI.Find(x => x.EntidadeAnexo.Codigo == obj.Codigo);
            if (anexoEDI != null)
            {
                CertificadoChavePrivada = anexoEDI.NomeArquivo;
                CertificadoChavePrivadaBase64 = Convert.ToBase64String(servicoAnexoLayoutEDI.DownloadAnexo(anexoEDI, unitOfWork));
            }

            return new
            {
                obj.Codigo,
                CodigoLayoutEDI = obj.LayoutEDI.Codigo,
                DescricaoLayoutEDI = obj.LayoutEDI.Descricao,
                TipoIntegracao = obj.TipoIntegracao.Tipo,
                DescricaoTipoIntegracao = obj.TipoIntegracao.Descricao,
                obj.Diretorio,
                obj.EmailsAlertaLeituraEDI,
                obj.Emails,
                obj.EnderecoFTP,
                obj.Passivo,
                obj.UtilizarSFTP,
                obj.SSL,
                obj.UtilizarLeituraArquivos,
                obj.AdicionarEDIFilaProcessamento,
                obj.CriarComNomeTemporaraio,
                obj.Porta,
                obj.Senha,
                obj.Usuario,
                CertificadoChavePrivada,
                CertificadoChavePrivadaBase64
            };
        }
    }


    #endregion
}
