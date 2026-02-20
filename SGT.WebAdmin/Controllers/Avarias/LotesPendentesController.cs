using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/LotesPendentes")]
    public class LotesPendentesController : BaseController
    {
        #region Construtores

        public LotesPendentesController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa(false);

                ObterDadosPesquisa(grid, unitOfWork);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar lotes pendentes.");
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
                Models.Grid.Grid grid = GridPesquisa(true);

                ObterDadosPesquisa(grid, unitOfWork);

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

        public async Task<IActionResult> Aprovar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                throw new NotImplementedException();
                /*// Repositorios
                Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (lote == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                List<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria> responsaveis = repResponsavelAvaria.BuscaPorLote(codigo);

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                foreach(Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria resp in  responsaveis)
                    EfetuarAprovacao(resp, true, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);*/
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.LoteAvariaAnexos repLoteAvariaAnexos = new Repositorio.Embarcador.Avarias.LoteAvariaAnexos(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, true);

                // Dados do filtro
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos> anexos = repLoteAvariaAnexos.Consultar(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repLoteAvariaAnexos.ContarConsulta(codigo);
                var lista = from obj in anexos
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                obj.NomeArquivo
                            };

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

        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Avarias.LoteAvariaAnexos repLoteAvariaAnexos = new Repositorio.Embarcador.Avarias.LoteAvariaAnexos(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos anexo = repLoteAvariaAnexos.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar anexo.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DelegarMultiplosLotes()
        {
            /* Busca todas as solicitacoes selecionadas
             * Vincula o usuário selecionado a todas elas seguindo critério de situacao
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                // Codigo da regra
                int codigoUsuario = 0;
                int.TryParse(Request.Params("Usuario"), out codigoUsuario);

                // Entidade responsavel
                Dominio.Entidades.Usuario responsavel = repUsuario.BuscarPorCodigo(codigoUsuario);

                // Valida justificativa (obrigatoria)
                if (responsavel == null)
                    return new JsonpResult(false, "Erro ao buscar usuário.");

                IList<int> lotesSelecionados = new List<int>();
                try
                {
                    lotesSelecionados = ObterLotesSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                Repositorio.Embarcador.Avarias.Lote repLote = new(unitOfWork);
                List<Dominio.Entidades.Embarcador.Avarias.Lote> lotes = repLote.BuscarPorCodigos(lotesSelecionados, false);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < lotes.Count(); i++)
                    EfetuarResponsabilidade(lotes[i], responsavel, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar os lotes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DelegarLote()
        {
            /* Busca a avaria e o usuario
             * Cria vínculo
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


                // Codigo da regra
                int codigoUsuario = 0;
                int.TryParse(Request.Params("Usuario"), out codigoUsuario);

                int codigoLote = 0;
                int.TryParse(Request.Params("Lote"), out codigoLote);

                // Entidades
                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigoLote);
                Dominio.Entidades.Usuario responsavel = repUsuario.BuscarPorCodigo(codigoUsuario);

                // Valida
                if (lote == null)
                    return new JsonpResult(false, true, "Erro ao buscar lote.");

                if (responsavel == null)
                    return new JsonpResult(false, true, "Erro ao buscar usuário.");

                // Inicia transacao
                unitOfWork.Start();

                // Adiciona responsável
                EfetuarResponsabilidade(lote, responsavel, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar o lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void ObterDadosPesquisa(Models.Grid.Grid grid, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.Lote repLote = new(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaLotesPendentes filtrosPesquisa = ObterFiltrosPesquisa();

            int totalRegistros = repLote.ContarConsultaLotesPendentes(filtrosPesquisa);
            if (totalRegistros == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);

            IList<Dominio.ObjetosDeValor.Embarcador.Avarias.DadosPesquisaLotesPendentes> dados = repLote.ConsultaLotesPendentes(filtrosPesquisa, parametrosConsulta);

            grid.AdicionaRows(dados);
            grid.setarQuantidadeTotal(totalRegistros);
        }

        private Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaLotesPendentes ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaLotesPendentes filtrosPesquisa = new()
            {
                NumeroLote = Request.GetIntParam("NumeroLote"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                SituacaoLote = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote>("Situacao"),
                EtapaLote = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote>("Etapa"),
            };

            return filtrosPesquisa;
        }

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa(bool exportar)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Etapa", false);
            grid.AdicionarCabecalho("Lote", "Numero", 20, Models.Grid.Align.center, true);

            if (exportar)
            {
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJEmpresa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Empresa", 20, Models.Grid.Align.left, false);
            }

            grid.AdicionarCabecalho("Data Abertura", "DataAberturaFormatada", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "SituacaoFormatada", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Etapa", "EtapaFormatada", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tempo na Etapa", "TempoEtapa", 20, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Criador", "Criador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Responsáveis", "Responsavel", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Filial", "Filial", 40, Models.Grid.Align.left, true);

            return grid;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Criador") propOrdenar = "Criador.Nome";
        }

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "LoteAvaria");

            return caminho;
        }

        /* ObterLotesSelecionadas
         * Duas maneiras de ocorrer a aprovacao em massa
         * - Selecionar todos (remove excecoes)
         * - Busca apenas selecionados
         */
        private IList<int> ObterLotesSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            IList<int> lotesSelecionados = [];

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                try
                {
                    Repositorio.Embarcador.Avarias.Lote repLote = new(unitOfWork);
                    lotesSelecionados = repLote.ConsultaLotesPendentes(ObterFiltrosPesquisa());
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaLotesNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("LotesNaoSelecionados"));
                foreach (var dybLotesNaoSelecionada in listaLotesNaoSelecionadas)
                    lotesSelecionados.Remove((int)dybLotesNaoSelecionada.Codigo);
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaLotesSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("LotesSelecionados"));
                foreach (var dynLoteSelecionada in listaLotesSelecionadas)
                    lotesSelecionados.Add((int)dynLoteSelecionada.Codigo);
            }

            return lotesSelecionados;
        }

        /* EfetuarResponsabilidade
         * Vincula responsável à solicitacao
         */
        private void EfetuarResponsabilidade(Dominio.Entidades.Embarcador.Avarias.Lote lote, Dominio.Entidades.Usuario responsavel, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

            for (var i = 0; i < lote.Avarias.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao = lote.Avarias[i];

                // Verifica se registro já não consta
                Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria verificacao = repResponsavelAvaria.BuscaPorUsuarioAvaria(solicitacao.Codigo, responsavel.Codigo);

                if (verificacao == null)
                {
                    // Cria entidade
                    Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria responsavelAvaria = new Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria();
                    responsavelAvaria.SolicitacaoAvaria = solicitacao;
                    responsavelAvaria.Usuario = responsavel;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacao, null, "Delegou o responsável " + responsavel.Nome, unitOfWork);

                    repResponsavelAvaria.Inserir(responsavelAvaria);
                }
            }
        }

        #endregion
    }
}
