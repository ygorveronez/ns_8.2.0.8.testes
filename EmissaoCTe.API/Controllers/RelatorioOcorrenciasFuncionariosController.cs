using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioOcorrenciasFuncionariosController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            int codigoVeiculo, codigoTipoOcorrencia, codigoFuncionario = 0;
            int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
            int.TryParse(Request.Params["TipoOcorrencia"], out codigoTipoOcorrencia);
            int.TryParse(Request.Params["Funcionario"], out codigoFuncionario);

            DateTime dataInicial, dataFinal;
            DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

            string tipoArquivo = Request.Params["TipoArquivo"];

            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

            Repositorio.TipoDeOcorrencia repTipoOcorrencia = new Repositorio.TipoDeOcorrencia(unitOfWork);
            Dominio.Entidades.TipoDeOcorrencia tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia, this.EmpresaUsuario.Codigo);

            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Dominio.Entidades.Usuario funcionario = repFuncionario.BuscarPorPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoFuncionario);

            Repositorio.OcorrenciaDeFuncionario repOcorrenciaDeFuncionario = new Repositorio.OcorrenciaDeFuncionario(unitOfWork);
            List<Dominio.Entidades.OcorrenciaDeFuncionario> listaOcorrenciaDeFuncionario = repOcorrenciaDeFuncionario.Relatorio(this.EmpresaUsuario.Codigo, codigoFuncionario, codigoVeiculo, codigoTipoOcorrencia, dataInicial, dataFinal);

            List<ReportParameter> parametros = new List<ReportParameter>();
            parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
            parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : string.Empty));
            parametros.Add(new ReportParameter("TipoDeOcorrencia", tipoOcorrencia != null ? tipoOcorrencia.Descricao : string.Empty));
            parametros.Add(new ReportParameter("Funcionario", funcionario != null ? string.Concat(funcionario.CPF, " - ", funcionario.Nome) : string.Empty));

            List<ReportDataSource> dataSources = new List<ReportDataSource>();
            dataSources.Add(new ReportDataSource("Ocorrencias", listaOcorrenciaDeFuncionario));

            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

            Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioOcorrenciasFuncionarios.rdlc", tipoArquivo, parametros, dataSources);

            return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioOcorrenciasFuncionarios." + arquivo.FileNameExtension);
        }
    }
}
