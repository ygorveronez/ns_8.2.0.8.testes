using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize(new string[] { "DownloadAnexo" }, "Frotas/Abastecimento")]
    public class AbastecimentoAnexoController : BaseController
    {
		#region Construtores

		public AbastecimentoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.AbastecimentoAnexo repAbastecimentoAnexo = new Repositorio.AbastecimentoAnexo(unitOfWork);

                // Busca Ocorrencia
                int.TryParse(Request.Params("Abastecimento"), out int codigo);

                if (!this.InsereArquivos(codigo, unitOfWork, out string erro))
                    return new JsonpResult(false, erro);

                List<Dominio.Entidades.AbastecimentoAnexo> anexos = repAbastecimentoAnexo.BuscarPorCodigoAbastecimento(codigo);

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
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo a ocorrência.");
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
                Repositorio.AbastecimentoAnexo repAbastecimentoAnexo = new Repositorio.AbastecimentoAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.AbastecimentoAnexo AbastecimentoAnexo = repAbastecimentoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (AbastecimentoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(AbastecimentoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, AbastecimentoAnexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", AbastecimentoAnexo.NomeArquivo);
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frotas/Abastecimento");

                // Repositorios
                Repositorio.AbastecimentoAnexo repAbastecimentoAnexo = new Repositorio.AbastecimentoAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.AbastecimentoAnexo AbastecimentoAnexo = repAbastecimentoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (AbastecimentoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                /*
                if (!(Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Abastecimento_PermiteRemoverAnexos)))
                    return new JsonpResult(false, true, "Você não possui permissão para remover os anexos.");
                */

                // Monta apontamento ao arquivo
                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(AbastecimentoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, AbastecimentoAnexo.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                Servicos.Auditoria.Auditoria.Auditar(Auditado, AbastecimentoAnexo.Abastecimento, null, "Excluiu o anexo " + AbastecimentoAnexo.NomeArquivo + ".", unitOfWork);
                repAbastecimentoAnexo.Deletar(AbastecimentoAnexo);

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
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Abastecimento" });
        }

        private dynamic InsereArquivos(int codigo, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.AbastecimentoAnexo repAbastecimentoAnexo = new Repositorio.AbastecimentoAnexo(unitOfWork);

            Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigo);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (abastecimento == null)
            {
                erro = "Abastecimento não localizada para anexar arquivo.";
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
                file.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo));

                // Insere no banco
                Dominio.Entidades.AbastecimentoAnexo abastecimentoAnexo = new Dominio.Entidades.AbastecimentoAnexo
                {
                    Abastecimento = abastecimento,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                };

                repAbastecimentoAnexo.Inserir(abastecimentoAnexo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, abastecimentoAnexo.Abastecimento, null, "Adicionou o anexo " + abastecimentoAnexo.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }
    }
}
