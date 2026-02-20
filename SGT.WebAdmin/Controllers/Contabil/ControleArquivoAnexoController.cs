using SGT.WebAdmin.Controllers.Anexo;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/ControleArquivoAnexo", "Contabils/ControleArquivo")]
    public class ControleArquivoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo>
    {
		#region Construtores

		public ControleArquivoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> DownloadAnexoOverride()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo>(unitOfWork);
                Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo anexo = repositorioAnexo.BuscarPorCodigo(codigo, true);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo>(unitOfWork);
                byte[] arquivoBinario = servicoAnexo.DownloadAnexo(anexo, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.EntidadeAnexo, null, $"Realizou o download do arquivo {anexo.NomeArquivo}.", unitOfWork);
                anexo.RealizouDownload = true;
                repositorioAnexo.Atualizar(anexo, Auditado);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar o anexo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadTodosAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo> anexos = repositorioAnexo.BuscarPorEntidade(codigo);

                if (anexos == null || anexos.Count == 0)
                    return new JsonpResult(false, true, "Não foi encontrado anexo para este controle.");

                foreach (var anexo in anexos)
                {
                    anexo.Initialize();
                    anexo.RealizouDownload = true;
                    repositorioAnexo.Atualizar(anexo, Auditado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexos[0].EntidadeAnexo, null, $"Realizou o download de todos os anexos.", unitOfWork);
                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Anexo.ControleArquivoAnexo, Dominio.Entidades.Embarcador.Anexo.ControleArquivo>(unitOfWork);
                System.IO.MemoryStream arquivo = servicoAnexo.DownloadAnexos(anexos, unitOfWork);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(anexos[0].Descricao), "_Anexos.zip"));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do anexo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }       

        #endregion
    }
}