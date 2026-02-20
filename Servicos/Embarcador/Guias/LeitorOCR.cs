using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using JsonFx.IO;
using Org.BouncyCastle.Utilities;
using Servicos.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Guias
{
    public class LeitorOCR : ServicoBase
    {
        #region Atributos Privados

        private string caminhoArquivos = Servicos.FS.GetPath(@"C:\Arquivos\FTP");
        readonly private Servicos.Global.ServicoOCR _servicoOCR;        
        readonly private string _patternCodigoBarra = @"\d{11}-\d";
        readonly private string _patternValores = @"\b(?:R\$\s?)?(\d{1,3}(?:\.\d{3})*(?:,\d{2}))\b";
        readonly private string _data = @"\b\d{1,2}\/\d{1,2}\/(?:\d{2}|\d{4})\b";
        readonly private string _regexCnpj = @"[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}\-?[0-9]{2}";
        readonly private List<string> _estadosBrasil = new List<string>
        {
            "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG","PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
        };

        #endregion

        #region Construtores
        public LeitorOCR(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LeitorOCR(Servicos.Global.ServicoOCR servicoOrc, Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
        {
            _servicoOCR = servicoOrc;            
        }

        #endregion

        #region Métodos Públicos

        public string ObterBase64DaImagem(Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = guiaAnexo?.EntidadeAnexo != null ? ObterCaminhoArquivosVinculados(unitOfWork) : ObterCaminhoArquivosNaoVinculados(unitOfWork);
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guiaAnexo.GuidArquivo);

            // Alguns arquivos possuem extensão TIF, porém não são de natureza TIF
            if (guiaAnexo.ExtensaoArquivo == ExtensaoArquivo.TIF.ToString())
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(nomeCompletoArquivo)))
                {
                    if (System.Drawing.Imaging.ImageFormat.Tiff.Equals(image.RawFormat))
                    {
                        string tmp = Path.GetTempFileName();

                        Utilidades.IO.FileStorageService.Storage.SaveImage(tmp, image, System.Drawing.Imaging.ImageFormat.Png);

                        nomeCompletoArquivo = tmp;
                    }
                }
            }

            string caminhoComExtensao = string.Concat(nomeCompletoArquivo, ".", guiaAnexo.ExtensaoArquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoComExtensao))
                return null;

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoComExtensao);
            string base64 = Convert.ToBase64String(imageArray);

            return base64;
        }

        public MemoryStream ObterStremingPDF(Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoArquivosVinculados(unitOfWork);
            string nomeCompletoArquivo = string.Concat(Utilidades.IO.FileStorageService.Storage.Combine(caminho, guiaAnexo.GuidArquivo), ".pdf");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                return null;

            byte[] pdfArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeCompletoArquivo);
            MemoryStream output = new MemoryStream();

            output.Write(pdfArray, 0, pdfArray.Length);
            output.Position = 0;

            return output;
        }
        public MemoryStream ObterStremingPDFGuia(Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            if (guiaAnexo.ExtensaoArquivo != "pdf")
                return null;

            string caminho = ObterCaminhoArquivosVinculados(unitOfWork);
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guiaAnexo.GuidArquivo + ".pdf");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                return null;

            byte[] pdfArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeCompletoArquivo);
            MemoryStream output = new MemoryStream();

            output.Write(pdfArray, 0, pdfArray.Length);
            output.Position = 0;

            return output;
        }

        public MemoryStream ObterStreamingPDFProcessados(Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo)
        {
            string caminho = ObterCaminhoArquivosNaoVinculados(_unitOfWork);
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guiaAnexo.GuidArquivo}.{guiaAnexo.ExtensaoArquivo}");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                return null;

            byte[] pdfArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeCompletoArquivo);
            MemoryStream output = new MemoryStream();

            output.Write(pdfArray, 0, pdfArray.Length);
            output.Position = 0;

            return output;
        }

        public void AdicionarArquivoNaPastaProcessados(Repositorio.UnitOfWork unitOfWork, CustomFile arquivo, string caminhoRaiz)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "GuiaNacionalRecolhimentoTributoEstadual", "NaoVinculados");

            string nomeArquivo = $"{Guid.NewGuid().ToString().Replace("_", "")}";
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{nomeArquivo}{Path.GetExtension(arquivo.FileName)}");

            if (Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                throw new ServicoException($"Já existe um arquivo dentro da pasta \"{caminhoArquivos}\" com o mesmo nome");

            arquivo.SaveAs(nomeCompletoArquivo);

            try
            {
                AdicionarGuiaAnexoProcessados(unitOfWork, arquivo.FileName, nomeArquivo);
            }
            catch
            {
                Utilidades.IO.FileStorageService.Storage.Delete(nomeCompletoArquivo);
                throw;
            }
        }

        public void VincularAnexoGuia(Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia, string caminhoCompleto, Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Guias.GuiaRecolhimento svcGuia = new Servicos.Embarcador.Guias.GuiaRecolhimento(unitOfWork);
            Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
            Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repGuia = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

            string nomeArquivo = System.IO.Path.GetFileNameWithoutExtension(guiaAnexo.GuidArquivo);

            if (guiaAnexo.EntidadeAnexo != null)
            {
                if (guiaAnexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Guia)
                    guiaAnexo.EntidadeAnexo.SituacaoDigitalizacaoGuiaRecolhimento = SituacaoDigitalizacaoGuiaRecolhimento.NaoDigitalizado;

                if (guiaAnexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante)
                    guiaAnexo.EntidadeAnexo.SituacaoDigitalizacaoComprovante = SituacaoDigitalizacaoGuiaComprovante.NaoDigitalizado;
            }

            if (guiaAnexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Guia)
                guia.SituacaoDigitalizacaoGuiaRecolhimento = SituacaoDigitalizacaoGuiaRecolhimento.Digitalizado;
            if (guiaAnexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante)
                guia.SituacaoDigitalizacaoComprovante = SituacaoDigitalizacaoGuiaComprovante.Digitalizado;


            Servicos.Embarcador.Guias.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(new Servicos.Global.ServicoOCR(unitOfWork), unitOfWork);

            MemoryStream arquivo = ObterStreamingPDFProcessados(guiaAnexo);

            if (guiaAnexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante)
                srvLeitorOCR.ProcessarTextoComprovante(arquivo.ToArray(), guia);
            else if (guiaAnexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Guia)
                srvLeitorOCR.ProcessarTextoGuia(arquivo.ToArray(), guia);


            MoverParaPastaVinculados(guiaAnexo, caminhoCompleto, unitOfWork);

            guiaAnexo.EntidadeAnexo = guia;

            repGuiaAnexo.Atualizar(guiaAnexo);
            repGuia.Atualizar(guia);
        }

        public string ObterCaminhoArquivosVinculados(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "GuiaNacionalRecolhimentoTributoEstadual", "Vinculados");
        }

        public string ObterCaminhoArquivosNaoVinculados(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "GuiaNacionalRecolhimentoTributoEstadual", "NaoVinculados");
        }

        public Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo AdicionarGuiaAnexoProcessados(Repositorio.UnitOfWork unitOfWork, string nomeArquivo, string guidArquivo)
        {
            return AdicionarOuAtualizarGuiaAnexos(unitOfWork, nomeArquivo, guidArquivo);
        }


        public void ProcessarTextoComprovante(byte[] comprovante, Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual gnre)
        {
            Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(_unitOfWork);

            var responseOCR = _servicoOCR.ExecutarServico(comprovante, false, true);

            if (!responseOCR.Sucesso())
            {
                gnre.SituacaoLeituraOCRComprovante = SituacaoLeituraOCR.Inconsistente;
                repositorioGNRE.Atualizar(gnre);
                return;
            }

            string texto = responseOCR.ObterTexto();

            MatchCollection matchesCodigosBarra = Regex.Matches(texto, _patternCodigoBarra);
            MatchCollection matchesValores = Regex.Matches(texto, _patternValores);
            List<string> codigosBarraCompativeis = new List<string>();
            List<decimal> listaValores = new List<decimal>();

            foreach (Match match in matchesCodigosBarra)
                codigosBarraCompativeis.Add(match.Value);

            foreach (Match match in matchesValores)
            {
                decimal.TryParse(match.Value, out decimal valor);

                if (!(valor > 0))
                    continue;

                listaValores.Add(valor);
            }

            gnre.CodigoBarraComprovante = string.Join(" ", codigosBarraCompativeis);
            gnre.ValorComprovante = listaValores.Count > 0 ? listaValores.Max() : 0;

            bool valoresIGuais = gnre.ValorGuia == gnre.ValorComprovante && gnre.Valor == gnre.ValorComprovante;
            bool codigoBarraIguais = gnre.CodigoBarraComprovante == gnre.CodigoBarraGuia;

            gnre.SituacaoLeituraOCRComprovante = (valoresIGuais && codigoBarraIguais) ? SituacaoLeituraOCR.Validado : SituacaoLeituraOCR.Inconsistente;

            if (!valoresIGuais)
                CriarIrregulariadeNoControleDocumento(gnre, "Valor total da guia");

            if (!codigoBarraIguais)
                CriarIrregulariadeNoControleDocumento(gnre, "Comprovante de Pagamento x Guia");

            repositorioGNRE.Atualizar(gnre);
        }

        public void ProcessarTextoGuia(byte[] comprovante, Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual gnre)
        {
            Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(_unitOfWork);

            var responseOCR = _servicoOCR.ExecutarServico(comprovante, false, true);

            if (!responseOCR.Sucesso())
            {
                gnre.SituacaoLeituraOCRGuia = SituacaoLeituraOCR.Inconsistente;
                repositorioGNRE.Atualizar(gnre);
                return;
            }

            string texto = responseOCR.ObterTexto();
            List<string> textoLista = responseOCR.ObterTextoEmLinhas();

            MatchCollection matchesCodigosBarra = Regex.Matches(texto, _patternCodigoBarra);
            MatchCollection matchesCNPJ = Regex.Matches(texto, _regexCnpj);
            MatchCollection matchesValores = Regex.Matches(texto, _patternValores);
            MatchCollection matchesData = Regex.Matches(texto, _data);
            MatchCollection ufsBrasil = Regex.Matches(texto, @"\b(" + string.Join("|", _unitOfWork) + @")\b");

            List<string> codigosBarraCompativeis = new List<string>();
            List<string> cnpjs = new List<string>();
            List<string> estados = new List<string>();
            List<decimal> listaValores = new List<decimal>();
            List<DateTime> listaDatas = new List<DateTime>();

            foreach (Match match in matchesCodigosBarra)
                codigosBarraCompativeis.Add(match.Value);

            foreach (Match match in matchesCNPJ)
                cnpjs.Add(match.Value);

            foreach (Match match in matchesValores)
            {
                decimal.TryParse(match.Value, out decimal valor);

                if (!(valor > 0))
                    continue;

                listaValores.Add(valor);
            }

            foreach (Match match in matchesData)
            {
                DateTime.TryParse(match.Value, out DateTime valor);

                if (valor == DateTime.MinValue || valor == DateTime.MaxValue)
                    continue;

                listaDatas.Add(valor);
            }

            foreach (var linha in textoLista)
                foreach (var estado in _estadosBrasil)
                {
                    if (!linha.Contains(estado))
                        continue;

                    if (linha.Length == 2 && linha.Contains(estado))
                    {
                        estados.Add(estado);
                        continue;
                    }
                }

            bool guiaMesmoCnpf = cnpjs.Count > 0 ? cnpjs.FirstOrDefault().ObterSomenteNumeros() == gnre.Cte.Empresa.CNPJ_SemFormato : false;
            bool ufIguais = estados.Count > 0 ? estados.Any(x => x == gnre.Cte.LocalidadeInicioPrestacao.Estado.Sigla) : false;
            bool contemCodigoReceita = texto.Contains("100030");
            bool dataEmisaoCombinan = listaDatas.Count > 0 ? gnre.Cte.DataEmissao <= listaDatas.Max() : false;
            bool mesmoMesEmissao = listaDatas.Count > 0 ? gnre.Cte.DataEmissao.Value.Month == listaDatas.Max().Month : false;
            bool numeroDocumentoValido = texto.Contains($"{gnre.Cte.Numero}");
            bool mesmosValores = listaValores.Count > 0 ? listaValores.Contains(gnre.Valor) : false;
            bool ValorTotal = listaValores.Count > 0 && gnre.Valor > 0 ? listaValores.Contains(gnre.Valor) : false;

            gnre.ValorGuia = listaValores.Count > 0 ? listaValores.Max() : 0;
            gnre.CodigoBarraGuia = string.Join(" ", codigosBarraCompativeis);
            gnre.GuiaValidadaAutomaticamente = guiaMesmoCnpf && ufIguais && contemCodigoReceita && dataEmisaoCombinan && mesmoMesEmissao && numeroDocumentoValido && mesmosValores && ValorTotal;
            gnre.SituacaoLeituraOCRGuia = gnre.GuiaValidadaAutomaticamente ? SituacaoLeituraOCR.Validado : SituacaoLeituraOCR.Inconsistente;

            if (!guiaMesmoCnpf)
                CriarIrregulariadeNoControleDocumento(gnre, "Dados do Contribuinte Emitente");

            if (!ufIguais)
                CriarIrregulariadeNoControleDocumento(gnre, "UF Favorecida");

            if (!contemCodigoReceita)
                CriarIrregulariadeNoControleDocumento(gnre, "Código da Receita");

            if (!dataEmisaoCombinan)
                CriarIrregulariadeNoControleDocumento(gnre, "Data de Vencimento");

            if (!mesmoMesEmissao)
                CriarIrregulariadeNoControleDocumento(gnre, "Período de referência");

            if (!mesmosValores)
                CriarIrregulariadeNoControleDocumento(gnre, "Valor principal e total a recolher");

            if (!ValorTotal)
                CriarIrregulariadeNoControleDocumento(gnre, "Valor total da guia");

            repositorioGNRE.Atualizar(gnre);
        }

        #endregion


        #region Métodos Privados

        private void MoverParaPastaVinculados(Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo, string nomeCompletoArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoArquivosVinculados(unitOfWork);

            MoverParaOutraPasta(guiaAnexo, nomeCompletoArquivo, caminho);
        }

        private void MoverParaOutraPasta(Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo, string nomeCompletoArquivo, string caminho)
        {
            string nomeCompletoArquivoDestino = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guiaAnexo.GuidArquivo}.{guiaAnexo.ExtensaoArquivo}");

            try
            {
                if (Utilidades.IO.FileStorageService.Storage.Exists($"{nomeCompletoArquivo}.{guiaAnexo.ExtensaoArquivo}"))
                    Utilidades.IO.FileStorageService.Storage.Move($"{nomeCompletoArquivo}.{guiaAnexo.ExtensaoArquivo}", nomeCompletoArquivoDestino);
            }
            catch (IOException ex)
            {
                guiaAnexo.GuidArquivo = $"{Guid.NewGuid().ToString().Replace("_", "")}";

                string nomeCompletoArquivoDestinoRenomeado = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guiaAnexo.GuidArquivo}.{guiaAnexo.ExtensaoArquivo}");

                Utilidades.IO.FileStorageService.Storage.Move(nomeCompletoArquivo, nomeCompletoArquivoDestinoRenomeado);
            }
        }

        private Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo AdicionarOuAtualizarGuiaAnexos(Repositorio.UnitOfWork unitOfWork, string nomeArquivo, string guidArquivo)
        {
            Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);

            Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo = new Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo
            {
                DataAnexo = DateTime.Now,
                GuidArquivo = guidArquivo,
                EntidadeAnexo = null,
                NomeArquivo = nomeArquivo
            };

            repGuiaAnexo.Inserir(guiaAnexo);


            return guiaAnexo;
        }

        private void CriarIrregulariadeNoControleDocumento(Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual gnre, string nome)
        {
            Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade repositorioMotivoIrregulariadades = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(_unitOfWork);
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositorioHistoricoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(_unitOfWork);
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumentos = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            var motivoIrregularidade = repositorioMotivoIrregulariadades.BuscarPorDescricao(nome);
            var controleDocumento = repositorioControleDocumentos.BuscarPorCodigoCtE(gnre?.Cte?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade novoHistorioIrregularidade = repositorioHistoricoIrregularidade.BuscarPorMotivo(motivoIrregularidade?.Codigo ?? 0, controleDocumento?.Codigo ?? 0);

            if (novoHistorioIrregularidade != null)
                return;

            novoHistorioIrregularidade = new Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade()
            {
                ControleDocumento = controleDocumento,
                DataIrregularidade = DateTime.Now,
                MotivoIrregularidade = motivoIrregularidade != null ? motivoIrregularidade : null,
                Irregularidade = motivoIrregularidade != null ? motivoIrregularidade.Irregularidade : null,
                Observacao = "Irregularidade criada na validação OCR da GNRE",
                SituacaoIrregularidade = SituacaoIrregularidade.AguardandoAprovacao
            };

            repositorioHistoricoIrregularidade.Inserir(novoHistorioIrregularidade);
        }

        #endregion
    }
}
