using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioNFSeEmitidasController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int numeroInicial, numeroFinal;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                double cpfCnpjTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Tomador"]), out cpfCnpjTomador);

                Dominio.Enumeradores.StatusNFSe ? status = null;
                Dominio.Enumeradores.StatusNFSe statusAux;

                if (Enum.TryParse<Dominio.Enumeradores.StatusNFSe>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string tipoArquivo = Request.Params["TipoArquivo"];
                
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                
                Dominio.Entidades.Cliente cliente = cpfCnpjTomador > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioNFSeEmitidasPorEmbarcador> listaNFSes = repNFSe.RelatorioNFSeEmitidas(this.EmpresaUsuario.Codigo, cliente != null ? cliente.CPF_CNPJ_SemFormato : string.Empty, dataInicial, dataFinal, numeroInicial, numeroFinal, status);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataFinal != DateTime.MinValue ? dataFinal.ToString("dd/MM/yyyy") : "99/99/9999")));
                parametros.Add(new ReportParameter("Tomador", cliente != null ? cliente.CPF_CNPJ_Formatado + " - " + cliente.Nome : "Todos"));
                parametros.Add(new ReportParameter("Status", status != null ? status.Value.ToString("G") : "Todos"));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Documentos", listaNFSes));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioNFSesEmitidas.rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioNFSesEmitidas." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadLoteXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int numeroInicial, numeroFinal;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                double cpfCnpjTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Tomador"]), out cpfCnpjTomador);

                Dominio.Enumeradores.StatusNFSe? status = null;
                Dominio.Enumeradores.StatusNFSe statusAux;

                if (Enum.TryParse<Dominio.Enumeradores.StatusNFSe>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                Dominio.Entidades.Cliente cliente = cpfCnpjTomador > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;
               
                List<int> listaCodigosNFSe = repNFSe.BuscarListaCodigos(this.EmpresaUsuario.Codigo, cliente != null ? cliente.CPF_CNPJ_SemFormato : string.Empty, dataInicial, dataFinal, numeroInicial, numeroFinal, status);

                if (listaCodigosNFSe.Count > 500)
                    return Json<bool>(false, false, string.Concat("Quantidade de NFSe para geração de lote inválida (", listaCodigosNFSe.Count, "). É permitido o download de um lote de no máximo 500."));

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);


                return Arquivo(svcNFSe.ObterLoteDeXML(listaCodigosNFSe, this.EmpresaUsuario.Codigo, unitOfWork), "application/zip", "LoteXML.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de NFS-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadLotePDF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int numeroInicial, numeroFinal;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                double cpfCnpjTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Tomador"]), out cpfCnpjTomador);

                Dominio.Enumeradores.StatusNFSe? status = null;
                Dominio.Enumeradores.StatusNFSe statusAux;

                if (Enum.TryParse<Dominio.Enumeradores.StatusNFSe>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                Dominio.Entidades.Cliente cliente = cpfCnpjTomador > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;

                List<int> listaCodigosNFSe = repNFSe.BuscarListaCodigos(this.EmpresaUsuario.Codigo, cliente != null ? cliente.CPF_CNPJ_SemFormato : string.Empty, dataInicial, dataFinal, numeroInicial, numeroFinal, status);

                if (listaCodigosNFSe.Count > 500)
                    return Json<bool>(false, false, string.Concat("Quantidade de NFSe para geração de lote inválida (", listaCodigosNFSe.Count, "). É permitido o download de um lote de no máximo 500."));

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);


                return Arquivo(svcNFSe.ObterLoteDeDANFSE(listaCodigosNFSe, unitOfWork), "application/zip", "LotePDF.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o lote de NFS-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
