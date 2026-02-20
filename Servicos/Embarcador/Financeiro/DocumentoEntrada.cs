using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Rest.Financeiro;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Financeiro
{
    public class DocumentoEntrada : ServicoBase
    {
        #region Propiedades Privadas

        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        protected string _adminStringConexao;

        #endregion

        #region Construtores

        public DocumentoEntrada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public DocumentoEntrada(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, string stringConexaoAdmin) : base(unitOfWork, tipoServicoMultisoftware, cliente)
        {
            _auditado = auditado;
            _clienteAcesso = clienteURLAcesso;
            _adminStringConexao = stringConexaoAdmin;
        }

        public DocumentoEntrada(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion

        #region Métodos Globais

        public object ObterDetalhesPorNFe(object notaFiscal, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, string documentoImportadoXML)
        {
            if (notaFiscal.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                return this.ObterDetalhesPorNFe(unidadeDeTrabalho, empresa, (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)notaFiscal, tipoServicoMultisoftware, dataEntrada, documentoImportadoXML);
            else if (notaFiscal.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                return this.ObterDetalhesPorNFe(unidadeDeTrabalho, empresa, (MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)notaFiscal, tipoServicoMultisoftware, dataEntrada, documentoImportadoXML);
            else
                return null;
        }

        public object ObterDetalhesPorNFseCuritiba(dynamic nfse, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada)
        {
            return ObterDetalhesPorNFSeCuritiba(unidadeDeTrabalho, empresa, nfse, tipoServicoMultisoftware, dataEntrada);
        }

        public object ObterDetalhesPorCTe(object conhecimento, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, string documentoImportadoXML)
        {
            if (conhecimento.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                return ObterDetalhesPorCTe(unidadeDeTrabalho, empresa, (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)conhecimento, tipoServicoMultisoftware, dataEntrada, documentoImportadoXML);

            if (conhecimento.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                return ObterDetalhesPorCTe(unidadeDeTrabalho, empresa, (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)conhecimento, tipoServicoMultisoftware, dataEntrada, documentoImportadoXML);

            return null;
        }

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS GerarDocumentoEntradaPorNFe(object nfeProc, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuarioLogado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, bool naoControlarKM, bool possuiPermissaoGravarValorDiferente, out string erro, bool lancarDocumentoEntradaAbertoSeKMEstiverErrado)
        {
            erro = string.Empty;
            if (nfeProc == null)
                return null;

            if (nfeProc.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                return GerarDocumentoEntradaPorNFe((MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)nfeProc, empresa, unidadeTrabalho, tipoServicoMultisoftware, usuarioLogado, Auditado, dataCompetencia, dataEntrada, naoControlarKM, possuiPermissaoGravarValorDiferente, out erro);
            else if (nfeProc.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                return GerarDocumentoEntradaPorNFe((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)nfeProc, empresa, unidadeTrabalho, tipoServicoMultisoftware, usuarioLogado, Auditado, dataCompetencia, dataEntrada, naoControlarKM, possuiPermissaoGravarValorDiferente, out erro, lancarDocumentoEntradaAbertoSeKMEstiverErrado);
            else
                return null;
        }

        private Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS GerarDocumentoEntradaPorNFe(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfeProc, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuarioLogado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, bool naoControlarKM, bool possuiPermissaoGravarValorDiferente, out string erro)
        {
            erro = string.Empty;
            return SalvarDocumentoEntradaPorNFe(unidadeTrabalho, empresa, nfeProc, tipoServicoMultisoftware, usuarioLogado, Auditado, dataCompetencia, dataEntrada, naoControlarKM, possuiPermissaoGravarValorDiferente, out erro);
        }

        private Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS GerarDocumentoEntradaPorNFe(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfeProc, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuarioLogado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, bool naoControlarKM, bool possuiPermissaoGravarValorDiferente, out string erro, bool lancarDocumentoEntradaAbertoSeKMEstiverErrado)
        {
            erro = string.Empty;
            return SalvarDocumentoEntradaPorNFe(unidadeTrabalho, empresa, nfeProc, tipoServicoMultisoftware, usuarioLogado, Auditado, dataCompetencia, dataEntrada, naoControlarKM, possuiPermissaoGravarValorDiferente, out erro, lancarDocumentoEntradaAbertoSeKMEstiverErrado);
        }

        public bool AtualizarTitulosAPagar(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceira = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao repConfiguracaoFinanceiraTipoOperacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(StringConexao);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira = repConfiguracaoFinanceira.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracao = null;

            if (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao)
                configuracao = Servicos.Embarcador.Terceiros.ContratoFrete.ObterConfiguracaoFinanceiraPorTipoOperacao(contratoFrete, configuracaoFinanceira, unidadeDeTrabalho);

            if (configuracaoFinanceira == null ||
                (configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete && (configuracaoFinanceira.TipoMovimentoReversaoValorPagoTerceiro == null || configuracaoFinanceira.TipoMovimentoValorPagoTerceiro == null)) ||
                (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao && (configuracao == null || configuracao.TipoMovimentoGeracaoTitulo == null || configuracao.TipoMovimentoReversaoGeracaoTitulo == null)) ||
                (!configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete && !configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao))
            {
                if (configuracaoFinanceira != null && configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao && (configuracao == null || configuracao.TipoMovimentoGeracaoTitulo == null || configuracao.TipoMovimentoReversaoGeracaoTitulo == null))
                {
                    configuracao = repConfiguracaoFinanceiraTipoOperacao.BuscarPrimeiraConfiguracao();
                    if (configuracao == null || configuracao.TipoMovimentoGeracaoTitulo == null || configuracao.TipoMovimentoReversaoGeracaoTitulo == null)
                    {
                        erro = "Não existe configuração financeira para a geração de movimentos do contrato de frete.";
                        return false;
                    }
                }
                else
                {
                    erro = "Não existe configuração financeira para a geração de movimentos do contrato de frete.";
                    return false;
                }
            }

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoUso = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoReversao = null;

            if (configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete)
            {
                tipoMovimentoUso = configuracaoFinanceira.TipoMovimentoValorPagoTerceiro;
                tipoMovimentoReversao = configuracaoFinanceira.TipoMovimentoReversaoValorPagoTerceiro;
            }
            else if (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao)
            {
                tipoMovimentoUso = configuracao.TipoMovimentoGeracaoTitulo;
                tipoMovimentoReversao = configuracao.TipoMovimentoReversaoGeracaoTitulo;
            }

            DateTime dataMovimento;

            if (contratoFrete.UtilizarDataEmissaoParaMovimentoFinanceiro)
                dataMovimento = contratoFrete.DataEmissaoContrato;
            else
                dataMovimento = contratoFrete.Carga.CargaCTes.FirstOrDefault()?.CTe?.DataEmissao ?? contratoFrete.DataEmissaoContrato;

            if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto ||
                contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao ||
                contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado ||
                contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.SemRegra ||
                contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Rejeitado)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosCancelar = repTitulo.BuscarTodosPorContratoFrete(contratoFrete.Codigo);

                if (titulosCancelar.Any(o => o.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada))
                {
                    erro = "Já existem títulos quitados para este contrato de frete, não sendo possível extornar os mesmos.";
                    return false;
                }

                foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo tituloExtornar in titulosCancelar)
                {
                    if (tituloExtornar.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado)
                        continue;

                    if (repTituloBaixa.ExisteAtivaPorTitulo(tituloExtornar.Codigo))
                    {
                        erro = $"O título {tituloExtornar.Codigo} está vinculado à uma baixa, sendo necessário cancelar a mesma para que ele seja extornado.";
                        return false;
                    }

                    if (tituloExtornar.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                    {
                        erro = $"O título {tituloExtornar.Codigo} está quitado, sendo necessário reverter a baixa do mesmo para que ele seja extornado.";
                        return false;
                    }

                    tituloExtornar.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                    tituloExtornar.DataAlteracao = DateTime.Now;
                    tituloExtornar.DataCancelamento = dataMovimento;

                    repTitulo.Atualizar(tituloExtornar);

                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, tipoMovimentoReversao, dataMovimento, tituloExtornar.ValorOriginal, contratoFrete.NumeroContrato.ToString(), $"Referente ao extorno do contrato de frete nº {contratoFrete.NumeroContrato}, título {tituloExtornar.Codigo}.", unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, tituloExtornar.Codigo, null, tituloExtornar.Pessoa))
                        return false;

                    new Servicos.Embarcador.Integracao.IntegracaoTitulo(unidadeDeTrabalho).IniciarIntegracoesDeTitulos(tituloExtornar, TipoAcaoIntegracao.Cancelamento);

                }
            }
            else if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado ||
                     contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada)
            {
                int sequencia = 1;

                if (contratoFrete.ValorAdiantamento > 0m)
                {
                    if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                    {
                        string obsAdiantamento = "Referente ao adiantamento do contrato de frete nº " + contratoFrete.NumeroContrato + ".";

                        Dominio.Entidades.Embarcador.Financeiro.Titulo tituloAdiantamento = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                        tituloAdiantamento.ContratoFrete = contratoFrete;
                        tituloAdiantamento.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                        tituloAdiantamento.DataEmissao = contratoFrete.DataEmissaoContrato;

                        if (contratoFrete.DiasVencimentoAdiantamento > 0)
                            tituloAdiantamento.DataVencimento = DateTime.Now.AddDays(contratoFrete.DiasVencimentoAdiantamento);
                        else
                            tituloAdiantamento.DataVencimento = DateTime.Now;

                        tituloAdiantamento.DataProgramacaoPagamento = tituloAdiantamento.DataVencimento;
                        tituloAdiantamento.Pessoa = contratoFrete.TransportadorTerceiro;
                        tituloAdiantamento.GrupoPessoas = contratoFrete.TransportadorTerceiro.GrupoPessoas;
                        if (tituloAdiantamento.GrupoPessoas == null && tituloAdiantamento.Pessoa != null && tituloAdiantamento.Pessoa.GrupoPessoas != null)
                            tituloAdiantamento.GrupoPessoas = tituloAdiantamento.Pessoa.GrupoPessoas;
                        tituloAdiantamento.Sequencia = sequencia;
                        tituloAdiantamento.ValorOriginal = contratoFrete.ValorAdiantamento;
                        tituloAdiantamento.ValorPendente = tituloAdiantamento.ValorOriginal;
                        tituloAdiantamento.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                        tituloAdiantamento.DataAlteracao = DateTime.Now;
                        tituloAdiantamento.Observacao = obsAdiantamento;
                        tituloAdiantamento.ValorTituloOriginal = tituloAdiantamento.ValorOriginal;
                        tituloAdiantamento.TipoDocumentoTituloOriginal = "Contrato de Frete";
                        tituloAdiantamento.NumeroDocumentoTituloOriginal = contratoFrete.NumeroContrato.ToString();
                        tituloAdiantamento.TipoMovimento = tipoMovimentoUso;
                        tituloAdiantamento.Empresa = contratoFrete?.Carga.Empresa;
                        tituloAdiantamento.Adiantado = true;
                        tituloAdiantamento.DataLancamento = DateTime.Now;
                        tituloAdiantamento.Usuario = contratoFrete?.Usuario;

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                            tituloAdiantamento.TipoAmbiente = tipoAmbiente;

                        repTitulo.Inserir(tituloAdiantamento);

                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, tipoMovimentoUso, dataMovimento, tituloAdiantamento.ValorOriginal, contratoFrete.NumeroContrato.ToString(), obsAdiantamento, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, tituloAdiantamento.Codigo, null, tituloAdiantamento.Pessoa))
                            return false;

                        new Servicos.Embarcador.Integracao.IntegracaoTitulo(unidadeDeTrabalho).IniciarIntegracoesDeTitulos(tituloAdiantamento, TipoAcaoIntegracao.Criacao);

                    }

                    sequencia++;
                }

                if (contratoFrete.SaldoAReceber > 0m &&
                    (contratoFrete.TipoGeracaoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTituloContratoFrete.NaAprovacao && contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado) ||
                    (contratoFrete.TipoGeracaoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTituloContratoFrete.NaAprovacaoEEncerramento && contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada))
                {
                    string obsTotal = "Referente ao pagamento do restante do valor no contrato de frete nº " + contratoFrete.NumeroContrato + ".";

                    Dominio.Entidades.Embarcador.Financeiro.Titulo tituloContrato = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                    tituloContrato.ContratoFrete = contratoFrete;
                    tituloContrato.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                    tituloContrato.DataEmissao = contratoFrete.DataEmissaoContrato;

                    if (contratoFrete.PagamentoAgregado != null)
                        tituloContrato.DataVencimento = contratoFrete.PagamentoAgregado.DataPagamento;
                    else
                    {
                        tituloContrato.DataVencimento = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(contratoFrete);
                    }

                    tituloContrato.DataProgramacaoPagamento = tituloContrato.DataVencimento;
                    tituloContrato.Pessoa = contratoFrete.TransportadorTerceiro;
                    tituloContrato.GrupoPessoas = contratoFrete.TransportadorTerceiro.GrupoPessoas;
                    if (tituloContrato.GrupoPessoas == null && tituloContrato.Pessoa != null && tituloContrato.Pessoa.GrupoPessoas != null)
                        tituloContrato.GrupoPessoas = tituloContrato.Pessoa.GrupoPessoas;
                    tituloContrato.Sequencia = sequencia;
                    tituloContrato.ValorOriginal = contratoFrete.SaldoAReceber;
                    tituloContrato.ValorPendente = tituloContrato.ValorOriginal;
                    tituloContrato.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    tituloContrato.DataAlteracao = DateTime.Now;
                    tituloContrato.Observacao = obsTotal;
                    tituloContrato.ValorTituloOriginal = tituloContrato.ValorOriginal;
                    tituloContrato.TipoDocumentoTituloOriginal = "Contrato de Frete";
                    tituloContrato.NumeroDocumentoTituloOriginal = contratoFrete.NumeroContrato.ToString();
                    tituloContrato.TipoMovimento = tipoMovimentoUso;
                    tituloContrato.Empresa = contratoFrete.Carga?.Empresa;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        tituloContrato.TipoAmbiente = tipoAmbiente;

                    repTitulo.Inserir(tituloContrato);

                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, tipoMovimentoUso, dataMovimento, tituloContrato.ValorOriginal, contratoFrete.NumeroContrato.ToString(), obsTotal, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, tituloContrato.Codigo, null, tituloContrato.Pessoa))
                        return false;

                    new Servicos.Embarcador.Integracao.IntegracaoTitulo(unidadeDeTrabalho).IniciarIntegracoesDeTitulos(tituloContrato, TipoAcaoIntegracao.Criacao);

                }
            }

            erro = string.Empty;
            return true;
        }

        public bool AtualizarTitulosAPagar(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia, bool lancarDocumentoEntradaAbertoSeKMEstiverErrado)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa repDocumentoEntradaCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unidadeDeTrabalho);

            ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(StringConexao);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            if (repTituloBaixa.ContarPorDocumentoEntrada(documentoEntrada.Codigo) > 0)
            {
                erro = "Já existem títulos em negociação para este documento de entrada, não sendo possível atualizar os mesmos.";
                return false;
            }

            if (repTitulo.ContarPorStatusEDocumentoEntrada(documentoEntrada.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada) > 0)
            {
                erro = "Já existem títulos quitados para este documento de entrada, não sendo possível atualizar os mesmos.";
                return false;
            }

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (!documentoEntrada.NaturezaOperacao.GerarMovimentoAutomaticoEntrada || documentoEntrada.NaturezaOperacao.TipoMovimentoUsoEntrada == null || documentoEntrada.NaturezaOperacao.TipoMovimentoReversaoEntrada == null)
                {
                    if (!documentoEntrada.Modelo.GerarMovimentoAutomaticoEntrada || documentoEntrada.Modelo.TipoMovimentoUsoEntrada == null || documentoEntrada.Modelo.TipoMovimentoReversaoEntrada == null)
                    {
                        erro = "O modelo de documento e/ou a natureza da operação não estão configurados para geração dos movimentos financeiros, não sendo possível finalizar o documento.";
                        return false;
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata> duplicatas = repDuplicata.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            decimal valorRetencaoCOFINS = itens.Where(o => o.CFOP != null && o.CFOP.ReduzValorLiquidoRetencaoCOFINS).Select(o => o.ValorRetencaoCOFINS).Sum();
            decimal valorRetencaoCSLL = itens.Where(o => o.CFOP != null && o.CFOP.ReduzValorLiquidoRetencaoCSLL).Select(o => o.ValorRetencaoCSLL).Sum();
            decimal valorRetencaoINSS = itens.Where(o => o.CFOP != null && o.CFOP.ReduzValorLiquidoRetencaoINSS).Select(o => o.ValorRetencaoINSS).Sum();
            decimal valorRetencaoIPI = itens.Where(o => o.CFOP != null && o.CFOP.ReduzValorLiquidoRetencaoIPI).Select(o => o.ValorRetencaoIPI).Sum();
            decimal valorRetencaoIR = itens.Where(o => o.CFOP != null && o.CFOP.ReduzValorLiquidoRetencaoIR).Select(o => o.ValorRetencaoIR).Sum();
            decimal valorRetencaoISS = itens.Where(o => o.CFOP != null && o.CFOP.ReduzValorLiquidoRetencaoISS).Select(o => o.ValorRetencaoISS).Sum();
            decimal valorRetencaoOutras = itens.Where(o => o.CFOP != null && o.CFOP.ReduzValorLiquidoRetencaoOutras).Select(o => o.ValorRetencaoOutras).Sum();
            decimal valorRetencaoPIS = itens.Where(o => o.CFOP != null && o.CFOP.ReduzValorLiquidoRetencaoPIS).Select(o => o.ValorRetencaoPIS).Sum();

            decimal totalRetencao = valorRetencaoCOFINS + valorRetencaoCSLL + valorRetencaoINSS + valorRetencaoIPI + valorRetencaoIR + valorRetencaoISS + valorRetencaoOutras + valorRetencaoPIS;

            if (documentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
            {
                if (duplicatas == null || duplicatas.Count <= 0)
                {
                    if (documentoEntrada.Fornecedor != null && documentoEntrada.IndicadorPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.APrazo)
                    {
                        if (LancarDuplicatasAutomaticas(documentoEntrada, unidadeDeTrabalho))
                            duplicatas = repDuplicata.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);
                    }
                }

                if (documentoEntrada.NaturezaOperacao.GeraTitulo)
                {
                    if (duplicatas.Count <= 0)
                    {
                        erro = "Esta natureza de operação requer a geração de ao menos uma duplicata.";
                        return lancarDocumentoEntradaAbertoSeKMEstiverErrado;
                    }
                }

                bool contemValorBonificacao = itens.Any(o => o.NaturezaOperacao != null && o.NaturezaOperacao.Bonificacao);
                decimal totalSemBonificacao = itens.Where(o => o.NaturezaOperacao != null && !o.NaturezaOperacao.Bonificacao)?.Sum(o => o.ValorCustoTotal) ?? 0m;

                if (totalSemBonificacao > 0 && contemValorBonificacao)
                {
                    if (duplicatas.Count > 0 && Math.Round(duplicatas.Sum(o => o.Valor), 2) != Math.Round(totalSemBonificacao, 2) && totalRetencao <= 0)
                    {
                        erro = "O valor total das duplicatas (" + duplicatas.Sum(o => o.Valor).ToString("n2") + ") deve ser igual ao valor total do documento de entrada sem bonificação (" + totalSemBonificacao.ToString("n2") + ").";
                        return lancarDocumentoEntradaAbertoSeKMEstiverErrado;
                    }
                }
                else if (duplicatas.Count > 0 && Math.Round(duplicatas.Sum(o => o.Valor), 2) != Math.Round(documentoEntrada.ValorTotal, 2) && totalRetencao <= 0)
                {
                    erro = "O valor total das duplicatas (" + duplicatas.Sum(o => o.Valor).ToString("n2") + ") deve ser igual ao valor total do documento de entrada (" + documentoEntrada.ValorTotal.ToString("n2") + ").";
                    return lancarDocumentoEntradaAbertoSeKMEstiverErrado;
                }
            }

            decimal valorTotalDuplicatas = duplicatas.Sum(o => o.Valor);
            if (totalRetencao > 0 && (valorTotalDuplicatas == documentoEntrada.ValorTotal))
            {
                decimal valorTotalFinal = documentoEntrada.ValorTotal - totalRetencao;
                decimal valorDuplicatas = 0;
                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicata in duplicatas)
                {
                    duplicata.Valor = Math.Round((duplicata.Valor - ((duplicata.Valor / valorTotalDuplicatas) * totalRetencao)), 2);
                    valorDuplicatas += duplicata.Valor;
                    repDuplicata.Atualizar(duplicata);
                }
                if (valorTotalFinal != valorDuplicatas)
                {
                    decimal diferenca = valorTotalFinal - valorDuplicatas;
                    if (diferenca > 0)
                        duplicatas[0].Valor = Math.Round(duplicatas[0].Valor + diferenca, 2);
                    else
                        duplicatas[0].Valor = Math.Round(duplicatas[0].Valor - (diferenca * -1), 2);
                    repDuplicata.Atualizar(duplicatas[0]);
                }
            }
            bool contemDuplicatas = false;
            duplicatas = repDuplicata.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);
            int sequencia = 1;

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicata in duplicatas)
            {
                contemDuplicatas = true;
                if (documentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorDuplicataDocumentoEntrada(duplicata.Codigo);

                    if (titulo == null)
                    {
                        titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                        titulo.DuplicataDocumentoEntrada = duplicata;
                        titulo.DataLancamento = DateTime.Now;
                        titulo.Usuario = duplicata.DocumentoEntrada.OperadorLancamentoDocumento;
                    }

                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                    titulo.DataEmissao = dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao;
                    titulo.DataVencimento = duplicata.DataVencimento;
                    titulo.DataProgramacaoPagamento = duplicata.DataVencimento;
                    titulo.GrupoPessoas = documentoEntrada.Fornecedor.GrupoPessoas;
                    titulo.Pessoa = documentoEntrada.Fornecedor;
                    titulo.Sequencia = sequencia;
                    titulo.ValorOriginal = documentoEntrada.ValorMoedaCotacao > 0 ? (Math.Round((documentoEntrada.ValorMoedaCotacao * duplicata.Valor), 2, MidpointRounding.AwayFromZero)) : duplicata.Valor;
                    titulo.ValorPendente = documentoEntrada.ValorMoedaCotacao > 0 ? (Math.Round((documentoEntrada.ValorMoedaCotacao * duplicata.Valor), 2, MidpointRounding.AwayFromZero)) : duplicata.Valor;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.Observacao = $"Referente à duplicata nº {duplicata.Numero} do documento de entrada nº {documentoEntrada.Numero}.";
                    titulo.Observacao += !string.IsNullOrWhiteSpace(duplicata.Observacao) ? " " + duplicata.Observacao : string.Empty;
                    titulo.Empresa = documentoEntrada.Destinatario;
                    titulo.ValorTituloOriginal = titulo.ValorOriginal;
                    titulo.TipoDocumentoTituloOriginal = "Documento de Entrada";
                    titulo.NumeroDocumentoTituloOriginal = documentoEntrada.Numero.ToString();
                    if (duplicata.Forma == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros && documentoEntrada.Fornecedor.FormaTituloFornecedor.HasValue)
                        titulo.FormaTitulo = documentoEntrada.Fornecedor.FormaTituloFornecedor.Value;
                    else
                        titulo.FormaTitulo = duplicata.Forma;
                    titulo.NossoNumero = duplicata.NumeroBoleto;
                    titulo.Portador = duplicata.Portador;

                    titulo.MoedaCotacaoBancoCentral = documentoEntrada.MoedaCotacaoBancoCentral;
                    titulo.DataBaseCRT = documentoEntrada.DataBaseCRT;
                    titulo.ValorMoedaCotacao = documentoEntrada.ValorMoedaCotacao;
                    titulo.ValorOriginalMoedaEstrangeira = duplicata.Valor;

                    if (!string.IsNullOrWhiteSpace(titulo.NossoNumero))
                    {
                        if (repTitulo.ContemTituloNossoNumeroDuplicado(titulo.Codigo, titulo.NossoNumero))
                        {
                            erro = "Já existe um título a pagar lançado com o mesmo número de boleto para o pagamento eletrônico.";
                            return lancarDocumentoEntradaAbertoSeKMEstiverErrado;
                        }
                    }

                    if (dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada && titulo.DataVencimento.Value.Date < duplicata.DocumentoEntrada.DataEntrada.Date)
                    {
                        erro = "A data de vencimento das duplicatas não podem ser menor que a data de entrada do documento.";
                        return lancarDocumentoEntradaAbertoSeKMEstiverErrado;
                    }

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        titulo.TipoAmbiente = tipoAmbiente;
                        titulo.TipoMovimento = documentoEntrada.NaturezaOperacao.TipoMovimento != null ? documentoEntrada.NaturezaOperacao.TipoMovimento : null;
                    }
                    else
                    {
                        if (documentoEntrada.NaturezaOperacao.TipoMovimentoUsoEntrada != null)
                            titulo.TipoMovimento = documentoEntrada.NaturezaOperacao.TipoMovimentoUsoEntrada;
                        else
                            titulo.TipoMovimento = documentoEntrada.Modelo.TipoMovimentoUsoEntrada;
                    }

                    if (titulo.Codigo > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Atualizado pelo Documento de Entrada.", unidadeDeTrabalho);
                        repTitulo.Atualizar(titulo);
                    }
                    else
                    {
                        if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                            titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                        repTitulo.Inserir(titulo);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Adicionado pelo Documento de Entrada.", unidadeDeTrabalho);
                    }

                    List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> lancamentos = repLancamentoCentroResultado.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

                    if (lancamentos != null && lancamentos.Count > 0)
                    {
                        int countCentroResultado = lancamentos.Count;
                        decimal valorTotalRateado = 0m;
                        for (int i = 0; i < lancamentos.Count; i++)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado = repLancamentoCentroResultado.BuscarPorCodigo(lancamentos[i].Codigo, false);
                            Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamento = new Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado();

                            lancamento.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoLancamentoCentroResultado.Titulo;
                            lancamento.Ativo = true;
                            lancamento.Titulo = titulo;
                            lancamento.Data = DateTime.Now;
                            lancamento.CentroResultado = lancamentoCentroResultado.CentroResultado;
                            lancamento.Percentual = lancamentoCentroResultado.Percentual;

                            if ((i + 1) == countCentroResultado)
                                lancamento.Valor = titulo.ValorPendente - valorTotalRateado;
                            else
                                lancamento.Valor = Math.Round(Math.Floor(titulo.ValorPendente * lancamento.Percentual) / 100, 2, MidpointRounding.AwayFromZero);

                            valorTotalRateado += lancamento.Valor;

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, "Adicionou o centro de resultados " + lancamento.Descricao, unidadeDeTrabalho);
                            repLancamentoCentroResultado.Inserir(lancamento, Auditado);
                        }
                    }

                    if (configuracaoFinanceiro.AtivarControleDespesas)
                    {
                        List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa> centrosResultadoTiposDespesa = repDocumentoEntradaCentroResultadoTipoDespesa.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa centroResultadoTipoDespesa in centrosResultadoTiposDespesa)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa tituloCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa();

                            tituloCentroResultadoTipoDespesa.Titulo = titulo;
                            tituloCentroResultadoTipoDespesa.TipoDespesaFinanceira = centroResultadoTipoDespesa.TipoDespesaFinanceira;
                            tituloCentroResultadoTipoDespesa.CentroResultado = centroResultadoTipoDespesa.CentroResultado;
                            tituloCentroResultadoTipoDespesa.Percentual = centroResultadoTipoDespesa.Percentual;

                            repTituloCentroResultadoTipoDespesa.Inserir(tituloCentroResultadoTipoDespesa);
                        }
                    }

                    if (documentoEntrada.Veiculos != null && documentoEntrada.Veiculos.Count > 0)
                    {
                        if (titulo.Veiculos == null)
                            titulo.Veiculos = new List<Dominio.Entidades.Veiculo>();
                        foreach (var veic in documentoEntrada.Veiculos)
                        {
                            if (!titulo.Veiculos.Any(v => v.Codigo == veic.Codigo))
                            {
                                Dominio.Entidades.Veiculo veicc = repVeiculo.BuscarPorCodigo(veic.Codigo);

                                titulo.Veiculos.Add(veicc);
                            }
                            repTitulo.Atualizar(titulo);
                        }
                    }

                    string observacaoMovimento = "Referente à emissão da duplicata nº " + duplicata.Numero + " do documento de entrada nº " + documentoEntrada.Numero + ".";
                    if (configuracaoTMS.GerarMovimentacaoNaBaixaIndividualmente)
                        observacaoMovimento = "NF " + documentoEntrada.Numero + " - " + (documentoEntrada.Fornecedor?.Nome ?? "");
                    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        if (documentoEntrada.NaturezaOperacao.GerarMovimentoAutomaticoEntrada && documentoEntrada.NaturezaOperacao.TipoMovimentoUsoEntrada != null)
                        {
                            if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.NaturezaOperacao.TipoMovimentoUsoEntrada, dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao, titulo.ValorOriginal, duplicata.Numero, observacaoMovimento, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, null, documentoEntrada.DataEmissao))
                                return false;
                        }
                        else
                        {
                            if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.Modelo.TipoMovimentoUsoEntrada, dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao, titulo.ValorOriginal, duplicata.Numero, observacaoMovimento, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, null, documentoEntrada.DataEmissao))
                                return false;
                        }
                    }
                    else
                    {
                        if (documentoEntrada.NaturezaOperacao.GeraTitulo && documentoEntrada.NaturezaOperacao.TipoMovimento != null)
                        {
                            if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.NaturezaOperacao.TipoMovimento, dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao, titulo.ValorOriginal, duplicata.Numero, observacaoMovimento, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, null, documentoEntrada.DataEmissao))
                                return false;
                        }
                    }

                    sequencia++;
                }
                else if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarTodosPorDuplicataDocumentoEntrada(duplicata.Codigo);
                    if (titulo != null)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorTitulo(titulo.Codigo);
                        if (tituloBaixa != null)
                        {
                            erro = "Existe título vinculado a uma baixa, favor cancelar a mesma para efetuar esse procedimento.";
                            return false;
                        }

                        if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                        {
                            erro = "Existe título que se encontra quitado, favor reverta o mesmo.";
                            return false;
                        }

                        if (titulo.DataAutorizacao.HasValue)
                        {
                            erro = "Existe título que se encontra com pagamento autorizado, favor desfazer a autorização do mesmo.";
                            return false;
                        }

                        if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        {
                            if (documentoEntrada.NaturezaOperacao.GerarMovimentoAutomaticoEntrada && documentoEntrada.NaturezaOperacao.TipoMovimentoReversaoEntrada != null)
                            {
                                if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.NaturezaOperacao.TipoMovimentoReversaoEntrada, dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao, titulo.ValorOriginal, duplicata.Numero, "Referente ao extorno da duplicata nº " + duplicata.Numero + " do documento de entrada nº " + documentoEntrada.Numero + ".", unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, null, documentoEntrada.DataEmissao))
                                    return false;
                            }
                            else
                            {
                                if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.Modelo.TipoMovimentoReversaoEntrada, dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao, titulo.ValorOriginal, duplicata.Numero, "Referente ao extorno da duplicata nº " + duplicata.Numero + " do documento de entrada nº " + documentoEntrada.Numero + ".", unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, null, documentoEntrada.DataEmissao))
                                    return false;
                            }
                        }
                        else
                        {
                            if (documentoEntrada.NaturezaOperacao.GeraTitulo && documentoEntrada.NaturezaOperacao.TipoMovimento != null)
                            {
                                if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao, titulo.ValorOriginal, duplicata.Numero, "Referente ao extorno da duplicata nº " + duplicata.Numero + " do documento de entrada nº " + documentoEntrada.Numero + ".", unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, documentoEntrada.NaturezaOperacao.TipoMovimento.PlanoDeContaDebito, documentoEntrada.NaturezaOperacao.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, titulo.Pessoa, null, documentoEntrada.DataEmissao))
                                    return false;
                            }
                        }

                        repTitulo.RemoverTituloDoMovimentoFinanceiro(titulo.Codigo, unidadeDeTrabalho);
                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.DataCancelamento = dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao;
                        titulo.DuplicataDocumentoEntrada = null;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Cancelado pelo Documento de Entrada.", unidadeDeTrabalho);
                        repTitulo.Atualizar(titulo);
                    }
                }
            }

            if (contemDuplicatas && documentoEntrada.IndicadorPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.AVista)
            {
                documentoEntrada.IndicadorPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.APrazo;
                repDocumentoEntrada.Atualizar(documentoEntrada);
            }

            erro = string.Empty;
            return true;
        }

        public bool AtualizarGuiasAPagar(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaGuia repGuia = new Repositorio.Embarcador.Financeiro.DocumentoEntradaGuia(unidadeDeTrabalho);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(StringConexao);

            if (repTituloBaixa.ContarGuiaPorDocumentoEntrada(documentoEntrada.Codigo) > 0)
            {
                erro = "Já existem guias em negociação para este documento de entrada, não sendo possível atualizar os mesmos.";
                return false;
            }

            if (repTitulo.ContarGuiaPorStatusEDocumentoEntrada(documentoEntrada.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada) > 0)
            {
                erro = "Já existem guias quitados para este documento de entrada, não sendo possível atualizar os mesmos.";
                return false;
            }
            if ((documentoEntrada.Guias == null || documentoEntrada.Guias.Count == 0) && (documentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado))
                GerarGuiasDocumentoEntrada(documentoEntrada, unidadeDeTrabalho, dataCompetencia);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia> guias = repGuia.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            int sequencia = 1;

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia in guias)
            {
                if (documentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorGuiaDocumentoEntrada(guia.Codigo);

                    if (titulo == null)
                    {
                        titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                        titulo.DocumentoEntradaGuia = guia;
                        titulo.DataLancamento = DateTime.Now;
                        titulo.Usuario = documentoEntrada.OperadorLancamentoDocumento;
                    }

                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                    titulo.DataEmissao = dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao;
                    titulo.DataVencimento = guia.DataVencimento;
                    titulo.DataProgramacaoPagamento = guia.DataVencimento;
                    titulo.GrupoPessoas = guia.Fornecedor.GrupoPessoas;
                    titulo.Pessoa = guia.Fornecedor;
                    titulo.Sequencia = sequencia;
                    titulo.ValorOriginal = documentoEntrada.ValorMoedaCotacao > 0 ? (Math.Round((documentoEntrada.ValorMoedaCotacao * guia.Valor), 2, MidpointRounding.AwayFromZero)) : guia.Valor;
                    titulo.ValorPendente = documentoEntrada.ValorMoedaCotacao > 0 ? (Math.Round((documentoEntrada.ValorMoedaCotacao * guia.Valor), 2, MidpointRounding.AwayFromZero)) : guia.Valor;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.Observacao = string.Concat("Referente à guia nº " + guia.Numero + " do documento de entrada nº " + documentoEntrada.Numero.ToString() + ".");
                    titulo.Empresa = documentoEntrada.Destinatario;
                    titulo.ValorTituloOriginal = titulo.ValorOriginal;
                    titulo.TipoDocumentoTituloOriginal = "Documento de Entrada";
                    titulo.NumeroDocumentoTituloOriginal = documentoEntrada.Numero.ToString();
                    titulo.DocumentoEntradaGuia = guia;
                    titulo.TipoMovimento = guia.TipoMovimentoGeracao;

                    titulo.MoedaCotacaoBancoCentral = documentoEntrada.MoedaCotacaoBancoCentral;
                    titulo.DataBaseCRT = documentoEntrada.DataBaseCRT;
                    titulo.ValorMoedaCotacao = documentoEntrada.ValorMoedaCotacao;
                    titulo.ValorOriginalMoedaEstrangeira = guia.Valor;

                    if (titulo.Codigo > 0)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Atualizado título.", unidadeDeTrabalho);
                        repTitulo.Atualizar(titulo);
                    }
                    else
                    {
                        if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                            titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                        repTitulo.Inserir(titulo);
                    }

                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, guia.TipoMovimentoGeracao, dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao, titulo.ValorOriginal, guia.Numero, "Referente à emissão da guia nº " + guia.Numero + " do documento de entrada nº " + documentoEntrada.Numero + ".", unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, null, documentoEntrada.DataEmissao))
                        return false;

                    sequencia++;
                }
                else if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarTodosPorGuiaDocumentoEntrada(guia.Codigo);
                    if (titulo != null)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorTitulo(titulo.Codigo);
                        if (tituloBaixa != null)
                        {
                            erro = "Existe título vinculado a uma baixa, favor cancelar a mesma para efetuar esse procedimento.";
                            return false;
                        }

                        if (titulo.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                        {
                            erro = "Existe título que se encontra quitado, favor reverta o mesmo.";
                            return false;
                        }

                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, guia.TipoMovimentoReversao, dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao, titulo.ValorOriginal, guia.Numero, "Referente ao extorno da guia nº " + guia.Numero + " do documento de entrada nº " + documentoEntrada.Numero + ".", unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, titulo.Pessoa, null, documentoEntrada.DataEmissao))
                            return false;

                        repTitulo.RemoverTituloDoMovimentoFinanceiro(titulo.Codigo, unidadeDeTrabalho);
                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.DataCancelamento = dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao;
                        titulo.DuplicataDocumentoEntrada = null;
                        titulo.DocumentoEntradaGuia = null;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Cancelado título.", unidadeDeTrabalho);
                        repTitulo.Atualizar(titulo);
                    }
                    repGuia.Deletar(guia);
                }
            }

            erro = string.Empty;
            return true;
        }

        public bool GerarMovimentoFinanceiroDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado || documentoEntrada.Modelo == null)
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(StringConexao);

            DateTime dataMovimento = dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao;
            string observacaoMovimentos = "Referente à emissão do documento de entrada nº " + documentoEntrada.Numero + ".";

            if (documentoEntrada.Modelo.GerarMovimentoBaseSTRetido && documentoEntrada.Modelo.TipoMovimentoBaseSTRetidoEmissao != null && documentoEntrada.BaseSTRetido > 0)
            {
                if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.Modelo.TipoMovimentoBaseSTRetidoEmissao, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (documentoEntrada.BaseSTRetido * documentoEntrada.ValorMoedaCotacao) : documentoEntrada.BaseSTRetido), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                    return false;
            }

            if (documentoEntrada.Modelo.GerarMovimentoValorSTRetido && documentoEntrada.Modelo.TipoMovimentoValorSTRetidoEmissao != null && documentoEntrada.ValorSTRetido > 0)
            {
                if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.Modelo.TipoMovimentoValorSTRetidoEmissao, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (documentoEntrada.ValorSTRetido * documentoEntrada.ValorMoedaCotacao) : documentoEntrada.ValorSTRetido), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        public bool ReverterMovimentoFinanceiroDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto || documentoEntrada.Modelo == null)
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(StringConexao);

            DateTime dataMovimento = dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao;
            string observacaoMovimentos = "Referente à reversão de faturamento do documento de entrada nº " + documentoEntrada.Numero + ".";

            if (documentoEntrada.Modelo.GerarMovimentoBaseSTRetido && documentoEntrada.Modelo.TipoMovimentoBaseSTRetidoReversao != null && documentoEntrada.BaseSTRetido > 0)
            {
                if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.Modelo.TipoMovimentoBaseSTRetidoReversao, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (documentoEntrada.BaseSTRetido * documentoEntrada.ValorMoedaCotacao) : documentoEntrada.BaseSTRetido), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                    return false;
            }

            if (documentoEntrada.Modelo.GerarMovimentoValorSTRetido && documentoEntrada.Modelo.TipoMovimentoValorSTRetidoReversao != null && documentoEntrada.ValorSTRetido > 0)
            {
                if (!svcProcessoMovimento.GerarMovimentacao(out erro, documentoEntrada.Modelo.TipoMovimentoValorSTRetidoReversao, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (documentoEntrada.ValorSTRetido * documentoEntrada.ValorMoedaCotacao) : documentoEntrada.ValorSTRetido), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        public bool GerarMovimentoFinanceiroEmissaoItens(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(unidadeDeTrabalho);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(StringConexao);

            DateTime dataMovimento = dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao;

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensDocumentoEntrada = repItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itensDocumentoEntrada)
            {
                if (item.TipoMovimento == null)
                {
                    erro = "O item " + item.Sequencial + " está sem tipo de movimento configurado.";
                    return false;
                }

                string observacaoMovimentos = "Referente à emissão do documento de entrada nº " + documentoEntrada.Numero + ", item " + item.Sequencial + " - " + item.Produto.Descricao + ".";

                List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> centros = repLancamentoCentroResultado.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

                int countCentros = centros?.Count ?? 0;

                if (countCentros > 0)
                {
                    decimal valorTotalRateado = 0m;

                    for (int i = 0; i < countCentros; i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado centro = centros[i];

                        decimal valorCentro = 0m;

                        if ((i + 1) == countCentros)
                            valorCentro = (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorTotal) - valorTotalRateado;
                        else
                            valorCentro = Math.Round(Math.Floor((documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorTotal) * centro.Percentual) / 100, 2, MidpointRounding.AwayFromZero);

                        valorTotalRateado += valorCentro;

                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.TipoMovimento, dataMovimento, valorCentro, documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao, centro.CentroResultado))
                            return false;
                    }
                }
                else if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.TipoMovimento, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorTotal), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                    return false;

                if (item.CFOP.GerarMovimentoAutomaticoCOFINS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoCOFINS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCOFINS * documentoEntrada.ValorMoedaCotacao) : item.ValorCOFINS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoDesconto)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoDesconto, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.Desconto * documentoEntrada.ValorMoedaCotacao) : item.Desconto), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoDiferencial)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoDiferencial, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorDiferencial * documentoEntrada.ValorMoedaCotacao) : item.ValorDiferencial), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoFrete)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoFrete, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorFrete * documentoEntrada.ValorMoedaCotacao) : item.ValorFrete), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoICMS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoICMS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorICMS * documentoEntrada.ValorMoedaCotacao) : item.ValorICMS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoICMSST)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoICMSST, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorICMSST * documentoEntrada.ValorMoedaCotacao) : item.ValorICMSST), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoIPI)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoIPI, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorIPI * documentoEntrada.ValorMoedaCotacao) : item.ValorIPI), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoOutrasDespesas)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoOutrasDespesas, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.OutrasDespesas * documentoEntrada.ValorMoedaCotacao) : item.OutrasDespesas), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoPIS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoPIS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorPIS * documentoEntrada.ValorMoedaCotacao) : item.ValorPIS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoSeguro)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoSeguro, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorSeguro * documentoEntrada.ValorMoedaCotacao) : item.ValorSeguro), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoFreteFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoFreteFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorFreteFora * documentoEntrada.ValorMoedaCotacao) : item.ValorFreteFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoOutrasFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoOutrasFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorOutrasDespesasFora * documentoEntrada.ValorMoedaCotacao) : item.ValorOutrasDespesasFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoDescontoFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoDescontoFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorDescontoFora * documentoEntrada.ValorMoedaCotacao) : item.ValorDescontoFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoImpostoFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoImpostoFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorImpostosFora * documentoEntrada.ValorMoedaCotacao) : item.ValorImpostosFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoDiferencialFreteFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoDiferencialFreteFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorDiferencialFreteFora * documentoEntrada.ValorMoedaCotacao) : item.ValorDiferencialFreteFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoICMSFreteFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoICMSFreteFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorICMSFreteFora * documentoEntrada.ValorMoedaCotacao) : item.ValorICMSFreteFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoCusto)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoCusto, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCustoTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorCustoTotal), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoCOFINS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoRetencaoCOFINS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoCOFINS * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoCOFINS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoCSLL)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoRetencaoCSLL, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoCSLL * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoCSLL), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoISS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoRetencaoISS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoISS * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoISS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoIR)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoRetencaoIR, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoIR * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoIR), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoINSS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoRetencaoINSS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoINSS * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoINSS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoIPI)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoRetencaoIPI, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoIPI * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoIPI), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoOutras)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoRetencaoOutras, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoOutras * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoOutras), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoPIS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoUsoRetencaoPIS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoPIS * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoPIS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }
            }

            erro = string.Empty;
            return true;
        }

        public bool GerarMovimentoFinanceiroReversaoItens(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.LancamentoCentroResultado repLancamentoCentroResultado = new Repositorio.Embarcador.Financeiro.LancamentoCentroResultado(unidadeDeTrabalho);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(StringConexao);

            DateTime dataMovimento = dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao;

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensDocumentoEntrada = repItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itensDocumentoEntrada)
            {
                if (item.TipoMovimento == null)
                {
                    erro = "O item " + item.Sequencial + " está sem tipo de movimento configurado.";
                    return false;
                }

                string observacaoMovimentos = "Referente à reversão de faturamento do documento de entrada nº " + documentoEntrada.Numero + ", item " + item.Sequencial + " - " + item.Produto.Descricao + ".";

                List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado> centros = repLancamentoCentroResultado.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

                int countCentros = centros?.Count ?? 0;

                if (countCentros > 0)
                {
                    decimal valorTotalRateado = 0m;

                    for (int i = 0; i < countCentros; i++)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado centro = centros[i];

                        decimal valorCentro = 0m;

                        if ((i + 1) == countCentros)
                            valorCentro = (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorTotal) - valorTotalRateado;
                        else
                            valorCentro = Math.Round(Math.Floor((documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorTotal) * centro.Percentual) / 100, 2, MidpointRounding.AwayFromZero);

                        valorTotalRateado += valorCentro;

                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, dataMovimento, valorCentro, documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, item.TipoMovimento.PlanoDeContaDebito, item.TipoMovimento.PlanoDeContaCredito, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao, centro.CentroResultado))
                            return false;
                    }
                }
                else if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorTotal), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, item.TipoMovimento.PlanoDeContaDebito, item.TipoMovimento.PlanoDeContaCredito, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                    return false;

                if (item.CFOP.GerarMovimentoAutomaticoCOFINS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoCOFINS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCOFINS * documentoEntrada.ValorMoedaCotacao) : item.ValorCOFINS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoDesconto)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoDesconto, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.Desconto * documentoEntrada.ValorMoedaCotacao) : item.Desconto), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoDiferencial)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoDiferencial, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorDiferencial * documentoEntrada.ValorMoedaCotacao) : item.ValorDiferencial), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoFrete)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoFrete, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorFrete * documentoEntrada.ValorMoedaCotacao) : item.ValorFrete), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoICMS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoICMS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorICMS * documentoEntrada.ValorMoedaCotacao) : item.ValorICMS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoICMSST)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoICMSST, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorICMSST * documentoEntrada.ValorMoedaCotacao) : item.ValorICMSST), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoIPI)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoIPI, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorIPI * documentoEntrada.ValorMoedaCotacao) : item.ValorIPI), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoOutrasDespesas)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoOutrasDespesas, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.OutrasDespesas * documentoEntrada.ValorMoedaCotacao) : item.OutrasDespesas), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoPIS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoPIS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorPIS * documentoEntrada.ValorMoedaCotacao) : item.ValorPIS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoSeguro)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoSeguro, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorSeguro * documentoEntrada.ValorMoedaCotacao) : item.ValorSeguro), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoFreteFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoFreteFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorFreteFora * documentoEntrada.ValorMoedaCotacao) : item.ValorFreteFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoOutrasFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoOutrasFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorOutrasDespesasFora * documentoEntrada.ValorMoedaCotacao) : item.ValorOutrasDespesasFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoDescontoFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoDescontoFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorDescontoFora * documentoEntrada.ValorMoedaCotacao) : item.ValorDescontoFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoImpostoFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoImpostoFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorImpostosFora * documentoEntrada.ValorMoedaCotacao) : item.ValorImpostosFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoDiferencialFreteFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoDiferencialFreteFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorDiferencialFreteFora * documentoEntrada.ValorMoedaCotacao) : item.ValorDiferencialFreteFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoICMSFreteFora)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoICMSFreteFora, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorICMSFreteFora * documentoEntrada.ValorMoedaCotacao) : item.ValorICMSFreteFora), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoCusto)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoCusto, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCustoTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorCustoTotal), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoCOFINS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoRetencaoCOFINS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoCOFINS * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoCOFINS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoCSLL)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoRetencaoCSLL, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoCSLL * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoCSLL), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoISS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoRetencaoISS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoISS * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoISS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoIR)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoRetencaoIR, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoIR * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoIR), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoINSS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoRetencaoINSS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoINSS * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoINSS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoIPI)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoRetencaoIPI, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoIPI * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoIPI), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoOutras)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoRetencaoOutras, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoOutras * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoOutras), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }

                if (item.CFOP.GerarMovimentoAutomaticoRetencaoPIS)
                {
                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, item.CFOP.TipoMovimentoReversaoRetencaoPIS, dataMovimento, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorRetencaoPIS * documentoEntrada.ValorMoedaCotacao) : item.ValorRetencaoPIS), documentoEntrada.Numero.ToString(), observacaoMovimentos, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada, tipoServicoMultisoftware, 0, null, null, 0, null, documentoEntrada.Fornecedor, null, documentoEntrada.DataEmissao))
                        return false;
                }
            }

            erro = string.Empty;
            return true;
        }

        public bool MovimentarEstoque(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                if (item.NaturezaOperacao == null || !item.NaturezaOperacao.ControlaEstoque || item.CFOP == null || !item.CFOP.GeraEstoque)
                    continue;

                if (!servicoEstoque.MovimentarEstoque(out erro, item.Produto, item.Quantidade, Dominio.Enumeradores.TipoMovimento.Entrada, "ENT", documentoEntrada.Numero + "-" + documentoEntrada.Serie, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCustoUnitario * documentoEntrada.ValorMoedaCotacao) : item.ValorCustoUnitario), documentoEntrada.Destinatario, documentoEntrada.DataEntrada, tipoServicoMultisoftware, null, item.LocalArmazenamento))
                    return false;
            }

            return true;
        }

        public bool CadastrarPneu(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Frota.Pneu repPneu = new Repositorio.Embarcador.Frota.Pneu(unidadeTrabalho);
            Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                //Quando não for configurado os pneu na aba ou vem pelo Documentos Destinados, continua cadastrando com dados fixos                
                Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado = item.Almoxarifado;
                Dominio.Entidades.Produto produto = item.Produto;
                if (almoxarifado == null)
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        almoxarifado = repAlmoxarifado.BuscarPrimeiroAlmoxarifado(documentoEntrada.Destinatario.Codigo);
                    else
                        almoxarifado = repAlmoxarifado.BuscarPrimeiroAlmoxarifado(0);
                }

                if (produto != null && produto.GerarPneuAutomatico != null && produto.GerarPneuAutomatico.Value && produto.Modelo != null && produto.BandaRodagem != null
                    && almoxarifado != null && !repPneu.ContemPneuCadastradoPelaNotaEntrada(item.Codigo))
                {
                    int numeroFogo = item.NumeroFogoInicial;
                    for (int i = 0; i < item.Quantidade; i++)
                    {
                        Dominio.Entidades.Embarcador.Frota.Pneu pneu = new Dominio.Entidades.Embarcador.Frota.Pneu();

                        pneu.DataEntrada = DateTime.Now;
                        pneu.DescricaoNota = item.DescricaoProdutoFornecedor;
                        pneu.DTO = item.DescricaoProdutoFornecedor;
                        pneu.KmAtualRodado = item.KMAbastecimento;
                        pneu.NumeroFogo = numeroFogo > 0 ? numeroFogo.ToString() : item.DescricaoProdutoFornecedor;
                        pneu.TipoAquisicao = item.TipoAquisicao.HasValue ? item.TipoAquisicao.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu.PneuNovoReposicao;
                        pneu.ValorAquisicao = (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorUnitario * documentoEntrada.ValorMoedaCotacao) : item.ValorUnitario);
                        pneu.ValorCustoAtualizado = (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorUnitario * documentoEntrada.ValorMoedaCotacao) : item.ValorUnitario);
                        pneu.ValorCustoKmAtualizado = 0;
                        pneu.VidaAtual = item.VidaAtual.HasValue ? item.VidaAtual.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneu.PneuNovo;
                        pneu.Almoxarifado = almoxarifado;
                        pneu.BandaRodagem = produto.BandaRodagem;
                        pneu.DocumentoEntradaItem = item;
                        pneu.Empresa = documentoEntrada.Destinatario;
                        pneu.Modelo = produto.Modelo;
                        pneu.Produto = produto;
                        pneu.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu.Disponivel;

                        repPneu.Inserir(pneu);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pneu, null, "Novo pneu inserido.", unidadeTrabalho);

                        numeroFogo += numeroFogo > 0 ? 1 : 0;
                    }
                }
            }
            return true;
        }

        public void GerarBaixarTituloDuplicata(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int countDuplicatas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.Entidades.Usuario usuario)
        {
            if (countDuplicatas == 0 && documentoEntrada.IndicadorPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.AVista
                && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe
                && documentoEntrada.Destinatario != null && documentoEntrada.Destinatario.GerarParcelaAutomaticamente
                && documentoEntrada.Destinatario.TipoPagamentoRecebimento != null && documentoEntrada.NaturezaOperacao.TipoMovimento != null && documentoEntrada.NaturezaOperacao.GeraTitulo)
            {
                Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento repTituloBaixaTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento(unidadeDeTrabalho);

                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unidadeDeTrabalho.StringConexao);
                Servicos.Embarcador.Financeiro.BaixaTituloPagar servBaixaTituloPagar = new Servicos.Embarcador.Financeiro.BaixaTituloPagar(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicataDoc = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata();
                duplicataDoc.DocumentoEntrada = documentoEntrada;
                duplicataDoc.DataVencimento = documentoEntrada.DataEmissao;
                duplicataDoc.Numero = documentoEntrada.Numero.ToString();
                duplicataDoc.Sequencia = 1;
                duplicataDoc.Valor = documentoEntrada.ValorTotal;
                duplicataDoc.Pago = true;
                duplicataDoc.DataPagamento = documentoEntrada.DataEmissao;
                duplicataDoc.Observacao = "GERADO AUTOMATICAMENTE PELO LANÇAMENTO DE DOCUMENTO DE ENTRADA Nº " + documentoEntrada.Numero.ToString();
                duplicataDoc.Forma = documentoEntrada.Fornecedor.FormaTituloFornecedor ?? documentoEntrada.Fornecedor.GrupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;

                repDuplicata.Inserir(duplicataDoc);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                titulo.DuplicataDocumentoEntrada = duplicataDoc;
                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                titulo.DataEmissao = documentoEntrada.DataEmissao;
                titulo.DataVencimento = documentoEntrada.DataEmissao;
                titulo.DataProgramacaoPagamento = documentoEntrada.DataEmissao;
                titulo.Pessoa = documentoEntrada.Fornecedor;
                titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                titulo.Sequencia = 1;
                titulo.ValorOriginal = documentoEntrada.ValorMoedaCotacao > 0 ? (Math.Round((documentoEntrada.ValorMoedaCotacao * documentoEntrada.ValorTotal), 2, MidpointRounding.AwayFromZero)) : documentoEntrada.ValorTotal;
                titulo.ValorPendente = documentoEntrada.ValorMoedaCotacao > 0 ? (Math.Round((documentoEntrada.ValorMoedaCotacao * documentoEntrada.ValorTotal), 2, MidpointRounding.AwayFromZero)) : documentoEntrada.ValorTotal;
                titulo.ValorPendente = 0;
                titulo.ValorPago = (documentoEntrada.ValorMoedaCotacao > 0 ? (documentoEntrada.ValorTotal * documentoEntrada.ValorMoedaCotacao) : documentoEntrada.ValorTotal);
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                titulo.DataAlteracao = DateTime.Now;
                titulo.Observacao = string.Concat("Referente à duplicata nº " + duplicataDoc.Numero + " do documento de entrada nº " + documentoEntrada.Numero.ToString() + ".");
                titulo.Empresa = documentoEntrada.Destinatario;
                titulo.ValorTituloOriginal = titulo.ValorOriginal;
                titulo.TipoDocumentoTituloOriginal = "Documento de Entrada";
                titulo.NumeroDocumentoTituloOriginal = documentoEntrada.Numero.ToString();
                titulo.TipoAmbiente = tipoAmbiente;
                titulo.TipoMovimento = documentoEntrada.NaturezaOperacao.TipoMovimento;
                titulo.DataLiquidacao = documentoEntrada.DataEmissao;
                titulo.DataBaseLiquidacao = documentoEntrada.DataEmissao;
                titulo.FormaTitulo = duplicataDoc.Forma;

                titulo.MoedaCotacaoBancoCentral = documentoEntrada.MoedaCotacaoBancoCentral;
                titulo.DataBaseCRT = documentoEntrada.DataBaseCRT;
                titulo.ValorMoedaCotacao = documentoEntrada.ValorMoedaCotacao;
                titulo.ValorOriginalMoedaEstrangeira = duplicataDoc.Valor;

                titulo.DataLancamento = DateTime.Now;
                titulo.Usuario = documentoEntrada.OperadorLancamentoDocumento;

                repTitulo.Inserir(titulo);

                servProcessoMovimento.GerarMovimentacao(documentoEntrada.NaturezaOperacao.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(),
                    "Título " + titulo.Sequencia.ToString() + " Documento de Entrada " + documentoEntrada.Numero.ToString(), unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada,
                    AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe, 0, null, null, titulo.Codigo, null, titulo.Pessoa);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();
                tituloBaixa.DataBaixa = documentoEntrada.DataEmissao;
                tituloBaixa.DataBase = documentoEntrada.DataEmissao;
                tituloBaixa.DataOperacao = DateTime.Now;
                tituloBaixa.Numero = 1;
                tituloBaixa.Observacao = "Gerado automaticamente pelo documento de entrada nº " + documentoEntrada.Numero.ToString();
                tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;
                tituloBaixa.Sequencia = 1;
                tituloBaixa.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (documentoEntrada.ValorTotal * documentoEntrada.ValorMoedaCotacao) : documentoEntrada.ValorTotal);
                tituloBaixa.TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                tituloBaixa.Pessoa = documentoEntrada.Fornecedor;
                tituloBaixa.Titulo = titulo;
                tituloBaixa.TipoPagamentoRecebimento = documentoEntrada.Destinatario.TipoPagamentoRecebimento;
                tituloBaixa.Usuario = usuario;

                tituloBaixa.MoedaCotacaoBancoCentral = documentoEntrada.MoedaCotacaoBancoCentral;
                tituloBaixa.DataBaseCRT = documentoEntrada.DataBaseCRT;
                tituloBaixa.ValorMoedaCotacao = documentoEntrada.ValorMoedaCotacao;
                tituloBaixa.ValorOriginalMoedaEstrangeira = duplicataDoc.Valor;

                repTituloBaixa.Inserir(tituloBaixa);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento tituloBaixaTipoPagamentoRecebimento = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento();
                tituloBaixaTipoPagamentoRecebimento.TipoPagamentoRecebimento = documentoEntrada.Destinatario.TipoPagamentoRecebimento;
                tituloBaixaTipoPagamentoRecebimento.TituloBaixa = tituloBaixa;
                tituloBaixaTipoPagamentoRecebimento.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (documentoEntrada.ValorTotal * documentoEntrada.ValorMoedaCotacao) : documentoEntrada.ValorTotal);
                repTituloBaixaTipoPagamentoRecebimento.Inserir(tituloBaixaTipoPagamentoRecebimento);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                tituloAgrupado.TituloBaixa = tituloBaixa;
                tituloAgrupado.Titulo = titulo;
                tituloAgrupado.DataBaixa = documentoEntrada.DataEmissao;
                tituloAgrupado.DataBase = documentoEntrada.DataEmissao;

                repTituloBaixaAgrupado.Inserir(tituloAgrupado);

                servBaixaTituloPagar.GeraReverteMovimentacaoFinanceira(out string erro, tituloBaixa.Codigo, unidadeDeTrabalho, stringConexao, tipoServicoMultisoftware, false, tituloBaixa.TipoPagamentoRecebimento?.PlanoConta, repMovimentoFinanceiro.BuscarContaCreditoTitulo(titulo.Codigo), titulo.Codigo);
            }
        }

        public bool GerarAbastecimentos(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, bool naoControlarKM, bool possuiPermissaoGravarValorDiferente, bool lancarDocumentoEntradaAbertoSeKMEstiverErrado)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unidadeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento repDocumentoEntradaItemAbastecimento = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento(unidadeTrabalho);
            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            if (configuracaoAbastecimento == null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                erro = "Não possui configuração para lançamento de abastecimento automático.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento> documentoEntradaItemAbastecimento = repDocumentoEntradaItemAbastecimento.BuscarPorDocumentoEntradaItem(item.Codigo);
                if (((item.Veiculo != null && item.KMAbastecimento > 0) || (item.Equipamento != null && item.Horimetro > 0)) && documentoEntradaItemAbastecimento.Count() == 0)
                {
                    if (item.Produto != null && item.Produto.ProdutoCombustivel.HasValue && item.Produto.ProdutoCombustivel.Value && item.NaturezaOperacao?.Devolucao == false)
                    {
                        Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();
                        abastecimento.Veiculo = item.Veiculo;
                        abastecimento.Kilometragem = item.KMAbastecimento;
                        abastecimento.Equipamento = item.Equipamento;
                        abastecimento.Horimetro = item.Horimetro;

                        Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                        int kilometragem = (int)abastecimento.Kilometragem;
                        int horimetro = abastecimento.Horimetro;
                        decimal litros = item.Quantidade;

                        //Preenche abastecimento de equipamento compartilhado
                        bool gerouAbastecimentoEquipamentoCompartilhado = false;
                        if (item.Equipamento != null && item.Equipamento.UtilizaTanqueCompartilhado && item.Equipamento.MediaPadrao > 0)
                        {
                            Dominio.Entidades.Abastecimento abastecimentoEquipamento = new Dominio.Entidades.Abastecimento();
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimentoEquipamento;
                            if (item.Produto.CodigoNCM.StartsWith("271121") || item.Produto.CodigoNCM.StartsWith("271019") || item.Produto.CodigoNCM.StartsWith("271012"))
                                tipoAbastecimentoEquipamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                            else
                                tipoAbastecimentoEquipamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;

                            abastecimentoEquipamento.Data = item.DataAbastecimento != null && item.DataAbastecimento.Value > DateTime.MinValue ? item.DataAbastecimento.Value : documentoEntrada.DataAbastecimento != null && documentoEntrada.DataAbastecimento.Value > DateTime.MinValue ? documentoEntrada.DataAbastecimento.Value : documentoEntrada.DataEmissao;

                            int ultimoHorimetro = repAbastecimento.BuscarUltimoHorimetroAbastecimento(item.Equipamento.Codigo, abastecimentoEquipamento.Data.Value, 0, tipoAbastecimentoEquipamento);
                            decimal litrosUsadoEquipamento = litros;
                            if (ultimoHorimetro > 0 && ultimoHorimetro < horimetro)
                            {
                                litrosUsadoEquipamento = (horimetro - ultimoHorimetro) * item.Equipamento.MediaPadrao;
                                if (litrosUsadoEquipamento <= 0)
                                    litrosUsadoEquipamento = litros;
                                else if (litrosUsadoEquipamento < litros)
                                    litros = litros - litrosUsadoEquipamento;
                            }

                            abastecimentoEquipamento.Motorista = null;
                            abastecimentoEquipamento.Posto = documentoEntrada.Fornecedor;
                            abastecimentoEquipamento.Produto = item.Produto;
                            abastecimentoEquipamento.Kilometragem = kilometragem;
                            abastecimentoEquipamento.TipoAbastecimento = tipoAbastecimentoEquipamento;
                            abastecimentoEquipamento.Litros = litrosUsadoEquipamento;
                            abastecimentoEquipamento.ValorUnitario = litrosUsadoEquipamento > 0 ? (configuracaoGeral.NaoDescontarValorDescontoItemAosAbastecimentosGeradosDocumentoEntrada ? (item.ValorTotal / litrosUsadoEquipamento) : (item.ValorTotal - item.Desconto) / litrosUsadoEquipamento) : item.ValorUnitario;
                            if (documentoEntrada.ValorMoedaCotacao > 0)
                                abastecimentoEquipamento.ValorUnitario = abastecimentoEquipamento.ValorUnitario * documentoEntrada.ValorMoedaCotacao;
                            abastecimentoEquipamento.Status = "A";
                            abastecimentoEquipamento.Situacao = "A";
                            abastecimentoEquipamento.DataAlteracao = DateTime.Now;
                            abastecimentoEquipamento.Documento = documentoEntrada.Numero.ToString();
                            abastecimentoEquipamento.Equipamento = item.Equipamento;
                            abastecimentoEquipamento.Horimetro = horimetro;
                            if (item.Veiculo != null)
                                abastecimentoEquipamento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(item.Veiculo.Codigo);

                            if (abastecimentoEquipamento.Motorista == null && abastecimentoEquipamento.Equipamento != null)
                            {
                                Dominio.Entidades.Veiculo veiculoEquipamento = repVeiculo.BuscarPorEquipamento(abastecimentoEquipamento.Equipamento.Codigo);
                                Dominio.Entidades.Usuario MotoristaEquipamento = veiculoEquipamento != null ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoEquipamento.Codigo) : null;

                                if (veiculoEquipamento != null && MotoristaEquipamento != null)
                                    abastecimentoEquipamento.Motorista = MotoristaEquipamento;
                                else if (veiculoEquipamento != null)
                                {
                                    Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(veiculoEquipamento.Codigo);
                                    if (veiculoTracao != null)
                                        abastecimentoEquipamento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                                }
                            }
                            else if (abastecimentoEquipamento.Motorista == null && item.Veiculo != null)
                            {
                                Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(item.Veiculo.Codigo);
                                if (veiculoTracao != null)
                                    abastecimentoEquipamento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                            }

                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                            {
                                abastecimentoEquipamento.Empresa = documentoEntrada.Destinatario;
                                abastecimentoEquipamento.TipoMovimento = null;
                            }
                            else
                                abastecimentoEquipamento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;

                            if (!repAbastecimento.ContemAbastecimentoPorNota(abastecimentoEquipamento.Veiculo?.Codigo ?? 0, abastecimentoEquipamento.Kilometragem, abastecimentoEquipamento.Documento, abastecimentoEquipamento.Litros, abastecimentoEquipamento.Documento))
                            {
                                if (!naoControlarKM && abastecimentoEquipamento.Equipamento != null && repAbastecimento.ContemAbastecimentoEquipamentoHorimetroMaior(abastecimentoEquipamento.Equipamento.Codigo, abastecimentoEquipamento.Horimetro, abastecimentoEquipamento.Data.Value, abastecimentoEquipamento.TipoAbastecimento))
                                {
                                    erro = "Existe um abastecimento (Item: " + item.Descricao + ") lançado para este Equipamento (" + abastecimentoEquipamento.Equipamento.Descricao + ") com Horímetro (" + abastecimentoEquipamento.Horimetro.ToString("n0") + ") superior ao informado nesta nota.";
                                    if (!lancarDocumentoEntradaAbertoSeKMEstiverErrado)
                                        return false;
                                }
                                else
                                {
                                    Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimentoEquipamento, unidadeTrabalho, abastecimentoEquipamento.Veiculo, null, configuracaoTMS);

                                    repAbastecimento.Inserir(abastecimentoEquipamento);

                                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento itemAbastecimento = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento();
                                    itemAbastecimento.Abastecimento = abastecimentoEquipamento;
                                    itemAbastecimento.DocumentoEntradaItem = item;
                                    repDocumentoEntradaItemAbastecimento.Inserir(itemAbastecimento);
                                    gerouAbastecimentoEquipamentoCompartilhado = true;
                                }
                            }
                        }

                        //Preenche abastecimento
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
                        if (item.Produto.CodigoNCM.StartsWith("271121") || item.Produto.CodigoNCM.StartsWith("271019") || item.Produto.CodigoNCM.StartsWith("271012"))
                            tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                        else
                            tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;

                        abastecimento.Motorista = null;
                        abastecimento.Posto = documentoEntrada.Fornecedor;
                        abastecimento.Produto = item.Produto;
                        abastecimento.TipoAbastecimento = tipoAbastecimento;
                        abastecimento.Litros = litros;
                        abastecimento.ValorUnitario = litros > 0 ? (item.ValorTotal - item.Desconto) / litros : item.ValorUnitario;
                        if (documentoEntrada.ValorMoedaCotacao > 0)
                            abastecimento.ValorUnitario = abastecimento.ValorUnitario * documentoEntrada.ValorMoedaCotacao;
                        abastecimento.Status = "A";
                        abastecimento.Situacao = "A";
                        abastecimento.DataAlteracao = DateTime.Now;
                        abastecimento.Data = item.DataAbastecimento != null && item.DataAbastecimento.Value > DateTime.MinValue ? item.DataAbastecimento.Value : documentoEntrada.DataAbastecimento != null && documentoEntrada.DataAbastecimento.Value > DateTime.MinValue ? documentoEntrada.DataAbastecimento.Value : documentoEntrada.DataEmissao;
                        abastecimento.Documento = documentoEntrada.Numero.ToString();
                        if (gerouAbastecimentoEquipamentoCompartilhado)
                        {
                            abastecimento.Equipamento = null;
                            abastecimento.Horimetro = 0;
                        }
                        if (item.Veiculo != null)
                            abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(item.Veiculo.Codigo);

                        if (abastecimento.Motorista == null && abastecimento.Equipamento != null)
                        {
                            Dominio.Entidades.Veiculo veiculoEquipamento = repVeiculo.BuscarPorEquipamento(abastecimento.Equipamento.Codigo);
                            Dominio.Entidades.Usuario MotoristaEquipamento = veiculoEquipamento != null ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoEquipamento.Codigo) : null;

                            if (veiculoEquipamento != null && MotoristaEquipamento != null)
                                abastecimento.Motorista = MotoristaEquipamento;
                            else if (veiculoEquipamento != null)
                            {
                                Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(veiculoEquipamento.Codigo);
                                if (veiculoTracao != null)
                                    abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                            }
                        }
                        else if (abastecimento.Motorista == null && item.Veiculo != null)
                        {
                            Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(item.Veiculo.Codigo);
                            if (veiculoTracao != null)
                                abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                        }

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        {
                            abastecimento.Empresa = documentoEntrada.Destinatario;
                            abastecimento.TipoMovimento = null;
                        }
                        else
                            abastecimento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;

                        if (!repAbastecimento.ContemAbastecimentoPorNota(abastecimento.Veiculo?.Codigo ?? 0, abastecimento.Kilometragem, abastecimento.Documento, abastecimento.Litros, abastecimento.Documento))
                        {
                            //decimal valorTabelaFornecedor = repPostoCombustivelTabelaValores.BuscarValorCombustivel(abastecimento.Produto.Codigo, abastecimento.Posto.CPF_CNPJ, abastecimento.Data.Value);
                            (decimal ValorDe, decimal ValorAte) valorTabelaFornecedor = repPostoCombustivelTabelaValores.BuscarValorCombustivelDeAte(abastecimento.Produto.Codigo, abastecimento.Posto.CPF_CNPJ, abastecimento.Data.Value);

                            bool valorTabelaFornecedorComDivergencia = false;
                            if (valorTabelaFornecedor.ValorDe > 0m && valorTabelaFornecedor.ValorAte > 0m)
                                valorTabelaFornecedorComDivergencia = ((Math.Round(valorTabelaFornecedor.ValorDe, 4) > Math.Round(abastecimento.ValorUnitario, 4)) || (Math.Round(valorTabelaFornecedor.ValorAte, 4) < Math.Round(abastecimento.ValorUnitario, 4)));
                            else
                                valorTabelaFornecedorComDivergencia = valorTabelaFornecedor.ValorDe > 0m && (Math.Round(valorTabelaFornecedor.ValorDe, 4) != Math.Round(abastecimento.ValorUnitario, 4));

                            if (!naoControlarKM && repAbastecimento.ContemAbastecimentoKMMaior(abastecimento.Veiculo?.Codigo ?? 0, abastecimento.Kilometragem, abastecimento.Data.Value, abastecimento.Equipamento?.Codigo ?? 0, abastecimento.TipoAbastecimento))
                            {
                                erro = "Existe um abastecimento lançado para este veículo com KM superior ao informado nesta nota.";
                                if (!lancarDocumentoEntradaAbertoSeKMEstiverErrado)
                                    return false;
                            }
                            else if (!naoControlarKM && abastecimento.Equipamento != null && repAbastecimento.ContemAbastecimentoEquipamentoHorimetroMaior(abastecimento.Equipamento.Codigo, abastecimento.Horimetro, abastecimento.Data.Value, abastecimento.TipoAbastecimento))
                            {
                                erro = "Existe um abastecimento (Item: " + item.Descricao + ") lançado para este Equipamento (" + abastecimento.Equipamento.Descricao + ") com Horímetro (" + abastecimento.Horimetro.ToString("n0") + ") superior ao informado nesta nota.";
                                if (!lancarDocumentoEntradaAbertoSeKMEstiverErrado)
                                    return false;
                            }
                            else if (!possuiPermissaoGravarValorDiferente && valorTabelaFornecedorComDivergencia)
                            {
                                erro = @"O preço do combustível desta Nota não está de acordo com a tabela cadastrada no fornecedor. Produto: " + item.Produto.Descricao + @" <br />
                                        Valor Tabela R$ " + valorTabelaFornecedor.ValorDe.ToString("n4") + (valorTabelaFornecedor.ValorAte > 0m ? $" até R$ {valorTabelaFornecedor.ValorAte.ToString("n4")}" : string.Empty) + " Valor Nota R$ " + abastecimento.ValorUnitario.ToString("n4") + @" <br />
                                        Favor verifique os valores ou solicite a finalização da nota ao operador com permissão.";
                                if (!lancarDocumentoEntradaAbertoSeKMEstiverErrado)
                                    return false;
                            }
                            else
                            {
                                Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unidadeTrabalho, abastecimento.Veiculo, null, configuracaoTMS);

                                if (configuracaoTMS.NaoFinalizarDocumentoEntradaComAbastecimentoInconsistente && abastecimento.Situacao == "I" && !possuiPermissaoGravarValorDiferente)
                                {
                                    if (abastecimento.Veiculo != null)
                                        erro = "O abastecimento pro veículo " + abastecimento.Veiculo.Placa_Formatada;
                                    else
                                        erro = "O abastecimento pro equipamento " + abastecimento.Equipamento.Descricao;
                                    erro += $" não foi gerado pelo seguinte motivo de inconsistência:<br />{abastecimento.MotivoInconsistencia}";
                                    if (!lancarDocumentoEntradaAbertoSeKMEstiverErrado)
                                        return false;
                                }
                                else
                                {
                                    repAbastecimento.Inserir(abastecimento);

                                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento itemAbastecimento = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento();
                                    itemAbastecimento.Abastecimento = abastecimento;
                                    itemAbastecimento.DocumentoEntradaItem = item;
                                    repDocumentoEntradaItemAbastecimento.Inserir(itemAbastecimento);
                                }
                            }

                            item.ValorAbastecimentoTabelaFornecedor = valorTabelaFornecedor.ValorDe;
                            item.ValorAbastecimentoComDivergencia = valorTabelaFornecedorComDivergencia;
                            repDocumentoEntradaItem.Atualizar(item);
                        }
                    }
                }
            }

            return true;
        }

        public bool ReverterAbastecimentos(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeTrabalho);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento repComissaoAbastecimento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento repDocumentoEntradaItemAbastecimento = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento(unidadeTrabalho);
            Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngellira repRetornoAngellira = new Repositorio.Embarcador.Frota.ConsultaAbastecimentoAngellira.RetornoConsultaAbastecimentoAngellira(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                if (item.Abastecimentos != null && item.Abastecimentos.Count > 0)
                {
                    foreach (var itemAbastecimento in item.Abastecimentos)
                    {
                        if (itemAbastecimento.Abastecimento != null)
                        {
                            Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(itemAbastecimento.Abastecimento.Codigo);

                            if (abastecimento != null)
                            {
                                if (!repAcertoAbastecimento.ContemAbastecimentoEmAcerto(abastecimento.Codigo) && !repComissaoAbastecimento.ContemAbastecimentoEmComissao(abastecimento.Codigo) && !repRetornoAngellira.ContemAbastecimentoEmComissao(abastecimento.Codigo) && !repAbastecimento.ContemAbastecimentoAgrupado(abastecimento.Codigo))
                                {
                                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento> abastecimentosItemDeletar = repDocumentoEntradaItemAbastecimento.BuscarPorAbastecimentoDocumentoEntradaItem(abastecimento.Codigo);
                                    for (int i = 0; i < abastecimentosItemDeletar.Count; i++)
                                        repDocumentoEntradaItemAbastecimento.Deletar(abastecimentosItemDeletar[i]);

                                    repAbastecimento.Deletar(abastecimento, auditado);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public bool ReverterEstoque(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                if (item.NaturezaOperacao == null || !item.NaturezaOperacao.ControlaEstoque || item.CFOP == null || !item.CFOP.GeraEstoque)
                    continue;

                if (!servicoEstoque.MovimentarEstoque(out erro, item.Produto, item.Quantidade, Dominio.Enumeradores.TipoMovimento.Saida, "ENT", documentoEntrada.Numero + "-" + documentoEntrada.Serie, (documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCustoUnitario * documentoEntrada.ValorMoedaCotacao) : item.ValorCustoUnitario), documentoEntrada.Destinatario, documentoEntrada.DataEntrada, tipoServicoMultisoftware, null, item.LocalArmazenamento))
                    return false;
            }

            return true;
        }

        public bool VerificarCadastroItens(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Entidades.Usuario usuarioLogado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento = repProdutoNCMAbastecimento.BuscarNCMsAtivos();

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem itemDoc = item;
                if (itemDoc.Produto == null)
                {
                    if (!RegistrarProduto(out erro, ref itemDoc, documentoEntrada, unidadeTrabalho, tipoServicoMultisoftware, usuarioLogado.Empresa, ncmsAbastecimento, configuracao, configuracaoDocumentoEntrada, null, true))
                        return false;
                    else
                    {
                        repDocumentoEntradaItem.Atualizar(itemDoc);
                        if (itemDoc.Produto == null && !string.IsNullOrWhiteSpace(itemDoc.CodigoProdutoFornecedor))
                        {
                            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeTrabalho);

                            Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorProdutoEFornecedor(itemDoc.CodigoProdutoFornecedor, documentoEntrada.Fornecedor.CPF_CNPJ,
                                tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? documentoEntrada.Destinatario != null ? documentoEntrada.Destinatario.Codigo : 0 : 0);

                            if (produtoFornecedor == null)
                                produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor();

                            produtoFornecedor.CodigoProduto = itemDoc.CodigoProdutoFornecedor;
                            produtoFornecedor.Fornecedor = documentoEntrada.Fornecedor;
                            produtoFornecedor.Produto = itemDoc.Produto;

                            if (produtoFornecedor.Codigo > 0)
                                repProdutoFornecedor.Atualizar(produtoFornecedor);
                            else
                            {
                                produtoFornecedor.FatorConversao = 0;
                                repProdutoFornecedor.Inserir(produtoFornecedor);
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool ValidarRegraEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro)
        {
            erro = string.Empty;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                if (item.RegraEntradaDocumento == null && documentoEntrada.Destinatario != null && documentoEntrada.Fornecedor != null)
                {
                    item.RegraEntradaDocumento = RetornaRegraEntrada(documentoEntrada.Destinatario, documentoEntrada.Fornecedor, item.NCMProdutoFornecedor, unidadeTrabalho);
                    repDocumentoEntradaItem.Atualizar(item);
                }

                if (item.RegraEntradaDocumento != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = item.RegraEntradaDocumento;
                    Dominio.Entidades.Produto produto = item.Produto;
                    string descricaoItem = !string.IsNullOrWhiteSpace(item.DescricaoProdutoFornecedor) ? item.DescricaoProdutoFornecedor : produto?.Descricao;

                    if (regraEntradaDocumento.ObrigarInformarVeiculo && item.Veiculo == null && item.Equipamento == null)
                        erro += "Item " + descricaoItem + " não possui vínculo com o veículo. ";
                    if (regraEntradaDocumento.ObrigarInformarVeiculo && item.Veiculo != null && item.KMAbastecimento <= 0 && item.Equipamento == null)
                        erro += "Item " + descricaoItem + " possui veículo porém não contem KM de abastecimento informado. ";

                    if (produto != null)
                    {
                        bool itemARLA = produto.CodigoNCM.StartsWith("310210") && produto.ProdutoCombustivel.HasValue && produto.ProdutoCombustivel.Value;

                        if (regraEntradaDocumento.NaoFinalizarQuandoArlaTiverQuantidadeSuperior && itemARLA && item.Quantidade > regraEntradaDocumento.QuantidadeSuperiorArla)
                            erro += $"Item {descricaoItem} possui a quantidade superior ao permitido ({regraEntradaDocumento.QuantidadeSuperiorArla.ToString("n4")}) para ARLA. ";
                        if (regraEntradaDocumento.NaoFinalizarQuandoArlaEstiverAssociadaReboqueEquipamento && itemARLA && (item.Veiculo?.IsTipoVeiculoReboque() ?? false || item.Equipamento != null))
                            erro += $"Item {descricaoItem} está associado a um Reboque ou Equipamento, não sendo permitido para ARLA. ";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(erro))
                return true;
            else
            {
                erro = "Erros de validação da regra de entrada: </br>" + erro;
                return false;
            }
        }

        public bool AtualizarCusto(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.HistoricoCustoProduto repHistoricoCustoProduto = new Repositorio.Embarcador.Financeiro.HistoricoCustoProduto(unidadeTrabalho);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unidadeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);

            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                if (item.NaturezaOperacao == null || !item.NaturezaOperacao.ControlaEstoque || item.NaturezaOperacao.Devolucao || item.CFOP == null || !item.CFOP.GeraEstoque)
                    continue;

                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(item.Produto.Codigo, true);
                Dominio.Entidades.Produto produtoVinculado = item.ProdutoVinculado != null ? repProduto.BuscarPorCodigo(item.ProdutoVinculado.Codigo, true) : null;
                Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoque;
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = item.LocalArmazenamento;
                if (configuracao.UtilizaMultiplosLocaisArmazenamento)
                    produtoEstoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo, documentoEntrada.Destinatario?.Codigo, localArmazenamento?.Codigo);
                else
                {
                    localArmazenamento = null;
                    produtoEstoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo, documentoEntrada.Destinatario?.Codigo ?? 0);
                    if (produtoEstoque == null)
                        produtoEstoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo);
                }

                if (produtoEstoque == null)
                    produtoEstoque = servicoEstoque.AdicionarEstoque(produto, documentoEntrada.Destinatario, tipoServicoMultisoftware, configuracao, localArmazenamento);
                else if (documentoEntrada.Destinatario != null)
                {
                    produtoEstoque.Initialize();

                    produtoEstoque.Empresa = documentoEntrada.Destinatario;
                    repProdutoEstoque.Atualizar(produtoEstoque, Auditado);
                }

                decimal custoNotaFiscal = documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCustoUnitario * documentoEntrada.ValorMoedaCotacao) : item.ValorCustoUnitario;
                if (produtoVinculado == null)
                {
                    decimal estoqueItem = produtoEstoque.Quantidade;
                    decimal quantidadeCompra = item.Quantidade;
                    decimal custoMedioAnterior = produto.CustoMedio;
                    decimal custoMedioAnteriorEmpresa = produtoEstoque.CustoMedio;
                    decimal estoqueAtual = estoqueItem + quantidadeCompra;

                    decimal novoCustoMedio = 0, novoCustoMedioEmpresa = 0;

                    if (estoqueAtual <= 0 || estoqueItem <= 0)
                    {
                        novoCustoMedio = custoNotaFiscal;
                        novoCustoMedioEmpresa = custoNotaFiscal;
                    }
                    else if (estoqueAtual != 0)
                    {
                        novoCustoMedio = (((documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCustoTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorCustoTotal) + (estoqueItem * custoMedioAnterior)) / estoqueAtual);
                        novoCustoMedioEmpresa = (((documentoEntrada.ValorMoedaCotacao > 0 ? (item.ValorCustoTotal * documentoEntrada.ValorMoedaCotacao) : item.ValorCustoTotal) + (estoqueItem * custoMedioAnteriorEmpresa)) / estoqueAtual);
                    }
                    else
                    {
                        novoCustoMedio = custoNotaFiscal;
                        novoCustoMedioEmpresa = custoNotaFiscal;
                    }

                    Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto historico = new Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto();
                    historico.CustoMedioAnteriorEmpresa = produtoEstoque.CustoMedio;
                    historico.CustoMedioAnterior = produto.CustoMedio;
                    historico.UltimoCustoAnterior = produto.UltimoCusto;
                    historico.DataAtualizacao = DateTime.Now;
                    historico.DocumentoEntrada = documentoEntrada;
                    historico.DocumentoEntradaItem = item;
                    historico.NovoCustoMedio = novoCustoMedio;
                    historico.NovoCustoMedioEmpresa = novoCustoMedioEmpresa;
                    historico.NovoUltimoCusto = custoNotaFiscal;
                    historico.Produto = produto;
                    historico.QuantidadeCompra = item.Quantidade;
                    historico.QuantidadeEstoqueAnterior = estoqueItem;

                    repHistoricoCustoProduto.Inserir(historico);

                    produto.UltimoCusto = custoNotaFiscal;
                    produto.CustoMedio = novoCustoMedio;

                    repProduto.Atualizar(produto, Auditado);

                    Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque = produtoEstoque;
                    if (estoque != null)
                    {
                        estoque.Initialize();
                        estoque.UltimoCusto = custoNotaFiscal;
                        estoque.CustoMedio = novoCustoMedioEmpresa;

                        repProdutoEstoque.Atualizar(estoque, Auditado);
                    }
                    else if (documentoEntrada.Destinatario != null)
                    {
                        estoque.Initialize();
                        estoque.Empresa = documentoEntrada.Destinatario;
                        repProdutoEstoque.Atualizar(estoque);
                    }
                }
                else //Produto Vinculado
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque produtoEstoqueVinculado;
                    if (configuracao.UtilizaMultiplosLocaisArmazenamento)
                        produtoEstoqueVinculado = repProdutoEstoque.BuscarPorProduto(produtoVinculado.Codigo, documentoEntrada.Destinatario?.Codigo, localArmazenamento?.Codigo);
                    else
                    {
                        produtoEstoqueVinculado = repProdutoEstoque.BuscarPorProduto(produtoVinculado.Codigo, documentoEntrada.Destinatario?.Codigo ?? 0);
                        if (produtoEstoqueVinculado == null)
                            produtoEstoqueVinculado = repProdutoEstoque.BuscarPorProduto(produtoVinculado.Codigo);
                    }

                    if (produtoEstoqueVinculado == null)
                        produtoEstoqueVinculado = servicoEstoque.AdicionarEstoque(produto, documentoEntrada.Destinatario, tipoServicoMultisoftware, configuracao, localArmazenamento);
                    else if (documentoEntrada.Destinatario != null)
                    {
                        produtoEstoqueVinculado.Initialize();
                        produtoEstoqueVinculado.Empresa = documentoEntrada.Destinatario;
                        repProdutoEstoque.Atualizar(produtoEstoqueVinculado, Auditado);
                    }

                    decimal estoqueItem = produtoEstoqueVinculado.Quantidade;
                    decimal custoMedioAnterior = produtoVinculado.CustoMedio;
                    decimal custoMedioAnteriorEmpresa = produtoEstoqueVinculado.CustoMedio;

                    decimal novoCustoMedio, novoCustoMedioEmpresa;
                    if (estoqueItem != 0)
                    {
                        decimal valorCustoTotalItem = item.ValorCustoTotal;
                        if (item.QuantidadeProdutoVinculado > 0)
                            valorCustoTotalItem = valorCustoTotalItem / item.QuantidadeProdutoVinculado;

                        decimal valorCustoTotal = documentoEntrada.ValorMoedaCotacao > 0 ? (valorCustoTotalItem * documentoEntrada.ValorMoedaCotacao) : valorCustoTotalItem;
                        novoCustoMedio = (valorCustoTotal + (estoqueItem * custoMedioAnterior)) / estoqueItem;
                        novoCustoMedioEmpresa = (valorCustoTotal + (estoqueItem * custoMedioAnteriorEmpresa)) / estoqueItem;
                    }
                    else
                    {
                        if (item.QuantidadeProdutoVinculado > 0)
                        {
                            novoCustoMedio = custoMedioAnterior + (custoNotaFiscal / item.QuantidadeProdutoVinculado);
                            novoCustoMedioEmpresa = custoMedioAnteriorEmpresa + (custoNotaFiscal / item.QuantidadeProdutoVinculado);
                        }
                        else
                        {
                            novoCustoMedio = custoMedioAnterior + custoNotaFiscal;
                            novoCustoMedioEmpresa = custoMedioAnteriorEmpresa + custoNotaFiscal;
                        }
                    }

                    decimal novoCusto = produtoVinculado.UltimoCusto;
                    if (item.QuantidadeProdutoVinculado > 0)
                        novoCusto += custoNotaFiscal / item.QuantidadeProdutoVinculado;
                    else
                        novoCusto += custoNotaFiscal;

                    Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto historico = new Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto();
                    historico.CustoMedioAnteriorEmpresa = produtoVinculado.CustoMedio;
                    historico.CustoMedioAnterior = produtoVinculado.CustoMedio;
                    historico.UltimoCustoAnterior = produtoVinculado.UltimoCusto;
                    historico.DataAtualizacao = DateTime.Now;
                    historico.DocumentoEntrada = documentoEntrada;
                    historico.DocumentoEntradaItem = item;
                    historico.NovoCustoMedio = novoCustoMedio;
                    historico.NovoCustoMedioEmpresa = novoCustoMedioEmpresa;
                    historico.NovoUltimoCusto = novoCusto;
                    historico.Produto = produtoVinculado;

                    repHistoricoCustoProduto.Inserir(historico);

                    produtoVinculado.UltimoCusto = novoCusto;
                    produtoVinculado.CustoMedio = novoCustoMedio;

                    repProduto.Atualizar(produtoVinculado, Auditado);

                    if (produtoEstoqueVinculado != null)
                    {
                        produtoEstoqueVinculado.Initialize();
                        produtoEstoqueVinculado.UltimoCusto = novoCusto;
                        produtoEstoqueVinculado.CustoMedio = novoCustoMedio;

                        repProdutoEstoque.Atualizar(produtoEstoqueVinculado, Auditado);
                    }
                }
            }

            return true;
        }

        public bool ReverterCusto(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out string erro, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.HistoricoCustoProduto repHistoricoCustoProduto = new Repositorio.Embarcador.Financeiro.HistoricoCustoProduto(unidadeTrabalho);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unidadeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                if (item.NaturezaOperacao == null || !item.NaturezaOperacao.ControlaEstoque)
                    continue;

                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(item.Produto.Codigo, true);
                Dominio.Entidades.Produto produtoVinculado = item.ProdutoVinculado != null ? repProduto.BuscarPorCodigo(item.ProdutoVinculado.Codigo, true) : null;
                Dominio.Entidades.Embarcador.Financeiro.HistoricoCustoProduto historico = repHistoricoCustoProduto.BuscarPorItemNota(item.Codigo);

                if (historico == null || produto == null)
                    continue;

                if (produtoVinculado == null)
                {
                    produto.UltimoCusto = historico.UltimoCustoAnterior;
                    produto.CustoMedio = historico.CustoMedioAnterior;

                    repProduto.Atualizar(produto, auditado);

                    Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque;
                    Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = item.LocalArmazenamento;
                    if (configuracao.UtilizaMultiplosLocaisArmazenamento)
                        estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo, documentoEntrada.Destinatario?.Codigo, localArmazenamento?.Codigo);
                    else
                    {
                        estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo, documentoEntrada.Destinatario?.Codigo ?? 0);
                        if (estoque == null)
                            estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo);
                    }

                    if (estoque != null)
                    {
                        estoque.UltimoCusto = historico.UltimoCustoAnterior;
                        estoque.CustoMedio = historico.CustoMedioAnteriorEmpresa;

                        repProdutoEstoque.Atualizar(estoque);
                    }
                    else if (documentoEntrada.Destinatario != null)
                    {
                        estoque.Empresa = documentoEntrada.Destinatario;
                        repProdutoEstoque.Atualizar(estoque);
                    }
                }
                else
                {
                    produtoVinculado.UltimoCusto = historico.UltimoCustoAnterior;
                    produtoVinculado.CustoMedio = historico.CustoMedioAnterior;

                    repProduto.Atualizar(produtoVinculado, auditado);
                }

                repHistoricoCustoProduto.Deletar(historico);
            }

            return true;
        }

        public static bool RegistrarProduto(out string erro, ref Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> ncmsAbastecimento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, bool naoValidarNatureza = false)
        {
            erro = string.Empty;

            if (item == null || item.Produto != null)
                return true;

            if (naoValidarNatureza)
            {
                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (item.NaturezaOperacao.ControlaEstoque)
                    {
                        erro = "É necessário vincular um produto existente para uma natureza de operação que controle estoque. Item " + item.Sequencial + ".";
                        return false;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(item.NCMProdutoFornecedor))
            {
                erro = "A NCM do item " + item.Sequencial + " (proveniente do fornecedor) é inválida, não sendo possível cadastrar o produto automaticamente.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.DescricaoProdutoFornecedor))
            {
                erro = "A descrição do item " + item.Sequencial + " (proveniente do fornecedor) é inválida, não sendo possível cadastrar o produto automaticamente.";
                return false;
            }

            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeTrabalho);

            Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorProdutoEFornecedor(item.CodigoProdutoFornecedor, documentoEntrada.Fornecedor.CPF_CNPJ,
                tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? empresa != null ? empresa.Codigo : 0 : 0);

            if (produtoFornecedor != null)
            {
                item.Produto = produtoFornecedor.Produto;
                return true;
            }

            Repositorio.NCM repNCM = new Repositorio.NCM(unidadeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeTrabalho);

            Dominio.Entidades.Produto produto = null;
            if (configuracaoDocumentoEntrada.BloquearCadastroProdutoComMesmoCodigo && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (item.LocalArmazenamento != null)
                    produto = repProduto.BuscarPorCodigoProdutoELocalArmazenamento(item.CodigoProdutoFornecedor, item.LocalArmazenamento.Codigo);
                if (produto == null)
                    produto = repProduto.BuscarPorCodigoProdutoELocalArmazenamento(item.CodigoProdutoFornecedor, 0);
                if (produto == null)
                    produto = repProduto.BuscarPorCodigoProduto(item.CodigoProdutoFornecedor);

                if (produto != null)
                {
                    item.Produto = produto;
                    return true;
                }
            }

            produto = new Dominio.Entidades.Produto();
            produto.CodigoNCM = item.NCMProdutoFornecedor;
            produto.NCM = repNCM.BuscarPorNumero(item.NCMProdutoFornecedor);
            produto.Status = "A";
            produto.UnidadeDeMedida = item.UnidadeMedida;
            produto.DescricaoNotaFiscal = item.DescricaoProdutoFornecedor;
            produto.Descricao = item.DescricaoProdutoFornecedor;
            produto.CodigoCEST = item.CESTProdutoFornecedor;
            produto.CategoriaProduto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.MercadoriaRevenda;
            produto.CalculoCustoProduto = Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unidadeTrabalho);
            produto.CodigoProduto = item.CodigoProdutoFornecedor;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && empresa != null)
                produto.Empresa = empresa;
            else
                produto.Empresa = documentoEntrada.Destinatario;
            produto.OrigemMercadoria = item.OrigemMercadoria ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;
            produto.CodigoBarrasEAN = item.CodigoBarrasEAN;

            if (ncmsAbastecimento?.Count() > 0 && !string.IsNullOrWhiteSpace(produto.CodigoNCM))
            {
                if (ncmsAbastecimento.Where(o => produto.CodigoNCM.Contains(o.NCM)).Count() > 0)
                    produto.ProdutoCombustivel = true;
                else
                    produto.ProdutoCombustivel = false;
            }
            else
                produto.ProdutoCombustivel = false;

            repProduto.Inserir(produto);

            if (auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, produto, "Adicionado automaticamente via documento de entrada", unidadeTrabalho);

            produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor();

            produtoFornecedor.CodigoProduto = item.CodigoProdutoFornecedor;
            produtoFornecedor.Fornecedor = documentoEntrada.Fornecedor;
            produtoFornecedor.Produto = produto;

            repProdutoFornecedor.Inserir(produtoFornecedor);

            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);
            servicoEstoque.AdicionarEstoque(produto, produto.Empresa, tipoServicoMultisoftware, configuracao);

            item.Produto = produto;

            return true;
        }

        public void GerarCentrosResultadoTiposDespesaAutomaticamente(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                return;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa repDocumentoEntradaCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico repDocumentoEntradaItemOrdemServico = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento repDocumentoEntradaItemAbastecimento = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            if (!configuracaoFinanceiro.AtivarControleDespesas)
                return;

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa> centrosResultadoTiposDespesa = repDocumentoEntradaCentroResultadoTipoDespesa.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);
            if (centrosResultadoTiposDespesa.Count > 0)
                return;

            List<(Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira TipoDespesa, Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado, decimal Percentual)> listaGeracao = new List<(Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira, Dominio.Entidades.Embarcador.Financeiro.CentroResultado, decimal percentual)>();

            decimal percentual = 0m;
            int count = 0;

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico> ordensServicoItens = repDocumentoEntradaItemOrdemServico.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemAbastecimento> abastecimentosItens = repDocumentoEntradaItemAbastecimento.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            if (itens.Count > 1)
            {
                if (ordensServicoItens.Count > 0)
                    throw new ServicoException("Só é possível gerar automaticamente o rateio do Centro de Resultado/Tipo de Despesa por OS quando tiver apenas um item lançado.");
                else if (abastecimentosItens.Count > 0)
                    throw new ServicoException("Só é possível gerar automaticamente o rateio do Centro de Resultado/Tipo de Despesa por Abastecimento quando tiver apenas um item lançado.");
            }

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                count++;
                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa = repTipoMovimentoTipoDespesa.BuscarTipoDespesaFinanceira(item.TipoMovimento.Codigo);
                if (tipoDespesa == null)
                    continue;

                //Rateio por OS
                int countOS = 0;
                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordensServicoItem = ordensServicoItens.Where(o => o.DocumentoEntradaItem.Codigo == item.Codigo).Select(o => o.OrdemServico).ToList();
                if (ordensServicoItem.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico in ordensServicoItem)
                    {
                        countOS++;
                        if (ordemServico.CentroResultado == null)
                            throw new ServicoException("Não foi encontrado o Centro de Resultado para a OS " + ordemServico.Numero);

                        decimal valorOrcado = ordemServico.Orcamento?.ValorTotalOrcado ?? 0m;
                        if (valorOrcado == 0)
                            throw new ServicoException("Valor orçado está zerado na OS " + ordemServico.Numero);

                        decimal calculoPercentualOS = Math.Round((valorOrcado * 100) / documentoEntrada.ValorTotal, 2);
                        if (countOS == ordensServicoItem.Count && percentual + calculoPercentualOS != 100)
                        {
                            decimal diferenca = 100 - (percentual + calculoPercentualOS);
                            calculoPercentualOS += diferenca;
                            if (calculoPercentualOS <= 0)
                                throw new ServicoException("Não foi possível aplicar a diferença de percentual para o item " + item.Produto.Descricao);
                        }
                        percentual += calculoPercentualOS;

                        listaGeracao.Add(ValueTuple.Create(tipoDespesa, ordemServico.CentroResultado, calculoPercentualOS));
                    }
                }

                if (ordensServicoItens.Count > 0)
                    continue;

                //Rateio por Abastecimento
                int countAbastecimento = 0;
                List<Dominio.Entidades.Abastecimento> abastecimentosItem = abastecimentosItens.Where(o => o.DocumentoEntradaItem.Codigo == item.Codigo).Select(o => o.Abastecimento).ToList();
                if (abastecimentosItem.Count > 0)
                {
                    foreach (Dominio.Entidades.Abastecimento abastecimento in abastecimentosItem)
                    {
                        countAbastecimento++;
                        if (abastecimento.CentroResultado == null)
                            throw new ServicoException("Não foi encontrado o Centro de Resultado para a abastecimento " + abastecimento.Descricao);

                        if (abastecimento.ValorTotal == 0)
                            throw new ServicoException("Valor do abastecimento está zerado");

                        decimal calculoPercentualAbastecimento = Math.Round((abastecimento.ValorTotal * 100) / documentoEntrada.ValorTotal, 2);
                        if (countAbastecimento == abastecimentosItem.Count && percentual + calculoPercentualAbastecimento != 100)
                        {
                            decimal diferenca = 100 - (percentual + calculoPercentualAbastecimento);
                            calculoPercentualAbastecimento += diferenca;
                            if (calculoPercentualAbastecimento <= 0)
                                throw new ServicoException("Não foi possível aplicar a diferença de percentual para o item " + item.Produto.Descricao);
                        }
                        percentual += calculoPercentualAbastecimento;

                        listaGeracao.Add(ValueTuple.Create(tipoDespesa, abastecimento.CentroResultado, calculoPercentualAbastecimento));
                    }
                }

                if (abastecimentosItens.Count > 0)
                    continue;

                //Rateio normal pelos itens
                if (item.Equipamento == null && item.Veiculo == null)
                    continue;

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = item.Equipamento?.CentroResultado ?? item.Veiculo?.CentroResultado ?? null;
                if (centroResultado == null)
                    throw new ServicoException("Não foi encontrado o Centro de Resultado para o item " + item.Produto.Descricao);

                decimal calculoPercentual = Math.Round((item.ValorCustoTotal * 100) / documentoEntrada.ValorTotal, 2);
                if (count == itens.Count && percentual + calculoPercentual != 100)
                {
                    decimal diferenca = 100 - (percentual + calculoPercentual);
                    calculoPercentual += diferenca;
                    if (calculoPercentual <= 0)
                        throw new ServicoException("Não foi possível aplicar a diferença de percentual para o item " + item.Produto.Descricao);
                }
                percentual += calculoPercentual;

                listaGeracao.Add(ValueTuple.Create(tipoDespesa, centroResultado, calculoPercentual));
            }

            List<(Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira TipoDespesa, Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado, decimal Percentual)> listaGeracaoAgrupada = listaGeracao
                .GroupBy(obj => new { obj.TipoDespesa, obj.CentroResultado })
                .Select(obj => ValueTuple.Create(obj.Key.TipoDespesa, obj.Key.CentroResultado, obj.Sum(p => p.Percentual)))
                .ToList();

            percentual = 0m;
            foreach (var item in listaGeracaoAgrupada)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa documentoEntradaCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaCentroResultadoTipoDespesa();
                documentoEntradaCentroResultadoTipoDespesa.DocumentoEntrada = documentoEntrada;
                documentoEntradaCentroResultadoTipoDespesa.TipoDespesaFinanceira = item.TipoDespesa;
                documentoEntradaCentroResultadoTipoDespesa.CentroResultado = item.CentroResultado;
                documentoEntradaCentroResultadoTipoDespesa.Percentual = item.Percentual;

                repDocumentoEntradaCentroResultadoTipoDespesa.Inserir(documentoEntradaCentroResultadoTipoDespesa);

                percentual += documentoEntradaCentroResultadoTipoDespesa.Percentual;
            }

            if (listaGeracaoAgrupada.Count > 0 && percentual != 100m)
                throw new ServicoException("O percentual rateado entre os Centros de Resultado/Tipos de Despesa difere de 100%.");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>> BuscarDocumentoEntradaPendenteIntegracao(RequestDocumentoEntradaPendente requestDocumentoEntrada)
        {
            if (requestDocumentoEntrada.QuantidadeRegistros > 100)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>>.CriarRetornoExcecao("Não é possivel retornar mais do que 100 registros");

            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repositorioDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork);
            Servicos.WebService.Financeiro.DocumentoEntrada servicoDocumentoEntrada = new WebService.Financeiro.DocumentoEntrada(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada> listaDocumento = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>();

            int totalDocumentoPendenteIntegracao = repositorioDocumentoEntrada.ContarDocumentoPendentesIntegracao();

            if (totalDocumentoPendenteIntegracao == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>>.CriarRetornoSucesso(listaDocumento);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentosPendenteIntegracao = repositorioDocumentoEntrada.BuscarDocumentosEntradaPendentesIntegracao(requestDocumentoEntrada.Inicio, requestDocumentoEntrada.QuantidadeRegistros, requestDocumentoEntrada.CodigoTipoMovimento);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento in documentosPendenteIntegracao)
                listaDocumento.Add(servicoDocumentoEntrada.ConverterObjetoDocumentoEntrada(documento, _unitOfWork));

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>>.CriarRetornoSucesso(listaDocumento);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoDocumento(List<int> protocolos)
        {
            if (protocolos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Nenhum Protocolo Informado Para Confirmar");

            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repositorioDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork);
            int documentoConfirmados = 0;

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentos = repositorioDocumentoEntrada.BuscarPorCodigos(protocolos);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento in documentos)
            {
                documento.Integrado = true;
                repositorioDocumentoEntrada.Atualizar(documento);
                documentoConfirmados++;
            }
            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, $"Integração de Documentos Confirmada Com Sucesso {documentoConfirmados}/{protocolos.Count} ");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoProvisaoDT(Dominio.ObjetosDeValor.WebService.Financeiro.RetornoProvisaoDT retornoProvisao)
        {
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioFechamentoFrete = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao provisaoIntegracao = repositorioFechamentoFrete.BuscarPorCarga(retornoProvisao.ProtocoloIntegracaoCarga);

            if (provisaoIntegracao == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Integração de provisão por carga {retornoProvisao.ProtocoloIntegracaoCarga} não encontrada ");

            if (provisaoIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado && provisaoIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgIntegracao)
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

            if (!string.IsNullOrEmpty(retornoProvisao?.MensagemRetornoCarga ?? "") && retornoProvisao.MensagemRetornoCarga.Contains("Faltam informações de Etapa para esse documento"))
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

            foreach (var stage in retornoProvisao.Stages)
            {
                var existeDocumentoComStage = provisaoIntegracao.Provisao?.DocumentosProvisao.Where(s => s.Stage.NumeroStage == stage.NumeroStage).FirstOrDefault();

                if (existeDocumentoComStage == null)
                    continue;

                existeDocumentoComStage.Stage.NumeroFolha = stage.NumeroFolha;
                existeDocumentoComStage.Stage.DataFolha = stage.DataFolha.ToNullableDateTime();
                existeDocumentoComStage.Stage.Calculo = stage.Calculo;
                existeDocumentoComStage.Stage.Atribuido = stage.Atribuido;
                existeDocumentoComStage.Stage.Transferido = stage.Transferido;
                existeDocumentoComStage.Stage.Cancelado = stage.Cancelado;
                existeDocumentoComStage.Stage.Inconsistente = stage.Inconsistente;
                existeDocumentoComStage.Stage.MensagemRetornoEtapa = stage.MensagemRetornoEtapa;

                if (stage.Cancelado)
                    existeDocumentoComStage.Situacao = SituacaoProvisaoDocumento.Cancelado;

                repositorioStage.Atualizar(existeDocumentoComStage.Stage);
            }

            bool todosDocumentosCancelados = true;

            foreach (var documentosProvisao in provisaoIntegracao.Provisao.DocumentosProvisao)
                if (!documentosProvisao.Stage.Cancelado)
                    todosDocumentosCancelados = false;

            provisaoIntegracao.Provisao.Situacao = provisaoIntegracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao
                                                                                        ? SituacaoProvisao.FalhaIntegracao : todosDocumentosCancelados
                                                                                        ? SituacaoProvisao.Cancelado : SituacaoProvisao.Finalizado;

            Servicos.Log.TratarErro($"Alterou provisaoIntegracao.ProblemaIntegracao ({provisaoIntegracao.Provisao.Codigo}) de: '{provisaoIntegracao.ProblemaIntegracao}' para: '{retornoProvisao?.MensagemRetornoCarga}' ", "RetornoProvisaoDT");

            provisaoIntegracao.ProblemaIntegracao = string.IsNullOrWhiteSpace(retornoProvisao?.MensagemRetornoCarga) ? provisaoIntegracao.ProblemaIntegracao : retornoProvisao.MensagemRetornoCarga;

            repositorioFechamentoFrete.Atualizar(provisaoIntegracao);

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoTituloFinanceiro(List<int> protocolos)
        {
            if (protocolos == null && protocolos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Precisa informar os protocolos que serar comfirmados");

            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Financeiro.Titulo> repositorioIntegracao = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Financeiro.Titulo>(_unitOfWork);
            IList<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosPendenteIntegracao = repositorioIntegracao.BuscarRegitrosPendentesIntegracaoPeloProtocolos(protocolos);
            List<int> protocolosNaoProcessado = protocolos.Where(c => !titulosPendenteIntegracao.Any(m => m.Codigo == c)).ToList();

            foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in titulosPendenteIntegracao)
            {
                titulo.IntegradoERP = true;
                repositorioIntegracao.Atualizar(titulo);
            }

            if (protocolosNaoProcessado != null && protocolosNaoProcessado.Count > 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, $"Para os Protocolo(s) {string.Join(",", protocolosNaoProcessado)} Não foram encontrados registros ou ja foram comfirmados.");

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Todos os protocolo confirmados com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>> BuscarTitulosPentendesIntegracao(int quantidade)
        {
            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Financeiro.Titulo> repositorioTitulos = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Financeiro.Titulo>(_unitOfWork);
            int totalRegistrosPendentes = repositorioTitulos.ContarRegistroPendenteIntegracao();

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo> retorno = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>(),
                NumeroTotalDeRegistro = totalRegistrosPendentes
            };

            if (totalRegistrosPendentes == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>>.CriarRetornoSucesso(retorno);

            IList<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosPendenteIntegracao = repositorioTitulos.BuscarRegitrosPendenteIntegracao(quantidade);
            Servicos.WebService.Financeiro.Titulo serWSTitulo = new Servicos.WebService.Financeiro.Titulo(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Financeiro.Titulo titulo in titulosPendenteIntegracao)
                retorno.Itens.Add(serWSTitulo.ConverterObjetoTituloAPagar(titulo, _unitOfWork));

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>>.CriarRetornoSucesso(retorno);
        }

        #endregion

        #region Métodos Privados

        private void GerarGuiasDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaGuia repGuia = new Repositorio.Embarcador.Financeiro.DocumentoEntradaGuia(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            if (itens == null || itens.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> cfopCOFINS = itens.Where(o => o.CFOP != null && o.CFOP.GerarGuiaPagarRetencaoCOFINS == true && o.ValorRetencaoCOFINS > 0).ToList();
            int sequencia = 1;
            foreach (var imposto in cfopCOFINS)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia();
                guia.DocumentoEntrada = documentoEntrada;
                guia.Fornecedor = imposto.CFOP.FornecedorRetencaoCOFINS;
                guia.Numero = documentoEntrada.Numero.ToString() + " / " + sequencia.ToString() + " - COFINS";
                guia.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (imposto.ValorRetencaoCOFINS * documentoEntrada.ValorMoedaCotacao) : imposto.ValorRetencaoCOFINS);
                guia.DataVencimento = DataVencimentoGuiaImposto((dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao).AddMonths(1), imposto.CFOP.DiaGerencaoRetencaoCOFINS);
                guia.DataPagamento = null;
                guia.Observacao = "GERADO PELA NOTA DE ENTRADA " + documentoEntrada.ToString();
                guia.TipoMovimentoGeracao = imposto.CFOP.TipoMovimentoUsoTituloRetencaoCOFINS;
                guia.TipoMovimentoReversao = imposto.CFOP.TipoMovimentoReversaoTituloRetencaoCOFINS;
                sequencia++;
                repGuia.Inserir(guia);
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> cfopPIS = itens.Where(o => o.CFOP != null && o.CFOP.GerarGuiaPagarRetencaoPIS == true && o.ValorRetencaoPIS > 0).ToList();
            sequencia = 1;
            foreach (var imposto in cfopPIS)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia();
                guia.DocumentoEntrada = documentoEntrada;
                guia.Fornecedor = imposto.CFOP.FornecedorRetencaoPIS;
                guia.Numero = documentoEntrada.Numero.ToString() + " / " + sequencia.ToString() + " - PIS";
                guia.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (imposto.ValorRetencaoPIS * documentoEntrada.ValorMoedaCotacao) : imposto.ValorRetencaoPIS);
                guia.DataVencimento = ObterDataVencimentoGuia(documentoEntrada, imposto, dataCompetencia);
                guia.DataPagamento = null;
                guia.Observacao = "GERADO PELA NOTA DE ENTRADA " + documentoEntrada.ToString();
                guia.TipoMovimentoGeracao = imposto.CFOP.TipoMovimentoUsoTituloRetencaoPIS;
                guia.TipoMovimentoReversao = imposto.CFOP.TipoMovimentoReversaoTituloRetencaoPIS;
                sequencia++;
                repGuia.Inserir(guia);
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> cfopINSS = itens.Where(o => o.CFOP != null && o.CFOP.GerarGuiaPagarRetencaoINSS == true && o.ValorRetencaoINSS > 0).ToList();
            sequencia = 1;
            foreach (var imposto in cfopINSS)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia();
                guia.DocumentoEntrada = documentoEntrada;
                guia.Fornecedor = imposto.CFOP.FornecedorRetencaoINSS;
                guia.Numero = documentoEntrada.Numero.ToString() + " / " + sequencia.ToString() + " - INSS";
                guia.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (imposto.ValorRetencaoINSS * documentoEntrada.ValorMoedaCotacao) : imposto.ValorRetencaoINSS);
                guia.DataVencimento = DataVencimentoGuiaImposto((dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao).AddMonths(1), imposto.CFOP.DiaGerencaoRetencaoINSS);
                guia.DataPagamento = null;
                guia.Observacao = "GERADO PELA NOTA DE ENTRADA " + documentoEntrada.ToString();
                guia.TipoMovimentoGeracao = imposto.CFOP.TipoMovimentoUsoTituloRetencaoINSS;
                guia.TipoMovimentoReversao = imposto.CFOP.TipoMovimentoReversaoTituloRetencaoINSS;
                sequencia++;
                repGuia.Inserir(guia);
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> cfopIPI = itens.Where(o => o.CFOP != null && o.CFOP.GerarGuiaPagarRetencaoIPI == true && o.ValorRetencaoIPI > 0).ToList();
            sequencia = 1;
            foreach (var imposto in cfopIPI)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia();
                guia.DocumentoEntrada = documentoEntrada;
                guia.Fornecedor = imposto.CFOP.FornecedorRetencaoIPI;
                guia.Numero = documentoEntrada.Numero.ToString() + " / " + sequencia.ToString() + " - IPI";
                guia.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (imposto.ValorRetencaoIPI * documentoEntrada.ValorMoedaCotacao) : imposto.ValorRetencaoIPI);
                guia.DataVencimento = DataVencimentoGuiaImposto((dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao).AddMonths(1), imposto.CFOP.DiaGerencaoRetencaoIPI);
                guia.DataPagamento = null;
                guia.Observacao = "GERADO PELA NOTA DE ENTRADA " + documentoEntrada.ToString();
                guia.TipoMovimentoGeracao = imposto.CFOP.TipoMovimentoUsoTituloRetencaoIPI;
                guia.TipoMovimentoReversao = imposto.CFOP.TipoMovimentoReversaoTituloRetencaoIPI;
                sequencia++;
                repGuia.Inserir(guia);
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> cfopCSLL = itens.Where(o => o.CFOP != null && o.CFOP.GerarGuiaPagarRetencaoCSLL == true && o.ValorRetencaoCSLL > 0).ToList();
            sequencia = 1;
            foreach (var imposto in cfopCSLL)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia();
                guia.DocumentoEntrada = documentoEntrada;
                guia.Fornecedor = imposto.CFOP.FornecedorRetencaoCSLL;
                guia.Numero = documentoEntrada.Numero.ToString() + " / " + sequencia.ToString() + " - CSLL";
                guia.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (imposto.ValorRetencaoCSLL * documentoEntrada.ValorMoedaCotacao) : imposto.ValorRetencaoCSLL);
                guia.DataVencimento = DataVencimentoGuiaImposto((dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao).AddMonths(1), imposto.CFOP.DiaGerencaoRetencaoCSLL);
                guia.DataPagamento = null;
                guia.Observacao = "GERADO PELA NOTA DE ENTRADA " + documentoEntrada.ToString();
                guia.TipoMovimentoGeracao = imposto.CFOP.TipoMovimentoUsoTituloRetencaoCSLL;
                guia.TipoMovimentoReversao = imposto.CFOP.TipoMovimentoReversaoTituloRetencaoCSLL;
                sequencia++;
                repGuia.Inserir(guia);
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> cfopOutras = itens.Where(o => o.CFOP != null && o.CFOP.GerarGuiaPagarRetencaoOutras == true && o.ValorRetencaoOutras > 0).ToList();
            sequencia = 1;
            foreach (var imposto in cfopOutras)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia();
                guia.DocumentoEntrada = documentoEntrada;
                guia.Fornecedor = imposto.CFOP.FornecedorRetencaoOutras;
                guia.Numero = documentoEntrada.Numero.ToString() + " / " + sequencia.ToString() + " - Outras";
                guia.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (imposto.ValorRetencaoOutras * documentoEntrada.ValorMoedaCotacao) : imposto.ValorRetencaoOutras);
                guia.DataVencimento = DataVencimentoGuiaImposto((dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao).AddMonths(1), imposto.CFOP.DiaGerencaoRetencaoOutras);
                guia.DataPagamento = null;
                guia.Observacao = "GERADO PELA NOTA DE ENTRADA " + documentoEntrada.ToString();
                guia.TipoMovimentoGeracao = imposto.CFOP.TipoMovimentoUsoTituloRetencaoOutras;
                guia.TipoMovimentoReversao = imposto.CFOP.TipoMovimentoReversaoTituloRetencaoOutras;
                sequencia++;
                repGuia.Inserir(guia);
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> cfopISS = itens.Where(o => o.CFOP != null && o.CFOP.GerarGuiaPagarRetencaoISS == true && o.ValorRetencaoISS > 0).ToList();
            sequencia = 1;
            foreach (var imposto in cfopISS)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia();
                guia.DocumentoEntrada = documentoEntrada;
                guia.Fornecedor = imposto.CFOP.FornecedorRetencaoISS;
                guia.Numero = documentoEntrada.Numero.ToString() + " / " + sequencia.ToString() + " - ISS";
                guia.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (imposto.ValorRetencaoISS * documentoEntrada.ValorMoedaCotacao) : imposto.ValorRetencaoISS);
                guia.DataVencimento = DataVencimentoGuiaImposto((dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao).AddMonths(1), imposto.CFOP.DiaGerencaoRetencaoISS);
                guia.DataPagamento = null;
                guia.Observacao = "GERADO PELA NOTA DE ENTRADA " + documentoEntrada.ToString();
                guia.TipoMovimentoGeracao = imposto.CFOP.TipoMovimentoUsoTituloRetencaoISS;
                guia.TipoMovimentoReversao = imposto.CFOP.TipoMovimentoReversaoTituloRetencaoISS;
                sequencia++;
                repGuia.Inserir(guia);
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> cfopIR = itens.Where(o => o.CFOP != null && o.CFOP.GerarGuiaPagarRetencaoIR == true && o.ValorRetencaoIR > 0).ToList();
            sequencia = 1;
            foreach (var imposto in cfopIR)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia guia = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia();
                guia.DocumentoEntrada = documentoEntrada;
                guia.Fornecedor = imposto.CFOP.FornecedorRetencaoIR;
                guia.Numero = documentoEntrada.Numero.ToString() + " / " + sequencia.ToString() + " - IR";
                guia.Valor = (documentoEntrada.ValorMoedaCotacao > 0 ? (imposto.ValorRetencaoIR * documentoEntrada.ValorMoedaCotacao) : imposto.ValorRetencaoIR);
                guia.DataVencimento = DataVencimentoGuiaImposto((dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao).AddMonths(1), imposto.CFOP.DiaGerencaoRetencaoIR);
                guia.DataPagamento = null;
                guia.Observacao = "GERADO PELA NOTA DE ENTRADA " + documentoEntrada.ToString();
                guia.TipoMovimentoGeracao = imposto.CFOP.TipoMovimentoUsoTituloRetencaoIR;
                guia.TipoMovimentoReversao = imposto.CFOP.TipoMovimentoReversaoTituloRetencaoIR;
                sequencia++;
                repGuia.Inserir(guia);
            }
        }

        private DateTime DataVencimentoGuiaImposto(DateTime data, int dia)
        {
            if (dia <= 0)
                dia = 1;
            DateTime dataRetorno;
            string strDataRetorno = dia.ToString() + "/" + data.ToString("MM/yyyy");
            if (!DateTime.TryParse(strDataRetorno, out dataRetorno))
            {
                strDataRetorno = "01/" + data.ToString("MM/yyyy");
                DateTime.TryParse(strDataRetorno, out dataRetorno);
            }
            return dataRetorno;
        }

        private void RetornaTagAbastecimento(out string kmObservacao, out string placaObservacao, out string horimetroObservacao, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, Dominio.Entidades.Cliente emitente)
        {
            kmObservacao = string.Empty;
            placaObservacao = string.Empty;
            horimetroObservacao = string.Empty;
            try
            {
                string observacaoNF = nfe.NFe.infNFe.infAdic != null ? !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) ? nfe.NFe.infNFe.infAdic.infCpl : string.Empty : string.Empty;
                observacaoNF += nfe.NFe.infNFe.infAdic != null ? !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infAdFisco) ? nfe.NFe.infNFe.infAdic.infAdFisco : string.Empty : string.Empty;
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = emitente.GrupoPessoas;

                if (emitente != null && emitente.LerVeiculoObservacaoNotaParaAbastecimento.HasValue && emitente.LerVeiculoObservacaoNotaParaAbastecimento.Value)
                {
                    if (!string.IsNullOrWhiteSpace(emitente.LerPlacaObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerPlacaObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerPlacaObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string placa = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                                    placaObservacao = placa.ToUpper();
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emitente.LerKMObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerKMObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerKMObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string km = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(km))
                                    kmObservacao = km;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emitente.LerHorimetroObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerHorimetroObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerHorimetroObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string horimetro = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(horimetro))
                                    horimetroObservacao = horimetro;
                            }
                        }
                    }
                }
                else if (grupoPessoas != null && grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento.HasValue && grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento.Value)
                {
                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string placa = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                                    placaObservacao = placa.ToUpper();
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerKMObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string km = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(km))
                                    kmObservacao = km;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string horimetro = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(horimetro))
                                    horimetroObservacao = horimetro;
                            }
                        }
                    }
                }

                if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";PLACA: ") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";KM: "))
                {
                    placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";PLACA: ") + 8, 8);
                    placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "").Replace(";", "");
                    kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";KM: ") + 5, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";KM: ") + 5));
                    if (kmObservacao.IndexOf(";") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(";"));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else if (kmObservacao.IndexOf(" ") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else
                        kmObservacao = "";
                }
                else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";PLACA:") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";KM:"))
                {
                    placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";PLACA:") + 7, 8);
                    placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                    kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";KM:") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";KM:") + 4));
                    if (kmObservacao.IndexOf(";") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(";"));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else if (kmObservacao.IndexOf(" ") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else
                        kmObservacao = "";
                }
                else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(" - PLACA:") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(" - ODOMETRO:"))
                {
                    placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(" - PLACA:") + 9, 10);
                    placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                    kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("- ODOMETRO:") + 11, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("- ODOMETRO:") + 11));
                    if (kmObservacao.IndexOf("-") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("-"));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else if (kmObservacao.IndexOf(" ") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else
                        kmObservacao = "";
                }
                else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("KM: ") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("PLACA: "))
                {
                    placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("PLACA: ") + 7, 8);
                    placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                    kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM: ") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM: ") + 4));
                    if (kmObservacao.IndexOf("-") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("-"));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else if (kmObservacao.IndexOf(".") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("."));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else if (kmObservacao.IndexOf(" ") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else
                        kmObservacao = "";
                }
                else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("KM:-") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("PLACA:"))
                {
                    placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("PLACA:") + 6, 8);
                    placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                    kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM:-") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM:-") + 4));
                    if (kmObservacao.IndexOf(" ") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else
                        kmObservacao = "";
                }
                else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("KMF: ") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("PLACA:"))
                {
                    placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("PLACA:") + 6, 9);
                    placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                    kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KMF: ") + 5, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KMF: ") + 5));
                    if (kmObservacao.IndexOf(" ") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else
                        kmObservacao = "";
                }
                else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("KM:-") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("PCA:"))
                {
                    placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("PCA:") + 4, 7);
                    placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                    kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM:-") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM:-") + 4));
                    if (kmObservacao.IndexOf(" ") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else
                        kmObservacao = "";
                }
                else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && string.IsNullOrWhiteSpace(placaObservacao) && string.IsNullOrWhiteSpace(kmObservacao) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("/KM"))
                {
                    placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/KM") - 8, 8);
                    placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "").Replace("/", "");
                    kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/KM") + 3, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/KM") + 3));
                    if (kmObservacao.IndexOf(",") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(","));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else if (kmObservacao.IndexOf(" ") > 0)
                    {
                        kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                        kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                    }
                    else
                        kmObservacao = "";
                }
                if (!string.IsNullOrWhiteSpace(kmObservacao))
                    kmObservacao = kmObservacao.TrimStart('0');
            }
            catch (Exception)
            {
                kmObservacao = string.Empty;
                placaObservacao = string.Empty;
            }
        }

        public void RetornaTagAbastecimento(out string kmObservacao, out string placaObservacao, out string horimetroObservacao, out string chassiObservacao, MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, Dominio.Entidades.Cliente emitente)
        {
            kmObservacao = string.Empty;
            placaObservacao = string.Empty;
            horimetroObservacao = string.Empty;
            chassiObservacao = string.Empty;

            try
            {
                string observacaoNF = nfe.NFe.infNFe.infAdic != null ? !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) ? nfe.NFe.infNFe.infAdic.infCpl : string.Empty : string.Empty;
                observacaoNF += " " + (nfe.NFe.infNFe.infAdic != null ? !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infAdFisco) ? nfe.NFe.infNFe.infAdic.infAdFisco : string.Empty : string.Empty);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = emitente.GrupoPessoas;

                if (emitente != null && emitente.LerVeiculoObservacaoNotaParaAbastecimento.HasValue && emitente.LerVeiculoObservacaoNotaParaAbastecimento.Value)
                {
                    if (!string.IsNullOrWhiteSpace(emitente.LerPlacaObservacaoNotaParaAbastecimentoInicial) || !string.IsNullOrWhiteSpace(emitente.LerPlacaObservacaoNotaParaAbastecimentoFinal))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerPlacaObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerPlacaObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string placa = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                                    placaObservacao = placa.ToUpper();
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emitente.LerKMObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerKMObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerKMObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string km = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(km))
                                    kmObservacao = km;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emitente.LerHorimetroObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerHorimetroObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerHorimetroObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string horimetro = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(horimetro))
                                    horimetroObservacao = horimetro;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emitente.LerChassiObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerChassiObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerChassiObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string chassi = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(chassi))
                                    chassiObservacao = chassi;
                            }
                        }
                    }
                }
                else if (grupoPessoas != null && grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento.HasValue && grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento.Value)
                {
                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string placa = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                                    placaObservacao = placa.ToUpper();
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerKMObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string km = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(km))
                                    kmObservacao = km;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string horimetro = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(horimetro))
                                    horimetroObservacao = horimetro;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string chassi = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(chassi))
                                    chassiObservacao = chassi;
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(placaObservacao) && string.IsNullOrWhiteSpace(chassiObservacao) && string.IsNullOrWhiteSpace(kmObservacao))
                {
                    if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";PLACA: ") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";KM: "))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";PLACA: ") + 8, 8);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "").Replace(";", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";KM: ") + 5, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";KM: ") + 5));
                        if (kmObservacao.IndexOf(";") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(";"));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";PLACA:") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";KM:"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";PLACA:") + 7, 8);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";KM:") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";KM:") + 4));
                        if (kmObservacao.IndexOf(";") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(";"));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";PLACA:") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(" KM:"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";PLACA:") + 7, 8);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "").Replace(";", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(" KM:") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(" KM:") + 4));

                        if (!string.IsNullOrWhiteSpace(kmObservacao))
                        {
                            kmObservacao = kmObservacao.Trim();
                            if (kmObservacao.Length > 13)
                                kmObservacao = kmObservacao.Substring(0, 13);
                            else if (kmObservacao.Length > 10)
                                kmObservacao = kmObservacao.Substring(0, 10);
                        }

                        if (kmObservacao.IndexOf(";") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(";"));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(";PLACA:") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(" KM.:"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(";PLACA:") + 7, 8);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "").Replace(";", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(" KM.:") + 5, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(" KM.:") + 5));

                        if (!string.IsNullOrWhiteSpace(kmObservacao))
                        {
                            kmObservacao = kmObservacao.Trim();
                            if (kmObservacao.Length > 13)
                                kmObservacao = kmObservacao.Substring(0, 13);
                            else if (kmObservacao.Length > 10)
                                kmObservacao = kmObservacao.Substring(0, 10);
                        }

                        if (kmObservacao.IndexOf(";") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(";"));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(" - PLACA:") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(" - ODOMETRO:"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(" - PLACA:") + 9, 10);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("- ODOMETRO:") + 11, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("- ODOMETRO:") + 11));
                        if (kmObservacao.IndexOf("-") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("-"));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(" - PLACA ") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains(" - KM "))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf(" - PLACA ") + 9, 10);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("- KM ") + 5, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("- KM ") + 5));

                        if (!string.IsNullOrWhiteSpace(kmObservacao))
                        {
                            kmObservacao = kmObservacao.Trim();
                            if (kmObservacao.Length > 13)
                                kmObservacao = kmObservacao.Substring(0, 13);
                            else if (kmObservacao.Length > 10)
                                kmObservacao = kmObservacao.Substring(0, 10);
                        }

                        if (kmObservacao.IndexOf("-") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("-"));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("KM: ") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("PLACA: "))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("PLACA: ") + 7, 8);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "").Replace(";", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM: ") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM: ") + 4));
                        if (!string.IsNullOrWhiteSpace(kmObservacao))
                        {
                            kmObservacao = kmObservacao.Trim();
                            if (kmObservacao.Length > 13)
                                kmObservacao = kmObservacao.Substring(0, 13);
                            else if (kmObservacao.Length > 10)
                                kmObservacao = kmObservacao.Substring(0, 10);
                        }


                        if (kmObservacao.IndexOf("-") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("-"));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(".") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("."));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (!string.IsNullOrEmpty(kmObservacao))
                        {
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("/ KM:") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("/ PLACA:"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/ PLACA:") + 8, 8);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/ KM:") + 5, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/ KM:") + 5));
                        if (!string.IsNullOrWhiteSpace(kmObservacao))
                        {
                            kmObservacao = kmObservacao.Trim();
                            if (kmObservacao.Length > 13)
                                kmObservacao = kmObservacao.Substring(0, 13);
                            else if (kmObservacao.Length > 10)
                                kmObservacao = kmObservacao.Substring(0, 10);
                        }


                        if (kmObservacao.IndexOf("-") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("-"));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(".") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf("."));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("KM:-") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("PLACA:"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("PLACA:") + 6, 8);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM:-") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM:-") + 4));
                        if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("KMF: ") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("PLACA:"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("PLACA:") + 6, 9);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KMF: ") + 5, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KMF: ") + 5));
                        if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("KM:-") && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("PCA:"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("PCA:") + 4, 7);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM:-") + 4, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("KM:-") + 4));
                        if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                    else if (nfe.NFe.infNFe.infAdic != null && !string.IsNullOrWhiteSpace(nfe.NFe.infNFe.infAdic.infCpl) && string.IsNullOrWhiteSpace(placaObservacao) && string.IsNullOrWhiteSpace(kmObservacao) && nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Contains("/KM"))
                    {
                        placaObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/KM") - 8, 8);
                        placaObservacao = placaObservacao.Replace("-", "").Replace(" ", "").Replace("/", "");
                        kmObservacao = nfe.NFe.infNFe.infAdic.infCpl.ToUpper().Substring(nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/KM") + 3, nfe.NFe.infNFe.infAdic.infCpl.Length - (nfe.NFe.infNFe.infAdic.infCpl.ToUpper().IndexOf("/KM") + 3));
                        if (kmObservacao.IndexOf(",") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(","));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else if (kmObservacao.IndexOf(" ") > 0)
                        {
                            kmObservacao = kmObservacao.Substring(0, kmObservacao.IndexOf(" "));
                            kmObservacao = Utilidades.String.OnlyNumbers(kmObservacao);
                        }
                        else
                            kmObservacao = "";
                    }
                }

                if (!string.IsNullOrWhiteSpace(kmObservacao))
                    kmObservacao = kmObservacao.TrimStart('0');
                if (!string.IsNullOrWhiteSpace(horimetroObservacao))
                    horimetroObservacao = horimetroObservacao.TrimStart('0');
            }
            catch (Exception)
            {
                kmObservacao = string.Empty;
                placaObservacao = string.Empty;
                horimetroObservacao = string.Empty;
                chassiObservacao = string.Empty;
            }
        }

        public void RetornaTagAbastecimento(out string kmObservacao, out string placaObservacao, out string horimetroObservacao, out string chassiObservacao, string observacaoNF, Dominio.Entidades.Cliente emitente)
        {
            kmObservacao = string.Empty;
            placaObservacao = string.Empty;
            horimetroObservacao = string.Empty;
            chassiObservacao = string.Empty;

            if (string.IsNullOrWhiteSpace(observacaoNF))
                return;

            try
            {

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = emitente.GrupoPessoas;

                if (emitente != null && emitente.LerVeiculoObservacaoNotaParaAbastecimento.HasValue && emitente.LerVeiculoObservacaoNotaParaAbastecimento.Value)
                {
                    if (!string.IsNullOrWhiteSpace(emitente.LerPlacaObservacaoNotaParaAbastecimentoInicial) || !string.IsNullOrWhiteSpace(emitente.LerPlacaObservacaoNotaParaAbastecimentoFinal))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerPlacaObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerPlacaObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string placa = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                                    placaObservacao = placa.ToUpper();
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emitente.LerKMObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerKMObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerKMObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string km = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(km))
                                    kmObservacao = km;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emitente.LerHorimetroObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerHorimetroObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerHorimetroObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string horimetro = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(horimetro))
                                    horimetroObservacao = horimetro;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emitente.LerChassiObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = emitente.LerChassiObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = emitente.LerChassiObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string chassi = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(chassi))
                                    chassiObservacao = chassi;
                            }
                        }
                    }
                }
                else if (grupoPessoas != null && grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento.HasValue && grupoPessoas.LerVeiculoObservacaoNotaParaAbastecimento.Value)
                {
                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerPlacaObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string placa = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 7)
                                    placaObservacao = placa.ToUpper();
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerKMObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerKMObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string km = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(km))
                                    kmObservacao = km;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerHorimetroObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string horimetro = idxFim > 0 ? Utilidades.String.OnlyNumbers(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(horimetro))
                                    horimetroObservacao = horimetro;
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoInicial))
                    {
                        string observacao = observacaoNF.ToLower();

                        if (!string.IsNullOrWhiteSpace(observacao) && observacao.Length > 0)
                        {
                            string strInicio = grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoInicial.ToLower();
                            string strFim = grupoPessoas.LerChassiObservacaoNotaParaAbastecimentoFinal.ToLower();

                            int idxInicio = observacao.IndexOf(strInicio);

                            if (idxInicio > -1)
                            {
                                idxInicio += strInicio.Length;

                                int idxFim = string.IsNullOrEmpty(strFim) ? observacao.Length : observacao.IndexOf(strFim, idxInicio);

                                string chassi = idxFim > 0 ? Utilidades.String.OnlyNumbersAndChars(observacao.Substring(idxInicio, idxFim - idxInicio)) : string.Empty;

                                if (!string.IsNullOrWhiteSpace(chassi))
                                    chassiObservacao = chassi;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(kmObservacao))
                    kmObservacao = kmObservacao.TrimStart('0');
                if (!string.IsNullOrWhiteSpace(horimetroObservacao))
                    horimetroObservacao = horimetroObservacao.TrimStart('0');
            }
            catch (Exception)
            {
                kmObservacao = string.Empty;
                placaObservacao = string.Empty;
                horimetroObservacao = string.Empty;
                chassiObservacao = string.Empty;
            }
        }

        private object ObterDetalhesPorNFe(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, string documentoImportadoXML)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("55");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("nfe");

            Servicos.NFe svcNFe = new Servicos.NFe(unidadeDeTrabalho);

            Dominio.Entidades.Empresa destinatario = repEmpresa.BuscarPorCNPJ(nfe.NFe.infNFe.dest.Item);
            if (empresa == null && destinatario != null)
                empresa = repEmpresa.BuscarPorCNPJ(destinatario.CNPJ_SemFormato);

            Dominio.Entidades.Cliente emitente = svcNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa?.Codigo ?? 0, unidadeDeTrabalho);
            Dominio.Entidades.Veiculo veiculoObs = null;

            if (destinatario?.CNPJ != empresa.CNPJ && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                throw new ServicoException("Destinatário do XML não é a sua empresa!");

            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = null;
            if (destinatario != null && emitente != null)
                regraEntradaDocumento = RetornaRegraEntrada(destinatario, emitente, "", unidadeDeTrabalho);
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;
            if (regraEntradaDocumento != null && destinatario != null && emitente != null)
            {
                if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumento.CFOPDentro != null)
                    cfopNota = regraEntradaDocumento.CFOPDentro;
                else if (regraEntradaDocumento.CFOPFora != null)
                    cfopNota = regraEntradaDocumento.CFOPFora;
                if (regraEntradaDocumento.NaturezaOperacao != null)
                    naturezaDaOperacaoNota = regraEntradaDocumento.NaturezaOperacao;
                if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                    tipoMovimentoNota = cfopNota.TipoMovimentoUso;
            }

            DateTime dataEntradaNota = DateTime.MinValue;
            DateTime.TryParseExact(nfe.NFe.infNFe.ide.dhEmi.Split('T')[0], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataEntradaNota);
            if (dataEntradaNota <= DateTime.MinValue || dataEntrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada.DataLancamento)
                dataEntradaNota = DateTime.Now;

            string placaObservacao = "";
            string kmObservacao = "";
            string horimetroObservacao = "";
            RetornaTagAbastecimento(out kmObservacao, out placaObservacao, out horimetroObservacao, nfe, emitente);
            if (!string.IsNullOrWhiteSpace(placaObservacao))
                veiculoObs = repVeiculo.BuscarPlaca(placaObservacao);

            decimal totalBaseCalculoICMS = 0;
            decimal valorTotalCOFINS = 0;
            decimal valorTotalICMS = 0;
            decimal valorTotalIPI = 0;
            decimal valorTotalPIS = 0;
            decimal valorTotalCreditoPresumido = 0;
            decimal valorTotalDiferencial = 0;
            decimal valorTotalCusto = 0;
            decimal totalBaseSTRetido = 0;
            decimal totalValorSTRetido = 0;
            var ItensNFe = ObterItens(nfe.NFe.infNFe.det, empresa, emitente, cultura, unidadeDeTrabalho, veiculoObs, kmObservacao, horimetroObservacao, regraEntradaDocumento, destinatario, out totalBaseCalculoICMS, out valorTotalCOFINS, out valorTotalICMS, out valorTotalIPI, out valorTotalPIS, out valorTotalCreditoPresumido, out valorTotalDiferencial, out valorTotalCusto, tipoServicoMultisoftware, out totalBaseSTRetido, out totalValorSTRetido);

            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = null;
            if (veiculoObs != null)// && !string.IsNullOrWhiteSpace(kmObservacao))
            {
                if (veiculoObs.Equipamentos != null && veiculoObs.Equipamentos.Count > 0)
                {
                    if (veiculoObs.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true)?.Count() == 1)
                        equipamento = veiculoObs.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true).FirstOrDefault();
                }
            }

            var retorno = new
            {
                BaseSTRetido = totalBaseSTRetido.ToString("n2"),
                ValorSTRetido = totalValorSTRetido.ToString("n2"),
                BaseCalculoICMS = totalBaseCalculoICMS.ToString("n2"),
                BaseCalculoICMSST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBCST, cultura).ToString("n2"),
                Chave = nfe.protNFe != null ? nfe.protNFe.infProt.chNFe : string.Empty,
                Fornecedor = new
                {
                    Codigo = emitente.CPF_CNPJ,
                    Descricao = emitente.Nome
                },
                Destinatario = new
                {
                    Codigo = destinatario?.Codigo ?? 0,
                    Descricao = destinatario?.RazaoSocial ?? string.Empty
                },
                CFOP = new
                {
                    Codigo = cfopNota != null ? cfopNota.Codigo : 0,
                    Descricao = cfopNota != null ? cfopNota.CFOPComExtensao : string.Empty
                },
                NaturezaOperacao = new
                {
                    Codigo = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Codigo : 0,
                    Descricao = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Descricao : string.Empty
                },
                TipoMovimento = new
                {
                    Codigo = tipoMovimentoNota != null ? tipoMovimentoNota.Codigo : 0,
                    Descricao = tipoMovimentoNota != null ? tipoMovimentoNota.Descricao : string.Empty
                },
                Veiculo = new
                {
                    Codigo = veiculoObs != null ? veiculoObs.Codigo : 0,
                    Descricao = veiculoObs != null ? veiculoObs.Placa : string.Empty
                },
                Equipamento = new
                {
                    Codigo = equipamento != null ? equipamento.Codigo : 0,
                    Descricao = equipamento != null ? equipamento.Descricao : string.Empty
                },
                KMAbastecimento = (equipamento == null || (equipamento?.UtilizaTanqueCompartilhado ?? false)) ? kmObservacao : "",
                Horimetro = veiculoObs != null && veiculoObs.TipoVeiculo == "0" ? string.Empty : !string.IsNullOrWhiteSpace(horimetroObservacao) ? horimetroObservacao : equipamento != null ? kmObservacao : string.Empty,
                DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null).ToString("dd/MM/yyyy HH:mm"),
                DataEntrada = dataEntradaNota.ToString("dd/MM/yyyy"),
                IndicadorPagamento = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada)nfe.NFe.infNFe.ide.indPag,
                Numero = int.Parse(nfe.NFe.infNFe.ide.nNF),
                Serie = nfe.NFe.infNFe.ide.serie,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto,
                ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n2"),
                ValorBruto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n2"),
                ValorProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura).ToString("n2"),
                ValorTotalCOFINS = valorTotalCOFINS.ToString("n2"),
                ValorTotalDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura).ToString("n2"),
                ValorTotalSeguro = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vSeg, cultura).ToString("n2"),
                ValorTotalFrete = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vFrete, cultura).ToString("n2"),
                ValorTotalICMS = valorTotalICMS.ToString("n2"),
                ValorTotalICMSST = "0,00",
                ValorTotalIPI = valorTotalIPI.ToString("n2"),
                ValorTotalOutrasDespesas = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura).ToString("n2"),
                ValorTotalPIS = valorTotalPIS.ToString("n2"),
                ValorTotalCreditoPresumido = valorTotalCreditoPresumido.ToString("n2"),
                ValorTotalDiferencial = valorTotalDiferencial.ToString("n2"),
                ValorTotalFreteFora = "0,00",
                ValorTotalOutrasDespesasFora = "0,00",
                ValorTotalDescontoFora = "0,00",
                ValorTotalImpostosFora = "0,00",
                ValorTotalDiferencialFreteFora = "0,00",
                ValorTotalICMSFreteFora = "0,00",
                ValorTotalRetencaoPIS = "0,00",
                ValorTotalRetencaoCOFINS = "0,00",
                ValorTotalRetencaoIPI = "0,00",
                ValorTotalRetencaoINSS = "0,00",
                ValorTotalRetencaoCSLL = "0,00",
                ValorTotalRetencaoISS = "0,00",
                ValorTotalRetencaoIR = "0,00",
                ValorTotalRetencaoOutras = "0,00",
                ValorTotalCusto = valorTotalCusto.ToString("n2"),
                Especie = new
                {
                    especie.Codigo,
                    especie.Descricao
                },
                Modelo = new
                {
                    modelo.Codigo,
                    modelo.Descricao
                },
                Observacao = string.Empty,
                Duplicatas = ObterDuplicatas(nfe, cultura, emitente, DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi.Split('T')[0], "yyyy-MM-dd", null), decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura), unidadeDeTrabalho),
                Itens = ItensNFe,
                DocumentoImportadoXML = documentoImportadoXML ?? string.Empty,
            };

            return retorno;
        }

        private object ObterDetalhesPorNFe(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, string documentoImportadoXML)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("55");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("nfe");

            Servicos.NFe svcNFe = new Servicos.NFe(unidadeDeTrabalho);

            Dominio.Entidades.Empresa destinatario = repEmpresa.BuscarPorCNPJ(nfe.NFe.infNFe.dest.Item);
            if (empresa == null && destinatario != null)
                empresa = repEmpresa.BuscarPorCNPJ(destinatario.CNPJ_SemFormato);

            Dominio.Entidades.Cliente emitente = svcNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa?.Codigo ?? 0, unidadeDeTrabalho, tipoServicoMultisoftware);
            Dominio.Entidades.Veiculo veiculoObs = null;

            if (destinatario?.CNPJ != empresa.CNPJ && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                throw new ServicoException("Destinatário do XML não é a sua empresa!");

            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = null;
            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoDoItem = null;
            if (destinatario != null && emitente != null)
                regraEntradaDocumento = RetornaRegraEntrada(destinatario, emitente, "", unidadeDeTrabalho);

            DateTime dataEntradaNota = DateTime.MinValue;
            DateTime.TryParseExact(nfe.NFe.infNFe.ide.dhEmi.Split('T')[0], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataEntradaNota);
            if (dataEntradaNota <= DateTime.MinValue || dataEntrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada.DataLancamento)
                dataEntradaNota = DateTime.Now;

            string placaObservacao = "";
            string kmObservacao = "";
            string horimetroObservacao = "";
            string chassiObservacao = "";
            RetornaTagAbastecimento(out kmObservacao, out placaObservacao, out horimetroObservacao, out chassiObservacao, nfe, emitente);
            if (!string.IsNullOrWhiteSpace(placaObservacao))
                veiculoObs = repVeiculo.BuscarPlaca(placaObservacao);
            else if (!string.IsNullOrWhiteSpace(chassiObservacao))
                veiculoObs = repVeiculo.BuscarPorChassi(chassiObservacao);

            decimal totalBaseCalculoICMS = 0;
            decimal valorTotalCOFINS = 0;
            decimal valorTotalICMS = 0;
            decimal valorTotalIPI = 0;
            decimal valorTotalPIS = 0;
            decimal valorTotalCreditoPresumido = 0;
            decimal valorTotalDiferencial = 0;
            decimal valorTotalCusto = 0;
            decimal totalBaseSTRetido = 0;
            decimal totalValorSTRetido = 0;
            var ItensNFe = ObterItens(nfe.NFe.infNFe.det, empresa, emitente, cultura, unidadeDeTrabalho, veiculoObs, kmObservacao, horimetroObservacao, regraEntradaDocumento, destinatario, out totalBaseCalculoICMS, out valorTotalCOFINS, out valorTotalICMS, out valorTotalIPI, out valorTotalPIS, out valorTotalCreditoPresumido, out valorTotalDiferencial, out valorTotalCusto, tipoServicoMultisoftware, out totalBaseSTRetido, out totalValorSTRetido, out regraEntradaDocumentoDoItem);

            if (regraEntradaDocumentoDoItem != null)
                regraEntradaDocumento = regraEntradaDocumentoDoItem;

            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = null;
            if (veiculoObs != null)// && !string.IsNullOrWhiteSpace(kmObservacao))
            {
                if (veiculoObs.Equipamentos != null && veiculoObs.Equipamentos.Count > 0)
                {
                    if (veiculoObs.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true)?.Count() == 1)
                        equipamento = veiculoObs.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true).FirstOrDefault();
                }
            }

            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;
            if (regraEntradaDocumento != null && destinatario != null && emitente != null)
            {
                if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumento.CFOPDentro != null)
                    cfopNota = regraEntradaDocumento.CFOPDentro;
                else if (regraEntradaDocumento.CFOPFora != null)
                    cfopNota = regraEntradaDocumento.CFOPFora;
                if (regraEntradaDocumento.NaturezaOperacao != null)
                    naturezaDaOperacaoNota = regraEntradaDocumento.NaturezaOperacao;
                if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                    tipoMovimentoNota = cfopNota.TipoMovimentoUso;
            }

            List<object> duplicatas = ObterDuplicatas(nfe, cultura, emitente, DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi.Split('T')[0], "yyyy-MM-dd", null), decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura), unidadeDeTrabalho);

            var retorno = new
            {
                BaseSTRetido = totalBaseSTRetido.ToString("n2"),
                ValorSTRetido = totalValorSTRetido.ToString("n2"),
                BaseCalculoICMS = totalBaseCalculoICMS.ToString("n2"),
                BaseCalculoICMSST = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vBCST, cultura).ToString("n2"),
                Chave = nfe.protNFe != null ? nfe.protNFe.infProt.chNFe : string.Empty,
                Fornecedor = new
                {
                    Codigo = emitente.CPF_CNPJ,
                    Descricao = emitente.Nome
                },
                Destinatario = new
                {
                    Codigo = destinatario?.Codigo ?? 0,
                    Descricao = destinatario?.RazaoSocial ?? string.Empty
                },
                CFOP = new
                {
                    Codigo = cfopNota != null ? cfopNota.Codigo : 0,
                    Descricao = cfopNota != null ? cfopNota.CFOPComExtensao : string.Empty
                },
                NaturezaOperacao = new
                {
                    Codigo = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Codigo : 0,
                    Descricao = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Descricao : string.Empty
                },
                TipoMovimento = new
                {
                    Codigo = tipoMovimentoNota != null ? tipoMovimentoNota.Codigo : 0,
                    Descricao = tipoMovimentoNota != null ? tipoMovimentoNota.Descricao : string.Empty
                },
                Veiculo = new
                {
                    Codigo = veiculoObs != null ? veiculoObs?.Codigo ?? 0 : 0,
                    Descricao = veiculoObs != null ? veiculoObs?.Placa ?? "" : string.Empty
                },
                Equipamento = new
                {
                    Codigo = equipamento != null ? equipamento.Codigo : 0,
                    Descricao = equipamento != null ? equipamento.Descricao : string.Empty
                },
                KMAbastecimento = (equipamento == null || (equipamento?.UtilizaTanqueCompartilhado ?? false)) ? kmObservacao : "",
                Horimetro = veiculoObs != null && veiculoObs.TipoVeiculo == "0" ? string.Empty : !string.IsNullOrWhiteSpace(horimetroObservacao) ? horimetroObservacao : equipamento != null ? kmObservacao : string.Empty,
                DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null).ToString("dd/MM/yyyy HH:mm"),
                DataEntrada = dataEntradaNota.ToString("dd/MM/yyyy"),
                IndicadorPagamento = regraEntradaDocumento != null ? regraEntradaDocumento.IndicadorPagamento : duplicatas.Count <= 1 ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.AVista : Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.APrazo,
                Numero = int.Parse(nfe.NFe.infNFe.ide.nNF),
                Serie = nfe.NFe.infNFe.ide.serie,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto,
                ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n2"),
                ValorBruto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n2"),
                ValorProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura).ToString("n2"),
                ValorTotalCOFINS = valorTotalCOFINS.ToString("n2"),
                ValorTotalDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura).ToString("n2"),
                ValorTotalSeguro = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vSeg, cultura).ToString("n2"),
                ValorTotalFrete = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vFrete, cultura).ToString("n2"),
                ValorTotalICMS = valorTotalICMS.ToString("n2"),
                ValorTotalICMSST = "0,00",
                ValorTotalIPI = valorTotalIPI.ToString("n2"),
                ValorTotalOutrasDespesas = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura).ToString("n2"),
                ValorTotalPIS = valorTotalPIS.ToString("n2"),
                ValorTotalCreditoPresumido = valorTotalCreditoPresumido.ToString("n2"),
                ValorTotalDiferencial = valorTotalDiferencial.ToString("n2"),
                ValorTotalFreteFora = "0,00",
                ValorTotalOutrasDespesasFora = "0,00",
                ValorTotalDescontoFora = "0,00",
                ValorTotalImpostosFora = "0,00",
                ValorTotalDiferencialFreteFora = "0,00",
                ValorTotalICMSFreteFora = "0,00",
                ValorTotalRetencaoPIS = "0,00",
                ValorTotalRetencaoCOFINS = "0,00",
                ValorTotalRetencaoIPI = "0,00",
                ValorTotalRetencaoINSS = "0,00",
                ValorTotalRetencaoCSLL = "0,00",
                ValorTotalRetencaoIR = "0,00",
                ValorTotalRetencaoISS = "0,00",
                ValorTotalRetencaoOutras = "0,00",
                ValorTotalCusto = valorTotalCusto.ToString("n2"),
                Especie = new
                {
                    especie.Codigo,
                    especie.Descricao
                },
                Modelo = new
                {
                    modelo.Codigo,
                    modelo.Descricao
                },
                Observacao = string.Empty,
                Duplicatas = duplicatas,
                Itens = ItensNFe,
                DocumentoImportadoXML = documentoImportadoXML ?? string.Empty,
            };

            return retorno;
        }

        private object ObterDetalhesPorNFSeCuritiba(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Empresa empresa, dynamic nfseCuritiba, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("39");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("NFS");

            //(string)nfse.InfRps.IdentificacaoRps.Numero

            Dominio.Entidades.Empresa destinatario = repEmpresa.BuscarPorCNPJ((string)nfseCuritiba.InfRps.Tomador.IdentificacaoTomador.CpfCnpj.Cnpj);
            if (destinatario?.CNPJ != empresa.CNPJ && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                throw new ServicoException("Destinatário do XML não é a sua empresa!");

            string cnpjEmitente = (string)nfseCuritiba.InfRps.Prestador.Cnpj;
            double.TryParse(cnpjEmitente, out double cnpjCPFEmitente);
            Dominio.Entidades.Cliente emitente = cnpjCPFEmitente > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCPFEmitente) : null;

            DateTime dataEntradaNota = DateTime.MinValue;
            DateTime.TryParseExact((string)nfseCuritiba.InfRps.DataEmissao, "MM/dd/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEntradaNota);
            if (dataEntradaNota <= DateTime.MinValue || dataEntrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada.DataLancamento)
                dataEntradaNota = DateTime.Now;

            decimal valorTotal = decimal.Parse((string)nfseCuritiba.InfRps.Servico.Valores.ValorLiquidoNfse, cultura);

            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = null;
            if (destinatario != null && emitente != null)
                regraEntradaDocumento = RetornaRegraEntrada(destinatario, emitente, "", unidadeDeTrabalho);
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;
            if (regraEntradaDocumento != null && destinatario != null && emitente != null)
            {
                if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumento.CFOPDentro != null)
                    cfopNota = regraEntradaDocumento.CFOPDentro;
                else if (regraEntradaDocumento.CFOPFora != null)
                    cfopNota = regraEntradaDocumento.CFOPFora;
                if (regraEntradaDocumento.NaturezaOperacao != null)
                    naturezaDaOperacaoNota = regraEntradaDocumento.NaturezaOperacao;
                if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                    tipoMovimentoNota = cfopNota.TipoMovimentoUso;
            }

            var retorno = new
            {
                BaseSTRetido = 0.ToString("n2"),
                ValorSTRetido = 0.ToString("n2"),
                BaseCalculoICMS = 0.ToString("n2"),
                BaseCalculoICMSST = 0.ToString("n2"),
                Chave = string.Empty,
                Fornecedor = new
                {
                    Codigo = emitente.CPF_CNPJ,
                    Descricao = emitente.Nome
                },
                Destinatario = new
                {
                    Codigo = destinatario?.Codigo ?? 0,
                    Descricao = destinatario?.RazaoSocial ?? string.Empty
                },
                CFOP = new
                {
                    Codigo = cfopNota != null ? cfopNota.Codigo : 0,
                    Descricao = cfopNota != null ? cfopNota.Descricao : string.Empty
                },
                NaturezaOperacao = new
                {
                    Codigo = naturezaDaOperacaoNota?.Codigo ?? 0,
                    Descricao = naturezaDaOperacaoNota?.Descricao ?? string.Empty
                },
                TipoMovimento = new
                {
                    Codigo = tipoMovimentoNota?.Codigo ?? 0,
                    Descricao = tipoMovimentoNota?.Descricao ?? string.Empty
                },
                Veiculo = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                Equipamento = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                KMAbastecimento = string.Empty,
                Horimetro = string.Empty,
                DataEmissao = DateTime.ParseExact((string)nfseCuritiba.InfRps.DataEmissao, "MM/dd/yyyy HH:mm:ss", null).ToString("dd/MM/yyyy HH:mm"),
                DataEntrada = dataEntradaNota.ToString("dd/MM/yyyy"),
                IndicadorPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.APrazo,
                Numero = int.Parse((string)nfseCuritiba.InfRps.IdentificacaoRps.Numero),
                Serie = (string)nfseCuritiba.InfRps.IdentificacaoRps.Serie,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto,
                ValorTotal = valorTotal.ToString("n2"),
                ValorBruto = valorTotal.ToString("n2"),
                ValorProdutos = valorTotal.ToString("n2"),
                ValorTotalCOFINS = 0.ToString("n2"),
                ValorTotalDesconto = 0.ToString("n2"),
                ValorTotalSeguro = 0.ToString("n2"),
                ValorTotalFrete = 0.ToString("n2"),
                ValorTotalICMS = 0.ToString("n2"),
                ValorTotalICMSST = 0.ToString("n2"),
                ValorTotalIPI = 0.ToString("n2"),
                ValorTotalOutrasDespesas = 0.ToString("n2"),
                ValorTotalPIS = 0.ToString("n2"),
                ValorTotalCreditoPresumido = 0.ToString("n2"),
                ValorTotalDiferencial = 0.ToString("n2"),
                ValorTotalFreteFora = 0.ToString("n2"),
                ValorTotalOutrasDespesasFora = 0.ToString("n2"),
                ValorTotalDescontoFora = 0.ToString("n2"),
                ValorTotalImpostosFora = 0.ToString("n2"),
                ValorTotalDiferencialFreteFora = 0.ToString("n2"),
                ValorTotalICMSFreteFora = 0.ToString("n2"),
                ValorTotalRetencaoPIS = 0.ToString("n2"),
                ValorTotalRetencaoCOFINS = 0.ToString("n2"),
                ValorTotalRetencaoIPI = 0.ToString("n2"),
                ValorTotalRetencaoINSS = 0.ToString("n2"),
                ValorTotalRetencaoCSLL = 0.ToString("n2"),
                ValorTotalRetencaoIR = 0.ToString("n2"),
                ValorTotalRetencaoISS = 0.ToString("n2"),
                ValorTotalRetencaoOutras = 0.ToString("n2"),
                ValorTotalCusto = valorTotal.ToString("n2"),
                Especie = new
                {
                    especie.Codigo,
                    especie.Descricao
                },
                Modelo = new
                {
                    modelo.Codigo,
                    modelo.Descricao
                },
                Expedidor = new
                {
                    Codigo = 0d,
                    Descricao = string.Empty
                },
                Recebedor = new
                {
                    Codigo = 0d,
                    Descricao = string.Empty
                },
                Observacao = (string)nfseCuritiba.InfRps.Servico.Discriminacao,
                Duplicatas = new List<object>(),
                Itens = ObterItens(nfseCuritiba, destinatario, emitente, cultura, unidadeDeTrabalho, destinatario, regraEntradaDocumento, out decimal valorTotalCusto, tipoServicoMultisoftware)
            };

            return retorno;
        }

        private List<object> ObterItens(dynamic nfseCuritiba, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente emitente, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Empresa destinatario, Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, out decimal valorTotalCusto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            valorTotalCusto = 0;
            List<object> itens = new List<object>();
            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoItem = null;
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);

            decimal valorICMSST = 0m;
            decimal baseSTRetido = 0m;
            decimal valorSTRetido = 0m;
            decimal bcSTDest = 0m;
            decimal icmsTDest = 0m;
            decimal valorIPI = 0m;

            decimal valorTotal = decimal.Parse((string)nfseCuritiba.InfRps.Servico.Valores.ValorLiquidoNfse, cultura);

            valorTotal += valorIPI + valorICMSST;

            Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigoProdutoECategoria((string)nfseCuritiba.InfRps.Servico.ItemListaServico, Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto.Servicos);

            if (destinatario != null && emitente != null)
                regraEntradaDocumentoItem = RetornaRegraEntrada(destinatario, emitente, "99", unidadeDeTrabalho);
            if (regraEntradaDocumentoItem == null && regraEntradaDocumento != null)
                regraEntradaDocumentoItem = regraEntradaDocumento;

            cfopNota = null;
            naturezaDaOperacaoNota = null;
            tipoMovimentoNota = null;
            if (regraEntradaDocumentoItem != null && destinatario != null && emitente != null)
            {
                if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumentoItem.CFOPDentro != null)
                    cfopNota = regraEntradaDocumentoItem.CFOPDentro;
                else if (regraEntradaDocumentoItem.CFOPFora != null)
                    cfopNota = regraEntradaDocumentoItem.CFOPFora;
                if (regraEntradaDocumentoItem.NaturezaOperacao != null)
                    naturezaDaOperacaoNota = regraEntradaDocumentoItem.NaturezaOperacao;
                if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                    tipoMovimentoNota = cfopNota.TipoMovimentoUso;
            }

            decimal fatorConversao = 0m;
            decimal quantidade = 0m;
            decimal quantidadeComercial = 1m;
            if (fatorConversao > 0)
                quantidade = quantidadeComercial * fatorConversao;
            else if (fatorConversao < 0)
                quantidade = quantidadeComercial / (fatorConversao * -1);
            else
                quantidade = quantidadeComercial;

            decimal valorUnitario = 0;
            decimal valorUnitarioComercial = valorTotal;
            if (fatorConversao != 0)
                valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
            else
            {
                valorUnitario = valorUnitarioComercial;
                if (valorIPI > 0 || valorICMSST > 0)
                    valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
            }

            decimal desconto = 0m;
            decimal valorSeguro = 0m;
            decimal valorFrete = 0m;
            decimal outrasDespesas = 0m;
            decimal baseCalculoImposto = valorTotal - desconto + outrasDespesas + valorFrete + valorSeguro;

            Dominio.ObjetosDeValor.Embarcador.Financeiro.DadosRegraEntradaDocumento regraEntrada = RetornaDadosEntradaDocumento(baseCalculoImposto, cfopNota, destinatario, emitente, regraEntradaDocumentoItem);

            bool desconsideraIcmsEfetivo = regraEntradaDocumentoItem?.NaturezaOperacao?.DesconsideraICMSEfetivo ?? false;
            if (regraEntrada != null && regraEntrada.AliquotaICMS == 0 && regraEntrada.ValorICMS == 0 && bcSTDest > 0 && icmsTDest > 0 && !desconsideraIcmsEfetivo)
            {
                if (string.IsNullOrWhiteSpace(regraEntrada.CSTICMS))
                    regraEntrada.CSTICMS = "060";
                regraEntrada.AliquotaICMS = 0;
                regraEntrada.ValorICMS = icmsTDest;
                regraEntrada.BaseICMS = bcSTDest;
                regraEntrada.AliquotaICMS = Math.Truncate(((icmsTDest * 100) / bcSTDest));
            }

            dynamic item = new
            {
                AliquotaICMS = new { val = regraEntrada?.AliquotaICMS ?? 0m, tipo = "decimal", configDecimal = new { precision = 4 } }, //new { val = aliquotaICMS, tipo = "decimal" },
                AliquotaIPI = new { val = regraEntrada?.AliquotaIPI ?? 0m, tipo = "decimal" }, //new { val = aliquotaIPI, tipo = "decimal" },
                AliquotaICMSST = new { val = 0m, tipo = "decimal" },
                AliquotaPIS = new { val = regraEntrada?.AliquotaPIS ?? 0m, tipo = "decimal" },
                AliquotaCOFINS = new { val = regraEntrada?.AliquotaCOFINS ?? 0m, tipo = "decimal" },
                AliquotaCreditoPresumido = new { val = regraEntrada?.AliquotaCreditoPresumido ?? 0m, tipo = "decimal" },
                AliquotaDiferencial = new { val = regraEntrada?.AliquotaDiferencial ?? 0m, tipo = "decimal" },
                BaseCalculoICMS = new { val = regraEntrada?.BaseICMS ?? 0m, tipo = "decimal" }, //new { val = baseCalculoICMS, tipo = "decimal" },
                BaseCalculoICMSST = new { val = 0m, tipo = "decimal" }, //new { val = baseCalculoICMSST, tipo = "decimal" },
                BaseCalculoIPI = new { val = regraEntrada?.BaseCalculoIPI ?? 0m, tipo = "decimal" },//new { val = baseCalculoIPI, tipo = "decimal" },
                BaseCalculoPIS = new { val = regraEntrada?.BaseCalculoIPI ?? 0m, tipo = "decimal" },
                BaseCalculoCOFINS = new { val = regraEntrada?.BaseCalculoCOFINS ?? 0m, tipo = "decimal" },
                BaseCalculoCreditoPresumido = new { val = regraEntrada?.BaseCalculoCreditoPresumido ?? 0m, tipo = "decimal" },
                BaseCalculoDiferencial = new { val = regraEntrada?.BaseCalculoDiferencial ?? 0m, tipo = "decimal" },
                BaseSTRetido = new { val = baseSTRetido, tipo = "decimal" },
                ValorSTRetido = new { val = valorSTRetido, tipo = "decimal" },
                ValorFreteFora = new { val = 0m, tipo = "decimal" },
                ValorOutrasDespesasFora = new { val = 0m, tipo = "decimal" },
                ValorDescontoFora = new { val = 0m, tipo = "decimal" },
                ValorImpostosFora = new { val = 0m, tipo = "decimal" },
                ValorDiferencialFreteFora = new { val = 0m, tipo = "decimal" },
                ValorICMSFreteFora = new { val = 0m, tipo = "decimal" },
                ValorRetencaoPIS = new { val = regraEntrada?.ValorRetencaoPIS ?? 0m, tipo = "decimal" },
                ValorRetencaoCOFINS = new { val = regraEntrada?.ValorRetencaoCOFINS ?? 0m, tipo = "decimal" },
                ValorRetencaoIPI = new { val = regraEntrada?.ValorRetencaoIPI ?? 0m, tipo = "decimal" },
                ValorRetencaoINSS = new { val = regraEntrada?.ValorRetencaoINSS ?? 0m, tipo = "decimal" },
                ValorRetencaoCSLL = new { val = regraEntrada?.ValorRetencaoCSLL ?? 0m, tipo = "decimal" },
                ValorRetencaoISS = new { val = regraEntrada?.ValorRetencaoISS ?? 0m, tipo = "decimal" },
                ValorRetencaoIR = new { val = regraEntrada?.ValorRetencaoIR ?? 0m, tipo = "decimal" },
                ValorRetencaoOutras = new { val = regraEntrada?.ValorRetencaoOutras ?? 0m, tipo = "decimal" },
                Codigo = Guid.NewGuid().ToString(),
                OrdemServico = new { Codigo = 0, Descricao = string.Empty },
                OrdemCompraMercadoria = new { Codigo = 0, Descricao = string.Empty },
                Equipamento = new { Codigo = 0, Descricao = string.Empty },
                Horimetro = string.Empty,
                DataAbastecimento = string.Empty,
                RegraEntradaDocumento = new
                {
                    Codigo = regraEntradaDocumentoItem != null ? regraEntradaDocumentoItem.Codigo : 0,
                    Descricao = regraEntradaDocumentoItem != null ? regraEntradaDocumentoItem.Descricao : string.Empty
                },
                CFOP = new
                {
                    Codigo = cfopNota != null ? cfopNota.Codigo : 0,
                    Descricao = cfopNota != null ? cfopNota.CFOPComExtensao : string.Empty
                },
                NaturezaOperacao = new
                {
                    Codigo = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Codigo : 0,
                    Descricao = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Descricao : string.Empty
                },
                TipoMovimento = new
                {
                    Codigo = tipoMovimentoNota != null ? tipoMovimentoNota.Codigo : 0,
                    Descricao = tipoMovimentoNota != null ? tipoMovimentoNota.Descricao : string.Empty
                },
                Veiculo = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                KMAbastecimento = "",
                CodigoProdutoFornecedor = (string)nfseCuritiba.InfRps.Servico.ItemListaServico,
                DescricaoProdutoFornecedor = produto?.Descricao ?? string.Empty,
                NCMProdutoFornecedor = "99",
                CESTProdutoFornecedor = produto?.CodigoCEST ?? "",
                CalculoCustoProduto = produto?.CalculoCustoProduto ?? "",
                CodigoBarrasEAN = produto?.CodigoEAN ?? "",
                Produto = new
                {
                    Codigo = produto?.Codigo ?? 0,
                    Descricao = produto?.Descricao ?? ""
                },
                UnidadeMedida = produto?.UnidadeDeMedida ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico,
                NCM = produto?.CodigoNCM ?? string.Empty,
                SiglaUnidadeMedida = produto != null && produto.UnidadeDeMedida.HasValue ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(produto.UnidadeDeMedida.Value) : "SERV",
                CSTICMS = regraEntrada?.CSTICMS ?? "", //cstICMS,
                CSTIPI = regraEntrada?.CSTIPI ?? "", //cstIPI,
                CSTPIS = regraEntrada?.CSTPIS ?? "", //cstPIS,
                CSTCOFINS = regraEntrada?.CSTCOFINS ?? "", //cstCOFINS,
                Desconto = new { val = 0m, tipo = "decimal" },
                ValorSeguro = new { val = 0m, tipo = "decimal" },
                Quantidade = new { val = quantidade, tipo = "decimal", configDecimal = new { precision = 4 } },
                Sequencial = 1,
                PercentualReducaoBaseCalculoIPI = new { val = regraEntrada?.PercentualReducaoIPI ?? 0m, tipo = "decimal" },
                PercentualReducaoBaseCalculoPIS = new { val = regraEntrada?.PercentualReducaoPIS ?? 0m, tipo = "decimal" },
                PercentualReducaoBaseCalculoCOFINS = new { val = regraEntrada?.PercentualReducaoCOFINS ?? 0m, tipo = "decimal" },
                ValorCOFINS = new { val = regraEntrada?.ValorCOFINS ?? 0m, tipo = "decimal" }, // new { val = valorCOFINS, tipo = "decimal" },
                ValorCreditoPresumido = new { val = regraEntrada?.ValorCreditoPresumido ?? 0m, tipo = "decimal" },
                ValorDiferencial = new { val = regraEntrada?.ValorDiferencial ?? 0m, tipo = "decimal" },
                ValorFrete = new { val = 0m, tipo = "decimal" },
                ValorICMS = new { val = regraEntrada?.ValorICMS ?? 0m, tipo = "decimal" }, //new { val = valorICMS, tipo = "decimal" },
                ValorICMSST = new { val = 0m, tipo = "decimal" }, //new { val = valorICMSST, tipo = "decimal" },
                ValorIPI = new { val = regraEntrada?.ValorIPICFOP ?? 0m, tipo = "decimal" }, //new { val = valorIPI, tipo = "decimal" },
                ValorOutrasDespesas = new { val = 0m, tipo = "decimal" },
                ValorPIS = new { val = regraEntrada?.ValorPIS ?? 0m, tipo = "decimal" }, //new { val = valorPIS, tipo = "decimal" },
                ValorTotal = new { val = valorTotal, tipo = "decimal" },
                ValorUnitario = new { val = valorUnitario, tipo = "decimal", configDecimal = new { precision = 4 } },
                ValorCustoUnitario = new { val = (((valorTotal) + (regraEntrada?.ValorDiferencial ?? 0m) + (regraEntrada?.ValorIPICFOP ?? 0m) + valorFrete + valorSeguro + outrasDespesas - desconto) / (quantidade > 0 ? quantidade : 1)), tipo = "decimal" },
                ValorCustoTotal = new { val = ((valorTotal) + (regraEntrada?.ValorDiferencial ?? 0m) + (regraEntrada?.ValorIPICFOP ?? 0m) + valorFrete + valorSeguro + outrasDespesas - desconto), tipo = "decimal" },
                NumeroFogoInicial = string.Empty,
                TipoAquisicao = string.Empty,
                VidaAtual = string.Empty,
                Almoxarifado = new { Codigo = 0, Descricao = string.Empty },
                ProdutoVinculado = new { Codigo = 0, Descricao = string.Empty },
                QuantidadeProdutoVinculado = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                LocalArmazenamento = new { Codigo = 0, Descricao = string.Empty },
                ObservacaoItem = string.Empty,
                UnidadeMedidaFornecedor = "SERV",
                QuantidadeFornecedor = new { val = quantidadeComercial, tipo = "decimal", configDecimal = new { precision = 4 } },
                ValorUnitarioFornecedor = new { val = valorUnitarioComercial, tipo = "decimal", configDecimal = new { precision = 4 } },
                CentroResultado = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                OrigemMercadoria = string.Empty,
                EncerrarOrdemServico = false
            };

            valorTotalCusto = ((valorTotal) + (regraEntrada?.ValorDiferencial ?? 0m) + (regraEntrada?.ValorIPICFOP ?? 0m) + valorFrete + valorSeguro + outrasDespesas - desconto);
            itens.Add(item);

            return itens;
        }

        private object ObterDetalhesPorCTe(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Empresa empresa, MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cte, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, string documentoImportadoXML)
        {
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);

            Dominio.Enumeradores.TipoTomador tipoTomador = ObterTipoTomadorCTe(cte);

            string cnpjEmpresa = cte.CTe.infCte.dest?.Item ?? "";
            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                cnpjEmpresa = cte.CTe.infCte.rem.Item;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                cnpjEmpresa = cte.CTe.infCte.exped.Item;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                cnpjEmpresa = cte.CTe.infCte.receb.Item;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                cnpjEmpresa = ((MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cte.CTe.infCte.ide.Item).Item;

            Dominio.Entidades.Empresa destinatario = repEmpresa.BuscarPorCNPJ(cnpjEmpresa);
            if (empresa == null && destinatario != null)
                empresa = repEmpresa.BuscarPorCNPJ(destinatario.CNPJ_SemFormato);

            if (destinatario?.CNPJ != empresa.CNPJ && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                throw new ServicoException("Tomador do XML não é a sua empresa!");

            Dominio.Entidades.Cliente emitente = svcCTe.ObterEmitente(cte.CTe.infCte.emit, empresa.Codigo, unidadeDeTrabalho);
            Dominio.Entidades.Cliente expedidor = svcCTe.ObterExpedidor(cte.CTe.infCte.exped, empresa.Codigo, unidadeDeTrabalho);
            Dominio.Entidades.Cliente recebedor = svcCTe.ObterRecebedor(cte.CTe.infCte.receb, empresa.Codigo, unidadeDeTrabalho);

            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("57");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("cte");
            Dominio.Entidades.Localidade localidadeInicioPrestacao = repLocalidade.BuscarPorCodigoIBGE(int.Parse(cte.CTe.infCte.ide.cMunIni));
            Dominio.Entidades.Localidade localidadeTerminoPrestacao = repLocalidade.BuscarPorCodigoIBGE(int.Parse(cte.CTe.infCte.ide.cMunFim));

            DateTime dataEntradaNota = DateTime.MinValue;
            DateTime.TryParseExact(cte.CTe.infCte.ide.dhEmi.Split('T')[0], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataEntradaNota);
            if (dataEntradaNota <= DateTime.MinValue || dataEntrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada.DataLancamento)
                dataEntradaNota = DateTime.Now;

            Dominio.Enumeradores.ModalidadeFrete tipoFrete = tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? Dominio.Enumeradores.ModalidadeFrete.Emitente :
                tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? Dominio.Enumeradores.ModalidadeFrete.Destinatario : Dominio.Enumeradores.ModalidadeFrete.SemFrete;

            var retorno = new
            {
                BaseSTRetido = 0.ToString("n2"),
                ValorSTRetido = 0.ToString("n2"),
                BaseCalculoICMS = 0.ToString("n2"),
                BaseCalculoICMSST = 0.ToString("n2"),
                Chave = cte.protCTe != null ? cte.protCTe.infProt.chCTe : string.Empty,
                Fornecedor = new
                {
                    Codigo = emitente.CPF_CNPJ,
                    Descricao = emitente.Nome
                },
                Destinatario = new
                {
                    Codigo = destinatario?.Codigo ?? 0,
                    Descricao = destinatario?.RazaoSocial ?? string.Empty
                },
                CFOP = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                NaturezaOperacao = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                TipoMovimento = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                Veiculo = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                Equipamento = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                KMAbastecimento = string.Empty,
                Horimetro = string.Empty,
                DataEmissao = DateTime.ParseExact(cte.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null).ToString("dd/MM/yyyy HH:mm"),
                DataEntrada = dataEntradaNota.ToString("dd/MM/yyyy"),
                IndicadorPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.APrazo,
                Numero = int.Parse(cte.CTe.infCte.ide.nCT),
                Serie = cte.CTe.infCte.ide.serie,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto,
                ValorTotal = 0.ToString("n2"),
                ValorBruto = 0.ToString("n2"),
                ValorProdutos = 0.ToString("n2"),
                ValorTotalCOFINS = 0.ToString("n2"),
                ValorTotalDesconto = 0.ToString("n2"),
                ValorTotalSeguro = 0.ToString("n2"),
                ValorTotalFrete = 0.ToString("n2"),
                ValorTotalICMS = 0.ToString("n2"),
                ValorTotalICMSST = 0.ToString("n2"),
                ValorTotalIPI = 0.ToString("n2"),
                ValorTotalOutrasDespesas = 0.ToString("n2"),
                ValorTotalPIS = 0.ToString("n2"),
                ValorTotalCreditoPresumido = 0.ToString("n2"),
                ValorTotalDiferencial = 0.ToString("n2"),
                ValorTotalFreteFora = 0.ToString("n2"),
                ValorTotalOutrasDespesasFora = 0.ToString("n2"),
                ValorTotalDescontoFora = 0.ToString("n2"),
                ValorTotalImpostosFora = 0.ToString("n2"),
                ValorTotalDiferencialFreteFora = 0.ToString("n2"),
                ValorTotalICMSFreteFora = 0.ToString("n2"),
                ValorTotalRetencaoPIS = 0.ToString("n2"),
                ValorTotalRetencaoCOFINS = 0.ToString("n2"),
                ValorTotalRetencaoIPI = 0.ToString("n2"),
                ValorTotalRetencaoINSS = 0.ToString("n2"),
                ValorTotalRetencaoCSLL = 0.ToString("n2"),
                ValorTotalRetencaoIR = 0.ToString("n2"),
                ValorTotalRetencaoISS = 0.ToString("n2"),
                ValorTotalRetencaoOutras = 0.ToString("n2"),
                ValorTotalCusto = 0.ToString("n2"),
                Especie = new
                {
                    especie.Codigo,
                    especie.Descricao
                },
                Modelo = new
                {
                    modelo.Codigo,
                    modelo.Descricao
                },
                Expedidor = new
                {
                    Codigo = expedidor?.CPF_CNPJ ?? 0d,
                    Descricao = expedidor?.Nome ?? string.Empty
                },
                Recebedor = new
                {
                    Codigo = recebedor?.CPF_CNPJ ?? 0d,
                    Descricao = recebedor?.Nome ?? string.Empty
                },
                LocalidadeInicioPrestacao = new
                {
                    Codigo = localidadeInicioPrestacao?.Codigo ?? 0,
                    Descricao = localidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty
                },
                LocalidadeTerminoPrestacao = new
                {
                    Codigo = localidadeTerminoPrestacao?.Codigo ?? 0,
                    Descricao = localidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty
                },
                Observacao = string.Empty,
                TipoFrete = tipoFrete,
                Duplicatas = new List<object>(),
                Itens = new List<object>(),
                DocumentoImportadoXML = documentoImportadoXML ?? string.Empty,
            };

            return retorno;
        }

        private object ObterDetalhesPorCTe(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Empresa empresa, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cte, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, string documentoImportadoXML)
        {
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

            Dominio.Enumeradores.TipoTomador tipoTomador = ObterTipoTomadorCTe(cte);

            string cnpjEmpresa = cte.CTe.infCte.dest?.Item ?? "";
            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                cnpjEmpresa = cte.CTe.infCte.rem.Item;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                cnpjEmpresa = cte.CTe.infCte.exped.Item;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                cnpjEmpresa = cte.CTe.infCte.receb.Item;
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                cnpjEmpresa = ((MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)cte.CTe.infCte.ide.Item).Item;

            Dominio.Entidades.Empresa destinatario = repEmpresa.BuscarPorCNPJ(cnpjEmpresa);
            if (empresa == null && destinatario != null)
                empresa = repEmpresa.BuscarPorCNPJ(destinatario.CNPJ_SemFormato);

            if (destinatario?.CNPJ != empresa.CNPJ && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                throw new ServicoException("Tomador do XML não é a sua empresa!");

            Dominio.Entidades.Cliente emitente = svcCTe.ObterEmitente(cte.CTe.infCte.emit, empresa.Codigo, unidadeDeTrabalho);
            Dominio.Entidades.Cliente expedidor = svcCTe.ObterExpedidor(cte.CTe.infCte.exped, empresa.Codigo, unidadeDeTrabalho);
            Dominio.Entidades.Cliente recebedor = svcCTe.ObterRecebedor(cte.CTe.infCte.receb, empresa.Codigo, unidadeDeTrabalho);

            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("57");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("cte");
            Dominio.Entidades.Localidade localidadeInicioPrestacao = repLocalidade.BuscarPorCodigoIBGE(int.Parse(cte.CTe.infCte.ide.cMunIni));
            Dominio.Entidades.Localidade localidadeTerminoPrestacao = repLocalidade.BuscarPorCodigoIBGE(int.Parse(cte.CTe.infCte.ide.cMunFim));

            DateTime dataEntradaNota = DateTime.MinValue;
            DateTime.TryParseExact(cte.CTe.infCte.ide.dhEmi.Split('T')[0], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataEntradaNota);
            if (dataEntradaNota <= DateTime.MinValue || dataEntrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada.DataLancamento)
                dataEntradaNota = DateTime.Now;

            Dominio.Enumeradores.ModalidadeFrete tipoFrete = tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente ? Dominio.Enumeradores.ModalidadeFrete.Emitente :
                tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? Dominio.Enumeradores.ModalidadeFrete.Destinatario : Dominio.Enumeradores.ModalidadeFrete.SemFrete;

            var retorno = new
            {
                BaseSTRetido = 0.ToString("n2"),
                ValorSTRetido = 0.ToString("n2"),
                BaseCalculoICMS = 0.ToString("n2"),
                BaseCalculoICMSST = 0.ToString("n2"),
                Chave = cte.protCTe != null ? cte.protCTe.infProt.chCTe : string.Empty,
                Fornecedor = new
                {
                    Codigo = emitente.CPF_CNPJ,
                    Descricao = emitente.Nome
                },
                Destinatario = new
                {
                    Codigo = destinatario?.Codigo ?? 0,
                    Descricao = destinatario?.RazaoSocial ?? string.Empty
                },
                CFOP = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                NaturezaOperacao = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                TipoMovimento = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                Veiculo = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                Equipamento = new
                {
                    Codigo = 0,
                    Descricao = string.Empty
                },
                KMAbastecimento = string.Empty,
                Horimetro = string.Empty,
                DataEmissao = DateTime.ParseExact(cte.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null).ToString("dd/MM/yyyy HH:mm"),
                DataEntrada = dataEntradaNota.ToString("dd/MM/yyyy"),
                IndicadorPagamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.APrazo,
                Numero = int.Parse(cte.CTe.infCte.ide.nCT),
                Serie = cte.CTe.infCte.ide.serie,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto,
                ValorTotal = 0.ToString("n2"),
                ValorBruto = 0.ToString("n2"),
                ValorProdutos = 0.ToString("n2"),
                ValorTotalCOFINS = 0.ToString("n2"),
                ValorTotalDesconto = 0.ToString("n2"),
                ValorTotalSeguro = 0.ToString("n2"),
                ValorTotalFrete = 0.ToString("n2"),
                ValorTotalICMS = 0.ToString("n2"),
                ValorTotalICMSST = 0.ToString("n2"),
                ValorTotalIPI = 0.ToString("n2"),
                ValorTotalOutrasDespesas = 0.ToString("n2"),
                ValorTotalPIS = 0.ToString("n2"),
                ValorTotalCreditoPresumido = 0.ToString("n2"),
                ValorTotalDiferencial = 0.ToString("n2"),
                ValorTotalFreteFora = 0.ToString("n2"),
                ValorTotalOutrasDespesasFora = 0.ToString("n2"),
                ValorTotalDescontoFora = 0.ToString("n2"),
                ValorTotalImpostosFora = 0.ToString("n2"),
                ValorTotalDiferencialFreteFora = 0.ToString("n2"),
                ValorTotalICMSFreteFora = 0.ToString("n2"),
                ValorTotalRetencaoPIS = 0.ToString("n2"),
                ValorTotalRetencaoCOFINS = 0.ToString("n2"),
                ValorTotalRetencaoIPI = 0.ToString("n2"),
                ValorTotalRetencaoINSS = 0.ToString("n2"),
                ValorTotalRetencaoCSLL = 0.ToString("n2"),
                ValorTotalRetencaoIR = 0.ToString("n2"),
                ValorTotalRetencaoISS = 0.ToString("n2"),
                ValorTotalRetencaoOutras = 0.ToString("n2"),
                ValorTotalCusto = 0.ToString("n2"),
                Especie = new
                {
                    especie.Codigo,
                    especie.Descricao
                },
                Modelo = new
                {
                    modelo.Codigo,
                    modelo.Descricao
                },
                Expedidor = new
                {
                    Codigo = expedidor?.CPF_CNPJ ?? 0d,
                    Descricao = expedidor?.Nome ?? string.Empty
                },
                Recebedor = new
                {
                    Codigo = recebedor?.CPF_CNPJ ?? 0d,
                    Descricao = recebedor?.Nome ?? string.Empty
                },
                LocalidadeInicioPrestacao = new
                {
                    Codigo = localidadeInicioPrestacao?.Codigo ?? 0,
                    Descricao = localidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty
                },
                LocalidadeTerminoPrestacao = new
                {
                    Codigo = localidadeTerminoPrestacao?.Codigo ?? 0,
                    Descricao = localidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty
                },
                Observacao = string.Empty,
                TipoFrete = tipoFrete,
                Duplicatas = new List<object>(),
                Itens = new List<object>(),
                DocumentoImportadoXML = documentoImportadoXML ?? string.Empty,
            };

            return retorno;
        }

        private List<object> ObterItens(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDet[] itensNota, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente emitente, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Veiculo veiculoOBS, string KMObs, string horimetroObservacao, Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, Dominio.Entidades.Empresa destinatario, out decimal totalBaseCalculoICMS, out decimal valorTotalCOFINS, out decimal valorTotalICMS, out decimal valorTotalIPI, out decimal valorTotalPIS, out decimal valorTotalCreditoPresumido, out decimal valorTotalDiferencial, out decimal valorTotalCusto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out decimal totalBaseSTRetido, out decimal totalValorSTRetido)
        {
            totalBaseCalculoICMS = 0;
            valorTotalCOFINS = 0;
            valorTotalICMS = 0;
            valorTotalIPI = 0;
            valorTotalPIS = 0;
            valorTotalCreditoPresumido = 0;
            valorTotalDiferencial = 0;
            valorTotalCusto = 0;
            totalBaseSTRetido = 0;
            totalValorSTRetido = 0;

            List<object> itens = new List<object>();
            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoItem = null;
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;

            foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDet det in itensNota)
            {
                object icms = this.ObterICMS(det.imposto);

                //decimal baseCalculoICMS = (decimal?)icms?.GetType().GetProperty("BaseCalculoICMS")?.GetValue(icms, null) ?? 0m;
                //decimal aliquotaICMS = (decimal?)icms?.GetType().GetProperty("AliquotaICMS")?.GetValue(icms, null) ?? 0m;
                //decimal valorICMS = (decimal?)icms?.GetType().GetProperty("ValorICMS")?.GetValue(icms, null) ?? 0m;
                string cstICMS = icms != null && icms.GetType().GetProperty("CST") != null ? int.Parse((string)icms.GetType().GetProperty("CST").GetValue(icms, null)).ToString("D3") : "";
                //decimal baseCalculoICMSST = (decimal?)icms?.GetType().GetProperty("BaseCalculoICMSST")?.GetValue(icms, null) ?? 0m;
                decimal valorICMSST = (decimal?)icms?.GetType().GetProperty("ValorICMSST")?.GetValue(icms, null) ?? 0m;

                decimal baseSTRetido = (decimal?)icms?.GetType().GetProperty("vBCSTRet")?.GetValue(icms, null) ?? 0m;
                decimal valorSTRetido = (decimal?)icms?.GetType().GetProperty("vICMSSTRet")?.GetValue(icms, null) ?? 0m;

                totalBaseSTRetido += baseSTRetido;
                totalValorSTRetido += valorSTRetido;

                object ipi = this.ObterIPI(det.imposto);

                //string cstIPI = ipi != null && ipi.GetType().GetProperty("CST") != null ? (string)ipi.GetType().GetProperty("CST").GetValue(ipi, null) : "";
                //decimal baseCalculoIPI = ipi != null && ipi.GetType().GetProperty("BaseCalculoIPI") != null ? (decimal)ipi.GetType().GetProperty("BaseCalculoIPI").GetValue(ipi, null) : 0m;
                //decimal aliquotaIPI = ipi != null && ipi.GetType().GetProperty("AliquotaIPI") != null ? (decimal)ipi.GetType().GetProperty("AliquotaIPI").GetValue(ipi, null) : 0m;
                decimal valorIPI = ipi != null && ipi.GetType().GetProperty("ValorIPI") != null ? (decimal)ipi.GetType().GetProperty("ValorIPI").GetValue(ipi, null) : 0m;

                //object pis = this.ObterPIS(det.imposto.PIS);

                //string cstPIS = pis != null && pis.GetType().GetProperty("CST") != null ? (string)pis.GetType().GetProperty("CST").GetValue(pis, null) : "";
                //decimal valorPIS = (decimal?)pis?.GetType().GetProperty("ValorPIS")?.GetValue(pis, null) ?? 0m;

                //object cofins = this.ObterCOFINS(det.imposto.COFINS);

                //string cstCOFINS = cofins != null && cofins.GetType().GetProperty("CST") != null ? (string)cofins.GetType().GetProperty("CST").GetValue(cofins, null) : "";
                //decimal valorCOFINS = (decimal?)cofins?.GetType().GetProperty("ValorCOFINS")?.GetValue(cofins, null) ?? 0m;

                decimal valorTotal = det.prod.vProd != null ? decimal.Parse(det.prod.vProd, cultura) : 0m;

                valorTotal += valorIPI + valorICMSST;

                object produto = this.ObterProduto(empresa, emitente, det.prod, unidadeDeTrabalho, tipoServicoMultisoftware);

                if (destinatario != null && emitente != null)
                    regraEntradaDocumentoItem = RetornaRegraEntrada(destinatario, emitente, (string)produto.GetType().GetProperty("NCMProdutoFornecedor").GetValue(produto, null), unidadeDeTrabalho);
                if (regraEntradaDocumentoItem == null && regraEntradaDocumento != null)
                    regraEntradaDocumentoItem = regraEntradaDocumento;

                cfopNota = null;
                naturezaDaOperacaoNota = null;
                tipoMovimentoNota = null;
                if (regraEntradaDocumentoItem != null && destinatario != null && emitente != null)
                {
                    if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumentoItem.CFOPDentro != null)
                        cfopNota = regraEntradaDocumentoItem.CFOPDentro;
                    else if (regraEntradaDocumentoItem.CFOPFora != null)
                        cfopNota = regraEntradaDocumentoItem.CFOPFora;
                    if (regraEntradaDocumentoItem.NaturezaOperacao != null)
                        naturezaDaOperacaoNota = regraEntradaDocumentoItem.NaturezaOperacao;
                    if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                        tipoMovimentoNota = cfopNota.TipoMovimentoUso;
                }

                decimal fatorConversao = decimal.Parse(((string)produto.GetType().GetProperty("FatorConversao").GetValue(produto, null)));
                decimal quantidade = 0m;
                decimal quantidadeComercial = det.prod.qCom != null ? decimal.Parse(det.prod.qCom, cultura) : 0m;
                if (fatorConversao > 0)
                    quantidade = quantidadeComercial * fatorConversao;
                else if (fatorConversao < 0)
                    quantidade = quantidadeComercial / (fatorConversao * -1);
                else
                    quantidade = quantidadeComercial;

                decimal valorUnitario = 0;
                decimal valorUnitarioComercial = det.prod.vUnCom != null ? decimal.Parse(det.prod.vUnCom, cultura) : 0m;
                if (fatorConversao != 0)
                    valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
                else
                {
                    valorUnitario = valorUnitarioComercial;
                    if (valorIPI > 0 || valorICMSST > 0)
                        valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
                }

                decimal desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m;
                decimal valorSeguro = det.prod.vSeg != null ? decimal.Parse(det.prod.vSeg, cultura) : 0m;
                decimal valorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m;
                decimal outrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m;
                decimal baseCalculoImposto = valorTotal - desconto + outrasDespesas + valorFrete + valorSeguro;

                Dominio.ObjetosDeValor.Embarcador.Financeiro.DadosRegraEntradaDocumento regraEntrada = RetornaDadosEntradaDocumento(baseCalculoImposto, cfopNota, destinatario, emitente, regraEntradaDocumentoItem);

                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = null;
                if (veiculoOBS != null)// && !string.IsNullOrWhiteSpace(KMObs))
                {
                    if (veiculoOBS.Equipamentos != null && veiculoOBS.Equipamentos.Count > 0)
                    {
                        if (veiculoOBS.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true)?.Count() == 1)
                            equipamento = veiculoOBS.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true).FirstOrDefault();
                    }
                }

                dynamic item = new
                {
                    AliquotaICMS = new { val = regraEntrada.AliquotaICMS, tipo = "decimal", configDecimal = new { precision = 4 } },
                    AliquotaIPI = new { val = regraEntrada.AliquotaIPI, tipo = "decimal" },
                    AliquotaICMSST = new { val = 0m, tipo = "decimal" },
                    AliquotaPIS = new { val = regraEntrada.AliquotaPIS, tipo = "decimal" },
                    AliquotaCOFINS = new { val = regraEntrada.AliquotaCOFINS, tipo = "decimal" },
                    AliquotaCreditoPresumido = new { val = regraEntrada.AliquotaCreditoPresumido, tipo = "decimal" },
                    AliquotaDiferencial = new { val = regraEntrada.AliquotaDiferencial, tipo = "decimal" },
                    BaseCalculoICMS = new { val = regraEntrada.BaseICMS, tipo = "decimal" },
                    BaseCalculoICMSST = new { val = 0m, tipo = "decimal" },
                    BaseCalculoIPI = new { val = regraEntrada.BaseCalculoIPI, tipo = "decimal" },
                    BaseCalculoPIS = new { val = regraEntrada.BaseCalculoIPI, tipo = "decimal" },
                    BaseCalculoCOFINS = new { val = regraEntrada.BaseCalculoCOFINS, tipo = "decimal" },
                    BaseCalculoCreditoPresumido = new { val = regraEntrada.BaseCalculoCreditoPresumido, tipo = "decimal" },
                    BaseCalculoDiferencial = new { val = regraEntrada.BaseCalculoDiferencial, tipo = "decimal" },
                    BaseSTRetido = new { val = baseSTRetido, tipo = "decimal" },
                    ValorSTRetido = new { val = valorSTRetido, tipo = "decimal" },
                    ValorFreteFora = new { val = 0m, tipo = "decimal" },
                    ValorOutrasDespesasFora = new { val = 0m, tipo = "decimal" },
                    ValorDescontoFora = new { val = 0m, tipo = "decimal" },
                    ValorImpostosFora = new { val = 0m, tipo = "decimal" },
                    ValorDiferencialFreteFora = new { val = 0m, tipo = "decimal" },
                    ValorICMSFreteFora = new { val = 0m, tipo = "decimal" },
                    ValorRetencaoPIS = new { val = regraEntrada.ValorRetencaoPIS, tipo = "decimal" },
                    ValorRetencaoCOFINS = new { val = regraEntrada.ValorRetencaoCOFINS, tipo = "decimal" },
                    ValorRetencaoIPI = new { val = regraEntrada.ValorRetencaoIPI, tipo = "decimal" },
                    ValorRetencaoINSS = new { val = regraEntrada.ValorRetencaoINSS, tipo = "decimal" },
                    ValorRetencaoCSLL = new { val = regraEntrada.ValorRetencaoCSLL, tipo = "decimal" },
                    ValorRetencaoISS = new { val = regraEntrada.ValorRetencaoISS, tipo = "decimal" },
                    ValorRetencaoIR = new { val = regraEntrada.ValorRetencaoIR, tipo = "decimal" },
                    ValorRetencaoOutras = new { val = regraEntrada.ValorRetencaoOutras, tipo = "decimal" },
                    Codigo = Guid.NewGuid().ToString(),
                    CstIcmsFornecedor = cstICMS,
                    OrdemServico = new { Codigo = 0, Descricao = string.Empty },
                    OrdemCompraMercadoria = new { Codigo = 0, Descricao = string.Empty },
                    Equipamento = new { Codigo = equipamento?.Codigo ?? 0, Descricao = equipamento?.Descricao ?? "" },
                    Horimetro = veiculoOBS != null && veiculoOBS.TipoVeiculo == "0" ? string.Empty : !string.IsNullOrWhiteSpace(horimetroObservacao) ? horimetroObservacao : equipamento != null ? KMObs : string.Empty,
                    DataAbastecimento = string.Empty,
                    RegraEntradaDocumento = new
                    {
                        Codigo = regraEntradaDocumentoItem != null ? regraEntradaDocumentoItem.Codigo : 0,
                        Descricao = regraEntradaDocumentoItem != null ? regraEntradaDocumentoItem.Descricao : string.Empty
                    },
                    CFOP = new
                    {
                        Codigo = cfopNota != null ? cfopNota.Codigo : 0,
                        Descricao = cfopNota != null ? cfopNota.CFOPComExtensao : string.Empty
                    },
                    NaturezaOperacao = new
                    {
                        Codigo = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Codigo : 0,
                        Descricao = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Descricao : string.Empty
                    },
                    TipoMovimento = new
                    {
                        Codigo = tipoMovimentoNota != null ? tipoMovimentoNota.Codigo : 0,
                        Descricao = tipoMovimentoNota != null ? tipoMovimentoNota.Descricao : string.Empty
                    },
                    Veiculo = new
                    {
                        Codigo = veiculoOBS != null ? veiculoOBS.Codigo : 0,
                        Descricao = veiculoOBS != null ? veiculoOBS.Placa : string.Empty
                    },
                    KMAbastecimento = (equipamento == null || (equipamento?.UtilizaTanqueCompartilhado ?? false)) ? KMObs : "",
                    CodigoProdutoFornecedor = (string)produto.GetType().GetProperty("CodigoProdutoFornecedor").GetValue(produto, null),
                    DescricaoProdutoFornecedor = (string)produto.GetType().GetProperty("DescricaoProdutoFornecedor").GetValue(produto, null),
                    NCMProdutoFornecedor = (string)produto.GetType().GetProperty("NCMProdutoFornecedor").GetValue(produto, null),
                    CESTProdutoFornecedor = (string)produto.GetType().GetProperty("CESTProdutoFornecedor").GetValue(produto, null),
                    CalculoCustoProduto = (string)produto.GetType().GetProperty("CalculoCustoProduto").GetValue(produto, null),
                    CodigoBarrasEAN = (string)produto.GetType().GetProperty("CodigoBarrasEAN").GetValue(produto, null),
                    Produto = new
                    {
                        Codigo = (int)produto.GetType().GetProperty("Codigo").GetValue(produto, null),
                        Descricao = (string)produto.GetType().GetProperty("Descricao").GetValue(produto, null)
                    },
                    UnidadeMedida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)produto.GetType().GetProperty("UnidadeMedida").GetValue(produto, null),
                    NCM = (string)produto.GetType().GetProperty("CodigoNCM").GetValue(produto, null),
                    SiglaUnidadeMedida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla((Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)produto.GetType().GetProperty("UnidadeMedida").GetValue(produto, null)),
                    CSTICMS = regraEntrada.CSTICMS,
                    CSTIPI = regraEntrada.CSTIPI,
                    CSTPIS = regraEntrada.CSTPIS,
                    CSTCOFINS = regraEntrada.CSTCOFINS,
                    Desconto = new { val = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m, tipo = "decimal" },
                    ValorSeguro = new { val = det.prod.vSeg != null ? decimal.Parse(det.prod.vSeg, cultura) : 0m, tipo = "decimal" },
                    Quantidade = new { val = quantidade, tipo = "decimal", configDecimal = new { precision = 4 } },
                    Sequencial = int.Parse(det.nItem),
                    PercentualReducaoBaseCalculoIPI = new { val = regraEntrada.PercentualReducaoIPI, tipo = "decimal" },
                    PercentualReducaoBaseCalculoPIS = new { val = regraEntrada.PercentualReducaoPIS, tipo = "decimal" },
                    PercentualReducaoBaseCalculoCOFINS = new { val = regraEntrada.PercentualReducaoCOFINS, tipo = "decimal" },
                    ValorCOFINS = new { val = regraEntrada.ValorCOFINS, tipo = "decimal" },
                    ValorCreditoPresumido = new { val = regraEntrada.ValorCreditoPresumido, tipo = "decimal" },
                    ValorDiferencial = new { val = regraEntrada.ValorDiferencial, tipo = "decimal" },
                    ValorFrete = new { val = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m, tipo = "decimal" },
                    ValorICMS = new { val = regraEntrada.ValorICMS, tipo = "decimal" },
                    ValorICMSST = new { val = 0m, tipo = "decimal" },
                    ValorIPI = new { val = regraEntrada.ValorIPICFOP, tipo = "decimal" },
                    ValorOutrasDespesas = new { val = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m, tipo = "decimal" },
                    ValorPIS = new { val = regraEntrada.ValorPIS, tipo = "decimal" },
                    ValorTotal = new { val = valorTotal, tipo = "decimal" },
                    ValorUnitario = new { val = valorUnitario, tipo = "decimal", configDecimal = new { precision = 4 } },
                    ValorCustoUnitario = new { val = (((valorTotal) + regraEntrada.ValorDiferencial + regraEntrada.ValorIPICFOP + valorFrete + valorSeguro + outrasDespesas - desconto) / (quantidade > 0 ? quantidade : 1)), tipo = "decimal" },
                    ValorCustoTotal = new { val = ((valorTotal) + regraEntrada.ValorDiferencial + regraEntrada.ValorIPICFOP + valorFrete + valorSeguro + outrasDespesas - desconto), tipo = "decimal" },
                    NumeroFogoInicial = string.Empty,
                    TipoAquisicao = string.Empty,
                    VidaAtual = string.Empty,
                    Almoxarifado = new { Codigo = 0, Descricao = string.Empty },
                    ProdutoVinculado = new { Codigo = 0, Descricao = string.Empty },
                    QuantidadeProdutoVinculado = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                    LocalArmazenamento = new { Codigo = 0, Descricao = string.Empty },
                    ObservacaoItem = string.Empty,
                    UnidadeMedidaFornecedor = det.prod.uCom != null ? det.prod.uCom : string.Empty,
                    QuantidadeFornecedor = new { val = quantidadeComercial, tipo = "decimal", configDecimal = new { precision = 4 } },
                    ValorUnitarioFornecedor = new { val = valorUnitarioComercial, tipo = "decimal", configDecimal = new { precision = 4 } },
                    CentroResultado = new
                    {
                        Codigo = veiculoOBS?.CentroResultado?.Codigo ?? 0,
                        Descricao = veiculoOBS?.CentroResultado?.Descricao ?? string.Empty
                    },
                    OrigemMercadoria = icms?.GetType().GetProperty("OrigemMercadoria")?.GetValue(icms, null) ?? 0,
                    EncerrarOrdemServico = false
                };

                totalBaseCalculoICMS += regraEntrada.BaseICMS;
                valorTotalCOFINS += regraEntrada.ValorCOFINS;
                valorTotalICMS += regraEntrada.ValorICMS;
                valorTotalIPI += regraEntrada.ValorIPICFOP;
                valorTotalPIS += regraEntrada.ValorPIS;
                valorTotalCreditoPresumido += regraEntrada.ValorCreditoPresumido;
                valorTotalDiferencial += regraEntrada.ValorDiferencial;
                valorTotalCusto = ((valorTotal) + regraEntrada.ValorDiferencial + regraEntrada.ValorIPICFOP + valorFrete + valorSeguro + outrasDespesas - desconto);

                itens.Add(item);
            }

            return itens;
        }

        private List<object> ObterItens(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDet[] itensNota, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente emitente, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Veiculo veiculoOBS, string KMObs, string horimetroObservacao, Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, Dominio.Entidades.Empresa destinatario, out decimal totalBaseCalculoICMS, out decimal valorTotalCOFINS, out decimal valorTotalICMS, out decimal valorTotalIPI, out decimal valorTotalPIS, out decimal valorTotalCreditoPresumido, out decimal valorTotalDiferencial, out decimal valorTotalCusto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out decimal totalBaseSTRetido, out decimal totalValorSTRetido, out Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoDoItem)
        {
            totalBaseCalculoICMS = 0;
            valorTotalCOFINS = 0;
            valorTotalICMS = 0;
            valorTotalIPI = 0;
            valorTotalPIS = 0;
            valorTotalCreditoPresumido = 0;
            valorTotalDiferencial = 0;
            valorTotalCusto = 0;
            totalBaseSTRetido = 0;
            totalValorSTRetido = 0;

            List<object> itens = new List<object>();
            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoItem = null;
            regraEntradaDocumentoDoItem = null;
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;

            foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDet det in itensNota)
            {
                object icms = this.ObterICMS(det.imposto);

                decimal baseCalculoICMS = (decimal?)icms?.GetType().GetProperty("BaseCalculoICMS")?.GetValue(icms, null) ?? 0m;
                decimal aliquotaICMS = (decimal?)icms?.GetType().GetProperty("AliquotaICMS")?.GetValue(icms, null) ?? 0m;
                decimal valorICMS = (decimal?)icms?.GetType().GetProperty("ValorICMS")?.GetValue(icms, null) ?? 0m;
                string cstICMS = icms != null && icms.GetType().GetProperty("CST") != null ? icms.GetType().GetProperty("CST").GetValue(icms, null).ToString() : "";
                string origICMS = icms != null && icms.GetType().GetProperty("OrigemMercadoria") != null ? icms.GetType().GetProperty("OrigemMercadoria").GetValue(icms, null).ToString() : "";
                //decimal baseCalculoICMSST = (decimal?)icms?.GetType().GetProperty("BaseCalculoICMSST")?.GetValue(icms, null) ?? 0m;
                decimal valorICMSST = (decimal?)icms?.GetType().GetProperty("ValorICMSST")?.GetValue(icms, null) ?? 0m;
                string codigoCstIcmsComOrigem = cstICMS.Length > 2 ? cstICMS : $"{origICMS}{cstICMS}";


                decimal baseSTRetido = (decimal?)icms?.GetType().GetProperty("vBCSTRet")?.GetValue(icms, null) ?? 0m;
                decimal valorSTRetido = (decimal?)icms?.GetType().GetProperty("vICMSSTRet")?.GetValue(icms, null) ?? 0m;
                decimal bcSTDest = (decimal?)icms?.GetType().GetProperty("BCSTDest")?.GetValue(icms, null) ?? 0m;
                decimal icmsTDest = (decimal?)icms?.GetType().GetProperty("ICMSSTDest")?.GetValue(icms, null) ?? 0m;

                totalBaseSTRetido += baseSTRetido;
                totalValorSTRetido += valorSTRetido;

                object ipi = this.ObterIPI(det.imposto);

                string cstIPI = ipi != null && ipi.GetType().GetProperty("CST") != null ? (string)ipi.GetType().GetProperty("CST").GetValue(ipi, null) : "";
                decimal baseCalculoIPI = ipi != null && ipi.GetType().GetProperty("BaseCalculoIPI") != null ? (decimal)ipi.GetType().GetProperty("BaseCalculoIPI").GetValue(ipi, null) : 0m;
                decimal aliquotaIPI = ipi != null && ipi.GetType().GetProperty("AliquotaIPI") != null ? (decimal)ipi.GetType().GetProperty("AliquotaIPI").GetValue(ipi, null) : 0m;
                decimal valorIPI = ipi != null && ipi.GetType().GetProperty("ValorIPI") != null ? (decimal)ipi.GetType().GetProperty("ValorIPI").GetValue(ipi, null) : 0m;

                object pis = this.ObterPIS(det.imposto.PIS);

                string cstPIS = pis != null && pis.GetType().GetProperty("CST") != null ? (string)pis.GetType().GetProperty("CST").GetValue(pis, null) : "";
                decimal valorPIS = (decimal?)pis?.GetType().GetProperty("ValorPIS")?.GetValue(pis, null) ?? 0m;

                object cofins = this.ObterCOFINS(det.imposto.COFINS);

                string cstCOFINS = cofins != null && cofins.GetType().GetProperty("CST") != null ? (string)cofins.GetType().GetProperty("CST").GetValue(cofins, null) : "";
                decimal valorCOFINS = (decimal?)cofins?.GetType().GetProperty("ValorCOFINS")?.GetValue(cofins, null) ?? 0m;

                decimal valorTotal = det.prod.vProd != null ? decimal.Parse(det.prod.vProd, cultura) : 0m;
                decimal valorTotalProduto = det.prod.vProd != null ? decimal.Parse(det.prod.vProd, cultura) : 0m;

                valorTotal += valorIPI + valorICMSST;

                object produto = this.ObterProduto(empresa, emitente, det.prod, unidadeDeTrabalho, tipoServicoMultisoftware);

                if (destinatario != null && emitente != null)
                    regraEntradaDocumentoItem = RetornaRegraEntrada(destinatario, emitente, (string)produto.GetType().GetProperty("NCMProdutoFornecedor").GetValue(produto, null), unidadeDeTrabalho);
                if (regraEntradaDocumentoItem == null && regraEntradaDocumento != null)
                    regraEntradaDocumentoItem = regraEntradaDocumento;

                cfopNota = null;
                naturezaDaOperacaoNota = null;
                tipoMovimentoNota = null;
                if (regraEntradaDocumentoItem != null && destinatario != null && emitente != null)
                {
                    if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumentoItem.CFOPDentro != null)
                        cfopNota = regraEntradaDocumentoItem.CFOPDentro;
                    else if (regraEntradaDocumentoItem.CFOPFora != null)
                        cfopNota = regraEntradaDocumentoItem.CFOPFora;
                    if (regraEntradaDocumentoItem.NaturezaOperacao != null)
                        naturezaDaOperacaoNota = regraEntradaDocumentoItem.NaturezaOperacao;
                    if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                        tipoMovimentoNota = cfopNota.TipoMovimentoUso;

                    regraEntradaDocumentoDoItem = regraEntradaDocumentoItem;
                }

                decimal fatorConversao = decimal.Parse(((string)produto.GetType().GetProperty("FatorConversao").GetValue(produto, null)));
                decimal quantidade = 0m;
                decimal quantidadeComercial = det.prod.qCom != null ? decimal.Parse(det.prod.qCom, cultura) : 0m;
                if (quantidadeComercial <= 0)
                    quantidadeComercial = det.prod.qTrib != null ? decimal.Parse(det.prod.qTrib, cultura) : 0m;
                if (fatorConversao > 0)
                    quantidade = quantidadeComercial * fatorConversao;
                else if (fatorConversao < 0)
                    quantidade = quantidadeComercial / (fatorConversao * -1);
                else
                    quantidade = quantidadeComercial;

                decimal valorUnitario = 0;
                decimal valorUnitarioComercial = det.prod.vUnCom != null ? decimal.Parse(det.prod.vUnCom, cultura) : 0m;
                if (fatorConversao != 0)
                    valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
                else
                {
                    valorUnitario = valorUnitarioComercial;
                    if (valorIPI > 0 || valorICMSST > 0)
                    {
                        valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
                        valorUnitarioComercial = valorUnitario;
                    }
                }

                decimal desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m;
                decimal valorSeguro = det.prod.vSeg != null ? decimal.Parse(det.prod.vSeg, cultura) : 0m;
                decimal valorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m;
                decimal outrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m;
                decimal baseCalculoImposto = valorTotal - desconto + outrasDespesas + valorFrete + valorSeguro;

                Dominio.ObjetosDeValor.Embarcador.Financeiro.DadosRegraEntradaDocumento regraEntrada = RetornaDadosEntradaDocumento(baseCalculoImposto, cfopNota, destinatario, emitente, regraEntradaDocumentoItem);

                bool desconsideraIcmsEfetivo = regraEntradaDocumentoItem?.NaturezaOperacao?.DesconsideraICMSEfetivo ?? false;
                if (regraEntrada.AliquotaICMS == 0 && regraEntrada.ValorICMS == 0 && bcSTDest > 0 && icmsTDest > 0 && !desconsideraIcmsEfetivo)
                {
                    if (string.IsNullOrWhiteSpace(regraEntrada.CSTICMS))
                        regraEntrada.CSTICMS = "060";
                    regraEntrada.AliquotaICMS = 0;
                    regraEntrada.ValorICMS = icmsTDest;
                    regraEntrada.BaseICMS = bcSTDest;
                    regraEntrada.AliquotaICMS = Math.Truncate(((icmsTDest * 100) / bcSTDest));
                }

                if ((!string.IsNullOrWhiteSpace(cstICMS) || cstICMS.Equals("061")) && !regraEntrada.CSTICMS.Equals("040"))
                {
                    regraEntrada.CSTICMS = !string.IsNullOrWhiteSpace(regraEntrada.CSTICMS) ? regraEntrada.CSTICMS : cstICMS;
                    regraEntrada.BaseICMS = regraEntrada.BaseICMS > 0 ? regraEntrada.BaseICMS : baseCalculoICMS;
                    regraEntrada.AliquotaICMS = regraEntrada.AliquotaICMS > 0 ? regraEntrada.AliquotaICMS : aliquotaICMS;
                    regraEntrada.ValorICMS = regraEntrada.ValorICMS > 0 ? regraEntrada.ValorICMS : valorICMS;
                }

                if (!string.IsNullOrWhiteSpace(cstIPI))
                {
                    regraEntrada.CSTIPI = !string.IsNullOrWhiteSpace(regraEntrada.CSTIPI) ? regraEntrada.CSTIPI : cstIPI;
                    regraEntrada.BaseCalculoIPI = regraEntrada.BaseCalculoIPI > 0 ? regraEntrada.BaseCalculoIPI : baseCalculoIPI;
                    regraEntrada.AliquotaIPI = regraEntrada.AliquotaIPI > 0 ? regraEntrada.AliquotaIPI : aliquotaIPI;
                    regraEntrada.ValorIPICFOP = regraEntrada.ValorIPICFOP > 0 ? regraEntrada.ValorIPICFOP : valorIPI;
                }

                if (!string.IsNullOrWhiteSpace(cstCOFINS))
                {
                    regraEntrada.CSTCOFINS = !string.IsNullOrWhiteSpace(regraEntrada.CSTCOFINS) ? regraEntrada.CSTCOFINS : cstCOFINS;
                    //regraEntrada.BaseCalculoCOFINS = regraEntrada.BaseCalculoCOFINS > 0 ? regraEntrada.BaseCalculoCOFINS : baseCalculoCOFINS;
                    //regraEntrada.AliquotaCOFINS = regraEntrada.AliquotaCOFINS > 0 ? regraEntrada.AliquotaCOFINS : aliquotaCOFINS;
                    regraEntrada.ValorCOFINS = regraEntrada.ValorCOFINS > 0 ? regraEntrada.ValorCOFINS : valorCOFINS;
                }

                if (!string.IsNullOrWhiteSpace(cstPIS))
                {
                    regraEntrada.CSTPIS = !string.IsNullOrWhiteSpace(regraEntrada.CSTPIS) ? regraEntrada.CSTPIS : cstPIS;
                    //regraEntrada.BaseCalculoPIS = regraEntrada.BaseCalculoPIS > 0 ? regraEntrada.BaseCalculoPIS : baseCalculoPIS;
                    //regraEntrada.AliquotaPIS = regraEntrada.AliquotaPIS > 0 ? regraEntrada.AliquotaPIS : aliquotaPIS;
                    regraEntrada.ValorPIS = regraEntrada.ValorPIS > 0 ? regraEntrada.ValorPIS : valorPIS;
                }

                if ((cfopNota?.CreditoSobreTotalParaItensSujeitosICMSST ?? false) && (baseSTRetido > 0 || valorSTRetido > 0))
                {
                    totalBaseSTRetido -= baseSTRetido;
                    totalValorSTRetido -= valorSTRetido;
                    baseSTRetido = 0;
                    valorSTRetido = 0;
                }

                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = null;
                if (veiculoOBS != null)// && !string.IsNullOrWhiteSpace(KMObs))
                {
                    if (veiculoOBS.Equipamentos != null && veiculoOBS.Equipamentos.Count > 0)
                    {
                        if (veiculoOBS.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true)?.Count() == 1)
                            equipamento = veiculoOBS.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true).FirstOrDefault();
                    }
                }

                dynamic item = new
                {
                    AliquotaICMS = new { val = regraEntrada.AliquotaICMS, tipo = "decimal", configDecimal = new { precision = 4 } },
                    AliquotaIPI = new { val = regraEntrada.AliquotaIPI, tipo = "decimal" },
                    AliquotaICMSST = new { val = 0m, tipo = "decimal" },
                    AliquotaPIS = new { val = regraEntrada.AliquotaPIS, tipo = "decimal" },
                    AliquotaCOFINS = new { val = regraEntrada.AliquotaCOFINS, tipo = "decimal" },
                    AliquotaCreditoPresumido = new { val = regraEntrada.AliquotaCreditoPresumido, tipo = "decimal" },
                    AliquotaDiferencial = new { val = regraEntrada.AliquotaDiferencial, tipo = "decimal" },
                    BaseCalculoICMS = new { val = regraEntrada.BaseICMS, tipo = "decimal" },
                    BaseCalculoICMSST = new { val = 0m, tipo = "decimal" },
                    BaseCalculoIPI = new { val = regraEntrada.BaseCalculoIPI, tipo = "decimal" },
                    BaseCalculoPIS = new { val = regraEntrada.BaseCalculoPIS, tipo = "decimal" },
                    BaseCalculoCOFINS = new { val = regraEntrada.BaseCalculoCOFINS, tipo = "decimal" },
                    BaseCalculoCreditoPresumido = new { val = regraEntrada.BaseCalculoCreditoPresumido, tipo = "decimal" },
                    BaseCalculoDiferencial = new { val = regraEntrada.BaseCalculoDiferencial, tipo = "decimal" },
                    BaseSTRetido = new { val = baseSTRetido, tipo = "decimal" },
                    ValorSTRetido = new { val = valorSTRetido, tipo = "decimal" },
                    ValorFreteFora = new { val = 0m, tipo = "decimal" },
                    ValorOutrasDespesasFora = new { val = 0m, tipo = "decimal" },
                    ValorDescontoFora = new { val = 0m, tipo = "decimal" },
                    ValorImpostosFora = new { val = 0m, tipo = "decimal" },
                    ValorDiferencialFreteFora = new { val = 0m, tipo = "decimal" },
                    ValorICMSFreteFora = new { val = 0m, tipo = "decimal" },
                    ValorRetencaoPIS = new { val = regraEntrada.ValorRetencaoPIS, tipo = "decimal" },
                    ValorRetencaoCOFINS = new { val = regraEntrada.ValorRetencaoCOFINS, tipo = "decimal" },
                    ValorRetencaoIPI = new { val = regraEntrada.ValorRetencaoIPI, tipo = "decimal" },
                    ValorRetencaoINSS = new { val = regraEntrada.ValorRetencaoINSS, tipo = "decimal" },
                    ValorRetencaoCSLL = new { val = regraEntrada.ValorRetencaoCSLL, tipo = "decimal" },
                    ValorRetencaoISS = new { val = regraEntrada.ValorRetencaoISS, tipo = "decimal" },
                    ValorRetencaoIR = new { val = regraEntrada.ValorRetencaoIR, tipo = "decimal" },
                    ValorRetencaoOutras = new { val = regraEntrada.ValorRetencaoOutras, tipo = "decimal" },
                    Codigo = Guid.NewGuid().ToString(),
                    OrdemServico = new { Codigo = 0, Descricao = string.Empty },
                    OrdemCompraMercadoria = new { Codigo = 0, Descricao = string.Empty },
                    Equipamento = new { Codigo = equipamento?.Codigo ?? 0, Descricao = equipamento?.Descricao ?? "" },
                    Horimetro = veiculoOBS != null && veiculoOBS.TipoVeiculo == "0" ? string.Empty : !string.IsNullOrWhiteSpace(horimetroObservacao) ? horimetroObservacao : equipamento != null ? KMObs : string.Empty,
                    DataAbastecimento = string.Empty,
                    RegraEntradaDocumento = new
                    {
                        Codigo = regraEntradaDocumentoItem != null ? regraEntradaDocumentoItem.Codigo : 0,
                        Descricao = regraEntradaDocumentoItem != null ? regraEntradaDocumentoItem.Descricao : string.Empty
                    },
                    CFOP = new
                    {
                        Codigo = cfopNota != null ? cfopNota.Codigo : 0,
                        Descricao = cfopNota != null ? cfopNota.CFOPComExtensao : string.Empty
                    },
                    NaturezaOperacao = new
                    {
                        Codigo = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Codigo : 0,
                        Descricao = naturezaDaOperacaoNota != null ? naturezaDaOperacaoNota.Descricao : string.Empty
                    },
                    TipoMovimento = new
                    {
                        Codigo = tipoMovimentoNota != null ? tipoMovimentoNota.Codigo : 0,
                        Descricao = tipoMovimentoNota != null ? tipoMovimentoNota.Descricao : string.Empty
                    },
                    Veiculo = new
                    {
                        Codigo = veiculoOBS != null ? veiculoOBS.Codigo : 0,
                        Descricao = veiculoOBS != null ? veiculoOBS.Placa : string.Empty
                    },
                    KMAbastecimento = (equipamento == null || (equipamento?.UtilizaTanqueCompartilhado ?? false)) ? KMObs : "",
                    CodigoProdutoFornecedor = (string)produto.GetType().GetProperty("CodigoProdutoFornecedor").GetValue(produto, null),
                    DescricaoProdutoFornecedor = (string)produto.GetType().GetProperty("DescricaoProdutoFornecedor").GetValue(produto, null),
                    NCMProdutoFornecedor = (string)produto.GetType().GetProperty("NCMProdutoFornecedor").GetValue(produto, null),
                    CESTProdutoFornecedor = (string)produto.GetType().GetProperty("CESTProdutoFornecedor").GetValue(produto, null),
                    CalculoCustoProduto = (string)produto.GetType().GetProperty("CalculoCustoProduto").GetValue(produto, null),
                    CodigoBarrasEAN = (string)produto.GetType().GetProperty("CodigoBarrasEAN").GetValue(produto, null),
                    Produto = new
                    {
                        Codigo = (int)produto.GetType().GetProperty("Codigo").GetValue(produto, null),
                        Descricao = (string)produto.GetType().GetProperty("Descricao").GetValue(produto, null)
                    },
                    UnidadeMedida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)produto.GetType().GetProperty("UnidadeMedida").GetValue(produto, null),
                    NCM = (string)produto.GetType().GetProperty("CodigoNCM").GetValue(produto, null),
                    SiglaUnidadeMedida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla((Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)produto.GetType().GetProperty("UnidadeMedida").GetValue(produto, null)),
                    CSTICMS = regraEntrada.CSTICMS.Length > 2 ? regraEntrada.CSTICMS : codigoCstIcmsComOrigem,
                    CSTIPI = regraEntrada.CSTIPI,
                    CSTPIS = regraEntrada.CSTPIS,
                    CSTCOFINS = regraEntrada.CSTCOFINS,
                    Desconto = new { val = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m, tipo = "decimal" },
                    ValorSeguro = new { val = det.prod.vSeg != null ? decimal.Parse(det.prod.vSeg, cultura) : 0m, tipo = "decimal" },
                    Quantidade = new { val = quantidade, tipo = "decimal", configDecimal = new { precision = 4 } },
                    Sequencial = !string.IsNullOrEmpty(det.nItem) ? int.Parse(det.nItem) : 0,
                    PercentualReducaoBaseCalculoIPI = new { val = regraEntrada.PercentualReducaoIPI, tipo = "decimal" },
                    PercentualReducaoBaseCalculoPIS = new { val = regraEntrada.PercentualReducaoPIS, tipo = "decimal" },
                    PercentualReducaoBaseCalculoCOFINS = new { val = regraEntrada.PercentualReducaoCOFINS, tipo = "decimal" },
                    ValorCOFINS = new { val = regraEntrada.ValorCOFINS, tipo = "decimal" },
                    ValorCreditoPresumido = new { val = regraEntrada.ValorCreditoPresumido, tipo = "decimal" },
                    ValorDiferencial = new { val = regraEntrada.ValorDiferencial, tipo = "decimal" },
                    ValorFrete = new { val = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m, tipo = "decimal" },
                    ValorICMS = new { val = regraEntrada.ValorICMS, tipo = "decimal" },
                    ValorICMSST = new { val = 0m, tipo = "decimal" },
                    ValorIPI = new { val = regraEntrada.ValorIPICFOP, tipo = "decimal" },
                    ValorOutrasDespesas = new { val = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m, tipo = "decimal" },
                    ValorPIS = new { val = regraEntrada.ValorPIS, tipo = "decimal" },
                    ValorTotal = new { val = valorTotal, tipo = "decimal" },
                    ValorUnitario = new { val = valorUnitarioComercial > 0m ? valorUnitarioComercial : valorUnitario, tipo = "decimal", configDecimal = new { precision = 4 } },
                    ValorCustoUnitario = new { val = (((valorTotal) + regraEntrada.ValorDiferencial + regraEntrada.ValorIPICFOP + valorFrete + valorSeguro + outrasDespesas - desconto) / (quantidade > 0 ? quantidade : 1)), tipo = "decimal" },
                    ValorCustoTotal = new { val = ((valorTotal) + regraEntrada.ValorDiferencial + regraEntrada.ValorIPICFOP + valorFrete + valorSeguro + outrasDespesas - desconto), tipo = "decimal" },
                    NumeroFogoInicial = string.Empty,
                    TipoAquisicao = string.Empty,
                    CstIcmsFornecedor = codigoCstIcmsComOrigem ?? string.Empty,
                    BaseCalculoICMSFornecedor = new { val = baseCalculoICMS, tipo = "decimal", configDecimal = new { precision = 4 } },
                    VidaAtual = string.Empty,
                    Almoxarifado = new { Codigo = 0, Descricao = string.Empty },
                    ProdutoVinculado = new { Codigo = 0, Descricao = string.Empty },
                    QuantidadeProdutoVinculado = new { val = 0.0, tipo = "decimal", configDecimal = new { precision = 4 } },
                    LocalArmazenamento = new { Codigo = 0, Descricao = string.Empty },
                    ObservacaoItem = string.Empty,
                    UnidadeMedidaFornecedor = det.prod.uCom != null ? det.prod.uCom : string.Empty,
                    QuantidadeFornecedor = new { val = quantidadeComercial, tipo = "decimal", configDecimal = new { precision = 4 } },
                    ValorUnitarioFornecedor = new { val = valorUnitarioComercial, tipo = "decimal", configDecimal = new { precision = 4 } },
                    AliquotaICMSFornecedor = new { val = regraEntrada.AliquotaICMS, tipo = "decimal", configDecimal = new { precision = 4 } },
                    ValorICMSFornecedor = new { val = regraEntrada.ValorICMS, tipo = "decimal" },
                    CfopFornecedor = !string.IsNullOrWhiteSpace(det.prod.CFOP) ? det.prod.CFOP : null,
                    CentroResultado = new
                    {
                        Codigo = veiculoOBS?.CentroResultado?.Codigo ?? 0,
                        Descricao = veiculoOBS?.CentroResultado?.Descricao ?? string.Empty
                    },
                    OrigemMercadoria = icms?.GetType().GetProperty("OrigemMercadoria")?.GetValue(icms, null) ?? 0,
                    EncerrarOrdemServico = false
                };

                totalBaseCalculoICMS += regraEntrada.BaseICMS;
                valorTotalCOFINS += regraEntrada.ValorCOFINS;
                valorTotalICMS += regraEntrada.ValorICMS;
                valorTotalIPI += regraEntrada.ValorIPICFOP;
                valorTotalPIS += regraEntrada.ValorPIS;
                valorTotalCreditoPresumido += regraEntrada.ValorCreditoPresumido;
                valorTotalDiferencial += regraEntrada.ValorDiferencial;
                valorTotalCusto = ((valorTotal) + regraEntrada.ValorDiferencial + regraEntrada.ValorIPICFOP + valorFrete + valorSeguro + outrasDespesas - desconto);

                itens.Add(item);
            }

            return itens;
        }

        private List<object> ObterDuplicatas(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, System.Globalization.CultureInfo cultura, Dominio.Entidades.Cliente emitente, DateTime dataEmissao, decimal ValorTotal, Repositorio.UnitOfWork unitOfWork)
        {
            List<object> duplicatas = new List<object>();
            Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento repGrupoPessoasVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = emitente?.GrupoPessoas ?? null;

            bool usaGrupoPessoas = false;
            bool ignorarDuplicataRecebidaXMLNotaEntrada = (grupoPessoas?.IgnorarDuplicataRecebidaXMLNotaEntrada ?? false) || (emitente?.IgnorarDuplicataRecebidaXMLNotaEntrada ?? false);
            bool gerarDuplicataNotaEntrada = false;
            int parcelasDuplicataNotaEntrada = 0;
            string intervaloDiasDuplicataNotaEntrada = string.Empty;
            int diaPadraoDuplicataNotaEntrada = 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo = emitente?.FormaTituloFornecedor ?? grupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;

            if (emitente != null && emitente.GerarDuplicataNotaEntrada)
            {
                gerarDuplicataNotaEntrada = emitente.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = emitente.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = emitente.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = emitente.DiaPadraoDuplicataNotaEntrada;
            }
            else if (grupoPessoas != null && grupoPessoas.GerarDuplicataNotaEntrada)
            {
                usaGrupoPessoas = true;
                gerarDuplicataNotaEntrada = grupoPessoas.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = grupoPessoas.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = grupoPessoas.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = grupoPessoas.DiaPadraoDuplicataNotaEntrada;
            }

            if (nfe.NFe.infNFe.cobr != null && nfe.NFe.infNFe.cobr.dup != null && !ignorarDuplicataRecebidaXMLNotaEntrada)
            {
                for (var i = 0; i < nfe.NFe.infNFe.cobr.dup.Count(); i++)
                {
                    duplicatas.Add(new
                    {
                        Sequencia = i + 1,
                        Codigo = Guid.NewGuid().ToString(),
                        DataVencimento = nfe.NFe.infNFe.cobr.dup[i].dVenc != null ? DateTime.ParseExact(nfe.NFe.infNFe.cobr.dup[i].dVenc, "yyyy-MM-dd", null).ToString("dd/MM/yyyy") : string.Empty,
                        Numero = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.cobr.dup[i].nDup) ? (nfe.NFe.infNFe.ide.nNF + "/" + (i + 1)) : nfe.NFe.infNFe.cobr.dup[i].nDup,
                        Valor = new { val = decimal.Parse(nfe.NFe.infNFe.cobr.dup[i].vDup, cultura), tipo = "decimal" },
                        FormaTitulo = formaTitulo,
                        NumeroBoleto = string.Empty,
                        Portador = new { Codigo = 0, Descricao = string.Empty },
                        Observacao = string.Empty
                    });
                }
            }
            else if (gerarDuplicataNotaEntrada && parcelasDuplicataNotaEntrada > 0)
            {
                bool permiteMultiplosVencimentos;
                if (usaGrupoPessoas)
                    permiteMultiplosVencimentos = grupoPessoas.PermitirMultiplosVencimentos;
                else
                    permiteMultiplosVencimentos = emitente.Modalidades?.Where(f => f.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)?.FirstOrDefault()?.ModalidadesFornecedores?.FirstOrDefault()?.PermitirMultiplosVencimentos ?? false;

                if (permiteMultiplosVencimentos)
                {
                    int diaEmissao = dataEmissao.Date.Day;
                    int diaVencimento = 0;

                    if (usaGrupoPessoas)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento = repGrupoPessoasVencimento.BuscarDiaVencimento(grupoPessoas.Codigo, diaEmissao);
                        diaVencimento = vencimento?.Vencimento ?? 0;
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = repVencimento.BuscarDiaVencimento(emitente.CPF_CNPJ, diaEmissao);
                        diaVencimento = vencimento?.Vencimento ?? 0;
                    }

                    if (diaVencimento > 0)
                    {
                        DateTime novaData = ProximaDataTabelaVencimento(dataEmissao, diaVencimento);

                        duplicatas.Add(new
                        {
                            Sequencia = 1,
                            Codigo = Guid.NewGuid().ToString(),
                            DataVencimento = novaData.ToString("dd/MM/yyyy"),
                            Numero = (nfe.NFe.infNFe.ide.nNF + "/1"),
                            Valor = new { val = ValorTotal, tipo = "decimal" },
                            FormaTitulo = formaTitulo,
                            NumeroBoleto = string.Empty,
                            Portador = new { Codigo = 0, Descricao = string.Empty },
                            Observacao = string.Empty
                        });
                    }
                }
                else
                {
                    int quantidadeParcelas = parcelasDuplicataNotaEntrada;
                    decimal valorTotal = ValorTotal;
                    decimal valorParcela = Math.Round((valorTotal / quantidadeParcelas), 2);
                    decimal valorDiferenca = valorTotal - Math.Round((valorParcela * quantidadeParcelas), 2);
                    string[] arrayDias = null;

                    var x = intervaloDiasDuplicataNotaEntrada;
                    if (x.IndexOf(".") >= 0)
                    {
                        arrayDias = x.Split('.');
                        if (arrayDias.Length != quantidadeParcelas)
                        {
                            return null;
                        }
                        for (var i = 0; i < arrayDias.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(arrayDias[i]) || !(int.Parse(arrayDias[i]) > 0))
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        arrayDias = new string[1];
                        arrayDias[0] = x;
                        if (string.IsNullOrWhiteSpace(arrayDias[0]) || !(int.Parse(arrayDias[0]) > 0))
                        {
                            return null;
                        }
                    }
                    var dataVencimento = dataEmissao;

                    for (var i = 0; i < quantidadeParcelas; i++)
                    {
                        decimal valor = 0;
                        if (i == 0)
                            valor = Math.Round((valorParcela + valorDiferenca), 2);
                        else
                            valor = Math.Round(valorParcela, 2);

                        if (arrayDias.Length > 1)
                            dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[i]));
                        else
                            dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[0]));

                        DateTime novaData = dataVencimento;
                        if (i == 0 && diaPadraoDuplicataNotaEntrada > 0 && diaPadraoDuplicataNotaEntrada <= 31)
                        {
                            try
                            {
                                if (dataVencimento.Day > diaPadraoDuplicataNotaEntrada)
                                    dataVencimento = dataVencimento.AddMonths(1);

                                novaData = new DateTime(dataVencimento.Year, dataVencimento.Month, diaPadraoDuplicataNotaEntrada);
                            }
                            catch
                            {
                                novaData = dataVencimento;
                            }
                        }
                        dataVencimento = novaData;

                        duplicatas.Add(new
                        {
                            Sequencia = i + 1,
                            Codigo = Guid.NewGuid().ToString(),
                            DataVencimento = dataVencimento.ToString("dd/MM/yyyy"),
                            Numero = (nfe.NFe.infNFe.ide.nNF + "/" + (i + 1)),
                            Valor = new { val = valor, tipo = "decimal" },
                            FormaTitulo = formaTitulo,
                            NumeroBoleto = string.Empty,
                            Portador = new { Codigo = 0, Descricao = string.Empty },
                            Observacao = string.Empty
                        });
                    }
                }
            }

            return duplicatas;
        }

        private List<object> ObterDuplicatas(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, System.Globalization.CultureInfo cultura, Dominio.Entidades.Cliente emitente, DateTime dataEmissao, decimal ValorTotal, Repositorio.UnitOfWork unitOfWork)
        {
            List<object> duplicatas = new List<object>();
            Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento repGrupoPessoasVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento(unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = emitente?.GrupoPessoas ?? null;

            bool usaGrupoPessoas = false;
            bool ignorarDuplicataRecebidaXMLNotaEntrada = (grupoPessoas?.IgnorarDuplicataRecebidaXMLNotaEntrada ?? false) || (emitente?.IgnorarDuplicataRecebidaXMLNotaEntrada ?? false);
            bool gerarDuplicataNotaEntrada = false;
            int parcelasDuplicataNotaEntrada = 0;
            string intervaloDiasDuplicataNotaEntrada = string.Empty;
            int diaPadraoDuplicataNotaEntrada = 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo = emitente?.FormaTituloFornecedor ?? grupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;

            if (emitente != null && emitente.GerarDuplicataNotaEntrada)
            {
                gerarDuplicataNotaEntrada = emitente.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = emitente.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = emitente.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = emitente.DiaPadraoDuplicataNotaEntrada;
            }
            else if (grupoPessoas != null && grupoPessoas.GerarDuplicataNotaEntrada)
            {
                usaGrupoPessoas = true;
                gerarDuplicataNotaEntrada = grupoPessoas.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = grupoPessoas.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = grupoPessoas.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = grupoPessoas.DiaPadraoDuplicataNotaEntrada;
            }

            if (nfe.NFe.infNFe.cobr != null && nfe.NFe.infNFe.cobr.dup != null && !ignorarDuplicataRecebidaXMLNotaEntrada)
            {
                for (var i = 0; i < nfe.NFe.infNFe.cobr.dup.Count(); i++)
                {
                    duplicatas.Add(new
                    {
                        Sequencia = i + 1,
                        Codigo = Guid.NewGuid().ToString(),
                        DataVencimento = nfe.NFe.infNFe.cobr.dup[i].dVenc != null ? DateTime.ParseExact(nfe.NFe.infNFe.cobr.dup[i].dVenc, "yyyy-MM-dd", null).ToString("dd/MM/yyyy") : string.Empty,
                        Numero = string.IsNullOrWhiteSpace(nfe.NFe.infNFe.cobr.dup[i].nDup) ? (nfe.NFe.infNFe.ide.nNF + "/" + (i + 1)) : nfe.NFe.infNFe.cobr.dup[i].nDup,
                        Valor = new { val = decimal.Parse(nfe.NFe.infNFe.cobr.dup[i].vDup, cultura), tipo = "decimal" },
                        FormaTitulo = formaTitulo,
                        NumeroBoleto = string.Empty,
                        Portador = new { Codigo = 0, Descricao = string.Empty },
                        Observacao = string.Empty
                    });
                }
            }
            else if (gerarDuplicataNotaEntrada && parcelasDuplicataNotaEntrada > 0)
            {
                bool permiteMultiplosVencimentos;
                if (usaGrupoPessoas)
                    permiteMultiplosVencimentos = grupoPessoas.PermitirMultiplosVencimentos;
                else
                    permiteMultiplosVencimentos = emitente.Modalidades?.Where(f => f.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)?.FirstOrDefault()?.ModalidadesFornecedores?.FirstOrDefault()?.PermitirMultiplosVencimentos ?? false;

                if (permiteMultiplosVencimentos)
                {
                    int diaEmissao = dataEmissao.Date.Day;
                    int diaVencimento = 0;

                    if (usaGrupoPessoas)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento = repGrupoPessoasVencimento.BuscarDiaVencimento(grupoPessoas.Codigo, diaEmissao);
                        diaVencimento = vencimento?.Vencimento ?? 0;
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = repVencimento.BuscarDiaVencimento(emitente.CPF_CNPJ, diaEmissao);
                        diaVencimento = vencimento?.Vencimento ?? 0;
                    }

                    if (diaVencimento > 0)
                    {
                        DateTime novaData = ProximaDataTabelaVencimento(dataEmissao, diaVencimento);

                        duplicatas.Add(new
                        {
                            Sequencia = 1,
                            Codigo = Guid.NewGuid().ToString(),
                            DataVencimento = novaData.ToString("dd/MM/yyyy"),
                            Numero = (nfe.NFe.infNFe.ide.nNF + "/1"),
                            Valor = new { val = ValorTotal, tipo = "decimal" },
                            FormaTitulo = formaTitulo,
                            NumeroBoleto = string.Empty,
                            Portador = new { Codigo = 0, Descricao = string.Empty },
                            Observacao = string.Empty
                        });
                    }
                }
                else
                {
                    int quantidadeParcelas = parcelasDuplicataNotaEntrada;
                    decimal valorTotal = ValorTotal;
                    decimal valorParcela = Math.Round((valorTotal / quantidadeParcelas), 2);
                    decimal valorDiferenca = valorTotal - Math.Round((valorParcela * quantidadeParcelas), 2);
                    string[] arrayDias = null;

                    var x = intervaloDiasDuplicataNotaEntrada;
                    if (x.IndexOf(".") >= 0)
                    {
                        arrayDias = x.Split('.');
                        if (arrayDias.Length != quantidadeParcelas)
                        {
                            return null;
                        }
                        for (var i = 0; i < arrayDias.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(arrayDias[i]) || !(int.Parse(arrayDias[i]) > 0))
                            {
                                arrayDias[i] = "0";
                                //return null;
                            }
                        }
                    }
                    else
                    {
                        arrayDias = new string[1];
                        arrayDias[0] = x;
                        if (string.IsNullOrWhiteSpace(arrayDias[0]) || !(int.Parse(arrayDias[0]) > 0))
                        {
                            arrayDias[0] = "0";
                            //return null;
                        }
                    }
                    var dataVencimento = dataEmissao;

                    for (var i = 0; i < quantidadeParcelas; i++)
                    {
                        decimal valor = 0;
                        if (i == 0)
                            valor = Math.Round((valorParcela + valorDiferenca), 2);
                        else
                            valor = Math.Round(valorParcela, 2);

                        if (arrayDias.Length > 1)
                            dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[i]));
                        else
                            dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[0]));

                        DateTime novaData = dataVencimento;
                        if (i == 0 && diaPadraoDuplicataNotaEntrada > 0 && diaPadraoDuplicataNotaEntrada <= 31)
                        {
                            try
                            {
                                if (dataVencimento.Day > diaPadraoDuplicataNotaEntrada)
                                    dataVencimento = dataVencimento.AddMonths(1);

                                novaData = new DateTime(dataVencimento.Year, dataVencimento.Month, diaPadraoDuplicataNotaEntrada);
                            }
                            catch
                            {
                                novaData = dataVencimento;
                            }
                        }
                        dataVencimento = novaData;

                        duplicatas.Add(new
                        {
                            Sequencia = i + 1,
                            Codigo = Guid.NewGuid().ToString(),
                            DataVencimento = dataVencimento.ToString("dd/MM/yyyy"),
                            Numero = (nfe.NFe.infNFe.ide.nNF + "/" + (i + 1)),
                            Valor = new { val = valor, tipo = "decimal" },
                            FormaTitulo = formaTitulo,
                            NumeroBoleto = string.Empty,
                            Portador = new { Codigo = 0, Descricao = string.Empty },
                            Observacao = string.Empty
                        });
                    }
                }
            }

            return duplicatas;
        }

        private object ObterICMS(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var icms = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMS) select (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMS)obj).FirstOrDefault();

                if (icms != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoICMS = icms.Item.GetType();

                    if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00 impICMS00 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS00.pICMS != null ? decimal.Parse(impICMS00.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS00.vBC != null ? decimal.Parse(impICMS00.vBC, cultura) : 0m,
                            ValorICMS = impICMS00.vICMS != null ? decimal.Parse(impICMS00.vICMS, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS00.CST),
                            OrigemMercadoria = (int)impICMS00.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10 impICMS10 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS10.pICMS != null ? decimal.Parse(impICMS10.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS10.vBC != null ? decimal.Parse(impICMS10.vBC, cultura) : 0m,
                            ValorICMS = impICMS10.vICMS != null ? decimal.Parse(impICMS10.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMS10.vBCST != null ? decimal.Parse(impICMS10.vBCST, cultura) : 0m,
                            ValorICMSST = impICMS10.vICMSST != null ? decimal.Parse(impICMS10.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS10.CST),
                            OrigemMercadoria = (int)impICMS10.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20 impICMS20 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS20.pICMS != null ? decimal.Parse(impICMS20.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS20.vBC != null ? decimal.Parse(impICMS20.vBC, cultura) : 0m,
                            ValorICMS = impICMS20.vICMS != null ? decimal.Parse(impICMS20.vICMS, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS20.CST),
                            OrigemMercadoria = (int)impICMS20.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30 impICMS30 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = impICMS30.vBCST != null ? decimal.Parse(impICMS30.vBCST, cultura) : 0m,
                            ValorICMSST = impICMS30.vICMSST != null ? decimal.Parse(impICMS30.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS30.CST),
                            OrigemMercadoria = (int)impICMS30.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40 impICMS40 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS40.CST),
                            OrigemMercadoria = (int)impICMS40.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51 impICMS51 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS51.pICMS != null ? decimal.Parse(impICMS51.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS51.vBC != null ? decimal.Parse(impICMS51.vBC, cultura) : 0m,
                            ValorICMS = impICMS51.vICMS != null ? decimal.Parse(impICMS51.vICMS, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS51.CST),
                            OrigemMercadoria = (int)impICMS51.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60 impICMS60 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS60.CST),
                            OrigemMercadoria = (int)impICMS60.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70 impICMS70 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS70.pICMS != null ? decimal.Parse(impICMS70.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS70.vBC != null ? decimal.Parse(impICMS70.vBC, cultura) : 0m,
                            ValorICMS = impICMS70.vICMS != null ? decimal.Parse(impICMS70.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMS70.vBCST != null ? decimal.Parse(impICMS70.vBCST, cultura) : 0m,
                            ValorICMSST = impICMS70.vICMSST != null ? decimal.Parse(impICMS70.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS70.CST),
                            OrigemMercadoria = (int)impICMS70.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90 impICMS90 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS90.pICMS != null ? decimal.Parse(impICMS90.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS90.vBC != null ? decimal.Parse(impICMS90.vBC, cultura) : 0m,
                            ValorICMS = impICMS90.vICMS != null ? decimal.Parse(impICMS90.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMS90.vBCST != null ? decimal.Parse(impICMS90.vBCST, cultura) : 0m,
                            ValorICMSST = impICMS90.vICMSST != null ? decimal.Parse(impICMS90.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS90.CST),
                            OrigemMercadoria = (int)impICMS90.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart impICMSPart = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMSPart.pICMS != null ? decimal.Parse(impICMSPart.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMSPart.vBC != null ? decimal.Parse(impICMSPart.vBC, cultura) : 0m,
                            ValorICMS = impICMSPart.vICMS != null ? decimal.Parse(impICMSPart.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMSPart.vBCST != null ? decimal.Parse(impICMSPart.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSPart.vICMSST != null ? decimal.Parse(impICMSPart.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMSPart.CST),
                            OrigemMercadoria = (int)impICMSPart.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST impICMSST = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMSST.CST),
                            OrigemMercadoria = (int)impICMSST.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101 impICMSSN101 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN101.CSOSN),
                            OrigemMercadoria = (int)impICMSSN101.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102 impICMSSN102 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN102.CSOSN),
                            OrigemMercadoria = (int)impICMSSN102.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201 impICMSSN201 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = impICMSSN201.vBCST != null ? decimal.Parse(impICMSSN201.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN201.vICMSST != null ? decimal.Parse(impICMSSN201.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN201.CSOSN),
                            OrigemMercadoria = (int)impICMSSN201.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202 impICMSSN202 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = impICMSSN202.vBCST != null ? decimal.Parse(impICMSSN202.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN202.vICMSST != null ? decimal.Parse(impICMSSN202.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN202.CSOSN),
                            OrigemMercadoria = (int)impICMSSN202.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500 impICMSSN500 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN500.CSOSN),
                            OrigemMercadoria = (int)impICMSSN500.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900 impICMSSN900 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMSSN900.pICMS != null ? decimal.Parse(impICMSSN900.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMSSN900.vBC != null ? decimal.Parse(impICMSSN900.vBC, cultura) : 0m,
                            ValorICMS = impICMSSN900.vICMS != null ? decimal.Parse(impICMSSN900.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMSSN900.vBCST != null ? decimal.Parse(impICMSSN900.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN900.vICMSST != null ? decimal.Parse(impICMSSN900.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN900.CSOSN),
                            OrigemMercadoria = (int)impICMSSN900.orig
                        };
                    }
                }
            }

            return null;
        }

        private object ObterICMS(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var icms = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMS) select (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMS)obj).FirstOrDefault();

                if (icms != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoICMS = icms.Item.GetType();

                    if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00 impICMS00 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS00.pICMS != null ? decimal.Parse(impICMS00.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS00.vBC != null ? decimal.Parse(impICMS00.vBC, cultura) : 0m,
                            ValorICMS = impICMS00.vICMS != null ? decimal.Parse(impICMS00.vICMS, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS00.CST),
                            OrigemMercadoria = (int)impICMS00.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10 impICMS10 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS10.pICMS != null ? decimal.Parse(impICMS10.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS10.vBC != null ? decimal.Parse(impICMS10.vBC, cultura) : 0m,
                            ValorICMS = impICMS10.vICMS != null ? decimal.Parse(impICMS10.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMS10.vBCST != null ? decimal.Parse(impICMS10.vBCST, cultura) : 0m,
                            ValorICMSST = impICMS10.vICMSST != null ? decimal.Parse(impICMS10.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS10.CST),
                            OrigemMercadoria = (int)impICMS10.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20 impICMS20 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS20.pICMS != null ? decimal.Parse(impICMS20.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS20.vBC != null ? decimal.Parse(impICMS20.vBC, cultura) : 0m,
                            ValorICMS = impICMS20.vICMS != null ? decimal.Parse(impICMS20.vICMS, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS20.CST),
                            OrigemMercadoria = (int)impICMS20.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30 impICMS30 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = impICMS30.vBCST != null ? decimal.Parse(impICMS30.vBCST, cultura) : 0m,
                            ValorICMSST = impICMS30.vICMSST != null ? decimal.Parse(impICMS30.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS30.CST),
                            OrigemMercadoria = (int)impICMS30.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40 impICMS40 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS40.CST),
                            OrigemMercadoria = (int)impICMS40.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51 impICMS51 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS51.pICMS != null ? decimal.Parse(impICMS51.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS51.vBC != null ? decimal.Parse(impICMS51.vBC, cultura) : 0m,
                            ValorICMS = impICMS51.vICMS != null ? decimal.Parse(impICMS51.vICMS, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS51.CST),
                            OrigemMercadoria = (int)impICMS51.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60 impICMS60 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS60.CST),
                            vBCSTRet = impICMS60.vBCSTRet != null ? decimal.Parse(impICMS60.vBCSTRet, cultura) : 0m,
                            vICMSSTRet = impICMS60.vICMSSTRet != null ? decimal.Parse(impICMS60.vICMSSTRet, cultura) : 0m,
                            OrigemMercadoria = (int)impICMS60.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS61))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS61 impICMS61 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS61)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMS61.CST),
                            BaseCalculoICMS = impICMS61.qBCMonoRet != null ? decimal.Parse(impICMS61.qBCMonoRet, cultura) : 0m,
                            AliquotaICMS = impICMS61.adRemICMSRet != null ? decimal.Parse(impICMS61.adRemICMSRet, cultura) : 0m,
                            ValorICMS = impICMS61.vICMSMonoRet != null ? decimal.Parse(impICMS61.vICMSMonoRet, cultura) : 0m,
                            OrigemMercadoria = (int)impICMS61.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70 impICMS70 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS70.pICMS != null ? decimal.Parse(impICMS70.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS70.vBC != null ? decimal.Parse(impICMS70.vBC, cultura) : 0m,
                            ValorICMS = impICMS70.vICMS != null ? decimal.Parse(impICMS70.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMS70.vBCST != null ? decimal.Parse(impICMS70.vBCST, cultura) : 0m,
                            ValorICMSST = impICMS70.vICMSST != null ? decimal.Parse(impICMS70.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS70.CST),
                            OrigemMercadoria = (int)impICMS70.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90 impICMS90 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMS90.pICMS != null ? decimal.Parse(impICMS90.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMS90.vBC != null ? decimal.Parse(impICMS90.vBC, cultura) : 0m,
                            ValorICMS = impICMS90.vICMS != null ? decimal.Parse(impICMS90.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMS90.vBCST != null ? decimal.Parse(impICMS90.vBCST, cultura) : 0m,
                            ValorICMSST = impICMS90.vICMSST != null ? decimal.Parse(impICMS90.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMS90.CST),
                            OrigemMercadoria = (int)impICMS90.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart impICMSPart = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMSPart.pICMS != null ? decimal.Parse(impICMSPart.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMSPart.vBC != null ? decimal.Parse(impICMSPart.vBC, cultura) : 0m,
                            ValorICMS = impICMSPart.vICMS != null ? decimal.Parse(impICMSPart.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMSPart.vBCST != null ? decimal.Parse(impICMSPart.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSPart.vICMSST != null ? decimal.Parse(impICMSPart.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:00}", (int)impICMSPart.CST),
                            OrigemMercadoria = (int)impICMSPart.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST impICMSST = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:00}", (int)impICMSST.CST),
                            BCSTDest = impICMSST.vBCSTDest != null ? decimal.Parse(impICMSST.vBCSTDest, cultura) : 0m,
                            ICMSSTDest = impICMSST.vICMSSTDest != null ? decimal.Parse(impICMSST.vICMSSTDest, cultura) : 0m,
                            vBCSTRet = impICMSST.vBCSTRet != null ? decimal.Parse(impICMSST.vBCSTRet, cultura) : 0m,
                            vICMSSTRet = impICMSST.vICMSSTRet != null ? decimal.Parse(impICMSST.vICMSSTRet, cultura) : 0m,
                            OrigemMercadoria = (int)impICMSST.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101 impICMSSN101 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN101.CSOSN),
                            OrigemMercadoria = (int)impICMSSN101.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102 impICMSSN102 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN102.CSOSN),
                            OrigemMercadoria = (int)impICMSSN102.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201 impICMSSN201 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = impICMSSN201.vBCST != null ? decimal.Parse(impICMSSN201.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN201.vICMSST != null ? decimal.Parse(impICMSSN201.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN201.CSOSN),
                            OrigemMercadoria = (int)impICMSSN201.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202 impICMSSN202 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202)icms.Item;

                        return new
                        {
                            BaseCalculoICMSST = impICMSSN202.vBCST != null ? decimal.Parse(impICMSSN202.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN202.vICMSST != null ? decimal.Parse(impICMSSN202.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN202.CSOSN),
                            OrigemMercadoria = (int)impICMSSN202.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500 impICMSSN500 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500)icms.Item;

                        return new
                        {
                            CST = string.Format("{0:000}", (int)impICMSSN500.CSOSN),
                            vBCSTRet = impICMSSN500.vBCSTRet != null ? decimal.Parse(impICMSSN500.vBCSTRet, cultura) : 0m,
                            vICMSSTRet = impICMSSN500.vICMSSTRet != null ? decimal.Parse(impICMSSN500.vICMSSTRet, cultura) : 0m,
                            OrigemMercadoria = (int)impICMSSN500.orig
                        };
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900 impICMSSN900 = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900)icms.Item;

                        return new
                        {
                            AliquotaICMS = impICMSSN900.pICMS != null ? decimal.Parse(impICMSSN900.pICMS, cultura) : 0m,
                            BaseCalculoICMS = impICMSSN900.vBC != null ? decimal.Parse(impICMSSN900.vBC, cultura) : 0m,
                            ValorICMS = impICMSSN900.vICMS != null ? decimal.Parse(impICMSSN900.vICMS, cultura) : 0m,
                            BaseCalculoICMSST = impICMSSN900.vBCST != null ? decimal.Parse(impICMSSN900.vBCST, cultura) : 0m,
                            ValorICMSST = impICMSSN900.vICMSST != null ? decimal.Parse(impICMSSN900.vICMSST, cultura) : 0m,
                            CST = string.Format("{0:000}", (int)impICMSSN900.CSOSN),
                            OrigemMercadoria = (int)impICMSSN900.orig
                        };
                    }
                }
            }

            return null;
        }

        private object ObterIPI(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var ipi = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TIpi) select (MultiSoftware.NFe.v310.NotaFiscal.TIpi)obj).FirstOrDefault();

                if (ipi != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoIPI = ipi.Item.GetType();

                    if (tipoIPI == typeof(MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib impIPITrib = (MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib)ipi.Item;

                        decimal baseCalculo = 0m;
                        decimal aliquota = 0m;

                        if (impIPITrib.ItemsElementName != null && impIPITrib.Items != null)
                        {
                            if (impIPITrib.ItemsElementName[0] == MultiSoftware.NFe.v310.NotaFiscal.ItemsChoiceType.vBC && impIPITrib.ItemsElementName[1] == MultiSoftware.NFe.v310.NotaFiscal.ItemsChoiceType.pIPI)
                            {
                                baseCalculo = decimal.Parse(impIPITrib.Items[0], cultura);
                                aliquota = decimal.Parse(impIPITrib.Items[1], cultura);
                            }
                        }

                        return new
                        {
                            BaseCalculoIPI = baseCalculo,
                            AliquotaIPI = aliquota,
                            ValorIPI = decimal.Parse(impIPITrib.vIPI, cultura),
                            CST = string.Format("{0:00}", (int)impIPITrib.CST)
                        };
                    }
                }
            }

            return null;
        }

        private object ObterIPI(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var ipi = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscal.TIpi) select (MultiSoftware.NFe.v400.NotaFiscal.TIpi)obj).FirstOrDefault();

                if (ipi != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoIPI = ipi.Item.GetType();

                    if (tipoIPI == typeof(MultiSoftware.NFe.v400.NotaFiscal.TIpiIPITrib))
                    {
                        MultiSoftware.NFe.v400.NotaFiscal.TIpiIPITrib impIPITrib = (MultiSoftware.NFe.v400.NotaFiscal.TIpiIPITrib)ipi.Item;

                        decimal baseCalculo = 0m;
                        decimal aliquota = 0m;

                        if (impIPITrib.ItemsElementName != null && impIPITrib.Items != null)
                        {
                            if (impIPITrib.ItemsElementName[0] == MultiSoftware.NFe.v400.NotaFiscal.ItemsChoiceType.vBC && impIPITrib.ItemsElementName[1] == MultiSoftware.NFe.v400.NotaFiscal.ItemsChoiceType.pIPI)
                            {
                                baseCalculo = decimal.Parse(impIPITrib.Items[0], cultura);
                                aliquota = decimal.Parse(impIPITrib.Items[1], cultura);
                            }
                        }

                        return new
                        {
                            BaseCalculoIPI = baseCalculo,
                            AliquotaIPI = aliquota,
                            ValorIPI = decimal.Parse(impIPITrib.vIPI, cultura),
                            CST = string.Format("{0:00}", (int)impIPITrib.CST)
                        };
                    }
                }
            }

            return null;
        }

        private object ObterPIS(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPIS infNFeDetImpostoPIS)
        {
            if (infNFeDetImpostoPIS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoPIS = infNFeDetImpostoPIS.Item.GetType();

                if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq impPISAliq = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISAliq.CST),
                        ValorPIS = decimal.Parse(impPISAliq.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr impPISOutr = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISOutr.CST),
                        ValorPIS = decimal.Parse(impPISOutr.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde impPISQtde = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISQtde.CST),
                        ValorPIS = decimal.Parse(impPISQtde.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT impPISNT = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISNT.CST),
                        ValorPIS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterPIS(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPIS infNFeDetImpostoPIS)
        {
            if (infNFeDetImpostoPIS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoPIS = infNFeDetImpostoPIS.Item.GetType();

                if (tipoPIS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq impPISAliq = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISAliq.CST),
                        ValorPIS = decimal.Parse(impPISAliq.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr impPISOutr = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISOutr.CST),
                        ValorPIS = decimal.Parse(impPISOutr.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde impPISQtde = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISQtde.CST),
                        ValorPIS = decimal.Parse(impPISQtde.vPIS, cultura)
                    };
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT impPISNT = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT)infNFeDetImpostoPIS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impPISNT.CST),
                        ValorPIS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterCOFINS(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINS infNFeDetImpostoCOFINS)
        {
            if (infNFeDetImpostoCOFINS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoCOFINS = infNFeDetImpostoCOFINS.Item.GetType();

                if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq impCOFINSAliq = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSAliq.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSAliq.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr impCOFINSOutr = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSOutr.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSOutr.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde impCOFINSQtde = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSQtde.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSQtde.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST impCOFINSST = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = "",
                        ValorCOFINS = decimal.Parse(impCOFINSST.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT impCOFINSNT = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSNT.CST),
                        ValorCOFINS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterCOFINS(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINS infNFeDetImpostoCOFINS)
        {
            if (infNFeDetImpostoCOFINS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoCOFINS = infNFeDetImpostoCOFINS.Item.GetType();

                if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq impCOFINSAliq = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSAliq.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSAliq.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr impCOFINSOutr = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSOutr.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSOutr.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde impCOFINSQtde = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSQtde.CST),
                        ValorCOFINS = decimal.Parse(impCOFINSQtde.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST impCOFINSST = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = "",
                        ValorCOFINS = decimal.Parse(impCOFINSST.vCOFINS, cultura)
                    };
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT))
                {
                    MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT impCOFINSNT = (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT)infNFeDetImpostoCOFINS.Item;

                    return new
                    {
                        CST = string.Format("{0:00}", (int)impCOFINSNT.CST),
                        ValorCOFINS = 0m
                    };
                }
            }

            return null;
        }

        private object ObterProduto(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente fornecedor, MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetProd prod, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeDeTrabalho);

            int codigoEmpresa = 0;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = empresa.Codigo;

            Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorProdutoEFornecedor(prod.cProd, fornecedor.CPF_CNPJ, codigoEmpresa);

            if (produtoFornecedor != null)
            {
                return new
                {
                    Codigo = produtoFornecedor.Produto.Codigo,
                    Descricao = produtoFornecedor.Produto.Descricao,
                    UnidadeMedida = produtoFornecedor.Produto.UnidadeDeMedida,
                    CodigoProdutoFornecedor = prod.cProd,
                    CodigoBarrasEAN = prod.cEAN,
                    DescricaoProdutoFornecedor = Utilidades.String.RemoveAllSpecialCharacters(prod.xProd),
                    NCMProdutoFornecedor = prod.NCM ?? string.Empty,
                    CESTProdutoFornecedor = prod.CEST ?? string.Empty,
                    CalculoCustoProduto = string.IsNullOrWhiteSpace(produtoFornecedor.Produto.CalculoCustoProduto) ? Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unidadeDeTrabalho) : produtoFornecedor.Produto.CalculoCustoProduto,
                    FatorConversao = produtoFornecedor.FatorConversao.ToString("n5"),
                    CodigoNCM = produtoFornecedor.Produto.CodigoNCM,
                };
            }
            else
            {
                return new
                {
                    Codigo = 0,
                    Descricao = "",
                    UnidadeMedida = ObterUnidadeMedida(prod, unidadeDeTrabalho, codigoEmpresa),
                    CodigoProdutoFornecedor = prod.cProd,
                    CodigoBarrasEAN = prod.cEAN,
                    DescricaoProdutoFornecedor = Utilidades.String.RemoveAllSpecialCharacters(prod.xProd),
                    NCMProdutoFornecedor = prod.NCM ?? string.Empty,
                    CESTProdutoFornecedor = prod.CEST ?? string.Empty,
                    CalculoCustoProduto = Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unidadeDeTrabalho),
                    FatorConversao = 0.ToString("n5"),
                    CodigoNCM = "",
                };
            }
        }

        private object ObterProduto(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente fornecedor, MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetProd prod, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeDeTrabalho);

            int codigoEmpresa = 0;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = empresa.Codigo;

            Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorProdutoEFornecedor(prod.cProd, fornecedor.CPF_CNPJ, codigoEmpresa);

            if (produtoFornecedor != null)
            {
                return new
                {
                    Codigo = produtoFornecedor.Produto.Codigo,
                    Descricao = produtoFornecedor.Produto.Descricao,
                    UnidadeMedida = produtoFornecedor.Produto.UnidadeDeMedida != null ? produtoFornecedor.Produto.UnidadeDeMedida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade,
                    CodigoProdutoFornecedor = prod.cProd,
                    CodigoBarrasEAN = prod.cEAN,
                    DescricaoProdutoFornecedor = Utilidades.String.RemoveAllSpecialCharacters(prod.xProd),
                    NCMProdutoFornecedor = prod.NCM ?? string.Empty,
                    CESTProdutoFornecedor = prod.CEST ?? string.Empty,
                    CalculoCustoProduto = string.IsNullOrWhiteSpace(produtoFornecedor.Produto.CalculoCustoProduto) ? Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unidadeDeTrabalho) : produtoFornecedor.Produto.CalculoCustoProduto,
                    FatorConversao = produtoFornecedor.FatorConversao.ToString("n5"),
                    CodigoNCM = produtoFornecedor.Produto.CodigoNCM,
                };
            }
            else
            {
                return new
                {
                    Codigo = 0,
                    Descricao = "",
                    UnidadeMedida = ObterUnidadeMedida(prod, unidadeDeTrabalho, codigoEmpresa),
                    CodigoProdutoFornecedor = prod.cProd,
                    CodigoBarrasEAN = prod.cEAN,
                    DescricaoProdutoFornecedor = Utilidades.String.RemoveAllSpecialCharacters(prod.xProd),
                    NCMProdutoFornecedor = prod.NCM ?? string.Empty,
                    CESTProdutoFornecedor = prod.CEST ?? string.Empty,
                    CalculoCustoProduto = Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unidadeDeTrabalho),
                    FatorConversao = 0.ToString("n5"),
                    CodigoNCM = "",
                };
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida ObterUnidadeMedida(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetProd prod, Repositorio.UnitOfWork unidadeDeTrabalho, int codigoEmpresa)
        {
            Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor unidadeMedidaFornecedor = repUnidadeMedidaFornecedor.BuscarPorDescricao(prod.uCom, codigoEmpresa);
            if (unidadeMedidaFornecedor != null)
                return unidadeMedidaFornecedor.UnidadeDeMedida;

            switch (prod.uCom.ToLower())
            {
                case "kg":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilograma;
                case "lt":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Litros;
                case "mmbtu":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MMBTU;
                case "m3":
                case "m³":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroCubico;
                case "to":
                case "ton":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tonelada;
                case "un":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade;
                case "ser":
                case "srv":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico;
                case "cx":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Caixa;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilograma;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida ObterUnidadeMedida(MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDetProd prod, Repositorio.UnitOfWork unidadeDeTrabalho, int codigoEmpresa)
        {
            Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor unidadeMedidaFornecedor = repUnidadeMedidaFornecedor.BuscarPorDescricao(prod.uCom, codigoEmpresa);
            if (unidadeMedidaFornecedor != null)
                return unidadeMedidaFornecedor.UnidadeDeMedida;

            switch (prod.uCom.ToLower())
            {
                case "kg":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilograma;
                case "lt":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Litros;
                case "mmbtu":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MMBTU;
                case "m3":
                case "m³":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroCubico;
                case "to":
                case "ton":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tonelada;
                case "un":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade;
                case "ser":
                case "srv":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico;
                case "cx":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Caixa;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilograma;
            }
        }

        private Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS SalvarDocumentoEntradaPorNFe(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuarioLogado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, bool naoControlarKM, bool possuiPermissaoGravarValorDiferente, out string retornoErro)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            retornoErro = string.Empty;

            Servicos.Embarcador.Financeiro.TituloAPagar svcTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unidadeDeTrabalho);

            bool todasInformacoesLancadas = false;
            bool naoInformouKMVeiculo = false;

            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorChave(nfe.protNFe.infProt.chNFe);

            if (documentoEntrada != null)
                return documentoEntrada;

            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("55");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("nfe");

            Servicos.NFe svcNFe = new Servicos.NFe(unidadeDeTrabalho);

            Dominio.Entidades.Empresa destinatario = repEmpresa.BuscarPorCNPJ(nfe.NFe.infNFe.dest.Item);
            if (empresa == null && destinatario != null)
                empresa = repEmpresa.BuscarPorCNPJ(destinatario.CNPJ_SemFormato);

            Dominio.Entidades.Cliente emitente = svcNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa.Codigo, unidadeDeTrabalho);

            bool controlarTransacao = true;

            if (unidadeDeTrabalho.IsActiveTransaction())
                controlarTransacao = false;

            if (controlarTransacao)
                unidadeDeTrabalho.Start();

            documentoEntrada = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS();
            Dominio.Entidades.Veiculo veiculoObs = null;

            string placaObservacao = "";
            string kmObservacao = "";
            string horimetroObservacao = "";
            int kmAbastecimento = 0;
            RetornaTagAbastecimento(out kmObservacao, out placaObservacao, out horimetroObservacao, nfe, emitente);
            if (!string.IsNullOrWhiteSpace(placaObservacao))
                veiculoObs = repVeiculo.BuscarPlaca(placaObservacao);
            int.TryParse(kmObservacao, out kmAbastecimento);

            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = null;
            if (veiculoObs != null)// && kmAbastecimento > 0)
            {
                if (veiculoObs.Equipamentos != null && veiculoObs.Equipamentos.Count > 0)
                {
                    if (veiculoObs.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true)?.Count() == 1)
                        equipamento = veiculoObs.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true).FirstOrDefault();
                }
            }

            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = null;
            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoItem = null;
            if (destinatario != null && emitente != null)
                regraEntradaDocumento = RetornaRegraEntrada(destinatario, emitente, "", unidadeDeTrabalho);
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;

            documentoEntrada.Chave = nfe.protNFe.infProt.chNFe;
            documentoEntrada.Fornecedor = emitente;
            documentoEntrada.Destinatario = destinatario;
            documentoEntrada.DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documentoEntrada.DataEntrada = dataEntrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada.DataLancamento ? DateTime.Now : documentoEntrada.DataEmissao; //DateTime.Now;//DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);//DateTime.Now;
            documentoEntrada.Especie = especie;
            documentoEntrada.IndicadorPagamento = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada)nfe.NFe.infNFe.ide.indPag;
            documentoEntrada.Modelo = modelo;
            documentoEntrada.Numero = int.Parse(nfe.NFe.infNFe.ide.nNF);
            documentoEntrada.Serie = nfe.NFe.infNFe.ide.serie;
            documentoEntrada.Veiculo = veiculoObs;
            documentoEntrada.Equipamento = equipamento;

            if (kmAbastecimento > 0 && (equipamento == null || (equipamento?.UtilizaTanqueCompartilhado ?? false)))
                documentoEntrada.KMAbastecimento = kmAbastecimento;
            if (!string.IsNullOrWhiteSpace(horimetroObservacao) && (documentoEntrada.Veiculo == null || documentoEntrada.Veiculo.TipoVeiculo != "0"))
            {
                int.TryParse(horimetroObservacao, out int horimetro);
                documentoEntrada.Horimetro = horimetro;
            }
            else if (equipamento != null && kmAbastecimento > 0 && (documentoEntrada.Veiculo == null || documentoEntrada.Veiculo.TipoVeiculo != "0"))
                documentoEntrada.Horimetro = kmAbastecimento;

            documentoEntrada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto;
            documentoEntrada.ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura);
            documentoEntrada.ValorBruto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura);
            documentoEntrada.ValorProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura);
            documentoEntrada.ValorTotalDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura);
            documentoEntrada.ValorTotalFrete = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vFrete, cultura);
            documentoEntrada.ValorTotalSeguro = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vSeg, cultura);
            documentoEntrada.ValorTotalOutrasDespesas = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura);

            documentoEntrada.NumeroLancamento = repDocumentoEntrada.BuscarUltimoNumeroLancamento() + 1;

            if (documentoEntrada.Destinatario == null)
                documentoEntrada.Destinatario = empresa;

            if (usuarioLogado != null)
                documentoEntrada.OperadorLancamentoDocumento = usuarioLogado;

            repDocumentoEntrada.Inserir(documentoEntrada);

            if (documentoEntrada.Veiculo != null && documentoEntrada.KMAbastecimento <= 0)
                naoInformouKMVeiculo = true;

            bool todasDuplicatasLancadas = false;
            SalvarDuplicatas(documentoEntrada, nfe.NFe.infNFe.cobr, cultura, unidadeDeTrabalho, out todasDuplicatasLancadas);

            bool todosItensLancados = false;
            string msgRetornoItens = "";
            dynamic dynItens = SalvarItens(documentoEntrada, nfe.NFe.infNFe.det, empresa, emitente, cultura, unidadeDeTrabalho, veiculoObs, kmAbastecimento, horimetroObservacao, regraEntradaDocumento, destinatario, out todosItensLancados, tipoServicoMultisoftware, out regraEntradaDocumentoItem, equipamento, out msgRetornoItens);
            retornoErro = msgRetornoItens;
            if (regraEntradaDocumentoItem != null && destinatario != null && emitente != null)
            {
                regraEntradaDocumento = regraEntradaDocumentoItem;
                if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumentoItem.CFOPDentro != null)
                    cfopNota = regraEntradaDocumentoItem.CFOPDentro;
                else if (regraEntradaDocumentoItem.CFOPFora != null)
                    cfopNota = regraEntradaDocumentoItem.CFOPFora;
                if (regraEntradaDocumentoItem.NaturezaOperacao != null)
                    naturezaDaOperacaoNota = regraEntradaDocumentoItem.NaturezaOperacao;
                if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                    tipoMovimentoNota = cfopNota.TipoMovimentoUso;
            }
            else if (regraEntradaDocumento != null && destinatario != null && emitente != null)
            {
                if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumento.CFOPDentro != null)
                    cfopNota = regraEntradaDocumento.CFOPDentro;
                else if (regraEntradaDocumento.CFOPFora != null)
                    cfopNota = regraEntradaDocumento.CFOPFora;
                if (regraEntradaDocumento.NaturezaOperacao != null)
                    naturezaDaOperacaoNota = regraEntradaDocumento.NaturezaOperacao;
                if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                    tipoMovimentoNota = cfopNota.TipoMovimentoUso;
            }
            if (cfopNota != null)
                documentoEntrada.CFOP = cfopNota;
            if (naturezaDaOperacaoNota != null)
                documentoEntrada.NaturezaOperacao = naturezaDaOperacaoNota;
            if (tipoMovimentoNota != null)
                documentoEntrada.TipoMovimento = tipoMovimentoNota;

            if (documentoEntrada.TipoMovimento != null && documentoEntrada.CFOP != null && documentoEntrada.NaturezaOperacao != null)
                todasInformacoesLancadas = true;

            documentoEntrada.BaseSTRetido = dynItens.totalBaseSTRetido;
            documentoEntrada.ValorSTRetido = dynItens.totalValorSTRetido;
            documentoEntrada.BaseCalculoICMS = dynItens.totalBaseCalculoICMS;
            documentoEntrada.ValorTotalCOFINS = dynItens.valorTotalCOFINS;
            documentoEntrada.ValorTotalICMS = dynItens.valorTotalICMS;
            documentoEntrada.ValorTotalIPI = dynItens.valorTotalIPI;
            documentoEntrada.ValorTotalPIS = dynItens.valorTotalPIS;
            documentoEntrada.ValorTotalCreditoPresumido = dynItens.valorTotalCreditoPresumido;
            documentoEntrada.ValorTotalDiferencial = dynItens.valorTotalDiferencial;
            documentoEntrada.ValorTotalCusto = dynItens.valorTotalCusto;
            documentoEntrada.ValorTotalRetencaoPIS = dynItens.valorTotalRetencaoPIS;
            documentoEntrada.ValorTotalRetencaoCOFINS = dynItens.valorTotalRetencaoCOFINS;
            documentoEntrada.ValorTotalRetencaoINSS = dynItens.valorTotalRetencaoINSS;
            documentoEntrada.ValorTotalRetencaoIPI = dynItens.valorTotalRetencaoIPI;
            documentoEntrada.ValorTotalRetencaoCSLL = dynItens.valorTotalRetencaoCSLL;
            documentoEntrada.ValorTotalRetencaoOutras = dynItens.valorTotalRetencaoOutras;
            documentoEntrada.ValorTotalRetencaoIR = dynItens.valorTotalRetencaoIR;
            documentoEntrada.ValorTotalRetencaoISS = dynItens.valorTotalRetencaoISS;

            if (regraEntradaDocumento != null && regraEntradaDocumento.FinalizarFaturarNotaAutomaticamente && todasInformacoesLancadas && todasDuplicatasLancadas && todosItensLancados && !naoInformouKMVeiculo && (documentoEntrada.Veiculo == null || (documentoEntrada.Veiculo.Status == "A" && documentoEntrada.Veiculo.Tipo == "P")))
            {
                documentoEntrada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado;
                documentoEntrada.DocumentoFinalizadoAutomaticamente = true;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = usuarioLogado.Empresa.TipoAmbiente;

                string erro = "";

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

                if (!Servicos.Embarcador.Frota.OrdemServico.VincularDocumentoEntradaAOrdemServico(documentoEntrada, out erro, unidadeDeTrabalho, usuarioLogado, tipoServicoMultisoftware, Auditado, configuracaoTMS))
                {
                    retornoErro = erro;
                    Servicos.Log.TratarErro(erro);
                    return null;
                }

                if (!svcTituloAPagar.AtualizarTitulos(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, dataCompetencia))
                {
                    retornoErro = erro;
                    Servicos.Log.TratarErro(erro);
                    return null;
                }

                if (!VerificarCadastroItens(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, usuarioLogado, configuracaoTMS, configuracaoDocumentoEntrada))
                {
                    retornoErro = erro;
                    Servicos.Log.TratarErro(erro);
                    return null;
                }

                if (!AtualizarCusto(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, Auditado, configuracaoTMS))
                {
                    retornoErro = erro;
                    Servicos.Log.TratarErro(erro);
                    return null;
                }

                if (!MovimentarEstoque(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, dataCompetencia))
                {
                    retornoErro = erro;
                    Servicos.Log.TratarErro(erro);
                    return null;
                }

                if (!GerarAbastecimentos(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, naoControlarKM, possuiPermissaoGravarValorDiferente, false))
                {
                    retornoErro = erro;
                    Servicos.Log.TratarErro(erro);
                    return null;
                }

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    if (!GerarMovimentoFinanceiroEmissaoItens(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, dataCompetencia))
                    {
                        retornoErro = erro;
                        Servicos.Log.TratarErro(erro);
                        return null;
                    }
                }
            }

            if (controlarTransacao)
                unidadeDeTrabalho.CommitChanges();

            return documentoEntrada;
        }

        private Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS SalvarDocumentoEntradaPorNFe(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Empresa empresa, MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuarioLogado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada dataEntrada, bool naoControlarKM, bool possuiPermissaoGravarValorDiferente, out string retornoErro, bool lancarDocumentoEntradaAbertoSeKMEstiverErrado)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            retornoErro = string.Empty;

            Servicos.Embarcador.Financeiro.TituloAPagar svcTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.EspecieDocumentoFiscal repEspecie = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unidadeDeTrabalho);

            bool todasInformacoesLancadas = false;
            bool naoInformouKMVeiculo = false;

            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorChave(nfe.protNFe.infProt.chNFe);

            if (documentoEntrada != null)
                return documentoEntrada;

            Dominio.Entidades.ModeloDocumentoFiscal modelo = repModelo.BuscarPorModelo("55");
            Dominio.Entidades.EspecieDocumentoFiscal especie = repEspecie.BuscarPorSigla("nfe");

            Servicos.NFe svcNFe = new Servicos.NFe(unidadeDeTrabalho);
            Servicos.Embarcador.Patrimonio.Bem svcBem = new Servicos.Embarcador.Patrimonio.Bem(unidadeDeTrabalho);

            Dominio.Entidades.Empresa destinatario = repEmpresa.BuscarPorCNPJ(nfe.NFe.infNFe.dest.Item);
            if (empresa == null && destinatario != null)
                empresa = repEmpresa.BuscarPorCNPJ(destinatario.CNPJ_SemFormato);

            Dominio.Entidades.Cliente emitente = svcNFe.ObterEmitente(nfe.NFe.infNFe.emit, empresa?.Codigo ?? 0, unidadeDeTrabalho);

            bool controlarTransacao = true;

            if (unidadeDeTrabalho.IsActiveTransaction())
                controlarTransacao = false;

            if (controlarTransacao)
                unidadeDeTrabalho.Start();

            documentoEntrada = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS();
            Dominio.Entidades.Veiculo veiculoObs = null;

            string placaObservacao = "";
            string kmObservacao = "";
            string horimetroObservacao = "";
            string chassiObservacao = "";
            int kmAbastecimento = 0;
            RetornaTagAbastecimento(out kmObservacao, out placaObservacao, out horimetroObservacao, out chassiObservacao, nfe, emitente);
            if (!string.IsNullOrWhiteSpace(placaObservacao))
                veiculoObs = repVeiculo.BuscarPlaca(placaObservacao);
            else if (!string.IsNullOrWhiteSpace(chassiObservacao))
                veiculoObs = repVeiculo.BuscarPorChassi(chassiObservacao);
            int.TryParse(kmObservacao, out kmAbastecimento);

            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = null;
            if (veiculoObs != null)// && kmAbastecimento > 0)
            {
                if (veiculoObs.Equipamentos != null && veiculoObs.Equipamentos.Count > 0)
                {
                    if (veiculoObs.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true)?.Count() == 1)
                        equipamento = veiculoObs.Equipamentos.Where(e => e.EquipamentoAceitaAbastecimento == true).FirstOrDefault();
                }
            }

            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = null;
            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoItem = null;
            if (destinatario != null && emitente != null)
                regraEntradaDocumento = RetornaRegraEntrada(destinatario, emitente, "", unidadeDeTrabalho);
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;

            documentoEntrada.Chave = nfe.protNFe.infProt.chNFe;
            documentoEntrada.Fornecedor = emitente;
            documentoEntrada.Destinatario = destinatario;
            documentoEntrada.DataEmissao = DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            documentoEntrada.DataEntrada = dataEntrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataEntradaDocumentoEntrada.DataLancamento ? DateTime.Now : documentoEntrada.DataEmissao; //DateTime.Now;//DateTime.ParseExact(nfe.NFe.infNFe.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);//DateTime.Now;
            documentoEntrada.Especie = especie;
            documentoEntrada.Modelo = modelo;
            documentoEntrada.Numero = int.Parse(nfe.NFe.infNFe.ide.nNF);
            documentoEntrada.Serie = nfe.NFe.infNFe.ide.serie;
            documentoEntrada.Veiculo = veiculoObs;
            documentoEntrada.Equipamento = equipamento;

            if (kmAbastecimento > 0 && (equipamento == null || (equipamento?.UtilizaTanqueCompartilhado ?? false)))
                documentoEntrada.KMAbastecimento = kmAbastecimento;
            if (!string.IsNullOrWhiteSpace(horimetroObservacao) && (documentoEntrada.Veiculo == null || documentoEntrada.Veiculo.TipoVeiculo != "0"))
            {
                int.TryParse(horimetroObservacao, out int horimetro);
                documentoEntrada.Horimetro = horimetro;
            }
            else if (equipamento != null && kmAbastecimento > 0 && (documentoEntrada.Veiculo == null || documentoEntrada.Veiculo.TipoVeiculo != "0"))
                documentoEntrada.Horimetro = kmAbastecimento;

            documentoEntrada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto;
            documentoEntrada.ValorTotal = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura);
            documentoEntrada.ValorBruto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura);
            documentoEntrada.ValorProdutos = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vProd, cultura);
            documentoEntrada.ValorTotalDesconto = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vDesc, cultura);
            documentoEntrada.ValorTotalFrete = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vFrete, cultura);
            documentoEntrada.ValorTotalSeguro = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vSeg, cultura);
            documentoEntrada.ValorTotalOutrasDespesas = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vOutro, cultura);

            documentoEntrada.NumeroLancamento = repDocumentoEntrada.BuscarUltimoNumeroLancamento() + 1;

            if (documentoEntrada.Destinatario == null)
                documentoEntrada.Destinatario = empresa;

            if (usuarioLogado != null)
                documentoEntrada.OperadorLancamentoDocumento = usuarioLogado;

            repDocumentoEntrada.Inserir(documentoEntrada);

            if (documentoEntrada.Veiculo != null && documentoEntrada.Equipamento != null && documentoEntrada.KMAbastecimento <= 0 && documentoEntrada.Horimetro <= 0)
                naoInformouKMVeiculo = true;

            bool todasDuplicatasLancadas = false;
            SalvarDuplicatas(documentoEntrada, nfe.NFe.infNFe.cobr, cultura, unidadeDeTrabalho, out todasDuplicatasLancadas, out int quantidadeDuplicatas);

            bool todosItensLancados = false;
            string msgRetornoItens = "";
            dynamic dynItens = SalvarItens(documentoEntrada, nfe.NFe.infNFe.det, empresa, emitente, cultura, unidadeDeTrabalho, veiculoObs, kmAbastecimento, horimetroObservacao, regraEntradaDocumento, destinatario, out todosItensLancados, tipoServicoMultisoftware, out regraEntradaDocumentoItem, equipamento, out msgRetornoItens);
            retornoErro = msgRetornoItens;
            if (regraEntradaDocumentoItem != null && destinatario != null && emitente != null)
            {
                regraEntradaDocumento = regraEntradaDocumentoItem;
                if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumentoItem.CFOPDentro != null)
                    cfopNota = regraEntradaDocumentoItem.CFOPDentro;
                else if (regraEntradaDocumentoItem.CFOPFora != null)
                    cfopNota = regraEntradaDocumentoItem.CFOPFora;
                if (regraEntradaDocumentoItem.NaturezaOperacao != null)
                    naturezaDaOperacaoNota = regraEntradaDocumentoItem.NaturezaOperacao;
                if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                    tipoMovimentoNota = cfopNota.TipoMovimentoUso;
            }
            else if (regraEntradaDocumento != null && destinatario != null && emitente != null)
            {
                if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumento.CFOPDentro != null)
                    cfopNota = regraEntradaDocumento.CFOPDentro;
                else if (regraEntradaDocumento.CFOPFora != null)
                    cfopNota = regraEntradaDocumento.CFOPFora;
                if (regraEntradaDocumento.NaturezaOperacao != null)
                    naturezaDaOperacaoNota = regraEntradaDocumento.NaturezaOperacao;
                if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                    tipoMovimentoNota = cfopNota.TipoMovimentoUso;
            }
            if (cfopNota != null)
                documentoEntrada.CFOP = cfopNota;
            if (naturezaDaOperacaoNota != null)
                documentoEntrada.NaturezaOperacao = naturezaDaOperacaoNota;
            if (tipoMovimentoNota != null)
                documentoEntrada.TipoMovimento = tipoMovimentoNota;

            if (documentoEntrada.TipoMovimento != null && documentoEntrada.CFOP != null && documentoEntrada.NaturezaOperacao != null)
                todasInformacoesLancadas = true;

            documentoEntrada.BaseSTRetido = dynItens.totalBaseSTRetido;
            documentoEntrada.ValorSTRetido = dynItens.totalValorSTRetido;
            documentoEntrada.BaseCalculoICMS = dynItens.totalBaseCalculoICMS;
            documentoEntrada.ValorTotalCOFINS = dynItens.valorTotalCOFINS;
            documentoEntrada.ValorTotalICMS = dynItens.valorTotalICMS;
            documentoEntrada.ValorTotalIPI = dynItens.valorTotalIPI;
            documentoEntrada.ValorTotalPIS = dynItens.valorTotalPIS;
            documentoEntrada.ValorTotalCreditoPresumido = dynItens.valorTotalCreditoPresumido;
            documentoEntrada.ValorTotalDiferencial = dynItens.valorTotalDiferencial;
            documentoEntrada.ValorTotalCusto = dynItens.valorTotalCusto;
            documentoEntrada.ValorTotalRetencaoPIS = dynItens.valorTotalRetencaoPIS;
            documentoEntrada.ValorTotalRetencaoCOFINS = dynItens.valorTotalRetencaoCOFINS;
            documentoEntrada.ValorTotalRetencaoINSS = dynItens.valorTotalRetencaoINSS;
            documentoEntrada.ValorTotalRetencaoIPI = dynItens.valorTotalRetencaoIPI;
            documentoEntrada.ValorTotalRetencaoCSLL = dynItens.valorTotalRetencaoCSLL;
            documentoEntrada.ValorTotalRetencaoOutras = dynItens.valorTotalRetencaoOutras;
            documentoEntrada.ValorTotalRetencaoIR = dynItens.valorTotalRetencaoIR;
            documentoEntrada.ValorTotalRetencaoISS = dynItens.valorTotalRetencaoISS;

            documentoEntrada.IndicadorPagamento = regraEntradaDocumento != null ? regraEntradaDocumento.IndicadorPagamento : quantidadeDuplicatas <= 1 ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.AVista : Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada.APrazo;

            if (regraEntradaDocumento != null && regraEntradaDocumento.FinalizarFaturarNotaAutomaticamente && todasInformacoesLancadas && todasDuplicatasLancadas && todosItensLancados && !naoInformouKMVeiculo && (documentoEntrada.Veiculo == null || (documentoEntrada.Veiculo.Status == "A" && documentoEntrada.Veiculo.Tipo == "P")))
            {
                documentoEntrada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    tipoAmbiente = usuarioLogado.Empresa.TipoAmbiente;

                string erro = "";
                retornoErro = "";
                bool contemQuantidadePendenteOrdemCompra = false;

                if (string.IsNullOrWhiteSpace(retornoErro) && !ValidarRegraEntrada(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro))
                {
                    retornoErro = erro + " - Documento não finalizado";
                    documentoEntrada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

                    retornoErro = "";

                    if (!GerarAbastecimentos(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, naoControlarKM, possuiPermissaoGravarValorDiferente, lancarDocumentoEntradaAbertoSeKMEstiverErrado))
                    {
                        retornoErro = erro;
                        Servicos.Log.TratarErro(erro);
                        return null;
                    }
                    else if (!string.IsNullOrWhiteSpace(erro))
                    {
                        //caso deu erro, a nota deve ser salva com o status aberto e mostrar a mensagem ao operador
                        documentoEntrada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto;
                        retornoErro = $"Doc. Número: {documentoEntrada.Numero} - {erro}";
                        Servicos.Log.TratarErro(erro);
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !Servicos.Embarcador.Frota.OrdemServico.VincularDocumentoEntradaAOrdemServico(documentoEntrada, out erro, unidadeDeTrabalho, usuarioLogado, tipoServicoMultisoftware, Auditado, configuracaoTMS))
                    {
                        retornoErro = erro;
                        Servicos.Log.TratarErro(erro);
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !Servicos.Embarcador.Compras.OrdemCompra.FinalizarOrdemCompra(documentoEntrada.Codigo, out erro, out contemQuantidadePendenteOrdemCompra, unidadeDeTrabalho, tipoServicoMultisoftware, Auditado, configuracaoTMS))
                    {
                        retornoErro = erro;
                        Servicos.Log.TratarErro(erro);
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !contemQuantidadePendenteOrdemCompra)
                        Servicos.Embarcador.Compras.OrdemCompra.CriarQualificacaoFornecedor(documentoEntrada, unidadeDeTrabalho);

                    if (string.IsNullOrWhiteSpace(retornoErro) && !svcTituloAPagar.AtualizarTitulos(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, dataCompetencia, lancarDocumentoEntradaAbertoSeKMEstiverErrado))
                    {
                        retornoErro = erro;
                        Servicos.Log.TratarErro(erro);
                        return null;
                    }
                    else if (!string.IsNullOrWhiteSpace(erro))
                    {
                        //caso deu erro, a nota deve ser salva com o status aberto e mostrar a mensagem ao operador
                        documentoEntrada.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Aberto;
                        retornoErro = $"Doc. Número: {documentoEntrada.Numero} - {erro}";
                        Log.TratarErro(erro);
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !svcTituloAPagar.AtualizarGuias(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, dataCompetencia))
                    {
                        retornoErro = erro;
                        unidadeDeTrabalho.Rollback();
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !VerificarCadastroItens(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, usuarioLogado, configuracaoTMS, configuracaoDocumentoEntrada))
                    {
                        retornoErro = erro;
                        Servicos.Log.TratarErro(erro);
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !AtualizarCusto(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, Auditado, configuracaoTMS))
                    {
                        retornoErro = erro;
                        Servicos.Log.TratarErro(erro);
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !MovimentarEstoque(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, dataCompetencia))
                    {
                        retornoErro = erro;
                        Servicos.Log.TratarErro(erro);
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !CadastrarPneu(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, Auditado))
                    {
                        retornoErro = erro;
                        unidadeDeTrabalho.Rollback();
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !svcBem.CadastrarBem(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, Auditado))
                    {
                        retornoErro = erro;
                        unidadeDeTrabalho.Rollback();
                        return null;
                    }

                    if (string.IsNullOrWhiteSpace(retornoErro) && !GerarMovimentoFinanceiroDocumentoEntrada(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, dataCompetencia))
                    {
                        retornoErro = erro;
                        unidadeDeTrabalho.Rollback();
                        return null;
                    }

                    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    {
                        if (string.IsNullOrWhiteSpace(retornoErro) && !GerarMovimentoFinanceiroEmissaoItens(documentoEntrada, unidadeDeTrabalho, tipoServicoMultisoftware, out erro, dataCompetencia))
                        {
                            retornoErro = erro;
                            Servicos.Log.TratarErro(erro);
                            return null;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(retornoErro))
                    {
                        GerarBaixarTituloDuplicata(documentoEntrada, unidadeDeTrabalho, tipoAmbiente, quantidadeDuplicatas, tipoServicoMultisoftware, StringConexao, usuarioLogado);
                        documentoEntrada.DataFinalizacao = DateTime.Now;
                        documentoEntrada.DocumentoFinalizadoAutomaticamente = true;
                        if (usuarioLogado != null)
                            documentoEntrada.OperadorFinalizaDocumento = usuarioLogado;
                    }
                }
            }

            repDocumentoEntrada.Atualizar(documentoEntrada);

            if (controlarTransacao)
                unidadeDeTrabalho.CommitChanges();

            return documentoEntrada;
        }

        private dynamic SalvarItens(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDet[] itensNota, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente emitente, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Veiculo veiculoObs, int kmAbastecimento, string horimetroObservacao, Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, Dominio.Entidades.Empresa destinatario, out bool todasInformacoesLancadas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoItem, Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento, out string msgRetorno)
        {
            msgRetorno = "";
            todasInformacoesLancadas = true;

            decimal totalBaseCalculoICMS = 0;
            decimal valorTotalCOFINS = 0;
            decimal valorTotalICMS = 0;
            decimal valorTotalIPI = 0;
            decimal valorTotalPIS = 0;
            decimal valorTotalCreditoPresumido = 0;
            decimal valorTotalDiferencial = 0;
            decimal valorTotalCusto = 0;
            decimal totalBaseSTRetido = 0;
            decimal totalValorSTRetido = 0;
            decimal valorTotalRetencaoPIS = 0;
            decimal valorTotalRetencaoCOFINS = 0;
            decimal valorTotalRetencaoINSS = 0;
            decimal valorTotalRetencaoIPI = 0;
            decimal valorTotalRetencaoCSLL = 0;
            decimal valorTotalRetencaoOutras = 0;
            decimal valorTotalRetencaoIR = 0;
            decimal valorTotalRetencaoISS = 0;

            bool faltouInformacaoKM = false;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);
            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeDeTrabalho);
            regraEntradaDocumentoItem = null;
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;

            foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDet det in itensNota)
            {
                object icms = this.ObterICMS(det.imposto);
                decimal valorICMSST = (decimal?)icms?.GetType().GetProperty("ValorICMSST")?.GetValue(icms, null) ?? 0m;

                decimal baseSTRetido = (decimal?)icms?.GetType().GetProperty("vBCSTRet")?.GetValue(icms, null) ?? 0m;
                decimal valorSTRetido = (decimal?)icms?.GetType().GetProperty("vICMSSTRet")?.GetValue(icms, null) ?? 0m;

                totalBaseSTRetido += baseSTRetido;
                totalValorSTRetido += valorSTRetido;

                object ipi = this.ObterIPI(det.imposto);
                decimal valorIPI = ipi != null && ipi.GetType().GetProperty("ValorIPI") != null ? (decimal)ipi.GetType().GetProperty("ValorIPI").GetValue(ipi, null) : 0m;

                decimal valorTotal = det.prod.vProd != null ? decimal.Parse(det.prod.vProd, cultura) : 0m;

                valorTotal += valorIPI + valorICMSST;

                object produto = this.ObterProduto(empresa, emitente, det.prod, unidadeDeTrabalho, tipoServicoMultisoftware);
                decimal fatorConversao = decimal.Parse(((string)produto.GetType().GetProperty("FatorConversao").GetValue(produto, null)));
                decimal quantidade = 0m;
                decimal quantidadeComercial = det.prod.qCom != null ? decimal.Parse(det.prod.qCom, cultura) : 0m;
                if (fatorConversao > 0)
                    quantidade = quantidadeComercial * fatorConversao;
                else if (fatorConversao < 0)
                    quantidade = quantidadeComercial / (fatorConversao * -1);
                else
                    quantidade = quantidadeComercial;

                decimal valorUnitario = 0;
                decimal valorUnitarioComercial = det.prod.vUnCom != null ? decimal.Parse(det.prod.vUnCom, cultura) : 0m;
                if (fatorConversao != 0)
                    valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
                else
                {
                    valorUnitario = valorUnitarioComercial;
                    if (valorIPI > 0 || valorICMSST > 0)
                        valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
                }

                if (destinatario != null && emitente != null)
                    regraEntradaDocumentoItem = RetornaRegraEntrada(destinatario, emitente, (string)produto.GetType().GetProperty("NCMProdutoFornecedor").GetValue(produto, null), unidadeDeTrabalho);
                if (regraEntradaDocumentoItem == null && regraEntradaDocumento != null)
                    regraEntradaDocumentoItem = regraEntradaDocumento;

                cfopNota = null;
                naturezaDaOperacaoNota = null;
                tipoMovimentoNota = null;
                if (regraEntradaDocumentoItem != null && destinatario != null && emitente != null)
                {
                    if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumentoItem.CFOPDentro != null)
                        cfopNota = regraEntradaDocumentoItem.CFOPDentro;
                    else if (regraEntradaDocumentoItem.CFOPFora != null)
                        cfopNota = regraEntradaDocumentoItem.CFOPFora;
                    if (regraEntradaDocumentoItem.NaturezaOperacao != null)
                        naturezaDaOperacaoNota = regraEntradaDocumentoItem.NaturezaOperacao;
                    if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                        tipoMovimentoNota = cfopNota.TipoMovimentoUso;
                }

                decimal desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m;
                decimal valorSeguro = det.prod.vSeg != null ? decimal.Parse(det.prod.vSeg, cultura) : 0m;
                decimal valorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m;
                decimal outrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m;
                decimal baseCalculoImposto = valorTotal - desconto + outrasDespesas + valorFrete + valorSeguro;
                Dominio.ObjetosDeValor.Embarcador.Financeiro.DadosRegraEntradaDocumento regraEntrada = RetornaDadosEntradaDocumento(baseCalculoImposto, cfopNota, destinatario, emitente, regraEntradaDocumentoItem);

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem();

                item.BaseSTRetido = baseSTRetido;
                item.ValorSTRetido = valorSTRetido;

                item.DocumentoEntrada = documentoEntrada;

                item.CSTCOFINS = regraEntrada.CSTCOFINS;
                item.CSTICMS = regraEntrada.CSTICMS;
                item.CSTIPI = regraEntrada.CSTIPI;
                item.CSTPIS = regraEntrada.CSTPIS;

                item.BaseCalculoPIS = regraEntrada.BaseCalculoPIS;
                item.PercentualReducaoBaseCalculoPIS = regraEntrada.PercentualReducaoPIS;
                item.AliquotaPIS = regraEntrada.AliquotaPIS;
                item.ValorPIS = regraEntrada.ValorPIS;

                item.BaseCalculoCOFINS = regraEntrada.BaseCalculoCOFINS;
                item.PercentualReducaoBaseCalculoCOFINS = regraEntrada.PercentualReducaoCOFINS;
                item.AliquotaCOFINS = regraEntrada.AliquotaCOFINS;
                item.ValorCOFINS = regraEntrada.ValorCOFINS;

                item.BaseCalculoIPI = regraEntrada.BaseCalculoIPI;
                item.PercentualReducaoBaseCalculoIPI = regraEntrada.PercentualReducaoIPI;
                item.AliquotaIPI = regraEntrada.AliquotaIPI;
                item.ValorIPI = regraEntrada.ValorIPICFOP;

                item.AliquotaICMS = regraEntrada.AliquotaICMS;
                item.BaseCalculoICMS = regraEntrada.BaseICMS;
                item.ValorICMS = regraEntrada.ValorICMS;
                item.AliquotaCreditoPresumido = regraEntrada.AliquotaCreditoPresumido;
                item.BaseCalculoCreditoPresumido = regraEntrada.BaseCalculoCreditoPresumido;
                item.ValorCreditoPresumido = regraEntrada.ValorCreditoPresumido;
                item.AliquotaDiferencial = regraEntrada.AliquotaDiferencial;
                item.BaseCalculoDiferencial = regraEntrada.BaseCalculoDiferencial;
                item.ValorDiferencial = regraEntrada.ValorDiferencial;

                item.ValorRetencaoPIS = regraEntrada.ValorRetencaoPIS;
                item.ValorRetencaoCOFINS = regraEntrada.ValorRetencaoCOFINS;
                item.ValorRetencaoINSS = regraEntrada.ValorRetencaoINSS;
                item.ValorRetencaoIPI = regraEntrada.ValorRetencaoIPI;
                item.ValorRetencaoCSLL = regraEntrada.ValorRetencaoCSLL;
                item.ValorRetencaoOutras = regraEntrada.ValorRetencaoOutras;
                item.ValorRetencaoIR = regraEntrada.ValorRetencaoIR;
                item.ValorRetencaoISS = regraEntrada.ValorRetencaoISS;

                item.Veiculo = veiculoObs;
                item.Equipamento = equipamento;
                if (kmAbastecimento > 0 && (equipamento == null || (equipamento?.UtilizaTanqueCompartilhado ?? false)))
                    item.KMAbastecimento = kmAbastecimento;
                if (!string.IsNullOrWhiteSpace(horimetroObservacao) && (item.Veiculo == null || item.Veiculo.TipoVeiculo != "0"))
                {
                    int.TryParse(horimetroObservacao, out int horimetro);
                    item.Horimetro = horimetro;
                }
                else if (equipamento != null && kmAbastecimento > 0 && (item.Veiculo == null || item.Veiculo.TipoVeiculo != "0"))
                    item.Horimetro = kmAbastecimento;

                if (cfopNota != null)
                {
                    if (cfopNota.RealizarRateioSomenteQuandoTiverOS)
                    {
                        if (item.DocumentoEntrada.OrdemServico == null)
                        {
                            item.CFOP = cfopNota;
                            item.GeraRateioDespesaVeiculo = cfopNota.RealizarRateioDespesaVeiculo;
                        }
                        else
                        {
                            item.CFOP = cfopNota;
                            item.GeraRateioDespesaVeiculo = false;
                        }
                    }
                }

                if (naturezaDaOperacaoNota != null)
                    item.NaturezaOperacao = naturezaDaOperacaoNota;
                if (tipoMovimentoNota != null)
                    item.TipoMovimento = tipoMovimentoNota;
                if (regraEntradaDocumentoItem != null)
                    item.RegraEntradaDocumento = regraEntradaDocumentoItem;
                item.CodigoProdutoFornecedor = (string)produto.GetType().GetProperty("CodigoProdutoFornecedor").GetValue(produto, null);
                item.DescricaoProdutoFornecedor = (string)produto.GetType().GetProperty("DescricaoProdutoFornecedor").GetValue(produto, null);
                item.CESTProdutoFornecedor = (string)produto.GetType().GetProperty("CESTProdutoFornecedor").GetValue(produto, null);
                item.NCMProdutoFornecedor = (string)produto.GetType().GetProperty("NCMProdutoFornecedor").GetValue(produto, null);
                item.Produto = repProduto.BuscarPorCodigo((int)produto.GetType().GetProperty("Codigo").GetValue(produto, null));
                item.CalculoCustoProduto = (string)produto.GetType().GetProperty("CalculoCustoProduto").GetValue(produto, null);
                item.CodigoBarrasEAN = (string)produto.GetType().GetProperty("CodigoBarrasEAN").GetValue(produto, null);
                item.UnidadeMedida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)produto.GetType().GetProperty("UnidadeMedida").GetValue(produto, null);
                item.Desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m;
                item.Quantidade = quantidade;
                item.Sequencial = int.Parse(det.nItem);
                item.ValorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m;
                item.OutrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m;
                item.ValorTotal = valorTotal;
                item.ValorUnitario = valorUnitario;
                item.ValorSeguro = det.prod.vSeg != null ? decimal.Parse(det.prod.vSeg, cultura) : 0m;

                item.UnidadeMedidaFornecedor = det.prod.uCom != null ? det.prod.uCom : string.Empty;
                item.QuantidadeFornecedor = quantidadeComercial;
                item.ValorUnitarioFornecedor = valorUnitarioComercial;
                item.CentroResultado = veiculoObs?.CentroResultado;
                item.OrigemMercadoria = icms != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria)icms.GetType().GetProperty("OrigemMercadoria").GetValue(icms, null) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;

                if (item.Produto == null)
                {
                    Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorProdutoEFornecedor(item.CodigoProdutoFornecedor, documentoEntrada.Fornecedor.CPF_CNPJ,
                        tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? empresa != null ? empresa.Codigo : 0 : 0);
                    if (produtoFornecedor != null && produtoFornecedor.Produto != null)
                        item.Produto = produtoFornecedor.Produto;
                }

                if (item.Veiculo != null && item.KMAbastecimento <= 0)
                    faltouInformacaoKM = true;
                else
                    faltouInformacaoKM = false;

                totalBaseCalculoICMS += regraEntrada.BaseICMS;
                valorTotalCOFINS += regraEntrada.ValorCOFINS;
                valorTotalICMS += regraEntrada.ValorICMS;
                valorTotalIPI += regraEntrada.ValorIPICFOP;
                valorTotalPIS += regraEntrada.ValorPIS;
                valorTotalCreditoPresumido += regraEntrada.ValorCreditoPresumido;
                valorTotalDiferencial += regraEntrada.ValorDiferencial;
                valorTotalCusto = ((valorTotal) + regraEntrada.ValorDiferencial + regraEntrada.ValorIPICFOP + valorFrete + valorSeguro + outrasDespesas - desconto);

                valorTotalRetencaoPIS += regraEntrada.ValorRetencaoPIS;
                valorTotalRetencaoCOFINS += regraEntrada.ValorRetencaoCOFINS;
                valorTotalRetencaoINSS += regraEntrada.ValorRetencaoINSS;
                valorTotalRetencaoIPI += regraEntrada.ValorRetencaoIPI;
                valorTotalRetencaoCSLL += regraEntrada.ValorRetencaoCSLL;
                valorTotalRetencaoOutras += regraEntrada.ValorRetencaoOutras;
                valorTotalRetencaoIR += regraEntrada.ValorRetencaoIR;
                valorTotalRetencaoISS += regraEntrada.ValorRetencaoISS;

                if (todasInformacoesLancadas)
                {
                    if (regraEntradaDocumentoItem != null && !regraEntradaDocumentoItem.FinalizarFaturarNotaAutomaticamente)
                        todasInformacoesLancadas = false;
                    else if (string.IsNullOrWhiteSpace(item.CSTCOFINS) || string.IsNullOrWhiteSpace(item.CSTICMS) || string.IsNullOrWhiteSpace(item.CSTPIS) || item.CFOP == null || item.TipoMovimento == null || item.NaturezaOperacao == null)
                        todasInformacoesLancadas = false;
                    else if (regraEntradaDocumentoItem != null && regraEntradaDocumentoItem.ObrigarInformarVeiculo && item.Veiculo == null && item.Equipamento == null)
                        todasInformacoesLancadas = false;
                    else if (regraEntradaDocumentoItem != null && faltouInformacaoKM)
                        todasInformacoesLancadas = false;
                    else if (regraEntradaDocumentoItem != null && regraEntradaDocumentoItem.FinalizarFaturarNotaAutomaticamente && item.NaturezaOperacao != null && item.NaturezaOperacao.ControlaEstoque && item.Produto == null)
                    {
                        todasInformacoesLancadas = false;
                        msgRetorno = "Não foi possível finalizar o documento de entrada. É necessário vincular um produto existente para um natureza de operação que controla estoque e finalizar o documento de entrada. Item " + item.DescricaoProdutoFornecedor + ".";
                    }
                }

                repItem.Inserir(item);
            }

            dynamic dynRetorno = new
            {
                totalBaseCalculoICMS,
                valorTotalCOFINS,
                valorTotalICMS,
                valorTotalIPI,
                valorTotalPIS,
                valorTotalCreditoPresumido,
                valorTotalDiferencial,
                valorTotalCusto,
                totalBaseSTRetido,
                totalValorSTRetido,
                valorTotalRetencaoPIS,
                valorTotalRetencaoCOFINS,
                valorTotalRetencaoINSS,
                valorTotalRetencaoIPI,
                valorTotalRetencaoCSLL,
                valorTotalRetencaoOutras,
                valorTotalRetencaoIR,
                valorTotalRetencaoISS
            };

            return dynRetorno;
        }

        private dynamic SalvarItens(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDet[] itensNota, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente emitente, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Veiculo veiculoObs, int kmAbastecimento, string horimetroObservacao, Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, Dominio.Entidades.Empresa destinatario, out bool todasInformacoesLancadas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumentoItem, Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento, out string msgRetorno)
        {
            msgRetorno = "";
            todasInformacoesLancadas = true;

            decimal totalBaseCalculoICMS = 0;
            decimal valorTotalCOFINS = 0;
            decimal valorTotalICMS = 0;
            decimal valorTotalIPI = 0;
            decimal valorTotalPIS = 0;
            decimal valorTotalCreditoPresumido = 0;
            decimal valorTotalDiferencial = 0;
            decimal valorTotalCusto = 0;
            decimal totalBaseSTRetido = 0;
            decimal totalValorSTRetido = 0;
            decimal valorTotalRetencaoPIS = 0;
            decimal valorTotalRetencaoCOFINS = 0;
            decimal valorTotalRetencaoINSS = 0;
            decimal valorTotalRetencaoIPI = 0;
            decimal valorTotalRetencaoCSLL = 0;
            decimal valorTotalRetencaoOutras = 0;
            decimal valorTotalRetencaoIR = 0;
            decimal valorTotalRetencaoISS = 0;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);
            Repositorio.ProdutoFornecedor repProdutoFornecedor = new Repositorio.ProdutoFornecedor(unidadeDeTrabalho);
            regraEntradaDocumentoItem = null;
            Dominio.Entidades.CFOP cfopNota = null;
            Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacaoNota = null;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimentoNota = null;

            foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDet det in itensNota)
            {
                object icms = this.ObterICMS(det.imposto);

                decimal baseCalculoICMS = (decimal?)icms?.GetType().GetProperty("BaseCalculoICMS")?.GetValue(icms, null) ?? 0m;
                decimal aliquotaICMS = (decimal?)icms?.GetType().GetProperty("AliquotaICMS")?.GetValue(icms, null) ?? 0m;
                decimal valorICMS = (decimal?)icms?.GetType().GetProperty("ValorICMS")?.GetValue(icms, null) ?? 0m;
                string cstICMS = icms != null && icms.GetType().GetProperty("CST") != null ? int.Parse((string)icms.GetType().GetProperty("CST").GetValue(icms, null)).ToString("D3") : "";
                decimal valorICMSST = (decimal?)icms?.GetType().GetProperty("ValorICMSST")?.GetValue(icms, null) ?? 0m;

                decimal baseSTRetido = (decimal?)icms?.GetType().GetProperty("vBCSTRet")?.GetValue(icms, null) ?? 0m;
                decimal valorSTRetido = (decimal?)icms?.GetType().GetProperty("vICMSSTRet")?.GetValue(icms, null) ?? 0m;
                decimal bcSTDest = (decimal?)icms?.GetType().GetProperty("BCSTDest")?.GetValue(icms, null) ?? 0m;
                decimal icmsTDest = (decimal?)icms?.GetType().GetProperty("ICMSSTDest")?.GetValue(icms, null) ?? 0m;

                totalBaseSTRetido += baseSTRetido;
                totalValorSTRetido += valorSTRetido;

                object ipi = this.ObterIPI(det.imposto);
                string cstIPI = ipi != null && ipi.GetType().GetProperty("CST") != null ? (string)ipi.GetType().GetProperty("CST").GetValue(ipi, null) : "";
                decimal baseCalculoIPI = ipi != null && ipi.GetType().GetProperty("BaseCalculoIPI") != null ? (decimal)ipi.GetType().GetProperty("BaseCalculoIPI").GetValue(ipi, null) : 0m;
                decimal aliquotaIPI = ipi != null && ipi.GetType().GetProperty("AliquotaIPI") != null ? (decimal)ipi.GetType().GetProperty("AliquotaIPI").GetValue(ipi, null) : 0m;
                decimal valorIPI = ipi != null && ipi.GetType().GetProperty("ValorIPI") != null ? (decimal)ipi.GetType().GetProperty("ValorIPI").GetValue(ipi, null) : 0m;

                object pis = this.ObterPIS(det.imposto.PIS);

                string cstPIS = pis != null && pis.GetType().GetProperty("CST") != null ? (string)pis.GetType().GetProperty("CST").GetValue(pis, null) : "";
                decimal valorPISItem = (decimal?)pis?.GetType().GetProperty("ValorPIS")?.GetValue(pis, null) ?? 0m;

                object cofins = this.ObterCOFINS(det.imposto.COFINS);

                string cstCOFINS = cofins != null && cofins.GetType().GetProperty("CST") != null ? (string)cofins.GetType().GetProperty("CST").GetValue(cofins, null) : "";
                decimal valorCOFINSItem = (decimal?)cofins?.GetType().GetProperty("ValorCOFINS")?.GetValue(cofins, null) ?? 0m;

                decimal valorTotal = det.prod.vProd != null ? decimal.Parse(det.prod.vProd, cultura) : 0m;

                valorTotal += valorIPI + valorICMSST;

                object produto = this.ObterProduto(empresa, emitente, det.prod, unidadeDeTrabalho, tipoServicoMultisoftware);
                decimal fatorConversao = decimal.Parse(((string)produto.GetType().GetProperty("FatorConversao").GetValue(produto, null)));
                decimal quantidade = 0m;
                decimal quantidadeComercial = det.prod.qCom != null ? decimal.Parse(det.prod.qCom, cultura) : 0m;
                if (fatorConversao > 0)
                    quantidade = quantidadeComercial * fatorConversao;
                else if (fatorConversao < 0)
                    quantidade = quantidadeComercial / (fatorConversao * -1);
                else
                    quantidade = quantidadeComercial;

                decimal valorUnitario = 0;
                decimal valorUnitarioComercial = det.prod.vUnCom != null ? decimal.Parse(det.prod.vUnCom, cultura) : 0m;
                if (fatorConversao != 0)
                    valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
                else
                {
                    valorUnitario = valorUnitarioComercial;
                    if (valorIPI > 0 || valorICMSST > 0)
                        valorUnitario = valorTotal / (quantidade > 0 ? quantidade : 1);
                }

                if (destinatario != null && emitente != null)
                    regraEntradaDocumentoItem = RetornaRegraEntrada(destinatario, emitente, (string)produto.GetType().GetProperty("NCMProdutoFornecedor").GetValue(produto, null), unidadeDeTrabalho);
                if (regraEntradaDocumentoItem == null && regraEntradaDocumento != null)
                    regraEntradaDocumentoItem = regraEntradaDocumento;

                cfopNota = null;
                naturezaDaOperacaoNota = null;
                tipoMovimentoNota = null;
                if (regraEntradaDocumentoItem != null && destinatario != null && emitente != null)
                {
                    if (destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla && regraEntradaDocumentoItem.CFOPDentro != null)
                        cfopNota = regraEntradaDocumentoItem.CFOPDentro;
                    else if (regraEntradaDocumentoItem.CFOPFora != null)
                        cfopNota = regraEntradaDocumentoItem.CFOPFora;
                    if (regraEntradaDocumentoItem.NaturezaOperacao != null)
                        naturezaDaOperacaoNota = regraEntradaDocumentoItem.NaturezaOperacao;
                    if (cfopNota != null && cfopNota.TipoMovimentoUso != null)
                        tipoMovimentoNota = cfopNota.TipoMovimentoUso;
                }

                decimal desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m;
                decimal valorSeguro = det.prod.vSeg != null ? decimal.Parse(det.prod.vSeg, cultura) : 0m;
                decimal valorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m;
                decimal outrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m;
                decimal baseCalculoImposto = valorTotal - desconto + outrasDespesas + valorFrete + valorSeguro;
                Dominio.ObjetosDeValor.Embarcador.Financeiro.DadosRegraEntradaDocumento regraEntrada = RetornaDadosEntradaDocumento(baseCalculoImposto, cfopNota, destinatario, emitente, regraEntradaDocumentoItem);

                if (regraEntrada.AliquotaICMS == 0 && regraEntrada.ValorICMS == 0 && bcSTDest > 0 && icmsTDest > 0)
                {
                    if (string.IsNullOrWhiteSpace(regraEntrada.CSTICMS))
                        regraEntrada.CSTICMS = "060";
                    regraEntrada.AliquotaICMS = 0;
                    regraEntrada.ValorICMS = icmsTDest;
                    regraEntrada.BaseICMS = bcSTDest;
                    regraEntrada.AliquotaICMS = Math.Truncate(((icmsTDest * 100) / bcSTDest));
                }

                if ((!string.IsNullOrWhiteSpace(cstICMS) || cstICMS.Equals("061")) && !regraEntrada.CSTICMS.Equals("040"))
                {
                    regraEntrada.CSTICMS = !string.IsNullOrWhiteSpace(regraEntrada.CSTICMS) ? regraEntrada.CSTICMS : cstICMS;
                    regraEntrada.BaseICMS = regraEntrada.BaseICMS > 0 ? regraEntrada.BaseICMS : baseCalculoICMS;
                    regraEntrada.AliquotaICMS = regraEntrada.AliquotaICMS > 0 ? regraEntrada.AliquotaICMS : aliquotaICMS;
                    regraEntrada.ValorICMS = regraEntrada.ValorICMS > 0 ? regraEntrada.ValorICMS : valorICMS;
                }

                if (!string.IsNullOrWhiteSpace(cstIPI))
                {
                    regraEntrada.CSTIPI = !string.IsNullOrWhiteSpace(regraEntrada.CSTIPI) ? regraEntrada.CSTIPI : cstIPI;
                    regraEntrada.BaseCalculoIPI = regraEntrada.BaseCalculoIPI > 0 ? regraEntrada.BaseCalculoIPI : baseCalculoIPI;
                    regraEntrada.AliquotaIPI = regraEntrada.AliquotaIPI > 0 ? regraEntrada.AliquotaIPI : aliquotaIPI;
                    regraEntrada.ValorIPICFOP = regraEntrada.ValorIPICFOP > 0 ? regraEntrada.ValorIPICFOP : valorIPI;
                }

                if (!string.IsNullOrWhiteSpace(cstCOFINS))
                {
                    regraEntrada.CSTCOFINS = !string.IsNullOrWhiteSpace(regraEntrada.CSTCOFINS) ? regraEntrada.CSTCOFINS : cstCOFINS;
                    //regraEntrada.BaseCalculoCOFINS = regraEntrada.BaseCalculoCOFINS > 0 ? regraEntrada.BaseCalculoCOFINS : baseCalculoCOFINS;
                    //regraEntrada.AliquotaCOFINS = regraEntrada.AliquotaCOFINS > 0 ? regraEntrada.AliquotaCOFINS : aliquotaCOFINS;
                    regraEntrada.ValorCOFINS = regraEntrada.ValorCOFINS > 0 ? regraEntrada.ValorCOFINS : valorCOFINSItem;
                }

                if (!string.IsNullOrWhiteSpace(cstPIS))
                {
                    regraEntrada.CSTPIS = !string.IsNullOrWhiteSpace(regraEntrada.CSTPIS) ? regraEntrada.CSTPIS : cstPIS;
                    //regraEntrada.BaseCalculoPIS = regraEntrada.BaseCalculoPIS > 0 ? regraEntrada.BaseCalculoPIS : baseCalculoPIS;
                    //regraEntrada.AliquotaPIS = regraEntrada.AliquotaPIS > 0 ? regraEntrada.AliquotaPIS : aliquotaPIS;
                    regraEntrada.ValorPIS = regraEntrada.ValorPIS > 0 ? regraEntrada.ValorPIS : valorPISItem;
                }

                if ((cfopNota?.CreditoSobreTotalParaItensSujeitosICMSST ?? false) && (baseSTRetido > 0 || valorSTRetido > 0))
                {
                    totalBaseSTRetido -= baseSTRetido;
                    totalValorSTRetido -= valorSTRetido;
                    baseSTRetido = 0;
                    valorSTRetido = 0;
                }

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem();

                item.BaseSTRetido = baseSTRetido;
                item.ValorSTRetido = valorSTRetido;

                item.DocumentoEntrada = documentoEntrada;

                item.CSTCOFINS = regraEntrada.CSTCOFINS;
                item.CSTICMS = regraEntrada.CSTICMS;
                item.CSTIPI = regraEntrada.CSTIPI;
                item.CSTPIS = regraEntrada.CSTPIS;

                item.BaseCalculoPIS = regraEntrada.BaseCalculoPIS;
                item.PercentualReducaoBaseCalculoPIS = regraEntrada.PercentualReducaoPIS;
                item.AliquotaPIS = regraEntrada.AliquotaPIS;
                item.ValorPIS = regraEntrada.ValorPIS;

                item.BaseCalculoCOFINS = regraEntrada.BaseCalculoCOFINS;
                item.PercentualReducaoBaseCalculoCOFINS = regraEntrada.PercentualReducaoCOFINS;
                item.AliquotaCOFINS = regraEntrada.AliquotaCOFINS;
                item.ValorCOFINS = regraEntrada.ValorCOFINS;

                item.BaseCalculoIPI = regraEntrada.BaseCalculoIPI;
                item.PercentualReducaoBaseCalculoIPI = regraEntrada.PercentualReducaoIPI;
                item.AliquotaIPI = regraEntrada.AliquotaIPI;
                item.ValorIPI = regraEntrada.ValorIPICFOP;

                item.AliquotaICMS = regraEntrada.AliquotaICMS;
                item.BaseCalculoICMS = regraEntrada.BaseICMS;
                item.ValorICMS = regraEntrada.ValorICMS;
                item.AliquotaCreditoPresumido = regraEntrada.AliquotaCreditoPresumido;
                item.BaseCalculoCreditoPresumido = regraEntrada.BaseCalculoCreditoPresumido;
                item.ValorCreditoPresumido = regraEntrada.ValorCreditoPresumido;
                item.AliquotaDiferencial = regraEntrada.AliquotaDiferencial;
                item.BaseCalculoDiferencial = regraEntrada.BaseCalculoDiferencial;
                item.ValorDiferencial = regraEntrada.ValorDiferencial;

                item.ValorRetencaoPIS = regraEntrada.ValorRetencaoPIS;
                item.ValorRetencaoCOFINS = regraEntrada.ValorRetencaoCOFINS;
                item.ValorRetencaoINSS = regraEntrada.ValorRetencaoINSS;
                item.ValorRetencaoIPI = regraEntrada.ValorRetencaoIPI;
                item.ValorRetencaoCSLL = regraEntrada.ValorRetencaoCSLL;
                item.ValorRetencaoOutras = regraEntrada.ValorRetencaoOutras;
                item.ValorRetencaoIR = regraEntrada.ValorRetencaoIR;
                item.ValorRetencaoISS = regraEntrada.ValorRetencaoISS;

                item.Veiculo = veiculoObs;
                item.Equipamento = equipamento;
                if (kmAbastecimento > 0 && (equipamento == null || (equipamento?.UtilizaTanqueCompartilhado ?? false)))
                    item.KMAbastecimento = kmAbastecimento;
                if (!string.IsNullOrWhiteSpace(horimetroObservacao) && (item.Veiculo == null || item.Veiculo.TipoVeiculo != "0"))
                {
                    int.TryParse(horimetroObservacao, out int horimetro);
                    item.Horimetro = horimetro;
                }
                else if (equipamento != null && kmAbastecimento > 0 && (item.Veiculo == null || item.Veiculo.TipoVeiculo != "0"))
                    item.Horimetro = kmAbastecimento;

                if (cfopNota != null)
                {
                    item.CFOP = cfopNota;
                    item.GeraRateioDespesaVeiculo = cfopNota.RealizarRateioDespesaVeiculo;
                }
                if (naturezaDaOperacaoNota != null)
                    item.NaturezaOperacao = naturezaDaOperacaoNota;
                if (tipoMovimentoNota != null)
                    item.TipoMovimento = tipoMovimentoNota;
                if (regraEntradaDocumentoItem != null)
                    item.RegraEntradaDocumento = regraEntradaDocumentoItem;
                item.CodigoProdutoFornecedor = (string)produto.GetType().GetProperty("CodigoProdutoFornecedor").GetValue(produto, null);
                item.DescricaoProdutoFornecedor = (string)produto.GetType().GetProperty("DescricaoProdutoFornecedor").GetValue(produto, null);
                item.CESTProdutoFornecedor = (string)produto.GetType().GetProperty("CESTProdutoFornecedor").GetValue(produto, null);
                item.NCMProdutoFornecedor = (string)produto.GetType().GetProperty("NCMProdutoFornecedor").GetValue(produto, null);
                item.Produto = repProduto.BuscarPorCodigo((int)produto.GetType().GetProperty("Codigo").GetValue(produto, null));
                item.CalculoCustoProduto = (string)produto.GetType().GetProperty("CalculoCustoProduto").GetValue(produto, null);
                item.CodigoBarrasEAN = (string)produto.GetType().GetProperty("CodigoBarrasEAN").GetValue(produto, null);
                item.UnidadeMedida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida)produto.GetType().GetProperty("UnidadeMedida").GetValue(produto, null);
                item.Desconto = det.prod.vDesc != null ? decimal.Parse(det.prod.vDesc, cultura) : 0m;
                item.Quantidade = quantidade;
                item.Sequencial = int.Parse(det.nItem);
                item.ValorFrete = det.prod.vFrete != null ? decimal.Parse(det.prod.vFrete, cultura) : 0m;
                item.OutrasDespesas = det.prod.vOutro != null ? decimal.Parse(det.prod.vOutro, cultura) : 0m;
                item.ValorTotal = valorTotal;
                item.ValorUnitario = valorUnitario;
                item.ValorSeguro = det.prod.vSeg != null ? decimal.Parse(det.prod.vSeg, cultura) : 0m;

                item.UnidadeMedidaFornecedor = det.prod.uCom != null ? det.prod.uCom : string.Empty;
                item.QuantidadeFornecedor = quantidadeComercial;
                item.ValorUnitarioFornecedor = valorUnitarioComercial;
                item.CentroResultado = veiculoObs?.CentroResultado;
                item.OrigemMercadoria = icms != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria)icms.GetType().GetProperty("OrigemMercadoria").GetValue(icms, null) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;

                if (item.Produto == null)
                {
                    Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repProdutoFornecedor.BuscarPorProdutoEFornecedor(item.CodigoProdutoFornecedor, documentoEntrada.Fornecedor.CPF_CNPJ,
                        tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? empresa != null ? empresa.Codigo : 0 : 0);
                    if (produtoFornecedor != null && produtoFornecedor.Produto != null)
                        item.Produto = produtoFornecedor.Produto;
                }

                totalBaseCalculoICMS += regraEntrada.BaseICMS;
                valorTotalCOFINS += regraEntrada.ValorCOFINS;
                valorTotalICMS += regraEntrada.ValorICMS;
                valorTotalIPI += regraEntrada.ValorIPICFOP;
                valorTotalPIS += regraEntrada.ValorPIS;
                valorTotalCreditoPresumido += regraEntrada.ValorCreditoPresumido;
                valorTotalDiferencial += regraEntrada.ValorDiferencial;
                valorTotalCusto = ((valorTotal) + regraEntrada.ValorDiferencial + regraEntrada.ValorIPICFOP + valorFrete + valorSeguro + outrasDespesas - desconto);

                valorTotalRetencaoPIS += regraEntrada.ValorRetencaoPIS;
                valorTotalRetencaoCOFINS += regraEntrada.ValorRetencaoCOFINS;
                valorTotalRetencaoINSS += regraEntrada.ValorRetencaoINSS;
                valorTotalRetencaoIPI += regraEntrada.ValorRetencaoIPI;
                valorTotalRetencaoCSLL += regraEntrada.ValorRetencaoCSLL;
                valorTotalRetencaoOutras += regraEntrada.ValorRetencaoOutras;
                valorTotalRetencaoIR += regraEntrada.ValorRetencaoIR;
                valorTotalRetencaoISS += regraEntrada.ValorRetencaoISS;

                if (todasInformacoesLancadas)
                {
                    if (regraEntradaDocumentoItem != null && !regraEntradaDocumentoItem.FinalizarFaturarNotaAutomaticamente)
                        todasInformacoesLancadas = false;
                    else if (string.IsNullOrWhiteSpace(item.CSTICMS) || item.CFOP == null || item.TipoMovimento == null || item.NaturezaOperacao == null)
                        todasInformacoesLancadas = false;
                    else if (regraEntradaDocumentoItem != null && regraEntradaDocumentoItem.ObrigarInformarVeiculo && item.Veiculo == null && item.Equipamento == null)
                        todasInformacoesLancadas = false;
                    else if (regraEntradaDocumentoItem != null && regraEntradaDocumentoItem.ObrigarInformarVeiculo && item.Veiculo != null && item.Equipamento != null && item.KMAbastecimento <= 0 && item.Horimetro <= 0)
                        todasInformacoesLancadas = false;
                    else if (regraEntradaDocumentoItem != null && regraEntradaDocumentoItem.FinalizarFaturarNotaAutomaticamente && item.NaturezaOperacao != null && item.NaturezaOperacao.ControlaEstoque && item.Produto == null)
                    {
                        todasInformacoesLancadas = false;
                        msgRetorno = "Não foi possível finalizar o documento de entrada. É necessário vincular um produto existente para um natureza de operação que controla estoque e finalizar o documento de entrada. Item " + item.DescricaoProdutoFornecedor + ".";
                    }

                    if (regraEntradaDocumentoItem != null && (regraEntradaDocumentoItem?.FinalizarFaturarNotaAutomaticamente ?? false) && (regraEntradaDocumentoItem?.NaoFinalizarDocumentoSemProdutoPreCadastrado ?? false) && item.Produto == null)
                    {
                        todasInformacoesLancadas = false;
                        msgRetorno = "Não foi possível finalizar o documento de entrada. É necessário vincular um produto existente e finalizar o documento de entrada. Item " + item.DescricaoProdutoFornecedor + ".";
                    }
                }

                string formulaCusto = item.Produto?.CalculoCustoProduto ?? Servicos.Embarcador.Produto.Custo.ObterFormulaPadrao(unidadeDeTrabalho);
                if ((item.ValorCustoUnitario <= 0 || item.ValorCustoTotal <= 0) && !string.IsNullOrWhiteSpace(formulaCusto))
                {
                    string[] campos = formulaCusto.Split('#');
                    for (int i = 0; i < campos.Length; i++)
                    {
                        if (campos[i].Trim() == "")
                        {
                            campos.ToList().Remove(campos[i]);
                        }
                        else
                        {
                            campos[i] = campos[i].Trim();
                        }

                    }

                    quantidade = item.Quantidade;
                    valorUnitario = item.ValorUnitario;
                    valorTotal = item.ValorTotal;

                    if ((valorTotal) > 0)
                    {
                        valorICMS = item.ValorICMS;
                        decimal valorCreditoPresumido = item.ValorCreditoPresumido;
                        decimal valorDiferencial = item.ValorDiferencial;
                        valorICMSST = item.ValorICMSST;
                        valorIPI = item.ValorIPI;
                        valorFrete = item.ValorFrete;
                        decimal valorOutras = item.ValorOutrasDespesasFora;
                        valorSeguro = item.ValorSeguro;
                        decimal valorDesconto = item.Desconto;
                        decimal valorDescontoFora = item.ValorDescontoFora;
                        decimal valorImpostoFora = item.ValorImpostosFora;
                        decimal valorOutrasFora = item.ValorOutrasDespesasFora;
                        decimal valorFreteFora = item.ValorFreteFora;
                        decimal valorICMSFreteFora = item.ValorICMSFreteFora;
                        decimal valorDiferencialFreteFora = item.ValorDiferencialFreteFora;
                        decimal valorPIS = item.ValorPIS;
                        decimal valorCOFINS = item.ValorCOFINS;
                        decimal custoUnitario = 0m;
                        decimal custoTotal = 0m;

                        custoUnitario = (valorTotal);

                        if (valorDesconto > 0)
                        {
                            if (campos.Contains("ValorDesconto") && campos[campos.ToList().IndexOf("ValorDesconto") - 1] == "+")
                                custoUnitario = custoUnitario + valorDesconto;
                            else if (campos.Contains("ValorDesconto") && campos[campos.ToList().IndexOf("ValorDesconto") - 1] == "-")
                                custoUnitario = custoUnitario - valorDesconto;
                        }

                        if (valorOutras > 0)
                        {
                            if (campos.Contains("ValorOutras") && campos[campos.ToList().IndexOf("ValorOutras") - 1] == "+")
                                custoUnitario = custoUnitario + valorOutras;
                            else if (campos.Contains("ValorOutras") && campos[campos.ToList().IndexOf("ValorOutras") - 1] == "-")
                                custoUnitario = custoUnitario - valorOutras;
                        }

                        if (valorFrete > 0)
                        {
                            if (campos.Contains("ValorFrete") && campos[campos.ToList().IndexOf("ValorFrete") - 1] == "+")
                                custoUnitario = custoUnitario + valorFrete;
                            else if (campos.Contains("ValorFrete") && campos[campos.ToList().IndexOf("ValorFrete") - 1] == "-")
                                custoUnitario = custoUnitario - valorFrete;
                        }

                        if (valorSeguro > 0)
                        {
                            if (campos.Contains("ValorSeguro") && campos[campos.ToList().IndexOf("ValorSeguro") - 1] == "+")
                                custoUnitario = custoUnitario + valorSeguro;
                            else if (campos.Contains("ValorSeguro") && campos[campos.ToList().IndexOf("ValorSeguro") - 1] == "-")
                                custoUnitario = custoUnitario - valorSeguro;
                        }

                        if (valorICMS > 0)
                        {
                            if (campos.Contains("ValorICMS") && campos[campos.ToList().IndexOf("ValorICMS") - 1] == "+")
                                custoUnitario = custoUnitario + valorICMS;
                            else if (campos.Contains("ValorICMS") && campos[campos.ToList().IndexOf("ValorICMS") - 1] == "-")
                                custoUnitario = custoUnitario - valorICMS;
                        }

                        if (valorIPI > 0)
                        {
                            if (campos.Contains("ValorIPI") && campos[campos.ToList().IndexOf("ValorIPI") - 1] == "+")
                                custoUnitario = custoUnitario + valorIPI;
                            else if (campos.Contains("ValorIPI") && campos[campos.ToList().IndexOf("ValorIPI") - 1] == "-")
                                custoUnitario = custoUnitario - valorIPI;
                        }

                        if (valorICMSST > 0)
                        {
                            if (campos.Contains("ValorICMSST") && campos[campos.ToList().IndexOf("ValorICMSST") - 1] == "+")
                                custoUnitario = custoUnitario + valorICMSST;
                            else if (campos.Contains("ValorICMSST") && campos[campos.ToList().IndexOf("ValorICMSST") - 1] == "-")
                                custoUnitario = custoUnitario - valorICMSST;
                        }

                        if (valorCreditoPresumido > 0)
                        {
                            if (campos.Contains("ValorCreditoPresumido") && campos[campos.ToList().IndexOf("ValorCreditoPresumido") - 1] == "+")
                                custoUnitario = custoUnitario + valorCreditoPresumido;
                            else if (campos.Contains("ValorCreditoPresumido") && campos[campos.ToList().IndexOf("ValorCreditoPresumido") - 1] == "-")
                                custoUnitario = custoUnitario - valorCreditoPresumido;
                        }

                        if (valorDiferencial > 0)
                        {
                            if (campos.Contains("ValorDiferencial") && campos[campos.ToList().IndexOf("ValorDiferencial") - 1] == "+")
                                custoUnitario = custoUnitario + valorDiferencial;
                            else if (campos.Contains("ValorDiferencial") && campos[campos.ToList().IndexOf("ValorDiferencial") - 1] == "-")
                                custoUnitario = custoUnitario - valorDiferencial;
                        }

                        if (valorFreteFora > 0)
                        {
                            if (campos.Contains("ValorFreteFora") && campos[campos.ToList().IndexOf("ValorFreteFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorFreteFora;
                            else if (campos.Contains("ValorFreteFora") && campos[campos.ToList().IndexOf("ValorFreteFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorFreteFora;
                        }

                        if (valorOutrasFora > 0)
                        {
                            if (campos.Contains("ValorOutrasFora") && campos[campos.ToList().IndexOf("ValorOutrasFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorOutrasFora;
                            else if (campos.Contains("ValorOutrasFora") && campos[campos.ToList().IndexOf("ValorOutrasFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorOutrasFora;
                        }

                        if (valorImpostoFora > 0)
                        {
                            if (campos.Contains("ValorImpostoFora") && campos[campos.ToList().IndexOf("ValorImpostoFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorImpostoFora;
                            else if (campos.Contains("ValorImpostoFora") && campos[campos.ToList().IndexOf("ValorImpostoFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorImpostoFora;
                        }

                        if (valorDiferencialFreteFora > 0)
                        {
                            if (campos.Contains("ValorDiferencialFreteFora") && campos[campos.ToList().IndexOf("ValorDiferencialFreteFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorDiferencialFreteFora;
                            else if (campos.Contains("ValorDiferencialFreteFora") && campos[campos.ToList().IndexOf("ValorDiferencialFreteFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorDiferencialFreteFora;
                        }

                        if (valorPIS > 0)
                        {
                            if (campos.Contains("ValorPIS") && campos[campos.ToList().IndexOf("ValorPIS") - 1] == "+")
                                custoUnitario = custoUnitario + valorPIS;
                            else if (campos.Contains("ValorPIS") && campos[campos.ToList().IndexOf("ValorPIS") - 1] == "-")
                                custoUnitario = custoUnitario - valorPIS;
                        }

                        if (valorCOFINS > 0)
                        {
                            if (campos.Contains("ValorCOFINS") && campos[campos.ToList().IndexOf("ValorCOFINS") - 1] == "+")
                                custoUnitario = custoUnitario + valorCOFINS;
                            else if (campos.Contains("ValorCOFINS") && campos[campos.ToList().IndexOf("ValorCOFINS") - 1] == "-")
                                custoUnitario = custoUnitario - valorCOFINS;
                        }

                        if (valorICMSFreteFora > 0)
                        {
                            if (campos.Contains("ValorICMSFreteFora") && campos[campos.ToList().IndexOf("ValorICMSFreteFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorICMSFreteFora;
                            else if (campos.Contains("ValorICMSFreteFora") && campos[campos.ToList().IndexOf("ValorICMSFreteFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorICMSFreteFora;
                        }

                        if (valorDescontoFora > 0)
                        {
                            if (campos.Contains("ValorDescontoFora") && campos[campos.ToList().IndexOf("ValorDescontoFora") - 1] == "+")
                                custoUnitario = custoUnitario + valorDescontoFora;
                            else if (campos.Contains("ValorDescontoFora") && campos[campos.ToList().IndexOf("ValorDescontoFora") - 1] == "-")
                                custoUnitario = custoUnitario - valorDescontoFora;
                        }

                        custoTotal = custoUnitario;
                        custoUnitario = custoUnitario / quantidade;

                        if (custoUnitario > 0 && custoTotal > 0)
                        {
                            item.ValorCustoUnitario = custoUnitario;
                            item.ValorCustoTotal = custoTotal;
                        }
                    }
                }


                repItem.Inserir(item);
            }

            dynamic dynRetorno = new
            {
                totalBaseCalculoICMS,
                valorTotalCOFINS,
                valorTotalICMS,
                valorTotalIPI,
                valorTotalPIS,
                valorTotalCreditoPresumido,
                valorTotalDiferencial,
                valorTotalCusto,
                totalBaseSTRetido,
                totalValorSTRetido,
                valorTotalRetencaoPIS,
                valorTotalRetencaoCOFINS,
                valorTotalRetencaoINSS,
                valorTotalRetencaoIPI,
                valorTotalRetencaoCSLL,
                valorTotalRetencaoOutras,
                valorTotalRetencaoIR,
                valorTotalRetencaoISS
            };

            return dynRetorno;
        }

        private void SetarICMS(ref Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item, MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var icms = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMS) select (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMS)obj).FirstOrDefault();

                if (icms != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoICMS = icms.Item.GetType();

                    if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00 impICMS00 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS00)icms.Item;

                        item.AliquotaICMS = decimal.Parse(impICMS00.pICMS, cultura);
                        item.BaseCalculoICMS = decimal.Parse(impICMS00.vBC, cultura);
                        item.ValorICMS = decimal.Parse(impICMS00.vICMS, cultura);
                        item.CSTICMS = string.Format("{0:00}", (int)impICMS00.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10 impICMS10 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS10)icms.Item;

                        item.AliquotaICMS = decimal.Parse(impICMS10.pICMS, cultura);
                        item.BaseCalculoICMS = decimal.Parse(impICMS10.vBC, cultura);
                        item.ValorICMS = decimal.Parse(impICMS10.vICMS, cultura);
                        item.BaseCalculoICMSST = decimal.Parse(impICMS10.vBCST, cultura);
                        item.ValorICMSST = decimal.Parse(impICMS10.vICMSST, cultura);
                        item.CSTICMS = string.Format("{0:00}", (int)impICMS10.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20 impICMS20 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS20)icms.Item;

                        item.AliquotaICMS = decimal.Parse(impICMS20.pICMS, cultura);
                        item.BaseCalculoICMS = decimal.Parse(impICMS20.vBC, cultura);
                        item.ValorICMS = decimal.Parse(impICMS20.vICMS, cultura);
                        item.CSTICMS = string.Format("{0:00}", (int)impICMS20.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30 impICMS30 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS30)icms.Item;

                        item.BaseCalculoICMSST = decimal.Parse(impICMS30.vBCST, cultura);
                        item.ValorICMSST = decimal.Parse(impICMS30.vICMSST, cultura);
                        item.CSTICMS = string.Format("{0:00}", (int)impICMS30.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40 impICMS40 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS40)icms.Item;

                        item.CSTICMS = string.Format("{0:00}", (int)impICMS40.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51 impICMS51 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS51)icms.Item;

                        item.AliquotaICMS = decimal.Parse(impICMS51.pICMS, cultura);
                        item.BaseCalculoICMS = decimal.Parse(impICMS51.vBC, cultura);
                        item.ValorICMS = decimal.Parse(impICMS51.vICMS, cultura);
                        item.CSTICMS = string.Format("{0:00}", (int)impICMS51.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60 impICMS60 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS60)icms.Item;

                        item.CSTICMS = string.Format("{0:00}", (int)impICMS60.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70 impICMS70 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS70)icms.Item;

                        item.AliquotaICMS = decimal.Parse(impICMS70.pICMS, cultura);
                        item.BaseCalculoICMS = decimal.Parse(impICMS70.vBC, cultura);
                        item.ValorICMS = decimal.Parse(impICMS70.vICMS, cultura);
                        item.BaseCalculoICMSST = decimal.Parse(impICMS70.vBCST, cultura);
                        item.ValorICMSST = decimal.Parse(impICMS70.vICMSST, cultura);
                        item.CSTICMS = string.Format("{0:00}", (int)impICMS70.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90 impICMS90 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMS90)icms.Item;

                        item.AliquotaICMS = decimal.Parse(impICMS90.pICMS, cultura);
                        item.BaseCalculoICMS = decimal.Parse(impICMS90.vBC, cultura);
                        item.ValorICMS = decimal.Parse(impICMS90.vICMS, cultura);
                        item.BaseCalculoICMSST = decimal.Parse(impICMS90.vBCST, cultura);
                        item.ValorICMSST = decimal.Parse(impICMS90.vICMSST, cultura);
                        item.CSTICMS = string.Format("{0:00}", (int)impICMS90.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart impICMSPart = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSPart)icms.Item;

                        item.AliquotaICMS = decimal.Parse(impICMSPart.pICMS, cultura);
                        item.BaseCalculoICMS = decimal.Parse(impICMSPart.vBC, cultura);
                        item.ValorICMS = decimal.Parse(impICMSPart.vICMS, cultura);
                        item.BaseCalculoICMSST = decimal.Parse(impICMSPart.vBCST, cultura);
                        item.ValorICMSST = decimal.Parse(impICMSPart.vICMSST, cultura);
                        item.CSTICMS = string.Format("{0:00}", (int)impICMSPart.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST impICMSST = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSST)icms.Item;

                        item.CSTICMS = string.Format("{0:00}", (int)impICMSST.CST);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101 impICMSSN101 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN101)icms.Item;

                        item.CSTICMS = string.Format("{0:000}", (int)impICMSSN101.CSOSN);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102 impICMSSN102 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN102)icms.Item;

                        item.CSTICMS = string.Format("{0:000}", (int)impICMSSN102.CSOSN);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201 impICMSSN201 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN201)icms.Item;

                        item.BaseCalculoICMSST = decimal.Parse(impICMSSN201.vBCST, cultura);
                        item.ValorICMSST = decimal.Parse(impICMSSN201.vICMSST, cultura);
                        item.CSTICMS = string.Format("{0:000}", (int)impICMSSN201.CSOSN);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202 impICMSSN202 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN202)icms.Item;

                        item.BaseCalculoICMSST = decimal.Parse(impICMSSN202.vBCST, cultura);
                        item.ValorICMSST = decimal.Parse(impICMSSN202.vICMSST, cultura);
                        item.CSTICMS = string.Format("{0:000}", (int)impICMSSN202.CSOSN);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500 impICMSSN500 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN500)icms.Item;

                        item.CSTICMS = string.Format("{0:000}", (int)impICMSSN500.CSOSN);
                    }
                    else if (tipoICMS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900 impICMSSN900 = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoICMSICMSSN900)icms.Item;

                        item.AliquotaICMS = impICMSSN900.pICMS != null ? decimal.Parse(impICMSSN900.pICMS, cultura) : 0m;
                        item.BaseCalculoICMS = impICMSSN900.vBC != null ? decimal.Parse(impICMSSN900.vBC, cultura) : 0m;
                        item.ValorICMS = impICMSSN900.vICMS != null ? decimal.Parse(impICMSSN900.vICMS, cultura) : 0m;
                        item.BaseCalculoICMSST = impICMSSN900.vBCST != null ? decimal.Parse(impICMSSN900.vBCST, cultura) : 0m;
                        item.ValorICMSST = impICMSSN900.vICMSST != null ? decimal.Parse(impICMSSN900.vICMSST, cultura) : 0m;
                        item.CSTICMS = string.Format("{0:000}", (int)impICMSSN900.CSOSN);
                    }
                }
                else
                {
                    item.CSTICMS = string.Empty;
                }
            }
            else
            {
                item.CSTICMS = string.Empty;
            }
        }

        private void SetarIPI(ref Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item, MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImposto infNFeDetImposto)
        {
            if (infNFeDetImposto != null)
            {
                var ipi = (from obj in infNFeDetImposto.Items where obj.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscal.TIpi) select (MultiSoftware.NFe.v310.NotaFiscal.TIpi)obj).FirstOrDefault();

                if (ipi != null)
                {
                    System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                    var tipoIPI = ipi.Item.GetType();

                    if (tipoIPI == typeof(MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib))
                    {
                        MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib impIPITrib = (MultiSoftware.NFe.v310.NotaFiscal.TIpiIPITrib)ipi.Item;

                        decimal baseCalculo = 0m;
                        decimal aliquota = 0m;

                        if (impIPITrib.ItemsElementName != null && impIPITrib.Items != null)
                        {
                            if (impIPITrib.ItemsElementName[0] == MultiSoftware.NFe.v310.NotaFiscal.ItemsChoiceType.vBC && impIPITrib.ItemsElementName[1] == MultiSoftware.NFe.v310.NotaFiscal.ItemsChoiceType.pIPI)
                            {
                                baseCalculo = decimal.Parse(impIPITrib.Items[0], cultura);
                                aliquota = decimal.Parse(impIPITrib.Items[1], cultura);
                            }
                        }

                        item.BaseCalculoIPI = baseCalculo;
                        item.AliquotaIPI = aliquota;
                        item.ValorIPI = decimal.Parse(impIPITrib.vIPI, cultura);
                        item.CSTIPI = string.Format("{0:00}", (int)impIPITrib.CST);
                    }
                }
                else
                {
                    item.CSTIPI = string.Empty;
                }
            }
        }

        private void SetarPIS(ref Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item, MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPIS infNFeDetImpostoPIS)
        {
            if (infNFeDetImpostoPIS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoPIS = infNFeDetImpostoPIS.Item.GetType();

                if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq impPISAliq = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISAliq)infNFeDetImpostoPIS.Item;

                    item.CSTPIS = string.Format("{0:00}", (int)impPISAliq.CST);
                    item.ValorPIS = decimal.Parse(impPISAliq.vPIS, cultura);
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr impPISOutr = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISOutr)infNFeDetImpostoPIS.Item;

                    item.CSTPIS = string.Format("{0:00}", (int)impPISOutr.CST);
                    item.ValorPIS = decimal.Parse(impPISOutr.vPIS, cultura);
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde impPISQtde = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISQtde)infNFeDetImpostoPIS.Item;

                    item.CSTPIS = string.Format("{0:00}", (int)impPISQtde.CST);
                    item.ValorPIS = decimal.Parse(impPISQtde.vPIS, cultura);
                }
                else if (tipoPIS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT impPISNT = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoPISPISNT)infNFeDetImpostoPIS.Item;

                    item.CSTPIS = string.Format("{0:00}", (int)impPISNT.CST);
                    item.ValorPIS = 0m;
                }
            }
        }

        private void SetarCOFINS(ref Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item, MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINS infNFeDetImpostoCOFINS)
        {
            if (infNFeDetImpostoCOFINS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                var tipoCOFINS = infNFeDetImpostoCOFINS.Item.GetType();

                if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq impCOFINSAliq = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSAliq)infNFeDetImpostoCOFINS.Item;

                    item.CSTCOFINS = string.Format("{0:00}", (int)impCOFINSAliq.CST);
                    item.ValorCOFINS = decimal.Parse(impCOFINSAliq.vCOFINS, cultura);
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr impCOFINSOutr = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSOutr)infNFeDetImpostoCOFINS.Item;

                    item.CSTCOFINS = string.Format("{0:00}", (int)impCOFINSOutr.CST);
                    item.ValorCOFINS = decimal.Parse(impCOFINSOutr.vCOFINS, cultura);
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde impCOFINSQtde = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSQtde)infNFeDetImpostoCOFINS.Item;

                    item.CSTCOFINS = string.Format("{0:00}", (int)impCOFINSQtde.CST);
                    item.ValorCOFINS = decimal.Parse(impCOFINSQtde.vCOFINS, cultura);
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST impCOFINSST = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSST)infNFeDetImpostoCOFINS.Item;

                    item.CSTCOFINS = "";
                    item.ValorCOFINS = decimal.Parse(impCOFINSST.vCOFINS, cultura);
                }
                else if (tipoCOFINS == typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT))
                {
                    MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT impCOFINSNT = (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDetImpostoCOFINSCOFINSNT)infNFeDetImpostoCOFINS.Item;

                    item.CSTCOFINS = string.Format("{0:00}", (int)impCOFINSNT.CST);
                    item.ValorCOFINS = 0m;

                }
            }
        }

        private void SalvarDuplicatas(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeCobr cobranca, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unidadeTrabalho, out bool todasInformacoesLancadas)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeTrabalho);
            todasInformacoesLancadas = false;

            Dominio.Entidades.Cliente fornecedor = documentoEntrada.Fornecedor;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = fornecedor?.GrupoPessoas ?? null;

            bool usaGrupoPessoas = false;
            bool ignorarDuplicataRecebidaXMLNotaEntrada = (grupoPessoas?.IgnorarDuplicataRecebidaXMLNotaEntrada ?? false) || (fornecedor?.IgnorarDuplicataRecebidaXMLNotaEntrada ?? false);
            bool gerarDuplicataNotaEntrada = false;
            int parcelasDuplicataNotaEntrada = 0;
            string intervaloDiasDuplicataNotaEntrada = string.Empty;
            int diaPadraoDuplicataNotaEntrada = 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo = fornecedor?.FormaTituloFornecedor ?? grupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;

            if (fornecedor != null && fornecedor.GerarDuplicataNotaEntrada)
            {
                gerarDuplicataNotaEntrada = fornecedor.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = fornecedor.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = fornecedor.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = fornecedor.DiaPadraoDuplicataNotaEntrada;
            }
            else if (grupoPessoas != null && grupoPessoas.GerarDuplicataNotaEntrada)
            {
                usaGrupoPessoas = true;
                gerarDuplicataNotaEntrada = grupoPessoas.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = grupoPessoas.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = grupoPessoas.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = grupoPessoas.DiaPadraoDuplicataNotaEntrada;
            }

            int quantidadeDuplicatas = 0;

            if (cobranca != null && cobranca.dup != null && !ignorarDuplicataRecebidaXMLNotaEntrada)
            {
                quantidadeDuplicatas = cobranca.dup.Count();

                for (int i = 0; i < quantidadeDuplicatas; i++) // (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeCobrDup dup in cobranca.dup)
                {
                    todasInformacoesLancadas = true;
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicata = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata();

                    duplicata.DocumentoEntrada = documentoEntrada;
                    duplicata.DataVencimento = cobranca.dup[i].dVenc != null ? DateTime.ParseExact(cobranca.dup[i].dVenc, "yyyy-MM-dd", null) : DateTime.Now;

                    duplicata.Sequencia = i + 1;
                    if (string.IsNullOrWhiteSpace(cobranca.dup[i].nDup))
                        duplicata.Numero = documentoEntrada.Numero.ToString() + "/" + (i + 1);
                    else
                        duplicata.Numero = cobranca.dup[i].nDup;

                    duplicata.Valor = decimal.Parse(cobranca.dup[i].vDup, cultura);
                    duplicata.Forma = formaTitulo;

                    repDuplicata.Inserir(duplicata);
                }
            }
            else if (gerarDuplicataNotaEntrada && parcelasDuplicataNotaEntrada > 0)
            {
                Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unidadeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento repGrupoPessoasVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento(unidadeTrabalho);

                bool permiteMultiplosVencimentos;
                if (usaGrupoPessoas)
                    permiteMultiplosVencimentos = grupoPessoas.PermitirMultiplosVencimentos;
                else
                    permiteMultiplosVencimentos = fornecedor.Modalidades?.Where(f => f.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)?.FirstOrDefault()?.ModalidadesFornecedores?.FirstOrDefault()?.PermitirMultiplosVencimentos ?? false;

                if (permiteMultiplosVencimentos)
                {
                    int diaEmissao = documentoEntrada.DataEmissao.Date.Day;
                    int diaVencimento = 0;

                    if (usaGrupoPessoas)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento = repGrupoPessoasVencimento.BuscarDiaVencimento(grupoPessoas.Codigo, diaEmissao);
                        diaVencimento = vencimento?.Vencimento ?? 0;
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = repVencimento.BuscarDiaVencimento(fornecedor.CPF_CNPJ, diaEmissao);
                        diaVencimento = vencimento?.Vencimento ?? 0;
                    }

                    if (diaVencimento > 0)
                    {
                        DateTime novaData = ProximaDataTabelaVencimento(documentoEntrada.DataEmissao, diaVencimento);

                        Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata dup = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata()
                        {
                            DataVencimento = novaData,
                            Sequencia = 1,
                            Numero = (documentoEntrada.Numero.ToString("D") + "/1"),
                            Valor = documentoEntrada.ValorTotal,
                            DocumentoEntrada = documentoEntrada,
                            NumeroBoleto = string.Empty,
                            Portador = null,
                            Observacao = string.Empty,
                            Forma = formaTitulo
                        };
                        repDuplicata.Inserir(dup);
                    }
                }
                else
                {
                    quantidadeDuplicatas = parcelasDuplicataNotaEntrada;

                    decimal valorTotal = documentoEntrada.ValorTotal;
                    decimal valorParcela = Math.Round((valorTotal / quantidadeDuplicatas), 2);
                    decimal valorDiferenca = valorTotal - Math.Round((valorParcela * quantidadeDuplicatas), 2);
                    string[] arrayDias = null;

                    var x = intervaloDiasDuplicataNotaEntrada;
                    if (x.IndexOf(".") >= 0)
                    {
                        arrayDias = x.Split('.');
                        if (arrayDias.Length != quantidadeDuplicatas)
                        {
                            return;
                        }
                        for (var i = 0; i < arrayDias.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(arrayDias[i]) || !(int.Parse(arrayDias[i]) > 0))
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        arrayDias = new string[1];
                        arrayDias[0] = x;
                        if (string.IsNullOrWhiteSpace(arrayDias[0]) || !(int.Parse(arrayDias[0]) > 0))
                        {
                            return;
                        }
                    }
                    var dataVencimento = documentoEntrada.DataEmissao;

                    for (var i = 0; i < quantidadeDuplicatas; i++)
                    {
                        todasInformacoesLancadas = true;
                        decimal valor = 0;
                        if (i == 0)
                            valor = Math.Round((valorParcela + valorDiferenca), 2);
                        else
                            valor = Math.Round(valorParcela, 2);

                        if (arrayDias.Length > 1)
                            dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[i]));
                        else
                            dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[0]));

                        DateTime novaData = dataVencimento;
                        if (i == 0 && diaPadraoDuplicataNotaEntrada > 0 && diaPadraoDuplicataNotaEntrada <= 31)
                        {
                            try
                            {
                                if (dataVencimento.Day > diaPadraoDuplicataNotaEntrada)
                                    dataVencimento = dataVencimento.AddMonths(1);

                                novaData = new DateTime(dataVencimento.Year, dataVencimento.Month, diaPadraoDuplicataNotaEntrada);
                            }
                            catch
                            {
                                novaData = dataVencimento;
                            }
                        }
                        dataVencimento = novaData;

                        Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicata = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata();
                        duplicata.DocumentoEntrada = documentoEntrada;
                        duplicata.DataVencimento = dataVencimento;
                        duplicata.Sequencia = i + 1;
                        duplicata.Numero = documentoEntrada.Numero.ToString() + "/" + (i + 1);
                        duplicata.Valor = valor;
                        duplicata.Forma = formaTitulo;
                        repDuplicata.Inserir(duplicata);
                    }
                }
            }
        }

        private void SalvarDuplicatas(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeCobr cobranca, System.Globalization.CultureInfo cultura, Repositorio.UnitOfWork unidadeTrabalho, out bool todasInformacoesLancadas, out int quantidadeDuplicatas)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeTrabalho);
            todasInformacoesLancadas = false;
            quantidadeDuplicatas = 0;

            Dominio.Entidades.Cliente fornecedor = documentoEntrada.Fornecedor;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = fornecedor?.GrupoPessoas ?? null;

            bool usaGrupoPessoas = false;
            bool ignorarDuplicataRecebidaXMLNotaEntrada = (grupoPessoas?.IgnorarDuplicataRecebidaXMLNotaEntrada ?? false) || (fornecedor?.IgnorarDuplicataRecebidaXMLNotaEntrada ?? false);
            bool gerarDuplicataNotaEntrada = false;
            int parcelasDuplicataNotaEntrada = 0;
            string intervaloDiasDuplicataNotaEntrada = string.Empty;
            int diaPadraoDuplicataNotaEntrada = 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo = fornecedor?.FormaTituloFornecedor ?? grupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;

            if (fornecedor != null && fornecedor.GerarDuplicataNotaEntrada)
            {
                gerarDuplicataNotaEntrada = fornecedor.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = fornecedor.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = fornecedor?.IntervaloDiasDuplicataNotaEntrada ?? "";
                diaPadraoDuplicataNotaEntrada = fornecedor.DiaPadraoDuplicataNotaEntrada;
            }
            else if (grupoPessoas != null && grupoPessoas.GerarDuplicataNotaEntrada)
            {
                usaGrupoPessoas = true;
                gerarDuplicataNotaEntrada = grupoPessoas.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = grupoPessoas.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = grupoPessoas.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = grupoPessoas.DiaPadraoDuplicataNotaEntrada;
            }

            if (cobranca != null && cobranca.dup != null && !ignorarDuplicataRecebidaXMLNotaEntrada)
            {
                quantidadeDuplicatas = cobranca.dup.Count();

                for (int i = 0; i < quantidadeDuplicatas; i++) // (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeCobrDup dup in cobranca.dup)
                {
                    todasInformacoesLancadas = true;
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicata = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata();

                    duplicata.DocumentoEntrada = documentoEntrada;
                    duplicata.DataVencimento = cobranca.dup[i].dVenc != null ? DateTime.ParseExact(cobranca.dup[i].dVenc, "yyyy-MM-dd", null) : DateTime.Now;
                    duplicata.Sequencia = i + 1;

                    if (string.IsNullOrWhiteSpace(cobranca.dup[i].nDup))
                        duplicata.Numero = documentoEntrada.Numero.ToString() + "/" + (i + 1);
                    else
                        duplicata.Numero = cobranca.dup[i].nDup;

                    duplicata.Valor = decimal.Parse(cobranca.dup[i].vDup, cultura);
                    duplicata.Forma = formaTitulo;

                    repDuplicata.Inserir(duplicata);
                }
            }
            else if (gerarDuplicataNotaEntrada && parcelasDuplicataNotaEntrada > 0)
            {
                Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unidadeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento repGrupoPessoasVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento(unidadeTrabalho);

                bool permiteMultiplosVencimentos;
                if (usaGrupoPessoas)
                    permiteMultiplosVencimentos = grupoPessoas.PermitirMultiplosVencimentos;
                else
                    permiteMultiplosVencimentos = fornecedor.Modalidades?.Where(f => f.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)?.FirstOrDefault()?.ModalidadesFornecedores?.FirstOrDefault()?.PermitirMultiplosVencimentos ?? false;

                if (permiteMultiplosVencimentos)
                {
                    int diaEmissao = documentoEntrada.DataEmissao.Date.Day;
                    int diaVencimento = 0;

                    if (usaGrupoPessoas)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento = repGrupoPessoasVencimento.BuscarDiaVencimento(grupoPessoas.Codigo, diaEmissao);
                        diaVencimento = vencimento?.Vencimento ?? 0;
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = repVencimento.BuscarDiaVencimento(fornecedor.CPF_CNPJ, diaEmissao);
                        diaVencimento = vencimento?.Vencimento ?? 0;
                    }

                    if (diaVencimento > 0)
                    {
                        todasInformacoesLancadas = true;
                        DateTime novaData = ProximaDataTabelaVencimento(documentoEntrada.DataEmissao, diaVencimento);

                        Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata dup = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata()
                        {
                            DataVencimento = novaData,
                            Sequencia = 1,
                            Numero = (documentoEntrada.Numero.ToString("D") + "/1"),
                            Valor = documentoEntrada.ValorTotal,
                            DocumentoEntrada = documentoEntrada,
                            NumeroBoleto = string.Empty,
                            Portador = null,
                            Observacao = string.Empty,
                            Forma = formaTitulo
                        };
                        repDuplicata.Inserir(dup);
                        quantidadeDuplicatas++;
                    }
                }
                else
                {
                    quantidadeDuplicatas = parcelasDuplicataNotaEntrada;
                    decimal valorTotal = documentoEntrada.ValorTotal;
                    decimal valorParcela = Math.Round((valorTotal / quantidadeDuplicatas), 2);
                    decimal valorDiferenca = valorTotal - Math.Round((valorParcela * quantidadeDuplicatas), 2);
                    string[] arrayDias = null;

                    var x = intervaloDiasDuplicataNotaEntrada;
                    if (x.IndexOf(".") >= 0)
                    {
                        arrayDias = x.Split('.');
                        if (arrayDias.Length != quantidadeDuplicatas)
                        {
                            return;
                        }
                        for (var i = 0; i < arrayDias.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(arrayDias[i]) || !(int.Parse(arrayDias[i]) > 0))
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        arrayDias = new string[1];
                        arrayDias[0] = x;
                        if (string.IsNullOrWhiteSpace(arrayDias[0]) || !(int.Parse(arrayDias[0]) > 0))
                        {
                            return;
                        }
                    }
                    var dataVencimento = documentoEntrada.DataEmissao;

                    for (var i = 0; i < quantidadeDuplicatas; i++)
                    {
                        todasInformacoesLancadas = true;
                        decimal valor = 0;
                        if (i == 0)
                            valor = Math.Round((valorParcela + valorDiferenca), 2);
                        else
                            valor = Math.Round(valorParcela, 2);

                        if (arrayDias.Length > 1)
                            dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[i]));
                        else
                            dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[0]));

                        DateTime novaData = dataVencimento;
                        if (i == 0 && diaPadraoDuplicataNotaEntrada > 0 && diaPadraoDuplicataNotaEntrada <= 31)
                        {
                            try
                            {
                                if (dataVencimento.Day > diaPadraoDuplicataNotaEntrada)
                                    dataVencimento = dataVencimento.AddMonths(1);

                                novaData = new DateTime(dataVencimento.Year, dataVencimento.Month, diaPadraoDuplicataNotaEntrada);
                            }
                            catch
                            {
                                novaData = dataVencimento;
                            }
                        }
                        dataVencimento = novaData;

                        Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata duplicata = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata();
                        duplicata.DocumentoEntrada = documentoEntrada;
                        duplicata.DataVencimento = dataVencimento;
                        duplicata.Sequencia = i + 1;
                        duplicata.Numero = documentoEntrada.Numero.ToString() + "/" + (i + 1);
                        duplicata.Valor = valor;
                        duplicata.Forma = formaTitulo;
                        repDuplicata.Inserir(duplicata);
                    }
                }
            }
        }

        private Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento RetornaRegraEntrada(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente fornecedor, string ncm, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            bool naoUtilizarRegraEntradaDocumentoGrupoNCM = repConfiguracaoTMS.NaoUtilizarRegraEntradaDocumentoGrupoNCM();

            if (string.IsNullOrWhiteSpace(ncm) && naoUtilizarRegraEntradaDocumentoGrupoNCM)
                return null;

            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraNCMCompleto = repRegraEntradaDocumento.BuscarRegraEntradaNCMCompleto(empresa?.Codigo ?? 0, fornecedor.CPF_CNPJ, ncm);
            if (regraNCMCompleto == null && !naoUtilizarRegraEntradaDocumentoGrupoNCM)
                return repRegraEntradaDocumento.BuscarRegraEntrada(empresa?.Codigo ?? 0, fornecedor.CPF_CNPJ, ncm);
            else
                return regraNCMCompleto;
        }

        private decimal CalcularImposto(decimal baseCalculo, decimal aliquota, decimal percentualReducaoBC)
        {
            if (baseCalculo > 0 && aliquota > 0)
                return baseCalculo * (1 - (percentualReducaoBC / 100)) * (aliquota / 100);
            else
                return 0m;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.DadosRegraEntradaDocumento RetornaDadosEntradaDocumento(decimal baseCalculoImposto, Dominio.Entidades.CFOP cfopNota, Dominio.Entidades.Empresa destinatario, Dominio.Entidades.Cliente emitente, Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.DadosRegraEntradaDocumento regra = new Dominio.ObjetosDeValor.Embarcador.Financeiro.DadosRegraEntradaDocumento();

            regra.BaseCalculoPIS = 0;
            regra.PercentualReducaoPIS = 0;
            regra.AliquotaPIS = 0;
            regra.ValorPIS = 0;
            regra.CSTPIS = "";
            if (cfopNota != null)
            {
                if (cfopNota.AliquotaPIS > 0)
                    regra.BaseCalculoPIS = baseCalculoImposto;
                regra.PercentualReducaoPIS = cfopNota.ReducaoBCPIS;
                regra.AliquotaPIS = cfopNota.AliquotaPIS;
                regra.ValorPIS = CalcularImposto(regra.BaseCalculoPIS, regra.AliquotaPIS, regra.PercentualReducaoPIS);
                regra.CSTPIS = cfopNota.NumeroCSTPIS;
            }

            regra.BaseCalculoCOFINS = 0;
            regra.PercentualReducaoCOFINS = 0;
            regra.AliquotaCOFINS = 0;
            regra.ValorCOFINS = 0;
            regra.CSTCOFINS = "";
            if (cfopNota != null)
            {
                if (cfopNota.AliquotaCOFINS > 0)
                    regra.BaseCalculoCOFINS = baseCalculoImposto;
                regra.PercentualReducaoCOFINS = cfopNota.ReducaoBCCOFINS;
                regra.AliquotaCOFINS = cfopNota.AliquotaCOFINS;
                regra.ValorCOFINS = CalcularImposto(regra.BaseCalculoCOFINS, regra.AliquotaCOFINS, regra.PercentualReducaoCOFINS);
                regra.CSTCOFINS = cfopNota.NumeroCSTCOFINS;
            }

            regra.BaseCalculoIPI = 0;
            regra.PercentualReducaoIPI = 0;
            regra.AliquotaIPI = 0;
            regra.ValorIPICFOP = 0;
            regra.CSTIPI = "";
            if (cfopNota != null)
            {
                if (cfopNota.AliquotaIPI > 0)
                    regra.BaseCalculoIPI = baseCalculoImposto;
                regra.PercentualReducaoIPI = cfopNota.ReducaoBCIPI;
                regra.AliquotaIPI = cfopNota.AliquotaIPI;
                regra.ValorIPICFOP = CalcularImposto(regra.BaseCalculoIPI, regra.AliquotaIPI, regra.PercentualReducaoIPI);
                regra.CSTIPI = cfopNota.NumeroCSTIPI;
            }

            regra.CSTICMS = "";
            regra.AliquotaICMS = 0;
            regra.BaseICMS = 0;
            regra.ValorICMS = 0;
            regra.AliquotaCreditoPresumido = 0;
            regra.BaseCalculoCreditoPresumido = 0;
            regra.ValorCreditoPresumido = 0;
            regra.AliquotaDiferencial = 0;
            regra.BaseCalculoDiferencial = 0;
            regra.ValorDiferencial = 0;
            if (cfopNota != null)
            {
                regra.CSTICMS = cfopNota.NumeroCSTICMS;

                if (regraEntradaDocumento != null && regraEntradaDocumento.NaoTributarICMS)
                {
                    regra.AliquotaICMS = 0;
                    regra.BaseICMS = 0;
                    regra.ValorICMS = 0;
                }
                else
                {
                    if (destinatario != null && emitente != null && destinatario.Localidade.Estado.Sigla == emitente.Localidade.Estado.Sigla)
                        regra.AliquotaICMS = cfopNota.AliquotaICMSInterna;
                    else
                        regra.AliquotaICMS = cfopNota.AliquotaICMSInterestadual;
                    if (regra.AliquotaICMS > 0)
                        regra.BaseICMS = baseCalculoImposto;
                    regra.ValorICMS = CalcularImposto(regra.BaseICMS, regra.AliquotaICMS, 0);
                }

                if (cfopNota.CreditoSobreTotalParaItensSujeitosICMSST && regra.ValorICMS > 0)
                {
                    regra.BaseCalculoDiferencial = regra.BaseICMS;
                    regra.AliquotaDiferencial = regra.AliquotaICMS;
                    regra.ValorDiferencial = regra.ValorICMS;

                    regra.BaseICMS = 0;
                    regra.AliquotaICMS = 0;
                    regra.ValorICMS = 0;
                }
                else
                {
                    regra.AliquotaDiferencial = cfopNota.AliquotaDiferencial;
                    if (regra.AliquotaDiferencial > 0)
                        regra.BaseCalculoDiferencial = baseCalculoImposto;
                    regra.ValorDiferencial = CalcularImposto(regra.BaseCalculoDiferencial, regra.AliquotaDiferencial, 0);
                }

                if (cfopNota.CreditoSobreTotalParaProdutosUsoConsumo && regra.AliquotaCreditoPresumido > 0)
                {
                    regra.BaseCalculoCreditoPresumido = regra.BaseICMS;
                    regra.AliquotaCreditoPresumido = regra.AliquotaICMS;
                    regra.ValorCreditoPresumido = regra.ValorICMS;

                    regra.BaseICMS = 0;
                    regra.AliquotaICMS = 0;
                    regra.ValorICMS = 0;
                }
                else
                {
                    regra.AliquotaCreditoPresumido = cfopNota.AliquotaParaCredito;
                    if (regra.AliquotaCreditoPresumido > 0)
                        regra.BaseCalculoCreditoPresumido = baseCalculoImposto;
                    regra.ValorCreditoPresumido = CalcularImposto(regra.BaseCalculoCreditoPresumido, regra.AliquotaCreditoPresumido, 0);
                }
            }

            regra.ValorRetencaoPIS = 0;
            regra.ValorRetencaoCOFINS = 0;
            regra.ValorRetencaoINSS = 0;
            regra.ValorRetencaoIPI = 0;
            regra.ValorRetencaoCSLL = 0;
            regra.ValorRetencaoOutras = 0;
            regra.ValorRetencaoIR = 0;
            regra.ValorRetencaoISS = 0;
            if (cfopNota != null)
            {
                regra.ValorRetencaoPIS = CalcularImposto(baseCalculoImposto, cfopNota.AliquotaRetencaoPIS, 0);
                regra.ValorRetencaoCOFINS = CalcularImposto(baseCalculoImposto, cfopNota.AliquotaRetencaoCOFINS, 0);
                regra.ValorRetencaoINSS = CalcularImposto(baseCalculoImposto, cfopNota.AliquotaRetencaoINSS, 0);
                regra.ValorRetencaoIPI = CalcularImposto(baseCalculoImposto, cfopNota.AliquotaRetencaoIPI, 0);
                regra.ValorRetencaoCSLL = CalcularImposto(baseCalculoImposto, cfopNota.AliquotaRetencaoCSLL, 0);
                regra.ValorRetencaoOutras = CalcularImposto(baseCalculoImposto, cfopNota.AliquotaRetencaoOutras, 0);
                regra.ValorRetencaoIR = CalcularImposto(baseCalculoImposto, cfopNota.AliquotaRetencaoIR, 0);
                regra.ValorRetencaoISS = CalcularImposto(baseCalculoImposto, cfopNota.AliquotaRetencaoISS, 0);
            }

            return regra;
        }

        private bool LancarDuplicatasAutomaticas(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento repVencimento = new Repositorio.Embarcador.Pessoas.ClienteFornecedorVencimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento repGrupoPessoasVencimento = new Repositorio.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento(unidadeDeTrabalho);

            Dominio.Entidades.Cliente fornecedor = documentoEntrada.Fornecedor;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = fornecedor?.GrupoPessoas ?? null;

            bool usaGrupoPessoas = false;
            bool gerarDuplicataNotaEntrada = false;
            int parcelasDuplicataNotaEntrada = 0;
            string intervaloDiasDuplicataNotaEntrada = string.Empty;
            int diaPadraoDuplicataNotaEntrada = 0;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo = fornecedor?.FormaTituloFornecedor ?? grupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;

            if (fornecedor != null && fornecedor.GerarDuplicataNotaEntrada)
            {
                gerarDuplicataNotaEntrada = fornecedor.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = fornecedor.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = fornecedor.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = fornecedor.DiaPadraoDuplicataNotaEntrada;
            }
            else if (grupoPessoas != null && grupoPessoas.GerarDuplicataNotaEntrada)
            {
                usaGrupoPessoas = true;
                gerarDuplicataNotaEntrada = grupoPessoas.GerarDuplicataNotaEntrada;
                parcelasDuplicataNotaEntrada = grupoPessoas.ParcelasDuplicataNotaEntrada;
                intervaloDiasDuplicataNotaEntrada = grupoPessoas.IntervaloDiasDuplicataNotaEntrada;
                diaPadraoDuplicataNotaEntrada = grupoPessoas.DiaPadraoDuplicataNotaEntrada;
            }

            if (!gerarDuplicataNotaEntrada || parcelasDuplicataNotaEntrada == 0)
                return false;

            int quantidadeParcelas = parcelasDuplicataNotaEntrada;
            decimal valorTotal = documentoEntrada.ValorTotal;
            decimal valorParcela = Math.Round((valorTotal / quantidadeParcelas), 2);
            decimal valorDiferenca = valorTotal - Math.Round((valorParcela * quantidadeParcelas), 2);
            string[] arrayDias = null;

            bool permiteMultiplosVencimentos;
            if (usaGrupoPessoas)
                permiteMultiplosVencimentos = grupoPessoas.PermitirMultiplosVencimentos;
            else
                permiteMultiplosVencimentos = fornecedor.Modalidades?.Where(f => f.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor)?.FirstOrDefault()?.ModalidadesFornecedores?.FirstOrDefault()?.PermitirMultiplosVencimentos ?? false;

            if (permiteMultiplosVencimentos)
            {
                int diaEmissao = documentoEntrada.DataEmissao.Date.Day;
                int diaVencimento = 0;

                if (usaGrupoPessoas)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFornecedorVencimento vencimento = repGrupoPessoasVencimento.BuscarDiaVencimento(grupoPessoas.Codigo, diaEmissao);
                    diaVencimento = vencimento?.Vencimento ?? 0;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteFornecedorVencimento vencimento = repVencimento.BuscarDiaVencimento(fornecedor.CPF_CNPJ, diaEmissao);
                    diaVencimento = vencimento?.Vencimento ?? 0;
                }

                if (diaVencimento > 0)
                {
                    DateTime novaData = ProximaDataTabelaVencimento(documentoEntrada.DataEmissao, diaVencimento);

                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata dup = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata()
                    {
                        DataVencimento = novaData,
                        Sequencia = 1,
                        Numero = (documentoEntrada.Numero.ToString("D") + "/1"),
                        Valor = valorTotal,
                        DocumentoEntrada = documentoEntrada,
                        NumeroBoleto = string.Empty,
                        Portador = null,
                        Observacao = string.Empty,
                        Forma = formaTitulo
                    };
                    repDuplicata.Inserir(dup);
                }
            }
            else
            {
                var x = intervaloDiasDuplicataNotaEntrada;
                if (x.IndexOf(".") >= 0)
                {
                    arrayDias = x.Split('.');
                    if (arrayDias.Length != quantidadeParcelas)
                    {
                        return false;
                    }
                    for (var i = 0; i < arrayDias.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(arrayDias[i]) || !(int.Parse(arrayDias[i]) > 0))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    arrayDias = new string[1];
                    arrayDias[0] = x;
                    if (string.IsNullOrWhiteSpace(arrayDias[0]) || !(int.Parse(arrayDias[0]) > 0))
                    {
                        return false;
                    }
                }
                var dataVencimento = documentoEntrada.DataEmissao;

                for (var i = 0; i < quantidadeParcelas; i++)
                {
                    decimal valor = 0;
                    if (i == 0)
                        valor = Math.Round((valorParcela + valorDiferenca), 2);
                    else
                        valor = Math.Round(valorParcela, 2);

                    if (arrayDias.Length > 1)
                        dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[i]));
                    else
                        dataVencimento = dataVencimento.AddDays(int.Parse(arrayDias[0]));

                    DateTime novaData = dataVencimento;
                    if (i == 0 && diaPadraoDuplicataNotaEntrada > 0 && diaPadraoDuplicataNotaEntrada <= 31)
                    {
                        try
                        {
                            if (dataVencimento.Day > diaPadraoDuplicataNotaEntrada)
                                dataVencimento = dataVencimento.AddMonths(1);

                            novaData = new DateTime(dataVencimento.Year, dataVencimento.Month, diaPadraoDuplicataNotaEntrada);
                        }
                        catch
                        {
                            novaData = dataVencimento;
                        }
                    }
                    dataVencimento = novaData;

                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata dup = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata()
                    {
                        DataVencimento = dataVencimento,
                        Sequencia = i + 1,
                        Numero = (documentoEntrada.Numero.ToString("D") + "/" + (i + 1)),
                        Valor = valor,
                        DocumentoEntrada = documentoEntrada,
                        NumeroBoleto = string.Empty,
                        Portador = null,
                        Observacao = string.Empty,
                        Forma = formaTitulo
                    };
                    repDuplicata.Inserir(dup);
                }
            }
            return true;
        }

        private DateTime ProximaDataTabelaVencimento(DateTime dataEmissao, int vencimento)
        {
            DateTime novaData;
            DateTime proximoMesAno = dataEmissao.Date;
            int novoDia = vencimento;
            if (proximoMesAno.Day > novoDia)
                proximoMesAno = proximoMesAno.AddMonths(1);
            int diasMes = DateTime.DaysInMonth(proximoMesAno.Year, proximoMesAno.Month);
            if (novoDia > diasMes)
                novoDia = diasMes;

            try
            {
                novaData = new DateTime(proximoMesAno.Year, proximoMesAno.Month, novoDia);
            }
            catch
            {
                novaData = dataEmissao.Date;
            }

            return novaData;
        }

        private Dominio.Enumeradores.TipoTomador ObterTipoTomadorCTe(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cte)
        {
            MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde = cte.CTe.infCte.ide;
            Dominio.Enumeradores.TipoTomador tomador = Dominio.Enumeradores.TipoTomador.Remetente;

            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tptomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)infCTeIde.Item;
                    if (tptomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                        tomador = Dominio.Enumeradores.TipoTomador.Remetente;
                    else if (tptomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                        tomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                    else if (tptomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                        tomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                    else if (tptomador.toma == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                        tomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                    tomador = Dominio.Enumeradores.TipoTomador.Outros;
            }

            return tomador;
        }

        private Dominio.Enumeradores.TipoTomador ObterTipoTomadorCTe(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cte)
        {
            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde = cte.CTe.infCte.ide;
            Dominio.Enumeradores.TipoTomador tomador = Dominio.Enumeradores.TipoTomador.Remetente;

            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tptomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)infCTeIde.Item;
                    if (tptomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item0)
                        tomador = Dominio.Enumeradores.TipoTomador.Remetente;
                    else if (tptomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item1)
                        tomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                    else if (tptomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item2)
                        tomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                    else if (tptomador.toma == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma.Item3)
                        tomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                    tomador = Dominio.Enumeradores.TipoTomador.Outros;
            }

            return tomador;
        }

        private DateTime ObterDataVencimentoGuia(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem imposto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            DateTime dataVencimento;

            dataVencimento = (dataCompetencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada ? documentoEntrada.DataEntrada : documentoEntrada.DataEmissao);

            //Verifica se a data de vencimento da guia irá ser calculada a partir da Data de Vencimento da Duplicata.
            if (imposto.CFOP.ReduzValorLiquidoRetencaoPIS &&
                imposto.CFOP.GerarGuiaPagarRetencaoPIS &&
                imposto.CFOP.CalcularVenvimentoAPartirDataVencimentoTituloNotaRetencaoPIS)
            {
                //Quando o Documento de entrada tiver mais de uma duplicata pega a data de vencimento da primeira duplicata.
                var duplicata = documentoEntrada.Duplicatas.OrderBy(x => x.Sequencia).FirstOrDefault();
                dataVencimento = duplicata.DataVencimento;

            }

            return dataVencimento.AddDays(imposto.CFOP.DiaGerencaoRetencaoPIS);
        }

        #endregion
    }
}
