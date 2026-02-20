using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/LinhaSeparacao")]
    public class LinhaSeparacaoController : BaseController
    {
		#region Construtores

		public LinhaSeparacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAgrupa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoLinhaSeparacao);

                var agrupamentos = new Repositorio.Embarcador.Pedidos.LinhaSeparacaoAgrupa(unitOfWork).BuscarAgrupamentos(codigoLinhaSeparacao);

                var lista = (from p in agrupamentos
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao
                             }).ToList();

                return new JsonpResult(lista);
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
        public async Task<IActionResult> Agrupamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Filial"), out int codigoFilial);

                Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> linhas = repLinhaSeparacao.Consultar(string.Empty, string.Empty, codigoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, "Descricao", "asc", 0, 0);

                linhas = linhas.FindAll(x => x.Filial?.Codigo == codigoFilial).ToList();

                int size = 20;
                if (linhas?.Count > 20)
                    size = 8;
                else if (linhas?.Count > 15)
                    size = 10;
                else if (linhas?.Count > 10)
                    size = 12;
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("", "Descricao", 50, Models.Grid.Align.left);

                int sizeColumn = (int)((100 - size) / (linhas.Count == 0 ? 1 : linhas.Count));

                string css = (sizeColumn >= 8 ? string.Empty : "rotate-270");

                if (sizeColumn < 5)
                {
                    sizeColumn = 10;
                    css = "rotate-27025";
                }


                foreach (var linha in linhas)
                    grid.AdicionarCabecalho(linha.Descricao, "LS_" + linha.Codigo.ToString(), sizeColumn, Models.Grid.Align.center, false, css).Editable(new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aBool));

                List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> agrupamentos = new Repositorio.Embarcador.Pedidos.LinhaSeparacaoAgrupa(unitOfWork).BuscarTodos(codigoFilial);

                var rows = (
                    from p in linhas
                    select new
                    {
                        p.Codigo,
                        p.Descricao
                    }).ToList();

                grid.AdicionaRows(rows);

                for (int i = 0; i < grid.data.Count; i++)
                {
                    int codigo = int.Parse(grid.data[i]["Codigo"]);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linha in linhas)
                    {
                        if (codigo == linha.Codigo)
                            grid.data[i]["LS_" + linha.Codigo.ToString()] = true;
                        else
                        {
                            bool existe = agrupamentos.Exists(x => x.LinhaSeparacaoUm.Codigo == codigo && x.LinhaSeparacaoDois.Codigo == linha.Codigo ||
                                                                   x.LinhaSeparacaoUm.Codigo == linha.Codigo && x.LinhaSeparacaoDois.Codigo == codigo);
                            grid.data[i]["LS_" + linha.Codigo.ToString()] = existe;
                        }
                    }
                }

                grid.setarQuantidadeTotal(rows?.Count() ?? 0);

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

        public async Task<IActionResult> SalvarAgrupamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLS1 = Request.GetIntParam("CodigoLS1");
                int codigoLS2 = Request.GetIntParam("CodigoLS2");
                bool agrupa = Request.GetBoolParam("Agrupa");

                Repositorio.Embarcador.Pedidos.LinhaSeparacaoAgrupa repositorioLinhaSeparacaoAgrupa = new Repositorio.Embarcador.Pedidos.LinhaSeparacaoAgrupa(unitOfWork);
                // Consulta todos os agrupamentos da linha 01
                List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> agrupamentos = repositorioLinhaSeparacaoAgrupa.BuscarAgrupamentos(codigoLS1);
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao agrupamento = agrupamentos.Find(x => x.Codigo == codigoLS2);
                //Se existe exclui o agrupamento
                if (agrupamento != null && !agrupa)
                    repositorioLinhaSeparacaoAgrupa.ExcluirAgrupamentos(codigoLS1, new List<int>() { codigoLS2 });
                else if (agrupamento == null && agrupa)
                {
                    Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa item = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa();
                    item.LinhaSeparacaoUm = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao() { Codigo = codigoLS1 };
                    item.LinhaSeparacaoDois = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao() { Codigo = codigoLS2 };
                    repositorioLinhaSeparacaoAgrupa.Inserir(item);
                }

                //Agora vamos setar as linhas de separação já para agrupadas...
                Repositorio.Embarcador.Pedidos.LinhaSeparacao repositorioLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linha = repositorioLinhaSeparacao.BuscarPorCodigo(codigoLS1);
                if (!linha.ValidadoAgrupamentos)
                {
                    linha.ValidadoAgrupamentos = true;
                    repositorioLinhaSeparacao.Atualizar(linha);
                }

                linha = repositorioLinhaSeparacao.BuscarPorCodigo(codigoLS2);
                if (!linha.ValidadoAgrupamentos)
                {
                    linha.ValidadoAgrupamentos = true;
                    repositorioLinhaSeparacao.Atualizar(linha);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a montagem por pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> LinhasSemValidacaoAgrupamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFilial = Request.GetIntParam("Filial");

                var linhas = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork).ValidadoAgrupamentos(codigoFilial, false);

                var lista = (from p in linhas
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao
                             }).ToList();

                return new JsonpResult(lista);
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
        public async Task<IActionResult> SalvarAgrupa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Pedidos.LinhaSeparacaoAgrupa repAgrupa = new Repositorio.Embarcador.Pedidos.LinhaSeparacaoAgrupa(unitOfWork);
                Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinha = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

                var linha = repLinha.BuscarPorCodigo(codigo);
                var agrupamentos = repAgrupa.BuscarAgrupamentos(codigo);

                dynamic linhas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Agrupar"));

                List<int> codigos = new List<int>();
                // Inicia transacao
                unitOfWork.Start();

                foreach (var dynLinhaSeparacao in linhas)
                {
                    int codigoDois = (int)dynLinhaSeparacao.Codigo;
                    if (!agrupamentos.Exists(um => um.Codigo == codigoDois) && codigo != codigoDois)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa agrupa = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa();
                        agrupa.LinhaSeparacaoUm = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao() { Codigo = codigo };
                        agrupa.LinhaSeparacaoDois = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao() { Codigo = codigoDois };
                        repAgrupa.Inserir(agrupa);
                    }
                    if (codigo != codigoDois)
                        codigos.Add(codigoDois);
                }

                var remover = agrupamentos.FindAll(x => !codigos.Contains(x.Codigo));
                if (remover?.Count > 0)
                    repAgrupa.ExcluirAgrupamentos(codigo, (from r in remover select r.Codigo).ToList());

                //Atualizando a flag de linha de separação já parametricada os agrupamentso
                linha.ValidadoAgrupamentos = true;
                repLinha.Atualizar(linha);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma salvar o agrupamento.");
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
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linha = repLinhaSeparacao.BuscarPorCodigo(codigo);

                // Valida
                if (linha == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    linha.Codigo,
                    linha.CodigoIntegracao,
                    linha.Descricao,
                    linha.Ativo,
                    Observacao = linha.Observacao ?? string.Empty,
                    linha.Roteiriza,
                    linha.NivelPrioridade,
                    Filial = new
                    {
                        Codigo = linha.Filial?.Codigo ?? 0,
                        Descricao = linha.Filial?.Descricao ?? ""
                    }
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao LinhaSeparacao = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao();


                // Preenche entidade com dados
                PreencheEntidade(ref LinhaSeparacao, unitOfWork);

                if (!string.IsNullOrWhiteSpace(LinhaSeparacao.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao LinhaSeparacaoExiste = repLinhaSeparacao.BuscarPorCodigoIntegracao(LinhaSeparacao.CodigoIntegracao);
                    if (LinhaSeparacaoExiste != null)
                        return new JsonpResult(false, true, "Já existe um canal de entrega cadastrado com o código de integração ." + LinhaSeparacao.CodigoIntegracao + ".");
                }

                // Valida entidade
                string erro;
                if (!ValidaEntidade(LinhaSeparacao, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repLinhaSeparacao.Inserir(LinhaSeparacao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao LinhaSeparacao = repLinhaSeparacao.BuscarPorCodigo(codigo, true);

                // Valida
                if (LinhaSeparacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref LinhaSeparacao, unitOfWork);
                if (!string.IsNullOrWhiteSpace(LinhaSeparacao.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao LinhaSeparacaoExiste = repLinhaSeparacao.BuscarPorCodigoIntegracao(LinhaSeparacao.CodigoIntegracao);
                    if (LinhaSeparacaoExiste != null && LinhaSeparacaoExiste.Codigo != LinhaSeparacao.Codigo)
                        return new JsonpResult(false, true, "Já existe um canal de entrega cadastrado com o código de integração ." + LinhaSeparacao.CodigoIntegracao + ".");

                }
                // Valida entidade
                string erro;
                if (!ValidaEntidade(LinhaSeparacao, out erro))
                    return new JsonpResult(false, true, erro);

                // Inicia transacao
                unitOfWork.Start();

                // Persiste dados
                repLinhaSeparacao.Atualizar(LinhaSeparacao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao LinhaSeparacao = repLinhaSeparacao.BuscarPorCodigo(codigo);

                // Valida
                if (LinhaSeparacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repLinhaSeparacao.Deletar(LinhaSeparacao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Instancia Repositorios
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            string codigoIntegracao = Request.Params("CodigoIntegracao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            bool ativo = false;
            bool.TryParse(Request.Params("Ativo"), out ativo);

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            bool roteiriza = true;
            bool.TryParse(Request.Params("Roteiriza"), out roteiriza);

            int nivelPrioridade = Request.GetIntParam("NivelPrioridade");

            //int nivelPrioridade = Request.GetIntParam("NivelPrioridade");
            int.TryParse(Request.Params("Filial"), out int codigoFilial);

            // Vincula dados
            linhaSeparacao.Descricao = descricao;
            linhaSeparacao.CodigoIntegracao = codigoIntegracao;
            linhaSeparacao.Ativo = ativo;
            linhaSeparacao.Observacao = observacao;
            linhaSeparacao.Roteiriza = roteiriza;
            linhaSeparacao.NivelPrioridade = nivelPrioridade;
            linhaSeparacao.Filial = repFilial.BuscarPorCodigo(codigoFilial);
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao LinhaSeparacao, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (string.IsNullOrWhiteSpace(LinhaSeparacao.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (LinhaSeparacao.Descricao.Length > 200)
            {
                msgErro = "Descrição não pode passar de 200 caracteres.";
                return false;
            }


            if (LinhaSeparacao.Observacao.Length > 2000)
            {
                msgErro = "Observação não pode passar de 2000 caracteres.";
                return false;
            }

            if (LinhaSeparacao.Filial == null)
            {
                msgErro = "Filial é obrigatória.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
            if (!string.IsNullOrWhiteSpace(Request.Params("Ativo")))
                Enum.TryParse(Request.Params("Ativo"), out ativo);
            else
                ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            string codigoIntegracao = Request.Params("CodigoIntegracao");

            int codigoFilial = Request.GetIntParam("Filial");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> listaGrid = repLinhaSeparacao.Consultar(descricao, codigoIntegracao, codigoFilial, ativo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLinhaSeparacao.ContarConsulta(descricao, codigoIntegracao, codigoFilial, ativo);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            CodigoFilial = obj.Filial?.Codigo ?? 0,
                            Filial = obj.Filial?.Descricao ?? "",
                            obj.Descricao,
                            obj.CodigoIntegracao,
                            obj.DescricaoAtivo,
                            obj.NivelPrioridade
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoFilial", false);
            grid.AdicionarCabecalho("Filial", "Filial", 35, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Prioridade", "NivelPrioridade", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Ativo", "DescricaoAtivo", 10, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}
