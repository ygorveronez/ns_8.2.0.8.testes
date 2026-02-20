using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/LoteAvarias")]
    public class LoteAvariasController : BaseController
    {
		#region Construtores

		public LoteAvariasController(Conexao conexao) : base(conexao) { }

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

                PreencherValoresGrid(grid, unitOfWork);

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

        public async Task<IActionResult> GerarLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Busca as avarias
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> avarias = BuscaAvariasFiltradas(unitOfWork);

                // Vincula as avarias
                if (avarias.Count() == 0)
                    return new JsonpResult(false, true, "Nenhuma Avaria selecionada.");

                // Agrupa avarias por transportador
                List<List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>> avariasAgrupadas = AgruparPorCriterios(avarias);

                int totalLotes = avariasAgrupadas.Count();
                int lotesGerados = 0;

                // Finaliza instancia
                //unitOfWork.Dispose();

                // Gera um lote para cada grupo
                for (var i = 0; i < avariasAgrupadas.Count(); i++)
                {
                    //unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    unitOfWork.FlushAndClear();
                    try
                    {
                        unitOfWork.Start();
                        // Novo Lote
                        GerarLotePorAgrupamento(avariasAgrupadas[i], unitOfWork);

                        // Commita o lote
                        unitOfWork.CommitChanges();

                        // Incrementar contador de sucesso
                        lotesGerados++;
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex);
                    }
                    finally
                    {
                        //unitOfWork.Dispose();
                    }
                }

                // Retorna sucesso
                return new JsonpResult(new
                {
                    TotalLotes = totalLotes,
                    LotesGerados = lotesGerados
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = GridPesquisa();

                PreencherValoresGrid(grid, unitOfWork);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, grid.extensaoCSV, $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        /* GerarLotePorAgrupamento 
         * Gera um lote para cada grupo de avarias
         */
        private void GerarLotePorAgrupamento(List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> avarias, Repositorio.UnitOfWork unitOfWork)
        {
            // Inicia transacao
            //unitOfWork.Start();

            // Instancia repositorios
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            Repositorio.Embarcador.Avarias.TempoEtapaLote repTempoEtapaLote = new Repositorio.Embarcador.Avarias.TempoEtapaLote(unitOfWork);
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);

            // Busca informacoes
            Dominio.Entidades.Embarcador.Avarias.Lote lote = new Dominio.Entidades.Embarcador.Avarias.Lote();

            Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avariaPrincipal = repSolicitacaoAvaria.BuscarPorCodigo(avarias[0].Codigo);

            // Dados Criacao 
            lote.Numero = repLote.BuscarProximoNumero();
            lote.DataGeracao = DateTime.Now;
            lote.Transportador = avariaPrincipal.Transportador;
            lote.MotivoAvaria = avariaPrincipal.MotivoAvaria;
            lote.Criador = this.Usuario;
            lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmCriacao;
            lote.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.CriacaoLote;

            // Insere lote no banco
            repLote.Inserir(lote, Auditado);

            for (var i = 0; i < avarias.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = repSolicitacaoAvaria.BuscarPorCodigo(avarias[i].Codigo);
                avaria.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.LoteGerado;
                avaria.Lote = lote;
                repSolicitacaoAvaria.Atualizar(avaria);
            }

            // Inclui controle de tempo por etapa
            Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoEtapa = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote();
            tempoEtapa.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.CriacaoLote;
            tempoEtapa.Entrada = DateTime.Now;
            tempoEtapa.Saida = null;
            tempoEtapa.Lote = lote;
            repTempoEtapaLote.Inserir(tempoEtapa);
        }

        /* AgruparPorTransportador 
         * Retorna uma lista de lista de avarias conforme os dados recebidos
         * Todos dados agrupados por transportador (CNPJ)
         */
        private List<List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>> AgruparPorCriterios(List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> avarias)
        {
            // Retorno dos dados
            List<List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>> dadosAgrupados = new List<List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>>();

            // Dicionario para separacao
            Dictionary<string, List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>> dicionarioAgrupar = new Dictionary<string, List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>>();

            // -- Itera todas avarias para agrupar
            // Itera todos dados recebidos
            for (var i = 0; i < avarias.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria avaria = avarias[i];
                // Chave de agrupamento:
                // CNPJ do Transportador + Código do Motivo da Avaria
                string chave = (avaria.Transportador?.CNPJ ?? string.Empty) + "-" + (avaria.MotivoAvaria.Codigo.ToString());

                // Auxiliar das avarias do dicionario
                List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> avariaDicionario = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

                // Verifica se existe uma composição no dicionario
                // Caso existe, pegar as avarias ja agrupadas
                if (dicionarioAgrupar.ContainsKey(chave)) avariaDicionario = dicionarioAgrupar[chave];

                // Adiciona a nova avaria
                avariaDicionario.Add(avaria);

                // Seta novamente no dicionario
                dicionarioAgrupar[chave] = avariaDicionario;
            }


            //-- Gera a lista de lista
            // Converte os dados do dicionario numa lista de lista de avarias
            List<string> agrupamentos = dicionarioAgrupar.Keys.ToList();

            // Itera as chaves para pegar as listas do dicionario
            foreach (string chave in agrupamentos)
                dadosAgrupados.Add(dicionarioAgrupar[chave]);

            return dadosAgrupados;
        }

        /* BuscaAvariasFiltradas 
         * Converte valores recebidos da grid e retorna os dados conforme selecionado na grid
         */
        private List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> BuscaAvariasFiltradas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaSolicitacoes = new List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    dynamic dynPesquisa = ExecutaPesquisa(ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);

                    for (var i = 0; i < totalRegistros; i++)
                        listaSolicitacoes.Add((repSolicitacaoAvaria.BuscarPorCodigo((int)dynPesquisa[i].Codigo)));

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaAvariasNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AvariasNaoSelecionadas"));
                foreach (var dybAvariasNaoSelecionada in listaAvariasNaoSelecionadas)
                    listaSolicitacoes.Remove(new Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria() { Codigo = (int)dybAvariasNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaAvariasSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AvariasSelecionadas"));
                foreach (var dynOAvariasSelecionada in listaAvariasSelecionadas)
                    listaSolicitacoes.Add(repSolicitacaoAvaria.BuscarPorCodigo((int)dynOAvariasSelecionada.Codigo));
            }

            // Retorna lista
            return listaSolicitacoes;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "NumeroAvaria", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Motivo Avaria", "MotivoAvaria", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Responsável", "Responsavel", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Solicitante Avaria", "SolicitanteAvaria", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 15, Models.Grid.Align.left, true, TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor", "ValorAvaria", 10, Models.Grid.Align.right, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.SolicitacaoAvaria repSolicitacaoAvaria = new Repositorio.Embarcador.Avarias.SolicitacaoAvaria(unitOfWork);
            Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

            // Dados do filtro
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaSolicitacaoAvaria()
            {
                Transportadora = Request.GetIntParam("Transportadora"),
                Filial = Request.GetIntParam("Filial"),
                Motivo = Request.GetIntParam("Motivo"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                CodigoUsuario = this.Usuario.Codigo, // Sempre ira mostrar apenas do usuario logado
                TiposOperacao = Request.GetListParam<int>("TipoOperacao")
            };

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> listaGrid = repSolicitacaoAvaria.ConsultarAvariasDisponiveis(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repSolicitacaoAvaria.ContarConsultaAvariasDisponiveis(filtrosPesquisa);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            NumeroAvaria = obj.NumeroAvaria.ToString(),
                            MotivoAvaria = obj.MotivoAvaria?.Descricao ?? string.Empty,
                            Transportador = obj.Transportador?.RazaoSocial ?? string.Empty,
                            Responsavel = String.Join(", ", (from r in repResponsavelAvaria.ResponsavelSolicitacao(obj.Codigo) select r.Nome).ToArray()),
                            SolicitanteAvaria = obj.Solicitante.Nome,
                            Filial = obj.Carga.Filial?.Descricao ?? string.Empty,
                            ValorAvaria = obj.ValorAvaria.ToString("n2"),
                            TipoOperacao = obj.Carga?.TipoOperacao?.Descricao ?? ""
                        };

            return lista.ToList();
        }

        private void PreencherValoresGrid(Models.Grid.Grid grid, Repositorio.UnitOfWork unitOfWork)
        {
            string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

            int totalRegistros = 0;
            var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

            // Seta valores na grid
            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Transportador") propOrdenar = "Transportador.RazaoSocial";
            else if (propOrdenar == "SolicitanteAvaria") propOrdenar = "Solicitante.Nome";
            else if (propOrdenar == "Filial") propOrdenar = "Carga.Filial.Descricao";
            else if (propOrdenar == "MotivoAvaria") propOrdenar = "MotivoAvaria.Descricao";
        }

        #endregion
    }
}
