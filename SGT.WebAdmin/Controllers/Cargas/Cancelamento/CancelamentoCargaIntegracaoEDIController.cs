using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cancelamento
{
    [CustomAuthorize(new string[] { "ObterTotais" }, "Cargas/CancelamentoCarga")]
    public class CancelamentoCargaIntegracaoEDIController : BaseController
    {
		#region Construtores

		public CancelamentoCargaIntegracaoEDIController(Conexao conexao) : base(conexao) { }

		#endregion

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Layout", "Layout", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Integracao, "TipoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Tentativas, "Tentativas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.DataEnvio, "DataEnvio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Retorno, "Retorno", 25, Models.Grid.Align.left, false);

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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                // Instancia Repositorios
                Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);

                // Busca Parametros
                int.TryParse(Request.Params("CancelamentoCarga"), out int codigoCancelamento);

                // Busca Totalizadores
                int totalAguardandoIntegracao = repCargaCancelamentoIntegracaoEDI.ContarPorCancelamento(codigoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repCargaCancelamentoIntegracaoEDI.ContarPorCancelamento(codigoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repCargaCancelamentoIntegracaoEDI.ContarPorCancelamento(codigoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

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

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoesEDI);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Busca Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI = repCargaCancelamentoIntegracaoEDI.BuscarPorCodigo(codigo);

                // Valida
                if (cargaCancelamentoIntegracaoEDI == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.IntegracaoNaoEncontrada);

                // Altera status
                if (cargaCancelamentoIntegracaoEDI.CargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                {
                    cargaCancelamentoIntegracaoEDI.CargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento;
                    repCargaCancelamento.Atualizar(cargaCancelamentoIntegracaoEDI.CargaCancelamento);
                }

                if (cargaCancelamentoIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada
                     && cargaCancelamentoIntegracaoEDI.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao)
                {
                    cargaCancelamentoIntegracaoEDI.IniciouConexaoExterna = false;
                    cargaCancelamentoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    repCargaCancelamentoIntegracaoEDI.Atualizar(cargaCancelamentoIntegracaoEDI);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamentoIntegracaoEDI, null, Localization.Resources.Cargas.CancelamentoCarga.IntegracaoEnviada, unitOfWork);
                }
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaEnviarIntegração);
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
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Converte Parametros
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                    situacao = situacaoAux;

                int.TryParse(Request.Params("CancelamentoCarga"), out int codigoCancelamento);

                // Busca no banco
                List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI> integracoes = repCargaCancelamentoIntegracaoEDI.BuscarPorCancelamento(codigoCancelamento, situacao);

                // Itera integracoes
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI integracao in integracoes)
                {
                    if (integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada
                        && integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao)
                    {
                        // E altera status
                        integracao.IniciouConexaoExterna = false;
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, Localization.Resources.Cargas.CancelamentoCarga.IntegracaoEnviada, unitOfWork);
                        repCargaCancelamentoIntegracaoEDI.Atualizar(integracao);
                    }
                }

                if (integracoes.Count() > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = integracoes.FirstOrDefault()?.CargaCancelamento;

                    if (cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    {
                        cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento;
                        repCargaCancelamento.Atualizar(cargaCancelamento);
                    }
                }

                // Persiste dados
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaEnviarIntegração);
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
                Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);

                // Converte Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI cargaCancelamentoIntegracaoEDI = repCargaCancelamentoIntegracaoEDI.BuscarPorCodigo(codigo);

                // Valida dados
                if (cargaCancelamentoIntegracaoEDI == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.IntegracaoNaoEncontrada);

                // Gera arquvio EDI e nome
                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(cargaCancelamentoIntegracaoEDI, unitOfWork);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(cargaCancelamentoIntegracaoEDI, unitOfWork);

                // Retorna arquivo gerado
                return Arquivo(edi, "plain/text", nomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaEnviarIntegração);
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
            Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                situacao = situacaoAux;

            int.TryParse(Request.Params("CargaCancelamento"), out int cancelamento);

            int usuario = this.Usuario.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI> listaGrid = repCargaCancelamentoIntegracaoEDI.Consultar(cancelamento, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCargaCancelamentoIntegracaoEDI.ContarConsulta(cancelamento, situacao);

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
                            DT_RowColor = CorIntegracao(obj.SituacaoIntegracao),
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

        private string CorIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            if (situacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde;
            else if (situacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul;
        }
    }
}
