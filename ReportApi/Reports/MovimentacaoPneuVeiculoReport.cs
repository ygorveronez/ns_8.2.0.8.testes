using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.MovimentacaoPneuVeiculo)]
public class MovimentacaoPneuVeiculoReport : ReportBase
{
    public MovimentacaoPneuVeiculoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        try
        {
            int codigoVeiculo = extraData.GetValue<int>("codigoVeiculo");
            bool ocultarDados = extraData.GetValue<bool>("ocultarDados");
            string caminhimagemPneuDesabilitado = extraData.GetValue<string>("caminhimagemPneuDesabilitado");
            string caminhoimagemEixoDuplo = extraData.GetValue<string>("caminhoimagemEixoDuplo");
            string caminhoimagemEixoSimples = extraData.GetValue<string>("caminhoimagemEixoSimples");
            string caminhoimagemPneu = extraData.GetValue<string>("caminhoimagemPneu");

            var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

            var relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

            var relatorioTemporario = extraData.GetValue<string>("relatorioTemporario").FromJson<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio>();

            List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados> dataSourceMovimentacaoPneuVeiculoDados = extraData.GetValue<string>("dataSourceMovimentacaoPneuVeiculoDados").FromJson<List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculoDados>>();

            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(_unitOfWork);
            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(codigoVeiculo, _unitOfWork);
            List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculo> dataSourceMovimentacaoPneuVeiculo = new List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculo>() {
                    new Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoPneuVeiculo() {
                        Veiculo = veiculo.Placa_Formatada,
                        Imagem = CriarImagemVeiculo(veiculo,caminhimagemPneuDesabilitado, caminhoimagemEixoDuplo, caminhoimagemEixoSimples, caminhoimagemPneu),
                        TotalDados = dataSourceMovimentacaoPneuVeiculoDados.Count
                    }
                };

            ReportDataSet dataSetDados = new ReportDataSet()
            {
                Key = "MovimentacaoPneuVeiculoDados.rpt",
                DataSet = dataSourceMovimentacaoPneuVeiculoDados
            };

            List<ReportDataSet> subReports = new List<ReportDataSet>() { dataSetDados };

            ReportDataSet dataSet = new ReportDataSet()
            {
                DataSet = dataSourceMovimentacaoPneuVeiculo,
                SubReports = subReports
            };

            CrystalDecisions.CrystalReports.Engine.ReportDocument relatorio = RelatorioSemPadraoReportService.GerarCrystalReport(@"Areas\Relatorios\Reports\Default\Frota\MovimentacaoPneuVeiculo.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet);

            relatorio.PrintOptions.PaperOrientation = relatorioTemporario.OrientacaoRelatorio == Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato ? CrystalDecisions.Shared.PaperOrientation.Portrait : CrystalDecisions.Shared.PaperOrientation.Landscape;
            relatorio.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            IdentificacaoCamposRPT identificacaoCampos = new IdentificacaoCamposRPT() { ContadorRegistrosGrupo = "", ContadorRegistrosTotal = "" };

            _servicoRelatorioReportService.SetarSubCabecalhoRelatorio(relatorio.Subreports, relatorio, relatorioControleGeracao.TipoArquivoRelatorio, relatorioTemporario.Titulo, relatorioTemporario.FontePadrao, identificacaoCampos, _unitOfWork);
            _servicoRelatorioReportService.SetarSubRodapeRelatorio(relatorio.Subreports, relatorio, relatorioTemporario.CodigoControleRelatorios, relatorioTemporario.FontePadrao, relatorioControleGeracao.TipoArquivoRelatorio, identificacaoCampos);
            _servicoRelatorioReportService.FormatarRelatorio(relatorio.Subreports[0], relatorio, relatorioTemporario, relatorioControleGeracao.TipoArquivoRelatorio, identificacaoCampos);
            _servicoRelatorioReportService.GerarRelatorio(relatorio, relatorioControleGeracao, "Relatorios/Frota/MovimentacaoPneuVeiculo", _unitOfWork);
            return PrepareReportResult(FileType.PDF);
        }
        catch (Exception ex)
        {
            Servicos.Log.TratarErro(ex);
            throw new ServerException($"{ex.Message} - {ex.StackTrace}");
        }

    }
    private Dominio.Entidades.Veiculo ObterVeiculo(int codigoVeiculo, Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Veiculo repositorio = new Repositorio.Veiculo(unitOfWork);
        Dominio.Entidades.Veiculo veiculo = repositorio.BuscarPorCodigo(codigoVeiculo);

        if (veiculo == null)
            throw new ControllerException("Não foi possível encontrar o registro.");

        return veiculo;
    }
    private byte[] CriarImagemVeiculo(Dominio.Entidades.Veiculo veiculo, string caminhimagemPneuDesabilitado, string caminhoimagemEixoDuplo, string caminhoimagemEixoSimples, string caminhoimagemPneu)
    {
        int totalEixos = veiculo.ModeloVeicularCarga?.Eixos?.Count ?? 0;
        int totalEstepes = veiculo.ModeloVeicularCarga?.Estepes?.Count ?? 0;

        if ((totalEixos > 0) || (totalEstepes > 0))
        {
            int alturaImagem = 140;
            int espacoEntreImagens = 20;
            int espacoEntreImagensPneu = 2;
            int larguraImagem = 350;
            int larguraImagemPneu = 42;
            int alturaImagemEixos = totalEixos > 0 ? (totalEixos * (alturaImagem + espacoEntreImagens)) : 0;
            int alturaImagemEstepes = totalEstepes > 0 ? ((int)(totalEstepes / 8) + 1) * (alturaImagem + espacoEntreImagens) : 0;
            int espacoEntreImagensEixosEEstepes = (totalEixos > 0) && (totalEstepes > 0) ? 30 : 0;
            int altura = (alturaImagemEixos + alturaImagemEstepes + espacoEntreImagensEixosEEstepes);

            Bitmap imagemPneu = new Bitmap(caminhoimagemPneu);
            Bitmap imagem = new Bitmap(larguraImagem, altura);
            Graphics contextoGrafico = Graphics.FromImage(imagem);
            Font fonteNumeroFogoPneu = new Font("'Open Sans', Arial, Helvetica, Sans-Serif", 12, FontStyle.Regular, GraphicsUnit.Pixel);
            StringFormat formatacaoTexto = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            try
            {
                contextoGrafico.Clear(Color.White);

                if (totalEixos > 0)
                {
                    Bitmap imagemEixoSimples = new Bitmap(caminhoimagemEixoSimples);
                    Bitmap imagemEixoDuplo = new Bitmap(caminhoimagemEixoDuplo);
                    Font fonteNumeroEixo = new Font("'Open Sans', Arial, Helvetica, Sans-Serif", 20, FontStyle.Bold, GraphicsUnit.Pixel);
                    int indiceEixo = 0;

                    try
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo eixo in veiculo.ModeloVeicularCarga.Eixos.OrderBy(o => o.Numero))
                        {
                            int posicaoY = (indiceEixo++ * (alturaImagem + espacoEntreImagens));

                            if (eixo.QuantidadePneu == QuantidadePneuEixo.Simples)
                                contextoGrafico.DrawImage(imagemEixoSimples, 0, posicaoY, larguraImagem, alturaImagem);
                            else
                                contextoGrafico.DrawImage(imagemEixoDuplo, 0, posicaoY, larguraImagem, alturaImagem);

                            contextoGrafico.DrawString(eixo.Numero.ToString(), fonteNumeroEixo, Brushes.White, new RectangleF(150, posicaoY + 60, 50, 20), formatacaoTexto);

                            foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixoPneu eixoPneu in eixo.Pneus)
                            {
                                Dominio.Entidades.VeiculoPneu veiculoPneu = (from pneu in veiculo.Pneus where pneu.EixoPneu.Codigo == eixoPneu.Codigo select pneu).FirstOrDefault();

                                if (veiculoPneu != null)
                                {
                                    int posicaoX = (eixoPneu.ObterPosicaoPneu() - 1) * (larguraImagemPneu + espacoEntreImagensPneu);

                                    contextoGrafico.DrawImage(imagemPneu, posicaoX, posicaoY, larguraImagemPneu, alturaImagem);
                                    contextoGrafico.DrawString(veiculoPneu.Pneu.NumeroFogo.Substring(0, Math.Min(veiculoPneu.Pneu.NumeroFogo.Length, 5)), fonteNumeroFogoPneu, Brushes.Black, new RectangleF(posicaoX, posicaoY + 140, 42, 14), formatacaoTexto);
                                }
                            }
                        }
                    }
                    finally
                    {
                        imagemEixoSimples.Dispose();
                        imagemEixoDuplo.Dispose();
                    }
                }

                if (totalEstepes > 0)
                {
                    Bitmap imagemPneuDesabilitado = new Bitmap(caminhimagemPneuDesabilitado);
                    Font fonteNumeroEstepe = new Font("'Open Sans', Arial, Helvetica, Sans-Serif", 16, FontStyle.Bold, GraphicsUnit.Pixel);
                    int indiceEstepe = 0;
                    int posicaoYInicioImagensEstepe = alturaImagemEixos + espacoEntreImagensEixosEEstepes;

                    try
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEstepe estepe in veiculo.ModeloVeicularCarga.Estepes.OrderBy(o => o.Numero))
                        {
                            int posicaoY = ((int)(indiceEstepe++ / 8) * (alturaImagem + espacoEntreImagens)) + posicaoYInicioImagensEstepe;
                            int posicaoX = ((estepe.Numero - 1) % 8) * (larguraImagemPneu + espacoEntreImagensPneu);
                            Dominio.Entidades.VeiculoEstepe veiculoEstepe = (from pneuEstepe in veiculo.Estepes where pneuEstepe.Estepe.Codigo == estepe.Codigo select pneuEstepe).FirstOrDefault();

                            if (veiculoEstepe == null)
                                contextoGrafico.DrawImage(imagemPneuDesabilitado, posicaoX, posicaoY, larguraImagemPneu, alturaImagem);
                            else
                            {
                                contextoGrafico.DrawImage(imagemPneu, posicaoX, posicaoY, larguraImagemPneu, alturaImagem);
                                contextoGrafico.DrawString(veiculoEstepe.Pneu.NumeroFogo.Substring(0, Math.Min(veiculoEstepe.Pneu.NumeroFogo.Length, 5)), fonteNumeroFogoPneu, Brushes.Black, new RectangleF(posicaoX, posicaoY + 140, 42, 14), formatacaoTexto);
                            }

                            contextoGrafico.DrawString(estepe.Numero.ToString(), fonteNumeroEstepe, Brushes.White, new RectangleF(posicaoX, posicaoY + 55, 42, 30), formatacaoTexto);
                        }
                    }
                    finally
                    {
                        imagemPneuDesabilitado.Dispose();
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    imagem.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    return stream.ToArray();
                }
            }
            finally
            {
                imagemPneu.Dispose();
                imagem.Dispose();
            }
        }

        return null;
    }

}