using System;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioCTesEmitidosPorTransportador : PaginaBaseSegura
    {
        new protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            int codigoEmpresa = 0;
            int.TryParse(this.hddCodigoEmpresa.Value, out codigoEmpresa);

            DateTime dataInicial, dataFinal, dataAutorizacaoInicial, dataAutorizacaoFinal;
            DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);
            DateTime.TryParseExact(this.txtDataAutorizacaoInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoInicial);
            DateTime.TryParseExact(this.txtDataAutorizacaoFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAutorizacaoFinal);

            double cpfCnpjEmbarcador;
            double.TryParse(Utilidades.String.OnlyNumbers(this.hddCodigoEmbarcador.Value), out cpfCnpjEmbarcador);

            bool todosCNPJRaiz;
            bool.TryParse(this.selTodosCNPJRaiz.SelectedValue, out todosCNPJRaiz);

            string cnpjEmbarcadorUsuario = "";
            if (!string.IsNullOrWhiteSpace(this.Usuario.CNPJEmbarcador))
                cnpjEmbarcadorUsuario = this.Usuario.CNPJEmbarcador;

            dynamic listaObjetos = null;
            int[] series = null;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.Entidades.Cliente cliente = cpfCnpjEmbarcador > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjEmbarcador) : null;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);            
            listaObjetos = repCTe.RelatorioEmissao(codigoEmpresa, dataInicial, dataFinal, "", Dominio.Enumeradores.TipoCTE.Todos, 0, cpfCnpjEmbarcador.ToString(), "", "", "", "", 0, 0, "", "", "", this.EmpresaUsuario.TipoAmbiente, series, "", "", null, 0, 0, "", "", "", false, false, todosCNPJRaiz, false, false, false, false, dataAutorizacaoInicial, dataAutorizacaoFinal, "",null,"0", 0, "", cnpjEmbarcadorUsuario);

            this.rvwRelatorioCTesEmitidosPorTransportador.LocalReport.DataSources.Clear();
            this.rvwRelatorioCTesEmitidosPorTransportador.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CTesEmitidos", listaObjetos));

            this.rvwRelatorioCTesEmitidosPorTransportador.DataBind();
        }
    }
}