using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using SGT.WebAdmin.Extensions;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/AceiteLoteAvaria")]
    public class AceiteLoteAvariaController : BaseController
    {
        #region Construtores

        public AceiteLoteAvariaController(Conexao conexao) : base(conexao) { }

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
        public async Task<IActionResult> PesquisaProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaProduto();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdenaProduto(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaProduto(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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
                Models.Grid.Grid grid = GridPesquisa();

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
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);

                // Valida
                if (lote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    lote.Codigo,
                    Criador = lote.Criador.Nome,
                    DataGeracao = lote.DataGeracao.ToString("dd/MM/yyyy"),
                    Responsaveis = String.Join(", ", (from r in repResponsavelAvaria.ResponsavelLote(lote.Codigo) select r.Nome).ToArray()),
                    NumeroLote = lote.Numero.ToString(),
                    DescricaoSituacao = lote.DescricaoSituacao,
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

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.AceiteLote repAceiteLote = new Repositorio.Embarcador.Avarias.AceiteLote(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Avarias.AceiteLote aceite = repAceiteLote.BuscarUltimoAceite(lote.Codigo);

                // Valida
                if (lote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    lote.Codigo,
                    Solicitante = lote.Criador.Nome,
                    DataSolicitacao = lote.DataGeracao.ToString("dd/MM/yyyy"),
                    Responsavel = aceite?.ResponsavelRetorno.Nome ?? string.Empty,
                    DataRetorno = aceite?.DataRetorno.ToString("dd/MM/yyyy") ?? string.Empty,
                    SituacaoRetorno = lote.DescricaoSituacao,
                    ResponsavelAvaria = lote.MotivoAvaria?.Responsavel ?? null,
                    Observacao = lote.MotivoAvaria?.Responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria.CarregamentoDescarregamento ? "Lote sem necessidade de aprovação" : aceite?.Observacao ?? string.Empty
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
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, false);

                // Dados do filtro
                int codigo = 0;
                int.TryParse(Request.Params("Lote"), out codigo);

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

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Avarias.LoteAvariaAnexos repLoteAvariaAnexos = new Repositorio.Embarcador.Avarias.LoteAvariaAnexos(unitOfWork);

                // Busca Ocorrencia
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.Entidades.Embarcador.Avarias.Lote lote = repLote.BuscarPorCodigo(codigo);

                // Valida
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (lote == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");

                if (lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoTransportador)
                    return new JsonpResult(false, true, "Situação do lote não permite anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    // Extrai dados
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = this.CaminhoArquivos(unitOfWork);

                    // Salva na pasta
                    file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                    // Insere no banco
                    Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos loteAvariaAnexos = new Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos();

                    loteAvariaAnexos.Lote = lote;
                    loteAvariaAnexos.Descricao = i < descricoes.Length ? descricoes[i] : string.Empty; // Descrição vem numa lista separada
                    loteAvariaAnexos.GuidArquivo = guidArquivo;
                    loteAvariaAnexos.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));

                    repLoteAvariaAnexos.Inserir(loteAvariaAnexos, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, lote, null, "Arquivo " + loteAvariaAnexos.NomeArquivo + " adicionado ao lote", unitOfWork);
                }

                // Commita
                unitOfWork.CommitChanges();

                // Busca todos anexos
                List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos> anexoslote = repLoteAvariaAnexos.BuscarPorLote(lote.Codigo);

                // Retorna arquivos
                var dynAnexos = from obj in anexoslote
                                select new
                                {
                                    obj.Codigo,
                                    obj.Descricao,
                                    obj.NomeArquivo
                                };

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
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
                    return Arquivo(bArquivo, FileExtensions.ConvertArquivoRetorno(bArquivo, ""), anexo.NomeArquivo);
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

        public async Task<IActionResult> ExcluirAnexao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Avarias.LoteAvariaAnexos repLoteAvariaAnexos = new Repositorio.Embarcador.Avarias.LoteAvariaAnexos(unitOfWork);

                // Busca Anexo
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos anexos = repLoteAvariaAnexos.BuscarPorCodigo(codigo);

                // Valida
                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                if (anexos.Lote.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoTransportador)
                    return new JsonpResult(false, "Situação do Lote não permite excluir arquivos.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexos, null, "Arquivo " + anexos.NomeArquivo + " removido do lote", unitOfWork);
                repLoteAvariaAnexos.Deletar(anexos);

                // Commita
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> AprovaLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Responde o lote
                Dominio.Entidades.Embarcador.Avarias.Lote lote = new Dominio.Entidades.Embarcador.Avarias.Lote();
                string erro;
                if (!RespondeLote(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.IntegracaoLote, unitOfWork, ref lote, out erro))
                    return new JsonpResult(false, true, erro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, lote, null, "Aprovou o lote.", unitOfWork);

                // Commita
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovaLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Responde o lote
                Dominio.Entidades.Embarcador.Avarias.Lote lote = new Dominio.Entidades.Embarcador.Avarias.Lote();
                string erro;
                if (!RespondeLote(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.ReprovacaoTransportador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.CriacaoLote, unitOfWork, ref lote, out erro))
                    return new JsonpResult(false, true, erro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, lote, null, "Reprovou o lote.", unitOfWork);

                // Commita
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> TermoAceite()
        {
            try
            {
                var result = ReportRequest.WithType(ReportType.TermoAceite)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoLote", Request.GetIntParam("Codigo").ToString())
                    .CallReport();

                // Retorna o arquivo
                return Arquivo(result.GetContentFile(), "application/pdf", "Termo de Aceite do Lote.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o termo.");
            }
        }

        #endregion

        #region Métodos Privados

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "LoteAvaria");

            return caminho;
        }

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número Lote", "Numero", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Responsável", "Responsavel", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Criador", "Criador", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data da Geração", "DataGeracao", 10, Models.Grid.Align.left, true);

            return grid;
        }
        private Models.Grid.Grid GridPesquisaProduto()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("RemovidoLote", false);
            grid.AdicionarCabecalho("Produto", "Produto", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Caixas Avariadas", "CaixasAvariadas", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Unidades Avariadas", "UnidadesAvariadas", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Valor Total", "ValorAvaria", 10, Models.Grid.Align.right, true);

            return grid;
        }

        /* ExecutaPesquisa 
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.AgAprovacaoTransportador;

            // Filtro MultiCTe

            int numeroLote = 0;

            int.TryParse(Request.Params("NumeroLote"), out numeroLote);

            int transportadora = 0;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                transportadora = this.Empresa.Codigo;
            }

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.Lote> listaGrid = repLote.ConsultarAceite(numeroLote, transportadora, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLote.ContarConsultaAceite(numeroLote, transportadora, situacao);

            List<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria> responsaveis = repResponsavelAvaria.ResponsavelLotes((from obj in listaGrid select obj.Codigo).ToList());


            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Numero = obj.Numero.ToString(),
                            Transportador = obj.Transportador?.Descricao ?? string.Empty,
                            Responsavel = String.Join(", ", (from r in responsaveis where r.SolicitacaoAvaria.Lote != null && r.SolicitacaoAvaria.Lote.Codigo == obj.Codigo select r.Usuario.Nome).ToArray()),
                            Criador = obj.Criador.Nome,
                            Situacao = obj.DescricaoSituacao,
                            DataGeracao = obj.DataGeracao.ToString("dd/MM/yyyy")
                        };

            return lista.ToList();
        }
        private dynamic ExecutaPesquisaProduto(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);

            // Dados do filtro
            int lote = 0;
            int.TryParse(Request.Params("Lote"), out lote);

            int solicitacao = 0;
            int.TryParse(Request.Params("Solicitacao"), out solicitacao);

            string descricao = "";
            string codigoEmbarcador = "";

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> listaGrid = new List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();

            listaGrid = repLote.ConsultarProdutosAvaria(lote, solicitacao, descricao, codigoEmbarcador, false, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLote.ContarConsultaProdutosAvaria(lote, solicitacao, descricao, codigoEmbarcador, false);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            RemovidoLote = obj.RemovidoLote,
                            Produto = obj.ProdutoEmbarcador.Descricao,
                            GrupoProduto = obj.ProdutoEmbarcador.GrupoProduto?.Descricao ?? string.Empty,
                            CaixasAvariadas = obj.CaixasAvariadas,
                            UnidadesAvariadas = obj.UnidadesAvariadas,
                            ValorAvaria = obj.ValorAvaria.ToString("n2"),
                        };

            return lista.ToList();
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Criador") propOrdenar = "Criador.Nome";
        }
        private void PropOrdenaProduto(ref string propOrdenar)
        {
            if (propOrdenar == "Produto") propOrdenar = "ProdutoEmbarcador.Descricao";
            else if (propOrdenar == "GrupoProduto") propOrdenar = "ProdutoEmbarcador.GrupoProduto.Descricao";
        }

        private bool RespondeLote(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacaoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote etapaLote, Repositorio.UnitOfWork unitOfWork, ref Dominio.Entidades.Embarcador.Avarias.Lote lote, out string erro)
        {
            erro = "";

            // Repositorios
            Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
            Repositorio.Embarcador.Avarias.TempoEtapaLote repTempoEtapaLote = new Repositorio.Embarcador.Avarias.TempoEtapaLote(unitOfWork);
            Repositorio.Embarcador.Avarias.AceiteLote repAceiteLote = new Repositorio.Embarcador.Avarias.AceiteLote(unitOfWork);

            // Busca Ocorrencia
            int codigo = 0;
            int.TryParse(Request.Params("Lote"), out codigo);

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            // Busca o lote
            lote = repLote.BuscarPorCodigo(codigo);
            Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoFechar = repTempoEtapaLote.BuscarUltimaEtapa(codigo);

            // Valida
            if (lote == null)
            {
                erro = "Não foi possível encontrar o registro.";
                return false;
            }

            // Adiciona o aceite
            Dominio.Entidades.Embarcador.Avarias.AceiteLote aceiteLote = new Dominio.Entidades.Embarcador.Avarias.AceiteLote();
            aceiteLote.Lote = lote;
            aceiteLote.DataRetorno = DateTime.Now;
            aceiteLote.ResponsavelRetorno = this.Usuario;
            aceiteLote.Observacao = observacao;

            // Atualiza etapa tempo
            if (tempoFechar != null)
            {
                tempoFechar.Saida = DateTime.Now;
                repTempoEtapaLote.Atualizar(tempoFechar);
            }

            // Cria o tempo da nova etapa
            Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote tempoEtapa = new Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote();
            tempoEtapa.Entrada = DateTime.Now;
            tempoEtapa.Etapa = etapaLote;
            tempoEtapa.Lote = lote;
            tempoEtapa.Saida = null;

            // Atualiza o lote
            lote.Situacao = situacaoLote;
            lote.Etapa = etapaLote;

            // Persiste dados
            repLote.Atualizar(lote);
            repTempoEtapaLote.Inserir(tempoEtapa);
            repAceiteLote.Inserir(aceiteLote, Auditado);

            return true;
        }

        #endregion
    }
}
