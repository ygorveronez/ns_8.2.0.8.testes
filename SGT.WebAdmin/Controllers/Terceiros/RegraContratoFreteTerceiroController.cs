using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;


namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize("Terceiros/RegraContratoFreteTerceiro")]
    public class RegraContratoFreteTerceiroController : BaseController
    {
		#region Construtores

		public RegraContratoFreteTerceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region ObjetosJson
        private class ObjetoEntidade
        {
            public dynamic Codigo { get; set; } // dynamic pois o codigo pode ser também um cpf/cnpj
            public string Descricao { get; set; }
        }
        private class ObjetoAprovadores
        {
            public int Codigo { get; set; }
            public string Nome { get; set; }
        }
        private class RegrasPorTipo
        {
            public dynamic Codigo { get; set; }
            public int Ordem { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao Juncao { get; set; }
            public ObjetoEntidade Entidade { get; set; }
            public dynamic Valor { get; set; }
        }
        #endregion

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);

            return grid;
        }

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                // Retorna Dados
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


        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro repRegraContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato repAlcadaContratoFreteTerceiroValorContrato = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo repAlcadaContratoFreteTerceiroValorAcrescimo = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto repAlcadaContratoFreteTerceiroValorDesconto = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros repAlcadaContratoFreteTerceiroTerceiros = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros(unitOfWork);
                // Nova entidade
                Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro = new Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro();
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato> alcadaValorContrato = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato>();
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo> alcadaValorAcrescimo = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo>();
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto> alcadaValorDesconto = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto>();
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros> alcadaTerceiros = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros>();

                // Preenche a entidade
                PreencherEntidade(ref regraContratoFreteTerceiro, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regraContratoFreteTerceiro, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regraContratoFreteTerceiro, ref alcadaValorContrato, ref alcadaValorAcrescimo, ref alcadaValorDesconto, ref alcadaTerceiros, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegraContratoFreteTerceiro.Inserir(regraContratoFreteTerceiro, Auditado);

                // Insere regras
                for (var i = 0; i < alcadaValorContrato.Count; i++) repAlcadaContratoFreteTerceiroValorContrato.Inserir(alcadaValorContrato[i], Auditado);
                for (var i = 0; i < alcadaValorAcrescimo.Count; i++) repAlcadaContratoFreteTerceiroValorAcrescimo.Inserir(alcadaValorAcrescimo[i], Auditado);
                for (var i = 0; i < alcadaValorDesconto.Count; i++) repAlcadaContratoFreteTerceiroValorDesconto.Inserir(alcadaValorDesconto[i], Auditado);
                for (var i = 0; i < alcadaTerceiros.Count; i++) repAlcadaContratoFreteTerceiroTerceiros.Inserir(alcadaTerceiros[i], Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro repRegraContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo repAlcadaContratoFreteTerceiroValorAcrescimo = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto repAlcadaContratoFreteTerceiroValorDesconto = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato repAlcadaContratoFreteTerceiroValorContrato = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros repAlcadaContratoFreteTerceiroTerceiros = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro = repRegraContratoFreteTerceiro.BuscarPorCodigo(codigoRegra, true);

                if (regraContratoFreteTerceiro == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato> alcadaValorContrato = repAlcadaContratoFreteTerceiroValorContrato.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo> alcadaValorAcrescimo = repAlcadaContratoFreteTerceiroValorAcrescimo.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto> alcadaValorDesconto = repAlcadaContratoFreteTerceiroValorDesconto.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros> alcadaTerceiros = repAlcadaContratoFreteTerceiroTerceiros.BuscarPorRegra(codigoRegra);
                
                #endregion

                // Preenche a entidade
                PreencherEntidade(ref regraContratoFreteTerceiro, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regraContratoFreteTerceiro, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regraContratoFreteTerceiro, ref alcadaValorContrato, ref alcadaValorAcrescimo, ref alcadaValorDesconto, ref alcadaTerceiros, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                #region Insere Regras

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                SalvarAlteracaoCriterioDaRegra(regraContratoFreteTerceiro, alcadaValorContrato, repAlcadaContratoFreteTerceiroValorContrato, "Valor do Contrato", ref alteracoes, unitOfWork);
                SalvarAlteracaoCriterioDaRegra(regraContratoFreteTerceiro, alcadaValorAcrescimo, repAlcadaContratoFreteTerceiroValorAcrescimo, "Valor do Acréscimo", ref alteracoes, unitOfWork);
                SalvarAlteracaoCriterioDaRegra(regraContratoFreteTerceiro, alcadaValorDesconto, repAlcadaContratoFreteTerceiroValorDesconto, "Valor do Desconto", ref alteracoes, unitOfWork);
                SalvarAlteracaoCriterioDaRegra(regraContratoFreteTerceiro, alcadaTerceiros, repAlcadaContratoFreteTerceiroTerceiros, "Filia", ref alteracoes, unitOfWork);

                repRegraContratoFreteTerceiro.Atualizar(regraContratoFreteTerceiro, Auditado);

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regraContratoFreteTerceiro, alteracoes, "Alterou os critérios da regra.", unitOfWork);

                #endregion

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro repRegraContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato repAlcadaContratoFreteTerceiroValorContrato = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo repAlcadaContratoFreteTerceiroValorAcrescimo = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto repAlcadaContratoFreteTerceiroValorDesconto = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto(unitOfWork);
                Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros repAlcadaContratoFreteTerceiroTerceiros = new Repositorio.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro = repRegraContratoFreteTerceiro.BuscarPorCodigo(codigoRegra);

                if (regraContratoFreteTerceiro == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato> alcadaValorContrato = repAlcadaContratoFreteTerceiroValorContrato.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo> alcadaValorAcrescimo = repAlcadaContratoFreteTerceiroValorAcrescimo.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto> alcadaValorDesconto = repAlcadaContratoFreteTerceiroValorDesconto.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros> alcadaTerceiros = repAlcadaContratoFreteTerceiroTerceiros.BuscarPorRegra(codigoRegra);
                
                #endregion
                
                var dynRegra = new
                {
                    regraContratoFreteTerceiro.Codigo,
                    regraContratoFreteTerceiro.NumeroAprovadores,
                    Vigencia = regraContratoFreteTerceiro.Vigencia.HasValue ? regraContratoFreteTerceiro.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regraContratoFreteTerceiro.Descricao) ? regraContratoFreteTerceiro.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regraContratoFreteTerceiro.Observacoes) ? regraContratoFreteTerceiro.Observacoes : string.Empty,

                    Aprovadores = (from o in regraContratoFreteTerceiro.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorValorContrato = regraContratoFreteTerceiro.RegraPorValorContrato,
                    ValorContrato = (from obj in alcadaValorContrato select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato>(obj, true)).ToList(),

                    UsarRegraPorValorAcrescimo = regraContratoFreteTerceiro.RegraPorValorAcrescimo,
                    ValorAcrescimo = (from obj in alcadaValorAcrescimo select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo>(obj, true)).ToList(),

                    UsarRegraPorValorDesconto = regraContratoFreteTerceiro.RegraPorValorDesconto,
                    ValorDesconto = (from obj in alcadaValorDesconto select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto>(obj, true)).ToList(),

                    UsarRegraPorTerceiro = regraContratoFreteTerceiro.RegraPorTerceiros,
                    Terceiro = (from obj in alcadaTerceiros select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros>(obj)).ToList()
                };

                return new JsonpResult(dynRegra);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro repRegraContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro = repRegraContratoFreteTerceiro.BuscarPorCodigo(codigo);

                if (regraContratoFreteTerceiro == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regraContratoFreteTerceiro.Aprovadores.Clear();
                regraContratoFreteTerceiro.AlcadasValorContrato.Clear();
                regraContratoFreteTerceiro.AlcadasValorAcrescimo.Clear();
                regraContratoFreteTerceiro.AlcadasValorDesconto.Clear();
                regraContratoFreteTerceiro.AlcadasTerceiros.Clear();

                repRegraContratoFreteTerceiro.Deletar(regraContratoFreteTerceiro);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Já existem informações vinculadas à regra.");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion


        #region Métodos Privados
        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            // Converte parametros
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            int.TryParse(Request.Params("NumeroAprovadores"), out int numeroAprovadores);

            bool.TryParse(Request.Params("UsarRegraPorValorContrato"), out bool usarRegraPorValorContrato);
            bool.TryParse(Request.Params("UsarRegraPorValorAcrescimo"), out bool usarRegraPorValorAcrescimo);
            bool.TryParse(Request.Params("UsarRegraPorValorDesconto"), out bool usarRegraPorValorDesconto);
            bool.TryParse(Request.Params("UsarRegraPorTerceiro"), out bool usarRegraPorTerceiros);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regraContratoFreteTerceiro.Descricao = descricao;
            regraContratoFreteTerceiro.Observacoes = observacao;
            regraContratoFreteTerceiro.Vigencia = dataVigencia;
            regraContratoFreteTerceiro.NumeroAprovadores = numeroAprovadores;
            regraContratoFreteTerceiro.Aprovadores = listaAprovadores;

            regraContratoFreteTerceiro.RegraPorValorContrato = usarRegraPorValorContrato;
            regraContratoFreteTerceiro.RegraPorValorAcrescimo = usarRegraPorValorAcrescimo;
            regraContratoFreteTerceiro.RegraPorValorDesconto = usarRegraPorValorDesconto;
            regraContratoFreteTerceiro.RegraPorTerceiros = usarRegraPorTerceiros;
        }

        private void PreencherEntidadeRegra<T>(string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro, Func<dynamic, object> lambda = null) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
        {
            /* Descricao
             * RegrasAutorizacao é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
             * 
             * Esse método facilita a instancia de novas regras, já que todas possuem o mesmo padra
             * - Regra (Entidade Pai)
             * - Ordem
             * - Codicao
             * - Juncao
             * - TIPO
             * 
             * Esse último, é instanciado com o retorno do callback, já que é o único parametro que é modificado
             * Mas quando não for uma enteidade, mas um valor simples, basta usar a flag usarDynamic = true,
             * Fazendo isso é setado o valor que vem no RegrasPorTipo.Valor
             */

            // Converte json (com o parametro get)
            List<RegrasPorTipo> dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegrasPorTipo>>(Request.Params(parametroJson));

            if (dynRegras == null)
                throw new Exception("Erro ao converter os dados recebidos.");

            // Variavel auxiliar
            PropertyInfo prop;

            // Itera retornos
            for (var i = 0; i < dynRegras.Count(); i++)
            {
                int.TryParse(dynRegras[i].Codigo.ToString(), out int codigoRegra);
                int indexRegraNaLista = -1;

                // Instancia o objeto T (T não possui construor new)
                T regra = default(T);
                if (codigoRegra > 0)
                {
                    for (int j = 0; j < regrasPorTipo.Count; j++)
                        if ((int)((dynamic)regrasPorTipo[j]).Codigo == codigoRegra)
                        {
                            indexRegraNaLista = j;
                            break;
                        }
                }

                if (indexRegraNaLista >= 0)
                {
                    regra = regrasPorTipo[indexRegraNaLista];
                    regra.Initialize();
                }
                else
                    regra = Activator.CreateInstance<T>();

                // Seta as propriedas da entidade
                prop = regra.GetType().GetProperty("RegraContratoFreteTerceiro", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regraContratoFreteTerceiro, null);

                regra.Ordem = dynRegras[i].Ordem;
                regra.Condicao = dynRegras[i].Condicao;
                regra.Juncao = dynRegras[i].Juncao;


                if (!usarDynamic)
                {
                    // Executa lambda
                    var result = dynRegras[i].Entidade != null ? lambda(dynRegras[i].Entidade.Codigo) : null;

                    prop = regra.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(regra, result, null);
                }
                else
                {
                    prop = regra.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                    if (prop.PropertyType.Name.Equals("Decimal"))
                    {
                        decimal.TryParse(dynRegras[i].Valor.ToString(), out decimal valorDecimal);

                        prop.SetValue(regra, valorDecimal, null);
                    }
                    else
                    {
                        prop.SetValue(regra, dynRegras[i].Valor, null);
                    }
                }

                // Adiciona lista de retorno
                if (indexRegraNaLista >= 0)
                    regrasPorTipo[indexRegraNaLista] = regra;
                else
                    regrasPorTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regraContratoFreteTerceiro.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regraContratoFreteTerceiro.Aprovadores.Count() < regraContratoFreteTerceiro.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regraContratoFreteTerceiro.NumeroAprovadores.ToString());

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, List<T> regrasPorTipo, out List<string> erros)
        {
            erros = new List<string>();

            if (regrasPorTipo.Count() == 0)
                erros.Add("Nenhuma regra " + nomeRegra + " cadastrada.");
            else
            {
                // Variavel auxiliar
                PropertyInfo prop;

                // Itera validacao
                for (var i = 0; i < regrasPorTipo.Count(); i++)
                {
                    var regra = regrasPorTipo[i];
                    prop = regra.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);

                    if (prop.GetValue(regra) == null)
                        erros.Add(nomeRegra + " da regra é obrigatório.");
                }
            }

            return erros.Count() == 0;
        }

        private RegrasPorTipo RetornaRegraPorTipoDyn<T>(T obj, bool usarValor = false) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
        {
            // Variavel auxiliar
            PropertyInfo prop;

            prop = obj.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
            int.TryParse(prop.GetValue(obj).ToString(), out int codigo);

            ObjetoEntidade objetoEntidade = new ObjetoEntidade();
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                dynamic codigoEntidade = prop.GetValue(entidade);

                prop = entidade.GetType().GetProperty("Descricao", BindingFlags.Public | BindingFlags.Instance);
                string descricaoEntidade = prop.GetValue(entidade);

                objetoEntidade.Codigo = codigoEntidade;
                objetoEntidade.Descricao = descricaoEntidade;
            }
            else
            {
                prop = obj.GetType().GetProperty("Descricao", BindingFlags.Public | BindingFlags.Instance);
                valor = prop.GetValue(obj);
            }

            RegrasPorTipo restorno = new RegrasPorTipo()
            {
                Codigo = codigo,
                Ordem = obj.Ordem,
                Juncao = obj.Juncao,
                Condicao = obj.Condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return restorno;
        }

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro, ref List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato> alcadaValorContrato, ref List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo> alcadaValorAcrescimo, ref List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto> alcadaValorDesconto, ref List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros> alcadaTerceiros, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region ValorContrato
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraContratoFreteTerceiro.RegraPorValorContrato)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasValorContrato", true, ref alcadaValorContrato, ref regraContratoFreteTerceiro);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor do Contrato");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor do Contrato", alcadaValorContrato, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                alcadaValorContrato = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato>();
            }
            #endregion

            #region ValorAcrescimo
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraContratoFreteTerceiro.RegraPorValorAcrescimo)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasValorAcrescimo", true, ref alcadaValorAcrescimo, ref regraContratoFreteTerceiro);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor do Acréscimo");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor do Acréscimo", alcadaValorContrato, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                alcadaValorAcrescimo = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo>();
            }
            #endregion

            #region ValorDesconto
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraContratoFreteTerceiro.RegraPorValorDesconto)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasValorDesconto", true, ref alcadaValorDesconto, ref regraContratoFreteTerceiro);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor do Desconto");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor do Desconto", alcadaValorDesconto, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                alcadaValorDesconto = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto>();
            }
            #endregion

            #region Terceiros
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraContratoFreteTerceiro.RegraPorTerceiros)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasTerceiro", false, ref alcadaTerceiros, ref regraContratoFreteTerceiro, ((codigo) => {
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        double.TryParse(codigo.ToString(), out double codigoDouble);

                        return repCliente.BuscarPorCPFCNPJ(codigoDouble);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Terceiros");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Terceiros", alcadaTerceiros, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                alcadaTerceiros = new List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros>();
            }
            #endregion
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro repRegraContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.RegraContratoFreteTerceiro(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Aprovador"), out int codigoAprovador);

            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

            Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

            // Consulta
            List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaGrid = repRegraContratoFreteTerceiro.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegraContratoFreteTerceiro.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }

        private void SalvarAlteracaoCriterioDaRegra<T, R>(Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro regraContratoFreteTerceiro, List<T> criterios, R repositorio, string descricao, ref List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, Repositorio.UnitOfWork unitOfWork) where T : Dominio.Entidades.EntidadeBase where R : Repositorio.RepositorioBase<T>
        {
            bool inseriuCriterio = false;
            for (var i = 0; i < criterios.Count(); i++)
            {
                PropertyInfo prop;

                prop = criterios[i].GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                int.TryParse(prop.GetValue(criterios[i]).ToString(), out int codigo);

                if (codigo == 0)
                {
                    repositorio.Inserir(criterios[i]);
                    inseriuCriterio = true;
                }
                else
                {
                    alteracoes.AddRange(criterios[i].GetChanges());
                    repositorio.Atualizar(criterios[i]);
                }
            }
            if (inseriuCriterio)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, regraContratoFreteTerceiro, null, "Adicionou um critério de " + descricao + ".", unitOfWork);
        }
        #endregion
    }
}

