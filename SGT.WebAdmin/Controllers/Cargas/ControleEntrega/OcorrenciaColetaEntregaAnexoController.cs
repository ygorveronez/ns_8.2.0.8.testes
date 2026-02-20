using SGT.WebAdmin.Controllers.Anexo;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/ControleEntrega")]
    public class OcorrenciaColetaEntregaAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>
    {
		#region Construtores

		public OcorrenciaColetaEntregaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrencia)
        {
            return false;
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrencia)
        {
            return false;
        }

        #endregion



        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexoOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo anexo = repAnexo.BuscarPorCodigo(codigo, true);

                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                string caminhoOcorrencia = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencias" }), $"{anexo.GuidArquivo}.{anexo.ExtensaoArquivo}");
                byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoOcorrencia);


                //Servicos.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo> servicoAnexo = new Servicos.Embarcador.Anexo.Anexo<TAnexo, TEntidadeAnexo>(unitOfWork);
                //byte[] arquivoBinario = byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);


                Servicos.Auditoria.Auditoria.Auditar(Auditado, anexo.EntidadeAnexo, null, $"Realizou o download do arquivo {anexo.NomeArquivo}.", unitOfWork);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", anexo.NomeArquivo);
                else
                    return new JsonpResult(false, true, "Não foi possível encontrar a imagem.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download da imagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}