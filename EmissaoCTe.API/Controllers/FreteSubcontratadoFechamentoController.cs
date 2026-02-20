using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.API.Controllers
{
    public class FreteSubcontratadoFechamentoController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                var inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Dominio.Enumeradores.TipoFreteSubcontratado tipoAux;
                Dominio.Enumeradores.TipoFreteSubcontratado? tipo = null;
                if (Enum.TryParse<Dominio.Enumeradores.TipoFreteSubcontratado>(Request.Params["Tipo"], out tipoAux))
                    tipo = tipoAux;

                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CpfCnpjCliente"]), out cpfCnpjCliente);

                DateTime dataInicial = DateTime.MinValue, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);

                List<Dominio.Entidades.FreteSubcontratado> fretes = repFreteSubcontratado.ConsultarFretesAbertos(this.EmpresaUsuario.Codigo, tipo, dataInicial, dataFinal, cpfCnpjCliente, inicioRegistros, 50);

                int countFretes = repFreteSubcontratado.ContarFretesAbertos(this.EmpresaUsuario.Codigo, tipo, dataInicial, dataFinal, cpfCnpjCliente);

                var retorno = from obj in fretes select new { obj.Codigo, Destinatario = obj.Destinatario.Nome != null ? obj.Destinatario.Nome : string.Empty, Filial = obj.Filial, CTe = obj.NumeroCTe.ToString(), Peso = obj.Peso.ToString("n2"), ValorFreteLiquido = obj.ValorFreteLiquido.ToString("n2"), ValorComissao = obj.ValorComissao.ToString("n2"), ValorTotalComissao = (obj.ValorComissao + obj.ValorTDA + obj.ValorTDE + obj.ValorCarroDedicado).ToString("n2") };

                return Json(retorno, true, null, new string[] { "Codigo", "Destinatário|15", "Filial|15", "CTe|10", "Peso|15", "Vlr Frete Liquido|15", "Vlr Comissão|15", "Total Comissão|15" }, countFretes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter Fretes Subcontratados Abertos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterTotaisPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                var inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Dominio.Enumeradores.TipoFreteSubcontratado tipoAux;
                Dominio.Enumeradores.TipoFreteSubcontratado? tipo = null;
                if (Enum.TryParse<Dominio.Enumeradores.TipoFreteSubcontratado>(Request.Params["Tipo"], out tipoAux))
                    tipo = tipoAux;

                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CpfCnpjCliente"]), out cpfCnpjCliente);

                DateTime dataInicial = DateTime.MinValue, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);

                List<Dominio.Entidades.FreteSubcontratado> fretes = repFreteSubcontratado.ConsultarFretesAbertos(this.EmpresaUsuario.Codigo, tipo, dataInicial, dataFinal, cpfCnpjCliente, 0, 0);

                decimal peso = 0, freteLiquido = 0, comissao = 0, comissaoTotal = 0;

                foreach (Dominio.Entidades.FreteSubcontratado frete in fretes)
                {
                    peso += frete.Peso;
                    freteLiquido += frete.ValorFreteLiquido;
                    comissao += frete.ValorComissao;
                    comissaoTotal += (frete.ValorComissao + frete.ValorTDA + frete.ValorTDE + frete.ValorCarroDedicado);
                }

                var retorno = new
                {
                    Peso = peso.ToString("n2"),
                    FreteLiquido = freteLiquido.ToString("n2"),
                    ValorComissao = comissao.ToString("n2"),
                    ValorTotalComissao = comissaoTotal.ToString("n2")
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter Totais Fretes Subcontratados Abertos");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult FecharFretes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                var inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Dominio.Enumeradores.TipoFreteSubcontratado tipoAux;
                Dominio.Enumeradores.TipoFreteSubcontratado? tipo = null;
                if (Enum.TryParse<Dominio.Enumeradores.TipoFreteSubcontratado>(Request.Params["Tipo"], out tipoAux))
                    tipo = tipoAux;

                double cpfCnpjCliente = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CpfCnpjCliente"]), out cpfCnpjCliente);

                DateTime dataInicial = DateTime.MinValue, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);
                List<Dominio.Entidades.FreteSubcontratado> fretes = repFreteSubcontratado.ConsultarFretesAbertos(this.EmpresaUsuario.Codigo, tipo, dataInicial, dataFinal, cpfCnpjCliente, 0, 0);

                if (fretes.Count > 0)
                {
                    unitOfWork.Start();

                    foreach (Dominio.Entidades.FreteSubcontratado frete in fretes)
                    {
                        Dominio.Entidades.FreteSubcontratado freteSubcontratado = repFreteSubcontratado.BuscaPorCodigo(this.EmpresaUsuario.Codigo, frete.Codigo);
                        freteSubcontratado.Status = Dominio.Enumeradores.StatusFreteSubcontratado.Fechado;
                        repFreteSubcontratado.Atualizar(freteSubcontratado);
                    }

                    unitOfWork.CommitChanges();
                }


                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao fechar Fretes Subcontratados");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dataEntradaInicio, dataEntradaFim, dataEntregaInicio, dataEntregaFim;
                DateTime.TryParseExact(Request.Params["DataEntradaInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntradaInicio);
                DateTime.TryParseExact(Request.Params["DataEntradaFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntradaFim);
                DateTime.TryParseExact(Request.Params["DataEntregaInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntregaInicio);
                DateTime.TryParseExact(Request.Params["DataEntregaFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEntregaFim);

                double cnpjParceiro;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Parceiro"]), out cnpjParceiro);


                Dominio.Enumeradores.TipoFreteSubcontratado? tipo = null;
                Dominio.Enumeradores.TipoFreteSubcontratado tipoAux;

                if (Enum.TryParse<Dominio.Enumeradores.TipoFreteSubcontratado>(Request.Params["Tipo"], out tipoAux))
                    tipo = tipoAux;

                Dominio.Enumeradores.StatusFreteSubcontratado? status = null;
                Dominio.Enumeradores.StatusFreteSubcontratado statusAux;

                if (Enum.TryParse<Dominio.Enumeradores.StatusFreteSubcontratado>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string tipoArquivo = Request.Params["TipoArquivo"];

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.FreteSubcontratado repFreteSubcontratado = new Repositorio.FreteSubcontratado(unitOfWork);

                Dominio.Entidades.Cliente parceiro = cnpjParceiro > 0f ? repCliente.BuscarPorCPFCNPJ(cnpjParceiro) : null;

                List<Dominio.ObjetosDeValor.Relatorios.RelatorioFretesSubcontratados> fretesSubcontratados = repFreteSubcontratado.Relatorio(this.EmpresaUsuario.Codigo, cnpjParceiro, dataEntradaInicio, dataEntradaFim, dataEntregaInicio, dataEntregaFim, tipo, status);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("EnderecoEmpresa", string.Concat(this.EmpresaUsuario.Endereco, "  ", this.EmpresaUsuario.Localidade.Descricao, "-", this.EmpresaUsuario.Localidade.Estado.Sigla)));
                parametros.Add(new ReportParameter("CnpjIE", string.Concat("CNPJ:", this.EmpresaUsuario.CNPJ_Formatado, " IE:", this.EmpresaUsuario.InscricaoEstadual)));
                parametros.Add(new ReportParameter("Telefone", string.Concat("Fone:", this.EmpresaUsuario.Telefone)));
                parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataEntradaInicio != DateTime.MinValue ? dataEntradaInicio.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataEntradaFim != DateTime.MinValue ? dataEntradaFim.ToString("dd/MM/yyyy") : "99/99/9999")));
                parametros.Add(new ReportParameter("PeriodoEntrega", string.Concat("De ", dataEntregaInicio != DateTime.MinValue ? dataEntregaInicio.ToString("dd/MM/yyyy") : "00/00/0000", " até ", dataEntregaFim != DateTime.MinValue ? dataEntregaFim.ToString("dd/MM/yyyy") : "99/99/9999")));
                parametros.Add(new ReportParameter("Parceiro", parceiro != null ? parceiro.CPF_CNPJ_Formatado + " - " + parceiro.Nome : "Todos"));
                parametros.Add(new ReportParameter("Tipo", tipo != null ? tipo.Value.ToString("G") : "Todos"));
                parametros.Add(new ReportParameter("Status", "FRETES SUBCONTRATADOS PENDENTES DE FECHAMENTO"));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("FretesSubcontratados", fretesSubcontratados));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioFretesSubcontratados.rdlc", tipoArquivo, parametros, dataSources);
                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioFretesSubcontratados." + arquivo.FileNameExtension);

            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}