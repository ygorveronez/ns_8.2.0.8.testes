using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioHistoricosVeiculosController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoVeiculo, codigoServico;
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["Servico"], out codigoServico);

                string fornecedor = Utilidades.String.OnlyNumbers(Request.Params["Fornecedor"]);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.ServicoVeiculo repServico = new Repositorio.ServicoVeiculo(unitOfWork);
                Dominio.Entidades.ServicoVeiculo servico = repServico.BuscarPorCodigo(codigoServico, this.EmpresaUsuario.Codigo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente clienteFornecedor = !string.IsNullOrWhiteSpace(fornecedor) ? repCliente.BuscarPorCPFCNPJ(double.Parse(fornecedor)) : null;

                Repositorio.HistoricoVeiculo repHistoricoVeiculo = new Repositorio.HistoricoVeiculo(unitOfWork);
                List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosVeiculos> listaHistoricos = repHistoricoVeiculo.Relatorio(this.EmpresaUsuario.Codigo, veiculo != null ? veiculo.Codigo : 0, servico != null ? servico.Codigo : 0, dataInicial, dataFinal, clienteFornecedor != null ? clienteFornecedor.CPF_CNPJ : 0);

                List<ReportParameter> parametros = new List<ReportParameter>();
                string descricaoServico = servico != null ? servico.Descricao : string.Empty;
                if (clienteFornecedor != null)
                    descricaoServico = string.Concat(descricaoServico, " / Fornecedor: " + clienteFornecedor.Nome);
                    parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Servico", descricaoServico.Length > 100 ? descricaoServico.Substring(0,100) : descricaoServico));
                parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : string.Empty));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));              

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Historicos", listaHistoricos));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioHistoricosVeiculos.rdlc", tipoArquivo, parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioHistoricosVeiculos." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar relatorio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
