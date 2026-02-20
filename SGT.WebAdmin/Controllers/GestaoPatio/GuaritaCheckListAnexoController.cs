using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Extensions;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "DownloadAnexo" }, "GestaoPatio/GuaritaCheckList")]
    public class GuaritaCheckListAnexoController : BaseController
    {
		#region Construtores

		public GuaritaCheckListAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo repGuaritaCheckListAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo(unitOfWork);

                // Busca Ocorrencia
                int.TryParse(Request.Params("GuaritaCheckList"), out int codigo);

                if (!this.InsereArquivos(codigo, unitOfWork, out string erro))
                    return new JsonpResult(false, erro);

                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo> anexos = repGuaritaCheckListAnexo.BuscarPorCheckList(codigo);

                var dynAnexos = (from obj in anexos
                                 select new
                                 {
                                     obj.Codigo,
                                     obj.Descricao,
                                     obj.NomeArquivo,
                                 }).ToList();

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo ao check list.");
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
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo repGuaritaCheckListAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo GuaritaCheckListAnexo = repGuaritaCheckListAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (GuaritaCheckListAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(GuaritaCheckListAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, GuaritaCheckListAnexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, FileExtensions.ConvertArquivoRetorno(bArquivo,""), GuaritaCheckListAnexo.NomeArquivo);
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
                // Repositorios
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo repGuaritaCheckListAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo GuaritaCheckListAnexo = repGuaritaCheckListAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (GuaritaCheckListAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(GuaritaCheckListAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, GuaritaCheckListAnexo.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, GuaritaCheckListAnexo.GuaritaCheckList, null, "Excluiu o anexo " + GuaritaCheckListAnexo.NomeArquivo + ".", unitOfWork);
                repGuaritaCheckListAnexo.Deletar(GuaritaCheckListAnexo);

                return new JsonpResult(true);
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




        private string CaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "GuaritaCheckList" });
        }

        private dynamic InsereArquivos(int codigo, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo repGuaritaCheckListAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo(unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigo);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (guaritaCheckList == null)
            {
                erro = "Check List não localizada para anexar arquivo.";
                return false;
            }

            for (var i = 0; i < arquivos.Count(); i++)
            {
                // Extrai dados
                Servicos.DTO.CustomFile file = arquivos[i];
                var nomeArquivo = file.FileName;
                var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = this.CaminhoArquivos(unitOfWork);

                // Salva na pasta
                file.SaveAs(caminho + guidArquivo + extensaoArquivo);

                // Insere no banco
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo guaritaCheckListAnexo = new Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo
                {
                    GuaritaCheckList = guaritaCheckList,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                };

                repGuaritaCheckListAnexo.Inserir(guaritaCheckListAnexo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaCheckListAnexo.GuaritaCheckList, null, "Adicionou o anexo " + guaritaCheckListAnexo.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }
    }
}
