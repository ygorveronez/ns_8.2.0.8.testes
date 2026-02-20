using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Dominio.Entidades.Embarcador.Chamados;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/RegrasAnaliseChamados")]
    public class RegrasAnaliseChamadosController : BaseController
    {
		#region Construtores

		public RegrasAnaliseChamadosController(Conexao conexao) : base(conexao) { }

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

        private class RegrasTipoCargaDescarga
        {
            public dynamic Codigo { get; set; }
            public int Ordem { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria Juncao { get; set; }
            public dynamic ValidarValorInformadoCarga { get; set; }
            public dynamic ValidarValorInformadoDescarga { get; set; }
            public ObjetoEntidade Entidade { get; set; }
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
                Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasMotivoChamado repRegrasMotivoChamado = new Repositorio.Embarcador.Chamados.RegrasMotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosFilial repRegrasChamadosFilial = new Repositorio.Embarcador.Chamados.RegrasChamadosFilial(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosRegiaoDestino repRegrasChamadosRegiaoDestino = new Repositorio.Embarcador.Chamados.RegrasChamadosRegiaoDestino(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosCargaDescarga repRegrasCargaDescarga = new Repositorio.Embarcador.Chamados.RegrasChamadosCargaDescarga(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise = new Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado> regraMotivo = new List<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado>();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial> regraFilial = new List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial>();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino> regraRegiaoDestino = new List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino>();
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga> regraCargaDescarga = new List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga>();


                // Preenche a entidade
                PreencherEntidade(ref regrasAnalise, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasAnalise, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasAnalise, ref regraMotivo, ref regraFilial, ref regraRegiaoDestino,ref regraCargaDescarga, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegrasAnaliseChamados.Inserir(regrasAnalise, Auditado);

                // Insere regras
                for (var i = 0; i < regraMotivo.Count(); i++) repRegrasMotivoChamado.Inserir(regraMotivo[i]);
                for (var i = 0; i < regraFilial.Count(); i++) repRegrasChamadosFilial.Inserir(regraFilial[i]);
                for (var i = 0; i < regraRegiaoDestino.Count(); i++) repRegrasChamadosRegiaoDestino.Inserir(regraRegiaoDestino[i]);
                for (var i = 0; i < regraCargaDescarga.Count(); i++) repRegrasCargaDescarga.Inserir(regraCargaDescarga[i]);


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
                Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasMotivoChamado repRegrasMotivoChamado = new Repositorio.Embarcador.Chamados.RegrasMotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosFilial repRegrasChamadosFilial = new Repositorio.Embarcador.Chamados.RegrasChamadosFilial(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosRegiaoDestino repRegrasRegiaoDestino = new Repositorio.Embarcador.Chamados.RegrasChamadosRegiaoDestino(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosCargaDescarga repRegrasCargaDescarga = new Repositorio.Embarcador.Chamados.RegrasChamadosCargaDescarga(unitOfWork);
                
                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise = repRegrasAnaliseChamados.BuscarPorCodigo(codigoRegra, true);

                if (regrasAnalise == null)
                    return new JsonpResult(false, true, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado> regraMotivoChamado = repRegrasMotivoChamado.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial> regraFilial = repRegrasChamadosFilial.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino> regraRegiaoDestino = repRegrasRegiaoDestino.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga> regraCargaDescarga = repRegrasCargaDescarga.BuscarPorRegras(codigoRegra);
                #endregion

                List<Dominio.Entidades.Usuario> listaAprovadoresAnteriores = regrasAnalise.Aprovadores.ToList();

                // Preenche a entidade
                PreencherEntidade(ref regrasAnalise, unitOfWork);
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repRegrasAnaliseChamados.Atualizar(regrasAnalise, Auditado);

                #region Deleta Regras
                for (var i = 0; i < regraMotivoChamado.Count(); i++) repRegrasMotivoChamado.Deletar(regraMotivoChamado[i], Auditado, historico);
                for (var i = 0; i < regraFilial.Count(); i++) repRegrasChamadosFilial.Deletar(regraFilial[i], Auditado, historico);
                for (var i = 0; i < regraRegiaoDestino.Count(); i++) repRegrasRegiaoDestino.Deletar(regraRegiaoDestino[i], Auditado, historico);
                for (var i = 0; i < regraCargaDescarga.Count(); i++) repRegrasCargaDescarga.Deletar(regraCargaDescarga[i]);
                #endregion

                #region Novas Regras
                regraMotivoChamado = new List<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado>();
                regraFilial = new List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial>();
                regraRegiaoDestino = new List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino>();
                regraCargaDescarga = new List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga>();
                #endregion

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasAnalise, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasAnalise, ref regraMotivoChamado, ref regraFilial, ref regraRegiaoDestino,ref regraCargaDescarga, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras

                for (var i = 0; i < regraMotivoChamado.Count(); i++) repRegrasMotivoChamado.Inserir(regraMotivoChamado[i], Auditado, historico);
                for (var i = 0; i < regraFilial.Count(); i++) repRegrasChamadosFilial.Inserir(regraFilial[i], Auditado, historico);
                for (var i = 0; i < regraRegiaoDestino.Count(); i++) repRegrasRegiaoDestino.Inserir(regraRegiaoDestino[i], Auditado, historico);
                for (var i = 0; i < regraCargaDescarga.Count(); i++) repRegrasCargaDescarga.Inserir(regraCargaDescarga[i]);

                AtualizarAprovadoresChamadosEmAberto(regrasAnalise, listaAprovadoresAnteriores, unitOfWork);

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
                Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasMotivoChamado repRegrasMotivoChamado = new Repositorio.Embarcador.Chamados.RegrasMotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosFilial repRegrasChamadosFilial = new Repositorio.Embarcador.Chamados.RegrasChamadosFilial(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosRegiaoDestino repRegrasChamadosRegiaoDestino = new Repositorio.Embarcador.Chamados.RegrasChamadosRegiaoDestino(unitOfWork);
                Repositorio.Embarcador.Chamados.RegrasChamadosCargaDescarga repChamadosCargaDescarga = new Repositorio.Embarcador.Chamados.RegrasChamadosCargaDescarga(unitOfWork);
                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise = repRegrasAnaliseChamados.BuscarPorCodigo(codigo);

                if (regrasAnalise == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado> regraMotivo = repRegrasMotivoChamado.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial> regraFilial = repRegrasChamadosFilial.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino> regraRegiaoDestino = repRegrasChamadosRegiaoDestino.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga> regraCargaDescarga = repChamadosCargaDescarga.BuscarPorRegras(codigo);
                #endregion


                var dynRegra = new
                {
                    regrasAnalise.Codigo,
                    Vigencia = regrasAnalise.Vigencia.HasValue ? regrasAnalise.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = regrasAnalise.Descricao ?? string.Empty,
                    Observacao = regrasAnalise.Observacoes ?? string.Empty,
                    Aprovadores = (from o in regrasAnalise.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorMotivoChamado = regrasAnalise.RegraPorMotivoChamado,
                    MotivoChamado = (from obj in regraMotivo select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado>(obj, "MotivoChamado", "Descricao")).ToList(),

                    UsarRegraPorFilial = regrasAnalise.RegraPorFilial,
                    Filial = (from obj in regraFilial select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial>(obj, "Filial", "Descricao")).ToList(),

                    UsarRegraPorRegiaoDestino = regrasAnalise.RegraPorRegiaoDestino,
                    RegiaoDestino = (from obj in regraRegiaoDestino select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino>(obj, "Regiao", "Descricao")).ToList(),

                    UsarRegraCargaDescarga = regrasAnalise.RegraPorCargaDescarga,
                    CargaDescarga = (from obj in regraCargaDescarga select RetornaRegraPorTipoDynCargaDescarga<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga>(obj, "ValidarValorCarga", "ValidarValorDescarga")).ToList()
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
                Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise = repRegrasAnaliseChamados.BuscarPorCodigo(codigo);

                if (regrasAnalise == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasAnalise.Aprovadores.Clear();
                regrasAnalise.RegrasMotivoChamado.Clear();
                regrasAnalise.RegrasFilial.Clear();
                regrasAnalise.RegrasRegiaoDestino.Clear();

                repRegrasAnaliseChamados.Deletar(regrasAnalise, Auditado);

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

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            // Converte parametros
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            DateTime? dataVigencia = null;
            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            bool.TryParse(Request.Params("UsarRegraPorMotivoChamado"), out bool usarRegraPorMotivoChamado);
            bool.TryParse(Request.Params("UsarRegraPorFilial"), out bool usarRegraPorFilial);
            bool.TryParse(Request.Params("usarRegraPorRegiaoDestino"), out bool usarRegraPorRegiaoDestino);
            bool.TryParse(Request.Params("UsarRegraCargaDescarga"), out bool usarRegraPorCargaDescarga);

            // Seta na entidade
            regrasAnalise.Descricao = descricao;
            regrasAnalise.Observacoes = observacao;
            regrasAnalise.Vigencia = dataVigencia;

            regrasAnalise.RegraPorMotivoChamado = usarRegraPorMotivoChamado;
            regrasAnalise.RegraPorFilial = usarRegraPorFilial;
            regrasAnalise.RegraPorRegiaoDestino = usarRegraPorRegiaoDestino;
            regrasAnalise.RegraPorCargaDescarga = usarRegraPorCargaDescarga;

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);
            regrasAnalise.Aprovadores = listaAprovadores;
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise, Func<dynamic, object> lambda = null)
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

                prop = regra.GetType().GetProperty("RegrasAnaliseChamados", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasAnalise, null);

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

        private void PreencherRegraCargaDescarga<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise, Func<dynamic, object> lambda = null)
        {
            /* Descricao
             * Processo feito para Carga e Descarga 
             * 
             * Esse último, é instanciado com o retorno do callback, já que é o único parametro que é modificado
             * Mas quando não for uma entidade, mas um valor simples, basta usar a flag usarDynamic = true,
             * Fazendo isso é setado o valor que vem no RegrasPorTipo.Valor
             */

            // Converte json (com o parametro get)

            List<RegrasTipoCargaDescarga> dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegrasTipoCargaDescarga>>(Request.Params(parametroJson));

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

                prop = regra.GetType().GetProperty("RegrasAnaliseChamados", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasAnalise, null);

                prop = regra.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Ordem, null);

                prop = regra.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Condicao, null);

                prop = regra.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Juncao, null);

                prop = regra.GetType().GetProperty("ValidarValorCarga", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].ValidarValorInformadoCarga, null);

                prop = regra.GetType().GetProperty("ValidarValorDescarga", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].ValidarValorInformadoDescarga, null);

                // Adiciona lista de retorno
                regrasPorTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasAnalise.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasAnalise.Aprovadores == null || regrasAnalise.Aprovadores.Count() == 0)
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

        private RegrasTipoCargaDescarga RetornaRegraPorTipoDynCargaDescarga<T>(dynamic obj, string paramentro, string paramentroDescricaoValor, bool usarValor = false)
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

            prop = obj.GetType().GetProperty("ValidarValorCarga", BindingFlags.Public | BindingFlags.Instance);
            bool validarValorInformadoCarga = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("ValidarValorDescarga", BindingFlags.Public | BindingFlags.Instance);
            bool validarValorInformadoDescarga = prop.GetValue(obj);

            RegrasTipoCargaDescarga restorno = new RegrasTipoCargaDescarga()
            {
                Codigo = codigo,
                Ordem = ordem,
                Juncao = juncao,
                Condicao = condicao,
                ValidarValorInformadoCarga = validarValorInformadoCarga,
                ValidarValorInformadoDescarga = validarValorInformadoDescarga,
                Entidade = new ObjetoEntidade()
                {
                    Descricao = validarValorInformadoCarga == true && validarValorInformadoDescarga == true ? " Carga e Descarga " : validarValorInformadoDescarga == true ? " Descarga " : validarValorInformadoCarga == true ? " Carga " : " - ",
                }
            };

            return restorno;
        }

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado> regraMotivoChamado, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial> regraFilial, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino> regraRegiaoDestino, ref List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga> regraCargaDescarga, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region MotivoChamado
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAnalise.RegraPorMotivoChamado)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("MotivoChamado", "RegrasMotivoChamado", false, ref regraMotivoChamado, ref regrasAnalise, ((codigo) =>
                    {
                        Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                        int.TryParse(codigo.ToString(), out int codigoInt);
                        return repMotivoChamado.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Motivo do Chamado");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Motivo do Chamado", "MotivoChamado", regraMotivoChamado, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Filial
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAnalise.RegraPorFilial)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Filial", "RegrasFilial", false, ref regraFilial, ref regrasAnalise, ((codigo) =>
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

            #region Região Destino
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAnalise.RegraPorRegiaoDestino)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Regiao", "RegrasRegiaoDestino", false, ref regraRegiaoDestino, ref regrasAnalise, ((codigo) =>
                    {
                        Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                        int.TryParse(codigo.ToString(), out int codigoInt);
                        return repRegiao.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Regiao");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Regiao de Destino", "Regiao", regraRegiaoDestino, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Carga Descarga
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAnalise.RegraPorCargaDescarga)
            {
                // Preenche regra
                try
                {
                    PreencherRegraCargaDescarga("CargaDescarga", "RegraCargaDescarga", false, ref regraCargaDescarga, ref regrasAnalise);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Regiao");
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
            Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";


            // Consulta
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> listaGrid = repRegrasAnaliseChamados.ConsultarRegras(dataInicio, dataFim, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegrasAnaliseChamados.ContarConsultaRegras(dataInicio, dataFim, descricao);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }

        private void AtualizarAprovadoresChamadosEmAberto(Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regrasAnalise, List<Dominio.Entidades.Usuario> listaAprovadoresAnteriores, Repositorio.UnitOfWork unitOfWork)
        {
            if (listaAprovadoresAnteriores.All(regrasAnalise.Aprovadores.Contains) && listaAprovadoresAnteriores.Count == regrasAnalise.Aprovadores.Count)
                return;

            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repositorioChamado.BuscarChamadosEmAbertoPorRegraAnalise(regrasAnalise.Codigo);
            if (chamados.Count == 0)
                return;

            List<Dominio.Entidades.Usuario> listaAprovadoresRemovidos = listaAprovadoresAnteriores.Where(o => !regrasAnalise.Aprovadores.Contains(o)).ToList();

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
            {
                if (chamado.Analistas.All(regrasAnalise.Aprovadores.Contains) && chamado.Analistas.Count == regrasAnalise.Aprovadores.Count)
                    continue;

                chamado.Initialize();

                foreach (Dominio.Entidades.Usuario aprovador in regrasAnalise.Aprovadores)
                {
                    if (!chamado.Analistas.Contains(aprovador))
                        chamado.Analistas.Add(aprovador);
                }

                //Remove individual, pois ele pode estar em outra regra
                foreach (Dominio.Entidades.Usuario aprovador in listaAprovadoresRemovidos)
                {
                    bool aprovadorEmOutraRegra = chamado.RegrasAnalise.Where(o => o.Codigo != regrasAnalise.Codigo).Any(o => o.Aprovadores.Any(s => s.Codigo == aprovador.Codigo));
                    if (chamado.Analistas.Contains(aprovador) && chamado.Responsavel != aprovador && !aprovadorEmOutraRegra)
                        chamado.Analistas.Remove(aprovador);
                }

                repositorioChamado.Atualizar(chamado);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = chamado.GetChanges();
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, alteracoes, $"Atualizou aprovadores ao atualizar a regra de análise {regrasAnalise.Descricao}", unitOfWork);
            }
        }

        #endregion
    }
}

