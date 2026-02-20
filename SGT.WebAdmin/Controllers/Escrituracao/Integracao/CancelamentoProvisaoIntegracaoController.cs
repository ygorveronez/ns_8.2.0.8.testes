using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "PesquisaHistorico", "DownloadArquivosHistoricoIntegracao" }, "Escrituracao/CancelamentoProvisao")]
    public class CancelamentoProvisaoIntegracaoController : BaseController
    {
		#region Construtores

		public CancelamentoProvisaoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);

                // Busca Parametros
                int.TryParse(Request.Params("CancelamentoProvisao"), out int codigoCancelamentoProvisao);

                // Busca Totalizadores
                int totalAguardandoIntegracao = repCancelamentoProvisaoIntegracao.ContarPorCancelamentoProvisao(codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalAguardandoRetorno = repCancelamentoProvisaoIntegracao.ContarPorCancelamentoProvisao(codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);
                int totalIntegrado = repCancelamentoProvisaoIntegracao.ContarPorCancelamentoProvisao(codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repCancelamentoProvisaoIntegracao.ContarPorCancelamentoProvisao(codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                // Formata retorno
                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao > 0 ? totalAguardandoIntegracao : totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                // Retorna valores
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> PesquisaHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Integracao"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracao = repCancelamentoProvisaoIntegracao.BuscarPorCodigo(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Integracao"), out int codigoIntegracao);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracao = repCancelamentoProvisaoIntegracao.BuscarPorCodigo(codigoIntegracao);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Provisão " + integracao.CancelamentoProvisao.Descricao + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download.");
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Busca Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracao = repCancelamentoProvisaoIntegracao.BuscarPorCodigo(codigo);

                // Valida
                if (integracao == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                // Altera status
                integracao.CancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.CancelamentoProvisao, null, "Reenviou a Integração.", unitOfWork);

                // Perciste dados
                repCancelamentoProvisaoIntegracao.Atualizar(integracao);
                repCancelamentoProvisao.Atualizar(integracao.CancelamentoProvisao);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Converte Parametros
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                    situacao = situacaoAux;

                int.TryParse(Request.Params("CancelamentoProvisao"), out int codigoCancelamentoProvisao);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = repCancelamentoProvisao.BuscarPorCodigo(codigoCancelamentoProvisao);

                // Itera integracoes
                foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracao in cancelamentoProvisao.Integracoes)
                {
                    // E altera status
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.CancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao;

                    repCancelamentoProvisao.Atualizar(integracao.CancelamentoProvisao);
                    repCancelamentoProvisaoIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.CancelamentoProvisao, null, "Reenviou oa integração.", unitOfWork);
                }

                // Perciste dados
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Download()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);

                // Converte Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracao = repCancelamentoProvisaoIntegracao.BuscarPorCodigo(codigo);

                // Valida dados
                if (integracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                // Gera arquvio EDI e nome
                byte[] bytes = Servicos.Embarcador.Integracao.IntegracaoCancelamentoProvisao.GerarGZIP(integracao, unitOfWork, out string nomeArquivo, true);
                if (bytes != null)
                {
                    repCancelamentoProvisaoIntegracao.Atualizar(integracao);
                    // Retorna arquivo gerado
                    return Arquivo(bytes, "application/x-gzip", nomeArquivo + ".gz");
                }
                else
                {
                    return new JsonpResult(true, false, "Não foi possível gerar o arquivo.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCancelamentoProvisao;
                int.TryParse(Request.Params("CancelamentoProvisao"), out codigoCancelamentoProvisao);

                Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao repCancelamentoProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repCancelamentoProvisaoEDIIntegracao.BuscarTipoIntegracaoPorCancelamentoProvisao(codigoCancelamentoProvisao);

                return new JsonpResult(new
                {
                    TiposIntegracoesCTe = tiposIntegracoesCTe,
                    TiposIntegracoesEDI = tiposIntegracoesEDI
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao))
                //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCancelamentoProvisao;
                int.TryParse(Request.Params("CancelamentoProvisao"), out codigoCancelamentoProvisao);

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
                //Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unidadeDeTrabalho);
                //Repositorio.Embarcador.Cargas.CargaLog repLog = new Repositorio.Embarcador.Cargas.CargaLog(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = repCancelamentoProvisao.BuscarPorCodigo(codigoCancelamentoProvisao);

                if (cancelamentoProvisao == null)
                    return new JsonpResult(true, false, "Escrituracao  não encontrada.");

                if (cancelamentoProvisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao)
                    return new JsonpResult(true, false, "A situação da Escrituracao não permite a finalização da etapa.");

                //if (cancelamentoProvisao.GerandoIntegracoes)
                //    return new JsonpResult(false, true, "O sistema ainda está gerando as integrações, não sendo possível finalizar a etapa. Aguarde alguns minutos e tente novamente.");

                unidadeDeTrabalho.Start();

                //Dominio.Entidades.Embarcador.Cargas.CargaLog log = new Dominio.Entidades.Embarcador.Cargas.CargaLog();

                //log.Acao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCarga.FinalizarEtapaIntegracao;
                //log.Carga = cancelamentoProvisao;
                //log.Data = DateTime.Now;
                //log.Usuario = Usuario;

                //repLog.Inserir(log);

                cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.Cancelado;


                repCancelamentoProvisao.Atualizar(cancelamentoProvisao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cancelamentoProvisao, null, "Confirmou o cancelamento da provisão.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                //svcHubCarga.InformarCargaAtualizada(cancelamentoProvisao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

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

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 25, Models.Grid.Align.left, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                situacao = situacaoAux;

            int.TryParse(Request.Params("CancelamentoProvisao"), out int codigoCancelamentoProvisao);

            // Consulta
            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> listaGrid = repCancelamentoProvisaoIntegracao.Consultar(codigoCancelamentoProvisao, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCancelamentoProvisaoIntegracao.ContarConsulta(codigoCancelamentoProvisao, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Situacao = obj.DescricaoSituacaoIntegracao,
                            TipoIntegracao = obj.TipoIntegracao.DescricaoTipo,
                            Retorno = obj.ProblemaIntegracao,
                            Tentativas = obj.NumeroTentativas,
                            DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                            DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                   obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                   Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                            DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "TipoIntegracao")
                propOrdena = "TipoIntegracao.Tipo";
            else if (propOrdena == "Tentativas")
                propOrdena = "NumeroTentativas";
            else if (propOrdena == "DataEnvio")
                propOrdena = "DataIntegracao";
            else if (propOrdena == "Situacao")
                propOrdena = "SituacaoIntegracao";
        }
    }
}
