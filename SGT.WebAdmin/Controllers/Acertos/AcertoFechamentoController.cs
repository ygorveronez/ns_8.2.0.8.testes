using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IO;
using SGTAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize(new string[] { "PesquisarDesconto", "PesquisarBonificacao", "DadosFechamentoAcerto", "RemoverControleTacografo" }, "Acertos/AcertoViagem")]
    public class AcertoFechamentoController : BaseController
    {
        #region Construtores

        public AcertoFechamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro("Inicio Controller AtualizarFechamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria = new Repositorio.Embarcador.Acerto.AcertoDiaria(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoCargaBonificacao repAcertoCargaBonificacao = new Repositorio.Embarcador.Acerto.AcertoCargaBonificacao(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoCargaPedagio repAcertoCargaPedagio = new Repositorio.Embarcador.Acerto.AcertoCargaPedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagemTacografo repAcertoViagemTacografo = new Repositorio.Embarcador.Acerto.AcertoViagemTacografo(unitOfWork);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Cheque repCheque = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unitOfWork);

                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);

                Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigo, codigoSegmento = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("SegmentoVeiculo"), out codigoSegmento);
                int.TryParse(Request.Params("Cheque"), out int codigoCheque);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao;
                Enum.TryParse(Request.Params("Etapa"), out etapa);
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                if (!SalvarVeiculosFechamento(codigo, unitOfWork))
                    return new JsonpResult(false, "Por favor informe o(s) veículo(s) antes de realizar o fechamento do acerto.");

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");

                if (acertoViagem.Situacao != situacao)
                    return new JsonpResult(false, "O Acerto de Viagem já se encontra na situação " + acertoViagem.DescricaoSituacao + ", favor atualize a tela e tente novamente");

                if (!acertoViagem.DataFinal.HasValue || acertoViagem.DataFinal.Value == DateTime.MinValue)
                    return new JsonpResult(false, "Por favor informe a data final na etapa 1 antes de fechar o acerto.");

                if (acertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado && acertoViagem.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Fechamento)
                {
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteReabirAcerto)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para Re-abrir o Acerto de Viagem.");

                    servAcertoViagem.DeletarResultadosAcertoViagem(acertoViagem, unitOfWork);
                }

                if (acertoViagem.Veiculos == null || acertoViagem.Veiculos.Count() == 0)
                    return new JsonpResult(false, "Por favor informe o(s) veículo(s) antes de realizar o fechamento do acerto.");

                decimal.TryParse(Request.Params("ValorTotalAlimentacaoRepassado"), out decimal valorTotalAlimentacaoRepassado);
                decimal.TryParse(Request.Params("ValorAlimentacaoRepassado"), out decimal valorAlimentacaoRepassado);
                decimal.TryParse(Request.Params("ValorAlimentacaoComprovado"), out decimal valorAlimentacaoComprovado);
                decimal.TryParse(Request.Params("ValorAlimentacaoSaldo"), out decimal valorAlimentacaoSaldo);
                decimal.TryParse(Request.Params("ValorTotalAdiantamentoRepassado"), out decimal valorTotalAdiantamentoRepassado);
                decimal.TryParse(Request.Params("ValorAdiantamentoRepassado"), out decimal valorAdiantamentoRepassado);
                decimal.TryParse(Request.Params("ValorAdiantamentoComprovado"), out decimal valorAdiantamentoComprovado);
                decimal.TryParse(Request.Params("ValorAdiantamentoSaldo"), out decimal valorAdiantamentoSaldo);
                decimal.TryParse(Request.Params("SaldoPrevistoAlimentacaoMotorista"), out decimal saldoPrevistoAlimentacaoMotorista);
                decimal.TryParse(Request.Params("SaldoPrevistoOutrasDepesasMotorista"), out decimal saldoPrevistoOutrasDepesasMotorista);

                acertoViagem.ValorTotalAlimentacaoRepassado = valorTotalAlimentacaoRepassado;
                acertoViagem.ValorAlimentacaoRepassado = valorAlimentacaoRepassado;
                acertoViagem.ValorAlimentacaoComprovado = valorAlimentacaoComprovado;
                acertoViagem.ValorAlimentacaoSaldo = valorAlimentacaoSaldo;
                acertoViagem.ValorTotalAdiantamentoRepassado = valorTotalAdiantamentoRepassado;
                acertoViagem.ValorAdiantamentoRepassado = valorAdiantamentoRepassado;
                acertoViagem.ValorAdiantamentoComprovado = valorAdiantamentoComprovado;
                acertoViagem.ValorAdiantamentoSaldo = valorAdiantamentoSaldo;
                acertoViagem.SaldoPrevistoAlimentacaoMotorista = saldoPrevistoAlimentacaoMotorista;
                acertoViagem.SaldoPrevistoOutrasDepesasMotorista = saldoPrevistoOutrasDepesasMotorista;

                acertoViagem.FormaRecebimentoMotoristaAcerto = Request.GetEnumParam<FormaRecebimentoMotoristaAcerto>("FormaRecebimentoMotoristaAcerto");
                acertoViagem.DataVencimentoMotoristaAcerto = Request.GetNullableDateTimeParam("DataVencimentoMotoristaAcerto");
                acertoViagem.ObservacaoMotoristaAcerto = Request.GetStringParam("ObservacaoMotoristaAcerto");
                acertoViagem.TipoMovimentoMotoristaAcerto = repTipoMovimento.BuscarPorCodigo(Request.GetIntParam("TipoMovimentoMotoristaAcerto"));

                acertoViagem.ObservacaoAcertoMotorista = Request.GetStringParam("ObservacaoAcertoMotorista");
                acertoViagem.Titulo = repTitulo.BuscarPorCodigo(Request.GetIntParam("Titulo"));
                acertoViagem.Banco = repBanco.BuscarPorCodigo(Request.GetIntParam("Banco"));

                if (acertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado)
                {
                    acertoViagem.SaldoAtualAlimentacaoMotorista = acertoViagem.Motorista.SaldoDiaria;
                    acertoViagem.SaldoAtualOutrasDepesasMotorista = acertoViagem.Motorista.SaldoAdiantamento;
                }
                else
                {
                    acertoViagem.SaldoAtualAlimentacaoMotorista = 0;
                    acertoViagem.SaldoAtualOutrasDepesasMotorista = 0;
                }

                acertoViagem.Etapa = etapa;
                acertoViagem.Situacao = situacao;
                acertoViagem.DataAlteracao = DateTime.Now;
                if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado || acertoViagem.Operador == null)
                    acertoViagem.Operador = this.Usuario;
                if (codigoSegmento > 0)
                    acertoViagem.SegmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codigoSegmento);
                if (codigoCheque > 0)
                    acertoViagem.Cheque = repCheque.BuscarPorCodigo(codigoCheque);
                else
                    acertoViagem.Cheque = null;

                if (acertoViagem.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Fechamento)
                    acertoViagem.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado;
                else if (acertoViagem.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.OutrasDespesas)
                    acertoViagem.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento;

                if (acertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado && acertoViagem.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Fechamento)
                {
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteFecharAcerto)))
                        return new JsonpResult(false, "Seu usuário não possui permissão para Finalizar o Acerto de Viagem.");
                    if (!acertoViagem.AprovacaoAbastecimento || !acertoViagem.AprovacaoPedagio)
                        return new JsonpResult(false, "Por favor verifique a autorização dos pedágios e/ou dos abastecimentos e se os mesmos se encontram FECHADOS.");

                    if (ConfiguracaoEmbarcador.NaoFecharAcertoViagemAteReceberCanhotos)
                    {
                        List<int> ctesSemCanhoto = repAcertoCarga.ObterDocumentosComCanhotoPendente(acertoViagem.Codigo);
                        if (ctesSemCanhoto != null && ctesSemCanhoto.Count > 0)
                        {
                            if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteFinalizarSemCanhotosRecebidos)))
                                return new JsonpResult(false, $"Existem documentos com canhotos pendentes, não sendo possível fechar o acerto. Documentos: {string.Join(", ", ctesSemCanhoto)}.");
                        }
                    }
                    if (configuraoAcertoViagem?.NaoFecharAcertoViagemAteReceberPallets ?? false)
                    {
                        List<string> cargasSemPallet = repAcertoCarga.ObterCargasComPalletPendente(acertoViagem.Codigo);
                        if (cargasSemPallet != null && cargasSemPallet.Count > 0)
                        {
                            if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteFinalizarSemPalletsEntregues)))
                                return new JsonpResult(false, $"Existem cargas com pallets com situação não entregue, não sendo possível fechar o acerto. Cargas: {string.Join(", ", cargasSemPallet)}.");
                        }
                    }

                    if (acertoViagem.Abastecimentos != null && acertoViagem.Abastecimentos.Count > 0 && acertoViagem.Abastecimentos.Any(a => a.Abastecimento.Posto == null))
                        return new JsonpResult(false, "Existe abastecimento lançado sem o Posto informado (INCONSISTENTE).");

                    servAcertoViagem.InserirResultadosAcertoViagem(acertoViagem, unitOfWork, this.Usuario, ConfiguracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem, ConfiguracaoEmbarcador.AcertoDeViagemImpressaoDetalhada, ConfiguracaoEmbarcador.GerarTituloFolhaPagamento, ConfiguracaoEmbarcador.GerarReciboAcertoViagemDetalhado, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem);

                    decimal totalPercentualComprtilhamento = 0;
                    if (acertoViagem.Cargas != null && acertoViagem.Cargas.Count > 0)
                    {
                        foreach (var acertoCarga in acertoViagem.Cargas)
                        {
                            totalPercentualComprtilhamento = repAcertoCarga.BuscarSomaPercentualCompartilhamentoCarga(acertoCarga.Carga.Codigo);
                            if (totalPercentualComprtilhamento > 100)
                                return new JsonpResult(false, "A soma do percentual de compatilhamento da carga nº " + acertoCarga.Carga.CodigoCargaEmbarcador + " ultrapassa os 100%. Total: " + totalPercentualComprtilhamento.ToString("n2") + "%");
                        }
                    }

                    if (ConfiguracaoEmbarcador.SolicitarAprovacaoFolgaAcertoViagem && !acertoViagem.FolgasAprovadas && acertoViagem.QuantidadeDiasFolga > 0)
                        return new JsonpResult(false, "Necessário a aprovação das folgas do motorista para finalizar o acerto.");
                }

                repAcertoViagem.Atualizar(acertoViagem, Auditado);

                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unitOfWork);
                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroEntidade repMovimentoFinanceiroEntidade = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroEntidade(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();

                List<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo> tacografos = repAcertoViagemTacografo.BuscarPorAcerto(acertoViagem.Codigo);

                if (acertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado && acertoViagem.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Fechamento)
                {
                    if (configuracaoAcertoViagem == null || !configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem)
                        return new JsonpResult(false, "Não possui configuração financeira para realizar o fechamento do acerto de viagem");

                    if (acertoViagem.QuantidadeDiasFolga > 0 && acertoViagem.DataInicioFolga.HasValue && acertoViagem.DataFinalFolga.HasValue)
                    {
                        Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao situacaoFolga = repColaboradorSituacao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Folga);
                        if (situacaoFolga != null)
                        {
                            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = new Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento()
                            {
                                Colaborador = acertoViagem.Motorista,
                                ColaboradorSituacao = situacaoFolga,
                                Data = DateTime.Now,
                                DataInicial = acertoViagem.DataInicioFolga.Value,
                                DataFinal = acertoViagem.DataFinalFolga.Value,
                                Numero = repColaboradorLancamento.ProximoNumeroColaboradorLancamento(0),
                                Observacao = "GERADO A PARTIR DO ACERTO DE VIAGEM " + acertoViagem.Numero.ToString("n0"),
                                Operador = acertoViagem.Operador,
                                SituacaoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Agendado
                            };
                            repColaboradorLancamento.Inserir(colaboradorLancamento, Auditado);

                            acertoViagem.ColaboradorLancamento = colaboradorLancamento;
                        }
                    }

                    if (acertoViagem.DataFinal.HasValue)
                        acertoViagem.Motorista.DiasTrabalhado += (int)(acertoViagem.DataFinal.Value - acertoViagem.DataInicial).TotalDays;

                    if (!repAcertoViagem.ContemAcertoEmAberto(acertoViagem.Motorista?.Codigo ?? 0, acertoViagem.Codigo))
                    {
                        if (!repAcertoViagem.ContemAcertoFechadoMaior(acertoViagem.Motorista?.Codigo ?? 0, acertoViagem.Codigo))
                            acertoViagem.Motorista.DataFechamentoAcerto = acertoViagem.DataFinal;
                    }
                    repUsuario.Atualizar(acertoViagem.Motorista);

                    if ((configuraoAcertoViagem?.HabilitarLancamentoTacografo ?? false) && tacografos != null && tacografos.Count > 0)
                    {
                        foreach (var tacografo in tacografos)
                        {
                            Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo = repControleTacografo.BuscarPorCodigo(tacografo.ControleTacografo.Codigo, false);
                            controleTacografo.Situacao = 2;
                            controleTacografo.DataRetorno = DateTime.Now;
                            repControleTacografo.Atualizar(controleTacografo);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, controleTacografo, null, "Recebido pelo acerto " + acertoViagem.Numero.ToString("D") + ".", unitOfWork);
                        }
                    }
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagem.Motorista, null, "Fechou o acerto de viagem.", unitOfWork);

                    //Validar percentual das cargas
                    for (int i = 0; i < acertoViagem.Cargas.Count(); i++)
                    {
                        if (repAcertoCarga.CargaEmOutroAcerto(codigo, acertoViagem.Cargas[i].Carga.Codigo))
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga cargaDuplicada = repCarga.BuscarPorCodigo(acertoViagem.Cargas[i].Carga.Codigo);
                            if (repAcertoCarga.BuscarSomaPercentualCompartilhamentoCarga(cargaDuplicada.Codigo) > 100)
                            {
                                return new JsonpResult(false, "O percentual de compartilhamento da carga " + cargaDuplicada.CodigoCargaEmbarcador + " excede os 100%!");
                            }
                        }
                    }

                    bool jaGerouMovimentacaoFinanceira = false;
                    List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade> movimentosFuncionario = repMovimentoFinanceiroEntidade.BuscarPorMotoristaAcerto(acertoViagem.Motorista.Codigo, acertoViagem.Numero, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto);
                    if (movimentosFuncionario != null && movimentosFuncionario.Count() > 0)
                        jaGerouMovimentacaoFinanceira = true;

                    //if (jaGerouMovimentacaoFinanceira)
                    //{
                    //    bool teveMovimentoReversao = repMovimentoFinanceiroEntidade.BuscarPorMotoristaAcertoRevertido(acertoViagem.Motorista.Codigo, acertoViagem.Numero, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto);
                    //    if (teveMovimentoReversao)
                    //        jaGerouMovimentacaoFinanceira = false;
                    //}
                    //return new JsonpResult(false, "O acerto de viagem já possui movimentações realizadas, favor atualize a página!");

                    if (!jaGerouMovimentacaoFinanceira)
                    {
                        List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> receita = servAcertoViagem.RetornaObjetoReceitaViagem(codigo, unitOfWork, ConfiguracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem, ConfiguracaoEmbarcador.AcertoDeViagemImpressaoDetalhada, ConfiguracaoEmbarcador.GerarTituloFolhaPagamento, ConfiguracaoEmbarcador.GerarReciboAcertoViagemDetalhado, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, (configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false));

                        if (acertoViagem.Operador.PlanoConta != null && acertoViagem.Motorista.PlanoAcertoViagem != null)
                        {
                            GerarMovimentacaoOperadorMotorista(acertoViagem, unitOfWork, false, receita.Sum(o => o.AdiantamentoMotorista));
                        }
                        else if (acertoViagem.Operador.PlanoConta != null && acertoViagem.Motorista.PlanoAcertoViagem == null)
                            return new JsonpResult(false, "O motorista do Acerto não possui configuração de conta contábil.");

                        //if (ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem && (acertoViagem.SaldoPrevistoAlimentacaoMotorista < 0 || acertoViagem.SaldoPrevistoOutrasDepesasMotorista < 0))
                        //    return new JsonpResult(false, "O saldo previsto do motorista não pode ficar negativo.");

                        //Abastecimentos pago pelo motorista
                        for (int i = 0; i < acertoViagem.Abastecimentos.Count; i++)
                        {
                            if (acertoViagem.Abastecimentos[i].LancadoManualmente && (acertoViagem.Abastecimentos[i].Abastecimento.Posto == null || acertoViagem.Abastecimentos[i].Abastecimento.Posto.Modalidades == null || acertoViagem.Abastecimentos[i].Abastecimento.Posto.Modalidades[0].Codigo == 0 || repModalidadeFornecedor.BuscarPorCliente(acertoViagem.Abastecimentos[i].Abastecimento.Posto.CPF_CNPJ) == null || !repModalidadeFornecedor.BuscarPorCliente(acertoViagem.Abastecimentos[i].Abastecimento.Posto.CPF_CNPJ).PagoPorFatura || acertoViagem.Abastecimentos[i].Abastecimento.Posto.CPF_CNPJ_Formatado == acertoViagem.Motorista.CPF_Formatado))
                            {
                                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPeloMotorista?.Codigo ?? 0);
                                servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPeloMotorista, acertoViagem.Abastecimentos[i].Abastecimento.Data.Value, acertoViagem.Abastecimentos[i].Abastecimento.ValorTotal, acertoViagem.Numero.ToString(), "ABASTECIMENTO LANÇADO NO ACERTO PAGO PELO MOTORISTA", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            }
                        }
                        //Abastecimentos pagos pela empresa - NÃO GERA MAIS

                        //Pedágios recebidos pelo embarcador
                        for (int i = 0; i < acertoViagem.Cargas.Count(); i++)
                        {
                            if (acertoViagem.Cargas[i].PedagioAcerto > 0)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcador?.Codigo ?? 0);
                                servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcador, acertoViagem.DataFinal.Value, acertoViagem.Cargas[i].PedagioAcerto, acertoViagem.Numero.ToString(), "PEDAGIO LANÇADO NO ACERTO E PAGO PELO EMBARCADOR", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            }
                        }

                        //Pedágios recebidos pelo embarcador de crédito
                        for (int i = 0; i < acertoViagem.Cargas.Count(); i++)
                        {
                            if (acertoViagem.Cargas[i].PedagioAcertoCredito > 0)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito?.Codigo ?? 0);
                                servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito, acertoViagem.DataFinal.Value, acertoViagem.Cargas[i].PedagioAcertoCredito, acertoViagem.Numero.ToString(), "PEDAGIO LANÇADO NO ACERTO E PAGO PELO EMBARCADOR EM CRÉDITO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            }
                        }

                        //Outas despesas
                        if (acertoViagem.OutrasDespesas.Count() > 0)
                        {
                            decimal valorOutrasDespesasMotorista = 0;
                            decimal valorOutrasDespesasEmpresa = 0;
                            List<Dominio.Entidades.Embarcador.Fatura.Justificativa> listaJustificativa = repAcertoOutraDespesa.BuscarJustificativas(codigo);
                            for (int i = 0; i < listaJustificativa.Count(); i++)
                            {
                                List<Dominio.Entidades.Produto> produtos = repAcertoOutraDespesa.BuscarProdutos(codigo, listaJustificativa[i].Codigo);
                                if (produtos == null || produtos.Count == 0)
                                {
                                    produtos = new List<Dominio.Entidades.Produto>();
                                    produtos.Add(repAcertoOutraDespesa.ProdutoPrincipalOutrasDespesas(codigo, null, listaJustificativa[i].Codigo, false));
                                }
                                for (int a = 0; a < produtos.Count(); a++)
                                {
                                    valorOutrasDespesasMotorista = repAcertoOutraDespesa.ValorOutrasDespesas(codigo, null, listaJustificativa[i].Codigo, false, produtos[a]?.Codigo ?? 0);
                                    if (valorOutrasDespesasMotorista == 0)
                                        valorOutrasDespesasMotorista = repAcertoOutraDespesa.ValorOutrasDespesas(codigo, null, listaJustificativa[i].Codigo, false, 0);
                                    if (valorOutrasDespesasMotorista > 0)
                                    {
                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoMovimentoEntidade = ConfiguracaoEmbarcador.TipoMovimentoReversaoPagamentoMotorista;
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(listaJustificativa[i].TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(listaJustificativa[i].TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, valorOutrasDespesasMotorista, acertoViagem.Numero.ToString(), "OUTRAS DESPESAS PAGAS PELO MOTORISTA " + listaJustificativa[i].Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, tipoMovimentoEntidade, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira, produtos[a]);
                                    }
                                    valorOutrasDespesasEmpresa = repAcertoOutraDespesa.ValorOutrasDespesas(codigo, null, listaJustificativa[i].Codigo, true, produtos[a]?.Codigo ?? 0);
                                    if (valorOutrasDespesasEmpresa == 0)
                                        valorOutrasDespesasEmpresa = repAcertoOutraDespesa.ValorOutrasDespesas(codigo, null, listaJustificativa[i].Codigo, true, 0);
                                    if (valorOutrasDespesasEmpresa > 0)
                                    {
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(listaJustificativa[i].TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(listaJustificativa[i].TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, valorOutrasDespesasEmpresa, acertoViagem.Numero.ToString(), "OUTRAS DESPESAS PAGAS PELA EMPRESA EM FATURA " + listaJustificativa[i].Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira, produtos[a]);
                                    }
                                }
                            }
                        }

                        //Pedágios pagos pelo motorista
                        for (int i = 0; i < acertoViagem.Pedagios.Count(); i++)
                        {
                            if (acertoViagem.Pedagios[i].Pedagio.SituacaoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado && !acertoViagem.Pedagios[i].Pedagio.ImportadoDeSemParar && acertoViagem.Pedagios[i].Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoPedagioPagoPeloMotorista?.Codigo ?? 0);
                                servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoPedagioPagoPeloMotorista, acertoViagem.Pedagios[i].Pedagio.Data, acertoViagem.Pedagios[i].Pedagio.Valor, acertoViagem.Numero.ToString(), "PEDAGIO LANÇADO NO ACERTO PAGO PELO MOTORISTA", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            }
                            else if (acertoViagem.Pedagios[i].Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito)
                            {
                                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(acertoViagem.Pedagios[i].Pedagio.Codigo, true);

                                // Fecha pedagio
                                pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado;
                                pedagio.DataAlteracao = DateTime.Now;

                                if (pedagio.TipoMovimento == null)
                                    pedagio.TipoMovimento = configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito;

                                repPedagio.Atualizar(pedagio, Auditado);

                                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito?.Codigo ?? 0);
                                servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito, acertoViagem.DataFinal.Value, pedagio.Valor, acertoViagem.Numero.ToString(), "PEDAGIO DE CRÉDIDO LANÇADO NO ACERTO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            }
                        }

                        //Comissão do motorista
                        if (receita[0].ComissaoMotorista > 0)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoComissaoDoMotorista?.Codigo ?? 0);
                            servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoComissaoDoMotorista, acertoViagem.DataFinal.Value, receita[0].ComissaoMotorista, acertoViagem.Numero.ToString(), "COMISSAO LANÇADA NO ACERTO PAGA AO MOTORISTA", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                        }

                        if (receita[0].PremioComissaoMotorista > 0)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoComissaoDoMotorista?.Codigo ?? 0);
                            servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoComissaoDoMotorista, acertoViagem.DataFinal.Value, receita[0].PremioComissaoMotorista, acertoViagem.Numero.ToString(), "COMISSAO DE PREMIASSÃO LANÇADA NO ACERTO PAGA AO MOTORISTA", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                        }

                        //Diárias
                        if (acertoViagem.Diarias != null && acertoViagem.Diarias.Count > 0)
                        {
                            List<Dominio.Entidades.Embarcador.Fatura.Justificativa> justificativas = repAcertoDiaria.BuscarJustificativas(codigo);
                            if (justificativas != null)
                            {
                                for (int i = 0; i < justificativas.Count; i++)
                                {
                                    if (justificativas[i] != null && justificativas[i].TipoMovimentoUsoJustificativa != null)
                                    {
                                        decimal valorPorJustificativa = repAcertoDiaria.BuscarValorPorJustificativa(justificativas[i].Codigo, codigo);
                                        if (valorPorJustificativa > 0)
                                        {
                                            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(justificativas[i]?.TipoMovimentoUsoJustificativa?.Codigo ?? 0);

                                            if (acertoViagem.Diarias[0] != null && (acertoViagem.Diarias[0].TabelaDiariaPeriodo?.TabelaDiaria?.GerarMovimentoSaidaFixaMotorista ?? false))
                                                servProcessoMovimento.GerarMovimentacao(justificativas[i].TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, valorPorJustificativa, acertoViagem.Numero.ToString(), "DIÁRIA AGRUPADA PAGA AO MOTORISTA " + (acertoViagem.Motorista?.Nome ?? ""), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                            else
                                                servProcessoMovimento.GerarMovimentacao(justificativas[i].TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, valorPorJustificativa, acertoViagem.Numero.ToString(), "DIÁRIA AGRUPADA PAGA AO MOTORISTA " + (acertoViagem.Motorista?.Nome ?? ""), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                        }
                                    }
                                }
                            }
                            //for (int i = 0; i < acertoViagem.Diarias.Count(); i++)
                            //{
                            //    if (acertoViagem.Diarias[i].Valor > 0 && acertoViagem.Diarias[i].Justificativa != null && acertoViagem.Diarias[i].Justificativa.TipoMovimentoUsoJustificativa != null)
                            //    {
                            //        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.Diarias[i].Justificativa?.TipoMovimentoUsoJustificativa?.Codigo ?? 0);

                            //        if (acertoViagem.Diarias[i].TabelaDiariaPeriodo?.TabelaDiaria?.GerarMovimentoSaidaFixaMotorista ?? false)
                            //            servProcessoMovimento.GerarMovimentacao(acertoViagem.Diarias[i].Justificativa.TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, acertoViagem.Diarias[i].Valor, acertoViagem.Numero.ToString(), "DIÁRIA PAGA AO MOTORISTA " + acertoViagem.Diarias[i].Descricao + " " + (acertoViagem.Motorista?.Nome ?? ""), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            //        else
                            //            servProcessoMovimento.GerarMovimentacao(acertoViagem.Diarias[i].Justificativa.TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, acertoViagem.Diarias[i].Valor, acertoViagem.Numero.ToString(), "DIÁRIA PAGA AO MOTORISTA " + acertoViagem.Diarias[i].Descricao + " " + (acertoViagem.Motorista?.Nome ?? ""), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            //    }
                            //}
                        }

                        //Bonificações
                        for (int i = 0; i < acertoViagem.Bonificacoes.Count(); i++)
                        {
                            if (acertoViagem.Bonificacoes[i].Justificativa != null)
                            {
                                if (acertoViagem.Bonificacoes[i].Justificativa.GerarMovimentoAutomatico)
                                    if (acertoViagem.Bonificacoes[i].Justificativa.TipoMovimentoUsoJustificativa != null)
                                    {
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.Bonificacoes[i].Justificativa.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(acertoViagem.Bonificacoes[i].Justificativa.TipoMovimentoUsoJustificativa, acertoViagem.Bonificacoes[i].Data, acertoViagem.Bonificacoes[i].ValorBonificacao, acertoViagem.Numero.ToString(), "BONIFICAÇÃO LANÇADA NO ACERTO PAGO AO MOTORISTA " + acertoViagem.Bonificacoes[i].Motivo, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                    }
                                    else
                                        return new JsonpResult(false, "Não possui configuração financeira para os lançamentos das bonificações.");
                            }
                        }

                        //Descontos
                        for (int i = 0; i < acertoViagem.Descontos.Count(); i++)
                        {
                            if (acertoViagem.Descontos[i].Justificativa != null)
                            {
                                if (acertoViagem.Descontos[i].Justificativa.GerarMovimentoAutomatico)
                                    if (acertoViagem.Descontos[i].Justificativa.TipoMovimentoUsoJustificativa != null)
                                    {
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.Descontos[i].Justificativa.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(acertoViagem.Descontos[i].Justificativa.TipoMovimentoUsoJustificativa, acertoViagem.Descontos[i].Data, acertoViagem.Descontos[i].ValorDesconto, acertoViagem.Numero.ToString(), "DESCONTO LANÇADA NO ACERTO DESCONTADO AO MOTORISTA " + acertoViagem.Descontos[i].Motivo, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                    }
                                    else
                                        return new JsonpResult(false, "Não possui configuração financeira para os lançamentos dos descontos.");
                            }
                        }

                        //Devoluções
                        for (int i = 0; i < acertoViagem.DevolucoesMoedaEstrangeira.Count(); i++)
                        {
                            if (acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa != null)
                            {
                                if (acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa.GerarMovimentoAutomatico)
                                    if (acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa.TipoMovimentoUsoJustificativa != null)
                                    {
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa.TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, acertoViagem.DevolucoesMoedaEstrangeira[i].ValorDevolucao, acertoViagem.Numero.ToString(), "DEVOLUÇÃO NO ACERTO DO MOTORISTA DA MOEDA " + acertoViagem.DevolucoesMoedaEstrangeira[i].MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                    }
                                    else
                                        return new JsonpResult(false, "Não possui configuração financeira para os lançamentos das devoluções do motorista.");
                            }
                        }

                        //Variações
                        for (int i = 0; i < acertoViagem.VariacoesCambial.Count(); i++)
                        {
                            if (acertoViagem.VariacoesCambial[i].Justificativa != null)
                            {
                                if (acertoViagem.VariacoesCambial[i].Justificativa.GerarMovimentoAutomatico)
                                    if (acertoViagem.VariacoesCambial[i].Justificativa.TipoMovimentoUsoJustificativa != null)
                                    {
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.VariacoesCambial[i].Justificativa.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(acertoViagem.VariacoesCambial[i].Justificativa.TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, acertoViagem.VariacoesCambial[i].ValorVariacao, acertoViagem.Numero.ToString(), "VARIAÇÃO CAMBIAL NO ACERTO DO MOTORISTA DA MOEDA " + acertoViagem.VariacoesCambial[i].MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, null, null, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                    }
                                    else
                                        return new JsonpResult(false, "Não possui configuração financeira para os lançamentos das variações cambiais.");
                            }
                        }

                        //Bonificações lançadas pelo cliente
                        List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao> listaBonificacoesClientes = repAcertoCargaBonificacao.BuscarPorAcerto(acertoViagem.Codigo);
                        for (int i = 0; i < listaBonificacoesClientes.Count(); i++)
                        {
                            if (listaBonificacoesClientes[i].Justificativa != null)
                            {
                                if (listaBonificacoesClientes[i].Justificativa.GerarMovimentoAutomatico)
                                    if (listaBonificacoesClientes[i].Justificativa.TipoMovimentoUsoJustificativa != null)
                                    {
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(listaBonificacoesClientes[i].Justificativa.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(listaBonificacoesClientes[i].Justificativa.TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, listaBonificacoesClientes[i].Valor, acertoViagem.Numero.ToString(), "BONIFICAÇÃO DO CLIENTE LANÇADA NO ACERTO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                    }
                                    else
                                        return new JsonpResult(false, "Não possui configuração financeira para os lançamentos das bonificações do cliente.");
                            }
                        }

                        //Pedágios lançadas pelo cliente
                        List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio> listaAcertoCargaPedagio = repAcertoCargaPedagio.BuscarPorAcerto(acertoViagem.Codigo);
                        for (int i = 0; i < listaAcertoCargaPedagio.Count(); i++)
                        {
                            if (listaAcertoCargaPedagio[i].Justificativa != null)
                            {
                                if (listaAcertoCargaPedagio[i].Justificativa.GerarMovimentoAutomatico)
                                    if (listaAcertoCargaPedagio[i].Justificativa.TipoMovimentoUsoJustificativa != null)
                                    {
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(listaAcertoCargaPedagio[i].Justificativa.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(listaAcertoCargaPedagio[i].Justificativa.TipoMovimentoUsoJustificativa, acertoViagem.DataFinal.Value, listaAcertoCargaPedagio[i].Valor, acertoViagem.Numero.ToString(), "PEDÁGIO DO CLIENTE LANÇADA NO ACERTO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                    }
                                    else
                                        return new JsonpResult(false, "Não possui configuração financeira para os lançamentos dos pedágios do cliente.");
                            }
                        }

                        acertoViagem.DataFechamento = DateTime.Now;
                        string msgRetorno = "";
                        if ((configuraoAcertoViagem?.HabilitarFormaRecebimentoTituloAoMotorista ?? false) && receita[0].SaldoMotorista < 0)
                        {
                            if (acertoViagem.FormaRecebimentoMotoristaAcerto == FormaRecebimentoMotoristaAcerto.CriarTitulo)
                            {
                                if (!servAcertoViagem.GerarTituloMotoristaSaldo(ref msgRetorno, acertoViagem, unitOfWork, this.Usuario, Auditado, TipoServicoMultisoftware, _conexao.StringConexao, ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, receita[0].SaldoMotorista + acertoViagem.Motorista.SaldoAdiantamento, ConfiguracaoEmbarcador, acertoViagem.Motorista.SaldoAdiantamento))
                                    return new JsonpResult(false, msgRetorno);
                            }
                            else if (!servAcertoViagem.GerarControleSaldoMotorista(ref msgRetorno, acertoViagem, unitOfWork, this.Usuario, Auditado, TipoServicoMultisoftware, _conexao.StringConexao, ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, receita[0].SaldoMotorista))
                                return new JsonpResult(false, msgRetorno);
                        }
                        else if (!(configuraoAcertoViagem?.HabilitarFormaRecebimentoTituloAoMotorista ?? false) && !servAcertoViagem.GerarControleSaldoMotorista(ref msgRetorno, acertoViagem, unitOfWork, this.Usuario, Auditado, TipoServicoMultisoftware, _conexao.StringConexao, ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, receita[0].SaldoMotorista))
                            return new JsonpResult(false, msgRetorno);
                        else if (receita[0].SaldoMotorista > 0 && (configuraoAcertoViagem?.HabilitarFormaRecebimentoTituloAoMotorista ?? false) && acertoViagem.FormaRecebimentoMotoristaAcerto == FormaRecebimentoMotoristaAcerto.DescontarFicha)
                        {
                            if (!servAcertoViagem.GerarControleSaldoMotorista(ref msgRetorno, acertoViagem, unitOfWork, this.Usuario, Auditado, TipoServicoMultisoftware, _conexao.StringConexao, ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, receita[0].SaldoMotorista))
                                return new JsonpResult(false, msgRetorno);
                        }
                        else if (receita[0].SaldoMotorista > 0 && (configuraoAcertoViagem?.HabilitarFormaRecebimentoTituloAoMotorista ?? false) && acertoViagem.FormaRecebimentoMotoristaAcerto == FormaRecebimentoMotoristaAcerto.NadaFazer)
                            if (!servAcertoViagem.GerarPagamentoMotoristaSaldoMotorista(ref msgRetorno, acertoViagem, unitOfWork, this.Usuario, Auditado, TipoServicoMultisoftware, _conexao.StringConexao, ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, receita[0].SaldoMotorista - (acertoViagem.Motorista.SaldoAdiantamento * -1), ConfiguracaoEmbarcador))
                                return new JsonpResult(false, msgRetorno);


                        repAcertoViagem.Atualizar(acertoViagem);

                        Servicos.Embarcador.Transportadores.Motorista.AtualizarStatusColaborador(unitOfWork, Auditado, false, acertoViagem.Motorista.Codigo);
                    }
                }
                else if (acertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento && acertoViagem.Etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.OutrasDespesas)
                {
                    //validar se tem pagamento não cancelado
                    Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                    if (repPagamentoMotorista.ContemPorAcerto(acertoViagem.Codigo, acertoViagem.Motorista.Codigo))
                        return new JsonpResult(false, "Não é possível reabrir o acerto, o mesmo está vinculado a um pagamento ao motorista.");

                    if (repTitulo.ContemPorAcerto(acertoViagem.Codigo))
                        return new JsonpResult(false, "Não é possível reabrir o acerto, o mesmo está vinculado a um título ao motorista não cancelado.");

                    if (configuracaoAcertoViagem == null || !configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem)
                        return new JsonpResult(false, "Não possui configuração financeira para realizar a reversão do acerto de viagem");

                    if (acertoViagem.ColaboradorLancamento != null)
                    {
                        Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repColaboradorLancamento.BuscarPorCodigo(acertoViagem.ColaboradorLancamento.Codigo, true);
                        Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(colaboradorLancamento.Colaborador.Codigo, true);

                        usuario.SituacaoColaborador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, "Alterou a situação para Trabalhando.", unitOfWork);

                        if (colaboradorLancamento.ColaboradorSituacao.SituacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Folga)
                            usuario.DiasFolgaRetirado -= (int)(colaboradorLancamento.DataFinal - colaboradorLancamento.DataInicial).TotalDays;

                        usuario.DiasTrabalhado -= (int)(acertoViagem.DataFinal.Value - acertoViagem.DataInicial).TotalDays;

                        repUsuario.Atualizar(usuario, Auditado);
                        acertoViagem.ColaboradorLancamento = null;

                        colaboradorLancamento.SituacaoLancamento = SituacaoLancamentoColaborador.Cancelado;

                        repColaboradorLancamento.Atualizar(colaboradorLancamento, Auditado);
                    }

                    if ((configuraoAcertoViagem?.HabilitarLancamentoTacografo ?? false) && tacografos != null && tacografos.Count > 0)
                    {
                        foreach (var tacografo in tacografos)
                        {
                            Dominio.Entidades.Embarcador.Frota.ControleTacografo controleTacografo = repControleTacografo.BuscarPorCodigo(tacografo.ControleTacografo.Codigo, false);
                            controleTacografo.Situacao = 1;
                            controleTacografo.DataRetorno = null;
                            repControleTacografo.Atualizar(controleTacografo);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, controleTacografo, null, "Estornado recebimento pelo acerto " + acertoViagem.Numero.ToString("D") + ".", unitOfWork);
                        }
                    }

                    List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> receita = servAcertoViagem.RetornaObjetoReceitaViagem(codigo, unitOfWork, ConfiguracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem, ConfiguracaoEmbarcador.AcertoDeViagemImpressaoDetalhada, ConfiguracaoEmbarcador.GerarTituloFolhaPagamento, ConfiguracaoEmbarcador.GerarReciboAcertoViagemDetalhado, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, (configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false));

                    if (acertoViagem.Operador.PlanoConta != null && acertoViagem.Motorista.PlanoAcertoViagem != null)
                    {
                        GerarMovimentacaoOperadorMotorista(acertoViagem, unitOfWork, true, receita.Sum(o => o.AdiantamentoMotorista));
                    }

                    List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade> movimentosFuncionario = repMovimentoFinanceiroEntidade.BuscarPorMotoristaAcerto(acertoViagem.Motorista.Codigo, acertoViagem.Numero, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto);
                    for (int i = 0; i < movimentosFuncionario.Count; i++)
                    {
                        repMovimentoFinanceiroEntidade.Deletar(movimentosFuncionario[i]);
                    }

                    //Abastecimentos pago pelo motorista
                    for (int i = 0; i < acertoViagem.Abastecimentos.Count; i++)
                    {
                        if (acertoViagem.Abastecimentos[i].LancadoManualmente && (acertoViagem.Abastecimentos[i].Abastecimento.Posto == null || acertoViagem.Abastecimentos[i].Abastecimento.Posto.Modalidades == null || acertoViagem.Abastecimentos[i].Abastecimento.Posto.Modalidades[0].Codigo == 0 || repModalidadeFornecedor.BuscarPorCliente(acertoViagem.Abastecimentos[i].Abastecimento.Posto.CPF_CNPJ) == null || !repModalidadeFornecedor.BuscarPorCliente(acertoViagem.Abastecimentos[i].Abastecimento.Posto.CPF_CNPJ).PagoPorFatura) || acertoViagem.Abastecimentos[i].Abastecimento.Posto.CPF_CNPJ_Formatado == acertoViagem.Motorista.CPF_Formatado)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista?.Codigo ?? 0);
                            servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista, acertoViagem.Abastecimentos[i].Abastecimento.Data.Value, acertoViagem.Abastecimentos[i].Abastecimento.ValorTotal, acertoViagem.Numero.ToString(), "REVERSÃO ABASTECIMENTO LANÇADO NO ACERTO PAGO PELO MOTORISTA", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                        }
                    }
                    //Abastecimentos pagos pela empresa - NÃO GERA MAIS

                    //Pedágios recebidos pelo embarcador
                    for (int i = 0; i < acertoViagem.Cargas.Count(); i++)
                    {
                        if (acertoViagem.Cargas[i].PedagioAcerto > 0)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcador?.Codigo ?? 0);
                            servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcador, acertoViagem.DataFinal.Value, acertoViagem.Cargas[i].PedagioAcerto, acertoViagem.Numero.ToString(), "REVERSÃO PEDAGIO LANÇADO NO ACERTO E PAGO PELO EMBARCADOR", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                        }
                    }

                    //Pedágios recebidos pelo embarcador por crédito
                    for (int i = 0; i < acertoViagem.Cargas.Count(); i++)
                    {
                        if (acertoViagem.Cargas[i].PedagioAcertoCredito > 0)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito?.Codigo ?? 0);
                            servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito, acertoViagem.DataFinal.Value, acertoViagem.Cargas[i].PedagioAcertoCredito, acertoViagem.Numero.ToString(), "REVERSÃO PEDAGIO LANÇADO NO ACERTO E PAGO PELO EMBARCADOR EM CRÉDITO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                        }
                    }

                    //Outas despesas
                    if (acertoViagem.OutrasDespesas.Count() > 0)
                    {
                        decimal valorOutrasDespesasMotorista = 0;
                        decimal valorOutrasDespesasEmpresa = 0;
                        List<Dominio.Entidades.Embarcador.Fatura.Justificativa> listaJustificativa = repAcertoOutraDespesa.BuscarJustificativas(codigo);
                        //List<DateTime> listaDatas = repAcertoOutraDespesa.BuscarDatasJustificativas(codigo);
                        for (int i = 0; i < listaJustificativa.Count(); i++)
                        {
                            //for (int a = 0; a < listaDatas.Count(); a++)
                            //{
                            valorOutrasDespesasMotorista = repAcertoOutraDespesa.ValorOutrasDespesas(codigo, null, listaJustificativa[i].Codigo, false, 0);
                            if (valorOutrasDespesasMotorista > 0)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade tipoMovimentoEntidade = ConfiguracaoEmbarcador.TipoMovimentoPagamentoMotorista;
                                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(listaJustificativa[i].TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                servProcessoMovimento.GerarMovimentacao(listaJustificativa[i].TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, valorOutrasDespesasMotorista, acertoViagem.Numero.ToString(), "REVERSÃO OUTRAS DESPESAS PAGAS PELO MOTORISTA " + listaJustificativa[i].Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            }
                            valorOutrasDespesasEmpresa = repAcertoOutraDespesa.ValorOutrasDespesas(codigo, null, listaJustificativa[i].Codigo, true, 0);
                            if (valorOutrasDespesasEmpresa > 0)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(listaJustificativa[i].TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                servProcessoMovimento.GerarMovimentacao(listaJustificativa[i].TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, valorOutrasDespesasEmpresa, acertoViagem.Numero.ToString(), "REVERSÃO OUTRAS DESPESAS PAGAS PELA EMPRESA EM FATURA " + listaJustificativa[i].Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            }
                            //}
                        }
                    }

                    //Pedágios pagos pelo motorista
                    for (int i = 0; i < acertoViagem.Pedagios.Count(); i++)
                    {
                        if (acertoViagem.Pedagios[i].Pedagio.SituacaoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado && !acertoViagem.Pedagios[i].Pedagio.ImportadoDeSemParar && acertoViagem.Pedagios[i].Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoReversaoPedagioPagoPeloMotorista?.Codigo ?? 0);
                            servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoReversaoPedagioPagoPeloMotorista, acertoViagem.Pedagios[i].Pedagio.Data, acertoViagem.Pedagios[i].Pedagio.Valor, acertoViagem.Numero.ToString(), "REVERSÃO PEDAGIO LANÇADO NO ACERTO PAGO PELO MOTORISTA", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                        }
                        else if (acertoViagem.Pedagios[i].Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito)
                        {
                            Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = repPedagio.BuscarPorCodigo(acertoViagem.Pedagios[i].Pedagio.Codigo, true);

                            if (pedagio.SituacaoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado)
                            {
                                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito?.Codigo ?? 0);
                                servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito, acertoViagem.DataFinal.Value, pedagio.Valor, acertoViagem.Numero.ToString(), "REVERSÃO PEDAGIO DE CRÉDIDO LANÇADO NO ACERTO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                            }

                            pedagio.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Lancado;
                            pedagio.DataAlteracao = DateTime.Now;
                            pedagio.FechamentoPedagio = null;
                            repPedagio.Atualizar(pedagio);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedagio, pedagio.GetChanges(), "Revertido pelo acerto de viagem", unitOfWork);
                        }
                    }

                    //Comissão do motorista
                    if (receita[0].ComissaoMotorista > 0)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoReversaoComissaoDoMotorista?.Codigo ?? 0);
                        servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoReversaoComissaoDoMotorista, acertoViagem.DataFinal.Value, receita[0].ComissaoMotorista, acertoViagem.Numero.ToString(), "REVERSÃO COMISSAO LANÇADA NO ACERTO PAGA AO MOTORISTA", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                    }

                    if (receita[0].PremioComissaoMotorista > 0)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoReversaoComissaoDoMotorista?.Codigo ?? 0);
                        servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoReversaoComissaoDoMotorista, acertoViagem.DataFinal.Value, receita[0].PremioComissaoMotorista, acertoViagem.Numero.ToString(), "REVERSÃO COMISSAO DE PREMIASSÃO LANÇADA NO ACERTO PAGA AO MOTORISTA", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                    }

                    //Diárias
                    if (acertoViagem.Diarias != null && acertoViagem.Diarias.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Fatura.Justificativa> justificativas = repAcertoDiaria.BuscarJustificativas(codigo);
                        if (justificativas != null)
                        {
                            for (int i = 0; i < justificativas.Count; i++)
                            {
                                if (justificativas[i] != null && justificativas[i].TipoMovimentoReversaoUsoJustificativa != null)
                                {
                                    decimal valorPorJustificativa = repAcertoDiaria.BuscarValorPorJustificativa(justificativas[i].Codigo, codigo);
                                    if (valorPorJustificativa > 0)
                                    {
                                        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(justificativas[i]?.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                        servProcessoMovimento.GerarMovimentacao(justificativas[i].TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, valorPorJustificativa, acertoViagem.Numero.ToString(), "REVERSÃO DIÁRIA AGRUPADA PAGA AO MOTORISTA " + (acertoViagem.Motorista?.Nome ?? ""), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                    }
                                }
                            }
                        }

                        //for (int i = 0; i < acertoViagem.Diarias.Count(); i++)
                        //{
                        //    if (acertoViagem.Diarias[i].Valor > 0 && acertoViagem.Diarias[i].Justificativa != null && acertoViagem.Diarias[i].Justificativa.TipoMovimentoReversaoUsoJustificativa != null)
                        //    {
                        //        Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.Diarias[i].Justificativa.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                        //        if (acertoViagem.Diarias[i].TabelaDiariaPeriodo?.TabelaDiaria?.GerarMovimentoSaidaFixaMotorista ?? false)
                        //            servProcessoMovimento.GerarMovimentacao(acertoViagem.Diarias[i].Justificativa.TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, acertoViagem.Diarias[i].Valor, acertoViagem.Numero.ToString(), "REVERSÃO DIÁRIA PAGA AO MOTORISTA " + acertoViagem.Diarias[i].Descricao + " " + (acertoViagem.Motorista?.Nome ?? ""), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                        //        else
                        //            servProcessoMovimento.GerarMovimentacao(acertoViagem.Diarias[i].Justificativa.TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, acertoViagem.Diarias[i].Valor, acertoViagem.Numero.ToString(), "REVERSÃO DIÁRIA PAGA AO MOTORISTA " + acertoViagem.Diarias[i].Descricao + " " + (acertoViagem.Motorista?.Nome ?? ""), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                        //    }
                        //}
                    }

                    //Bonificações
                    for (int i = 0; i < acertoViagem.Bonificacoes.Count(); i++)
                    {
                        if (acertoViagem.Bonificacoes[i].Justificativa != null)
                        {
                            if (acertoViagem.Bonificacoes[i].Justificativa.GerarMovimentoAutomatico)
                                if (acertoViagem.Bonificacoes[i].Justificativa.TipoMovimentoReversaoUsoJustificativa != null)
                                {
                                    Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.Bonificacoes[i].Justificativa.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                    servProcessoMovimento.GerarMovimentacao(acertoViagem.Bonificacoes[i].Justificativa.TipoMovimentoReversaoUsoJustificativa, acertoViagem.Bonificacoes[i].Data, acertoViagem.Bonificacoes[i].ValorBonificacao, acertoViagem.Numero.ToString(), "REVERSÃO BONIFICAÇÃO LANÇADA NO ACERTO PAGO AO MOTORISTA " + acertoViagem.Bonificacoes[i].Motivo, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                }
                                else
                                    return new JsonpResult(false, "Não possui configuração financeira para os lançamentos das bonificações.");
                        }
                    }

                    //Descontos
                    for (int i = 0; i < acertoViagem.Descontos.Count(); i++)
                    {
                        if (acertoViagem.Descontos[i].Justificativa != null)
                        {
                            if (acertoViagem.Descontos[i].Justificativa.GerarMovimentoAutomatico)
                                if (acertoViagem.Descontos[i].Justificativa.TipoMovimentoReversaoUsoJustificativa != null)
                                {
                                    Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.Descontos[i].Justificativa.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                    servProcessoMovimento.GerarMovimentacao(acertoViagem.Descontos[i].Justificativa.TipoMovimentoReversaoUsoJustificativa, acertoViagem.Descontos[i].Data, acertoViagem.Descontos[i].ValorDesconto, acertoViagem.Numero.ToString(), "REVERSÃO DESCONTO LANÇADA NO ACERTO DESCONTADO AO MOTORISTA " + acertoViagem.Descontos[i].Motivo, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                }
                                else
                                    return new JsonpResult(false, "Não possui configuração financeira para os lançamentos dos descontos.");
                        }
                    }

                    //Devoluções
                    for (int i = 0; i < acertoViagem.DevolucoesMoedaEstrangeira.Count(); i++)
                    {
                        if (acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa != null)
                        {
                            if (acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa.GerarMovimentoAutomatico)
                                if (acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa.TipoMovimentoReversaoUsoJustificativa != null)
                                {
                                    Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                    servProcessoMovimento.GerarMovimentacao(acertoViagem.DevolucoesMoedaEstrangeira[i].Justificativa.TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, acertoViagem.DevolucoesMoedaEstrangeira[i].ValorDevolucao, acertoViagem.Numero.ToString(), "REVERSÃO DA DEVOLUÇÃO NO ACERTO DO MOTORISTA DA MOEDA " + acertoViagem.DevolucoesMoedaEstrangeira[i].MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                }
                                else
                                    return new JsonpResult(false, "Não possui configuração financeira para os lançamentos das devoluções.");
                        }
                    }

                    //Variações
                    for (int i = 0; i < acertoViagem.VariacoesCambial.Count(); i++)
                    {
                        if (acertoViagem.VariacoesCambial[i].Justificativa != null)
                        {
                            if (acertoViagem.VariacoesCambial[i].Justificativa.GerarMovimentoAutomatico)
                                if (acertoViagem.VariacoesCambial[i].Justificativa.TipoMovimentoReversaoUsoJustificativa != null)
                                {
                                    Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(acertoViagem.VariacoesCambial[i].Justificativa.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                    servProcessoMovimento.GerarMovimentacao(acertoViagem.VariacoesCambial[i].Justificativa.TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, acertoViagem.VariacoesCambial[i].ValorVariacao, acertoViagem.Numero.ToString(), "REVERSÃO DA VARIAÇÃO CAMBIAL NO ACERTO DO MOTORISTA DA MOEDA " + acertoViagem.VariacoesCambial[i].MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada(), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                }
                                else
                                    return new JsonpResult(false, "Não possui configuração financeira para os lançamentos das variações cambiais.");
                        }
                    }

                    //Bonificações lançadas pelo cliente
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao> listaBonificacoesClientes = repAcertoCargaBonificacao.BuscarPorAcerto(acertoViagem.Codigo);
                    for (int i = 0; i < listaBonificacoesClientes.Count(); i++)
                    {
                        if (listaBonificacoesClientes[i].Justificativa != null)
                        {
                            if (listaBonificacoesClientes[i].Justificativa.GerarMovimentoAutomatico)
                                if (listaBonificacoesClientes[i].Justificativa.TipoMovimentoReversaoUsoJustificativa != null)
                                {
                                    Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(listaBonificacoesClientes[i].Justificativa.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                    servProcessoMovimento.GerarMovimentacao(listaBonificacoesClientes[i].Justificativa.TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, listaBonificacoesClientes[i].Valor, acertoViagem.Numero.ToString(), "REVERSÃO BONIFICAÇÃO DO CLIENTE LANÇADA NO ACERTO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                }
                                else
                                    return new JsonpResult(false, "Não possui configuração financeira para os lançamentos das bonificações do cliente.");
                        }
                    }

                    //Pedágios lançadas pelo cliente
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio> listaAcertoCargaPedagio = repAcertoCargaPedagio.BuscarPorAcerto(acertoViagem.Codigo);
                    for (int i = 0; i < listaAcertoCargaPedagio.Count(); i++)
                    {
                        if (listaAcertoCargaPedagio[i].Justificativa != null)
                        {
                            if (listaAcertoCargaPedagio[i].Justificativa.GerarMovimentoAutomatico)
                                if (listaAcertoCargaPedagio[i].Justificativa.TipoMovimentoReversaoUsoJustificativa != null)
                                {
                                    Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(listaAcertoCargaPedagio[i].Justificativa.TipoMovimentoReversaoUsoJustificativa?.Codigo ?? 0);
                                    servProcessoMovimento.GerarMovimentacao(listaAcertoCargaPedagio[i].Justificativa.TipoMovimentoReversaoUsoJustificativa, acertoViagem.DataFinal.Value, listaAcertoCargaPedagio[i].Valor, acertoViagem.Numero.ToString(), "REVERSÃO PEDÁGIO DO CLIENTE LANÇADA NO ACERTO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, null, null, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                                }
                                else
                                    return new JsonpResult(false, "Não possui configuração financeira para os lançamentos dos pedágios do cliente.");
                        }
                    }

                    if (!repAcertoViagem.ContemAcertoFechadoMaior(acertoViagem.Motorista?.Codigo ?? 0, acertoViagem.Codigo))
                        acertoViagem.Motorista.DataFechamentoAcerto = null;

                    repUsuario.Atualizar(acertoViagem.Motorista);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagem.Motorista, null, "Estornou o acerto de viagem.", unitOfWork);

                    string msgRetorno = "";
                    if (!(configuraoAcertoViagem?.HabilitarFormaRecebimentoTituloAoMotorista ?? false) || acertoViagem.FormaRecebimentoMotoristaAcerto == FormaRecebimentoMotoristaAcerto.DescontarFicha)
                    {
                        if (!servAcertoViagem.EstornarControleSaldoMotorista(ref msgRetorno, acertoViagem, unitOfWork, this.Usuario, Auditado, TipoServicoMultisoftware, _conexao.StringConexao, ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, receita[0].SaldoMotorista))
                            return new JsonpResult(false, msgRetorno);
                    }
                    else if ((configuraoAcertoViagem?.HabilitarFormaRecebimentoTituloAoMotorista ?? false) && acertoViagem.ValorMovimentadoFichaMotorista != 0m)
                    {
                        if (!servAcertoViagem.EstornarControleSaldoMotorista(ref msgRetorno, acertoViagem, unitOfWork, this.Usuario, Auditado, TipoServicoMultisoftware, _conexao.StringConexao, ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, acertoViagem.ValorMovimentadoFichaMotorista))
                            return new JsonpResult(false, msgRetorno);
                    }
                    acertoViagem.DataFechamento = null;
                    repAcertoViagem.Atualizar(acertoViagem);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagem, null, "Fechou o acerto de viagem " + acertoViagem.Descricao + ".", unitOfWork);

                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Fechamento, Usuario);

                unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha processar o fechamento do acerto. " + ex.Message);
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AtualizarFechamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarTacografo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarTacografo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Models.Grid.EditableCell editableAtingiuMedia = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aBool);

                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código", "CodigoTacografo", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Excesso de Velocidade", "Excesso", 15, Models.Grid.Align.center, false, false, false, false, true, editableAtingiuMedia);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Acerto.AcertoViagemTacografo repAcertoViagemTacografo = new Repositorio.Embarcador.Acerto.AcertoViagemTacografo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo> listaAcertoViagemTacografo = repAcertoViagemTacografo.BuscarPorAcerto(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoViagemTacografo.ContarBuscarPorAcerto(codigoAcerto));
                var dynRetorno = from obj in listaAcertoViagemTacografo
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoTacografo = obj.ControleTacografo.Codigo,
                                     Motorista = obj.ControleTacografo.Motorista?.Descricao ?? "",
                                     Veiculo = obj.ControleTacografo.Veiculo?.Descricao ?? "",
                                     Excesso = obj.ControleTacografo.Excesso,
                                     DT_RowColor = "#FFFFFF"
                                 };

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarTacografo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        private void ArmazenarArquivoAssinatura(string assinatura, string caminho, out string guid)
        {
            string extensao = ".png";

            assinatura = assinatura.Split(',')[1];
            byte[] data = Convert.FromBase64String(assinatura);

            string token = Guid.NewGuid().ToString().Replace("-", "");
            guid = token;

            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guid + extensao);

            using (MemoryStream ms = new MemoryStream(data))
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
            {
                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, image);
            }
        }

        public void DeletarAssinatura(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string caminho)
        {
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{acertoViagem.GuidAssinatura}.png");
            if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);
            acertoViagem.GuidAssinatura = "";
        }
        public string ObterAssinaturaBase64(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string caminho)
        {
            if (acertoViagem == null || acertoViagem.GuidAssinatura == null)
                return "";
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{acertoViagem.GuidAssinatura}.png");
            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                return "";
            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            return "data:image/png;base64," + base64ImageRepresentation;
        }


        public async Task<IActionResult> ObterAssinatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem domAcertoViagem;
                if (codigo > 0)
                    domAcertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, true, "Acerto de viagem não encontrado.");

                if (string.IsNullOrEmpty(domAcertoViagem.GuidAssinatura))
                {
                    var retorno = new
                    {
                        Assinatura = "",
                        PossuiAssinatura = false
                    };
                    return new JsonpResult(retorno);
                }
                else
                {
                    var retorno = new
                    {
                        Assinatura = ObterAssinaturaBase64(domAcertoViagem, Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "AcertoViagem", "Assinatura" })),
                        PossuiAssinatura = true
                    };
                    return new JsonpResult(retorno);
                }

            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter assinatura.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }



        public async Task<IActionResult> SalvarAssinatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem domAcertoViagem;
                if (codigo > 0)
                    domAcertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, true, "Por favor inicie o acerto de viagem antes.");

                string assinatura = Request.GetStringParam("Assinatura");
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "AcertoViagem", "Assinatura" });



                if (string.IsNullOrWhiteSpace(assinatura))
                {
                    if (!string.IsNullOrEmpty(domAcertoViagem.GuidAssinatura))
                        DeletarAssinatura(domAcertoViagem, caminho);
                }
                else
                {
                    if (string.IsNullOrEmpty(assinatura))
                    {//limpar assinatura
                        domAcertoViagem.GuidAssinatura = "";
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, domAcertoViagem, "Excluiu a assinatura.", unitOfWork);
                    }
                    else
                    {// armazena assinatura
                        ArmazenarArquivoAssinatura(assinatura, caminho, out string guid);
                        domAcertoViagem.GuidAssinatura = guid;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, domAcertoViagem, "Adicionou a assinatura.", unitOfWork);
                    }
                }
                repAcertoViagem.Atualizar(domAcertoViagem);
                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarAdiantamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarAdiantamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Models.Grid.EditableCell editableAtingiuMedia = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aBool);

                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "DataPagamento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "PagamentoMotoristaTipo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("ValorMoedaCotacao", false);
                grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                propOrdenar = "PagamentoMotoristaTMS." + propOrdenar;

                Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento> listaAcertoAdiantamento = repAcertoAdiantamento.BuscarPorAcerto(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoAdiantamento.ContarBuscarPorAcerto(codigoAcerto));
                var dynRetorno = from obj in listaAcertoAdiantamento
                                 select new
                                 {
                                     Codigo = obj.PagamentoMotoristaTMS.Codigo,
                                     Numero = obj.PagamentoMotoristaTMS.Numero.ToString("n0"),
                                     DataPagamento = obj.PagamentoMotoristaTMS.DataPagamento.ToString("dd/MM/yyyy"),
                                     Motorista = obj.PagamentoMotoristaTMS.Motorista.Nome,
                                     PagamentoMotoristaTipo = obj.PagamentoMotoristaTMS.PagamentoMotoristaTipo?.Descricao ?? "",
                                     MoedaCotacaoBancoCentral = obj.PagamentoMotoristaTMS.MoedaCotacaoBancoCentral.HasValue ? obj.PagamentoMotoristaTMS.MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada() : "Reais",
                                     ValorMoedaCotacao = obj.PagamentoMotoristaTMS?.ValorMoedaCotacao.ToString("n2") ?? "",
                                     ValorOriginalMoedaEstrangeira = obj.PagamentoMotoristaTMS?.ValorOriginalMoedaEstrangeira.ToString("n2") ?? "",
                                     Valor = obj.PagamentoMotoristaTMS.Valor.ToString("n2")
                                 };

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarAdiantamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> InserirControleTacografo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirControleTacografo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto, codigoControleTacografo;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);
                int.TryParse(Request.Params("ControleTacografo"), out codigoControleTacografo);
                bool houveExcesso = Request.GetBoolParam("HouveExcesso");

                Repositorio.Embarcador.Acerto.AcertoViagemTacografo repAcertoViagemTacografo = new Repositorio.Embarcador.Acerto.AcertoViagemTacografo(unitOfWork);
                Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo acertoViagemTacografo = new Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo();
                acertoViagemTacografo.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                acertoViagemTacografo.ControleTacografo = repControleTacografo.BuscarPorCodigo(codigoControleTacografo, false);
                acertoViagemTacografo.Excesso = houveExcesso;
                acertoViagemTacografo.ControleTacografo.Excesso = houveExcesso;

                repControleTacografo.Atualizar(acertoViagemTacografo.ControleTacografo);
                repAcertoViagemTacografo.Inserir(acertoViagemTacografo, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagemTacografo.AcertoViagem, null, "Adicionou o tacógrafo " + acertoViagemTacografo.ControleTacografo.Descricao + " ao acerto.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir o tacógrafo.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirControleTacografo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> RemoverControleTacografo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverControleTacografo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoViagemTacografo repAcertoViagemTacografo = new Repositorio.Embarcador.Acerto.AcertoViagemTacografo(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo acertoViagemTacografo = repAcertoViagemTacografo.BuscarPorCodigo(codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagemTacografo.AcertoViagem, null, "Removeu o tacógrafo " + acertoViagemTacografo.ControleTacografo.Descricao + " do acerto.", unitOfWork);
                repAcertoViagemTacografo.Deletar(acertoViagemTacografo, Auditado);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o tacógrafo.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverControleTacografo " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> AlterarDadosTacografo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Acerto.AcertoViagemTacografo repAcertoViagemTacografo = new Repositorio.Embarcador.Acerto.AcertoViagemTacografo(unitOfWork);
            Repositorio.Embarcador.Frota.ControleTacografo repControleTacografo = new Repositorio.Embarcador.Frota.ControleTacografo(unitOfWork);

            try
            {
                unitOfWork.Start();

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                bool excesso = Request.GetBoolParam("Excesso");

                Dominio.Entidades.Embarcador.Acerto.AcertoViagemTacografo acertoViagemTacografo = repAcertoViagemTacografo.BuscarPorCodigo(codigo);
                acertoViagemTacografo.Excesso = excesso;
                acertoViagemTacografo.ControleTacografo.Excesso = excesso;

                repAcertoViagemTacografo.Atualizar(acertoViagemTacografo);
                repControleTacografo.Atualizar(acertoViagemTacografo.ControleTacografo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagemTacografo.AcertoViagem, null, "Alterou informação do excesso de velocidade do tacógrafo " + acertoViagemTacografo.ControleTacografo.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o excesso de velocidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarDesconto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAcerto", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Motivo", "Motivo", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 20, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoDesconto> listaAcertoDesconto = repAcertoDesconto.BuscarPorAcerto(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoDesconto.ContarBuscarPorAcerto(codigoAcerto));
                var dynRetorno = from obj in listaAcertoDesconto
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoAcerto = obj.AcertoViagem.Codigo,
                                     Data = obj.Data.ToString("dd/MM/yyyy"),
                                     Valor = obj.ValorDesconto.ToString("n2"),
                                     Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                     obj.Motivo,
                                     DT_RowColor = "#FFFFFF"
                                 };

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarDesconto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDescontoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoDesconto acertoDesconto = repAcertoDesconto.BuscarPorCodigo(codigo);

                if (acertoDesconto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynAcertoDesconto = new
                {
                    acertoDesconto.Codigo,
                    Veiculo = new { acertoDesconto.Veiculo.Codigo, Descricao = acertoDesconto.Veiculo.Placa },
                    Data = acertoDesconto.Data.ToDateTimeString(),
                    Valor = acertoDesconto.ValorDesconto,
                    Observacao = acertoDesconto.Motivo,
                    Justificativa = new { acertoDesconto.Justificativa.Codigo, acertoDesconto.Justificativa.Descricao },
                    MoedaCotacaoBancoCentral = acertoDesconto.MoedaCotacaoBancoCentral,
                    DataBaseCRT = acertoDesconto.DataBaseCRT?.ToDateTimeString() ?? string.Empty,
                    acertoDesconto.ValorMoedaCotacao,
                    acertoDesconto.ValorOriginalMoedaEstrangeira
                };

                return new JsonpResult(dynAcertoDesconto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarBonificacaoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao acertoBonificacao = repAcertoBonificacao.BuscarPorCodigo(codigo);

                if (acertoBonificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynAcertoBonificacao = new
                {
                    acertoBonificacao.Codigo,
                    Veiculo = new { acertoBonificacao.Veiculo.Codigo, Descricao = acertoBonificacao.Veiculo.Placa },
                    Data = acertoBonificacao.Data.ToDateTimeString(),
                    Valor = acertoBonificacao.ValorBonificacao.ToString("n2"),
                    Observacao = acertoBonificacao.Motivo,
                    Justificativa = new { acertoBonificacao.Justificativa.Codigo, acertoBonificacao.Justificativa.Descricao },
                    MoedaCotacaoBancoCentral = acertoBonificacao.MoedaCotacaoBancoCentral,
                    DataBaseCRT = acertoBonificacao.DataBaseCRT?.ToDateTimeString() ?? string.Empty,
                    ValorMoedaCotacao = acertoBonificacao.ValorMoedaCotacao.ToString("n10"),
                    ValorOriginalMoedaEstrangeira = acertoBonificacao.ValorOriginalMoedaEstrangeira.ToString("n2")
                };

                return new JsonpResult(dynAcertoBonificacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarDescontoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoJustificativa = Request.GetIntParam("Justificativa");

                Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoDesconto desconto = repAcertoDesconto.BuscarPorCodigo(codigo);

                if (desconto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                desconto.Data = Request.GetDateTimeParam("Data");
                desconto.Motivo = Request.GetStringParam("Observacao");
                desconto.ValorDesconto = Request.GetDecimalParam("Valor");
                if (codigoVeiculo > 0)
                    desconto.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                if (codigoJustificativa > 0)
                    desconto.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                desconto.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                desconto.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                desconto.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                desconto.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                repAcertoDesconto.Atualizar(desconto, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, desconto.AcertoViagem, null, "Atualizou o desconto " + desconto.Descricao, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarBonificacaoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoJustificativa = Request.GetIntParam("Justificativa");

                Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao bonificacao = repAcertoBonificacao.BuscarPorCodigo(codigo);

                if (bonificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                bonificacao.Data = Request.GetDateTimeParam("Data");
                bonificacao.Motivo = Request.GetStringParam("Observacao");
                bonificacao.ValorBonificacao = Request.GetDecimalParam("Valor");
                if (codigoVeiculo > 0)
                    bonificacao.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                if (codigoJustificativa > 0)
                    bonificacao.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                bonificacao.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                bonificacao.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                bonificacao.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                bonificacao.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                repAcertoBonificacao.Atualizar(bonificacao, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, bonificacao.AcertoViagem, null, "Atualizou a Bonificação " + bonificacao.Descricao, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a Bonificação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarDevolucoesMoedaEstrangeira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarDevolucoesMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoDevolucao", false);
                grid.AdicionarCabecalho("CodigoAcerto", false);
                grid.AdicionarCabecalho("CodigoMoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("CodigoJustificativa", false);
                grid.AdicionarCabecalho("Justificativa", false);
                grid.AdicionarCabecalho("DataBaseCRT", false);
                grid.AdicionarCabecalho("Moeda", "MoedaCotacaoBancoCentral", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor R$", "ValorOriginal", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Cotação", "ValorMoedaCotacao", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor", "ValorDevolucao", 15, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira repAcertoDevolucaoMoedaEstrangeira = new Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira(unitOfWork);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira> listaAcertoDevolucaoMoedaEstrangeira = repAcertoDevolucaoMoedaEstrangeira.BuscarPorAcerto(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoDevolucaoMoedaEstrangeira.ContarBuscarPorAcerto(codigoAcerto));
                var dynRetorno = from obj in listaAcertoDevolucaoMoedaEstrangeira
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoDevolucao = obj.Codigo,
                                     CodigoAcerto = obj.AcertoViagem.Codigo,
                                     CodigoJustificativa = obj.Justificativa?.Codigo ?? 0,
                                     Justificativa = obj.Justificativa?.Descricao ?? "",
                                     CodigoMoedaCotacaoBancoCentral = obj.MoedaCotacaoBancoCentral.HasValue ? obj.MoedaCotacaoBancoCentral.Value : MoedaCotacaoBancoCentral.Real,
                                     MoedaCotacaoBancoCentral = obj.MoedaCotacaoBancoCentral.HasValue ? obj.MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada() : "Reais",
                                     ValorDevolucao = obj.ValorDevolucao.ToString("n2"),
                                     DataBaseCRT = obj.DataBaseCRT.HasValue ? obj.DataBaseCRT.Value.ToString("dd/MM/yyyy") : "",
                                     ValorMoedaCotacao = obj.ValorMoedaCotacao.ToString("n10"),
                                     ValorOriginal = obj.ValorOriginal.ToString("n2"),
                                     DT_RowColor = "#FFFFFF"
                                 };

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarDevolucoesMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarVariacaoCambial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarVariacaoCambial " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoVariacao", false);
                grid.AdicionarCabecalho("CodigoAcerto", false);
                grid.AdicionarCabecalho("CodigoMoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("CodigoJustificativa", false);
                grid.AdicionarCabecalho("Justificativa", false);
                grid.AdicionarCabecalho("DataBaseCRT", false);
                grid.AdicionarCabecalho("Moeda", "MoedaCotacaoBancoCentral", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor R$", "ValorOriginal", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Cotação", "ValorMoedaCotacao", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor", "ValorVariacao", 15, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Acerto.AcertoVariacaoCambial repAcertoVariacaoCambial = new Repositorio.Embarcador.Acerto.AcertoVariacaoCambial(unitOfWork);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial> listaAcertoVariacaoCambial = repAcertoVariacaoCambial.BuscarPorAcerto(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoVariacaoCambial.ContarBuscarPorAcerto(codigoAcerto));
                var dynRetorno = from obj in listaAcertoVariacaoCambial
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoVariacao = obj.Codigo,
                                     CodigoAcerto = obj.AcertoViagem.Codigo,
                                     CodigoJustificativa = obj.Justificativa?.Codigo ?? 0,
                                     Justificativa = obj.Justificativa?.Descricao ?? "",
                                     CodigoMoedaCotacaoBancoCentral = obj.MoedaCotacaoBancoCentral.HasValue ? obj.MoedaCotacaoBancoCentral.Value : MoedaCotacaoBancoCentral.Real,
                                     MoedaCotacaoBancoCentral = obj.MoedaCotacaoBancoCentral.HasValue ? obj.MoedaCotacaoBancoCentral.Value.ObterDescricaoSimplificada() : "Reais",
                                     ValorVariacao = obj.ValorVariacao.ToString("n2"),
                                     DataBaseCRT = obj.DataBaseCRT.HasValue ? obj.DataBaseCRT.Value.ToString("dd/MM/yyyy") : "",
                                     ValorMoedaCotacao = obj.ValorMoedaCotacao.ToString("n10"),
                                     ValorOriginal = obj.ValorOriginal.ToString("n2"),
                                     DT_RowColor = "#FFFFFF"
                                 };

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarVariacaoCambial " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarDetalheMoedaEstrangeira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarDetalheMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0, tipoConsulta = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("Tipo"), out tipoConsulta);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAcerto", false);
                grid.AdicionarCabecalho("Data", "Data", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vlr. R$", "ValorReais", 9, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Moeda", "Moeda", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Vlr. Moeda", "ValorMoeda", 9, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Vlr. Total Moeda", "ValorTotalMoeda", 9, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                IList<Dominio.ObjetosDeValor.Embarcador.Acertos.DetalheMoedaEstrangeira> listaDetalhe = repAcertoViagem.DetalheMoedaEstrangeira(codigoAcerto, tipoConsulta, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoViagem.ContarDetalheMoedaEstrangeira(codigoAcerto, tipoConsulta));
                var dynRetorno = from obj in listaDetalhe
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoAcerto = codigoAcerto,
                                     Data = obj.Data.ToString("dd/MM/yyyy HH:mm"),
                                     obj.Pessoa,
                                     obj.Descricao,
                                     ValorReais = obj.ValorReais.ToString("n2"),
                                     Moeda = obj.Moeda.ObterDescricaoSimplificada(),
                                     ValorMoeda = obj.ValorMoeda.ToString("n10"),
                                     ValorTotalMoeda = obj.ValorTotalMoeda.ToString("n2"),
                                     DT_RowColor = "#FFFFFF"
                                 };

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarDetalheMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> TotaisDetalheMoedaEstrangeira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller TotaisDetalheMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0, tipoConsulta = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("Tipo"), out tipoConsulta);

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Acertos.TotalMoedaEstrangeira totalMoedaEstrangeira = repAcertoViagem.DetalheMoedaEstrangeira(codigoAcerto, tipoConsulta);
                var dynRetorno = new
                {
                    TotalReais = totalMoedaEstrangeira != null ? totalMoedaEstrangeira.TotalReais.ToString("n2") : "0,00",
                    TotalMoeda = totalMoedaEstrangeira != null ? totalMoedaEstrangeira.TotalMoeda.ToString("n2") : "0,00"
                };

                return new JsonpResult(dynRetorno, true, "Sucesso");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller TotaisDetalheMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarBonificacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarBonificacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAcerto", false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motivo", "Motivo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao> listaAcertoBonificacaoo = repAcertoBonificacao.BuscarPorAcerto(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoBonificacao.ContarBuscarPorAcerto(codigoAcerto));
                var dynRetorno = from obj in listaAcertoBonificacaoo
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoAcerto = obj.AcertoViagem.Codigo,
                                     Data = obj.Data.ToString("dd/MM/yyyy"),
                                     Valor = obj.ValorBonificacao.ToString("n2"),
                                     Tipo = obj.TipoBonificacao != null ? obj.TipoBonificacao.Descricao : string.Empty,
                                     obj.Motivo,
                                     Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty,
                                     DT_RowColor = "#FFFFFF"
                                 };

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarBonificacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller PesquisarVeiculos " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAcerto", false);
                grid.AdicionarCabecalho("Placa", "Placa", 45, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Reboques", "Reboques", 60, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("KM Inicial", "KmInicial", 45, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Placa")
                    propOrdenar = "Placa";

                Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);

                List<Dominio.Entidades.Veiculo> listaVeiculo = repAcertoVeiculo.BuscarVeiculoPorAcerto(codigoAcerto, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAcertoVeiculo.ContarBuscarVeiculoPorAcerto(codigoAcerto));
                var dynRetorno = (from obj in listaVeiculo
                                  select new
                                  {
                                      obj.Codigo,
                                      CodigoAcerto = codigoAcerto,
                                      Placa = obj.Placa,
                                      Reboques = obj.VeiculosVinculados != null ? string.Join(", ", (from p in obj.VeiculosVinculados select p.Placa)) : string.Empty,
                                      KmInicial = repAcertoResumoAbastecimento.BuscarKMInicialPorCodigoAcertoVeiculoTipo(codigoAcerto, obj.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel).ToString("n0"),
                                      DT_RowColor = "#FFFFFF"
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller PesquisarVeiculos " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> InserirBonificacaoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirBonificacaoMotorista " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteAdicionarBonificacao)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para adicionar bonificação ao motorista.");

                int codigoAcerto, codigoMotorista, codigoVeiculo, codigoJustificativa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("CodigoMotorista"), out codigoMotorista);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                DateTime data;
                DateTime.TryParse(Request.Params("Data"), out data);

                decimal valor = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao bonificacao = new Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao();
                bonificacao.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                bonificacao.Data = data;
                bonificacao.Motivo = observacao;
                bonificacao.ValorBonificacao = valor;
                if (codigoVeiculo > 0)
                    bonificacao.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                if (codigoJustificativa > 0)
                    bonificacao.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                bonificacao.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                bonificacao.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                bonificacao.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                bonificacao.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                repAcertoBonificacao.Inserir(bonificacao, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, bonificacao.AcertoViagem, null, "Adicionou a bonificação " + bonificacao.Descricao + " ao acerto.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir nova bonificação.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirBonificacaoMotorista " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> InserirDescontoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirDescontoMotorista " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteAdicionarDesconto)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para adicionar desconto ao motorista.");

                int codigoAcerto, codigoMotorista, codigoVeiculo, codigoJustificativa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("CodigoMotorista"), out codigoMotorista);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                DateTime data;
                DateTime.TryParse(Request.Params("Data"), out data);

                decimal valor = 0;
                decimal.TryParse(Request.Params("Valor"), out valor);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoDesconto desconto = new Dominio.Entidades.Embarcador.Acerto.AcertoDesconto();
                desconto.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                desconto.Data = data;
                desconto.Motivo = observacao;
                desconto.ValorDesconto = valor;
                if (codigoVeiculo > 0)
                    desconto.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                if (codigoJustificativa > 0)
                    desconto.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                desconto.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                desconto.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                desconto.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                desconto.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");

                repAcertoDesconto.Inserir(desconto, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, desconto.AcertoViagem, null, "Adicionou o desconto " + desconto.Descricao + " ao acerto.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir novo desconto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirDescontoMotorista " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> InserirDevolucoesMoedaEstrangeira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirDevolucoesMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto, codigoJustificativa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("CodigoDevolucao"), out int codigoDevolucao);
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                decimal valorDevolucao = 0;
                decimal.TryParse(Request.Params("ValorDevolucao"), out valorDevolucao);

                Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira repAcertoDevolucaoMoedaEstrangeira = new Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira devolucao = null;
                if (codigoDevolucao > 0)
                    devolucao = repAcertoDevolucaoMoedaEstrangeira.BuscarPorCodigo(codigoDevolucao, true);
                else
                    devolucao = new Dominio.Entidades.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira();
                devolucao.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                devolucao.ValorDevolucao = valorDevolucao;
                devolucao.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                if (codigoJustificativa > 0)
                    devolucao.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                devolucao.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                devolucao.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                devolucao.ValorOriginal = Request.GetDecimalParam("ValorOriginal");

                if (codigoDevolucao > 0)
                    repAcertoDevolucaoMoedaEstrangeira.Atualizar(devolucao, Auditado);
                else
                    repAcertoDevolucaoMoedaEstrangeira.Inserir(devolucao, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, devolucao.AcertoViagem, null, "Adicionou a devolução do motorista " + devolucao.Descricao + " ao acerto.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir a devolução do motorista.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirDevolucoesMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> InserirVariacaoCambial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirVariacaoCambial " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto, codigoJustificativa = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                int.TryParse(Request.Params("CodigoVariacao"), out int codigoVariacao);
                int.TryParse(Request.Params("Justificativa"), out codigoJustificativa);

                decimal valorVariacao = 0;
                decimal.TryParse(Request.Params("ValorVariacao"), out valorVariacao);

                Repositorio.Embarcador.Acerto.AcertoVariacaoCambial repAcertoVariacaoCambial = new Repositorio.Embarcador.Acerto.AcertoVariacaoCambial(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial variacao = null;
                if (codigoVariacao > 0)
                    variacao = repAcertoVariacaoCambial.BuscarPorCodigo(codigoVariacao, true);
                else
                    variacao = new Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial();
                variacao.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                variacao.ValorVariacao = valorVariacao;
                variacao.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                if (codigoJustificativa > 0)
                    variacao.Justificativa = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                variacao.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
                variacao.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
                variacao.ValorOriginal = Request.GetDecimalParam("ValorOriginal");

                if (codigoVariacao > 0)
                    repAcertoVariacaoCambial.Atualizar(variacao, Auditado);
                else
                    repAcertoVariacaoCambial.Inserir(variacao, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, variacao.AcertoViagem, null, "Adicionou a variação cambial " + variacao.Descricao + " ao acerto.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir a variação cambial.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirVariacaoCambial " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> InserirConversaoMoedaEstrangeira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirConversaoMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);

                Repositorio.Embarcador.Acerto.AcertoConversaoMoeda repAcertoConversaoMoeda = new Repositorio.Embarcador.Acerto.AcertoConversaoMoeda(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                MoedaCotacaoBancoCentral moedaCotacaoBancoCentralDestino = Request.GetEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentralDestino");
                MoedaCotacaoBancoCentral moedaCotacaoBancoCentralOrigem = Request.GetEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentralOrigem");
                bool somarValores = false;

                Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda conversao = repAcertoConversaoMoeda.BuscarPorAcertoeMoeda(codigoAcerto, moedaCotacaoBancoCentralDestino, moedaCotacaoBancoCentralOrigem);
                if (conversao == null)
                    conversao = new Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda();
                else
                    somarValores = conversao.ValorCotacao != Request.GetDecimalParam("ValorCotacao");

                conversao.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                conversao.MoedaCotacaoBancoCentralDestino = moedaCotacaoBancoCentralDestino;
                conversao.MoedaCotacaoBancoCentralOrigem = moedaCotacaoBancoCentralOrigem;
                conversao.ValorCotacao = somarValores ? ((conversao.ValorCotacao + Request.GetDecimalParam("ValorCotacao")) / 2) : Request.GetDecimalParam("ValorCotacao");
                conversao.ValorFinal = somarValores ? (conversao.ValorFinal + Request.GetDecimalParam("ValorFinal")) : Request.GetDecimalParam("ValorFinal");
                conversao.ValorOrigem = somarValores ? (conversao.ValorOrigem + Request.GetDecimalParam("ValorOrigem")) : Request.GetDecimalParam("ValorOrigem");
                if (conversao.MoedaCotacaoBancoCentralDestino == conversao.MoedaCotacaoBancoCentralOrigem)
                    return new JsonpResult(false, "Não é possível realizar a conversão da mesma moeda.");
                if (conversao.ValorFinal <= 0)
                    return new JsonpResult(false, "Favor informe um valor final para a conversão.");
                if (conversao.Codigo > 0)
                    repAcertoConversaoMoeda.Atualizar(conversao);
                else
                    repAcertoConversaoMoeda.Inserir(conversao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, conversao.AcertoViagem, null, "Adicionou a conversão da moeda estrangeira " + conversao.Descricao + " ao acerto.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir a conversão da moeda.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirConversaoMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> LimparConversaoMoedaEstrangeira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller LimparConversaoMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Repositorio.Embarcador.Acerto.AcertoConversaoMoeda repAcertoConversaoMoeda = new Repositorio.Embarcador.Acerto.AcertoConversaoMoeda(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);

                List<Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda> conversoes = repAcertoConversaoMoeda.BuscarPorAcerto(codigoAcerto);
                if (conversoes != null && conversoes.Count > 0)
                {
                    foreach (var conversao in conversoes)
                    {
                        Dominio.Entidades.Embarcador.Acerto.AcertoConversaoMoeda conv = repAcertoConversaoMoeda.BuscarPorCodigo(conversao.Codigo);
                        repAcertoConversaoMoeda.Deletar(conv);
                    }
                }
                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagem, null, "Realizou a limpeza das conversões de moedas.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao limpar as conversões.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller LimparConversaoMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AprovarFolgasAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AprovarFolgasAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                int codigoAcerto = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Repositorio.Embarcador.Acerto.AcertoViagem reoAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = reoAcertoViagem.BuscarPorCodigo(codigoAcerto, true);
                acertoViagem.FolgasAprovadas = true;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acertoViagem, null, "Aprovou as folgas lançadas", unitOfWork);
                reoAcertoViagem.Atualizar(acertoViagem, Auditado);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as foglas.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AprovarFolgasAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> RemoverAdiantamentoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverAdiantamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermitirRemoverAdiantamento)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para remover o adiantamento.");

                int codigo = 0, codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento adiantamento = repAcertoAdiantamento.BuscarPorCodigoPagamentoeAcerto(codigo, codigoAcerto);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, adiantamento.AcertoViagem, null, "Removeu a adiantamento " + adiantamento.Descricao + " " + adiantamento.PagamentoMotoristaTMS.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2") + " do acerto.", unitOfWork);
                repAcertoAdiantamento.Deletar(adiantamento, Auditado);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o adiantamento.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverAdiantamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarAdiantamentoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverAdiantamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                int codigoAcerto = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigoAcerto);


                dynamic adiantamentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Adiantamentos"));
                foreach (var adiant in adiantamentos)
                {
                    int codigoAdiantamento = (int)adiant.Codigo;

                    if (!acerto.Adiantamentos.Any(o => o.PagamentoMotoristaTMS.Codigo == codigoAdiantamento))
                    {
                        Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento adi = new Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento();
                        adi.AcertoViagem = acerto;
                        adi.PagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(codigoAdiantamento);
                        repAcertoAdiantamento.Inserir(adi);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acerto, null, "Adicionou adiantamentos ao acerto.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o adiantamento.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverAdiantamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> RemoverChequeAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverChequeAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                int codigo = 0, codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Repositorio.Embarcador.Acerto.AcertoCheque repAcertoCheque = new Repositorio.Embarcador.Acerto.AcertoCheque(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoCheque cheque = repAcertoCheque.BuscarPorCodigoChequeeAcerto(codigo, codigoAcerto);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cheque.AcertoViagem, null, "Removeu a cheque " + cheque.Descricao + " " + cheque.Cheque.Valor.ToString("n2") + " do acerto.", unitOfWork);
                repAcertoCheque.Deletar(cheque, Auditado);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o cheque.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverChequeAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverFolgaAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverFolgaAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                int codigoAcerto = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                acerto.Initialize();
                acerto.QuantidadeDiasFolga = 0;
                acerto.DataInicioFolga = null;
                acerto.DataFinalFolga = null;


                Servicos.Auditoria.Auditoria.Auditar(Auditado, acerto, null, "Removeu a folga lançada no acerto.", unitOfWork);
                repAcertoViagem.Atualizar(acerto, Auditado);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover as folgas.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverFolgaAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> InserirFolga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller InserirFolga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("Codigo"), out codigoAcerto);
                DateTime? dataInicioFolga = Request.GetNullableDateTimeParam("DataInicioFolga");
                DateTime? dataFinalFolga = Request.GetNullableDateTimeParam("DataFinalFolga");

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigoAcerto);

                if (dataInicioFolga.HasValue && dataFinalFolga.HasValue && dataFinalFolga.Value.Date > dataInicioFolga.Value.Date)
                {
                    acerto.Initialize();
                    acerto.DataInicioFolga = dataInicioFolga;
                    acerto.DataFinalFolga = dataFinalFolga;
                    acerto.QuantidadeDiasFolga = (int)(dataFinalFolga - dataInicioFolga).Value.TotalDays;

                    repAcertoViagem.Atualizar(acerto, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, acerto, null, "Adicionou folga ao motorista.", unitOfWork);
                    return new JsonpResult(true, "Sucesso");
                }
                else
                    return new JsonpResult(false, "Favor verifique as datas informadas.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir a folga.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller InserirFolga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarChequeAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AdicionarChequeAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                int codigoAcerto = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Repositorio.Embarcador.Acerto.AcertoCheque repAcertoCheque = new Repositorio.Embarcador.Acerto.AcertoCheque(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Financeiro.Cheque repCheque = new Repositorio.Embarcador.Financeiro.Cheque(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigoAcerto);


                dynamic cheques = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Cheques"));
                foreach (var cheq in cheques)
                {
                    int codigoCheque = (int)cheq.Codigo;

                    if (!acerto.Cheques.Any(o => o.Cheque.Codigo == codigoCheque))
                    {
                        Dominio.Entidades.Embarcador.Acerto.AcertoCheque cheque = new Dominio.Entidades.Embarcador.Acerto.AcertoCheque();
                        cheque.AcertoViagem = acerto;
                        cheque.Cheque = repCheque.BuscarPorCodigo(codigoCheque);
                        repAcertoCheque.Inserir(cheque);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acerto, null, "Adicionou cheque ao acerto.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o cheque.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AdicionarChequeAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> RemoverBonificacaoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverBonificacaoMotorista " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteAdicionarBonificacao)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para remover bonificação ao motorista.");

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao bonificacao = repAcertoBonificacao.BuscarPorCodigo(codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, bonificacao.AcertoViagem, null, "Removeu a bonificação " + bonificacao.Descricao + " do acerto.", unitOfWork);
                repAcertoBonificacao.Deletar(bonificacao, Auditado);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar a bonificação.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverBonificacaoMotorista " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> RemoverDescontoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverDescontoMotorista " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteAdicionarDesconto)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para remover desconto ao motorista.");

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoDesconto desconto = repAcertoDesconto.BuscarPorCodigo(codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, desconto.AcertoViagem, null, "Removeu o desconto " + desconto.Descricao + " do acerto.", unitOfWork);
                repAcertoDesconto.Deletar(desconto);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o desconto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverDescontoMotorista " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> RemoverDevolucaoMoedaEstrangeira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverDevolucaoMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira repAcertoDevolucaoMoedaEstrangeira = new Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira devolucao = repAcertoDevolucaoMoedaEstrangeira.BuscarPorCodigo(codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, devolucao.AcertoViagem, null, "Removeu a devolução do motorista " + devolucao.Descricao + " do acerto.", unitOfWork);
                repAcertoDevolucaoMoedaEstrangeira.Deletar(devolucao);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar a devolução.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverDevolucaoMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> RemoverVariacaoCambial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller RemoverVariacaoCambial " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoVariacaoCambial repAcertoVariacaoCambial = new Repositorio.Embarcador.Acerto.AcertoVariacaoCambial(unitOfWork);

                Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial variacaoCambial = repAcertoVariacaoCambial.BuscarPorCodigo(codigo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, variacaoCambial.AcertoViagem, null, "Removeu a variação " + variacaoCambial.Descricao + " do acerto.", unitOfWork);
                repAcertoVariacaoCambial.Deletar(variacaoCambial);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar a variação.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller RemoverVariacaoCambial " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DadosLimpoFechamentoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                var dynRetorno = new
                {
                    ValorTotalAlimentacaoRepassado = "",
                    ValorAlimentacaoRepassado = "",
                    ValorAlimentacaoComprovado = "",
                    ValorAlimentacaoSaldo = "",
                    ValorTotalAdiantamentoRepassado = "",
                    ValorAdiantamentoRepassado = "",
                    ValorAdiantamentoComprovado = "",
                    ValorAdiantamentoSaldo = "",
                    CodigoAcerto = 0,
                    Motorista = "",
                    CPFMotorista = "",
                    Periodo = "",
                    NumeroViagens = "",
                    NumeroViagensCompartilhada = "",
                    ValorViagensCompartilhada = "",
                    PlacasVeiculos = "",
                    PlacasReboques = "",
                    DiasViagem = "",
                    DespesaCombustivel = "",
                    DespesaArla = "",
                    PedagioPago = "",
                    DespesaMotorista = "",
                    TotalDespesa = "",
                    ReceitaFrete = "",
                    PedagioRecebido = "",
                    OutrosRecebimentos = "",
                    BonificacaoCliente = "",
                    Ocorrencias = "",
                    TotalReceita = "",
                    TotalSaldo = "",
                    FaturamentoLiquido = "",
                    FaturamentoBruto = "",
                    TotalImposto = "",
                    ComissaoMotorista = "",
                    ValorBonificacao = "",
                    MotivoBonificacao = "",
                    ValorTotalBonificacao = "",
                    ValorTotalDesconto = "",
                    PercentualComissao = "",

                    PercentualPremioComissao = "",
                    PremioComissaoMotorista = "",

                    AdiantamentoXDespesas = "",
                    TotalPagarMotorista = "",
                    ValorLiquidoMes = "",

                    AbastecimentoMotorista = "",
                    PedagioMotorista = "",
                    OutraDespesaMotorista = "",
                    AdiantamentoMotorista = "",
                    RetornoAdiantamento = "",
                    DiariaMotorista = "",
                    TotalDespesaMotorista = "",
                    TotalReceitaMotorista = "",
                    SaldoMotorista = "",
                    BonificacoesMotorista = "",
                    DescontosMotorista = "",
                    DevolucoesMotorista = "",
                    VariacaoCambial = "",
                    VariacaoCambialReceita = "",
                    ComissaoReceitaMotorista = "",
                    SaldoFichaMotorista = "",
                    SegmentoVeiculo = "",
                    Cheque = "",

                    SaldoAtualMotorista = "",

                    SaldoAtualAlimentacaoMotorista = "",
                    SaldoAtualOutrasDepesasMotorista = "",
                    SaldoPrevistoAlimentacaoMotorista = "",
                    SaldoPrevistoOutrasDepesasMotorista = "",

                    PrevisaoDiarias = "",
                    ListaCheque = "",
                    ListaFolga = "",

                    FormaRecebimentoMotoristaAcerto = "",
                    DataVencimentoMotoristaAcerto = "",
                    ObservacaoMotoristaAcerto = "",
                    TipoMovimentoMotoristaAcerto = "",
                    ObservacaoAcertoMotorista = "",
                    Titulo = "",
                    Banco = ""
                };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados do fechamento do acerto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller DadosFechamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DadosFechamentoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller DadosFechamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigo);

                List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> receita = servAcertoViagem.RetornaObjetoReceitaViagem(codigo, unitOfWork, ConfiguracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem, ConfiguracaoEmbarcador.AcertoDeViagemImpressaoDetalhada, ConfiguracaoEmbarcador.GerarTituloFolhaPagamento, ConfiguracaoEmbarcador.GerarReciboAcertoViagemDetalhado, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, (configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false));

                decimal valorAlimentacaoRepassado = 0, valorTotalAlimentacaoRepassado = 0, valorAlimentacaoComprovado = 0, valorAlimentacaoSaldo = 0,
                    valorTotalAdiantamentoRepassado = 0, valorAdiantamentoRepassado = 0, valorAdiantamentoComprovado = 0, valorAdiantamentoSaldo = 0;

                decimal previsaoDiarias = 0;

                if (!ConfiguracaoEmbarcador.DesabilitarSaldoViagemAcerto)
                {
                    valorAlimentacaoComprovado = acerto.ValorAlimentacaoComprovado;
                    valorAdiantamentoComprovado = acerto.ValorAdiantamentoComprovado;
                    if (acerto.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento)
                    {
                        valorTotalAlimentacaoRepassado = repAcertoAdiantamento.BuscarValorTotalPorAcerto(acerto.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria);
                        valorAlimentacaoRepassado = repAcertoAdiantamento.BuscarValorPorAcerto(acerto.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria);

                        if (ConfiguracaoEmbarcador.SomarSaldoAtualMotoristaNoAcerto)
                        {
                            valorTotalAlimentacaoRepassado += acerto.Motorista.SaldoDiaria;
                            valorAlimentacaoRepassado += acerto.Motorista.SaldoDiaria;
                        }

                        if (ConfiguracaoEmbarcador.AcertoDeViagemComDiaria && receita != null && receita.Count > 0)
                            valorAlimentacaoComprovado = receita[0].OutraDespesaMotorista + receita[0].DiariaMotorista;

                        valorAlimentacaoSaldo = (valorTotalAlimentacaoRepassado - valorAlimentacaoComprovado);

                        valorTotalAdiantamentoRepassado = repAcertoAdiantamento.BuscarValorTotalPorAcerto(acerto.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento);
                        valorAdiantamentoRepassado = repAcertoAdiantamento.BuscarValorPorAcerto(acerto.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento);

                        if (ConfiguracaoEmbarcador.SomarSaldoAtualMotoristaNoAcerto)
                        {
                            valorTotalAdiantamentoRepassado += acerto.Motorista.SaldoAdiantamento;
                            valorAdiantamentoRepassado += acerto.Motorista.SaldoAdiantamento;
                        }

                        valorAdiantamentoSaldo = (valorTotalAdiantamentoRepassado - valorAdiantamentoComprovado);
                    }
                    else
                    {
                        valorTotalAlimentacaoRepassado = acerto.ValorTotalAlimentacaoRepassado;
                        valorAlimentacaoRepassado = acerto.ValorAlimentacaoRepassado;
                        valorAlimentacaoSaldo = acerto.ValorAlimentacaoSaldo;

                        valorTotalAdiantamentoRepassado = acerto.ValorTotalAdiantamentoRepassado;
                        valorAdiantamentoRepassado = acerto.ValorAdiantamentoRepassado;
                        valorAdiantamentoSaldo = acerto.ValorAdiantamentoSaldo;
                    }
                }

                decimal saldoAtualAlimentacaoMotorista = 0;
                decimal saldoAtualOutrasDepesasMotorista = 0;
                decimal saldoPrevistoAlimentacaoMotorista = 0;
                decimal saldoPrevistoOutrasDepesasMotorista = 0;

                previsaoDiarias = receita[0].DiariaMotorista;
                if (previsaoDiarias <= 0)
                    previsaoDiarias = servAcertoViagem.RetornarPreviaValorDiaria(acerto, unitOfWork);
                decimal saldoAtualMotorista = acerto.Motorista.SaldoDiaria;//+ acerto.Motorista.SaldoAdiantamento;
                if (ConfiguracaoEmbarcador.SomarSaldoAtualMotoristaNoAcerto || ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem || ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem)
                    saldoAtualMotorista += acerto.Motorista.SaldoAdiantamento;

                saldoAtualAlimentacaoMotorista = acerto.Motorista.SaldoDiaria;
                saldoAtualOutrasDepesasMotorista = acerto.Motorista.SaldoAdiantamento;

                saldoPrevistoAlimentacaoMotorista = acerto.Motorista.SaldoDiaria + valorAlimentacaoSaldo;
                saldoPrevistoOutrasDepesasMotorista = acerto.Motorista.SaldoAdiantamento + valorAdiantamentoSaldo;

                if (ConfiguracaoEmbarcador.ExibirSaldoPrevistoAcertoViagem && (acerto.SaldoAtualAlimentacaoMotorista > 0 || acerto.SaldoAtualOutrasDepesasMotorista > 0 || acerto.Situacao == SituacaoAcertoViagem.Fechado))
                {
                    saldoAtualMotorista = acerto.SaldoAtualAlimentacaoMotorista + acerto.SaldoAtualOutrasDepesasMotorista;
                    saldoPrevistoAlimentacaoMotorista = acerto.SaldoAtualAlimentacaoMotorista + valorAlimentacaoSaldo;
                    saldoPrevistoOutrasDepesasMotorista = acerto.SaldoAtualOutrasDepesasMotorista + valorAdiantamentoSaldo;
                    saldoAtualAlimentacaoMotorista = acerto.SaldoAtualAlimentacaoMotorista;
                    saldoAtualOutrasDepesasMotorista = acerto.SaldoAtualOutrasDepesasMotorista;
                }
                if (configuraoAcertoViagem.HabilitarControlarOutrasDespesas)
                {
                    if (acerto.OutrasDespesas != null)
                    {
                        valorAlimentacaoComprovado = acerto.OutrasDespesas
                            .Where(d => d.TipoDespesa != null && d.TipoDespesa.TipoDeDespesa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa.Alimentacao)
                            .Sum(d => d.Valor);
                        valorAlimentacaoSaldo = (valorTotalAlimentacaoRepassado - valorAlimentacaoComprovado);
                    }
                    else
                    {
                        valorAlimentacaoComprovado = 0;
                        valorAlimentacaoSaldo = (valorTotalAlimentacaoRepassado - valorAlimentacaoComprovado);
                    }
                    if (acerto.OutrasDespesas != null)
                    {
                        valorAdiantamentoComprovado = acerto.OutrasDespesas
                            .Where(d => d.TipoDespesa == null
                                     || d.TipoDespesa.TipoDeDespesa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa.Geral)
                            .Sum(d => d.Valor);
                        valorAdiantamentoSaldo = (valorTotalAdiantamentoRepassado - valorAdiantamentoComprovado);
                    }
                    else
                    {
                        valorAdiantamentoComprovado = 0;
                    }
                }               
                List<Folga> folgas = new List<Folga>();
                folgas.Add(new Folga()
                {
                    Codigo = acerto.Codigo,
                    Descricao = "Inicio " + (acerto.DataInicioFolga.HasValue ? acerto.DataInicioFolga.Value.ToString("dd/MM/yyyy") : "") + " até " + (acerto.DataFinalFolga.HasValue ? acerto.DataFinalFolga.Value.ToString("dd/MM/yyyy") : "") + " total de " + acerto.QuantidadeDiasFolga.ToString("n0") + " dia(s).",
                    Dias = acerto.QuantidadeDiasFolga
                });

                var dynRetorno = new
                {
                    ValorTotalAlimentacaoRepassado = valorTotalAlimentacaoRepassado.ToString("n2"),
                    ValorAlimentacaoRepassado = valorAlimentacaoRepassado.ToString("n2"),
                    ValorAlimentacaoComprovado = valorAlimentacaoComprovado.ToString("n2"),
                    ValorAlimentacaoSaldo = valorAlimentacaoSaldo.ToString("n2"),
                    ValorTotalAdiantamentoRepassado = valorTotalAdiantamentoRepassado.ToString("n2"),
                    ValorAdiantamentoRepassado = valorAdiantamentoRepassado.ToString("n2"),
                    ValorAdiantamentoComprovado = valorAdiantamentoComprovado.ToString("n2"),
                    ValorAdiantamentoSaldo = valorAdiantamentoSaldo.ToString("n2"),
                    CodigoAcerto = receita[0].CodigoAcerto,
                    Motorista = receita[0].Motorista,
                    CPFMotorista = receita[0].CPFMotorista,
                    Periodo = receita[0].Periodo,
                    NumeroViagens = receita[0].NumeroViagens,
                    NumeroViagensCompartilhada = receita[0].NumeroViagensCompartilhada,
                    ValorViagensCompartilhada = receita[0].ValorViagensCompartilhada.ToString("n2"),
                    PlacasVeiculos = receita[0].PlacasVeiculos,
                    PlacasReboques = receita[0].PlacasReboques,
                    DiasViagem = receita[0].DiasViagem,
                    DespesaCombustivel = receita[0].DespesaCombustivel.ToString("n2"),
                    DespesaArla = receita[0].DespesaArla.ToString("n2"),
                    PedagioPago = receita[0].PedagioPago.ToString("n2"),
                    DespesaMotorista = receita[0].DespesaMotorista.ToString("n2"),
                    TotalDespesa = receita[0].TotalDespesa.ToString("n2"),
                    ReceitaFrete = receita[0].ReceitaFrete.ToString("n2"),
                    PedagioRecebido = receita[0].PedagioRecebido.ToString("n2"),
                    OutrosRecebimentos = receita[0].OutrosRecebimentos.ToString("n2"),
                    BonificacaoCliente = receita[0].BonificacaoCliente.ToString("n2"),
                    Ocorrencias = receita[0].Ocorrencias.ToString("n2"),
                    TotalReceita = receita[0].TotalReceita.ToString("n2"),
                    TotalSaldo = receita[0].TotalSaldo.ToString("n2"),
                    FaturamentoLiquido = receita[0].FaturamentoLiquido.ToString("n2"),
                    FaturamentoBruto = receita[0].FaturamentoBruto.ToString("n2"),
                    TotalImposto = receita[0].TotalImposto.ToString("n2"),
                    ComissaoMotorista = receita[0].ComissaoMotorista.ToString("n2"),
                    ValorBonificacao = receita[0].ValorBonificacao.ToString("n2"),
                    MotivoBonificacao = receita[0].MotivoBonificacao,
                    ValorTotalBonificacao = receita[0].ValorTotalBonificacao.ToString("n2"),
                    ValorTotalDesconto = receita[0].ValorTotalDesconto.ToString("n2"),
                    PercentualComissao = receita[0].PercentualComissao.ToString("n2"),

                    PercentualPremioComissao = receita[0].PercentualPremioComissao.ToString("n2"),
                    PremioComissaoMotorista = receita[0].PremioComissaoMotorista.ToString("n2"),

                    AdiantamentoXDespesas = receita[0].AdiantamentoXDespesas.ToString("n2"),
                    TotalPagarMotorista = receita[0].TotalPagarMotorista.ToString("n2"),
                    ValorLiquidoMes = receita[0].ValorLiquidoMes.ToString("n2"),

                    AbastecimentoMotorista = receita[0].AbastecimentoMotorista.ToString("n2"),
                    PedagioMotorista = receita[0].PedagioMotorista.ToString("n2"),
                    OutraDespesaMotorista = receita[0].OutraDespesaMotorista.ToString("n2"),
                    AdiantamentoMotorista = receita[0].AdiantamentoMotorista.ToString("n2"),
                    RetornoAdiantamento = receita[0].RetornoAdiantamento.ToString("n2"),
                    DiariaMotorista = receita[0].DiariaMotorista.ToString("n2"),
                    TotalDespesaMotorista = receita[0].TotalDespesaMotorista.ToString("n2"),
                    TotalReceitaMotorista = receita[0].TotalReceitaMotorista.ToString("n2"),
                    SaldoMotorista = receita[0].SaldoMotorista.ToString("n2"),
                    BonificacoesMotorista = receita[0].BonificacoesMotorista.ToString("n2"),
                    DescontosMotorista = receita[0].DescontosMotorista.ToString("n2"),
                    DevolucoesMotorista = receita[0].DevolucoesMotorista.ToString("n2"),
                    VariacaoCambial = receita[0].VariacaoCambial.ToString("n2"),
                    VariacaoCambialReceita = receita[0].VariacaoCambialReceita.ToString("n2"),
                    ComissaoReceitaMotorista = receita[0].ComissaoMotorista.ToString("n2"),
                    SaldoFichaMotorista = acerto.Motorista?.SaldoAdiantamento.ToString("n2") ?? "0,00",
                    SegmentoVeiculo = acerto.SegmentoVeiculo != null ? new { Codigo = acerto.SegmentoVeiculo.Codigo, Descricao = acerto.SegmentoVeiculo.Descricao } : null,
                    Cheque = acerto.Cheque != null ? new { Codigo = acerto.Cheque.Codigo, Descricao = acerto.Cheque.Descricao } : null,

                    SaldoAtualMotorista = saldoAtualMotorista.ToString("n2"),

                    SaldoAtualAlimentacaoMotorista = saldoAtualAlimentacaoMotorista.ToString("n2"),
                    SaldoAtualOutrasDepesasMotorista = saldoAtualOutrasDepesasMotorista.ToString("n2"),
                    SaldoPrevistoAlimentacaoMotorista = saldoPrevistoAlimentacaoMotorista.ToString("n2"),
                    SaldoPrevistoOutrasDepesasMotorista = saldoPrevistoOutrasDepesasMotorista.ToString("n2"),

                    PrevisaoDiarias = previsaoDiarias.ToString("n2"),
                    ListaCheque = acerto.Cheques != null && acerto.Cheques.Count() > 0 ? (from obj in acerto.Cheques
                                                                                          select new
                                                                                          {
                                                                                              Codigo = obj.Cheque.Codigo,
                                                                                              NumeroCheque = obj.Cheque.NumeroCheque,
                                                                                              Banco = obj.Cheque.Banco?.Descricao ?? "",
                                                                                              Valor = obj.Cheque.Valor.ToString("n2")
                                                                                          }).ToList() : null,
                    ListaFolga = acerto.QuantidadeDiasFolga > 0 ? (from obj in folgas
                                                                   select new
                                                                   {
                                                                       Codigo = obj.Codigo,
                                                                       Descricao = obj.Descricao,
                                                                       Dias = obj.Dias
                                                                   }).ToList() : null,

                    acerto.FormaRecebimentoMotoristaAcerto,
                    DataVencimentoMotoristaAcerto = acerto.DataVencimentoMotoristaAcerto.HasValue ? acerto.DataVencimentoMotoristaAcerto.Value.ToString("dd/MM/yyyy") : "",
                    acerto.ObservacaoMotoristaAcerto,
                    TipoMovimentoMotoristaAcerto = acerto.TipoMovimentoMotoristaAcerto != null ? new { Codigo = acerto.TipoMovimentoMotoristaAcerto.Codigo, Descricao = acerto.TipoMovimentoMotoristaAcerto.Descricao } : null,
                    acerto.ObservacaoAcertoMotorista,
                    Titulo = acerto.Titulo != null ? new { Codigo = acerto.Titulo.Codigo, Descricao = acerto.Titulo.Descricao } : null,
                    Banco = acerto.Banco != null ? new { Codigo = acerto.Banco.Codigo, Descricao = acerto.Banco.Descricao } : null,
                    Assinado = !string.IsNullOrEmpty(acerto.GuidAssinatura)
                };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados do fechamento do acerto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller DadosFechamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        [AllowAuthenticate]
        public async Task<IActionResult> ImprimirRecibo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigo);

                // Valida
                if (acerto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] pdf = ReportRequest.WithType(ReportType.ImpressaoRecibo)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoAcerto", acerto.Codigo)
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .AddExtraData("CodigoUsuario", Usuario.Codigo)
                    .CallReport()
                    .GetContentFile();

                // Retorna o arquivo
                return Arquivo(pdf, "application/pdf", "Recibo Acerto de Viagem " + codigo.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarVeiculoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AtualizarVeiculoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                unitOfWork.Start();

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (!SalvarVeiculosFechamento(codigo, unitOfWork))
                    return new JsonpResult(false, "Por favor verifique o veículo selecionado.");

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Veículo salvo com sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao inserir novo veículo.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AtualizarVeiculoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarInfracaoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AtualizarInfracaoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                unitOfWork.Start();

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (!SalvarInfracaoFechamento(codigo, unitOfWork))
                    return new JsonpResult(false, "Por favor verifique a infração selecionada.");

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Infrações atualizadas");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a infração.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AtualizarInfracaoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AssinarInfracaoAcerto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AssinarInfracaoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagemInfracao repAcertoViagemInfracao = new Repositorio.Embarcador.Acerto.AcertoViagemInfracao(unitOfWork);
                Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);

                unitOfWork.Start();

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoInfracao"), out int codigoInfracao);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao infracaoAcerto = repAcertoViagemInfracao.BuscarPorCodigo(codigoInfracao);
                if (infracaoAcerto != null)
                {
                    infracaoAcerto.InfracaoAssinada = true;
                    repAcertoViagemInfracao.Atualizar(infracaoAcerto);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, infracaoAcerto.Infracao, null, "Informou que a multa foi assinada pelo acerto de número " + infracaoAcerto.AcertoViagem.Numero.ToString(), unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, infracaoAcerto.AcertoViagem, null, "Informou que a multa de número " + infracaoAcerto.Infracao.Numero.ToString() + " foi assinada pelo acerto de número " + infracaoAcerto.AcertoViagem.Numero.ToString(), unitOfWork);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Infração assinada com sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao assinar a infração.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AssinarInfracaoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DadosSaldoMoedaEstrangeira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller DadosSaldoMoedaEstrangeira " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoConversaoMoeda repAcertoConversaoMoeda = new Repositorio.Embarcador.Acerto.AcertoConversaoMoeda(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira repDevolucao = new Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedaDolar = new MoedaCotacaoBancoCentral[]
                   {
                        MoedaCotacaoBancoCentral.DolarCompra,
                        MoedaCotacaoBancoCentral.DolarVenda
                   };

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedaPesoArgentino = new MoedaCotacaoBancoCentral[]
                   {
                        MoedaCotacaoBancoCentral.PesoArgentino
                   };

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedaPesoUruguaio = new MoedaCotacaoBancoCentral[]
                  {
                        MoedaCotacaoBancoCentral.PesoUruguaio
                  };

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedaPesoChileno = new MoedaCotacaoBancoCentral[]
                  {
                        MoedaCotacaoBancoCentral.PesoChileno
                  };

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedaGuarani = new MoedaCotacaoBancoCentral[]
                 {
                        MoedaCotacaoBancoCentral.Guarani
                 };
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedaNovoSol = new MoedaCotacaoBancoCentral[]
                 {
                        MoedaCotacaoBancoCentral.NovoSolPeruano
                 };
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedaReais = new MoedaCotacaoBancoCentral[]
                 {
                        MoedaCotacaoBancoCentral.Real
                 };

                decimal abastecimentoMotoristaDolar = repAcertoAbastecimento.BuscarValorMoedaestrangeira(codigo, moedaDolar);
                decimal pedagioMotoristaDolar = repAcertoPedagio.BuscarValorMoedaestrangeira(codigo, moedaDolar);
                decimal outraDespesaMotoristaDolar = repAcertoOutraDespesa.BuscarValorMoedaestrangeira(codigo, moedaDolar);
                decimal adiantamentoMotoristaDolar = repAcertoAdiantamento.BuscarValorMoedaestrangeira(codigo, moedaDolar);
                decimal bonificacoesMotoristaDolar = repAcertoBonificacao.BuscarValorMoedaestrangeira(codigo, moedaDolar);
                decimal devolucoesDolar = repDevolucao.BuscarValorMoedaestrangeira(codigo, moedaDolar);
                decimal descontosMotoristaDolar = repAcertoDesconto.BuscarValorMoedaestrangeira(codigo, moedaDolar);
                descontosMotoristaDolar -= repAcertoConversaoMoeda.BuscarValorMoedaEstrangeiraOrigem(codigo, moedaDolar);
                decimal recebidoConversaoDolar = repAcertoConversaoMoeda.BuscarValorMoedaestrangeira(codigo, moedaDolar);
                decimal abastecimentoMotoristaPesoArgentino = repAcertoAbastecimento.BuscarValorMoedaestrangeira(codigo, moedaPesoArgentino);
                decimal pedagioMotoristaPesoArgentino = repAcertoPedagio.BuscarValorMoedaestrangeira(codigo, moedaPesoArgentino);
                decimal outraDespesaMotoristaPesoArgentino = repAcertoOutraDespesa.BuscarValorMoedaestrangeira(codigo, moedaPesoArgentino);
                decimal adiantamentoMotoristaPesoArgentino = repAcertoAdiantamento.BuscarValorMoedaestrangeira(codigo, moedaPesoArgentino);
                decimal bonificacoesMotoristaPesoArgentino = repAcertoBonificacao.BuscarValorMoedaestrangeira(codigo, moedaPesoArgentino);
                decimal devolucoesPesoArgentino = repDevolucao.BuscarValorMoedaestrangeira(codigo, moedaPesoArgentino);
                decimal descontosMotoristaPesoArgentino = repAcertoDesconto.BuscarValorMoedaestrangeira(codigo, moedaPesoArgentino);
                descontosMotoristaPesoArgentino -= repAcertoConversaoMoeda.BuscarValorMoedaEstrangeiraOrigem(codigo, moedaPesoArgentino);
                decimal recebidoConversaoPesoArgentino = repAcertoConversaoMoeda.BuscarValorMoedaestrangeira(codigo, moedaPesoArgentino);
                decimal abastecimentoMotoristaPesoUruguaio = repAcertoAbastecimento.BuscarValorMoedaestrangeira(codigo, moedaPesoUruguaio);
                decimal pedagioMotoristaPesoUruguaio = repAcertoPedagio.BuscarValorMoedaestrangeira(codigo, moedaPesoUruguaio);
                decimal outraDespesaMotoristaPesoUruguaio = repAcertoOutraDespesa.BuscarValorMoedaestrangeira(codigo, moedaPesoUruguaio);
                decimal adiantamentoMotoristaPesoUruguaio = repAcertoAdiantamento.BuscarValorMoedaestrangeira(codigo, moedaPesoUruguaio);
                decimal bonificacoesMotoristaPesoUruguaio = repAcertoBonificacao.BuscarValorMoedaestrangeira(codigo, moedaPesoUruguaio);
                decimal devolucoesPesoUruguaio = repDevolucao.BuscarValorMoedaestrangeira(codigo, moedaPesoUruguaio);
                decimal descontosMotoristaPesoUruguaio = repAcertoDesconto.BuscarValorMoedaestrangeira(codigo, moedaPesoUruguaio);
                descontosMotoristaPesoUruguaio -= repAcertoConversaoMoeda.BuscarValorMoedaEstrangeiraOrigem(codigo, moedaPesoUruguaio);
                decimal recebidoConversaoPesoUruguaio = repAcertoConversaoMoeda.BuscarValorMoedaestrangeira(codigo, moedaPesoUruguaio);
                decimal abastecimentoMotoristaPesoChileno = repAcertoAbastecimento.BuscarValorMoedaestrangeira(codigo, moedaPesoChileno);
                decimal pedagioMotoristaPesoChileno = repAcertoPedagio.BuscarValorMoedaestrangeira(codigo, moedaPesoChileno);
                decimal outraDespesaMotoristaPesoChileno = repAcertoOutraDespesa.BuscarValorMoedaestrangeira(codigo, moedaPesoChileno);
                decimal adiantamentoMotoristaPesoChileno = repAcertoAdiantamento.BuscarValorMoedaestrangeira(codigo, moedaPesoChileno);
                decimal bonificacoesMotoristaPesoChileno = repAcertoBonificacao.BuscarValorMoedaestrangeira(codigo, moedaPesoChileno);
                decimal devolucoesPesoChileno = repDevolucao.BuscarValorMoedaestrangeira(codigo, moedaPesoChileno);
                decimal descontosMotoristaPesoChileno = repAcertoDesconto.BuscarValorMoedaestrangeira(codigo, moedaPesoChileno);
                descontosMotoristaPesoChileno -= repAcertoConversaoMoeda.BuscarValorMoedaEstrangeiraOrigem(codigo, moedaPesoChileno);
                decimal recebidoConversaoPesoChileno = repAcertoConversaoMoeda.BuscarValorMoedaestrangeira(codigo, moedaPesoChileno);
                decimal abastecimentoMotoristaGuarani = repAcertoAbastecimento.BuscarValorMoedaestrangeira(codigo, moedaGuarani);
                decimal pedagioMotoristaGuarani = repAcertoPedagio.BuscarValorMoedaestrangeira(codigo, moedaGuarani);
                decimal outraDespesaMotoristaGuarani = repAcertoOutraDespesa.BuscarValorMoedaestrangeira(codigo, moedaGuarani);
                decimal adiantamentoMotoristaGuarani = repAcertoAdiantamento.BuscarValorMoedaestrangeira(codigo, moedaGuarani);
                decimal bonificacoesMotoristaGuarani = repAcertoBonificacao.BuscarValorMoedaestrangeira(codigo, moedaGuarani);
                decimal devolucoesGuarani = repDevolucao.BuscarValorMoedaestrangeira(codigo, moedaGuarani);
                decimal descontosMotoristaGuarani = repAcertoDesconto.BuscarValorMoedaestrangeira(codigo, moedaGuarani);
                descontosMotoristaGuarani -= repAcertoConversaoMoeda.BuscarValorMoedaEstrangeiraOrigem(codigo, moedaGuarani);
                decimal recebidoConversaoGuarani = repAcertoConversaoMoeda.BuscarValorMoedaestrangeira(codigo, moedaGuarani);
                decimal abastecimentoMotoristaNovoSol = repAcertoAbastecimento.BuscarValorMoedaestrangeira(codigo, moedaNovoSol);
                decimal pedagioMotoristaNovoSol = repAcertoPedagio.BuscarValorMoedaestrangeira(codigo, moedaNovoSol);
                decimal outraDespesaMotoristaNovoSol = repAcertoOutraDespesa.BuscarValorMoedaestrangeira(codigo, moedaNovoSol);
                decimal adiantamentoMotoristaNovoSol = repAcertoAdiantamento.BuscarValorMoedaestrangeira(codigo, moedaNovoSol);
                decimal bonificacoesMotoristaNovoSol = repAcertoBonificacao.BuscarValorMoedaestrangeira(codigo, moedaNovoSol);
                decimal devolucoesNovoSol = repDevolucao.BuscarValorMoedaestrangeira(codigo, moedaNovoSol);
                decimal descontosMotoristaNovoSol = repAcertoDesconto.BuscarValorMoedaestrangeira(codigo, moedaNovoSol);
                descontosMotoristaNovoSol -= repAcertoConversaoMoeda.BuscarValorMoedaEstrangeiraOrigem(codigo, moedaNovoSol);
                decimal recebidoConversaoNovoSol = repAcertoConversaoMoeda.BuscarValorMoedaestrangeira(codigo, moedaNovoSol);
                decimal abastecimentoMotoristaReais = repAcertoAbastecimento.BuscarValorMoedaestrangeira(codigo, moedaReais);
                decimal pedagioMotoristaReais = repAcertoPedagio.BuscarValorMoedaestrangeira(codigo, moedaReais);
                decimal outraDespesaMotoristaReais = repAcertoOutraDespesa.BuscarValorMoedaestrangeira(codigo, moedaReais);
                decimal adiantamentoMotoristaReais = repAcertoAdiantamento.BuscarValorMoedaestrangeira(codigo, moedaReais);
                decimal bonificacoesMotoristaReais = repAcertoBonificacao.BuscarValorMoedaestrangeira(codigo, moedaReais);
                decimal devolucoesReais = repDevolucao.BuscarValorMoedaestrangeira(codigo, moedaReais);
                decimal descontosMotoristaReais = repAcertoDesconto.BuscarValorMoedaestrangeira(codigo, moedaReais);
                descontosMotoristaReais -= repAcertoConversaoMoeda.BuscarValorMoedaEstrangeiraOrigem(codigo, moedaReais);
                decimal recebidoConversaoReais = repAcertoConversaoMoeda.BuscarValorMoedaestrangeira(codigo, moedaReais);

                var dynRetorno = new
                {
                    AbastecimentoMotoristaDolar = abastecimentoMotoristaDolar.ToString("n2"),
                    PedagioMotoristaDolar = pedagioMotoristaDolar.ToString("n2"),
                    OutraDespesaMotoristaDolar = outraDespesaMotoristaDolar.ToString("n2"),
                    AdiantamentoMotoristaDolar = adiantamentoMotoristaDolar.ToString("n2"),
                    DiariaMotoristaDolar = "0,00",
                    TotalDespesaMotoristaDolar = (abastecimentoMotoristaDolar + pedagioMotoristaDolar + outraDespesaMotoristaDolar + bonificacoesMotoristaDolar + devolucoesDolar).ToString("n2"),
                    TotalReceitaMotoristaDolar = (adiantamentoMotoristaDolar + descontosMotoristaDolar + recebidoConversaoDolar).ToString("n2"),
                    SaldoMotoristaDolar = ((adiantamentoMotoristaDolar + descontosMotoristaDolar + recebidoConversaoDolar) - (abastecimentoMotoristaDolar + pedagioMotoristaDolar + outraDespesaMotoristaDolar + bonificacoesMotoristaDolar + devolucoesDolar)).ToString("n2"),
                    BonificacoesMotoristaDolar = bonificacoesMotoristaDolar.ToString("n2"),
                    DevolucoesDolar = devolucoesDolar.ToString("n2"),
                    DescontosMotoristaDolar = descontosMotoristaDolar.ToString("n2"),
                    RecebidoConversaoDolar = recebidoConversaoDolar.ToString("n2"),

                    AbastecimentoMotoristaPesoArgentino = abastecimentoMotoristaPesoArgentino.ToString("n2"),
                    PedagioMotoristaPesoArgentino = pedagioMotoristaPesoArgentino.ToString("n2"),
                    OutraDespesaMotoristaPesoArgentino = outraDespesaMotoristaPesoArgentino.ToString("n2"),
                    AdiantamentoMotoristaPesoArgentino = adiantamentoMotoristaPesoArgentino.ToString("n2"),
                    DiariaMotoristaPesoArgentino = "0,00",
                    TotalDespesaMotoristaPesoArgentino = (abastecimentoMotoristaPesoArgentino + pedagioMotoristaPesoArgentino + outraDespesaMotoristaPesoArgentino + bonificacoesMotoristaPesoArgentino + devolucoesPesoArgentino).ToString("n2"),
                    TotalReceitaMotoristaPesoArgentino = (adiantamentoMotoristaPesoArgentino + descontosMotoristaPesoArgentino + recebidoConversaoPesoArgentino).ToString("n2"),
                    SaldoMotoristaPesoArgentino = ((adiantamentoMotoristaPesoArgentino + descontosMotoristaPesoArgentino + recebidoConversaoPesoArgentino) - (abastecimentoMotoristaPesoArgentino + pedagioMotoristaPesoArgentino + outraDespesaMotoristaPesoArgentino + bonificacoesMotoristaPesoArgentino + devolucoesPesoArgentino)).ToString("n2"),
                    BonificacoesMotoristaPesoArgentino = bonificacoesMotoristaPesoArgentino.ToString("n2"),
                    DevolucoesPesoArgentino = devolucoesPesoArgentino.ToString("n2"),
                    DescontosMotoristaPesoArgentino = descontosMotoristaPesoArgentino.ToString("n2"),
                    RecebidoConversaoPesoArgentino = recebidoConversaoPesoArgentino.ToString("n2"),

                    AbastecimentoMotoristaPesoUruguaio = abastecimentoMotoristaPesoUruguaio.ToString("n2"),
                    PedagioMotoristaPesoUruguaio = pedagioMotoristaPesoUruguaio.ToString("n2"),
                    OutraDespesaMotoristaPesoUruguaio = outraDespesaMotoristaPesoUruguaio.ToString("n2"),
                    AdiantamentoMotoristaPesoUruguaio = adiantamentoMotoristaPesoUruguaio.ToString("n2"),
                    DiariaMotoristaPesoUruguaio = "0,00",
                    TotalDespesaMotoristaPesoUruguaio = (abastecimentoMotoristaPesoUruguaio + pedagioMotoristaPesoUruguaio + outraDespesaMotoristaPesoUruguaio + bonificacoesMotoristaPesoUruguaio + devolucoesPesoUruguaio).ToString("n2"),
                    TotalReceitaMotoristaPesoUruguaio = (adiantamentoMotoristaPesoUruguaio + descontosMotoristaPesoUruguaio + recebidoConversaoPesoUruguaio).ToString("n2"),
                    SaldoMotoristaPesoUruguaio = ((adiantamentoMotoristaPesoUruguaio + descontosMotoristaPesoUruguaio + recebidoConversaoPesoUruguaio) - (abastecimentoMotoristaPesoUruguaio + pedagioMotoristaPesoUruguaio + outraDespesaMotoristaPesoUruguaio + bonificacoesMotoristaPesoUruguaio + devolucoesPesoUruguaio)).ToString("n2"),
                    BonificacoesMotoristaPesoUruguaio = bonificacoesMotoristaPesoUruguaio.ToString("n2"),
                    DevolucoesPesoUruguaio = devolucoesPesoUruguaio.ToString("n2"),
                    DescontosMotoristaPesoUruguaio = descontosMotoristaPesoUruguaio.ToString("n2"),
                    RecebidoConversaoPesoUruguaio = recebidoConversaoPesoUruguaio.ToString("n2"),

                    AbastecimentoMotoristaPesoChileno = abastecimentoMotoristaPesoChileno.ToString("n2"),
                    PedagioMotoristaPesoChileno = pedagioMotoristaPesoChileno.ToString("n2"),
                    OutraDespesaMotoristaPesoChileno = outraDespesaMotoristaPesoChileno.ToString("n2"),
                    AdiantamentoMotoristaPesoChileno = adiantamentoMotoristaPesoChileno.ToString("n2"),
                    DiariaMotoristaPesoChileno = "0,00",
                    TotalDespesaMotoristaPesoChileno = (abastecimentoMotoristaPesoChileno + pedagioMotoristaPesoChileno + outraDespesaMotoristaPesoChileno + bonificacoesMotoristaPesoChileno + devolucoesPesoChileno).ToString("n2"),
                    TotalReceitaMotoristaPesoChileno = (adiantamentoMotoristaPesoChileno + descontosMotoristaPesoChileno + recebidoConversaoPesoChileno).ToString("n2"),
                    SaldoMotoristaPesoChileno = ((adiantamentoMotoristaPesoChileno + descontosMotoristaPesoChileno + recebidoConversaoPesoChileno) - (abastecimentoMotoristaPesoChileno + pedagioMotoristaPesoChileno + outraDespesaMotoristaPesoChileno + bonificacoesMotoristaPesoChileno + devolucoesPesoChileno)).ToString("n2"),
                    BonificacoesMotoristaPesoChileno = bonificacoesMotoristaPesoChileno.ToString("n2"),
                    DevolucoesPesoChileno = devolucoesPesoChileno.ToString("n2"),
                    DescontosMotoristaPesoChileno = descontosMotoristaPesoChileno.ToString("n2"),
                    RecebidoConversaoPesoChileno = recebidoConversaoPesoChileno.ToString("n2"),

                    AbastecimentoMotoristaGuarani = abastecimentoMotoristaGuarani.ToString("n2"),
                    PedagioMotoristaGuarani = pedagioMotoristaGuarani.ToString("n2"),
                    OutraDespesaMotoristaGuarani = outraDespesaMotoristaGuarani.ToString("n2"),
                    AdiantamentoMotoristaGuarani = adiantamentoMotoristaGuarani.ToString("n2"),
                    DiariaMotoristaGuarani = "0,00",
                    TotalDespesaMotoristaGuarani = (abastecimentoMotoristaGuarani + pedagioMotoristaGuarani + outraDespesaMotoristaGuarani + bonificacoesMotoristaGuarani + devolucoesGuarani).ToString("n2"),
                    TotalReceitaMotoristaGuarani = (adiantamentoMotoristaGuarani + descontosMotoristaGuarani + recebidoConversaoGuarani).ToString("n2"),
                    SaldoMotoristaGuarani = ((adiantamentoMotoristaGuarani + descontosMotoristaGuarani + recebidoConversaoGuarani) - (abastecimentoMotoristaGuarani + pedagioMotoristaGuarani + outraDespesaMotoristaGuarani + bonificacoesMotoristaGuarani + devolucoesGuarani)).ToString("n2"),
                    BonificacoesMotoristaGuarani = bonificacoesMotoristaGuarani.ToString("n2"),
                    DevolucoesGuarani = devolucoesGuarani.ToString("n2"),
                    DescontosMotoristaGuarani = descontosMotoristaGuarani.ToString("n2"),
                    RecebidoConversaoGuarani = recebidoConversaoGuarani.ToString("n2"),

                    AbastecimentoMotoristaNovoSol = abastecimentoMotoristaNovoSol.ToString("n2"),
                    PedagioMotoristaNovoSol = pedagioMotoristaNovoSol.ToString("n2"),
                    OutraDespesaMotoristaNovoSol = outraDespesaMotoristaNovoSol.ToString("n2"),
                    AdiantamentoMotoristaNovoSol = adiantamentoMotoristaNovoSol.ToString("n2"),
                    DiariaMotoristaNovoSol = "0,00",
                    TotalDespesaMotoristaNovoSol = (abastecimentoMotoristaNovoSol + pedagioMotoristaNovoSol + outraDespesaMotoristaNovoSol + bonificacoesMotoristaNovoSol + devolucoesNovoSol).ToString("n2"),
                    TotalReceitaMotoristaNovoSol = (adiantamentoMotoristaNovoSol + descontosMotoristaNovoSol + recebidoConversaoNovoSol).ToString("n2"),
                    SaldoMotoristaNovoSol = ((adiantamentoMotoristaNovoSol + descontosMotoristaNovoSol + recebidoConversaoNovoSol) - (abastecimentoMotoristaNovoSol + pedagioMotoristaNovoSol + outraDespesaMotoristaNovoSol + bonificacoesMotoristaNovoSol + devolucoesNovoSol)).ToString("n2"),
                    BonificacoesMotoristaNovoSol = bonificacoesMotoristaNovoSol.ToString("n2"),
                    DevolucoesNovoSol = devolucoesNovoSol.ToString("n2"),
                    DescontosMotoristaNovoSol = descontosMotoristaNovoSol.ToString("n2"),
                    RecebidoConversaoNovoSol = recebidoConversaoNovoSol.ToString("n2"),

                    AbastecimentoMotoristaReais = abastecimentoMotoristaReais.ToString("n2"),
                    PedagioMotoristaReais = pedagioMotoristaReais.ToString("n2"),
                    OutraDespesaMotoristaReais = outraDespesaMotoristaReais.ToString("n2"),
                    AdiantamentoMotoristaReais = adiantamentoMotoristaReais.ToString("n2"),
                    DiariaMotoristaReais = "0,00",
                    TotalDespesaMotoristaReais = (abastecimentoMotoristaReais + pedagioMotoristaReais + outraDespesaMotoristaReais + bonificacoesMotoristaReais + devolucoesReais).ToString("n2"),
                    TotalReceitaMotoristaReais = (adiantamentoMotoristaReais + descontosMotoristaReais + recebidoConversaoReais).ToString("n2"),
                    SaldoMotoristaReais = ((adiantamentoMotoristaReais + descontosMotoristaReais + recebidoConversaoReais) - (abastecimentoMotoristaReais + pedagioMotoristaReais + outraDespesaMotoristaReais + bonificacoesMotoristaReais + devolucoesReais)).ToString("n2"),
                    BonificacoesMotoristaReais = bonificacoesMotoristaReais.ToString("n2"),
                    DevolucoesReais = devolucoesReais.ToString("n2"),
                    DescontosMotoristaReais = descontosMotoristaReais.ToString("n2"),
                    RecebidoConversaoReais = recebidoConversaoReais.ToString("n2")
                };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados do fechamento do acerto.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller DadosFechamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImprimirReciboMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoVeiculoResultado repAcertoVeiculoResultado = new Repositorio.Embarcador.Acerto.AcertoVeiculoResultado(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigo);

                if (acerto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] pdf = ReportRequest.WithType(ReportType.ImpressaoReciboMotorista)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoAcerto", acerto.Codigo)
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .AddExtraData("CodigoUsuario", Usuario.Codigo)
                    .CallReport()
                    .GetContentFile();

                // Retorna o arquivo
                return Arquivo(pdf, "application/pdf", "Recibo Motorista Acerto de Viagem " + codigo.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CalcularVariacaoCambial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller CalcularVariacaoCambial " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                int codigoAcerto = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoConversaoMoeda repAcertoConversaoMoeda = new Repositorio.Embarcador.Acerto.AcertoConversaoMoeda(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira repDevolucao = new Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedaReais = new MoedaCotacaoBancoCentral[]
                 {
                        MoedaCotacaoBancoCentral.Real
                 };

                List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> receita = servAcertoViagem.RetornaObjetoReceitaViagem(codigoAcerto, unitOfWork, ConfiguracaoEmbarcador.NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem, ConfiguracaoEmbarcador.AcertoDeViagemImpressaoDetalhada, ConfiguracaoEmbarcador.GerarTituloFolhaPagamento, ConfiguracaoEmbarcador.GerarReciboAcertoViagemDetalhado, ConfiguracaoEmbarcador.VisualizarReciboPorMotoristaNoAcertoDeViagem, (configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false));

                decimal saldoMotorista = receita[0].SaldoMotorista;

                decimal abastecimentoMotoristaReais = repAcertoAbastecimento.BuscarValorMoedaestrangeira(codigoAcerto, moedaReais);
                decimal pedagioMotoristaReais = repAcertoPedagio.BuscarValorMoedaestrangeira(codigoAcerto, moedaReais);
                decimal outraDespesaMotoristaReais = repAcertoOutraDespesa.BuscarValorMoedaestrangeira(codigoAcerto, moedaReais);
                decimal adiantamentoMotoristaReais = repAcertoAdiantamento.BuscarValorMoedaestrangeira(codigoAcerto, moedaReais);
                decimal bonificacoesMotoristaReais = repAcertoBonificacao.BuscarValorMoedaestrangeira(codigoAcerto, moedaReais);
                decimal devolucoesReais = repDevolucao.BuscarValorMoedaestrangeira(codigoAcerto, moedaReais);
                decimal descontosMotoristaReais = repAcertoDesconto.BuscarValorMoedaestrangeira(codigoAcerto, moedaReais);
                descontosMotoristaReais -= repAcertoConversaoMoeda.BuscarValorMoedaEstrangeiraOrigem(codigoAcerto, moedaReais);
                decimal recebidoConversaoReais = repAcertoConversaoMoeda.BuscarValorMoedaestrangeira(codigoAcerto, moedaReais);
                decimal saldoMotoristaReais = ((adiantamentoMotoristaReais + descontosMotoristaReais + recebidoConversaoReais) - (abastecimentoMotoristaReais + pedagioMotoristaReais + outraDespesaMotoristaReais + bonificacoesMotoristaReais + devolucoesReais));

                unitOfWork.Start();

                acerto.VariacaoCambial = 0m;
                acerto.VariacaoCambialReceita = 0m;

                decimal diferenca = Math.Round(saldoMotorista, 2, MidpointRounding.ToEven) - Math.Round(saldoMotoristaReais, 2, MidpointRounding.ToEven);
                if (diferenca > 0)
                    acerto.VariacaoCambial = diferenca;
                else if (diferenca < 0)
                    acerto.VariacaoCambialReceita = diferenca * -1;

                unitOfWork.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, acerto, null, "Recalculou a variação cambial.", unitOfWork);

                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                if (unitOfWork.IsActiveTransaction())
                    unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar o cheque.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller CalcularVariacaoCambial " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool SalvarInfracaoFechamento(int codigoAcerto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (codigoAcerto == 0)
                return false;

            Servicos.Log.TratarErro(codigoAcerto.ToString() + " Inicio SalvarInfracaoFechamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoViagemInfracao repAcertoViagemInfracao = new Repositorio.Embarcador.Acerto.AcertoViagemInfracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeDeTrabalho);
            Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(unidadeDeTrabalho);

            List<int> listaInfracoesAcerto = repAcertoViagem.BuscarInfracoesDoAcerto(codigoAcerto);
            List<int> codigoInfracoes = new List<int>();

            dynamic listaInfracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaInfracoes"));

            foreach (var infracao in listaInfracoes)
            {
                codigoInfracoes.Add(int.Parse((string)infracao.Infracao.Codigo));

                if (!listaInfracoesAcerto.Contains(int.Parse((string)infracao.Infracao.Codigo)))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao infracaoAcerto = new Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao();
                    infracaoAcerto.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                    infracaoAcerto.Infracao = repInfracao.BuscarPorCodigo(int.Parse((string)infracao.Infracao.Codigo));
                    infracaoAcerto.InfracaoAssinada = false;

                    if (infracaoAcerto.Infracao != null)
                        repAcertoViagemInfracao.Inserir(infracaoAcerto);
                }
            }

            for (int i = 0; i < listaInfracoesAcerto.Count; i++)
            {
                if (!codigoInfracoes.Contains(listaInfracoesAcerto[i]))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao veiculoInfracao = repAcertoViagemInfracao.BuscarPorAcertoInfracao(codigoAcerto, listaInfracoesAcerto[i]);

                    repAcertoViagemInfracao.Deletar(veiculoInfracao);
                }
            }

            Servicos.Log.TratarErro(codigoAcerto.ToString() + " Fim SalvarVeiculosFechamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            return true;
        }

        private bool SalvarVeiculosFechamento(int codigoAcerto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (codigoAcerto == 0)
                return false;

            Servicos.Log.TratarErro(codigoAcerto.ToString() + " Inicio SalvarVeiculosFechamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento repAcertoVeiculoSegmento = new Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);

            List<int> listaAcertoVeiculo = repAcertoVeiculo.BuscarVeiculosDoAcerto(codigoAcerto);
            List<int> codigoVeiculos = new List<int>();

            dynamic listaVeiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaVeiculosFechamento"));

            foreach (var veiculo in listaVeiculos)
            {
                codigoVeiculos.Add(int.Parse((string)veiculo.Veiculo.Codigo));

                if (!listaAcertoVeiculo.Contains(int.Parse((string)veiculo.Veiculo.Codigo)))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo veiculoAcerto = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo();
                    veiculoAcerto.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                    veiculoAcerto.Veiculo = repVeiculo.BuscarPorCodigo(int.Parse((string)veiculo.Veiculo.Codigo));

                    repAcertoVeiculo.Inserir(veiculoAcerto);

                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento acertoVeiculoSegmento = repAcertoVeiculoSegmento.BuscarPorAcertoEVeiculo(codigoAcerto, veiculoAcerto.Veiculo.Codigo);
                    if (acertoVeiculoSegmento == null)
                    {
                        acertoVeiculoSegmento = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento();
                        acertoVeiculoSegmento.AcertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                        acertoVeiculoSegmento.Veiculo = repVeiculo.BuscarPorCodigo(int.Parse((string)veiculo.Veiculo.Codigo));
                        acertoVeiculoSegmento.ModeloVeicularCarga = null;
                        acertoVeiculoSegmento.GrupoPessoas = null;

                        repAcertoVeiculoSegmento.Inserir(acertoVeiculoSegmento);
                    }
                }
            }

            for (int i = 0; i < listaAcertoVeiculo.Count; i++)
            {
                if (!codigoVeiculos.Contains(listaAcertoVeiculo[i]))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo veiculoAcerto = repAcertoVeiculo.BuscarPorAcertoEVeiculo(codigoAcerto, listaAcertoVeiculo[i]);
                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento acertoVeiculoSegmento = repAcertoVeiculoSegmento.BuscarPorAcertoEVeiculo(codigoAcerto, listaAcertoVeiculo[i]);
                    if (acertoVeiculoSegmento != null)
                        repAcertoVeiculoSegmento.Deletar(acertoVeiculoSegmento);

                    List<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento> listasAcertoResumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculo(veiculoAcerto.AcertoViagem.Codigo, veiculoAcerto.Veiculo.Codigo);
                    foreach (var resumo in listasAcertoResumoAbastecimento)
                    {
                        repAcertoResumoAbastecimento.Deletar(resumo);
                    }
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> listaAcertoOutraDespesa = repAcertoOutraDespesa.BuscarPorCodigoAcertoVeiculo(veiculoAcerto.AcertoViagem.Codigo, veiculoAcerto.Veiculo.Codigo);
                    foreach (var despesa in listaAcertoOutraDespesa)
                    {
                        repAcertoOutraDespesa.Deletar(despesa);
                    }
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> abastecimenos = repAcertoAbastecimento.BuscarPorVeiculoCodigoAcerto(veiculoAcerto.AcertoViagem.Codigo, veiculoAcerto.Veiculo.Codigo);
                    foreach (var abastecimento in abastecimenos)
                    {
                        repAcertoAbastecimento.Deletar(abastecimento);
                    }
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> pedagios = repAcertoPedagio.BuscarPorCodigoAcertoVeiculo(veiculoAcerto.AcertoViagem.Codigo, veiculoAcerto.Veiculo.Codigo);
                    foreach (var pedagio in pedagios)
                    {
                        repAcertoPedagio.Deletar(pedagio);
                    }
                    repAcertoVeiculo.Deletar(veiculoAcerto);
                }
            }

            Servicos.Log.TratarErro(codigoAcerto.ToString() + " Fim SalvarVeiculosFechamento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            return true;
        }

        private void GerarMovimentacaoOperadorMotorista(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unitOfWork, bool reversao, decimal valorAdiancamento)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = configAcertoViagem.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(_conexao.StringConexao);
            string obsReversao = string.Empty;
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaDebito = null;
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaCredito = null;
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaMotorista = null;
            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = null;

            //SAIDA DA CONTA DO OPERADOR
            if (reversao)
            {
                obsReversao = "REVERSÃO ";
                planoContaCredito = acertoViagem.Motorista.PlanoAcertoViagem;
                planoContaDebito = acertoViagem.Operador.PlanoConta;
            }
            else
            {
                planoContaCredito = acertoViagem.Operador.PlanoConta;
                planoContaDebito = acertoViagem.Motorista.PlanoAcertoViagem;
            }
            planoContaMotorista = acertoViagem.Motorista.PlanoAcertoViagem;
            foreach (var diaria in acertoViagem.Diarias)
            {
                if (configuracaoAcertoViagem.TipoMovimentoOutrasDespesas != null && configuracaoAcertoViagem.TipoMovimentoReversaoOutrasDespesas != null && planoContaMotorista != null)
                {
                    tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem.TipoMovimentoOutrasDespesas?.Codigo ?? 0);

                    servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, diaria.Valor, acertoViagem.Numero.ToString(), obsReversao + "PAGAMENTO DE " + diaria.Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoContaCredito, planoContaDebito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                    if (reversao)
                        servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoReversaoOutrasDespesas, DateTime.Now.Date, diaria.Valor, acertoViagem.Numero.ToString(), obsReversao + "MOV CONTABIL PAGAMENTO DE " + diaria.Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, configuracaoAcertoViagem.TipoMovimentoOutrasDespesas.PlanoDeContaDebito, planoContaMotorista, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                    else
                        servProcessoMovimento.GerarMovimentacao(configuracaoAcertoViagem.TipoMovimentoOutrasDespesas, DateTime.Now.Date, diaria.Valor, acertoViagem.Numero.ToString(), obsReversao + "MOV CONTABIL PAGAMENTO DE " + diaria.Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, 0, planoContaMotorista, configuracaoAcertoViagem.TipoMovimentoOutrasDespesas.PlanoDeContaDebito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                }
                else
                {
                    tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(diaria.Justificativa?.TipoMovimentoUsoJustificativa?.Codigo ?? 0);

                    if (!reversao)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDiaria = diaria.Justificativa?.TipoMovimentoUsoJustificativa?.PlanoDeContaDebito ?? acertoViagem.Operador.PlanoConta;
                        servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, diaria.Valor, acertoViagem.Numero.ToString(), obsReversao + "PAGAMENTO DE " + diaria.Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoContaDebito, planoDiaria, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDiaria = diaria.Justificativa?.TipoMovimentoUsoJustificativa?.PlanoDeContaDebito ?? acertoViagem.Operador.PlanoConta;
                        servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, diaria.Valor, acertoViagem.Numero.ToString(), obsReversao + "REVERSAO PAGAMENTO DE " + diaria.Descricao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoDiaria, planoContaCredito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
                    }
                }
            }

            tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem?.TipoMovimentoAbastecimentoPagoPeloMotorista?.Codigo ?? 0);
            List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> abastecimentos = acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.Posto.Modalidades.Any(o => o.TipoModalidade == TipoModalidade.Fornecedor && o.ModalidadesFornecedores.Any(p => !p.PagoPorFatura)) select obj).ToList() : null;
            foreach (var abastecimento in abastecimentos)
                servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, abastecimento.Abastecimento.ValorTotal, acertoViagem.Numero.ToString(), obsReversao + "PAGAMENTO DE ABS. " + abastecimento.Descricao + " " + abastecimento.Abastecimento.Data.Value.ToString("dd/MM/yyyy"), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoContaCredito, planoContaDebito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);

            tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem?.TipoMovimentoPedagioPagoPeloMotorista?.Codigo ?? 0);
            List<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> pedagios = acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where !obj.Pedagio.ImportadoDeSemParar && obj.Pedagio.TipoPedagio == TipoPedagio.Debito select obj).ToList() : null;
            foreach (var pedagio in pedagios)
                servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, pedagio.Pedagio.Valor, acertoViagem.Numero.ToString(), obsReversao + "PAGAMENTO DE PEDÁGIO " + pedagio.Pedagio.Descricao + " " + pedagio.Pedagio.Data.ToString("dd/MM/yyyy"), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoContaCredito, planoContaDebito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);

            List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> outras = acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.TipoPagamento == TipoPagamentoAcertoDespesa.Motorista || (obj.TipoPagamento != TipoPagamentoAcertoDespesa.Empresa && obj.Pessoa.Modalidades.Any(o => o.TipoModalidade == TipoModalidade.Fornecedor && o.ModalidadesFornecedores.Any(p => !p.PagoPorFatura))) select obj).ToList() : null;
            foreach (var outra in outras)
            {
                tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(outra.Justificativa?.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, outra.Quantidade > 0 ? (outra.Quantidade * outra.Valor) : outra.Valor, acertoViagem.Numero.ToString(), obsReversao + "PAGAMENTO DE OUTRAS DESP. " + outra.Descricao + " " + outra.Observacao + " " + outra.Data.ToString("dd/MM/yyyy"), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoContaCredito, planoContaDebito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
            }

            List<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao> bonificacaos = acertoViagem.Bonificacoes != null ? (from obj in acertoViagem.Bonificacoes select obj).ToList() : null;
            foreach (var bonificacao in bonificacaos)
            {
                tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(bonificacao.Justificativa?.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, bonificacao.ValorBonificacao, acertoViagem.Numero.ToString(), obsReversao + "PAGAMENTO DE BONIFICAÇÃO " + bonificacao.Descricao + " " + bonificacao.Data.ToString("dd/MM/yyyy"), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoContaCredito, planoContaDebito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
            }

            //ENTRADA NA CONTA DO OPERADOR
            obsReversao = string.Empty;
            if (reversao)
            {
                obsReversao = "REVERSÃO ";
                planoContaCredito = acertoViagem.Operador.PlanoConta;
                planoContaDebito = acertoViagem.Motorista.PlanoAcertoViagem;
            }
            else
            {
                planoContaCredito = acertoViagem.Motorista.PlanoAcertoViagem;
                planoContaDebito = acertoViagem.Operador.PlanoConta;
            }
            if (valorAdiancamento > 0)
            {
                //tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(configuracaoAcertoViagem?.ipom?.Codigo ?? 0);
                servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, valorAdiancamento, acertoViagem.Numero.ToString(), obsReversao + "RETORNO DO VALOR DO ADIANTAMENTO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoContaCredito, planoContaDebito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
            }

            List<Dominio.Entidades.Embarcador.Acerto.AcertoDesconto> descontos = acertoViagem.Descontos != null ? (from obj in acertoViagem.Descontos select obj).ToList() : null;
            foreach (var desconto in descontos)
            {
                tipoDespesaFinanceira = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(desconto.Justificativa?.TipoMovimentoUsoJustificativa?.Codigo ?? 0);
                servProcessoMovimento.GerarMovimentacao(null, DateTime.Now.Date, desconto.ValorDesconto, acertoViagem.Numero.ToString(), obsReversao + "RETORNO DE DESCONTO " + desconto.Descricao + " " + desconto.Data.ToString("dd/MM/yyyy"), unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto, TipoServicoMultisoftware, acertoViagem.Motorista.Codigo, planoContaCredito, planoContaDebito, 0, null, null, null, null, acertoViagem.CentroResultado, null, null, null, null, null, 0, 0, tipoDespesaFinanceira);
            }
        }

        private class Folga
        {
            public int Codigo { get; set; }
            public string Descricao { get; set; }
            public int Dias { get; set; }
        }

        #endregion
    }
}


