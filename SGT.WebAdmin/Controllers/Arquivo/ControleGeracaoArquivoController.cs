using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Arquivo
{
    [CustomAuthorize("Arquivo/ControleGeracaoArquivo")]
    public class ControleGeracaoArquivoController : BaseController
    {
		#region Construtores

		public ControleGeracaoArquivoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoArquivo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo repositorio = new Repositorio.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo = repositorio.BuscarPorCodigo(codigoArquivo);

                if (controleGeracaoArquivo == null)
                    return new JsonpResult(true, false, "O arquivo não foi encontrado");

                if (controleGeracaoArquivo.Situacao == SituacaoGeracaoArquivo.Gerado)
                {
                    Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
                    byte[] binarioArquivo = servicoArquivo.ObterBinarioArquivo(controleGeracaoArquivo);

                    return Arquivo(binarioArquivo, $"application/{controleGeracaoArquivo.TipoArquivo.ObterExtensao()}", $"{controleGeracaoArquivo.Descricao.Replace(" ", "_")}_{controleGeracaoArquivo.DataInicioGeracao.ToString("dd-MM-yyyy_HH-mm")}.{controleGeracaoArquivo.TipoArquivo.ObterExtensao()}");

                }
                else if (controleGeracaoArquivo.Situacao == SituacaoGeracaoArquivo.FalhaAoGerar)
                    return new JsonpResult(true, false, $"Ocorreu uma falha ao gerar o arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()}, por favor tente gerá-lo novamente");
                else
                    return new JsonpResult(true, false, $"O arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()} ainda está sendo gerado, por favor aguarde a conclusão");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
