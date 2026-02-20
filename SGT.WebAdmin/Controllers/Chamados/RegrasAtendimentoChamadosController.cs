using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/RegrasAtendimentoChamado")]
    public class RegrasAtendimentoChamadosController : BaseController
    {
		#region Construtores

		public RegrasAtendimentoChamadosController(Conexao conexao) : base(conexao) { }

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
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria Juncao { get; set; }
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
                Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados repositorioRegrasAtendimentoChamados = new Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoFilial repositorioRegrasAtendimentoFilial = new Repositorio.Embarcador.Chamados.RegrasAtendimentoFilial(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoCanalVenda repositorioRegrasAtendimentoCanalVenda = new Repositorio.Embarcador.Chamados.RegrasAtendimentoCanalVenda(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoEstado repositorioRegrasAtendimentoEstado = new Repositorio.Embarcador.Chamados.RegrasAtendimentoEstado(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoTipoOperacao repositorioRegrasAtendimentoTipoOperacao = new Repositorio.Embarcador.Chamados.RegrasAtendimentoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoTransportador repositorioRegrasAtendimentoTransportador = new Repositorio.Embarcador.Chamados.RegrasAtendimentoTransportador(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamados = new Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial> regraFilial = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial>();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda> regraCanalVenda = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda>();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado> regraEstado = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado>();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao> regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao>();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador> regraTransportador = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador>();

                // Preenche a entidade
                PreencherEntidade(ref regrasAtendimentoChamados, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasAtendimentoChamados, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasAtendimentoChamados, ref regraCanalVenda, ref regraFilial, ref regraTipoOperacao, ref regraTransportador, ref regraEstado, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repositorioRegrasAtendimentoChamados.Inserir(regrasAtendimentoChamados, Auditado);

                // Insere regras
                for (var i = 0; i < regraFilial.Count(); i++) repositorioRegrasAtendimentoFilial.Inserir(regraFilial[i]);
                for (var i = 0; i < regraCanalVenda.Count(); i++) repositorioRegrasAtendimentoCanalVenda.Inserir(regraCanalVenda[i]);
                for (var i = 0; i < regraEstado.Count(); i++) repositorioRegrasAtendimentoEstado.Inserir(regraEstado[i]);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repositorioRegrasAtendimentoTipoOperacao.Inserir(regraTipoOperacao[i]);
                for (var i = 0; i < regraTransportador.Count(); i++) repositorioRegrasAtendimentoTransportador.Inserir(regraTransportador[i]);

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
                Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados repositorioRegrasAtendimentoChamados = new Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoFilial repositorioRegrasAtendimentoFilial = new Repositorio.Embarcador.Chamados.RegrasAtendimentoFilial(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoCanalVenda repositorioRegrasAtendimentoCanalVenda = new Repositorio.Embarcador.Chamados.RegrasAtendimentoCanalVenda(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoEstado repositorioRegrasAtendimentoEstado = new Repositorio.Embarcador.Chamados.RegrasAtendimentoEstado(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoTipoOperacao repositorioRegrasAtendimentoTipoOperacao = new Repositorio.Embarcador.Chamados.RegrasAtendimentoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoTransportador repositorioRegrasAtendimentoTransportador = new Repositorio.Embarcador.Chamados.RegrasAtendimentoTransportador(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamados = repositorioRegrasAtendimentoChamados.BuscarPorCodigo(codigoRegra, true);

                if (regrasAtendimentoChamados == null)
                    return new JsonpResult(false, true, "Não foi possível buscar a regra.");

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial> regraFilial = repositorioRegrasAtendimentoFilial.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao> regraTipoOperacao = repositorioRegrasAtendimentoTipoOperacao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda> regraCanalVenda = repositorioRegrasAtendimentoCanalVenda.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado> regraEstado = repositorioRegrasAtendimentoEstado.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador> regraTransportador = repositorioRegrasAtendimentoTransportador.BuscarPorRegras(codigoRegra);

                #endregion

                List<Dominio.Entidades.Usuario> listaAprovadoresAnteriores = regrasAtendimentoChamados.Aprovadores.ToList();

                // Preenche a entidade
                PreencherEntidade(ref regrasAtendimentoChamados, unitOfWork);
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repositorioRegrasAtendimentoChamados.Atualizar(regrasAtendimentoChamados, Auditado);

                #region Deleta Regras

                for (var i = 0; i < regraFilial.Count(); i++) repositorioRegrasAtendimentoFilial.Deletar(regraFilial[i], Auditado, historico);
                for (var i = 0; i < regraCanalVenda.Count(); i++) repositorioRegrasAtendimentoCanalVenda.Deletar(regraCanalVenda[i], Auditado, historico);
                for (var i = 0; i < regraEstado.Count(); i++) repositorioRegrasAtendimentoEstado.Deletar(regraEstado[i], Auditado, historico);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repositorioRegrasAtendimentoTipoOperacao.Deletar(regraTipoOperacao[i], Auditado, historico);
                for (var i = 0; i < regraTransportador.Count(); i++) repositorioRegrasAtendimentoTransportador.Deletar(regraTransportador[i], Auditado, historico);

                #endregion

                #region Novas Regras

                regraFilial = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial>();
                regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao>();
                regraCanalVenda = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda>();
                regraEstado = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado>();
                regraTransportador = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador>();

                #endregion

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasAtendimentoChamados, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasAtendimentoChamados, ref regraCanalVenda, ref regraFilial, ref regraTipoOperacao, ref regraTransportador, ref regraEstado, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                for (var i = 0; i < regraFilial.Count(); i++) repositorioRegrasAtendimentoFilial.Inserir(regraFilial[i], Auditado, historico);
                for (var i = 0; i < regraCanalVenda.Count(); i++) repositorioRegrasAtendimentoCanalVenda.Inserir(regraCanalVenda[i], Auditado, historico);
                for (var i = 0; i < regraEstado.Count(); i++) repositorioRegrasAtendimentoEstado.Inserir(regraEstado[i], Auditado, historico);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repositorioRegrasAtendimentoTipoOperacao.Inserir(regraTipoOperacao[i], Auditado, historico);
                for (var i = 0; i < regraTransportador.Count(); i++) repositorioRegrasAtendimentoTransportador.Inserir(regraTransportador[i], Auditado, historico);

                AtualizarAprovadoresChamadosEmAberto(regrasAtendimentoChamados, listaAprovadoresAnteriores, unitOfWork);

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
                Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados repositorioRegrasAtendimentoChamados = new Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoFilial repositorioRegrasAtendimentoFilial = new Repositorio.Embarcador.Chamados.RegrasAtendimentoFilial(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoCanalVenda repositorioRegrasAtendimentoCanalVenda = new Repositorio.Embarcador.Chamados.RegrasAtendimentoCanalVenda(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoEstado repositorioRegrasAtendimentoEstado = new Repositorio.Embarcador.Chamados.RegrasAtendimentoEstado(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoTipoOperacao repositorioRegrasAtendimentoTipoOperacao = new Repositorio.Embarcador.Chamados.RegrasAtendimentoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasAtendimentoTransportador repositorioRegrasAtendimentoTransportador = new Repositorio.Embarcador.Chamados.RegrasAtendimentoTransportador(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamados = repositorioRegrasAtendimentoChamados.BuscarPorCodigo(codigo);

                if (regrasAtendimentoChamados == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial> regraFilial = repositorioRegrasAtendimentoFilial.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao> regraTipoOperacao = repositorioRegrasAtendimentoTipoOperacao.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda> regraCanalVenda = repositorioRegrasAtendimentoCanalVenda.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado> regraEstado = repositorioRegrasAtendimentoEstado.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador> regraTransportador = repositorioRegrasAtendimentoTransportador.BuscarPorRegras(codigo);

                #endregion

                var dynRegra = new
                {
                    regrasAtendimentoChamados.Codigo,
                    Vigencia = regrasAtendimentoChamados.Vigencia.HasValue ? regrasAtendimentoChamados.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = regrasAtendimentoChamados.Descricao ?? string.Empty,
                    Observacao = regrasAtendimentoChamados.Observacoes ?? string.Empty,
                    Aprovadores = (from o in regrasAtendimentoChamados.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    RegraPorFilial = regrasAtendimentoChamados.RegraPorFilial,
                    Filial = (from obj in regraFilial select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial>(obj, "Filial", "Descricao")).ToList(),

                    RegraPorTipoOperacao = regrasAtendimentoChamados.RegraPorTipoOperacao,
                    TipoOperacao = (from obj in regraTipoOperacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao>(obj, "TipoOperacao", "Descricao")).ToList(),

                    RegraPorTransportador = regrasAtendimentoChamados.RegraPorTransportador,
                    Transportador = (from obj in regraTransportador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador>(obj, "Empresa", "Descricao")).ToList(),

                    RegraPorEstado = regrasAtendimentoChamados.RegraPorEstado,
                    Estado = (from obj in regraEstado select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado>(obj, "Estado", "Sigla")).ToList(),

                    RegraPorCanalVenda = regrasAtendimentoChamados.RegraPorCanalVenda,
                    CanalVenda = (from obj in regraCanalVenda select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda>(obj, "CanalVenda", "Descricao")).ToList(),

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
                Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados repositorioRegrasAtendimentoChamados = new Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamados = repositorioRegrasAtendimentoChamados.BuscarPorCodigo(codigo);

                if (regrasAtendimentoChamados == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasAtendimentoChamados.Aprovadores.Clear();
                regrasAtendimentoChamados.RegrasFilial.Clear();
                regrasAtendimentoChamados.RegrasCanalVenda.Clear();
                regrasAtendimentoChamados.RegrasEstado.Clear();
                regrasAtendimentoChamados.RegrasTipoOperacao.Clear();
                regrasAtendimentoChamados.RegrasTransportador.Clear();

                repositorioRegrasAtendimentoChamados.Deletar(regrasAtendimentoChamados, Auditado);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Já existem chamados vinculadas à regra.");
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

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamados, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            DateTime? dataVigencia = null;
            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            bool usarRegraPorFilial = Request.GetBoolParam("RegraPorFilial");
            bool usarRegraPorTipoOperacao = Request.GetBoolParam("RegraPorTipoOperacao");
            bool usarRegraPorCanalVenda = Request.GetBoolParam("RegraPorCanalVenda");
            bool usarRegraPorTransportador = Request.GetBoolParam("RegraPorTransportador");
            bool usarRegraPorEstado = Request.GetBoolParam("RegraPorEstado");

            // Seta na entidade
            regrasAtendimentoChamados.Descricao = descricao;
            regrasAtendimentoChamados.Observacoes = observacao;
            regrasAtendimentoChamados.Vigencia = dataVigencia;

            regrasAtendimentoChamados.RegraPorFilial = usarRegraPorFilial;
            regrasAtendimentoChamados.RegraPorTipoOperacao = usarRegraPorTipoOperacao;
            regrasAtendimentoChamados.RegraPorCanalVenda = usarRegraPorCanalVenda;
            regrasAtendimentoChamados.RegraPorTransportador = usarRegraPorTransportador;
            regrasAtendimentoChamados.RegraPorEstado = usarRegraPorEstado;

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);
            regrasAtendimentoChamados.Aprovadores = listaAprovadores;
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamados, Func<dynamic, object> lambda = null)
        {
            /* Descricao
             * RegrasAtendimentoChamados é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
             * 
             * Esse método facilita a instancia de novas regras, já que todas possuem o mesmo padrão
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

                prop = regra.GetType().GetProperty("RegrasAtendimentoChamados", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasAtendimentoChamados, null);

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

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamados, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasAtendimentoChamados.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasAtendimentoChamados.Aprovadores == null || regrasAtendimentoChamados.Aprovadores.Count() == 0)
                erros.Add("Nenhum aprovador selecionado.");

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
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria condicao = prop.GetValue(obj);


            ObjetoEntidade objetoEntidade = new ObjetoEntidade();
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty(paramentro, BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                dynamic codigoEntidade = null;
                string descricaoEntidade = "";
                if (paramentro == "Estado")
                {
                    prop = entidade.GetType().GetProperty("Sigla", BindingFlags.Public | BindingFlags.Instance);
                    codigoEntidade = prop.GetValue(entidade);

                    prop = entidade.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                    descricaoEntidade = prop.GetValue(entidade);
                }
                else
                {
                    prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                    codigoEntidade = prop.GetValue(entidade);

                    prop = entidade.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                    descricaoEntidade = prop.GetValue(entidade);
                }

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

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamado, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda> regrasAtendimentoCanalVenda, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial> regraFilial, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao> regrasAtendimentoTipoOperacao, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador> regrasAtendimentoTransportador, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado> regrasAtendimentoEstado, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region Filial
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAtendimentoChamado.RegraPorFilial)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Filial", "RegrasFilial", false, ref regraFilial, ref regrasAtendimentoChamado, ((codigo) =>
                    {
                        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                        int.TryParse(codigo.ToString(), out int codigoInt);
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

            #region Canal Venda
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAtendimentoChamado.RegraPorCanalVenda)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("CanalVenda", "RegrasCanalVenda", false, ref regrasAtendimentoCanalVenda, ref regrasAtendimentoChamado, ((codigo) =>
                    {
                        Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);
                        int.TryParse(codigo.ToString(), out int codigoInt);
                        return repositorioCanalVenda.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("CanalVenda");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Canal de Venda", "CanalVenda", regrasAtendimentoCanalVenda, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Tipo Operação
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAtendimentoChamado.RegraPorTipoOperacao)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("TipoOperacao", "RegrasTipoOperacao", false, ref regrasAtendimentoTipoOperacao, ref regrasAtendimentoChamado, ((codigo) =>
                    {
                        Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                        int.TryParse(codigo.ToString(), out int codigoInt);
                        return repositorioTipoOperacao.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("TipoOperacao");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Tipo Operação", "TipoOperacao", regrasAtendimentoTipoOperacao, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Transportador
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAtendimentoChamado.RegraPorTransportador)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Empresa", "RegrasTransportador", false, ref regrasAtendimentoTransportador, ref regrasAtendimentoChamado, ((codigo) =>
                    {
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repEmpresa.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Transportador");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Empresa", "Empresa", regrasAtendimentoTransportador, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Estado 
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAtendimentoChamado.RegraPorEstado)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Estado", "RegrasEstado", false, ref regrasAtendimentoEstado, ref regrasAtendimentoChamado, ((codigo) =>
                    {
                        Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                        return repEstado.BuscarPorSigla(codigo.ToString());
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Estado");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Estado", "Estado", regrasAtendimentoEstado, out erros))
                {
                    unitOfWork.Rollback();
                    throw new Exception(String.Join("<br>", erros));
                }
            }
            #endregion

        }

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

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados repositorioRegrasAtendimentoChamados = new Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";


            // Consulta
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaGrid = repositorioRegrasAtendimentoChamados.ConsultarRegras(dataInicio, dataFim, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repositorioRegrasAtendimentoChamados.ContarConsultaRegras(dataInicio, dataFim, descricao);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }

        private void AtualizarAprovadoresChamadosEmAberto(Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regrasAtendimentoChamados, List<Dominio.Entidades.Usuario> listaAprovadoresAnteriores, Repositorio.UnitOfWork unitOfWork)
        {
            if (listaAprovadoresAnteriores.All(regrasAtendimentoChamados.Aprovadores.Contains) && listaAprovadoresAnteriores.Count == regrasAtendimentoChamados.Aprovadores.Count)
                return;

            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repositorioChamado.BuscarChamadosEmAbertoPorRegraAnalise(regrasAtendimentoChamados.Codigo);
            if (chamados.Count == 0)
                return;

            List<Dominio.Entidades.Usuario> listaAprovadoresRemovidos = listaAprovadoresAnteriores.Where(o => !regrasAtendimentoChamados.Aprovadores.Contains(o)).ToList();

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
            {
                if (chamado.Analistas.All(regrasAtendimentoChamados.Aprovadores.Contains) && chamado.Analistas.Count == regrasAtendimentoChamados.Aprovadores.Count)
                    continue;

                chamado.Initialize();

                foreach (Dominio.Entidades.Usuario aprovador in regrasAtendimentoChamados.Aprovadores)
                {
                    if (!chamado.Analistas.Contains(aprovador))
                        chamado.Analistas.Add(aprovador);
                }

                //Remove individual, pois ele pode estar em outra regra
                foreach (Dominio.Entidades.Usuario aprovador in listaAprovadoresRemovidos)
                {
                    bool aprovadorEmOutraRegra = chamado.RegrasAnalise.Where(o => o.Codigo != regrasAtendimentoChamados.Codigo).Any(o => o.Aprovadores.Any(s => s.Codigo == aprovador.Codigo));
                    if (chamado.Analistas.Contains(aprovador) && chamado.Responsavel != aprovador && !aprovadorEmOutraRegra)
                        chamado.Analistas.Remove(aprovador);
                }

                repositorioChamado.Atualizar(chamado);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = chamado.GetChanges();
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, alteracoes, $"Atualizou aprovadores ao atualizar a regra de análise {regrasAtendimentoChamados.Descricao}", unitOfWork);
            }
        }

        #endregion
    }
}

