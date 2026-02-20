using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Seguros
{
    [CustomAuthorize("Seguros/ApoliceSeguro")]
    public class ApoliceSeguroAnexosController : BaseController
    {
		#region Construtores

		public ApoliceSeguroAnexosController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo repositorioApoliceSeguroAnexo = new Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome", "NomeArquivo", 10, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("ApoliceSeguro");

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo> anexos = repositorioApoliceSeguroAnexo.Consultar(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repositorioApoliceSeguroAnexo.ContarConsulta(codigo);
                var lista = from obj in anexos
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                obj.NomeArquivo
                            };

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

                Repositorio.Embarcador.Seguros.ApoliceSeguro repositorioApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo repositorioApoliceSeguroAnexo = new Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo(unitOfWork);

                int codigo = Request.GetIntParam("ApoliceSeguro");

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repositorioApoliceSeguro.BuscarPorCodigo(codigo);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (apoliceSeguro == null)
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

                    Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo apoliceSeguroAnexo = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo()
                    {
                        ApoliceSeguro = apoliceSeguro,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty, // Descrição vem numa lista separada
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                    };

                    repositorioApoliceSeguroAnexo.Inserir(apoliceSeguroAnexo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, apoliceSeguroAnexo.NomeArquivo + " inserido", unitOfWork);
                }

                unitOfWork.CommitChanges();

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo> apoliceSeguroAnexos = repositorioApoliceSeguroAnexo.BuscarPorApolice(apoliceSeguro.Codigo);

                var dynAnexos = from obj in apoliceSeguroAnexos
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

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo repositorioApoliceSeguroAnexo = new Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo anexo = repositorioApoliceSeguroAnexo.BuscarPorCodigo(codigo);

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
                unitOfWork.Start();

                Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo repositorioApoliceSeguroAnexo = new Repositorio.Embarcador.Seguros.ApoliceSeguroAnexo(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo anexos = repositorioApoliceSeguroAnexo.BuscarPorCodigo(codigo);

                if (anexos == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexos.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexos.GuidArquivo + extensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = anexos.ApoliceSeguro;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, apoliceSeguro, null, anexos.NomeArquivo + " excluído", unitOfWork);

                repositorioApoliceSeguroAnexo.Deletar(anexos);

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
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "ApoliceSeguro" });
        }

        #endregion

    }
}
