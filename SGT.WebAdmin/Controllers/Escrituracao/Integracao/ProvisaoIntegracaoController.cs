using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "PesquisaHistorico", "DownloadArquivosHistoricoIntegracao" }, "Escrituracao/Provisao")]
    public class ProvisaoIntegracaoController : BaseController
    {
        #region Construtores

        public ProvisaoIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Provisao"), out int codigoProvisao);

                int totalAguardandoIntegracao = repProvisaoIntegracao.ContarPorProvisaoESituacao(codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repProvisaoIntegracao.ContarPorProvisaoESituacao(codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repProvisaoIntegracao.ContarPorProvisaoESituacao(codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

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
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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
                Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Integracao"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao integracao = repProvisaoIntegracao.BuscarPorCodigo(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count);

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
                Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Integracao"), out int codigoIntegracao);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao integracao = repProvisaoIntegracao.BuscarPorCodigo(codigoIntegracao);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.FirstOrDefault(o => o.Codigo == codigo);

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Provisão " + integracao.Provisao.Descricao + ".zip");
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
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);

                unitOfWork.Start();

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao integracao = repProvisaoIntegracao.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                ValidaReenvioIntegracao(integracao.Provisao, unitOfWork);

                integracao.Provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.AgIntegracao;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Provisao, null, "Reenviou a Integração.", unitOfWork);

                repProvisaoIntegracao.Atualizar(integracao);
                repProvisao.Atualizar(integracao.Provisao);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
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
                Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);

                unitOfWork.Start();

                int.TryParse(Request.Params("Provisao"), out int codigoProvisao);

                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigoProvisao);

                ValidaReenvioIntegracao(provisao, unitOfWork);

                Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao> integracoes = repositorioProvisaoIntegracao.BuscarPorProvisao(provisao.Codigo);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao integracao in integracoes)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.AgIntegracao;

                    repProvisao.Atualizar(integracao.Provisao);
                    repProvisaoIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Provisao, null, "Reenviou oa integração.", unitOfWork);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao integracaoProvisao = repositorioProvisaoIntegracao.BuscarPorCodigo(codigo);

                if (integracaoProvisao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao provisaoEDIIntegracao = repositorioProvisaoEDIIntegracao.BuscarUltimoPorProvisao(integracaoProvisao.Provisao.Codigo);

                if (provisaoEDIIntegracao == null)
                    return new JsonpResult(false, "Não foi possível gerar o arquivo (Não foram provisionados CTes)");

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(provisaoEDIIntegracao, unitOfWork);

                if (edi == null)
                    return new JsonpResult(false, "Não foi possível gerar o arquivo");

                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(provisaoEDIIntegracao, incrementarSequencia: true, unitOfWork);

                return Arquivo(edi.ToArray(), "application/x-gzip", nomeArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                int codigoProvisao;
                int.TryParse(Request.Params("Provisao"), out codigoProvisao);

                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repProvisaoEDIIntegracao.BuscarTipoIntegracaoPorProvisao(codigoProvisao);

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
                int codigoProvisao;
                int.TryParse(Request.Params("Provisao"), out codigoProvisao);

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigoProvisao);

                if (provisao == null)
                    return new JsonpResult(true, false, "Escrituracao  não encontrada.");

                if (provisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.AgIntegracao)
                    return new JsonpResult(true, false, "A situação da Escrituracao não permite a finalização da etapa.");

                unidadeDeTrabalho.Start();

                provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.Finalizado;


                repProvisao.Atualizar(provisao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, provisao, null, "Finalizou.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

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
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                situacao = situacaoAux;

            int.TryParse(Request.Params("Provisao"), out int codigoProvisao);

            List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao> listaGrid = repProvisaoIntegracao.Consultar(codigoProvisao, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProvisaoIntegracao.ContarConsulta(codigoProvisao, situacao);

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

        private void ValidaReenvioIntegracao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            if (!configuracaoFinanceiro.BloqueioEnvioIntegracoesCargasAnuladaseCanceladas)
                return;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = provisao?.Carga;

            if (carga == null)
                throw new ServicoException("A carga foi anulada ou cancelada e sua integração não pode ser reenviada.");

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesNaoPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
            };

            if (situacoesNaoPermitidas.Contains(carga.SituacaoCarga))
                throw new ServicoException("A carga foi anulada ou cancelada e sua integração não pode ser reenviada.");
        }
    }
}
