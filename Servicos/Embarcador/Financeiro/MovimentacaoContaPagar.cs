using CsvHelper;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Financeiro
{
    public class MovimentacaoContaPagar
    {
        #region Atributos

        readonly private Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Construtor
        public MovimentacaoContaPagar(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Metodos Publicos
        public void ProcessarArquivoMovimentacaoContaPagar()
        {
            Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento repositorioLoteEscrituracaoMiroDocumento = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento(_unitOfWork);

            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacao = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(_unitOfWork);
            List<int> contasAProcessar = repositorioContaPagar.ObterContaApagarAguardandoProcessamento();

            foreach (var codigoConta in contasAProcessar)
            {
                List<string> errosArquivo = new List<string>();

                _unitOfWork.Start();
                var contaPagar = repositorioContaPagar.BuscarPorCodigo(codigoConta, false);

                try
                {
                    string caminhoArquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.ObterCaminhoArquivoIntegracao(_unitOfWork);

                    string localArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, contaPagar.ArquivoAProcessar.NomeArquivo);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(localArquivo))
                    {
                        contaPagar.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoArquivo.ErroNoArquivo;
                        contaPagar.MensagemProcessamento = "Arquivo não encontrado!";
                        repositorioContaPagar.Atualizar(contaPagar);
                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                        continue;
                    }

                    var configCsv = new CsvHelper.Configuration.Configuration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = "|",
                        HasHeaderRecord = true
                    };


                    contaPagar.Situacao = SituacaoProcessamentoArquivo.Processado;
                    List<TipoRegistro> tiposRegistrosPermitidos = new List<TipoRegistro>() { TipoRegistro.PendentesemAberto, TipoRegistro.PagoviaConfirming, TipoRegistro.Debitoscompensados, TipoRegistro.Pagoviacreditoemconta, TipoRegistro.TotaldeAdiantamento };
                    if (tiposRegistrosPermitidos.Contains(contaPagar.TipoRegistro))
                    {
                        using (var reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(localArquivo)))
                        using (var csv = new CsvReader(reader, configCsv))
                        {
                            var records = csv.GetRecords<Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloAMovimentoFinanceiro>();
                            int contador = 0;

                            foreach (var modeloA in records)
                            {
                                //if (contador == 0)
                                //{
                                //    contador++;
                                //    continue;
                                //}

                                contador++;

                                Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar novaMovimentacao = new Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar();

                                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigoIntegracao(modeloA.Vendor);
                                if (transportador == null)
                                {
                                    errosArquivo.Add($"linha {contador}: Transportador não encontrado. Código integração: {modeloA.Vendor}");
                                    continue;
                                }

                                if (errosArquivo.Count > 0)
                                    continue;

                                novaMovimentacao.Transportador = transportador;
                                novaMovimentacao.CodigoEmpresa = modeloA.CompanyCode;
                                novaMovimentacao.Referencia = modeloA.Reference;
                                novaMovimentacao.PmntBlock = Utilidades.String.Left(modeloA.PmntBlock, 1);
                                novaMovimentacao.PostedKey = PostedKeyHelper.ObterTipoPost(modeloA.PostingKey);
                                novaMovimentacao.CodigoTaxa = Utilidades.String.Left(modeloA.TaxCode, 10);

                                if (modeloA.DocumentDate.HasValue && modeloA.DocumentDate.Value != DateTime.MinValue)
                                    novaMovimentacao.DataDocumento = modeloA.DocumentDate.Value;

                                if (modeloA.DueDate.HasValue && modeloA.DueDate.Value != DateTime.MinValue)
                                {
                                    novaMovimentacao.DueData = modeloA.DueDate.Value;
                                    novaMovimentacao.SituacaoVencimento = modeloA.DueDate < DateTime.Now
                                        ? SituacaoVencimentoMovimentacao.Vencido : modeloA.DueDate.Value.Date == DateTime.Now.Date
                                        ? SituacaoVencimentoMovimentacao.Vencendo : SituacaoVencimentoMovimentacao.AVencer;
                                }

                                if (modeloA.Clearing.HasValue && modeloA.Clearing.Value != DateTime.MinValue)
                                    novaMovimentacao.DataCompensamento = modeloA.Clearing.Value;

                                if (modeloA.PostingDate.HasValue && modeloA.PostingDate.Value != DateTime.MinValue)
                                    novaMovimentacao.DataPostagem = modeloA.PostingDate.Value;

                                novaMovimentacao.TipoDocumento = Utilidades.String.Left(modeloA.DocumentType, 20);
                                novaMovimentacao.ValorMonetario = modeloA.AmountInLC;
                                novaMovimentacao.EspecialGL = Utilidades.String.Left(modeloA.SpecialGLIndicator, 20);

                                novaMovimentacao.ClrngDoc = Utilidades.String.Left(modeloA.ClearingDocument, 20);
                                novaMovimentacao.Texto = Utilidades.String.Left(modeloA.Text, 20);
                                novaMovimentacao.NumeroDocumento = Utilidades.String.Left(modeloA.DocumentNumber, 20);
                                novaMovimentacao.NomeUsuario = Utilidades.String.Left(modeloA.UserName, 30);
                                novaMovimentacao.InvoiceRef = Utilidades.String.Left(modeloA.InvoiceReference, 30);

                                novaMovimentacao.Atribuicao = Utilidades.String.Left(modeloA.Assignment, 30);
                                novaMovimentacao.DisBase = modeloA.DiscountBase;
                                novaMovimentacao.Moeda = Utilidades.String.Left(modeloA.Currency, 10);
                                novaMovimentacao.TermoPagamento = Utilidades.String.Left(modeloA.PaymentTerms, 10);
                                novaMovimentacao.DocTextoCabecalho = Utilidades.String.Left(modeloA.DocumentHeaderText, 30);
                                novaMovimentacao.ChaveReferencia = Utilidades.String.Left(modeloA.ReferenceKey, 30);
                                novaMovimentacao.DataUpload = DateTime.Now;
                                novaMovimentacao.SituacaoProcessamento = SituacaoProcessamento.AguardandoProcessamento;

                                novaMovimentacao.TipoRegistro = contaPagar.TipoRegistro;
                                novaMovimentacao.ContaPagar = contaPagar;
                                repositorioMovimentacao.Inserir(novaMovimentacao);
                            }
                        }

                        if (errosArquivo.Count > 0)
                            throw new ServicoException("Error");

                        repositorioContaPagar.Atualizar(contaPagar);
                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                        continue;
                    }

                    if (contaPagar.TipoRegistro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistro.NotasCompensadasXAdiantamento)
                    {
                        using (var reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(localArquivo)))
                        using (var csv = new CsvReader(reader, configCsv))
                        {
                            var records = csv.GetRecords<Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloBMovimentoFinanceiro>();
                            int contador = 0;

                            foreach (var modeloA in records)
                            {
                                //if (contador == 0)
                                //{
                                //    contador++;
                                //    continue;
                                //}

                                contador++;

                                Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar novaMovimentacao = new Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar();

                                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigoIntegracao(modeloA.Vendor);

                                if (transportador == null)
                                {
                                    errosArquivo.Add($"linha {contador}: Transportador não encontrado. Código integração: {modeloA.Vendor}");
                                    continue;
                                }

                                if (errosArquivo.Count > 0)
                                    continue;

                                novaMovimentacao.Transportador = transportador;
                                novaMovimentacao.Referencia = Utilidades.String.Left(modeloA.Reference, 20);
                                novaMovimentacao.NomeUsuario = Utilidades.String.Left(modeloA.Name, 20);
                                novaMovimentacao.ValorTotal = modeloA.TotalValue;
                                novaMovimentacao.ClrngDoc = Utilidades.String.Left(modeloA.ClearingDocument, 20);
                                novaMovimentacao.ValorMonetario = modeloA.AmountInLC;
                                novaMovimentacao.ChaveReferencia = Utilidades.String.Left(modeloA.MaterialDocument, 20);
                                novaMovimentacao.DocumentoCompra = Utilidades.String.Left(modeloA.PurchasingDocument, 20);
                                novaMovimentacao.TipoDocumento = Utilidades.String.Left(modeloA.DocumentType, 20);
                                novaMovimentacao.SituacaoProcessamento = SituacaoProcessamento.AguardandoProcessamento;
                                novaMovimentacao.DataUpload = DateTime.Now;

                                if (modeloA.Clearing.HasValue && modeloA.Clearing.Value != DateTime.MinValue)
                                    novaMovimentacao.DataCompensamento = modeloA.Clearing.Value;

                                novaMovimentacao.TipoRegistro = contaPagar.TipoRegistro;
                                novaMovimentacao.ContaPagar = contaPagar;
                                repositorioMovimentacao.Inserir(novaMovimentacao);
                            }
                        }

                        if (errosArquivo.Count > 0)
                            throw new ServicoException("Error");

                        repositorioContaPagar.Atualizar(contaPagar);
                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                        continue;
                    }

                    if (contaPagar.TipoRegistro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistro.Cockpit)
                    {
                        Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                        using (var reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(localArquivo)))
                        using (var csv = new CsvReader(reader, configCsv))
                        {
                            var records = csv.GetRecords<Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloCMovimentoFinanceiro>();
                            int contador = 0;

                            foreach (var modeloA in records)
                            {
                                //if (contador == 0)
                                //{
                                //    contador++;
                                //    continue;
                                //}

                                contador++;

                                Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar novaMovimentacao = new Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar();

                                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigoIntegracao(modeloA.Vendor);

                                if (transportador == null)
                                {
                                    errosArquivo.Add($"linha {contador}: Transportador não encontrado. Código integração: {modeloA.Vendor}");
                                    continue;
                                }

                                if (errosArquivo.Count > 0)
                                    continue;

                                novaMovimentacao.Transportador = transportador;
                                novaMovimentacao.ChaveAcesso = Utilidades.String.Left(modeloA.AccessKey, 20);

                                if (modeloA.PostingDate.HasValue && modeloA.PostingDate.Value != DateTime.MinValue)
                                {
                                    novaMovimentacao.DataPostagem = modeloA.PostingDate.Value;
                                    novaMovimentacao.DataDocumento = modeloA.PostingDate.Value;
                                }

                                if (modeloA.DueDate.HasValue && modeloA.DueDate.Value != DateTime.MinValue)
                                {
                                    novaMovimentacao.DueData = modeloA.DueDate.Value;
                                    novaMovimentacao.SituacaoVencimento = modeloA.DueDate < DateTime.Now
                                    ? SituacaoVencimentoMovimentacao.Vencido : modeloA.DueDate.Value.Date == DateTime.Now.Date
                                    ? SituacaoVencimentoMovimentacao.Vencendo : SituacaoVencimentoMovimentacao.AVencer;
                                }

                                if (modeloA.UploadDate.HasValue && modeloA.UploadDate.Value != DateTime.MinValue)
                                    novaMovimentacao.DataUpload = modeloA.UploadDate.Value;

                                if (modeloA.TaxApprovalDate.HasValue && modeloA.TaxApprovalDate.Value != DateTime.MinValue)
                                    novaMovimentacao.DataAprovacaoTaxa = modeloA.TaxApprovalDate;

                                novaMovimentacao.NumeroCte = Utilidades.String.Left(modeloA.CteNumber, 20);
                                novaMovimentacao.SerieCte = Utilidades.String.Left(modeloA.CteSeries, 20);
                                novaMovimentacao.CodigoEmpresa = Utilidades.String.Left(modeloA.CompanyCode, 20);
                                novaMovimentacao.NetValor = modeloA.NetAmount;
                                novaMovimentacao.DisBase = modeloA.BaseAmount;
                                novaMovimentacao.Moeda = Utilidades.String.Left(modeloA.Currency, 20);
                                novaMovimentacao.Protocolo = Utilidades.String.Left(modeloA.ProtocolNumber, 20);
                                novaMovimentacao.MensageLog = Utilidades.String.Left(modeloA.LogMessage, 20);
                                novaMovimentacao.DataUpload = DateTime.Now;

                                novaMovimentacao.NumeroDocumento = Utilidades.String.Left(modeloA.InvoiceDocumentNumber, 20);
                                novaMovimentacao.Comments = Utilidades.String.Left(modeloA.Comments, 100);
                                novaMovimentacao.SituacaoProcessamento = SituacaoProcessamento.AguardandoProcessamento;
                                novaMovimentacao.Destinatario = repositorioCliente.BuscarPorCodigoIntegracao(modeloA.CNPJReceiver);
                                novaMovimentacao.Recebedor = repositorioCliente.BuscarPorCodigoIntegracao(modeloA.CNPJReceipt);
                                novaMovimentacao.Remetente = repositorioCliente.BuscarPorCodigoIntegracao(modeloA.CNPJSender);
                                novaMovimentacao.Expedidor = repositorioCliente.BuscarPorCodigoIntegracao(modeloA.CNPJDispatcher);

                                novaMovimentacao.TipoRegistro = contaPagar.TipoRegistro;
                                novaMovimentacao.ContaPagar = contaPagar;
                                repositorioMovimentacao.Inserir(novaMovimentacao);

                            }
                        }

                        if (errosArquivo.Count > 0)
                            throw new ServicoException("Error");

                        repositorioContaPagar.Atualizar(contaPagar);

                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                        continue;
                    }

                    if (contaPagar.TipoRegistro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistro.BaixaResultado)
                    {
                        Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                        using (var reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(localArquivo)))
                        using (var csv = new CsvReader(reader, configCsv))
                        {
                            var records = csv.GetRecords<Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloDMovimentoFinanceiro>();
                            int contador = 0;

                            foreach (var modeloD in records)
                            {
                                //if (contador == 0)
                                //{
                                //    contador++;
                                //    continue;
                                //}

                                contador++;

                                Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar novaMovimentacao = new Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar();

                                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigoIntegracao(modeloD.Vendor);

                                if (transportador == null)
                                {
                                    errosArquivo.Add($"linha {contador}: Transportador não encontrado. Código integração: {modeloD.Vendor}");
                                    continue;
                                }

                                if (errosArquivo.Count > 0)
                                    continue;

                                novaMovimentacao.Transportador = transportador;
                                novaMovimentacao.CodigoEmpresa = Utilidades.String.Left(modeloD.CompanyCode, 20);
                                novaMovimentacao.ParceiroComercial = Utilidades.String.Left(modeloD.TradingPartner, 20);
                                novaMovimentacao.GL = Utilidades.String.Left(modeloD.GLAccount, 20);
                                if (modeloD.DocumentDate.HasValue && modeloD.DocumentDate.Value != DateTime.MinValue)
                                    novaMovimentacao.DataDocumento = modeloD.DocumentDate;

                                novaMovimentacao.CentroLucro = Utilidades.String.Left(modeloD.ProfitCenter, 30);
                                novaMovimentacao.Texto = Utilidades.String.Left(modeloD.Text, 20);
                                novaMovimentacao.Segmento = Utilidades.String.Left(modeloD.Segment, 30);
                                novaMovimentacao.Ordem = Utilidades.String.Left(modeloD.Order, 30);
                                novaMovimentacao.CentroCusto = Utilidades.String.Left(modeloD.CostCenter, 30);
                                novaMovimentacao.Referencia = Utilidades.String.Left(modeloD.Reference, 30);
                                novaMovimentacao.ValorMonetario = modeloD.AmountInLC;
                                novaMovimentacao.NomeUsuario = Utilidades.String.Left(modeloD.UserName, 30);
                                novaMovimentacao.CodigoTaxa = Utilidades.String.Left(modeloD.TaxCode, 10);
                                novaMovimentacao.Moeda = Utilidades.String.Left(modeloD.Currency, 30);
                                novaMovimentacao.NumeroDocumento = Utilidades.String.Left(modeloD.DocumentNumber, 20);
                                novaMovimentacao.ClrngDoc = Utilidades.String.Left(modeloD.ClearingDocument, 30);
                                novaMovimentacao.DebitoCredito = Utilidades.String.Left(modeloD.DebitCredit, 30);
                                novaMovimentacao.SituacaoProcessamento = SituacaoProcessamento.AguardandoProcessamento;
                                novaMovimentacao.DataUpload = DateTime.Now;
                                novaMovimentacao.TipoRegistro = contaPagar.TipoRegistro;
                                novaMovimentacao.ContaPagar = contaPagar;
                                repositorioMovimentacao.Inserir(novaMovimentacao);

                            }
                        }

                        if (errosArquivo.Count > 0)
                            throw new ServicoException("Error");

                        repositorioContaPagar.Atualizar(contaPagar);
                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                        continue;
                    }

                }
                catch (ServicoException se)
                {
                    _unitOfWork.Rollback();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    errosArquivo.Add(ex.Message);
                }

                if (errosArquivo.Count > 0)
                {
                    _unitOfWork.Start();
                    var exiteContaPagar = repositorioContaPagar.BuscarPorCodigo(codigoConta, false);
                    exiteContaPagar.Situacao = SituacaoProcessamentoArquivo.ErroNoProcessamento;
                    exiteContaPagar.MensagemProcessamento = "Problemas ao tentar processar validar logs de erro";

                    if (exiteContaPagar.MensagensProcessamento == null || exiteContaPagar.MensagensProcessamento.Count == 0)
                        exiteContaPagar.MensagensProcessamento = new List<string>();

                    foreach (var erro in errosArquivo)
                        exiteContaPagar.MensagensProcessamento.Add(erro);

                    repositorioContaPagar.Atualizar(exiteContaPagar);

                    _unitOfWork.CommitChanges();
                    _unitOfWork.FlushAndClear();
                }
            }
        }

        public void ProcessarMovimentoAguardando()
        {
            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacao = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            try
            {
                List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacaoPendente = repositorioMovimentacao.BuscarPendentesProcessamento();

                foreach (var movimentacao in movimentacaoPendente)
                {
                    _unitOfWork.Start();
                    movimentacao.DataUpload = DateTime.Now;
                    movimentacao.SituacaoProcessamento = SituacaoProcessamento.Processado;

                    if (string.IsNullOrEmpty(movimentacao.ChaveReferencia))
                    {
                        movimentacao.ObservacaoMiro = "O campo Chave de referência não possui valor";
                        repositorioMovimentacao.Atualizar(movimentacao);
                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                        continue;
                    }

                    string numeroMiro = Utilidades.String.Left(movimentacao.ChaveReferencia, 10);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico existeCte = repositorioDocumentoFaturamento.BuscarCtePorNumeroMiro(numeroMiro);

                    if (existeCte == null)
                    {
                        movimentacao.ObservacaoMiro = "Não foi encontrado documento com este número de MIRO";
                        repositorioMovimentacao.Atualizar(movimentacao);
                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                        continue;
                    }

                    movimentacao.CTe = existeCte;
                    movimentacao.MiroRecebida = true;
                    movimentacao.NumeroCte = $"{existeCte?.Numero ?? 0}";
                    movimentacao.SerieCte = $"{existeCte?.Serie?.Numero ?? 0}";
                    movimentacao.ObservacaoMiro = "Registro vinculado com sucesso ao documento de frete";
                    movimentacao.SituacaoProcessamento = SituacaoProcessamento.Processado;

                    repositorioMovimentacao.Atualizar(movimentacao);
                    _unitOfWork.CommitChanges();
                    _unitOfWork.FlushAndClear();
                }
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                _unitOfWork.Rollback();
            }
        }
        #endregion

        #region Metodos Privados
        private Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloAMovimentoFinanceiro ParseFinancialData(string[] fields)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloAMovimentoFinanceiro financialData = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloAMovimentoFinanceiro();

            DateTime.TryParseExact(fields[11], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataResultado);
            DateTime.TryParseExact(fields[7], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime DueData);
            DateTime.TryParseExact(fields[6], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime documentoDate);
            DateTime.TryParseExact(fields[17], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataResultadoPost);

            decimal.TryParse(fields[9], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal amoulLc);

            financialData.CompanyCode = fields[0];
            financialData.Vendor = fields[1];
            financialData.Reference = fields[2];
            financialData.PmntBlock = fields[3];
            financialData.PostingKey = fields[4];
            financialData.TaxCode = fields[5];
            financialData.DocumentType = fields[8];
            financialData.AmountInLC = amoulLc;
            financialData.SpecialGLIndicator = fields[10];
            financialData.DocumentDate = documentoDate;
            financialData.DueDate = DueData;
            financialData.Clearing = dataResultado;
            financialData.ClearingDocument = fields[12];
            financialData.Text = fields[13];
            financialData.DocumentNumber = fields[14];
            financialData.UserName = fields[15];
            financialData.InvoiceReference = fields.Count() >= 17 ? fields[16] : "";
            financialData.PostingDate = dataResultadoPost;
            financialData.Assignment = fields[18];
            financialData.DiscountBase = decimal.Parse(fields[19]);
            financialData.Currency = fields[20];
            financialData.PaymentTerms = fields[21];
            financialData.DocumentHeaderText = fields[22];
            financialData.ReferenceKey = fields[23];

            return financialData;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloBMovimentoFinanceiro ParseVendorData(string inputText)
        {
            string[] fields = inputText.Split('|');
            Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloBMovimentoFinanceiro novaMovimentacao = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloBMovimentoFinanceiro();
            decimal.TryParse(fields[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal totalValue);
            decimal.TryParse(fields[4], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal amoulLc);

            DateTime.TryParseExact(fields[5], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data);

            novaMovimentacao.Vendor = fields[0];
            novaMovimentacao.Name = fields[1];
            novaMovimentacao.TotalValue = totalValue;
            novaMovimentacao.Reference = fields[3];
            novaMovimentacao.AmountInLC = amoulLc;
            novaMovimentacao.Clearing = data;
            novaMovimentacao.ClearingDocument = fields.Count() >= 7 ? fields[6] : "";
            novaMovimentacao.MaterialDocument = fields.Count() >= 8 ? fields[7] : "";
            novaMovimentacao.PurchasingDocument = fields.Count() >= 9 ? fields[8] : "";
            novaMovimentacao.DocumentType = fields.Count() >= 10 ? fields[9] : "";

            return novaMovimentacao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloCMovimentoFinanceiro ParseCteData(string inputText)
        {
            string[] fields = inputText.Split('|');
            Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloCMovimentoFinanceiro movimento = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloCMovimentoFinanceiro();

            DateTime? PostDate = fields[1].ToDateTime("yyyyMMdd");

            DateTime? dueData = null;
            if (fields.Count() >= 25)
                dueData = fields[24].ToDateTime("yyyyMMdd");

            DateTime? UploadDate = null;
            if (fields.Count() >= 27)
                UploadDate = fields[26].ToDateTime("yyyyMMdd");

            DateTime? TaxApprovalDate = null;
            if (fields.Count() >= 29)
                TaxApprovalDate = fields[28].ToDateTime("yyyyMMdd");

            decimal.TryParse(fields[6], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal NetAmout);
            decimal.TryParse(fields[7], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal BaseAmount);

            movimento.AccessKey = fields[0];
            movimento.PostingDate = PostDate;
            movimento.CteNumber = fields[2];
            movimento.CteSeries = fields[3];
            movimento.CompanyCode = fields[4];
            movimento.CteStatus = fields[5];
            movimento.NetAmount = NetAmout;
            movimento.BaseAmount = BaseAmount;
            movimento.Currency = fields[8];
            movimento.ProtocolNumber = fields[9];
            movimento.InvoiceDocumentNumber = fields.Count() >= 11 ? fields[10] : "";
            movimento.CNPJIssuer = fields.Count() >= 12 ? fields[11] : "";
            movimento.IssuerName = fields.Count() >= 13 ? fields[12] : "";
            movimento.Vendor = fields.Count() >= 12 ? fields[13] : "";
            movimento.City = fields.Count() >= 15 ? fields[14] : "";
            movimento.Region = fields.Count() >= 16 ? fields[15] : "";
            movimento.CNPJSender = fields.Count() >= 17 ? fields[16] : "";
            movimento.SenderName = fields.Count() >= 18 ? fields[17] : "";
            movimento.CNPJReceiver = fields.Count() >= 19 ? fields[18] : "";
            movimento.ReceiverName = fields.Count() >= 20 ? fields[19] : "";
            movimento.CNPJDispatcher = fields.Count() >= 21 ? fields[20] : "";
            movimento.DispatcherName = fields.Count() >= 22 ? fields[21] : "";
            movimento.CNPJReceipt = fields.Count() >= 23 ? fields[22] : "";
            movimento.ReceiptName = fields.Count() >= 24 ? fields[23] : "";
            movimento.DueDate = dueData;
            movimento.LogMessage = fields.Count() >= 26 ? fields[25] : "";
            movimento.UploadDate = UploadDate;
            movimento.Comments = fields.Count() >= 28 ? fields[27] : "";
            movimento.TaxApprovalDate = TaxApprovalDate;
            movimento.Comment = fields.Count() >= 30 ? fields[29] : "";

            return movimento;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloDMovimentoFinanceiro ParseFinancialTransaction(string inputText)
        {
            string[] fields = inputText.Split('|');
            Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloDMovimentoFinanceiro financialTransaction = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ModeloDMovimentoFinanceiro();

            DateTime? PostDate = null;
            if (fields.Count() >= 5)
                PostDate = fields[5].ToDateTime("yyyyMMdd");

            decimal Amount = 0;
            if (fields.Count() >= 13)
                decimal.TryParse(fields[12], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Amount);

            financialTransaction.Name = fields[0];
            financialTransaction.Vendor = fields.Count() >= 2 ? fields[1] : "";
            financialTransaction.CompanyCode = fields.Count() >= 3 ? fields[2] : "";
            financialTransaction.TradingPartner = fields.Count() >= 4 ? fields[3] : "";
            financialTransaction.GLAccount = fields[4];
            financialTransaction.DocumentDate = PostDate;
            financialTransaction.ProfitCenter = fields.Count() >= 7 ? fields[6] : "";
            financialTransaction.Text = fields.Count() >= 8 ? fields[7] : "";
            financialTransaction.Segment = fields.Count() >= 9 ? fields[8] : "";
            financialTransaction.Order = fields.Count() >= 10 ? fields[9] : "";
            financialTransaction.CostCenter = fields.Count() >= 11 ? fields[10] : "";
            financialTransaction.Reference = fields.Count() >= 12 ? fields[11] : "";
            financialTransaction.AmountInLC = Amount;
            financialTransaction.UserName = fields.Count() >= 14 ? fields[13] : "";
            financialTransaction.TaxCode = fields.Count() >= 15 ? fields[14] : "";
            financialTransaction.Currency = fields.Count() >= 16 ? fields[15] : "";
            financialTransaction.DocumentNumber = fields.Count() >= 17 ? fields[16] : "";
            financialTransaction.ClearingDocument = fields.Count() >= 18 ? fields[17] : "";
            financialTransaction.DebitCredit = fields.Count() >= 19 ? fields[18] : "";

            return financialTransaction;
        }
        #endregion
    }
}