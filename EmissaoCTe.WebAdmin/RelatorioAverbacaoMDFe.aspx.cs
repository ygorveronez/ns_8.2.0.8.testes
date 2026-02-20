using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioAverbacaoMDFe : PaginaBaseSegura
    {
        new protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {

                int codigoEmpresa = 0;
                int.TryParse(this.hddCodigoEmpresa.Value, out codigoEmpresa);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                ConsultarAverbacoes(empresa != null ? empresa.Codigo : 0, dataInicial, dataFinal, unitOfWork);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidos> listaMDFes = repMDFe.RelatorioMDFesEmitidos(codigoEmpresa, dataInicial, dataFinal, 0, 0, "", "", "", "", "", Dominio.Enumeradores.StatusMDFe.Todos);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", empresa != null ? empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial : "Todas"));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));

                if (parametros != null)
                    this.rvwRelatorioAverbacaoMDFe.LocalReport.SetParameters(parametros);

                this.rvwRelatorioAverbacaoMDFe.LocalReport.DataSources.Clear();
                this.rvwRelatorioAverbacaoMDFe.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("MDFesEmitidos", listaMDFes));

                this.rvwRelatorioAverbacaoMDFe.LocalReport.SubreportProcessing += rvwRelatorioAverbacaoMDFe_SubreportProcessing;

                this.rvwRelatorioAverbacaoMDFe.DataBind();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected void rvwRelatorioAverbacaoMDFe_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.AverbacaoMDFe rebAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
                List<Dominio.Entidades.AverbacaoMDFe> listaAverbacaoMDFe = rebAverbacaoMDFe.BuscarPorCodigoMDFe(int.Parse(e.Parameters["CodigoMDFe"].Values[0]));

                e.DataSources.Add(new ReportDataSource("AverbacoesMDFe", listaAverbacaoMDFe));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected void ConsultarAverbacoes(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                List<int> listaMDFes = repMDFe.BuscarListaCodigosAutorizacaoOracle(codigoEmpresa, dataInicial, dataFinal);

                Servicos.AverbacaoMDFe svcAverbacao = new Servicos.AverbacaoMDFe(unitOfWork);

                for (int i = 0; i < listaMDFes.Count; i++)
                {
                    svcAverbacao.ConsultarAverbacoes(listaMDFes[i], unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

    }
}
