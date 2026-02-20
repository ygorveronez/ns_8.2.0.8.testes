using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;
using ICSharpCode.SharpZipLib.Zip;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Cargas/DownloadDocumentos")]
    public class CargasDownloadDocumentosController : BaseController
    {
		#region Construtores

		public CargasDownloadDocumentosController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            try
            {
                var configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                List<int> codigosCargas = Request.GetListParam<int>("ListaCarga");
                var cargas = repCarga.BuscarPorCodigos(codigosCargas);

                if (cargas.Count == 0)
                    return new JsonpResult(false, "Nenhum grupo selecionado.");

                if (configuracao.MaxDownloadsPorVez > 0 && configuracao.MaxDownloadsPorVez < cargas.Count)
                    return new JsonpResult(false, $"Não é permitido o download de mais de {configuracao.MaxDownloadsPorVez} itens por vez");

                MemoryStream fZip = new MemoryStream();
                ZipOutputStream zipOStream = new ZipOutputStream(fZip);
                zipOStream.SetLevel(9);

                int contadorArquivosAdicionados = 0;

                Dictionary<string, bool> arquivosJaAdicionados = new Dictionary<string, bool>();

                foreach (var carga in cargas)
                    AddTodosPDFCarga(zipOStream, carga, ref contadorArquivosAdicionados, ref arquivosJaAdicionados, unitOfWork);

                zipOStream.IsStreamOwner = false;
                zipOStream.Close();
                fZip.Position = 0;

                if (contadorArquivosAdicionados == 0)
                    return new JsonpResult(false, "Nenhum arquivo a ser baixado.");

                return Arquivo(fZip, "application/octet-stream", "DocumentosCargas.zip");
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar os arquivos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void AddTodosPDFCarga(ZipOutputStream zipOStream, Dominio.Entidades.Embarcador.Cargas.Carga carga, ref int contadorArquivosAdicionados, ref Dictionary<string, bool> arquivosJaAdicionados, Repositorio.UnitOfWork unitOfWork)
        {

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    throw new Exception("O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesNFesAutorizadosPorCarga(carga.Codigo, false, false);
                string numeroPrimeiroDocCarga = repCargaCTe.BuscarPrimeiroCTePorCarga(carga.Codigo)?.Numero.ToString();

                if (ctes.Count <= 0)
                    return;

                int codigoUsuario = this.Usuario.Codigo;
                string stringConexao = _conexao.StringConexao;
                string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos;
                string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                string caminhoArquivosAnexos = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" });

                List<byte[]> arquivos = new List<byte[]>();

                byte[] arquivo = Zeus.Embarcador.ZeusNFe.Zeus.GerarPDFTodosDocumentosEObterBytes(ctes, codigoUsuario, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, caminhoArquivosAnexos, unitOfWork, carga.Codigo);
                byte[] detalheCTeMDF = Servicos.Embarcador.Carga.Carga.GerarPDFDetalheCTeMDF(carga.Codigo, unitOfWork);

                if (arquivo != null)
                    arquivos.Add(arquivo);

                if (detalheCTeMDF != null)
                    arquivos.Add(detalheCTeMDF);

                Servicos.DACTE servicoDACTE = new Servicos.DACTE(unitOfWork);

                if (arquivos.Count == 0)
                    throw new ControllerException("Nenhum arquivo encontrado.");

                byte[] pdfAgrupado = servicoDACTE.MergeFiles(arquivos);

                if (pdfAgrupado != null)
                {
                    var nomeArquivo = $"{carga.Filial?.CodigoFilialEmbarcador} Doc {numeroPrimeiroDocCarga} Carga {carga.CodigoCargaEmbarcador}.pdf";

                    if (arquivosJaAdicionados.ContainsKey(nomeArquivo))
                        return;

                    ZipEntry entry = new ZipEntry(nomeArquivo);
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(pdfAgrupado, 0, pdfAgrupado.Length);
                    zipOStream.CloseEntry();

                    contadorArquivosAdicionados += 1;
                    arquivosJaAdicionados.Add(nomeArquivo, true);
                }
            }
            catch (ControllerException excecao)
            {
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw new Exception("Ocorreu uma falha ao realizar o download do lote dos documentos.");
            }
        }

    }
}
