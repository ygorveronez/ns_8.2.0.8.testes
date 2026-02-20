using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize(new string[] { "DownloadAnexo" }, "Pedidos/TipoOperacao")]
    public class TipoOperacaoAnexoController : BaseController
    {
		#region Construtores

		public TipoOperacaoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AnexarArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo repositorioTipoOperacaoAnexo = new Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo(unitOfWork);

                // Busca Ocorrencia
                int.TryParse(Request.Params("TipoOperacao"), out int codigo);

                if (!this.InsereArquivos(codigo, unitOfWork, out string erro))
                    return new JsonpResult(false, erro);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo> anexos = repositorioTipoOperacaoAnexo.BuscarPorCodigoTipoOperacao(codigo);

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
                Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo repositorioTipoOperacaoAnexo = new Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo(unitOfWork);

                // Busca Anexo
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo tipoOperacaoAnexo = repositorioTipoOperacaoAnexo.BuscarPorCodigo(codigo);

                // Valida
                if (tipoOperacaoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                string extencao = System.IO.Path.GetExtension(tipoOperacaoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tipoOperacaoAnexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", tipoOperacaoAnexo.NomeArquivo);
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
                Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo repositorioTipoOperacaoAnexo = new Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo tipoOperacaoAnexo = repositorioTipoOperacaoAnexo.BuscarPorCodigo(codigo);

                if (tipoOperacaoAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar os dados.");

                string caminho = this.CaminhoArquivos(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(tipoOperacaoAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tipoOperacaoAnexo.GuidArquivo + extensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacaoAnexo.TipoOperacao, null, "Excluiu o anexo " + tipoOperacaoAnexo.NomeArquivo + ".", unitOfWork);
                repositorioTipoOperacaoAnexo.Deletar(tipoOperacaoAnexo);

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
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "TipoOperacao" });
        }

        private dynamic InsereArquivos(int codigo, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            // Repositorios
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo repositorioTipoOperacaoAnexo = new Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigo);

            // Valida
            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            string[] descricoes = Request.TryGetArrayParam<string>("Descricao");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            if (tipoOperacao == null)
            {
                erro = "Veículo não localizada para anexar arquivo.";
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
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo tipoOperacaoAnexo = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo
                {
                    TipoOperacao = tipoOperacao,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                };

                repositorioTipoOperacaoAnexo.Inserir(tipoOperacaoAnexo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacaoAnexo.TipoOperacao, null, "Adicionou o anexo " + tipoOperacaoAnexo.NomeArquivo + ".", unitOfWork);
            }

            return true;
        }
    }
}
