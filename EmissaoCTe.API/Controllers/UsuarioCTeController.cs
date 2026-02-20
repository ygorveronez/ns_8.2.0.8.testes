using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EmissaoCTe.API.Controllers
{
    public class UsuarioCTeController : ApiController
    {
        #region Propriedades

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("geracaodectepornfe.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int numeroCTe = 0;
                int serieCTe = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["NumeroCTe"], out numeroCTe);
                int.TryParse(Request.Params["SerieCTe"], out serieCTe);

                string empresa = Request.Params["Empresa"];

                DateTime data;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.AssumeLocal, out data);

                Repositorio.UsuarioCTe repUsuarioCTe = new Repositorio.UsuarioCTe(unitOfWork);

                List<Dominio.Entidades.UsuarioCTe> listaCTes = repUsuarioCTe.ConsultarPorUsuario(this.EmpresaUsuario.Codigo, this.Usuario.Codigo, numeroCTe, serieCTe, data, empresa, inicioRegistros, 50);
                int countCTes = repUsuarioCTe.ContarConsultaPorUsuario(this.EmpresaUsuario.Codigo, this.Usuario.Codigo, numeroCTe, serieCTe, data, empresa);

                var retorno = from obj in listaCTes
                              select new
                              {
                                  Codigo = obj.CTe != null ? obj.CTe.Codigo : obj.NFSe.Codigo,
                                  CodigoEmpresa = obj.CTe != null ? obj.CTe.Empresa.Codigo : obj.NFSe.Empresa.Codigo,
                                  Tipo = obj.CTe != null ? "CT-e" : "NFs-e",
                                  Numero = obj.CTe != null ? obj.CTe.Numero : obj.NFSe.Numero,
                                  Serie = obj.CTe != null ? obj.CTe.Serie.Numero : obj.NFSe.Serie.Numero,
                                  DataEmissao = obj.CTe != null ? string.Format("{0:dd/MM/yyyy}", obj.CTe.DataEmissao.Value) : obj.NFSe.DataEmissao.ToString("dd/MM/yyyy"),
                                  RazaoSocial = obj.CTe != null ? obj.CTe.Empresa.RazaoSocial : obj.NFSe.Empresa.RazaoSocial,
                                  DescricaoStatus = obj.CTe != null ? obj.CTe.DescricaoStatus : obj.NFSe.DescricaoStatus,
                                  RetornoSefaz = obj.CTe != null ? obj.CTe.MensagemStatus != null ? obj.CTe.MensagemStatus.MensagemDoErro : obj.CTe.MensagemRetornoSefaz : obj.NFSe.RPS != null ? obj.NFSe.RPS.MensagemRetorno : string.Empty,
                                  ValorFrete = obj.CTe != null ? obj.CTe.ValorFrete.ToString("n2") : obj.NFSe.ValorServicos.ToString("n2")
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "CodigoEmpresa", "Tipo|4", "Núm.|6", "Série|4", "Emissão|10", "Empresa|20", "Status|10", "Retorno Sefaz|25", "Valor Frete|10" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult GerarCTePorListaNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.NFeAdmin> documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.NFeAdmin>>(Request.Params["Documentos"]);

                DateTime dataEmissao = DateTime.Now;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                decimal valorFrete, valorTotalMercadoria = 0;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorTotalMercadoria"], out valorTotalMercadoria);

                string observacao = Request.Params["Observacao"];

                return this.GerarPorListaNFe(documentos, valorFrete, valorTotalMercadoria, observacao, dataEmissao, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metodos Privados

        private ActionResult GerarPorListaNFe(List<Dominio.ObjetosDeValor.NFeAdmin> documentos, decimal valorFrete, decimal valorTotalMercadoria, string observacao, DateTime dataEmissao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorEmpresaPai(this.EmpresaUsuario.Codigo, documentos[0].NFe2 != null ? documentos[0].NFe2.NFe.infNFe.transp.transporta.Item : documentos[0].NFe3 != null ? documentos[0].NFe3.NFe.infNFe.transp.transporta.Item : documentos[0].NFe4.NFe.infNFe.transp.transporta.Item);

            if (empresa == null)
                return Json<bool>(false, false, "Empresa emissora não encontrada.");

            if (empresa.Configuracao == null)
                return Json<bool>(false, false, "Configurações da empresa emissora não encontradas.");

            if (empresa.Status == "I")
                return Json<bool>(false, false, "Empresa emissora não está ativa para emissão.");

            if (empresa.StatusFinanceiro == "B")
                return Json<bool>(true, false, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(unitOfWork);

            string tipoDocumento = "";

            if (documentos.FirstOrDefault().NFe2 != null)
                tipoDocumento = documentos.FirstOrDefault().NFe2.NFe.infNFe.emit.enderEmit.cMun == documentos.FirstOrDefault().NFe2.NFe.infNFe.dest.enderDest.cMun && !string.IsNullOrEmpty(empresa.Configuracao.SerieRPSNFSe) ? "NF-e" : "CT-e";
            else if (documentos.FirstOrDefault().NFe3 != null)
                tipoDocumento = documentos.FirstOrDefault().NFe3.NFe.infNFe.emit.enderEmit.cMun == documentos.FirstOrDefault().NFe3.NFe.infNFe.dest.enderDest.cMun && !string.IsNullOrEmpty(empresa.Configuracao.SerieRPSNFSe) ? "NF-e" : "CT-e";
            else
                tipoDocumento = documentos.FirstOrDefault().NFe4.NFe.infNFe.emit.enderEmit.cMun == documentos.FirstOrDefault().NFe4.NFe.infNFe.dest.enderDest.cMun && !string.IsNullOrEmpty(empresa.Configuracao.SerieRPSNFSe) ? "NF-e" : "CT-e";

            foreach (var documento in documentos)
            {
                string chaveNFe = "";

                if (documento.NFe2 != null)
                    chaveNFe = documento.NFe2.protNFe.infProt.chNFe;
                else if (documento.NFe3 != null)
                    chaveNFe = documento.NFe3.protNFe.infProt.chNFe;
                else
                    chaveNFe = documento.NFe4.protNFe.infProt.chNFe;

                List<int> numerosCTEsUtilizados = repDocumentos.BuscarNumeroDosCTes(empresa.Codigo, chaveNFe, new string[] { "A", "E", "P" });

                if (numerosCTEsUtilizados.Count() > 0)
                    return Json<bool>(false, false, "A NF-e " + chaveNFe + " já foi utilizada no CT-e número " + numerosCTEsUtilizados[0].ToString());
            }

            if (string.IsNullOrWhiteSpace(observacao))
                observacao = empresa.Configuracao.ObservacaoCTeNormal;
            else if (!string.IsNullOrWhiteSpace(empresa.Configuracao.ObservacaoCTeNormal))
                observacao = string.Concat(observacao, " / ", empresa.Configuracao.ObservacaoCTeNormal);

            List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();

            if ((documentos[0].NFe2 != null && documentos[0].NFe2.NFe.infNFe.transp.Items != null) || (documentos[0].NFe3 != null && documentos[0].NFe3.NFe.infNFe.transp.Items != null) || (documentos[0].NFe4 != null && documentos[0].NFe4.NFe.infNFe.transp.Items != null))
            {
                object[] items = documentos[0].NFe2 != null ? documentos[0].NFe2.NFe.infNFe.transp.Items : documentos[0].NFe3 != null ? documentos[0].NFe3.NFe.infNFe.transp.Items : documentos[0].NFe4.NFe.infNFe.transp.Items;

                string[] placas = (from item in items select ((Newtonsoft.Json.Linq.JObject)item).Property("placa").Value.ToString()).Distinct().ToArray();

                observacao += " - Veículos: " + string.Join(", ", placas);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                veiculos = repVeiculo.BuscarPorPlaca(empresa.Codigo, placas);
            }

            if (tipoDocumento == "NF-e")
            {
                Servicos.NFSe servicoNFe = new Servicos.NFSe(unitOfWork);

                Dominio.Entidades.NFSe nfse = servicoNFe.GerarNFSePorListaNFe(documentos, empresa.Codigo, dataEmissao, valorFrete, valorTotalMercadoria, observacao, true, this.Usuario, veiculos);

                if (nfse == null)
                    return Json<bool>(false, false, "Ocorreu uma falha ao emitir NFS-e.");

                if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
                {
                    Repositorio.UsuarioCTe repUsuarioCTe = new Repositorio.UsuarioCTe(unitOfWork);

                    Dominio.Entidades.UsuarioCTe usuarioCTe = new Dominio.Entidades.UsuarioCTe();

                    usuarioCTe.NFSe = nfse;
                    usuarioCTe.Usuario = this.Usuario;

                    repUsuarioCTe.Inserir(usuarioCTe);
                }

                if (!servicoNFe.Emitir(nfse))
                    return Json<bool>(false, false, "A NFS-e nº " + nfse.Numero.ToString() + " da empresa " + nfse.Empresa.CNPJ + " foi salva, porém, ocorreu uma falha ao emiti-la.");

                if (!this.AdicionarNFSeNaFilaDeConsulta(nfse))
                    return Json<bool>(false, false, "Não foi possível adicionar o NFS-e na fila de consulta.");

            }
            else
            {
                Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoCTe.GerarCTePorListaNFe(documentos, empresa.Codigo, dataEmissao, valorFrete, valorTotalMercadoria, observacao, true, this.Usuario, veiculos);

                if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
                {
                    Repositorio.UsuarioCTe repUsuarioCTe = new Repositorio.UsuarioCTe(unitOfWork);

                    Dominio.Entidades.UsuarioCTe usuarioCTe = new Dominio.Entidades.UsuarioCTe();

                    usuarioCTe.CTe = cte;
                    usuarioCTe.Usuario = this.Usuario;

                    repUsuarioCTe.Inserir(usuarioCTe);
                }

                if (!servicoCTe.Emitir(cte.Codigo, empresa.Codigo))
                    return Json<bool>(false, false, "O CT-e foi salvo, porém, ocorreu uma falha ao emiti-lo.");

                if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                    return Json<bool>(false, false, "Não foi possível adicionar o CT-e na fila de consulta.");
            }

            return Json<bool>(true, true);
        }

        private bool AdicionarCTeNaFilaDeConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                if (cte.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    return true;

                string postData = "CodigoCTe=" + cte.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["UriSistemaEmissaoCTe"], "/IntegracaoCTe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private bool AdicionarNFSeNaFilaDeConsulta(Dominio.Entidades.NFSe nfse)
        {
            try
            {
                string postData = "CodigoNFSe=" + nfse.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["UriSistemaEmissaoCTe"], "/IntegracaoNFSe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        #endregion
    }
}
