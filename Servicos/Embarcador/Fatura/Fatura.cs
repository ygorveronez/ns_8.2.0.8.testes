using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Fatura
{
    public class Fatura : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Fatura.Fatura,
        Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura,
        Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura
    >
    {
        #region Construtores

        public Fatura(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public void InserirLog(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeDeTrabalho, TipoLogFatura acao, Dominio.Entidades.Usuario usuario, string emailDestino = "")
        {
            Repositorio.Embarcador.Fatura.FaturaLog repFaturaLog = new Repositorio.Embarcador.Fatura.FaturaLog(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Fatura.FaturaLog log = new Dominio.Entidades.Embarcador.Fatura.FaturaLog();

            log.Fatura = fatura;
            log.DataHora = DateTime.Now;
            log.TipoLogFatura = acao;
            log.Usuario = usuario;
            log.EmailDestinoEDI = emailDestino;

            repFaturaLog.Inserir(log);
        }

        public void AtualizarValorVencimento(DateTime dataVencimento, int codigoFatura, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (dataVencimento == DateTime.MinValue)
                return;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentos = repDocumentoFaturamento.BuscarPorFatura(codigoFatura);
            if (documentos != null && documentos.Count > 0)
            {
                foreach (var doc in documentos)
                {
                    doc.DataVencimento = dataVencimento;
                    repDocumentoFaturamento.Atualizar(doc);
                }
            }
        }

        public dynamic RetornaObjetoCompletoFatura(int codigo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo);

            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> faturasIntegracao = repFaturaIntegracao.BuscarPorFatura(codigo);

            bool possuiFaturaProblemaIntegracao = false;
            if (faturasIntegracao != null)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao ultimaFaturaIntegracao = faturasIntegracao.LastOrDefault();
                possuiFaturaProblemaIntegracao = ultimaFaturaIntegracao != null && ultimaFaturaIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao;
            }

            return new
            {
                ListaCargas = fatura.Cargas != null ? (from obj in fatura.Cargas
                                                       orderby obj.Carga.CodigoCargaEmbarcador
                                                       select new
                                                       {
                                                           obj.Codigo,
                                                           obj.DescricaoStatus,
                                                           obj.StatusFaturaCarga,
                                                           CodigoCarga = obj.Carga.Codigo,
                                                           DataCarga = obj.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy"),
                                                           ValorFreteAPagar = obj.Carga.ValorFreteAPagar.ToString("n2"),
                                                           obj.Carga.CodigoCargaEmbarcador
                                                       }).ToList() : null,
                Pessoa = fatura.Cliente != null ? new { Codigo = fatura.Cliente.Codigo, Descricao = fatura.Cliente.Descricao } : null,
                NomePessoa = fatura.Cliente != null ? fatura.Cliente.Nome : string.Empty,
                fatura.Codigo,
                fatura.NovoModelo,
                fatura.GerarDocumentosAutomaticamente,
                fatura.GerarDocumentosApenasCanhotosAprovados,
                DataFatura = fatura.DataFatura.ToString("dd/MM/yyyy"),
                DataFinal = fatura.DataFinal.HasValue ? fatura.DataFinal.Value.ToString("dd/MM/yyyy") : "",
                DataInicial = fatura.DataInicial.HasValue ? fatura.DataInicial.Value.ToString("dd/MM/yyyy") : "",
                fatura.DescricaoEtapa,
                fatura.DescricaoPeriodo,
                Transportador = new { Descricao = fatura.Transportador?.Descricao ?? "", Codigo = fatura.Transportador?.Codigo ?? 0 },
                DescricaoSituacao = possuiFaturaProblemaIntegracao ? "Problema com a Integração" : fatura.DescricaoSituacao,
                fatura.Etapa,
                TipoCarga = new { Codigo = fatura.TipoCarga?.Codigo ?? 0, Descricao = fatura.TipoCarga?.Descricao ?? string.Empty },
                PedidoViagemNavio = new { Codigo = fatura.PedidoViagemNavio?.Codigo ?? 0, Descricao = fatura.PedidoViagemNavio?.Descricao ?? string.Empty },
                TerminalOrigem = new { Codigo = fatura.TerminalOrigem?.Codigo ?? 0, Descricao = fatura.TerminalOrigem?.Descricao ?? string.Empty },
                TerminalDestino = new { Codigo = fatura.TerminalDestino?.Codigo ?? 0, Descricao = fatura.TerminalDestino?.Descricao ?? string.Empty },
                Origem = new { Codigo = fatura.Origem?.Codigo ?? 0, Descricao = fatura.Origem?.Descricao ?? string.Empty },
                PaisOrigem = new { Codigo = fatura.PaisOrigem?.Codigo ?? 0, Descricao = fatura.PaisOrigem?.Descricao ?? string.Empty },
                Filial = new { Codigo = fatura.Filial?.Codigo ?? 0, Descricao = fatura.Filial?.Descricao ?? string.Empty },
                Destino = new { Codigo = fatura.Destino?.Codigo ?? 0, Descricao = fatura.Destino?.Descricao ?? string.Empty },
                Veiculo = new { Codigo = fatura.Veiculo?.Codigo ?? 0, Descricao = fatura.Veiculo?.Descricao ?? string.Empty },
                fatura.NumeroBooking,
                fatura.GeradoPorFaturaLote,
                TipoPropostaMultimodal = fatura.TipoPropostaMultimodal.Select(o => o).ToList(),
                fatura.AliquotaICMS,
                TipoOperacao = fatura.TipoOperacao != null ? new { fatura.TipoOperacao.Codigo, fatura.TipoOperacao.Descricao } : null,
                Transportadora = fatura.Transportadora != null ? new { Codigo = fatura.Transportadora.Codigo, Descricao = fatura.Transportadora.RazaoSocial } : null,
                GrupoPessoa = fatura.GrupoPessoas != null ? new { Codigo = fatura.GrupoPessoas.Codigo, Descricao = fatura.GrupoPessoas.Descricao } : null,
                DescricaoGrupoPessoa = fatura.GrupoPessoas != null ? fatura.GrupoPessoas.Descricao : string.Empty,
                fatura.ImprimeObservacaoFatura,
                Numero = fatura.NumeroFaturaIntegracao > 0 ? fatura.NumeroFaturaIntegracao : fatura.Numero,
                NumeroFatura = fatura.NumeroFaturaIntegracao > 0 ? fatura.NumeroFaturaIntegracao : fatura.Numero,
                NumeroPreFatura = fatura.NumeroPreFatura,
                fatura.NumeroFaturaOriginal,
                fatura.Observacao,
                fatura.ObservacaoFatura,
                ListaParcelas = fatura.Parcelas != null ? (from obj in fatura.Parcelas
                                                           orderby obj.Sequencia
                                                           select new
                                                           {
                                                               obj.Codigo,
                                                               Acrescimo = obj.Acrescimo.ToString("n2"),
                                                               DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                                               DataVencimento = obj.DataVencimento.ToString("dd/MM/yyyy"),
                                                               Desconto = obj.Desconto.ToString("n2"),
                                                               obj.DescricaoSituacao,
                                                               obj.Sequencia,
                                                               obj.SituacaoFaturaParcela,
                                                               Valor = obj.Valor.ToString("n2")
                                                           }).ToList() : null,
                Situacao = possuiFaturaProblemaIntegracao ? SituacaoFatura.ProblemaIntegracao : fatura.Situacao,
                fatura.TipoPessoa,
                Total = fatura.Total.ToString("n2"),
                TomadorFatura = fatura.ClienteTomadorFatura != null ? new { Codigo = fatura.ClienteTomadorFatura.CPF_CNPJ, Descricao = fatura.ClienteTomadorFatura.Descricao } : null,
                EmpresaFatura = fatura.Empresa != null ? new { Codigo = fatura.Empresa.Codigo, Descricao = fatura.Empresa.RazaoSocial } : null,
                Banco = fatura.Banco != null ? new { Codigo = fatura.Banco.Codigo, Descricao = fatura.Banco.Descricao } : null,
                Agencia = fatura.Agencia,
                Digito = fatura.DigitoAgencia,
                NumeroConta = fatura.NumeroConta,
                TipoConta = fatura.TipoContaBanco,
                fatura.MotivoCancelamento,
                fatura.NaoUtilizarMoedaEstrangeira,
                Tomador = fatura.Tomador != null ? new { Codigo = fatura.Tomador.Codigo, Descricao = fatura.Tomador.Descricao } : null,
                CentroDeResultado = fatura.CentroResultado != null ? new { Codigo = fatura.CentroResultado.Codigo, Descricao = fatura.CentroResultado.Descricao } : null,
                TipoOSConvertido = fatura.TiposOSConvertidos.Select(o => (int)o).ToList(),
                DataBaseCRT = fatura.DataBaseCRT?.ToString("dd/MM/yyyy HH:mm"),
                DataBaseAtual = DateTime.Now.ToDateTimeString()
            };
        }

        public bool InserirCargaFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, string stringConexao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao(stringConexao, cliente, tipoServicoMultisoftware, adminStringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<KeyValuePair<int, int>> cargaCTesFatura = repCarga.BuscarCodigosCargasCTeSemFatura(fatura);

                if (cargaCTesFatura.Count > 0)
                {
                    int totalConhecimentos = cargaCTesFatura.Count;
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);

                    for (int i = 0; i < totalConhecimentos; i++)
                    {
                        unitOfWork.Start();
                        int codigoCTe = cargaCTesFatura[i].Key;
                        int codigoCarga = cargaCTesFatura[i].Value;
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                        //if (!repFaturaCargaDocumento.ContemDocumentoFatura(fatura.Codigo, codigoCTe, TipoDocumentoFatura.Conhecimento))
                        if (cte.Fatura == null)
                        {
                            Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento faturaDocumento = new Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento();
                            faturaDocumento.Carga = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = codigoCarga };
                            faturaDocumento.ConhecimentoDeTransporteEletronico = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = codigoCTe };
                            faturaDocumento.Fatura = fatura;
                            faturaDocumento.NFSe = null;
                            faturaDocumento.NumeroFatura = fatura.Numero;
                            faturaDocumento.StatusDocumentoFatura = StatusDocumentoFatura.Normal;
                            faturaDocumento.TipoDocumentoFatura = TipoDocumentoFatura.Conhecimento;

                            repFaturaCargaDocumento.Inserir(faturaDocumento);

                            cte.Fatura = fatura;
                            repCTe.Atualizar(cte);
                            unitOfWork.CommitChanges();
                        }

                        int processados = 100 * i / totalConhecimentos;
                        serNotificacao.InfomarPercentualProcessamento(usuario, fatura.Codigo, "Faturas/Fatura", processados, TipoNotificacao.fatura, tipoServicoMultisoftware, unitOfWork);

                        if (i % 25 == 0)
                        {
                            unitOfWork.FlushAndClear();
                        }
                    }

                    List<int> listaCarga = (from obj in cargaCTesFatura select obj.Value).Distinct().ToList();
                    Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);

                    for (int i = 0; i < listaCarga.Count(); i++)
                    {
                        unitOfWork.Start();
                        if (!repFaturaCarga.ContemCargaoFatura(fatura.Codigo, listaCarga[i]))
                        {
                            Dominio.Entidades.Embarcador.Fatura.FaturaCarga faturaCarga = new Dominio.Entidades.Embarcador.Fatura.FaturaCarga();
                            faturaCarga.Fatura = fatura;
                            faturaCarga.Carga = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = listaCarga[i] };
                            faturaCarga.StatusFaturaCarga = StatusFaturaCarga.Faturada;

                            repFaturaCarga.Inserir(faturaCarga);

                            AtualizaStatusFaturaCarga(listaCarga[i], fatura.Codigo, unitOfWork);
                            unitOfWork.CommitChanges();
                        }
                    }
                }

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario user = repUsuario.BuscarPorCodigo(usuario.Codigo);
                serNotificacao.GerarNotificacao(user, fatura.Codigo, "Faturas/Fatura", Localization.Resources.Configuracao.Fatura.GeracaoCargasFaturasConcluidas, IconesNotificacao.sucesso, TipoNotificacao.fatura, tipoServicoMultisoftware, unitOfWork);

                return true;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                serNotificacao.GerarNotificacao(usuario, fatura.Codigo, "Faturas/Fatura", string.Format(Localization.Resources.Configuracao.Fatura.OcorreuFalhaGerarCargasFuturas, ex.Message), IconesNotificacao.falha, TipoNotificacao.fatura, tipoServicoMultisoftware, unitOfWork);
                return false;
            }
            finally
            {
                //unitOfWork.Dispose();
            }

        }

        public static bool AdicionarDocumentoNaFatura(out string erro, ref Dominio.Entidades.Embarcador.Fatura.Fatura fatura, int codigoDocumento, decimal valorACobrar, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, bool controlarTransacao = false)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigo(codigoDocumento);

            if (repFaturaDocumento.ExisteNaFatura(fatura.Codigo, codigoDocumento))
            {
                erro = $"O documento ({documentoFaturamento.CTe?.ChaveAcesso ?? string.Empty}) já existe nesta fatura.";
                return false;
            }

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = repFaturaDocumento.ObterMoedaFatura(fatura.Codigo);
            if ((moeda.HasValue && moeda != documentoFaturamento.Moeda) || ((!moeda.HasValue || moeda == MoedaCotacaoBancoCentral.Real) && (moeda.HasValue && documentoFaturamento.Moeda != MoedaCotacaoBancoCentral.Real)))
            {
                erro = $"A moeda do documento ({documentoFaturamento.CTe?.ChaveAcesso ?? string.Empty}) difere da moeda dos demais documentos da fatura.";
                return false;
            }

            if (documentoFaturamento.Situacao != SituacaoDocumentoFaturamento.Autorizado)
            {
                erro = $"O documento ({documentoFaturamento.CTe?.ChaveAcesso ?? string.Empty}) não está autorizado, não sendo possível adicionar à fatura.";
                return false;
            }

            if (documentoFaturamento.ValorAFaturar < valorACobrar)
            {
                erro = $"O valor à cobrar do documento ({documentoFaturamento.CTe?.ChaveAcesso ?? string.Empty}) é maior que o valor disponível para faturar ({documentoFaturamento.ValorAFaturar.ToString("n2")}).";
                return false;
            }


            decimal valorTotalEmFatura = repFaturaDocumento.ObterValorTotalEmFatura(codigoDocumento);

            if (documentoFaturamento.ValorDocumento < valorTotalEmFatura)
            {
                erro = $"O valor do documento {documentoFaturamento.Numero} ({documentoFaturamento.CTe?.ChaveAcesso ?? string.Empty}) ({documentoFaturamento.ValorDocumento.ToString("n2")}) é menor que o valor total em faturas ({valorTotalEmFatura.ToString("n2")}), não sendo possível adicionar.";
                return false;
            }

            if (controlarTransacao)
                unidadeTrabalho.Start();

            Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = new Dominio.Entidades.Embarcador.Fatura.FaturaDocumento();

            faturaDocumento.Fatura = fatura;
            faturaDocumento.Documento = documentoFaturamento;

            bool descontarValorDesseDocumentoFatura = faturaDocumento.Documento.ModeloDocumentoFiscal != null
                && faturaDocumento.Documento.ModeloDocumentoFiscal.DescontarValorDesseDocumentoFatura
                && faturaDocumento.Documento.CargaOcorrenciaPagamento.ComponenteFrete.DescontarValorTotalAReceber;

            faturaDocumento.AcrescentarOuDescontarValorDesseDocumentoFatura = descontarValorDesseDocumentoFatura;

            if (valorACobrar == 0m)
                faturaDocumento.ValorACobrar = documentoFaturamento.ValorDocumento - documentoFaturamento.ValorEmFatura;
            else
                faturaDocumento.ValorACobrar = valorACobrar;

            if (descontarValorDesseDocumentoFatura && faturaDocumento.Documento.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito)
                faturaDocumento.ValorACobrar = faturaDocumento.ValorACobrar * (-1);

            faturaDocumento.ValorTotalACobrar = faturaDocumento.ValorACobrar;

            repFaturaDocumento.Inserir(faturaDocumento);

            if (Auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Adicionou o documento " + documentoFaturamento.Numero.ToString() + " a fatura.", unidadeTrabalho);

            documentoFaturamento.ValorEmFatura += faturaDocumento.ValorACobrar;
            documentoFaturamento.ValorAFaturar -= faturaDocumento.ValorACobrar;
            documentoFaturamento.Fatura = faturaDocumento.Fatura;

            repDocumentoFaturamento.Atualizar(documentoFaturamento);

            if (controlarTransacao)
                unidadeTrabalho.CommitChanges();

            erro = string.Empty;
            return true;
        }

        public void InserirDocumentosFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente, tipoServicoMultisoftware, adminStringConexao);

            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtroDocumentosParaFatura = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
            {
                CodigoFatura = fatura.Codigo,
                DataInicial = fatura.DataInicial,
                DataFinal = fatura.DataFinal,
                CodigoCarga = fatura.Carga?.Codigo ?? 0,
                NumeroDocumento = "",
                CodigoGrupoPessoas = fatura.GrupoPessoas?.Codigo ?? 0,
                CPFCNPJTomador = fatura.Cliente?.CPF_CNPJ ?? 0d,
                TipoOperacao = fatura.TipoOperacao?.Codigo ?? 0,
                Empresa = fatura.Transportador?.Codigo ?? 0,
                TipoCarga = fatura.TipoCarga?.Codigo ?? 0,
                AliquotaICMS = fatura.AliquotaICMS,
                PedidoViagemNavio = fatura.PedidoViagemNavio?.Codigo ?? 0,
                TerminalOrigem = fatura.TerminalOrigem?.Codigo ?? 0,
                TerminalDestino = fatura.TerminalDestino?.Codigo ?? 0,
                Origem = fatura.Origem?.Codigo ?? 0,
                Destino = fatura.Destino?.Codigo ?? 0,
                NumeroBooking = fatura.NumeroBooking,
                TipoPropostaMultimodal = fatura.TipoPropostaMultimodal?.ToList(),
                TiposPropostasMultimodal = null,
                CodigoMDFe = fatura.MDFe?.Codigo ?? 0,
                CodigoContainer = fatura.Container?.Codigo ?? 0,
                NumeroControleCliente = fatura.NumeroControleCliente,
                NumeroReferenciaEDI = fatura.NumeroReferenciaEDI,
                CodigoCTe = fatura.CTe?.Codigo ?? 0,
                CodigoVeiculo = fatura.Veiculo?.Codigo ?? 0,
                IETomador = fatura.IETomador,
                PaisOrigem = fatura.PaisOrigem?.Codigo ?? 0,
                TipoCTe = fatura.TipoCTe.HasValue ? fatura.TipoCTe.Value : Dominio.Enumeradores.TipoCTE.Todos,
                CodigosCTes = fatura.FaturaLoteCTes != null && fatura.FaturaLoteCTes.Count > 0 ? fatura.FaturaLoteCTes.Select(c => c.CTe.Codigo).ToList() : null,
                ApenasFaturaExclusiva = fatura.FaturamentoExclusivo,
                HabilitarOpcaoGerarFaturasApenasCanhotosAprovados = fatura.GerarDocumentosApenasCanhotosAprovados,
                Filial = fatura.Filial?.Codigo ?? 0,
                TiposOSConvertidos = fatura.TiposOSConvertidos.ToList(),
                GerarDocumentosApenasCanhotosAprovados = fatura.GerarDocumentosApenasCanhotosAprovados,
            };

            List<int> codigosDocumentosParaFatura = repDocumentoFaturamento.ConsultarCodigosDocumentosParaFatura(filtroDocumentosParaFatura, 0, 30000);

            int totalDocumentos = codigosDocumentosParaFatura.Count;

            for (int i = 0; i < totalDocumentos; i++)
            {
                unitOfWork.Start();

                AdicionarDocumentoNaFatura(out string erro, ref fatura, codigosDocumentosParaFatura[i], 0m, unitOfWork);

                unitOfWork.CommitChanges();

                int processados = 100 * i / totalDocumentos;
                serNotificacao.InfomarPercentualProcessamento(usuario, fatura.Codigo, "Faturas/Fatura", processados, TipoNotificacao.fatura, tipoServicoMultisoftware, unitOfWork);

                if (i % 25 == 0)
                {
                    unitOfWork.FlushAndClear();
                }
            }

            fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

            unitOfWork.Start();

            AtualizarTotaisFatura(ref fatura, unitOfWork);

            fatura.Etapa = EtapaFatura.Documentos;

            repFatura.Atualizar(fatura);

            unitOfWork.CommitChanges();

            if (fatura.Usuario != null)
                serNotificacao.GerarNotificacao(fatura.Usuario, fatura.Codigo, "Faturas/Fatura", Localization.Resources.Configuracao.Fatura.GeracaoCargasFaturasConcluidas, IconesNotificacao.sucesso, TipoNotificacao.fatura, tipoServicoMultisoftware, unitOfWork);
        }

        public dynamic RetornaObjetoResumoCargaFatura(int codigo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigo);

            return new
            {
                Codigo = fatura.Codigo,
                QuantidadeCargas = fatura.Cargas != null ? fatura.Cargas.Count().ToString("n0") : 0.ToString("n0"),
                QuantidadeCargasFaturadas = fatura.Cargas != null ? (from obj in fatura.Cargas
                                                                     where obj.StatusFaturaCarga == StatusFaturaCarga.Faturada
                                                                     select obj).Count().ToString("n0") : 0.ToString("n0"),
                QuantidadeCargasParcialmenteFaturadas = fatura.Cargas != null ? (from obj in fatura.Cargas
                                                                                 where obj.StatusFaturaCarga == StatusFaturaCarga.FaturadaParcial
                                                                                 select obj).Count().ToString("n0") : 0.ToString("n0"),
                QuantidadeDocumentos = repFaturaCarga.QuantidadeDocumentosCarga(fatura.Codigo, fatura).ToString("n0"),
                QuantidadeDocumentosFaturados = repFaturaCarga.QuantidadeDocumentosFaturadosCarga(fatura.Codigo, fatura).ToString("n0"),
                QuantidadeDocumentosParcialmenteFaturados = repFaturaCarga.QuantidadeDocumentosParcialmenteFaturadosCarga(fatura.Codigo).ToString("n0"),
                ValorFaturar = repFaturaCarga.ValorConhecimentos(fatura.Codigo).ToString("n2")
            };
        }

        public void AtualizaStatusFaturaCarga(int codigoCarga, int codigoFatura, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeDeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

            int quantidadeDocumentosExcuidos = repFaturaCarga.QuantidadeDocumentosParcialmenteFaturadosCarga(codigoFatura, codigoCarga);
            int quantidadeTotalDocumentoNormal = repFaturaCarga.QuantidadeDocumentosCarga(codigoFatura, codigoCarga, fatura);
            int quantidadeTotalDocumentosCarga = repFaturaCarga.QuantidadeTotalDocumentosCarga(codigoFatura, codigoCarga);

            Dominio.Entidades.Embarcador.Fatura.FaturaCarga faturaCarga = repFaturaCarga.BuscarPorCargaFatura(codigoCarga, codigoFatura);
            if (quantidadeDocumentosExcuidos == 0 && quantidadeTotalDocumentoNormal > 0 && quantidadeTotalDocumentosCarga == quantidadeTotalDocumentoNormal)
                faturaCarga.StatusFaturaCarga = StatusFaturaCarga.Faturada;
            else if (quantidadeDocumentosExcuidos == quantidadeTotalDocumentosCarga)
                faturaCarga.StatusFaturaCarga = StatusFaturaCarga.NaoFaturada;
            else
                faturaCarga.StatusFaturaCarga = StatusFaturaCarga.FaturadaParcial;

            repFaturaCarga.Atualizar(faturaCarga);

            fatura.Total = repFaturaCarga.ValorConhecimentos(codigoFatura);

            repFatura.Atualizar(fatura);
        }

        public static void AtualizarTotaisFatura(ref Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);

            fatura.Total = repFaturaDocumento.ObterTotalACobrar(fatura.Codigo);
            fatura.Acrescimo = repFaturaDocumento.ObterTotalAcrescimo(fatura.Codigo);
            fatura.Desconto = repFaturaDocumento.ObterTotalDesconto(fatura.Codigo);
            fatura.TotalLiquido = repFaturaDocumento.ObterTotalACobrarLiquido(fatura.Codigo);

            if (!fatura.NaoUtilizarMoedaEstrangeira)
            {
                fatura.MoedaCotacaoBancoCentral = repFaturaDocumento.ObterMoedaFatura(fatura.Codigo);
                fatura.TotalMoedaEstrangeira = repFaturaDocumento.ObterTotalMoedaEstrangeira(fatura.Codigo);
            }

            if (fatura.DataInicial.HasValue && fatura.DataInicial.Value == DateTime.MinValue)
                fatura.DataInicial = null;

            if (fatura.DataFinal.HasValue && fatura.DataFinal.Value == DateTime.MinValue)
                fatura.DataFinal = null;

            repFatura.Atualizar(fatura);
        }

        public void CancelarTitulosBoletos(int codigoFatura, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Servicos.Embarcador.Financeiro.BoletoRemessa svcBoletoRemessa = new Servicos.Embarcador.Financeiro.BoletoRemessa(unidadeDeTrabalho);

            if (repTitulo.ContemTitulosPagosFatura(codigoFatura))
            {
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.RetornarTitulosPagosFatura(codigoFatura);
                foreach (var titulo in titulos)
                {
                    titulo.Initialize();
                    titulo.StatusTitulo = StatusTitulo.Cancelado;
                    repTitulo.Atualizar(titulo);

                    new Servicos.Embarcador.Integracao.IntegracaoTitulo(unidadeDeTrabalho).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Cancelamento);


                    Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Cancelou a baixa do título automaticamente pela fatura.", unidadeDeTrabalho);
                }
            }

            if (repTitulo.ContemBoletosFatura(codigoFatura))
            {
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.RetornarBoletosFatura(codigoFatura);

                svcBoletoRemessa.GerarRemessaDeCancelamento(titulos, unidadeDeTrabalho, Auditado, empresa);

                foreach (var titulo in titulos)
                {
                    titulo.Initialize();
                    titulo.NossoNumero = string.Empty;
                    titulo.BoletoRemessa = null;
                    repTitulo.Atualizar(titulo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Cancelou o boleto do título automaticamente pela fatura.", unidadeDeTrabalho);
                }
            }

            if (repTituloBaixa.BuscarPorFatura(codigoFatura) != null)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixa> baixas = repTituloBaixa.RetornarrPorFatura(codigoFatura);
                foreach (var baixa in baixas)
                {
                    baixa.Initialize();
                    baixa.SituacaoBaixaTitulo = SituacaoBaixaTitulo.Cancelada;
                    repTituloBaixa.Atualizar(baixa);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, baixa, null, "Cancelou a baixa automaticamente pela fatura.", unidadeDeTrabalho);
                }
            }
        }

        public void RetornarParametrosFaturamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho, bool faturamentoMultimodal, out int? diaMes, out DiaSemana? diaSemana, out bool? permiteFinalSemana, out int? diasPrazoFatura, out List<DiaSemana> diasSemana, out List<int> diasMes, out TipoPrazoFaturamento? tipoPrazoFatura, int codigoCarga, out FormaTitulo formaTitulo)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordo = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = null;
            if (codigoCarga > 0)
                primeiroCargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(codigoCarga);
            else
                primeiroCargaPedido = repFaturaDocumento.BuscarPrimeiroCargaPedidoPorCTe(cte.Codigo);

            diasPrazoFatura = 0;
            diaMes = 0;
            permiteFinalSemana = false;
            diaSemana = null;
            diasSemana = new List<DiaSemana>();
            diasMes = new List<int>();
            tipoPrazoFatura = null;
            formaTitulo = FormaTitulo.Outros;

            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoCliente = null;
            if (cte.TomadorPagador != null)
            {
                acordoCliente = repAcordo.BuscarAcordoCliente(cte.TomadorPagador.Cliente.CPF_CNPJ, 0);
                if (acordoCliente == null && cte.TomadorPagador.GrupoPessoas != null)
                    acordoCliente = repAcordo.BuscarAcordoCliente(0, cte.TomadorPagador.GrupoPessoas.Codigo);
            }
            if (acordoCliente == null && cte.TomadorPagador.GrupoPessoas != null)
                acordoCliente = repAcordo.BuscarAcordoCliente(0, cte.TomadorPagador.GrupoPessoas.Codigo);

            if (acordoCliente != null)
            {
                if (primeiroCargaPedido != null)
                {
                    if (primeiroCargaPedido.Carga.TipoOperacao != null && primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado)
                    {
                        if (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                        {
                            diasPrazoFatura = acordoCliente.LongoCursoDiasDePrazoFatura;
                            tipoPrazoFatura = acordoCliente.LongoCursoTipoPrazoFaturamento;
                            diasSemana = acordoCliente.LongoCursoDiasSemanaFatura != null ? acordoCliente.LongoCursoDiasSemanaFatura.ToList() : null;
                            diasMes = acordoCliente.LongoCursoDiasMesFatura != null ? acordoCliente.LongoCursoDiasMesFatura.ToList() : null;
                        }
                        if (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                        {
                            diasPrazoFatura = acordoCliente.CustoExtraDiasDePrazoFatura;
                            tipoPrazoFatura = acordoCliente.CustoExtraTipoPrazoFaturamento;
                            diasSemana = acordoCliente.CustoExtraDiasSemanaFatura != null ? acordoCliente.CustoExtraDiasSemanaFatura.ToList() : null;
                            diasMes = acordoCliente.CustoExtraDiasMesFatura != null ? acordoCliente.CustoExtraDiasMesFatura.ToList() : null;
                        }
                    }
                    else if (primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.DemurrageCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.DetentionCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.TakePayFeeder || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.TAkePayCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.NoShowCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.NotaDebito)
                    {
                        if (acordoCliente.ConsiderarParametrosDeFreteCabotagem)
                        {
                            diasPrazoFatura = acordoCliente.CabotagemDiasDePrazoFatura;
                            tipoPrazoFatura = acordoCliente.CabotagemTipoPrazoFaturamento;
                            diasSemana = acordoCliente.CabotagemDiasSemanaFatura != null ? acordoCliente.CabotagemDiasSemanaFatura.ToList() : null;
                            diasMes = acordoCliente.CabotagemDiasMesFatura != null ? acordoCliente.CabotagemDiasMesFatura.ToList() : null;
                        }
                        else
                        {
                            bool usarDiasPrazoFaturamentoDnD = primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.DemurrageCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.DetentionCabotagem;

                            diasPrazoFatura = usarDiasPrazoFaturamentoDnD ? acordoCliente.DiasPrazoFaturamentoDnD : acordoCliente.TakeOrPayDiasDePrazoFatura;
                            tipoPrazoFatura = TipoPrazoFaturamento.DataFatura;
                            diasSemana = null;
                            diasMes = null;
                        }
                    }
                    else if (primeiroCargaPedido != null && primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.VAS)
                    {
                        diasPrazoFatura = acordoCliente.LongoCursoDiasDePrazoFatura;
                        tipoPrazoFatura = acordoCliente.LongoCursoTipoPrazoFaturamento;
                        diasSemana = acordoCliente.LongoCursoDiasSemanaFatura != null ? acordoCliente.LongoCursoDiasSemanaFatura.ToList() : null;
                        diasMes = acordoCliente.LongoCursoDiasMesFatura != null ? acordoCliente.LongoCursoDiasMesFatura.ToList() : null;
                    }
                    else if (primeiroCargaPedido != null && (primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.CargaFechada || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.CargaFracionada || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder))
                    {
                        diasPrazoFatura = acordoCliente.CabotagemDiasDePrazoFatura;
                        tipoPrazoFatura = acordoCliente.CabotagemTipoPrazoFaturamento;
                        diasSemana = acordoCliente.CabotagemDiasSemanaFatura != null ? acordoCliente.CabotagemDiasSemanaFatura.ToList() : null;
                        diasMes = acordoCliente.CabotagemDiasMesFatura != null ? acordoCliente.CabotagemDiasMesFatura.ToList() : null;
                    }
                    else if (primeiroCargaPedido != null && primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.NotaDebito)
                    {
                        diasPrazoFatura = acordoCliente.DiasPrazoVencimentoNotaDebito > 0 ? acordoCliente.DiasPrazoVencimentoNotaDebito : acordoCliente.CabotagemDiasDePrazoFatura;
                        tipoPrazoFatura = acordoCliente.CabotagemTipoPrazoFaturamento;
                        diasSemana = acordoCliente.CabotagemDiasSemanaFatura != null ? acordoCliente.CabotagemDiasSemanaFatura.ToList() : null;
                        diasMes = acordoCliente.CabotagemDiasMesFatura != null ? acordoCliente.CabotagemDiasMesFatura.ToList() : null;
                    }
                    else
                    {
                        diasPrazoFatura = acordoCliente.CustoExtraDiasDePrazoFatura;
                        tipoPrazoFatura = acordoCliente.CustoExtraTipoPrazoFaturamento;
                        diasSemana = acordoCliente.CustoExtraDiasSemanaFatura != null ? acordoCliente.CustoExtraDiasSemanaFatura.ToList() : null;
                        diasMes = acordoCliente.CustoExtraDiasMesFatura != null ? acordoCliente.CustoExtraDiasMesFatura.ToList() : null;
                    }
                }
            }

            if (primeiroCargaPedido != null && (primeiroCargaPedido.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false))
            {
                permiteFinalSemana = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.PermiteFinalDeSemana ?? false;
                formaTitulo = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.FormaTitulo.HasValue == true ? primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.FormaTitulo.Value : FormaTitulo.Outros;
                if (tipoPrazoFatura == null)
                    tipoPrazoFatura = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.TipoPrazoFaturamento;
                if (diasPrazoFatura == 0)
                    diasPrazoFatura = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasDePrazoFatura ?? 0;
                if (diasMes == null || diasMes.Count == 0)
                    diasMes = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasMesFatura.ToList() ?? null;
                if (diasSemana == null || diasSemana.Count == 0)
                    diasSemana = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasSemanaFatura.ToList() ?? null;

            }
            else if (cte.TomadorPagador != null && !cte.TomadorPagador.Cliente.NaoUsarConfiguracaoFaturaGrupo)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPrimeiroGrupoCliente(cte.TomadorPagador.Cliente);
                if (grupo != null)
                {
                    diaMes = 0;// grupo.DiaMesFatura;
                    permiteFinalSemana = grupo.PermiteFinalDeSemana;
                    diaSemana = null;//grupo.DiaSemana;
                    formaTitulo = grupo.FormaTitulo.HasValue ? grupo.FormaTitulo.Value : FormaTitulo.Outros;
                    if (tipoPrazoFatura == null)
                        tipoPrazoFatura = grupo.TipoPrazoFaturamento;
                    if (diasPrazoFatura == 0)
                        diasPrazoFatura = grupo.DiasDePrazoFatura;
                    if (diasMes == null || diasMes.Count == 0)
                        diasMes = grupo.DiasMesFatura != null ? grupo.DiasMesFatura.ToList() : null;
                    if (diasSemana == null || diasSemana.Count == 0)
                        diasSemana = grupo.DiasSemanaFatura != null ? grupo.DiasSemanaFatura.ToList() : null;
                }
                else
                {
                    diaMes = 0;// cte.TomadorPagador.Cliente.DiaMesFatura;
                    permiteFinalSemana = cte.TomadorPagador.Cliente.PermiteFinalDeSemana;
                    diaSemana = null;// cte.TomadorPagador.Cliente.DiaSemana;
                    formaTitulo = cte.TomadorPagador.Cliente.FormaTitulo.HasValue ? cte.TomadorPagador.Cliente.FormaTitulo.Value : FormaTitulo.Outros;
                    if (tipoPrazoFatura == null)
                        tipoPrazoFatura = cte.TomadorPagador.Cliente.TipoPrazoFaturamento;
                    if (diasPrazoFatura == 0)
                        diasPrazoFatura = cte.TomadorPagador.Cliente.DiasDePrazoFatura;
                    if (diasMes == null || diasMes.Count == 0)
                        diasMes = cte.TomadorPagador.Cliente.DiasMesFatura != null ? cte.TomadorPagador.Cliente.DiasMesFatura.ToList() : null;
                    if (diasSemana == null || diasSemana.Count == 0)
                        diasSemana = cte.TomadorPagador.Cliente.DiasSemanaFatura != null ? cte.TomadorPagador.Cliente.DiasSemanaFatura.ToList() : null;
                }
            }
            else if (cte.TomadorPagador.Cliente != null)
            {
                diaMes = 0;// cte.TomadorPagador.Cliente.DiaMesFatura;
                permiteFinalSemana = cte.TomadorPagador.Cliente.PermiteFinalDeSemana;
                diaSemana = null;// cte.TomadorPagador.Cliente.DiaSemana;
                formaTitulo = cte.TomadorPagador.Cliente.FormaTitulo.HasValue ? cte.TomadorPagador.Cliente.FormaTitulo.Value : FormaTitulo.Outros;
                if (tipoPrazoFatura == null)
                    tipoPrazoFatura = cte.TomadorPagador.Cliente.TipoPrazoFaturamento;
                if (diasPrazoFatura == 0)
                    diasPrazoFatura = cte.TomadorPagador.Cliente.DiasDePrazoFatura;
                if (diasMes == null || diasMes.Count == 0)
                    diasMes = cte.TomadorPagador.Cliente.DiasMesFatura != null ? cte.TomadorPagador.Cliente.DiasMesFatura.ToList() : null;
                if (diasSemana == null || diasSemana.Count == 0)
                    diasSemana = cte.TomadorPagador.Cliente.DiasSemanaFatura != null ? cte.TomadorPagador.Cliente.DiasSemanaFatura.ToList() : null;
            }
            else if (cte.TomadorPagador.GrupoPessoas != null)
            {
                diaMes = 0;//cte.TomadorPagador.GrupoPessoas.DiaMesFatura;
                permiteFinalSemana = cte.TomadorPagador.GrupoPessoas.PermiteFinalDeSemana;
                diaSemana = null;//cte.TomadorPagador.GrupoPessoas.DiaSemana;
                formaTitulo = cte.TomadorPagador.GrupoPessoas.FormaTitulo.HasValue ? cte.TomadorPagador.GrupoPessoas.FormaTitulo.Value : FormaTitulo.Outros;
                if (tipoPrazoFatura == null)
                    tipoPrazoFatura = cte.TomadorPagador.GrupoPessoas.TipoPrazoFaturamento;
                if (diasPrazoFatura == 0)
                    diasPrazoFatura = cte.TomadorPagador.GrupoPessoas.DiasDePrazoFatura;
                if (diasMes == null || diasMes.Count == 0)
                    diasMes = cte.TomadorPagador.GrupoPessoas.DiasMesFatura != null ? cte.TomadorPagador.GrupoPessoas.DiasMesFatura.ToList() : null;
                if (diasSemana == null || diasSemana.Count == 0)
                    diasSemana = cte.TomadorPagador.GrupoPessoas.DiasSemanaFatura != null ? cte.TomadorPagador.GrupoPessoas.DiasSemanaFatura.ToList() : null;
            }
        }

        public void RetornarParametrosFaturamento(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeDeTrabalho, bool faturamentoMultimodal, out int? diaMes, out DiaSemana? diaSemana, out bool? permiteFinalSemana, out int? diasPrazoFatura, out List<DiaSemana> diasSemana, out List<int> diasMes, out TipoPrazoFaturamento? tipoPrazoFatura, int diaPrazoPadrao, out FormaTitulo formaTitulo)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordo = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unidadeDeTrabalho);

            diasPrazoFatura = 0;
            diaMes = 0;
            permiteFinalSemana = false;
            diaSemana = null;
            diasSemana = new List<DiaSemana>();
            diasMes = new List<int>();
            formaTitulo = FormaTitulo.Outros;

            tipoPrazoFatura = null;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repFaturaDocumento.BuscarPrimeiroCargaPedido(fatura.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoCliente = null;
            if (fatura.Cliente != null)
            {
                acordoCliente = repAcordo.BuscarAcordoCliente(fatura.Cliente.CPF_CNPJ, 0);
                if (acordoCliente == null && fatura.Cliente.GrupoPessoas != null)
                    acordoCliente = repAcordo.BuscarAcordoCliente(0, fatura.Cliente.GrupoPessoas.Codigo);
            }
            if (acordoCliente == null && fatura.GrupoPessoas != null)
                acordoCliente = repAcordo.BuscarAcordoCliente(0, fatura.GrupoPessoas.Codigo);

            if (acordoCliente != null)
            {
                if (primeiroCargaPedido != null)
                {
                    if (primeiroCargaPedido.Carga.TipoOperacao != null && primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado)
                    {
                        if (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                        {
                            diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : acordoCliente.LongoCursoDiasDePrazoFatura;
                            tipoPrazoFatura = acordoCliente.LongoCursoTipoPrazoFaturamento;
                            diasSemana = acordoCliente.LongoCursoDiasSemanaFatura != null ? acordoCliente.LongoCursoDiasSemanaFatura.ToList() : null;
                            diasMes = acordoCliente.LongoCursoDiasMesFatura != null ? acordoCliente.LongoCursoDiasMesFatura.ToList() : null;
                        }
                        if (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                        {
                            diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : acordoCliente.CustoExtraDiasDePrazoFatura;
                            tipoPrazoFatura = acordoCliente.CustoExtraTipoPrazoFaturamento;
                            diasSemana = acordoCliente.CustoExtraDiasSemanaFatura != null ? acordoCliente.CustoExtraDiasSemanaFatura.ToList() : null;
                            diasMes = acordoCliente.CustoExtraDiasMesFatura != null ? acordoCliente.CustoExtraDiasMesFatura.ToList() : null;
                        }
                    }
                    else if (primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.DemurrageCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.DetentionCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.TAkePayCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.TakePayFeeder || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.NoShowCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.NotaDebito)
                    {
                        if (acordoCliente.ConsiderarParametrosDeFreteCabotagem)
                        {
                            diasPrazoFatura = acordoCliente.CabotagemDiasDePrazoFatura;
                            tipoPrazoFatura = acordoCliente.CabotagemTipoPrazoFaturamento;
                            diasSemana = acordoCliente.CabotagemDiasSemanaFatura != null ? acordoCliente.CabotagemDiasSemanaFatura.ToList() : null;
                            diasMes = acordoCliente.CabotagemDiasMesFatura != null ? acordoCliente.CabotagemDiasMesFatura.ToList() : null;
                        }
                        else
                        {
                            bool usarDiasPrazoFaturamentoDnD = primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.DemurrageCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.DetentionCabotagem;

                            if (primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.NotaDebito)
                                diasPrazoFatura = acordoCliente.DiasPrazoVencimentoNotaDebito > 0 ? acordoCliente.DiasPrazoVencimentoNotaDebito : diaPrazoPadrao;
                            else if (diaPrazoPadrao > 0)
                                diasPrazoFatura = diaPrazoPadrao;
                            else
                                diasPrazoFatura = usarDiasPrazoFaturamentoDnD ? acordoCliente.DiasPrazoFaturamentoDnD : acordoCliente.TakeOrPayDiasDePrazoFatura;
                            tipoPrazoFatura = TipoPrazoFaturamento.DataFatura;
                            diasSemana = null;
                            diasMes = null;
                        }
                    }
                    else if (primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.VAS)
                    {
                        diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : acordoCliente.LongoCursoDiasDePrazoFatura;
                        tipoPrazoFatura = acordoCliente.LongoCursoTipoPrazoFaturamento;
                        diasSemana = acordoCliente.LongoCursoDiasSemanaFatura != null ? acordoCliente.LongoCursoDiasSemanaFatura.ToList() : null;
                        diasMes = acordoCliente.LongoCursoDiasMesFatura != null ? acordoCliente.LongoCursoDiasMesFatura.ToList() : null;
                    }
                    else if (primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.CargaFechada || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.CargaFracionada || primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder)
                    {
                        diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : acordoCliente.CabotagemDiasDePrazoFatura;
                        tipoPrazoFatura = acordoCliente.CabotagemTipoPrazoFaturamento;
                        diasSemana = acordoCliente.CabotagemDiasSemanaFatura != null ? acordoCliente.CabotagemDiasSemanaFatura.ToList() : null;
                        diasMes = acordoCliente.CabotagemDiasMesFatura != null ? acordoCliente.CabotagemDiasMesFatura.ToList() : null;
                    }
                    else if (primeiroCargaPedido != null && primeiroCargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.NotaDebito)
                    {
                        diasPrazoFatura = acordoCliente.DiasPrazoVencimentoNotaDebito > 0 ? acordoCliente.DiasPrazoVencimentoNotaDebito : acordoCliente.CabotagemDiasDePrazoFatura;
                        tipoPrazoFatura = acordoCliente.CabotagemTipoPrazoFaturamento;
                        diasSemana = acordoCliente.CabotagemDiasSemanaFatura != null ? acordoCliente.CabotagemDiasSemanaFatura.ToList() : null;
                        diasMes = acordoCliente.CabotagemDiasMesFatura != null ? acordoCliente.CabotagemDiasMesFatura.ToList() : null;
                    }
                    else
                    {
                        diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : acordoCliente.CustoExtraDiasDePrazoFatura;
                        tipoPrazoFatura = acordoCliente.CustoExtraTipoPrazoFaturamento;
                        diasSemana = acordoCliente.CustoExtraDiasSemanaFatura != null ? acordoCliente.CustoExtraDiasSemanaFatura.ToList() : null;
                        diasMes = acordoCliente.CustoExtraDiasMesFatura != null ? acordoCliente.CustoExtraDiasMesFatura.ToList() : null;
                    }
                }
            }

            if (primeiroCargaPedido?.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
            {
                permiteFinalSemana = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.PermiteFinalDeSemana ?? false;
                formaTitulo = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.FormaTitulo.HasValue == true ? primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.FormaTitulo.Value : FormaTitulo.Outros;
                if (tipoPrazoFatura == null)
                    tipoPrazoFatura = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.TipoPrazoFaturamento;
                if (diasPrazoFatura == 0)
                    diasPrazoFatura = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasDePrazoFatura ?? 0;
                if (diasMes == null || diasMes.Count == 0)
                    diasMes = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasMesFatura.ToList() ?? null;
                if (diasSemana == null || diasSemana.Count == 0)
                    diasSemana = primeiroCargaPedido.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DiasSemanaFatura.ToList() ?? null;

            }
            else if (fatura.Cliente != null && !fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPrimeiroGrupoCliente(fatura.Cliente);
                if (grupo != null)
                {
                    diaMes = grupo.DiaMesFatura;
                    permiteFinalSemana = grupo.PermiteFinalDeSemana;
                    diaSemana = grupo.DiaSemana;
                    formaTitulo = grupo.FormaTitulo.HasValue ? grupo.FormaTitulo.Value : FormaTitulo.Outros;
                    if (tipoPrazoFatura == null)
                        tipoPrazoFatura = grupo.TipoPrazoFaturamento;
                    if (diasPrazoFatura == 0)
                        diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : grupo.DiasDePrazoFatura;
                    if (diasMes == null || diasMes.Count == 0)
                        diasMes = grupo.DiasMesFatura != null ? grupo.DiasMesFatura.ToList() : null;
                    if (diasSemana == null || diasSemana.Count == 0)
                        diasSemana = grupo.DiasSemanaFatura != null ? grupo.DiasSemanaFatura.ToList() : null;
                }
                else
                {
                    diaMes = fatura.Cliente.DiaMesFatura;
                    permiteFinalSemana = fatura.Cliente.PermiteFinalDeSemana;
                    diaSemana = fatura.Cliente.DiaSemana;
                    formaTitulo = fatura.Cliente.FormaTitulo.HasValue ? fatura.Cliente.FormaTitulo.Value : FormaTitulo.Outros;
                    if (tipoPrazoFatura == null)
                        tipoPrazoFatura = fatura.Cliente.TipoPrazoFaturamento;
                    if (diasPrazoFatura == 0)
                        diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : fatura.Cliente.DiasDePrazoFatura;
                    if (diasMes == null || diasMes.Count == 0)
                        diasMes = fatura.Cliente.DiasMesFatura != null ? fatura.Cliente.DiasMesFatura.ToList() : null;
                    if (diasSemana == null || diasSemana.Count == 0)
                        diasSemana = fatura.Cliente.DiasSemanaFatura != null ? fatura.Cliente.DiasSemanaFatura.ToList() : null;
                }
            }
            else if (fatura.Cliente != null)
            {
                diaMes = fatura.Cliente.DiaMesFatura;
                permiteFinalSemana = fatura.Cliente.PermiteFinalDeSemana;
                diaSemana = fatura.Cliente.DiaSemana;
                formaTitulo = fatura.Cliente.FormaTitulo.HasValue ? fatura.Cliente.FormaTitulo.Value : FormaTitulo.Outros;
                if (tipoPrazoFatura == null)
                    tipoPrazoFatura = fatura.Cliente.TipoPrazoFaturamento;
                if (diasPrazoFatura == 0)
                    diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : fatura.Cliente.DiasDePrazoFatura;
                if (diasMes == null || diasMes.Count == 0)
                    diasMes = fatura.Cliente.DiasMesFatura != null ? fatura.Cliente.DiasMesFatura.ToList() : null;
                if (diasSemana == null || diasSemana.Count == 0)
                    diasSemana = fatura.Cliente.DiasSemanaFatura != null ? fatura.Cliente.DiasSemanaFatura.ToList() : null;
            }
            else if (fatura.GrupoPessoas != null)
            {
                diaMes = fatura.GrupoPessoas.DiaMesFatura;
                permiteFinalSemana = fatura.GrupoPessoas.PermiteFinalDeSemana;
                diaSemana = fatura.GrupoPessoas.DiaSemana;
                formaTitulo = fatura.GrupoPessoas.FormaTitulo.HasValue ? fatura.GrupoPessoas.FormaTitulo.Value : FormaTitulo.Outros;
                if (tipoPrazoFatura == null)
                    tipoPrazoFatura = fatura.GrupoPessoas.TipoPrazoFaturamento;
                if (diasPrazoFatura == 0)
                    diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : fatura.GrupoPessoas.DiasDePrazoFatura;
                if (diasMes == null || diasMes.Count == 0)
                    diasMes = fatura.GrupoPessoas.DiasMesFatura != null ? fatura.GrupoPessoas.DiasMesFatura.ToList() : null;
                if (diasSemana == null || diasSemana.Count == 0)
                    diasSemana = fatura.GrupoPessoas.DiasSemanaFatura != null ? fatura.GrupoPessoas.DiasSemanaFatura.ToList() : null;
            }
            else if (fatura.Tomador != null)
            {
                diaMes = fatura.Tomador.DiaMesFatura;
                permiteFinalSemana = fatura.Tomador.PermiteFinalDeSemana;
                diaSemana = fatura.Tomador.DiaSemana;
                formaTitulo = fatura.Tomador.FormaTitulo.HasValue ? fatura.Tomador.FormaTitulo.Value : FormaTitulo.Outros;
                if (tipoPrazoFatura == null)
                    tipoPrazoFatura = fatura.Tomador.TipoPrazoFaturamento;
                if (diasPrazoFatura == 0)
                    diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : fatura.Tomador.DiasDePrazoFatura;
                if (diasMes == null || diasMes.Count == 0)
                    diasMes = fatura.Tomador.DiasMesFatura != null ? fatura.Tomador.DiasMesFatura.ToList() : null;
                if (diasSemana == null || diasSemana.Count == 0)
                    diasSemana = fatura.Tomador.DiasSemanaFatura != null ? fatura.Tomador.DiasSemanaFatura.ToList() : null;
            }
        }

        public void RetornarParametrosFaturamentoTransportador(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeDeTrabalho, bool faturamentoMultimodal, out int? diaMes, out DiaSemana? diaSemana, out bool? permiteFinalSemana, out int? diasPrazoFatura, out List<DiaSemana> diasSemana, out List<int> diasMes, out TipoPrazoPagamento? tipoPrazoFatura, int diaPrazoPadrao, out FormaTitulo formaTitulo, out bool existeConfiguracaoFaturaTransportador)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);

            DateTime dataDocumento = fatura.DataGeracaoFatura ?? DateTime.Now;
            DateTime dataPagamento = fatura.DataFatura;

            Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador repositorioCondicaoPagamentoTransportador = new Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador(unidadeDeTrabalho);
            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamentoTransportador = repositorioCondicaoPagamentoTransportador.BuscarObjetoPorEmpresa(fatura.Transportador.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoPagamentoTransportador = ObterCondicaoPagamentoFiltrada(condicoesPagamentoTransportador, dataDocumento, dataPagamento);

            if (condicaoPagamentoTransportador == null && fatura.Filial != null)
            {
                Repositorio.Embarcador.Filiais.CondicaoPagamentoFilial repositorioCondicaoPagamentoFilial = new Repositorio.Embarcador.Filiais.CondicaoPagamentoFilial(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamentoFilial = repositorioCondicaoPagamentoFilial.BuscarObjetoPorFilial(fatura.Filial.Codigo);

                condicaoPagamentoTransportador = ObterCondicaoPagamentoFiltrada(condicoesPagamentoFilial, dataDocumento, dataPagamento);
            }

            diasPrazoFatura = 0;
            diaMes = 0;
            permiteFinalSemana = false;
            diaSemana = null;
            diasSemana = new List<DiaSemana>();
            diasMes = new List<int>();
            formaTitulo = FormaTitulo.Outros;

            tipoPrazoFatura = null;

            if (condicaoPagamentoTransportador != null)
            {
                diasPrazoFatura = diaPrazoPadrao > 0 ? diaPrazoPadrao : condicaoPagamentoTransportador.DiasDePrazoPagamento;
                tipoPrazoFatura = condicaoPagamentoTransportador.TipoPrazoPagamento;
                if (condicaoPagamentoTransportador.DiaSemana != null) diasSemana.Add((DiaSemana)(condicaoPagamentoTransportador.DiaSemana));
                if (condicaoPagamentoTransportador.DiaMes != null) diasMes.Add((int)condicaoPagamentoTransportador.DiaMes);
                existeConfiguracaoFaturaTransportador = true;
            }
            else
                existeConfiguracaoFaturaTransportador = false;
        }

        public void LancarParcelaFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeDeTrabalho, bool faturamentoMultimodal, List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoParcela> Parcelas = null, DateTime? dataAprovacaoFatura = null)
        {
            Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeDeTrabalho);

            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> listaParcelas = repFaturaParcela.BuscarPorFaturaSemFetch(fatura.Codigo);
            DateTime dataUltimaParcela = DateTime.MinValue;

            for (int i = 0; i < listaParcelas.Count; i++)
                repFaturaParcela.Deletar(listaParcelas[i]);

            if (GerarParcelaFaturaDeAdiantamentoSaldo(fatura, unidadeDeTrabalho, faturamentoMultimodal, Parcelas, dataAprovacaoFatura))
                return;

            List<int> diasParcela = Servicos.Embarcador.Financeiro.Titulo.ObterDiasPrazoVencimento(fatura.ClienteTomadorFatura, null);

            int qtdParcelas = diasParcela.Count;

            decimal valorTotal = Math.Round(fatura.Total - fatura.Descontos + fatura.Acrescimos, 2, MidpointRounding.ToEven);
            decimal valorParcelas = qtdParcelas > 0 ? Math.Round(valorTotal / qtdParcelas, 2, MidpointRounding.ToEven) : valorTotal;

            decimal valorMoedaTotal = fatura.TotalMoedaEstrangeira - fatura.DescontosMoeda + fatura.AcrescimosMoeada;
            decimal valorMoedaParcelas = qtdParcelas > 0 ? Math.Round(valorMoedaTotal / qtdParcelas, 2, MidpointRounding.ToEven) : valorMoedaTotal;

            int posParcela = 0;
            decimal valorAcumulado = 0m, valorMoedaAcumulado = 0m;
            FormaTitulo formaTitulo = FormaTitulo.Outros;

            bool existeConfiguracaoFaturaTransportador = false;
            int? diaMes = null;
            DiaSemana? diaSemana = null;
            bool? permiteFinalSemana = null;
            int? diasPrazoFatura = null;
            List<DiaSemana> diasSemana = new List<DiaSemana>();
            List<int> diasMes = new List<int>();
            TipoPrazoFaturamento? tipoPrazoFatura = null;
            TipoPrazoPagamento? tipoPrazoPagamento = null;

            foreach (int dia in diasParcela)
            {
                posParcela++;

                valorAcumulado += valorParcelas;
                valorMoedaAcumulado += valorMoedaParcelas;

                if (posParcela == qtdParcelas)
                {
                    if (valorAcumulado != valorTotal)
                        valorParcelas += valorTotal - valorAcumulado;

                    if (valorMoedaAcumulado != valorMoedaTotal)
                        valorMoedaParcelas += valorMoedaTotal - valorMoedaAcumulado;
                }

                if (configuracaoFinanceiro.UtilizarConfiguracoesTransportadorParaFatura)
                {
                    RetornarParametrosFaturamentoTransportador(fatura,
                                                               unidadeDeTrabalho,
                                                               faturamentoMultimodal,
                                                               out diaMes,
                                                               out diaSemana,
                                                               out permiteFinalSemana,
                                                               out diasPrazoFatura,
                                                               out diasSemana,
                                                               out diasMes,
                                                               out tipoPrazoPagamento,
                                                               dia,
                                                               out formaTitulo,
                                                               out existeConfiguracaoFaturaTransportador);
                }

                if (!existeConfiguracaoFaturaTransportador)
                {
                    RetornarParametrosFaturamento(fatura,
                                                  unidadeDeTrabalho,
                                                  faturamentoMultimodal,
                                                  out diaMes,
                                                  out diaSemana,
                                                  out permiteFinalSemana,
                                                  out diasPrazoFatura,
                                                  out diasSemana,
                                                  out diasMes,
                                                  out tipoPrazoFatura,
                                                  dia,
                                                  out formaTitulo);
                }

                DateTime? dataBaseParcela;
                if (!existeConfiguracaoFaturaTransportador)
                {
                    dataBaseParcela = dataAprovacaoFatura != null ? dataAprovacaoFatura : RetornarDataBase(tipoPrazoFatura, fatura, faturamentoMultimodal, unidadeDeTrabalho);
                }
                else
                {
                    dataBaseParcela = dataAprovacaoFatura != null ? dataAprovacaoFatura : RetornarDataBaseTransportador(tipoPrazoPagamento, fatura, faturamentoMultimodal, unidadeDeTrabalho);
                }

                if (diasPrazoFatura == null)
                    diasPrazoFatura = 0;

                if (diaMes != null || diaSemana != null || diasPrazoFatura != null || tipoPrazoFatura != null || diasMes.Count > 0 || diasSemana.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = new Dominio.Entidades.Embarcador.Fatura.FaturaParcela
                    {
                        DataEmissao = fatura.DataFatura,
                        DataVencimento = RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes, fatura.ClienteTomadorFatura, fatura.GrupoPessoas, existeConfiguracaoFaturaTransportador, unidadeDeTrabalho),
                        Desconto = 0,
                        Fatura = fatura,
                        Sequencia = posParcela,
                        SituacaoFaturaParcela = SituacaoFaturaParcela.EmAberto,
                        Valor = valorParcelas,
                        ValorTotalMoeda = valorMoedaParcelas,
                        FormaTitulo = formaTitulo
                    };

                    if (existeConfiguracaoFaturaTransportador && faturamentoMultimodal && parcela.DataVencimento.Date <= DateTime.Now.Date && dataBaseParcela.HasValue && tipoPrazoPagamento.HasValue && tipoPrazoPagamento.Value != TipoPrazoPagamento.DataPagamento)
                    {
                        dataBaseParcela = RetornarDataBase((TipoPrazoFaturamento?)TipoPrazoPagamento.DataPagamento, fatura, faturamentoMultimodal, unidadeDeTrabalho);
                        parcela.DataVencimento = RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes, fatura.ClienteTomadorFatura, fatura.GrupoPessoas, existeConfiguracaoFaturaTransportador, unidadeDeTrabalho);
                    }
                    else if (faturamentoMultimodal && parcela.DataVencimento.Date <= DateTime.Now.Date && dataBaseParcela.HasValue && tipoPrazoFatura.HasValue && tipoPrazoFatura.Value != TipoPrazoFaturamento.DataFatura)
                    {
                        dataBaseParcela = RetornarDataBase(TipoPrazoFaturamento.DataFatura, fatura, faturamentoMultimodal, unidadeDeTrabalho);
                        parcela.DataVencimento = RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes, fatura.ClienteTomadorFatura, fatura.GrupoPessoas, existeConfiguracaoFaturaTransportador, unidadeDeTrabalho);
                    }



                    repFaturaParcela.Inserir(parcela);
                    dataUltimaParcela = parcela.DataVencimento;
                }
            }

            servFatura.AtualizarValorVencimento(dataUltimaParcela, fatura.Codigo, unidadeDeTrabalho);
        }

        public DateTime? RetornarDataBase(TipoPrazoFaturamento? tipoPrazoFatura, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool faturamentoMultimodal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);

            DateTime? dataBaseParcela = DateTime.Now;
            if (tipoPrazoFatura != null)
            {
                if (tipoPrazoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento.DataFatura)
                {
                    dataBaseParcela = cte.DataEmissao;

                    if (!dataBaseParcela.HasValue)
                        dataBaseParcela = DateTime.Now;
                }
                else if (tipoPrazoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento.DataDocumento)
                {
                    dataBaseParcela = cte.DataEmissao;

                    if (!dataBaseParcela.HasValue)
                        dataBaseParcela = DateTime.Now;
                }
                else if (tipoPrazoFatura == TipoPrazoFaturamento.DataPrevisaoEncerramento)
                {
                    if (faturamentoMultimodal)
                    {
                        dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoChegadaNavioPorCTe(cte.Codigo);
                        if (!dataBaseParcela.HasValue)
                            dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoEncerramentoPorCTe(cte.Codigo);
                    }

                    if (!dataBaseParcela.HasValue)
                        dataBaseParcela = DateTime.Now;
                    if (faturamentoMultimodal)
                    {
                        if ((DateTime.Now.Date - dataBaseParcela.Value.Date).TotalDays > 2)
                            dataBaseParcela = DateTime.Now;
                    }
                }
                else if (tipoPrazoFatura == TipoPrazoFaturamento.DataPrevisaoInicioViagem)
                {
                    if (faturamentoMultimodal)
                    {
                        dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoSaidaNavioPorCTe(cte.Codigo);
                        if (!dataBaseParcela.HasValue)
                            dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoInicioPorCTe(cte.Codigo);
                    }

                    if (!dataBaseParcela.HasValue)
                        dataBaseParcela = DateTime.Now;

                    if (faturamentoMultimodal)
                    {
                        if ((DateTime.Now.Date - dataBaseParcela.Value.Date).TotalDays > 2)
                            dataBaseParcela = DateTime.Now;
                    }
                }
                else
                    dataBaseParcela = DateTime.Now;
            }
            return dataBaseParcela;
        }

        public DateTime? RetornarDataBase(TipoPrazoFaturamento? tipoPrazoFatura, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, bool faturamentoMultimodal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);

            DateTime? dataBaseParcela = DateTime.Now;
            if (tipoPrazoFatura != null)
            {
                if (tipoPrazoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento.DataFatura)
                {
                    if (fatura.DataFinal.HasValue && fatura.DataFinal.Value > DateTime.MinValue)
                        dataBaseParcela = fatura.DataFinal;
                    else
                        dataBaseParcela = fatura.DataFatura;
                }
                else if (tipoPrazoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento.DataDocumento)
                {
                    if (fatura.NovoModelo)
                        dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataEmissao(fatura.Codigo);
                    else
                        dataBaseParcela = repFaturaCargaDocumento.BuscarPrimeiraDataEmissaoCTe(fatura.Codigo);

                    if (!dataBaseParcela.HasValue)
                        dataBaseParcela = DateTime.Now;
                }
                else if (tipoPrazoFatura == TipoPrazoFaturamento.DataPrevisaoEncerramento)
                {
                    if (faturamentoMultimodal)
                    {
                        dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoChegadaNavio(fatura.Codigo);
                        if (!dataBaseParcela.HasValue)
                            dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoEncerramento(fatura.Codigo);
                    }
                    else if (fatura.NovoModelo)
                        dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoEncerramento(fatura.Codigo);
                    else
                        dataBaseParcela = repFaturaCargaDocumento.BuscarPrimeiraDataEmissaoCTe(fatura.Codigo);

                    if (!dataBaseParcela.HasValue)
                        dataBaseParcela = DateTime.Now;

                    if (faturamentoMultimodal)
                    {
                        if ((DateTime.Now.Date - dataBaseParcela.Value.Date).TotalDays > 2)
                            dataBaseParcela = DateTime.Now;
                    }
                }
                else if (tipoPrazoFatura == TipoPrazoFaturamento.DataPrevisaoInicioViagem)
                {
                    if (faturamentoMultimodal)
                    {
                        dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoSaidaNavio(fatura.Codigo);
                        if (!dataBaseParcela.HasValue)
                            dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoInicio(fatura.Codigo);
                    }
                    else
                    if (fatura.NovoModelo)
                        dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataPrevisaoInicio(fatura.Codigo);
                    else
                        dataBaseParcela = repFaturaCargaDocumento.BuscarPrimeiraDataEmissaoCTe(fatura.Codigo);

                    if (!dataBaseParcela.HasValue)
                        dataBaseParcela = DateTime.Now;

                    if (faturamentoMultimodal)
                    {
                        if ((DateTime.Now.Date - dataBaseParcela.Value.Date).TotalDays > 2)
                            dataBaseParcela = DateTime.Now;
                    }
                }
                else if (tipoPrazoFatura == TipoPrazoFaturamento.ApartirDataGeracaoFatura)
                {
                    dataBaseParcela = fatura.DataGeracaoFatura;
                }
                else if (tipoPrazoFatura == TipoPrazoFaturamento.ApartirAprovacaoFatura)
                {
                    dataBaseParcela = null;
                }
                else
                    dataBaseParcela = DateTime.Now;
            }
            return dataBaseParcela;
        }

        public DateTime? RetornarDataBaseTransportador(TipoPrazoPagamento? tipoPrazoPagamento, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, bool faturamentoMultimodal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);

            DateTime? dataBaseParcela = DateTime.Now;
            if (tipoPrazoPagamento != null)
            {
                if (tipoPrazoPagamento == TipoPrazoPagamento.DataPagamento)
                {
                    if (fatura.DataFinal.HasValue && fatura.DataFinal.Value > DateTime.MinValue)
                        dataBaseParcela = fatura.DataFinal;
                    else
                        dataBaseParcela = fatura.DataFatura;
                }
                else if (tipoPrazoPagamento == TipoPrazoPagamento.DataDocumento)
                {
                    if (fatura.NovoModelo)
                        dataBaseParcela = repFaturaDocumento.BuscarPrimeiraDataEmissao(fatura.Codigo);
                    else
                        dataBaseParcela = repFaturaCargaDocumento.BuscarPrimeiraDataEmissaoCTe(fatura.Codigo);

                    if (!dataBaseParcela.HasValue)
                        dataBaseParcela = DateTime.Now;
                }
                else if (tipoPrazoPagamento == TipoPrazoPagamento.ApartirDataGeracaoFatura)
                {
                    dataBaseParcela = fatura.DataGeracaoFatura;
                }
                else if (tipoPrazoPagamento == TipoPrazoPagamento.ApartirAprovacaoFatura)
                {
                    dataBaseParcela = null;
                }
                else
                    dataBaseParcela = DateTime.Now;
            }
            return dataBaseParcela;
        }

        public DateTime RetornaDataPadraoFatura(int? diaMes, DiaSemana? diaSemana, bool? permiteFinalSemana, DateTime? dataBaseVencimento, int? diasPrazoFatura, List<DiaSemana> diasSemana, List<int> diasMes, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, bool existeConfiguracaoFaturaTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaVencimento repVencimentoGrupo = new Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaVencimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.PessoaFaturaVencimento repVencimentoCliente = new Repositorio.Embarcador.Pessoas.PessoaFaturaVencimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            DateTime dataVencimento = DateTime.Now;
            try
            {
                DateTime dataVencimentoOriginal = DateTime.Now;
                if (diasPrazoFatura != null && diasPrazoFatura > 0)
                {
                    dataVencimento = dataBaseVencimento.Value.AddDays((double)diasPrazoFatura); //DateTime.Now;
                    dataVencimentoOriginal = dataVencimento;
                }
                else if (dataBaseVencimento.HasValue)
                {
                    dataVencimento = dataBaseVencimento.Value;
                    dataVencimentoOriginal = dataVencimento;
                }
                int diaAtual = dataVencimento.Day;//DateTime.Now.Day;
                int mesAtual = dataVencimento.Month;//DateTime.Now.Month;
                int anoAtual = dataVencimento.Year;//DateTime.Now.Year;

                if (diasMes != null && diasMes.Count > 0)
                {
                    int primeiroDia = 0;
                    int qtdDias = 0;
                    foreach (var dia in diasMes)
                    {
                        if (primeiroDia == 0)
                            primeiroDia = dia;

                        if (!SelecionarDataDoDia(dia, diaAtual, mesAtual, anoAtual, out dataVencimento, diasMes.Count, dataVencimento))
                        {
                            mesAtual = mesAtual + 1;
                            qtdDias += 1;
                            if (qtdDias == diasMes.Count)
                                SelecionarDataDoDia(dia, diaAtual, mesAtual, anoAtual, out dataVencimento, diasMes.Count, dataVencimento);
                        }
                        if (dataVencimento.Date >= DateTime.Now.Date && dataVencimento.Date >= dataVencimentoOriginal.Date)
                            break;

                        qtdDias += 1;

                    }
                    if ((dataVencimento.Date < DateTime.Now.Date || dataVencimento.Date < dataVencimentoOriginal.Date) && primeiroDia > 0)
                    {
                        mesAtual = mesAtual + 1;
                        SelecionarDataDoDia(primeiroDia, diaAtual, mesAtual, anoAtual, out dataVencimento, diasMes.Count, dataVencimento);
                    }
                }

                if (!existeConfiguracaoFaturaTransportador && tomador != null && tomador.NaoUsarConfiguracaoFaturaGrupo && tomador.HabilitarPeriodoVencimentoEspecifico)
                {
                    List<Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento> vencimentos = repVencimentoCliente.BuscarPorCliente(tomador.CPF_CNPJ);
                    if (vencimentos != null && vencimentos.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.PessoaFaturaVencimento faturaVencimento = vencimentos.Where(c => c.DiaInicial <= dataVencimento.Day && c.DiaFinal >= dataVencimento.Day)?.FirstOrDefault() ?? null;
                        int diaMultiplosVencimentos = faturaVencimento?.DiaVencimento ?? 0;

                        if (diaMultiplosVencimentos > 0)
                        {
                            bool pularMes = faturaVencimento.DiaInicial > diaMultiplosVencimentos && faturaVencimento.DiaFinal > diaMultiplosVencimentos;
                            bool pularAno = (dataVencimento.Month + 1) > 12;
                            int mes = pularMes ? (dataVencimento.Month + 1) : dataVencimento.Month;
                            if (mes > 12)
                                mes = mes - 12;
                            dataVencimento = new DateTime(pularAno ? dataVencimento.Year + 1 : dataVencimento.Year, mes, diaMultiplosVencimentos);
                        }
                    }
                }
                else if (!existeConfiguracaoFaturaTransportador && grupoPessoas != null && grupoPessoas.HabilitarPeriodoVencimentoEspecifico)
                {
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento> vencimentos = repVencimentoGrupo.BuscarPorGrupoPessoas(grupoPessoas.Codigo);

                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaVencimento faturaVencimento = vencimentos.Where(c => c.DiaInicial <= dataVencimento.Day && c.DiaFinal >= dataVencimento.Day)?.FirstOrDefault() ?? null;
                    int diaMultiplosVencimentos = faturaVencimento?.DiaVencimento ?? 0;

                    if (diaMultiplosVencimentos > 0)
                    {
                        bool pularMes = faturaVencimento.DiaInicial > diaMultiplosVencimentos && faturaVencimento.DiaFinal > diaMultiplosVencimentos;
                        bool pularAno = (dataVencimento.Month + 1) > 12;
                        int mes = pularMes ? (dataVencimento.Month + 1) : dataVencimento.Month;
                        if (mes > 12)
                            mes = mes - 12;
                        dataVencimento = new DateTime(pularAno ? dataVencimento.Year + 1 : dataVencimento.Year, mes, diaMultiplosVencimentos);
                    }
                }

                bool encontrouFinalSemana = false;
                if (diasSemana != null && diasSemana.Count > 0)
                {
                    foreach (var dia in diasSemana)
                    {
                        if ((int)dataVencimento.DayOfWeek == (int)dia - 1)
                        {
                            encontrouFinalSemana = true;
                            break;
                        }
                    }
                    if (!encontrouFinalSemana)
                    {
                        foreach (var dia in diasSemana)
                        {
                            if (!encontrouFinalSemana)
                            {
                                while ((int)dataVencimento.DayOfWeek != (int)dia - 1)
                                {
                                    encontrouFinalSemana = true;
                                    dataVencimento = dataVencimento.AddDays(1);
                                }
                            }
                        }
                    }
                }

                if (permiteFinalSemana != null)
                {
                    if (permiteFinalSemana == false)
                    {
                        if (dataVencimento.DayOfWeek == DayOfWeek.Saturday || dataVencimento.DayOfWeek == DayOfWeek.Sunday)
                        {
                            while (dataVencimento.DayOfWeek != DayOfWeek.Monday)
                            {
                                dataVencimento = dataVencimento.AddDays(1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "CalculoDataVencimento");
            }
            return dataVencimento;
        }

        private bool SelecionarDataDoDia(int dia, int diaAtual, int mesAtual, int anoAtual, out DateTime dataVencimento, int qtdDiasMes, DateTime dataVencimentoOriginal)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            dataVencimento = DateTime.Now;
            bool retorno = true;
            int diaSelecao = dia;
            if (mesAtual > 12)
            {
                mesAtual = 1;
                anoAtual = anoAtual + 1;
            }
            string data = Convert.ToString(diaSelecao).PadLeft(2, '0') + '/' + Convert.ToString(mesAtual).PadLeft(2, '0') + '/' + Convert.ToString(anoAtual);
            if (!DateTime.TryParseExact(data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento))
            {
                retorno = false;
                if (mesAtual == 2 && diaSelecao > 28)
                    diaSelecao = 28;
                if ((mesAtual == 4 || mesAtual == 6 || mesAtual == 9 || mesAtual == 11) && diaSelecao > 30)
                    diaSelecao = 30;
                data = Convert.ToString(diaSelecao).PadLeft(2, '0') + '/' + Convert.ToString(mesAtual).PadLeft(2, '0') + '/' + Convert.ToString(anoAtual);
                if (!DateTime.TryParseExact(data, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento))
                {
                    dataVencimento = DateTime.Now;
                    retorno = false;
                }
                retorno = true;
            }
            if (qtdDiasMes <= 1 && dataVencimento.Date < DateTime.Now.Date)
                retorno = false;
            return retorno;
        }

        public string ValidarCanhotosCTes(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (fatura.GrupoPessoas != null && (fatura.GrupoPessoas.NaoGerarFaturaAteReceberCanhotos == false || fatura.GrupoPessoas.NaoGerarFaturaAteReceberCanhotos == null))
                return null;
            if (fatura.Cliente != null && (fatura.Cliente.NaoGerarFaturaAteReceberCanhotos == false || fatura.Cliente.NaoGerarFaturaAteReceberCanhotos == null))
                return null;
            if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
            {
                if (!(fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ExigeCanhotoFisico ?? false)
                    || !(fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.NaoGerarFaturaAteReceberCanhotos ?? false))
                    return null;
            }
            else if (fatura.Cliente != null && (fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo || fatura.Cliente.GrupoPessoas == null))
            {
                if (fatura.Cliente.GrupoPessoas == null || fatura.Cliente.GrupoPessoas.ExigeCanhotoFisico == false || fatura.Cliente.GrupoPessoas.NaoGerarFaturaAteReceberCanhotos == false)
                    return null;
            }
            else if (fatura.GrupoPessoas != null && (fatura.GrupoPessoas.ExigeCanhotoFisico == false || fatura.GrupoPessoas.NaoGerarFaturaAteReceberCanhotos == null))
                return null;
            else if (fatura.Cliente != null && (fatura.Cliente.ExigeCanhotoFisico == false || fatura.Cliente.NaoGerarFaturaAteReceberCanhotos == null))
                return null;
            else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && fatura.Cliente == null && fatura.GrupoPessoas == null)
                return null;

            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);

            List<string> numerosDocumentos = repFaturaDocumento.ObterDocumentosComCanhotoPendente(fatura.Codigo);

            if (numerosDocumentos.Count > 0)
                return $"Existem documentos com canhotos pendentes, não sendo possível fechar a fatura. Documentos: {string.Join(", ", numerosDocumentos)}.";

            return null;
        }

        public string ValidarOcorrenciaFinalizadoraCTes(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (fatura.GrupoPessoas != null && (fatura.GrupoPessoas.FaturarSomenteOcorrenciasFinalizadoras == false || fatura.GrupoPessoas.FaturarSomenteOcorrenciasFinalizadoras == null))
                return "";
            if (fatura.Cliente != null && (fatura.Cliente.FaturarSomenteOcorrenciasFinalizadoras == false || fatura.Cliente.FaturarSomenteOcorrenciasFinalizadoras == null))
                return "";
            if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
            {
                if (!(fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.FaturarSomenteOcorrenciasFinalizadoras ?? false))
                    return "";
            }
            else if (fatura.Cliente != null && (fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo || fatura.Cliente.GrupoPessoas == null))
            {
                if (fatura.Cliente.GrupoPessoas == null || fatura.Cliente.GrupoPessoas.FaturarSomenteOcorrenciasFinalizadoras == false)
                    return "";
            }
            else if (fatura.GrupoPessoas != null && (fatura.GrupoPessoas.FaturarSomenteOcorrenciasFinalizadoras == false || fatura.GrupoPessoas.FaturarSomenteOcorrenciasFinalizadoras == null))
                return "";
            else if (fatura.Cliente != null && (fatura.Cliente.FaturarSomenteOcorrenciasFinalizadoras == false || fatura.Cliente.FaturarSomenteOcorrenciasFinalizadoras == null))
                return "";
            else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && fatura.Cliente == null && fatura.GrupoPessoas == null)
                return null;

            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(fatura.Codigo);
            if (documentos.Count > 0)
            {
                bool contemOcorrenciaFinalizadora = false;
                string retorno = string.Empty;

                for (int i = 0; i < documentos.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaDocumento documento = documentos[i];

                    if (documento.Documento?.CTe != null && documento.Documento?.ModeloDocumentoFiscal != null && documento.Documento?.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento && documento.Documento.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS && !repCargaOcorrenciaDocumento.ContemOcorrenciaFinalizadora(documento.Documento.CTe.Codigo))
                    {
                        contemOcorrenciaFinalizadora = true;

                        retorno += (i + 1).ToString() + "º - CT-e: " + documentos[i].Documento.CTe.Numero.ToString() + " Série: " + documentos[i].Documento.CTe.Serie.Numero.ToString() + " Valor: " + documentos[i].Documento.CTe.ValorAReceber.ToString("n2") + " </br>";
                    }
                }

                if (contemOcorrenciaFinalizadora)
                    return "Existem os seguintes conhecimentos sem ocorrência finalizadora: </br>" + retorno + "</br>Verifique estes documentos antes de fechar a fatura.";

                return "";
            }

            return "";
        }

        public void ProcessarFaturasEmFechamento(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);

            //List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturas = repFatura.BuscarPorSituacao(SituacaoFatura.EmFechamento, "DataFechamento", "asc", 0, 3);
            List<int> codigosFaturas = repFatura.BuscarCodigosPorSituacao(SituacaoFatura.EmFechamento, "Codigo", "asc", 0, 3);

            for (var i = 0; i < codigosFaturas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigosFaturas[i]);

                try
                {
                    if (!FinalizarFatura(out string erro, fatura, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, auditado))
                        throw new Exception(erro);
                }
                catch (Exception ex)
                {
                    unidadeTrabalho.Rollback();
                    Servicos.Log.TratarErro(ex);
                }


                unidadeTrabalho.FlushAndClear();
            }
        }

        public void ProcessarFaturasEmCancelamento(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);

                List<int> codigosFaturas = repFatura.BuscarCodigosPorSituacao(SituacaoFatura.EmCancelamento, 3);

                for (var i = 0; i < codigosFaturas.Count; i++)
                {
                    if (!CancelarFatura(out string erro, codigosFaturas[i], unidadeTrabalho, stringConexao, tipoServicoMultisoftware))
                        throw new Exception(erro);

                    unidadeTrabalho.FlushAndClear();
                }

            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public static List<string> BuscarEmailAcordoFaturamentoTomadorFatura(List<int> codigosFatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordo = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem repEmailCabotagem = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso repEmailLongoCurso = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra repEmailCustoExtra = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);

            List<string> emails = new List<string>();
            List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturas = repFatura.BuscarPorCodigo(codigosFatura);
            if (faturas != null && faturas.Count > 0)
            {
                Dominio.Entidades.Embarcador.Fatura.Fatura primeiraFatura = faturas.FirstOrDefault();
                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoCliente = null;
                if (primeiraFatura.Cliente != null)
                {
                    acordoCliente = repAcordo.BuscarAcordoCliente(primeiraFatura.Cliente.CPF_CNPJ, 0);
                    if (acordoCliente == null && primeiraFatura.Cliente.GrupoPessoas != null)
                        acordoCliente = repAcordo.BuscarAcordoCliente(0, primeiraFatura.Cliente.GrupoPessoas.Codigo);
                }
                if (acordoCliente == null && primeiraFatura.GrupoPessoas != null)
                    acordoCliente = repAcordo.BuscarAcordoCliente(0, primeiraFatura.GrupoPessoas.Codigo);

                Dominio.Entidades.Cliente pessoaAcordoFaturamento = null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasAcordoFaturamento = null;
                pessoaAcordoFaturamento = primeiraFatura.Cliente;
                grupoPessoasAcordoFaturamento = primeiraFatura.GrupoPessoas;
                if (pessoaAcordoFaturamento == null)
                    pessoaAcordoFaturamento = primeiraFatura.ClienteTomadorFatura;
                if (grupoPessoasAcordoFaturamento == null)
                    grupoPessoasAcordoFaturamento = primeiraFatura.Cliente?.GrupoPessoas;
                if (grupoPessoasAcordoFaturamento == null)
                    grupoPessoasAcordoFaturamento = primeiraFatura.ClienteTomadorFatura?.GrupoPessoas;

                if (acordoCliente != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repFaturaDocumento.BuscarPrimeiroCargaPedido(primeiraFatura.Codigo);
                    if (primeiroCargaPedido != null)
                    {
                        if (primeiroCargaPedido.Carga.TipoOperacao != null && primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado)
                        {
                            if (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                            {
                                if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                                {
                                    List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> emailsLongoCurso = null;
                                    if (pessoaAcordoFaturamento != null)
                                        emailsLongoCurso = repEmailLongoCurso.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                    if (grupoPessoasAcordoFaturamento != null && emailsLongoCurso == null)
                                        emailsLongoCurso = repEmailLongoCurso.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                    if (emailsLongoCurso != null && emailsLongoCurso.Count > 0)
                                    {
                                        foreach (var emailLongoCurso in emailsLongoCurso)
                                        {
                                            if (!string.IsNullOrWhiteSpace(emailLongoCurso.Email))
                                                emails.AddRange(emailLongoCurso.Email.Split(';').ToList());
                                        }
                                    }
                                    else if (!string.IsNullOrWhiteSpace(acordoCliente.LongoCursoEmail))
                                        emails.AddRange(acordoCliente.LongoCursoEmail.Split(';').ToList());
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.LongoCursoEmail))
                                    emails.AddRange(acordoCliente.LongoCursoEmail.Split(';').ToList());
                            }
                            if (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                            {
                                if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                                {
                                    List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> emailsCustoExtra = null;
                                    if (pessoaAcordoFaturamento != null)
                                        emailsCustoExtra = repEmailCustoExtra.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                    if (grupoPessoasAcordoFaturamento != null && emailsCustoExtra == null)
                                        emailsCustoExtra = repEmailCustoExtra.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                    if (emailsCustoExtra != null && emailsCustoExtra.Count > 0)
                                    {
                                        foreach (var emailCustoExtra in emailsCustoExtra)
                                        {
                                            if (!string.IsNullOrWhiteSpace(emailCustoExtra.Email))
                                                emails.AddRange(emailCustoExtra.Email.Split(';').ToList());
                                        }
                                    }
                                    else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                                        emails.AddRange(acordoCliente.CustoExtraEmail.Split(';').ToList());
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                                    emails.AddRange(acordoCliente.CustoExtraEmail.Split(';').ToList());
                            }
                        }
                        else if (primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS)
                        {
                            if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                            {
                                List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> emailsLongoCurso = null;
                                if (pessoaAcordoFaturamento != null)
                                    emailsLongoCurso = repEmailLongoCurso.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                if (grupoPessoasAcordoFaturamento != null && emailsLongoCurso == null)
                                    emailsLongoCurso = repEmailLongoCurso.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                if (emailsLongoCurso != null && emailsLongoCurso.Count > 0)
                                {
                                    foreach (var emailLongoCurso in emailsLongoCurso)
                                    {
                                        if (!string.IsNullOrWhiteSpace(emailLongoCurso.Email))
                                            emails.AddRange(emailLongoCurso.Email.Split(';').ToList());
                                    }
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.LongoCursoEmail))
                                    emails.AddRange(acordoCliente.LongoCursoEmail.Split(';').ToList());
                            }
                            else if (!string.IsNullOrWhiteSpace(acordoCliente.LongoCursoEmail))
                                emails.AddRange(acordoCliente.LongoCursoEmail.Split(';').ToList());


                        }
                        else if ((primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TAkePayCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TakePayFeeder || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Feeder))
                        {
                            if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                            {
                                List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem> emailsCabotagem = null;
                                if (pessoaAcordoFaturamento != null)
                                    emailsCabotagem = repEmailCabotagem.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                if (grupoPessoasAcordoFaturamento != null && emailsCabotagem == null)
                                    emailsCabotagem = repEmailCabotagem.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                if (emailsCabotagem != null && emailsCabotagem.Count > 0)
                                {
                                    foreach (var emailCabotagem in emailsCabotagem)
                                    {
                                        if (!string.IsNullOrWhiteSpace(emailCabotagem.Email))
                                            emails.AddRange(emailCabotagem.Email.Split(';').ToList());
                                    }
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.CabotagemEmail))
                                    emails.AddRange(acordoCliente.CabotagemEmail.Split(';').ToList());
                            }
                            else if (!string.IsNullOrWhiteSpace(acordoCliente.CabotagemEmail))
                                emails.AddRange(acordoCliente.CabotagemEmail.Split(';').ToList());
                        }
                        else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                        {
                            if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                            {
                                List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> emailsCustoExtra = null;
                                if (pessoaAcordoFaturamento != null)
                                    emailsCustoExtra = repEmailCustoExtra.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                if (grupoPessoasAcordoFaturamento != null && emailsCustoExtra == null)
                                    emailsCustoExtra = repEmailCustoExtra.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                if (emailsCustoExtra != null && emailsCustoExtra.Count > 0)
                                {
                                    foreach (var emailCustoExtra in emailsCustoExtra)
                                    {
                                        if (!string.IsNullOrWhiteSpace(emailCustoExtra.Email))
                                            emails.AddRange(emailCustoExtra.Email.Split(';').ToList());
                                    }
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                                    emails.AddRange(acordoCliente.CustoExtraEmail.Split(';').ToList());
                            }
                            else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                                emails.AddRange(acordoCliente.CustoExtraEmail.Split(';').ToList());
                        }
                    }
                }
            }

            if (emails != null && emails.Count > 0)
                emails = emails.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            return emails;
        }

        public static void EnviarFaturaLote(List<int> codigosFatura, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiTransacao = false;
            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            else
                possuiTransacao = true;

            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            string mensagemErro = string.Empty;
            try
            {
                if (!possuiTransacao)
                    unitOfWork.Start();

                Servicos.DACTE svcDACTE = new Servicos.DACTE(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordo = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem repEmailCabotagem = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem(unitOfWork);
                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso repEmailLongoCurso = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso(unitOfWork);
                Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra repEmailCustoExtra = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Integracao.ArquivoIntegracao repArquivoIntegracao = new Repositorio.Embarcador.Integracao.ArquivoIntegracao(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();


                List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturas = repFatura.BuscarPorCodigo(codigosFatura);
                if (faturas == null || faturas.Count == 0)
                {
                    mensagemErro = "Nenhuma fatura selecionada.";
                    if (faturaIntegracao != null)
                    {
                        faturaIntegracao.MensagemRetorno = mensagemErro;
                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        faturaIntegracao.DataEnvio = DateTime.Now;
                        faturaIntegracao.Tentativas++;

                        repFaturaIntegracao.Atualizar(faturaIntegracao);
                    }
                    if (!possuiTransacao)
                        unitOfWork.CommitChanges();
                    return;
                }
                if (email == null)
                {
                    mensagemErro = "Não existe configuração de e-mail ativa para envio dos documentos.";
                    if (faturaIntegracao != null)
                    {
                        faturaIntegracao.MensagemRetorno = mensagemErro;
                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        faturaIntegracao.DataEnvio = DateTime.Now;
                        faturaIntegracao.Tentativas++;

                        repFaturaIntegracao.Atualizar(faturaIntegracao);
                    }
                    if (!possuiTransacao)
                        unitOfWork.CommitChanges();
                    return;
                }


                Dominio.Entidades.Embarcador.Fatura.Fatura primeiraFatura = faturas.FirstOrDefault();

                Servicos.Log.TratarErro("Enviando fatura " + primeiraFatura.Numero.ToString(), "EnvioEmailFatura");

                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                List<string> emails = new List<string>();
                string observacaoFatura = "";
                string assuntoEmailFatura = "";
                string corpoEmailFatura = "";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura tipoEnvioFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.Todos;
                bool enviarArquivosDescompactados = false;
                bool naoEnviarEmailFaturaAutomaticamente = false;

                Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoCliente = null;
                if (primeiraFatura.Cliente != null)
                {
                    acordoCliente = repAcordo.BuscarAcordoCliente(primeiraFatura.Cliente.CPF_CNPJ, 0);
                    if (acordoCliente == null && primeiraFatura.Cliente.GrupoPessoas != null)
                        acordoCliente = repAcordo.BuscarAcordoCliente(0, primeiraFatura.Cliente.GrupoPessoas.Codigo);
                }
                if (acordoCliente == null && primeiraFatura.GrupoPessoas != null)
                    acordoCliente = repAcordo.BuscarAcordoCliente(0, primeiraFatura.GrupoPessoas.Codigo);

                Dominio.Entidades.Cliente pessoaAcordoFaturamento = null;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasAcordoFaturamento = null;
                pessoaAcordoFaturamento = primeiraFatura.Cliente;
                grupoPessoasAcordoFaturamento = primeiraFatura.GrupoPessoas;
                if (pessoaAcordoFaturamento == null)
                    pessoaAcordoFaturamento = primeiraFatura.ClienteTomadorFatura;
                if (grupoPessoasAcordoFaturamento == null)
                    grupoPessoasAcordoFaturamento = primeiraFatura.Cliente?.GrupoPessoas;
                if (grupoPessoasAcordoFaturamento == null)
                    grupoPessoasAcordoFaturamento = primeiraFatura.ClienteTomadorFatura?.GrupoPessoas;

                if (acordoCliente != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repFaturaDocumento.BuscarPrimeiroCargaPedido(primeiraFatura.Codigo);
                    if (primeiroCargaPedido != null)
                    {
                        if (primeiroCargaPedido.Carga.TipoOperacao != null && primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento.ToString() != "0" && primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento != TipoAcordoFaturamento.NaoInformado
                            && (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso || primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra))
                        {
                            if (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.FreteLongoCurso)
                            {
                                naoEnviarEmailFaturaAutomaticamente = acordoCliente.LongoCursoNaoEnviarEmailFaturaAutomaticamente;
                                if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                                {
                                    List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> emailsLongoCurso = null;
                                    if (pessoaAcordoFaturamento != null)
                                        emailsLongoCurso = repEmailLongoCurso.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                    if (grupoPessoasAcordoFaturamento != null && emailsLongoCurso == null)
                                        emailsLongoCurso = repEmailLongoCurso.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                    if (emailsLongoCurso != null && emailsLongoCurso.Count > 0)
                                    {
                                        foreach (var emailLongoCurso in emailsLongoCurso)
                                        {
                                            if (!string.IsNullOrWhiteSpace(emailLongoCurso.Email))
                                                emails.AddRange(emailLongoCurso.Email.Split(';').ToList());
                                        }
                                    }
                                    else if (!string.IsNullOrWhiteSpace(acordoCliente.LongoCursoEmail))
                                        emails.AddRange(acordoCliente.LongoCursoEmail.Split(';').ToList());
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.LongoCursoEmail))
                                    emails.AddRange(acordoCliente.LongoCursoEmail.Split(';').ToList());
                            }
                            if (primeiroCargaPedido.Carga.TipoOperacao.ConfiguracaoCarga?.AcordoFaturamento == TipoAcordoFaturamento.CustoExtra)
                            {
                                naoEnviarEmailFaturaAutomaticamente = acordoCliente.CustoExtraNaoEnviarEmailFaturaAutomaticamente;
                                if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                                {
                                    List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> emailsCustoExtra = null;
                                    if (pessoaAcordoFaturamento != null)
                                        emailsCustoExtra = repEmailCustoExtra.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                    if (grupoPessoasAcordoFaturamento != null && emailsCustoExtra == null)
                                        emailsCustoExtra = repEmailCustoExtra.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                    if (emailsCustoExtra != null && emailsCustoExtra.Count > 0)
                                    {
                                        foreach (var emailCustoExtra in emailsCustoExtra)
                                        {
                                            if (!string.IsNullOrWhiteSpace(emailCustoExtra.Email))
                                                emails.AddRange(emailCustoExtra.Email.Split(';').ToList());
                                        }
                                    }
                                    else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                                        emails.AddRange(acordoCliente.CustoExtraEmail.Split(';').ToList());
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                                    emails.AddRange(acordoCliente.CustoExtraEmail.Split(';').ToList());
                            }
                        }
                        else if (primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS)
                        {
                            naoEnviarEmailFaturaAutomaticamente = acordoCliente.LongoCursoNaoEnviarEmailFaturaAutomaticamente;
                            if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                            {
                                List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso> emailsLongoCurso = null;
                                if (pessoaAcordoFaturamento != null)
                                    emailsLongoCurso = repEmailLongoCurso.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                if (grupoPessoasAcordoFaturamento != null && emailsLongoCurso == null)
                                    emailsLongoCurso = repEmailLongoCurso.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                if (emailsLongoCurso != null && emailsLongoCurso.Count > 0)
                                {
                                    foreach (var emailLongoCurso in emailsLongoCurso)
                                    {
                                        if (!string.IsNullOrWhiteSpace(emailLongoCurso.Email))
                                            emails.AddRange(emailLongoCurso.Email.Split(';').ToList());
                                    }
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.LongoCursoEmail))
                                    emails.AddRange(acordoCliente.LongoCursoEmail.Split(';').ToList());
                            }
                            else if (!string.IsNullOrWhiteSpace(acordoCliente.LongoCursoEmail))
                                emails.AddRange(acordoCliente.LongoCursoEmail.Split(';').ToList());


                        }
                        else if ((primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TakePayFeeder || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TAkePayCabotagem || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada || primeiroCargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Feeder))
                        {
                            naoEnviarEmailFaturaAutomaticamente = acordoCliente.CabotagemNaoEnviarEmailFaturaAutomaticamente;
                            if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                            {
                                List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem> emailsCabotagem = null;
                                if (pessoaAcordoFaturamento != null)
                                    emailsCabotagem = repEmailCabotagem.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                if (grupoPessoasAcordoFaturamento != null && emailsCabotagem == null)
                                    emailsCabotagem = repEmailCabotagem.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                if (emailsCabotagem != null && emailsCabotagem.Count > 0)
                                {
                                    foreach (var emailCabotagem in emailsCabotagem)
                                    {
                                        if (!string.IsNullOrWhiteSpace(emailCabotagem.Email))
                                            emails.AddRange(emailCabotagem.Email.Split(';').ToList());
                                    }
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.CabotagemEmail))
                                    emails.AddRange(acordoCliente.CabotagemEmail.Split(';').ToList());
                            }
                            else if (!string.IsNullOrWhiteSpace(acordoCliente.CabotagemEmail))
                                emails.AddRange(acordoCliente.CabotagemEmail.Split(';').ToList());
                        }
                        else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                        {
                            naoEnviarEmailFaturaAutomaticamente = acordoCliente.CustoExtraNaoEnviarEmailFaturaAutomaticamente;
                            if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                            {
                                List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra> emailsCustoExtra = null;
                                if (pessoaAcordoFaturamento != null)
                                    emailsCustoExtra = repEmailCustoExtra.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                                if (grupoPessoasAcordoFaturamento != null && emailsCustoExtra == null)
                                    emailsCustoExtra = repEmailCustoExtra.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                                if (emailsCustoExtra != null && emailsCustoExtra.Count > 0)
                                {
                                    foreach (var emailCustoExtra in emailsCustoExtra)
                                    {
                                        if (!string.IsNullOrWhiteSpace(emailCustoExtra.Email))
                                            emails.AddRange(emailCustoExtra.Email.Split(';').ToList());
                                    }
                                }
                                else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                                    emails.AddRange(acordoCliente.CustoExtraEmail.Split(';').ToList());
                            }
                            else if (!string.IsNullOrWhiteSpace(acordoCliente.CustoExtraEmail))
                                emails.AddRange(acordoCliente.CustoExtraEmail.Split(';').ToList());
                        }
                    }
                }

                Dominio.Entidades.Cliente tomador = primeiraFatura.Cliente;
                if (tomador == null)
                    tomador = primeiraFatura.ClienteTomadorFatura;

                if (!configuracaoTMS.UtilizaEmissaoMultimodal)
                {
                    if (!string.IsNullOrWhiteSpace(primeiraFatura?.ClienteTomadorFatura?.EmailFatura))
                        emails.AddRange(primeiraFatura.ClienteTomadorFatura.EmailFatura.Split(';').ToList());
                    else if (!string.IsNullOrWhiteSpace(primeiraFatura?.ClienteTomadorFatura?.GrupoPessoas?.EmailFatura))
                        emails.AddRange(primeiraFatura.ClienteTomadorFatura.GrupoPessoas.EmailFatura.Split(';').ToList());
                }

                if (tomador != null)
                {
                    if (!configuracaoTMS.UtilizaEmissaoMultimodal)
                    {
                        if (!string.IsNullOrWhiteSpace(tomador.EmailFatura))
                            emails.AddRange(tomador.EmailFatura.Split(';').ToList());
                        else if (!string.IsNullOrWhiteSpace(tomador.GrupoPessoas?.EmailFatura))
                            emails.AddRange(tomador.GrupoPessoas.EmailFatura.Split(';').ToList());

                        if (!string.IsNullOrWhiteSpace(tomador.Email))
                            emails.AddRange(tomador.Email.Split(';').ToList());

                        for (int a = 0; a < tomador.Emails.Count; a++)
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = tomador.Emails[a];
                            if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A"
                                && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Administrativo)
                                emails.Add(outroEmail.Email);
                        }
                    }
                    if (primeiraFatura != null && primeiraFatura.Carga != null && primeiraFatura.Carga.CargaTakeOrPay)
                    {
                        int qtdDisponibilizadas = repCargaPedido.BuscarQuantidadeDisponibilizadas(primeiraFatura.Carga.Codigo);
                        int qtdNaoEmbarcadas = repCargaPedido.BuscarQuantidadeNaoEmbarcadas(primeiraFatura.Carga.Codigo);

                        if (
                            primeiraFatura.Carga.TipoOperacao != null &&
                            (
                            primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.DemurrageCabotagem ||
                            primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.DetentionCabotagem ||
                            primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.NotaDebito
                            )
                           )
                        {
                            bool layoutEmailTipoPropostaTipoOperacao = (primeiraFatura.Carga?.TipoOperacao?.ConfiguracaoCarga?.LayoutEmailTipoPropostaTipoOperacao ?? false);
                            string tipoTakeOrPay = "Embarque Certo";
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.DemurrageCabotagem)
                                tipoTakeOrPay = "Demurrage - Cabotagem";
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.DetentionCabotagem)
                                tipoTakeOrPay = "Detention - Cabotagem";
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.NotaDebito && layoutEmailTipoPropostaTipoOperacao)
                                tipoTakeOrPay = "Nota de Débito";

                            (assuntoEmailFatura, corpoEmailFatura) = EmailTemplatePadrao();

                            string tipoOperacao = primeiraFatura.Carga?.TipoOperacao?.Descricao ?? string.Empty;

                            string assuntoFormatado = layoutEmailTipoPropostaTipoOperacao
                            ? $"{tipoTakeOrPay} – {tipoOperacao} – {tomador.Nome}"
                            : $"{tipoTakeOrPay} – {tomador.Nome}";

                            string corpoFormatado = layoutEmailTipoPropostaTipoOperacao
                            ? $" de {tipoTakeOrPay} – {tipoOperacao}"
                            : $" de {tipoTakeOrPay}";

                            assuntoEmailFatura = assuntoEmailFatura.Replace("{assunto}", assuntoFormatado);
                            corpoEmailFatura = corpoEmailFatura.Replace("{de}", corpoFormatado);
                            corpoEmailFatura = corpoEmailFatura.Replace("{cliente}", tomador.Nome);
                        }
                        else
                        {
                            string tipoTakeOrPay = "Embarque Certo";
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.NoShowCabotagem)
                                tipoTakeOrPay = "No Show";
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                                tipoTakeOrPay = "Faturamento - Contabilidade";
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.DemurrageCabotagem)
                                tipoTakeOrPay = "Demurrage - Cabotagem";
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.DetentionCabotagem)
                                tipoTakeOrPay = "Detention - Cabotagem";
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.NotaDebito && (primeiraFatura.Carga?.TipoOperacao?.ConfiguracaoCarga?.LayoutEmailTipoPropostaTipoOperacao ?? false))
                                tipoTakeOrPay = "Nota de Débito";

                            assuntoEmailFatura = "Faturamento Aliança " + tipoTakeOrPay
                                + " – " + (primeiraFatura.Carga.PedidoViagemNavio?.Descricao ?? "")
                                + " - " + (primeiraFatura.Carga.PortoOrigem?.Descricao ?? "")
                                + " – " + tomador.Nome;
                            if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.NoShowCabotagem)
                                assuntoEmailFatura += " - Fatura " + primeiraFatura.Numero.ToString("D");
                            else if (primeiraFatura.Carga.TipoOperacao != null && primeiraFatura.Carga.TipoOperacao.TipoPropostaMultimodal == TipoPropostaMultimodal.FaturamentoContabilidade)
                                assuntoEmailFatura += " - Fatura " + primeiraFatura.Numero.ToString("D");

                            corpoEmailFatura = "Prezado " + tomador.Nome + ", <br/>";
                            corpoEmailFatura += "Segue anexo faturamento da Aliança Navegação e Logistica Ltda referente a cobrança do " + tipoTakeOrPay + ". <br/>";
                            corpoEmailFatura += "<br/>";

                            corpoEmailFatura += "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>";
                            corpoEmailFatura += "<script src='http://www.developerdan.com/table-to-json/javascripts/jquery.tabletojson.min.js'></script>";
                            corpoEmailFatura += "<table style='width:100%; align='center'; border='1';>";
                            corpoEmailFatura += "<tr>";
                            corpoEmailFatura += "<th>Navio/Viagem/Direção</th>";
                            corpoEmailFatura += "<th>Porto de Origem</th>";
                            corpoEmailFatura += "<th>Quantidade de unidades disponibilizadas</th>";
                            corpoEmailFatura += "<th>Quantidade de unidades não embarcadas</th>";
                            corpoEmailFatura += "<th>Observação</th>";
                            corpoEmailFatura += "</tr>";

                            corpoEmailFatura += "<tr>";
                            corpoEmailFatura += "<td>" + (primeiraFatura.Carga.PedidoViagemNavio?.Descricao ?? "") + "</td>";
                            corpoEmailFatura += "<td>" + (primeiraFatura.Carga.PortoOrigem?.Descricao ?? "") + "</td>";
                            corpoEmailFatura += "<td>" + (Utilidades.String.OnlyNumbers(qtdDisponibilizadas.ToString("n0"))) + "</td>";
                            corpoEmailFatura += "<td>" + (Utilidades.String.OnlyNumbers(qtdNaoEmbarcadas.ToString("n0"))) + "</td>";
                            corpoEmailFatura += "<td>" + (!string.IsNullOrEmpty(primeiraFatura.Carga.ObservacaoParaFaturamento) ? primeiraFatura.Carga.ObservacaoParaFaturamento : "") + "</td>";
                            corpoEmailFatura += "</tr>";

                            corpoEmailFatura += "</table>";
                        }

                        observacaoFatura = tomador.GrupoPessoas?.ObservacaoFatura ?? tomador.ObservacaoFatura;
                        tipoEnvioFatura = tomador.GrupoPessoas != null && tomador.GrupoPessoas.TipoEnvioFatura != null ? tomador.GrupoPessoas.TipoEnvioFatura.Value : tomador.TipoEnvioFatura != null ? tomador.TipoEnvioFatura.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.Todos;
                        enviarArquivosDescompactados = tomador.GrupoPessoas?.EnviarArquivosDescompactados ?? tomador.EnviarArquivosDescompactados;
                    }
                    else if (tomador.GrupoPessoas != null && !string.IsNullOrWhiteSpace(tomador.GrupoPessoas.AssuntoEmailFatura))
                    {
                        observacaoFatura = tomador.GrupoPessoas.ObservacaoFatura;
                        assuntoEmailFatura = tomador.GrupoPessoas.AssuntoEmailFatura;
                        corpoEmailFatura = tomador.GrupoPessoas.CorpoEmailFatura;
                        tipoEnvioFatura = tomador.GrupoPessoas.TipoEnvioFatura != null ? tomador.GrupoPessoas.TipoEnvioFatura.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.Todos;
                        enviarArquivosDescompactados = tomador.GrupoPessoas.EnviarArquivosDescompactados;
                    }
                    else
                    {
                        observacaoFatura = tomador.ObservacaoFatura;
                        assuntoEmailFatura = tomador.AssuntoEmailFatura;
                        corpoEmailFatura = tomador.CorpoEmailFatura;
                        tipoEnvioFatura = tomador.TipoEnvioFatura != null ? tomador.TipoEnvioFatura.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.Todos;
                        enviarArquivosDescompactados = tomador.EnviarArquivosDescompactados;
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = primeiraFatura.GrupoPessoas;

                    if (!configuracaoTMS.UtilizaEmissaoMultimodal)
                    {
                        if (!string.IsNullOrWhiteSpace(grupoPessoas.EmailFatura))
                            emails.AddRange(grupoPessoas.EmailFatura.Split(';').ToList());

                        if (!string.IsNullOrWhiteSpace(grupoPessoas.Email))
                            emails.AddRange(grupoPessoas.Email.Split(';').ToList());
                    }
                    observacaoFatura = grupoPessoas.ObservacaoFatura;
                    assuntoEmailFatura = grupoPessoas.AssuntoEmailFatura;
                    corpoEmailFatura = grupoPessoas.CorpoEmailFatura;
                    tipoEnvioFatura = grupoPessoas.TipoEnvioFatura != null ? grupoPessoas.TipoEnvioFatura.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.Todos;
                    enviarArquivosDescompactados = grupoPessoas.EnviarArquivosDescompactados;
                }
                if (primeiraFatura.Carga?.TipoOperacao?.ConfiguracaoImpressao?.AlterarLayoutDaFaturaIncluirTipoServico ?? false)
                {
                    (assuntoEmailFatura, corpoEmailFatura) = EmailTemplatePadrao();

                    string tipoFaturamento = primeiraFatura.Carga?.TipoOperacao?.TipoPropostaMultimodal.ObterDescricao() + "-" + primeiraFatura.Carga?.TipoOperacao?.Descricao;

                    assuntoEmailFatura = assuntoEmailFatura.Replace("{assunto}", $"{tipoFaturamento} - {tomador.Nome}");
                    corpoEmailFatura = corpoEmailFatura.Replace("{cliente}", tomador.Nome);
                    corpoEmailFatura = corpoEmailFatura.Replace("{de}", " " + tipoFaturamento);
                }
                foreach (var codigoFatura in codigosFatura)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                    List<byte[]> sourceFiles = new List<byte[]>();

                    if (naoEnviarEmailFaturaAutomaticamente && faturaIntegracao != null)
                    {
                        mensagemErro = "Está configurado no tomador/grupo da fatura para não realizar o envio automático.";
                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        faturaIntegracao.MensagemRetorno = mensagemErro;
                        faturaIntegracao.DataEnvio = DateTime.Now;
                        faturaIntegracao.Tentativas++;

                        repFaturaIntegracao.Atualizar(faturaIntegracao);

                        serCargaDadosSumarizados.AtualizarDadosCTesFaturadosIntegrados(faturaIntegracao.Fatura.Codigo, unitOfWork);

                        if (!possuiTransacao)
                            unitOfWork.CommitChanges();
                        return;
                    }

                    emails = emails.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                    if (emails == null || emails.Count == 0)
                    {
                        mensagemErro = "E-mail para envio dos documentos é inválido.";
                        if (configuracaoTMS.UtilizaEmissaoMultimodal)
                            mensagemErro += " Não foi encontrado nenhum e-mail no cadastro de Acordo de Faturamento";
                        if (faturaIntegracao != null)
                        {
                            faturaIntegracao.MensagemRetorno = mensagemErro;
                            faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            faturaIntegracao.DataEnvio = DateTime.Now;
                            faturaIntegracao.Tentativas++;

                            repFaturaIntegracao.Atualizar(faturaIntegracao);
                        }
                        if (!possuiTransacao)
                            unitOfWork.CommitChanges();
                        return;
                    }


                    string nomeArquivo = "";
                    bool contemBoleto = false;
                    byte[] data = null;

                    if (tipoEnvioFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.Todos || tipoEnvioFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.SomenteFatura || tipoEnvioFatura == TipoEnvioFatura.CTeFaturaSemXML || tipoEnvioFatura == TipoEnvioFatura.PDFCTeFaturaAgrupado || tipoEnvioFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.TodosOsDocumentosDaFatura
                        || tipoEnvioFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.EnviarTodosDocumentosPDFNFSe)
                    {
                        List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> parcelas = repFaturaParcela.BuscarPorFatura(codigoFatura);
                        foreach (var parcela in parcelas)
                        {
                            Servicos.Log.TratarErro("Cod Titulo " + parcela.CodigoTitulo.ToString(), "EnvioEmailFatura");

                            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(parcela.CodigoTitulo);
                            if (!configuracaoTMS.UtilizaEmissaoMultimodal && titulo == null)
                            {
                                Servicos.Log.TratarErro("Cod Titulo " + parcela.CodigoTitulo.ToString() + " título ainda não gerado.", "EnvioEmailFatura");
                                mensagemErro = "Título ainda não gerado, favor aguarde o fechamento da fatura.";
                                if (faturaIntegracao != null)
                                {
                                    faturaIntegracao.MensagemRetorno = mensagemErro;
                                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                                    faturaIntegracao.DataEnvio = DateTime.Now;
                                    faturaIntegracao.Tentativas++;

                                    if (faturaIntegracao.Tentativas >= 50)
                                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                                    repFaturaIntegracao.Atualizar(faturaIntegracao);
                                    System.Threading.Thread.Sleep(5000);
                                }
                                if (!possuiTransacao)
                                    unitOfWork.CommitChanges();
                                return;
                            }

                            if ((configuracaoTMS.UtilizaEmissaoMultimodal) && (titulo == null || titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.EmGeracao || titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido))
                            {
                                mensagemErro = $"O boleto está sendo gerado, favor aguarde o envio automatico. [Bloco 1] Erros : Cod Titulo: {parcela.CodigoTitulo}, UtilizaEmissaoMultimodal: {configuracaoTMS.UtilizaEmissaoMultimodal}, TituloNull: {titulo == null}, BoletoStatus: {titulo?.BoletoStatusTitulo}";
                                if (faturaIntegracao != null)
                                {
                                    faturaIntegracao.MensagemRetorno = mensagemErro;
                                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                                    faturaIntegracao.DataEnvio = DateTime.Now;
                                    faturaIntegracao.Tentativas++;

                                    repFaturaIntegracao.Atualizar(faturaIntegracao);
                                    System.Threading.Thread.Sleep(5000);
                                }
                                if (!possuiTransacao)
                                    unitOfWork.CommitChanges();
                                return;
                            }
                            else if (!configuracaoTMS.UtilizaEmissaoMultimodal && titulo != null && (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.EmGeracao || titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido))
                            {
                                Servicos.Log.TratarErro("Cod Titulo " + parcela.CodigoTitulo.ToString() + "Fatura:" + codigoFatura + " não tem boleto", "EnvioEmailFatura");
                                mensagemErro = $"O boleto está sendo gerado, favor aguarde o envio automatico. [Bloco 2] Erros : Cod Titulo: {parcela.CodigoTitulo}, UtilizaEmissaoMultimodal: {configuracaoTMS.UtilizaEmissaoMultimodal}, TituloNull: {titulo == null}, BoletoStatus: {titulo?.BoletoStatusTitulo}, Tentativas: {faturaIntegracao?.Tentativas}";
                                if (faturaIntegracao != null)
                                {
                                    faturaIntegracao.MensagemRetorno = mensagemErro;
                                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                                    faturaIntegracao.DataEnvio = DateTime.Now;
                                    faturaIntegracao.Tentativas++;

                                    if (faturaIntegracao.Tentativas >= 50)
                                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                                    repFaturaIntegracao.Atualizar(faturaIntegracao);
                                    System.Threading.Thread.Sleep(5000);
                                }
                                if (!possuiTransacao)
                                    unitOfWork.CommitChanges();
                                return;
                            }
                            else if (!configuracaoTMS.UtilizaEmissaoMultimodal && titulo != null && titulo.BoletoConfiguracao != null && (string.IsNullOrWhiteSpace(titulo.CaminhoBoleto) || !Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto)))
                            {
                                Servicos.Log.TratarErro("Cod Titulo " + parcela.CodigoTitulo.ToString() + "Fatura:" + codigoFatura + " não tem boleto nem caminho", "EnvioEmailFatura");
                                mensagemErro = $"O boleto está sendo gerado, favor aguarde o envio automatico. [Bloco 3] Erros : Cod Titulo: {parcela.CodigoTitulo}, UtilizaEmissaoMultimodal: {configuracaoTMS.UtilizaEmissaoMultimodal}, CaminhoBoleto: {titulo?.CaminhoBoleto}, ExisteArquivo: {Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto)}, Tentativas: {faturaIntegracao?.Tentativas}";
                                if (faturaIntegracao != null)
                                {
                                    faturaIntegracao.MensagemRetorno = mensagemErro;
                                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                                    faturaIntegracao.DataEnvio = DateTime.Now;
                                    faturaIntegracao.Tentativas++;

                                    if (faturaIntegracao.Tentativas >= 50)
                                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                                    repFaturaIntegracao.Atualizar(faturaIntegracao);
                                    System.Threading.Thread.Sleep(5000);
                                }
                                if (!possuiTransacao)
                                    unitOfWork.CommitChanges();
                                return;
                            }

                            if (titulo != null && !string.IsNullOrWhiteSpace(titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(titulo.CaminhoBoleto))
                            {
                                Servicos.Log.TratarErro($"[Bloco 4] Cod Titulo: {parcela.CodigoTitulo}, adicionando boleto, CaminhoBoleto: {titulo.CaminhoBoleto}, UtilizaEmissaoMultimodal: {configuracaoTMS.UtilizaEmissaoMultimodal}, contemBoleto: {contemBoleto}", "EnvioEmailFatura");

                                nomeArquivo = Path.GetFileName(titulo.CaminhoBoleto);
                                data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(titulo.CaminhoBoleto);
                                if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                    conteudoCompactar.Add(nomeArquivo, data);
                                else
                                    Servicos.Log.TratarErro($"[Bloco 4] Cod Titulo: {parcela.CodigoTitulo}, já tem o anexo ou data é nula, DataNull: {data == null}", "EnvioEmailFatura");

                                if (configuracaoTMS.UtilizaEmissaoMultimodal && !(faturaIntegracao?.Fatura?.Carga?.CargaTakeOrPay ?? false))
                                    contemBoleto = true;

                                if (tipoEnvioFatura == TipoEnvioFatura.PDFCTeFaturaAgrupado && data != null)
                                    sourceFiles.Add(data);
                            }
                            else
                                Servicos.Log.TratarErro($"[Bloco 5] Cod Titulo: {parcela.CodigoTitulo}, não tem titulo ou boleto. TituloNull: {titulo == null}, CaminhoBoleto: {titulo?.CaminhoBoleto}, UtilizaEmissaoMultimodal: {configuracaoTMS.UtilizaEmissaoMultimodal}", "EnvioEmailFatura");
                        }

                        if (!contemBoleto || tipoEnvioFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.TodosOsDocumentosDaFatura || tipoEnvioFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.EnviarTodosDocumentosPDFNFSe)
                        {
                            Servicos.Embarcador.Fatura.FaturaImpressao servicoFaturaImpressao = Servicos.Embarcador.Fatura.FaturaImpressaoFactory.Criar(fatura, unitOfWork, tipoServicoMultisoftware);
                            string guidArquivoUltimoRelatorioGerado = servicoFaturaImpressao.ObterGuidArquivoUltimoRelatorioGerado(fatura);
                            if (!string.IsNullOrWhiteSpace(guidArquivoUltimoRelatorioGerado))
                            {
                                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), guidArquivoUltimoRelatorioGerado) + ".pdf";
                                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                    guidArquivoUltimoRelatorioGerado = "";
                            }

                            if (string.IsNullOrWhiteSpace(guidArquivoUltimoRelatorioGerado))
                            {
                                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoFaturaImpressao.ObterRelatorio();
                                Dominio.Entidades.Usuario usuario = fatura.Usuario;
                                if (usuario == null)
                                    usuario = repUsuario.BuscarPorLogin("multisoftware");

                                if (fatura.Carga?.TipoOperacao?.ConfiguracaoImpressao?.AlterarLayoutDaFaturaIncluirTipoServico ?? false)
                                    relatorio.Titulo += " - " + fatura.Carga.TipoOperacao.TipoPropostaMultimodal.ObterDescricao();

                                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = servicoRelatorio.AdicionarRelatorioParaGeracao(relatorio, usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork, fatura.Codigo);
                                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoFaturaImpressao.ObterRelatorioTemporario(relatorio);

                                servicoFaturaImpressao.GerarRelatorio(fatura, relatorioControleGeracao, relatorioTemporario, configuracaoTMS.TipoImpressaoFatura);

                                guidArquivoUltimoRelatorioGerado = servicoFaturaImpressao.ObterGuidArquivoUltimoRelatorioGerado(fatura);
                            }

                            if (!string.IsNullOrWhiteSpace(guidArquivoUltimoRelatorioGerado))
                            {
                                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), guidArquivoUltimoRelatorioGerado) + ".pdf";
                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                {
                                    nomeArquivo = Path.GetFileName(caminhoPDF);
                                    data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                                    if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                        conteudoCompactar.Add(nomeArquivo, data);
                                    if (tipoEnvioFatura == TipoEnvioFatura.PDFCTeFaturaAgrupado && data != null)
                                        sourceFiles.Add(data);
                                }
                            }
                        }
                    }

                    List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(codigoFatura);

                    if (new[]
                    {
                         TipoEnvioFatura.Todos,
                         TipoEnvioFatura.SomenteCTe,
                         TipoEnvioFatura.CTeFaturaSemXML,
                         TipoEnvioFatura.SomenteCTeSemXML,
                         TipoEnvioFatura.PDFCTeFaturaAgrupado,
                         TipoEnvioFatura.TodosOsDocumentosDaFatura,
                         TipoEnvioFatura.EnviarTodosDocumentosPDFNFSe
                    }.Contains(tipoEnvioFatura))
                    {
                        foreach (var documento in documentos)
                        {
                            if (documento.Documento.CTe != null)
                            {
                                nomeArquivo = "";
                                data = null;

                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(documento.Documento.CTe.Codigo);
                                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";

                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                {
                                    nomeArquivo = Path.GetFileName(caminhoPDF);
                                    data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                                    if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                        conteudoCompactar.Add(nomeArquivo, data);
                                    if (tipoEnvioFatura == TipoEnvioFatura.PDFCTeFaturaAgrupado && data != null)
                                        sourceFiles.Add(data);
                                }
                                if (new[]
                                {
                                     TipoEnvioFatura.Todos,
                                     TipoEnvioFatura.SomenteCTe,
                                     TipoEnvioFatura.TodosOsDocumentosDaFatura,
                                     TipoEnvioFatura.EnviarTodosDocumentosPDFNFSe
                                }.Contains(tipoEnvioFatura))
                                {
                                    if (cte.ModeloDocumentoFiscal.Numero == "39")
                                    {
                                        nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";
                                        Servicos.NFSe svcNFSe = new Servicos.NFSe();
                                        data = svcNFSe.ObterXMLAutorizacaoCTe(cte.Codigo, unitOfWork);
                                        if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                            conteudoCompactar.Add(nomeArquivo, data);
                                    }
                                    else
                                    {
                                        nomeArquivo = string.Concat(cte.Chave, ".xml");
                                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                                        data = svcCTe.ObterXMLAutorizacao(cte, unitOfWork);
                                        if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                            conteudoCompactar.Add(nomeArquivo, data);
                                    }
                                }
                                if (tipoEnvioFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura.EnviarTodosDocumentosPDFNFSe)
                                {
                                    data = null;
                                    nomeArquivo = null;

                                    if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                                        nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".pdf";

                                    if (nomeArquivo != null && (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))
                                    {
                                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                                        data = svcNFSe.ObterDANFSECTe(cte.Codigo);

                                        if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                            conteudoCompactar.Add(nomeArquivo, data);
                                    }

                                    if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
                                    {
                                        string nomeDocumento = cte.ModeloDocumentoFiscal.Abreviacao;
                                        nomeArquivo = cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao + ".pdf";

                                        if (!string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal.Relatorio))
                                        {
                                            data = new Servicos.Embarcador.Relatorios.OutrosDocumentos(unitOfWork).ObterPdf(cte);

                                            if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                                conteudoCompactar.Add(nomeArquivo, data);
                                        }
                                    }

                                    data = null;
                                    nomeArquivo = null;
                                    string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;
                                    string nomeArquivoFisico = null;

                                    if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                                        nomeArquivoFisico = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString();

                                    if (configuracaoTMS.GerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                        nomeArquivoFisico += "_Canc";

                                    if (cte.Status == "F")
                                        nomeArquivoFisico += "_FSDA";

                                    nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, cte.Empresa.CNPJ, nomeArquivoFisico) + ".pdf";

                                    if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
                                    {
                                        if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                                        {
                                            if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                                                continue;

                                            data = svcDACTE.GerarPorProcesso(cte.Codigo, null, configuracaoTMS.GerarPDFCTeCancelado);
                                        }
                                        else
                                        {
                                            data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
                                        }
                                    }

                                    if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                        conteudoCompactar.Add(nomeArquivo, data);
                                }
                            }
                        }
                    }

                    if ((tipoEnvioFatura == TipoEnvioFatura.TodosOsDocumentosDaFatura || tipoEnvioFatura == TipoEnvioFatura.EnviarTodosDocumentosPDFNFSe) && primeiraFatura != null)
                    {
                        try
                        {
                            List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> integracoesFatura = repFaturaIntegracao.BuscarPorFatura(primeiraFatura.Codigo);

                            if (integracoesFatura != null && integracoesFatura.Count > 0)
                            {
                                foreach (var integracaoFatura in integracoesFatura)
                                {
                                    if (integracaoFatura != null && integracaoFatura.LayoutEDI != null)
                                    {
                                        using (MemoryStream arquivoEDI = IntegracaoEDI.GerarEDI(integracaoFatura, unitOfWork))
                                        {
                                            Stream stream = new MemoryStream(arquivoEDI.ToArray());
                                            string nomeArquivoEDI = IntegracaoEDI.ObterNomeArquivoEDI(integracoesFatura.FirstOrDefault(), unitOfWork, false);
                                            nomeArquivoEDI = nomeArquivoEDI.Replace("-", "");
                                            data = Utilidades.File.ReadToEnd(stream);
                                            if (data != null && !conteudoCompactar.ContainsKey(nomeArquivo))
                                                conteudoCompactar.Add(nomeArquivoEDI, data);
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            corpoEmailFatura += "<br/><br/>Problemas ao anexar o EDI configurado, por gentileza solicite o arquivo<br/><br/>.";
                        }
                    }

                    if (tipoEnvioFatura == TipoEnvioFatura.PDFCTeFaturaAgrupado && sourceFiles != null && sourceFiles.Count > 0)
                    {
                        byte[] pdfAgrupado = null;
                        if (sourceFiles != null && sourceFiles.Count > 0)
                        {
                            pdfAgrupado = svcDACTE.MergeFiles(sourceFiles);
                            if (pdfAgrupado != null)
                            {
                                conteudoCompactar = new Dictionary<string, byte[]>();
                                conteudoCompactar.Add($"Fatura Nº {Utilidades.String.OnlyNumbers(fatura.Numero.ToString("n0"))} {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}.pdf", pdfAgrupado);
                                foreach (var conteudo in conteudoCompactar)
                                {
                                    Stream stream = new MemoryStream(conteudo.Value);
                                    attachments.Add(new System.Net.Mail.Attachment(stream, conteudo.Key));
                                }
                            }
                        }
                    }
                    else if (enviarArquivosDescompactados)
                    {
                        foreach (var conteudo in conteudoCompactar)
                        {
                            Stream stream = new MemoryStream(conteudo.Value);
                            attachments.Add(new System.Net.Mail.Attachment(stream, conteudo.Key));
                        }
                    }
                    else
                    {
                        MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                        if (arquivoCompactado != null)
                            attachments.Add(new System.Net.Mail.Attachment(arquivoCompactado, $"Fatura Nº {Utilidades.String.OnlyNumbers(fatura.Numero.ToString("n0"))} {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}.zip"));
                    }

                }

                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> todosDocumentos = repFaturaDocumento.BuscarPorFatura(codigosFatura);
                string assunto = "Faturas Nºs " + string.Join(", ", faturas.Select(f => f.Numero).ToList());
                string corpo = "Segue em anexo as faturas de números " + string.Join(", ", faturas.Select(f => f.Numero).ToList()) + ". " + observacaoFatura;

                if (!string.IsNullOrWhiteSpace(assuntoEmailFatura))
                {
                    string datasVencimentosBoletos = string.Join(", ", (from o in primeiraFatura.Parcelas select o.DataVencimento.ToString("dd/MM/yyyy")));

                    assuntoEmailFatura = assuntoEmailFatura.Replace("#NumeroFatura", string.Join(", ", faturas.Select(f => f.Numero).ToList()));
                    assuntoEmailFatura = assuntoEmailFatura.Replace("#ObservacaoFatura", observacaoFatura);
                    assuntoEmailFatura = assuntoEmailFatura.Replace("#DataFatura", primeiraFatura.DataFatura.ToString("dd/MM/yyyy"));
                    assuntoEmailFatura = assuntoEmailFatura.Replace("#DatasVencimentosBoletos", datasVencimentosBoletos);
                    assuntoEmailFatura = assuntoEmailFatura.Replace("#CNPJTomador", primeiraFatura.ClienteTomadorFatura?.CPF_CNPJ_Formatado);
                    assuntoEmailFatura = assuntoEmailFatura.Replace("#NomeTomador", primeiraFatura.ClienteTomadorFatura?.Nome);
                    assuntoEmailFatura = assuntoEmailFatura.Replace("#CNPJEmpresa", primeiraFatura.Empresa?.CNPJ_Formatado);
                    assuntoEmailFatura = assuntoEmailFatura.Replace("#NomeEmpresa", primeiraFatura.Empresa?.RazaoSocial);
                    assuntoEmailFatura = assuntoEmailFatura.Replace("#Viagem", string.Join(", ", faturas.Where(f => f.PedidoViagemNavio != null).Select(f => f.PedidoViagemNavio.Descricao).Distinct().ToList()));
                    if (todosDocumentos != null && todosDocumentos.Count > 0)
                    {
                        assuntoEmailFatura = assuntoEmailFatura.Replace("#PortoOrigem", string.Join(", ", todosDocumentos.Where(f => f.Documento != null && f.Documento.CTe != null && f.Documento.CTe.PortoOrigem != null).Select(f => f.Documento.CTe.PortoOrigem.Descricao).Distinct().ToList()));
                        assuntoEmailFatura = assuntoEmailFatura.Replace("#PortoDestino", string.Join(", ", todosDocumentos.Where(f => f.Documento != null && f.Documento.CTe != null && f.Documento.CTe.PortoDestino != null).Select(f => f.Documento.CTe.PortoDestino.Descricao).Distinct().ToList()));
                        assuntoEmailFatura = assuntoEmailFatura.Replace("#NumeroCTe", string.Join(", ", todosDocumentos.Where(f => f.Documento != null && f.Documento.CTe != null).Select(f => f.Documento.CTe.Numero).Distinct().ToList()));
                    }

                    assunto = assuntoEmailFatura;
                }

                if (!string.IsNullOrWhiteSpace(corpoEmailFatura))
                {
                    string datasVencimentosBoletos = string.Join(", ", (from o in primeiraFatura.Parcelas select o.DataVencimento.ToString("dd/MM/yyyy")));

                    corpoEmailFatura = corpoEmailFatura.Replace("#QuebraLinha", "<br/>");
                    corpoEmailFatura = corpoEmailFatura.Replace("#NumeroFatura", string.Join(", ", faturas.Select(f => f.Numero).ToList()));
                    corpoEmailFatura = corpoEmailFatura.Replace("#ObservacaoFatura", observacaoFatura);
                    corpoEmailFatura = corpoEmailFatura.Replace("#DataFatura", primeiraFatura.DataFatura.ToString("dd/MM/yyyy"));
                    corpoEmailFatura = corpoEmailFatura.Replace("#DatasVencimentosBoletos", datasVencimentosBoletos);
                    corpoEmailFatura = corpoEmailFatura.Replace("#CNPJTomador", primeiraFatura.ClienteTomadorFatura?.CPF_CNPJ_Formatado);
                    corpoEmailFatura = corpoEmailFatura.Replace("#NomeTomador", primeiraFatura.ClienteTomadorFatura?.Nome);
                    corpoEmailFatura = corpoEmailFatura.Replace("#CNPJEmpresa", primeiraFatura.Empresa?.CNPJ_Formatado);
                    corpoEmailFatura = corpoEmailFatura.Replace("#NomeEmpresa", primeiraFatura.Empresa?.RazaoSocial);
                    corpoEmailFatura = corpoEmailFatura.Replace("#ValorFatura", $"R$ {primeiraFatura.Total:N2}");

                    string tabelaCorpo = "";
                    if (corpoEmailFatura.Contains("#Tabela"))
                    {
                        tabelaCorpo = "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>";
                        tabelaCorpo += "<script src='http://www.developerdan.com/table-to-json/javascripts/jquery.tabletojson.min.js'></script>";
                        tabelaCorpo += "<table style='width:100%; align='center'; border='1';>";
                        tabelaCorpo += "<tr>";
                        tabelaCorpo += "<th>Documento</th>";
                        tabelaCorpo += "<th>Número</th>";
                        tabelaCorpo += "<th>Booking</th>";
                        tabelaCorpo += "<th>Navio</th>";
                        tabelaCorpo += "<th>Nº Controle Cliente</th>";
                        tabelaCorpo += "<th>Fatura</th>";
                        tabelaCorpo += "</tr>";

                        foreach (var documento in todosDocumentos)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(documento.Documento.CTe?.Codigo ?? 0);
                            if (cte != null)
                            {
                                string numeroBooking = configuracaoGeral.HabilitarFuncionalidadesProjetoGollum || configuracaoFatura.HabilitarLayoutFaturaNFSManual
                                    ? repCTe.BuscarNumeroBooking(cte.Codigo)
                                    : cte.NumeroBooking;

                                string navio = configuracaoGeral.HabilitarFuncionalidadesProjetoGollum || configuracaoFatura.HabilitarLayoutFaturaNFSManual
                                    ? repCTe.BuscarViagem(cte.Codigo)
                                    : cte.Viagem?.Descricao;

                                string numeroControleCliente = configuracaoGeral.HabilitarFuncionalidadesProjetoGollum || configuracaoFatura.HabilitarLayoutFaturaNFSManual
                                    ? repCTe.BuscarNumeroControleCliente(cte.Codigo)
                                    : string.Join(", ", cte.XMLNotaFiscais?.Select(o => o.NumeroControleCliente));

                                tabelaCorpo += "<tr>";
                                tabelaCorpo += "<td>" + cte.ModeloDocumentoFiscal?.Abreviacao + "</td>";
                                tabelaCorpo += "<td>" + cte.Numero + "</td>";
                                tabelaCorpo += "<td>" + numeroBooking + "</td>";
                                tabelaCorpo += "<td>" + navio + "</td>";
                                tabelaCorpo += "<td>" + numeroControleCliente + "</td>";
                                tabelaCorpo += "<td>" + Utilidades.String.OnlyNumbers((documento.Fatura?.Numero.ToString("n0") ?? "")) + "</td>";
                                tabelaCorpo += "</tr>";
                            }
                        }

                        tabelaCorpo += "</table>";
                    }
                    corpoEmailFatura = corpoEmailFatura.Replace("#Tabela", tabelaCorpo);

                    corpo = corpoEmailFatura;
                }
                var retornoEnvios = Servicos.Email.EnviarEmailComApiAsync(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, corpo, email.Smtp, email.DisplayEmail, attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, 0, false).GetAwaiter().GetResult();
                if (retornoEnvios.Success)
                {
                    mensagemErro = "Envio realizado com sucesso.";

                    if (faturaIntegracao != null)
                    {
                        servFatura.InserirLog(faturaIntegracao.Fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.EnviouFatura, faturaIntegracao.Fatura.Usuario, string.Join("; ", emails.ToArray()));

                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        faturaIntegracao.MensagemRetorno = "Envio realizado com sucesso.";
                        faturaIntegracao.DataEnvio = DateTime.Now;
                        faturaIntegracao.Tentativas++;

                        repFaturaIntegracao.Atualizar(faturaIntegracao);

                        Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivo = new Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao()
                        {
                            NomeArquivo = "Envio Email"
                        };
                        repArquivoIntegracao.Inserir(arquivo);

                        Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo()
                        {
                            ArquivoRequisicao = arquivo,
                            ArquivoResposta = arquivo,
                            Data = DateTime.Now,
                            Mensagem = "Email(s): " + string.Join("; ", emails.ToArray()),
                            Tipo = TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
                        };
                        repFaturaIntegracaoArquivo.Inserir(faturaIntegracaoArquivo);

                        faturaIntegracao.ArquivosIntegracao.Add(faturaIntegracaoArquivo);

                        serCargaDadosSumarizados.AtualizarDadosCTesFaturadosIntegrados(faturaIntegracao.Fatura.Codigo, unitOfWork);
                    }
                    else if (faturas != null && faturas.Count > 0)
                    {
                        foreach (var faturaImpressao in faturas)
                        {
                            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracaoEmail = repFaturaIntegracao.BuscarLayoutFaturaPorFatura(faturaImpressao.Codigo);
                            if (faturaIntegracaoEmail != null)
                            {
                                faturaIntegracaoEmail.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                                faturaIntegracaoEmail.DataEnvio = DateTime.Now;
                                faturaIntegracaoEmail.Tentativas++;

                                repFaturaIntegracao.Atualizar(faturaIntegracaoEmail);

                                Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivo = new Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao()
                                {
                                    NomeArquivo = "Envio Email"
                                };
                                repArquivoIntegracao.Inserir(arquivo);

                                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo()
                                {
                                    ArquivoRequisicao = arquivo,
                                    ArquivoResposta = arquivo,
                                    Data = DateTime.Now,
                                    Mensagem = "Email(s): " + string.Join("; ", emails.ToArray()),
                                    Tipo = TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
                                };
                                repFaturaIntegracaoArquivo.Inserir(faturaIntegracaoArquivo);

                                faturaIntegracaoEmail.ArquivosIntegracao.Add(faturaIntegracaoArquivo);

                                repFaturaIntegracao.Atualizar(faturaIntegracaoEmail);

                                serCargaDadosSumarizados.AtualizarDadosCTesFaturadosIntegrados(faturaIntegracaoEmail.Fatura.Codigo, unitOfWork);
                            }
                        }
                    }
                    if (!possuiTransacao)
                        unitOfWork.CommitChanges();
                    return;
                }
                else
                {
                    if (faturaIntegracao != null)
                    {
                        faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        faturaIntegracao.MensagemRetorno = mensagemErro;
                        faturaIntegracao.DataEnvio = DateTime.Now;
                        faturaIntegracao.Tentativas++;

                        repFaturaIntegracao.Atualizar(faturaIntegracao);
                    }
                    if (!possuiTransacao)
                        unitOfWork.CommitChanges();
                    return;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Falha genérica ao enviar as faturas";
                if (faturaIntegracao != null)
                {
                    faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    faturaIntegracao.MensagemRetorno = mensagemErro;
                    faturaIntegracao.DataEnvio = DateTime.Now;
                    faturaIntegracao.Tentativas++;

                    repFaturaIntegracao.Atualizar(faturaIntegracao);
                }
                if (!possuiTransacao)
                    unitOfWork.Rollback();
                return;
            }
            finally
            {
                if (!possuiTransacao)
                    unitOfWork.Dispose();
            }
        }

        public void ValidarCancelamentoFatura(int codigoFatura, DateTime? dataCancelamento = null)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
            if (fatura == null)
                throw new ServicoException("Fatura não encontrada.");

            DateTime? dataUltimoFechamento = repFechamentoDiario.ObterUltimaDataFechamento();

            if (!configuracaoFatura.InformarDataCancelamentoCancelamentoFatura || !dataCancelamento.HasValue)
                dataCancelamento = fatura.DataFatura;

            if (dataUltimoFechamento.HasValue && dataUltimoFechamento.Value >= dataCancelamento)
                throw new ServicoException("Existe uma data de fechamento (" + dataUltimoFechamento.Value.ToString("dd/MM/yyyy") + ") posterior à data de emissão dos títulos (" + dataCancelamento.Value.ToString("dd/MM/yyyy") + "). O cancelamento dos títulos ocorrerá em " + dataUltimoFechamento.Value.AddDays(1).ToString("dd/MM/yyyy") + ".");
        }

        public void IniciarCancelamentoFatura(int codigoFatura, string motivo, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool duplicarFatura = false, DateTime? dataCancelamento = null, int codigoJustificativaCancelamento = 0, bool cancelamentoViaIntegracao = false)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(_unitOfWork);
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(_unitOfWork);
            Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro repJustificativaCancelamentoFinanceiro = new Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro(_unitOfWork);

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

            if (fatura == null)
                throw new ServicoException("Fatura não encontrada.");

            if (fatura.Etapa == EtapaFatura.LancandoCargas)
                throw new ServicoException("A fatura ainda está em processo de lançamento de documentos, não sendo possível realizar o cancelamento.");

            if (fatura.Situacao == SituacaoFatura.Cancelado)
                throw new ServicoException("A situação atual da fatura não permite o cancelamento da mesma.");

            if (fatura.Situacao == SituacaoFatura.EmCancelamento)
                throw new ServicoException("A situação atual da fatura já está em cancelamento, favor aguardar.");

            if (fatura.FaturaRecebidaDeIntegracao)
                if (!cancelamentoViaIntegracao)
                    throw new ServicoException("Não é possível realizar a operação para uma fatura recebida pela integração.");

            if (configuracao.UtilizaEmissaoMultimodal)
                CancelarTitulosBoletos(codigoFatura, _unitOfWork, auditado, usuario.Empresa);
            else
            {
                if (repTitulo.ContemTitulosPagosFatura(codigoFatura))
                    throw new ServicoException("Esta fatura já possui título(s) quitado(s), impossível cancelar a mesma.");

                if (repTitulo.ContemBoletosFatura(codigoFatura))
                    throw new ServicoException("Esta fatura possui título(s) com boleto vinculado, impossível cancelar a mesma.");

                if (repTituloBaixa.BuscarPorFatura(codigoFatura) != null)
                    throw new ServicoException("Esta fatura já possui baixa de título, impossível cancelar a mesma");
            }

            if (motivo.Length < 20 && codigoJustificativaCancelamento == 0)
                throw new ServicoException("O motivo deve ter ao menos 20 caracteres.");

            DateTime? dataUltimoFechamento = repFechamentoDiario.ObterUltimaDataFechamento();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();

            if (!configuracaoFatura.InformarDataCancelamentoCancelamentoFatura || !dataCancelamento.HasValue)
                dataCancelamento = fatura.DataFatura;

            if (dataUltimoFechamento.HasValue && dataUltimoFechamento.Value >= dataCancelamento)
                dataCancelamento = dataUltimoFechamento.Value.AddDays(1);

            fatura.Duplicar = duplicarFatura;
            fatura.UsuarioCancelamento = usuario;
            fatura.NotificadoOperador = false;
            fatura.SituacaoNoCancelamento = fatura.Situacao;
            fatura.Situacao = SituacaoFatura.EmCancelamento;
            fatura.DataCancelamentoFatura = dataCancelamento;
            fatura.MotivoCancelamento = motivo;
            fatura.JustificativaCancelamento = codigoJustificativaCancelamento > 0 ? repJustificativaCancelamentoFinanceiro.BuscarPorCodigo(codigoJustificativaCancelamento, false) : null;

            InserirLog(fatura, _unitOfWork, TipoLogFatura.CancelouFatura, usuario);

            if (cancelamentoViaIntegracao)
                Servicos.Auditoria.Auditoria.Auditar(auditado, fatura, "Cancelou a fatura via integração.", _unitOfWork);
            else
                Servicos.Auditoria.Auditoria.Auditar(auditado, fatura, "Cancelou a fatura.", _unitOfWork);

            repFatura.Atualizar(fatura);

            //List<TipoIntegracao> tiposIntegracaoAutorizados = new List<TipoIntegracao> { TipoIntegracao.Intercab, TipoIntegracao.EMP, TipoIntegracao.NFTP, TipoIntegracao.SAP_ESTORNO_FATURA };
            //List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tiposIntegracaoAutorizados);

            //if (tiposIntegracao?.Count > 0)
            //{
            //    foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            //    {
            //        if (tipoIntegracao.Tipo == TipoIntegracao.Intercab)
            //        {
            //            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
            //            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

            //            if (integracaoIntercab != null && fatura.FaturaIntegracaComSucesso)
            //                AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, _unitOfWork);
            //        }
            //        if (tipoIntegracao.Tipo == TipoIntegracao.EMP)
            //        {
            //            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            //            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

            //            if (integracaoEMP != null && integracaoEMP.PossuiIntegracaoEMP && integracaoEMP.AtivarIntegracaoCancelamentoFaturaEMP)
            //                AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, _unitOfWork);
            //        }
            //        if (tipoIntegracao.Tipo == TipoIntegracao.NFTP)
            //        {
            //            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
            //            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

            //            if (integracaoEMP != null && integracaoEMP.AtivarIntegracaoNFTPEMP)
            //                AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, _unitOfWork);
            //        }
            //        if (tipoIntegracao.Tipo == TipoIntegracao.SAP_ESTORNO_FATURA)// deveria ser SAP_Fatura mas foi criado apenas como SAP
            //            AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, _unitOfWork);
            //    }
            //}
        }

        public bool CancelarFatura(out string erro, int codigoFatura, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Hubs.Fatura servicoNotificacaoFatura = new Servicos.Embarcador.Hubs.Fatura();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

            if (fatura.Situacao != SituacaoFatura.EmCancelamento)
            {
                erro = "Não é possível cancelar uma fatura na situação " + fatura.DescricaoSituacao + ".";
                return false;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);

            List<int> codigosTitulos = repTitulo.BuscarCodigosNaoCanceladosPorFatura(fatura.Codigo);
            List<int> codigosFaturaDocumentos = repFaturaDocumento.BuscarCodigosNaoCanceladosPorFatura(fatura.Codigo);

            int countDocumentosProcessar = codigosTitulos.Count + codigosFaturaDocumentos.Count + 1;
            int countDocumentosProcessados = 0;

            DateTime? dataBaseMovimento = null;
            DateTime dataMovimento = fatura.DataCancelamentoFatura.Value;

            if (configuracaoFatura.InformarDataCancelamentoCancelamentoFatura)
            {
                dataMovimento = DateTime.Now;
                dataBaseMovimento = fatura.DataCancelamentoFatura.Value;
            }

            foreach (int codigoFaturaDocumento in codigosFaturaDocumentos)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarPorCodigo(codigoFaturaDocumento);

                if (faturaDocumento.Cancelado)
                    continue;

                unitOfWork.Start();

                if (!fatura.Duplicar)
                {
                    faturaDocumento.Documento.ValorAFaturar += faturaDocumento.ValorACobrar;
                    faturaDocumento.Documento.ValorEmFatura -= faturaDocumento.ValorACobrar;

                    if (fatura.SituacaoNoCancelamento == SituacaoFatura.Fechado || fatura.SituacaoNoCancelamento == SituacaoFatura.Liquidado)
                    {
                        faturaDocumento.Documento.ValorAcrescimo -= faturaDocumento.ValorAcrescimo;
                        faturaDocumento.Documento.ValorDesconto -= faturaDocumento.ValorDesconto;
                    }
                }
                else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS
                    && fatura.Duplicar
                    && faturaDocumento != null
                    && faturaDocumento.Documento != null
                    && faturaDocumento.Documento.ValorDesconto > 0)
                {
                    faturaDocumento.Documento.ValorAcrescimo -= faturaDocumento.ValorAcrescimo;
                    faturaDocumento.Documento.ValorDesconto -= faturaDocumento.ValorDesconto;
                }

                faturaDocumento.Cancelado = true;

                if (faturaDocumento.Documento.CTe != null)
                {
                    faturaDocumento.Documento.CTe.CTePendenteIntegracaoFatura = true;
                    repCTe.Atualizar(faturaDocumento.Documento.CTe);
                }

                repDocumentoFaturamento.Atualizar(faturaDocumento.Documento);
                repFaturaDocumento.Atualizar(faturaDocumento);

                unitOfWork.CommitChanges();

                countDocumentosProcessados++;

                if (countDocumentosProcessar < 10 || (countDocumentosProcessados % 5) == 0)
                {
                    unitOfWork.FlushAndClear();

                    servicoNotificacaoFatura.InformarQuantidadeDocumentosProcessadosFechamento(fatura.Codigo, countDocumentosProcessar, countDocumentosProcessados);
                }
            }

            servicoNotificacaoFatura.InformarQuantidadeDocumentosProcessadosCancelamento(fatura.Codigo, 100, 50);

            fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

            foreach (int codigoTitulo in codigosTitulos)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);

                if (titulo.StatusTitulo == StatusTitulo.Cancelado)
                    continue;

                List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> tituloDocumentos = repTituloDocumento.BuscarPorTitulo(titulo.Codigo);

                unitOfWork.Start();

                servProcessoMovimento.GerarMovimentacao(fatura.TipoMovimentoReversao, dataMovimento, titulo.ValorOriginal, titulo.Codigo.ToString(), "Reversão de título da fatura.", unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, fatura.Cliente, fatura.GrupoPessoas, dataBaseMovimento);

                titulo.StatusTitulo = StatusTitulo.Cancelado;
                titulo.DataAlteracao = DateTime.Now;
                titulo.DataCancelamento = fatura.DataCancelamentoFatura.Value;

                repTitulo.Atualizar(titulo);

                new Servicos.Embarcador.Integracao.IntegracaoTitulo(unitOfWork).IniciarIntegracoesDeTitulosAReceber(titulo, TipoAcaoIntegracao.Cancelamento);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento in tituloDocumentos)
                {
                    List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> tituloDocumentoAcrescimosDescontos = repTituloDocumentoAcrescimoDesconto.BuscarPorDocumento(tituloDocumento.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto in tituloDocumentoAcrescimosDescontos)
                        servProcessoMovimento.GerarMovimentacao(tituloDocumentoAcrescimoDesconto.TipoMovimentoReversao, dataMovimento, tituloDocumentoAcrescimoDesconto.Valor, titulo.Codigo.ToString(), "Ref. à reversão do " + tituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa.ToLower() + " informado na geração do título " + titulo.Codigo.ToString() + " da fatura " + fatura.Numero.ToString() + ".", unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, fatura.Cliente, fatura.GrupoPessoas, dataBaseMovimento, null, tituloDocumentoAcrescimoDesconto.TipoMovimentoReversao.ContasExportacao.ToList(), tituloDocumentoAcrescimoDesconto, TipoMovimentoExportacao.CancelamentoAcrescimoDescontoFatura);
                }

                unitOfWork.CommitChanges();

                countDocumentosProcessados++;

                if (countDocumentosProcessar < 10 || (countDocumentosProcessados % 5) == 0)
                {
                    unitOfWork.FlushAndClear();

                    servicoNotificacaoFatura.InformarQuantidadeDocumentosProcessadosFechamento(fatura.Codigo, countDocumentosProcessar, countDocumentosProcessados);
                }
            }

            if (!fatura.ReverteuAcrescimoDesconto)
            {
                fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

                unitOfWork.Start();

                if (fatura.Descontos > 0m)
                    servProcessoMovimento.GerarMovimentacao(fatura.TipoMovimentoReversaoDesconto, dataMovimento, fatura.Descontos, fatura.Numero.ToString(), "Reversão do desconto da fatura.", unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, 0, null, fatura.Cliente, fatura.GrupoPessoas, dataBaseMovimento);

                if (fatura.Acrescimos > 0m)
                    servProcessoMovimento.GerarMovimentacao(fatura.TipoMovimentoReversaoAcrescimo, dataMovimento, fatura.Acrescimos, fatura.Numero.ToString(), "Reversão do acréscimo da fatura.", unitOfWork, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, 0, null, fatura.Cliente, fatura.GrupoPessoas, dataBaseMovimento);

                fatura.ReverteuAcrescimoDesconto = true;

                repFatura.Atualizar(fatura);

                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();
            }

            repFaturaCargaDocumento.LimpaConhecimentoPorFatura(fatura.Codigo);

            servicoNotificacaoFatura.InformarQuantidadeDocumentosProcessadosFechamento(fatura.Codigo, 1, 1);

            unitOfWork.Start();

            fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

            if (fatura.Duplicar)
                DuplicarFatura(fatura, unitOfWork);

            fatura.Situacao = SituacaoFatura.Cancelado;

            repFatura.Atualizar(fatura);

            unitOfWork.CommitChanges();

            servicoNotificacaoFatura.InformarFaturaAtualizada(fatura.Codigo);
            serCargaDadosSumarizados.AtualizarDadosCTesFaturados(fatura.Codigo, unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cancelamentoCarga = fatura.CargaCancelamento;
            if (cancelamentoCarga != null)
            {
                cancelamentoCarga.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                repCargaCancelamento.Atualizar(cancelamentoCarga);
            }

            List<TipoIntegracao> tiposIntegracaoAutorizados = new List<TipoIntegracao> { TipoIntegracao.Intercab, TipoIntegracao.EMP, TipoIntegracao.NFTP, TipoIntegracao.SAP_ESTORNO_FATURA };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tiposIntegracaoAutorizados);

            if (tiposIntegracao.Count == 0)
            {
                erro = "Nenhum tipo de integração encontrado";
                return false;
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                if (tipoIntegracao.Tipo == TipoIntegracao.Intercab)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                    if (integracaoIntercab != null && fatura.FaturaIntegracaComSucesso)
                        AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, unitOfWork);
                }
                if (tipoIntegracao.Tipo == TipoIntegracao.EMP)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

                    if (integracaoEMP != null && integracaoEMP.PossuiIntegracaoEMP && integracaoEMP.AtivarIntegracaoCancelamentoFaturaEMP)
                        AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, unitOfWork);
                }
                if (tipoIntegracao.Tipo == TipoIntegracao.NFTP)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();

                    if (integracaoEMP != null && integracaoEMP.AtivarIntegracaoNFTPEMP)
                        AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, unitOfWork);
                }
                if (tipoIntegracao.Tipo == TipoIntegracao.SAP_ESTORNO_FATURA)// deveria ser SAP_Fatura mas foi criado apenas como SAP
                    AdicionarFaturaCancelamentoIntegracao(fatura, tipoIntegracao, unitOfWork);
            }

            erro = string.Empty;
            return true;
        }

        public void AdicionarFaturaCancelamentoIntegracao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao()
            {
                Fatura = fatura,
                TipoIntegracao = tipoIntegracao,
                DataEnvio = DateTime.Now,
                MensagemRetorno = "",
                Tentativas = 0,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracaoFatura = TipoIntegracaoFatura.Fatura
            };

            repFaturaIntegracao.Inserir(faturaIntegracao);
        }

        private static void DuplicarFatura(Dominio.Entidades.Embarcador.Fatura.Fatura faturaAntiga, Repositorio.UnitOfWork unitOfWork)
        {
            if (!faturaAntiga.Duplicar)
                return;

            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

            Dominio.Entidades.Embarcador.Fatura.Fatura faturaNova = new Dominio.Entidades.Embarcador.Fatura.Fatura();

            Utilidades.Object.CopiarPropriedadesObjeto(faturaAntiga, faturaNova);

            faturaNova.ControleNumeracao = null;
            faturaNova.Numero = repFatura.UltimoNumeracao() + 1;
            faturaNova.NumeroFaturaOriginal = faturaAntiga.Numero;
            faturaNova.Situacao = SituacaoFatura.EmAntamento;
            faturaNova.Etapa = EtapaFatura.Fechamento;
            faturaNova.UsuarioCancelamento = null;
            faturaNova.DataCancelamentoFatura = null;
            faturaNova.MotivoCancelamento = null;
            faturaNova.SituacaoNoCancelamento = null;
            faturaNova.ReverteuAcrescimoDesconto = false;
            faturaNova.FaturaOriginal = faturaAntiga;

            repFatura.Inserir(faturaNova);

            Servicos.Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema }, faturaNova, $"Fatura gerada à partir do cancelamento e duplicação da fatura {faturaAntiga.Numero} (código {faturaAntiga.Codigo}).", unitOfWork);

            DuplicarFaturaDocumento(faturaAntiga, faturaNova, unitOfWork);
            DuplicarFaturaParcela(faturaAntiga, faturaNova, unitOfWork);
        }

        private static void DuplicarFaturaDocumento(Dominio.Entidades.Embarcador.Fatura.Fatura faturaAntiga, Dominio.Entidades.Embarcador.Fatura.Fatura faturaNova, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> faturaDocumentos = repFaturaDocumento.BuscarPorFatura(faturaAntiga.Codigo);
            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> faturaDocumentosAcrescimosDescontos = repFaturaDocumentoAcrescimoDesconto.BuscarPorFatura(faturaAntiga.Codigo);

            bool possuiAcrescimoDesconto = faturaDocumentosAcrescimosDescontos.Any();

            for (int i = 0; i < faturaDocumentos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumentoAntigo = faturaDocumentos[i];
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumentoNovo = new Dominio.Entidades.Embarcador.Fatura.FaturaDocumento();

                Utilidades.Object.CopiarPropriedadesObjeto(faturaDocumentoAntigo, faturaDocumentoNovo);

                faturaDocumentoNovo.Cancelado = false;
                faturaDocumentoNovo.TituloGerado = false;
                faturaDocumentoNovo.Fatura = faturaNova;

                repFaturaDocumento.Inserir(faturaDocumentoNovo);

                if (possuiAcrescimoDesconto)
                {
                    List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> faturaDocumentoAcrescimoDescontosDocumento = faturaDocumentosAcrescimosDescontos.Where(o => o.FaturaDocumento.Codigo == faturaDocumentoAntigo.Codigo).ToList();

                    for (int j = 0; j < faturaDocumentoAcrescimoDescontosDocumento.Count; j++)
                    {
                        Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDescontoAntigo = faturaDocumentoAcrescimoDescontosDocumento[j];
                        Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDescontoNovo = new Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto();

                        Utilidades.Object.CopiarPropriedadesObjeto(faturaDocumentoAcrescimoDescontoAntigo, faturaDocumentoAcrescimoDescontoNovo);

                        faturaDocumentoAcrescimoDescontoNovo.FaturaDocumento = faturaDocumentoNovo;

                        repFaturaDocumentoAcrescimoDesconto.Inserir(faturaDocumentoAcrescimoDescontoNovo);
                    }
                }
            }
        }

        private static void DuplicarFaturaParcela(Dominio.Entidades.Embarcador.Fatura.Fatura faturaAntiga, Dominio.Entidades.Embarcador.Fatura.Fatura faturaNova, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);

            List<Dominio.Entidades.Embarcador.Fatura.FaturaParcela> faturaParcelas = repFaturaParcela.BuscarPorFatura(faturaAntiga.Codigo);

            for (int i = 0; i < faturaParcelas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaParcela faturaParcelaAntiga = faturaParcelas[i];
                Dominio.Entidades.Embarcador.Fatura.FaturaParcela faturaParcelaNova = new Dominio.Entidades.Embarcador.Fatura.FaturaParcela();

                Utilidades.Object.CopiarPropriedadesObjeto(faturaParcelaAntiga, faturaParcelaNova);

                faturaParcelaNova.Fatura = faturaNova;
                faturaParcelaNova.TituloGerado = false;

                repFaturaParcela.Inserir(faturaParcelaNova);
            }
        }

        private bool FinalizarFatura(out string erro, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);

            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Servicos.Embarcador.Hubs.Fatura servicoNotificacaoFatura = new Servicos.Embarcador.Hubs.Fatura();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaGeracaoTituloFatura? formaGeracaoTituloFatura = null;
            if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
                formaGeracaoTituloFatura = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.FormaGeracaoTituloFatura;
            else if (fatura.Cliente != null && fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                formaGeracaoTituloFatura = fatura.Cliente.FormaGeracaoTituloFatura;
            else if (fatura.Cliente != null && fatura.Cliente.GrupoPessoas != null)
                formaGeracaoTituloFatura = fatura.Cliente.GrupoPessoas.FormaGeracaoTituloFatura;
            else if (fatura.GrupoPessoas != null)
                formaGeracaoTituloFatura = fatura.GrupoPessoas.FormaGeracaoTituloFatura;

            if (fatura.Carga == null && configuracaoTMS.UtilizaEmissaoMultimodal)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga primeiraCarga = repFaturaDocumento.BuscarPrimeiraCarga(fatura.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(primeiraCarga.Codigo);
                if (primeiraCarga != null && primeiraCarga.CargaTakeOrPay)
                {
                    fatura.Carga = primeiraCarga;
                    fatura.ImprimeObservacaoFatura = true;

                    if (cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem || cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem)
                    {
                        fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                        fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                        fatura.ObservacaoFatura += "Porto de Destino: " + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                        fatura.ObservacaoFatura += "Tipo Proposta: " + (cargaPedido?.TipoPropostaMultimodal.ObterDescricao()) + "\n" + "\n";
                    }
                    else if (cargaPedido?.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade)
                    {
                        int qtdDisponibilizadas = repCargaPedido.BuscarQuantidadeDisponibilizadas(fatura.Carga.Codigo);
                        int qtdNaoEmbarcadas = repCargaPedido.BuscarQuantidadeNaoEmbarcadas(fatura.Carga.Codigo);

                        fatura.ObservacaoFatura += "Fatura de Penalidade Contratual \n\n";
                        fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                        fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                        fatura.ObservacaoFatura += "Porto de Destino: " + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                        fatura.ObservacaoFatura += "Quantidade de unidades disponibilizadas: " + (Utilidades.String.OnlyNumbers(qtdDisponibilizadas.ToString("n0"))) + " \n";
                        fatura.ObservacaoFatura += "Quantidade de unidades não embarcadas: " + (Utilidades.String.OnlyNumbers(qtdNaoEmbarcadas.ToString("n0"))) + "\n";
                    }
                    else
                        fatura.FaturaPropostaFaturamento = true;

                    if (!string.IsNullOrWhiteSpace(primeiraCarga.ObservacaoParaFaturamento))
                    {
                        if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                            fatura.ObservacaoFatura += "\n\n";
                        fatura.ObservacaoFatura += primeiraCarga.ObservacaoParaFaturamento;
                    }

                    repFatura.Atualizar(fatura);
                }
            }

            if (formaGeracaoTituloFatura != null && formaGeracaoTituloFatura.HasValue && formaGeracaoTituloFatura.Value != FormaGeracaoTituloFatura.Padrao)
            {
                if (formaGeracaoTituloFatura.Value == FormaGeracaoTituloFatura.PorDocumento && !fatura.FaturaGeradaPelaIntegracao)
                {
                    if (!GerarTitulosPorDocumento(out erro, fatura, unidadeTrabalho, stringConexao, tipoServicoMultisoftware))
                    {
                        unidadeTrabalho.Rollback();
                        return false;
                    }
                }
                else
                {
                    if (!GerarTitulosPorParcela(out erro, fatura, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, configuracaoTMS))
                    {
                        unidadeTrabalho.Rollback();
                        return false;
                    }
                }
            }
            else if (configuracaoTMS.TipoGeracaoTituloFatura == TipoGeracaoTituloFatura.PorDocumento && !fatura.FaturaGeradaPelaIntegracao)
            {
                if (!GerarTitulosPorDocumento(out erro, fatura, unidadeTrabalho, stringConexao, tipoServicoMultisoftware))
                {
                    unidadeTrabalho.Rollback();
                    return false;
                }
            }
            else
            {
                if (!GerarTitulosPorParcela(out erro, fatura, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, configuracaoTMS))
                {
                    unidadeTrabalho.Rollback();
                    return false;
                }
            }

            unidadeTrabalho.Start();

            fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

            fatura.Situacao = SituacaoFatura.Fechado;

            repFatura.Atualizar(fatura);

            GerarIntegracoesFatura(fatura, unidadeTrabalho, tipoServicoMultisoftware, auditado, configuracaoTMS);

            unidadeTrabalho.CommitChanges();

            servicoNotificacaoFatura.InformarFaturaAtualizada(fatura.Codigo);
            serCargaDadosSumarizados.AtualizarDadosCTesFaturados(fatura.Codigo, unidadeTrabalho);

            erro = string.Empty;
            return true;
        }

        private static bool GerarTitulosPorDocumento(out string erro, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);
            Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeTrabalho);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unidadeTrabalho);
            Servicos.Embarcador.Hubs.Fatura servicoNotificacaoFatura = new Servicos.Embarcador.Hubs.Fatura();

            List<int> codigosFaturaDocumentos = repFaturaDocumento.BuscarCodigosPorFatura(fatura.Codigo);

            int codigoBoletoConfiguracao = 0;

            bool gerarBoletoAutomaticamente = false;
            bool enviarBoletoPorEmailAutomaticamente = false;
            bool enviarDocumentacaoFaturamentoCTe = false;

            if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
            {
                gerarBoletoAutomaticamente = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.GerarBoletoAutomaticamente ?? false;
                enviarBoletoPorEmailAutomaticamente = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.EnviarBoletoPorEmailAutomaticamente ?? false;
                enviarDocumentacaoFaturamentoCTe = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.EnviarDocumentacaoFaturamentoCTe ?? false;
                codigoBoletoConfiguracao = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.BoletoConfiguracao?.Codigo ?? 0;
            }
            else if (fatura.GrupoPessoas != null)
            {
                gerarBoletoAutomaticamente = fatura.GrupoPessoas.GerarBoletoAutomaticamente;
                enviarBoletoPorEmailAutomaticamente = fatura.GrupoPessoas.EnviarBoletoPorEmailAutomaticamente;
                enviarDocumentacaoFaturamentoCTe = fatura.GrupoPessoas.EnviarDocumentacaoFaturamentoCTe;
                codigoBoletoConfiguracao = fatura.GrupoPessoas.BoletoConfiguracao?.Codigo ?? 0;
            }
            else if (fatura.Cliente != null && fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
            {
                gerarBoletoAutomaticamente = fatura.Cliente.GerarBoletoAutomaticamente;
                enviarBoletoPorEmailAutomaticamente = fatura.Cliente.EnviarBoletoPorEmailAutomaticamente;
                enviarDocumentacaoFaturamentoCTe = fatura.Cliente.EnviarDocumentacaoFaturamentoCTe;
                codigoBoletoConfiguracao = fatura.Cliente.BoletoConfiguracao?.Codigo ?? 0;
            }
            else if (fatura.Cliente != null && fatura.Cliente.GrupoPessoas != null)
            {
                gerarBoletoAutomaticamente = fatura.Cliente.GrupoPessoas.GerarBoletoAutomaticamente;
                enviarBoletoPorEmailAutomaticamente = fatura.Cliente.GrupoPessoas.EnviarBoletoPorEmailAutomaticamente;
                enviarDocumentacaoFaturamentoCTe = fatura.Cliente.GrupoPessoas.EnviarDocumentacaoFaturamentoCTe;
                codigoBoletoConfiguracao = fatura.Cliente.GrupoPessoas.BoletoConfiguracao?.Codigo ?? 0;
            }

            int quantidadeParcelas = fatura.Parcelas.Count;
            int quantidadeDocumentos = codigosFaturaDocumentos.Count;

            for (var x = 0; x < quantidadeDocumentos; x++)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarPorCodigo(codigosFaturaDocumentos[x]);

                if (faturaDocumento.TituloGerado)
                    continue;

                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> faturaDocumentoAcrescimosDescontos = repFaturaDocumentoAcrescimoDesconto.BuscarPorFaturaDocumento(faturaDocumento.Codigo);

                decimal valor = Math.Round(faturaDocumento.ValorACobrar, 2, MidpointRounding.AwayFromZero);
                decimal valorTotal = Math.Round(faturaDocumento.ValorTotalACobrar, 2, MidpointRounding.AwayFromZero);
                decimal valorTotalParcela = Math.Round(Math.Floor((valorTotal / quantidadeParcelas) * 100) / 100, 2, MidpointRounding.AwayFromZero);
                decimal valorParcela = Math.Round(Math.Floor((valor / quantidadeParcelas) * 100) / 100, 2, MidpointRounding.AwayFromZero);
                decimal valorDiferenca = valor - Math.Round(valorParcela * quantidadeParcelas, 2, MidpointRounding.AwayFromZero);
                decimal valorDiferencaTotal = valorTotal - Math.Round(valorTotalParcela * quantidadeParcelas, 2, MidpointRounding.AwayFromZero);

                decimal valorMoeda = faturaDocumento.Documento.ValorTotalMoeda ?? 0m;
                decimal valorTotalMoeda = faturaDocumento.Documento.ValorTotalMoeda ?? 0m;
                decimal valorTotalParcelaMoeda = Math.Round(Math.Floor((valorTotalMoeda / quantidadeParcelas) * 100) / 100, 2, MidpointRounding.AwayFromZero);
                decimal valorParcelaMoeda = Math.Round(Math.Floor((valorMoeda / quantidadeParcelas) * 100) / 100, 2, MidpointRounding.AwayFromZero);
                decimal valorDiferencaMoeda = valorMoeda - Math.Round(valorParcelaMoeda * quantidadeParcelas, 2, MidpointRounding.AwayFromZero);
                decimal valorDiferencaTotalMoeda = valorTotalMoeda - Math.Round(valorTotalParcelaMoeda * quantidadeParcelas, 2, MidpointRounding.AwayFromZero);

                unidadeTrabalho.Start();

                for (int i = 0; i < quantidadeParcelas; i++)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = fatura.Parcelas[i];

                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                    {
                        DataEmissao = fatura.DataFatura,
                        DataVencimento = parcela.DataVencimento,
                        DataProgramacaoPagamento = parcela.DataVencimento,
                        FaturaParcela = parcela,
                        FaturaDocumento = faturaDocumento,
                        DataLancamento = DateTime.Now,
                        Usuario = fatura.Usuario,
                        NossoNumero = parcela.NossoNumeroIntegrado
                    };

                    if (parcela.CodigoTituloIntegracao > 0)
                        titulo.CodigoRecebidoIntegracao = parcela.CodigoTituloIntegracao;

                    if (gerarBoletoAutomaticamente)
                    {
                        titulo.BoletoStatusTitulo = BoletoStatusTitulo.Emitido;
                        titulo.BoletoConfiguracao = codigoBoletoConfiguracao > 0 ? repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao) : repBoletoConfiguracao.BuscarPrimeiraConfiguracao();
                        titulo.BoletoEnviadoPorEmail = false;
                        titulo.BoletoGeradoAutomaticamente = enviarBoletoPorEmailAutomaticamente;
                    }

                    titulo.EnviarDocumentacaoFaturamentoCTe = enviarDocumentacaoFaturamentoCTe;

                    if (fatura.TipoPessoa == TipoPessoa.GrupoPessoa && fatura.GrupoPessoas != null)
                        titulo.GrupoPessoas = fatura.GrupoPessoas;

                    if (fatura.ImprimeObservacaoFatura && !string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                        titulo.Observacao = fatura.ObservacaoFatura;

                    if (fatura.ClienteTomadorFatura != null)
                        titulo.Pessoa = fatura.ClienteTomadorFatura;
                    else if (fatura.TipoPessoa == TipoPessoa.Pessoa && fatura.Cliente != null)
                        titulo.Pessoa = fatura.Cliente;

                    if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                        titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

                    titulo.FormaTitulo = parcela.FormaTitulo ?? servTitulo.ObterFormaTituloGrupoPessoa(titulo);

                    string historicoTitulo = "";
                    if (fatura.Usuario != null && fatura.DataInicial.HasValue && fatura.DataFinal.HasValue)
                        historicoTitulo = "Título gerado à partir da fatura " + fatura.Numero.ToString() + " / parcela " + parcela.Sequencia.ToString() + ", referente ao período " + fatura.DataInicial.Value.ToString("dd/MM/yyyy") + " a " + fatura.DataFinal.Value.ToString("dd/MM/yyyy") + ". Pelo operador " + fatura.Usuario.Nome + " " +
                             (faturaDocumento.Documento.TipoDocumento == TipoDocumentoFaturamento.CTe ? ("CT-e " + faturaDocumento.Documento.CTe.Numero.ToString() + "-" + faturaDocumento.Documento.CTe.Serie.Numero.ToString()) : ("Carga " + (faturaDocumento.Documento.Carga?.CodigoCargaEmbarcador ?? ""))) + ".";
                    else if (fatura.Usuario != null)
                        historicoTitulo = "Título gerado à partir da fatura " + fatura.Numero.ToString() + " / parcela " + parcela.Sequencia.ToString() + ". Pelo operador " + fatura.Usuario.Nome + " " +
                             (faturaDocumento.Documento.TipoDocumento == TipoDocumentoFaturamento.CTe ? ("CT-e " + faturaDocumento.Documento.CTe.Numero.ToString() + "-" + faturaDocumento.Documento.CTe.Serie.Numero.ToString()) : ("Carga " + (faturaDocumento.Documento.Carga?.CodigoCargaEmbarcador ?? ""))) + ".";
                    else
                        historicoTitulo = "Título gerado à partir da fatura " + fatura.Numero.ToString() + " / parcela " + parcela.Sequencia.ToString() + ". " +
                             (faturaDocumento.Documento.TipoDocumento == TipoDocumentoFaturamento.CTe ? ("CT-e " + faturaDocumento.Documento.CTe.Numero.ToString() + "-" + faturaDocumento.Documento.CTe.Serie.Numero.ToString()) : ("Carga " + (faturaDocumento.Documento.Carga?.CodigoCargaEmbarcador ?? ""))) + ".";

                    titulo.Historico = historicoTitulo;
                    titulo.Sequencia = i + 1;
                    titulo.StatusTitulo = StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TipoTitulo = TipoTitulo.Receber;

                    decimal valorTotalParcelaAtual = valorTotalParcela;
                    decimal valorParcelaAtual = valorParcela;

                    decimal valorTotalParcelaAtualMoeda = valorTotalParcelaMoeda;
                    decimal valorParcelaAtualMoeda = valorParcelaMoeda;

                    if ((i == 0 && fatura.TipoArredondamentoParcelas == TipoArredondamento.Primeira) ||
                        ((i + 1) == quantidadeParcelas && fatura.TipoArredondamentoParcelas == TipoArredondamento.Ultima))
                    {
                        valorTotalParcelaAtual += valorDiferencaTotal;
                        valorParcelaAtual += valorDiferenca;

                        valorTotalParcelaAtualMoeda += valorDiferencaTotalMoeda;
                        valorParcelaAtualMoeda += valorDiferencaMoeda;
                    }

                    titulo.ValorOriginal = valorTotalParcelaAtual;
                    titulo.ValorPendente = valorTotalParcelaAtual;
                    titulo.ValorTituloOriginal = valorTotalParcelaAtual;

                    if (fatura.MoedaCotacaoBancoCentral.HasValue && fatura.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                    {
                        titulo.MoedaCotacaoBancoCentral = faturaDocumento.Documento.Moeda;
                        titulo.ValorOriginalMoedaEstrangeira = valorTotalParcelaAtualMoeda;
                        titulo.ValorMoedaCotacao = faturaDocumento.Documento.ValorCotacaoMoeda ?? 0m;
                    }

                    Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento();
                    tituloDocumento.Titulo = titulo;

                    if (faturaDocumento.Documento.TipoDocumento == TipoDocumentoFaturamento.CTe)
                    {
                        titulo.TipoDocumentoTituloOriginal = "CT-e";
                        titulo.NumeroDocumentoTituloOriginal = faturaDocumento.Documento.CTe.Numero.ToString() + "-" + faturaDocumento.Documento.CTe.Serie.Numero.ToString();

                        tituloDocumento.CTe = faturaDocumento.Documento.CTe;
                        tituloDocumento.TipoDocumento = TipoDocumentoTitulo.CTe;
                    }
                    else
                    {
                        titulo.TipoDocumentoTituloOriginal = "Carga";
                        titulo.NumeroDocumentoTituloOriginal = faturaDocumento.Documento.Carga?.CodigoCargaEmbarcador ?? "";

                        tituloDocumento.Carga = faturaDocumento.Documento.Carga;
                        tituloDocumento.TipoDocumento = TipoDocumentoTitulo.Carga;
                    }

                    tituloDocumento.Valor = valorParcelaAtual;
                    tituloDocumento.ValorTotal = valorTotalParcelaAtual;
                    tituloDocumento.ValorPendente = valorTotalParcelaAtual;
                    tituloDocumento.FaturaDocumento = faturaDocumento;

                    if (fatura.MoedaCotacaoBancoCentral.HasValue && fatura.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                    {
                        tituloDocumento.ValorMoeda = valorTotalParcelaAtualMoeda;
                        tituloDocumento.ValorTotalMoeda = valorTotalParcelaAtualMoeda;
                        tituloDocumento.ValorPendenteMoeda = valorTotalParcelaAtualMoeda;
                        tituloDocumento.ValorCotacaoMoeda = faturaDocumento.Documento.ValorCotacaoMoeda ?? 0m;
                    }

                    titulo.Valor = valorParcelaAtual;
                    titulo.ValorTotal = valorTotalParcelaAtual;
                    titulo.TipoMovimento = fatura.TipoMovimentoUso;
                    titulo.Observacao += " Fat.: " + fatura.Numero.ToString();
                    titulo.Empresa = fatura.Empresa;
                    titulo.DataLancamento = DateTime.Now;

                    repTitulo.Inserir(titulo);
                    repTituloDocumento.Inserir(tituloDocumento);

                    if (gerarBoletoAutomaticamente && titulo.BoletoStatusTitulo == BoletoStatusTitulo.Emitido)
                        servTitulo.IntegrarEmitido(titulo, unidadeTrabalho);

                    if (!servProcessoMovimento.GerarMovimentacao(out erro, fatura.TipoMovimentoUso, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Historico, unidadeTrabalho, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, fatura.Cliente, fatura.GrupoPessoas))
                        return false;

                    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    {
                        foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDesconto in faturaDocumentoAcrescimosDescontos)
                        {
                            decimal valorTotalAcrescimoDesconto = Math.Round(faturaDocumentoAcrescimoDesconto.Valor, 2, MidpointRounding.AwayFromZero);
                            decimal valorParcelaAcrescimoDesconto = Math.Round(Math.Floor((valorTotalAcrescimoDesconto / quantidadeParcelas) * 100) / 100, 2, MidpointRounding.AwayFromZero);
                            decimal valorDiferencaAcrescimoDesconto = valorTotalAcrescimoDesconto - Math.Round(valorParcelaAcrescimoDesconto * quantidadeParcelas, 2, MidpointRounding.AwayFromZero);

                            decimal valorMovimentar = 0;

                            if (i == 0 && fatura.TipoArredondamentoParcelas == TipoArredondamento.Primeira)
                                valorMovimentar = valorParcelaAcrescimoDesconto + valorDiferencaAcrescimoDesconto;
                            else if ((i + 1) == quantidadeParcelas && fatura.TipoArredondamentoParcelas == TipoArredondamento.Ultima)
                                valorMovimentar = valorParcelaAcrescimoDesconto + valorDiferencaAcrescimoDesconto;
                            else
                                valorMovimentar = valorParcelaAcrescimoDesconto;

                            if (!servProcessoMovimento.GerarMovimentacao(out erro, faturaDocumentoAcrescimoDesconto.TipoMovimentoUso, titulo.DataEmissao.Value, valorMovimentar, titulo.Codigo.ToString(), "Ref. ao " + faturaDocumentoAcrescimoDesconto.DescricaoTipoJustificativa.ToLower() + " informado na geração do título " + titulo.Codigo.ToString() + " da fatura " + fatura.Numero.ToString() + ".", unidadeTrabalho, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, fatura.Cliente, fatura.GrupoPessoas))
                                return false;

                            Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto()
                            {
                                Justificativa = faturaDocumentoAcrescimoDesconto.Justificativa,
                                Observacao = faturaDocumentoAcrescimoDesconto.Observacao,
                                Tipo = EnumTipoAcrescimoDescontoTituloDocumento.Geracao,
                                TipoJustificativa = faturaDocumentoAcrescimoDesconto.TipoJustificativa,
                                TipoMovimentoReversao = faturaDocumentoAcrescimoDesconto.TipoMovimentoReversao,
                                TipoMovimentoUso = faturaDocumentoAcrescimoDesconto.TipoMovimentoUso,
                                TituloDocumento = tituloDocumento,
                                Valor = valorMovimentar,
                                Usuario = faturaDocumentoAcrescimoDesconto.Usuario
                            };

                            repTituloDocumentoAcrescimoDesconto.Inserir(tituloDocumentoAcrescimoDesconto);

                            if (tituloDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Acrescimo)
                            {
                                tituloDocumento.ValorAcrescimo += valorMovimentar;
                                titulo.ValorAcrescimo += valorMovimentar;
                            }
                            else
                            {
                                tituloDocumento.ValorDesconto += valorMovimentar;
                                titulo.ValorDesconto += valorMovimentar;
                            }
                        }

                        repTituloDocumento.Atualizar(tituloDocumento);
                        repTitulo.Atualizar(titulo);
                    }

                    if (!QuitarTituloSemValor(out erro, fatura, titulo, unidadeTrabalho, tipoServicoMultisoftware))
                        return false;
                }

                faturaDocumento.Documento.ValorAcrescimo += faturaDocumento.ValorAcrescimo;
                faturaDocumento.Documento.ValorDesconto += faturaDocumento.ValorDesconto;

                if (faturaDocumento.Documento.CTe != null)
                {
                    faturaDocumento.Documento.CTe.CTePendenteIntegracaoFatura = true;
                    repCTe.Atualizar(faturaDocumento.Documento.CTe);
                }

                repDocumentoFaturamento.Atualizar(faturaDocumento.Documento);

                faturaDocumento.TituloGerado = true;

                repFaturaDocumento.Atualizar(faturaDocumento);

                unidadeTrabalho.CommitChanges();

                if (quantidadeDocumentos < 10 || ((x + 1) % 5) == 0)
                {
                    unidadeTrabalho.FlushAndClear();

                    servicoNotificacaoFatura.InformarQuantidadeDocumentosProcessadosFechamento(fatura.Codigo, quantidadeDocumentos, (x + 1));
                }
            }

            servicoNotificacaoFatura.InformarQuantidadeDocumentosProcessadosFechamento(fatura.Codigo, quantidadeDocumentos, quantidadeDocumentos);

            erro = string.Empty;
            return true;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> ObterDocumentosRateadosFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> documentosRateio = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio>();

            List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> tituloDocumentos = repTituloDocumento.BuscarPorFatura(fatura.Codigo);
            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento in tituloDocumentos)
            {
                int indexDocumento = documentosRateio.FindIndex(o => o.Codigo == tituloDocumento.FaturaDocumento.Codigo);

                if (indexDocumento < 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio itemRateio = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio()
                    {
                        Codigo = tituloDocumento.FaturaDocumento.Codigo,
                        ValorTotal = tituloDocumento.FaturaDocumento.ValorACobrar,
                        ValorRateado = tituloDocumento.Valor
                    };

                    documentosRateio.Add(itemRateio);
                }
                else
                {
                    documentosRateio[indexDocumento].ValorRateado += tituloDocumento.Valor;
                }
            }

            return documentosRateio;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> ObterAcrescimosDescontosRateadosFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> acrescimosDescontosRateio = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio>();

            List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> tituloDocumentosAcrescimosDescontos = repTituloDocumentoAcrescimoDesconto.BuscarPorFatura(fatura.Codigo);
            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto in tituloDocumentosAcrescimosDescontos)
            {
                int indexDocumento = acrescimosDescontosRateio.FindIndex(o => o.Codigo == tituloDocumentoAcrescimoDesconto.FaturaDocumentoAcrescimoDesconto.Codigo);

                if (indexDocumento < 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio itemRateio = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio()
                    {
                        Codigo = tituloDocumentoAcrescimoDesconto.FaturaDocumentoAcrescimoDesconto.Codigo,
                        ValorTotal = tituloDocumentoAcrescimoDesconto.FaturaDocumentoAcrescimoDesconto.Valor,
                        ValorRateado = tituloDocumentoAcrescimoDesconto.Valor
                    };

                    acrescimosDescontosRateio.Add(itemRateio);
                }
                else
                {
                    acrescimosDescontosRateio[indexDocumento].ValorRateado += tituloDocumentoAcrescimoDesconto.Valor;
                }
            }

            return acrescimosDescontosRateio;
        }

        private static decimal ObterValorAcrescimoDescontoRateio(List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> faturaDocumentoAcrescimosDescontos, List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> acrescimosDescontosRateio, decimal percentualRateio)
        {
            decimal valorParcelaAcrescimoDesconto = 0m;

            foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDesconto in faturaDocumentoAcrescimosDescontos)
            {
                int indexAcrescimoDescontoRateio = acrescimosDescontosRateio.FindIndex(o => o.Codigo == faturaDocumentoAcrescimoDesconto.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio acrescimoDescontoRateio = null;

                if (indexAcrescimoDescontoRateio < 0)
                {
                    acrescimoDescontoRateio = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio()
                    {
                        Codigo = faturaDocumentoAcrescimoDesconto.Codigo,
                        ValorRateado = 0m,
                        ValorTotal = faturaDocumentoAcrescimoDesconto.Valor
                    };
                }
                else
                    acrescimoDescontoRateio = acrescimosDescontosRateio.ElementAt(indexAcrescimoDescontoRateio);

                decimal valorRateado = Math.Ceiling(faturaDocumentoAcrescimoDesconto.Valor * percentualRateio * 100) / 100;

                if (acrescimoDescontoRateio.ValorTotal < (acrescimoDescontoRateio.ValorRateado + valorRateado))
                    valorRateado = acrescimoDescontoRateio.ValorTotal - acrescimoDescontoRateio.ValorRateado;

                if (faturaDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Acrescimo)
                    valorParcelaAcrescimoDesconto += valorRateado;
                else
                    valorParcelaAcrescimoDesconto -= valorRateado;
            }

            return valorParcelaAcrescimoDesconto;
        }

        private static bool GerarTitulosPorParcela(out string erro, Dominio.Entidades.Embarcador.Fatura.Fatura faturaProcessar, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);
            Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeTrabalho);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto repFaturaDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto repTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Servicos.Embarcador.Hubs.Fatura servicoNotificacaoFatura = new Servicos.Embarcador.Hubs.Fatura();

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(faturaProcessar.Codigo);
            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> faturaAcrescimosDescontos = repFaturaDocumentoAcrescimoDesconto.BuscarPorFatura(fatura.Codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> tiposMovimentoUsoFaturaAcrescimosDescontos = faturaAcrescimosDescontos.Select(o => o.TipoMovimentoUso).Distinct().ToList();

            int codigoBoletoConfiguracao = 0;
            bool gerarBoletoAutomaticamente = false;
            bool enviarBoletoPorEmailAutomaticamente = false;
            bool enviarDocumentacaoFaturamentoCTe = false;

            if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
            {
                gerarBoletoAutomaticamente = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.GerarBoletoAutomaticamente ?? false;
                enviarBoletoPorEmailAutomaticamente = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.EnviarBoletoPorEmailAutomaticamente ?? false;
                enviarDocumentacaoFaturamentoCTe = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.EnviarDocumentacaoFaturamentoCTe ?? false;
                codigoBoletoConfiguracao = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.BoletoConfiguracao?.Codigo ?? 0;
            }
            else if (fatura.GrupoPessoas != null)
            {
                gerarBoletoAutomaticamente = fatura.GrupoPessoas.GerarBoletoAutomaticamente;
                enviarBoletoPorEmailAutomaticamente = fatura.GrupoPessoas.EnviarBoletoPorEmailAutomaticamente;
                enviarDocumentacaoFaturamentoCTe = fatura.GrupoPessoas.EnviarDocumentacaoFaturamentoCTe;
                codigoBoletoConfiguracao = fatura.GrupoPessoas.BoletoConfiguracao?.Codigo ?? 0;
            }
            else if (fatura.Cliente != null && fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
            {
                gerarBoletoAutomaticamente = fatura.Cliente.GerarBoletoAutomaticamente;
                enviarBoletoPorEmailAutomaticamente = fatura.Cliente.EnviarBoletoPorEmailAutomaticamente;
                enviarDocumentacaoFaturamentoCTe = fatura.Cliente.EnviarDocumentacaoFaturamentoCTe;
                codigoBoletoConfiguracao = fatura.Cliente.BoletoConfiguracao?.Codigo ?? 0;
            }
            else if (fatura.Cliente != null && fatura.Cliente.GrupoPessoas != null)
            {
                gerarBoletoAutomaticamente = fatura.Cliente.GrupoPessoas.GerarBoletoAutomaticamente;
                enviarBoletoPorEmailAutomaticamente = fatura.Cliente.GrupoPessoas.EnviarBoletoPorEmailAutomaticamente;
                enviarDocumentacaoFaturamentoCTe = fatura.Cliente.GrupoPessoas.EnviarDocumentacaoFaturamentoCTe;
                codigoBoletoConfiguracao = fatura.Cliente.GrupoPessoas.BoletoConfiguracao?.Codigo ?? 0;
            }

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = null;
            Dominio.Entidades.Cliente tomador = null;

            if (fatura.TipoPessoa == TipoPessoa.GrupoPessoa && fatura.GrupoPessoas != null)
                grupoPessoas = fatura.GrupoPessoas;

            if (fatura.ClienteTomadorFatura != null)
                tomador = fatura.ClienteTomadorFatura;
            else if (fatura.TipoPessoa == TipoPessoa.Pessoa && fatura.Cliente != null)
                tomador = fatura.Cliente;

            if (grupoPessoas == null && tomador?.GrupoPessoas != null)
                grupoPessoas = tomador.GrupoPessoas;

            int quantidadeParcelas = fatura.Parcelas.Count;
            decimal valorTotalParcelasFatura = fatura.Parcelas.Sum(o => o.Valor);

            List<int> codigosParcelas = fatura.Parcelas.Select(o => o.Codigo).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> documentosRateio = ObterDocumentosRateadosFatura(fatura, unidadeTrabalho);
            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio> acrescimosDescontosRateio = ObterAcrescimosDescontosRateadosFatura(fatura, unidadeTrabalho);

            for (int i = 0; i < quantidadeParcelas; i++)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaParcela faturaParcela = repFaturaParcela.BuscarPorCodigo(codigosParcelas[i]);

                if (faturaParcela.TituloGerado)
                    continue;

                List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> faturaDocumentos = repFaturaDocumento.BuscarPorFatura(faturaProcessar.Codigo).OrderBy(o => o.ValorTotalACobrar).ToList();

                int quantidadeDocumentos = faturaDocumentos.Count;

                decimal valorMoedaTotalParcelaFatura = faturaParcela.ValorTotalMoeda;
                decimal valorTotalParcelaFatura = faturaParcela.Valor;
                decimal valorTotalParcelaFaturaRateado = 0m;
                decimal valorMoedaTotalParcelaFaturaRateado = 0m;

                decimal percentualRateio = valorTotalParcelasFatura > 0m ? valorTotalParcelaFatura / valorTotalParcelasFatura : 0m;

                unidadeTrabalho.Start();

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                {
                    DataEmissao = fatura.DataFatura,
                    DataVencimento = faturaParcela.DataVencimento,
                    DataProgramacaoPagamento = faturaParcela.DataVencimento,
                    FaturaParcela = faturaParcela,
                    Pessoa = tomador,
                    GrupoPessoas = grupoPessoas,
                    Sequencia = i + 1,
                    StatusTitulo = StatusTitulo.EmAberto,
                    DataAlteracao = DateTime.Now,
                    TipoTitulo = TipoTitulo.Receber,
                    TipoDocumentoTituloOriginal = "Fatura",
                    NumeroDocumentoTituloOriginal = fatura.Numero.ToString() + "/" + faturaParcela.Sequencia.ToString(),
                    TipoMovimento = fatura.TipoMovimentoUso,
                    Empresa = fatura.Empresa,
                    MoedaCotacaoBancoCentral = fatura.MoedaCotacaoBancoCentral,
                    DataLancamento = DateTime.Now,
                    Usuario = fatura.Usuario,
                    NossoNumero = faturaParcela.NossoNumeroIntegrado
                };

                if (faturaParcela.CodigoTituloIntegracao > 0)
                    titulo.CodigoRecebidoIntegracao = faturaParcela.CodigoTituloIntegracao;

                if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;

                titulo.FormaTitulo = faturaParcela.FormaTitulo ?? servTitulo.ObterFormaTituloGrupoPessoa(titulo);

                if (gerarBoletoAutomaticamente && string.IsNullOrWhiteSpace(titulo.NossoNumero))
                {
                    titulo.BoletoStatusTitulo = BoletoStatusTitulo.Emitido;
                    titulo.BoletoConfiguracao = codigoBoletoConfiguracao > 0 ? repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao) : repBoletoConfiguracao.BuscarPrimeiraConfiguracao();
                    titulo.BoletoGeradoAutomaticamente = enviarBoletoPorEmailAutomaticamente;
                    titulo.BoletoEnviadoPorEmail = false;
                }

                titulo.EnviarDocumentacaoFaturamentoCTe = enviarDocumentacaoFaturamentoCTe;

                string historicoTitulo = "";
                if (fatura.Usuario != null && fatura.DataInicial.HasValue && fatura.DataFinal.HasValue)
                    historicoTitulo = "Título gerado à partir da fatura " + fatura.Numero.ToString() + " / parcela " + faturaParcela.Sequencia.ToString() + ", referente ao período " + fatura.DataInicial.Value.ToString("dd/MM/yyyy") + " a " + fatura.DataFinal.Value.ToString("dd/MM/yyyy") + ". Pelo operador " + fatura.Usuario.Nome;
                else if (fatura.Usuario != null)
                    historicoTitulo = "Título gerado à partir da fatura " + fatura.Numero.ToString() + " / parcela " + faturaParcela.Sequencia.ToString() + ". Pelo operador " + fatura.Usuario.Nome;
                else
                    historicoTitulo = "Título gerado à partir da fatura " + fatura.Numero.ToString() + " / parcela " + faturaParcela.Sequencia.ToString() + ". ";

                titulo.Historico = historicoTitulo;

                if (fatura.ImprimeObservacaoFatura && !string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                    titulo.Observacao = fatura.ObservacaoFatura;

                titulo.Observacao += " Fat.: " + fatura.Numero.ToString();
                titulo.Observacao = titulo.Observacao.Left(1000);

                repTitulo.Inserir(titulo);

                if (gerarBoletoAutomaticamente && titulo.BoletoStatusTitulo == BoletoStatusTitulo.Emitido)
                    servTitulo.IntegrarEmitido(titulo, unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> titulosDocumentos = new List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento>();
                List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto> titulosDocumentosAcrescimosDescontos = new List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto>();

                for (int j = 0; j < quantidadeDocumentos; j++)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = faturaDocumentos[j];

                    List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto> faturaDocumentoAcrescimosDescontos = faturaAcrescimosDescontos.Where(o => o.FaturaDocumento.Codigo == faturaDocumento.Codigo).OrderBy(o => o.TipoJustificativa).ToList();

                    int indexDocumentoRateio = documentosRateio.FindIndex(o => o.Codigo == faturaDocumento.Codigo);

                    Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio documentoRateio = null;

                    if (indexDocumentoRateio < 0)
                    {
                        documentoRateio = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio()
                        {
                            Codigo = faturaDocumento.Codigo,
                            ValorRateado = 0m,
                            ValorTotal = faturaDocumento.ValorACobrar,
                            ValorMoedaRateado = 0m
                        };

                        if (fatura.MoedaCotacaoBancoCentral.HasValue && fatura.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real)
                            documentoRateio.ValorMoedaTotal = faturaDocumento.Documento.ValorTotalMoeda ?? 0m;
                    }
                    else
                        documentoRateio = documentosRateio.ElementAt(indexDocumentoRateio);

                    decimal valorParcelaDocumento = 0m;
                    decimal valorMoedaParcelaDocumento = 0m;
                    decimal valoresAcrescimosDescontos = ObterValorAcrescimoDescontoRateio(faturaDocumentoAcrescimosDescontos, acrescimosDescontosRateio, percentualRateio);

                    if ((i + 1) == quantidadeParcelas)
                    {
                        valorParcelaDocumento = documentoRateio.ValorTotal - documentoRateio.ValorRateado;
                        valorMoedaParcelaDocumento = documentoRateio.ValorMoedaTotal - documentoRateio.ValorMoedaRateado;
                    }
                    else
                    {
                        valorParcelaDocumento = Math.Ceiling(documentoRateio.ValorTotal * percentualRateio * 100) / 100;

                        if (documentoRateio.ValorTotal < (documentoRateio.ValorRateado + valorParcelaDocumento))
                            valorParcelaDocumento = documentoRateio.ValorTotal - documentoRateio.ValorRateado;

                        if ((valorTotalParcelaFaturaRateado + valorParcelaDocumento + valoresAcrescimosDescontos) > valorTotalParcelaFatura)
                            valorParcelaDocumento -= (valorTotalParcelaFaturaRateado + valorParcelaDocumento + valoresAcrescimosDescontos) - valorTotalParcelaFatura;

                        if (valorParcelaDocumento < 0m)
                            valorParcelaDocumento = 0m;

                        if (quantidadeDocumentos == j + 1)
                        {
                            decimal diferenca = valorTotalParcelaFatura - valorTotalParcelaFaturaRateado - valorParcelaDocumento - valoresAcrescimosDescontos;

                            if (diferenca != 0m)
                                valorParcelaDocumento += diferenca;
                        }

                        if (documentoRateio.ValorMoedaTotal > 0m)
                        {
                            valorMoedaParcelaDocumento = Math.Ceiling(documentoRateio.ValorMoedaTotal * percentualRateio * 100) / 100;

                            if (documentoRateio.ValorMoedaTotal < (documentoRateio.ValorMoedaRateado + valorMoedaParcelaDocumento))
                                valorMoedaParcelaDocumento = documentoRateio.ValorMoedaTotal - documentoRateio.ValorMoedaRateado;

                            if (valorMoedaParcelaDocumento < 0m)
                                valorMoedaParcelaDocumento = 0m;

                            if (quantidadeDocumentos == j + 1)
                            {
                                decimal diferencaMoeda = valorMoedaTotalParcelaFatura - valorMoedaTotalParcelaFaturaRateado - valorMoedaParcelaDocumento;

                                if (diferencaMoeda != 0m)
                                    valorMoedaParcelaDocumento += diferencaMoeda;
                            }
                        }
                    }

                    valorTotalParcelaFaturaRateado += valorParcelaDocumento;
                    valorMoedaTotalParcelaFaturaRateado += valorMoedaParcelaDocumento;

                    documentoRateio.ValorRateado += valorParcelaDocumento;
                    documentoRateio.ValorMoedaRateado += valorMoedaParcelaDocumento;

                    if (indexDocumentoRateio < 0)
                        documentosRateio.Add(documentoRateio);
                    else
                        documentosRateio[indexDocumentoRateio] = documentoRateio;

                    Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumento
                    {
                        Titulo = titulo,
                        FaturaDocumento = faturaDocumento,
                        Valor = valorParcelaDocumento,
                        ValorTotal = valorParcelaDocumento,
                        ValorPendente = valorParcelaDocumento,
                        ValorMoeda = valorMoedaParcelaDocumento,
                        ValorTotalMoeda = valorMoedaParcelaDocumento,
                        ValorPendenteMoeda = valorMoedaParcelaDocumento,
                        ValorCotacaoMoeda = faturaDocumento.Documento.ValorCotacaoMoeda ?? 0m
                    };

                    if (faturaDocumento.Documento.TipoDocumento == TipoDocumentoFaturamento.CTe)
                    {
                        tituloDocumento.CTe = faturaDocumento.Documento.CTe;
                        tituloDocumento.TipoDocumento = TipoDocumentoTitulo.CTe;
                    }
                    else
                    {
                        tituloDocumento.Carga = faturaDocumento.Documento.Carga;
                        tituloDocumento.TipoDocumento = TipoDocumentoTitulo.Carga;
                    }

                    repTituloDocumento.Inserir(tituloDocumento);

                    titulo.Valor += valorParcelaDocumento;
                    titulo.ValorTotal += valorParcelaDocumento;
                    titulo.ValorOriginal += valorParcelaDocumento;
                    titulo.ValorPendente += valorParcelaDocumento;
                    titulo.ValorTituloOriginal += valorParcelaDocumento;
                    titulo.ValorOriginalMoedaEstrangeira += valorMoedaParcelaDocumento;

                    foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumentoAcrescimoDesconto faturaDocumentoAcrescimoDesconto in faturaDocumentoAcrescimosDescontos)
                    {
                        int indexAcrescimoDescontoRateio = acrescimosDescontosRateio.FindIndex(o => o.Codigo == faturaDocumentoAcrescimoDesconto.Codigo);

                        Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio acrescimoDescontoRateio = null;

                        if (indexAcrescimoDescontoRateio < 0)
                        {
                            acrescimoDescontoRateio = new Dominio.ObjetosDeValor.Embarcador.Financeiro.ItemRateio()
                            {
                                Codigo = faturaDocumentoAcrescimoDesconto.Codigo,
                                ValorRateado = 0m,
                                ValorTotal = faturaDocumentoAcrescimoDesconto.Valor
                            };
                        }
                        else
                            acrescimoDescontoRateio = acrescimosDescontosRateio.ElementAt(indexAcrescimoDescontoRateio);

                        decimal valorParcelaAcrescimoDesconto = 0m;

                        if ((i + 1) == quantidadeParcelas)
                        {
                            valorParcelaAcrescimoDesconto = acrescimoDescontoRateio.ValorTotal - acrescimoDescontoRateio.ValorRateado;
                        }
                        else
                        {
                            valorParcelaAcrescimoDesconto = Math.Ceiling(faturaDocumentoAcrescimoDesconto.Valor * percentualRateio * 100) / 100;

                            if (acrescimoDescontoRateio.ValorTotal < (acrescimoDescontoRateio.ValorRateado + valorParcelaAcrescimoDesconto))
                                valorParcelaAcrescimoDesconto = acrescimoDescontoRateio.ValorTotal - acrescimoDescontoRateio.ValorRateado;

                            decimal valorChecagemParcela = valorTotalParcelaFaturaRateado;

                            if (faturaDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Acrescimo)
                                valorChecagemParcela += valorParcelaAcrescimoDesconto;
                            else
                                valorChecagemParcela -= valorParcelaAcrescimoDesconto;

                            if (valorChecagemParcela > valorTotalParcelaFatura)
                                valorParcelaAcrescimoDesconto -= valorChecagemParcela - valorTotalParcelaFatura;

                            if (valorParcelaAcrescimoDesconto < 0m)
                                valorParcelaAcrescimoDesconto = 0m;
                        }

                        if (faturaDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Acrescimo)
                            valorTotalParcelaFaturaRateado += valorParcelaAcrescimoDesconto;
                        else
                            valorTotalParcelaFaturaRateado -= valorParcelaAcrescimoDesconto;

                        acrescimoDescontoRateio.ValorRateado += valorParcelaAcrescimoDesconto;

                        if (indexAcrescimoDescontoRateio < 0)
                            acrescimosDescontosRateio.Add(acrescimoDescontoRateio);
                        else
                            acrescimosDescontosRateio[indexAcrescimoDescontoRateio] = acrescimoDescontoRateio;

                        if (valorParcelaAcrescimoDesconto <= 0m)
                            continue;

                        Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto()
                        {
                            Justificativa = faturaDocumentoAcrescimoDesconto.Justificativa,
                            Observacao = faturaDocumentoAcrescimoDesconto.Observacao,
                            Tipo = EnumTipoAcrescimoDescontoTituloDocumento.Geracao,
                            TipoJustificativa = faturaDocumentoAcrescimoDesconto.TipoJustificativa,
                            TipoMovimentoReversao = faturaDocumentoAcrescimoDesconto.TipoMovimentoReversao,
                            TipoMovimentoUso = faturaDocumentoAcrescimoDesconto.TipoMovimentoUso,
                            TituloDocumento = tituloDocumento,
                            Valor = valorParcelaAcrescimoDesconto,
                            FaturaDocumentoAcrescimoDesconto = faturaDocumentoAcrescimoDesconto,
                            Usuario = faturaDocumentoAcrescimoDesconto.Usuario
                        };

                        repTituloDocumentoAcrescimoDesconto.Inserir(tituloDocumentoAcrescimoDesconto);

                        if (tituloDocumentoAcrescimoDesconto.TipoJustificativa == TipoJustificativa.Acrescimo)
                        {
                            tituloDocumento.ValorAcrescimo += valorParcelaAcrescimoDesconto;
                            tituloDocumento.ValorTotal += valorParcelaAcrescimoDesconto;
                            tituloDocumento.ValorPendente += valorParcelaAcrescimoDesconto;

                            titulo.ValorAcrescimo += valorParcelaAcrescimoDesconto;
                            titulo.ValorTotal += valorParcelaAcrescimoDesconto;
                            titulo.ValorOriginal += valorParcelaAcrescimoDesconto;
                            titulo.ValorPendente += valorParcelaAcrescimoDesconto;
                            titulo.ValorTituloOriginal += valorParcelaAcrescimoDesconto;

                            faturaDocumento.Documento.ValorAcrescimo += valorParcelaAcrescimoDesconto;
                        }
                        else
                        {
                            tituloDocumento.ValorDesconto += valorParcelaAcrescimoDesconto;
                            tituloDocumento.ValorTotal -= valorParcelaAcrescimoDesconto;
                            tituloDocumento.ValorPendente -= valorParcelaAcrescimoDesconto;

                            titulo.ValorDesconto += valorParcelaAcrescimoDesconto;
                            titulo.ValorTotal -= valorParcelaAcrescimoDesconto;
                            titulo.ValorOriginal -= valorParcelaAcrescimoDesconto;
                            titulo.ValorPendente -= valorParcelaAcrescimoDesconto;
                            titulo.ValorTituloOriginal -= valorParcelaAcrescimoDesconto;

                            faturaDocumento.Documento.ValorDesconto += valorParcelaAcrescimoDesconto;
                        }

                        titulosDocumentosAcrescimosDescontos.Add(tituloDocumentoAcrescimoDesconto);
                    }

                    if (configuracaoTMS.UtilizaEmissaoMultimodal && faturaDocumento.Documento.CTe != null)
                    {
                        faturaDocumento.Documento.CTe.CTePendenteIntegracaoFatura = true;
                        repCTe.Atualizar(faturaDocumento.Documento.CTe);
                    }

                    repDocumentoFaturamento.Atualizar(faturaDocumento.Documento);
                    repTituloDocumento.Atualizar(tituloDocumento);

                    titulosDocumentos.Add(tituloDocumento);
                }

                if (titulo.ValorTotal != valorTotalParcelaFatura)
                {
                    decimal diferenca = titulo.ValorTotal - valorTotalParcelaFatura;
                    if ((diferenca > 0.01m) || (diferenca < (-0.01m)))
                        throw new Exception("Valor da parcela diferente do valor do título.");
                }

                repTitulo.Atualizar(titulo);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto in titulosDocumentosAcrescimosDescontos)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoUsoAcrescimoDesconto = tiposMovimentoUsoFaturaAcrescimosDescontos.Where(o => o.Codigo == tituloDocumentoAcrescimoDesconto.TipoMovimentoUso.Codigo).FirstOrDefault();

                    if (!servProcessoMovimento.GerarMovimentacao(out erro, tipoMovimentoUsoAcrescimoDesconto, titulo.DataEmissao.Value, tituloDocumentoAcrescimoDesconto.Valor, titulo.Codigo.ToString(), "Ref. ao " + tituloDocumentoAcrescimoDesconto.DescricaoTipoJustificativa.ToLower() + " informado na geração do título " + titulo.Codigo.ToString() + " da fatura " + fatura.Numero.ToString() + ".", unidadeTrabalho, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, fatura.Cliente, fatura.GrupoPessoas, null, null, tipoMovimentoUsoAcrescimoDesconto.Exportar ? tipoMovimentoUsoAcrescimoDesconto.ContasExportacao.ToList() : null, tituloDocumentoAcrescimoDesconto, TipoMovimentoExportacao.AcrescimoDescontoFatura))
                        return false;
                }

                if (!servProcessoMovimento.GerarMovimentacao(out erro, fatura.TipoMovimentoUso, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(), titulo.Historico, unidadeTrabalho, TipoDocumentoMovimento.Faturamento, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, fatura.Cliente, fatura.GrupoPessoas))
                    return false;

                if (!QuitarTituloSemValor(out erro, fatura, titulo, unidadeTrabalho, tipoServicoMultisoftware))
                    return false;

                faturaParcela.TituloGerado = true;

                repFaturaParcela.Atualizar(faturaParcela);

                unidadeTrabalho.CommitChanges();

                servicoNotificacaoFatura.InformarQuantidadeDocumentosProcessadosFechamento(fatura.Codigo, quantidadeParcelas, (i + 1));

                unidadeTrabalho.FlushAndClear();
            }

            erro = string.Empty;
            return true;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura> ObterDadosContabilizacao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Empresa empresaLayout, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Fatura.FaturaDocumento repositorioFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);

            ConfiguracaoContabil.ConfiguracaoCentroResultado servicoConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            ConfiguracaoContabil.ConfiguracaoContaContabil servicoConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();

            Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = servicoConfiguracaoContaContabil.ObterConfiguracaoContaContabil(null, null, fatura.ClienteTomadorFatura, null, empresaLayout, fatura.TipoOperacao, null, null, null, unidadeTrabalho);
            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = servicoConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(null, null, null, null, fatura.ClienteTomadorFatura, null, empresaLayout, empresa, fatura.TipoOperacao, null, null, null, null, unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            if ((configuracaoContaContabil == null) || (configuracaoCentroResultado == null))
                return new List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura>();

            List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura> contabilizacoesFatura = new List<Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura>();

            decimal totalReceber = repositorioFaturaDocumento.BuscarTotalReceber(fatura.Codigo);
            decimal totalPrestacao = repositorioFaturaDocumento.BuscarTotalDaPrestacao(fatura.Codigo);

            decimal basePisCofins = totalPrestacao;
            decimal valorICMS = repositorioFaturaDocumento.BuscarTotalICMS(fatura.Codigo);

            if (configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins)
                basePisCofins = totalReceber - valorICMS;

            decimal valorICMSST = repositorioFaturaDocumento.BuscarTotalICMSSST(fatura.Codigo);
            decimal valorISS = repositorioFaturaDocumento.BuscarTotalISS(fatura.Codigo);
            decimal valorISSRetido = repositorioFaturaDocumento.BuscarTotalISSRetido(fatura.Codigo);
            decimal valorEmpresaCOFINS = fatura.Transportador?.Configuracao.AliquotaCOFINS ?? 0;
            decimal valorCOFINS = basePisCofins * (valorEmpresaCOFINS / 100);
            decimal valorEmpresaPIS = fatura.Transportador?.Configuracao.AliquotaPIS ?? 0;
            decimal valorPIS = basePisCofins * (valorEmpresaPIS / 100);

            foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
            {
                Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura contabilizacaoFatura = new Dominio.ObjetosDeValor.Embarcador.Fatura.ContabilizacaoFatura();

                contabilizacaoFatura.codigoContaContabil = configuracao.PlanoContabilidade;
                contabilizacaoFatura.descricaoContaContabil = configuracao.PlanoContaDescricao;
                contabilizacaoFatura.codigoCentroResultado = configuracaoCentroResultado.CentroResultadoContabilizacao.PlanoContabilidade;
                contabilizacaoFatura.descricaoCentroResultado = configuracaoCentroResultado.CentroResultadoContabilizacao.Descricao;
                contabilizacaoFatura.tipoContabilizacao = (configuracao.TipoContabilizacao == TipoContabilizacao.Credito) ? "Crédito" : "Débito";

                if (configuracao.TipoContaContabil == TipoContaContabil.ICMS)
                {
                    if (valorICMS <= 0)
                        continue;
                    contabilizacaoFatura.valor = valorICMS.ToString("n2");
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.ICMSST)
                {
                    if (valorICMSST <= 0)//quando não for ST não insere a linha
                        continue;
                    contabilizacaoFatura.valor = valorICMSST.ToString("n2");
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.PIS)
                {
                    if (fatura.Transportador?.Configuracao != null)
                    {
                        if (valorPIS > 0)
                            contabilizacaoFatura.valor = valorPIS.ToString("n2");
                        else
                            continue;
                    }
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.COFINS)
                {
                    if (fatura.Transportador?.Configuracao != null)
                    {
                        if (valorCOFINS > 0)
                            contabilizacaoFatura.valor = valorCOFINS.ToString("n2");
                        else
                            continue;
                    }
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.FreteLiquido)
                {
                    decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                    contabilizacaoFatura.valor = freteLiquido.ToString("n2");
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.FreteLiquido2)
                {
                    decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                    contabilizacaoFatura.valor = freteLiquido.ToString("n2");
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.FreteLiquido9)
                {
                    decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS - valorISS;
                    contabilizacaoFatura.valor = freteLiquido.ToString("n2");
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.TotalReceber)
                {
                    contabilizacaoFatura.valor = totalReceber.ToString("n2");
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.ISS)
                {
                    contabilizacaoFatura.valor = (valorISS).ToString("n2");
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.ISSRetido)
                {
                    contabilizacaoFatura.valor = (valorISSRetido).ToString("n2");
                }
                else if (configuracao.TipoContaContabil == TipoContaContabil.FreteValor)
                {
                    decimal freteValor = totalReceber - valorICMS - valorISS;
                    contabilizacaoFatura.valor = freteValor.ToString("n2");
                }

                contabilizacoesFatura.Add(contabilizacaoFatura);
            }

            return contabilizacoesFatura;
        }

        public Dominio.Entidades.LayoutEDI ObterLayoutEDIFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, List<Dominio.Enumeradores.TipoLayoutEDI> tipoLayoutEDI)
        {
            if (fatura.TipoOperacao?.LayoutsEDI?.Count > 0)
                return fatura.TipoOperacao.LayoutsEDI.Where(o => tipoLayoutEDI.Contains(o.LayoutEDI.Tipo)).Select(o => o.LayoutEDI).FirstOrDefault();
            else if (fatura.Cliente?.LayoutsEDI?.Count > 0)
                return fatura.Cliente.LayoutsEDI.Where(o => tipoLayoutEDI.Contains(o.LayoutEDI.Tipo)).Select(o => o.LayoutEDI).FirstOrDefault();
            else if (fatura.Cliente?.GrupoPessoas?.LayoutsEDI?.Count > 0)
                return fatura.Cliente.GrupoPessoas.LayoutsEDI.Where(o => tipoLayoutEDI.Contains(o.LayoutEDI.Tipo)).Select(o => o.LayoutEDI).FirstOrDefault();
            else if (fatura.GrupoPessoas?.LayoutsEDI?.Count > 0)
                return fatura.GrupoPessoas.LayoutsEDI.Where(o => tipoLayoutEDI.Contains(o.LayoutEDI.Tipo)).Select(o => o.LayoutEDI).FirstOrDefault();
            else if (fatura.Transportador?.TransportadorLayoutsEDI?.Count > 0)
                return fatura.Transportador.TransportadorLayoutsEDI.Where(o => tipoLayoutEDI.Contains(o.LayoutEDI.Tipo)).Select(o => o.LayoutEDI).FirstOrDefault();
            else
                return null;
        }

        public void SalvarTituloVencimentoDocumentoFaturamento(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            Dominio.Entidades.Embarcador.Fatura.FaturaParcela primeiraParcela = repFaturaParcela.BuscarPrimeiraPorFatura(fatura.Codigo);
            if (primeiraParcela == null)
                return;

            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(fatura.Codigo);
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(primeiraParcela.CodigoTitulo);
            foreach (var documento in documentos)
            {
                if (documento.Documento != null && primeiraParcela.CodigoTitulo > 0)
                {
                    documento.Documento.Fatura = fatura;
                    documento.Documento.Titulo = titulo;
                    documento.Documento.DataVencimento = documento.Documento.Titulo?.DataVencimento;

                    repDocumentoFaturamento.Atualizar(documento.Documento);
                }
            }
        }

        public void GerarIntegracoesFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, bool ativarIntegracaoEMP = false)
        {
            if ((fatura.FaturaGeradaPelaIntegracao || fatura.FaturaRecebidaDeIntegracao) && !ativarIntegracaoEMP)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem repIntegracaoPortalCabotagem = new Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao> listaFaturaIntegracao = repFaturaIntegracao.BuscarPorFatura(fatura.Codigo);

            // Tipos de layouts para gerar
            List<Dominio.Enumeradores.TipoLayoutEDI> layouts = new List<Dominio.Enumeradores.TipoLayoutEDI>() {
                Dominio.Enumeradores.TipoLayoutEDI.DOCCOB,
                Dominio.Enumeradores.TipoLayoutEDI.DOCCOB_CT,
                Dominio.Enumeradores.TipoLayoutEDI.INTPFAR,
                Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP,
                Dominio.Enumeradores.TipoLayoutEDI.DOCCOB_VAXXINOVA
            };

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                layouts.Add(Dominio.Enumeradores.TipoLayoutEDI.PREFAT);

            if (listaFaturaIntegracao.Count > 0)
            {
                if (listaFaturaIntegracao.Any(obj => obj.LayoutEDI?.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR))
                    return;

                for (int i = 0; i < listaFaturaIntegracao.Count(); i++)
                    repFaturaIntegracao.Deletar(listaFaturaIntegracao[i]);
            }

            bool ediTipoOperacao = false;
            if (fatura.TipoOperacao != null)
            {
                if (fatura.TipoOperacao.LayoutsEDI != null && fatura.TipoOperacao.LayoutsEDI.Count() > 0)
                {
                    for (int i = 0; i < fatura.TipoOperacao.LayoutsEDI.Count(); i++)
                    {
                        ediTipoOperacao = true;
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI layoutEDI = fatura.TipoOperacao.LayoutsEDI[i];

                        if (configuracaoTMS.UtilizaEmissaoMultimodal && layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                            AdicionarEDI(fatura.TipoOperacao.LayoutsEDI[i].LayoutEDI, fatura.TipoOperacao.LayoutsEDI[i].TipoIntegracao, fatura, null, unitOfWork, tipoServicoMultisoftware, Auditado, fatura.TipoOperacao.LayoutsEDI[i].ReenviarAutomaticamenteOutraVezAposMinutos);
                        else if (layouts.Contains(layoutEDI.LayoutEDI.Tipo) && repFaturaIntegracao.ContarPorFatura(fatura.Codigo, TipoIntegracaoFatura.EDI, layoutEDI.LayoutEDI.Codigo) <= 0)
                            AdicionarEDI(fatura.TipoOperacao.LayoutsEDI[i].LayoutEDI, fatura.TipoOperacao.LayoutsEDI[i].TipoIntegracao, fatura, null, unitOfWork, tipoServicoMultisoftware, Auditado, fatura.TipoOperacao.LayoutsEDI[i].ReenviarAutomaticamenteOutraVezAposMinutos);

                    }
                }
            }

            if (!ediTipoOperacao)
            {
                if (fatura.Cliente != null)
                {
                    if (fatura.Cliente.LayoutsEDI != null && fatura.Cliente.LayoutsEDI.Count() > 0)
                    {
                        for (int i = 0; i < fatura.Cliente.LayoutsEDI.Count(); i++)
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI layoutEDI = fatura.Cliente.LayoutsEDI[i];

                            if (configuracaoTMS.UtilizaEmissaoMultimodal && layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                                AdicionarEDI(fatura.Cliente.LayoutsEDI[i].LayoutEDI, fatura.Cliente.LayoutsEDI[i].TipoIntegracao, fatura, null, unitOfWork, tipoServicoMultisoftware, Auditado, fatura.Cliente.LayoutsEDI[i].ReenviarAutomaticamenteOutraVezAposMinutos);
                            else if (layouts.Contains(layoutEDI.LayoutEDI.Tipo) && repFaturaIntegracao.ContarPorFatura(fatura.Codigo, TipoIntegracaoFatura.EDI, layoutEDI.LayoutEDI.Codigo) <= 0)
                                AdicionarEDI(fatura.Cliente.LayoutsEDI[i].LayoutEDI, fatura.Cliente.LayoutsEDI[i].TipoIntegracao, fatura, null, unitOfWork, tipoServicoMultisoftware, Auditado, fatura.Cliente.LayoutsEDI[i].ReenviarAutomaticamenteOutraVezAposMinutos);
                        }
                    }
                    else
                    {
                        if (fatura.Cliente.GrupoPessoas != null)
                        {
                            if (fatura.Cliente.GrupoPessoas.LayoutsEDI != null && fatura.Cliente.GrupoPessoas.LayoutsEDI.Count() > 0)
                            {
                                for (int i = 0; i < fatura.Cliente.GrupoPessoas.LayoutsEDI.Count(); i++)
                                {
                                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutEDI = fatura.Cliente.GrupoPessoas.LayoutsEDI[i];

                                    if (configuracaoTMS.UtilizaEmissaoMultimodal && layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                                        AdicionarEDI(fatura.Cliente.GrupoPessoas.LayoutsEDI[i].LayoutEDI, fatura.Cliente.GrupoPessoas.LayoutsEDI[i].TipoIntegracao, fatura, null, unitOfWork, tipoServicoMultisoftware, Auditado, fatura.Cliente.GrupoPessoas.LayoutsEDI[i].ReenviarAutomaticamenteOutraVezAposMinutos);
                                    else if (layouts.Contains(layoutEDI.LayoutEDI.Tipo) && repFaturaIntegracao.ContarPorFatura(fatura.Codigo, TipoIntegracaoFatura.EDI, layoutEDI.LayoutEDI.Codigo) <= 0)
                                        AdicionarEDI(fatura.Cliente.GrupoPessoas.LayoutsEDI[i].LayoutEDI, fatura.Cliente.GrupoPessoas.LayoutsEDI[i].TipoIntegracao, fatura, null, unitOfWork, tipoServicoMultisoftware, Auditado, fatura.Cliente.GrupoPessoas.LayoutsEDI[i].ReenviarAutomaticamenteOutraVezAposMinutos);
                                }
                            }
                        }
                    }
                }

                if (fatura.GrupoPessoas != null)
                {
                    if (fatura.GrupoPessoas.LayoutsEDI != null && fatura.GrupoPessoas.LayoutsEDI.Count() > 0)
                    {
                        for (int i = 0; i < fatura.GrupoPessoas.LayoutsEDI.Count(); i++)
                        {
                            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutEDI = fatura.GrupoPessoas.LayoutsEDI[i];
                            if (configuracaoTMS.UtilizaEmissaoMultimodal && layouts.Contains(layoutEDI.LayoutEDI.Tipo) && repFaturaIntegracao.ContarPorFatura(fatura.Codigo, TipoIntegracaoFatura.EDI, layoutEDI.LayoutEDI.Codigo) <= 0)
                                AdicionarEDI(fatura.GrupoPessoas.LayoutsEDI[i].LayoutEDI, fatura.GrupoPessoas.LayoutsEDI[i].TipoIntegracao, fatura, null, unitOfWork, tipoServicoMultisoftware, Auditado, fatura.GrupoPessoas.LayoutsEDI[i].ReenviarAutomaticamenteOutraVezAposMinutos);
                            else if (layouts.Contains(layoutEDI.LayoutEDI.Tipo) && repFaturaIntegracao.ContarPorFatura(fatura.Codigo, TipoIntegracaoFatura.EDI, layoutEDI.LayoutEDI.Codigo) <= 0)
                                AdicionarEDI(fatura.GrupoPessoas.LayoutsEDI[i].LayoutEDI, fatura.GrupoPessoas.LayoutsEDI[i].TipoIntegracao, fatura, null, unitOfWork, tipoServicoMultisoftware, Auditado, fatura.GrupoPessoas.LayoutsEDI[i].ReenviarAutomaticamenteOutraVezAposMinutos);
                        }
                    }
                }

                if (fatura.Transportador != null)
                {
                    if (fatura.Transportador.TransportadorLayoutsEDI != null && fatura.Transportador.TransportadorLayoutsEDI.Count() > 0)
                    {
                        for (int i = 0; i < fatura.Transportador.TransportadorLayoutsEDI.Count(); i++)
                        {
                            Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutEDI = fatura.Transportador.TransportadorLayoutsEDI[i];
                            if (configuracaoTMS.UtilizaEmissaoMultimodal && layouts.Contains(layoutEDI.LayoutEDI.Tipo))
                                AdicionarEDI(layoutEDI.LayoutEDI, layoutEDI.TipoIntegracao, fatura, fatura.Transportador, unitOfWork, tipoServicoMultisoftware, Auditado, layoutEDI.ReenviarAutomaticamenteOutraVezAposMinutos);
                            else if (layouts.Contains(layoutEDI.LayoutEDI.Tipo) && repFaturaIntegracao.ContarPorFatura(fatura.Codigo, TipoIntegracaoFatura.EDI, layoutEDI.LayoutEDI.Codigo) <= 0)
                                AdicionarEDI(layoutEDI.LayoutEDI, layoutEDI.TipoIntegracao, fatura, fatura.Transportador, unitOfWork, tipoServicoMultisoftware, Auditado, layoutEDI.ReenviarAutomaticamenteOutraVezAposMinutos);
                        }
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracoes = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura? tipoEnvioFatura = null;
            Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus integracaoGlobus = repIntegracaoGlobus.Buscar();

            if (!(integracaoGlobus?.PossuiIntegracao ?? false))
            {
                if ((configuracaoFinanceiro?.UtilizarPreenchimentoTomadorFaturaConfiguracao ?? false) && fatura?.Tomador != null && fatura.Tomador.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    tipoIntegracoes.Add(fatura?.Tomador?.TipoIntegracao);
                    tipoEnvioFatura = fatura?.Tomador?.TipoEnvioFatura;
                }
                else if ((configuracaoFinanceiro?.UtilizarPreenchimentoTomadorFaturaConfiguracao ?? false) && fatura?.Tomador != null && fatura?.Tomador?.GrupoPessoas != null)
                {
                    tipoIntegracoes.Add(fatura.Tomador.GrupoPessoas?.TipoIntegracao);
                    tipoEnvioFatura = fatura.Tomador.GrupoPessoas?.TipoEnvioFatura;
                }

                if (fatura.Cliente != null && fatura.Cliente.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    tipoIntegracoes.Add(fatura.Cliente.TipoIntegracao);
                    tipoEnvioFatura = fatura.Cliente.TipoEnvioFatura;
                }
                else if (fatura.GrupoPessoas != null)
                {
                    tipoIntegracoes.Add(fatura.GrupoPessoas.TipoIntegracao);
                    tipoEnvioFatura = fatura.GrupoPessoas.TipoEnvioFatura;
                }
                else if (fatura.Cliente != null && fatura.Cliente.GrupoPessoas != null)
                {
                    tipoIntegracoes.Add(fatura.Cliente.GrupoPessoas.TipoIntegracao);
                    tipoEnvioFatura = fatura.Cliente.GrupoPessoas.TipoEnvioFatura;
                }
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoSAP repIntegracaoSAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(unitOfWork);
            Repositorio.Embarcador.Filiais.FilialTipoIntegracao repFilialTipoIntegracao = new(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP integracaoSAP = repIntegracaoSAP.Buscar();

            List<TipoIntegracao> tiposIntegracoesFiliaisPermitidas = new List<TipoIntegracao>();

            if (fatura.Filial != null)
                tiposIntegracoesFiliaisPermitidas = repFilialTipoIntegracao.BuscarTiposIntegracaoPorFilial(fatura.Filial.Codigo);

            List<TipoIntegracao> tiposIntegracoesPermitidas = new()
            {
                TipoIntegracao.Natura,
                TipoIntegracao.Avon,
                TipoIntegracao.Protheus,
                TipoIntegracao.Marilan,
                TipoIntegracao.LoggiFaturas,
                TipoIntegracao.Calisto,
                TipoIntegracao.Globus,
                TipoIntegracao.Olfar,
                TipoIntegracao.Efesus,
                TipoIntegracao.ItalacFaturas,
            };

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPortalCabotagem integracaoPortalCabotagem = repIntegracaoPortalCabotagem.BuscarPrimeiroRegistro();

            if ((integracaoPortalCabotagem?.AtivarEnvioPDFBoleto ?? false) || (integracaoPortalCabotagem?.AtivarEnvioPDFFatura ?? false))
                tipoIntegracoes.Add(repTipoIntegracao.BuscarPorTipo(TipoIntegracao.PortalCabotagem));

            if (tiposIntegracoesFiliaisPermitidas.Count > 0)
                tipoIntegracoes = repTipoIntegracao.BuscarPorTipos(tiposIntegracoesFiliaisPermitidas);

            if (tipoIntegracoes.Count == 0)
                tipoIntegracoes = repTipoIntegracao.BuscarPorTipos(tiposIntegracoesPermitidas);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tipoIntegracoes)
            {
                if (tipoIntegracao != null && tiposIntegracoesPermitidas.Contains(tipoIntegracao.Tipo))
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracaoDuplicada = repFaturaIntegracao.BuscarPorFaturaETipo(fatura.Codigo, tipoIntegracao.Codigo);

                    if (faturaIntegracaoDuplicada != null)
                        continue;

                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao();
                    integracao.Fatura = fatura;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.Tentativas = 0;
                    integracao.TipoIntegracao = tipoIntegracao;
                    integracao.TipoIntegracaoFatura = TipoIntegracaoFatura.Fatura;
                    integracao.MensagemRetorno = string.Empty;
                    integracao.DataEnvio = DateTime.Now;
                    integracao.Tentativas = 0;

                    if (Auditado != null)
                        repFaturaIntegracao.Inserir(integracao, Auditado);
                    else
                        repFaturaIntegracao.Inserir(integracao);
                }
            }

            if (fatura.FaturaRecebidaDeIntegracao != true && (integracaoIntercab?.AtivarIntegracaoFatura ?? false))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoIntercab = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Intercab);
                if (tipoIntegracaoIntercab != null)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao();
                    integracao.Fatura = fatura;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.Tentativas = 0;
                    integracao.TipoIntegracao = tipoIntegracaoIntercab;
                    integracao.TipoIntegracaoFatura = TipoIntegracaoFatura.Fatura;
                    integracao.MensagemRetorno = string.Empty;
                    integracao.DataEnvio = DateTime.Now;
                    integracao.Tentativas = 0;

                    if (Auditado != null)
                        repFaturaIntegracao.Inserir(integracao, Auditado);
                    else
                        repFaturaIntegracao.Inserir(integracao);

                }
            }
            if ((fatura.FaturaRecebidaDeIntegracao != true || ativarIntegracaoEMP) && (integracaoEMP?.AtivarIntegracaoFaturaEMP ?? false))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.EMP);
                if (tipoIntegracao != null)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao();
                    integracao.Fatura = fatura;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.Tentativas = 0;
                    integracao.TipoIntegracao = tipoIntegracao;
                    integracao.TipoIntegracaoFatura = TipoIntegracaoFatura.Fatura;
                    integracao.MensagemRetorno = string.Empty;
                    integracao.DataEnvio = DateTime.Now;
                    integracao.Tentativas = 0;

                    if (Auditado != null)
                        repFaturaIntegracao.Inserir(integracao, Auditado);
                    else
                        repFaturaIntegracao.Inserir(integracao);
                }
            }
            if ((fatura.FaturaRecebidaDeIntegracao != true || ativarIntegracaoEMP) && (integracaoEMP?.AtivarIntegracaoNFTPEMP ?? false))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.NFTP);
                if (tipoIntegracao != null)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao();
                    integracao.Fatura = fatura;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.Tentativas = 0;
                    integracao.TipoIntegracao = tipoIntegracao;
                    integracao.TipoIntegracaoFatura = TipoIntegracaoFatura.Fatura;
                    integracao.MensagemRetorno = string.Empty;
                    integracao.DataEnvio = DateTime.Now;
                    integracao.Tentativas = 0;

                    if (Auditado != null)
                        repFaturaIntegracao.Inserir(integracao, Auditado);
                    else
                        repFaturaIntegracao.Inserir(integracao);
                }
            }

            if (configuracaoTMS.TipoImpressaoFatura == TipoImpressaoFatura.Multimodal || (tipoEnvioFatura.HasValue && tipoEnvioFatura.Value != TipoEnvioFatura.Todos))
            {
                if (fatura.FaturaRecebidaDeIntegracao != true)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao();
                    integracao.Fatura = fatura;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.Tentativas = 0;
                    integracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Email);
                    integracao.TipoIntegracaoFatura = TipoIntegracaoFatura.Fatura;
                    integracao.MensagemRetorno = string.Empty;
                    integracao.DataEnvio = DateTime.Now;
                    integracao.Tentativas = 0;

                    if (integracao.TipoIntegracao != null)
                    {
                        if (Auditado != null)
                            repFaturaIntegracao.Inserir(integracao, Auditado);
                        else
                            repFaturaIntegracao.Inserir(integracao);
                    }
                }
            }

            if (integracaoSAP != null && integracaoSAP.PossuiIntegracao && integracaoSAP.RealizarIntegracaoComDadosFatura && !fatura.FaturaRecebidaDeIntegracao)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao();
                integracao.Fatura = fatura;
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.Tentativas = 0;
                integracao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.SAP);
                integracao.TipoIntegracaoFatura = TipoIntegracaoFatura.Fatura;
                integracao.MensagemRetorno = string.Empty;
                integracao.DataEnvio = DateTime.Now;
                integracao.Tentativas = 0;

                if (integracao.TipoIntegracao != null)
                {
                    if (Auditado != null)
                        repFaturaIntegracao.Inserir(integracao, Auditado);
                    else
                        repFaturaIntegracao.Inserir(integracao);
                }
            }

        }

        public void SalvarArquivosIntegracaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno)
        {
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = AdicionarArquivoTransacaoFatura(arquivoRequisicao, arquivoRetorno, faturaIntegracao.MensagemRetorno);

            if (faturaIntegracaoArquivo == null)
                return;

            if (faturaIntegracao.ArquivosIntegracao == null)
                faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();

            faturaIntegracao.ArquivosIntegracao.Add(faturaIntegracaoArquivo);
        }

        public static object ObterDetalhesAprovacao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorioAprovacaoAlcadaFatura = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(unitOfWork);
            int aprovacoes = repositorioAprovacaoAlcadaFatura.ContarAprovacoes(fatura.Codigo);
            int aprovacoesNecessarias = repositorioAprovacaoAlcadaFatura.ContarAprovacoesNecessarias(fatura.Codigo);
            int reprovacoes = repositorioAprovacaoAlcadaFatura.ContarReprovacoes(fatura.Codigo);

            return new
            {
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                fatura.DescricaoSituacao,
                fatura.Situacao,
                fatura.Codigo
            };
        }
        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork = null)
        {
            Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorioAprovacaoAlcadaFatura = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(unitOfWork);
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;
            if (!repositorioAprovacaoAlcadaFatura.PossuiRegrasCadastradas())
                return;

            List<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura> regras = ObterRegrasAutorizacao(fatura);

            if (regras.Count > 0)
                CriarRegrasAprovacao(fatura, regras, tipoServicoMultisoftware);
            else
                fatura.Situacao = SituacaoFatura.SemRegraAprovacao;

            fatura.Etapa = (fatura.Situacao == SituacaoFatura.EmFechamento) ? EtapaFatura.Fechamento : EtapaFatura.Aprovacao;
        }
        public bool GerarFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir objValorfaturaInserir, out string msgErro, out string msgAlert, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Usuario Usuario)
        {
            msgErro = "";
            msgAlert = "";
            try
            {
                #region Inserir Fatura

                Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(_unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
                Repositorio.Pais repPais = new Repositorio.Pais(_unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                if (objValorfaturaInserir.DataFatura < DateTime.MinValue)
                {
                    msgAlert = "Por favor informe a data de emissão da fatura.";
                    return false;
                }
                if (objValorfaturaInserir.DataFatura > DateTime.Now.Date)
                {
                    msgAlert = "Por favor informe a data de emissão da fatura menor que a data atual.";
                    return false;
                }

                if (objValorfaturaInserir.DataInicial < DateTime.MinValue)
                {
                    msgAlert = "Por favor informe a data de inicial da fatura.";
                    return false;
                }

                if (objValorfaturaInserir.DataFatura < objValorfaturaInserir.DataInicial)
                {
                    msgAlert = "Por favor informe a data de emissão maior que a data inicial.";
                    return false;
                }

                if (objValorfaturaInserir.TipoPessoa == null || objValorfaturaInserir.TipoPessoa == 0)
                {
                    msgErro = "Não foi selecionado nenhum tipo de pessoa para a geração da fatura.";
                    return false;
                }

                if (objValorfaturaInserir.DataInicial == null || objValorfaturaInserir.DataInicial == DateTime.MinValue)
                {
                    msgErro = "Não foi selecionado a data inicial.";
                    return false;
                }

                if (objValorfaturaInserir.DataFinal == null || objValorfaturaInserir.DataFinal == DateTime.MinValue)
                {
                    msgErro = "Não foi selecionado a data final.";
                    return false;
                }

                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = objValorfaturaInserir.Codigo > 0 ? repFatura.BuscarPorCodigo(objValorfaturaInserir.Codigo, true) : null;

                if (fatura != null && fatura.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas)
                {
                    msgAlert = "A fatura está em processo de lançamento de documentos, não sendo possível alterar a mesma.";
                    return false;
                }

                if (fatura == null)
                {
                    fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura();
                    fatura.DataGeracaoFatura = DateTime.Now;
                    if (ConfiguracaoEmbarcador.GerarNumeracaoFaturaAnual)
                    {
                        int anoAtual = DateTime.Now.Year;
                        fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao(anoAtual);
                        anoAtual = (anoAtual % 100);
                        if (fatura.ControleNumeracao == 1 || (fatura.ControleNumeracao < ((anoAtual * 1000000) + 1)))
                            fatura.ControleNumeracao = (anoAtual * 1000000) + 1;
                    }
                    else
                        fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao();
                }

                fatura.DataInicial = objValorfaturaInserir.DataInicial;
                fatura.DataFinal = objValorfaturaInserir.DataFinal;
                fatura.DataFatura = objValorfaturaInserir.DataFatura;
                fatura.NumeroFaturaOriginal = objValorfaturaInserir.NumeroFaturaOriginal;
                fatura.NovoModelo = true;

                if (objValorfaturaInserir.GerarDocumentosAutomaticamente)
                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas;
                else
                    fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Documentos;

                fatura.Situacao = objValorfaturaInserir.Situacao;
                fatura.Observacao = objValorfaturaInserir.Observacao;
                fatura.Usuario = Usuario;
                fatura.TipoPessoa = objValorfaturaInserir.TipoPessoa;
                fatura.GerarDocumentosAutomaticamente = objValorfaturaInserir.GerarDocumentosAutomaticamente;
                fatura.NaoUtilizarMoedaEstrangeira = objValorfaturaInserir.NaoUtilizarMoedaEstrangeira;
                fatura.GerarDocumentosApenasCanhotosAprovados = objValorfaturaInserir.GerarDocumentosApenasCanhotosAprovados;

                fatura.AliquotaICMS = objValorfaturaInserir.AliquotaICMS > 0m ? (decimal?)objValorfaturaInserir.AliquotaICMS : null;
                fatura.TipoCarga = (objValorfaturaInserir?.TipoCarga?.Codigo ?? 0) > 0 ? repTipoCarga.BuscarPorCodigo(objValorfaturaInserir.TipoCarga.Codigo) : null;
                fatura.ImprimeObservacaoFatura = objValorfaturaInserir.ImprimeObservacaoFatura;

                if (Double.TryParse((objValorfaturaInserir?.Cliente?.CPFCNPJ ?? ""), out double cpfCnpjCliente))
                    fatura.Cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                if (Double.TryParse((objValorfaturaInserir?.Tomador?.CPFCNPJ ?? ""), out double cpfCnpjTomador))
                    fatura.Tomador = repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

                if (objValorfaturaInserir.GrupoPessoas != null)
                    if (objValorfaturaInserir.GrupoPessoas.Codigo > 0)
                        fatura.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(objValorfaturaInserir.GrupoPessoas.Codigo);

                if (objValorfaturaInserir.TipoOperacao != null)
                    if (objValorfaturaInserir.TipoOperacao.Codigo > 0)
                        fatura.TipoOperacao = repTipoOperacao.BuscarPorCodigo(objValorfaturaInserir.TipoOperacao.Codigo);

                if (objValorfaturaInserir.Transportador != null)
                    if (objValorfaturaInserir.Transportador.Codigo > 0)
                        fatura.Transportador = repEmpresa.BuscarPorCodigo(objValorfaturaInserir.Transportador.Codigo);

                if (objValorfaturaInserir.PedidoViagemNavio != null)
                    if (objValorfaturaInserir.PedidoViagemNavio.Codigo > 0)
                        fatura.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(objValorfaturaInserir.PedidoViagemNavio.Codigo);

                if (objValorfaturaInserir.TerminalOrigem != null)
                    if (objValorfaturaInserir.TerminalOrigem.Codigo > 0)
                        fatura.TerminalOrigem = repTerminal.BuscarPorCodigo(objValorfaturaInserir.TerminalOrigem.Codigo);


                if (objValorfaturaInserir.TerminalDestino != null)
                    if (objValorfaturaInserir.TerminalDestino.Codigo > 0)
                        fatura.TerminalDestino = repTerminal.BuscarPorCodigo(objValorfaturaInserir.TerminalDestino.Codigo);

                if (objValorfaturaInserir.Origem != null)
                    if (objValorfaturaInserir.Origem.Codigo > 0)
                        fatura.Origem = repLocalidade.BuscarPorCodigo(objValorfaturaInserir.Origem.Codigo);

                if (objValorfaturaInserir.PaisOrigem != null)
                    if (objValorfaturaInserir.PaisOrigem.CodigoPais > 0)
                        fatura.PaisOrigem = repPais.BuscarPorCodigo(objValorfaturaInserir.PaisOrigem.CodigoPais);

                if (objValorfaturaInserir.CodigoFilial > 0)
                    fatura.Filial = repFilial.BuscarPorCodigo(objValorfaturaInserir.CodigoFilial);

                if (objValorfaturaInserir.Destino != null)
                    if (objValorfaturaInserir.Destino.Codigo > 0)
                        fatura.Destino = repLocalidade.BuscarPorCodigo(objValorfaturaInserir.Destino.Codigo);

                fatura.NumeroBooking = objValorfaturaInserir.NumeroBooking;
                fatura.DataBaseCRT = objValorfaturaInserir.DataBaseCRT;

                if (objValorfaturaInserir.Veiculo != null)
                    if (objValorfaturaInserir.Veiculo.Codigo > 0)
                        fatura.Veiculo = repVeiculo.BuscarPorCodigo(objValorfaturaInserir.Veiculo.Codigo);

                if (objValorfaturaInserir.CentroDeResultado != null)
                    if (objValorfaturaInserir.CentroDeResultado.Codigo > 0)
                        fatura.CentroResultado = repCentroResultado.BuscarPorCodigo(objValorfaturaInserir.CentroDeResultado.Codigo);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    if (objValorfaturaInserir.TipoOperacao == null && !configuracaoFinanceiro.NaoObrigarTipoOperacaoFatura)
                    {
                        //regra para Danone, rever, onde se o CNPJ informado for danone não deve obrigar tipo de operação, se não for deve informar o tipo de operação.
                        Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCNPJ(fatura.Cliente?.CPF_CNPJ_SemFormato ?? "");
                        if (fatura.GrupoPessoas != null || (fatura.GrupoPessoas == null && filial == null))
                        {
                            msgErro = "É obrigatório informar o tipo de operação.";
                            return false;
                        }
                    }
                }

                if (fatura.GrupoPessoas == null && fatura.Cliente == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    msgErro = "Não foi selecionado nenhum grupo de pessoa ou cliente para a geração da fatura.";
                    return false;
                }

                if (fatura.Codigo > 0)
                {
                    repFatura.LimparCargasCtePorFatura(fatura.Codigo);
                    repFatura.Atualizar(fatura, Auditado);
                }
                else
                {
                    fatura.ImprimeObservacaoFatura = false;
                    fatura.Total = 0;
                    fatura.Numero = 0;
                    fatura.DataFatura = objValorfaturaInserir.DataFatura;
                    fatura.DataCancelamentoFatura = null;
                    repFatura.Inserir(fatura, Auditado);
                }

                if (fatura.TiposOSConvertidos == null)
                    fatura.TiposOSConvertidos = new List<TipoOSConvertido>();

                if (objValorfaturaInserir.TipoOSConvertido != null && objValorfaturaInserir.TipoOSConvertido.Count > 0)
                    foreach (var tipoOS in objValorfaturaInserir.TipoOSConvertido)
                        fatura.TiposOSConvertidos.Add(tipoOS);

                string tipoPropostaMultimodalAnterior = "";
                if (fatura.TipoPropostaMultimodal == null)
                    fatura.TipoPropostaMultimodal = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();
                else
                {
                    tipoPropostaMultimodalAnterior = string.Join(", ", fatura.TipoPropostaMultimodal);
                    fatura.TipoPropostaMultimodal.Clear();
                }

                if (objValorfaturaInserir.TipoPropostaMultimodal != null)
                {
                    foreach (var tipo in objValorfaturaInserir.TipoPropostaMultimodal)
                    {
                        fatura.TipoPropostaMultimodal.Add(tipo);
                    }
                }

                repFatura.Atualizar(fatura);

                string tipoPropostaMultimodalAtual = string.Join(", ", fatura.TipoPropostaMultimodal);
                if (!tipoPropostaMultimodalAnterior.Equals(tipoPropostaMultimodalAtual))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Alterou Tipo da Proposta Multimodal de '" + tipoPropostaMultimodalAnterior + "' para '" + tipoPropostaMultimodalAtual + "'.", _unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Gerou a Fatura.", _unitOfWork);

                this.InserirLog(fatura, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.IniciouFatura, Usuario);
                objValorfaturaInserir.Codigo = fatura.Codigo;

                #endregion

                if (objValorfaturaInserir.CodigosDocumentos != null)
                {
                    _unitOfWork.FlushAndClear();
                    fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

                    #region AdicionarDocumentosFatura
                    foreach (int codigoDocumento in objValorfaturaInserir.CodigosDocumentos)
                    {
                        if (!Servicos.Embarcador.Fatura.Fatura.AdicionarDocumentoNaFatura(out msgErro, ref fatura, codigoDocumento, 0m, _unitOfWork, Auditado))
                        {
                            return false;
                        }
                    }
                    Servicos.Embarcador.Fatura.Fatura.AtualizarTotaisFatura(ref fatura, base._unitOfWork);
                    #endregion

                    _unitOfWork.FlushAndClear();
                    fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

                    #region ConfirmarDocumentos
                    if (!this.ConfirmarDocumentos(out msgErro, fatura, ConfiguracaoEmbarcador, Usuario, Auditado, objValorfaturaInserir))
                        return false;
                    #endregion

                    _unitOfWork.FlushAndClear();
                    fatura = repFatura.BuscarPorCodigo(fatura.Codigo);

                    #region ValidarFechamentoFatura
                    if (!this.ValidarFechamentoFatura(out msgErro, fatura, ConfiguracaoEmbarcador))
                        return false;
                    #endregion

                    #region FecharFatura
                    if (!this.FecharFatura(objValorfaturaInserir, fatura, out msgErro, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario))
                        return false;
                    #endregion
                }

                objValorfaturaInserir.Numero = fatura.Numero;
                objValorfaturaInserir.Situacao = fatura.Situacao;

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                msgErro = "Ocorreu uma falha ao adicionar.";
                return false;
            }
        }
        public bool FecharFatura(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir objValorFecharFatura, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, out string msgErro, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Usuario Usuario)
        {
            msgErro = "";
            try
            {
                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/Fatura");
                //if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Fatura_PermiteFecharFatura)))
                //    return new JsonpResult(false, "Seu usuário não possui permissão para fechar uma fatura.");

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(_unitOfWork);
                Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(_unitOfWork);

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(_unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(_unitOfWork);

                //   int codigo = 0;
                //   int.TryParse(Request.Params["Codigo"], out codigo);
                //   double tomadorFatura;
                //   int empresaFatura, banco = 0;
                //   double.TryParse(Request.Params["TomadorFatura"], out tomadorFatura);
                //   int.TryParse(Request.Params["EmpresaFatura"], out empresaFatura);
                //   int.TryParse(Request.Params["Banco"], out banco);
                //
                //   string agencia = Request.Params["Agencia"];
                //   string digito = Request.Params["Digito"];
                //   string numeroConta = Request.Params["NumeroConta"];
                //
                //   string observacaoFatura = Request.Params["ObservacaoFatura"];
                //   bool imprimirObservacaoFatura = false;
                //   bool.TryParse(Request.Params["ImprimirObservacaoFatura"], out imprimirObservacaoFatura);
                //
                //   Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco tipoConta;
                //   Enum.TryParse(Request.Params["TipoConta"], out tipoConta);


                if (fatura == null)
                    fatura = repFatura.BuscarPorCodigo(objValorFecharFatura.Codigo, true);

                if (fatura == null)
                {
                    msgErro = "Fatura não encontrada.";
                    return false;
                }

                if (fatura.NovoModelo)
                {
                    if (!ConfiguracaoEmbarcador.NaoValidarDataCancelamentoTituloNoFechamentoDaFatura)
                    {
                        DateTime maiorDataEmissaoDocumentos = repFaturaDocumento.ObterMaiorDataEmissaoPorFatura(fatura.Codigo);

                        if (fatura.DataFatura.Date < maiorDataEmissaoDocumentos.Date)
                        {
                            msgErro = "A data da fatura não pode ser menor que a maior data de emissão dos documentos (" + maiorDataEmissaoDocumentos.ToString("dd/MM/yyyy") + ").";
                            return false;
                        }
                        if (fatura.Parcelas != null && fatura.Parcelas.Any(o => o.DataEmissao.Date < maiorDataEmissaoDocumentos.Date))
                        {
                            msgErro = "A data de emissão das parcelas não pode ser menor que a maior data de emissão dos documentos (" + maiorDataEmissaoDocumentos.ToString("dd/MM/yyyy") + ").";
                            return false;
                        }
                    }
                }

                // So faz movimento financeiro quando o sistema for multi tms
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();

                    if (configuracaoFinanceiraFatura == null || !configuracaoFinanceiraFatura.GerarMovimentoAutomatico || configuracaoFinanceiraFatura.TipoMovimentoReversao == null || configuracaoFinanceiraFatura.TipoMovimentoUso == null)
                    {
                        msgErro = "Não existe configuração para a geração de movimentos da fatura.";
                        return false;
                    }

                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repFaturaDocumento.BuscarPrimeiroDocumentoFaturamento(fatura.Codigo);

                    fatura.TipoMovimentoUso = servicoTitulo.ObterTipoMovimentoConfiguracaoFinanceiraFatura(documentoFaturamento.CTe, documentoFaturamento.Carga, configuracaoFinanceiraFatura);
                    fatura.TipoMovimentoReversao = servicoTitulo.ObterTipoMovimentoConfiguracaoFinanceiraFatura(documentoFaturamento.CTe, documentoFaturamento.Carga, configuracaoFinanceiraFatura, true);
                }

                if (fatura == null)
                {
                    msgErro = "Fatura não encontrada.";
                    return false;
                }

                if (fatura.Situacao != SituacaoFatura.EmAntamento)
                {
                    msgErro = "A situação atual da fatura não permite o fechamento da mesma.";
                    return false;
                }


                if (fatura.Parcelas == null || fatura.Parcelas.Count <= 0)
                {
                    msgErro = "É necessário gerar as parcelas da fatura antes de fechar a mesma.";
                    return false;
                }


                if (fatura.FaturaRecebidaDeIntegracao)
                {
                    msgErro = "Não é possível realizar a operação para uma fatura recebida pela integração.";
                    return false;
                }


                if (ConfiguracaoEmbarcador.BloquearBaixaParcialOuParcelamentoFatura)
                {
                    if (fatura.Parcelas.Count > 1)
                    {
                        msgErro = "Não é permitido o parcelamento de faturas (mais que uma parcela).";
                        return false;
                    }


                    if (repFaturaDocumento.PossuiFaturamentoParcialDocumento(fatura.Codigo))
                    {
                        msgErro = "Não é permitido o faturamento parcial de documentos.";
                        return false;
                    }

                }

                decimal valorTotal = Math.Round(fatura.Total - fatura.Descontos + fatura.Acrescimos, 2, MidpointRounding.ToEven);
                decimal valorParcelas = fatura.Parcelas?.Sum(o => o.Valor) ?? 0m;
                decimal valorDiferenca = Math.Round(valorTotal - valorParcelas, 2, MidpointRounding.ToEven);

                if (valorDiferenca != 0m)
                {
                    msgErro = $"O valor total das parcelas não está de acordo com o valor total da fatura. Valor da fatura R$ {valorTotal:n2} / Valor das parcelas R$ {valorParcelas:n2}.";
                    return false;
                }


                if (fatura.MoedaCotacaoBancoCentral.HasValue && fatura.MoedaCotacaoBancoCentral.Value != MoedaCotacaoBancoCentral.Real)
                {
                    decimal valorMoedaTotal = fatura.TotalMoedaEstrangeira - fatura.DescontosMoeda + fatura.AcrescimosMoeada;
                    decimal valorMoedaParcelas = fatura.Parcelas?.Sum(o => o.ValorTotalMoeda) ?? 0m;
                    decimal valorDiferencaMoeda = valorMoedaTotal - valorMoedaParcelas;

                    if (valorDiferencaMoeda > 0m)
                    {
                        msgErro = $"O valor total em moeda estrangeira das parcelas não está de acordo com o valor total em moeda estrangeira da fatura. Valor em moeda da fatura {valorMoedaTotal:n2} / Valor em moeda das parcelas {valorMoedaParcelas:n2}.";
                        return false;
                    }
                }

                if (repTitulo.ContemTitulosPagosFatura(fatura.Codigo))
                {
                    msgErro = "Esta fatura já possui títulos quitados, não sendo possível fechar a mesma.";
                    return false;
                }

                string retornoConhotos = servFatura.ValidarCanhotosCTes(fatura, TipoServicoMultisoftware, _unitOfWork);
                if (string.IsNullOrWhiteSpace(retornoConhotos))
                    retornoConhotos = servFatura.ValidarOcorrenciaFinalizadoraCTes(fatura, TipoServicoMultisoftware, _unitOfWork);

                if (!string.IsNullOrWhiteSpace(retornoConhotos))
                {
                    msgErro = retornoConhotos;
                    return false;
                }

                msgErro = string.Empty;

                if (fatura.Usuario == null)
                    fatura.Usuario = Usuario;

                if (fatura.NovoModelo)
                    fatura.Situacao = SituacaoFatura.EmFechamento;
                else
                    fatura.Situacao = SituacaoFatura.Fechado;

                fatura.DataFechamento = DateTime.Now;
                fatura.Etapa = EtapaFatura.Fechamento;

                if (Double.TryParse((objValorFecharFatura?.ClienteTomadorFatura?.CPFCNPJ ?? ""), out double cpfCnpjTomador))
                    fatura.ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

                //if ( tomadorFatura > 0D)
                //    fatura.ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ(tomadorFatura);
                //else
                //    fatura.ClienteTomadorFatura = null;

                if (objValorFecharFatura.Empresa != null)
                {
                    if (objValorFecharFatura.Empresa.Codigo > 0)
                        fatura.Empresa = new Dominio.Entidades.Empresa { Codigo = objValorFecharFatura.Empresa.Codigo };
                }

                //if (empresaFatura > 0)
                //    fatura.Empresa = repEmpresa.BuscarPorCodigo(empresaFatura);
                //else
                //    fatura.Empresa = null;

                if (objValorFecharFatura.CodigoBanco > 0)
                    fatura.Banco = repBanco.BuscarPorCodigo(objValorFecharFatura.CodigoBanco);
                else
                    fatura.Banco = null;
                //if (banco > 0)
                //    fatura.Banco = repBanco.BuscarPorCodigo(banco);
                //else
                //    fatura.Banco = null;

                fatura.Agencia = objValorFecharFatura.Agencia;
                fatura.DigitoAgencia = objValorFecharFatura.DigitoAgencia;
                fatura.NumeroConta = objValorFecharFatura.NumeroConta;
                fatura.TipoContaBanco = objValorFecharFatura.TipoContaBanco;
                fatura.ObservacaoFatura = objValorFecharFatura.ObservacaoFatura;
                fatura.ImprimeObservacaoFatura = objValorFecharFatura.ImprimeObservacaoFatura;

                if (fatura.Numero == 0)
                {
                    if (ConfiguracaoEmbarcador.GerarNumeracaoFaturaAnual)
                    {
                        int anoAtual = DateTime.Now.Year;
                        fatura.Numero = repFatura.UltimoNumeracao(anoAtual) + 1;
                        anoAtual = (anoAtual % 100);
                        if (fatura.Numero == 0 || (fatura.Numero < ((anoAtual * 1000000) + 1)))
                            fatura.Numero = (anoAtual * 1000000) + 1;
                    }
                    else
                        fatura.Numero = repFatura.UltimoNumeracao() + 1;

                    fatura.ControleNumeracao = null;
                }

                repFatura.Atualizar(fatura, Auditado);

                servFatura.EtapaAprovacao(fatura, TipoServicoMultisoftware, _unitOfWork);
                if (!fatura.Situacao.Equals(SituacaoFatura.AguardandoAprovacao))
                {
                    servFatura.GerarIntegracoesFatura(fatura, _unitOfWork, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador);
                    servFatura.SalvarTituloVencimentoDocumentoFaturamento(fatura, _unitOfWork);
                    serCargaDadosSumarizados.AtualizarDadosCTesFaturados(fatura.Codigo, _unitOfWork);
                }

                servFatura.InserirLog(fatura, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.FechouFatura, Usuario);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fatura, null, "Fechou a fatura", _unitOfWork);
                objValorFecharFatura.Codigo = fatura.Codigo;
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                msgErro = "Ocorreu uma falha ao fechar fatura.";
                return false;

            }
        }
        public bool ValidarFechamentoFatura(out string msgErro, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador)
        {
            msgErro = "";
            try
            {
                if (!ConfiguracaoEmbarcador.NaoValidarDataCancelamentoTituloNoFechamentoDaFatura)
                {
                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
                    IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloCancelamento> titulos = repFatura.ObterTitulosComDataCancelamentoSuperiorAFaturaPorCTe(fatura.Codigo);

                    if (!titulos.Any())
                        titulos = repFatura.ObterTitulosComDataCancelamentoSuperiorAFaturaPorCarga(fatura.Codigo);

                    if (titulos.Count > 0)
                    {
                        DateTime maiorDataCancelamento = titulos.Max(o => o.DataCancelamento.Value);
                        msgErro = "Existem documentos com títulos cancelados. A data da fatura (" + fatura.DataFatura.ToString("dd/MM/yyyy") + ") não pode ser menor que a data de cancelamento dos títulos (" + maiorDataCancelamento.ToString("dd/MM/yyyy") + "). Títulos: " + string.Join(", ", titulos.Select(o => o.Codigo)) + ".";
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                msgErro = "Ocorreu uma falha ao validar o fechamento da fatura.";
                return false;
            }
        }
        public bool ConfirmarDocumentos(out string msgErro, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaInserir objValorfaturaInserir)
        {
            msgErro = "";
            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
                Repositorio.Embarcador.Moedas.Cotacao repCotacao = new(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();


                if (fatura.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas)
                {
                    msgErro = "A fatura ainda está em processo de lançamento de documentos, não sendo possível confirmar os documentos.";
                    return false;
                }

                if (fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                {
                    msgErro = "Os documentos não podem ser confirmados na situação atual da fatura.";
                    return false;
                }

                if (repFaturaDocumento.ContarPorFatura(fatura.Codigo) <= 0)
                {
                    msgErro = "Não existem documentos vinculados à fatura, não sendo possível avançar.";
                    return false;
                }

                fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.Fechamento;
                fatura.ImprimeObservacaoFatura = false;

                if (fatura.Numero == 0)
                {
                    if (configuracaoEmbarcador.GerarNumeracaoFaturaAnual)
                    {
                        int anoAtual = DateTime.Now.Year;
                        fatura.Numero = repFatura.UltimoNumeracao(anoAtual) + 1;
                        anoAtual = (anoAtual % 100);
                        if (fatura.Numero == 0 || (fatura.Numero < ((anoAtual * 1000000) + 1)))
                            fatura.Numero = (anoAtual * 1000000) + 1;
                    }
                    else
                        fatura.Numero = repFatura.UltimoNumeracao() + 1;
                    fatura.ControleNumeracao = null;
                }

                if (fatura.Empresa == null)
                    fatura.Empresa = repFaturaDocumento.ObterPrimeiraEmpresaEmissora(fatura.Codigo);

                if (configuracaoEmbarcador.UtilizarDadosBancariosDaEmpresa && fatura.Empresa != null && fatura.Empresa.Banco != null)
                {
                    fatura.Banco = fatura.Empresa.Banco;
                    fatura.Agencia = fatura.Empresa.Agencia;
                    fatura.DigitoAgencia = fatura.Empresa.DigitoAgencia;
                    fatura.NumeroConta = fatura.Empresa.NumeroConta;
                    fatura.TipoContaBanco = fatura.Empresa.TipoContaBanco;
                }

                if (fatura.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
                {
                    if (fatura.Banco == null)
                    {
                        fatura.Banco = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.Banco;
                        fatura.Agencia = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.Agencia;
                        fatura.DigitoAgencia = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.DigitoAgencia;
                        fatura.NumeroConta = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.NumeroConta;
                        fatura.TipoContaBanco = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.TipoContaBanco;
                    }
                    fatura.ClienteTomadorFatura = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ClienteTomadorFatura;
                    fatura.ObservacaoFatura = fatura.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ObservacaoFatura;
                }
                else if (fatura.Cliente != null && !fatura.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                {
                    if (fatura.Cliente.GrupoPessoas != null)
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = fatura.Cliente.GrupoPessoas.Banco;
                            fatura.Agencia = fatura.Cliente.GrupoPessoas.Agencia;
                            fatura.DigitoAgencia = fatura.Cliente.GrupoPessoas.DigitoAgencia;
                            fatura.NumeroConta = fatura.Cliente.GrupoPessoas.NumeroConta;
                            fatura.TipoContaBanco = fatura.Cliente.GrupoPessoas.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = fatura.Cliente.GrupoPessoas.ClienteTomadorFatura;
                        fatura.ObservacaoFatura = fatura.Cliente.GrupoPessoas.ObservacaoFatura;
                    }
                    else
                    {
                        if (fatura.Banco == null)
                        {
                            fatura.Banco = fatura.Cliente.Banco;
                            fatura.Agencia = fatura.Cliente.Agencia;
                            fatura.DigitoAgencia = fatura.Cliente.DigitoAgencia;
                            fatura.NumeroConta = fatura.Cliente.NumeroConta;
                            fatura.TipoContaBanco = fatura.Cliente.TipoContaBanco;
                        }
                        fatura.ClienteTomadorFatura = fatura.Cliente.ClienteTomadorFatura;
                        fatura.ObservacaoFatura = fatura.Cliente.ObservacaoFatura;
                    }
                }
                else if (fatura.Cliente != null)
                {
                    if (fatura.Banco == null)
                    {
                        fatura.Banco = fatura.Cliente.Banco;
                        fatura.Agencia = fatura.Cliente.Agencia;
                        fatura.DigitoAgencia = fatura.Cliente.DigitoAgencia;
                        fatura.NumeroConta = fatura.Cliente.NumeroConta;
                        fatura.TipoContaBanco = fatura.Cliente.TipoContaBanco;
                    }
                    fatura.ClienteTomadorFatura = fatura.Cliente.ClienteTomadorFatura;
                    fatura.ObservacaoFatura = fatura.Cliente.ObservacaoFatura;
                }
                else if (fatura.GrupoPessoas != null)
                {
                    if (fatura.Banco == null)
                    {
                        fatura.Banco = fatura.GrupoPessoas.Banco;
                        fatura.Agencia = fatura.GrupoPessoas.Agencia;
                        fatura.DigitoAgencia = fatura.GrupoPessoas.DigitoAgencia;
                        fatura.NumeroConta = fatura.GrupoPessoas.NumeroConta;
                        fatura.TipoContaBanco = fatura.GrupoPessoas.TipoContaBanco;
                    }
                    fatura.ClienteTomadorFatura = fatura.GrupoPessoas.ClienteTomadorFatura;
                    fatura.ObservacaoFatura = fatura.GrupoPessoas.ObservacaoFatura;
                }

                if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                    fatura.ImprimeObservacaoFatura = true;

                if (fatura.ClienteTomadorFatura == null)
                    fatura.ClienteTomadorFatura = repFaturaDocumento.ObterPrimeiroTomador(fatura.Codigo);

                if (configuracaoFinanceiro.PermitirConfirmarDocumentosFaturaApenasComCtesEscriturados == true)
                {
                    if ((fatura.Documentos.Select(o => o.Documento?.CTe?.CodigoEscrituracao)).Any(b => string.IsNullOrEmpty(b)))
                    {
                        msgErro = "Não foi possível confirmar os documentos. Existem documentos sem escrituração.";
                        return false;
                    }
                }

                if (fatura.DataBaseCRT.HasValue)
                {
                    List<Dominio.Entidades.Embarcador.Moedas.Cotacao> cotacoes = repCotacao.ListarCotacoes(fatura.DataBaseCRT.Value);
                    if (cotacoes.Count > 1)
                    {
                        msgErro = $"Exite mais de uma Cotação para a Data Base CRT {fatura.DataBaseCRT:dd/MM/yyyy HH:mm}.";
                        return false;
                    }

                    Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = cotacoes.FirstOrDefault();
                    if (cotacao == null)
                    {
                        msgErro = $"Cotação não encontrada para Data Base CRT {fatura.DataBaseCRT:dd/MM/yyyy HH:mm}.";
                        return false;
                    }

                    if (cotacao.CotacaoAutomaticaViaWS && cotacao.UtilizarCotacaoRetroativa)
                    {
                        Servicos.Embarcador.Moedas.Cotacao servicoCotacao = new Servicos.Embarcador.Moedas.Cotacao(_unitOfWork);
                        cotacao.ValorMoeda = servicoCotacao.ObterValoMoedaDiaria(cotacao.MoedaCotacaoBancoCentral, fatura.DataBaseCRT.Value, _unitOfWork);
                    }

                    bool manterValorMoeda = configuracaoFinanceiro.ManterValorMoedaConfirmarDocumentosFatura;

                    IEnumerable<int> codigosCarga = fatura.Documentos
                        .Where(faturaDocumento => faturaDocumento.Documento.CTe != null)
                        .SelectMany(faturaDocumento => faturaDocumento.Documento.CTe.CargaCTes.Select(cargaCte => cargaCte.Carga.Codigo));

                    List<int> codigosDocumentos = fatura.Documentos?.Select(o => o.Codigo).ToList() ?? new List<int>();

                    foreach (int codigoCarga in codigosCarga)
                    {
                        new Carga.Moeda(_unitOfWork, auditado).AlterarMoedaCarga(out string erro, codigoCarga, cotacao.MoedaCotacaoBancoCentral, cotacao.ValorMoeda, fatura, codigosDocumentos, manterValorMoeda);

                        if (!string.IsNullOrWhiteSpace(erro))
                        {
                            msgErro = $"Erro ao alterar moeda da Carga código {codigoCarga}: {erro}";
                            return false;
                        }
                    }

                    IEnumerable<int> codigosCargaOcorrencia = fatura.Documentos
                        .Where(faturaDocumento => faturaDocumento.Documento.CargaOcorrenciaPagamento != null)
                        .Select(faturaDocumento => faturaDocumento.Documento.CargaOcorrenciaPagamento.Codigo);

                    foreach (int codigoCargaOcorrencia in codigosCargaOcorrencia)
                    {
                        new Carga.Moeda(_unitOfWork, auditado).AlterarMoedaOcorrencia(out string erro, codigoCargaOcorrencia, cotacao.MoedaCotacaoBancoCentral, cotacao.ValorMoeda, fatura.Codigo, manterValorMoeda);
                        if (!string.IsNullOrWhiteSpace(erro))
                        {
                            msgErro = $"Erro ao alterar moeda da Carga Ocorrencia código {codigoCargaOcorrencia}: {erro}";
                            return false;
                        }
                    }

                    fatura.ValorMoedaCotacao = cotacao.ValorMoeda;

                    AtualizarTotaisFatura(ref fatura, _unitOfWork);
                }


                foreach (var documento in fatura.Documentos)
                {
                    // TODO testar alguma fatura com um TipoDocumento != 1 ou != CTe
                    if (documento.Documento?.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFaturamento.CTe)
                        continue;

                    var remarkSped = $"Receita de {documento?.Documento?.TipoDocumento} {documento?.Descricao.Split('-')[0].Trim()}";
                }

                repFatura.Atualizar(fatura, auditado);

                this.InserirLog(fatura, _unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.SalvouCargas, usuario);
                List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoParcela> Parcelas = objValorfaturaInserir?.Parcelas ?? null;
                this.LancarParcelaFatura(fatura, _unitOfWork, configuracaoEmbarcador.UtilizaEmissaoMultimodal, Parcelas);

                repFatura.Atualizar(fatura);

                return true;
            }
            catch (Exception ex)
            {
                msgErro = "Ocorreu uma falha ao confirmar os documentos da fatura.";
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        public string ObterCaminhoPDF(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Fatura.FaturaImpressao servicoFaturaImpressao = Servicos.Embarcador.Fatura.FaturaImpressaoFactory.Criar(fatura, unitOfWork, tipoServicoMultisoftware);
            string guidArquivoUltimoRelatorioGerado = servicoFaturaImpressao.ObterGuidArquivoUltimoRelatorioGerado(fatura);

            if (string.IsNullOrWhiteSpace(guidArquivoUltimoRelatorioGerado))
            {
                guidArquivoUltimoRelatorioGerado = Guid.NewGuid().ToString().Replace("-", "");
            }

            return Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), guidArquivoUltimoRelatorioGerado) + ".pdf";
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: fatura.Codigo,
                URLPagina: "Fatura/Fatura",
                titulo: Localization.Resources.Configuracao.Fatura.TituloFatura,
                nota: string.Format(Localization.Resources.Configuracao.Fatura.CriadaSolicitacaoAprovacaoFatura, fatura.Numero),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Privados

        private bool GerarParcelaFaturaDeAdiantamentoSaldo(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unidadeDeTrabalho, bool faturamentoMultimodal, List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracaoParcela> objValorParcelas = null, DateTime? dataAprovacaoFatura = null)
        {
            Repositorio.Embarcador.Fatura.FaturaParcela repFaturaParcela = new Repositorio.Embarcador.Fatura.FaturaParcela(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeDeTrabalho);

            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unidadeDeTrabalho);
            Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Financeiro.Titulo(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            DateTime dataUltimaParcela = DateTime.MinValue;

            if (!servicoTitulo.PossuiGeracaoTituloAdiantamentoSaldo(fatura.ClienteTomadorFatura, fatura.ClienteTomadorFatura?.GrupoPessoas))
                return false;

            if (objValorParcelas != null && objValorParcelas.Count > 0)
            {
                foreach (var objParcela in objValorParcelas)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = new Dominio.Entidades.Embarcador.Fatura.FaturaParcela
                    {
                        Acrescimo = objParcela.Acrescimo,
                        CodigoTitulo = objParcela.CodigoTitulo,
                        DataEmissao = objParcela.DataEmissao,
                        DataVencimento = objParcela.DataVencimento,
                        Desconto = objParcela.Desconto,
                        FormaTitulo = objParcela.FormaTitulo,
                        Sequencia = objParcela.Sequencia,
                        Valor = objParcela.Valor,
                        ValorTotalMoeda = objParcela.ValorTotalMoeda,
                        VencimentoTitulo = objParcela.VencimentoTitulo,
                        NossoNumeroTitulo = objParcela.NossoNumeroBoleto
                    };
                    repFaturaParcela.Inserir(parcela);
                }
            }
            else
            {
                List<(int Dia, decimal Percentual)> diasPercentualPrazoVencimento = servicoTitulo.ObterDiasPrazoVencimentoAdiantamentoSaldo(fatura.ClienteTomadorFatura, null);

                int qtdParcelas = diasPercentualPrazoVencimento.Count;

                decimal valorTotal = Math.Round(fatura.Total - fatura.Descontos + fatura.Acrescimos, 2, MidpointRounding.ToEven);
                decimal valorMoedaTotal = fatura.TotalMoedaEstrangeira - fatura.DescontosMoeda + fatura.AcrescimosMoeada;

                int posParcela = 0;
                decimal valorAcumulado = 0m, valorMoedaAcumulado = 0m;
                FormaTitulo formaTitulo = FormaTitulo.Outros;

                bool existeConfiguracaoFaturaTransportador = false;
                int? diaMes = null;
                DiaSemana? diaSemana = null;
                bool? permiteFinalSemana = null;
                int? diasPrazoFatura = null;
                List<DiaSemana> diasSemana = new List<DiaSemana>();
                List<int> diasMes = new List<int>();
                TipoPrazoFaturamento? tipoPrazoFatura = null;
                TipoPrazoPagamento? tipoPrazoPagamento = null;

                foreach ((int Dia, decimal Percentual) diaPercentual in diasPercentualPrazoVencimento)
                {
                    posParcela++;

                    decimal valorParcelas = Math.Round((diaPercentual.Percentual / 100) * valorTotal, 2);
                    decimal valorMoedaParcelas = Math.Round((diaPercentual.Percentual / 100) * valorMoedaTotal, 2);

                    valorAcumulado += valorParcelas;
                    valorMoedaAcumulado += valorMoedaParcelas;

                    if (posParcela == qtdParcelas)
                    {
                        if (valorAcumulado != valorTotal)
                            valorParcelas += valorTotal - valorAcumulado;

                        if (valorMoedaAcumulado != valorMoedaTotal)
                            valorMoedaParcelas += valorMoedaTotal - valorMoedaAcumulado;
                    }

                    if (configuracaoFinanceiro.UtilizarConfiguracoesTransportadorParaFatura)
                    {
                        RetornarParametrosFaturamentoTransportador(fatura,
                                                                   unidadeDeTrabalho,
                                                                   faturamentoMultimodal,
                                                                   out diaMes,
                                                                   out diaSemana,
                                                                   out permiteFinalSemana,
                                                                   out diasPrazoFatura,
                                                                   out diasSemana,
                                                                   out diasMes,
                                                                   out tipoPrazoPagamento,
                                                                   diaPercentual.Dia,
                                                                   out formaTitulo,
                                                                   out existeConfiguracaoFaturaTransportador);
                    }

                    if (!existeConfiguracaoFaturaTransportador)
                    {
                        RetornarParametrosFaturamento(fatura,
                                                      unidadeDeTrabalho,
                                                      faturamentoMultimodal,
                                                      out diaMes,
                                                      out diaSemana,
                                                      out permiteFinalSemana,
                                                      out diasPrazoFatura,
                                                      out diasSemana,
                                                      out diasMes,
                                                      out tipoPrazoFatura,
                                                      diaPercentual.Dia,
                                                      out formaTitulo);
                    }

                    DateTime? dataBaseParcela;
                    if (!existeConfiguracaoFaturaTransportador)
                    {
                        dataBaseParcela = dataAprovacaoFatura != null ? dataAprovacaoFatura : RetornarDataBase(tipoPrazoFatura, fatura, faturamentoMultimodal, unidadeDeTrabalho);
                    }
                    else
                    {
                        dataBaseParcela = dataAprovacaoFatura != null ? dataAprovacaoFatura : RetornarDataBaseTransportador(tipoPrazoPagamento, fatura, faturamentoMultimodal, unidadeDeTrabalho);
                    }

                    if (diasPrazoFatura == null)
                        diasPrazoFatura = 0;

                    if (diaMes != null || diaSemana != null || diasPrazoFatura != null || tipoPrazoFatura != null || diasMes.Count > 0 || diasSemana.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Fatura.FaturaParcela parcela = new Dominio.Entidades.Embarcador.Fatura.FaturaParcela
                        {
                            DataEmissao = fatura.DataFatura,
                            DataVencimento = RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes, fatura.ClienteTomadorFatura, fatura.GrupoPessoas, existeConfiguracaoFaturaTransportador, unidadeDeTrabalho),
                            Desconto = 0,
                            Fatura = fatura,
                            Sequencia = posParcela,
                            SituacaoFaturaParcela = SituacaoFaturaParcela.EmAberto,
                            Valor = valorParcelas,
                            ValorTotalMoeda = valorMoedaParcelas,
                            FormaTitulo = formaTitulo
                        };

                        if (existeConfiguracaoFaturaTransportador && faturamentoMultimodal && parcela.DataVencimento.Date <= DateTime.Now.Date && dataBaseParcela.HasValue && tipoPrazoPagamento.HasValue && tipoPrazoPagamento.Value != TipoPrazoPagamento.DataPagamento)
                        {
                            dataBaseParcela = RetornarDataBase((TipoPrazoFaturamento?)TipoPrazoPagamento.DataPagamento, fatura, faturamentoMultimodal, unidadeDeTrabalho);
                            parcela.DataVencimento = RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes, fatura.ClienteTomadorFatura, fatura.GrupoPessoas, existeConfiguracaoFaturaTransportador, unidadeDeTrabalho);
                        }
                        else if (faturamentoMultimodal && parcela.DataVencimento.Date <= DateTime.Now.Date && dataBaseParcela.HasValue && tipoPrazoFatura.HasValue && tipoPrazoFatura.Value != TipoPrazoFaturamento.DataFatura)
                        {
                            dataBaseParcela = RetornarDataBase(TipoPrazoFaturamento.DataFatura, fatura, faturamentoMultimodal, unidadeDeTrabalho);
                            parcela.DataVencimento = RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes, fatura.ClienteTomadorFatura, fatura.GrupoPessoas, existeConfiguracaoFaturaTransportador, unidadeDeTrabalho);
                        }

                        repFaturaParcela.Inserir(parcela);
                        dataUltimaParcela = parcela.DataVencimento;
                    }
                }

            }


            servFatura.AtualizarValorVencimento(dataUltimaParcela, fatura.Codigo, unidadeDeTrabalho);

            return true;
        }

        private static bool QuitarTituloSemValor(out string erro, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            erro = string.Empty;

            if (titulo.ValorPendente > 0m)
                return true;

            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa()
            {
                DataBaixa = fatura.DataFatura,
                DataBase = fatura.DataFatura,
                DataBaseCRT = fatura.DataBaseCRT,
                DataOperacao = DateTime.Now,
                Numero = 1,
                Observacao = (fatura.GrupoPessoas != null ? fatura.GrupoPessoas.Descricao : fatura.Cliente != null ? fatura.Cliente.Nome : "") + " - FATURA Nº " + fatura.Numero.ToString() + " (" + fatura.DataFatura.ToString("dd/MM/yyyy") + ")",
                SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada,
                Sequencia = 1,
                Valor = 0m,
                ValorTotalAPagar = 0m,
                TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber,
                TipoPagamentoRecebimento = null,
                Usuario = fatura.Usuario,
                Pessoa = fatura.ClienteTomadorFatura,
                GrupoPessoas = fatura.GrupoPessoas
            };

            repTituloBaixa.Inserir(tituloBaixa);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = Servicos.Embarcador.Financeiro.BaixaTituloReceber.AdicionarTituloABaixa(tituloBaixa, titulo.Codigo, unitOfWork, null, 0m, 0m, false);

            Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixaAgrupado(ref tituloBaixaAgrupado, unitOfWork);

            int countDocumentosBaixados = 0;
            if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.BaixarTitulo(out erro, tituloBaixa, tituloBaixaAgrupado, tituloBaixa.Observacao, null, unitOfWork, tipoServicoMultisoftware, null, false, 0, ref countDocumentosBaixados))
                return false;

            Servicos.Embarcador.Financeiro.BaixaTituloReceber.AtualizarTotaisTituloBaixa(ref tituloBaixa, unitOfWork, true);

            return true;
        }

        private void AdicionarEDI(Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Empresa empresaLayout, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, int reenviarAutomaticamenteOutraVezAposMinutos)
        {
            if (fatura.FaturaGeradaPelaIntegracao || fatura.FaturaRecebidaDeIntegracao)
                return;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR)
                return;

            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);

            if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR && fatura.NovoModelo)
            {
                GerarFaturasEDIPorImposto(layoutEDI, tipoIntegracao, fatura, empresaLayout, unitOfWork, Auditado, reenviarAutomaticamenteOutraVezAposMinutos);
            }
            else
            {
                Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = GerarFaturaEDI(layoutEDI, tipoIntegracao, fatura, empresaLayout, reenviarAutomaticamenteOutraVezAposMinutos);

                if (Auditado != null)
                    repFaturaIntegracao.Inserir(integracao, Auditado);
                else
                    repFaturaIntegracao.Inserir(integracao);

            }

        }

        private void GerarFaturasEDIPorImposto(Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Empresa empresaLayout, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, int reenviarAutomaticamenteOutraVezAposMinutos)
        {
            if (fatura.FaturaGeradaPelaIntegracao || fatura.FaturaRecebidaDeIntegracao)
                return;

            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);

            List<string> tipoConsultas = new List<string>
            {
                "Isenta",
                "ST",
                "Normal",
                "ISS",
                "ISSRetido"
            };

            for (int i = 0; i < tipoConsultas.Count; i++)
            {
                bool modeloCTe = true;
                bool usarCST = false;
                List<string> CSTs = new List<string>();

                if (tipoConsultas[i] == "Isenta")
                {
                    usarCST = true;
                    CSTs.Add("");
                    CSTs.Add("40");
                    CSTs.Add("51");
                }
                else if (tipoConsultas[i] == "ST")
                {
                    usarCST = true;
                    CSTs.Add("60");
                }
                else if (tipoConsultas[i] == "Normal")
                {
                    usarCST = false;
                    CSTs.Add("");
                    CSTs.Add("40");
                    CSTs.Add("51");
                    CSTs.Add("60");
                }
                else if (tipoConsultas[i] == "ISS")
                {
                    modeloCTe = false;
                }
                else
                    continue;

                if (repFaturaDocumento.PossuiDadosPorCodigosCTesCSTs(fatura.Codigo, CSTs, usarCST, modeloCTe))
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracao = GerarFaturaEDI(layoutEDI, tipoIntegracao, fatura, empresaLayout, reenviarAutomaticamenteOutraVezAposMinutos);

                    integracao.UsarCST = usarCST;
                    integracao.ModeloCTe = modeloCTe;
                    integracao.TipoImposto = tipoConsultas[i];
                    integracao.CSTs = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST>();
                    foreach (string cst in CSTs)
                        integracao.CSTs.Add(ObterOuGerarCSTIntegracao(cst, unitOfWork));

                    if (Auditado != null)
                        repFaturaIntegracao.Inserir(integracao, Auditado);
                    else
                        repFaturaIntegracao.Inserir(integracao);

                }
            }
        }

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST ObterOuGerarCSTIntegracao(string cst, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracaoCST repFaturaIntegracaoCST = new Repositorio.Embarcador.Fatura.FaturaIntegracaoCST(unitOfWork);

            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST cstIntegracao = repFaturaIntegracaoCST.BuscarPorCST(cst);

            if (cstIntegracao == null)
            {
                cstIntegracao = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST()
                {
                    CST = cst
                };
                repFaturaIntegracaoCST.Inserir(cstIntegracao);
            }

            return cstIntegracao;
        }

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao GerarFaturaEDI(Dominio.Entidades.LayoutEDI layoutEDI, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Empresa empresaLayout, int reenviarAutomaticamenteOutraVezAposMinutos)
        {
            return new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao
            {
                Fatura = fatura,
                LayoutEDI = layoutEDI,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                Tentativas = 0,
                TipoIntegracao = tipoIntegracao,
                Empresa = empresaLayout,
                TipoIntegracaoFatura = TipoIntegracaoFatura.EDI,
                MensagemRetorno = string.Empty,
                DataEnvio = DateTime.Now,
                ReenviarAutomaticamenteOutraVezAposMinutos = reenviarAutomaticamenteOutraVezAposMinutos
            };
        }

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, List<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorio = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        var aprovacao = new Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura()
                        {
                            OrigemAprovacao = fatura,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = fatura.DataFatura,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(fatura, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura()
                    {
                        OrigemAprovacao = fatura,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = fatura.DataFatura
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            fatura.Situacao = existeRegraSemAprovacao ? SituacaoFatura.AguardandoAprovacao : SituacaoFatura.EmFechamento;
        }

        private List<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura>(_unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaCargaDocumento repositorioFaturaCargaDocumento = new Repositorio.Embarcador.Fatura.FaturaCargaDocumento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura>();
            List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> listaFaturaCargaDocumentos = repositorioFaturaCargaDocumento.BuscarPorFatura(fatura?.Codigo ?? 0, StatusDocumentoFatura.Normal);

            decimal valorOrcado = fatura.Total;

            foreach (Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, fatura.Documentos.Select(o => o?.Documento?.CargaPagamento?.Filial?.Codigo ?? 0).ToList()))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, fatura.TipoOperacao?.Codigo))
                    continue;

                if (regra.RegraPorTomador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaTomador, Dominio.Entidades.Cliente>(regra.AlcadasTomador, fatura.ClienteTomadorFatura.CPF_CNPJ))
                    continue;

                if (regra.RegraPorValor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AlcadaValor, decimal>(regra.AlcadasValor, valorOrcado))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento ObterCondicaoPagamentoFiltrada(List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamento, DateTime dataDocumento, DateTime dataPagamento)
        {
            if (condicoesPagamento.Count() == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamentoFiltradas = condicoesPagamento;
            int diaDataDocumento = dataDocumento.Day;
            int diaDataPagamento = dataPagamento.Day;

            condicoesPagamentoFiltradas = (from condicao in condicoesPagamentoFiltradas where !condicao.CodigoTipoCarga.HasValue select condicao).ToList();
            condicoesPagamentoFiltradas = (from condicao in condicoesPagamentoFiltradas where !condicao.CodigoTipoOperacao.HasValue select condicao).ToList();

            condicoesPagamentoFiltradas = (
                from condicao in condicoesPagamentoFiltradas
                where (
                    !condicao.DiaEmissaoLimite.HasValue ||
                    ((condicao.TipoPrazoPagamento == TipoPrazoPagamento.DataDocumento) && (diaDataDocumento <= condicao.DiaEmissaoLimite.Value)) ||
                    ((condicao.TipoPrazoPagamento == TipoPrazoPagamento.DataPagamento) && (diaDataPagamento <= condicao.DiaEmissaoLimite.Value))
                )
                select condicao
            ).ToList();

            return condicoesPagamentoFiltradas.FirstOrDefault();
        }

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo AdicionarArquivoTransacaoFatura(string arquivoRequisicao, string arquivoRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(arquivoRequisicao) && string.IsNullOrWhiteSpace(arquivoRetorno))
                return null;

            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repositorioFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioFaturaIntegracaoArquivo.Inserir(faturaIntegracaoArquivo);

            return faturaIntegracaoArquivo;
        }

        private static (string assunto, string corpo) EmailTemplatePadrao()
        {
            string corpoEmail = "Prezado {cliente}, <br/>";
            corpoEmail += "<br/>";
            corpoEmail += "Anexo o faturamento{de}. <br/>";
            corpoEmail += "<br/>";
            corpoEmail += "Dúvidas e questionamentos, gentileza tratar através do e-mail: faturamento@alianca.com.br. <br/>";
            corpoEmail += "<br/>";
            corpoEmail += "Em caso de solicitações de prorrogação de vencimento e envios de comprovante de pagamento devem ser tratados diretamente com a equipe de cobrança, através do e-mail sao-cobranca-cab@alianca.com.br. <br/>";
            corpoEmail += "<br/>";
            corpoEmail += "Atenciosamente, <br/>";
            corpoEmail += "Aliança Navegação e Logística Ltda <br/>";
            corpoEmail += "<br/>";
            corpoEmail += "Este é um e-mail automático, favor não responder. <br/>";

            return (assunto: "Faturamento Aliança {assunto}", corpo: corpoEmail);
        }

        #endregion
    }
}
