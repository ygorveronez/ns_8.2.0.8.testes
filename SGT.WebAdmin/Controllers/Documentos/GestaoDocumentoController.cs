using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize(new string[] { "ObterDetalhes", "RegrasAprovacao", "PesquisarTomadores" }, "Documentos/GestaoDocumento")]
    public class GestaoDocumentoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento,
        Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento,
        Dominio.Entidades.Embarcador.Documentos.GestaoDocumento
    >
    {
        #region Construtores

        public GestaoDocumentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> PesquisarTomadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario?.Codigo ?? 0);

                if (operador?.Tomadores == null)
                    return new JsonpResult(false);

                if (operador.Tomadores.Count <= 0)
                    return new JsonpResult(false);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Não foi possível pesquisar tomadores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarTodosInconsistentes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigoUsuario = this.Usuario.Codigo;
                string stringConexao = _conexao.StringConexao;
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa = ObterFiltrosPesquisa(unidadeDeTrabalho);

                filtrosPesquisa.SituacaoGestaoDocumento = new List<SituacaoGestaoDocumento> { SituacaoGestaoDocumento.Inconsistente };

                Task.Factory.StartNew(() => ProcessarAprovacaoTodosInconsistentes(stringConexao, filtrosPesquisa, codigoUsuario));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AprovarSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigos = Request.GetListParam<int>("Codigos");
                string motivoAprovacao = Request.GetNullableStringParam("MotivoAprovado");
                bool ajustarValorFrete = Request.GetBoolParam("AjustarValorFrete");
                Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, Auditado);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento> listaGestaoDocumento = repositorioGestaoDocumento.BuscarInconsistentesPorCodigos(codigos);
                Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCodigo(this.Usuario.Codigo);

                string observacaoAprovacao;
                if (!string.IsNullOrWhiteSpace(motivoAprovacao))
                    observacaoAprovacao = $"Aprovação autorizada por {usuario.Nome}. Motivo: {motivoAprovacao}";
                else
                    observacaoAprovacao = $"Aprovação autorizada por {usuario.Nome}";

                int resetUnitOfWork = 0;
                foreach (Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento in listaGestaoDocumento)
                {

                    unitOfWork.Start();

                    if ((gestaoDocumento.CargaCTe != null && !gestaoDocumento.CargaCTe.Carga.SituacaoCarga.IsSituacaoCargaCancelada()) || (gestaoDocumento.CargaCTeComplementoInfo != null && gestaoDocumento.CargaCTeComplementoInfo.CargaOcorrencia?.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada))
                    {
                        gestaoDocumento.Initialize();
                        servicoGestaoDocumento.Aprovar(gestaoDocumento, observacaoAprovacao, TipoServicoMultisoftware, ConfiguracaoEmbarcador, ajustarValorFrete);
                    }

                    unitOfWork.CommitChanges();
                    if (resetUnitOfWork > 20)
                    {
                        unitOfWork.FlushAndClear();
                        resetUnitOfWork = 0;
                    }
                    resetUnitOfWork++;
                }

                unitOfWork.FlushAndClear();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os registros selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarInconsistencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigoGestaoDocumento = Request.GetIntParam("Codigo");
                string motivoAprovacao = Request.GetStringParam("ObservacaoAprovacao");
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCodigo(codigoGestaoDocumento);

                VerificarAprovacao(gestaoDocumento, motivoAprovacao);

                gestaoDocumento.Initialize();

                Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, Auditado);
                servicoGestaoDocumento.Aprovar(gestaoDocumento, motivoAprovacao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EmTratativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigoGestaoDocumento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCodigo(codigoGestaoDocumento);

                if (gestaoDocumento == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.Inconsistente)
                    throw new ControllerException("Não é possível deixar o documento em trativa em sua atual situação");

                gestaoDocumento.Initialize();

                Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, Auditado);
                servicoGestaoDocumento.ColocarEmTratativa(gestaoDocumento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao colocar em tratativa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarInconsistencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigoGestaoDocumento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCodigo(codigoGestaoDocumento, true);

                if (gestaoDocumento == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.Inconsistente && gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.EmTratativa)
                    throw new ControllerException("Não é possível rejeitar o documento em sua atual situação.");

                gestaoDocumento.SituacaoGestaoDocumento = SituacaoGestaoDocumento.Rejeitado;

                repositorioGestaoDocumento.Atualizar(gestaoDocumento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesfazerAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Documentos/GestaoDocumento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.GestaoDocumento_PermitirDesfazerAprovacao))
                    return new JsonpResult(false, "Você não possui permissão para desfazer a aprovação.");

                unitOfWork.Start();

                int codigoGestaoDocumento = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, Auditado);

                servicoGestaoDocumento.DesfazerAprovacao(codigoGestaoDocumento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao desfazer a aprovação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesfazerRejeicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigoGestaoDocumento = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, Auditado);

                servicoGestaoDocumento.DesfazerRejeicao(codigoGestaoDocumento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao desfazer a rejeição.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AplicarDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                if (!configuracao.HabilitarDescontoGestaoDocumento) return new JsonpResult(false, "Desconto desabilitado.");

                int codigoGestaoDocumento = Request.GetIntParam("Codigo");
                decimal valorDesconto = Request.GetDecimalParam("ValorDesconto");

                Repositorio.Embarcador.Documentos.GestaoDocumento repGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repGestaoDocumento.BuscarPorCodigo(codigoGestaoDocumento, true);
                string mensagemValidacao = ValidarDesconto(gestaoDocumento, valorDesconto);
                if (!string.IsNullOrWhiteSpace(mensagemValidacao)) return new JsonpResult(false, mensagemValidacao);

                gestaoDocumento.Initialize();
                gestaoDocumento.ValorDesconto = valorDesconto;
                gestaoDocumento.SituacaoGestaoDocumento = ObterSituacaoAprovadoDesconto(gestaoDocumento);
                repGestaoDocumento.Atualizar(gestaoDocumento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aplicar o desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfiguracaoImportacaoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                if (!configuracao.HabilitarDescontoGestaoDocumento) return new JsonpResult(false, "Desconto desabilitado.");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
                {
                    new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Nº Pedido Cliente", Propriedade = "NumeroPedidoCliente", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } },
                    new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Valor de desconto", Propriedade = "ValorDesconto", Tamanho = 20, Obrigatorio = true, Regras = new List<string> { "required" } }
                };
                return new JsonpResult(configuracoes.ToList());
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as configurações de importação do arquivo de descontos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportacaoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                if (!configuracao.HabilitarDescontoGestaoDocumento) return new JsonpResult(false, "Desconto desabilitado.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                retornoImportacao.Total = linhas.Count;
                retornoImportacao.Importados = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    bool sucesso = false;
                    string mensagem = string.Empty;

                    string codigoPedidoCliente = string.Empty;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedidoCliente = (from obj in linhas[i].Colunas where obj.NomeCampo == "NumeroPedidoCliente" select obj).FirstOrDefault();
                    if (colNumeroPedidoCliente != null) codigoPedidoCliente = colNumeroPedidoCliente.Valor;
                    if (string.IsNullOrEmpty(codigoPedidoCliente))
                    {
                        sucesso = false;
                        mensagem = "Registro ignorado na importação: número do pedido é obrigatório.";
                    }
                    else
                    {

                        decimal valorDesconto = -1;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorDesconto = (from obj in linhas[i].Colunas where obj.NomeCampo == "ValorDesconto" select obj).FirstOrDefault();
                        if (colValorDesconto != null)
                        {
                            if (colValorDesconto.Valor != null)
                            {
                                try
                                {
                                    valorDesconto = Utilidades.Decimal.Converter(((string)colValorDesconto.Valor).Replace("-", ""));
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter valor de desconto: {ex.ToString()}", "CatchNoAction");
                                }
                            }
                        }
                        if (valorDesconto < 0)
                        {
                            sucesso = false;
                            mensagem = "Valor de desconto inválido.";
                        }
                        else
                        {
                            Repositorio.Embarcador.Documentos.GestaoDocumento repGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                            Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repGestaoDocumento.BuscarPorCodigoPedidoCliente(codigoPedidoCliente);
                            string mensagemValidacao = ValidarDesconto(gestaoDocumento, valorDesconto);
                            if (!string.IsNullOrWhiteSpace(mensagemValidacao))
                            {
                                sucesso = false;
                                mensagem = mensagemValidacao;
                            }
                            else
                            {
                                gestaoDocumento.Initialize();
                                gestaoDocumento.ValorDesconto = valorDesconto;
                                gestaoDocumento.SituacaoGestaoDocumento = ObterSituacaoAprovadoDesconto(gestaoDocumento);
                                repGestaoDocumento.Atualizar(gestaoDocumento, Auditado);
                                sucesso = true;
                            }
                        }
                    }

                    retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, mensagemFalha = mensagem, processou = sucesso, contar = sucesso });
                    if (sucesso) retornoImportacao.Importados++;

                }
                unitOfWork.CommitChanges();

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar o arquivo de descontos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigoGestaoDocumento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCodigo(codigoGestaoDocumento);

                if (gestaoDocumento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if ((gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.Inconsistente) && (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.SemRegraAprovacao) && (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.EmTratativa))
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                Servicos.Embarcador.Documentos.GestaoDocumentoAprovacao servicoGestaoDocumentoAprovacao = new Servicos.Embarcador.Documentos.GestaoDocumentoAprovacao(unitOfWork);

                servicoGestaoDocumentoAprovacao.CriarAprovacao(gestaoDocumento, TipoServicoMultisoftware);
                repositorioGestaoDocumento.Atualizar(gestaoDocumento);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.SemRegraAprovacao });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar o documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplosDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosDocumentos = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumento> listaGestaoDocumento = repositorioGestaoDocumento.BuscarSemRegraAprovacaoPorCodigos(codigosDocumentos);
                Servicos.Embarcador.Documentos.GestaoDocumentoAprovacao servicoGestaoDocumentoAprovacao = new Servicos.Embarcador.Documentos.GestaoDocumentoAprovacao(unitOfWork);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento in listaGestaoDocumento)
                {
                    servicoGestaoDocumentoAprovacao.CriarAprovacao(gestaoDocumento, TipoServicoMultisoftware);

                    if ((gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.Inconsistente) && (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.SemRegraAprovacao) && (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.EmTratativa))
                    {
                        repositorioGestaoDocumento.Atualizar(gestaoDocumento);
                        totalRegrasReprocessadas++;
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegrasReprocessadas = totalRegrasReprocessadas });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar os documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGestaoDocumento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCodigo(codigoGestaoDocumento);

                if (gestaoDocumento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.CTe.ComponenteFrete servicoComponenteFrete = new Servicos.Embarcador.CTe.ComponenteFrete(unitOfWork);

                string notasCTe = string.Join(",", (from obj in gestaoDocumento.CTe.Documentos orderby obj.Numero select obj.Numero).ToList()).Left(47);
                string notasPreCTe = gestaoDocumento.PreCTe != null ? string.Join(",", (from obj in gestaoDocumento.PreCTe.Documentos orderby obj.Numero select obj.Numero).ToList()).Left(47) : string.Empty;

                var retorno = new
                {
                    Detalhes = new
                    {
                        gestaoDocumento.Codigo,
                        gestaoDocumento.MotivoInconsistenciaGestaoDocumento,
                        gestaoDocumento.ObservacaoAprovacao,
                        gestaoDocumento.SituacaoGestaoDocumento
                    },
                    CTe = new
                    {
                        gestaoDocumento.CTe.Codigo,
                        CodigoEmpresa = gestaoDocumento.CTe.Empresa.Codigo,
                        ValorDesconto = gestaoDocumento.ValorDesconto.ToString("n2"),
                        ValorAReceber = gestaoDocumento.CTe.ValorAReceber.ToString("n2"),
                        AliquotaICMS = gestaoDocumento.CTe.AliquotaICMS.ToString("n2"),
                        BaseCalculoICMS = gestaoDocumento.CTe.BaseCalculoICMS.ToString("n2"),
                        ValorICMS = gestaoDocumento.CTe.ValorICMS.ToString("n2"),
                        CST = gestaoDocumento.CTe?.CST ?? string.Empty,
                        CFOP = gestaoDocumento.CTe.CFOP?.CodigoCFOP ?? 0,
                        AliquotaCBS = (gestaoDocumento.CTe?.AliquotaCBS ?? 0).ToString("n2"),
                        ValorCBS = (gestaoDocumento.CTe?.ValorCBS ?? 0).ToString("n2"),
                        AliquotaIBSUF = (gestaoDocumento.CTe?.AliquotaIBSEstadual ?? 0).ToString("n2"),
                        ValorIBSUF = (gestaoDocumento.CTe?.ValorIBSEstadual ?? 0).ToString("n2"),
                        AliquotaIBSMunicipal = (gestaoDocumento.CTe?.AliquotaIBSMunicipal ?? 0).ToString("n2"),
                        ValorIBSMunicipal = (gestaoDocumento.CTe?.ValorIBSMunicipal ?? 0).ToString("n2"),
                        BaseCalculoIBSCBS = (gestaoDocumento.CTe?.BaseCalculoIBSCBS ?? 0).ToString("n2"),
                        CSTIBSCBS = gestaoDocumento.CTe?.CSTIBSCBS ?? string.Empty,
                        ClassificacaoTributariaIBSCBS = string.IsNullOrEmpty(gestaoDocumento.CTe?.ClassificacaoTributariaIBSCBS) ? "0.00" : gestaoDocumento.CTe.ClassificacaoTributariaIBSCBS,
                        Tomador = gestaoDocumento.CTe.TomadorPagador?.Cliente?.Descricao ?? string.Empty,
                        TipoAmbiente = gestaoDocumento.CTe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "Homologação" : "Produção",
                        Remetente = gestaoDocumento.CTe.Remetente?.Cliente?.Descricao ?? string.Empty,
                        Destinatario = gestaoDocumento.CTe.Destinatario?.Cliente?.Descricao ?? string.Empty,
                        Expedidor = gestaoDocumento.CTe.Expedidor?.Cliente.Descricao ?? "",
                        Recebedor = gestaoDocumento.CTe.Recebedor?.Cliente.Descricao ?? "",
                        Origem = gestaoDocumento.CTe.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                        Destino = gestaoDocumento.CTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                        Emissor = gestaoDocumento.CTe.Empresa?.Descricao + " - " + gestaoDocumento.CTe.Empresa?.CNPJ_Formatado,
                        ValorFrete = gestaoDocumento.CTe.ValorFrete.ToString("n2"),
                        Complemento = gestaoDocumento.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento,
                        Documentos = notasCTe,
                        NumeroCTe = !(gestaoDocumento.CTe?.Numero > 0) ? gestaoDocumento.CTe?.NumeroCRT : gestaoDocumento.CTe?.Numero.ToString(),
                        Rota = gestaoDocumento.Carga?.Rota?.Descricao ?? string.Empty,
                        Peso = gestaoDocumento.CTe?.Peso.ToString("n2") ?? string.Empty,
                        ValorMercadoria = gestaoDocumento.CTe?.ValorTotalMercadoria.ToString("n2") ?? string.Empty
                    },
                    PreCTe = gestaoDocumento.PreCTe != null ? new
                    {
                        gestaoDocumento.PreCTe.Codigo,
                        CodigoEmpresa = gestaoDocumento.PreCTe.Empresa.Codigo,
                        ValorDesconto = gestaoDocumento.ValorDesconto.ToString("n2"),
                        ValorAReceber = gestaoDocumento.PreCTe.ValorAReceber.ToString("n2"),
                        AliquotaICMS = gestaoDocumento.PreCTe.AliquotaICMS.ToString("n2"),
                        BaseCalculoICMS = gestaoDocumento.PreCTe.BaseCalculoICMS.ToString("n2"),
                        ValorICMS = gestaoDocumento.PreCTe.ValorICMS.ToString("n2"),
                        CST = gestaoDocumento.PreCTe.CST ?? string.Empty,
                        CFOP = gestaoDocumento.PreCTe.CFOP?.CodigoCFOP ?? 0,
                        AliquotaCBS = (gestaoDocumento.PreCTe?.AliquotaCBS ?? 0).ToString("n2"),
                        ValorCBS = (gestaoDocumento.PreCTe?.ValorCBS ?? 0).ToString("n2"),
                        AliquotaIBSUF = (gestaoDocumento.PreCTe?.AliquotaIBSEstadual ?? 0).ToString("n2"),
                        ValorIBSUF = (gestaoDocumento.PreCTe?.ValorIBSEstadual ?? 0).ToString("n2"),
                        AliquotaIBSMunicipal = (gestaoDocumento.PreCTe?.AliquotaIBSMunicipal ?? 0).ToString("n2"),
                        ValorIBSMunicipal = (gestaoDocumento.PreCTe?.ValorIBSMunicipal ?? 0).ToString("n2"),
                        BaseCalculoIBSCBS = (gestaoDocumento.PreCTe?.BaseCalculoIBSCBS ?? 0).ToString("n2"),
                        CSTIBSCBS = gestaoDocumento.PreCTe?.CSTIBSCBS ?? string.Empty,
                        ClassificacaoTributariaIBSCBS = string.IsNullOrEmpty(gestaoDocumento.PreCTe?.ClassificacaoTributariaIBSCBS) ? "0.00" : gestaoDocumento.PreCTe.ClassificacaoTributariaIBSCBS,
                        Tomador = gestaoDocumento.PreCTe.Tomador?.Cliente?.Descricao ?? string.Empty,
                        Remetente = gestaoDocumento.PreCTe.Remetente?.Cliente?.Descricao ?? string.Empty,
                        TipoAmbiente = gestaoDocumento.PreCTe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "Homologação" : "Produção",
                        Destinatario = gestaoDocumento.PreCTe.Destinatario?.Cliente?.Descricao ?? string.Empty,
                        Expedidor = gestaoDocumento.PreCTe.Expedidor?.Cliente?.Descricao ?? "",
                        Recebedor = gestaoDocumento.PreCTe.Recebedor?.Cliente?.Descricao ?? "",
                        Origem = gestaoDocumento.PreCTe.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                        Destino = gestaoDocumento.PreCTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                        Emissor = gestaoDocumento.PreCTe.Empresa?.Descricao + " - " + gestaoDocumento.PreCTe.Empresa?.CNPJ_Formatado,
                        Rota = gestaoDocumento.Carga?.Rota?.Descricao ?? string.Empty,
                        ValorFrete = gestaoDocumento.PreCTe.ValorFrete.ToString("n2"),
                        Complemento = gestaoDocumento.PreCTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento,
                        Documentos = notasPreCTe,
                        NumeroCTe = !(gestaoDocumento.CTe?.Numero > 0) ? gestaoDocumento.CTe?.NumeroCRT : gestaoDocumento.CTe?.Numero.ToString(),
                        Peso = gestaoDocumento.PreCTe?.PesoTotalCarga.ToString("n2") ?? string.Empty,
                        ValorMercadoria = gestaoDocumento.PreCTe?.ValorTotalMercadoria.ToString("n2") ?? string.Empty
                    } : null,
                    FluxoInternacional = gestaoDocumento.Carga.Internacional,
                    ComponentesFrete = servicoComponenteFrete.ObterInformacoesComponentesFrete(gestaoDocumento.CTe, gestaoDocumento.PreCTe),
                    CTesAnteriores = ObterCTesAnteriores(gestaoDocumento, unitOfWork)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter detalhes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais Sobrescritos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento ObterFiltrosPesquisa(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            string chaves = Request.GetStringParam("Chave");
            string NumeroNotasFiscais = Request.GetStringParam("NumeroNF");
            string NumeroPedidosClientes = Request.GetStringParam("NumeroPedidoCliente");
            string chaveNFe = Request.GetStringParam("ChaveNFe");
            List<int> codigosTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? new List<int>() { this.Usuario?.Empresa?.Codigo ?? 0 } : Request.GetListParam<int>("Empresa");

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento()
            {
                Chaves = string.IsNullOrWhiteSpace(chaves) ? new List<string>() : chaves.Replace(" ", "").Split(',').ToList<string>(),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoOcorrencia = Request.GetIntParam("Ocorrencia"),
                CodigoTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigoUsuarioAprovador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? 0 : Request.GetIntParam("Usuario"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                MotivoInconsistenciaGestaoDocumento = Request.GetListEnumParam<MotivoInconsistenciaGestaoDocumento>("MotivoInconsistenciaGestaoDocumento"),
                CodigoCTe = Request.GetListParam<int>("NumeroCTe"),
                NumeroNotasFiscais = string.IsNullOrWhiteSpace(NumeroNotasFiscais) ? new List<string>() : NumeroNotasFiscais.Replace(" ", "").Split(',').ToList<string>(),
                NumeroPedidosClientes = string.IsNullOrWhiteSpace(NumeroPedidosClientes) ? new List<string>() : NumeroPedidosClientes.Replace(" ", "").Split(',').ToList<string>(),
                RegistroComCarga = Request.GetBoolParam("RegistroComCarga"),
                Remetente = Request.GetDoubleParam("Remetente"),
                Serie = Request.GetIntParam("Serie"),
                SituacaoGestaoDocumento = Request.GetListEnumParam<SituacaoGestaoDocumento>("SituacaoGestaoDocumento"),
                ChavesNFe = string.IsNullOrWhiteSpace(chaveNFe) ? new List<string>() : chaveNFe.Replace(" ", "").Split(',').ToList(),
            };

            List<double> tomadores = Request.GetListParam<double>("Tomador");
            filtrosPesquisa.Tomador = tomadores.Count > 0 ? tomadores : ObterListaCnpjCpfTomadorPermitidosOperadorLogistica(unidadeDeTrabalho);

            if (codigosTransportador.Count > 0)
                codigosTransportador.AddRange(repositorioEmpresa.BuscarCodigosFiliaisVinculadas(codigosTransportador));

            filtrosPesquisa.CodigoEmpresa = codigosTransportador;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa(bool exportarPesquisa)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork).BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork).BuscarPrimeiroRegistro();

                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                bool verificarComponenteFreteValorComOutraDescricao = !ConfiguracaoEmbarcador.ValidarSomenteFreteLiquidoNaImportacaoCTe && configuracaoCargaEmissaoDocumento.ControlarValoresComponentesCTe;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCargaCTe", false);
                grid.AdicionarCabecalho("CodigoPreCTe", false);
                grid.AdicionarCabecalho("SituacaoGestaoDocumento", false);
                grid.AdicionarCabecalho("MotivoInconsistenciaGestaoDocumento", false);
                grid.AdicionarCabecalho("Número", "Numero", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Carga", "Carga", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("NFes", "NFeRecebida", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Chave", "Chave", 9, Models.Grid.Align.left, false).UtilizarFormatoTexto(true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Emissão", "DataEmissaoFormatada", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tomador", "Tomador", 12, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho("Transportador", false);
                else
                    grid.AdicionarCabecalho("Transportador", "Transportador", 12, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Frete NF (XML)", "FreteNfXml", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Motivo da Pendência", "MotivoPendencia", 7, Models.Grid.Align.center, false);

                if (ConfiguracaoEmbarcador.HabilitarDescontoGestaoDocumento)
                    grid.AdicionarCabecalho("Valor Desconto", "ValorDesconto", 8, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("Valor Recebido", "ValorRecebido", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor Esperado", "ValorEsperado", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Diferença Valores", "DiferencaValores", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("% da Diferença", "PercentualDiferencaValor", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Valor ICMS Recebido", "ValorICMSRecebido", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor ICMS Esperado", "ValorICMSEsperado", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Aliquota IBS Estadual", "AliquotaIBSUF", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Aliquota IBS Municipal", "AliquotaIBSMunicipal", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor IBS Estadual", "ValorIBSEstadual", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor IBS Municipal", "ValorIBSMunicipal", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Aliquota CBS", "AliquotaCBS", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor CBS", "ValorCBS", 8, Models.Grid.Align.center, false);

                if (ConfiguracaoEmbarcador.ExibirFiltroEColunaCodigoPedidoClienteGestaoDocumentos)
                    grid.AdicionarCabecalho("Nº Pedido Cliente", "NumeroPedidoCliente", 8, Models.Grid.Align.right, false);

                if (ConfiguracaoEmbarcador.ExibirPesoCargaEPesoCubadoGestaoDocumentos)
                {
                    grid.AdicionarCabecalho("Peso Carga", "PesoCarga", 8, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Peso Cubado", "PesoCubado", 8, Models.Grid.Align.right, false);
                }

                grid.AdicionarCabecalho("Destinatário", "Destinatario", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Cidade Destino", "CidadeDestino", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Destino", "UFDestino", 7, Models.Grid.Align.left, false);

                if (configuracaoFinanceiro.ExibirNumeroPedidoEmbarcadorGestaoDocumentos)
                    grid.AdicionarCabecalho("N° Pedido", "NumeroPedido", 7, Models.Grid.Align.left, false);

                List<(string Descricao, int CodigoComponente, string DescricaoComponente)> componentesFreteComOutraDescricao = new List<(string Descricao, int CodigoComponente, string DescricaoComponente)>();
                List<(int CodigoComponente, string DescricaoComponente)> componentesFrete = new List<(int CodigoComponente, string DescricaoComponente)>();
                int ultimaColunaDinamica = 1;
                int numeroMaximoComplementos = 30;

                grid.AdicionarCabecalho("Valor total NFes", "ValorNota", 7, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Prioridade", "Prioridade", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Regra", "Regra", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Importações", "QuantidadeImportacoesCTe", 7, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Data Importação", "DataImportacaoCTeFormatada", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Último Aprovador", "UltimoAprovador", 10, Models.Grid.Align.left, false, false);

                if (configuracaoCargaEmissaoDocumento.ControlarValoresComponentesCTe)
                {
                    if (exportarPesquisa)
                        grid.AdicionarCabecalho("CEP Destino", "CEPDestino", 7, Models.Grid.Align.left, false);

                    grid.AdicionarCabecalho("Chave Nota", "ChaveNota", 7, Models.Grid.Align.left, false).UtilizarFormatoTexto(true);
                    grid.AdicionarCabecalho("Motivo Rejeição", "MotivoRejeicao", 7, Models.Grid.Align.left, false);

                    Repositorio.TipoDeOcorrenciaDeCTe repositorioOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                    Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado repositorioTransportadorComponenteCTeImportado = new Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado(unitOfWork);

                    if (verificarComponenteFreteValorComOutraDescricao)
                    {
                        List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> componentes = repositorioTransportadorComponenteCTeImportado.BuscarComponentesPorOutraDescricaoDePara();
                        componentesFreteComOutraDescricao = componentes.Select(o => ValueTuple.Create(o.Descricao, o.ComponenteFrete.Codigo, o.ComponenteFrete.Descricao)).Distinct().ToList();
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = repositorioTransportadorComponenteCTeImportado.BuscarComponentesDePara();
                        componentesFreteComOutraDescricao = componentes.Select(o => ValueTuple.Create(o.Descricao, o.Codigo, o.Descricao)).ToList();
                    }

                    componentesFrete = componentesFreteComOutraDescricao.Select(o => ValueTuple.Create(o.CodigoComponente, o.DescricaoComponente)).Distinct().ToList();

                    List<(int CodigoComponente, bool PossuiBloqueio)> listaBloqueiosComponentes = componentesFrete.Count > 0 ? repositorioOcorrencia.ExisteBloqueioTransportadorPorComponentes(componentesFrete.Select(obj => obj.CodigoComponente).ToList()) : new List<(int CodigoComponente, bool PossuiBloqueio)>();

                    for (int indiceComponenteFrete = 0; indiceComponenteFrete < componentesFrete.Count; indiceComponenteFrete++)
                    {
                        if ((indiceComponenteFrete == numeroMaximoComplementos) || (ultimaColunaDinamica >= 57))
                            break;

                        (int CodigoComponente, string DescricaoComponente) componenteFrete = componentesFrete[indiceComponenteFrete];

                        bool ocultarComponente = (
                            (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) &&
                            listaBloqueiosComponentes.Any(o => o.CodigoComponente == componenteFrete.CodigoComponente && o.PossuiBloqueio)
                        );

                        if (ocultarComponente)
                            continue;

                        grid.AdicionarCabecalho($"{componenteFrete.DescricaoComponente} Recebido", $"ValorComponente{ultimaColunaDinamica}", 8, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, componenteFrete.CodigoComponente);
                        grid.AdicionarCabecalho($"{componenteFrete.DescricaoComponente} Esperado", $"ValorComponente{(ultimaColunaDinamica + 1)}", 8, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, componenteFrete.CodigoComponente);

                        ultimaColunaDinamica += 2;
                    }
                }

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "GestaoDocumento/Pesquisa", "grid-gestao-documento");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                int totalRegistros = repositorioGestaoDocumento.ContarConsulta(filtrosPesquisa);
                IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento> gestaoDocumentos = (totalRegistros > 0) ? repositorioGestaoDocumento.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento>();

                if ((componentesFrete.Count > 0) && (totalRegistros > 0))
                {
                    List<int> codigosPreCTe = gestaoDocumentos.Select(o => o.CodigoPreCTe).ToList();
                    List<int> codigosCTe = gestaoDocumentos.Select(o => o.CodigoCTe).ToList();
                    List<Dominio.Entidades.ComponentePrestacaoPreCTE> componentesPrestacaoPreCTE = new Repositorio.ComponentePrestacaoPreCTE(unitOfWork).BuscarPorPreCTes(codigosPreCTe);
                    List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCTE = new Repositorio.ComponentePrestacaoCTE(unitOfWork).BuscarInformadosPorCTes(codigosCTe, verificarComponenteFreteValorComOutraDescricao);

                    for (int indiceGestaoDocumento = 0; indiceGestaoDocumento < gestaoDocumentos.Count; indiceGestaoDocumento++)
                    {
                        int numeroProximoComponente = 0;

                        for (int indiceComponenteFrete = 0; indiceComponenteFrete < componentesFrete.Count; indiceComponenteFrete++)
                        {
                            if ((indiceComponenteFrete == numeroMaximoComplementos) || (numeroProximoComponente >= ultimaColunaDinamica))
                                continue;

                            System.Reflection.PropertyInfo propertyInfoValorRecebido = gestaoDocumentos[indiceGestaoDocumento].GetType().GetProperty($"ValorComponente{++numeroProximoComponente}");
                            System.Reflection.PropertyInfo propertyInfoValorEsperado = gestaoDocumentos[indiceGestaoDocumento].GetType().GetProperty($"ValorComponente{++numeroProximoComponente}");

                            Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento gestaoDocumento = gestaoDocumentos[indiceGestaoDocumento];
                            (int CodigoComponente, string DescricaoComponente) componente = componentesFrete[indiceComponenteFrete];

                            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacao = componentesPrestacaoCTE.Where(o => o.CTE.Codigo == gestaoDocumento.CodigoCTe && o.Nome == componente.DescricaoComponente).ToList();

                            if (verificarComponenteFreteValorComOutraDescricao && componentesPrestacao.Count == 0)
                            {
                                List<(string Descricao, int CodigoComponente, string DescricaoComponente)> teste = componentesFreteComOutraDescricao.Where(o => o.CodigoComponente == componente.CodigoComponente && o.Descricao != componente.DescricaoComponente).ToList();
                                if (teste.Count > 0)
                                    componentesPrestacao = componentesPrestacaoCTE.Where(o => o.CTE.Codigo == gestaoDocumento.CodigoCTe && teste.Any(t => t.Descricao == o.Nome)).ToList();
                            }

                            string valorComponenteRecebido = componentesPrestacao.Select(o => o.Valor).Sum().ToString("n2");
                            string valorComponenteEsperado = componentesPrestacaoPreCTE.Where(o => o.PreCTE.Codigo == gestaoDocumento.CodigoPreCTe && o.Nome == componente.DescricaoComponente)?.Select(o => o.Valor)?.Sum().ToString("n2");

                            propertyInfoValorRecebido.SetValue(gestaoDocumentos[indiceGestaoDocumento], valorComponenteRecebido, null);
                            propertyInfoValorEsperado.SetValue(gestaoDocumentos[indiceGestaoDocumento], valorComponenteEsperado, null);
                        }
                    }
                }

                grid.AdicionaRows(gestaoDocumentos);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("DataEmissaoFormatada"))
                return propriedadeOrdenar.Replace("Formatada", "");

            return propriedadeOrdenar;
        }

        private SituacaoGestaoDocumento ObterSituacaoAprovadoDesconto(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            if (gestaoDocumento.SituacaoGestaoDocumentoAprovada)
                return (gestaoDocumento.ValorDesconto > 0) ? SituacaoGestaoDocumento.AprovadoComDesconto : SituacaoGestaoDocumento.Aprovado;

            return gestaoDocumento.SituacaoGestaoDocumento;
        }

        private void ProcessarAprovacaoTodosInconsistentes(string stringConexao, Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa, int codigoUsuario)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCodigo(codigoUsuario);
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    Texto = "",
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                    Usuario = usuario
                };

                Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, auditado);
                Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);

                IList<int> codigos = repositorioGestaoDocumento.ConsultarCodigos(filtrosPesquisa);
                string observacaoAprovacao = $"Aprovação em massa autorizada por {usuario.Nome}";

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                foreach (int codigo in codigos)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento = repositorioGestaoDocumento.BuscarPorCodigo(codigo);

                    if ((gestaoDocumento.CargaCTe != null && !gestaoDocumento.CargaCTe.Carga.SituacaoCarga.IsSituacaoCargaCancelada()) || (gestaoDocumento.CargaCTeComplementoInfo != null && gestaoDocumento.CargaCTeComplementoInfo.CargaOcorrencia?.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada))
                    {
                        gestaoDocumento.Initialize();
                        servicoGestaoDocumento.Aprovar(gestaoDocumento, observacaoAprovacao, TipoServicoMultisoftware, configuracao, false);
                    }

                    unitOfWork.CommitChanges();
                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private string ValidarDesconto(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, decimal valorDesconto)
        {
            if (gestaoDocumento == null) return "Gestão de documento não encontrado.";
            if (valorDesconto < 0) return "Desconto não pode ser negativo.";
            if (valorDesconto > gestaoDocumento.CTe.ValorAReceber) return "Desconto não pode ser maior que o valor a receber do CTe.";
            return null;
        }

        private dynamic ObterCTesAnteriores(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.GestaoDocumentoHistoricoCTe repositorioGestaoDocumentoHistoricoCTe = new Repositorio.Embarcador.Documentos.GestaoDocumentoHistoricoCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumentoHistoricoCTe> gestaoDocumentoHistoricoCTes = repositorioGestaoDocumentoHistoricoCTe.BuscarPorGestaoDocumento(gestaoDocumento.Codigo);

            var ctesAnteriores = (from gestaoDocumentoHistorico in gestaoDocumentoHistoricoCTes
                                  select new
                                  {
                                      gestaoDocumentoHistorico.CTe.Codigo,
                                      CodigoEmpresa = gestaoDocumentoHistorico.CTe.Empresa.Codigo,
                                      ValorDesconto = 0.ToString("n2"),
                                      ValorAReceber = gestaoDocumentoHistorico.CTe.ValorAReceber.ToString("n2"),
                                      AliquotaICMS = gestaoDocumentoHistorico.CTe.AliquotaICMS.ToString("n2"),
                                      BaseCalculoICMS = gestaoDocumentoHistorico.CTe.BaseCalculoICMS.ToString("n2"),
                                      ValorICMS = gestaoDocumentoHistorico.CTe.ValorICMS.ToString("n2"),
                                      CST = gestaoDocumentoHistorico.CTe?.CST ?? string.Empty,
                                      CFOP = gestaoDocumentoHistorico.CTe.CFOP?.CodigoCFOP ?? 0,
                                      Tomador = gestaoDocumentoHistorico.CTe.TomadorPagador?.Cliente?.Descricao ?? string.Empty,
                                      TipoAmbiente = gestaoDocumentoHistorico.CTe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "Homologação" : "Produção",
                                      Remetente = gestaoDocumentoHistorico.CTe.Remetente?.Cliente?.Descricao ?? string.Empty,
                                      Destinatario = gestaoDocumentoHistorico.CTe.Destinatario?.Cliente?.Descricao ?? string.Empty,
                                      Expedidor = gestaoDocumentoHistorico.CTe.Expedidor?.Cliente.Descricao ?? "",
                                      Recebedor = gestaoDocumentoHistorico.CTe.Recebedor?.Cliente.Descricao ?? "",
                                      Origem = gestaoDocumentoHistorico.CTe.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                                      Destino = gestaoDocumentoHistorico.CTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                                      Emissor = gestaoDocumentoHistorico.CTe.Empresa?.Descricao ?? string.Empty,
                                      ValorFrete = gestaoDocumentoHistorico.CTe.ValorFrete.ToString("n2"),
                                      Complemento = gestaoDocumentoHistorico.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento,
                                      Documentos = string.Join(",", (from obj in gestaoDocumentoHistorico.CTe.Documentos select obj.Numero).ToList()).Left(47),
                                      NumeroCTe = gestaoDocumentoHistorico.CTe?.Numero ?? 0,
                                      Rota = string.Empty,
                                  }).ToList();

            return ctesAnteriores;
        }

        private void EnviarEmailRejeicao(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.DocumentosPreCTE repositorioDocumentosPreCTE = new Repositorio.DocumentosPreCTE(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Servicos.PreCTe svcPreCTe = new Servicos.PreCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            string emailTransportador = repositorioEmpresa.BuscarEmailEnvioCTeRejeitado(gestaoDocumento.CTe.Empresa.Codigo);

            if (configuracaoEmail == null || string.IsNullOrEmpty(emailTransportador))
                return;

            string chave = repositorioDocumentosPreCTE.BuscarChaveNFePorPreCte(gestaoDocumento.PreCTe.Codigo);
            byte[] documentoAnexo = System.Text.Encoding.Unicode.GetBytes(svcPreCTe.BuscarXMLPreCte(gestaoDocumento.PreCTe));

            string motivo = Request.GetStringParam("Motivo");

            if (documentoAnexo == null)
                return;

            string assunto = "CTe referente a carga " + gestaoDocumento.Carga.CodigoCargaEmbarcador + " rejeitado.";

            string mensagemEmail = $"Olá,<br/><br/>Carga:{gestaoDocumento.Carga.CodigoCargaEmbarcador}<br/><br/>";
            mensagemEmail += $"N° CTe:  {gestaoDocumento.CTe.Numero} <br/>";
            mensagemEmail += $"Situação:  Rejeitado <br/>";
            mensagemEmail += $"Motivo :  {motivo} <br/>";
            mensagemEmail += $"Chave :  {chave} <br/><br/>";
            mensagemEmail += $"Em anexo o xml do Pré CT-e";

            Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, emailTransportador, null, null, assunto, mensagemEmail, configuracaoEmail.Smtp, out string mensagem, configuracaoEmail.DisplayEmail, new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(new System.IO.MemoryStream(documentoAnexo), "pre_cte" + ".xml", "application/xml") }, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, unitOfWork);
        }

        private void VerificarAprovacao(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, string motivoAprovacao)
        {
            if (gestaoDocumento == null)
                throw new ControllerException("Não foi possível encontrar o registro.");

            if (!ConfiguracaoEmbarcador.NaoExigirMotivoAprovacaoCTeInconsistente && string.IsNullOrEmpty(motivoAprovacao))
                throw new ControllerException("É obrigatório informar o motivo da aprovação");

            if (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.Inconsistente && gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.EmTratativa)
                throw new ControllerException("Não é possível aprovar o documento em sua atual situação");

            if (gestaoDocumento.CargaCTe == null && gestaoDocumento.CargaCTeComplementoInfo == null)
                throw new ControllerException("Não é possível aprovar um CT-e sem carga ou ocorrência");

            if (gestaoDocumento.CargaCTe != null && gestaoDocumento.CargaCTe.Carga.SituacaoCarga.IsSituacaoCargaCancelada())
                throw new ControllerException("A carga do documento está cancelada ou anulada.");

            if (gestaoDocumento.CargaCTeComplementoInfo != null && gestaoDocumento.CargaCTeComplementoInfo.CargaOcorrencia?.SituacaoOcorrencia == SituacaoOcorrencia.Cancelada)
                throw new ControllerException("A ocorrência do documento está cancelada ou anulada.");
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento)
        {
            return gestaoDocumento.SituacaoGestaoDocumento == SituacaoGestaoDocumento.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);
            IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento> listaGestaoDocumento;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaGestaoDocumento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                listaGestaoDocumento = repositorioGestaoDocumento.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaGestaoDocumento.Remove(new Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                listaGestaoDocumento = new List<Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaGestaoDocumento.Add(new Dominio.Relatorios.Embarcador.DataSource.Documentos.GestaoDocumento() { Codigo = (int)itemSelecionado.Codigo });
            }

            return (from gestaoDocumento in listaGestaoDocumento select Convert.ToInt32(gestaoDocumento.Codigo)).ToList();
        }

        protected override Models.Grid.Grid ObterGridExportarPesquisa()
        {
            return ObterGridPesquisa(exportarPesquisa: true);
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            return ObterGridPesquisa(exportarPesquisa: false);
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Documentos.GestaoDocumento gestaoDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            if (gestaoDocumento.SituacaoGestaoDocumento != SituacaoGestaoDocumento.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(gestaoDocumento.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Documentos.GestaoDocumento repositorioGestaoDocumento = new Repositorio.Embarcador.Documentos.GestaoDocumento(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                Servicos.Embarcador.Documentos.GestaoDocumentoAprovacao servicoGestaoDocumentoAprovacao = new Servicos.Embarcador.Documentos.GestaoDocumentoAprovacao(unitOfWork);

                if (!servicoGestaoDocumentoAprovacao.LiberarProximaPrioridadeAprovacao(gestaoDocumento, TipoServicoMultisoftware))
                    return;

                bool ajustarValorFrete = Request.GetBoolParam("AjustarValorFrete");
                Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork);
                servicoGestaoDocumento.Aprovar(gestaoDocumento, "Aprovação autorizada por regra de autorização", TipoServicoMultisoftware, ConfiguracaoEmbarcador, ajustarValorFrete);
            }
            else
            {
                gestaoDocumento.SituacaoGestaoDocumento = SituacaoGestaoDocumento.Rejeitado;

                if (gestaoDocumento.PreCTe != null)
                    EnviarEmailRejeicao(gestaoDocumento, unitOfWork);
            }

            repositorioGestaoDocumento.Atualizar(gestaoDocumento);
        }

        #endregion Métodos Protegidos Sobrescritos
    }
}
