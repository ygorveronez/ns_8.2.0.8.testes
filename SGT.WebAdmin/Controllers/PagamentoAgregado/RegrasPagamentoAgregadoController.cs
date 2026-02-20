using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.PagamentoAgregado
{
    [CustomAuthorize("PagamentosAgregados/RegrasPagamentoAgregado")]
    public class RegrasPagamentoAgregadoController : BaseController
    {
		#region Construtores

		public RegrasPagamentoAgregadoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado repRegraPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado repAlcadaPessoaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado repAlcadaValorPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte = new Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado();
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado> regraPessoa = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado>();                
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado> regraValor = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado>();

                // Preenche a entidade
                PreencherEntidade(ref regrasDescarte, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasDescarte, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasDescarte, ref regraPessoa, ref regraValor, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegraPagamentoAgregado.Inserir(regrasDescarte, Auditado);
                              
                
                // Insere regras
                for (var i = 0; i < regraPessoa.Count(); i++) repAlcadaPessoaPagamentoAgregado.Inserir(regraPessoa[i], Auditado);
                for (var i = 0; i < regraValor.Count(); i++) repAlcadaValorPagamentoAgregado.Inserir(regraValor[i], Auditado);

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
                Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado repRegraPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado repAlcadaPessoaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado(unitOfWork);                
                Repositorio.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado repAlcadaValorPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte = repRegraPagamentoAgregado.BuscarPorCodigo(codigoRegra, true);

                if (regrasDescarte == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado> regraPessoa = repAlcadaPessoaPagamentoAgregado.BuscarPorRegra(codigoRegra);                
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado> regraValor = repAlcadaValorPagamentoAgregado.BuscarPorRegra(codigoRegra);
                #endregion

                // Preenche a entidade
                PreencherEntidade(ref regrasDescarte, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasDescarte, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasDescarte, ref regraPessoa, ref regraValor, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                #region Insere Regras
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                SalvarAlteracaoCriterioDaRegra(regrasDescarte, regraPessoa, repAlcadaPessoaPagamentoAgregado, "Agregado", ref alteracoes, unitOfWork);                
                SalvarAlteracaoCriterioDaRegra(regrasDescarte, regraValor, repAlcadaValorPagamentoAgregado, "Valor", ref alteracoes, unitOfWork);

                repRegraPagamentoAgregado.Atualizar(regrasDescarte, Auditado);
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasDescarte, alteracoes, "Alterou os critérios da regra.", unitOfWork);
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
                Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado repRegraPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado repAlcadaPessoaPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado(unitOfWork);                
                Repositorio.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado repAlcadaValorPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte = repRegraPagamentoAgregado.BuscarPorCodigo(codigoRegra);

                if (regrasDescarte == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado> regraPessoa = repAlcadaPessoaPagamentoAgregado.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado> regraValor = repAlcadaValorPagamentoAgregado.BuscarPorRegra(codigoRegra);
                #endregion


                var dynRegra = new
                {
                    regrasDescarte.Codigo,
                    regrasDescarte.NumeroAprovadores,
                    Vigencia = regrasDescarte.Vigencia.HasValue ? regrasDescarte.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasDescarte.Descricao) ? regrasDescarte.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasDescarte.Observacoes) ? regrasDescarte.Observacoes : string.Empty,

                    Aprovadores = (from o in regrasDescarte.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorCliente = regrasDescarte.RegraPorCliente,
                    Cliente = (from obj in regraPessoa select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado>(obj)).ToList(),                    

                    UsarRegraPorValor = regrasDescarte.RegraPorValor,
                    Valor = (from obj in regraValor select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado>(obj, true)).ToList()
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
                Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado repRegraPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte = repRegraPagamentoAgregado.BuscarPorCodigo(codigo);

                if (regrasDescarte == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasDescarte.Aprovadores.Clear();
                regrasDescarte.AlcadasPessoa.Clear();
                regrasDescarte.AlcadasValor.Clear();

                repRegraPagamentoAgregado.Deletar(regrasDescarte);

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
        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte, Repositorio.UnitOfWork unitOfWork)
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

            bool.TryParse(Request.Params("UsarRegraPorCliente"), out bool usarRegraPorCliente);
            bool.TryParse(Request.Params("UsarRegraPorValor"), out bool usarRegraPorValor);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regrasDescarte.Descricao = descricao;
            regrasDescarte.Observacoes = observacao;
            regrasDescarte.Vigencia = dataVigencia;
            regrasDescarte.NumeroAprovadores = numeroAprovadores;
            regrasDescarte.Aprovadores = listaAprovadores;

            regrasDescarte.RegraPorCliente = usarRegraPorCliente;
            regrasDescarte.RegraPorValor = usarRegraPorValor;
        }

        private void PreencherEntidadeRegra<T>(string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte, Func<dynamic, object> lambda = null) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
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
                prop = regra.GetType().GetProperty("RegraPagamentoAgregado", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasDescarte, null);

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

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasDescarte.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasDescarte.Aprovadores.Count() < regrasDescarte.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regrasDescarte.NumeroAprovadores.ToString());

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

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte, ref List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado> regraPessoa, ref List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado> regraValor, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region Cliente
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasDescarte.RegraPorCliente)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasCliente", false, ref regraPessoa, ref regrasDescarte, ((codigo) => {
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        double.TryParse(codigo.ToString(), out double codigoInt);

                        return repCliente.BuscarPorCPFCNPJ(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Agregado");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Agregado", regraPessoa, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraPessoa = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado>();
            }
            #endregion

            #region Valor
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasDescarte.RegraPorValor)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasValor", true, ref regraValor, ref regrasDescarte);
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
                regraValor = new List<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado>();
            }
            #endregion
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado repRegraPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.RegraPagamentoAgregado(unitOfWork);
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
            List<Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado> listaGrid = repRegraPagamentoAgregado.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegraPagamentoAgregado.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }

        private void SalvarAlteracaoCriterioDaRegra<T, R>(Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado regrasDescarte, List<T> criterios, R repositorio, string descricao, ref List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, Repositorio.UnitOfWork unitOfWork) where T : Dominio.Entidades.EntidadeBase where R : Repositorio.RepositorioBase<T>
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
                Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasDescarte, null, "Adicionou um critério de " + descricao + ".", unitOfWork);
        }
        #endregion
    }
}

