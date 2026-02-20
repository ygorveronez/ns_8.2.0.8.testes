using System;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioCTesEmitidosPorEmbarcador : PaginaBaseSegura
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

    //public partial class RelatorioCTesEmitidosPorEmbarcador : PaginaBaseSegura
    //{
    //    new protected void Page_Load(object sender, EventArgs e)
    //    {

    //    }

    //    protected void btnGerarRelatorio_Click(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(Conexao.StringConexao);
    //            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(Conexao.StringConexao);
    //            Repositorio.Cliente repCliente = new Repositorio.Cliente(Conexao.StringConexao);

    //            int.TryParse(this.hddCodigoEmpresa.Value, out int codigoEmpresa);

    //            DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
    //            DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);
    //            DateTime.TryParseExact(this.txtDataAutorizacaoInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAutorizacaoInicial);
    //            DateTime.TryParseExact(this.txtDataAutorizacaoFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAutorizacaoFinal);

    //            double.TryParse(Utilidades.String.OnlyNumbers(this.hddCodigoEmbarcador.Value), out double cpfCnpjEmbarcador);
    //            bool.TryParse(this.selTodosCNPJRaiz.SelectedValue, out bool todosCNPJRaiz);

    //            string cnpjEmbarcadorUsuario = this.Usuario.CNPJEmbarcador ?? string.Empty;
    //            string tipoEmissao = this.selTipoEmissao.Text;
    //            string tipoRelatorio = this.selTipoRelatorio.Text;

    //            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
    //            Dominio.Entidades.Cliente cliente = cpfCnpjEmbarcador > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjEmbarcador) : null;
    //            List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> listaCTes = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador>();

    //            this.rvwRelatorioCTesEmitidos.LocalReport.ReportPath = "Relatorios/RelatorioCTesEmitidosPorEmbarcador" + tipoRelatorio + ".rdlc";
    //            this.rvwRelatorioCTesEmitidos.LocalReport.DataSources.Clear();
    //            List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> listaCTesTeste = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador>();
    //            this.rvwRelatorioCTesEmitidos.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CTes", listaCTesTeste));
    //            this.rvwRelatorioCTesEmitidos.DataBind();

    //            if (tipoRelatorio == "")
    //                listaCTes = repCTe.RelatorioCTesEmitidosPorEmbarcador(this.EmpresaUsuario.Codigo, codigoEmpresa, cliente != null ? cliente.CPF_CNPJ_SemFormato : null, dataAutorizacaoInicial, dataAutorizacaoFinal, dataInicial, dataFinal, todosCNPJRaiz, cnpjEmbarcadorUsuario, tipoEmissao);
    //            else if (tipoRelatorio == "Sum")
    //                listaCTes = repCTe.RelatorioCTesEmitidosPorEmbarcadorSumarizado(this.EmpresaUsuario.Codigo, codigoEmpresa, cliente != null ? cliente.CPF_CNPJ_SemFormato : null, dataAutorizacaoInicial, dataAutorizacaoFinal, dataInicial, dataFinal, todosCNPJRaiz, cnpjEmbarcadorUsuario, tipoEmissao);

    //            this.rvwRelatorioCTesEmitidos.LocalReport.ReportPath = "Relatorios/RelatorioCTesEmitidosPorEmbarcador" + tipoRelatorio + ".rdlc";
    //            this.rvwRelatorioCTesEmitidos.LocalReport.DataSources.Clear();
    //            this.rvwRelatorioCTesEmitidos.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CTes", listaCTes));
    //            this.rvwRelatorioCTesEmitidos.DataBind();
    //        }
    //        catch (Exception ex)
    //        {
    //            Servicos.Log.TratarErro(ex);
    //        }
    //    }

    //    //void SubReportComponentesPrestacao(object sender, SubreportProcessingEventArgs e)
    //    //{
    //    //    if (e.ReportPath.Contains("RelatorioCTesEmitidosPorEmbarcador_Componentes"))
    //    //    {
    //    //        e.DataSources.Add(new ReportDataSource("Componentes", new Repositorio.ComponentePrestacaoCTE(Conexao.StringConexao).BuscarPorCTe(int.Parse(e.Parameters["CodigoCTe"].Values[0]))));
    //    //    }
    //    //}
    //}
}