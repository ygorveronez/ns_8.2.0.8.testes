using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/RegraFuncionarioComissao")]
    public class RegraFuncionarioComissaoController : BaseController
    {
		#region Construtores

		public RegraFuncionarioComissaoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao repRegraFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao(unitOfWork);
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario repAlcadaFuncionario = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario(unitOfWork);
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaValor repAlcadaValor = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaValor(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao = new Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao();
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario> regraFuncionario = new List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario>();
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor> regraValor = new List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor>();

                // Preenche a entidade
                PreencherEntidade(ref regraFuncionarioComissao, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regraFuncionarioComissao, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regraFuncionarioComissao, ref regraFuncionario, ref regraValor, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegraFuncionarioComissao.Inserir(regraFuncionarioComissao, Auditado);

                // Insere regras
                for (var i = 0; i < regraFuncionario.Count(); i++) repAlcadaFuncionario.Inserir(regraFuncionario[i], Auditado);
                for (var i = 0; i < regraValor.Count(); i++) repAlcadaValor.Inserir(regraValor[i], Auditado);

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
                Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao repRegraFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao(unitOfWork);
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario repAlcadaFuncionario = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario(unitOfWork);
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaValor repAlcadaValor = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaValor(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao = repRegraFuncionarioComissao.BuscarPorCodigo(codigoRegra, true);

                if (regraFuncionarioComissao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario> regraFuncionario = repAlcadaFuncionario.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor> regraValor = repAlcadaValor.BuscarPorRegra(codigoRegra);
                #endregion



                // Preenche a entidade
                PreencherEntidade(ref regraFuncionarioComissao, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regraFuncionarioComissao, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regraFuncionarioComissao, ref regraFuncionario, ref regraValor, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                #region Insere Regras
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                SalvarAlteracaoCriterioDaRegra(regraFuncionarioComissao, regraFuncionario, repAlcadaFuncionario, "Funcionario", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regraFuncionarioComissao, regraValor, repAlcadaValor, "Valor", ref alteracoes, unitOfWork);

                repRegraFuncionarioComissao.Atualizar(regraFuncionarioComissao, Auditado);
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regraFuncionarioComissao, alteracoes, "Alterou os critérios da regra.", unitOfWork);
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
                Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao repRegraFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao(unitOfWork);
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario repAlcadaFuncionario = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario(unitOfWork);
                Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaValor repAlcadaValor = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AlcadaValor(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao = repRegraFuncionarioComissao.BuscarPorCodigo(codigoRegra);

                if (regraFuncionarioComissao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario> regraFuncionario = repAlcadaFuncionario.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor> regraValor = repAlcadaValor.BuscarPorRegra(codigoRegra);
                #endregion


                var dynRegra = new
                {
                    regraFuncionarioComissao.Codigo,
                    regraFuncionarioComissao.NumeroAprovadores,
                    Vigencia = regraFuncionarioComissao.Vigencia.HasValue ? regraFuncionarioComissao.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regraFuncionarioComissao.Descricao) ? regraFuncionarioComissao.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regraFuncionarioComissao.Observacoes) ? regraFuncionarioComissao.Observacoes : string.Empty,

                    Aprovadores = (from o in regraFuncionarioComissao.Aprovadores select new { o.Codigo, o.Nome }).ToList(),


                    UsarRegraPorFuncionario = regraFuncionarioComissao.RegraPorFuncionario,
                    AlcadasFuncionario = (from obj in regraFuncionario select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario>(obj)).ToList(),

                    UsarRegraPorValor = regraFuncionarioComissao.RegraPorValor,
                    AlcadasValor = (from obj in regraValor select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor>(obj, true)).ToList(),
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
                Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao repRegraFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao = repRegraFuncionarioComissao.BuscarPorCodigo(codigo);

                if (regraFuncionarioComissao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regraFuncionarioComissao.Aprovadores.Clear();
                regraFuncionarioComissao.AlcadasFuncionario.Clear();
                regraFuncionarioComissao.AlcadasValor.Clear();

                repRegraFuncionarioComissao.Deletar(regraFuncionarioComissao);

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
        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao, Repositorio.UnitOfWork unitOfWork)
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

            bool.TryParse(Request.Params("UsarRegraPorFuncionario"), out bool usarRegraPorFuncionario);
            bool.TryParse(Request.Params("UsarRegraPorOperador"), out bool usarRegraPorOperador);
            bool.TryParse(Request.Params("UsarRegraPorSetorOperador"), out bool usarRegraPorSetorOperador);
            bool.TryParse(Request.Params("UsarRegraPorProduto"), out bool usarRegraPorProduto);
            bool.TryParse(Request.Params("UsarRegraPorValor"), out bool usarRegraPorValor);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<Dominio.ObjetosDeValor.Embarcador.Alcada.Aprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Alcada.Aprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regraFuncionarioComissao.Descricao = descricao;
            regraFuncionarioComissao.Observacoes = observacao;
            regraFuncionarioComissao.Vigencia = dataVigencia;
            regraFuncionarioComissao.NumeroAprovadores = numeroAprovadores;
            regraFuncionarioComissao.Aprovadores = listaAprovadores;

            regraFuncionarioComissao.RegraPorFuncionario = usarRegraPorFuncionario;
            regraFuncionarioComissao.RegraPorValor = usarRegraPorValor;
            regraFuncionarioComissao.Empresa = this.Usuario.Empresa;
        }

        private void PreencherEntidadeRegra<T>(string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao, Func<dynamic, object> lambda = null) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
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
            List<Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo> dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo>>(Request.Params(parametroJson));

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
                prop = regra.GetType().GetProperty("RegraFuncionarioComissao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regraFuncionarioComissao, null);

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

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regraFuncionarioComissao.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regraFuncionarioComissao.Aprovadores.Count() < regraFuncionarioComissao.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regraFuncionarioComissao.NumeroAprovadores.ToString());

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

        private Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo RetornaRegraPorTipoDyn<T>(T obj, bool usarValor = false) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
        {
            // Variavel auxiliar
            PropertyInfo prop;

            prop = obj.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
            int.TryParse(prop.GetValue(obj).ToString(), out int codigo);

            Dominio.ObjetosDeValor.Embarcador.Alcada.Entidade objetoEntidade = null;
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                dynamic codigoEntidade = prop.GetValue(entidade);

                prop = entidade.GetType().GetProperty("Descricao", BindingFlags.Public | BindingFlags.Instance);
                string descricaoEntidade = prop.GetValue(entidade);

                objetoEntidade = new Dominio.ObjetosDeValor.Embarcador.Alcada.Entidade
                {
                    Codigo = codigoEntidade,
                    Descricao = descricaoEntidade
                };
            }
            else
            {
                prop = obj.GetType().GetProperty("Descricao", BindingFlags.Public | BindingFlags.Instance);
                valor = prop.GetValue(obj);
            }

            Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo retorno = new Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo()
            {
                Codigo = codigo,
                Ordem = obj.Ordem,
                Juncao = obj.Juncao,
                Condicao = obj.Condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return retorno;
        }

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao, ref List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario> regraFuncionario, ref List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor> regraValor, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region Funcionario
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraFuncionarioComissao.RegraPorFuncionario)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasFuncionario", false, ref regraFuncionario, ref regraFuncionarioComissao, ((codigo) =>
                    {
                        Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

                        int.TryParse(codigo.ToString(), out int codigoFuncionario);

                        return repFuncionario.BuscarPorCodigo(codigoFuncionario);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Funcionario");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Funcionario", regraFuncionario, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraFuncionario = new List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaFuncionario>();
            }
            #endregion

            #region Valor
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraFuncionarioComissao.RegraPorValor)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasValor", true, ref regraValor, ref regraFuncionarioComissao);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor", regraValor, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraValor = new List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor>();
            }
            #endregion


        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao repRegraFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Aprovador"), out int codigoAprovador);

            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

            // Consulta
            List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> listaGrid = repRegraFuncionarioComissao.ConsultarRegras(codigoEmpresa, dataInicio, dataFim, aprovador, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegraFuncionarioComissao.ContarConsultaRegras(codigoEmpresa, dataInicio, dataFim, aprovador, descricao);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }

        private void SalvarAlteracaoCriterioDaRegra<T, R>(Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regraFuncionarioComissao, List<T> criterios, R repositorio, string descricao, ref List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, Repositorio.UnitOfWork unitOfWork) where T : Dominio.Entidades.EntidadeBase where R : Repositorio.RepositorioBase<T>
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
                Servicos.Auditoria.Auditoria.Auditar(Auditado, regraFuncionarioComissao, null, "Adicionou um critério de " + descricao + ".", unitOfWork);
        }
        #endregion
    }
}
