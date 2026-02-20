using CsvHelper;
using ReportApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Relatorios;

public class RelatorioReportService : Servicos.Embarcador.Relatorios.Relatorio
{
    public RelatorioReportService(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
    {
        _clienteMultisoftware = ConnectionFactory.Cliente;
        _unitOfWork = unitOfWork;
    }

    #region Atributos Privados

    private bool utilizaFormatoCSV = false;
    private dynamic dataSourceCSV;
    private Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorioCSV;
    private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
    private readonly Repositorio.UnitOfWork _unitOfWork;

    private void AjustarLinhasRelatorio(CrystalDecisions.CrystalReports.Engine.ReportObjects reportObjects, CrystalDecisions.CrystalReports.Engine.ReportDocument report, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT)
    {
        if (IdentificacaoCamposRPT.AjustarAutomaticamenteLinhas)
        {
            foreach (CrystalDecisions.CrystalReports.Engine.ReportObject linha in reportObjects)
            {
                if (linha.Kind == CrystalDecisions.Shared.ReportObjectKind.LineObject)
                {
                    CrystalDecisions.CrystalReports.Engine.LineObject line = (CrystalDecisions.CrystalReports.Engine.LineObject)linha;
                    line.Right = report.PrintOptions.PageContentWidth;
                    line.Left = 0;
                }
            }
        }
    }

    private CrystalDecisions.CrystalReports.Engine.ReportObject BuscarFieldHeadingObject(CrystalDecisions.CrystalReports.Engine.ReportDocument report, string fieldObjectName)
    {
        CrystalDecisions.CrystalReports.Engine.ReportObject cabelalhoCrystal = null;

        foreach (CrystalDecisions.CrystalReports.Engine.ReportObject header in report.ReportDefinition.ReportObjects)
        {
            if (header.Kind == CrystalDecisions.Shared.ReportObjectKind.FieldHeadingObject)
            {
                CrystalDecisions.CrystalReports.Engine.FieldHeadingObject cabecalho = (CrystalDecisions.CrystalReports.Engine.FieldHeadingObject)header;
                if (cabecalho.FieldObjectName == fieldObjectName)
                {
                    cabelalhoCrystal = header;
                    break;
                }
            }
        }
        return cabelalhoCrystal;
    }

    private void FormatarRelatorio(CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio, Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio, CrystalDecisions.CrystalReports.Engine.PrintOptions opcoesImpressao, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT, bool ajustarLinhasAutomaticamente)
    {
        try
        {

            if (IdentificacaoCamposRPT == null)
                IdentificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();

            System.Drawing.Font fonte = new System.Drawing.Font(configuracaoRelatorio.FontePadrao, configuracaoRelatorio.TamanhoPadraoFonte);
            System.Drawing.Font fonteBold = new System.Drawing.Font(configuracaoRelatorio.FontePadrao, configuracaoRelatorio.TamanhoPadraoFonte, System.Drawing.FontStyle.Bold);
            int tamanhoPaginaRelatorio = opcoesImpressao.PageContentWidth;

            if (relatorio.DataDefinition.Groups.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(configuracaoRelatorio.PropriedadeAgrupa))
                {
                    CrystalDecisions.CrystalReports.Engine.FieldDefinition fieldAgrupa = relatorio.Database.Tables[0].Fields[configuracaoRelatorio.PropriedadeAgrupa];

                    if (!string.IsNullOrEmpty(IdentificacaoCamposRPT.GrupoFormula))
                    {
                        relatorio.DataDefinition.FormulaFields[IdentificacaoCamposRPT.GrupoFormula].Text = fieldAgrupa.FormulaName;
                        if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.GrupoSort))
                        {
                            if (configuracaoRelatorio.OrdemAgrupamento == "asc")
                                relatorio.DataDefinition.FormulaFields[IdentificacaoCamposRPT.GrupoSort].Text = "crAscendingOrder";
                            else
                                relatorio.DataDefinition.FormulaFields[IdentificacaoCamposRPT.GrupoSort].Text = "crDescendingOrder";
                        }
                    }

                    CrystalDecisions.CrystalReports.Engine.ReportObject formulaAgrupamento = relatorio.ReportDefinition.ReportObjects[IdentificacaoCamposRPT.GroupNameGrupoFormula];

                    formulaAgrupamento.Width = tamanhoPaginaRelatorio;
                    formulaAgrupamento.Left = 0;
                    formulaAgrupamento.Height = formulaAgrupamento.Height + 20;
                    formulaAgrupamento.ObjectFormat.EnableCanGrow = true;

                    CrystalDecisions.CrystalReports.Engine.FieldObject fieldAgrupamento = (CrystalDecisions.CrystalReports.Engine.FieldObject)formulaAgrupamento;

                    fieldAgrupamento.ApplyFont(fonteBold);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.GroupFooterSection))
                        relatorio.ReportDefinition.Sections[IdentificacaoCamposRPT.GroupFooterSection].SectionFormat.EnableSuppress = true;

                    if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.GroupHeaderSection))
                        relatorio.ReportDefinition.Sections[IdentificacaoCamposRPT.GroupHeaderSection].SectionFormat.EnableSuppress = true;
                }
            }

            if (!string.IsNullOrEmpty(IdentificacaoCamposRPT.CorFundoNomeFormula))
            {
                if (configuracaoRelatorio.FundoListrado)
                    relatorio.DataDefinition.FormulaFields[IdentificacaoCamposRPT.CorFundoNomeFormula].Text = "if RecordNumber mod 2 = 0 then " + IdentificacaoCamposRPT.CorFundoListra + " else crNoColor";
                else
                    relatorio.DataDefinition.FormulaFields[IdentificacaoCamposRPT.CorFundoNomeFormula].Text = "crNoColor";
            }

            int tamanhoLiberado = 0;
            int tamanhoEspacamento = 75;
            int TamanhoTotalEspacamento = configuracaoRelatorio.Colunas != null ? tamanhoEspacamento * (from obj in configuracaoRelatorio.Colunas where obj.Visivel select obj).Count() : 1050;
            int tamanhoPagina = tamanhoPaginaRelatorio - TamanhoTotalEspacamento;
            decimal diferencaPercentual = configuracaoRelatorio.Colunas != null ? 100 - configuracaoRelatorio.Colunas.Sum(obj => obj.Tamanho) : -19;

            if (diferencaPercentual != 0)
                tamanhoLiberado += ObterTamanhoCampo(tamanhoPagina, diferencaPercentual);

            if (configuracaoRelatorio.Colunas != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna coluna in configuracaoRelatorio.Colunas)
                {
                    try
                    {
                        relatorio.ReportDefinition.ReportObjects[coluna.Propriedade + IdentificacaoCamposRPT.IndiceData].Width = ObterTamanhoCampo(tamanhoPagina, coluna.Tamanho);

                        if (!coluna.Visivel)
                            tamanhoLiberado += relatorio.ReportDefinition.ReportObjects[coluna.Propriedade + IdentificacaoCamposRPT.IndiceData].Width;
                    }
                    catch (Exception ex)
                    {
                        Log.TratarErro($"Ocorreu uma falha ao Adicionar a Coluna {coluna.Titulo} - Prop {coluna.Propriedade} : \n {ex}");
                    }


                }
            }

            if (ajustarLinhasAutomaticamente)
            {
                foreach (CrystalDecisions.CrystalReports.Engine.ReportObject linha in relatorio.ReportDefinition.ReportObjects)
                {
                    if (linha.Kind == CrystalDecisions.Shared.ReportObjectKind.LineObject)
                    {
                        CrystalDecisions.CrystalReports.Engine.DrawingObject line = (CrystalDecisions.CrystalReports.Engine.DrawingObject)linha;
                        line.Right = relatorio.PrintOptions.PageContentWidth;
                        linha.Left = 0;
                    }
                }

                AjustarLinhasRelatorio(relatorio.ReportDefinition.ReportObjects, relatorio, IdentificacaoCamposRPT);
            }

            int left = 0;
            int leftUltimaColunaCount = 0;
            int widthPrimeiraColunaCount = 0;
            int widthUltimaColunaCount = 0;
            bool finalizouPrimeiraColuna = false;
            bool primeiraColunaLiberada = false;
            bool ultimaColunaLiberada = false;

            if (configuracaoRelatorio.Colunas != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna ultimaColunaVisivel = configuracaoRelatorio.Colunas.Where(o => o.Visivel).LastOrDefault();

                foreach (Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna coluna in configuracaoRelatorio.Colunas)
                {

                    CrystalDecisions.CrystalReports.Engine.ReportObject colunaCrystal = relatorio.ReportDefinition.ReportObjects[coluna.Propriedade + IdentificacaoCamposRPT.IndiceData];
                    CrystalDecisions.CrystalReports.Engine.ReportObject cabelalhoCrystal = BuscarFieldHeadingObject(relatorio, colunaCrystal.Name);

                    string prefixoCampoSum = coluna.TipoSumarizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.media && IdentificacaoCamposRPT.PrefixoCamposSum.Equals("Somade") ? "Médiade" : IdentificacaoCamposRPT.PrefixoCamposSum;

                    if (coluna.Visivel)
                    {
                        CrystalDecisions.Shared.Alignment defaulAling = CrystalDecisions.Shared.Alignment.LeftAlign;
                        if (coluna.Alinhamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.center)
                            defaulAling = CrystalDecisions.Shared.Alignment.HorizontalCenterAlign;

                        if (coluna.Alinhamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.right)
                            defaulAling = CrystalDecisions.Shared.Alignment.RightAlign;

                        colunaCrystal.ObjectFormat.EnableSuppress = false;
                        colunaCrystal.Width = ObterDiferencaTamanho(colunaCrystal.Width, tamanhoLiberado, tamanhoPagina);

                        if (coluna.Propriedade == ultimaColunaVisivel.Propriedade)
                        {
                            colunaCrystal.Width += ((left + colunaCrystal.Width + tamanhoEspacamento) - tamanhoPaginaRelatorio) + tamanhoEspacamento;

                            if (colunaCrystal.Width < 1)
                                colunaCrystal.Width = 1;
                        }

                        colunaCrystal.Left = left;
                        colunaCrystal.ObjectFormat.HorizontalAlignment = defaulAling;
                        colunaCrystal.ObjectFormat.EnableCanGrow = !configuracaoRelatorio.CortarLinhas;

                        CrystalDecisions.CrystalReports.Engine.FieldObject field = (CrystalDecisions.CrystalReports.Engine.FieldObject)colunaCrystal;
                        field.ApplyFont(fonte);

                        if (cabelalhoCrystal != null)
                        {
                            CrystalDecisions.CrystalReports.Engine.TextObject textoCabeca = (CrystalDecisions.CrystalReports.Engine.TextObject)cabelalhoCrystal;
                            textoCabeca.Text = coluna.Titulo;
                            textoCabeca.ApplyFont(fonteBold);

                            cabelalhoCrystal.ObjectFormat.EnableSuppress = false;
                            cabelalhoCrystal.Width = colunaCrystal.Width;
                            cabelalhoCrystal.ObjectFormat.HorizontalAlignment = defaulAling;
                            cabelalhoCrystal.Left = left;

                            cabelalhoCrystal.ObjectFormat.EnableCanGrow = !configuracaoRelatorio.CortarLinhas;
                        }

                        if (configuracaoRelatorio.ExibirSumarios && (tipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.PDF || configuracaoRelatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R009_ResultadoAcertoViagem || configuracaoRelatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R030_Titulo))
                        {
                            if (coluna.TipoSumarizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum)
                            {
                                if (!string.IsNullOrWhiteSpace(configuracaoRelatorio.PropriedadeAgrupa))
                                {
                                    if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.GroupFooterSection))
                                        relatorio.ReportDefinition.Sections[IdentificacaoCamposRPT.GroupFooterSection].SectionFormat.EnableSuppress = false;

                                    CrystalDecisions.CrystalReports.Engine.ReportObject colunaGroupSumary = relatorio.ReportDefinition.ReportObjects[prefixoCampoSum + coluna.Propriedade + IdentificacaoCamposRPT.IndiceSumGroup];
                                    colunaGroupSumary.ObjectFormat.EnableSuppress = false;
                                    colunaGroupSumary.Width = colunaCrystal.Width;
                                    colunaGroupSumary.Left = left;
                                    colunaGroupSumary.ObjectFormat.HorizontalAlignment = defaulAling;
                                    colunaGroupSumary.ObjectFormat.EnableCanGrow = !configuracaoRelatorio.CortarLinhas;

                                    CrystalDecisions.CrystalReports.Engine.FieldObject fieldGroupSumary = (CrystalDecisions.CrystalReports.Engine.FieldObject)colunaGroupSumary;
                                    fieldGroupSumary.ApplyFont(fonteBold);
                                }

                                if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.SectionSumary))
                                    relatorio.ReportDefinition.Sections[IdentificacaoCamposRPT.SectionSumary].SectionFormat.EnableSuppress = false;

                                CrystalDecisions.CrystalReports.Engine.ReportObject colunaSumary = relatorio.ReportDefinition.ReportObjects[prefixoCampoSum + coluna.Propriedade + IdentificacaoCamposRPT.IndiceSumReport];
                                colunaSumary.ObjectFormat.EnableSuppress = false;
                                colunaSumary.Width = colunaCrystal.Width;
                                colunaSumary.Left = left;

                                colunaSumary.ObjectFormat.HorizontalAlignment = defaulAling;
                                colunaSumary.ObjectFormat.EnableCanGrow = !configuracaoRelatorio.CortarLinhas;

                                CrystalDecisions.CrystalReports.Engine.FieldObject fieldSumary = (CrystalDecisions.CrystalReports.Engine.FieldObject)colunaSumary;

                                fieldSumary.ApplyFont(fonteBold);

                                leftUltimaColunaCount = (left + colunaSumary.Width + tamanhoEspacamento);
                                widthUltimaColunaCount = 0;
                                finalizouPrimeiraColuna = true;
                            }
                            else
                            {
                                if (primeiraColunaLiberada && !finalizouPrimeiraColuna)
                                    widthPrimeiraColunaCount += colunaCrystal.Width;

                                if (left == 0)
                                {
                                    primeiraColunaLiberada = true;
                                    widthPrimeiraColunaCount = colunaCrystal.Width;
                                }

                                widthUltimaColunaCount += colunaCrystal.Width;
                                if (coluna.Propriedade == ultimaColunaVisivel.Propriedade)
                                    ultimaColunaLiberada = true;
                            }

                            if (coluna.Propriedade == ultimaColunaVisivel.Propriedade)
                            {
                                if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.ContadorRegistrosGrupo))
                                    SetarContador(relatorio, configuracaoRelatorio.CortarLinhas, IdentificacaoCamposRPT.ContadorRegistrosGrupo, fonteBold, ultimaColunaLiberada, widthUltimaColunaCount, leftUltimaColunaCount, primeiraColunaLiberada, widthPrimeiraColunaCount);

                                if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.ContadorRegistrosTotal))
                                    SetarContador(relatorio, configuracaoRelatorio.CortarLinhas, IdentificacaoCamposRPT.ContadorRegistrosTotal, fonteBold, ultimaColunaLiberada, widthUltimaColunaCount, leftUltimaColunaCount, primeiraColunaLiberada, widthPrimeiraColunaCount);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.SectionSumary))
                                relatorio.ReportDefinition.Sections[IdentificacaoCamposRPT.SectionSumary].SectionFormat.EnableSuppress = true;
                            if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.GroupFooterSection))
                                relatorio.ReportDefinition.Sections[IdentificacaoCamposRPT.GroupFooterSection].SectionFormat.EnableSuppress = true;
                        }

                        left += (colunaCrystal.Width + tamanhoEspacamento);
                    }
                    else
                    {
                        colunaCrystal.ObjectFormat.EnableSuppress = true;

                        if (cabelalhoCrystal != null)
                            cabelalhoCrystal.ObjectFormat.EnableSuppress = true;

                        if (coluna.TipoSumarizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum)
                        {
                            if (!string.IsNullOrWhiteSpace(IdentificacaoCamposRPT.GroupFooterSection))
                                relatorio.ReportDefinition.ReportObjects[prefixoCampoSum + coluna.Propriedade + IdentificacaoCamposRPT.IndiceSumGroup].ObjectFormat.EnableSuppress = true;

                            relatorio.ReportDefinition.ReportObjects[prefixoCampoSum + coluna.Propriedade + IdentificacaoCamposRPT.IndiceSumReport].ObjectFormat.EnableSuppress = true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private void GerarCSVMercante(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, string paginaRelatorio, Repositorio.UnitOfWork unitOfWork)
    {
        try
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Servicos.Embarcador.Notificacao.Notificacao(_unitOfWork.StringConexao, null, controleGeracao?.TipoServicoMultisoftware ?? 0, "");

            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

            string pastaRelatorios = ObterConfiguracaoArquivo(unitOfWork).CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, controleGeracao.GuidArquivo) + ".csv";

            using (var stream = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoArquivo))
            using (var writer = new System.IO.StreamWriter(stream))
            using (var csv = new CsvWriter(writer))
            {
                List<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna> colunasVisiveis = configuracaoRelatorioCSV.Colunas.Where(o => o.Visivel && !string.IsNullOrWhiteSpace(o.Titulo)).ToList();

                csv.Configuration.Delimiter = ";";
                csv.Configuration.CultureInfo = new System.Globalization.CultureInfo("pt-BR");
                //Cabecalho
                foreach (var coluna in colunasVisiveis)
                    csv.WriteField(coluna.Titulo);

                csv.NextRecord();

                foreach (dynamic dados in dataSourceCSV)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna coluna in colunasVisiveis)
                    {
                        dynamic propriedade = dados.GetType().GetProperty(coluna.Propriedade);
                        dynamic valor = propriedade.GetValue(dados);
                        string tipoPropriedade = propriedade.PropertyType.Name;

                        if ((tipoPropriedade == "Decimal") || (tipoPropriedade == "Double") || (coluna.DataTypeExportacao == "Decimal"))
                        {
                            if (coluna.DataTypeExportacao == "TimeSpan")
                                csv.WriteField(TimeSpan.FromMinutes((double)valor).ToString(@"hh\:mm\:ss"), false);
                            else
                            {
                                if (coluna.DataTypeExportacao == "Decimal" && tipoPropriedade == "String")
                                {
                                    decimal.TryParse((string)valor, out decimal valorConvertido);
                                    csv.WriteField(valorConvertido.ToString($"n{((coluna.PrecisaoDecimal > 0) ? coluna.PrecisaoDecimal.ToString() : "2")}", new System.Globalization.CultureInfo("en-US")).Replace(",", ""), false);
                                }
                                else
                                    csv.WriteField(valor.ToString($"n{((coluna.PrecisaoDecimal > 0) ? coluna.PrecisaoDecimal.ToString() : "2")}", new System.Globalization.CultureInfo("en-US")).Replace(",", ""), false);
                            }
                        }
                        else if (tipoPropriedade == "String")
                        {
                            if (coluna.UtilizarFormatoTexto)
                                csv.WriteField(Utilidades.String.ReplaceInvalidCharacters($"= \"{valor?.ToString() ?? ""}\""), false);
                            else
                                csv.WriteField(Utilidades.String.ReplaceInvalidCharacters($"{valor?.ToString() ?? ""}"), false);
                        }
                        else if (tipoPropriedade == "DateTime")
                            csv.WriteField(valor.ToString("dd/MM/yyyy HH:mm:ss"), false);
                        else if (tipoPropriedade == "TimeSpan")
                            csv.WriteField(valor.ToString("HH:mm:ss"), false);
                        else
                            csv.WriteField(valor);
                    }

                    csv.NextRecord();
                }
            }

            controleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo);
            controleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado;
            controleGeracao.DataFinalGeracao = DateTime.Now;
            repRelatorioControleGeracao.Atualizar(controleGeracao);


            Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.pdf;

            if (controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.XLS)
                icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.excel;

            serNotificaocao.GerarNotificacao(controleGeracao.Usuario, controleGeracao.Codigo, paginaRelatorio, String.Format(Localization.Resources.Relatorios.Relatorio.ORelatorioEstaDisponivelParaDownload, (controleGeracao.Titulo + " " + controleGeracao.DataFinalGeracao.ToString("dd/MM/yyyy HH:mm"))), icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, controleGeracao?.TipoServicoMultisoftware ?? 0, unitOfWork, _clienteMultisoftware?.Codigo ?? 0);
        }
        catch (Exception ex)
        {
            Servicos.Log.TratarErro(ex);
            ExcluirArquivoRelatorio(controleGeracao, unitOfWork);
        }

    }

    private void GerarCSV(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, string paginaRelatorio, Repositorio.UnitOfWork unitOfWork)
    {
        try
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Servicos.Embarcador.Notificacao.Notificacao(_unitOfWork.StringConexao, null, controleGeracao.TipoServicoMultisoftware ?? 0, "");

            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

            string pastaRelatorios = ObterConfiguracaoArquivo(unitOfWork).CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, controleGeracao.GuidArquivo) + ".csv";

            using (var stream = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoArquivo))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var csv = new CsvWriter(writer))
            {
                List<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna> colunasVisiveis = configuracaoRelatorioCSV.Colunas.Where(obj => obj.Visivel && !string.IsNullOrWhiteSpace(obj.Titulo)).ToList();

                csv.Configuration.Delimiter = ";";
                csv.Configuration.CultureInfo = new System.Globalization.CultureInfo("pt-BR");
                //Cabecalho
                foreach (var coluna in colunasVisiveis)
                    csv.WriteField(coluna.Titulo);

                csv.NextRecord();

                foreach (dynamic dados in dataSourceCSV)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna coluna in colunasVisiveis)
                    {
                        dynamic propriedade = dados.GetType().GetProperty(coluna.Propriedade);
                        dynamic valor = propriedade.GetValue(dados);
                        string tipoPropriedade = propriedade.PropertyType.Name;

                        if ((tipoPropriedade == "Decimal") || (tipoPropriedade == "Double") || (coluna.DataTypeExportacao == "Decimal"))
                        {
                            if (coluna.DataTypeExportacao == "TimeSpan")
                                csv.WriteField(TimeSpan.FromMinutes((double)valor).ToString(@"hh\:mm\:ss"));
                            else
                            {
                                if (coluna.DataTypeExportacao == "Decimal" && tipoPropriedade == "String")
                                {
                                    decimal.TryParse((string)valor, out decimal valorConvertido);
                                    csv.WriteField(valorConvertido.ToString($"n{((coluna.PrecisaoDecimal > 0) ? coluna.PrecisaoDecimal.ToString() : "2")}", new System.Globalization.CultureInfo("pt-BR")));
                                }
                                else
                                    csv.WriteField(valor.ToString($"n{((coluna.PrecisaoDecimal > 0) ? coluna.PrecisaoDecimal.ToString() : "2")}", new System.Globalization.CultureInfo("pt-BR")));
                            }
                        }
                        else if (tipoPropriedade == "String")
                        {
                            if (coluna.UtilizarFormatoTexto)
                                csv.WriteField($"=\"{valor?.ToString() ?? ""}\"", true);
                            else
                                csv.WriteField($"{valor?.ToString() ?? ""}", true);
                        }
                        else if (tipoPropriedade == "DateTime")
                            csv.WriteField(valor.ToString("dd/MM/yyyy HH:mm:ss"));
                        else if (tipoPropriedade == "TimeSpan")
                            csv.WriteField(valor.ToString("HH:mm:ss"));
                        else
                            csv.WriteField(valor);
                    }

                    csv.NextRecord();
                }
            }

            controleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo);
            controleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado;
            controleGeracao.DataFinalGeracao = DateTime.Now;
            repRelatorioControleGeracao.Atualizar(controleGeracao);


            Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.pdf;

            if (controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.XLS)
                icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.excel;

            serNotificaocao.GerarNotificacao(controleGeracao.Usuario, controleGeracao.Codigo, paginaRelatorio, String.Format(Localization.Resources.Relatorios.Relatorio.ORelatorioEstaDisponivelParaDownload, (controleGeracao.Titulo + " " + controleGeracao.DataFinalGeracao.ToString("dd/MM/yyyy HH:mm"))), icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, controleGeracao.TipoServicoMultisoftware ?? 0, unitOfWork, _clienteMultisoftware?.Codigo ?? 0);
        }
        catch (Exception ex)
        {
            Servicos.Log.TratarErro(ex);
            ExcluirArquivoRelatorio(controleGeracao, unitOfWork);
        }
    }

    private void InicializarCSV(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio, dynamic dataSource, Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo);

        relatorioControleGeracao.TipoArquivoRelatorio = Dominio.Enumeradores.TipoArquivoRelatorio.CSV;

        repRelatorioControleGeracao.Atualizar(relatorioControleGeracao);

        utilizaFormatoCSV = true;
        dataSourceCSV = dataSource;
        configuracaoRelatorioCSV = configuracaoRelatorio;
    }

    private int ObterDiferencaTamanho(int tamanhoElemento, int espacoLiberado, int tamanhoPagina)
    {
        int diferenca = tamanhoElemento * espacoLiberado / (tamanhoPagina - espacoLiberado);
        return (tamanhoElemento + diferenca) < 1 ? 1 : (tamanhoElemento + diferenca);
    }

    private int ObterTamanhoCampo(int tamanhoPagina, decimal percentual)
    {
        decimal retorno = percentual * tamanhoPagina / 100;
        if (retorno < 0)
            retorno = 1;
        return (int)retorno;
    }



    private void SetarContador(CrystalDecisions.CrystalReports.Engine.ReportDocument report, bool cortarLinhas, string ContadorRegistros, System.Drawing.Font fonte, bool ultimaColunaLiberada, int widthUltimaColunaCount, int leftUltimaColunaCount, bool primeiraColunaLiberada, int widthPrimeiraColunaCount)
    {
        CrystalDecisions.CrystalReports.Engine.ReportObject colunaCount = report.ReportDefinition.ReportObjects[ContadorRegistros];
        colunaCount.ObjectFormat.EnableSuppress = false;
        colunaCount.ObjectFormat.EnableCanGrow = !cortarLinhas;
        CrystalDecisions.CrystalReports.Engine.FieldObject fieldCountSumary = (CrystalDecisions.CrystalReports.Engine.FieldObject)colunaCount;
        fieldCountSumary.ApplyFont(fonte);

        if (ultimaColunaLiberada)
        {
            colunaCount.Width = widthUltimaColunaCount;
            colunaCount.Left = leftUltimaColunaCount;
            colunaCount.ObjectFormat.HorizontalAlignment = CrystalDecisions.Shared.Alignment.RightAlign;
        }
        else
        {
            if (primeiraColunaLiberada)
            {
                colunaCount.Width = widthPrimeiraColunaCount;
                colunaCount.Left = 0;
                colunaCount.ObjectFormat.HorizontalAlignment = CrystalDecisions.Shared.Alignment.LeftAlign;
            }
            else
            {
                colunaCount.ObjectFormat.EnableSuppress = true;
            }
        }
    }

    private bool UtilizarCSV(Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

        return configuracaoEmbarcador.UtilizarExportacaoRelatorioCSV;
    }

    #endregion

    #region Métodos Públicos

    public CrystalDecisions.CrystalReports.Engine.ReportDocument CriarRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, dynamic dataSource, Repositorio.UnitOfWork unitOfWork, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT = null, List<KeyValuePair<string, dynamic>> subReportDataSources = null, bool ajustarLinhasAutomaticamente = true, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServico = null, string caminhoLogo = null)
    {
        return CriarRelatorio(controleGeracao, ObterConfiguracaoRelatorio(relatorioTemp), dataSource, unitOfWork, IdentificacaoCamposRPT, subReportDataSources, ajustarLinhasAutomaticamente, tipoServico, caminhoLogo);
    }

    public CrystalDecisions.CrystalReports.Engine.ReportDocument CriarRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio, dynamic dataSource, Repositorio.UnitOfWork unitOfWork, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT = null, List<KeyValuePair<string, dynamic>> subReportDataSources = null, bool ajustarLinhasAutomaticamente = true, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServico = null, string caminhoLogo = null)
    {
        if ((controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.CSV ||
             controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.XLS ||
             controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.DOC) &&
            (UtilizarCSV(unitOfWork) ||
            controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R343_MonitoramentoNovo ||
             controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R233_AFRMMControlMercante ||
             controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R201_AFRMM ||
             controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R353_Permanencias ||
             controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R355_DevolucaoNotasFiscais
             ))
        {
            InicializarCSV(controleGeracao, configuracaoRelatorio, dataSource, unitOfWork);
            return null;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(caminhoLogo) && controleGeracao.Usuario != null && controleGeracao.Usuario.Empresa != null && !string.IsNullOrWhiteSpace(ObterConfiguracaoArquivo(unitOfWork).CaminhoLogoEmbarcador))
            {
                caminhoLogo = Utilidades.IO.FileStorageService.Storage.Combine(ObterConfiguracaoArquivo(unitOfWork).CaminhoLogoEmbarcador, controleGeracao.Usuario.Empresa.CNPJ + ".png");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoLogo))
                    caminhoLogo = "";
            }
        }
        catch
        {
        }

        if (IdentificacaoCamposRPT == null)
            IdentificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();

        CrystalDecisions.CrystalReports.Engine.ReportDocument report = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

        string diretorioBase = System.Configuration.ConfigurationManager.AppSettings["CaminhoRelatoriosCrystal"].ToString();

#if DEBUG
        diretorioBase = AppDomain.CurrentDomain.BaseDirectory;
#endif
        try
        {
            report.ReportClientDocument.LocaleID = (CrystalDecisions.ReportAppServer.DataDefModel.CeLocale)System.Globalization.CultureInfo.CurrentUICulture.LCID;

            report.Load(Utilidades.IO.FileStorageService.Storage.Combine(diretorioBase, configuracaoRelatorio.CaminhoRelatorio, configuracaoRelatorio.ArquivoRelatorio));
            report.PrintOptions.PaperOrientation = configuracaoRelatorio.OrientacaoRelatorio == Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato ? CrystalDecisions.Shared.PaperOrientation.Portrait : CrystalDecisions.Shared.PaperOrientation.Landscape;
            report.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            int count = report.ParameterFields.Count;

            FormatarRelatorio(report, configuracaoRelatorio, report.PrintOptions, controleGeracao.TipoArquivoRelatorio, IdentificacaoCamposRPT, ajustarLinhasAutomaticamente);

            if (subReportDataSources != null && subReportDataSources.Count > 0)
                for (var i = 0; i < subReportDataSources.Count; i++)
                    report.Subreports[subReportDataSources[i].Key].SetDataSource(subReportDataSources[i].Value);

            report.SetDataSource(dataSource);

            if (configuracaoRelatorio.OcultarDetalhe)
            {
                for (int i = 0; i < report.ReportDefinition.Sections.Count; i++)
                {
                    if (report.ReportDefinition.Sections[i].Kind == CrystalDecisions.Shared.AreaSectionKind.Detail)
                    {
                        report.ReportDefinition.Sections[i].SectionFormat.EnableSuppress = true;
                        break;
                    }
                }
            }

            if (configuracaoRelatorio.NovaPaginaAposAgrupamento)
            {
                for (int i = 0; i < report.ReportDefinition.Sections.Count; i++)
                {
                    if (report.ReportDefinition.Sections[i].Kind == CrystalDecisions.Shared.AreaSectionKind.GroupFooter)
                    {
                        report.ReportDefinition.Sections[i].SectionFormat.EnableNewPageAfter = true;
                        break;
                    }
                }
            }

            SetarSubCabecalhoRelatorio(report.Subreports, report, controleGeracao.TipoArquivoRelatorio, configuracaoRelatorio.Titulo, configuracaoRelatorio.FontePadrao, IdentificacaoCamposRPT, unitOfWork, tipoServico, caminhoLogo);
            SetarSubRodapeRelatorio(report.Subreports, report, configuracaoRelatorio.CodigoControleRelatorios, configuracaoRelatorio.FontePadrao, controleGeracao.TipoArquivoRelatorio, IdentificacaoCamposRPT);

            return report;
        }
        catch (Exception ex)
        {
            report.Dispose();
            throw;
        }
    }

    public void FormatarRelatorio(CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT)
    {
        try
        {
            FormatarRelatorio(relatorio, ObterConfiguracaoRelatorio(relatorioTemporario), relatorio.PrintOptions, tipoArquivoRelatorio, IdentificacaoCamposRPT, ajustarLinhasAutomaticamente: true);
        }
        catch
        {
            relatorio.Dispose();
            throw;
        }
    }

    public void FormatarRelatorio(CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio, CrystalDecisions.CrystalReports.Engine.ReportDocument relatorioPai, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT)
    {
        try
        {
            FormatarRelatorio(relatorio, ObterConfiguracaoRelatorio(relatorioTemporario), relatorioPai.PrintOptions, tipoArquivoRelatorio, IdentificacaoCamposRPT, ajustarLinhasAutomaticamente: true);
        }
        catch (Exception)
        {
            relatorioPai.Dispose();
            throw;
        }
    }

    public void FormatarRelatorio(CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio, CrystalDecisions.CrystalReports.Engine.ReportDocument relatorioPai, Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio relatorioTemporario, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT)
    {
        try
        {
            FormatarRelatorio(relatorio, relatorioTemporario, relatorioPai.PrintOptions, tipoArquivoRelatorio, IdentificacaoCamposRPT, ajustarLinhasAutomaticamente: true);
        }
        catch (Exception)
        {
            relatorioPai.Dispose();
            throw;
        }
    }

    public void GerarRelatorio(CrystalDecisions.CrystalReports.Engine.ReportDocument report, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, string paginaRelatorio, Repositorio.UnitOfWork unitOfWork)
    {
        Servicos.Log.TratarErro("GerarRelatorio codigo " + (controleGeracao?.Codigo ?? 0));

        if ((controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.CSV ||
             controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.XLS ||
             controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.DOC) &&
             (utilizaFormatoCSV ||
              controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R233_AFRMMControlMercante ||
              controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R201_AFRMM))
        {
            if (controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R233_AFRMMControlMercante || controleGeracao.Relatorio.CodigoControleRelatorios == Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R201_AFRMM)
                GerarCSVMercante(controleGeracao, paginaRelatorio, unitOfWork);
            else
                GerarCSV(controleGeracao, paginaRelatorio, unitOfWork);
            return;
        }

        try
        {

            using (report)
            {
                string pastaRelatorios = ObterConfiguracaoArquivo(unitOfWork).CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();

                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, controleGeracao.GuidArquivo);

                Servicos.Log.TratarErro("Report path: " + caminhoArquivo);
                Servicos.Log.TratarErro("Storage type: " + Utilidades.IO.FileStorageService.StorageType.ToString("G"));

                if (controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.PDF)
                {
                    using (System.IO.Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat))
                        Utilidades.IO.FileStorageService.Storage.SaveStream(caminhoArquivo + ".pdf", stream);
                }

                if (controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.XLS)
                {
                    using (System.IO.Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord))
                        Utilidades.IO.FileStorageService.Storage.SaveStream(caminhoArquivo + ".xls", stream);
                }
            }

            Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, controleGeracao.TipoServicoMultisoftware ?? 0, "");// informar 0 pois o relatório sempre deve ser notificado no mesmo serviçoMultisoftware que fez a requisição.
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
            Repositorio.Embarcador.Notificacoes.Notificacao repNotificacao = new Repositorio.Embarcador.Notificacoes.Notificacao(unitOfWork);
            controleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo); //busca novamente pois as configurações do relatorio podem ser setadas temporariamente na propriedade

            var controleGeracaoAtualizado = repRelatorioControleGeracao.BuscarPorCodigo(controleGeracao.Codigo); // ?? busca novamente pois as configurações do relatorio podem ser setadas temporariamente na propriedade 

            if (controleGeracaoAtualizado == null)
                throw new Exception("O registro de controle " + controleGeracao.Codigo + " ja nao existe ");

            controleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado;
            controleGeracao.DataFinalGeracao = DateTime.Now;

            repRelatorioControleGeracao.Atualizar(controleGeracao);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.pdf;

            if (controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.XLS)
                icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.excel;

            serNotificaocao.GerarNotificacao(controleGeracao.Usuario, controleGeracao.Codigo, paginaRelatorio, string.Format(Localization.Resources.Relatorios.Relatorio.ORelatorioEstaDisponivelParaDownload, $"{controleGeracao.Titulo} {controleGeracao.DataFinalGeracao:dd/MM/yyyy HH:mm)}"), icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, controleGeracao.TipoServicoMultisoftware ?? 0, unitOfWork, _clienteMultisoftware?.Codigo ?? 0);
        }
        catch (Exception excecao)
        {
            Servicos.Log.TratarErro(excecao);
            ExcluirArquivoRelatorio(controleGeracao, unitOfWork);
        }
        finally
        {
            report.Dispose();
            report = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    public void PreecherParamentrosFiltro(CrystalDecisions.CrystalReports.Engine.ReportDocument report, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, string idSelectFiltros = "ReportHeaderSection1", string reportHeaderSectionLine = "ReportHeaderSection2")
    {
        PreecherParamentrosFiltro(report, controleGeracao, relatorioTemp.FontePadrao, relatorioTemp.TamanhoPadraoFonte, parametros, idSelectFiltros, reportHeaderSectionLine);
    }

    public void PreecherParamentrosFiltro(CrystalDecisions.CrystalReports.Engine.ReportDocument report, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, string idSelectFiltros = "ReportHeaderSection1", string reportHeaderSectionLine = "ReportHeaderSection2")
    {
        PreecherParamentrosFiltro(report, controleGeracao, configuracaoRelatorio.FontePadrao, configuracaoRelatorio.TamanhoPadraoFonte, parametros, idSelectFiltros, reportHeaderSectionLine);
    }

    public void PreecherParamentrosFiltro(CrystalDecisions.CrystalReports.Engine.ReportDocument report, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, string fontePadrao, float tamanhoPadraoFonte, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, string idSelectFiltros = "ReportHeaderSection1", string ReportHeaderSectionLine = "ReportHeaderSection2")
    {
        if (utilizaFormatoCSV)
            return;

        try
        {
            System.Drawing.Font fonte = new System.Drawing.Font(fontePadrao, tamanhoPadraoFonte);
            System.Drawing.Font fonteBold = new System.Drawing.Font(fontePadrao, tamanhoPadraoFonte, System.Drawing.FontStyle.Bold);
            bool possuiParametroVisivel = false;
            int top = 0;
            int maiorWidthTextoVisivel = 0;

            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametro in parametros)
            {
                if (parametro.Visivel)
                {
                    CrystalDecisions.CrystalReports.Engine.TextObject textoParametroElemento = report.ReportDefinition.ReportObjects[parametro.IDTextoParametro] as CrystalDecisions.CrystalReports.Engine.TextObject;
                    if (textoParametroElemento.Width > maiorWidthTextoVisivel)
                        maiorWidthTextoVisivel = textoParametroElemento.Width;

                    if (!string.IsNullOrWhiteSpace(parametro.DescricaoParametro))
                        textoParametroElemento.Text = parametro.DescricaoParametro + ":";
                }
            }

            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro parametro in parametros)
            {
                report.SetParameterValue(parametro.NomeParametro, parametro.ValorParametro);
                CrystalDecisions.CrystalReports.Engine.ReportObject ParametroElemento = string.IsNullOrWhiteSpace(parametro.IDParametro) ? null : report.ReportDefinition.ReportObjects[parametro.IDParametro];
                CrystalDecisions.CrystalReports.Engine.ReportObject TextoParametroElemento = string.IsNullOrWhiteSpace(parametro.IDTextoParametro) ? null : report.ReportDefinition.ReportObjects[parametro.IDTextoParametro];

                if (parametro.Visivel)
                {
                    possuiParametroVisivel = true;

                    ((CrystalDecisions.CrystalReports.Engine.TextObject)TextoParametroElemento).ApplyFont(fonteBold);
                    TextoParametroElemento.ObjectFormat.EnableSuppress = false;
                    TextoParametroElemento.Left = 0;
                    TextoParametroElemento.Top = top;
                    TextoParametroElemento.Width = maiorWidthTextoVisivel;

                    ((CrystalDecisions.CrystalReports.Engine.FieldObject)ParametroElemento).ApplyFont(fonte);
                    ParametroElemento.ObjectFormat.EnableSuppress = false;
                    ParametroElemento.Left = TextoParametroElemento.Width;
                    ParametroElemento.Top = top;
                    ParametroElemento.Width = report.PrintOptions.PageContentWidth - TextoParametroElemento.Width;

                    top += ParametroElemento.Height;
                }
                else
                {
                    if (ParametroElemento != null)
                    {
                        ParametroElemento.ObjectFormat.EnableSuppress = true;
                        ParametroElemento.Top = 0;
                    }

                    if (TextoParametroElemento != null)
                    {
                        TextoParametroElemento.ObjectFormat.EnableSuppress = true;
                        TextoParametroElemento.Top = 0;
                    }
                }
            }

            if (possuiParametroVisivel && controleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.PDF)
            {
                if (!string.IsNullOrWhiteSpace(ReportHeaderSectionLine))
                    report.ReportDefinition.Sections[ReportHeaderSectionLine].SectionFormat.EnableSuppress = false;

                if (!string.IsNullOrWhiteSpace(idSelectFiltros))
                {
                    report.ReportDefinition.Sections[idSelectFiltros].SectionFormat.EnableSuppress = false;
                    report.ReportDefinition.Sections[idSelectFiltros].Height = top + 35;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(idSelectFiltros))
                    report.ReportDefinition.Sections[idSelectFiltros].SectionFormat.EnableSuppress = true;
                if (!string.IsNullOrWhiteSpace(ReportHeaderSectionLine))
                    report.ReportDefinition.Sections[ReportHeaderSectionLine].SectionFormat.EnableSuppress = true;
            }

            CrystalDecisions.Shared.ParameterField parametroRegistros = report.ParameterFields.Find("Registros", "");

            if (parametroRegistros != null)
                report.SetParameterValue("Registros", Localization.Resources.Relatorios.Relatorio.Registros);
        }
        catch (Exception ex)
        {
            report.Dispose();
            throw;
        }
    }



    public void SetarSubCabecalhoRelatorio(CrystalDecisions.CrystalReports.Engine.Subreports sub, CrystalDecisions.CrystalReports.Engine.ReportDocument report, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio, string titulo, string fontePadrao, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServico = null, string pathLogo = null)
    {
        try
        {
            string header = "header.rpt";
            sub[header].ReportDefinition.Sections["Padrao"].SectionFormat.EnableSuppress = false;
            report.SetParameterValue("TituloRelatorio", titulo, header);

            string caminhoLogo = Utilidades.IO.FileStorageService.LocalStorage.Combine(ObterConfiguracaoArquivo(unitOfWork).CaminhoLogoEmbarcador, "crystal.png");

            if (tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || Utilidades.IO.FileStorageService.LocalStorage.Exists(pathLogo))
            {
                if (Utilidades.IO.FileStorageService.LocalStorage.Exists(pathLogo))
                {
                    report.SetParameterValue("CaminhoImagem", pathLogo, header);
                }
                else
                {
                    sub[header].ReportDefinition.ReportObjects["Logo"].ObjectFormat.EnableSuppress = true;
                    report.SetParameterValue("CaminhoImagem", "", header);
                }
            }
            else
            {
                report.SetParameterValue("CaminhoImagem", caminhoLogo, header);
            }

            CrystalDecisions.CrystalReports.Engine.ReportObject tituloRelatorio = sub[header].ReportDefinition.ReportObjects["TituloRelatorio1"];
            tituloRelatorio.Left = 0;
            tituloRelatorio.Width = report.PrintOptions.PageContentWidth;

            CrystalDecisions.CrystalReports.Engine.FieldObject tituloRelatorioText = (CrystalDecisions.CrystalReports.Engine.FieldObject)tituloRelatorio;
            tituloRelatorioText.ApplyFont(new System.Drawing.Font(fontePadrao, tituloRelatorioText.Font.Size));

            CrystalDecisions.CrystalReports.Engine.ReportObject dataRelatorioInfo = sub[header].ReportDefinition.ReportObjects["DataText"];
            dataRelatorioInfo.Left = report.PrintOptions.PageContentWidth - dataRelatorioInfo.Width;
            CrystalDecisions.CrystalReports.Engine.TextObject dataRelatorioInfoText = (CrystalDecisions.CrystalReports.Engine.TextObject)dataRelatorioInfo;
            dataRelatorioInfoText.ApplyFont(new System.Drawing.Font(fontePadrao, dataRelatorioInfoText.Font.Size, System.Drawing.FontStyle.Bold));
            dataRelatorioInfoText.Text = Localization.Resources.Relatorios.Relatorio.Data + ":";

            CrystalDecisions.CrystalReports.Engine.ReportObject horaRelatorioInfo = sub[header].ReportDefinition.ReportObjects["HoraText"];
            horaRelatorioInfo.Left = report.PrintOptions.PageContentWidth - horaRelatorioInfo.Width;
            CrystalDecisions.CrystalReports.Engine.TextObject horaRelatorioInfoText = (CrystalDecisions.CrystalReports.Engine.TextObject)horaRelatorioInfo;
            horaRelatorioInfoText.ApplyFont(new System.Drawing.Font(fontePadrao, horaRelatorioInfoText.Font.Size, System.Drawing.FontStyle.Bold));
            horaRelatorioInfoText.Text = Localization.Resources.Relatorios.Relatorio.Hora + ":";

            CrystalDecisions.CrystalReports.Engine.ReportObject dataRelatorio = sub[header].ReportDefinition.ReportObjects["DatadosDados1"];
            dataRelatorio.Left = (dataRelatorioInfo.Left + 500);
            CrystalDecisions.CrystalReports.Engine.FieldObject dataRelatorioText = (CrystalDecisions.CrystalReports.Engine.FieldObject)dataRelatorio;
            dataRelatorioText.ApplyFont(new System.Drawing.Font(fontePadrao, dataRelatorioText.Font.Size));

            CrystalDecisions.CrystalReports.Engine.ReportObject horaRelatorio = sub[header].ReportDefinition.ReportObjects["HoradosDados1"];
            horaRelatorio.Left = (horaRelatorioInfo.Left + 500);
            CrystalDecisions.CrystalReports.Engine.FieldObject horaRelatorioText = (CrystalDecisions.CrystalReports.Engine.FieldObject)horaRelatorio;
            horaRelatorioText.ApplyFont(new System.Drawing.Font(fontePadrao, horaRelatorioText.Font.Size));

            AjustarLinhasRelatorio(sub[header].ReportDefinition.ReportObjects, report, IdentificacaoCamposRPT);

            if (tipoArquivoRelatorio != Dominio.Enumeradores.TipoArquivoRelatorio.PDF)
            {
                sub[header].ReportDefinition.Sections["Padrao"].SectionFormat.EnableSuppress = true;
                report.ReportDefinition.Sections["Section1"].SectionFormat.EnableSuppress = true;
            }
        }
        catch
        {
            report.Dispose();
            throw;
        }
    }

    public void SetarSubRodapeRelatorio(CrystalDecisions.CrystalReports.Engine.Subreports sub, CrystalDecisions.CrystalReports.Engine.ReportDocument report, Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorios, string fontePadrao, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT)
    {
        try
        {
            string rodape = "pagefooter.rpt";

            CrystalDecisions.CrystalReports.Engine.ReportObject codigoRelatorio = sub[rodape].ReportDefinition.ReportObjects["CodigoRelatorio"];
            codigoRelatorio.Left = 10;
            CrystalDecisions.CrystalReports.Engine.FieldObject codigoRelatorioText = (CrystalDecisions.CrystalReports.Engine.FieldObject)codigoRelatorio;
            codigoRelatorioText.ApplyFont(new System.Drawing.Font(fontePadrao, codigoRelatorioText.Font.Size, System.Drawing.FontStyle.Italic));

            report.SetParameterValue("CodigoRelatorio", "R. " + (int)codigoControleRelatorios, rodape);

            report.SetParameterValue("RodapeTexto", string.Format(Localization.Resources.Relatorios.Relatorio.DesenvolvidoPorMultisoftware, DateTime.Now.Year), rodape);
            CrystalDecisions.CrystalReports.Engine.ReportObject RodapeTexto = sub[rodape].ReportDefinition.ReportObjects["RodapeTexto"];
            RodapeTexto.Left = 0;
            RodapeTexto.Width = report.PrintOptions.PageContentWidth;
            CrystalDecisions.CrystalReports.Engine.FieldObject RodapeTextoText = (CrystalDecisions.CrystalReports.Engine.FieldObject)RodapeTexto;
            RodapeTextoText.ApplyFont(new System.Drawing.Font(fontePadrao, RodapeTextoText.Font.Size));

            CrystalDecisions.CrystalReports.Engine.ReportObject NumeroPaginas = sub[rodape].ReportDefinition.ReportObjects["NumeroPaginas"];
            NumeroPaginas.Left = 0;
            NumeroPaginas.Width = report.PrintOptions.PageContentWidth - 10;
            CrystalDecisions.CrystalReports.Engine.FieldObject NumeroPaginasText = (CrystalDecisions.CrystalReports.Engine.FieldObject)NumeroPaginas;
            NumeroPaginasText.ApplyFont(new System.Drawing.Font(fontePadrao, NumeroPaginasText.Font.Size));

            AjustarLinhasRelatorio(sub[rodape].ReportDefinition.ReportObjects, report, IdentificacaoCamposRPT);

            if (tipoArquivoRelatorio != Dominio.Enumeradores.TipoArquivoRelatorio.PDF)
                sub[rodape].ReportDefinition.Sections["ReportFooterSection1"].SectionFormat.EnableSuppress = true;
        }
        catch (Exception)
        {
            report.Dispose();
            throw;
        }
    }

    #endregion
}