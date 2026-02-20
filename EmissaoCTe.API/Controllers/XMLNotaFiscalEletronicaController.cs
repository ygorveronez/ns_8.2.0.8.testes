using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Servicos;
using System.Text.RegularExpressions;

namespace EmissaoCTe.API.Controllers
{
    public class XMLNotaFiscalEletronicaController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataEmissao;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                double cpfCnpjEmitente;
                double.TryParse(Request.Params["CPF_CNPJ_Emitente"], out cpfCnpjEmitente);
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string numeroNota = Request.Params["NumeroNota"];
                if (!string.IsNullOrWhiteSpace(numeroNota))
                {
                    numeroNota = Regex.Replace(numeroNota, "_", "");
                }
                Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unitOfWork);
                IList<Dominio.Entidades.XMLNotaFiscalEletronica> listaXML = repXMLNotaFiscalEletronica.Consultar(this.EmpresaUsuario.Codigo, numeroNota, cpfCnpjEmitente, dataEmissao, inicioRegistros, 50);
                int countXML = repXMLNotaFiscalEletronica.ContarConsulta(EmpresaUsuario.Codigo, numeroNota, cpfCnpjEmitente, dataEmissao);

                var retorno = (from obj in listaXML
                              select new
                              {
                                  CPF_CNPJ_Emitente = obj.Emitente.Tipo == "J" ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.Emitente.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.Emitente.CPF_CNPJ),
                                  CPF_CNPJ_Destinatario = obj.Destinatario.Tipo == "J" ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.Destinatario.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.Destinatario.CPF_CNPJ),
                                  obj.Peso,
                                  obj.Placa,
                                  FormaPagamento = obj.FormaDePagamento,
                                  obj.Numero,
                                  obj.Emitente.Nome,
                                  obj.Chave,
                                  DataEmissao = string.Format("{0:dd/MM/yyyy}", obj.DataEmissao),
                                  Valor = Math.Round(obj.Valor, 2, MidpointRounding.AwayFromZero).ToString("n2")
                              }).ToList();

                return Json(retorno, true, null, new string[] { "CPF_CNPJ_Emitente", "CPF_CNPJ_Destinatario", "Peso", "Placa", "FormaPagamento", "Número|10", "Emitente|28", "Chave NFe|28", "Data Emissão|12", "Valor Total|12" }, countXML);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as notas fiscais. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataEmissao;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                double cpfCnpjEmitente;
                double.TryParse(Request.Params["CPF_CNPJ_Emitente"], out cpfCnpjEmitente);
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string numeroNota = Request.Params["NumeroNota"];
                if (!string.IsNullOrWhiteSpace(numeroNota))
                {
                    numeroNota = Regex.Replace(numeroNota, "_", "");
                }
                Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unitOfWork);
                var listaXML = repXMLNotaFiscalEletronica.Consultar(this.EmpresaUsuario.Codigo, numeroNota, cpfCnpjEmitente, dataEmissao, inicioRegistros, 50);
                int countXML = repXMLNotaFiscalEletronica.ContarConsulta(this.EmpresaUsuario.Codigo, numeroNota, cpfCnpjEmitente, dataEmissao);

                var retorno = from obj in listaXML
                              select new
                              {
                                  obj.Codigo,
                                  CPF_CNPJ_Emitente = obj.Emitente.Tipo == "J" ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.Emitente.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.Emitente.CPF_CNPJ),
                                  CPF_CNPJ_Destinatario = obj.Destinatario.Tipo == "J" ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.Destinatario.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.Destinatario.CPF_CNPJ),
                                  obj.Peso,
                                  obj.Placa,
                                  obj.Numero,
                                  obj.Emitente.Nome,
                                  obj.Chave,
                                  DataEmissao = string.Format("{0:dd/MM/yyyy}", obj.DataEmissao),
                                  Valor = Math.Round(obj.Valor, 2, MidpointRounding.AwayFromZero).ToString("n2")
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "CPF_CNPJ_Emitente", "CPF_CNPJ_Destinatario", "Peso", "Placa", "Número|10", "Emitente|28", "Chave NFe|28", "Data Emissão|12", "Valor Total|12" }, countXML);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as notas fiscais. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarNotasImportadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime.TryParse(Request.Params["DataInicial"], out DateTime dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out DateTime dataFinal);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJEmiente"]), out double cnpjEmitente);
                string nomeEmitente = Request.Params["Emiente"];
                string chave = Utilidades.String.OnlyNumbers(Request.Params["Chave"]);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["fimRegistros"], out int fimRegistros);
                int.TryParse(Request.Params["Numero"], out int numero);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);

                bool? notasSemNFSe = null;
                if (bool.TryParse(Request.Params["NotasSemNFSe"], out bool notasSemNFSeAux))
                    notasSemNFSe = notasSemNFSeAux;


                Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unitOfWork);
                var listaXML = repXMLNotaFiscalEletronica.ConsultarNotasImportadas(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, numero, numeroFinal, chave, cnpjEmitente, nomeEmitente, notasSemNFSe, inicioRegistros, fimRegistros);
                int countXML = repXMLNotaFiscalEletronica.ContarNotasImportadas(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, numero, numeroFinal, chave, cnpjEmitente, nomeEmitente, notasSemNFSe, inicioRegistros, fimRegistros);

                var retorno = from obj in listaXML
                              select new
                              {
                                  obj.Codigo,
                                  Emitente = (obj.Emitente.Tipo == "J" ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.Emitente.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.Emitente.CPF_CNPJ)) + " " + obj.Emitente.Nome,
                                  Destinatario = (obj.Destinatario.Tipo == "J" ? String.Format(@"{0:00\.000\.000\/0000\-00}", obj.Destinatario.CPF_CNPJ) : String.Format(@"{0:000\.000\.000\-00}", obj.Destinatario.CPF_CNPJ)) + " " + obj.Destinatario.Nome,
                                  obj.Numero,
                                  Emissao = string.Format("{0:dd/MM/yyyy}", obj.DataEmissao),
                                  Valor = Math.Round(obj.Valor, 2, MidpointRounding.AwayFromZero).ToString("n2")
                              };

                return Json(retorno, true, null, new string[] { "Código", "Emitente|30", "Destinatario|20", "Número|10", "Emissão|10", "Valor|10" }, countXML);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as notas. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SelecionarTodosNotasImportadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime.TryParse(Request.Params["DataInicial"], out DateTime dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out DateTime dataFinal);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJEmiente"]), out double cnpjEmitente);
                string nomeEmitente = Request.Params["Emiente"];
                string chave = Utilidades.String.OnlyNumbers(Request.Params["Chave"]);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["fimRegistros"], out int fimRegistros);
                int.TryParse(Request.Params["Numero"], out int numero);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);

                bool? notasSemNFSe = null;
                if (bool.TryParse(Request.Params["NotasSemNFSe"], out bool notasSemNFSeAux))
                    notasSemNFSe = notasSemNFSeAux;

                Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unitOfWork);

                if (cnpjEmitente == 0 && numero == 0 && numeroFinal == 0 && dataInicial.Date == DateTime.MinValue && dataFinal.Date == DateTime.MinValue)
                    return Json<bool>(false, false, "É obrigatório informar CNPJ do Emitente, o intervalodo de numeração ou período de emissão.");

                var listaXML = repXMLNotaFiscalEletronica.ConsultarNotasImportadas(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, numero, numeroFinal, chave, cnpjEmitente, nomeEmitente, notasSemNFSe, inicioRegistros, fimRegistros);

                var lista = (from obj in listaXML
                             select new
                             {
                                 obj.Codigo,
                                 obj.Numero
                             }).ToList();

                return Json(lista, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult ObterDocumentoPorXML()
        {
            string fileName = string.Empty;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    Stream stream2 = new MemoryStream();
                    CopyStream(file.InputStream, stream2);

                    StreamReader stream = new StreamReader(file.InputStream, Encoding.GetEncoding("ISO-8859-1"));

                    if (System.IO.Path.GetExtension(file.FileName).ToLower().Equals(".xml"))
                    {
                        Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                        object documento = svcNFe.ObterDocumentoPorXML(file.InputStream, this.EmpresaUsuario.Codigo, this.UsuarioAdministrativo, unitOfWork);

                        if (documento == null)
                            documento = svcNFe.ObterDocumentoPorXMLFrimesa(stream2, this.EmpresaUsuario.Codigo, unitOfWork);

                        if (documento == null)
                            return Json<bool>(false, false, "Nenhuma NF-e foi importada.");

                        return Json(documento, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, "A extensão do arquivo " + file.FileName + " é inválida. Somente a extensão XML é aceita.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao ler o arquivo xml.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDocumento()
        {
            try
            {
                object nfe = MultiSoftware.NFe.Servicos.Leitura.Ler(Request.Files[0].InputStream);

                if (nfe != null)
                    return Json(nfe, true);
                else
                    return Json<bool>(false, false, "Não foi possível ler a nota fiscal eletrônica.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao ler a nota fiscal eletrônica.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterPalavrasChaves()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.PalavrasChaveNFe repPalavrasChave = new Repositorio.PalavrasChaveNFe(unitOfWork);

                List<Dominio.Entidades.PalavrasChaveNFe> palavrasChaves = repPalavrasChave.ConsultarTodas();

                if (palavrasChaves != null)
                    return Json(palavrasChaves, true);
                else
                    return Json<bool>(false, false, "Não foi possível consultar palavras chaves.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar palavras chaves.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BaixarNFeSEFAZ()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz;
                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
                Servicos.CTe svcCTe = new CTe(unitOfWork);
                string caminhoDiretorioConsultasSefaz = System.Configuration.ConfigurationManager.AppSettings["DiretorioConsultasSefaz"];
                if (string.IsNullOrWhiteSpace(caminhoDiretorioConsultasSefaz))
                    return Json(false, false, "Caminho não está configurado.");

                var chaveNFe = Request.Params["ChaveNFe"];
                var chaves = chaveNFe.Split('\n');
                string chavesErro = "";

                for (int i = 0; i < chaves.Count(); i++)
                {
                    if (Utilidades.String.OnlyNumbers(chaves[i].Replace(" ", "")) != "" && Utilidades.String.OnlyNumbers(chaves[i].Replace(" ", "")).Length == 44)
                    {
                        string chave = Utilidades.String.OnlyNumbers(chaves[i].Replace(" ", ""));
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDiretorioConsultasSefaz + "/" + chave + ".txt"))
                        {
                            if (serDocumento.ValidarChave(chave))
                            {
                                var dadosSEFAZ = RequisicaoSEFAZ();

                                if (!string.IsNullOrWhiteSpace((string)dadosSEFAZ.GetType().GetProperty("SessionID").GetValue(dadosSEFAZ, null)))
                                {
                                    requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz();
                                    requisicaoSefaz.VIEWSTATE = (string)dadosSEFAZ.GetType().GetProperty("VIEWSTATE").GetValue(dadosSEFAZ, null);
                                    requisicaoSefaz.EVENTVALIDATION = (string)dadosSEFAZ.GetType().GetProperty("EVENTVALIDATION").GetValue(dadosSEFAZ, null);
                                    requisicaoSefaz.TokenCaptcha = (string)dadosSEFAZ.GetType().GetProperty("token").GetValue(dadosSEFAZ, null);
                                    requisicaoSefaz.SessionID = (string)dadosSEFAZ.GetType().GetProperty("SessionID").GetValue(dadosSEFAZ, null);

                                    string strCaptcha = (string)dadosSEFAZ.GetType().GetProperty("imgCaptcha").GetValue(dadosSEFAZ, null);
                                    strCaptcha = strCaptcha.Replace("data:image/png;base64,", "");
                                    byte[] bytesImagem = System.Convert.FromBase64String(strCaptcha);

                                    DescriptografaCaptcha.Service1Client scvDescriptografaCaptcha = new DescriptografaCaptcha.Service1Client();
                                    string captcha = scvDescriptografaCaptcha.DescriptografaCaptchaByte("multisoftware", "wprano13", 200, bytesImagem);

                                    ConsultaNFe.ConsultaNFeClient consultaNFe = new ConsultaNFe.ConsultaNFeClient();
                                    OperationContextScope scope = new OperationContextScope(consultaNFe.InnerChannel);
                                    MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                                    OperationContext.Current.OutgoingMessageHeaders.Add(header);

                                    ConsultaNFe.RetornoOfConsultaSefazgj5B5PAD consultaSefaz = consultaNFe.ConsultarSefaz(requisicaoSefaz, chave, captcha);

                                    if (consultaSefaz.Status)
                                    {
                                        if (consultaSefaz.Objeto.ConsultaValida)
                                        {
                                            if (!string.IsNullOrWhiteSpace(caminhoDiretorioConsultasSefaz))
                                            {
                                                string arquivoConsultaSefaz = Newtonsoft.Json.JsonConvert.SerializeObject(consultaSefaz);
                                                if (!string.IsNullOrWhiteSpace(arquivoConsultaSefaz))
                                                    svcCTe.SalvarArquivoEmTxt(caminhoDiretorioConsultasSefaz, consultaSefaz.Objeto.NotaFiscal.Chave, arquivoConsultaSefaz);

                                                var retorno = new
                                                {
                                                    Chave = consultaSefaz.Objeto.NotaFiscal.Chave,
                                                    ValorTotal = consultaSefaz.Objeto.NotaFiscal.Valor,
                                                    DataEmissao = consultaSefaz.Objeto.NotaFiscal.DataEmissao.Substring(0, 10),

                                                    Remetente = consultaSefaz.Objeto.NotaFiscal.Emitente.CPFCNPJ,
                                                    RemetenteIE = consultaSefaz.Objeto.NotaFiscal.Emitente.RGIE,
                                                    RemetenteNome = consultaSefaz.Objeto.NotaFiscal.Emitente.RazaoSocial,
                                                    RemetenteLogradouro = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Logradouro,
                                                    RemetenteCEP = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.CEP,
                                                    RemetenteBairro = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Bairro,
                                                    RemetenteNumero = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Numero,
                                                    RemetenteCidade = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Cidade.IBGE,

                                                    Destinatario = consultaSefaz.Objeto.NotaFiscal.Destinatario.CPFCNPJ,
                                                    DestinatarioIE = consultaSefaz.Objeto.NotaFiscal.Destinatario.RGIE,
                                                    DestinatarioNome = consultaSefaz.Objeto.NotaFiscal.Destinatario.RazaoSocial,
                                                    DestinatarioLogradouro = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Logradouro,
                                                    DestinatarioCEP = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.CEP,
                                                    DestinatarioBairro = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Bairro,
                                                    DestinatarioNumero = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Numero,
                                                    DestinatarioCidade = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.IBGE,
                                                    DestinatarioNomeCidadeUF = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.Descricao + " / " + consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.SiglaUF,
                                                    DestinatarioExportacao = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.IBGE == 9999999 ? this.ObterDadosDestinatarioExportacao(consultaSefaz.Objeto.NotaFiscal.Destinatario, unitOfWork) : null,

                                                    Numero = consultaSefaz.Objeto.NotaFiscal.Numero,
                                                    Peso = consultaSefaz.Objeto.NotaFiscal.PesoBruto,
                                                    FormaPagamento = consultaSefaz.Objeto.NotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar ? "1" : "0",
                                                    Placa = consultaSefaz.Objeto.NotaFiscal.Veiculo != null ? consultaSefaz.Objeto.NotaFiscal.Veiculo.Placa : string.Empty,
                                                    Serie = svcCTe.ObterCodigoSerie(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.Localidade.Estado.Sigla, consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Cidade.SiglaUF, consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.SiglaUF, unitOfWork),
                                                    Observacao = consultaSefaz.Objeto.NotaFiscal.InformacoesComplementares,
                                                    Volume = consultaSefaz.Objeto.NotaFiscal.VolumesTotal,
                                                    Excluir = false
                                                };

                                                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                                                Dominio.Entidades.Cliente remetente = ObterCliente(retorno.Remetente, retorno.RemetenteIE, retorno.RemetenteNome, retorno.RemetenteLogradouro, retorno.RemetenteCEP, retorno.RemetenteBairro, retorno.RemetenteNumero, retorno.RemetenteCidade.ToString(), unitOfWork);
                                                Dominio.Entidades.Cliente destinatario = ObterCliente(retorno.Destinatario, retorno.DestinatarioIE, retorno.DestinatarioNome, retorno.DestinatarioLogradouro, retorno.DestinatarioCEP, retorno.DestinatarioBairro, retorno.DestinatarioNumero, retorno.DestinatarioCidade.ToString(), unitOfWork);
                                            }
                                        }
                                        else if (Utilidades.String.OnlyNumbers(chaves[i].Replace(" ", "")) != "")
                                        {
                                            chavesErro += chaves[i] + "\n";
                                        }
                                    }
                                    else
                                    {
                                        return Json<bool>(false, false, consultaSefaz.Mensagem);
                                    }
                                }
                                else
                                    return Json(false, false, "Não foi possível conectar ao site da RECEITA, por favor tente novamente mais tarde.");
                            }
                            else if (Utilidades.String.OnlyNumbers(chaves[i].Replace(" ", "")) != "")
                                chavesErro += chaves[i] + "\n";
                        }
                    }
                    else if (Utilidades.String.OnlyNumbers(chaves[i].Replace(" ", "")) != "")
                        chavesErro += chaves[i] + "\n";
                }

                var retornoBaixa = new
                {
                    ChavesErros = chavesErro
                };

                return Json(retornoBaixa, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json(false, false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private dynamic RequisicaoSEFAZ()
        {
            ConsultaNFe.ConsultaNFeClient consultaNFe = new ConsultaNFe.ConsultaNFeClient();
            OperationContextScope scope = new OperationContextScope(consultaNFe.InnerChannel);
            MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

            ConsultaNFe.RetornoOfRequisicaoSefazgj5B5PAD requisicaoSefaz = consultaNFe.SolicitarRequisicaoSefaz();

            if (requisicaoSefaz.Status)
            {
                var retorno = new
                {
                    NotaAdicionada = false,
                    VIEWSTATE = requisicaoSefaz.Objeto.VIEWSTATE,
                    EVENTVALIDATION = requisicaoSefaz.Objeto.EVENTVALIDATION,
                    imgCaptcha = requisicaoSefaz.Objeto.Captcha,
                    token = requisicaoSefaz.Objeto.TokenCaptcha,
                    SessionID = requisicaoSefaz.Objeto.SessionID
                };
                return retorno;
            }
            else
            {
                var retorno = new
                {
                    NotaAdicionada = false,
                    VIEWSTATE = "",
                    EVENTVALIDATION = "",
                    imgCaptcha = "",
                    token = "",
                    SessionID = ""
                };
                return retorno;
            }
        }

        private Dominio.Entidades.Cliente ObterCliente(string cnpj, string ie, string nome, string rua, string cep, string bairro, string numero, string cidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            double cpfCnpj = 0;
            double.TryParse(Utilidades.String.OnlyNumbers(cnpj), out cpfCnpj);

            if (cpfCnpj > 0)
            {

                bool inserir = false;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    inserir = true;
                }

                cliente.Bairro = bairro;
                cliente.CEP = cep;
                cliente.Complemento = cliente == null ? string.Empty : cliente.Complemento;
                cliente.CPF_CNPJ = cpfCnpj;
                cliente.Endereco = rua;
                if (ie != null && ie != "")
                    cliente.IE_RG = ie;
                else
                {
                    cliente.IE_RG = "ISENTO";
                }
                cliente.InscricaoMunicipal = cliente == null ? string.Empty : cliente.InscricaoMunicipal;
                cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(cidade));
                cliente.Nome = nome;
                cliente.NomeFantasia = nome;
                cliente.Numero = numero;
                cliente.Telefone1 = cliente == null ? string.Empty : cliente.Telefone1;
                cliente.Tipo = Utilidades.String.OnlyNumbers(cnpj).Length == 14 ? "J" : "F";

                if (cliente.Atividade == null)
                    cliente.Atividade = Atividade.ObterAtividade(this.EmpresaUsuario.Codigo, cliente.Tipo, Conexao.StringConexao);

                if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                    cliente.IE_RG = "ISENTO";

                if (inserir)
                {
                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }
                    cliente.DataCadastro = DateTime.Now;
                    cliente.Ativo = true;
                    repCliente.Inserir(cliente);
                }
                else
                {
                    cliente.DataUltimaAtualizacao = DateTime.Now;
                    cliente.Integrado = false;
                    repCliente.Atualizar(cliente);
                }

                return cliente;
            }
            else
                return null;
        }

        private Dominio.ObjetosDeValor.Cliente ObterDadosDestinatarioExportacao(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

            Dominio.ObjetosDeValor.Cliente clienteExportacao = new Dominio.ObjetosDeValor.Cliente();
            clienteExportacao.RazaoSocial = destinatario.RazaoSocial;
            clienteExportacao.NomeFantasia = destinatario.NomeFantasia;
            clienteExportacao.Nome = destinatario.RazaoSocial;
            clienteExportacao.Endereco = destinatario.Endereco.Logradouro;
            clienteExportacao.Bairro = destinatario.Endereco.Bairro;
            clienteExportacao.Cidade = destinatario.Endereco.Cidade.Descricao;
            clienteExportacao.Complemento = destinatario.Endereco.Complemento;
            clienteExportacao.Emails = string.Empty;
            clienteExportacao.Numero = "S/N";
            clienteExportacao.SiglaPais = repPais.BuscarPorCodigo(destinatario.Endereco.Cidade.Pais.CodigoPais).Sigla;
            clienteExportacao.Exportacao = true;

            return clienteExportacao;
        }

        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
            input.Position = 0;
            output.Position = 0;
        }
    }
}
