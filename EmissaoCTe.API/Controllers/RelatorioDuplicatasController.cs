using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;


namespace EmissaoCTe.API.Controllers
{
    public class RelatorioDuplicatasController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataLctoInicial, dataLctoFinal, dataVctoInicial, dataVctoFinal;
                DateTime.TryParseExact(Request.Params["DataLctoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataLctoInicial);
                DateTime.TryParseExact(Request.Params["DataLctoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataLctoFinal);
                DateTime.TryParseExact(Request.Params["DataVctoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVctoInicial);
                DateTime.TryParseExact(Request.Params["DataVctoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVctoFinal);

                int codigoVeiculo1, codigoVeiculo2, codigoVeiculo3, codigoMotorista, ordenacao;
                int.TryParse(Request.Params["Veiculo1"], out codigoVeiculo1);
                int.TryParse(Request.Params["Veiculo2"], out codigoVeiculo2);
                int.TryParse(Request.Params["Veiculo3"], out codigoVeiculo3);
                int.TryParse(Request.Params["Motorista"], out codigoMotorista);
                int.TryParse(Request.Params["Ordenacao"], out ordenacao);

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Pessoa"]), out cpfCnpjPessoa);

                Dominio.Enumeradores.TipoDuplicata? tipo = null;
                Dominio.Enumeradores.TipoDuplicata tipoAux;

                if (Enum.TryParse<Dominio.Enumeradores.TipoDuplicata>(Request.Params["Tipo"], out tipoAux))
                    tipo = tipoAux;

                Dominio.Enumeradores.StatusDuplicata? statusPgto = null;
                Dominio.Enumeradores.StatusDuplicata statusPgtoAux;

                if (Enum.TryParse<Dominio.Enumeradores.StatusDuplicata>(Request.Params["StatusPgto"], out statusPgtoAux))
                    statusPgto = statusPgtoAux;

                string tipoArquivo = Request.Params["TipoArquivo"];
                string status = Request.Params["Status"];
                string agrupamento = Request.Params["Agrupamento"];                

                bool ctesSemDuplicata, raizCNPJ = false;
                bool.TryParse(Request.Params["CTesSemDuplicata"], out ctesSemDuplicata);
                bool.TryParse(Request.Params["RaizCNPJ"], out raizCNPJ);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unitOfWork);

                Dominio.Entidades.Veiculo veiculo1 = codigoVeiculo1 > 0 ? repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo1) : null;
                Dominio.Entidades.Veiculo veiculo2 = codigoVeiculo2 > 0 ? repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo2) : null;
                Dominio.Entidades.Veiculo veiculo3 = codigoVeiculo3 > 0 ? repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo3) : null;
                Dominio.Entidades.Cliente pessoa = cpfCnpjPessoa > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                Dominio.Entidades.Usuario motorista = codigoMotorista > 0 ? repUsuario.BuscarPorCodigo(codigoMotorista) : null;

                if (ctesSemDuplicata && tipo == Dominio.Enumeradores.TipoDuplicata.AReceber)
                {
                    var cpfMotorista = "";
                    if (motorista != null)
                        cpfMotorista = motorista.CPF;

                    List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeSemDuplicata> ctes = repDuplicata.RelatorioCTeSemDuplicata(this.EmpresaUsuario.Codigo, codigoVeiculo1, codigoVeiculo2, codigoVeiculo3, pessoa != null ? pessoa.CPF_CNPJ_SemFormato : string.Empty, cpfMotorista, dataLctoInicial, dataLctoFinal, dataVctoInicial, dataVctoFinal, tipo, statusPgto, ordenacao, raizCNPJ);

                    List<ReportParameter> parametros = new List<ReportParameter>();
                    parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                    parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataLctoInicial != DateTime.MinValue ? dataLctoInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataLctoFinal != DateTime.MinValue ? dataLctoFinal.ToString("dd/MM/yyyy") : "99/99/9999")));
                    parametros.Add(new ReportParameter("Tomador", pessoa != null ? pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome : "Todos"));
                    parametros.Add(new ReportParameter("Motorista", motorista != null ? motorista.CPF_Formatado + " - " + motorista.Nome : "Todos"));
                    parametros.Add(new ReportParameter("Veiculo1", veiculo1 != null ? veiculo1.Placa : "Todos"));
                    parametros.Add(new ReportParameter("Veiculo2", veiculo2 != null ? veiculo2.Placa : "Todos"));
                    parametros.Add(new ReportParameter("Veiculo3", veiculo3 != null ? veiculo3.Placa : "Todos"));

                    List<ReportDataSource> dataSources = new List<ReportDataSource>();
                    dataSources.Add(new ReportDataSource("CTeSemDuplicata", ctes));

                    Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                    Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioCTeSemDuplicata.rdlc", tipoArquivo, parametros, dataSources);
                    unitOfWork.Dispose();
                    return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioCTeSemDuplicata." + arquivo.FileNameExtension);
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Relatorios.RelatorioDuplicatas> duplicatas = repDuplicata.Relatorio(this.EmpresaUsuario.Codigo, codigoVeiculo1, codigoVeiculo2, codigoVeiculo3, pessoa != null ? pessoa.CPF_CNPJ_SemFormato : string.Empty, codigoMotorista, dataLctoInicial, dataLctoFinal, dataVctoInicial, dataVctoFinal, tipo, statusPgto, ordenacao, raizCNPJ, status);

                    List<ReportParameter> parametros = new List<ReportParameter>();
                    parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                    parametros.Add(new ReportParameter("PeriodoLcto", string.Concat("De ", dataLctoInicial != DateTime.MinValue ? dataLctoInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataLctoFinal != DateTime.MinValue ? dataLctoFinal.ToString("dd/MM/yyyy") : "99/99/9999")));
                    parametros.Add(new ReportParameter("PeriodoVcto", string.Concat("De ", dataVctoInicial != DateTime.MinValue ? dataVctoInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataVctoFinal != DateTime.MinValue ? dataVctoFinal.ToString("dd/MM/yyyy") : "99/99/9999")));
                    parametros.Add(new ReportParameter("Pessoa", pessoa != null ? pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome : "Todos"));
                    parametros.Add(new ReportParameter("Motorista", motorista != null ? motorista.CPF_Formatado + " - " + motorista.Nome : "Todos"));
                    parametros.Add(new ReportParameter("Veiculo1", veiculo1 != null ? veiculo1.Placa : "Todos"));
                    parametros.Add(new ReportParameter("Veiculo2", veiculo2 != null ? veiculo2.Placa : "Todos"));
                    parametros.Add(new ReportParameter("Veiculo3", veiculo3 != null ? veiculo3.Placa : "Todos"));
                    parametros.Add(new ReportParameter("Tipo", tipo != null ? tipo.Value.ToString("G") : "Todos"));
                    parametros.Add(new ReportParameter("StatusPgto", statusPgto != null ? statusPgto.Value.ToString("G") : "Todos"));

                    List<ReportDataSource> dataSources = new List<ReportDataSource>();
                    dataSources.Add(new ReportDataSource("Duplicatas", duplicatas));

                    Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                    if (agrupamento == "Duplicata")
                    {
                        Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioDuplicatas.rdlc", tipoArquivo, parametros, dataSources);
                        unitOfWork.Dispose();
                        return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioDuplicatas." + arquivo.FileNameExtension);
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioDuplicatasParcelas.rdlc", tipoArquivo, parametros, dataSources);
                        unitOfWork.Dispose();
                        return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioDuplicatasParcelas." + arquivo.FileNameExtension);
                    };

                }
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