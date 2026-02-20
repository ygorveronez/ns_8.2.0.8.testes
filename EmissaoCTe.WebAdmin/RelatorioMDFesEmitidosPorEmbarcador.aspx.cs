using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioMDFesEmitidosPorEmbarcador : PaginaBaseSegura
    {
        #region Manipuladores de Eventos

        new protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            int codigoEmpresa = 0;
            int.TryParse(this.hddCodigoEmpresa.Value, out codigoEmpresa);

            DateTime dataInicial, dataFinal;
            DateTime.TryParseExact(this.txtDataInicial.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(this.txtDataFinal.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

            double cpfCnpjEmbarcador;
            double.TryParse(Utilidades.String.OnlyNumbers(this.hddCodigoEmbarcador.Value), out cpfCnpjEmbarcador);

            bool todosCNPJRaiz;
            bool.TryParse(this.selTodosCNPJRaiz.SelectedValue, out todosCNPJRaiz);

            string cnpjEmbarcadorUsuario = "";
            if (!string.IsNullOrWhiteSpace(this.Usuario.CNPJEmbarcador))
                cnpjEmbarcadorUsuario = this.Usuario.CNPJEmbarcador;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.Entidades.Cliente embarcador = null;
            if (cpfCnpjEmbarcador > 0)
                embarcador = repCliente.BuscarPorCPFCNPJ(cpfCnpjEmbarcador);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            List<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFesEmitidos> listaMDFes = repMDFe.RelatorioMDFesEmitidos(codigoEmpresa, dataInicial, dataFinal, 0, 0, "", "", "", "", "", Dominio.Enumeradores.StatusMDFe.Todos, this.EmpresaUsuario.Codigo, embarcador != null ? embarcador.CPF_CNPJ_SemFormato : string.Empty, todosCNPJRaiz, cnpjEmbarcadorUsuario);

            this.rvwRelatorioMDFesEmitidosPorEmbarcador.LocalReport.SetParameters(new ReportParameter("Empresa", empresa != null ? empresa.RazaoSocial : "Todas"));
            this.rvwRelatorioMDFesEmitidosPorEmbarcador.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
            this.rvwRelatorioMDFesEmitidosPorEmbarcador.LocalReport.SetParameters(new ReportParameter("Embarcador", embarcador != null ? embarcador.Nome + (todosCNPJRaiz == true ? "(Filtro pela raiz do CNPJ)" : string.Empty) : "Todos"));

            this.rvwRelatorioMDFesEmitidosPorEmbarcador.LocalReport.DataSources.Clear();
            this.rvwRelatorioMDFesEmitidosPorEmbarcador.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("MDFesEmitidos", listaMDFes));

            this.rvwRelatorioMDFesEmitidosPorEmbarcador.DataBind();
        }

        #endregion
    }
}