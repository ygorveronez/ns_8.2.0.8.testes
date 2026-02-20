using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/RegraContratoFreteTransportador")]
    public class RegraContratoFreteTransportadorController : BaseController
    {
		#region Construtores

		public RegraContratoFreteTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Objetos Json

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
                Repositorio.Embarcador.Frete.RegraContratoFreteTransportador repRegraContratoFreteTransportador = new Repositorio.Embarcador.Frete.RegraContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaTransportador repAlcadaTransportador = new Repositorio.Embarcador.Frete.AlcadaTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaFilial repAlcadaFilial = new Repositorio.Embarcador.Frete.AlcadaFilial(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasContratoValorContrato repRegrasContratoValorContrato = new Repositorio.Embarcador.Frete.RegrasContratoValorContrato(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador = new Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador();
                List<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador> alcadaTransportador = new List<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador>();
                List<Dominio.Entidades.Embarcador.Frete.AlcadaFilial> alcadaFilial = new List<Dominio.Entidades.Embarcador.Frete.AlcadaFilial>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato> regrasContratoValorContratos = new List<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato>();

                // Preenche a entidade
                PreencherEntidade(ref regraContratoFreteTransportador, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regraContratoFreteTransportador, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regraContratoFreteTransportador, ref alcadaTransportador, out List<int> codigosRegrasTransportadorAtulizados, ref alcadaFilial, ref regrasContratoValorContratos, out List<int> codigosRegrasFiliaisAtulizados, out List<int> codigosRegrasContratosValores, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegraContratoFreteTransportador.Inserir(regraContratoFreteTransportador, Auditado);

                // Insere regras
                for (var i = 0; i < alcadaTransportador.Count(); i++) repAlcadaTransportador.Inserir(alcadaTransportador[i], Auditado);
                for (var i = 0; i < alcadaFilial.Count(); i++) repAlcadaFilial.Inserir(alcadaFilial[i], Auditado);
                for (var i = 0; i < regrasContratoValorContratos.Count(); i++) repRegrasContratoValorContrato.Inserir(regrasContratoValorContratos[i]);

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
                Repositorio.Embarcador.Frete.RegraContratoFreteTransportador repRegraContratoFreteTransportador = new Repositorio.Embarcador.Frete.RegraContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaTransportador repAlcadaTransportador = new Repositorio.Embarcador.Frete.AlcadaTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaFilial repAlcadaFilial = new Repositorio.Embarcador.Frete.AlcadaFilial(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasContratoValorContrato repRegrasContratoValorContrato = new Repositorio.Embarcador.Frete.RegrasContratoValorContrato(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador = repRegraContratoFreteTransportador.BuscarPorCodigo(codigoRegra, true);

                if (regraContratoFreteTransportador == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador> alcadaTransportador = repAlcadaTransportador.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.AlcadaFilial> alcadaFilial = repAlcadaFilial.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato> regrasContratoValorContratos = repRegrasContratoValorContrato.BuscarPorRegras(codigoRegra);

                // List<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato> regrasContratoValorContratos = repRegrasContratoValorContrato.BuscarPorRegras(codigoRegra);

                #endregion



                // Preenche a entidade
                PreencherEntidade(ref regraContratoFreteTransportador, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regraContratoFreteTransportador, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                List<int> codigosRegrasTransportadorAtulizados;
                List<int> codigosRegrasFiliaisAtulizados;
                List<int> codigosRegrasContratosValores;
                try
                {
                    PreencherTodasRegras(ref regraContratoFreteTransportador, ref alcadaTransportador, out codigosRegrasTransportadorAtulizados, ref alcadaFilial, ref regrasContratoValorContratos, out codigosRegrasFiliaisAtulizados, out codigosRegrasContratosValores, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                #region Insere Regras
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                SalvarAlteracaoCriterioDaRegra(regraContratoFreteTransportador, alcadaTransportador, repAlcadaTransportador, codigosRegrasTransportadorAtulizados, "Transportador", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regraContratoFreteTransportador, alcadaFilial, repAlcadaFilial, codigosRegrasFiliaisAtulizados, "Filial", ref alteracoes, unitOfWork);



                SalvarAlteracaoCriterioDaRegra(regraContratoFreteTransportador, regrasContratoValorContratos, repRegrasContratoValorContrato, codigosRegrasContratosValores, "Regras de Valor do Contrato", ref alteracoes, unitOfWork);

                repRegraContratoFreteTransportador.Atualizar(regraContratoFreteTransportador, Auditado);
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regraContratoFreteTransportador, alteracoes, "Alterou os critérios da regra.", unitOfWork);
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
                Repositorio.Embarcador.Frete.RegraContratoFreteTransportador repRegraContratoFreteTransportador = new Repositorio.Embarcador.Frete.RegraContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaTransportador repAlcadaTransportador = new Repositorio.Embarcador.Frete.AlcadaTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaFilial repAlcadaFilial = new Repositorio.Embarcador.Frete.AlcadaFilial(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasContratoValorContrato repRegrasContratoValorContrato = new Repositorio.Embarcador.Frete.RegrasContratoValorContrato(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador = repRegraContratoFreteTransportador.BuscarPorCodigo(codigoRegra);

                if (regraContratoFreteTransportador == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador> alcadaTransportador = repAlcadaTransportador.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.AlcadaFilial> alcadaFilial = repAlcadaFilial.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato> regrasContratoValorContratos = repRegrasContratoValorContrato.BuscarPorRegras(codigoRegra);
                #endregion

                List<Dominio.Entidades.Usuario> aprovadores = (regraContratoFreteTransportador.TipoAprovadorRegra == TipoAprovadorRegra.Usuario) ? regraContratoFreteTransportador.Aprovadores.ToList() : new List<Dominio.Entidades.Usuario>();

                var dynRegra = new
                {
                    regraContratoFreteTransportador.Codigo,
                    regraContratoFreteTransportador.NumeroAprovadores,
                    regraContratoFreteTransportador.PrioridadeAprovacao,
                    regraContratoFreteTransportador.Ativo,
                    regraContratoFreteTransportador.TipoAprovadorRegra,
                    Vigencia = regraContratoFreteTransportador.Vigencia.HasValue ? regraContratoFreteTransportador.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regraContratoFreteTransportador.Descricao) ? regraContratoFreteTransportador.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regraContratoFreteTransportador.Observacoes) ? regraContratoFreteTransportador.Observacoes : string.Empty,

                    Aprovadores = (from o in aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorTransportador = regraContratoFreteTransportador.RegraPorTransportador,
                    Transportador = (from obj in alcadaTransportador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador>(obj)).ToList(),

                    UsarRegraPorFilial = regraContratoFreteTransportador.RegraPorFilial,
                    Filial = (from obj in alcadaFilial select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.AlcadaFilial>(obj)).ToList(),

                    UsarRegraPorValorContrato = regraContratoFreteTransportador.RegraPorValorContrato,
                    RegrasValorContrato = (from obj in regrasContratoValorContratos select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato>(obj, true)).ToList(),
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
                Repositorio.Embarcador.Frete.RegraContratoFreteTransportador repRegraContratoFreteTransportador = new Repositorio.Embarcador.Frete.RegraContratoFreteTransportador(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador = repRegraContratoFreteTransportador.BuscarPorCodigo(codigo);

                if (regraContratoFreteTransportador == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regraContratoFreteTransportador.Aprovadores.Clear();
                regraContratoFreteTransportador.AlcadasTransportadores.Clear();
                regraContratoFreteTransportador.RegrasContratoValorContrato.Clear();
                regraContratoFreteTransportador.AlcadasFilial.Clear();

                repRegraContratoFreteTransportador.Deletar(regraContratoFreteTransportador);

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
            grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

            return grid;
        }

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador, Repositorio.UnitOfWork unitOfWork)
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
            int.TryParse(Request.Params("PrioridadeAprovacao"), out int prioridadeAprovacao);

            bool.TryParse(Request.Params("UsarRegraPorTransportador"), out bool usarRegraPorTransportador);
            bool.TryParse(Request.Params("UsarRegraPorFilial"), out bool usarRegraPorFilial);
            bool.TryParse(Request.Params("UsarRegraPorValorContrato"), out bool usarRegraPorValorContrato);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }

            

            bool ativo = true;
            bool.TryParse(Request.Params("Ativo"), out ativo);
            // Seta na entidade
            regraContratoFreteTransportador.Descricao = descricao;
            regraContratoFreteTransportador.Observacoes = observacao;
            regraContratoFreteTransportador.Vigencia = dataVigencia;
            regraContratoFreteTransportador.NumeroAprovadores = numeroAprovadores;
            regraContratoFreteTransportador.PrioridadeAprovacao = prioridadeAprovacao;
            regraContratoFreteTransportador.RegraPorValorContrato = usarRegraPorValorContrato;
            regraContratoFreteTransportador.Ativo = ativo;
            regraContratoFreteTransportador.RegraPorTransportador = usarRegraPorTransportador;
            regraContratoFreteTransportador.RegraPorFilial = usarRegraPorFilial;
            regraContratoFreteTransportador.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
            regraContratoFreteTransportador.Aprovadores = new List<Dominio.Entidades.Usuario>();

            if (regraContratoFreteTransportador.TipoAprovadorRegra == TipoAprovadorRegra.Usuario)
                regraContratoFreteTransportador.Aprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);
        }

        private List<int> PreencherEntidadeRegra<T>(string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador, Func<dynamic, object> lambda = null) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
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
            List<int> codigosRegrasAtulizados = new List<int>();

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
                            codigosRegrasAtulizados.Add(codigoRegra);
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
                prop = regra.GetType().GetProperty("RegraContratoFreteTransportador", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regraContratoFreteTransportador, null);

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

            return codigosRegrasAtulizados;
        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regraContratoFreteTransportador.Descricao))
                erros.Add("Descrição é obrigatória.");

            if ((regraContratoFreteTransportador.TipoAprovadorRegra == TipoAprovadorRegra.Usuario) && (regraContratoFreteTransportador.Aprovadores.Count() < regraContratoFreteTransportador.NumeroAprovadores))
                erros.Add($"O número de aprovadores selecionados deve ser maior ou igual a {regraContratoFreteTransportador.NumeroAprovadores}");

            if (regraContratoFreteTransportador.Vigencia.HasValue && regraContratoFreteTransportador.Vigencia.Value < DateTime.Now.Date && regraContratoFreteTransportador.Ativo)
                erros.Add("A data da vigência precisa ser superior a data atual.");

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

        private void PreencherTodasRegras(
            ref Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador,

            ref List<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador> alcadaTransportador,
            out List<int> codigosRegrasTransportadorAtulizados,
            ref List<Dominio.Entidades.Embarcador.Frete.AlcadaFilial> alcadaFilial,
            ref List<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato> regrasContratoValorContratos,
            out List<int> codigosRegrasFiliaisAtulizados,
            out List<int> codigosRegrasContratosValores,
            ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            codigosRegrasTransportadorAtulizados = new List<int>();
            codigosRegrasFiliaisAtulizados = new List<int>();
            codigosRegrasContratosValores = new List<int>();

            #region Transportador
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraContratoFreteTransportador.RegraPorTransportador)
            {
                // Preenche regra
                try
                {
                    codigosRegrasTransportadorAtulizados = PreencherEntidadeRegra("AlcadasTransportador", false, ref alcadaTransportador, ref regraContratoFreteTransportador, ((codigo) =>
                    {
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                        int.TryParse(codigo.ToString(), out int codigoInt);

                        return repEmpresa.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Transportador");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Transportador", alcadaTransportador, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                alcadaTransportador = new List<Dominio.Entidades.Embarcador.Frete.AlcadaTransportador>();
            }
            #endregion

            #region Filial
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraContratoFreteTransportador.RegraPorFilial)
            {
                // Preenche regra
                try
                {
                    codigosRegrasFiliaisAtulizados = PreencherEntidadeRegra("AlcadasFilial", false, ref alcadaFilial, ref regraContratoFreteTransportador, ((codigo) =>
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
                if (!ValidarEntidadeRegra("Filial", alcadaFilial, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                alcadaFilial = new List<Dominio.Entidades.Embarcador.Frete.AlcadaFilial>();
            }
            #endregion

            #region ValorContrato

            if (regraContratoFreteTransportador.RegraPorValorContrato)
            {
                // Preenche regra
                try
                {
                    codigosRegrasContratosValores = PreencherEntidadeRegra("RegrasValorContrato", true, ref regrasContratoValorContratos, ref regraContratoFreteTransportador, ((valor) =>
                    {
                        decimal.TryParse(valor.ToString(), out decimal valorContrato);
                        return valorContrato;
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("RegrasValorContrato");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("RegrasValorContrato", regrasContratoValorContratos, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasContratoValorContratos = new List<Dominio.Entidades.Embarcador.Frete.RegrasContratoValorContrato>();
            }

            #endregion

        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frete.RegraContratoFreteTransportador repRegraContratoFreteTransportador = new Repositorio.Embarcador.Frete.RegraContratoFreteTransportador(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Aprovador"), out int codigoAprovador);

            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;


            Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

            // Consulta
            List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> listaGrid = repRegraContratoFreteTransportador.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, ativo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegraContratoFreteTransportador.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao, ativo);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             obj.DescricaoAtivo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }

        private void SalvarAlteracaoCriterioDaRegra<T, R>(Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador regraContratoFreteTransportador, List<T> criterios, R repositorio, List<int> codigosRegrasAtulizados, string descricao, ref List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, Repositorio.UnitOfWork unitOfWork) where T : Dominio.Entidades.EntidadeBase where R : Repositorio.RepositorioBase<T>
        {
            bool inseriuCriterio = false;
            bool deletouCriterio = false;
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
                else if (codigosRegrasAtulizados.Contains(codigo))
                {
                    alteracoes.AddRange(criterios[i].GetChanges());
                    repositorio.Atualizar(criterios[i]);
                }
                else
                {
                    repositorio.Deletar(criterios[i]);
                    deletouCriterio = true;
                }
            }

            if (inseriuCriterio)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, regraContratoFreteTransportador, null, "Adicionou um critério de " + descricao + ".", unitOfWork);

            if (deletouCriterio)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, regraContratoFreteTransportador, null, "Exclui um critério de " + descricao + ".", unitOfWork);
        }
        
        #endregion
    }
}

