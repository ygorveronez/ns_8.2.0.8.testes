using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioAcertosViagensController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.AcertoDeViagem repAcertoViagem = new Repositorio.AcertoDeViagem(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.DespesaDoAcertoDeViagem repDespesa = new Repositorio.DespesaDoAcertoDeViagem(unitOfWork);
                Repositorio.DestinoDoAcertoDeViagem repDestino = new Repositorio.DestinoDoAcertoDeViagem(unitOfWork);
                Repositorio.ValeDoAcertoDeViagem repVale = new Repositorio.ValeDoAcertoDeViagem(unitOfWork);

                int.TryParse(Request.Params["Veiculo"], out int codigoVeiculo);
                int.TryParse(Request.Params["Motorista"], out int codigoMotorista);

                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                string situacao = Request.Params["Situacao"] ?? string.Empty;
                string tipoArquivo = Request.Params["TipoArquivo"] ?? string.Empty;
                string tipoRelatorio = Request.Params["TipoRelatorio"] ?? string.Empty;

                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

                bool relatorioCompleto = tipoRelatorio == "AcertoDetalhadoTotais";

                IList<Dominio.Entidades.AcertoDeViagem> listaAcertos = repAcertoViagem.Relatorio(this.EmpresaUsuario.Codigo, codigoVeiculo, codigoMotorista, dataInicial, dataFinal, situacao);
                List<int> codigosAcertos = (from o in listaAcertos select o.Codigo).ToList();

                List < ReportParameter > parametros = new List<ReportParameter>
                {
                    new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial),
                    new ReportParameter("InscricaoEstadual", this.EmpresaUsuario.InscricaoEstadual),
                    new ReportParameter("CNPJ", this.EmpresaUsuario.CNPJ_Formatado),
                    new ReportParameter("Logo", this.EmpresaUsuario.CaminhoLogoDacte),
                    new ReportParameter("Periodo", this.MontarPeriodo(dataInicial, dataFinal)),
                    new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : "Todos"),
                    new ReportParameter("Motorista", motorista != null ? string.Concat(motorista.CPF, " - ", motorista.Nome) : "Todos"),
                    new ReportParameter("Situacao", this.DescricaoSituacao(situacao)),
                    new ReportParameter("Tipo", this.DescricaoTipo(tipoRelatorio))
                };

                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Acertos", listaAcertos)
                };

                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = new List<Dominio.Entidades.Abastecimento>();
                List<Dominio.Entidades.DespesaDoAcertoDeViagem> listaDespesas = new List<Dominio.Entidades.DespesaDoAcertoDeViagem>();
                List<Dominio.Entidades.DestinoDoAcertoDeViagem> listaDestinos = new List<Dominio.Entidades.DestinoDoAcertoDeViagem>();
                List<Dominio.Entidades.ValeDoAcertoDeViagem> listaVales = new List<Dominio.Entidades.ValeDoAcertoDeViagem>();

                if (relatorioCompleto)
                {
                    listaAbastecimentos = repAbastecimento.BuscarPorAcertos(codigosAcertos);
                    listaDespesas = repDespesa.BuscarPorAcertos(codigosAcertos);
                    listaDestinos = repDestino.BuscarPorAcertos(codigosAcertos);
                    listaVales = repVale.BuscarPorAcertos(codigosAcertos);
                }

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/Relatorio" + tipoRelatorio + ".rdlc", tipoArquivo, parametros, dataSources, (object sender, SubreportProcessingEventArgs e) =>
                {
                    if (e.ReportPath.Contains("Abastecimentos"))
                    {
                        if(!relatorioCompleto)
                            listaAbastecimentos = repAbastecimento.BuscarPorAcertoDeViagem(int.Parse(e.Parameters["CodigoAcertoViagem"].Values[0]));
                        
                        e.DataSources.Add(new ReportDataSource("Abastecimentos", listaAbastecimentos));
                    }
                    else if (e.ReportPath.Contains("Despesas"))
                    {
                        if (!relatorioCompleto)
                            listaDespesas = repDespesa.BuscarPorAcertoDeViagem(int.Parse(e.Parameters["CodigoAcertoViagem"].Values[0]));

                        e.DataSources.Add(new ReportDataSource("Despesas", listaDespesas));
                    }
                    else if (e.ReportPath.Contains("Destinos"))
                    {
                        if (!relatorioCompleto)
                            listaDestinos = repDestino.BuscarPorAcertoDeViagem(int.Parse(e.Parameters["CodigoAcertoViagem"].Values[0]));

                        e.DataSources.Add(new ReportDataSource("Destinos", listaDestinos));
                    }
                    else if (e.ReportPath.Contains("Vales"))
                    {
                        if (!relatorioCompleto)
                            listaVales = repVale.BuscarPorAcertoDeViagem(int.Parse(e.Parameters["CodigoAcertoViagem"].Values[0]));

                        e.DataSources.Add(new ReportDataSource("Vales", listaVales));
                    }
                });

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, String.Concat("Relatorio", tipoRelatorio, ".", arquivo.FileNameExtension));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string DescricaoSituacao(string situacao)
        {
            switch (situacao)
            {
                case "A":
                    return "Aberto";
                case "P":
                    return "Pendente Pagamento";
                case "F":
                    return "Fechado";
                default:
                    return "Todas";
            }
        }

        private string DescricaoTipo(string tipo)
        {
            switch (tipo)
            {
                case "AcertosViagens":
                    return "Agrupado por Acerto";
                case "AcertoTotalizadores":
                    return "Totalizadores";
                case "AcertoDetalhadoTotais":
                    return "Detalhado com Totais";
                default:
                    return "";
            }
        }
        
        private string MontarPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            string periodo = "";

            if (dataInicial != DateTime.MinValue)
                periodo += " De " + dataInicial.ToString("dd/MM/yyyy");

            if (dataFinal != DateTime.MinValue)
                periodo += " Até " + dataFinal.ToString("dd/MM/yyyy");
                
            return periodo.Trim();
        }
    }
}
