using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RelatorioDREController : ApiController
    {
        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorio()
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            DateTime dataInicial, dataFinal;
            DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
            DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

            int codigoVeiculo, codigoMotorista = 0;
            int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
            int.TryParse(Request.Params["Motorista"], out codigoMotorista);

            string tipoArquivo = Request.Params["TipoArquivo"];

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            List<Dominio.ObjetosDeValor.Relatorios.RelatorioAbastecimentoAgrupado> listaAbastecimentos = repAbastecimento.ObterDadosSumarizadosPorPeriodo(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, codigoVeiculo);

            Repositorio.CustoFixo repCustoFixo = new Repositorio.CustoFixo(unitOfWork);
            List<Dominio.Entidades.CustoFixo> listaCustosFixos = repCustoFixo.BuscarPorPeriodoEVeiculo(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, codigoVeiculo, codigoMotorista);

            Repositorio.PlanoDeConta repPlanoConta = new Repositorio.PlanoDeConta(unitOfWork);
            Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unitOfWork);

            List<Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado> listaMovimentos = repMovimento.ObterMovimentosAgrupadosPorPeriodo(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, codigoVeiculo, codigoMotorista);
            List<string> grupoConta = (from obj in listaMovimentos where obj.Conta.Contains('.') select obj.Conta.Substring(0, obj.Conta.LastIndexOf('.'))).Distinct().ToList();
            List<Dominio.ObjetosDeValor.Relatorios.RelatorioCustoFixoAgrupado> listaCustosFixosAgrupados = new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCustoFixoAgrupado>();
            List<Dominio.ObjetosDeValor.Relatorios.DescricaoEValor> listaItensGrafico = new List<Dominio.ObjetosDeValor.Relatorios.DescricaoEValor>();

            foreach (string conta in grupoConta)
                this.ObterContaPai(ref listaMovimentos, conta, unitOfWork);

            List<int> listaPlanos = (from obj in listaMovimentos where obj.TipoPlano.Equals("A") select obj.CodigoPlano).Distinct().ToList();

            for (DateTime dt = dataInicial; dt <= dataFinal; dt = dt.AddMonths(1))
            {
                this.ObterCustoFixoSumarizado(ref listaCustosFixosAgrupados, listaCustosFixos, dt.Month, dt.Year);

                this.ObterAbastecimentosSumarizados(ref listaAbastecimentos, listaMovimentos, listaCustosFixosAgrupados, dt.Month, dt.Year);

                this.ObterMovimentosSumarizados(ref listaMovimentos, listaPlanos, dt.Month, dt.Year);
            }

            listaItensGrafico.AddRange((from obj in listaMovimentos
                                        where obj.ValorTotal > 0
                                        group obj by obj.CodigoPlano into objetos
                                        select new Dominio.ObjetosDeValor.Relatorios.DescricaoEValor() { Descricao = objetos.First().DescricaoPlano, Valor = objetos.Sum(o => o.ValorTotal) }).ToList());

            listaItensGrafico.Add(new Dominio.ObjetosDeValor.Relatorios.DescricaoEValor() { Descricao = "Custos Fixos", Valor = listaCustosFixosAgrupados.Sum(o => o.Valor) });

            List<ReportParameter> parametros = new List<ReportParameter>();
            parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.RazaoSocial));
            parametros.Add(new ReportParameter("Periodo", string.Concat("De ", dataInicial.ToString("dd/MM/yyyy"), " até ", dataFinal.ToString("dd/MM/yyyy"))));
            parametros.Add(new ReportParameter("Veiculo", veiculo != null ? veiculo.Placa : string.Empty));

            List<ReportDataSource> dataSources = new List<ReportDataSource>();
            dataSources.Add(new ReportDataSource("Abastecimentos", listaAbastecimentos));
            dataSources.Add(new ReportDataSource("Movimentos", listaMovimentos));
            dataSources.Add(new ReportDataSource("CustosFixos", listaCustosFixosAgrupados));
            dataSources.Add(new ReportDataSource("Grafico", listaItensGrafico));

            Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

            Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioDRE.rdlc", tipoArquivo, parametros, dataSources);

            return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioDRE." + arquivo.FileNameExtension);
        }

        #region Métodos Privados

        private void ObterCustoFixoSumarizado(ref List<Dominio.ObjetosDeValor.Relatorios.RelatorioCustoFixoAgrupado> listaCustosFixosAgrupados, List<Dominio.Entidades.CustoFixo> listaCustosFixos, int mes, int ano)
        {
            DateTime dataInicial = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
            DateTime dataFinal = new DateTime(ano, mes, 1);

            listaCustosFixosAgrupados.AddRange((from obj in listaCustosFixos
                                                select new Dominio.ObjetosDeValor.Relatorios.RelatorioCustoFixoAgrupado()
                                                {
                                                    Ano = ano,
                                                    Mes = mes,
                                                    Descricao = obj.Descricao,
                                                    PlacaVeiculo = obj.Veiculo != null ? obj.PlacaVeiculo : obj.Funcionario != null ? obj.Motorista : string.Empty ,
                                                    Valor = obj.DataInicial <= dataInicial && obj.DataFinal >= dataFinal ? obj.ValorMensal : 0
                                                }).ToList());
        }

        private void ObterAbastecimentosSumarizados(ref List<Dominio.ObjetosDeValor.Relatorios.RelatorioAbastecimentoAgrupado> listaAbastecimentos, List<Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado> listaMovimentos, List<Dominio.ObjetosDeValor.Relatorios.RelatorioCustoFixoAgrupado> listaCustoFixo, int mes, int ano)
        {
            Dominio.ObjetosDeValor.Relatorios.RelatorioAbastecimentoAgrupado abastecimento = listaAbastecimentos.Where(o => o.Mes == mes && o.Ano == ano).FirstOrDefault();
            if (abastecimento == null)
            {
                listaAbastecimentos.Add(new Dominio.ObjetosDeValor.Relatorios.RelatorioAbastecimentoAgrupado()
                {
                    Ano = ano,
                    Mes = mes,
                    LitrosGastos = 0,
                    QuilometrosRodados = 0,
                    ValorTotalDespesa = (from obj in listaMovimentos
                                         where
                                             obj.Mes == mes &&
                                             obj.Ano == ano &&
                                             obj.TipoConta.Equals("D")
                                         select obj.ValorTotal).Sum() +
                                        (from obj in listaCustoFixo
                                         where
                                             obj.Mes == mes &&
                                             obj.Ano == ano
                                         select obj.Valor).Sum(),
                    ValorTotalReceita = (from obj in listaMovimentos
                                         where
                                             obj.Mes == mes &&
                                             obj.Ano == ano &&
                                             obj.TipoConta.Equals("R")
                                         select obj.ValorTotal).Sum()
                });
            }
            else
            {
                int indice = listaAbastecimentos.FindIndex(o => o.Mes == mes && o.Ano == ano);

                listaAbastecimentos[indice].ValorTotalDespesa = (from obj in listaMovimentos
                                                                 where
                                                                     obj.Mes == mes &&
                                                                     obj.Ano == ano &&
                                                                     obj.TipoConta.Equals("D")
                                                                 select obj.ValorTotal).Sum();

                listaAbastecimentos[indice].ValorTotalDespesa += (from obj in listaCustoFixo
                                                                  where
                                                                  obj.Mes == mes &&
                                                                  obj.Ano == ano
                                                                  select obj.Valor).Sum();

                listaAbastecimentos[indice].ValorTotalReceita = (from obj in listaMovimentos
                                                                 where
                                                                     obj.Mes == mes &&
                                                                     obj.Ano == ano &&
                                                                     obj.TipoConta.Equals("R")
                                                                 select obj.ValorTotal).Sum();
            }
        }

        private void ObterMovimentosSumarizados(ref List<Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado> listaMovimentos, List<int> listaPlanos, int mes, int ano)
        {
            foreach (int codigoPlano in listaPlanos)
            {
                Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado movimento = (from obj in listaMovimentos where obj.CodigoPlano == codigoPlano && obj.Mes == mes && obj.Ano == ano select obj).FirstOrDefault();
                if (movimento == null)
                {
                    movimento = (from obj in listaMovimentos where obj.CodigoPlano == codigoPlano select obj).FirstOrDefault();
                    listaMovimentos.Add(new Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado()
                    {
                        Ano = ano,
                        Mes = mes,
                        CodigoPlano = codigoPlano,
                        Conta = movimento.Conta,
                        DescricaoPlano = movimento.DescricaoPlano,
                        TipoConta = movimento.TipoConta,
                        TipoPlano = movimento.TipoPlano,
                        ValorTotal = 0
                    });
                }
            }
        }

        private void ObterContaPai(ref List<Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado> listaMovimentos, string conta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.PlanoDeConta repPlanoConta = new Repositorio.PlanoDeConta(unitOfWork);
            Dominio.Entidades.PlanoDeConta plano = repPlanoConta.BuscarPorConta(this.EmpresaUsuario.Codigo, conta);
            if (plano != null)
            {
                listaMovimentos.Add(new Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado()
                {
                    CodigoPlano = plano.Codigo,
                    Conta = plano.Conta,
                    DescricaoPlano = plano.Descricao,
                    TipoConta = plano.TipoDeConta,
                    TipoPlano = plano.Tipo,
                    ValorTotal = 0,
                    Mes = 0,
                    Ano = 0
                });

                if (conta.IndexOf('.') > 0)
                {
                    this.ObterContaPai(ref listaMovimentos, conta.Substring(0, conta.LastIndexOf('.')), unitOfWork);
                }
            }
        }

        #endregion
    }
}
