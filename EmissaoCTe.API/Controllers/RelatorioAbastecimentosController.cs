using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioAbastecimentosController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            int codigoVeiculo, codigoModelo;
            int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
            int.TryParse(Request.Params["ModeloVeiculo"], out codigoModelo);

            DateTime dataInicial, dataFinal;
            DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

            string nomePosto = Request.Params["NomePosto"];
            string tipoArquivo = Request.Params["TipoArquivo"];
            string pagamento = "";

            if (!string.IsNullOrWhiteSpace(Request.Params["Pagamento"]))
                pagamento = Request.Params["Pagamento"];

            double cpfCnpjCliente;
            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Cliente"]), out cpfCnpjCliente);

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                 
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
            Dominio.Entidades.ModeloVeiculo modeloVeiculo = repModeloVeiculo.BuscarPorCodigo(codigoModelo, this.EmpresaUsuario.Codigo);
            Dominio.Entidades.Cliente posto = null;

            if (cpfCnpjCliente > 0)
                posto = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            IList<Dominio.Entidades.Abastecimento> listaAbastecimentos = repAbastecimento.Relatorio(this.EmpresaUsuario.Codigo, codigoVeiculo, codigoModelo, dataInicial, dataFinal, cpfCnpjCliente, pagamento, nomePosto);

            List<ReportParameter> parametros = new List<ReportParameter>();
            parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
            parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : string.Empty));
            parametros.Add(new ReportParameter("Modelo", modeloVeiculo != null ? modeloVeiculo.Descricao : string.Empty));
            string postoParametro = posto != null ? posto.Nome : string.Empty;
            if (!string.IsNullOrWhiteSpace(nomePosto))
                postoParametro = !string.IsNullOrWhiteSpace(postoParametro) ? string.Concat(postoParametro, " ", nomePosto) : nomePosto;
            parametros.Add(new ReportParameter("Posto", postoParametro));
            parametros.Add(new ReportParameter("Pagamento", pagamento.Equals("1") ? "Pago" : pagamento.Equals("0") ? "A Pagar" : "Todos"));

            List<ReportDataSource> dataSources = new List<ReportDataSource>();
            dataSources.Add(new ReportDataSource("Abastecimentos", listaAbastecimentos));

            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

            Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioAbastecimentos.rdlc", tipoArquivo, parametros, dataSources);

            return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioAbastecimentos." + arquivo.FileNameExtension);
        }
    }
}
