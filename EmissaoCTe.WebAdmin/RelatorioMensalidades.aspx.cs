using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmissaoCTe.WebAdmin
{
    public partial class RelatorioMensalidades : PaginaBaseSegura
    {

        #region Variáveis Globais

        List<Dominio.Entidades.DespesaAdicionalEmpresa> DespesasAdicionais = null;

        #endregion

        new protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                this.PreencherAnos();
        }

        protected void btnGerarRelatorio_Click(object sender, EventArgs e)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            int codigoEmpresa, mes, ano = 0;
            int.TryParse(this.hddCodigoEmpresa.Value, out codigoEmpresa);
            int.TryParse(this.selAno.SelectedValue, out ano);
            int.TryParse(this.selMes.SelectedValue, out mes);

            DateTime dataInicial = new DateTime(ano, mes, 1);
            DateTime dataFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa emp = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;

            List<Dominio.Entidades.Empresa> listaEmpresas = new List<Dominio.Entidades.Empresa>();

            if (emp != null)
                listaEmpresas.Add(emp);
            else
                listaEmpresas = repEmpresa.BuscarPorEmpresaPaiEStatus(this.EmpresaUsuario.Codigo, "A");

            Repositorio.DespesaAdicionalEmpresa repDespesaAdicional = new Repositorio.DespesaAdicionalEmpresa(unitOfWork);
            this.DespesasAdicionais = repDespesaAdicional.BuscarPorEmpresas(this.EmpresaUsuario.Codigo, (from obj in listaEmpresas select obj.Codigo).ToArray(), dataInicial, dataFinal, "A", "A");

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            IList<Dominio.ObjetosDeValor.Relatorios.RelatorioCTeAgrupado> listaCTesAgrupados = repCTe.RelatorioCTesAgrupados(this.EmpresaUsuario.Codigo, codigoEmpresa, DateTime.MinValue, DateTime.MinValue, dataInicial, dataFinal, null);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            IList<Dominio.ObjetosDeValor.Relatorios.RelatorioMDFeAgrupado> listaMDFesAgrupados =  repMDFe.RelatorioMDFesAgrupados(this.EmpresaUsuario.Codigo,codigoEmpresa, DateTime.MinValue, DateTime.MinValue, dataInicial, dataFinal);

            Repositorio.FaixaEmissaoCTe repFaixaEmissaoCTe = new Repositorio.FaixaEmissaoCTe(unitOfWork);

            List<Dominio.ObjetosDeValor.Relatorios.RelatorioMensalidade> mensalidades = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioMensalidade>();

            foreach (Dominio.Entidades.Empresa empresa in listaEmpresas)
            {
                Dominio.ObjetosDeValor.Relatorios.RelatorioMensalidade mensalidade = new Dominio.ObjetosDeValor.Relatorios.RelatorioMensalidade();
                mensalidade.CodigoEmpresa = empresa.Codigo;
                mensalidade.CNPJ = empresa.CNPJ;
                mensalidade.Nome = empresa.NomeFantasia;
                mensalidade.QuantidadeAutorizados = (from obj in listaCTesAgrupados where obj.Status.Equals("A") && obj.CodigoEmpresa == empresa.Codigo select obj.CountCTes).FirstOrDefault();
                mensalidade.QuantidadeCancelados = (from obj in listaCTesAgrupados where obj.Status.Equals("C") && obj.CodigoEmpresa == empresa.Codigo select obj.CountCTes).FirstOrDefault();
                mensalidade.QuantidadeMDFeAutorizados = (from obj in listaMDFesAgrupados where (obj.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || obj.Status == Dominio.Enumeradores.StatusMDFe.Encerrado) && obj.CodigoEmpresa == empresa.Codigo select obj.CountMDFes).FirstOrDefault();
                mensalidade.QuantidadeMDFeCancelados = (from obj in listaMDFesAgrupados where obj.Status == Dominio.Enumeradores.StatusMDFe.Cancelado && obj.CodigoEmpresa == empresa.Codigo select obj.CountMDFes).FirstOrDefault();

                mensalidade.ValorDespesasAdicionais = (from obj in this.DespesasAdicionais where obj.Empresa.Codigo == empresa.Codigo select obj.Valor).Sum();

                if (empresa.PlanoEmissaoCTe != null)
                {
                    Dominio.Entidades.FaixaEmissaoCTe faixaEmissao = repFaixaEmissaoCTe.BuscarPorPlanoEQuantidade(empresa.PlanoEmissaoCTe.Codigo, (mensalidade.QuantidadeAutorizados + mensalidade.QuantidadeCancelados + mensalidade.QuantidadeMDFeAutorizados + mensalidade.QuantidadeMDFeCancelados));

                    if (faixaEmissao != null)
                    {
                        mensalidade.ValorMensalidade = faixaEmissao.Valor;
                        mensalidade.DescricaoPlano = string.Concat(empresa.PlanoEmissaoCTe.Descricao, " (até ", faixaEmissao.Quantidade, ")");
                    }
                }

                mensalidades.Add(mensalidade);
            }

            this.rvwRelatorioMensalidades.LocalReport.SetParameters(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            this.rvwRelatorioMensalidades.LocalReport.SetParameters(new ReportParameter("Periodo", string.Concat(string.Format("{0:00}", mes), "/", ano)));
            this.rvwRelatorioMensalidades.LocalReport.SetParameters(new ReportParameter("EmpresaEmissora", emp != null ? emp.RazaoSocial : "Todas"));

            this.rvwRelatorioMensalidades.LocalReport.DataSources.Clear();
            this.rvwRelatorioMensalidades.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Mensalidades", mensalidades));

            this.rvwRelatorioMensalidades.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(rvwRelatorioMensalidades_EventHandler);

            this.rvwRelatorioMensalidades.DataBind();
        }

        private void rvwRelatorioMensalidades_EventHandler(object sender, SubreportProcessingEventArgs e)
        {
            List<Dominio.Entidades.DespesaAdicionalEmpresa> despesas = (from obj in this.DespesasAdicionais where obj.Empresa.Codigo == int.Parse(e.Parameters["CodigoEmpresa"].Values[0]) select obj).ToList();
            e.DataSources.Add(new ReportDataSource("DespesasAdicionais", despesas));
        }

        private void PreencherAnos()
        {
            DateTime data = DateTime.Now;

            for (var i = (data.Year - 5); i <= data.Year; i++)
            {
                this.selAno.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
        }
    }
}