using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosTransporte
{
    [CustomAuthorize(new string[] { "ObterTotais", "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaDadosTransporteIntegracaoController : BaseController
    {
        #region Construtores

        public CargaDadosTransporteIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("Tipo");

                int codigoCarga = Request.GetIntParam("Carga");
                bool etapaIncioSemNota = Request.GetBoolParam("EtapaInicio");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("Pedido", "Pedido", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Protocolo", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Retorno", 18, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

                int countCTes = repIntegracao.ContarConsulta(codigoCarga, situacao, tipo, etapaIncioSemNota);

                TipoIntegracao tipoIntegracaoTrizyContrato = TipoIntegracao.TrizyContrato;

                List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

                if (countCTes > 0)
                {
                    if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";

                    listaIntegracao = repIntegracao.Consultar(codigoCarga, situacao, tipo, etapaIncioSemNota, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                }

                grid.setarQuantidadeTotal(countCTes);

                grid.AdicionaRows(listaIntegracao.Select(obj => new
                {
                    obj.Codigo,
                    obj.TipoIntegracao.Tipo,
                    obj.SituacaoIntegracao,
                    Pedido = obj.CargaPedido?.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                    Situacao = obj.SituacaoIntegracao.ObterDescricao(),
                    TipoIntegracao = obj.IntegracaoColeta ? obj.TipoIntegracao.Tipo.ObterDescricao() + " (Coleta)" : obj.TipoIntegracao.Tipo.ObterDescricao(),
                    Retorno = obj.ProblemaIntegracao,
                    Protocolo = !string.IsNullOrWhiteSpace(obj.Protocolo) ? obj.Protocolo : "",
                    Tentativas = obj.NumeroTentativas,
                    DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                    DT_RowColor = obj.SituacaoIntegracao.ObterCorLinha(),
                    DT_FontColor = obj.SituacaoIntegracao.ObterCorFonte(),
                }).ToList());

                if (!listaIntegracao.Any(o => o.TipoIntegracao.Tipo == tipoIntegracaoTrizyContrato))
                    grid.OcultarCabecalho("Pedido");

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

        public async Task<IActionResult> ObterComprovanteAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaDadosTransporteIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (cargaDadosTransporteIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma retornoRotograma = Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.ObterComprovanteAgendamento(cargaDadosTransporteIntegracao.Carga, TipoServicoMultisoftware, unitOfWork, _conexao.StringConexao);
                    return new JsonpResult(retornoRotograma);
                }
                else
                {
                    return new JsonpResult(false, "O tipo de integração não possui um link de acesso externo habilitado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o comprovante de agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterRotogramaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaDadosTransporteIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (cargaDadosTransporteIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma retornoRotograma = Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.ObterRotograma(cargaDadosTransporteIntegracao.Carga, TipoServicoMultisoftware, unitOfWork, _conexao.StringConexao);
                    return new JsonpResult(retornoRotograma);
                }
                else
                {
                    return new JsonpResult(false, "O tipo de integração não possui um link de acesso externo habilitado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o Rotograma.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                bool etapaInicioSemNota = Request.GetBoolParam("EtapaInicio");

                int totalAguardandoIntegracao = 0, totalIntegrado = 0, totalProblemaIntegracao = 0, totalAguardandoRetorno = 0;
                int totalGeral = repIntegracao.ContarConsulta(codigoCarga, null, null, etapaInicioSemNota);

                if (totalGeral > 0)
                {
                    totalAguardandoIntegracao = repIntegracao.ContarConsulta(codigoCarga, SituacaoIntegracao.AgIntegracao, null, etapaInicioSemNota);
                    totalIntegrado = repIntegracao.ContarConsulta(codigoCarga, SituacaoIntegracao.Integrado, null, etapaInicioSemNota);
                    totalProblemaIntegracao = repIntegracao.ContarConsulta(codigoCarga, SituacaoIntegracao.ProblemaIntegracao, null, etapaInicioSemNota);
                    totalAguardandoRetorno = repIntegracao.ContarConsulta(codigoCarga, SituacaoIntegracao.AgRetorno, null, etapaInicioSemNota);
                }

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalGeral,
                    Visibilidade = totalGeral > 0
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = await repIntegracao.BuscarPorCodigoAsync(codigo);

                if (cargaDadosTransporteIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (cargaDadosTransporteIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                {
                    List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                if (cargaDadosTransporteIntegracao.TipoIntegracao.Tipo == TipoIntegracao.Trizy && cargaDadosTransporteIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    return new JsonpResult(false, true, "Não é possível Reenviar a Integração Trizy quando a Situação da integração estiver Integrado.");

                if (cargaDadosTransporteIntegracao.TipoIntegracao.Tipo != TipoIntegracao.OpenTech || cargaDadosTransporteIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                {
                    if (cargaDadosTransporteIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                        return new JsonpResult(false, true, "Não é possível Reenviar a Integração enquanto a Situação da integração estiver Aguardando Integração.");

                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaDadosTransporteIntegracao, null, "Reenviou Integração.", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, cancellationToken);
                    await repIntegracao.AtualizarAsync(cargaDadosTransporteIntegracao);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReenviarTodos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("Tipo"); ;

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> integracoes = await repCargaDadosTransporteIntegracao.BuscarPorCargaAsync(codigoCarga, situacao, tipo);

                await unitOfWork.StartAsync(cancellationToken);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracao in integracoes)
                {
                    if (integracao.TipoIntegracao.Tipo == TipoIntegracao.Trizy && integracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                        return new JsonpResult(false, true, "Não é possível Reenviar a Integração Trizy quando a Situação da integração estiver Integrado.");

                    if (integracao.TipoIntegracao.Tipo != TipoIntegracao.OpenTech || integracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                    {
                        if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                            return new JsonpResult(false, true, "Não é possível Reenviar a Integração enquanto a Situação da integração estiver Aguardando Integração.");

                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                        await repCargaDadosTransporteIntegracao.AtualizarAsync(integracao);
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, integracao, null, "Reenviou Integração.", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update, cancellationToken);
                    }
                }

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Reenviou as integrações da Carga", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom repositorioConfigDigitalCom = new Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.IntegracaoDigitalComArquivosTransacao repositorioDigitalComArquivos = new Repositorio.Embarcador.Cargas.IntegracaoDigitalComArquivosTransacao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom configuracaoDigitalCom = await repositorioConfigDigitalCom.BuscarAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = await repositorioConfiguracaoVeiculo.BuscarPrimeiroRegistroAsync();
                List<Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao> integracaoDigitalComArquivos = new List<Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao>();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                if (configuracaoDigitalCom != null && (configuracaoDigitalCom.ValidacaoTAGDigitalCom || configuracaoVeiculo.ValidarTAGDigitalCom))
                {
                    grid.AdicionarCabecalho("Meio Pagamento", "MeioPagamentoDigitalCom", 15, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("ID Retorno", "IDRetornoDigitalCom", 8, Models.Grid.Align.center, true);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracao = await repositorioIntegracao.BuscarPorCodigoAsync(codigo);

                if (configuracaoDigitalCom != null)
                    integracaoDigitalComArquivos = await repositorioDigitalComArquivos.BuscarPorCargaAsync(integracao.Carga.Codigo);

                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count);

                var retorno = integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite).Select(obj => new
                {
                    obj.Codigo,
                    Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                    obj.DescricaoTipo,
                    obj.Mensagem,
                    MeioPagamentoDigitalCom = integracaoDigitalComArquivos.Where(x => x.CargaCTeIntegracaoArquivo.Codigo == obj.Codigo).Select(mdc => mdc.MeioPagamentoDigitalCom).FirstOrDefault().HasValue ? integracaoDigitalComArquivos.Where(x => x.CargaCTeIntegracaoArquivo.Codigo == obj.Codigo).Select(mdc => mdc.MeioPagamentoDigitalCom).FirstOrDefault().Value.ObterDescricao() : string.Empty,
                    IDRetornoDigitalCom = integracaoDigitalComArquivos.Where(x => x.CargaCTeIntegracaoArquivo.Codigo == obj.Codigo).Select(mdc => mdc.IDRetornoDigitalCom).FirstOrDefault()
                }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadAutorizacaoEmbarque()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string mensagemErro = string.Empty;

                byte[] arquivo = new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unidadeDeTrabalho).ObterAutorizacaoEmbarque(cargaIntegracao, out mensagemErro);

                if (arquivo == null)
                    return new JsonpResult(false, true, mensagemErro);

                return Arquivo(arquivo, "application/pdf", "AE_" + cargaIntegracao.Protocolo + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da autorização de embarque.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracao = repCargaDadosTransporteIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Carga " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download.");
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCarga = Request.GetIntParam("Carga");

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaLog repLog = new Repositorio.Embarcador.Cargas.CargaLog(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo, TipoPedido.Entrega, true, configuracaoEmbarcador.RatearFretePedidosAposLiberarEmissaoSemNFe);

                if (carga == null)
                    return new JsonpResult(true, false, "Carga não encontrada.");

                if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                    return new JsonpResult(true, false, "A situação da carga não permite a finalização da etapa.");

                unidadeDeTrabalho.Start();

                carga.LiberadaComProblemaIntegracaoDadosTransporte = true;
                carga.PossuiPendencia = false;
                carga.MotivoPendencia = "";
                carga.AguardarIntegracaoDadosTransporte = false;

                if (carga.NaoExigeVeiculoParaEmissao || (carga.Veiculo != null && carga.Motoristas.Count > 0 && carga.TipoDeCarga != null && carga.ModeloVeicularCarga != null))
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;

                new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho).GerarOperacaoContainer(ref carga, TipoServicoMultisoftware, configuracaoEmbarcador, cargaPedidos, unidadeDeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Liberou a carga sem efetuar as integrações (dados de transporte).", unidadeDeTrabalho);

                repCarga.Atualizar(carga);

                unidadeDeTrabalho.CommitChanges();

                svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
