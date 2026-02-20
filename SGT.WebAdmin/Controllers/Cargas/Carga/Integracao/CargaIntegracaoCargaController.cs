using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "DownloadLoteDocumentos" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoCargaController : BaseController
    {
        #region Construtores

        public CargaIntegracaoCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                bool integracaoTransportador = Request.GetBoolParam("IntegracaoTransportador");
                bool integracaoFilialEmissora = Request.GetBoolParam("IntegracaoFilialEmissora", false);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Protocolo", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Retorno", 18, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho, integracaoTransportador);

                int countCTes = repIntegracao.ContarConsulta(codigoCarga, situacao, tipo, integracaoFilialEmissora);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

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

                    listaIntegracao = repIntegracao.Consultar(codigoCarga, situacao, tipo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, integracaoFilialEmissora);
                }

                grid.setarQuantidadeTotal(countCTes);

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.TipoIntegracao.Tipo,
                                       obj.SituacaoIntegracao,
                                       Situacao = obj.SituacaoIntegracao.ObterDescricao(),
                                       TipoIntegracao = obj.TipoIntegracao.Tipo.ObterDescricao() + (obj.IntegracaoColeta ? " (Coleta)" : string.Empty),
                                       Retorno = obj.ProblemaIntegracao,
                                       Protocolo = !string.IsNullOrWhiteSpace(obj.Protocolo) ? obj.Protocolo : "",
                                       Tentativas = obj.NumeroTentativas,
                                       DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                       DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo :
                                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                                       DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                                   }).ToList());

                return new JsonpResult(grid);
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

        public async Task<IActionResult> ObterComprovanteAgendamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (cargaIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma retornoRotograma = Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.ObterComprovanteAgendamento(cargaIntegracao.Carga, TipoServicoMultisoftware, unidadeDeTrabalho, _conexao.StringConexao);
                    return new JsonpResult(retornoRotograma);
                }
                else
                {
                    return new JsonpResult(false, "O tipo de integração não possui nenhum link de acesso externo habilitado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o comprovante de agendamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterRotogramaIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (cargaIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Rotograma.RetornoRotograma retornoRotograma = Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.ObterRotograma(cargaIntegracao.Carga, TipoServicoMultisoftware, unidadeDeTrabalho, _conexao.StringConexao);
                    return new JsonpResult(retornoRotograma);
                }
                else
                {
                    return new JsonpResult(false, "O tipo de integração não possui nenhum link de acesso externo habilitado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o Rotograma.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BaixarRotograma()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (cargaIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk retornoRotograma = Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.ObterRotogramaBrk(cargaIntegracao, TipoServicoMultisoftware, unidadeDeTrabalho);

                    if (retornoRotograma == null || string.IsNullOrEmpty(retornoRotograma.Base64))
                        return new JsonpResult(false, "Arquivo não encontrado.");

                    byte[] arquivoBytes = Convert.FromBase64String(retornoRotograma.Base64);

                    if (arquivoBytes == null || arquivoBytes.Length == 0)
                    {
                        return new JsonpResult(false, "Arquivo inválido ou vazio.");
                    }

                    string contentType = "application/pdf";
                    string nomeArquivo = $"Rotograma_{codigo}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                    return Arquivo(arquivoBytes, contentType, nomeArquivo);
                }
                else
                {
                    return new JsonpResult(false, "Tipo de integração não suportado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o Rotograma.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                bool integracaoTransportador = Request.GetBoolParam("IntegracaoTransportador");

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho, integracaoTransportador);

                int totalAguardandoIntegracao = repIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, false);
                int totalIntegrado = repIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, false);
                int totalProblemaIntegracao = repIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, false);
                int totalAguardandoRetorno = repIntegracao.ContarPorCarga(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno, false);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
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
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);
                bool integracaoTransportador = Request.GetBoolParam("IntegracaoTransportador");

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho, integracaoTransportador);

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (cargaIntegracao.IntegracaoColeta && cargaIntegracao.TipoIntegracao.Tipo == TipoIntegracao.OpenTech)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoSM = repIntegracao.BuscarPorCargaTipoIntegracaoColetaSituacao(cargaIntegracao.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                    if (cargaIntegracaoSM != null)
                        return new JsonpResult(false, "Já existe uma integração de SM gerada, não é possível reenviar a de Coleta.");
                }

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes) && cargaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                    return new JsonpResult(false, true, "Não é possível Reenviar a Integração enquanto a Situação da integração estiver Aguardando Integração.");


                if (cargaIntegracao.TipoIntegracao.Tipo == TipoIntegracao.Trizy && cargaIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    return new JsonpResult(false, true, "Não é possível Reenviar a Integração Trizy quando a Situação da integração estiver Integrado.");

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaIntegracao, null, "Reenviou Integração.", unidadeDeTrabalho);
                repIntegracao.Atualizar(cargaIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AnteciparEnvio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool integracaoTransportador = Request.GetBoolParam("IntegracaoTransportador");

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork, integracaoTransportador);
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes) && cargaIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaIntegracao, null, "Enviou a Integração antecipadamente .", unitOfWork);

                switch (cargaIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FrimesaFrete:
                        new Servicos.Embarcador.Integracao.Frimesa.IntegracaoFrimesa(unitOfWork).IntegrarFrete(cargaIntegracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FrimesaValePedagio:
                        new Servicos.Embarcador.Integracao.Frimesa.IntegracaoFrimesa(unitOfWork).IntegrarValePedagio(cargaIntegracao);
                        break;
                    default:
                        cargaIntegracao.ProblemaIntegracao = "Integração não configurada para antecipar";
                        cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.NumeroTentativas++;
                        cargaIntegracao.DataIntegracao = DateTime.Now;
                        repIntegracao.Atualizar(cargaIntegracao);
                        break;
                }

                repIntegracao.Atualizar(cargaIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoCarga = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                bool integracaoTransportador = Request.GetBoolParam("IntegracaoTransportador");

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho, integracaoTransportador);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> integracoes = repCargaIntegracao.BuscarPorCarga(codigoCarga, situacao, tipo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao in integracoes)
                {
                    if (integracao.TipoIntegracao.Tipo == TipoIntegracao.Trizy && integracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                        return new JsonpResult(false, true, "Não é possível Reenviar a Integração Trizy quando a Situação da integração estiver Integrado.");

                    if (integracao.TipoIntegracao.Tipo != TipoIntegracao.OpenTech || integracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                    {
                        if (integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                            return new JsonpResult(false, true, "Não é possível Reenviar a Integração enquanto a Situação da integração estiver Aguardando Integração.");

                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                        repCargaIntegracao.Atualizar(integracao);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou Integração.", unidadeDeTrabalho);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Reenviou as integrações da Carga", unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).ThenByDescending(o => o.Codigo).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
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
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

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
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteDocumentos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, false, false);

                List<int> mdfes = repCargaMDFe.BuscarCodigosMDFePorAutorizadosCarga(codigoCarga);

                List<int> valePedagios = repCargaValePedagio.BuscarCodigosValePedagiosSemPararPorCarga(codigoCarga);

                if (ctes.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados documentos autorizados para esta carga.");

                if (ctes.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLotePDFsTMS(codigoCarga, ctes, mdfes, valePedagios, unidadeTrabalho, TipoServicoMultisoftware, Cliente.NomeFantasia);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_Documentos.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de documentos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadMicDta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);
                Servicos.Embarcador.Carga.MICDTA serMICDTA = new Servicos.Embarcador.Carga.MICDTA(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                if (cargaIntegracao.TipoIntegracao.Tipo != TipoIntegracao.MicDta)
                    return new JsonpResult(false, true, "Arquivo exclusivo para MIC/DTA.");

                return Arquivo(serMICDTA.ObterPdfMicDta(cargaIntegracao.Carga, unidadeDeTrabalho, null, cargaIntegracao), "application/pdf", $"MIC/DTA {cargaIntegracao.Carga.CodigoCargaEmbarcador}.pdf");
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o MIC/DTA.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

    }
}
