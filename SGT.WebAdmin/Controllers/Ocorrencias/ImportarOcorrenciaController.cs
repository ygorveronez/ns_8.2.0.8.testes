using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/ImportarOcorrencia")]
    public class ImportarOcorrenciaController : BaseController
    {
		#region Construtores

		public ImportarOcorrenciaController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia = repImportarOcorrencia.BuscarPorCodigo(codigo);

                // Valida
                if (importarOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    importarOcorrencia.Codigo,
                    importarOcorrencia.NumeroCTe,
                    importarOcorrencia.SerieCTe,
                    importarOcorrencia.DataOcorrencia,
                    importarOcorrencia.Observacao,
                    importarOcorrencia.ObservacaoImpressa,
                    importarOcorrencia.Booking,
                    importarOcorrencia.OrdemServico,
                    importarOcorrencia.NumeroCarga,
                    importarOcorrencia.CST,
                    importarOcorrencia.CodigoTipoOcorrencia,
                    importarOcorrencia.CodigoComponenteFrete,
                    importarOcorrencia.ValorOcorrencia,
                    importarOcorrencia.AliquotaICMS,
                    TipoOcorrencia = importarOcorrencia.TipoOcorrencia != null ? new { importarOcorrencia.TipoOcorrencia.Codigo, importarOcorrencia.TipoOcorrencia.Descricao } : null,
                    Carga = importarOcorrencia.Carga != null ? new { importarOcorrencia.Carga.Codigo, importarOcorrencia.Carga.Descricao } : null,
                    CTe = importarOcorrencia.CTe != null ? new { importarOcorrencia.CTe.Codigo, importarOcorrencia.CTe.Descricao } : null,
                    ComponenteFrete = importarOcorrencia.ComponenteFrete != null ? new { importarOcorrencia.ComponenteFrete.Codigo, importarOcorrencia.ComponenteFrete.Descricao } : null,
                    CargaOcorrencia = importarOcorrencia.CargaOcorrencia != null ? new { importarOcorrencia.CargaOcorrencia.Codigo, importarOcorrencia.CargaOcorrencia.Descricao } : null,
                    Usuario = importarOcorrencia.Usuario != null ? new { importarOcorrencia.Usuario.Codigo, importarOcorrencia.Usuario.Descricao } : null,
                    importarOcorrencia.RetornoImportacao,
                    importarOcorrencia.NomeArquivo,
                    importarOcorrencia.SituacaoImportarOcorrencia
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
                Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia();

                // Preenche entidade com dados
                PreencheEntidade(ref importarOcorrencia, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(importarOcorrencia, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repImportarOcorrencia.Inserir(importarOcorrencia, Auditado);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia = repImportarOcorrencia.BuscarPorCodigo(codigo, true);

                // Valida
                if (importarOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref importarOcorrencia, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(importarOcorrencia, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repImportarOcorrencia.Atualizar(importarOcorrencia, Auditado);
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
                Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia = repImportarOcorrencia.BuscarPorCodigo(codigo);

                // Valida
                if (importarOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repImportarOcorrencia.Deletar(importarOcorrencia, Auditado);
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

        #region Importação

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoOcorrencia();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                string nomeArquivo = Request.Params("Nome");
                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoOcorrencia();
                List<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia> ocorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia>();

                var retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, ocorrencias, ((dados) =>
                {
                    var servicoImportarOcorrencia = new Servicos.Embarcador.Ocorrencia.ImportarOcorrencia(unitOfWork, TipoServicoMultisoftware, dados, configuracao);

                    return servicoImportarOcorrencia.ObterImportarOcorrencia();
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;

                Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);

                foreach (var importarOcorrencia in ocorrencias)
                {
                    if (permiteInserir)
                    {
                        importarOcorrencia.TipoOcorrencia = repTipoOcorrencia.BuscarPorCodigoIntegracao(importarOcorrencia.CodigoTipoOcorrencia);
                        importarOcorrencia.Carga = repCarga.BuscarCargaPorCodigoEmbarcador(importarOcorrencia.NumeroCarga);
                        importarOcorrencia.CTe = repCargaCTe.BuscarPorNumeroSerieCTe(importarOcorrencia.NumeroCTe, importarOcorrencia.SerieCTe, importarOcorrencia.Carga?.Codigo ?? 0);
                        importarOcorrencia.ComponenteFrete = repComponenteFrete.BuscarPorCodigoIntegracao(importarOcorrencia.CodigoComponenteFrete);

                        importarOcorrencia.Usuario = this.Usuario;
                        importarOcorrencia.RetornoImportacao = "";
                        importarOcorrencia.NomeArquivo = nomeArquivo;

                        if (importarOcorrencia.TipoOcorrencia == null)
                            importarOcorrencia.RetornoImportacao += "Não foi localizado um tipo de ocorrência com o código importado. ";
                        if (importarOcorrencia.Carga == null)
                            importarOcorrencia.RetornoImportacao += "Não foi localizado uma carga com o número importado. ";
                        if (importarOcorrencia.CTe == null)
                            importarOcorrencia.RetornoImportacao += "Não foi localizado o CT-e com o número importado. ";
                        if (importarOcorrencia.ComponenteFrete == null)
                            importarOcorrencia.RetornoImportacao += "Não foi localizado o componente to frete com o código importado. ";

                        if (string.IsNullOrWhiteSpace(importarOcorrencia.RetornoImportacao))
                        {
                            //if (repImportarOcorrencia.RegistroDuplicado(importarOcorrencia.NumeroCarga, importarOcorrencia.CodigoTipoOcorrencia, importarOcorrencia.CodigoComponenteFrete, importarOcorrencia.NumeroCTe, importarOcorrencia.SerieCTe, importarOcorrencia.ValorOcorrencia, importarOcorrencia.DataOcorrencia, importarOcorrencia.AliquotaICMS))
                            //{
                            //    importarOcorrencia.RetornoImportacao = "Já existe uma importação de ocorrência com os mesmos dados de carga/cte/ocorrencia.";
                            //    importarOcorrencia.SituacaoImportarOcorrencia = SituacaoImportarOcorrencia.Falha;
                            //}
                            //else
                            importarOcorrencia.SituacaoImportarOcorrencia = SituacaoImportarOcorrencia.AgIntegracao;
                        }
                        else
                            importarOcorrencia.SituacaoImportarOcorrencia = SituacaoImportarOcorrencia.Falha;

                        repImportarOcorrencia.Inserir(importarOcorrencia, Auditado);

                        totalRegistrosImportados++;
                    }
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            importarOcorrencia.NumeroCTe = Request.GetIntParam("NumeroCTe");
            importarOcorrencia.SerieCTe = Request.GetIntParam("SerieCTe");
            importarOcorrencia.DataOcorrencia = Request.GetNullableDateTimeParam("");
            importarOcorrencia.Observacao = Request.GetStringParam("Observacao");
            importarOcorrencia.ObservacaoImpressa = Request.GetStringParam("ObservacaoImpressa");
            importarOcorrencia.Booking = Request.GetStringParam("Booking");
            importarOcorrencia.OrdemServico = Request.GetStringParam("OrdemServico");
            importarOcorrencia.NumeroCarga = Request.GetStringParam("NumeroCarga");
            importarOcorrencia.CST = Request.GetStringParam("CST");
            importarOcorrencia.CodigoTipoOcorrencia = Request.GetStringParam("CodigoTipoOcorrencia");
            importarOcorrencia.CodigoComponenteFrete = Request.GetStringParam("CodigoComponenteFrete");
            importarOcorrencia.ValorOcorrencia = Request.GetDecimalParam("ValorOcorrencia");
            importarOcorrencia.AliquotaICMS = Request.GetDecimalParam("AliquotaICMS");
            importarOcorrencia.RetornoImportacao = Request.GetStringParam("RetornoImportacao");
            importarOcorrencia.NomeArquivo = Request.GetStringParam("NomeArquivo");
            importarOcorrencia.SituacaoImportarOcorrencia = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia>("SituacaoImportarOcorrencia");
            importarOcorrencia.TipoOcorrencia = Request.GetIntParam("TipoOcorrencia") > 0 ? repTipoOcorrencia.BuscarPorCodigo(Request.GetIntParam("TipoOcorrencia")) : null;
            importarOcorrencia.Carga = Request.GetIntParam("Carga") > 0 ? repCarga.BuscarPorCodigo(Request.GetIntParam("Carga")) : null;
            importarOcorrencia.CTe = Request.GetIntParam("CTe") > 0 ? repCTe.BuscarPorCodigo(Request.GetIntParam("CTe")) : null;
            importarOcorrencia.ComponenteFrete = Request.GetIntParam("ComponenteFrete") > 0 ? repComponenteFrete.BuscarPorCodigo(Request.GetIntParam("ComponenteFrete")) : null;
            importarOcorrencia.CargaOcorrencia = Request.GetIntParam("CargaOcorrencia") > 0 ? repCargaOcorrencia.BuscarPorCodigo(Request.GetIntParam("CargaOcorrencia")) : null;
            importarOcorrencia.Usuario = this.Usuario;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (importarOcorrencia.CTe == null)
            {
                msgErro = "CT-e é obrigatório.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia situacao;
            if (!string.IsNullOrWhiteSpace(Request.Params("Situacao")))
                Enum.TryParse(Request.Params("Situacao"), out situacao);
            else
                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia.Todos;

            string numeroCarga = Request.Params("NumeroCarga");

            // Consulta
            List<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia> listaGrid = repImportarOcorrencia.Consultar(numeroCarga, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repImportarOcorrencia.ContarConsulta(numeroCarga, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            NumeroCarga = obj.NumeroCarga,
                            obj.NumeroCTe,
                            DescricaoSituacao = obj.SituacaoImportarOcorrencia.ObterDescricao()
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "DescricaoSituacao") propOrdenar = "SituacaoImportarOcorrencia";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nº CTe", "NumeroCTe", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "DescricaoSituacao", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoOcorrencia()
        {
            List<ConfiguracaoImportacao> configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "Cód. Tipo Ocorrencia", Propriedade = "CodigoTipoOcorrencia", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "Cód. Componente Frete", Propriedade = "CodigoComponenteFrete", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Nº Carga", Propriedade = "NumeroCarga", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Nº CTe", Propriedade = "NumeroCTe", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Nº Série CTe", Propriedade = "SerieCTe", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Nº Booking", Propriedade = "Booking", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Nº O.S.", Propriedade = "OrdemServico", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = "Valor da Ocorrência", Propriedade = "ValorOcorrencia", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = "Data Ocorrência", Propriedade = "DataOcorrencia", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 10, Descricao = "Observação Impressa", Propriedade = "ObservacaoImpressa", Tamanho = 1000, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 11, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 1000, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 12, Descricao = "Alíquota ICMS", Propriedade = "AliquotaICMS", Tamanho = tamanho, Obrigatorio = false });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 13, Descricao = "CST", Propriedade = "CST", Tamanho = 3, CampoInformacao = true });


            return configuracoes;
        }

        #endregion
    }
}
