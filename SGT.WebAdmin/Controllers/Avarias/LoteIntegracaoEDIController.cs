using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/Lotes")]
    public class LoteIntegracaoEDIController : BaseController
    {
        #region Construtores

        public LoteIntegracaoEDIController(Conexao conexao) : base(conexao) { }

        #endregion

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Layout", "Layout", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 25, Models.Grid.Align.left, false);

            return grid;
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

        public async Task<IActionResult> PesquisaIntegracoesLoteAvaria()
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
                var lista = ExecutaPesquisaIntegracoesLoteAvaria(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        public async Task<IActionResult> ObterTotais(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Avarias.LoteEDIIntegracao repositorioLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(unitOfWork, cancellationToken);

                // Busca Parametros
                int codigoLote;
                int.TryParse(Request.Params("Lote"), out codigoLote);

                // Busca Totalizadores
                int totalAguardandoIntegracao = await repositorioLoteEDIIntegracao.ContarPorLoteAsync(codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = await repositorioLoteEDIIntegracao.ContarPorLoteAsync(codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = await repositorioLoteEDIIntegracao.ContarPorLoteAsync(codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

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
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.LoteEDIIntegracao repLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Busca Parametros
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao loteEDI = repLoteEDIIntegracao.BuscarPorCodigo(codigo);

                // Valida
                if (loteEDI == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                // Altera status
                loteEDI.Lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmIntegracao;
                loteEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                loteEDI.IniciouConexaoExterna = false;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteEDI.Lote, null, "Reenviou o EDI " + loteEDI.Descricao, unitOfWork);

                // Perciste dados
                repLoteEDIIntegracao.Atualizar(loteEDI);
                repLote.Atualizar(loteEDI.Lote);
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
                Repositorio.Embarcador.Avarias.LoteEDIIntegracao repLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(unitOfWork);
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Converte Parametros
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                int codigoLote = 0;
                int.TryParse(Request.Params("Lote"), out codigoLote);

                // Busca no banco
                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigoLote);
                List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> integracoes = repLoteEDIIntegracao.BuscarPorLote(codigoLote, situacao);

                // Itera integracoes
                foreach (Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao integracao in integracoes)
                {
                    // E altera status
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmIntegracao;
                    integracao.IniciouConexaoExterna = false;
                    repLote.Atualizar(integracao.Lote);
                    repLoteEDIIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Lote, null, "Reenviou o EDI " + integracao.Descricao, unitOfWork);
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
                Repositorio.Embarcador.Avarias.LoteEDIIntegracao repLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(unitOfWork);

                // Converte Parametros
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao loteEDI = repLoteEDIIntegracao.BuscarPorCodigo(codigo);

                // Valida dados
                if (loteEDI == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                // Gera arquvio EDI e nome
                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(loteEDI, unitOfWork);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(loteEDI, unitOfWork);

                // Retorna arquivo gerado
                return Arquivo(edi, "plain/text", nomeArquivo);
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

        /* ExecutaPesquisa 
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.LoteEDIIntegracao repLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
            if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                situacao = situacaoAux;

            int codigoLote;
            int.TryParse(Request.Params("Lote"), out codigoLote);

            int usuario = this.Usuario.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> listaGrid = repLoteEDIIntegracao.Consultar(codigoLote, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLoteEDIIntegracao.ContarConsulta(codigoLote, situacao);

            var lista = from obj in listaGrid
                        select
                        new
                        {
                            obj.Codigo,
                            Layout = obj.LayoutEDI.Descricao,
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

        private dynamic ExecutaPesquisaIntegracoesLoteAvaria(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.LoteEDIIntegracao repLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
            if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                situacao = situacaoAux;

            int codigoLote = Request.GetIntParam("Lote");

            int usuario = this.Usuario.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> listaGrid = codigoLote > 0 ? repLoteEDIIntegracao.Consultar(codigoLote, situacao, propOrdenar, dirOrdena, inicio, limite) : new List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();
            totalRegistros = codigoLote > 0 ? repLoteEDIIntegracao.ContarConsulta(codigoLote, situacao) : 0;

            var lista = from obj in listaGrid
                        select
                        new
                        {
                            obj.Codigo,
                            Layout = obj.LayoutEDI.Descricao,
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
        }
    }
}
