using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "DownloadImagem" }, "Chamados/ChamadoOcorrencia")]
    public class OcorrenciaImagemController : BaseController
    {
		#region Construtores

		public OcorrenciaImagemController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> DownloadImagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("CodigoImagem"));
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaImagem repCargaOcorrenciaImagem = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaImagem(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem cargaOcorrenciaImagem = repCargaOcorrenciaImagem.BuscarPorCodigo(codigo);
                if (cargaOcorrenciaImagem != null)
                {

                    string extensao = System.IO.Path.GetExtension(cargaOcorrenciaImagem.NomeArquivo).ToLower();

                    string caminho = retornarCaminhoImagemOcorrencia(cargaOcorrenciaImagem, unitOfWork);
                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaOcorrenciaImagem.GuidNomeArquivo + extensao);
                    string caminhoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaOcorrenciaImagem.NomeArquivo);

                    byte[] bufferCanhoto = null;

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    {
                        return new JsonpResult(false, false, "Não foi possível baixar a imagem pois ela não foi localizado.");
                    }
                    else
                    {
                        bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);
                    }

                    if (bufferCanhoto != null)
                        return Arquivo(bufferCanhoto, "application/jpg", System.IO.Path.GetFileName(caminhoOriginal));
                    else
                        return new JsonpResult(false, false, "Não foi possível baixar a imagem, atualize a página e tente novamente.");
                }
                else
                {
                    return new JsonpResult(false, true, "Não existe imagem para a ocorrência.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da imagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string retornarCaminhoImagemOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem cargaOcorrenciaImagem, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = "";
            caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoOcorrencias, "OcorrenciasMobiles");
            return caminho;
        }
    }
}
