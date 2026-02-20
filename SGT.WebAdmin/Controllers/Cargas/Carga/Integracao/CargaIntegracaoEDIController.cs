using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoEDIController : BaseController
    {
        #region Construtores

        public CargaIntegracaoEDIController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");

                int codigoCarga = Request.GetIntParam("Carga");

                bool integracaoFilialEmissora = Request.GetBoolParam("FilialEmissora");
                bool modificarTimelineIntegracaoCarga = Request.GetBoolParam("ModificarTimelineIntegracaoCarga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);

                grid.AdicionarCabecalho("Layout", "Layout", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                if (modificarTimelineIntegracaoCarga)
                    grid.AdicionarCabecalho("E-mail", "Email", 20, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Retorno", "Retorno", 22, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Remetente", "Remetente", 12, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unidadeDeTrabalho);

                int countEDIs = await repositorioIntegracao.ContarConsultaAsync(codigoCarga, situacao, integracaoFilialEmissora);

                List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao>();

                if (countEDIs > 0)
                {
                    if (propOrdena == "Layout")
                        propOrdena = "LayoutEDI.Descricao";
                    else if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";
                    else if (propOrdena == "Remetente")
                        propOrdena = "Remetente.Nome";

                    listaIntegracao = await repositorioIntegracao.ConsultarAsync(codigoCarga, situacao, integracaoFilialEmissora, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                }

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.TipoIntegracao.Tipo,
                                       Remetente = obj.Remetente?.Descricao ?? "Todos",
                                       Layout = obj.LayoutEDI.Descricao,
                                       obj.SituacaoIntegracao,
                                       Situacao = obj.DescricaoSituacaoIntegracao,
                                       TipoIntegracao = obj.TipoIntegracao.DescricaoTipo,
                                       Retorno = obj.ProblemaIntegracao,
                                       Tentativas = obj.NumeroTentativas,
                                       DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                       Email = obj.DestinoEnvio,
                                       DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                                       DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                                   }).ToList());

                grid.setarQuantidadeTotal(countEDIs);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterTotais(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);


                bool integracaoFilialEmissora = false;
                bool.TryParse(Request.Params("FilialEmissora"), out integracaoFilialEmissora);

                Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unidadeDeTrabalho, cancellationToken);

                int totalAguardandoIntegracao = await repositorioCargaEDIIntegracao.ContarPorCargaAsync(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, integracaoFilialEmissora);
                int totalIntegrado = await repositorioCargaEDIIntegracao.ContarPorCargaAsync(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, integracaoFilialEmissora);
                int totalProblemaIntegracao = await repositorioCargaEDIIntegracao.ContarPorCargaAsync(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, integracaoFilialEmissora);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de EDI.");
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> Reenviar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = await repCargaEDIIntegracao.BuscarPorCodigoAsync(codigo, false);

                if (cargaEDIIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes) && cargaEDIIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                await unitOfWork.StartAsync();

                cargaEDIIntegracao.IniciouConexaoExterna = false;
                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                await repCargaEDIIntegracao.AtualizarAsync(cargaEDIIntegracao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEDIIntegracao, null, "Reenviou Integração.", unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEDIIntegracao.Carga;
                carga.PossuiPendencia = false;
                await repositorioCarga.AtualizarAsync(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Reenviou Integração EDI da Carga.", unitOfWork);

                await unitOfWork.CommitChangesAsync();

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

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
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                int codigoCarga = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                bool integracaoFilialEmissora = false;
                bool.TryParse(Request.Params("FilialEmissora"), out integracaoFilialEmissora);

                Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao> integracoes = await repositorioCargaEDIIntegracao.BuscarPorCargaAsync(codigoCarga, situacao, integracaoFilialEmissora);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao integracao in integracoes)
                {
                    if (integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao)
                    {
                        integracao.IniciouConexaoExterna = false;
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                        await repositorioCargaEDIIntegracao.AtualizarAsync(integracao);
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, integracao, null, "Reenviou Integração.", unitOfWork);
                    }
                }

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);
                carga.PossuiPendencia = false;
                await repositorioCarga.AtualizarAsync(carga);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, carga, null, "Reenviou Integração EDI da Carga.", unitOfWork);

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

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

        public async Task<IActionResult> Download(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_DownloadArquivoIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao = await repCargaEDIIntegracao.BuscarPorCodigoAsync(codigo, false);

                if (cargaEDIIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string extensao = string.Empty;

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(cargaEDIIntegracao, TipoServicoMultisoftware, unitOfWork, _conexao.StringConexao, out extensao);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(cargaEDIIntegracao, extensao, unitOfWork);

                if ((cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL ||
                     cargaEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.UVT_RN) &&
                    cargaEDIIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                    cargaEDIIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    await unitOfWork.StartAsync();

                    cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    await repCargaEDIIntegracao.AtualizarAsync(cargaEDIIntegracao);


                    Servicos.Embarcador.Integracao.IntegracaoCarga servicoIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork, TipoServicoMultisoftware, cancellationToken);

                    await servicoIntegracaoCarga.AtualizarSituacaoCargaIntegracaoAsync(cargaEDIIntegracao.Carga.Codigo);

                    await unitOfWork.CommitChangesAsync();
                }

                return Arquivo(edi, extensao == ".zip" ? "application/zip" : "plain/text", nomeArquivo);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion
    }
}
