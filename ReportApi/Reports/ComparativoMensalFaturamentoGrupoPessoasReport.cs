using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.ComparativoMensalFaturamentoGrupoPessoas)]
public class ComparativoMensalFaturamentoGrupoPessoasReport : ReportBase
{
    public ComparativoMensalFaturamentoGrupoPessoasReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        List<int> codigosGruposPessoas = extraData.GetValue<string>("GruposPessoas").FromJson<List<int>>();
        int quantidadeMesesAnteriores = extraData.GetValue<int>("QuantidadeMesesAnteriores");
        DateTime dataInicialEmissao = extraData.GetValue<DateTime>("DataInicialEmissao");
        DateTime dataFinalEmissao = extraData.GetValue<DateTime>("DataFinalEmissao");
        string propriedadeVeiculo = extraData.GetValue<string>("PropriedadeVeiculo");
        
        bool tipoArquivoPDF = extraData.GetValue<bool>("TipoArquivoPDF");

        Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivo =
            tipoArquivoPDF ? TipoArquivoRelatorio.PDF : TipoArquivoRelatorio.XLS;
        
        List<Dominio.ObjetosDeValor.Embarcador.CTe.Periodo> periodos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Periodo>();

                for (var i = (quantidadeMesesAnteriores - 1); i >= 0; i--)
                    periodos.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.Periodo(dataInicialEmissao.AddMonths(-i), dataFinalEmissao.AddMonths(-i)));

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ComparativoFaturamentoGrupoPessoas> comparativos = repCTe.ConsultarRelatorioComparativoMensalFaturamentoGrupoPessoas(periodos, codigosGruposPessoas, propriedadeVeiculo, BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa")).TipoAmbiente);
                List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ComparativoFaturamentoGrupoPessoasDetalhes> comparativosRelatorio = new List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ComparativoFaturamentoGrupoPessoasDetalhes>();

                List<int> gruposPessoasExistentes = (from obj in comparativos select obj.CodigoGrupoPessoas).Distinct().ToList();

                foreach (int grupoPessoas in gruposPessoasExistentes)
                {
                    Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ComparativoFaturamentoGrupoPessoasDetalhes comparativoRelatorio = new Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ComparativoFaturamentoGrupoPessoasDetalhes()
                    {
                        CodigoGrupoPessoas = grupoPessoas,
                        GrupoPessoas = grupoPessoas <= 0 ? "Sem Grupo de Pessoas" : comparativos.Where(o => o.CodigoGrupoPessoas == grupoPessoas).Select(o => o.GrupoPessoas).FirstOrDefault()
                    };

                    int mes = 1;

                    foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Periodo periodo in periodos)
                    {
                        decimal valor = comparativos.Where(o => o.CodigoGrupoPessoas == grupoPessoas && o.Mes == periodo.DataInicial.Month && o.Ano == periodo.DataInicial.Year).Sum(o => o.ValorReceber);

                        if (mes == 1)
                            comparativoRelatorio.Mes1 = valor;
                        else if (mes == 2)
                            comparativoRelatorio.Mes2 = valor;
                        else if (mes == 3)
                            comparativoRelatorio.Mes3 = valor;
                        else if (mes == 4)
                            comparativoRelatorio.Mes4 = valor;
                        else if (mes == 5)
                            comparativoRelatorio.Mes5 = valor;
                        else if (mes == 6)
                            comparativoRelatorio.Mes6 = valor;
                        else if (mes == 7)
                            comparativoRelatorio.Mes7 = valor;
                        else if (mes == 8)
                            comparativoRelatorio.Mes8 = valor;
                        else if (mes == 9)
                            comparativoRelatorio.Mes9 = valor;
                        else if (mes == 10)
                            comparativoRelatorio.Mes10 = valor;
                        else if (mes == 11)
                            comparativoRelatorio.Mes11 = valor;
                        else if (mes == 12)
                            comparativoRelatorio.Mes12 = valor;

                        mes++;
                    }

                    comparativosRelatorio.Add(comparativoRelatorio);
                }

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = codigosGruposPessoas.Any() ? repGrupoPessoas.BuscarPorCodigo(codigosGruposPessoas.ToArray()) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

                Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    DataSet = comparativosRelatorio,
                    Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
                    {
                        new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoBase", dataInicialEmissao.ToString("dd") + " à " + dataFinalEmissao.ToString("dd/MM/yyyy"), true),
                        new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("QuantidadeMesesAnteriores", quantidadeMesesAnteriores.ToString(), true),
                        new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PropriedadeVeiculo", propriedadeVeiculo == "T" ? "Terceiros" : propriedadeVeiculo == "P" ? "Próprio" : propriedadeVeiculo == "O" ? "Outros" : "Todos", true),
                        new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", gruposPessoas.Any()? string.Join(", ", gruposPessoas.Select(o => o.Descricao)) : "Todos", true)
                    }
                };

                for (int i = 1; i <= 12; i++)
                {
                    string descricao = string.Empty;

                    if (periodos.Count >= i)
                        descricao = periodos[i - 1].Descricao;

                    dataSet.Parameters.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Mes" + i.ToString(), descricao, true));
                }

                byte[] relatorio = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\CTe\ComparativoMensalFaturamentoGrupoPessoas.rpt", tipoArquivo, dataSet, true);

                return PrepareReportResult(tipoArquivoPDF ? FileType.PDF : FileType.EXCEL, relatorio);
    }
}