using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

using System.IO;

namespace SGT.WebAdmin.Controllers.PagamentoMotorista
{
    [CustomAuthorize("PagamentosMotoristas/PendenciaMotorista")]
    public class PendenciaMotoristaAnexosController : BaseController
    {
		#region Construtores

		public PendenciaMotoristaAnexosController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo repPendenciaMotoristaAnexo = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, false);

                // Dados do filtro
                int.TryParse(Request.Params("PendenciaMotorista"), out int codigo);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo> anexos = repPendenciaMotoristaAnexo.Consultar(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repPendenciaMotoristaAnexo.ContarConsulta(codigo);
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
                unitOfWork.Start();

                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista repPendenciaMotorista = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotorista(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo repPendenciaMotoristaAnexo = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo(unitOfWork);

                int codigo = Request.GetIntParam("PendenciaMotorista");

                Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista pendenciaMotorista = repPendenciaMotorista.BuscarPorCodigo(codigo);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (pendenciaMotorista == null)
                    return new JsonpResult(false, true, "Erro ao buscar registro.");


                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var nomeArquivo = file.FileName;
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    string caminho = this.CaminhoArquivos(unitOfWork);
                    string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo);

                    file.SaveAs(arquivo);

                    Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo pendenciaMotoristaAnexo = new Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo()
                    {
                        PendenciaMotorista = pendenciaMotorista,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty, // Descrição vem numa lista separada
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo))),
                    };

                    repPendenciaMotoristaAnexo.Inserir(pendenciaMotoristaAnexo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pendenciaMotorista, null, pendenciaMotoristaAnexo.NomeArquivo + " inserido", unitOfWork);
                }

                unitOfWork.CommitChanges();

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo> pendenciaMotoristaAnexos = repPendenciaMotoristaAnexo.BuscarPorPendenciaMotorista(pendenciaMotorista.Codigo);

                var dynAnexos = from obj in pendenciaMotoristaAnexos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Descricao,
                                    obj.NomeArquivo,
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

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo repPendenciaMotoristaAnexo = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo anexo = repPendenciaMotoristaAnexo.BuscarPorCodigo(codigo);

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

        public async Task<IActionResult> ExcluirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo repPendenciaMotoristaAnexo = new Repositorio.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo anexos = repPendenciaMotoristaAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista pendenciaMotorista = anexos.PendenciaMotorista;

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pendenciaMotorista, null, anexos.NomeArquivo + " excluído", unitOfWork);
                // Remove do banco
                repPendenciaMotoristaAnexo.Deletar(anexos);

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

        #endregion

        #region Métodos Privados

        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "PendenciaMotorista" });
        }

        #endregion
    }
}
