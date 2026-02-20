using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Escrituracao.Fechamento
{
    [CustomAuthorize(new string[] { "ObterTotais", "PesquisaHistorico", "DownloadArquivosHistoricoIntegracao" }, "Fechamento/FechamentoFrete")]
    public class FechamentoFreteIntegracaoController : BaseController
    {
		#region Construtores

		public FechamentoFreteIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(unitOfWork);

                // Busca Parametros
                int.TryParse(Request.Params("FechamentoFrete"), out int codigo);

                // Busca Totalizadores
                int totalAguardandoIntegracao = repFechamentoFreteCTeIntegracao.ContarPorFechamento(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repFechamentoFreteCTeIntegracao.ContarPorFechamento(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repFechamentoFreteCTeIntegracao.ContarPorFechamento(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                // Formata retorno
                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao
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
                Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(unitOfWork);

                int.TryParse(Request.Params("Integracao"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao = repFechamentoFreteCTeIntegracao.BuscarPorCodigo(codigo);

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
                Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Integracao"), out int codigoIntegracao);

                // Busca no banco
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao = repFechamentoFreteCTeIntegracao.BuscarPorCodigo(codigoIntegracao);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Fechamento " + integracao.FechamentoFrete.Descricao + ".zip");
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
                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Busca Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao = repFechamentoFreteCTeIntegracao.BuscarPorCodigo(codigo);

                // Valida
                if (integracao == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                // Altera status
                integracao.FechamentoFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.AgIntegracao;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.FechamentoFrete, null, "Reenviou a Integração.", unitOfWork);

                // Perciste dados
                repFechamentoFreteCTeIntegracao.Atualizar(integracao);
                repFechamentoFrete.Atualizar(integracao.FechamentoFrete);
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
                Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(unitOfWork);
                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Converte Parametros
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                    situacao = situacaoAux;

                int.TryParse(Request.Params("FechamentoFrete"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento = repFechamentoFrete.BuscarPorCodigo(codigo);

                // Itera integracoes
                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao in fechamento.Integracoes)
                {
                    // E altera status
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.FechamentoFrete.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.AgIntegracao;

                    repFechamentoFrete.Atualizar(integracao.FechamentoFrete);
                    repFechamentoFreteCTeIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.FechamentoFrete, null, "Reenviou a integração.", unitOfWork);
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
                Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(unitOfWork);

                // Converte Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao = repFechamentoFreteCTeIntegracao.BuscarPorCodigo(codigo);

                // Valida dados
                if (integracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                // Gera arquvio EDI e nome
                byte[] bytes = null;
                string nomeArquivo = "";
                if (integracao.LayoutEDI != null)
                {
                    // Obtem EDI
                    bytes = new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork).GerarGZIPEDIFechamento(integracao, out nomeArquivo, incrementarSequencia: true);
                }
                else
                {
                    // Obtem XML
                    nomeArquivo = integracao.Complemento.CTe.Chave + ".xml.gz";
                    bytes = Servicos.Embarcador.Integracao.IntegracaoCTe.GerarGZIPXMLAutorizacao(integracao.Complemento.CTe, unitOfWork);
                }

                repFechamentoFreteCTeIntegracao.Atualizar(integracao);

                // Retorna arquivo gerado
                return Arquivo(bytes, "application/x-gzip", nomeArquivo);
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

        //public async Task<IActionResult> ObterDadosIntegracoes()
        //{
        //    Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        int.TryParse(Request.Params("FechamentoFrete"), out int codigo);

        //        Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unidadeDeTrabalho);

        //        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>();

        //        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repProvisaoEDIIntegracao.BuscarTipoIntegracaoPorProvisao(codigo);

        //        return new JsonpResult(new
        //        {
        //            TiposIntegracoesCTe = tiposIntegracoesCTe,
        //            TiposIntegracoesEDI = tiposIntegracoesEDI
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);

        //        return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
        //    }
        //    finally
        //    {
        //        unidadeDeTrabalho.Dispose();
        //    }
        //}


        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Referência", "Referencia", 10, Models.Grid.Align.left, false);
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
            Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                situacao = situacaoAux;

            int.TryParse(Request.Params("FechamentoFrete"), out int codigo);

            // Consulta
            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao> listaGrid = repFechamentoFreteCTeIntegracao.Consultar(codigo, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repFechamentoFreteCTeIntegracao.ContarConsulta(codigo, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Situacao = obj.DescricaoSituacaoIntegracao,
                            Referencia = obj.LayoutEDI?.DescricaoTipo ?? (obj.Complemento != null ? "CT-e " + obj.Complemento.CTe.Descricao : string.Empty),
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
