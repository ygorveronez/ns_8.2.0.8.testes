using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;


namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/RegraControleReajusteFretePlanilha")]
    public class RegraControleReajusteFretePlanilhaController : BaseController
    {
		#region Construtores

		public RegraControleReajusteFretePlanilhaController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha repRegraControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao repAlcadaReajusteFreteTipoOperacao = new Repositorio.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaReajusteFreteFilial repAlcadaReajusteFreteFilial = new Repositorio.Embarcador.Frete.AlcadaReajusteFreteFilial(unitOfWork);
                // Nova entidade
                Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha = new Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha();
                List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao> alcadaTipoOperacao = new List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao>();
                List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial> alcadaFilial = new List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial>();

                // Preenche a entidade
                PreencherEntidade(ref regraControleReajusteFretePlanilha, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regraControleReajusteFretePlanilha, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regraControleReajusteFretePlanilha, ref alcadaTipoOperacao, ref alcadaFilial, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegraControleReajusteFretePlanilha.Inserir(regraControleReajusteFretePlanilha, Auditado);

                // Insere regras
                for (var i = 0; i < alcadaTipoOperacao.Count(); i++) repAlcadaReajusteFreteTipoOperacao.Inserir(alcadaTipoOperacao[i], Auditado);
                for (var i = 0; i < alcadaFilial.Count(); i++) repAlcadaReajusteFreteFilial.Inserir(alcadaFilial[i], Auditado);

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
                Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha repRegraControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao repAlcadaReajusteFreteTipoOperacao = new Repositorio.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaReajusteFreteFilial repAlcadaReajusteFreteFilial = new Repositorio.Embarcador.Frete.AlcadaReajusteFreteFilial(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha = repRegraControleReajusteFretePlanilha.BuscarPorCodigo(codigoRegra, true);

                if (regraControleReajusteFretePlanilha == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao> alcadaTipoOperacao = repAlcadaReajusteFreteTipoOperacao.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial> alcadaFilial = repAlcadaReajusteFreteFilial.BuscarPorRegra(codigoRegra);
                #endregion



                // Preenche a entidade
                PreencherEntidade(ref regraControleReajusteFretePlanilha, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regraControleReajusteFretePlanilha, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regraControleReajusteFretePlanilha, ref alcadaTipoOperacao, ref alcadaFilial, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                #region Insere Regras
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                SalvarAlteracaoCriterioDaRegra(regraControleReajusteFretePlanilha, alcadaTipoOperacao, repAlcadaReajusteFreteTipoOperacao, "Tipo de Operação", ref alteracoes, unitOfWork);

                SalvarAlteracaoCriterioDaRegra(regraControleReajusteFretePlanilha, alcadaFilial, repAlcadaReajusteFreteFilial, "Filiar", ref alteracoes, unitOfWork);

                repRegraControleReajusteFretePlanilha.Atualizar(regraControleReajusteFretePlanilha, Auditado);
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regraControleReajusteFretePlanilha, alteracoes, "Alterou os critérios da regra.", unitOfWork);
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
                Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha repRegraControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao repAlcadaReajusteFreteTipoOperacao = new Repositorio.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaReajusteFreteFilial repAlcadaReajusteFreteFilial = new Repositorio.Embarcador.Frete.AlcadaReajusteFreteFilial(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha = repRegraControleReajusteFretePlanilha.BuscarPorCodigo(codigoRegra);

                if (regraControleReajusteFretePlanilha == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao> alcadaTipoOperacao = repAlcadaReajusteFreteTipoOperacao.BuscarPorRegra(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial> alcadaFilial = repAlcadaReajusteFreteFilial.BuscarPorRegra(codigoRegra);
                #endregion


                var dynRegra = new
                {
                    regraControleReajusteFretePlanilha.Codigo,
                    regraControleReajusteFretePlanilha.NumeroAprovadores,
                    Vigencia = regraControleReajusteFretePlanilha.Vigencia.HasValue ? regraControleReajusteFretePlanilha.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regraControleReajusteFretePlanilha.Descricao) ? regraControleReajusteFretePlanilha.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regraControleReajusteFretePlanilha.Observacoes) ? regraControleReajusteFretePlanilha.Observacoes : string.Empty,

                    Aprovadores = (from o in regraControleReajusteFretePlanilha.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorTipoOperacao = regraControleReajusteFretePlanilha.RegraPorTipoOperacao,
                    TipoOperacao = (from obj in alcadaTipoOperacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao>(obj)).ToList(),

                    UsarRegraPorFilial = regraControleReajusteFretePlanilha.RegraPorFilial,
                    Filial = (from obj in alcadaFilial select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial>(obj)).ToList()
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
                Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha repRegraControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha = repRegraControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                if (regraControleReajusteFretePlanilha == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regraControleReajusteFretePlanilha.Aprovadores.Clear();
                regraControleReajusteFretePlanilha.AlcadasTipoOperacao.Clear();
                regraControleReajusteFretePlanilha.AlcadasFilial.Clear();

                repRegraControleReajusteFretePlanilha.Deletar(regraControleReajusteFretePlanilha);

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
        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha, Repositorio.UnitOfWork unitOfWork)
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

            bool.TryParse(Request.Params("UsarRegraPorTipoOperacao"), out bool usarRegraPorTipoOperacao);
            bool.TryParse(Request.Params("UsarRegraPorFilial"), out bool usarRegraPorFilial);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regraControleReajusteFretePlanilha.Descricao = descricao;
            regraControleReajusteFretePlanilha.Observacoes = observacao;
            regraControleReajusteFretePlanilha.Vigencia = dataVigencia;
            regraControleReajusteFretePlanilha.NumeroAprovadores = numeroAprovadores;
            regraControleReajusteFretePlanilha.Aprovadores = listaAprovadores;

            regraControleReajusteFretePlanilha.RegraPorTipoOperacao = usarRegraPorTipoOperacao;
            regraControleReajusteFretePlanilha.RegraPorFilial = usarRegraPorFilial;
        }

        private void PreencherEntidadeRegra<T>(string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha, Repositorio.UnitOfWork unitOfWork ,Func<dynamic, object> lambda = null) where T : Dominio.Entidades.Embarcador.Alcada.Alcada
        {
            // Converte json (com o parametro get)
            List<RegrasPorTipo> dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegrasPorTipo>>(Request.Params(parametroJson));
            Repositorio.RepositorioBase<T> repositorioBase = new Repositorio.RepositorioBase<T>(unitOfWork);

            if (dynRegras == null)
                throw new Exception("Erro ao converter os dados recebidos.");

            // Variavel auxiliar
            PropertyInfo prop;

            // Criar uma lista auxiliar para marcar itens que precisam ser removidos
            List<T> itensParaRemover = new List<T>(regrasPorTipo);

            // Itera retornos
            foreach (var dynRegra in dynRegras)
            {
                int.TryParse(dynRegra.Codigo.ToString(), out int codigoRegra);
                int indexRegraNaLista = -1;

                // Procura pelo código na lista existente
                for (int j = 0; j < regrasPorTipo.Count; j++)
                {
                    if ((int)((dynamic)regrasPorTipo[j]).Codigo == codigoRegra)
                    {
                        indexRegraNaLista = j;
                        break;
                    }
                }

                T regra;

                if (indexRegraNaLista >= 0)
                {
                    // Item encontrado na lista existente, atualiza e remove da lista de remoção
                    regra = regrasPorTipo[indexRegraNaLista];
                    itensParaRemover.Remove(regra);
                    regra.Initialize();
                }
                else
                {
                    // Item não encontrado na lista existente, cria um novo
                    regra = Activator.CreateInstance<T>();
                    regrasPorTipo.Add(regra);
                }

                // Setar as propriedades da entidade
                prop = regra.GetType().GetProperty("RegraControleReajusteFretePlanilha", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regraControleReajusteFretePlanilha, null);

                regra.Ordem = dynRegra.Ordem;
                regra.Condicao = dynRegra.Condicao;
                regra.Juncao = dynRegra.Juncao;

                if (!usarDynamic)
                {
                    // Executa lambda
                    var result = dynRegra.Entidade != null ? lambda(dynRegra.Entidade.Codigo) : null;

                    prop = regra.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(regra, result, null);
                }
                else
                {
                    prop = regra.GetType().GetProperty("PropriedadeAlcada", BindingFlags.Public | BindingFlags.Instance);
                    if (prop.PropertyType.Name.Equals("Decimal"))
                    {
                        decimal.TryParse(dynRegra.Valor.ToString(), out decimal valorDecimal);
                        prop.SetValue(regra, valorDecimal, null);
                    }
                    else
                    {
                        prop.SetValue(regra, dynRegra.Valor, null);
                    }
                }
            }

            // Remover itens que estão em regrasPorTipo mas não estão em dynRegras
            foreach (var itemParaRemover in itensParaRemover)
            {
                repositorioBase.Deletar(itemParaRemover);
                regrasPorTipo.Remove(itemParaRemover);
            }
        }


        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regraControleReajusteFretePlanilha.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regraControleReajusteFretePlanilha.Aprovadores.Count() < regraControleReajusteFretePlanilha.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regraControleReajusteFretePlanilha.NumeroAprovadores.ToString());

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

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha, ref List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao> alcadaTipoOperacao, ref List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial> alcadaFilial, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region TipoOperacao
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraControleReajusteFretePlanilha.RegraPorTipoOperacao)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasTipoOperacao", false, ref alcadaTipoOperacao, ref regraControleReajusteFretePlanilha, unitOfWork, ((codigo) => {
                        Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                        int.TryParse(codigo.ToString(), out int codigoInt);

                        return repTipoOperacao.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e); 
                    errosRegras.Add("Tipo de Operação");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Tipo de Operação", alcadaTipoOperacao, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                alcadaTipoOperacao = new List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao>();
            }
            #endregion

            #region Filial
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraControleReajusteFretePlanilha.RegraPorFilial)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("AlcadasFilial", false, ref alcadaFilial, ref regraControleReajusteFretePlanilha, unitOfWork, ((codigo) => {
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
                alcadaFilial = new List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial>();
            }
            #endregion
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha repRegraControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.RegraControleReajusteFretePlanilha(unitOfWork);
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
            List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> listaGrid = repRegraControleReajusteFretePlanilha.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegraControleReajusteFretePlanilha.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }

        private void SalvarAlteracaoCriterioDaRegra<T, R>(Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regraControleReajusteFretePlanilha, List<T> criterios, R repositorio, string descricao, ref List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes, Repositorio.UnitOfWork unitOfWork) where T : Dominio.Entidades.EntidadeBase where R : Repositorio.RepositorioBase<T>
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
                Servicos.Auditoria.Auditoria.Auditar(Auditado, regraControleReajusteFretePlanilha, null, "Adicionou um critério de " + descricao + ".", unitOfWork);
        }
        #endregion
    }
}

