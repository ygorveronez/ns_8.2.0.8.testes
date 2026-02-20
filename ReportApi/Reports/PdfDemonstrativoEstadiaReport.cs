using System;
using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.Relatorios;


namespace ReportApi.Reports;

[UseReportType(ReportType.PdfDemonstrativoEstadia)]
public class PdfDemonstrativoEstadiaReport : ReportBase
{
    public PdfDemonstrativoEstadiaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService,
        IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        var codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal =
            new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
        Repositorio.Embarcador.Pedidos.PedidoTratativa repositorioPedidoTratativa =
            new Repositorio.Embarcador.Pedidos.PedidoTratativa(_unitOfWork);
        Repositorio.RotaFreteFronteira repositorioRotaFreteFronteira = new Repositorio.RotaFreteFronteira(_unitOfWork);
        Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia =
            new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido =
            repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

        Dominio.Entidades.Embarcador.Cargas.CargaPedido
            cargaPedido = listaCargaPedido.FirstOrDefault(); //Apenas o primeiro, já que é pra TMS
        if (cargaPedido == null)
            throw new ServicoException("Não foi possível encontrar o pedido.");

        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
        Dominio.Entidades.Empresa empresa = carga.Empresa;

        List<Dominio.Entidades.RotaFreteFronteira> fronteiras = carga.Rota != null
            ? repositorioRotaFreteFronteira.BuscarPorRotaFrete(carga.Rota.Codigo)
            : new List<Dominio.Entidades.RotaFreteFronteira>();
        List<Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa> tratativas =
            repositorioPedidoTratativa.BuscarPorPedido(pedido.Codigo);
        List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias =
            repositorioCargaOcorrencia.BuscarOcorrenciasDeEstadiasPorCarga(carga.Codigo);

        string fronteirasCarga = string.Join(", ", fronteiras.Select(o => o.Cliente.Nome).ToList());
        string ctesCarga = string.Join(", ", carga.CargaCTes.Select(o => o.CTe.Numero).ToList());
        string notasCarga = string.Join(", ", repositorioPedidoXMLNotaFiscal.ObterNumerosNotasPorCarga(carga.Codigo));

        Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.DemonstrativoEstadia
            dataSourceDemonstrativoEstadia =
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.DemonstrativoEstadia()
                {
                    NumeroCarga = carga.CodigoCargaEmbarcador,
                    NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                    Empresa = empresa != null ? empresa.RazaoSocial : string.Empty,
                    Filial = empresa != null
                        ? empresa.RazaoSocial + " - " + empresa.Localidade.Estado.Sigla
                        : string.Empty,
                    DataCarga = carga.DataCarregamentoCarga?.ToDateTimeString() ?? string.Empty,
                    CentroResultado = pedido.CentroResultado?.Descricao ?? string.Empty,
                    TipoCarga = carga.TipoDeCarga?.Descricao ?? string.Empty,
                    Terceiro = carga.Terceiro?.Descricao ?? string.Empty,
                    Remetente = pedido.GrupoPessoas?.Descricao ?? pedido.Remetente?.Nome ?? string.Empty,
                    Destinatario = pedido.Destinatario?.Nome ?? string.Empty,
                    Pagador = cargaPedido.ObterTomador()?.Descricao ?? string.Empty,
                    Origem = pedido.Origem?.DescricaoCidadeEstadoPais,
                    Destino = pedido.Destino?.DescricaoCidadeEstadoPais,
                    Motorista = carga.NomeMotoristas,
                    Placa = carga.PlacasVeiculos,
                    StatusViagem = carga.DataFimViagem != null ? "Finalizada" :
                        carga.DataFimViagem == null ? "Não Finalizada" :
                        carga.DataInicioViagem != null ? "Iniciada" :
                        carga.DataInicioViagem == null ? "Não Iniciada" :
                        carga.DataInicioViagem != null && carga.DataFimViagem == null ? "Em Andamento" : string.Empty,
                    Fronteira = fronteirasCarga,
                    CtesCarga = ctesCarga,
                    NotasCarga = notasCarga
                };

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.DemonstrativoEstadiaOcorrencia>
            listaDataSourceOcorrencias =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.
                    DemonstrativoEstadiaOcorrencia>();
        foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia obj in ocorrencias)
        {
            decimal valorPorHora = Math.Round((obj.ValorOcorrencia / (decimal)(obj.HorasEstadia ?? 0d)), 2);
            decimal valorTotal = Math.Round(obj.ValorOcorrencia, 2);
            double horasEstadia = obj.HorasEstadia ?? 0d;

            if (Math.Round((valorPorHora * Math.Round((decimal)horasEstadia, 2)), 2) != valorTotal)
            {
                horasEstadia = Math.Floor(horasEstadia);
                valorPorHora = valorTotal / (decimal)horasEstadia;
            }

            Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.DemonstrativoEstadiaOcorrencia
                dataSourceOcorrencia =
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.
                        DemonstrativoEstadiaOcorrencia()
                        {
                            TipoCargaEntrega = obj.TipoCargaEntrega?.ObterDescricao() ?? string.Empty,
                            DataInicial = obj.DataInicialEstadia?.ToDateTimeString() ?? string.Empty,
                            DataFinal = obj.DataFinalEstadia?.ToDateTimeString() ?? string.Empty,
                            HorasEstadia = TimeSpan.FromHours(horasEstadia).ToTimeString(),
                            HorasFreetime = TimeSpan.FromHours(obj.HorasFreetime ?? 0d).ToTimeString(),
                            ValorTotal = valorTotal.ToString("n2"),
                            ValorPorHora = (valorPorHora).ToString("n2")
                        };
            listaDataSourceOcorrencias.Add(dataSourceOcorrencia);
        }

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.DemonstrativoEstadiaTratativa>
            listaDataSourceTratativas =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.
                    DemonstrativoEstadiaTratativa>();
        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa obj in tratativas)
        {
            Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.DemonstrativoEstadiaTratativa
                dataSourceTratativas =
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.
                        DemonstrativoEstadiaTratativa()
                        {
                            Descricao = obj.RegistroFormatado
                        };
            listaDataSourceTratativas.Add(dataSourceTratativas);
        }

        IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro> listaDemonstrativo =
            repCargaEntrega.BuscarResumoRoteiro(carga.Codigo, "", "", 0, 0);
        if (listaDemonstrativo == null || listaDemonstrativo.Count == 0)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro resumoRoteiro =
                new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro()
                {
                    Etapa = "",
                    Exedente = 0m,
                    Fim = DateTime.Now,
                    Freetime = 0m,
                    Inicio = DateTime.Now,
                    Total = 0,
                    ValorHora = 0m,
                    ValorTotal = 0m
                };
            listaDemonstrativo.Add(resumoRoteiro);
        }

        for (int i = 0; i < listaDemonstrativo.Count; i++)
        {
            listaDemonstrativo[i].ValorTotal = 0m;
            listaDemonstrativo[i].ValorHora = 0m;

            if (listaDemonstrativo[i].Exedente > 0)
            {
                LocalFreeTime localFreeTime = LocalFreeTime.Coleta;
                if (listaDemonstrativo[i].Etapa == "Coleta")
                    localFreeTime = LocalFreeTime.Coleta;
                else if (listaDemonstrativo[i].Etapa == "Entrega")
                    localFreeTime = LocalFreeTime.Entrega;
                else
                    localFreeTime = LocalFreeTime.Fronteira;
                listaDemonstrativo[i].ValorTotal = CalcularValorOcorrencia(localFreeTime,
                    (double)listaDemonstrativo[i].Exedente, listaDemonstrativo[i].Inicio, listaDemonstrativo[i].Fim,
                    carga, _unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
                if (listaDemonstrativo[i].ValorTotal > 0 && listaDemonstrativo[i].Exedente > 0)
                    listaDemonstrativo[i].ValorHora =
                        Math.Round(listaDemonstrativo[i].ValorTotal / (listaDemonstrativo[i].Exedente / 60), 2,
                            MidpointRounding.ToEven);
            }
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds1 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "Ocorrencias",
                DataSet = listaDataSourceOcorrencias
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds2 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "Tratativas",
                DataSet = listaDataSourceTratativas
            };
        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet ds3 =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "Demonstrativo",
                DataSet = listaDemonstrativo
            };

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet> subReports =
            new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>();
        subReports.Add(ds1);
        subReports.Add(ds2);
        subReports.Add(ds3);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet =
                    new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DemonstrativoEstadia.
                        DemonstrativoEstadia>() { dataSourceDemonstrativoEstadia },
                SubReports = subReports
            };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\DemonstrativoEstadia.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }

    private static decimal CalcularValorOcorrencia(LocalFreeTime localFreeTime, double totalHoraEstadia,
        DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.Embarcador.Cargas.Carga carga,
        Repositorio.UnitOfWork unitOfWork,
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
    {
        try
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoOcorrencia =
                new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();
            Servicos.Embarcador.Carga.Ocorrencia servicoOcorrenciaCalculoFrete =
                new Servicos.Embarcador.Carga.Ocorrencia();
            Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo servicoGatilhoOcorrencia =
                new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(unitOfWork);

            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia
                repGatilhoGeracaoAutomaticaOcorrencia =
                    new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia =
                new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal =
                new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS =
                new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS =
                repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia
                parametrosCalcularValorOcorrencia =
                    new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia()
                    {
                        ApenasReboque = false,
                        CodigoCarga = carga.Codigo,
                        CodigoParametroBooleano = 0,
                        CodigoParametroInteiro = 0,
                        CodigoParametroPeriodo =
                            repParametroOcorrencia.BuscarPorTipo(TipoParametroOcorrencia.Periodo)?.Codigo ?? 0,
                        CodigoTipoOcorrencia = carga.TipoOperacao.TipoOcorrencia?.Codigo ?? 0,
                        DataFim = dataInicial.AddHours(totalHoraEstadia / 60),
                        DataInicio = dataInicial,
                        ParametroData = dataInicial,
                        Minutos = 0,
                        HorasSemFranquia = carga.TipoOperacao.TipoOcorrencia?.HorasSemFranquia ?? 0,
                        KmInformado = 0,
                        PermiteInformarValor = false,
                        ValorOcorrencia = 0m,
                        LocalFreeTime = localFreeTime
                    };

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia =
                repTipoDeOcorrencia.BuscarPorCodigo(parametrosCalcularValorOcorrencia.CodigoTipoOcorrencia);

            Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho =
                repGatilhoGeracaoAutomaticaOcorrencia.BuscarPorTipoOcorrencia(parametrosCalcularValorOcorrencia
                    .CodigoTipoOcorrencia);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = null;

            if (cargaCTEs == null)
                cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo, true);

            if (gatilho != null && carga != null)
            {
                parametrosCalcularValorOcorrencia.DeducaoHoras =
                    servicoGatilhoOcorrencia.ObterHorasDeducaoPorGatilho(carga, gatilho);

                if (!gatilho.GerarAutomaticamente)
                {
                    (DateTime? DataInicio, DateTime? DataFim) dados =
                        servicoGatilhoOcorrencia.ObterDataInicioEFimGatilho(
                            parametrosCalcularValorOcorrencia.ParametroData,
                            parametrosCalcularValorOcorrencia.DataInicio, parametrosCalcularValorOcorrencia.DataFim);

                    if (dados.DataInicio.HasValue && dados.DataFim.HasValue)
                    {
                        parametrosCalcularValorOcorrencia.DataInicio = dados.DataInicio.Value;
                        parametrosCalcularValorOcorrencia.DataFim = dados.DataFim.Value;
                    }
                }
            }

            parametrosCalcularValorOcorrencia.ListaCargaCTe = cargaCTEs;
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia =
                servicoOcorrenciaCalculoFrete.CalcularValorOcorrencia(parametrosCalcularValorOcorrencia, unitOfWork,
                    configuracaoTMS, tipoServicoMultisoftware);

            return Math.Round(calculoFreteOcorrencia.ValorOcorrencia, 2, MidpointRounding.ToEven);
        }
        catch (ServicoException excecao)
        {
            //Log.TratarErro(excecao.Message, "OcorrenciaEstadia");  //TODO - gravar log
            return 0.01m;
        }
        catch (Exception excecao)
        {
            //Servicos.Log.TratarErro(excecao, "OcorrenciaEstadia"); //TODO - gravar log
            return 0.01m;
        }
    }
}