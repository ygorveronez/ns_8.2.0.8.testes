using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ExtratoContaController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            int codigoPlanoConta, codigoMotorista, codigoVeiculo = 0;
            int.TryParse(Request.Params["PlanoConta"], out codigoPlanoConta);
            int.TryParse(Request.Params["Motorista"], out codigoMotorista);
            int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);

            double cpfCnpjPessoa = 0f;
            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Pessoa"]), out cpfCnpjPessoa);

            DateTime dataLancamentoInicial, dataLancamentoFinal, dataPagamentoInicial, dataPagamentoFinal, dataBaixaInicial, dataBaixaFinal;
            DateTime.TryParseExact(Request.Params["DataLancamentoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataLancamentoInicial);
            DateTime.TryParseExact(Request.Params["DataLancamentoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataLancamentoFinal);
            DateTime.TryParseExact(Request.Params["DataPagamentoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagamentoInicial);
            DateTime.TryParseExact(Request.Params["DataPagamentoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPagamentoFinal);
            DateTime.TryParseExact(Request.Params["DataBaixaInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataBaixaInicial);
            DateTime.TryParseExact(Request.Params["DataBaixaFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataBaixaFinal);

            string tipoArquivo = Request.Params["TipoArquivo"];
            string tipoPlanoConta = Request.Params["TipoPlanoConta"];
            string situacaoMovimento = Request.Params["SituacaoMovimento"];

            if (this.UsuarioAdministrativo == null && this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.GeraDuplicatasAutomaticamente == Dominio.Enumeradores.OpcaoSimNao.Sim)
            {
                if (dataBaixaInicial > DateTime.MinValue || dataBaixaInicial > DateTime.MinValue)
                    return Json<bool>(false, false, "Datas de Baixa disponível apenas quando sistema configurado para não gerar Duplicata.");
                if (dataPagamentoInicial > DateTime.MinValue || dataPagamentoFinal > DateTime.MinValue)
                    return Json<bool>(false, false, "Datas de Vencimento disponível apenas quando sistema configurado para não gerar Duplicata.");

            }

            Repositorio.PlanoDeConta repPlanoConta = new Repositorio.PlanoDeConta(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            Dominio.Entidades.PlanoDeConta planoConta = codigoPlanoConta > 0 ? repPlanoConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlanoConta) : null;
            Dominio.Entidades.Cliente pessoa = cpfCnpjPessoa > 0f ? repCliente.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
            Dominio.Entidades.Usuario motorista = codigoMotorista > 0 ? repMotorista.BuscarPorCodigo(codigoMotorista) : null;
            Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;

            Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unitOfWork);
            List<Dominio.Entidades.MovimentoDoFinanceiro> movimentos = repMovimento.BuscarExtratoContas(this.EmpresaUsuario.Codigo, codigoPlanoConta, cpfCnpjPessoa, dataLancamentoInicial, dataLancamentoFinal, dataPagamentoInicial, dataPagamentoFinal, dataBaixaInicial, dataBaixaFinal, situacaoMovimento, tipoPlanoConta, codigoMotorista, codigoVeiculo);

            List<ReportParameter> parametros = new List<ReportParameter>();
            parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            parametros.Add(new ReportParameter("DataLancamento", string.Concat("De ", dataLancamentoInicial != DateTime.MinValue ? dataLancamentoInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataLancamentoFinal != DateTime.MinValue ? dataLancamentoFinal.ToString("dd/MM/yyyy") : "99/99/9999")));
            parametros.Add(new ReportParameter("DataPagamento", string.Concat("De ", dataPagamentoInicial != DateTime.MinValue ? dataPagamentoInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataPagamentoFinal != DateTime.MinValue ? dataPagamentoFinal.ToString("dd/MM/yyyy") : "99/99/9999")));
            parametros.Add(new ReportParameter("DataBaixa", string.Concat("De ", dataBaixaInicial != DateTime.MinValue ? dataBaixaInicial.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataBaixaFinal != DateTime.MinValue ? dataBaixaFinal.ToString("dd/MM/yyyy") : "99/99/9999")));
            parametros.Add(new ReportParameter("PlanoConta", planoConta != null ? planoConta.Conta + " - " + planoConta.Descricao : "Todos"));
            parametros.Add(new ReportParameter("Pessoa", pessoa != null ? pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome : "Todos"));
            parametros.Add(new ReportParameter("Motorista", motorista != null ? motorista.CPF_Formatado + " - " + motorista.Nome : "Todos"));
            parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : "Todos"));
            parametros.Add(new ReportParameter("TipoPlanoConta", tipoPlanoConta));
            parametros.Add(new ReportParameter("SituacaoMovimentos", situacaoMovimento));
            
            List<ReportDataSource> dataSources = new List<ReportDataSource>();
            dataSources.Add(new ReportDataSource("Movimentos", movimentos));

            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

            Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/ExtratoContas.rdlc", tipoArquivo, parametros, dataSources);

            return Arquivo(arquivo.Arquivo, arquivo.MimeType, "ExtratoDeContas." + arquivo.FileNameExtension);
        }
    }
}
