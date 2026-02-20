using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.PdfPlanoViagem)]
public class PdfPlanoViagemReport : ReportBase
{   
    public PdfPlanoViagemReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

        var carga = repCarga.BuscarPorCodigo(extraData.GetValue<int>("CodigoCarga"));
        
        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido =
            new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido =
            repositorioCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);
        Dominio.Entidades.Cliente clienteFilial = (carga.Filial != null)
            ? repositorioCliente.BuscarPorCPFCNPJ(carga.Filial.CNPJ.ToDouble())
            : null;
        Dominio.Entidades.Usuario motorista = carga.Motoristas?.FirstOrDefault();
        Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao =
            new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);
        Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotoristaTelerisco =
            (motorista != null)
                ? repositorioMotoristaIntegracao.BuscarPorMotoristaETipo(motorista.Codigo, TipoIntegracao.Telerisco)
                : null;

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagem> dataSourcePlanoViagem =
            new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagem>()
            {
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagem
                {
                    Filial = carga.Filial?.Descricao ?? "",
                    MotoristaCelular = motorista?.Celular ?? "",
                    MotoristaDataConsultaIntegracao =
                        integracaoMotoristaTelerisco?.DataIntegracao.ToString("dd/MM/yyyy HH:mm") ?? "",
                    MotoristaNome = motorista?.Nome ?? "",
                    MotoristaProtocoloConsultaIntegracao = integracaoMotoristaTelerisco?.Protocolo ?? "",
                    MotoristaMensagemConsultaIntegracao = integracaoMotoristaTelerisco?.Mensagem ?? "",
                    MotoristaTipoConsultaIntegracao = integracaoMotoristaTelerisco?.DescricaoTipo ?? "",
                    NumeroCarga = carga.CodigoCargaEmbarcador,
                    NumeroProtocoloCarga = (carga.Protocolo > 0) ? carga.Protocolo.ToString() : "",
                    Observacoes = cargaPedido?.Pedido?.ObservacaoCTe ?? "",
                    PrevisaoInicioViagem = cargaPedido?.Pedido?.DataPrevisaoSaida?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    Rota = carga.Rota?.Descricao ?? "",
                    TipoOperacao = carga.TipoOperacao?.Descricao ?? "",
                    Transportador = carga.Empresa?.Descricao ?? ""
                }
            };
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDestinatario>
            dataSourcePlanoViagemDestinatario =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDestinatario>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento>
            dataSourcePlanoViagemDocumento =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento>
            dataSourcePlanoViagemDocumentoNotas =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemEntrega>
            dataSourcePlanoViagemEntrega =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemEntrega>();
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemVeiculo>
            dataSourcePlanoViagemVeiculo =
                new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemVeiculo>();
        List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();

        if (carga.Veiculo != null)
            veiculos.Add(carga.Veiculo);

        if (carga.VeiculosVinculados?.Count > 0)
            veiculos.AddRange(carga.VeiculosVinculados.ToList());

        for (int i = 0; i < veiculos.Count; i++)
        {
            Dominio.Entidades.Veiculo veiculo = veiculos[i];
            Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemVeiculo planoViagemVeiculo =
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemVeiculo()
                {
                    Ano = (veiculo.AnoFabricacao > 0) ? veiculo.AnoFabricacao.ToString() : "",
                    Categoria = veiculo.ModeloVeicularCarga?.Descricao ?? "",
                    Cor = veiculo?.CorVeiculo?.Descricao,
                    Marca = veiculo.Marca?.Descricao ?? "",
                    Modelo = veiculo.Modelo?.Descricao ?? "",
                    NumeroEixos = veiculo.ModeloVeicularCarga?.NumeroEixos.ToString() ?? "",
                    Ordem = i + 1,
                    Placa = veiculo.Placa,
                    TipoVeiculo = veiculo.DescricaoTipoRodado
                };

            if (veiculo.PossuiRastreador)
            {
                planoViagemVeiculo.DispositivoRastreamentoCodigo = veiculo.NumeroEquipamentoRastreador;
                planoViagemVeiculo.DispositivoRastreamentoProvedor = veiculo.TecnologiaRastreador?.Descricao ?? "";
                planoViagemVeiculo.DispositivoRastreamentoTipo = veiculo.TipoComunicacaoRastreador?.Descricao ?? "";
            }

            dataSourcePlanoViagemVeiculo.Add(planoViagemVeiculo);
        }

        Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe =
            new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe =
            new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega =
            new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre =
            new Repositorio.Embarcador.Cargas.CargaLacre(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioRotaFretePontosPassagem =
            new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes =
            repositorioCargaCTe.BuscarPorCarga(carga.Codigo, false, false, true);
        List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes =
            repositorioCargaMDFe.BuscarPorAutorizadosCarga(carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasEColetas =
            repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> cargaLacres =
            repositorioCargaLacre.BuscarPorCarga(carga.Codigo);
        Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFreteRetorno =
            repositorioRotaFretePontosPassagem.BuscarPontoRetornoPorCarga(carga.Codigo);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas =
            (from o in entregasEColetas where !o.Coleta orderby o.Ordem select o).ToList();
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraColeta =
            (from o in entregasEColetas where o.Coleta orderby o.Ordem select o).FirstOrDefault();
        int distanciaAcumulada = 0;
        int ordem = 1;

        dataSourcePlanoViagemEntrega.Add(
            new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemEntrega()
            {
                Cidade = carga.Filial?.Localidade?.Descricao ?? "",
                DataEntrega = carga.DataInicioViagem?.ToString("dd/MM/yy HH:mm"),
                DataEntregaPrevista = carga.DataInicioViagemPrevista?.ToString("dd/MM/yy HH:mm"),
                DistanciaAcumulada = 0.ToString("n0"),
                DistanciaParcial = 0.ToString("n0"),
                Endereco = clienteFilial?.Endereco,
                Ordem = 0,
                PontoParada = carga.Filial?.Descricao ?? "",
                Tipo = "IV",
                UF = carga.Filial?.Localidade?.Estado?.Sigla ?? ""
            });

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFes)
        {
            dataSourcePlanoViagemDocumento.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                {
                    Destino = "",
                    Numero = cargaMDFe.MDFe.Numero.ToString(),
                    Ordem = 0,
                    Tipo = "MDF-e"
                });
        }

        if (primeiraColeta != null)
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido =
                repositorioCargaEntrega.BuscarPrimeiroPedidoDaEntrega(primeiraColeta.Codigo);

            if (pedido?.DataPrevisaoSaida != null)
            {
                dataSourcePlanoViagem[0].PrevisaoInicioViagem =
                    pedido.DataPrevisaoSaida.Value.ToString("dd/MM/yyyy HH:mm") ?? "";
                dataSourcePlanoViagemEntrega[0].DataEntregaPrevista =
                    pedido.DataPrevisaoSaida.Value.AddHours(-1).ToString("dd/MM/yy HH:mm") ?? "";
            }
        }

        DateTime? dataUltimaEntrega = null;

        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in entregas)
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido =
                repositorioCargaEntrega.BuscarPrimeiroPedidoDaEntrega(entrega.Codigo);

            dataSourcePlanoViagemDestinatario.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDestinatario()
                {
                    Consignatario = entrega.Cliente?.Descricao ?? "",
                    DataPrevisaoChegada = pedido?.PrevisaoEntrega?.ToString("dd/MM/yy HH:mm"),
                    Destinatario = entrega.Cliente?.Descricao ?? "",
                    Ordem = ordem
                });

            distanciaAcumulada += entrega.Distancia;

            dataSourcePlanoViagemEntrega.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemEntrega()
                {
                    Cidade = entrega.Cliente?.Localidade?.Descricao ?? "",
                    DataEntrega = entrega.DataInicio?.ToString("dd/MM/yy HH:mm"),
                    DataEntregaPrevista = pedido?.PrevisaoEntrega?.ToString("dd/MM/yy HH:mm"),
                    DistanciaAcumulada = distanciaAcumulada.ToString("n0"),
                    DistanciaParcial = entrega.Distancia.ToString("n0"),
                    Endereco = entrega.Cliente?.Endereco ?? "",
                    Ordem = ordem,
                    PontoParada = entrega.Cliente?.Descricao ?? "",
                    Tipo = "CH",
                    UF = entrega.Cliente?.Localidade?.Estado?.Sigla ?? ""
                });

            dataSourcePlanoViagemEntrega.Add(
                new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemEntrega()
                {
                    Cidade = entrega.Cliente?.Localidade?.Descricao ?? "",
                    DataEntrega = entrega.DataFim?.ToString("dd/MM/yy HH:mm"),
                    DataEntregaPrevista = pedido?.PrevisaoEntrega?.AddHours(2).ToString("dd/MM/yy HH:mm"),
                    DistanciaAcumulada = distanciaAcumulada.ToString("n0"),
                    DistanciaParcial = entrega.Distancia.ToString("n0"),
                    Endereco = entrega.Cliente?.Endereco ?? "",
                    Ordem = ordem,
                    PontoParada = entrega.Cliente?.Descricao ?? "",
                    Tipo = "FV",
                    UF = entrega.Cliente?.Localidade?.Estado?.Sigla ?? ""
                });

            dataUltimaEntrega = pedido?.PrevisaoEntrega?.AddHours(2);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = (from o in cargaCTes
                where o.CTe.Destinatario.Cliente.CPF_CNPJ == (entrega.Cliente?.CPF_CNPJ ?? 0d)
                select o.CTe).ToList();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                dataSourcePlanoViagemDocumento.Add(
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                    {
                        Destino = entrega.Cliente?.Descricao ?? "",
                        Numero = cte.Numero.ToString(),
                        Ordem = ordem,
                        Tipo = "CT-e"
                    });
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> lacresCliente = (from o in cargaLacres
                where o.TipoLacre != null && o.Cliente.CPF_CNPJ == (entrega.Cliente?.CPF_CNPJ ?? 0d)
                select o).ToList();
            var lacresPorTipo = lacresCliente.GroupBy(obj => obj.TipoLacre).Select(obj =>
                new { obj.Key.Descricao, Numero = string.Join("-", obj.Select(o => o.Numero)) }).ToList();

            foreach (var lacreTipo in lacresPorTipo)
            {
                dataSourcePlanoViagemDocumento.Add(
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                    {
                        Destino = entrega.Cliente?.Descricao ?? "",
                        Numero = lacreTipo.Numero,
                        Ordem = ordem,
                        Tipo = "Lacre " + lacreTipo.Descricao
                    });
            }

            if (entrega.NotasFiscais?.Count > 0)
            {
                int quantidadeNotas = 0,
                    volumesSemClassificacao = 0,
                    volumesRevenda = 0,
                    volumesNaoRevenda = 0,
                    volumesNFEletronicos = 0,
                    volumesRetira = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal notaFiscal in
                         entrega.NotasFiscais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal =
                        notaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal;

                    if (!xmlNotaFiscal.nfAtiva)
                        continue;

                    if (xmlNotaFiscal.Volumes > 0)
                    {
                        if (!xmlNotaFiscal.ClassificacaoNFe.HasValue ||
                            xmlNotaFiscal.ClassificacaoNFe == ClassificacaoNFe.SemClassificacao)
                            volumesSemClassificacao += xmlNotaFiscal.Volumes;
                        else if (xmlNotaFiscal.ClassificacaoNFe.Value == ClassificacaoNFe.Revenda)
                            volumesRevenda += xmlNotaFiscal.Volumes;
                        else if (xmlNotaFiscal.ClassificacaoNFe.Value == ClassificacaoNFe.NaoRevenda)
                            volumesNaoRevenda += xmlNotaFiscal.Volumes;
                        else if (xmlNotaFiscal.ClassificacaoNFe.Value == ClassificacaoNFe.NFEletronicos)
                            volumesNFEletronicos += xmlNotaFiscal.Volumes;
                        else if (xmlNotaFiscal.ClassificacaoNFe.Value == ClassificacaoNFe.Retira)
                            volumesRetira += xmlNotaFiscal.Volumes;
                    }

                    dataSourcePlanoViagemDocumentoNotas.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                        {
                            Destino = entrega.Cliente?.Descricao ?? "",
                            Numero = xmlNotaFiscal.Numero.ToString(),
                            Ordem = ordem,
                            ClassificacaoNFe = xmlNotaFiscal.ClassificacaoNFe?.ObterDescricao() ?? string.Empty
                        });

                    quantidadeNotas++;
                }

                if (volumesSemClassificacao > 0)
                {
                    dataSourcePlanoViagemDocumento.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                        {
                            Destino = entrega.Cliente?.Descricao ?? "",
                            Numero = volumesSemClassificacao.ToString(),
                            Ordem = ordem,
                            Tipo = "Quantidade Total de Peças"
                        });
                }

                if (volumesRevenda > 0)
                {
                    dataSourcePlanoViagemDocumento.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                        {
                            Destino = entrega.Cliente?.Descricao ?? "",
                            Numero = volumesRevenda.ToString(),
                            Ordem = ordem,
                            Tipo = "Quantidade de Peças Total Revenda"
                        });
                }

                if (volumesNaoRevenda > 0)
                {
                    dataSourcePlanoViagemDocumento.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                        {
                            Destino = entrega.Cliente?.Descricao ?? "",
                            Numero = volumesNaoRevenda.ToString(),
                            Ordem = ordem,
                            Tipo = "Quantidade de Peças Total Não Revenda"
                        });
                }

                if (volumesNFEletronicos > 0)
                {
                    dataSourcePlanoViagemDocumento.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                        {
                            Destino = entrega.Cliente?.Descricao ?? "",
                            Numero = volumesNFEletronicos.ToString(),
                            Ordem = ordem,
                            Tipo = "Quantidade de Peças Total NF Eletrônicos"
                        });
                }

                if (volumesRetira > 0)
                {
                    dataSourcePlanoViagemDocumento.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                        {
                            Destino = entrega.Cliente?.Descricao ?? "",
                            Numero = volumesRetira.ToString(),
                            Ordem = ordem,
                            Tipo = "Quantidade de Peças Total Retira"
                        });
                }

                if (quantidadeNotas > 0)
                {
                    dataSourcePlanoViagemDocumento.Add(
                        new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemDocumento()
                        {
                            Destino = entrega.Cliente?.Descricao ?? "",
                            Numero = quantidadeNotas.ToString(),
                            Ordem = ordem,
                            Tipo = "Quantidade Total de Notas"
                        });
                }

                ordem++;
            }
        }

        int distanciaParcial = 0;
        string previsaoRetorno = string.Empty;

        if (carga.DataRetornoCD.HasValue)
            previsaoRetorno = carga.DataRetornoCD.Value.ToString("dd/MM/yy HH:mm");
        else if (cargaRotaFreteRetorno != null)
        {
            distanciaParcial = cargaRotaFreteRetorno.Distancia;
            distanciaAcumulada += distanciaParcial;

            if (dataUltimaEntrega.HasValue)
                previsaoRetorno = dataUltimaEntrega.Value.AddMinutes(cargaRotaFreteRetorno.Tempo)
                    .ToString("dd/MM/yy HH:mm");
        }

        dataSourcePlanoViagemEntrega.Add(
            new Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem.PlanoViagemEntrega()
            {
                Cidade = carga.Filial?.Localidade?.Descricao ?? "",
                DataEntrega = carga.DataFimViagem?.ToString("dd/MM/yy HH:mm"),
                //DataEntregaPrevista = carga.DataFimViagemPrevista?.ToString("dd/MM/yy HH:mm"),
                DataEntregaPrevista = previsaoRetorno,
                DistanciaAcumulada = (distanciaAcumulada > 0 ? distanciaAcumulada.ToString("n0") : ""),
                DistanciaParcial = (distanciaParcial > 0 ? distanciaParcial.ToString("n0") : ""),
                Endereco = clienteFilial?.Endereco,
                Ordem = (dataSourcePlanoViagemDestinatario.LastOrDefault()?.Ordem ?? 0) + 1,
                PontoParada = carga.Filial?.Descricao ?? "",
                Tipo = "FV",
                UF = carga.Filial?.Localidade?.Estado?.Sigla ?? ""
            });

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dataSourcePlanoViagem,
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>(),
                SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                {
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "PlanoViagemDestinatario.rpt",
                        DataSet = dataSourcePlanoViagemDestinatario
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "PlanoViagemDocumento.rpt",
                        DataSet = dataSourcePlanoViagemDocumento
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "PlanoViagemEntrega.rpt",
                        DataSet = dataSourcePlanoViagemEntrega
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "PlanoViagemVeiculo.rpt",
                        DataSet = dataSourcePlanoViagemVeiculo
                    },
                    new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                    {
                        Key = "PlanoViagemDocumentoNotas.rpt",
                        DataSet = dataSourcePlanoViagemDocumentoNotas
                    },
                }
            };

        byte[] pdf =RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\PlanoViagem.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF,
            dataSet, possuiLogo: true);

        if (pdf == null)
            throw new ServicoException("Não foi possível gerar o plano de viagem.");

        return PrepareReportResult(FileType.PDF, pdf);
    }
}