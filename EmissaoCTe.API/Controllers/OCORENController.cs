using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class OCORENController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao(string arquivo)
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals(arquivo) select obj).FirstOrDefault();
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
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);
                string placa = Request.Params["Placa"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<Dominio.ObjetosDeValor.ConsultaCTe> ctes = repCTe.ConsultarPorDuplicata(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, 0, 0, placa, string.Empty, cpfCnpjRemetente, string.Empty, new string[] { "A","S" }, Dominio.Enumeradores.TipoCTE.Todos, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, string.Empty, string.Empty, inicioRegistros, 50, 0, false);
                int countCTes = repCTe.ContarConsultaPorDuplicata(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, 0, 0, placa, string.Empty, cpfCnpjRemetente, string.Empty, new string[] { "A", "S" }, Dominio.Enumeradores.TipoCTE.Todos, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, string.Empty, string.Empty, 0, false);

                var retorno = (from cte in ctes
                               select new
                               {
                                   cte.Codigo,
                                   Numero = cte.Numero.ToString(),
                                   Serie = cte.Serie.ToString(),
                                   Documento = cte.Documento,
                                   Remetente = cte.Remetente != null ? cte.Remetente.Nome : string.Empty,
                                   LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Localidade != null ? cte.Remetente.Localidade.Estado.Sigla + " / " + cte.Remetente.Localidade.Descricao : cte.Remetente.Cidade : string.Empty,
                                   Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                                   LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Localidade != null ? cte.Destinatario.Localidade.Estado.Sigla + " / " + cte.Destinatario.Localidade.Descricao : cte.Remetente.Cidade : string.Empty,
                                   ValorFrete = cte.Valor.ToString("n2")
                               }).ToList();

                return Json(retorno, true, "", new string[] { "Codigo", "Número|10", "Série|8", "Documento|5", "Remetente|15", "Cidade Rem.|15", "Destinatário|15", "Cidade Dest.|10", "Valor Frete|12" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SelecionarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);
                string placa = Request.Params["Placa"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int countCTes = repCTe.ContarConsultaPorDuplicata(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, 0, 0, placa, string.Empty, cpfCnpjRemetente, string.Empty, new string[] { "A", "S" }, Dominio.Enumeradores.TipoCTE.Todos, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, string.Empty, string.Empty, 0, false);
                List<Dominio.ObjetosDeValor.ConsultaCTe> ctes = repCTe.ConsultarPorDuplicata(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, 0, 0, placa, string.Empty, cpfCnpjRemetente, string.Empty, new string[] { "A", "S" }, Dominio.Enumeradores.TipoCTE.Todos, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, string.Empty, string.Empty, 0, countCTes, 0, false);

                var retorno = (from cte in ctes
                               select new
                               {
                                   cte.Codigo,
                                   Numero = cte.Numero.ToString(),
                                   ValorFrete = cte.Valor.ToString("n2")
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao selecionar os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult PreparaGerarao()
        {
            /* Devido a um problema na selecao de muitos CTes, o tamanho do GET excedia o limite
             * 
             * Para solução do problema:
             * - A requisição do download é enviada via POST
             * - Informações são salvas numa sessão
             * - Retorna o nome da sessão
             * - Método do download envia essa sessão
             */
            string idsRequisicao = Request.Params["CTes"];
            string nomeSessao = "OCOREN" + DateTime.Now.ToString("HHmmss");

            Session[nomeSessao] = idsRequisicao;

            return Json(nomeSessao, true);
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult Gerar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao("ocoren.aspx") == null || this.Permissao("ocoren.aspx").PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);
                string nomeSessao = Request.Params["Sessao"];

                int codigoVeiculo = 0;
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                int codigoLayout = 0;
                int.TryParse(Request.Params["Versao"], out codigoLayout);

                List<int> codigosCTes = JsonConvert.DeserializeObject<List<int>>(Session[nomeSessao].ToString());

                Session.Remove(nomeSessao);

                Repositorio.LayoutEDI repLayout = new Repositorio.LayoutEDI(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.LayoutEDI layout = repLayout.BuscarPorCodigoETipo(codigoLayout, Dominio.Enumeradores.TipoLayoutEDI.OCOREN);

                if (layout == null)
                    return Json<bool>(false, false, "Layout do arquivo não encontrado.");

                System.IO.MemoryStream arquivo = this.GerarOCOREN(cpfCnpjRemetente, dataInicial, dataFinal, layout, codigoVeiculo, codigosCTes, unitOfWork);

                Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork);
                string nomeArquivo = string.Empty;
                if (!string.IsNullOrWhiteSpace(layout.Nomenclatura))
                {
                    Dominio.Entidades.Cliente cliente = null;
                    double.TryParse(cpfCnpjRemetente, out double cnpjCliente);
                    if (cnpjCliente > 0)
                        cliente = repCliente.BuscarPorCPFCNPJ(cnpjCliente);

                    nomeArquivo = svcEDI.ObterNomenclaturaLayoutEDI(layout.Nomenclatura, this.EmpresaUsuario, cliente, string.Empty, DateTime.Now);
                }

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    if (string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                        nomeArquivo = string.Concat("OCOREN_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"));
                    else
                        nomeArquivo = string.Concat("OCOREN_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"));
                }

                if (string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                    return Arquivo(arquivo, "application/zip", string.Concat(nomeArquivo, ".zip"));
                else
                    return Arquivo(arquivo, "text/plain", string.Concat(nomeArquivo, ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo OCOREN.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult GerarNFSe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao("ocorennfse.aspx") == null || this.Permissao("ocorennfse.aspx").PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);

                int codigoLayout = 0;
                int.TryParse(Request.Params["Versao"], out codigoLayout);

                Repositorio.LayoutEDI repLayout = new Repositorio.LayoutEDI(unitOfWork);
                Dominio.Entidades.LayoutEDI layout = repLayout.BuscarPorCodigoETipo(codigoLayout, Dominio.Enumeradores.TipoLayoutEDI.OCOREN_NFS);

                if (layout == null)
                    return Json<bool>(false, false, "Layout do arquivo não encontrado.");

                System.IO.MemoryStream arquivo = this.GerarOCORENNFSe(cpfCnpjRemetente, dataInicial, dataFinal, layout, unitOfWork);

                if (string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                    return Arquivo(arquivo, "application/zip", string.Concat("OCOREN_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".zip"));
                else
                    return Arquivo(arquivo, "text/plain", string.Concat("OCOREN_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo OCOREN.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult GerarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao("ocorennfse.aspx") == null || this.Permissao("ocorennfse.aspx").PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]), out double cpfCnpjRemetente);

                int codigoLayout = 0;
                int.TryParse(Request.Params["Versao"], out codigoLayout);

                Repositorio.LayoutEDI repLayout = new Repositorio.LayoutEDI(unitOfWork);
                Dominio.Entidades.LayoutEDI layout = repLayout.BuscarPorCodigoETipo(codigoLayout, Dominio.Enumeradores.TipoLayoutEDI.OCOREN_NFS);

                if (layout == null)
                    return Json<bool>(false, false, "Layout do arquivo não encontrado.");

                System.IO.MemoryStream arquivo = this.GerarOCORENNFe(cpfCnpjRemetente, dataInicial, dataFinal, layout, unitOfWork);

                if (cpfCnpjRemetente > 0)
                    return Arquivo(arquivo, "application/zip", string.Concat("OCOREN_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".zip"));
                else
                    return Arquivo(arquivo, "text/plain", string.Concat("OCOREN_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo OCOREN.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private System.IO.MemoryStream GerarOCOREN(string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.LayoutEDI layout, int codigoVeiculo, List<int> codigosCTes, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.GeracaoEDI svcEDI = null;

            if (string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                svcEDI = new Servicos.GeracaoEDI(unitOfWork, layout, this.EmpresaUsuario, dataInicial, dataFinal, codigoVeiculo, true, codigosCTes, 0, new string[] { "A", "S" }, null);
                return svcEDI.GerarLote();
            }
            else
            {
                svcEDI = new Servicos.GeracaoEDI(unitOfWork, layout, this.EmpresaUsuario, cpfCnpjRemetente, string.Empty, dataInicial, dataFinal, codigoVeiculo, true, codigosCTes, 0, new string[] { "A", "S" }, null);
                return svcEDI.GerarArquivo();
            }
        }

        private MemoryStream GerarOCORENNFSe(string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.LayoutEDI layout, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.OcorrenciaDeNFSe repOcorrencia = new Repositorio.OcorrenciaDeNFSe(unitOfWork);
            Servicos.Embarcador.Integracao.EDI.OCOREN svcOCOREN = new Servicos.Embarcador.Integracao.EDI.OCOREN();
            Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork, layout, this.EmpresaUsuario);

            Dominio.Enumeradores.StatusNFSe[] status = new Dominio.Enumeradores.StatusNFSe[] { Dominio.Enumeradores.StatusNFSe.Autorizado, Dominio.Enumeradores.StatusNFSe.EmDigitacao };

            if (string.IsNullOrWhiteSpace(cpfCnpjRemetente))
            {
                // Prepara criacao do zip
                MemoryStream fZip = new MemoryStream();
                ZipOutputStream zipOStream = new ZipOutputStream(fZip);
                zipOStream.SetLevel(9);

                // Busca todos remetentes do filtro
                List<string> remetentes = repOcorrencia.BuscarRemetentesPorFiltro(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, status, this.EmpresaUsuario.TipoAmbiente);              

                // Itera resultado
                foreach (string remetente in remetentes)
                {
                    // Busca por remetente
                    List<Dominio.Entidades.NFSe> nfes = repOcorrencia.BuscarNFSesPorRemetente(this.EmpresaUsuario.Codigo, remetente, dataInicial, dataFinal, status, this.EmpresaUsuario.TipoAmbiente);
                    Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN edi = svcOCOREN.ConverterParaOCOREN(nfes, unitOfWork);
                    if (edi == null) continue;

                    // Gera EDI
                    MemoryStream ediRemetente = svcEDI.GerarArquivoRecursivo(edi);
                    byte[] arquivo = ediRemetente.GetBuffer();

                    // Adiciona ao ZIP
                    ZipEntry entry = new ZipEntry(string.Concat(Utilidades.String.RemoveAllSpecialCharacters(nfes.FirstOrDefault().Tomador.Nome), " - ", nfes.FirstOrDefault().Tomador.CPF_CNPJ, ".txt"));
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(arquivo, 0, arquivo.Length);
                    zipOStream.CloseEntry();
                }

                // Finaliza encapsulamento do ZIP
                zipOStream.IsStreamOwner = false;
                zipOStream.Close();
                fZip.Position = 0;

                return fZip;
            }
            else
            {
                List<Dominio.Entidades.NFSe> nfes = repOcorrencia.BuscarNFSesPorRemetente(this.EmpresaUsuario.Codigo, cpfCnpjRemetente, dataInicial, dataFinal, status, this.EmpresaUsuario.TipoAmbiente);
                Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN edi = svcOCOREN.ConverterParaOCOREN(nfes, unitOfWork);

                return svcEDI.GerarArquivoRecursivo(edi);
            }
        }

        private MemoryStream GerarOCORENNFe(double cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.LayoutEDI layout, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.OcorrenciaDeNFe repOcorrencia = new Repositorio.OcorrenciaDeNFe(unitOfWork);
            Servicos.Embarcador.Integracao.EDI.OCOREN svcOCOREN = new Servicos.Embarcador.Integracao.EDI.OCOREN();
            Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork, layout, this.EmpresaUsuario);

            if (cpfCnpjRemetente > 0)
            {
                // Prepara criacao do zip
                MemoryStream fZip = new MemoryStream();
                ZipOutputStream zipOStream = new ZipOutputStream(fZip);
                zipOStream.SetLevel(9);

                // Busca todos remetentes do filtro
                List<double> remetentes = repOcorrencia.BuscarRemetentesPorFiltro(this.EmpresaUsuario.Codigo, dataInicial, dataFinal);

                // Itera resultado
                foreach (var remetente in remetentes)
                {
                    // Busca por remetente
                    List<Dominio.Entidades.XMLNotaFiscalEletronica> nfes = repOcorrencia.BuscarNFesPorRemetente(this.EmpresaUsuario.Codigo, remetente, dataInicial, dataFinal);
                    Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN edi = svcOCOREN.ConverterParaOCOREN(nfes, unitOfWork);
                    if (edi == null) continue;

                    // Gera EDI
                    MemoryStream ediRemetente = svcEDI.GerarArquivoRecursivo(edi);
                    byte[] arquivo = ediRemetente.GetBuffer();

                    // Adiciona ao ZIP
                    ZipEntry entry = new ZipEntry(string.Concat(Utilidades.String.RemoveAllSpecialCharacters(nfes.FirstOrDefault().Emitente.Nome), " - ", nfes.FirstOrDefault().Emitente.CPF_CNPJ_SemFormato, ".txt"));
                    entry.DateTime = DateTime.Now;
                    zipOStream.PutNextEntry(entry);
                    zipOStream.Write(arquivo, 0, arquivo.Length);
                    zipOStream.CloseEntry();
                }

                // Finaliza encapsulamento do ZIP
                zipOStream.IsStreamOwner = false;
                zipOStream.Close();
                fZip.Position = 0;

                return fZip;
            }
            else
            {
                List<Dominio.Entidades.XMLNotaFiscalEletronica> nfes = repOcorrencia.BuscarNFesPorRemetente(this.EmpresaUsuario.Codigo, cpfCnpjRemetente, dataInicial, dataFinal);
                Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN edi = svcOCOREN.ConverterParaOCOREN(nfes, unitOfWork);

                return svcEDI.GerarArquivoRecursivo(edi);
            }
        }

        #endregion
    }
}
