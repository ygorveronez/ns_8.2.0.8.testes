using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "Escrituracao/Provisao")]
    public class ProvisaoIntegracaoEDIController : BaseController
    {
		#region Construtores

		public ProvisaoIntegracaoEDIController(Conexao conexao) : base(conexao) { }

		#endregion

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
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

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);

                // Busca Parametros
                int.TryParse(Request.Params("Provisao"), out int codigoProvisao);

                // Busca Totalizadores
                int totalAguardandoIntegracao = repProvisaoEDIIntegracao.ContarPorProvisaoESituacao(codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repProvisaoEDIIntegracao.ContarPorProvisaoESituacao(codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repProvisaoEDIIntegracao.ContarPorProvisaoESituacao(codigoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

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

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/Provisao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                // Instancia Repositorios
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Busca Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao provisaoEDI = repProvisaoEDIIntegracao.BuscarPorCodigo(codigo);

                // Valida
                if (provisaoEDI == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                // Altera status
                provisaoEDI.Provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.AgIntegracao;
                provisaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                provisaoEDI.IniciouConexaoExterna = false;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, provisaoEDI.Provisao, null, "Reenviou o EDI " + provisaoEDI.Descricao, unitOfWork);

                // Perciste dados
                repProvisaoEDIIntegracao.Atualizar(provisaoEDI);
                repProvisao.Atualizar(provisaoEDI.Provisao);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/Provisao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                // Instancia Repositorios
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);

                // Inicia instancia
                unitOfWork.Start();

                // Converte Parametros
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                    situacao = situacaoAux;

                int.TryParse(Request.Params("Provisao"), out int codigoProvisao);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.Provisao lote = repProvisao.BuscarPorCodigo(codigoProvisao);
                List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao> integracoes = repProvisaoEDIIntegracao.BuscarPorProvisao(codigoProvisao, situacao);

                // Itera integracoes
                foreach (Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao integracao in integracoes)
                {
                    // E altera status
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.AgIntegracao;
                    integracao.IniciouConexaoExterna = false;
                    repProvisao.Atualizar(integracao.Provisao);
                    repProvisaoEDIIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.Provisao, null, "Reenviou o EDI " + integracao.Descricao, unitOfWork);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/Provisao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_DownloadArquivoIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                // Instancia Repositorios
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);

                // Converte Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca no banco
                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao provisaoEDI = repProvisaoEDIIntegracao.BuscarPorCodigo(codigo);
                

                // Valida dados
                if (provisaoEDI == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                // Gera arquvio EDI e nome
                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(provisaoEDI, unitOfWork);
                if(edi != null)
                {
                    string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(provisaoEDI, true, unitOfWork);

                    provisaoEDI.SequenciaIntegracao++;
                    repProvisaoEDIIntegracao.Atualizar(provisaoEDI);

                    // Retorna arquivo gerado
                    return Arquivo(edi, "plain/text", nomeArquivo);
                }
                else
                {
                    return new JsonpResult(false, false, "Não foi possível gerar o arquivo.");
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

        /* ExecutaPesquisa 
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux))
                situacao = situacaoAux;

            int.TryParse(Request.Params("Provisao"), out int codigoProvisao);

            int usuario = this.Usuario.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao> listaGrid = repProvisaoEDIIntegracao.Consultar(codigoProvisao, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProvisaoEDIIntegracao.ContarConsulta(codigoProvisao, situacao);

            var lista = from obj in listaGrid
                        select new
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
