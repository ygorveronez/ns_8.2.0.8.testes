using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioServicosVeiculosController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            int codigoVeiculo, codigoServico = 0;
            int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
            int.TryParse(Request.Params["Servico"], out codigoServico);

            DateTime dataInicial, dataFinal;
            DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

            string tipoArquivo = Request.Params["TipoArquivo"];

            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.ServicoVeiculo repServicoVeiculo = new Repositorio.ServicoVeiculo(unitOfWork);

            Dominio.Entidades.ConfiguracaoEmpresa configuracao = this.EmpresaUsuario.Configuracao == null && this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Configuracao : this.EmpresaUsuario.Configuracao;            
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
            Dominio.Entidades.ServicoVeiculo servicoVeiculo = repServicoVeiculo.BuscarPorCodigo(codigoServico, this.EmpresaUsuario.Codigo);

            Repositorio.HistoricoVeiculo repHistoricoVeiculo = new Repositorio.HistoricoVeiculo(unitOfWork);
            IList<Dominio.ObjetosDeValor.Relatorios.RelatorioServicosVeiculos> listaServicos = repHistoricoVeiculo.RelatorioServicos(this.EmpresaUsuario.Codigo, codigoVeiculo, codigoServico, dataInicial, dataFinal);
            
            List<ReportParameter> parametros = new List<ReportParameter>();
            parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
            parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : string.Empty));
            parametros.Add(new ReportParameter("Servico", servicoVeiculo != null ? servicoVeiculo.Descricao : string.Empty));

            List<ReportDataSource> dataSources = new List<ReportDataSource>();
            dataSources.Add(new ReportDataSource("Servicos", listaServicos));

            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

            Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioServicosVeiculos.rdlc", tipoArquivo, parametros, dataSources);

            return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioServicosVeiculos." + arquivo.FileNameExtension);
        }
    }
}
