//using SGTAdmin.Controllers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//
//using System.Web.Configuration;
//using Microsoft.AspNetCore.Mvc;
//using System.Web.Script.Serialization;

//namespace SGT.WebAdmin.Controllers.NFe
//{
//    [CustomAuthorize("NFe/GerarCTePorNFe")]
//    public class GerarCTePorNFeController : BaseController
//    {

//        #region Métodos Globais

//        public async Task<IActionResult> Index()
//        {
//            return View();
//        }

//        [AllowAuthenticate]
//        [AcceptVerbs("POST")]
//        public async Task<IActionResult> Pesquisar()
//        {
//            //try
//            //{
//            //    Repositorio.UsuarioCTe repUsuarioCTe = new Repositorio.UsuarioCTe(_conexao.StringConexao);

//            //    Models.Grid.Grid grid = new Models.Grid.Grid(Request);

//            //    grid.AdicionarCabecalho("Codigo", false);
//            //    grid.AdicionarCabecalho("CodigoEmpresa", false);
//            //    grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.left, false);
//            //    grid.AdicionarCabecalho("Série", "Serie", 4, Models.Grid.Align.left, false);
//            //    grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, false);
//            //    grid.AdicionarCabecalho("Empresa", "NomeFantasia", 25, Models.Grid.Align.left, false);
//            //    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, false);
//            //    grid.AdicionarCabecalho("Retorno SEFAZ", "RetornoSefaz", 25, Models.Grid.Align.left, false);
//            //    grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 10, Models.Grid.Align.right, false);

//            //    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repUsuarioCTe.ConsultarPorUsuario(this.Empresa.Codigo, this.Usuario.Codigo, grid.inicio, grid.limite);
//            //    int countCTes = repUsuarioCTe.ContarConsultaPorUsuario(this.Empresa.Codigo, this.Usuario.Codigo);

//            //    grid.AdicionaRows((from obj in listaCTes
//            //                       select new
//            //                       {
//            //                           obj.Codigo,
//            //                           CodigoEmpresa = obj.Empresa.Codigo,
//            //                           obj.Numero,
//            //                           Serie = obj.Serie.Numero,
//            //                           DataEmissao = string.Format("{0:dd/MM/yyyy}", obj.DataEmissao.Value),
//            //                           obj.Empresa.NomeFantasia,
//            //                           obj.DescricaoStatus,
//            //                           RetornoSefaz = obj.MensagemStatus != null ? obj.MensagemStatus.MensagemDoErro : obj.MensagemRetornoSefaz,
//            //                           ValorFrete = obj.ValorFrete.ToString("n2")
//            //                       }).ToList());

//            //    grid.setarQuantidadeTotal(countCTes);

//            //    return new JsonpResult(grid);
//            //}
//            //catch (Exception ex)
//            //{
//            //    Servicos.Log.TratarErro(ex);
//            //    return new JsonpResult(false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
//            //}

//            return new JsonpResult(false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
//        }

//        [AcceptVerbs("POST")]
//        public async Task<IActionResult> GerarCTePorListaNFe()
//        {
//            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
//            try
//            {
//                List<Dominio.ObjetosDeValor.NFeAdmin> documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.NFeAdmin>>(Request.Params("Documentos"));

//                DateTime dataEmissao = DateTime.Now;
//                DateTime.TryParseExact(Request.Params("DataEmissao"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

//                decimal valorFrete, valorTotalMercadoria = 0;
//                decimal.TryParse(Request.Params("ValorFrete"), out valorFrete);
//                decimal.TryParse(Request.Params("ValorTotalMercadoria"), out valorTotalMercadoria);

//                string observacao = Request.Params("Observacao");

//                return this.GerarCTePorListaNFe(documentos, valorFrete, valorTotalMercadoria, observacao, dataEmissao, unitOfWork);
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao gerar o CT-e.");
//            }
//            finally
//            {
//                unitOfWork.Dispose();
//            }
//        }

//        [AcceptVerbs("POST")]
//        public async Task<IActionResult> ObterDocumentoParaGeracao()
//        {
//            string fileName = string.Empty;

//            try
//            {
//                fileName = Request.Files[0].FileName;

//                object nfe = MultiSoftware.NFe.Servicos.Leitura.Ler(Request.Files[0].InputStream);

//                if (nfe != null)
//                    return new JsonpResult(nfe);
//                else
//                    return new JsonpResult(false, "Não foi possível ler a nota fiscal eletrônica " + fileName + ".");
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return new JsonpResult(false, "Ocorreu uma falha ao ler a nota fiscal eletrônica.");
//            }
//        }

//        #endregion

//        #region Métodos Privados

//        private IActionResult GerarCTePorListaNFe(List<Dominio.ObjetosDeValor.NFeAdmin> documentos, decimal valorFrete, decimal valorTotalMercadoria, string observacao, DateTime dataEmissao, Repositorio.UnitOfWork unitOfWork)
//        {
//            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_conexao.StringConexao);
//            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorEmpresaPai(this.Empresa.Codigo, documentos[0].NFe2 != null ? documentos[0].NFe2.NFe.infNFe.transp.transporta.Item : documentos[0].NFe3.NFe.infNFe.transp.transporta.Item);

//            if (empresa == null)
//                return new JsonpResult(false, "Empresa emissora não encontrada.");

//            if (empresa.Configuracao == null)
//                return new JsonpResult(false, "Configurações da empresa emissora não encontradas.");

//            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(_conexao.StringConexao);

//            foreach (var documento in documentos)
//            {
//                string chaveNFe = "";

//                if (documento.NFe2 != null)
//                    chaveNFe = documento.NFe2.protNFe.infProt.chNFe;
//                else
//                    chaveNFe = documento.NFe3.protNFe.infProt.chNFe;

//                List<int> numerosCTEsUtilizados = repDocumentos.BuscarNumeroDosCTes(empresa.Codigo, chaveNFe, new string[] { "A", "E", "P" });

//                if (numerosCTEsUtilizados.Count() > 0)
//                    return new JsonpResult(false, "A NF-e " + chaveNFe + " já foi utilizada no CT-e número " + numerosCTEsUtilizados[0].ToString());
//            }

//            if (string.IsNullOrWhiteSpace(observacao))
//                observacao = empresa.Configuracao.ObservacaoCTeNormal;
//            else if (!string.IsNullOrWhiteSpace(empresa.Configuracao.ObservacaoCTeNormal))
//                observacao = string.Concat(observacao, " / ", empresa.Configuracao.ObservacaoCTeNormal);

//            if ((documentos[0].NFe2 != null && documentos[0].NFe2.NFe.infNFe.transp.Items != null) || (documentos[0].NFe3 != null && documentos[0].NFe3.NFe.infNFe.transp.Items != null))
//            {
//                observacao += " - Veículos: ";

//                object[] items = documentos[0].NFe2 != null ? documentos[0].NFe2.NFe.infNFe.transp.Items : documentos[0].NFe3.NFe.infNFe.transp.Items;

//                foreach (var item in items)
//                    observacao += ((Newtonsoft.Json.Linq.JObject)item).Property("placa").Value.ToString() + ", ";

//                observacao = observacao.Remove(observacao.Length - 2);
//            }

//            Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

//            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoCTe.GerarCTePorListaNFe(documentos, empresa.Codigo, dataEmissao, valorFrete, valorTotalMercadoria, observacao, true, this.Usuario);

//            if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin || this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador)
//            {
//                Repositorio.UsuarioCTe repUsuarioCTe = new Repositorio.UsuarioCTe(_conexao.StringConexao);

//                Dominio.Entidades.UsuarioCTe usuarioCTe = new Dominio.Entidades.UsuarioCTe();

//                usuarioCTe.CTe = cte;
//                usuarioCTe.Usuario = this.Usuario;

//                repUsuarioCTe.Inserir(usuarioCTe);
//            }

//            Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Gerou o CT-e por lista de NF-e.", unitOfWork);

//            if (!servicoCTe.Emitir(cte.Codigo, empresa.Codigo))
//                return new JsonpResult(false, "O CT-e foi salvo, porém, ocorreu uma falha ao emiti-lo.");

//            if (!this.AdicionarCTeNaFilaDeConsulta(cte))
//                return new JsonpResult(false, "Não foi possível adicionar o CT-e na fila de consulta.");

//            return new JsonpResult(true, "CT-e gerado com sucesso.");
//        }

//        private bool AdicionarCTeNaFilaDeConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
//        {
//            try
//            {
//                string postData = "CodigoCTe=" + cte.Codigo;
//                byte[] bytes = Encoding.UTF8.GetBytes(postData);

//                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebServiceConsultaCTe, "/IntegracaoCTe/AdicionarNaFilaDeConsulta"));

//                request.Method = "POST";
//                request.ContentType = "application/x-www-form-urlencoded";
//                request.ContentLength = bytes.Length;

//                Stream requestStream = request.GetRequestStream();
//                requestStream.Write(bytes, 0, bytes.Length);

//                WebResponse response = request.GetResponse();

//                Stream stream = response.GetResponseStream();

//                StreamReader reader = new StreamReader(stream);
//                var result = reader.ReadToEnd();

//                stream.Dispose();
//                reader.Dispose();

//                JavaScriptSerializer serializer = new JavaScriptSerializer();
//                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

//                return (bool)retorno["Sucesso"];
//            }
//            catch (Exception ex)
//            {
//                Servicos.Log.TratarErro(ex);
//                return false;
//            }
//        }

//        #endregion
//    }
//}
