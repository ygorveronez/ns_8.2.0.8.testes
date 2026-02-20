using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize("Canhotos/EnviarCanhotos")]
    public class EnviarCanhotosController : BaseController
    {
		#region Construtores

		public EnviarCanhotosController(Conexao conexao) : base(conexao) { }

		#endregion

        private static Dictionary<string, List<Servicos.Global.OCR.Canhotos.CanhotoOCR>> _cache = new Dictionary<string, List<Servicos.Global.OCR.Canhotos.CanhotoOCR>>();
        private static object _lock = new object();

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            return null;
        }

        public async Task<IActionResult> VincularCanhoto()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string antiForgeryKey = Request.GetStringParam("Key");
                int codigoCarga = Request.GetIntParam("Carga");
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosDaCarga(codigoCarga, unitOfWork);

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                List<string> notasNaoEncontradas = new List<string>();
                List<string> notasVinculadas = new List<string>();
                List<string> notasNaoAutorizadas = new List<string>();
                StringBuilder falha = new StringBuilder();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        if (Empresa.Codigo != canhoto.Empresa.Codigo)
                            continue;
                    /*
                    var a = canhoto.Emitente.Empresa.Codigo;
                    var b = canhoto.XMLNotaFiscal.Empresa.Codigo;
                    var c = canhoto.Empresa.Codigo;
                    */
                    if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado ||
                    canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                    {
                        Servicos.Log.TratarErro($"Embarcador - EnviarCanhoto - Canhoto já foi digitalizado", "EnviarCanhoto");
                        continue;
                    }
                    canhoto.DataEnvioCanhoto = DateTime.Now;
                    canhoto.Observacao = canhoto.Observacao ?? string.Empty;
                    canhoto.DataUltimaModificacao = DateTime.Now;

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    string base64 = string.Empty;
                    lock (_lock)
                    {
                        if (_cache.ContainsKey(antiForgeryKey))
                        {
                            Servicos.Global.OCR.Canhotos.CanhotoOCR ocr = _cache[antiForgeryKey].Where(x => x.NumeroNFe == canhoto.XMLNotaFiscal.Numero).FirstOrDefault();
                            if (ocr == null)
                            {
                                notasNaoEncontradas.Add(canhoto.XMLNotaFiscal.Numero.ToString());
                                continue;
                            }
                            base64 = ocr.Base64.Replace("data:image/png;base64,", string.Empty);
                        }
                        else
                        {
                            return new JsonpResult(false, "Erro ao processar o arquivo em memória. Atualize a página e tente novamente.");
                        }
                    }

                    if ((canhoto.Carga.TipoOperacao?.ConfiguracaoCanhoto?.NaoPermiteUploadDeCanhotosComCTeNaoAutorizado ?? false) && serCanhoto.CanhotoPossuiCTeNaoAutorizado(canhoto))
                    {
                        notasNaoAutorizadas.Add(canhoto.XMLNotaFiscal.Numero.ToString());
                        continue;
                    }

                    unitOfWork.Start();
                    if (!configuracao.ExigeAprovacaoDigitalizacaoCanhoto)
                    {
                        canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                        canhoto.DigitalizacaoIntegrada = false;

                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, TipoServicoMultisoftware, Cliente);
                        Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracao, TipoServicoMultisoftware, Cliente, unitOfWork);
                        Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);
                    }
                    else
                    {
                        canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracao, unitOfWork);
                    }

                    canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Portal;
                    canhoto.DataDigitalizacao = DateTime.Now;

                    string caminhoCanhoto = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
                    string extensao = ".jpg";
                    canhoto.GuidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    canhoto.NomeArquivo = canhoto.Numero.ToString() + extensao;
                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhoto.GuidNomeArquivo + extensao);

                    byte[] data = System.Convert.FromBase64String(base64);

                    using (MemoryStream ms = new MemoryStream(data))
                    using (Image canhotofile = Image.FromStream(ms))
                    using (Bitmap canhotoImagem = new Bitmap(canhotofile))
                        Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, canhotoImagem, System.Drawing.Imaging.ImageFormat.Jpeg);

                    serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, $"Imagem do Canhoto digitalizada via Sistema OCR com o nome {canhoto.GuidNomeArquivo}", unitOfWork);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    {
                        serCanhoto.GerarHistoricoCanhoto(canhoto, this.Usuario, $"Falha ao salvar imagem do Canhoto", unitOfWork);
                        Servicos.Log.TratarErro($"Falha ao salvar imagem do Canhoto de id {canhoto.Codigo} e guid {canhoto.GuidNomeArquivo}.", "EnviarCanhoto");
                    }
                    canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;
                    canhoto.UsuarioDigitalizacao = this.Usuario;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, null, "Enviou imagem do canhoto", unitOfWork);
                    repCanhoto.Atualizar(canhoto);
                    unitOfWork.CommitChanges();
                    notasVinculadas.Add(canhoto.XMLNotaFiscal.Numero.ToString());
                }
                lock (_lock)
                    if (_cache.ContainsKey(antiForgeryKey))
                        _cache.Remove(antiForgeryKey);

                if (notasNaoEncontradas.Any())
                    falha.Append("As Notas Fiscais números " + string.Join(", ", notasNaoEncontradas) + " não foram encontradas. ");
                if (notasNaoAutorizadas.Any())
                    falha.Append("As Notas Fiscais números " + string.Join(", ", notasNaoAutorizadas) + " possuem CT-e não autorizado e não podem ser vinculadas. ");
                var resultado = new
                {
                    TeveFalha = notasNaoEncontradas.Any() || notasNaoAutorizadas.Any(),
                    TeveSucesso = notasVinculadas.Any(),
                    Falha = falha,
                    Sucesso = "Os canhotos números " + string.Join(", ", notasVinculadas) + " foram vinculados com sucesso."
                };

                if (notasVinculadas.Any())
                    Servicos.Log.TratarErro($"Embarcador - VincularCanhoto - Imagens salvas com sucesso", "EnviarCanhoto");

                return new JsonpResult(resultado, true, "a");
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> UploadImagens()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string antiForgeryKey = Request.GetStringParam("Key");
                int codigoCarga = Request.GetIntParam("Carga");
                List<int> numerosNotasFiscais = ObterNumerosDasNotasFiscaisDaCarga(codigoCarga, unitOfWork);

                if (numerosNotasFiscais.IsNullOrEmpty())
                    return new JsonpResult(false, "Nenhuma nota fiscal encontrada para a carga selecionada.");

                Servicos.Global.ServicoOCR servicoOcr = new Servicos.Global.ServicoOCR(unitOfWork);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Imagem");
                List<string> erros = new List<string>();
                List<string> extensoesValidas = new List<string>() { ".jpg", ".tif", ".png", ".jpeg", ".bmp", ".gif", ".pdf" };

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                Servicos.DTO.CustomFile file = arquivos[0];
                string extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!extensoesValidas.Contains(extensaoArquivo))
                    return new JsonpResult(false, "Extensão " + extensaoArquivo + " não permitida.");

                MemoryStream ms = new MemoryStream();
                arquivos[0].InputStream.CopyTo(ms);

                Servicos.Global.RespostaOCR ocr;

                if (extensaoArquivo != ".pdf")
                    ocr = servicoOcr.ExecutarServico(ms.ToArray());
                else
                    ocr = servicoOcr.ExecutarServico(ms.ToArray(), tipoPdf: true);

                if (ocr == null || !ocr.Sucesso())
                    return new JsonpResult(false, "Erro ao ler a imagem no serviço OCR. Tente novamente.");

                Servicos.Global.OCR.Canhotos.ExtratorDeDadosCanhotoOCR extratorDeDadosCanhotoOCR = new Servicos.Global.OCR.Canhotos.ExtratorDeDadosCanhotoOCR();
                List<Servicos.Global.OCR.Canhotos.CanhotoOCR> resposta = extratorDeDadosCanhotoOCR.ObterCanhotosMinerva(ocr, numerosNotasFiscais);
                if (resposta == null || resposta.Count == 0)
                    return new JsonpResult(false, "Nenhuma nota fiscal encontrada. Tente novamente.");

                resposta.RemoveAll(x => x.NumeroNFe == 0);

                ms.Position = 0;
                Image image = StreamToImage(ms);
                foreach (Servicos.Global.OCR.Canhotos.CanhotoOCR r in resposta)
                {
                    if (r.Height == 0)
                        r.Height = image.Height - r.Y;

                    Image img = CropImage(image, new Rectangle(0, r.Y, image.Width, r.Height));
                    r.Base64 = "data:image/png;base64," + ImageToBase64(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                    r.DataDeCriacao = DateTime.Now;
                    img.Dispose();
                }

                lock (_lock)
                {
                    if (_cache.ContainsKey(antiForgeryKey))
                        _cache[antiForgeryKey] = resposta;
                    else
                        _cache.Add(antiForgeryKey, resposta);

                    List<string> keysToRemove = new List<string>();
                    foreach (KeyValuePair<string, List<Servicos.Global.OCR.Canhotos.CanhotoOCR>> val in _cache.Where(x => x.Key != antiForgeryKey))
                        if (val.Value.Any(x => (DateTime.Now - x.DataDeCriacao).Minutes >= 20))
                            keysToRemove.Add(val.Key);

                    foreach (string k in keysToRemove)
                        _cache.Remove(k);
                }
                return new JsonpResult(resposta);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region PRIVATE METHODS

        private static Image StreamToImage(Stream img)
        {
            Image image = Image.FromStream(img);
            return image;
        }
        private static Image CropImage(Image image, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(image);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        private string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        private List<int> ObterNumerosDasNotasFiscaisDaCarga(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repoCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repoCanhoto.BuscarPorCarga(codigoCarga);
            return canhotos.Select(x => x.XMLNotaFiscal.Numero).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ObterCanhotosDaCarga(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repoCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            return repoCanhoto.BuscarPorCarga(codigoCarga);
        }

        #endregion
    }
}
