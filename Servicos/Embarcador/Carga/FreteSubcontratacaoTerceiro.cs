using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Carga
{
    public class FreteSubcontratacaoTerceiro : ServicoBase
    {        
        public FreteSubcontratacaoTerceiro(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public void AtualizarValorComponentesObrigatoriosParaTerceiro(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            if (contratoFrete.ConfiguracaoCIOT == null || !contratoFrete.ConfiguracaoCIOT.ValorPedagioRetornadoIntegradora)
            {
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                contratoFrete.ValorPedagio = repCargaComponenteFrete.BuscarTotalCargaPorCompomente(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, null, false);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacao ObterValorSubContratacao(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
        {
            if (contratoFrete == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacao freteSubContratacao = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacao
            {
                TipoTabelaFrete = contratoFrete.TipoFreteEscolhido,
                ValorFreteSubcontratacao = contratoFrete.ValorFreteSubcontratacao,
                ValorPedagio = contratoFrete.ValorPedagio,
                ObservacaoManual = contratoFrete.Observacao,
                ValorFreteSubContratacaoTabelaDeFrete = contratoFrete.ValorFreteSubContratacaoTabelaFrete,
                PercentualAdiantamento = contratoFrete.PercentualAdiantamento,
                PercentualAbastecimento = contratoFrete.PercentualAbastecimento,
                PercentualSaldo = 100 - contratoFrete.PercentualAdiantamento - contratoFrete.PercentualAbastecimento,
                PercentualDescontoTerceiro = "(" + contratoFrete.PercentualCobradoDoTerceiro.ToString("n2") + " % de desconto)",
                ValorAdiantamento = contratoFrete.ValorAdiantamento,
                ValorSaldo = contratoFrete.SaldoAReceber,
                Desconto = contratoFrete.Descontos,
                ValorAbastecimento = contratoFrete.ValorAbastecimento,
                DiasVencimentoAdiantamento = contratoFrete.DiasVencimentoAdiantamento,
                DiasVencimentoSaldo = contratoFrete.DiasVencimentoSaldo,
                DataVencimentoAdiantamento = DateTime.Now.AddDays(contratoFrete.DiasVencimentoAdiantamento).ToString("dd/MM/yyyy"),
                DataVencimentoSaldo = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(contratoFrete).ToString("dd/MM/yyyy"),
                ValoresAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacaoValorAdicional>(),
                ValorTotalPrestacao = contratoFrete.ValorLiquidoSemAdiantamento,
                ValorTotalDescontos = contratoFrete.ValoresAdicionais?.Where(o => o.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal && o.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Sum(o => o.Valor) ?? 0m,
                ValorTotalAcrescimos = contratoFrete.ValoresAdicionais?.Where(o => o.AplicacaoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoValorJustificativaContratoFrete.NoTotal && o.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Sum(o => o.Valor) ?? 0m,
                IntegrouValoresAcrescimoDesconto = contratoFrete.IntegrouValoresAcrescimoDesconto,
                PermiteAlterarValor = true
            };

            if (contratoFrete.TabelaFreteCliente != null)
            {
                freteSubContratacao.TabelaFreteCliente = $"{(!string.IsNullOrWhiteSpace(contratoFrete.TabelaFreteCliente.CodigoIntegracao) ? contratoFrete.TabelaFreteCliente.CodigoIntegracao + " - " : string.Empty)}{contratoFrete.TabelaFreteCliente.DescricaoOrigem} até {contratoFrete.TabelaFreteCliente.DescricaoDestino}";
                freteSubContratacao.TabelaFrete = contratoFrete.TabelaFreteCliente.TabelaFrete?.Descricao;
                freteSubContratacao.PermiteAlterarValor = contratoFrete.TabelaFreteCliente.TabelaFrete?.PermiteAlterarValor ?? false;
            }

            if (contratoFrete.ValoresAdicionais?.Count > 0)
            {
                freteSubContratacao.ValoresAdicionais = contratoFrete.ValoresAdicionais.Select(valor => new Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacaoValorAdicional()
                {
                    Descricao = valor.Justificativa.Descricao,
                    Valor = valor.Valor.ToString("n2")
                }).ToList();
            }

            return freteSubContratacao;
        }

        public void ObterPercentuaisTerceiro(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente transportadorTerceiro, ref List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto> valoresAdicionaisContrato, Repositorio.UnitOfWork unitOfWork, out decimal percentualDesconto, out decimal percentualAdiantamentoFretesTerceiro, out decimal percentualAbastecimentoFretesTerceiro, bool valorTerceitoDoPedido, bool contemAlgumaConfiguracaoTerceiro, Dominio.ObjetosDeValor.Embarcador.Terceiros.BuscarConfiguracaoContratoFreteTerceiro buscarConfiguracaoContratoFreteTerceiro)
        {
            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoa = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacao repTabelaFreteClienteSubContratacao = new Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral = new Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral(unitOfWork);

            percentualDesconto = 0m;
            percentualAdiantamentoFretesTerceiro = buscarConfiguracaoContratoFreteTerceiro.PercentualAdiantamentoFreteTerceiro;
            percentualAbastecimentoFretesTerceiro = buscarConfiguracaoContratoFreteTerceiro.PercentualAbastecimentoFreteTerceiro;

            bool percentualDescontoUtilizado = false;
            if (carga.TabelaFrete != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaFreteCliente = repCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, false);

                if (cargaTabelaFreteCliente != null)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao tabelaFreteClienteSubContratacao = repTabelaFreteClienteSubContratacao.BuscarPorTabelaETerceiro(cargaTabelaFreteCliente.TabelaFreteCliente.Codigo, transportadorTerceiro.CPF_CNPJ);
                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral> tabelaFreteClienteSubContratacaoAcrescimoDescontoGeral = repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.BuscarPorTabelaFrete(cargaTabelaFreteCliente.TabelaFreteCliente.Codigo);

                    if (tabelaFreteClienteSubContratacao != null)
                    {
                        valoresAdicionaisContrato = tabelaFreteClienteSubContratacao.Valores.ToList();
                        percentualDesconto = tabelaFreteClienteSubContratacao.PercentualDesconto;
                        percentualDescontoUtilizado = true;
                    }

                    if (tabelaFreteClienteSubContratacaoAcrescimoDescontoGeral != null && tabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.Count > 0)
                    {
                        if (valoresAdicionaisContrato == null)
                        {
                            valoresAdicionaisContrato = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto>();
                        }

                        foreach (var tabFreteClienteGeral in tabelaFreteClienteSubContratacaoAcrescimoDescontoGeral)
                        {
                            valoresAdicionaisContrato.Add
                                    (
                                        new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto()
                                        {
                                            Valor = tabFreteClienteGeral.Valor,
                                            Justificativa = tabFreteClienteGeral.Justificativa
                                        }
                                    );
                        }

                        if (valoresAdicionaisContrato.Count() == 0)
                        {
                            valoresAdicionaisContrato = null;
                        }
                    }

                    if (!percentualDescontoUtilizado && cargaTabelaFreteCliente.TabelaFreteCliente.PercentualCobrancaPadraoTerceiros > 0m)
                    {
                        percentualDesconto = cargaTabelaFreteCliente.TabelaFreteCliente.PercentualCobrancaPadraoTerceiros;
                        percentualDescontoUtilizado = true;
                    }
                }

                if (!percentualDescontoUtilizado)
                {
                    Repositorio.Embarcador.Frete.SubcontratacaoTabelaFrete repSubcontratacaoTabelaFrete = new Repositorio.Embarcador.Frete.SubcontratacaoTabelaFrete(unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete tabelaFreteSubcontratacao = repSubcontratacaoTabelaFrete.BuscarPorTabelaETerceiro(carga.TabelaFrete.Codigo, transportadorTerceiro.CPF_CNPJ);

                    if (tabelaFreteSubcontratacao != null)
                    {
                        percentualDesconto = tabelaFreteSubcontratacao.PercentualDesconto;
                        percentualDescontoUtilizado = true;
                    }
                    if (!percentualDescontoUtilizado && carga.TabelaFrete.PercentualCobrancaPadraoTerceiros > 0m)
                    {
                        percentualDesconto = carga.TabelaFrete.PercentualCobrancaPadraoTerceiros;
                        percentualDescontoUtilizado = true;
                    }
                }
            }

            if (carga.TipoOperacao != null && (carga.TipoOperacao?.UtilizarConfiguracaoTerceiro ?? false))
            {
                if (!percentualDescontoUtilizado && (carga.TipoOperacao?.PercentualCobrancaPadraoTerceiros ?? 0) > 0)
                {
                    percentualDesconto = carga.TipoOperacao?.PercentualCobrancaPadraoTerceiros ?? 0m;
                    percentualDescontoUtilizado = true;
                }
            }

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoa = repModalidadePessoa.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, transportadorTerceiro.Codigo);

            if (modalidadePessoa != null && ((carga.TipoOperacao?.UtilizarConfiguracaoTerceiroComoPadrao ?? false) || !(carga.TipoOperacao?.UtilizarConfiguracaoTerceiro ?? false)))
            {
                if (contemAlgumaConfiguracaoTerceiro)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadorPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoa.Codigo);
                    percentualDesconto = modalidadeTransportadorPessoas.PercentualDesconto;
                    percentualDescontoUtilizado = true;
                }
            }

            if (valorTerceitoDoPedido)
                percentualDesconto = 0m;
        }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFrete CalcularFreteSubcontratacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido tipoFreteEscolhido, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao repContratoFreteValorPadrao = new Repositorio.Embarcador.Terceiros.ContratoFreteValorPadrao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAcrescimoDesconto repPedidoAcrescimoDesconto = new Repositorio.Embarcador.Pedidos.PedidoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);

            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorCarga(carga.Codigo);

            bool gerarCIOTParaTodos = Servicos.Embarcador.CIOT.CIOT.VerificarSeGeraCIOTParaTodos(carga, configuracaoTMS, unitOfWork);
            bool valorTerceitoDoPedido = false;

            if (carga.FreteDeTerceiro || gerarCIOTParaTodos)
            {
                if (!apenasVerificar)
                {
                    if (carga.Veiculo == null && carga.ProvedorOS == null)
                        return null;

                    if (!gerarCIOTParaTodos && carga.Veiculo?.Proprietario == null && carga.ProvedorOS == null)
                        return null;

                    if (gerarCIOTParaTodos && !carga.FreteDeTerceiro)
                    {
                        if (carga.Empresa == null)
                            return null;

                        carga.Terceiro = SalvarEmpresaCompTransportadorTerceiro(carga.Empresa, configuracaoTMS.TipoPagamentoCIOT, unitOfWork);
                    }
                    else
                    {
                        if (carga.ProvedorOS != null)
                            carga.Terceiro = carga.ProvedorOS;
                        else
                            carga.Terceiro = carga.Veiculo?.Proprietario;
                    }

                    repCarga.Atualizar(carga);

                    if (carga.TipoOperacao?.NaoGerarContratoFreteTerceiro ?? false)
                        return null;

                    if (configuracaoTMS.TipoContratoFreteTerceiro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoFreteTerceiro.PorPagamentoAgregado)
                        return null;

                    if (repCargaPedido.ExisteCTeEmitidoNoEmbarcadorPorCarga(carga.Codigo) && configuracaoTMS.NaoGerarContratoFreteParaCTeEmitidoNoEmbarcador)
                        return null;

                    if (carga.Terceiro == null)
                        return null;

                    if (carga.Terceiro.Modalidades == null)
                        return null;

                    Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoa = carga.Terceiro.Modalidades.Where(o => o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = modalidadePessoa != null ? repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoa.Codigo) : null;

                    if (gerarCIOTParaTodos && modalidadeTerceiro == null)
                        return null;

                    bool inserir = false;
                    if (contratoFrete == null)
                    {
                        inserir = true;
                        contratoFrete = new Dominio.Entidades.Embarcador.Terceiros.ContratoFrete
                        {
                            Carga = carga,
                            NumeroContrato = repContratoFreteTerceiro.BuscarProximoCodigo(),
                            SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto,
                        };
                    }

                    contratoFrete.TipoTerceiro = modalidadeTerceiro?.TipoTerceiro ?? null;
                    contratoFrete.TabelaFreteCliente = null;
                    contratoFrete.CalcularAdiantamentoComPedagio = configuracaoContratoFreteTerceiro.CalcularAdiantamentoComPedagio;
                    contratoFrete.NaoConsiderarDescontoCalculoImpostos = configuracaoContratoFreteTerceiro.NaoConsiderarDescontoCalculoImpostos;
                    contratoFrete.UtilizarDataEmissaoParaMovimentoFinanceiro = configuracaoTMS.UtilizarDataEmissaoContratoParaMovimentoFinanceiro;
                    contratoFrete.TipoGeracaoTitulo = configuracaoTMS.TipoGeracaoTituloContratoFrete;
                    contratoFrete.TipoFreteEscolhido = tipoFreteEscolhido;
                    contratoFrete.TransportadorTerceiro = carga.Terceiro;
                    contratoFrete.TipoContratoCIOT = carga.FreteDeTerceiro ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoCIOT.Terceiro : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoCIOT.Proprio;
                    contratoFrete.ValorPedagio = 0m;
                    contratoFrete.DataEmissaoContrato = DateTime.Now;

                    if (carga.Operador != null)
                        contratoFrete.Usuario = carga.Operador;

                    if (!contratoFrete.AlterouObservacaoManualmente)
                        contratoFrete.Observacao = configuracaoContratoFreteTerceiro.Observacao;

                    contratoFrete.DiasVencimentoAdiantamento = 0;
                    contratoFrete.DiasVencimentoSaldo = 0;
                    contratoFrete.TextoAdicionalContratoFrete = configuracaoContratoFreteTerceiro.TextoAdicional;
                    contratoFrete.ReterImpostosContratoFrete = configuracaoContratoFreteTerceiro.ReterImpostos;

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete tipoPagamentoContratoFrete = configuracaoTMS.TipoPagamentoContratoFrete;
                    bool naoSomarValorPedagio = false;

                    if (carga.TipoOperacao != null && carga.TipoOperacao.UtilizarConfiguracaoTerceiro && !carga.TipoOperacao.UtilizarConfiguracaoTerceiroComoPadrao)
                    {
                        if (carga.TipoOperacao.TipoPagamentoContratoFreteTerceiro.HasValue)
                            tipoPagamentoContratoFrete = carga.TipoOperacao.TipoPagamentoContratoFreteTerceiro.Value;

                        if (carga.TipoOperacao.ConfiguracaoTerceiro?.NaoSomarValorPedagioContratoFrete.HasValue ?? false)
                            naoSomarValorPedagio = carga.TipoOperacao.ConfiguracaoTerceiro.NaoSomarValorPedagioContratoFrete.Value;
                    }

                    if (modalidadeTerceiro != null)
                    {
                        if (modalidadeTerceiro.TipoPagamentoContratoFreteTerceiro.HasValue)
                            tipoPagamentoContratoFrete = modalidadeTerceiro.TipoPagamentoContratoFreteTerceiro.Value;

                        if (modalidadeTerceiro.NaoSomarValorPedagioContratoFrete.HasValue)
                            naoSomarValorPedagio = modalidadeTerceiro.NaoSomarValorPedagioContratoFrete.Value;
                    }

                    if (carga.TipoOperacao != null && carga.TipoOperacao.UtilizarConfiguracaoTerceiro && !carga.TipoOperacao.UtilizarConfiguracaoTerceiroComoPadrao)
                    {
                        if (carga.TipoOperacao.TipoPagamentoContratoFreteTerceiro.HasValue)
                            tipoPagamentoContratoFrete = carga.TipoOperacao.TipoPagamentoContratoFreteTerceiro.Value;

                        if (carga.TipoOperacao.ConfiguracaoTerceiro?.NaoSomarValorPedagioContratoFrete.HasValue ?? false)
                            naoSomarValorPedagio = carga.TipoOperacao.ConfiguracaoTerceiro.NaoSomarValorPedagioContratoFrete.Value;
                    }

                    contratoFrete.NaoSomarValorPedagio = naoSomarValorPedagio;
                    contratoFrete.TipoPagamentoContratoFreteTerceiro = tipoPagamentoContratoFrete;

                    Servicos.Log.TratarErro($"3 - Carga: '{carga.Codigo}' -> Emissao Contingencia: {carga.ContingenciaEmissao}", "EmissaoContingencia");
                    if (modalidadeTerceiro != null && modalidadeTerceiro.GerarCIOT && !(carga.Empresa?.EmissaoDocumentosForaDoSistema ?? false) || (carga.TipoOperacao?.EmissaoDocumentosForaDoSistema ?? false) || carga.ContingenciaEmissao)
                    {
                        if (carga.Filial?.ConfiguracaoCIOT != null)
                            contratoFrete.ConfiguracaoCIOT = carga.Filial.ConfiguracaoCIOT;

                        else if (modalidadeTerceiro?.ConfiguracaoCIOTWithTipoTerceiro != null &&
                                (
                                    tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    (
                                        tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                                        !(carga.TipoOperacao?.ConfiguracaoTerceiro?.NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro ?? false)
                                    )
                                )
                            )
                            contratoFrete.ConfiguracaoCIOT = modalidadeTerceiro.ConfiguracaoCIOTWithTipoTerceiro;

                        else if (carga.Empresa?.ConfiguracaoCIOT != null)
                            contratoFrete.ConfiguracaoCIOT = carga.Empresa?.ConfiguracaoCIOT;
                        else if (carga.TipoOperacao?.ConfiguracaoCIOT != null)
                            contratoFrete.ConfiguracaoCIOT = carga.TipoOperacao?.ConfiguracaoCIOT;
                        else
                            contratoFrete.ConfiguracaoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterConfiguracaoCIOT(contratoFrete.TransportadorTerceiro, carga.Empresa, unitOfWork);
                    }

                    if (contratoFrete.ConfiguracaoCIOT?.OperadoraCIOT != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, contratoFrete.ConfiguracaoCIOT.OperadoraCIOT);
                        contratoFrete.TipoPagamentoCIOT = tipoPagamentoCIOT;
                    }

                    decimal valorFreteTransportadorTerceiroPedidos = repCargaPedido.BuscarValorFreteTransportadorTerceiroPorCarga(carga.Codigo);
                    List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesCalculoFrete = null;
                    Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = null;

                    if (carga.Integracoes?.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercement) ?? false && valorFreteTransportadorTerceiroPedidos <= 0)
                    {
                        contratoFrete.ValorFreteSubcontratacao = carga.ValorFreteLiquido;

                        decimal aliquotaImposto = repCargaPedido.BuscarAliquotaICMSPorCarga(carga.Codigo);
                        aliquotaImposto += carga.Empresa?.Configuracao?.AliquotaPIS ?? 0m;
                        aliquotaImposto += carga.Empresa?.Configuracao?.AliquotaCOFINS ?? 0m;

                        if (aliquotaImposto > 0m)
                            contratoFrete.ValorFreteSubcontratacao = Math.Round(contratoFrete.ValorFreteSubcontratacao / (1 - (aliquotaImposto / 100)), 2, MidpointRounding.ToEven);
                    }
                    else if (tipoPagamentoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete.SobreFreteCarga && valorFreteTransportadorTerceiroPedidos <= 0)
                    {
                        contratoFrete.ValorFreteSubcontratacao = carga.ValorFreteLiquido;

                        AtualizarValorComponentesObrigatoriosParaTerceiro(carga, ref contratoFrete, unitOfWork);
                    }
                    else
                    {
                        valorFreteTransportadorTerceiroPedidos = 0m;
                        if (configuracaoTMS.SolicitarValorFretePorTonelada)
                        {
                            valorFreteTransportadorTerceiroPedidos = repCargaPedido.BuscarValorFreteToneladaTerceiroPorCarga(carga.Codigo);
                            if (valorFreteTransportadorTerceiroPedidos > 0)
                            {
                                decimal pesoNotas = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(carga.Codigo);
                                if (pesoNotas > 0)
                                    valorFreteTransportadorTerceiroPedidos = (valorFreteTransportadorTerceiroPedidos * (pesoNotas / 1000));
                            }
                        }
                        else
                        {
                            valorFreteTransportadorTerceiroPedidos = repCargaPedido.BuscarValorFreteTransportadorTerceiroPorCarga(carga.Codigo);
                            if (valorFreteTransportadorTerceiroPedidos > 0m)
                                valorTerceitoDoPedido = true;
                        }

                        if (valorFreteTransportadorTerceiroPedidos > 0m)
                            contratoFrete.ValorFreteSubcontratacao = valorFreteTransportadorTerceiroPedidos;
                        else
                            SetarDadosContratoFretePelaTabelaFrete(ref contratoFrete, ref dadosCalculoFrete, unitOfWork, tipoServicoMultisoftware, stringConexao, configuracaoTMS, out componentesCalculoFrete);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Terceiros.BuscarConfiguracaoContratoFreteTerceiro buscarConfiguracaoContratoFreteTerceiro = BuscarConfiguracaoContratoFreteTerceiro(configuracaoContratoFreteTerceiro, modalidadeTerceiro, carga.TipoOperacao, dadosCalculoFrete, carga.Veiculo);
                    contratoFrete.DiasVencimentoAdiantamento = buscarConfiguracaoContratoFreteTerceiro.DiasVencimentoAdiantamento;
                    contratoFrete.DiasVencimentoSaldo = buscarConfiguracaoContratoFreteTerceiro.DiasVencimentoSaldo;
                    contratoFrete.DataFixaVencimentoSaldo = buscarConfiguracaoContratoFreteTerceiro.DataFixaVencimentoSaldo;

                    bool contemAlgumaConfiguracaoTerceiro = false;
                    if (modalidadeTerceiro != null && ((carga.TipoOperacao?.UtilizarConfiguracaoTerceiroComoPadrao ?? false) || !(carga.TipoOperacao?.UtilizarConfiguracaoTerceiro ?? false)))
                    {

                        if ((modalidadeTerceiro.DiasVencimentoAdiantamentoContratoFrete.HasValue && modalidadeTerceiro.DiasVencimentoAdiantamentoContratoFrete.Value > 0) ||
                            (modalidadeTerceiro.DiasVencimentoSaldoContratoFrete.HasValue && modalidadeTerceiro.DiasVencimentoSaldoContratoFrete.Value > 0) ||
                            modalidadeTerceiro.PercentualDesconto > 0 || modalidadeTerceiro.PercentualAbastecimentoFretesTerceiro > 0 || modalidadeTerceiro.PercentualAdiantamentoFretesTerceiro > 0)
                            contemAlgumaConfiguracaoTerceiro = true;

                        if (!string.IsNullOrWhiteSpace(modalidadeTerceiro.ObservacaoContratoFrete) && !contratoFrete.AlterouObservacaoManualmente)
                            contratoFrete.Observacao += modalidadeTerceiro.ObservacaoContratoFrete;

                        if (!string.IsNullOrWhiteSpace(modalidadeTerceiro.TextoAdicionalContratoFrete))
                            contratoFrete.TextoAdicionalContratoFrete += modalidadeTerceiro.TextoAdicionalContratoFrete;

                        if (modalidadeTerceiro.ReterImpostosContratoFreteWithTipoTerceiro.HasValue)
                            contratoFrete.ReterImpostosContratoFrete = modalidadeTerceiro.ReterImpostosContratoFreteWithTipoTerceiro.Value;
                    }
                    else if (modalidadeTerceiro != null)
                    {
                        if (!string.IsNullOrWhiteSpace(modalidadeTerceiro.ObservacaoContratoFrete) && !contratoFrete.AlterouObservacaoManualmente)
                            contratoFrete.Observacao += modalidadeTerceiro.ObservacaoContratoFrete;

                        if (!string.IsNullOrWhiteSpace(modalidadeTerceiro.TextoAdicionalContratoFrete))
                            contratoFrete.TextoAdicionalContratoFrete += modalidadeTerceiro.TextoAdicionalContratoFrete;

                        if (modalidadeTerceiro.ReterImpostosContratoFreteWithTipoTerceiro.HasValue)
                            contratoFrete.ReterImpostosContratoFrete = modalidadeTerceiro.ReterImpostosContratoFreteWithTipoTerceiro.Value;
                    }

                    decimal percentualAdiantamentoFretesTerceiro = 0m, percentualAbastecimentoFretesTerceiro = 0m, percentualDesconto = 0m;

                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto> valoresAdicionaisContrato = null;

                    ObterPercentuaisTerceiro(carga, contratoFrete.TransportadorTerceiro, ref valoresAdicionaisContrato, unitOfWork, out percentualDesconto, out percentualAdiantamentoFretesTerceiro, out percentualAbastecimentoFretesTerceiro, valorTerceitoDoPedido, contemAlgumaConfiguracaoTerceiro, buscarConfiguracaoContratoFreteTerceiro);

                    if (percentualDesconto == 0m && configuracaoTMS.PercentualCIOTDescontadoTerceiros > 0m)
                        percentualDesconto = configuracaoTMS.PercentualCIOTDescontadoTerceiros;

                    List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao> valoresPadroes = repContratoFreteValorPadrao.BuscarAtivos(contratoFrete.TransportadorTerceiro.CPF_CNPJ);

                    contratoFrete.TarifaSaque = 0m;
                    contratoFrete.TarifaTransferencia = 0m;

                    Servicos.Log.TratarErro($"4 - Carga: '{carga.Codigo}' -> Emissao Contingencia: {carga.ContingenciaEmissao}", "EmissaoContingencia");
                    if ((modalidadeTerceiro?.GerarCIOT ?? false) && !(carga.Empresa?.EmissaoDocumentosForaDoSistema ?? false) || (carga.TipoOperacao?.EmissaoDocumentosForaDoSistema ?? false) || carga.ContingenciaEmissao)
                    {
                        Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

                        if (contratoFrete.ConfiguracaoCIOT != null && (contratoFrete.ConfiguracaoCIOT.TarifaSaque > 0m || contratoFrete.ConfiguracaoCIOT.TarifaTransferencia > 0m))
                        {
                            bool existeCIOTAberto = repCIOT.ExisteCIOTAbertoComTarifaJaInclusa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);

                            if (!existeCIOTAberto)
                            {
                                contratoFrete.TarifaSaque = contratoFrete.ConfiguracaoCIOT.TarifaSaque;
                                contratoFrete.TarifaTransferencia = contratoFrete.ConfiguracaoCIOT.TarifaTransferencia;
                            }
                        }
                    }

                    contratoFrete.ValorFreteSubcontratacao -= Math.Round(contratoFrete.ValorFreteSubcontratacao * (percentualDesconto / 100), 2, MidpointRounding.ToEven);
                    contratoFrete.ValorFreteSubContratacaoTabelaFrete = contratoFrete.ValorFreteSubcontratacao;
                    contratoFrete.PercentualCobradoDoTerceiro = percentualDesconto;
                    contratoFrete.PercentualAdiantamento = percentualAdiantamentoFretesTerceiro;
                    contratoFrete.PercentualAbastecimento = percentualAbastecimentoFretesTerceiro;

                    if (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.InformarValorFreteTerceiroManualmente ?? false)
                    {
                        contratoFrete.ValorFreteSubcontratacao = 0;
                        contratoFrete.ValorFreteSubContratacaoTabelaFrete = 0;
                    }

                    if (inserir)
                        repContratoFreteTerceiro.Inserir(contratoFrete);
                    else
                        repContratoFreteTerceiro.Atualizar(contratoFrete);

                    List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> valores = repContratoFreteValor.BuscarPorContratoFrete(contratoFrete.Codigo);

                    AplicarAcrescimoDescontoPendenciaContratoFretePassado(contratoFrete, valores, unitOfWork, tipoServicoMultisoftware);

                    foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor valor in valores)
                        repContratoFreteValor.Deletar(valor);

                    if (valoresAdicionaisContrato != null && valoresAdicionaisContrato.Count > 0)
                    {
                        for (int i = 0; i < valoresAdicionaisContrato.Count; i++)
                        {
                            if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, valoresAdicionaisContrato[i].Valor, valoresAdicionaisContrato[i].Justificativa.Codigo, null, 0, null))
                                throw new Exception(erro);
                        }
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto> acrescimosDescontosPedido = repPedidoAcrescimoDesconto.BuscarPorCarga(carga.Codigo);

                    decimal valorAcrescimosPedido = acrescimosDescontosPedido.Where(o => o.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo).Sum(o => o.Valor);
                    decimal valorDescontosPedido = acrescimosDescontosPedido.Where(o => o.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto).Sum(o => o.Valor);
                    //if (0 > valorAcrescimosPedido - valorDescontosPedido)
                    //{
                    //    throw new Exception("Valor dos acréscimos e descontos não podem gerar um valor negativo");
                    //}

                    for (int i = 0, s = acrescimosDescontosPedido.Count; i < s; i++)
                    {
                        if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, acrescimosDescontosPedido[i].Valor, acrescimosDescontosPedido[i].Justificativa.Codigo, null, 0, null))
                            throw new Exception(erro);
                    }

                    if (valoresPadroes != null && valoresPadroes.Count > 0)
                    {
                        for (int i = 0; i < valoresPadroes.Count; i++)
                        {
                            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValorPadrao contratoFreteValorPadrao = valoresPadroes[i];

                            if (contratoFreteValorPadrao.ApenasQuandoEmitirCIOT && !(modalidadeTerceiro?.GerarCIOT ?? false))
                                continue;

                            if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, contratoFreteValorPadrao.Valor, contratoFreteValorPadrao.Justificativa.Codigo, null, 0, null))
                                throw new Exception(erro);
                        }
                    }

                    if (componentesCalculoFrete?.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente componente in componentesCalculoFrete)
                        {
                            if (componente.Justificativa == null)
                                continue;

                            if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, componente.ValorComponente, componente.Justificativa.Codigo, null, 0, null))
                                throw new Exception(erro);
                        }
                    }

                    AplicarAcrescimoDescontoAutomatico(contratoFrete, unitOfWork, tipoServicoMultisoftware);

                    contratoFrete.IntegrouValoresAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork).GerarIntegracaoContratoFreteAcrescimoDesconto(carga, contratoFrete, null, tipoServicoMultisoftware);

                    Servicos.Embarcador.Terceiros.ContratoFrete.CalcularImpostos(ref contratoFrete, unitOfWork, tipoServicoMultisoftware);
                    repContratoFreteTerceiro.Atualizar(contratoFrete);

                    if (carga.Integracoes?.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercement) ?? false)
                    {
                        contratoFrete.ValorFreteSubcontratacao += repCargaComponentesFrete.BuscarValorTotalIncluirIntegralContratoFrete(carga.Codigo);

                        repContratoFreteTerceiro.Atualizar(contratoFrete);
                    }


                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    {
                        bool exigeConfirmacaoFreteAntesEmissao = carga.Filial?.ExigeConfirmacaoFreteAntesEmissao ?? false;
                        if (!exigeConfirmacaoFreteAntesEmissao)
                            exigeConfirmacaoFreteAntesEmissao = carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? true;

                        Servicos.Log.TratarErro($"5 - Carga: '{carga.Codigo}' -> Emissao Contingencia: {carga.ContingenciaEmissao}", "EmissaoContingencia");
                        if (!exigeConfirmacaoFreteAntesEmissao && (modalidadeTerceiro?.GerarCIOT ?? false) && !(carga.Empresa?.EmissaoDocumentosForaDoSistema ?? false) || (carga.TipoOperacao?.EmissaoDocumentosForaDoSistema ?? false) || carga.ContingenciaEmissao)
                        {
                            string retornoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterCIOTCarga(carga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);
                            if (!string.IsNullOrWhiteSpace(retornoCIOT))
                            {
                                carga.DataInicioEmissaoDocumentos = null;
                                carga.DataEnvioUltimaNFe = null;
                                carga.DataInicioGeracaoCTes = null;
                                //carga.MotivoPendencia = retornoCIOT;
                                //carga.PossuiPendencia = true;
                                Servicos.Log.TratarErro("CIOT da carga " + carga.CodigoCargaEmbarcador + " não gerado: " + retornoCIOT, "CIOT");
                            }
                        }
                    }
                }

                return contratoFrete;
            }
            else
            {
                if (carga.Terceiro != null)
                {
                    carga.Terceiro = null;
                    repCarga.Atualizar(carga);
                }

                if (contratoFrete != null)
                    repContratoFreteTerceiro.Deletar(contratoFrete);

                return null;
            }
        }

        public static DateTime ObterVencimentoSaldoContrato(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
        {
            return ObterVencimentoSaldoContrato(contratoFrete, DateTime.Now);
        }

        public static DateTime ObterVencimentoSaldoPessoa(Dominio.Entidades.Cliente cliente, DateTime dataBase)
        {
            List<Dominio.Entidades.Embarcador.Pessoas.PessoaDataFixaVencimento> configuracaoCIOTDataFixaVencimento = cliente.PessoaDataFixaVencimento.ToList();
            if (configuracaoCIOTDataFixaVencimento.Any())
            {
                int mesAtual = dataBase.Month;
                int anoAtual = dataBase.Year;

                DateTime dataInicial;
                DateTime dataFinal;
                int ultimoDiaDoMes = DateTime.DaysInMonth(anoAtual, mesAtual);

                foreach (var item in configuracaoCIOTDataFixaVencimento)
                {
                    dataInicial = new DateTime(anoAtual, mesAtual, Math.Min(item.DiaInicialEmissao, ultimoDiaDoMes));
                    dataFinal = new DateTime(anoAtual, mesAtual, Math.Min(item.DiaFinalEmissao, ultimoDiaDoMes));

                    if (dataInicial > dataFinal)
                    {
                        if (dataInicial < dataBase && dataFinal.AddMonths(1) >= dataBase)
                        {
                            return CalcularDataVencimento(ObterAnoDataFixa(dataBase), dataBase, item.DiaVencimento, ultimoDiaDoMes);
                        }
                        else if (dataInicial.AddMonths(-1) <= dataBase && dataFinal > dataBase)
                        {
                            return CalcularDataVencimento(ObterAnoDataFixa(dataBase), dataBase, item.DiaVencimento, ultimoDiaDoMes);
                        }
                    }
                    else if (dataInicial <= dataBase && dataFinal >= dataBase)
                    {
                        return CalcularDataVencimento(ObterAnoDataFixa(dataBase), dataBase, item.DiaVencimento, ultimoDiaDoMes);
                    }
                }
            }
            return dataBase;
        }

        public static DateTime ObterVencimentoSaldoContrato(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, DateTime dataBase)
        {
            Dominio.Entidades.Cliente cliente = contratoFrete.TransportadorTerceiro;
            if (cliente != null && contratoFrete.DataFixaVencimentoSaldo)
            {
                return ObterVencimentoSaldoPessoa(cliente, dataBase);
            }
            return dataBase.AddDays(contratoFrete?.DiasVencimentoSaldo ?? 0);
        }

        #endregion

        #region Métodos Privados

        private void AplicarAcrescimoDescontoPendenciaContratoFretePassado(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> valores, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (contratoFrete == null || contratoFrete.Carga == null)
                return;

            if (valores != null)
            {
                foreach (var item in valores)
                {
                    Servicos.Embarcador.Terceiros.ContratoFrete.RemoverVinculoPendenciaContratoFrete(item, unitOfWork);
                }
            }
            Repositorio.Embarcador.Terceiros.PendenciaContratoFreteFuturo repPendenciaContratoFreteFuturo = new Repositorio.Embarcador.Terceiros.PendenciaContratoFreteFuturo(unitOfWork);

            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork, tipoServicoMultisoftware);

            List<Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo> listaPendenciaContratoFreteFuturo = repPendenciaContratoFreteFuturo.BuscarPorTerceiroEVeiculo(contratoFrete.TransportadorTerceiro.CPF_CNPJ, contratoFrete.Carga.Veiculo.Codigo);

            if (listaPendenciaContratoFreteFuturo != null && listaPendenciaContratoFreteFuturo.Count > 0)
            {
                foreach (var pendenciaContratoFreteFuturo in listaPendenciaContratoFreteFuturo)
                {
                    if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, pendenciaContratoFreteFuturo.Valor, pendenciaContratoFreteFuturo.Justificativa.Codigo, null, 0, null, false, false, pendenciaContratoFreteFuturo))
                        continue;
                    else
                    {
                        pendenciaContratoFreteFuturo.Ativo = false;
                        pendenciaContratoFreteFuturo.ContratoFreteDestino = contratoFrete;
                        repPendenciaContratoFreteFuturo.Atualizar(pendenciaContratoFreteFuturo);
                    }
                }
            }
        }

        private void AplicarAcrescimoDescontoAutomatico(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteValor repContratoFreteValor = new Repositorio.Embarcador.Terceiros.ContratoFreteValor(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico repClienteContratoFreteAcrescimoDescontoAutomatico = new Repositorio.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico(unitOfWork);

            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Terceiros.ContratoFrete(unitOfWork, tipoServicoMultisoftware);

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor> valores = repContratoFreteValor.BuscarGeradosAutomaticamentePorContratoFrete(contratoFrete.Codigo);
            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFreteValor valor in valores)
                repContratoFreteValor.Deletar(valor);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico> acrescimoDescontoAutomaticos = repClienteContratoFreteAcrescimoDescontoAutomatico.BuscarPorPessoa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);
            if (acrescimoDescontoAutomaticos.Count == 0)
                return;

            int dias = 0;
            if (acrescimoDescontoAutomaticos.Any(o => o.AcrescimoDescontoAutomatico.TipoCalculo == TipoCalculoContratoFreteADA.DiasEntreAgendamentoPrevisaoSaida))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = contratoFrete.Carga.Pedidos.FirstOrDefault();
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                if (pedido.DataPrevisaoSaida.HasValue && pedido.DataAgendamento.HasValue)
                {
                    dias = (pedido.DataAgendamento.Value - pedido.DataPrevisaoSaida.Value).Days;
                    if (dias <= 0)
                        dias = 1;
                }
            }

            foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteContratoFreteAcrescimoDescontoAutomatico clienteAcrescimoDescontoAutomatico in acrescimoDescontoAutomaticos)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico acrescimoDescontoAutomatico = clienteAcrescimoDescontoAutomatico.AcrescimoDescontoAutomatico;
                decimal valor = acrescimoDescontoAutomatico.Valor;

                if (acrescimoDescontoAutomatico.TipoValor == TipoValorContratoFreteADA.Calculado)
                {
                    if (acrescimoDescontoAutomatico.TipoCalculo == TipoCalculoContratoFreteADA.DiasEntreAgendamentoPrevisaoSaida)
                    {
                        if (dias <= 0)
                            continue;

                        valor = dias * valor;
                    }
                }

                if (!servicoContratoFrete.AdicionarValorAoContrato(out string erro, ref contratoFrete, valor, acrescimoDescontoAutomatico.Justificativa.Codigo, acrescimoDescontoAutomatico.Observacoes, 0, null, false, true))
                    throw new Exception(erro);
            }
        }

        private void SetarDadosContratoFretePelaTabelaFrete(ref Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, ref Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, out List<Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente> componentesCalculoFrete)
        {
            componentesCalculoFrete = null;

            Servicos.Embarcador.Carga.Frete svcFrete = new Frete(unitOfWork, tipoServicoMultisoftware);

            dadosCalculoFrete = svcFrete.ObterDadosCalculoFreteParaSubcontratacao(contratoFrete.Carga, unitOfWork, tipoServicoMultisoftware, stringConexao, configuracao);

            if (!dadosCalculoFrete.FreteCalculado)
            {
                contratoFrete.ValorFreteSubcontratacao = 0m;
                return;
            }

            if (dadosCalculoFrete.TabelaFrete.PercentualCobrancaVeiculoFrotaTerceiros > 0 && contratoFrete?.Carga?.Veiculo?.TipoFrota != TipoFrota.NaoDefinido)
                contratoFrete.ValorFreteSubcontratacao = contratoFrete.Carga.ValorFreteLiquido * dadosCalculoFrete.TabelaFrete.PercentualCobrancaVeiculoFrotaTerceiros / 100;
            else if (dadosCalculoFrete.TabelaFrete.PercentualCobrancaPadraoTerceiros > 0)
                contratoFrete.ValorFreteSubcontratacao = contratoFrete.Carga.ValorFreteLiquido * dadosCalculoFrete.TabelaFrete.PercentualCobrancaPadraoTerceiros / 100;
            else if (dadosCalculoFrete.TabelaFreteCliente?.PercentualCobrancaVeiculoFrota > 0 && contratoFrete?.Carga?.Veiculo?.TipoFrota != TipoFrota.NaoDefinido)
                contratoFrete.ValorFreteSubcontratacao = contratoFrete.Carga.ValorFreteLiquido * dadosCalculoFrete.TabelaFreteCliente.PercentualCobrancaVeiculoFrota / 100;
            else if (dadosCalculoFrete.TabelaFreteCliente?.PercentualCobrancaPadraoTerceiros > 0)
                contratoFrete.ValorFreteSubcontratacao = contratoFrete.Carga.ValorFreteLiquido * dadosCalculoFrete.TabelaFreteCliente.PercentualCobrancaPadraoTerceiros / 100;
            else
                contratoFrete.ValorFreteSubcontratacao = dadosCalculoFrete.ValorFrete + dadosCalculoFrete.Componentes.Where(obj => obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO && !obj.DescontarValorTotalAReceber).Sum(o => o.ValorComponente);

            contratoFrete.Descontos = dadosCalculoFrete.Componentes.Where(obj => obj.DescontarValorTotalAReceber).Sum(o => -o.ValorComponente);
            contratoFrete.ValorPedagio = dadosCalculoFrete.Componentes.Where(obj => obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO && !obj.DescontarValorTotalAReceber).Sum(o => o.ValorComponente);

            if (!string.IsNullOrWhiteSpace(dadosCalculoFrete.ObservacaoContratoFrete))
                contratoFrete.Observacao = dadosCalculoFrete.ObservacaoContratoFrete;

            if (!string.IsNullOrWhiteSpace(dadosCalculoFrete.TextoAdicionalContratoFrete))
                contratoFrete.TextoAdicionalContratoFrete = dadosCalculoFrete.TextoAdicionalContratoFrete;

            contratoFrete.ReterImpostosContratoFrete = dadosCalculoFrete.ReterImpostosContratoFrete;
            //contratoFrete.DiasVencimentoAdiantamento = dadosCalculoFrete.DiasVencimentoAdiantamentoContratoFrete;
            //contratoFrete.DiasVencimentoSaldo = dadosCalculoFrete.DiasVencimentoSaldoContratoFrete;
            contratoFrete.TabelaFreteCliente = dadosCalculoFrete.TabelaFreteCliente;

            componentesCalculoFrete = dadosCalculoFrete.Componentes.ToList();
        }

        private Dominio.Entidades.Cliente SalvarEmpresaCompTransportadorTerceiro(Dominio.Entidades.Empresa empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOTPadrao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (!double.TryParse(empresa.CNPJ, out double cpfCnpj) || cpfCnpj <= 0D)
                return null;

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);
            if (cliente == null)
            {
                cliente = Servicos.Embarcador.Pessoa.Pessoa.Converter(empresa, unitOfWork);
                cliente.Ativo = true;
                repCliente.Inserir(cliente);
            }
            else
            {
                if (cliente.TipoContaBanco == null && empresa.TipoContaBanco != null)
                    cliente.TipoContaBanco = empresa.TipoContaBanco;
                if (cliente.Banco == null && empresa.Banco != null)
                    cliente.Banco = empresa.Banco;
                if (string.IsNullOrWhiteSpace(cliente.Agencia) && !string.IsNullOrWhiteSpace(empresa.Agencia))
                    cliente.Agencia = empresa.Agencia;
                if (string.IsNullOrWhiteSpace(cliente.DigitoAgencia) && !string.IsNullOrWhiteSpace(empresa.DigitoAgencia))
                    cliente.DigitoAgencia = empresa.DigitoAgencia;
                if (string.IsNullOrWhiteSpace(cliente.NumeroConta) && !string.IsNullOrWhiteSpace(empresa.NumeroConta))
                    cliente.NumeroConta = empresa.NumeroConta;
                cliente.DataUltimaAtualizacao = DateTime.Now;
                cliente.Integrado = false;
                repCliente.Atualizar(cliente);
            }


            Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, cliente.CPF_CNPJ);
            if (modalidadePessoas == null)
            {
                modalidadePessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas();
                modalidadePessoas.Cliente = cliente;
                modalidadePessoas.TipoModalidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro;
                repModalidadePessoas.Inserir(modalidadePessoas);
            }

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repositorioModalidadeTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

            if (modalidadeTransportadoraPessoas == null)
            {
                modalidadeTransportadoraPessoas = new Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas();
                modalidadeTransportadoraPessoas.ModalidadePessoas = modalidadePessoas;
                modalidadeTransportadoraPessoas.TipoGeracaoCIOT = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoCIOT.PorViagem;
                modalidadeTransportadoraPessoas.TipoFavorecidoCIOT = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador;
                modalidadeTransportadoraPessoas.PercentualDesconto = 0;
                modalidadeTransportadoraPessoas.PercentualCobranca = 0;
                modalidadeTransportadoraPessoas.PercentualAdiantamentoFretesTerceiro = 0;
                modalidadeTransportadoraPessoas.RNTRC = empresa.RegistroANTT;
                modalidadeTransportadoraPessoas.TipoTransportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.Outros;
                modalidadeTransportadoraPessoas.GerarCIOT = true;
                repModalidadeTransportadoraPessoas.Inserir(modalidadeTransportadoraPessoas);

                Servicos.Cliente.SalvarTiposPagamentoCIOTPorOperadora((tipoPagamentoCIOTPadrao != null ? tipoPagamentoCIOTPadrao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto), modalidadeTransportadoraPessoas, unitOfWork);

            }
            else
            {
                if (modalidadeTransportadoraPessoas.TipoGeracaoCIOT == null)
                    modalidadeTransportadoraPessoas.TipoGeracaoCIOT = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoCIOT.PorViagem;
                if (modalidadeTransportadoraPessoas.TipoFavorecidoCIOT == null)
                    modalidadeTransportadoraPessoas.TipoFavorecidoCIOT = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador;
                modalidadeTransportadoraPessoas.RNTRC = empresa.RegistroANTT;
                repModalidadeTransportadoraPessoas.Atualizar(modalidadeTransportadoraPessoas);

                if (!repositorioModalidadeTipoPagamentoCIOT.ExistePorModalidadeTransportador(modalidadeTransportadoraPessoas.Codigo))
                    Servicos.Cliente.SalvarTiposPagamentoCIOTPorOperadora((tipoPagamentoCIOTPadrao != null ? tipoPagamentoCIOTPadrao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto), modalidadeTransportadoraPessoas, unitOfWork);
            }

            return cliente;
        }

        private static int ObterAnoDataFixa(DateTime dataAtual)
        {
            return dataAtual.Month == 12 ? dataAtual.AddYears(1).Year : dataAtual.Year;
        }

        private static DateTime CalcularDataVencimento(int ano, DateTime dataAtual, int diaVencimentoCIOT, int ultimoDiaDoMes)
        {
            if (diaVencimentoCIOT < dataAtual.Day)
            {
                return new DateTime(ano, dataAtual.AddMonths(1).Month, diaVencimentoCIOT);
            }
            else if (diaVencimentoCIOT > ultimoDiaDoMes)
            {
                return new DateTime(ano, dataAtual.Month, ultimoDiaDoMes);
            }
            else
            {
                return new DateTime(ano, dataAtual.Month, diaVencimentoCIOT);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Terceiros.BuscarConfiguracaoContratoFreteTerceiro BuscarConfiguracaoContratoFreteTerceiro(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete, Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Terceiros.BuscarConfiguracaoContratoFreteTerceiro retorno = new Dominio.ObjetosDeValor.Embarcador.Terceiros.BuscarConfiguracaoContratoFreteTerceiro();

            if (modalidadeTerceiro?.TipoTerceiro?.EspecificarConfiguracaoContratoFreteTipoTerceiro ?? false)
            {
                retorno.PercentualAdiantamentoFreteTerceiro = 0m;
                retorno.PercentualAbastecimentoFreteTerceiro = 0m;
                retorno.DiasVencimentoAdiantamento = 0;
                retorno.DiasVencimentoSaldo = 0;

                if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoPercentualAdiantamentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorPessoa)
                    retorno.PercentualAdiantamentoFreteTerceiro = modalidadeTerceiro?.PercentualAdiantamentoFretesTerceiro ?? 0m;
                else if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoPercentualAdiantamentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorTipoDeOperacao)
                    retorno.PercentualAdiantamentoFreteTerceiro = tipoOperacao?.PercentualAdiantamentoFreteTerceiro ?? 0m;
                else if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoPercentualAdiantamentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro)
                    retorno.PercentualAdiantamentoFreteTerceiro = modalidadeTerceiro?.TipoTerceiro?.PercentualAdiantamentoFretesTerceiro ?? 0m;

                if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoPercentualAbastecimentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorPessoa)
                    retorno.PercentualAbastecimentoFreteTerceiro = modalidadeTerceiro?.PercentualAbastecimentoFretesTerceiro ?? 0m;
                else if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoPercentualAbastecimentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorTipoDeOperacao)
                    retorno.PercentualAbastecimentoFreteTerceiro = tipoOperacao?.PercentualAbastecimentoFreteTerceiro ?? 0m;
                else if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoPercentualAbastecimentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro)
                    retorno.PercentualAbastecimentoFreteTerceiro = modalidadeTerceiro?.TipoTerceiro?.PercentualAbastecimentoFretesTerceiro ?? 0m;

                if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorPessoa)
                    retorno.DiasVencimentoAdiantamento = modalidadeTerceiro?.DiasVencimentoAdiantamentoContratoFrete ?? 0;
                else if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorTipoDeOperacao)
                    retorno.DiasVencimentoAdiantamento = tipoOperacao?.DiasVencimentoAdiantamentoContratoFrete ?? 0;
                else if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro)
                    retorno.DiasVencimentoAdiantamento = modalidadeTerceiro?.TipoTerceiro?.DiasVencimentoAdiantamentoContratoFrete ?? 0;

                if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoDiasVencimentoSaldoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorPessoa)
                {
                    retorno.DataFixaVencimentoSaldo = modalidadeTerceiro?.HabilitarDataFixaVencimento ?? false;
                    retorno.DiasVencimentoSaldo = modalidadeTerceiro?.DiasVencimentoSaldoContratoFrete ?? 0;
                }
                else if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoDiasVencimentoSaldoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorTipoDeOperacao)
                    retorno.DiasVencimentoSaldo = tipoOperacao?.DiasVencimentoSaldoContratoFrete ?? 0;
                else if (modalidadeTerceiro?.TipoTerceiro?.ConfiguracaoDiasVencimentoSaldoContratoFrete == TipoTerceiroConfiguracaoContratoFrete.PorTipoDeTerceiro)
                    retorno.DiasVencimentoSaldo = modalidadeTerceiro?.TipoTerceiro?.DiasVencimentoSaldoContratoFrete ?? 0;
            }
            else
            {
                retorno.PercentualAdiantamentoFreteTerceiro = 0m;
                retorno.PercentualAbastecimentoFreteTerceiro = 0m;
                retorno.DiasVencimentoAdiantamento = configuracaoContratoFreteTerceiro.DiasVencimentoAdiantamento;
                retorno.DiasVencimentoSaldo = configuracaoContratoFreteTerceiro.DiasVencimentoSaldo;

                if (dadosCalculoFrete != null && dadosCalculoFrete.FreteCalculado)
                {
                    retorno.DiasVencimentoAdiantamento = dadosCalculoFrete.DiasVencimentoAdiantamentoContratoFrete;
                    retorno.DiasVencimentoSaldo = dadosCalculoFrete.DiasVencimentoSaldoContratoFrete;
                }

                if (tipoOperacao?.UtilizarConfiguracaoTerceiro ?? false)
                {
                    retorno.PercentualAdiantamentoFreteTerceiro = tipoOperacao?.PercentualAdiantamentoFreteTerceiro ?? 0m;
                    retorno.PercentualAbastecimentoFreteTerceiro = tipoOperacao?.PercentualAbastecimentoFreteTerceiro ?? 0m;

                    if (tipoOperacao.DiasVencimentoAdiantamentoContratoFrete.HasValue)
                        retorno.DiasVencimentoAdiantamento = tipoOperacao.DiasVencimentoAdiantamentoContratoFrete.Value;

                    if (tipoOperacao.DiasVencimentoSaldoContratoFrete.HasValue)
                        retorno.DiasVencimentoSaldo = tipoOperacao.DiasVencimentoSaldoContratoFrete.Value;
                }

                bool contemAlgumaConfiguracaoTerceiro = false;
                if (modalidadeTerceiro != null && ((tipoOperacao?.UtilizarConfiguracaoTerceiroComoPadrao ?? false) || !(tipoOperacao?.UtilizarConfiguracaoTerceiro ?? false)))
                {
                    if ((modalidadeTerceiro.DiasVencimentoAdiantamentoContratoFrete.HasValue && modalidadeTerceiro.DiasVencimentoAdiantamentoContratoFrete.Value > 0) ||
                        (modalidadeTerceiro.DiasVencimentoSaldoContratoFrete.HasValue && modalidadeTerceiro.DiasVencimentoSaldoContratoFrete.Value > 0) ||
                        modalidadeTerceiro.PercentualDesconto > 0 || modalidadeTerceiro.PercentualAbastecimentoFretesTerceiro > 0 || modalidadeTerceiro.PercentualAdiantamentoFretesTerceiro > 0)
                        contemAlgumaConfiguracaoTerceiro = true;

                    if (contemAlgumaConfiguracaoTerceiro)
                    {
                        retorno.PercentualAdiantamentoFreteTerceiro = modalidadeTerceiro.PercentualAdiantamentoFretesTerceiro;
                        retorno.PercentualAbastecimentoFreteTerceiro = modalidadeTerceiro.PercentualAbastecimentoFretesTerceiro;
                    }

                    if (modalidadeTerceiro.DiasVencimentoAdiantamentoContratoFrete.HasValue)
                        retorno.DiasVencimentoAdiantamento = modalidadeTerceiro.DiasVencimentoAdiantamentoContratoFrete.Value;
                    else if ((tipoOperacao?.UtilizarConfiguracaoTerceiroComoPadrao ?? false) && contemAlgumaConfiguracaoTerceiro)
                        retorno.DiasVencimentoAdiantamento = 0;

                    if (modalidadeTerceiro.DiasVencimentoSaldoContratoFrete.HasValue)
                        retorno.DiasVencimentoSaldo = modalidadeTerceiro.DiasVencimentoSaldoContratoFrete.Value;
                    else if ((tipoOperacao?.UtilizarConfiguracaoTerceiroComoPadrao ?? false) && contemAlgumaConfiguracaoTerceiro)
                        retorno.DiasVencimentoSaldo = 0;
                }

                if (retorno.PercentualAdiantamentoFreteTerceiro == 0 && !(tipoOperacao?.UtilizarConfiguracaoTerceiroComoPadrao ?? false))
                    retorno.PercentualAdiantamentoFreteTerceiro = configuracaoContratoFreteTerceiro?.PercentualAdiantamentoFreteTerceiros ?? 0m;
            }

            if ((modalidadeTerceiro?.TipoTerceiro?.AdicionarPercentualAbastecimentoAdiantamentoCartaoNaoInformado ?? false) && string.IsNullOrEmpty(veiculo.NumeroCartaoAbastecimento))
            {
                retorno.PercentualAdiantamentoFreteTerceiro += retorno.PercentualAbastecimentoFreteTerceiro;
                retorno.PercentualAbastecimentoFreteTerceiro = 0;
            }

            return retorno;
        }

        #endregion
    }
}