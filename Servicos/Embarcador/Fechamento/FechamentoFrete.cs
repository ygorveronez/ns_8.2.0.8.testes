using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Fechamento
{
    public sealed class FechamentoFrete
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public FechamentoFrete(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null, configuracaoEmbarcador: null) { }

        public FechamentoFrete(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, configuracaoEmbarcador: null) { }

        public FechamentoFrete(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, auditado: null, configuracaoEmbarcador: configuracaoEmbarcador) { }

        public FechamentoFrete(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _auditado = auditado;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void AdicionarComplementosFechamentoPorMotoristas(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual, List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento, List<int> codigosCTeUtilizados)
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado repositorioMotoristaUtilizado = new Repositorio.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado> motoristasUtilizadosFechamento = repositorioMotoristaUtilizado.BuscarPorFechamento(fechamentoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado motoristaUtilizado in motoristasUtilizadosFechamento)
            {
                if (motoristaUtilizado.ValorTotal <= 0m)
                    continue;

                int quantidadeComplementosAdicionar = (int)motoristaUtilizado.Quantidade.RoundUp(0);

                for (int i = 0; i < quantidadeComplementosAdicionar; i++)
                {
                    bool ultimoComplementoAdicionar = i == (quantidadeComplementosAdicionar - 1);
                    decimal valorComplemeto = ultimoComplementoAdicionar ? motoristaUtilizado.Valor * (motoristaUtilizado.Quantidade - i) : motoristaUtilizado.Valor;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe primeiroCargaCTe = cargasCTe
                        .Where(cargaCTe =>
                            cargaCTe.CTe.TomadorPagador.Cliente.CPF_CNPJ == motoristaUtilizado.Tomador.CPF_CNPJ &&
                            !ObterCodigosCTeDesconsiderarParaComplemento(codigosCTeUtilizados).Contains(cargaCTe.CTe.Codigo)
                        )
                        .OrderByDescending(cargaCTe => string.IsNullOrWhiteSpace(cargaCTe.CTe.CST) || cargaCTe.CTe.CST == "40" || cargaCTe.CTe.CST == "51")
                        .FirstOrDefault();

                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual primeiroDocumentoParaEmissaoNFSManual = documentosParaEmissaoNFSManual
                        .Where(documento =>
                            documento.Tomador.CPF_CNPJ == motoristaUtilizado.Tomador.CPF_CNPJ
                        )
                        .FirstOrDefault();

                    if (primeiroCargaCTe != null)
                    {
                        codigosCTeUtilizados.Add(primeiroCargaCTe.CTe.Codigo);

                        complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                        {
                            CargaCTe = primeiroCargaCTe,
                            Observacao = "Motorista",
                            ValorComplemento = valorComplemeto
                        });
                    }
                    else if (primeiroDocumentoParaEmissaoNFSManual != null)
                        complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                        {
                            CargaDocumentoParaEmissaoNFSManual = primeiroDocumentoParaEmissaoNFSManual,
                            ValorComplemento = valorComplemeto
                        });
                }
            }
        }

        private void AdicionarComplementosFechamentoPorValoresOutrosRecursos(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual, List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento, List<int> codigosCTeUtilizados)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorCliente repositorioContratoCliente = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorCliente(_unitOfWork);
            Repositorio.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos repositorioValoresOutrosRecursos = new Repositorio.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> valoresOutrosRecursos = repositorioValoresOutrosRecursos.BuscarPorFechamento(fechamentoFrete.Codigo);
            List<double> cpfCnpjClientesContrato = repositorioContratoCliente.BuscarCpfCnpjClientesPorContrato(fechamentoFrete.Contrato.Codigo);

            if (cpfCnpjClientesContrato.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos valorOutroRecurso in valoresOutrosRecursos)
            {
                foreach (double cpfCnpjClienteContrato in cpfCnpjClientesContrato)
                {
                    if (valorOutroRecurso.Tomador?.CPF_CNPJ != cpfCnpjClienteContrato)
                        continue;

                    for (int i = 0; i < (int)valorOutroRecurso.Quantidade; i++)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe primeiroCargaCTeParaOutroRecurso = cargasCTe
                            .Where(cargaCTe => !ObterCodigosCTeDesconsiderarParaComplemento(codigosCTeUtilizados).Contains(cargaCTe.CTe.Codigo))
                            .OrderByDescending(cargaCTe => string.IsNullOrWhiteSpace(cargaCTe.CTe.CST) || cargaCTe.CTe.CST == "40" || cargaCTe.CTe.CST == "51")
                            .ThenByDescending(cargaCTe => cargaCTe.CTe.TomadorPagador.Cliente.CPF_CNPJ == cpfCnpjClienteContrato)
                            .FirstOrDefault();

                        Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual primeiroDocumentoParaEmissaoNFSManual = documentosParaEmissaoNFSManual.FirstOrDefault();

                        if (primeiroCargaCTeParaOutroRecurso != null)
                        {
                            complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento()
                            {
                                CargaCTe = primeiroCargaCTeParaOutroRecurso,
                                Observacao = valorOutroRecurso.ValoresOutrosRecursos.TipoMaoDeObra,
                                ValorComplemento = valorOutroRecurso.ValoresOutrosRecursos.PrecoAtual
                            });

                            codigosCTeUtilizados.Add(primeiroCargaCTeParaOutroRecurso.CTe.Codigo);
                        }
                        else if (primeiroDocumentoParaEmissaoNFSManual != null)
                            complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento()
                            {
                                CargaDocumentoParaEmissaoNFSManual = primeiroDocumentoParaEmissaoNFSManual,
                                ValorComplemento = valorOutroRecurso.ValoresOutrosRecursos.PrecoAtual
                            });
                    }
                }
            }
        }

        private void AdicionarComplementosFechamentoPorVeiculos(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual, List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento, List<int> codigosCTeUtilizados)
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado repositorioVeiculoUtilizado = new Repositorio.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado> veiculosUtilizadosFechamento = repositorioVeiculoUtilizado.BuscarPorFechamento(fechamentoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado veiculoUtilizado in veiculosUtilizadosFechamento)
            {
                if (veiculoUtilizado.ValorTotal <= 0m)
                    continue;

                int quantidadeComplementosAdicionar = (int)veiculoUtilizado.Quantidade.RoundUp(0);

                for (int i = 0; i < quantidadeComplementosAdicionar; i++)
                {
                    bool ultimoComplementoAdicionar = i == (quantidadeComplementosAdicionar - 1);
                    decimal valorComplemeto = ultimoComplementoAdicionar ? veiculoUtilizado.Valor * (veiculoUtilizado.Quantidade - i) : veiculoUtilizado.Valor;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe primeiroCargaCTe = cargasCTe
                        .Where(cargaCTe =>
                            cargaCTe.Carga.Veiculo.ModeloVeicularCarga.Codigo == veiculoUtilizado.ModeloVeicularCarga.Codigo &&
                            cargaCTe.CTe.TomadorPagador.Cliente.CPF_CNPJ == veiculoUtilizado.Tomador.CPF_CNPJ &&
                            !ObterCodigosCTeDesconsiderarParaComplemento(codigosCTeUtilizados).Contains(cargaCTe.CTe.Codigo)
                        )
                        .OrderByDescending(cargaCTe => string.IsNullOrWhiteSpace(cargaCTe.CTe.CST) || cargaCTe.CTe.CST == "40" || cargaCTe.CTe.CST == "51")
                        .FirstOrDefault();

                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual primeiroDocumentoParaEmissaoNFSManual = documentosParaEmissaoNFSManual
                        .Where(documento =>
                            documento.Carga.Veiculo.ModeloVeicularCarga.Codigo == veiculoUtilizado.ModeloVeicularCarga.Codigo &&
                            documento.Tomador.CPF_CNPJ == veiculoUtilizado.Tomador.CPF_CNPJ
                        )
                        .FirstOrDefault();

                    if (primeiroCargaCTe != null)
                    {
                        codigosCTeUtilizados.Add(primeiroCargaCTe.CTe.Codigo);

                        complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                        {
                            CargaCTe = primeiroCargaCTe,
                            Observacao = veiculoUtilizado.ModeloVeicularCarga.Descricao,
                            ValorComplemento = valorComplemeto
                        });
                    }
                    else if (primeiroDocumentoParaEmissaoNFSManual != null)
                        complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                        {
                            CargaDocumentoParaEmissaoNFSManual = primeiroDocumentoParaEmissaoNFSManual,
                            ValorComplemento = valorComplemeto
                        });
                }
            }
        }

        private void CancelarComplementos(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            Servicos.CTe servicoCte = new Servicos.CTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargasCTeComplementaresInfo = repositorioCargaCTeComplementoInfo.BuscarCTesPorFechamento(fechamentoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargasCTeComplementaresInfo)
            {
                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cargaCTeComplementoInfo.CTe.SistemaEmissor).CancelarCte(cargaCTeComplementoInfo.CTe.Codigo, cargaCTeComplementoInfo.CTe.Empresa.Codigo, "Fechamento de frete reaberto", _unitOfWork))
                    throw new ServicoException($"Não foi possível cancelar o CT-e {cargaCTeComplementoInfo.CTe.Numero}");
            }

            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
            Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada repositorioCargaDocumentoParaEmissaoNFSManualCancelada = new Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual = repositorioCargaDocumentoParaEmissaoNFSManual.ConsultarPorFechamentoFrete(fechamentoFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoParaEmissaoNFSManual in documentosParaEmissaoNFSManual)
            {
                if ((documentoParaEmissaoNFSManual.LancamentoNFSManual != null) && (documentoParaEmissaoNFSManual.LancamentoNFSManual.Situacao == SituacaoLancamentoNFSManual.DadosNota))
                    throw new ServicoException($"O documento {documentoParaEmissaoNFSManual.Numero} está sendo utilizado no lançamento de NFS Manual {documentoParaEmissaoNFSManual.LancamentoNFSManual.DadosNFS.Numero}. Para prosseguir, remova o documento do lançamento");

                if (documentoParaEmissaoNFSManual.LancamentoNFSManual != null)
                    throw new ServicoException($"O documento {documentoParaEmissaoNFSManual.Numero} foi utilizado no lançamento de NFS Manual {documentoParaEmissaoNFSManual.LancamentoNFSManual.DadosNFS.Numero}");

                Dominio.Entidades.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada documentoParaEmissaoNFSManualCancelado = repositorioCargaDocumentoParaEmissaoNFSManualCancelada.BuscarPorDocumentoParaEmissaoNFSManual(documentoParaEmissaoNFSManual.Codigo);

                if (documentoParaEmissaoNFSManualCancelado != null)
                    throw new ServicoException($"O documento {documentoParaEmissaoNFSManual.Numero} foi utilizado no lançamento de NFS Manual {documentoParaEmissaoNFSManualCancelado.LancamentoNFSManual.DadosNFS.Numero} que está cancelado.");

                repositorioCargaDocumentoParaEmissaoNFSManual.Deletar(documentoParaEmissaoNFSManual);
            }

            Frete.ContratoFreteTransportador.RemoverSaldoContratoPorFechamento(fechamentoFrete, _unitOfWork);
        }

        private void GerarComplementos(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!IsGerarComplementos(fechamentoFrete))
                return;

            List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento = ObterComplementosFechamento(fechamentoFrete);

            if (complementosFechamento.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = fechamentoFrete.Contrato.ComponenteFreteValorContrato ?? ObterComponenteFreteComplementoFechamento();
            Servicos.Embarcador.Carga.RateioCTeComplementar servicoRateioFrete = new Servicos.Embarcador.Carga.RateioCTeComplementar(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);

            foreach (Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento complementoFechamento in complementosFechamento)
            {
                if (complementoFechamento.CargaCTe != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo
                    {
                        CargaCTeComplementado = complementoFechamento.CargaCTe,
                        IncluirICMSFrete = complementoFechamento.CargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim,
                        ComplementoIntegradoEmbarcador = false,
                        ComponenteFrete = componenteFrete,
                        ObservacaoCTe = complementoFechamento.Observacao ?? "",
                        ValorComplemento = complementoFechamento.ValorComplemento,
                        FechamentoFrete = fechamentoFrete
                    };

                    string mensagemErroCalcularImpostosComplemento = servicoRateioFrete.CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, _unitOfWork, configuracaoEmbarcador);

                    if (!string.IsNullOrWhiteSpace(mensagemErroCalcularImpostosComplemento))
                        throw new ServicoException(mensagemErroCalcularImpostosComplemento);

                    repositorioCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfo);
                }
                else
                {
                    fechamentoFrete.AguardandoNFSManual = true;

                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual()
                    {
                        Carga = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.Carga,
                        CargaOrigem = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.CargaOrigem,
                        DataEmissao = DateTime.Now,
                        Descricao = "",
                        Destinatario = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.Destinatario,
                        FechamentoFrete = fechamentoFrete,
                        LocalidadePrestacao = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.LocalidadePrestacao,
                        ModeloDocumentoFiscal = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.ModeloDocumentoFiscal,
                        Moeda = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.Moeda,
                        Remetente = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.Remetente,
                        Tomador = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.Tomador,
                        ValorCotacaoMoeda = complementoFechamento.CargaDocumentoParaEmissaoNFSManual.ValorCotacaoMoeda,
                        ValorFrete = complementoFechamento.ValorComplemento
                    };

                    if (cargaDocumentoParaEmissaoNFSManual.Moeda.HasValue)
                        cargaDocumentoParaEmissaoNFSManual.ValorTotalMoeda = Math.Round(cargaDocumentoParaEmissaoNFSManual.ValorFrete / (cargaDocumentoParaEmissaoNFSManual.ValorCotacaoMoeda ?? 0), 2, MidpointRounding.AwayFromZero);

                    Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(cargaDocumentoParaEmissaoNFSManual, repositorioCargaDocumentoParaEmissaoNFSManual, _unitOfWork);
                }
            }

            if (configuracaoEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorKm)
                Frete.ContratoFreteTransportador.GerarSaldoContratoPorFechamento(fechamentoFrete, tipoServicoMultisoftware, _unitOfWork);
        }

        private void GerarControleFaturamentoDocumentos(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            //todo: fazer a geração de pagamento, tem que gerar a provisão por nota para poder gerar a exportação do pagamento.
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe != null)
                {
                    Servicos.Log.GravarInfo($"GerarControleFaturamentoDocumentos inserindo documento faturamento - Carga {cargaCTeComplementoInfo?.CargaCTeComplementado.Carga?.Codigo ?? 0} - CTe {cargaCTeComplementoInfo?.CTe?.Codigo ?? 0}", "ControleDocumentoFaturamento");
                    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(cargaCTeComplementoInfo.CargaCTeComplementado.Carga, cargaCTeComplementoInfo.CTe, null, null, fechamentoFrete, null, cargaCTeComplementoInfo.ProvisaoPelaNotaFiscal, cargaCTeComplementoInfo.ComplementoFilialEmissora, false, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);
                }
            }
        }

        private void IntegrarGZIPCarrefour(Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao)
        {
            Integracao.Carrefour.IntegracaoCarrefour servicoIntegracaoCarrefour = new Integracao.Carrefour.IntegracaoCarrefour(_unitOfWork);

            if (integracao.LayoutEDI != null)
            {
                byte[] gz = GerarGZIPEDIFechamento(integracao, out string nomeArquivo, incrementarSequencia: false);

                servicoIntegracaoCarrefour.IntegrarCTeFechamentoFreteNotaDebito(integracao, gz, nomeArquivo);
            }
            else
            {
                byte[] gz = Integracao.IntegracaoCTe.GerarXMLAutorizacao(integracao.Complemento.CTe, _unitOfWork);

                servicoIntegracaoCarrefour.IntegrarCTeFechamentoFrete(integracao, gz);
            }
        }

        private bool IsGerarComplementos(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            if (fechamentoFrete.Contrato.NaoEmitirComplementoFechamentoFrete)
                return false;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (configuracaoEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorFaixaKm)
                return true;

            bool possuiTipoFranquia = (fechamentoFrete.Contrato.TipoFranquia != PeriodoAcordoContratoFreteTransportador.NaoPossui);
            bool ultimoPeriodoFechamento = (fechamentoFrete.Periodo + 1) == fechamentoFrete.Contrato.PeriodoAcordo.ObterQuantidade();

            if (possuiTipoFranquia && fechamentoFrete.NaoEmitirComplemento && !ultimoPeriodoFechamento)
                return false;

            if ((ObterValorComplemento(fechamentoFrete) <= 0) && (fechamentoFrete.Contrato.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorTomador
                        || fechamentoFrete.Contrato.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorTomadorEcarga))
                return false;

            return true;
        }

        private void LiquidarDocumentosProvisao(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            LiquidarDocumentosProvisaoCarga(fechamentoFrete);
            LiquidarDocumentosProvisaoOcorrencia(fechamentoFrete);
        }

        private void LiquidarDocumentosProvisaoCarga(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            List<int> codigosCargas = (from obj in fechamentoFrete.Cargas select obj.Carga.Codigo).ToList();

            if (codigosCargas.Count > 0)
            {
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisaoCarga = repositorioDocumentoProvisao.BuscarPorCargasAgProvisao(codigosCargas);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosProvisaoCarga)
                {
                    documento.Situacao = SituacaoProvisaoDocumento.Liquidado;
                    if (documento.DataEmissao == DateTime.MinValue)
                        documento.DataEmissao = DateTime.Now;

                    repositorioDocumentoProvisao.Atualizar(documento);
                }
            }
        }

        private void LiquidarDocumentosProvisaoOcorrencia(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            List<int> codigosOcorrencias = (from obj in fechamentoFrete.Ocorrencias select obj.Ocorrencia.Codigo).ToList();

            if (codigosOcorrencias.Count > 0)
            {
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisaoOcorrencia = repositorioDocumentoProvisao.BuscarPorOcorrenciasAgProvisao(codigosOcorrencias);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosProvisaoOcorrencia)
                {
                    documento.Situacao = SituacaoProvisaoDocumento.Liquidado;
                    if (documento.DataEmissao == DateTime.MinValue)
                        documento.DataEmissao = DateTime.Now;

                    repositorioDocumentoProvisao.Atualizar(documento);
                }
            }
        }

        private decimal ObterValorComplemento(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            return fechamentoFrete.ValorBaseFranquia - fechamentoFrete.ValorPagar + fechamentoFrete.TotalAcrescimosAplicar - fechamentoFrete.TotalDescontosAplicar;
        }

        private void RetornarDocumentosProvisaoParaAguardando(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            RetornarDocumentosProvisaoCargaParaAguardando(fechamentoFrete);
            RetornarDocumentosProvisaoOcorrenciaParaAguardando(fechamentoFrete);
        }

        private void RetornarDocumentosProvisaoCargaParaAguardando(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            List<int> codigosCargas = (from obj in fechamentoFrete.Cargas select obj.Carga.Codigo).ToList();

            if (codigosCargas.Count > 0)
            {
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisaoCarga = repositorioDocumentoProvisao.BuscarPorCargasAgProvisao(codigosCargas);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosProvisaoCarga)
                {
                    documento.Situacao = SituacaoProvisaoDocumento.AgProvisao;
                    repositorioDocumentoProvisao.Atualizar(documento);
                }
            }
        }

        private void RetornarDocumentosProvisaoOcorrenciaParaAguardando(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            List<int> codigosOcorrencias = (from obj in fechamentoFrete.Ocorrencias select obj.Ocorrencia.Codigo).ToList();

            if (codigosOcorrencias.Count > 0)
            {
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisaoOcorrencia = repositorioDocumentoProvisao.BuscarPorOcorrenciasAgProvisao(codigosOcorrencias);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosProvisaoOcorrencia)
                {
                    documento.Situacao = SituacaoProvisaoDocumento.AgProvisao;
                    repositorioDocumentoProvisao.Atualizar(documento);
                }
            }
        }

        private bool VerificarIntegracoes(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia fechamentoFreteOcorrencia in fechamentoFrete.Ocorrencias.ToList())
            {
                TipoDocumentoCreditoDebito tipoDocumentoCreditoDebito = fechamentoFreteOcorrencia.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito ?? TipoDocumentoCreditoDebito.Credito;

                if (tipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Credito)
                    cargaCTesComplementoInfo.AddRange(repositorioCargaCTeComplementoInfo.BuscarPorOcorrencia(fechamentoFreteOcorrencia.Ocorrencia.Codigo));
            }

            int totalComplementos = cargaCTesComplementoInfo.Count;

            if ((totalComplementos == 0) && (fechamentoFrete.Ocorrencias.Count() == 0))
                return false;

            Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repositorioFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Fechamento.FechamentoFreteIntegracao repositorioFechamentoFreteIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao repositorioTipoOperacaoIntegracao = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteIntegracao> integracoes = repositorioFechamentoFreteIntegracao.BuscarPorFechamento(fechamentoFrete.Codigo);
            bool possuiIntegracao = false;

            foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteIntegracao integracao in integracoes)
            {
                List<int> codigosTiposOperacaoPermitemGerarIntegracao = repositorioTipoOperacaoIntegracao.BuscarCodigosTiposOperacaoPorTipoIntegracao(integracao.TipoIntegracao.Tipo);

                for (int i = 0; i < totalComplementos; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = cargaCTesComplementoInfo[i];
                    bool tipoDocumentoEmissaoPermiteAdicionarIntegracao = (
                        (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) ||
                        (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS) ||
                        (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    );

                    if (!tipoDocumentoEmissaoPermiteAdicionarIntegracao)
                        continue;

                    if (!codigosTiposOperacaoPermitemGerarIntegracao.Contains(cargaCTeComplementoInfo.CargaComplementado?.TipoOperacao.Codigo ?? 0))
                        continue;

                    Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracaoCTe = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao()
                    {
                        FechamentoFrete = fechamentoFrete,
                        Complemento = cargaCTeComplementoInfo,
                        TipoIntegracao = integracao.TipoIntegracao,
                        NumeroTentativas = 0,
                        ProblemaIntegracao = "",
                        SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                        DataIntegracao = DateTime.Now,
                    };

                    repositorioFechamentoFreteCTeIntegracao.Inserir(integracaoCTe);
                    possuiIntegracao = true;
                }

                // Uma unica integração para integrar todas ocorrencias
                if (integracao.TipoIntegracao.Tipo == TipoIntegracao.Carrefour && fechamentoFrete.Ocorrencias.Count() > 0)
                {
                    bool possuiND = false;

                    foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia fechamentoFreteOcorrencia in fechamentoFrete.Ocorrencias.ToList())
                    {
                        if (
                            (fechamentoFreteOcorrencia.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal != null) &&
                            (fechamentoFreteOcorrencia.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe) &&
                            (fechamentoFreteOcorrencia.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe) &&
                            (fechamentoFreteOcorrencia.Ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
                        )
                        {
                            possuiND = true;
                            break;
                        }
                    }

                    if (possuiND)
                    {
                        Repositorio.LayoutEDI repositorioLayoutEDI = new Repositorio.LayoutEDI(_unitOfWork);
                        Dominio.Entidades.LayoutEDI layoutEDI = repositorioLayoutEDI.BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI.GEN).FirstOrDefault();

                        if (layoutEDI != null)
                        {
                            Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracaoCTe = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao()
                            {
                                FechamentoFrete = fechamentoFrete,
                                Complemento = null,
                                TipoIntegracao = integracao.TipoIntegracao,
                                NumeroTentativas = 0,
                                ProblemaIntegracao = "",
                                LayoutEDI = layoutEDI,
                                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                                DataIntegracao = DateTime.Now,
                            };

                            repositorioFechamentoFreteCTeIntegracao.Inserir(integracaoCTe);
                            possuiIntegracao = true;
                        }
                    }
                }
            }

            return possuiIntegracao;
        }

        #endregion

        #region Métodos Privados de Consulta

        /// <summary>
        /// Retorna somente CT-e após fazer a geração da contabilização
        /// </summary>
        private List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ObterCargasCTe(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            List<int> codigosCargas = (from o in fechamentoFrete.Cargas select o.Carga.Codigo).ToList();

            return repositorioCargaCTe.BuscarPorCargas(codigosCargas);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> ObterDocumentosParaEmissaoNFSManual(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            return new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            //Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(_unitOfWork);
            //List<int> codigosCargas = (from o in fechamentoFrete.Cargas select o.Carga.Codigo).ToList();

            //return repositorioNFSManual.BuscarPorCargasOrigem(codigosCargas);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.Carga> ObterCargasPeriodo(int codigoFechamento, int codigoContrato, DateTime dataInicio, DateTime dataFim)
        {
            return new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork).BuscarCargasParaFechamento(codigoContrato, codigoFechamento, dataInicio, dataFim, parametrosConsulta: null);
        }

        private List<int> ObterCodigosCTeDesconsiderarParaComplemento(List<int> codigosCTeUtilizados)
        {
            if (codigosCTeUtilizados.Count < 5)
                return new List<int>();

            List<int> codigosCTeDesconsiderar = codigosCTeUtilizados
               .GroupBy(codigoCTe => codigoCTe)
               .Select(agrupamento => new { CodigoCTe = agrupamento.Key, QuantidadeUtilizada = agrupamento.Count() })
               .Where(agrupamento => agrupamento.QuantidadeUtilizada >= 5)
               .Select(agrupamento => agrupamento.CodigoCTe)
               .ToList();

            return codigosCTeDesconsiderar;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> ObterComplementosFechamento(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = ObterCargasCTe(fechamentoFrete);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual = ObterDocumentosParaEmissaoNFSManual(fechamentoFrete);

            if ((cargasCTe.Count == 0) && (documentosParaEmissaoNFSManual.Count == 0))
                return new List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento>();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (configuracaoEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorFaixaKm)
                return ObterComplementosFechamentoPorFaixaKm(fechamentoFrete, cargasCTe, documentosParaEmissaoNFSManual);

            if (fechamentoFrete.Contrato.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoEMotorista)
                return ObterComplementosFechamentoPorVeiculoEMotorista(fechamentoFrete, cargasCTe, documentosParaEmissaoNFSManual);

            if (fechamentoFrete.Contrato.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorVeiculoDoContrato)
                return ObterComplementosFechamentoPorVeiculoDoContrato(fechamentoFrete, cargasCTe, documentosParaEmissaoNFSManual);

            if (fechamentoFrete.Contrato.TipoEmissaoComplemento == TipoEmissaoComplementoContratoFreteTransportador.PorTomadorEcarga)
                return ObterComplementosFechamentoPorDestinatarioECarga(fechamentoFrete, cargasCTe, documentosParaEmissaoNFSManual);

            return ObterComplementosFechamentoPorDestinatario(fechamentoFrete, cargasCTe, documentosParaEmissaoNFSManual);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> ObterComplementosFechamentoPorFaixaKm(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento = new List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento>();
            Dominio.Entidades.Embarcador.Cargas.CargaCTe primeiroCargaCTe = cargasCTe.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual primeiroDocumentoParaEmissaoNFSManual = documentosParaEmissaoNFSManual.FirstOrDefault();

            if (primeiroCargaCTe != null)
                complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                {
                    CargaCTe = primeiroCargaCTe,
                    ValorComplemento = fechamentoFrete.ValorPagar
                });
            else if (primeiroDocumentoParaEmissaoNFSManual != null)
                complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                {
                    CargaDocumentoParaEmissaoNFSManual = primeiroDocumentoParaEmissaoNFSManual,
                    ValorComplemento = fechamentoFrete.ValorPagar
                });

            return complementosFechamento;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> ObterComplementosFechamentoPorDestinatario(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual)
        {
            fechamentoFrete.ValorComplementos = ObterValorComplemento(fechamentoFrete);

            decimal valorTotalPago = 0m;
            List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento = new List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento>();
            List<double> cpfCnpjDestinatarios = (
                from o in cargasCTe select o.CTe.Destinatario.Cliente.CPF_CNPJ).Concat((
                from o in documentosParaEmissaoNFSManual select o.Destinatario.CPF_CNPJ)
            ).Distinct().ToList();

            foreach (double cpfCnpjDestinatario in cpfCnpjDestinatarios)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe primeiroCargaCTePorDestinatario = cargasCTe.Find(cargaCTe => cargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == cpfCnpjDestinatario);
                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual primeiroDocumentoParaEmissaoNFSManual = documentosParaEmissaoNFSManual.Find(documento => documento.Destinatario.CPF_CNPJ == cpfCnpjDestinatario);
                decimal valorPago = (
                    cargasCTe.Where(cargaCTe => cargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == cpfCnpjDestinatario).Sum(cargaCTe => cargaCTe.CTe.ValorAReceber) +
                    documentosParaEmissaoNFSManual.Where(documento => documento.Destinatario.CPF_CNPJ == cpfCnpjDestinatario).Sum(documento => documento.ValorFrete)
                );

                valorTotalPago += valorPago;

                if (primeiroCargaCTePorDestinatario != null)
                    complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                    {
                        CargaCTe = primeiroCargaCTePorDestinatario,
                        ValorPago = valorPago
                    });
                else if (primeiroDocumentoParaEmissaoNFSManual != null)
                    complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                    {
                        CargaDocumentoParaEmissaoNFSManual = primeiroDocumentoParaEmissaoNFSManual,
                        ValorPago = valorPago
                    });
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento complementoFechamento in complementosFechamento)
                complementoFechamento.ValorComplemento = Math.Round((complementoFechamento.ValorPago * fechamentoFrete.ValorComplementos) / valorTotalPago, 2, MidpointRounding.AwayFromZero);

            decimal valorTotalComplementos = complementosFechamento.Sum(o => o.ValorComplemento);
            decimal valorDiferencaRateio = (valorTotalComplementos - fechamentoFrete.ValorComplementos);
            int totalInteiroDiferencaRateio = Math.Abs((int)(valorDiferencaRateio * 100));
            int totalComplementos = complementosFechamento.Count;
            List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamentoOrdenadoPorValorComplemento = complementosFechamento.OrderByDescending(o => o.ValorComplemento).ToList();

            for (int i = 0; i < totalInteiroDiferencaRateio; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento complementoFechamento = complementosFechamentoOrdenadoPorValorComplemento.ElementAt(i % totalComplementos);

                if (valorDiferencaRateio > 0)
                    complementoFechamento.ValorComplemento -= 0.01m;
                else
                    complementoFechamento.ValorComplemento += 0.01m;
            }

            return complementosFechamento;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> ObterComplementosFechamentoPorVeiculoDoContrato(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo repositorioAcordo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> acordos = repositorioAcordo.BuscarPorContrato(fechamentoFrete.Contrato.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento = new List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento>();

            if (acordos.Count == 0)
                return complementosFechamento;

            decimal valorTotalComplemento = 0m;
            List<int> codigosVeiculos = cargasCTe
                .Select(cargaCTe => cargaCTe.Carga.Veiculo.Codigo)
                .Concat(documentosParaEmissaoNFSManual.Select(documento => documento.Carga.Veiculo.Codigo))
                .Distinct()
                .ToList();

            foreach (int codigoVeiculo in codigosVeiculos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe primeiroCargaCTe = cargasCTe
                    .Where(cargaCTe => cargaCTe.Carga.Veiculo.Codigo == codigoVeiculo)
                    .FirstOrDefault();

                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual primeiroDocumentoParaEmissaoNFSManual = documentosParaEmissaoNFSManual
                    .Where(documento => documento.Carga.Veiculo.Codigo == codigoVeiculo)
                    .FirstOrDefault();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = primeiroCargaCTe?.Carga ?? primeiroDocumentoParaEmissaoNFSManual?.Carga;
                decimal valorAcordado = acordos.Where(acordo => acordo.Periodo == fechamentoFrete.Periodo && acordo.ModeloVeicular.Codigo == (carga.Veiculo.ModeloVeicularCarga?.Codigo ?? 0)).FirstOrDefault()?.ValorAcordado ?? 0m;

                if (valorAcordado <= 0m)
                    continue;

                decimal valorPago = (
                    cargasCTe.Where(cargaCTe => cargaCTe.Carga.Veiculo.Codigo == codigoVeiculo).Sum(cargaCTe => cargaCTe.CTe.ValorAReceber) +
                    documentosParaEmissaoNFSManual.Where(documento => documento.Carga.Veiculo.Codigo == codigoVeiculo).Sum(documento => documento.ValorFrete)
                );

                if (valorPago >= valorAcordado)
                    continue;

                decimal valorComplemento = (valorAcordado - valorPago);

                if (primeiroCargaCTe != null)
                    complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                    {
                        CargaCTe = primeiroCargaCTe,
                        Observacao = $"Complemento do veículo {carga.Veiculo.Placa}",
                        ValorComplemento = valorComplemento,
                        ValorPago = valorPago
                    });
                else if (primeiroDocumentoParaEmissaoNFSManual != null)
                    complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                    {
                        CargaDocumentoParaEmissaoNFSManual = primeiroDocumentoParaEmissaoNFSManual,
                        ValorComplemento = valorComplemento,
                        ValorPago = valorPago
                    });

                valorTotalComplemento += valorComplemento;
            }

            fechamentoFrete.ValorComplementos = valorTotalComplemento;

            return complementosFechamento;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> ObterComplementosFechamentoPorVeiculoEMotorista(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento = new List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento>();
            List<int> codigosCTeUtilizados = new List<int>();

            AdicionarComplementosFechamentoPorVeiculos(fechamentoFrete, cargasCTe, documentosParaEmissaoNFSManual, complementosFechamento, codigosCTeUtilizados);
            AdicionarComplementosFechamentoPorMotoristas(fechamentoFrete, cargasCTe, documentosParaEmissaoNFSManual, complementosFechamento, codigosCTeUtilizados);
            AdicionarComplementosFechamentoPorValoresOutrosRecursos(fechamentoFrete, cargasCTe, documentosParaEmissaoNFSManual, complementosFechamento, codigosCTeUtilizados);

            fechamentoFrete.ValorComplementos = complementosFechamento.Sum(complemento => complemento.ValorComplemento);

            return complementosFechamento;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> ObterComplementosFechamentoPorDestinatarioECarga(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSManual)
        {
            fechamentoFrete.ValorComplementos = ObterValorComplemento(fechamentoFrete);

            decimal valorTotalPago = 0m;
            List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamento = new List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento>();
            List<double> cpfCnpjDestinatarios = (from o in cargasCTe select o.CTe.Destinatario.Cliente.CPF_CNPJ).Concat((from o in documentosParaEmissaoNFSManual select o.Destinatario.CPF_CNPJ)).Distinct().ToList();

            foreach (double cpfCnpjDestinatario in cpfCnpjDestinatarios)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTePorDestinatario = cargasCTe.Where(cargaCTe => cargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == cpfCnpjDestinatario).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTePorDestinatario in cargasCTePorDestinatario)
                {
                    decimal valorPago = cargasCTe.Where(cargaCTe => cargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == cpfCnpjDestinatario && cargaCTe.Carga.Codigo == cargaCTePorDestinatario.Carga.Codigo).Sum(cargaCTe => cargaCTe.CTe.ValorAReceber);

                    complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                    {
                        CargaCTe = cargaCTePorDestinatario,
                        ValorPago = valorPago
                    });

                    valorTotalPago += valorPago;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissoesNFSManual = documentosParaEmissaoNFSManual.Where(documento => documento.Destinatario.CPF_CNPJ == cpfCnpjDestinatario).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoParaEmissaoNFSManual in documentosParaEmissoesNFSManual)
                {
                    decimal valorPago = documentosParaEmissaoNFSManual.Where(documento => documento.Destinatario.CPF_CNPJ == cpfCnpjDestinatario).Sum(documento => documento.ValorFrete);

                    complementosFechamento.Add(new Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento
                    {
                        CargaDocumentoParaEmissaoNFSManual = documentoParaEmissaoNFSManual,
                        ValorPago = valorPago
                    });

                    valorTotalPago += valorPago;
                }
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento complementoFechamento in complementosFechamento)
                complementoFechamento.ValorComplemento = Math.Round((complementoFechamento.ValorPago * fechamentoFrete.ValorComplementos) / valorTotalPago, 2, MidpointRounding.AwayFromZero);

            decimal valorTotalComplementos = complementosFechamento.Sum(o => o.ValorComplemento);
            decimal valorDiferencaRateio = (valorTotalComplementos - fechamentoFrete.ValorComplementos);
            int totalInteiroDiferencaRateio = Math.Abs((int)(valorDiferencaRateio * 100));
            int totalComplementos = complementosFechamento.Count;
            List<Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento> complementosFechamentoOrdenadoPorValorComplemento = complementosFechamento.OrderByDescending(o => o.ValorComplemento).ToList();

            for (int i = 0; i < totalInteiroDiferencaRateio; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Fechamento.ComplementoFechamento complementoFechamento = complementosFechamentoOrdenadoPorValorComplemento.ElementAt(i % totalComplementos);

                if (valorDiferencaRateio > 0)
                    complementoFechamento.ValorComplemento -= 0.01m;
                else
                    complementoFechamento.ValorComplemento += 0.01m;
            }

            return complementosFechamento;
        }

        private Dominio.Entidades.Embarcador.Frete.ComponenteFrete ObterComponenteFreteComplementoFechamento()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = ObterConfiguracaoEmbarcador();

            return configuracaoTMS.ComponenteFreteComplementoFechamento;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private decimal ObterDistanciaExcedentePorContratoFrete(int codigoContrato, DateTime dataFim)
        {
            return new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork).ConsultarDistanciaExcedentePorContratoFrete(codigoContrato, new DateTime(dataFim.Year, dataFim.Month, day: 1), dataFim);
        }

        private decimal ObterDistanciaTotalPorContratoFrete(int codigoContrato, DateTime dataFim)
        {
            return new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork).ConsultarDistanciaTotalPorContratoFrete(codigoContrato, new DateTime(dataFim.Year, dataFim.Month, day: 1), dataFim);
        }

        private int ObterTotalKmFranquia(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            return fechamentoFrete?.TotalKmFranquia ?? contrato.FranquiaTotalKM;
        }

        private decimal ObterValorBaseFranquiaPorContrato(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato)
        {
            if (contrato.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui)
                return contrato.ValorMensal;

            return Math.Round(contrato.FranquiaContratoMensal / contrato.PeriodoAcordo.ObterQuantidade(), 2, MidpointRounding.AwayFromZero);
        }

        private decimal ObterValorBaseFranquiaPorPeriodo(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, DateTime dataFim)
        {
            if (fechamentoFrete.Contrato.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui)
                return fechamentoFrete.Contrato.ValorMensal;

            decimal valorFranquiaPago = ObterValorFranquiaPagoPorContrato(fechamentoFrete.Contrato.Codigo, dataFim);
            decimal valorFranquiaRestante = fechamentoFrete.Contrato.FranquiaContratoMensal - valorFranquiaPago;
            decimal valorBaseFranquia = Math.Round(valorFranquiaRestante / (fechamentoFrete.Contrato.PeriodoAcordo.ObterQuantidade() - fechamentoFrete.Periodo), 2, MidpointRounding.AwayFromZero);

            return valorBaseFranquia > 0 ? valorBaseFranquia : 0;
        }

        private decimal ObterValorExcedentePorContratoFretePeriodo(int codigoContrato, DateTime dataInicio, DateTime dataFim)
        {
            return new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork).ConsultarValorExcedentePorContratoFrete(codigoContrato, dataInicio, dataFim);
        }

        private decimal ObterValorExcedentePorContratoFreteMes(int codigoContrato, DateTime dataFim)
        {
            return new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork).ConsultarValorExcedentePorContratoFrete(codigoContrato, new DateTime(dataFim.Year, dataFim.Month, day: 1), dataFim);
        }

        private decimal ObterValorFranquia(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            return fechamentoFrete?.ValorFranquia ?? ObterValorFranquiaPorContrato(contrato);
        }

        private decimal ObterValorFranquiaPorContrato(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato)
        {
            return (contrato.TipoFranquia == PeriodoAcordoContratoFreteTransportador.NaoPossui) ? contrato.ValorMensal : contrato.FranquiaContratoMensal;
        }

        private decimal ObterValorFranquiaPorKm(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, DateTime dataFim)
        {
            return fechamentoFrete?.ValorFranquiaPorKm ?? ObterValorFranquiaPorKmPorTipoFechamentoFrete(contrato, dataFim);
        }

        private decimal ObterValorFranquiaPorKmExcedente(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            return fechamentoFrete?.ValorFranquiaPorKmExcedente ?? contrato.FranquiaValorKmExcedente;
        }

        private decimal ObterValorFranquiaPorKmPorTipoFechamentoFrete(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, DateTime dataFim)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (configuracaoEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorKm)
                return contrato.FranquiaValorKM;

            int distancia = (int)ObterDistanciaTotalPorContratoFrete(contrato.Codigo, dataFim);
            decimal valorPorKm = (
                from o in contrato.FaixasKmFranquia
                where o.QuilometragemInicial <= distancia && o.QuilometragemFinal >= distancia
                select o.ValorPorQuilometro
            ).FirstOrDefault();

            return valorPorKm;
        }

        private decimal ObterValorFranquiaPagoPorContrato(int codigoContrato, DateTime dataFim)
        {
            return new Repositorio.Embarcador.Fechamento.FechamentoFrete(_unitOfWork).BuscarValorFranquiaPagoPorContrato(new DateTime(dataFim.Year, dataFim.Month, day: 1), dataFim, codigoContrato);
        }

        private decimal ObterValorFranquiaPorPeriodo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            if (fechamentoFrete != null)
            {
                if (fechamentoFrete.Situacao == SituacaoFechamentoFrete.Aberto)
                    return ObterValorBaseFranquiaPorPeriodo(fechamentoFrete, fechamentoFrete.DataFim);

                return fechamentoFrete.ValorBaseFranquia;
            }

            return ObterValorBaseFranquiaPorContrato(contrato);
        }

        private decimal ObterValorPagoPorContratoFretePeriodo(int codigoContrato, DateTime dataInicio, DateTime dataFim)
        {
            return new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork).ConsultarValorTotalPorContratoFrete(codigoContrato, dataInicio, dataFim, somenteComCarga: true);
        }

        private decimal ObterValorPagoPorContratoFreteMes(int codigoContrato, DateTime dataFim)
        {
            return new Repositorio.Embarcador.Frete.ContratoSaldoMes(_unitOfWork).ConsultarValorTotalPorContratoFrete(codigoContrato, new DateTime(dataFim.Year, dataFim.Month, day: 1), dataFim, somenteComCarga: true);
        }

        #endregion

        #region Métodos Públicos

        public void AdicionarIntegracoes(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao repositorioTipoOperacaoIntegracao = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao(_unitOfWork);
            List<TipoIntegracao> tiposIntegracaoPorTipoOperacao = repositorioTipoOperacaoIntegracao.BuscarTiposIntegracaoPorFechamento(fechamentoFrete.Codigo);

            if (tiposIntegracaoPorTipoOperacao.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Fechamento.FechamentoFreteIntegracao repositorioFechamentoFreteIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteIntegracao(_unitOfWork);
            List<TipoIntegracao> tiposIntegracaoPermitidos = new List<TipoIntegracao>() { TipoIntegracao.Carrefour };
            List<TipoIntegracao> tiposIntegracaoAdicionar = tiposIntegracaoPorTipoOperacao.Where(tipo => tiposIntegracaoPermitidos.Contains(tipo)).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(tiposIntegracaoAdicionar);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                if (repositorioFechamentoFreteIntegracao.ExistePorFechamentoETIpo(fechamentoFrete.Codigo, tipoIntegracao.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteIntegracao integracao = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteIntegracao()
                {
                    FechamentoFrete = fechamentoFrete,
                    TipoIntegracao = tipoIntegracao
                };

                repositorioFechamentoFreteIntegracao.Inserir(integracao);
            }
        }

        public void Finalizar(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (fechamentoFrete.Situacao != SituacaoFechamentoFrete.Aberto)
                throw new ServicoException("Não é possível finalizar o fechamento em sua atual situação.");

            fechamentoFrete.ComponenteFreteValorContrato = fechamentoFrete.Contrato.ComponenteFreteValorContrato;
            fechamentoFrete.Situacao = SituacaoFechamentoFrete.EmEmissaoComplemento;
            fechamentoFrete.TotalKmFranquia = fechamentoFrete.Contrato.FranquiaTotalKM;
            fechamentoFrete.ValorBaseFranquia = ObterValorBaseFranquiaPorPeriodo(fechamentoFrete, fechamentoFrete.DataFim);
            fechamentoFrete.ValorFranquia = ObterValorFranquiaPorContrato(fechamentoFrete.Contrato);
            fechamentoFrete.ValorFranquiaPorKm = ObterValorFranquiaPorKmPorTipoFechamentoFrete(fechamentoFrete.Contrato, fechamentoFrete.DataFim);
            fechamentoFrete.ValorFranquiaPorKmExcedente = fechamentoFrete.Contrato.FranquiaValorKmExcedente;

            LiquidarDocumentosProvisao(fechamentoFrete);
            GerarComplementos(fechamentoFrete, tipoServicoMultisoftware);

            new Repositorio.Embarcador.Fechamento.FechamentoFrete(_unitOfWork).Atualizar(fechamentoFrete);

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, fechamentoFrete, "Finalizou o fechamento.", _unitOfWork);
        }

        public byte[] GerarGZIPEDIFechamento(Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao, out string nomeArquivo, bool incrementarSequencia)
        {
            nomeArquivo = "";

            System.IO.MemoryStream edi = Integracao.IntegracaoEDI.GerarEDI(integracao, _unitOfWork);
            nomeArquivo = Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, incrementarSequencia, _unitOfWork);

            return edi.ToArray();
        }

        public void IntegrarComplementosFechamento()
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao repositorioFechamentoFreteCTeIntegracao = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Fechamento.FechamentoFrete repositorioFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao> integracoesPendentes = repositorioFechamentoFreteCTeIntegracao.BuscarCTeIntegracaoPendente(tentativasLimite: 2, tempoProximaTentativaMinutos: 5);
            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> fechamentos = (from obj in integracoesPendentes select obj.FechamentoFrete).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao in integracoesPendentes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Carrefour:
                        IntegrarGZIPCarrefour(integracao);
                        break;
                    default:
                        break;
                }

                repositorioFechamentoFreteCTeIntegracao.Atualizar(integracao);
            }

            foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento in fechamentos)
            {
                if (repositorioFechamentoFreteCTeIntegracao.ContarPorFechamento(fechamento.Codigo, SituacaoIntegracao.AgIntegracao) > 0)
                    fechamento.Situacao = SituacaoFechamentoFrete.AgIntegracao;
                else if (repositorioFechamentoFreteCTeIntegracao.ContarPorFechamento(fechamento.Codigo, SituacaoIntegracao.ProblemaIntegracao) > 0)
                    fechamento.Situacao = SituacaoFechamentoFrete.ProblemaIntegracao;
                else if (repositorioFechamentoFreteCTeIntegracao.ContarPorFechamento(fechamento.Codigo, SituacaoIntegracao.AgIntegracao) == 0)
                    fechamento.Situacao = SituacaoFechamentoFrete.Fechado;

                repositorioFechamentoFrete.Atualizar(fechamento);
            }
        }

        public void Reabrir(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete)
        {
            if (fechamentoFrete.Situacao != SituacaoFechamentoFrete.Fechado)
                throw new ServicoException("Não é possível reabrir o fechamento em sua atual situação.");

            try
            {
                _unitOfWork.Start();

                CancelarComplementos(fechamentoFrete);
                RetornarDocumentosProvisaoParaAguardando(fechamentoFrete);

                fechamentoFrete.Situacao = SituacaoFechamentoFrete.Aberto;
                fechamentoFrete.TotalKmFranquia = null;
                fechamentoFrete.ValorFranquia = null;
                fechamentoFrete.ValorFranquiaPorKm = null;
                fechamentoFrete.ValorFranquiaPorKmExcedente = null;
                fechamentoFrete.ComponenteFreteValorContrato = null;

                new Repositorio.Embarcador.Fechamento.FechamentoFrete(_unitOfWork).Atualizar(fechamentoFrete);

                if (_auditado != null)
                    Auditoria.Auditoria.Auditar(_auditado, fechamentoFrete, "Reabriu o fechamento.", _unitOfWork);

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void ValidarEmissaoComplementosFechamentoFrete(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceOracle = "")
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarPorFechamento(fechamentoFrete.Codigo);

            bool emitiuTodos = true;
            bool rejeicao = false;
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> ctesParaEmissao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTesComplementoInfo)
            {
                if (cargaCTeComplementoInfo.CTe == null)
                {
                    emitiuTodos = false;
                    ctesParaEmissao.Add(cargaCTeComplementoInfo);
                }
                else
                {
                    if (cargaCTeComplementoInfo.CTe != null)
                    {
                        if (cargaCTeComplementoInfo.CTe.Status != "A")
                            emitiuTodos = false;

                        if (cargaCTeComplementoInfo.CTe.Status == "E" && cargaCTeComplementoInfo.CTe.CodigoCTeIntegrador == 0 && (cargaCTeComplementoInfo.CTe.SistemaEmissor ?? TipoEmissorDocumento.Integrador) == TipoEmissorDocumento.Integrador)
                        {
                            cargaCTeComplementoInfo.CTe.Status = "R";
                            cargaCTeComplementoInfo.CTe.MensagemRetornoSefaz = "888 - Falha ao conectar com o Sefaz.";
                            repCTe.Atualizar(cargaCTeComplementoInfo.CTe);
                        }

                        if (cargaCTeComplementoInfo.CTe.Status == "R")
                        {
                            rejeicao = true;
                            break;
                        }
                    }

                }
            }

            _unitOfWork.Start();

            if (emitiuTodos)
            {
                Servicos.Embarcador.Hubs.Fechamento hubFechamento = new Hubs.Fechamento();

                if (VerificarIntegracoes(fechamentoFrete, cargaCTesComplementoInfo))
                    fechamentoFrete.Situacao = SituacaoFechamentoFrete.AgIntegracao;
                else
                    fechamentoFrete.Situacao = SituacaoFechamentoFrete.Fechado;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfoFechamento = (from obj in cargaCTesComplementoInfo where obj.FechamentoFrete != null select obj).ToList();

                Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentosParaFechamento(fechamentoFrete, cargaCTesComplementoInfoFechamento, tipoServicoMultisoftware, _unitOfWork);
                GerarControleFaturamentoDocumentos(fechamentoFrete, cargaCTesComplementoInfoFechamento, tipoServicoMultisoftware);
                Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(fechamentoFrete, cargaCTesComplementoInfoFechamento, tipoServicoMultisoftware, _unitOfWork);

                repFechamentoFrete.Atualizar(fechamentoFrete);

                hubFechamento.InformarFechamentoAtualizada(fechamentoFrete.Codigo, _unitOfWork.StringConexao);
            }
            else
            {
                fechamentoFrete.Situacao = rejeicao ? SituacaoFechamentoFrete.PendenciaEmissao : SituacaoFechamentoFrete.EmEmissaoComplemento;

                repFechamentoFrete.Atualizar(fechamentoFrete);
            }

            _unitOfWork.CommitChanges();

            if (ctesParaEmissao.Count > 0)
            {
                Carga.CTeComplementar servicoCargaCTeComplementar = new Carga.CTeComplementar(_unitOfWork);
                servicoCargaCTeComplementar.EmitirDocumentoComplementar(ctesParaEmissao, _unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware, webServiceOracle, fechamentoFrete.Contrato.ModeloDocumentoFiscal);
            }
        }

        #endregion

        #region Métodos Públicos de Consulta

        public dynamic ObterDadosSumarizados(int codigoFechamento, int codigoContrato, DateTime dataInicio, DateTime dataFim)
        {
            Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork).BuscarPorCodigo(codigoContrato);
            Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete = codigoFechamento > 0 ? new Repositorio.Embarcador.Fechamento.FechamentoFrete(_unitOfWork).BuscarPorCodigo(codigoFechamento) : null;
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> acordos = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(_unitOfWork).BuscarPorContrato(codigoContrato);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = ObterCargasPeriodo(codigoFechamento, codigoContrato, dataInicio, dataFim);
            List<dynamic> motoristasUtilizados = new List<dynamic>();
            List<dynamic> veiculosUtilizados = new List<dynamic>();

            decimal valorFranquiaPorKm = ObterValorFranquiaPorKm(contrato, fechamentoFrete, dataFim);
            decimal valorExcedenteMes = ObterValorExcedentePorContratoFreteMes(codigoContrato, dataFim);
            decimal valorExcedentePeriodo = ObterValorExcedentePorContratoFretePeriodo(codigoContrato, dataInicio, dataFim);
            decimal valorTotalPagoTabelaMes = ObterValorPagoPorContratoFreteMes(codigoContrato, dataFim);
            decimal valorTotalPagoTabelaPeriodo = ObterValorPagoPorContratoFretePeriodo(codigoContrato, dataInicio, dataFim);
            int totalKMRealizado = (int)ObterDistanciaTotalPorContratoFrete(codigoContrato, dataFim);
            int totalKMExcedido = (int)ObterDistanciaExcedentePorContratoFrete(codigoContrato, dataFim);
            decimal totalFranquia = ObterValorFranquia(contrato, fechamentoFrete);
            decimal totalFranquiaPeriodo = ObterValorFranquiaPorPeriodo(contrato, fechamentoFrete);
            decimal valorDiferencaTotalCargas = (totalFranquia - valorTotalPagoTabelaMes);
            decimal totalJaPagoTabelaSemana1 = 0m;
            decimal totalJaPagoTabelaSemana2 = 0m;
            decimal totalJaPagoTabelaSemana3 = 0m;
            decimal totalJaPagoTabelaSemana4 = 0m;
            decimal totalJaPagoTabelaDezena1 = 0m;
            decimal totalJaPagoTabelaDezena2 = 0m;
            decimal totalJaPagoTabelaDezena3 = 0m;
            decimal totalJaPagoTabelaQuinzena1 = 0m;
            decimal totalJaPagoTabelaQuinzena2 = 0m;

            int totalDias = (
                from carga in cargas
                select new
                {
                    DataCarregamentoCarga = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? carga.DataCriacaoCarga.ToString("dd/MM/yyyy")
                }
            ).GroupBy(o => o.DataCarregamentoCarga).Count();

            if (fechamentoFrete == null)
            {
                List<int> codigosCargas = cargas.Select(o => o.Codigo).ToList();
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCTe.BuscarPorCargas(codigosCargas);

                List<(int Codigo, string Descricao)> modelosVeicularesCarga = cargas
                    .Where(carga => carga.Veiculo?.ModeloVeicularCarga != null)
                    .Select(carga => ValueTuple.Create(carga.Veiculo.ModeloVeicularCarga.Codigo, carga.Veiculo.ModeloVeicularCarga.Descricao))
                    .Distinct()
                    .ToList();

                foreach ((int Codigo, string Descricao) modeloVeicularCarga in modelosVeicularesCarga)
                {
                    decimal valorAcordadoPorPlaca = acordos.Where(acordo => acordo.ModeloVeicular.Codigo == modeloVeicularCarga.Codigo).FirstOrDefault()?.ValorAcordado ?? 0m;
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPorModeloVeicularCarga = cargas.Where(carga => carga.Veiculo?.ModeloVeicularCarga.Codigo == modeloVeicularCarga.Codigo).ToList();
                    List<(int CodigoVeiculo, double CpfCnpjTomador, string NomeTomador)> dadosVeiculosPorTomadores = new List<(int CodigoVeiculo, double CpfCnpjTomador, string NomeTomador)>();

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasPorModeloVeicularCarga)
                    {
                        List<Dominio.Entidades.Cliente> tomadores = cargaCTes.Where(o => o.Carga.Codigo == carga.Codigo && o.CTe.TomadorPagador?.Cliente != null).Select(o => o.CTe.TomadorPagador.Cliente).Distinct().ToList();

                        foreach (Dominio.Entidades.Cliente tomador in tomadores)
                            dadosVeiculosPorTomadores.Add(ValueTuple.Create(carga.Veiculo.Codigo, tomador.CPF_CNPJ, tomador.Nome));
                    }

                    IEnumerable<IGrouping<(double, string), (int CodigoVeiculo, double CpfCnpjTomador, string NomeTomador)>> dadosVeiculosPorTomadoresAgrupados = dadosVeiculosPorTomadores
                        .GroupBy(o => ValueTuple.Create(o.CpfCnpjTomador, o.NomeTomador));

                    foreach (IGrouping<(double CpfCnpjTomador, string NomeTomador), (int CodigoVeiculo, double CpfCnpjTomador, string NomeTomador)> dadosVeiculosPorTomador in dadosVeiculosPorTomadoresAgrupados)
                    {
                        int totalPlacasUtilizadas = dadosVeiculosPorTomador.Select(o => o.CodigoVeiculo).Distinct().Count();
                        decimal valorTotalPorPlaca = (totalPlacasUtilizadas * valorAcordadoPorPlaca);

                        veiculosUtilizados.Add(
                            new
                            {
                                Codigo = Guid.NewGuid().ToString(),
                                CodigoModeloVeicularCarga = modeloVeicularCarga.Codigo,
                                CodigoTomador = dadosVeiculosPorTomador.Key.CpfCnpjTomador,
                                ModeloVeicularCarga = modeloVeicularCarga.Descricao,
                                Tomador = dadosVeiculosPorTomador.Key.NomeTomador,
                                Quantidade = totalPlacasUtilizadas.ToString("n2"),
                                Valor = valorAcordadoPorPlaca.ToString("n2"),
                                Total = valorTotalPorPlaca.ToString("n2")
                            }
                        );
                    }
                }

                List<(int CodigoMotorista, double CpfCnpjTomador, string NomeTomador)> dadosMotoristasPorTomadores = new List<(int CodigoVeiculo, double CpfCnpjTomador, string NomeTomador)>();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    List<Dominio.Entidades.Cliente> tomadores = cargaCTes.Where(o => o.Carga.Codigo == carga.Codigo && o.CTe.TomadorPagador?.Cliente != null).Select(o => o.CTe.TomadorPagador.Cliente).Distinct().ToList();

                    foreach (Dominio.Entidades.Cliente tomador in tomadores)
                        foreach (Dominio.Entidades.Usuario motoristaCarga in carga.Motoristas)
                            dadosMotoristasPorTomadores.Add(ValueTuple.Create(motoristaCarga.Codigo, tomador.CPF_CNPJ, tomador.Nome));
                }

                IEnumerable<IGrouping<(double, string), (int CodigoMotorista, double CpfCnpjTomador, string NomeTomador)>> dadosMotoristasPorTomadoresAgrupados = dadosMotoristasPorTomadores
                    .GroupBy(o => ValueTuple.Create(o.CpfCnpjTomador, o.NomeTomador));

                foreach (IGrouping<(double CpfCnpjTomador, string NomeTomador), (int CodigoMotorista, double CpfCnpjTomador, string NomeTomador)> dadosMotoristasPorTomador in dadosMotoristasPorTomadoresAgrupados)
                {
                    int totalMotoristasUtilizados = dadosMotoristasPorTomador.Select(o => o.CodigoMotorista).Distinct().Count();
                    decimal valorTotalPorMotorista = (totalMotoristasUtilizados * contrato.ValorPorMotorista);

                    motoristasUtilizados.Add(
                        new
                        {
                            Codigo = Guid.NewGuid().ToString(),
                            CodigoTomador = dadosMotoristasPorTomador.Key.CpfCnpjTomador,
                            Tomador = dadosMotoristasPorTomador.Key.NomeTomador,
                            Quantidade = totalMotoristasUtilizados.ToString("n2"),
                            Valor = contrato.ValorPorMotorista.ToString("n2"),
                            Total = valorTotalPorMotorista.ToString("n2")
                        }
                    );
                }
            }
            else
            {
                Repositorio.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado repositorioVeiculoUtilizado = new Repositorio.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado> veiculosUtilizadosFechamento = repositorioVeiculoUtilizado.BuscarPorFechamento(fechamentoFrete.Codigo);

                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado veiculoUtilizadoFechamento in veiculosUtilizadosFechamento)
                    veiculosUtilizados.Add(
                        new
                        {
                            Codigo = veiculoUtilizadoFechamento.Codigo,
                            CodigoModeloVeicularCarga = veiculoUtilizadoFechamento.ModeloVeicularCarga.Codigo,
                            CodigoTomador = veiculoUtilizadoFechamento.Tomador.CPF_CNPJ,
                            ModeloVeicularCarga = veiculoUtilizadoFechamento.ModeloVeicularCarga.Descricao,
                            Tomador = veiculoUtilizadoFechamento.Tomador.Nome,
                            Quantidade = veiculoUtilizadoFechamento.Quantidade.ToString("n2"),
                            Valor = veiculoUtilizadoFechamento.Valor.ToString("n2"),
                            Total = veiculoUtilizadoFechamento.ValorTotal.ToString("n2")
                        }
                    );

                Repositorio.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado repositorioMotoristaUtilizado = new Repositorio.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado> motoristasUtilizadosFechamento = repositorioMotoristaUtilizado.BuscarPorFechamento(fechamentoFrete.Codigo);

                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado motoristaUtilizadoFechamento in motoristasUtilizadosFechamento)
                    motoristasUtilizados.Add(
                        new
                        {
                            Codigo = motoristaUtilizadoFechamento.Codigo,
                            CodigoTomador = motoristaUtilizadoFechamento.Tomador.CPF_CNPJ,
                            Tomador = motoristaUtilizadoFechamento.Tomador.Nome,
                            Quantidade = motoristaUtilizadoFechamento.Quantidade.ToString("n2"),
                            Valor = motoristaUtilizadoFechamento.Valor.ToString("n2"),
                            Total = motoristaUtilizadoFechamento.ValorTotal.ToString("n2")
                        }
                    );
            }

            if (contrato.PeriodoAcordo == PeriodoAcordoContratoFreteTransportador.Semanal)
            {
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoSemana1 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Semanal, dataInicio, periodo: 0);
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoSemana2 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Semanal, dataInicio, periodo: 1);
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoSemana3 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Semanal, dataInicio, periodo: 2);
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoSemana4 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Semanal, dataInicio, periodo: 3);

                totalJaPagoTabelaSemana1 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoSemana1.DataInicio, fechamentoFretePeriodoSemana1.DataFim);
                totalJaPagoTabelaSemana2 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoSemana2.DataInicio, fechamentoFretePeriodoSemana2.DataFim);
                totalJaPagoTabelaSemana3 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoSemana3.DataInicio, fechamentoFretePeriodoSemana3.DataFim);
                totalJaPagoTabelaSemana4 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoSemana4.DataInicio, fechamentoFretePeriodoSemana4.DataFim);
            }
            else if (contrato.PeriodoAcordo == PeriodoAcordoContratoFreteTransportador.Decendial)
            {
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoDezena1 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Decendial, dataInicio, periodo: 0);
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoDezena2 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Decendial, dataInicio, periodo: 1);
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoDezena3 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Decendial, dataInicio, periodo: 2);

                totalJaPagoTabelaDezena1 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoDezena1.DataInicio, fechamentoFretePeriodoDezena1.DataFim);
                totalJaPagoTabelaDezena2 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoDezena2.DataInicio, fechamentoFretePeriodoDezena2.DataFim);
                totalJaPagoTabelaDezena3 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoDezena3.DataInicio, fechamentoFretePeriodoDezena3.DataFim);
            }
            else if (contrato.PeriodoAcordo == PeriodoAcordoContratoFreteTransportador.Quinzenal)
            {
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoQuinzena1 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Quinzenal, dataInicio, periodo: 0);
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodoQuinzena2 = ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador.Quinzenal, dataInicio, periodo: 1);

                totalJaPagoTabelaQuinzena1 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoQuinzena1.DataInicio, fechamentoFretePeriodoQuinzena1.DataFim);
                totalJaPagoTabelaQuinzena2 = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, fechamentoFretePeriodoQuinzena2.DataInicio, fechamentoFretePeriodoQuinzena2.DataFim);
            }

            return new
            {
                SumarizadoViagensRealizadas = new
                {
                    TotalViagens = cargas.Count,
                    ValorTotalPagoTabela = valorTotalPagoTabelaPeriodo,
                    AdicionalKM = valorExcedentePeriodo
                },
                SumarizadoFranquia = new
                {
                    TotalKMFranquia = ObterTotalKmFranquia(contrato, fechamentoFrete),
                    TotalKMRealizado = totalKMRealizado,
                    TotalKMExcedido = totalKMExcedido,
                    ValorKMFranquia = valorFranquiaPorKm,
                    ValorKMExcedido = ObterValorFranquiaPorKmExcedente(contrato, fechamentoFrete),
                    ValorTotalKMFranquia = totalFranquia
                },
                SumarizadoFranquiaPorFaixaKm = new
                {
                    TotalKMRealizado = totalKMRealizado,
                    ValorFranquia = totalFranquia,
                    ValorFranquiaPorKm = valorFranquiaPorKm,
                    ValorTotalCargas = valorTotalPagoTabelaMes,
                    ValorDiferencaTotalCargas = (valorDiferencaTotalCargas > 0) ? valorDiferencaTotalCargas : 0m,
                    ValorTotalPorFaixaKm = Math.Round(totalKMRealizado * valorFranquiaPorKm, 2, MidpointRounding.AwayFromZero)
                },
                SumarizadoFechamentoMensal = new
                {
                    TotalJaPagoTabela = valorTotalPagoTabelaMes,
                    TotalJaPagoTabelaSemana1 = totalJaPagoTabelaSemana1,
                    TotalJaPagoTabelaSemana2 = totalJaPagoTabelaSemana2,
                    TotalJaPagoTabelaSemana3 = totalJaPagoTabelaSemana3,
                    TotalJaPagoTabelaSemana4 = totalJaPagoTabelaSemana4,
                    TotalJaPagoTabelaDezena1 = totalJaPagoTabelaDezena1,
                    TotalJaPagoTabelaDezena2 = totalJaPagoTabelaDezena2,
                    TotalJaPagoTabelaDezena3 = totalJaPagoTabelaDezena3,
                    TotalJaPagoTabelaQuinzena1 = totalJaPagoTabelaQuinzena1,
                    TotalJaPagoTabelaQuinzena2 = totalJaPagoTabelaQuinzena2,
                    TotalFranquia = totalFranquia,
                    FranquiaExcedente = valorExcedenteMes,
                    FixoDiaria = contrato.ValorDiariaPorVeiculo,
                    TotalVeiculos = veiculosUtilizados.Count,
                    TotalDias = totalDias,
                    TotalFixoDiaria = Math.Round(totalDias * contrato.ValorDiariaPorVeiculo * veiculosUtilizados.Count, 2, MidpointRounding.AwayFromZero),
                },
                SumarizadoFechamentoPeriodo = new
                {
                    TotalJaPagoTabela = valorTotalPagoTabelaPeriodo,
                    TotalFranquia = totalFranquiaPeriodo,
                    FixoDiaria = contrato.ValorDiariaPorVeiculo,
                },
                MotoristasUtilizados = motoristasUtilizados,
                VeiculosUtilizados = veiculosUtilizados
            };
        }

        public Dominio.ObjetosDeValor.Embarcador.Fechamento.Sumarizado ObterDadosSumarizadosRelatorio(int codigoFechamento, int codigoContrato, DateTime dataInicio, DateTime dataFim)
        {
            Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork).BuscarPorCodigo(codigoContrato);
            Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete = codigoFechamento > 0 ? new Repositorio.Embarcador.Fechamento.FechamentoFrete(_unitOfWork).BuscarPorCodigo(codigoFechamento) : null;
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = ObterCargasPeriodo(codigoFechamento, codigoContrato, dataInicio, dataFim);
            decimal valorExcedenteMes = ObterValorExcedentePorContratoFreteMes(codigoContrato, dataFim);
            decimal valorExcedentePeriodo = ObterValorExcedentePorContratoFretePeriodo(codigoContrato, dataInicio, dataFim);
            decimal valorTotalPagoTabelaMes = ObterValorPagoPorContratoFreteMes(codigoContrato, dataFim);
            decimal valorTotalPagoTabelaPeriodo = ObterValorPagoPorContratoFretePeriodo(codigoContrato, dataInicio, dataFim);
            int totalKMRealizado = (int)ObterDistanciaTotalPorContratoFrete(codigoContrato, dataFim);
            int totalKMExcedido = (int)ObterDistanciaExcedentePorContratoFrete(codigoContrato, dataFim);

            return new Dominio.ObjetosDeValor.Embarcador.Fechamento.Sumarizado()
            {
                SumarizadoViagensRealizadas = new Dominio.ObjetosDeValor.Embarcador.Fechamento.SumViagensRealizadas()
                {
                    TotalViagens = cargas.Count,
                    ValorTotalPagoTabela = valorTotalPagoTabelaPeriodo,
                    AdicionalKM = valorExcedentePeriodo
                },
                SumarizadoFranquia = new Dominio.ObjetosDeValor.Embarcador.Fechamento.SumFranquia()
                {
                    TotalKMFranquia = ObterTotalKmFranquia(contrato, fechamentoFrete),
                    TotalKMRealizado = totalKMRealizado,
                    TotalKMExcedido = totalKMExcedido,
                    ValorKMFranquia = ObterValorFranquiaPorKm(contrato, fechamentoFrete, dataFim),
                    ValorKMExcedido = ObterValorFranquiaPorKmExcedente(contrato, fechamentoFrete),
                },
                SumarizadoFechamento = new Dominio.ObjetosDeValor.Embarcador.Fechamento.SumFechamento()
                {
                    TotalJaPagoTabela = valorTotalPagoTabelaMes,
                    TotalAcordado = ObterValorFranquia(contrato, fechamentoFrete),
                    TotalFranquia = ObterValorFranquia(contrato, fechamentoFrete),
                    FranquiaExcedente = valorExcedenteMes
                }
            };
        }

        public Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador periodoAcordo, DateTime dataReferencia)
        {
            DateTime dataBase = new DateTime(dataReferencia.Year, dataReferencia.Month, day: 1);

            if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Semanal)
            {
                if (dataReferencia <= dataBase.AddDays(6))
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase, DataFim = dataBase.AddDays(6), Periodo = 0 };

                if (dataReferencia < dataBase.AddDays(13))
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(7), DataFim = dataBase.AddDays(13), Periodo = 1 };

                if (dataReferencia < dataBase.AddDays(20))
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(14), DataFim = dataBase.AddDays(20), Periodo = 2 };

                return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(21), DataFim = dataBase.AddMonths(1).AddDays(-1), Periodo = 3 };
            }

            if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Decendial)
            {
                if (dataReferencia <= dataBase.AddDays(9))
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase, DataFim = dataBase.AddDays(9), Periodo = 0 };

                if (dataReferencia < dataBase.AddDays(19))
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(10), DataFim = dataBase.AddDays(19), Periodo = 1 };

                return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(20), DataFim = dataBase.AddMonths(1).AddDays(-1), Periodo = 2 };
            }

            if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Quinzenal)
            {
                if (dataReferencia < dataBase.AddDays(14))
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase, DataFim = dataBase.AddDays(14), Periodo = 0 };

                return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(15), DataFim = dataBase.AddMonths(1).AddDays(-1), Periodo = 1 };
            }

            return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase, DataFim = dataBase.AddMonths(1).AddDays(-1), Periodo = 0 };
        }

        public Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador periodoAcordo, DateTime dataReferencia, int periodo)
        {
            DateTime dataBase = new DateTime(dataReferencia.Year, dataReferencia.Month, day: 1);

            if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Semanal)
            {
                if (periodo == 0)
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase, DataFim = dataBase.AddDays(6), Periodo = 0 };

                if (periodo == 1)
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(7), DataFim = dataBase.AddDays(13), Periodo = 1 };

                if (periodo == 2)
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(14), DataFim = dataBase.AddDays(20), Periodo = 2 };

                return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(21), DataFim = dataBase.AddMonths(1).AddDays(-1), Periodo = 3 };
            }

            if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Decendial)
            {
                if (periodo == 0)
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase, DataFim = dataBase.AddDays(9), Periodo = 0 };

                if (periodo == 1)
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(10), DataFim = dataBase.AddDays(19), Periodo = 1 };

                return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(20), DataFim = dataBase.AddMonths(1).AddDays(-1), Periodo = 2 };
            }

            if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Quinzenal)
            {
                if (periodo == 0)
                    return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase, DataFim = dataBase.AddDays(14), Periodo = 0 };

                return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase.AddDays(15), DataFim = dataBase.AddMonths(1).AddDays(-1), Periodo = 1 };
            }

            return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo() { DataInicio = dataBase, DataFim = dataBase.AddMonths(1).AddDays(-1), Periodo = 0 };
        }

        public decimal ObterValorPagarPorContratoFretePeriodo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, DateTime dataInicio, DateTime dataFim)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            decimal valorTotalPago = ObterValorPagoPorContratoFretePeriodo(contrato.Codigo, dataInicio, dataFim);

            if (configuracaoEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorKm)
                return valorTotalPago;

            int totalKMRealizado = (int)ObterDistanciaTotalPorContratoFrete(contrato.Codigo, dataFim);
            decimal valorDiferencaTotalCargas = (contrato.ValorMensal - valorTotalPago);

            if (valorDiferencaTotalCargas < 0m)
                valorDiferencaTotalCargas = 0m;

            decimal valorPorKm = (
                from o in contrato.FaixasKmFranquia
                where o.QuilometragemInicial <= totalKMRealizado && o.QuilometragemFinal >= totalKMRealizado
                select o.ValorPorQuilometro
            ).FirstOrDefault();

            return valorDiferencaTotalCargas + (totalKMRealizado * valorPorKm);
        }

        #endregion
    }
}
