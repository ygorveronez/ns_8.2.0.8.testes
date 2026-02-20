using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Financeiro
{
    public class ContratoFinanciamento : ServicoBase
    {        
        public ContratoFinanciamento(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #region Métodos Globais

        public bool AtualizarTitulos(Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela repContratoFinanciamentoParcela = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcela(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor repContratoFinanciamentoParcelaValor = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarPrimeiroRegistro();
            Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa repTituloCentroResultadoTipoDespesa = new Repositorio.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa repTipoMovimentoTipoDespesa = new Repositorio.Embarcador.Financeiro.TipoMovimentoTipoDespesa(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado repTipoMovimentoCentroResultado = new Repositorio.Embarcador.Financeiro.TipoMovimentoCentroResultado(unidadeTrabalho);

            ProcessoMovimento svcProcessoMovimento = new ProcessoMovimento(StringConexao);


            if (repContratoFinanciamentoParcela.ContarPorContratoFinanciamento(contratoFinanciamento.Codigo) > 0)
            {
                erro = "Já existem títulos em negociação para este contrato de financiamento, não sendo possível atualizar os mesmos.";
                return false;
            }

            if (repContratoFinanciamentoParcela.ContarPorStatusEContratoFinanciamento(contratoFinanciamento.Codigo) > 0)
            {
                erro = "Já existem títulos quitados para este contrato de financiamento, não sendo possível atualizar os mesmos.";
                return false;
            }

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa tiposDespesaMovimento = repTipoMovimentoTipoDespesa.BuscarPorTipoMovimento(contratoFinanciamento.TipoMovimento?.Codigo ?? 0).FirstOrDefault();
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado centroResultadoMovimento = repTipoMovimentoCentroResultado.BuscarPorTipoMovimento(contratoFinanciamento.TipoMovimento?.Codigo ?? 0).FirstOrDefault();

            List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela> parcelas = repContratoFinanciamentoParcela.BuscarPorContratoFinanciamento(contratoFinanciamento.Codigo);
            foreach (Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcela parcela in parcelas)
            {
                if (contratoFinanciamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Finalizado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                    titulo.DataEmissao = contratoFinanciamento.DataEmissao;
                    titulo.DataVencimento = parcela.DataVencimento;
                    titulo.DataProgramacaoPagamento = parcela.DataVencimento;
                    titulo.Pessoa = contratoFinanciamento.Fornecedor;
                    titulo.GrupoPessoas = contratoFinanciamento.Fornecedor.GrupoPessoas;
                    if (titulo.GrupoPessoas == null && titulo.Pessoa != null && titulo.Pessoa.GrupoPessoas != null)
                        titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                    titulo.Sequencia = parcela.Sequencia;
                    titulo.ValorOriginal = parcela.Valor + parcela.ValorAcrescimo;
                    titulo.ValorPendente = parcela.Valor + parcela.ValorAcrescimo;
                    titulo.Desconto = 0;
                    titulo.Acrescimo = 0;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.Observacao = !string.IsNullOrEmpty(parcela.Observacao) ? "Contrato Financiamento - " + parcela.Observacao : string.Concat("Referente à parcela " + parcela.Sequencia + " do contrato de financiamento nº " + contratoFinanciamento.Numero.ToString() + ".");
                    if (parcela.ValorAcrescimo > 0)
                        titulo.Observacao += " ACRESCIMO APLICADO EM R$ " + parcela.ValorAcrescimo.ToString("n2");
                    titulo.Empresa = contratoFinanciamento.Empresa;
                    titulo.ValorTituloOriginal = titulo.ValorOriginal;
                    titulo.TipoDocumentoTituloOriginal = "Contrato Financiamento";
                    titulo.NumeroDocumentoTituloOriginal = parcela.NumeroDocumento;
                    titulo.FormaTitulo = contratoFinanciamento.FormaTitulo.HasValue ? contratoFinanciamento.FormaTitulo.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Financiamento;
                    titulo.NossoNumero = parcela.CodigoBarras;
                    titulo.TipoMovimento = contratoFinanciamento.TipoMovimento;
                    titulo.Provisao = contratoFinanciamento.Provisao;
                    titulo.DataLancamento = DateTime.Now;
                    titulo.Usuario = parcela.Titulo?.Usuario;

                    var tituloCentroResultadoTipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.TituloCentroResultadoTipoDespesa();
                    if (tiposDespesaMovimento != null || centroResultadoMovimento != null)
                    {
                        tituloCentroResultadoTipoDespesa.Titulo = titulo;
                        tituloCentroResultadoTipoDespesa.TipoDespesaFinanceira = tiposDespesaMovimento?.TipoDespesaFinanceira;
                        tituloCentroResultadoTipoDespesa.CentroResultado = centroResultadoMovimento?.CentroResultado;
                        tituloCentroResultadoTipoDespesa.Percentual = 100;
                    }

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        titulo.TipoAmbiente = tipoAmbiente;

                    repTitulo.Inserir(titulo);

                    if (tituloCentroResultadoTipoDespesa.Titulo != null)
                        repTituloCentroResultadoTipoDespesa.Inserir(tituloCentroResultadoTipoDespesa);

                    parcela.Titulo = titulo;
                    repContratoFinanciamentoParcela.Atualizar(parcela);

                    DateTime dataMovimentacaoFinanceira = (configuracaoFinanceiro?.GerarMovimentoPelaDataVencimentoContratoFinanceiro ?? false) ? parcela.DataVencimento : contratoFinanciamento.DataEmissao;

                    if (contratoFinanciamento.Provisao && configuracaoFinanceiro.MovimentacaoFinanceiraParaTitulosDeProvisao)
                    {
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, titulo.TipoMovimento, dataMovimentacaoFinanceira, parcela.Valor, contratoFinanciamento.Numero.ToString(), titulo.Observacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, null, null, contratoFinanciamento.DataEmissao))
                            return false;

                        if (parcela.ValorAcrescimo > 0 && contratoFinanciamento.TipoMovimentoAcrescimo != null)
                            if (!svcProcessoMovimento.GerarMovimentacao(out erro, contratoFinanciamento.TipoMovimentoAcrescimo, dataMovimentacaoFinanceira, parcela.ValorAcrescimo, contratoFinanciamento.Numero.ToString(), "Acréscimo na parcela do Contrato Financiamento nº " + contratoFinanciamento.Numero.ToString(), unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo?.Codigo ?? 0, null, null, null, contratoFinanciamento.DataEmissao))
                                return false;
                    }

                    if (!contratoFinanciamento.Provisao)
                    {
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, titulo.TipoMovimento, dataMovimentacaoFinanceira, parcela.Valor, contratoFinanciamento.Numero.ToString(), titulo.Observacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, null, null, contratoFinanciamento.DataEmissao))
                            return false;

                        if (parcela.ValorAcrescimo > 0 && contratoFinanciamento.TipoMovimentoAcrescimo != null)
                            if (!svcProcessoMovimento.GerarMovimentacao(out erro, contratoFinanciamento.TipoMovimentoAcrescimo, dataMovimentacaoFinanceira, parcela.ValorAcrescimo, contratoFinanciamento.Numero.ToString(), "Acréscimo na parcela do Contrato Financiamento nº " + contratoFinanciamento.Numero.ToString(), unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo?.Codigo ?? 0, null, null, null, contratoFinanciamento.DataEmissao))
                                return false;
                    }
                }
                else if (contratoFinanciamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento.Finalizado)
                {

                    DateTime dataMovimentacaoFinanceira = (configuracaoFinanceiro?.GerarMovimentoPelaDataVencimentoContratoFinanceiro ?? false) ? parcela.DataVencimento : contratoFinanciamento.DataEmissao;

                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repContratoFinanciamentoParcela.BuscarPorContratoFinanciamentoParcela(parcela.Codigo);
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

                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, dataMovimentacaoFinanceira, titulo.ValorOriginal, contratoFinanciamento.Numero.ToString(), titulo.Observacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, contratoFinanciamento.TipoMovimento.PlanoDeContaDebito, contratoFinanciamento.TipoMovimento.PlanoDeContaCredito, titulo.Codigo, null, null, null, contratoFinanciamento.DataEmissao))
                            return false;

                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.DataCancelamento = DateTime.Now;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Cancelado título.", unidadeTrabalho);
                        repTitulo.Atualizar(titulo);

                        parcela.Titulo = null;
                        repContratoFinanciamentoParcela.Atualizar(parcela);
                    }

                    if (parcela.ValorAcrescimo > 0 && contratoFinanciamento.TipoMovimentoAcrescimo != null && contratoFinanciamento.Provisao && configuracaoFinanceiro.MovimentacaoFinanceiraParaTitulosDeProvisao)
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, dataMovimentacaoFinanceira, parcela.ValorAcrescimo, contratoFinanciamento.Numero.ToString(), "Estorno do Acrescimo da Parcela do Contrato Financiamento nº " + contratoFinanciamento.Numero.ToString(), unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, contratoFinanciamento.TipoMovimentoAcrescimo.PlanoDeContaDebito, contratoFinanciamento.TipoMovimentoAcrescimo.PlanoDeContaCredito, titulo?.Codigo ?? 0, null, null, null, contratoFinanciamento.DataEmissao))
                            return false;

                    if (parcela.ValorAcrescimo > 0 && contratoFinanciamento.TipoMovimentoAcrescimo != null && !contratoFinanciamento.Provisao)
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, dataMovimentacaoFinanceira, parcela.ValorAcrescimo, contratoFinanciamento.Numero.ToString(), "Estorno do Acrescimo da Parcela do Contrato Financiamento nº " + contratoFinanciamento.Numero.ToString(), unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, contratoFinanciamento.TipoMovimentoAcrescimo.PlanoDeContaDebito, contratoFinanciamento.TipoMovimentoAcrescimo.PlanoDeContaCredito, titulo?.Codigo ?? 0, null, null, null, contratoFinanciamento.DataEmissao))
                            return false;
                }
            }

            return true;
        }

        public void VincularDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntradaTMS, Repositorio.UnitOfWork unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (documentoEntradaTMS.ContratoFinanciamento != null)
            {
                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada repContratoFinanciamentoDocumentoEntrada = new Repositorio.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento contratoFinanciamento =
                    documentoEntradaTMS.ContratoFinanciamento.Codigo > 0 ? repContratoFinanciamento.BuscarPorCodigo(documentoEntradaTMS.ContratoFinanciamento.Codigo) : null;

                Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada contratoFinanciamentoDocumentoEntrada
                    = repContratoFinanciamentoDocumentoEntrada.BuscarPorDocumentoEntradaEContrato(documentoEntradaTMS.Codigo, documentoEntradaTMS.ContratoFinanciamento.Codigo);

                if (contratoFinanciamentoDocumentoEntrada == null)
                {
                    contratoFinanciamentoDocumentoEntrada = new Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada();

                    contratoFinanciamentoDocumentoEntrada.DocumentoEntradaTMS = documentoEntradaTMS;
                    contratoFinanciamentoDocumentoEntrada.ContratoFinanciamento = contratoFinanciamento;

                    repContratoFinanciamentoDocumentoEntrada.Inserir(contratoFinanciamentoDocumentoEntrada);
                }
            }
        }

        #endregion
    }
}
