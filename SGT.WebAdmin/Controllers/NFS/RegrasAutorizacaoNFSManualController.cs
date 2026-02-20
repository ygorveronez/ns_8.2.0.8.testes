using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize("NFS/RegrasAutorizacaoNFSManual")]
    public class RegrasAutorizacaoNFSManualController : BaseController
    {
		#region Construtores

		public RegrasAutorizacaoNFSManualController(Conexao conexao) : base(conexao) { }

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
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual Juncao { get; set; }
            public ObjetoEntidade Entidade { get; set; }
            public dynamic Valor { get; set; }
        }
        #endregion

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
                Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual repRegrasAutorizacaoNFSManual = new Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualFilial repRegrasNFSManualFilial = new Repositorio.Embarcador.NFS.RegrasNFSManualFilial(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualTransportadora repRegrasNFSManualTransportadora = new Repositorio.Embarcador.NFS.RegrasNFSManualTransportadora(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualTomador repRegrasNFSManualTomador = new Repositorio.Embarcador.NFS.RegrasNFSManualTomador(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualValorPrestacaoServico repRegrasNFSManualValorPrestacaoServico = new Repositorio.Embarcador.NFS.RegrasNFSManualValorPrestacaoServico(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regrasNFSManual = new Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual();
                List<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual> regraFilial = new List<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual>();
                List<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual> regraTransportadora = new List<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual>();
                List<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual> regraTomador = new List<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual>();
                List<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual> regraValorPrestacaoServico = new List<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual>();

                // Preenche a entidade
                PreencherEntidade(ref regrasNFSManual, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasNFSManual, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasNFSManual, ref regraFilial, ref regraTransportadora, ref regraTomador, ref regraValorPrestacaoServico, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegrasAutorizacaoNFSManual.Inserir(regrasNFSManual, Auditado);

                // Insere regras
                for (var i = 0; i < regraFilial.Count(); i++) repRegrasNFSManualFilial.Inserir(regraFilial[i]);
                for (var i = 0; i < regraTransportadora.Count(); i++) repRegrasNFSManualTransportadora.Inserir(regraTransportadora[i]);
                for (var i = 0; i < regraTomador.Count(); i++) repRegrasNFSManualTomador.Inserir(regraTomador[i]);
                for (var i = 0; i < regraValorPrestacaoServico.Count(); i++) repRegrasNFSManualValorPrestacaoServico.Inserir(regraValorPrestacaoServico[i]);

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
                Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual repRegrasAutorizacaoNFSManual = new Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualFilial repRegrasNFSManualFilial = new Repositorio.Embarcador.NFS.RegrasNFSManualFilial(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualTransportadora repRegrasNFSManualTransportadora = new Repositorio.Embarcador.NFS.RegrasNFSManualTransportadora(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualTomador repRegrasNFSManualTomador = new Repositorio.Embarcador.NFS.RegrasNFSManualTomador(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualValorPrestacaoServico repRegrasNFSManualValorPrestacaoServico = new Repositorio.Embarcador.NFS.RegrasNFSManualValorPrestacaoServico(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regrasNFSManual = repRegrasAutorizacaoNFSManual.BuscarPorCodigo(codigoRegra, true);

                if (regrasNFSManual == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual> regraFilial = repRegrasNFSManualFilial.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual> regraTransportadora = repRegrasNFSManualTransportadora.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual> regraTomador = repRegrasNFSManualTomador.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual> regraValorPrestacaoServico = repRegrasNFSManualValorPrestacaoServico.BuscarPorRegras(codigoRegra);
                #endregion


                #region Deleta Regras
                for (var i = 0; i < regraFilial.Count(); i++) repRegrasNFSManualFilial.Deletar(regraFilial[i]);
                for (var i = 0; i < regraTransportadora.Count(); i++) repRegrasNFSManualTransportadora.Deletar(regraTransportadora[i]);
                for (var i = 0; i < regraTomador.Count(); i++) repRegrasNFSManualTomador.Deletar(regraTomador[i]);
                for (var i = 0; i < regraValorPrestacaoServico.Count(); i++) repRegrasNFSManualValorPrestacaoServico.Deletar(regraValorPrestacaoServico[i]);
                #endregion


                #region Novas Regras
                regraFilial = new List<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual>();
                regraTransportadora = new List<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual>();
                regraTomador = new List<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual>();
                regraValorPrestacaoServico = new List<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual>();
                #endregion


                // Preenche a entidade
                PreencherEntidade(ref regrasNFSManual, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasNFSManual, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                // Atualiza Entidade
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repRegrasAutorizacaoNFSManual.Atualizar(regrasNFSManual, Auditado);

                try
                {
                    PreencherTodasRegras(ref regrasNFSManual, ref regraFilial, ref regraTransportadora, ref regraTomador, ref regraValorPrestacaoServico, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                for (var i = 0; i < regraFilial.Count(); i++) repRegrasNFSManualFilial.Inserir(regraFilial[i], Auditado, historico);
                for (var i = 0; i < regraTransportadora.Count(); i++) repRegrasNFSManualTransportadora.Inserir(regraTransportadora[i], Auditado, historico);
                for (var i = 0; i < regraTomador.Count(); i++) repRegrasNFSManualTomador.Inserir(regraTomador[i], Auditado, historico);
                for (var i = 0; i < regraValorPrestacaoServico.Count(); i++) repRegrasNFSManualValorPrestacaoServico.Inserir(regraValorPrestacaoServico[i], Auditado, historico);

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
                Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual repRegrasAutorizacaoNFSManual = new Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualFilial repRegrasNFSManualFilial = new Repositorio.Embarcador.NFS.RegrasNFSManualFilial(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualTransportadora repRegrasNFSManualTransportadora = new Repositorio.Embarcador.NFS.RegrasNFSManualTransportadora(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualTomador repRegrasNFSManualTomador = new Repositorio.Embarcador.NFS.RegrasNFSManualTomador(unitOfWork);
                Repositorio.Embarcador.NFS.RegrasNFSManualValorPrestacaoServico repRegrasNFSManualValorPrestacaoServico = new Repositorio.Embarcador.NFS.RegrasNFSManualValorPrestacaoServico(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regrasNFSManual = repRegrasAutorizacaoNFSManual.BuscarPorCodigo(codigo);

                if (regrasNFSManual == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual> regraFilial = repRegrasNFSManualFilial.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual> regraTransportadora = repRegrasNFSManualTransportadora.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual> regraTomador = repRegrasNFSManualTomador.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual> regraValorPrestacaoServico = repRegrasNFSManualValorPrestacaoServico.BuscarPorRegras(codigo);
                #endregion


                var dynRegra = new
                {
                    regrasNFSManual.Codigo,
                    regrasNFSManual.NumeroAprovadores,
                    Requisito = regrasNFSManual.Requisito != null ? new { regrasNFSManual.Requisito.Codigo, regrasNFSManual.Requisito.Descricao } : null,
                    Vigencia = regrasNFSManual.Vigencia.HasValue ? regrasNFSManual.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasNFSManual.Descricao) ? regrasNFSManual.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasNFSManual.Observacoes) ? regrasNFSManual.Observacoes : string.Empty,
                    Status = regrasNFSManual.Ativo,
                    regrasNFSManual.PrioridadeAprovacao,

                    Aprovadores = (from o in regrasNFSManual.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorFilial = regrasNFSManual.RegraPorFilial,
                    Filial = (from obj in regraFilial select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual>(obj, "Filial", "Descricao")).ToList(),

                    UsarRegraPorTransportador = regrasNFSManual.RegraPorTransportadora,
                    Transportador = (from obj in regraTransportadora select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual>(obj, "Transportadora", "RazaoSocial")).ToList(),

                    UsarRegraPorTomador = regrasNFSManual.RegraPorTomador,
                    Tomador = (from obj in regraTomador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual>(obj, "Tomador", "Nome")).ToList(),

                    UsarRegraPorValorPrestacaoServico = regrasNFSManual.RegraPorValorPrestacaoServico,
                    ValorPrestacaoServico = (from obj in regraValorPrestacaoServico select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual>(obj, "ValorPrestacaoServico", "Valor", true)).ToList(),
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
                Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual repRegrasAutorizacaoNFSManual = new Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regrasNFSManual = repRegrasAutorizacaoNFSManual.BuscarPorCodigo(codigo);

                if (regrasNFSManual == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasNFSManual.Aprovadores.Clear();
                regrasNFSManual.RegrasFilial.Clear();
                regrasNFSManual.RegrasTransportadora.Clear();
                regrasNFSManual.RegrasTomador.Clear();
                regrasNFSManual.RegrasValorPrestacaoServico.Clear();
                

                repRegrasAutorizacaoNFSManual.Deletar(regrasNFSManual, Auditado);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Já existem NFSs vinculadas à regra.");
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

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);

            return grid;
        }

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regrasNFSManual, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual repRegrasAutorizacaoNFSManual = new Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual(unitOfWork);


            // Converte parametros
            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : string.Empty;
            string observacao = !string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? Request.Params("Observacao") : string.Empty;

            DateTime dataVigenciaAux;
            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            int.TryParse(Request.Params("NumeroAprovadores"), out int numeroAprovadores);
            int.TryParse(Request.Params("Requisito"), out int requisito);

            bool.TryParse(Request.Params("UsarRegraPorFilial"), out bool usarRegraPorFilial);
            bool.TryParse(Request.Params("UsarRegraPorTransportador"), out bool usarRegraPorTransportador);
            bool.TryParse(Request.Params("UsarRegraPorTomador"), out bool usarRegraPorTomador);
            bool.TryParse(Request.Params("UsarRegraPorValorPrestacaoServico"), out bool usarRegraPorValorPrestacaoServico);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regrasNFSManual.Descricao = descricao;
            regrasNFSManual.Observacoes = observacao;
            regrasNFSManual.Vigencia = dataVigencia;
            regrasNFSManual.NumeroAprovadores = numeroAprovadores;
            regrasNFSManual.Aprovadores = listaAprovadores;
            regrasNFSManual.Requisito = repRegrasAutorizacaoNFSManual.BuscarPorCodigo(requisito);
            regrasNFSManual.PrioridadeAprovacao = Request.GetIntParam("PrioridadeAprovacao");
            regrasNFSManual.Ativo = Request.GetBoolParam("Status");

            regrasNFSManual.RegraPorFilial = usarRegraPorFilial;
            regrasNFSManual.RegraPorTransportadora = usarRegraPorTransportador;
            regrasNFSManual.RegraPorTomador = usarRegraPorTomador;
            regrasNFSManual.RegraPorValorPrestacaoServico = usarRegraPorValorPrestacaoServico;
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regrasNFSManual, Func<dynamic, object> lambda = null)
        {
            /* Descricao
             * RegrasAutorizacaoOcorrencia é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
             * 
             * Esse método facilita a instancia de novas regras, já que todas possuem o mesmo padra
             * - RegraOcorrencia (Entidade Pai)
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
                // Instancia o objeto T (T não possui construor new)
                T regra = default(T);
                regra = Activator.CreateInstance<T>();

                // Seta as propriedas da entidade
                int codigoRegra = 0;
                int.TryParse(dynRegras[i].Codigo.ToString(), out codigoRegra);
                prop = regra.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, codigoRegra, null);

                prop = regra.GetType().GetProperty("RegrasAutorizacaoNFSManual", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasNFSManual, null);

                prop = regra.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Ordem, null);

                prop = regra.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Condicao, null);

                prop = regra.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Juncao, null);

                if (!usarDynamic)
                {
                    // Executa lambda
                    var result = dynRegras[i].Entidade != null ? lambda(dynRegras[i].Entidade.Codigo) : null;

                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(regra, result, null);
                }
                else
                {
                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);
                    if (prop.PropertyType.Name.Equals("Decimal"))
                    {
                        decimal valorDecimal = 0;
                        decimal.TryParse(dynRegras[i].Valor.ToString(), out valorDecimal);

                        prop.SetValue(regra, valorDecimal, null);
                    }
                    else
                    {
                        prop.SetValue(regra, dynRegras[i].Valor, null);
                    }
                }

                // Adiciona lista de retorno
                regrasPorTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regra, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regra.Descricao))
                erros.Add("Descrição é obrigatória.");            

            if (regra.Aprovadores.Count() < regra.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regra.NumeroAprovadores.ToString());

            if (regra.Requisito != null && regra.Requisito.Codigo == regra.Codigo)
                erros.Add("O requisito não pode ser a mesma que a regra.");

            if (regra.Requisito != null && regra.Requisito.Requisito != null)
                erros.Add("O requisito selecionado já possui um requisito.");

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, string nomePropriedade, List<T> regrasPorTipo, out List<string> erros)
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
                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);

                    if (prop.GetValue(regra) == null)
                        erros.Add(nomeRegra + " da regra é obrigatório.");
                }
            }

            return erros.Count() == 0;
        }

        private RegrasPorTipo RetornaRegraPorTipoDyn<T>(dynamic obj, string paramentro, string paramentroDescricaoValor, bool usarValor = false)
        {
            // Variavel auxiliar
            PropertyInfo prop;

            prop = obj.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
            int codigo = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
            int ordem = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual condicao = prop.GetValue(obj);


            ObjetoEntidade objetoEntidade = new ObjetoEntidade();
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty(paramentro, BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                dynamic codigoEntidade = prop.GetValue(entidade);

                prop = entidade.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                string descricaoEntidade = prop.GetValue(entidade);

                objetoEntidade.Codigo = codigoEntidade;
                objetoEntidade.Descricao = descricaoEntidade;
            }
            else
            {
                prop = obj.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                valor = prop.GetValue(obj);
            }

            RegrasPorTipo restorno = new RegrasPorTipo()
            {
                Codigo = codigo,
                Ordem = ordem,
                Juncao = juncao,
                Condicao = condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return restorno;
        }

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual regrasNFSManual, ref List<Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual> regraFilial, ref List<Dominio.Entidades.Embarcador.NFS.RegrasTransportadoraNFSManual> regraTransportadora, ref List<Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual> regraTomador, ref List<Dominio.Entidades.Embarcador.NFS.RegrasValorPrestacaoServicoNFSManual> regraValorPrestacaoServico, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region Filial
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasNFSManual.RegraPorFilial)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Filial", "RegrasFilial", false, ref regraFilial, ref regrasNFSManual, ((codigo) => {
                        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repFilial.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Filial");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Filial", "Filial", regraFilial, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion
            
            #region Transportadora
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasNFSManual.RegraPorTransportadora)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Transportadora", "RegrasTransportador", false, ref regraTransportadora, ref regrasNFSManual, ((codigo) => {
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repEmpresa.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Transportadora");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Transportador", "Transportadora", regraTransportadora, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Tomador
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasNFSManual.RegraPorTomador)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Tomador", "RegrasTomador", false, ref regraTomador, ref regrasNFSManual, ((codigo) => {
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        double.TryParse(codigo.ToString(), out double codigoDb);

                        return repCliente.BuscarPorCPFCNPJ(codigoDb);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Tomador");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Tomador", "Tomador", regraTomador, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Valor Prestacao Servico
            if (regrasNFSManual.RegraPorValorPrestacaoServico)
            {
                try
                {
                    PreencherEntidadeRegra("Valor", "RegrasValorPrestacaoServico", true, ref regraValorPrestacaoServico, ref regrasNFSManual);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor da Prestação do Serviço");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor da Prestação do Serviço", "Valor", regraValorPrestacaoServico, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.NFS.RegrasAutorizacaoNFSManual(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Aprovador"), out int codigoAprovador);

            DateTime? dataInicio = null, dataFim = null;
            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                dataInicio = dataInicioAux;
            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                dataFim = dataFimAux;

            string descricao = Request.Params("Descricao") ?? string.Empty;

            int.TryParse(Request.Params("RegraNFSManual"), out int regraNFSManual); // Filtro de busca requisito
            bool? emVigencia = null;
            if (bool.TryParse(Request.Params("EmVigencia"), out bool emVigenciaAux))// Filtro de busca requisito
                emVigencia = emVigenciaAux;

            Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

            // Consulta
            List<Dominio.Entidades.Embarcador.NFS.RegrasAutorizacaoNFSManual> listaGrid = repRegrasAutorizacaoOcorrencia.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, emVigenciaAux, regraNFSManual, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegrasAutorizacaoOcorrencia.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao, emVigenciaAux, regraNFSManual);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }
        
        #endregion
    }
}
