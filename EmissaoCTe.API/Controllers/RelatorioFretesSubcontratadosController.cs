using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioFretesSubcontratadosController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataEntradaInicio, dataEntradaFim, dataEntregaInicio, dataEntregaFim;
                DateTime.TryParseExact(Request.Params["DataEntradaInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntradaInicio);
                DateTime.TryParseExact(Request.Params["DataEntradaFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntradaFim);
                DateTime.TryParseExact(Request.Params["DataEntregaInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntregaInicio);
                DateTime.TryParseExact(Request.Params["DataEntregaFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntregaFim);

                double cnpjParceiro;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Parceiro"]), out cnpjParceiro);

                string relatorio = Request.Params["TipoRelatorio"];

                Dominio.Enumeradores.TipoFreteSubcontratado? tipo = null;
                Dominio.Enumeradores.TipoFreteSubcontratado tipoAux;

                if (Enum.TryParse<Dominio.Enumeradores.TipoFreteSubcontratado>(Request.Params["Tipo"], out tipoAux))
                    tipo = tipoAux;

                Dominio.Enumeradores.StatusFreteSubcontratado? status = null;
                Dominio.Enumeradores.StatusFreteSubcontratado statusAux;

                if (Enum.TryParse<Dominio.Enumeradores.StatusFreteSubcontratado>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);

                Dominio.Entidades.Cliente parceiro = cnpjParceiro > 0f ? repCliente.BuscarPorCPFCNPJ(cnpjParceiro) : null;

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioFretesSubcontratados> fretesSubcontratados = repFreteSubcontratado.Relatorio(this.EmpresaUsuario.Codigo, cnpjParceiro, dataEntradaInicio, dataEntradaFim, dataEntregaInicio, dataEntregaFim, tipo, status);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("EnderecoEmpresa", string.Concat(this.EmpresaUsuario.Endereco, "  " , this.EmpresaUsuario.Localidade.Descricao, "-", this.EmpresaUsuario.Localidade.Estado.Sigla)));
                parametros.Add(new ReportParameter("CnpjIE", string.Concat("CNPJ:" , this.EmpresaUsuario.CNPJ_Formatado, " IE:", this.EmpresaUsuario.InscricaoEstadual)));
                parametros.Add(new ReportParameter("Telefone", string.Concat("Fone:", this.EmpresaUsuario.Telefone)));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataEntradaInicio != DateTime.MinValue ? dataEntradaInicio.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataEntradaFim != DateTime.MinValue ? dataEntradaFim.ToString("dd/MM/yyyy") : "99/99/9999")));
                parametros.Add(new ReportParameter("PeriodoEntrega", string.Concat("De ", dataEntregaInicio != DateTime.MinValue ? dataEntregaInicio.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataEntregaFim != DateTime.MinValue ? dataEntregaFim.ToString("dd/MM/yyyy") : "99/99/9999")));
                parametros.Add(new ReportParameter("Parceiro", parceiro != null ? parceiro.CPF_CNPJ_Formatado + " - " + parceiro.Nome : "Todos"));
                parametros.Add(new ReportParameter("Tipo", tipo != null ? tipo.Value.ToString("G") : "Todos"));
                parametros.Add(new ReportParameter("Status", status != null ? status.Value.ToString("G") : "Todos"));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("FretesSubcontratados", fretesSubcontratados));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/"+ relatorio + ".rdlc", tipoArquivo, parametros, dataSources);
                unitOfWork.Dispose();
                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioFretesSubcontratados." + arquivo.FileNameExtension);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unitOfWork.Dispose();

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }
    }
}