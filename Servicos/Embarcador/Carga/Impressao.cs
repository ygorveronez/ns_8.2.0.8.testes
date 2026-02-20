using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Carga
{
    public sealed class Impressao
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Impressao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public Impressao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public string EnviarDocumentosParaImpressao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            try
            {
                int numeroImpressora = ObterNumeroImpressora(carga);
                Repositorio.IntegracaoCTe repositorioIntegracaoCTe = new Repositorio.IntegracaoCTe(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                List<int> ctes = repositorioCargaCTe.BuscarCodigosCTeAutorizadoPorCarga(carga.Codigo, false, false);

                for (int i = 0; i < ctes.Count; i++)
                {
                    Dominio.Entidades.IntegracaoCTe integracaoCTe = new Dominio.Entidades.IntegracaoCTe
                    {
                        Arquivo = "",
                        CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = ctes[i] },
                        NumeroDaUnidade = numeroImpressora,
                        Status = Dominio.Enumeradores.StatusIntegracao.Pendente,
                        Tipo = Dominio.Enumeradores.TipoIntegracao.Emissao,
                        TipoArquivo = Dominio.Enumeradores.TipoArquivoIntegracao.CTe
                    };

                    repositorioIntegracaoCTe.Inserir(integracaoCTe);
                }

                return "";
            }
            catch (ServicoException excecao)
            {
                return excecao.Message;
            }
        }

        public void EnviarPlanoViagemParaDestinatariosPorEmail(Dominio.Entidades.Embarcador.Cargas.Carga carga, string assunto)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfiguracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfiguracaoEmail.BuscarEmailEnviaDocumentoAtivo();

            if (configuracaoEmail == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo, TipoPedido.Entrega);

            List<string> emails = (
                from o in cargaPedidos where !o.PedidoPallet && o.Pedido.Destinatario != null && !string.IsNullOrWhiteSpace(o.Pedido.Destinatario.Email) select o.Pedido.Destinatario.Email).ToList().Concat((
                from o in cargaPedidos where !o.PedidoPallet && o.Recebedor != null && !string.IsNullOrWhiteSpace(o.Recebedor.Email) select o.Recebedor.Email).ToList()
            ).Distinct().ToList();

            if (carga.Empresa != null && !string.IsNullOrWhiteSpace(carga.Empresa.Email) && carga.TipoOperacao.ConfiguracaoImpressao.EnviarPlanoViagemTransportador)
                emails.Add(carga.Empresa.Email);

            if (emails.Count == 0)
                return;

            List<string> emailsSeparados = new List<string>();

            foreach (string email in emails)
                emailsSeparados.AddRange(email.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList());

            try
            {
                List<string> codigoIntegracaoDestinatarios = repositorioCargaEntrega.BuscarCodigoIntegracaoDestinatariosCargaEntrega(carga.Codigo);
                codigoIntegracaoDestinatarios = codigoIntegracaoDestinatarios.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();

                if (codigoIntegracaoDestinatarios.Count > 0)
                    assunto += ": " + string.Join("/", codigoIntegracaoDestinatarios);

                byte[] pdfPlanoViagem = ObterPdfPlanoViagem(carga);
                List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(new MemoryStream(pdfPlanoViagem), $"Plano de Viagem {carga.CodigoCargaEmbarcador}.pdf", "application/pdf") };
                System.Text.StringBuilder mensagem = new System.Text.StringBuilder();

                mensagem.Append("Olá, ").AppendLine().AppendLine();
                mensagem.Append($"Segue em anexo o plano de viagem referente à carga número {carga.CodigoCargaEmbarcador}.").AppendLine();
                mensagem.Append($"Quantidade de Volumes Previsto: {carga.DadosSumarizados?.VolumesTotal ?? 0}.").AppendLine();

                if (cargaPedidos.Count > 0)
                {
                    foreach (var cargaPedido in cargaPedidos)
                    {
                        mensagem.AppendLine($"{cargaPedido.Pedido.Destinatario.CodigoIntegracao}: {cargaPedido.Pedido.QtVolumes}.");
                    }
                }

                mensagem.AppendLine("E-mail enviado automaticamente. Por favor, não responda.");

                Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, null, emailsSeparados.Distinct().ToArray(), null, assunto, mensagem.ToString(), configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, anexos, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro($"Erro ao enviar e-mail do plano de viagem: {excecao.ToString()}");
            }
        }

        public string EnviarMDFeParaImpressao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            try
            {
                int numeroImpressora = ObterNumeroImpressora(carga);
                Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
                Repositorio.IntegracaoMDFe repositorioIntegracaoMDFe = new Repositorio.IntegracaoMDFe(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repositorioCargaMDFe.BuscarPorCarga(carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFes)
                {
                    if (cargaMDFe.MDFe != null)
                    {
                        Dominio.Entidades.IntegracaoMDFe integracaoMDFe = new Dominio.Entidades.IntegracaoMDFe()
                        {
                            Arquivo = "",
                            MDFe = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais { Codigo = cargaMDFe.MDFe.Codigo },
                            NumeroDaUnidade = numeroImpressora.ToString(),
                            NumeroDaCarga = carga.CodigoCargaEmbarcador,
                            Status = Dominio.Enumeradores.StatusIntegracao.Pendente,
                            Tipo = Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao,
                            TipoArquivo = Dominio.Enumeradores.TipoArquivoIntegracao.CTe,
                            GerouCargaEmbarcador = true
                        };

                        repositorioIntegracaoMDFe.Inserir(integracaoMDFe);
                    }
                }

                return "";
            }
            catch (ServicoException excecao)
            {
                return excecao.Message;
            }
        }

        public byte[] ObterPdfRelacaoEntrega(int codigoCarga, string nomeCliente)
        {
            return ReportRequest.WithType(ReportType.PdfRelacaoEntrega)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", codigoCarga.ToString())
                .AddExtraData("NomeCliente", nomeCliente)
                .CallReport()
                .GetContentFile();
        }

        public byte[] ObterPdfRelacaoEntregaPorPedido(int codigoCarga, string nomeCliente)
        {
            return ReportRequest.WithType(ReportType.PdfRelacaoEntregaPorPedido)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", codigoCarga.ToString())
                .AddExtraData("NomeCliente", nomeCliente)
                .CallReport()
                .GetContentFile();
        }

        public byte[] ObterPdfPlanoViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return ReportRequest.WithType(ReportType.PdfPlanoViagem)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", carga.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public byte[] ObterPdfPedidoPacoteCarga(int codigoCarga)
        {
            return ReportRequest.WithType(ReportType.RelacaoPedidoPacoteCarga)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoCarga", codigoCarga.ToString())
                    .CallReport()
                    .GetContentFile();
        }

        public byte[] ObterTodosPdfDiarioBordoCompactado(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            if (carga == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            return ObterTodosPdfTipoImpressaoDiarioBordoCompactado(carga, carga.CargaAgrupamento != null);
        }

        public byte[] ObterCRTsCompactado(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(carga.Codigo, carga.Empresa?.Codigo ?? 0);

            return ObterCRTsCompactado(carga, cargaJanelaCarregamentoTransportador);
        }

        public byte[] ObterPDFCRTComplementar(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia)
        {
            return ObterCRTComplementar(cte, cargaOcorrencia);
        }

        public byte[] ObterPDFCRT(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return ObterCRT(cte, carga);
        }

        public byte[] ObterTodosPdfTipoImpressaoDiarioBordoCompactado(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool buscarPorCargaOrigem = false)
        {
            if (carga.TipoOperacao?.TipoImpressaoDiarioBordo == TipoImpressaoDiarioBordo.Nenhum)
                throw new ServicoException("Tipo Operação não está configurado para gerar diário de bordo.");

            if (carga.TipoOperacao?.TipoImpressaoDiarioBordo == TipoImpressaoDiarioBordo.MinutaFreteBovino)
                return ObterTodosPdfMinutaFreteBovinoDiarioBordoCompactado(carga, buscarPorCargaOrigem);
            else
                return ObterTodosPdfDiarioBordoCompactado(carga, buscarPorCargaOrigem);
        }

        public byte[] ObterPdfRelacaoEmbarque(int codigoCarga)
        {
            return ReportRequest.WithType(ReportType.PdfRelacaoEmbarque)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", codigoCarga.ToString())
                .CallReport()
                .GetContentFile();
        }

        public byte[] ObterPdfRelatorioDeEmbarque(int codigoCarga)
        {
            return ReportRequest.WithType(ReportType.PdfRelatorioDeEmbarque)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", codigoCarga.ToString())
                .CallReport()
                .GetContentFile();
        }

        public byte[] ObterPdfDemonstrativoEstadia(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return ReportRequest.WithType(ReportType.PdfDemonstrativoEstadia)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", carga.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public byte[] ObterPdfComprovanteColeta(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            return ReportRequest.WithType(ReportType.PdfComprovanteColeta)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCargaEntrega", cargaEntrega.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public byte[] ObterMinutaTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return ReportRequest.WithType(ReportType.MinutaTransporte)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", carga.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public byte[] ObterCheckListMinutaTransporte(int codigoCarga)
        {
            return ReportRequest.WithType(ReportType.CheckListMinutaTransporte)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCarga", codigoCarga.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private byte[] ObterCRTsCompactado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador)
        {
            Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            decimal valorMoeda = repositorioCotacao.BuscarCotacao(MoedaCotacaoBancoCentral.DolarVenda, DateTime.Now)?.ValorMoeda ?? 0m;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = carga.Moeda;

            string siglaMoeda = string.Empty;

            if (moeda.HasValue)
                siglaMoeda = moeda.Value.ObterSigla();

            if (valorMoeda <= 0m)
            {
                Moedas.Cotacao servicoCotacao = new Moedas.Cotacao(_unitOfWork);
                valorMoeda = servicoCotacao.BuscarCotacaoDiaWSBancoCentral(MoedaCotacaoBancoCentral.DolarVenda, _unitOfWork);
            }

            if (valorMoeda <= 0m)
                valorMoeda = 1;

            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras = repCargaFronteira.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentesFretes = repCargaPedidoComponentesFrete.BuscarPorCarga(carga.Codigo, carga.EmpresaFilialEmissora != null);
            Dominio.Entidades.Empresa empresa = carga.Empresa;
            double filialCnpj = carga.Filial.CNPJ.ToDouble();
            Dominio.Entidades.Cliente filial = repositorioCliente.BuscarPorCPFCNPJ(filialCnpj);

            Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                Dominio.Entidades.Cliente remetente = pedido.Remetente;
                Dominio.Entidades.Cliente destinatario = pedido.ObterDestinatario();
                Dominio.Entidades.Cliente tomador = pedido.ObterTomador();

                int indiceFronteira = carga.DadosSumarizados?.Destinos?.IndexOf("Fronteira") ?? -1;

                decimal valor = carga.Pedidos.SelectMany(t => t.NotasFiscais).Select(t => t.XMLNotaFiscal?.Valor ?? 0).Sum();

                Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia conhecimentoTransporteInternacional = new Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia()
                {
                    Numero = $"BR{pedido.NumeroPedidoEmbarcador}",
                    PesoBruto = $"{carga.DadosSumarizados?.PesoTotal.ToString("n4")} KGs",
                    PesoLiquido = $"{carga.DadosSumarizados?.PesoLiquidoTotal.ToString("n4")} KGs",
                    Moeda = siglaMoeda,
                    Valor = valor.ToString("n2"),
                    Produtos = $"{string.Join(" / ", pedido.Produtos.Distinct().Select(obj => string.Join("-", obj.Quantidade, obj.Produto.Descricao, obj.PesoTotal.ToString("n4"))))}",
                    PrazoEntrega = carga.DataPrevisaoTerminoCarga?.ToString("dd/MM/yyyy"),
                    DataPosseTransportador = cargaJanelaCarregamentoTransportador?.DataCargaContratada?.ToString("dd/MM/yyyy") ?? string.Empty,
                    LocalidadeEmissao = cargaPedido.Origem?.DescricaoCidadeEstadoPais,
                    LocalidadePosseTransportador = cargaPedido.Origem?.DescricaoCidadeEstadoPais,
                    LocalidadeDestino = cargaPedido.Destino?.DescricaoCidadeEstadoPais,
                    Volume = $"{cargaPedido.Produtos.Sum(obj => obj.Produto.MetroCubito * obj.Quantidade).ToString("n4")} m³",
                    ValorFrete = (carga.ValorFreteAPagar / valorMoeda).ToString("n4"),
                    ValorTotal = (carga.ValorFreteAPagar / valorMoeda).ToString("n4"),
                    ValorMercadoria = $"{siglaMoeda}{valor.ToString("n2")}",
                    FormalidadesAlfandega = fronteiras?.Count() > 0 ? "Fronteira " + string.Join(", ", fronteiras.Select(o => o.Fronteira.Localidade.DescricaoCidadeEstado)) : indiceFronteira >= 0 ? carga.DadosSumarizados?.Destinos.Substring(carga.DadosSumarizados.Destinos.IndexOf("Fronteira")) : string.Empty,
                    DeclaracoesObservacoes = pedido?.DeclaracaoObservacaoCRT ?? string.Empty,
                };

                PreencherPessoasDataSourceCRT(conhecimentoTransporteInternacional, empresa, remetente, destinatario, tomador, cargaPedido.NotificacaoCRT, filial);

                conteudoCompactar.Add($"CRT_{pedido.NumeroPedidoEmbarcador}.pdf", ObterPdfCRT(conhecimentoTransporteInternacional));
            }

            MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
            byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

            arquivoCompactado.Dispose();

            if (arquivoCompactadoBinario == null)
                throw new ServicoException("Não foi possível gerar o(s) CRT(s).");

            return arquivoCompactadoBinario;
        }

        private byte[] ObterCRTComplementar(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repositorioRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = repositorioRelacaoDocumento.BuscarPorCTeGerado(cte.Codigo).CTeOriginal;

            Dominio.Entidades.Empresa empresa = cte.Empresa;
            Dominio.Entidades.Cliente remetente = cte.Remetente.Cliente;
            Dominio.Entidades.Cliente destinatario = cte.Destinatario.Cliente;
            Dominio.Entidades.Cliente tomador = cte.Tomador.Cliente;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaOcorrencia.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(carga.Codigo, carga.Empresa?.Codigo ?? 0);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(carga);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTipoPagamentoPago = cargasPedido.Where(o => o.Pedido?.TipoPagamento == TipoPagamento.Pago).Select(o => o.Pedido).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTipoPagamentoAPagar = cargasPedido.Where(o => o.Pedido?.TipoPagamento == TipoPagamento.A_Pagar).Select(o => o.Pedido).ToList();

            Dominio.Entidades.Cliente notificacaoCRT = cargasPedido.FirstOrDefault(obj => obj.NotificacaoCRT != null)?.NotificacaoCRT;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargasPedido.FirstOrDefault();

            double filialCnpj = carga.Filial.CNPJ.ToDouble();
            Dominio.Entidades.Cliente filial = repositorioCliente.BuscarPorCPFCNPJ(filialCnpj);
            string fronteiras = ObterFronteiras(carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = cteOriginal.Moeda;

            string valorFreteOrigem = string.Empty, valorFreteDestino = string.Empty, valorFreteTotal = string.Empty, siglaMoeda = string.Empty, desrcricaoMoeda = string.Empty;
            decimal valorFreteTotalDecimal = 0m, valorTotalMercadoria = 0m, valorCotacaoMoeda = 0m, valor = 0m;
            bool traduzirMoeda = false;

            valorCotacaoMoeda = cte.ValorCotacaoMoeda ?? 0m;
            valor = cte.ValorTotalMercadoria / (valorCotacaoMoeda > 0m ? valorCotacaoMoeda : 1m);


            if (moeda == MoedaCotacaoBancoCentral.DolarVenda)
                traduzirMoeda = true;


            if (cargaOcorrencia.ValorTotalMoeda.HasValue)
            {
                valorFreteTotal = cargaOcorrencia.ValorTotalMoeda.Value.ToString("n2");
                valorFreteTotalDecimal = cargaOcorrencia.ValorTotalMoeda.Value;
            }

            if (pedidosTipoPagamentoPago.Any())
            {
                valorFreteOrigem = valorFreteTotal;
                valorTotalMercadoria = valor + valorFreteTotalDecimal;
            }
            else if (pedidosTipoPagamentoAPagar.Any())
            {
                valorFreteDestino = valorFreteTotal;
                valorTotalMercadoria = valor;
            }

            if (moeda.HasValue)
            {
                siglaMoeda = moeda.Value.ObterSigla();
                desrcricaoMoeda = moeda.Value.ObterDescricaoSimplificada();
            }

            (string qtdVolume, string pesoBruto, string pesoLiquido) = SetarPesoEVolumeNotas(carga.Codigo);

            Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia conhecimentoTransporteInternacional = new Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia()
            {
                ValorFreteDestino = valorFreteDestino,
                MoedaDestino = valorFreteDestino != string.Empty ? siglaMoeda : string.Empty,
                ValorFreteOrigem = valorFreteOrigem,
                MoedaOrigem = valorFreteOrigem != string.Empty ? siglaMoeda : string.Empty,
                Numero = $"BR{cargaOcorrencia.NumeroOcorrencia}",
                PesoBruto = pesoBruto,
                PesoLiquido = pesoLiquido,
                Moeda = siglaMoeda,
                Valor = valor.ToString("n2"),
                Produtos = cargaPedido.DetalheMercadoria ?? string.Empty,
                PrazoEntrega = carga.DataPrevisaoTerminoCarga?.ToString("dd/MM/yyyy") ?? string.Empty,
                DataPosseTransportador = cargaJanelaCarregamentoTransportador?.DataCargaContratada?.ToString("dd/MM/yyyy") ?? string.Empty,
                LocalidadeEmissao = cargasPedido.Select(cp => cp.Pedido?.Filial?.Localidade?.DescricaoCidadeEstadoPais).FirstOrDefault(localidade => !string.IsNullOrWhiteSpace(localidade)) ?? string.Empty,
                LocalidadePosseTransportador = cteOriginal.LocalidadeInicioPrestacao?.DescricaoCidadeEstadoPais ?? string.Empty,
                LocalidadeDestino = cteOriginal.LocalidadeTerminoPrestacao?.DescricaoCidadeEstadoPais ?? string.Empty,
                Volume = qtdVolume,
                ValorTotalOrigem = valorFreteOrigem != string.Empty ? valorFreteOrigem : string.Empty,
                ValorTotalDestino = valorFreteDestino != string.Empty ? valorFreteDestino : string.Empty,
                ValorFrete = cargaOcorrencia.ValorTotalMoeda.HasValue ? Utilidades.Conversor.DecimalToWords(cargaOcorrencia.ValorTotalMoeda.Value, traduzirMoeda: traduzirMoeda) : string.Empty,
                ValorTotal = cargaOcorrencia.ValorTotalMoeda.HasValue ? cargaOcorrencia.ValorTotalMoeda.Value.ToString("n2") : string.Empty,
                ValorMercadoria = $"{siglaMoeda} {valorTotalMercadoria.ToString("n2")}",
                FormalidadesAlfandega = fronteiras,
                DeclaracoesObservacoes = cargasPedido.Select(cp => cp.Pedido?.DeclaracaoObservacaoCRT).FirstOrDefault(obs => !string.IsNullOrWhiteSpace(obs)) ?? string.Empty,
                Incoterm = cargasPedido.FirstOrDefault(o => o.Incoterm.HasValue)?.Incoterm?.ObterDescricao() ?? string.Empty,
                DataEmissao = cte.DataEmissao.ToDateString(),
                DocumentosAnexos = !string.IsNullOrWhiteSpace(cteOriginal.ObservacoesGerais) ? cteOriginal.ObservacoesGerais : cargaPedido.Pedido.ObservacaoCTe
            };

            PreencherPessoasDataSourceCRT(conhecimentoTransporteInternacional, empresa, remetente, destinatario, tomador, notificacaoCRT, filial);

            byte[] arquivo = ObterPdfCRT(conhecimentoTransporteInternacional);

            if (arquivo == null)
                throw new ServicoException("Não foi possível gerar o(s) CRT(s).");

            return arquivo;
        }

        private byte[] ObterCRT(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Empresa empresa = cte.Empresa;
            Dominio.Entidades.Cliente remetente = cte.Remetente.Cliente;
            Dominio.Entidades.Cliente destinatario = cte.Destinatario.Cliente;
            Dominio.Entidades.Cliente tomador = cte.Tomador.Cliente;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(carga.Codigo, carga.Empresa?.Codigo ?? 0);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentesFretes = repCargaPedidoComponentesFrete.BuscarPorCarga(carga.Codigo, carga.EmpresaFilialEmissora != null);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(carga);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTipoPagamentoPago = cargasPedido.Where(o => o.Pedido?.TipoPagamento == TipoPagamento.Pago).Select(o => o.Pedido).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTipoPagamentoAPagar = cargasPedido.Where(o => o.Pedido?.TipoPagamento == TipoPagamento.A_Pagar).Select(o => o.Pedido).ToList();

            Dominio.Entidades.Cliente notificacaoCRT = cargasPedido.FirstOrDefault(obj => obj.NotificacaoCRT != null)?.NotificacaoCRT;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargasPedido.FirstOrDefault();

            double filialCnpj = carga.Filial.CNPJ.ToDouble();
            Dominio.Entidades.Cliente filial = repositorioCliente.BuscarPorCPFCNPJ(filialCnpj);
            string fronteiras = ObterFronteiras(carga.Codigo);

            string valorFreteOrigem = string.Empty, valorFreteDestino = string.Empty;
            string valorSeguroOrigem = string.Empty, valorSeguroDestino = string.Empty;
            string valorTotalOrigem = string.Empty, valorTotalDestino = string.Empty;
            string valorOutrosOrigem = string.Empty, valorOutrosDestino = string.Empty;
            string valorFreteCrt = string.Empty, valorFreteCrtTotal = string.Empty;
            string valorOutros = string.Empty, valorSeguro = string.Empty;

            decimal valorOutrosDecimal = 0m, valorFrete = 0m, valorTotalMercadoria = 0m, valorSeguroDecimal = 0m, valorCotacaoMoeda = 0m, valor = 0m;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = carga.Moeda;

            string siglaMoeda = string.Empty;
            bool traduzirMoeda = false;

            if (moeda.HasValue)
                siglaMoeda = moeda.Value.ObterSigla();

            valorCotacaoMoeda = cte.ValorCotacaoMoeda ?? 0m;
            valor = cte.ValorTotalMercadoria / (valorCotacaoMoeda > 0m ? valorCotacaoMoeda : 1m);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete componenteFrete in componentesFretes)
            {
                decimal valorComponente = (moeda == MoedaCotacaoBancoCentral.Real) ? componenteFrete.ValorComponente : componenteFrete.ValorTotalMoeda ?? componenteFrete.ValorComponente;

                if (componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                    valorSeguroDecimal += valorComponente;
                else
                    valorOutrosDecimal += valorComponente;
            }

            if (moeda == MoedaCotacaoBancoCentral.Real)
            {
                valorFreteCrt = carga.ValorFrete > 0 ? carga.ValorFrete.ToString("n2") : string.Empty;
                valorFreteCrtTotal = carga.ValorFreteAPagar > 0 ? carga.ValorFreteAPagar.ToString("n2") : string.Empty;
                valorOutros = carga.ValorFreteAPagar > 0 && carga.ValorFreteAPagar > carga.ValorFrete ? (carga.ValorFreteAPagar - carga.ValorFrete - valorSeguroDecimal).ToString("n2") : string.Empty;
                valorSeguro = valorSeguroDecimal > 0 ? valorSeguroDecimal.ToString("n2") : string.Empty;
                valorFrete = carga.ValorFreteAPagar;
            }
            else if (moeda == MoedaCotacaoBancoCentral.DolarVenda)
            {
                decimal valorFreteCrtTotalDecimal = cte.ValorTotalMoeda.HasValue ? cte.ValorTotalMoeda.Value : 0m;

                valorFreteCrt = valorFreteCrtTotalDecimal > 0 && valorFreteCrtTotalDecimal > valorOutrosDecimal ? (valorFreteCrtTotalDecimal - valorOutrosDecimal - valorSeguroDecimal).ToString("n2") : string.Empty;
                valorFreteCrtTotal = valorFreteCrtTotalDecimal > 0 ? valorFreteCrtTotalDecimal.ToString("n2") : string.Empty;
                valorOutros = valorOutrosDecimal > 0 ? valorOutrosDecimal.ToString("n2") : string.Empty;
                valorSeguro = valorSeguroDecimal > 0 ? valorSeguroDecimal.ToString("n2") : string.Empty;
                valorFrete = valorFreteCrtTotalDecimal;
                traduzirMoeda = true;
            }

            if (pedidosTipoPagamentoPago.Any())
            {
                valorFreteOrigem = valorFreteCrt;
                valorSeguroOrigem = valorSeguro;
                valorTotalOrigem = valorFreteCrtTotal;
                valorOutrosOrigem = valorOutros;
                valorTotalMercadoria = valor + valorFrete;
            }
            else if (pedidosTipoPagamentoAPagar.Any())
            {
                valorFreteDestino = valorFreteCrt;
                valorSeguroDestino = valorSeguro;
                valorTotalDestino = valorFreteCrtTotal;
                valorOutrosDestino = valorOutros;
                valorTotalMercadoria = valor;
            }

            (string qtdVolume, string pesoBruto, string pesoLiquido) = SetarPesoEVolumeNotas(carga.Codigo);

            Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia conhecimentoTransporteInternacional = new Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia()
            {
                ValorFreteDestino = valorFreteDestino,
                MoedaDestino = siglaMoeda,
                ValorFreteOrigem = valorFreteOrigem,
                MoedaOrigem = siglaMoeda,
                Numero = cte.NumeroCRT,
                PesoBruto = pesoBruto,
                PesoLiquido = pesoLiquido,
                Moeda = siglaMoeda,
                Valor = valor.ToString("n2"),
                Produtos = cargaPedido.DetalheMercadoria ?? string.Empty,
                PrazoEntrega = carga.DataPrevisaoTerminoCarga?.ToString("dd/MM/yyyy") ?? string.Empty,
                DataPosseTransportador = cargaJanelaCarregamentoTransportador?.DataCargaContratada?.ToString("dd/MM/yyyy") ?? string.Empty,
                LocalidadeEmissao = cargasPedido.Select(cp => cp.Pedido?.Filial?.Localidade?.DescricaoCidadeEstadoPais).FirstOrDefault(localidade => !string.IsNullOrWhiteSpace(localidade)) ?? string.Empty,
                LocalidadePosseTransportador = cte.LocalidadeInicioPrestacao?.DescricaoCidadeEstadoPais ?? string.Empty,
                LocalidadeDestino = cte.LocalidadeTerminoPrestacao?.DescricaoCidadeEstadoPais ?? string.Empty,
                Volume = qtdVolume,
                ValorFrete = Utilidades.Conversor.DecimalToWords(valorFrete, traduzirMoeda: traduzirMoeda),
                ValorTotal = valorFreteCrtTotal,
                ValorTotalOrigem = valorTotalOrigem,
                ValorTotalDestino = valorTotalDestino,
                ValorMercadoria = $"{siglaMoeda} {valorTotalMercadoria.ToString("n2")}",
                FormalidadesAlfandega = fronteiras,
                DeclaracoesObservacoes = cargasPedido.Select(cp => cp.Pedido?.DeclaracaoObservacaoCRT).FirstOrDefault(obs => !string.IsNullOrWhiteSpace(obs)) ?? string.Empty,
                Incoterm = cargasPedido.FirstOrDefault(o => o.Incoterm.HasValue)?.Incoterm?.ObterDescricao() ?? string.Empty,
                ValorOutrosOrigem = valorOutrosOrigem,
                ValorOutrosDestino = valorOutrosDestino,
                DataEmissao = cte.DataEmissao.ToDateString(),
                ValorSeguroOrigem = valorSeguroOrigem,
                ValorSeguroDestino = valorSeguroDestino,
                DocumentosAnexos = !string.IsNullOrWhiteSpace(cte.ObservacoesGerais) ? cte.ObservacoesGerais : cargaPedido.Pedido.ObservacaoCTe
            };

            PreencherPessoasDataSourceCRT(conhecimentoTransporteInternacional, empresa, remetente, destinatario, tomador, notificacaoCRT, filial);

            byte[] arquivo = ObterPdfCRT(conhecimentoTransporteInternacional);

            if (arquivo == null)
                throw new ServicoException("Não foi possível gerar o(s) CRT(s).");

            return arquivo;
        }

        private void PreencherPessoasDataSourceCRT(Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia crt, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente notificacaoCRT, Dominio.Entidades.Cliente filial)
        {
            string cnpjRemetente = ObterIdentificacao(remetente);
            string cnpjDestinatario = ObterIdentificacao(destinatario);
            string cnpjTomador = ObterIdentificacao(tomador);
            string cnpjNotificacaoCRT = ObterIdentificacao(notificacaoCRT);
            string cnpjFilial = ObterIdentificacao(filial);

            crt.NomeDestinatario = destinatario?.Nome ?? string.Empty;
            crt.NomeRemetente = remetente?.Nome ?? string.Empty;
            crt.NomeConsignatario = tomador?.Nome ?? string.Empty;
            crt.NomeTransportador = filial?.Nome ?? string.Empty;
            crt.NotificacaoCRT = notificacaoCRT?.Nome ?? tomador?.Nome ?? string.Empty;
            crt.NomeCPFCNPJTransportador = ObterNomeCPFCNPJ(filial);

            string enderecoConsignatario = ObterEndereco(tomador, cnpjTomador);

            crt.EnderecoDestinatario = ObterEndereco(destinatario, cnpjDestinatario);
            crt.EnderecoRemetente = ObterEndereco(remetente, cnpjRemetente);
            crt.EnderecoConsignatario = enderecoConsignatario;
            crt.EnderecoTransportador = ObterEndereco(filial, cnpjFilial);
            crt.EnderecoNotificacaoCRT = !string.IsNullOrEmpty(cnpjNotificacaoCRT) ? ObterEndereco(notificacaoCRT, cnpjNotificacaoCRT) : enderecoConsignatario;
        }

        private string ObterNomeCPFCNPJ(Dominio.Entidades.Cliente cliente)
        {
            if (cliente == null) return string.Empty;

            return cliente.Tipo.Equals("E")
                ? $"{cliente.Nome} {cliente.NumeroCUITRUT}"
                : $"{cliente.Nome} {cliente.CPF_CNPJ_Formatado}";
        }

        private string ObterIdentificacao(Dominio.Entidades.Cliente cliente)
        {
            if (cliente == null) return string.Empty;

            return cliente.Tipo.Equals("E")
                ? $"RUT: {cliente.NumeroCUITRUT}"
                : $"CNPJ NUMBER {cliente.CPF_CNPJ_Formatado}";
        }

        private string ObterEndereco(Dominio.Entidades.Cliente cliente, string identificacao)
        {
            if (cliente == null) return string.Empty;

            return $"{cliente.Endereco?.Trim()} {cliente.Numero?.Trim()} {cliente.Complemento?.Trim()} - " +
                   $"{cliente.Localidade?.Descricao} {cliente.Localidade?.Pais?.Descricao} - {identificacao}";
        }

        private byte[] ObterTodosPdfDiarioBordoCompactado(Dominio.Entidades.Embarcador.Cargas.Carga cargaGeracao, bool buscarPorCargaOrigem)
        {
            Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
            CargaDadosSumarizados servicoCargaDadosSumarizados = new CargaDadosSumarizados(_unitOfWork);
            GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaVeiculoContainer repositorioCargaVeiculoContainer = new Repositorio.Embarcador.Cargas.CargaVeiculoContainer(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(cargaGeracao);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = buscarPorCargaOrigem ? repositorioCargaPedido.BuscarPorCargaOrigem(cargaGeracao.Codigo) : repositorioCargaPedido.BuscarPorCarga(cargaGeracao.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXMLNotaFiscal = buscarPorCargaOrigem ? repositorioPedidoXMLNotaFiscal.BuscarPorCargaOrigem(cargaGeracao.Codigo) : repositorioPedidoXMLNotaFiscal.BuscarPorCarga(cargaGeracao.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> listaCargaVeiculoContainer = buscarPorCargaOrigem ? repositorioCargaVeiculoContainer.BuscarPorCarga(cargaGeracao.CargaAgrupamento.Codigo) : repositorioCargaVeiculoContainer.BuscarPorCarga(cargaGeracao.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = !buscarPorCargaOrigem ? listaCargaPedido.Select(o => o.CargaOrigem).Distinct().ToList() : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            if (cargas.Count == 0)
                cargas.Add(cargaGeracao);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                int contadorClienteExportacao = 0;
                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = carga.CargaAgrupamento ?? carga;
                Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = carga;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = !buscarPorCargaOrigem ? listaCargaPedido.Where(o => o.CargaOrigem.Codigo == carga.Codigo).ToList() : new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                if (configuracaoEmbarcador.GerarFluxoPatioPorCargaAgrupada && (carga.CargaAgrupamento != null) && (carga.Filial?.Codigo == carga.CargaAgrupamento.Filial?.Codigo))
                    cargaReferencia = carga.CargaAgrupamento;

                if (cargasPedido.Count == 0)
                    cargasPedido.AddRange(listaCargaPedido);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(cargaReferencia.Codigo);

                IEnumerable<Dominio.Entidades.Cliente> destinatarios = (
                    from o in cargasPedido where o.Recebedor == null && o.Pedido.Destinatario != null select o.Pedido.Destinatario).ToList().Concat((
                    from o in cargasPedido where o.Recebedor != null select o.Recebedor).ToList()
                ).Distinct();

                IEnumerable<Dominio.Entidades.Cliente> remetentes = (
                    from o in cargasPedido where o.Pedido.Remetente != null select o.Pedido.Remetente).ToList().Concat((
                    from o in cargasPedido where o.Expedidor != null select o.Expedidor).ToList()
                ).Distinct();

                foreach (Dominio.Entidades.Cliente remetente in remetentes)
                {
                    foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidoPorRemetenteEDestinatario = (
                            from o in cargasPedido
                            where (
                                (
                                    (o.Pedido.Destinatario != null && o.Pedido.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ) ||
                                    (o.Recebedor != null && o.Recebedor.CPF_CNPJ == destinatario.CPF_CNPJ)
                                ) &&
                                (
                                    (o.Pedido.Destinatario != null && o.Pedido.Remetente.CPF_CNPJ == remetente.CPF_CNPJ) ||
                                    (o.Expedidor != null && o.Expedidor.CPF_CNPJ == remetente.CPF_CNPJ)
                                )
                            )
                            select o
                        ).ToList();

                        string descricaoRemetente = remetente.CPF_CNPJ_SemFormato;
                        string descricaoDestinatario = destinatario.CPF_CNPJ_SemFormato;

                        if (destinatario.Tipo.Equals("E"))
                            descricaoDestinatario = string.IsNullOrWhiteSpace(destinatario.CodigoIntegracao) ? $"exportacao_{++contadorClienteExportacao}" : destinatario.CodigoIntegracao;

                        if ((cargaAgrupada.ModeloVeicularCarga?.NumeroReboques > 1) && (cargaAgrupada.VeiculosVinculados?.Count > 1))
                        {
                            Dictionary<NumeroReboque, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> listaCargaPedidoPorReboque = new Dictionary<NumeroReboque, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>>();

                            listaCargaPedidoPorReboque.Add(NumeroReboque.ReboqueUm, listaCargaPedidoPorRemetenteEDestinatario.Where(o => o.NumeroReboque == NumeroReboque.SemReboque || o.NumeroReboque == NumeroReboque.ReboqueUm).ToList());
                            listaCargaPedidoPorReboque.Add(NumeroReboque.ReboqueDois, listaCargaPedidoPorRemetenteEDestinatario.Where(o => o.NumeroReboque == NumeroReboque.ReboqueDois).ToList());

                            foreach (KeyValuePair<NumeroReboque, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>> cargaPedidoPorReboque in listaCargaPedidoPorReboque)
                            {
                                if (cargaPedidoPorReboque.Value.Count > 0)
                                {
                                    Dominio.Entidades.Veiculo reboque = cargaAgrupada.VeiculosVinculados?.ElementAt(((int)cargaPedidoPorReboque.Key) - 1);
                                    Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer = (from o in listaCargaVeiculoContainer where o.Veiculo.Codigo == reboque.Codigo select o).FirstOrDefault();
                                    Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo diarioBordo = ObterDiarioBordoBase(carga, cargaReferencia, cargaAgrupada, remetente, destinatario, fluxoGestaoPatio, cargaJanelaCarregamento);

                                    diarioBordo.Booking = string.Join(" / ", cargaPedidoPorReboque.Value.Select(o => o.Pedido.Reserva).Distinct().ToList());
                                    diarioBordo.DataEntrega = cargaReferencia.DataPrevisaoTerminoCarga?.ToString("dd/MM/yyyy HH:mm") ?? cargaPedidoPorReboque.Value.FirstOrDefault().Pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "";
                                    diarioBordo.DataRetiradaCtrn = cargaVeiculoContainer?.DataRetiradaCtrn?.ToString("dd/MM/yyyy") ?? "";
                                    diarioBordo.NotaFiscal = string.Join(" / ", (from o in listaPedidoXMLNotaFiscal where cargaPedidoPorReboque.Value.Any(p => p.Codigo == o.CargaPedido.Codigo) select o.XMLNotaFiscal.Numero.ToString()));
                                    diarioBordo.NumeroContainer = cargaVeiculoContainer?.NumeroContainer ?? "";
                                    diarioBordo.Ordem = string.Join(" / ", cargaPedidoPorReboque.Value.Select(o => o.Pedido.Ordem).Distinct().ToList());
                                    diarioBordo.Pedido = string.Join(" / ", cargaPedidoPorReboque.Value.Select(o => o.Pedido.NumeroPedidoEmbarcador).Distinct().ToList());
                                    diarioBordo.PlacaReboques = reboque.Placa_Formatada;

                                    conteudoCompactar.Add($"diario_bordo_{cargaReferencia.CodigoCargaEmbarcador}_{descricaoRemetente}_{descricaoDestinatario}_{reboque.Placa}.pdf", ObterPdfDiarioBordo(diarioBordo));
                                }
                            }
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedidoPorRemetenteEDestinatario.FirstOrDefault();
                            if (cargaPedido != null)
                            {
                                Dominio.Entidades.Veiculo reboque = cargaAgrupada.VeiculosVinculados?.FirstOrDefault();
                                Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer = (reboque == null) ? null : (from o in listaCargaVeiculoContainer where o.Veiculo.Codigo == reboque.Codigo select o).FirstOrDefault();
                                Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo diarioBordo = ObterDiarioBordoBase(carga, cargaReferencia, cargaAgrupada, remetente, destinatario, fluxoGestaoPatio, cargaJanelaCarregamento);

                                diarioBordo.Booking = string.Join(" / ", listaCargaPedidoPorRemetenteEDestinatario.Select(o => o.Pedido.Reserva).Distinct().ToList());
                                diarioBordo.DataEntrega = cargaReferencia.DataPrevisaoTerminoCarga?.ToString("dd/MM/yyyy HH:mm") ?? cargaPedido.Pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "";
                                diarioBordo.DataRetiradaCtrn = cargaVeiculoContainer?.DataRetiradaCtrn?.ToString("dd/MM/yyyy") ?? "";
                                diarioBordo.NotaFiscal = string.Join(" / ", (from o in listaPedidoXMLNotaFiscal where listaCargaPedidoPorRemetenteEDestinatario.Any(p => p.Codigo == o.CargaPedido.Codigo) select o.XMLNotaFiscal.Numero.ToString()));
                                diarioBordo.NumeroContainer = cargaVeiculoContainer?.NumeroContainer ?? "";
                                diarioBordo.Ordem = string.Join(" / ", listaCargaPedidoPorRemetenteEDestinatario.Select(o => o.Pedido.Ordem).Distinct().ToList());
                                diarioBordo.Pedido = string.Join(" / ", listaCargaPedidoPorRemetenteEDestinatario.Select(o => o.Pedido.NumeroPedidoEmbarcador).Distinct().ToList());
                                diarioBordo.PlacaReboques = reboque?.Placa_Formatada;

                                conteudoCompactar.Add($"diario_bordo_{cargaReferencia.CodigoCargaEmbarcador}_{descricaoRemetente}_{descricaoDestinatario}_{cargaPedido.Pedido.NumeroPedidoEmbarcador}.pdf", ObterPdfDiarioBordo(diarioBordo));
                            }
                        }
                    }
                }
            }

            MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
            byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

            arquivoCompactado.Dispose();

            if (arquivoCompactadoBinario == null)
                throw new ServicoException("Não foi possível gerar o diário de bordo.");

            return arquivoCompactadoBinario;
        }

        private byte[] ObterTodosPdfMinutaFreteBovinoDiarioBordoCompactado(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool buscarPorCargaOrigem)
        {
            Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = buscarPorCargaOrigem ? repositorioCargaPedido.BuscarPorCargaOrigem(carga.Codigo) : repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                Dominio.Entidades.Cliente cliente = repositorioCliente.BuscarPorCPFCNPJ(pedido.Filial?.CNPJ.ToDouble() ?? 0);

                Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.MinutaFreteBovino dataSourceMinutaFreteBovino = new Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.MinutaFreteBovino()
                {
                    Numero = pedido.NumeroPedidoEmbarcador,
                    Filial = pedido.Filial?.Descricao,
                    CidadeFilial = pedido.Filial?.Localidade?.DescricaoCidadeEstado ?? cliente?.Localidade?.DescricaoCidadeEstado,
                    EnderecoFilial = cliente?.Endereco + ", " + cliente?.Numero + " - " + cliente?.Complemento,
                    TelefoneFilial = cliente?.Telefone1,
                    Remetente = pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente?.Descricao ?? "",
                    Resumo = pedido.Resumo,
                    Origem = pedido.Origem?.DescricaoCidadeEstado,
                    DataPrevista = pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    AnimaisEmbarque = pedido.Reserva + (!string.IsNullOrWhiteSpace(pedido.Reserva) ? " - " + pedido.Produtos.Select(o => o.Quantidade).Sum().ToString("n2") : ""),
                    Transportador = carga.Empresa?.Descricao,
                    Motorista = carga.NomeMotoristas,
                    Placa = carga.PlacasVeiculos,
                    ModeloVeicular = carga.ModeloVeicularCarga?.Descricao,
                    Roteiro = Servicos.Embarcador.Pedido.Pedido.RetornarObservacaoCTeDoPedidoFormatado(pedido, carga, _unitOfWork)
                };

                conteudoCompactar.Add($"minuta_frete_bovino_{pedido.NumeroPedidoEmbarcador}.pdf", ObterPdfMinutaFrete(dataSourceMinutaFreteBovino));
            }

            MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
            byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

            arquivoCompactado.Dispose();

            if (arquivoCompactadoBinario == null)
                throw new ServicoException("Não foi possível gerar a minuta de frete bovino.");

            return arquivoCompactadoBinario;
        }

        private Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo ObterDiarioBordoBase(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia, Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Dominio.Entidades.Usuario motorista = cargaAgrupada.Motoristas?.FirstOrDefault();
            Dominio.Entidades.Empresa transportador = cargaAgrupada.CargaDeComplemento ? carga.Empresa : cargaAgrupada.Empresa;

            Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo dataSourceDiarioBordoBase = new Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo()
            {
                Carga = cargaReferencia.CodigoCargaEmbarcador,
                Cliente = destinatario.Descricao,
                CpfCnpjMotorista = motorista?.CPF_Formatado ?? "",
                DataChegadaEmbarque = fluxoGestaoPatio?.DataChegadaVeiculo?.ToString("dd/MM/yyyy HH:mm"),
                DataDocumentacaoLiberada = fluxoGestaoPatio?.DataTravaChave?.ToString("dd/MM/yyyy HH:mm"),
                DataProgramacaoEmbarque = "",
                Destino = destinatario.Localidade.DescricaoCidadeEstado,
                Filial = cargaReferencia.Filial?.Descricao ?? "",
                Lacre = string.Join(", ", (from lacre in cargaAgrupada.Lacres select lacre.Numero)),
                NomeMotorista = motorista?.Nome ?? "",
                Origem = remetente.Localidade.DescricaoCidadeEstado,
                PlacaTracao = cargaAgrupada.Veiculo?.Placa_Formatada ?? "",
                QuantidadeEntrega = carga.DadosSumarizados.NumeroEntregas,
                RgMotorista = motorista?.RG ?? "",
                Titulo = $"Diário de Bordo{(cargaReferencia.Filial == null ? "" : $" - {cargaReferencia.Filial.Descricao}")}",
                Transportador = transportador?.Descricao ?? ""
            };

            if (cargaJanelaCarregamento != null && cargaJanelaCarregamento.InicioCarregamento != DateTime.MinValue)
                dataSourceDiarioBordoBase.DataProgramacaoEmbarque = cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy HH:mm");
            else if (carga.Carregamento != null && carga.Carregamento.DataCarregamentoCarga != DateTime.MinValue)
                dataSourceDiarioBordoBase.DataProgramacaoEmbarque = carga.Carregamento.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "";

            return dataSourceDiarioBordoBase;
        }

        private int ObterNumeroImpressora(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.WMS.Deposito repositorioDeposito = new Repositorio.Embarcador.WMS.Deposito(_unitOfWork);
            Dominio.Entidades.Embarcador.WMS.Deposito deposito = repositorioDeposito.BuscarPorCarga(carga.Codigo);

            int numeroImpressora;
            if (deposito != null)
                numeroImpressora = deposito.NumeroUnidadeImpressao;
            else
                numeroImpressora = carga.Filial.NumeroUnidadeImpressao;

            if (!string.IsNullOrWhiteSpace(carga.NumeroImpressora))
            {
                Repositorio.Impressora repositorioImpressora = new Repositorio.Impressora(_unitOfWork);
                Dominio.Entidades.Impressora impressora = repositorioImpressora.BuscarPorCodigoIntegracao(carga.NumeroImpressora);

                if (impressora == null)
                    throw new ServicoException($"Não existe uma impressora cadastrada com código de integração {carga.NumeroImpressora}.");

                if (impressora.NumeroDaUnidade == 0)
                    throw new ServicoException("Impressora não possui um número para solicitar a impressão.");

                numeroImpressora = impressora.NumeroDaUnidade;
            }

            if (numeroImpressora <= 0)
                throw new ServicoException("A filial/carga não possui impressão configurada.");

            return numeroImpressora;
        }

        private byte[] ObterPdfDiarioBordo(Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.DiarioBordo dataSourceDiarioBordo)
        {
            return ReportRequest.WithType(ReportType.PdfDiarioBordo)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("DataSourceDiarioBordo", dataSourceDiarioBordo.ToJson())// TODO: Verificar serializacao pq esta sendo passado a classe inteira
                .CallReport()
                .GetContentFile();
        }

        private byte[] ObterPdfCRT(Dominio.Relatorios.Embarcador.DataSource.Cargas.CRT.ConhecimentoTransporteInternacionalRodovia dataSourceCrt)
        {
            return ReportRequest.WithType(ReportType.PdfCRT)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("DataSourceCrt", dataSourceCrt.ToJson()) //TODO: Validar serializacao
                .CallReport()
                .GetContentFile();
        }

        private byte[] ObterPdfMinutaFrete(Dominio.Relatorios.Embarcador.DataSource.Cargas.DiarioBordo.MinutaFreteBovino dataSourceMinutaFreteBovino)
        {
            return ReportRequest.WithType(ReportType.PdfMinutaFrete)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("DataSourceMinutaFreteBovino", dataSourceMinutaFreteBovino.ToJson())
                .CallReport()
                .GetContentFile();
        }

        private string ObterFronteiras(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);

            List<(string NomeFantasia, string Nome)> listaFronteirasOrdenadas = repositorioCargaRotaFretePontosPassagem.BuscarNomeFantasiaPorCargaOrdenado(codigoCarga);

            if (!listaFronteirasOrdenadas.Any())
                return string.Empty;

            return string.Join(", ", listaFronteirasOrdenadas.Select(fronteira => !string.IsNullOrWhiteSpace(fronteira.NomeFantasia) ? fronteira.NomeFantasia : fronteira.Nome));
        }

        private (string qtdVolume, string pesoBruto, string pesoLiquido) SetarPesoEVolumeNotas(int codigoCarga)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repositorioPedidoXMLNotaFiscal.BuscarNotasPorCargaETipoFatura(codigoCarga, true);

            int qtdVolume = notasFiscais.Sum(obj => obj.XMLNotaFiscal.Volumes);
            decimal pesoBruto = notasFiscais.Sum(obj => obj.XMLNotaFiscal.Peso);
            decimal pesoLiquido = notasFiscais.Sum(obj => obj.XMLNotaFiscal.PesoLiquido);

            return ($"{qtdVolume:n4} m³", $"{pesoBruto:n4} KGs", $"{pesoLiquido:n4} KGs");
        }

        #endregion
    }
}
