using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Financeiro
{
    public class ProcessoMovimento
    {
        #region Propriedades

        private List<Dominio.ObjetosDeValor.Embarcador.Financeiro.FechamentoDiario> _FechamentosExistentes = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.FechamentoDiario>();
        private UnitOfWork _unitOfWork;
        #endregion

        #region Construtores
        
        public ProcessoMovimento(UnitOfWork unitOfWork) { 
            _unitOfWork = unitOfWork;
        }

        public ProcessoMovimento(string stringConexao)
        {
            _FechamentosExistentes = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.FechamentoDiario>();
        }
        public ProcessoMovimento()
        {
            _FechamentosExistentes = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.FechamentoDiario>();
        }

        #endregion

        #region Métodos Globais

        public void GerarMovimentacao(Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, DateTime data, decimal valor, string numeroDocumento, string observacao, Repositorio.UnitOfWork unidadeDeTrabalho,
            TipoDocumentoMovimento tipoDocumento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoMotorista = 0, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaCredito = null,
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaDebito = null, int codigoTitulo = 0, TipoMovimentoEntidade? tipoMovimentoEntidade = null, Dominio.Entidades.Cliente pessoaFavorecida = null,
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaFavorecida = null, DateTime? dataBase = null, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centro = null,
            List<Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao> configuracoesExportacao = null, object objetoExportacao = null, TipoMovimentoExportacao? tipoMovimentoExportacao = null,
            MoedaCotacaoBancoCentral? moedaCotacaoBancoCentral = null, DateTime? dataBaseCRT = null, decimal valorMoedaCotacao = 0, decimal valorOriginalMoedaEstrangeira = 0,
            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = null, Dominio.Entidades.Produto produto = null, FormaTitulo? formaTitulo = null)
        {
            string erro = string.Empty;

            if (!GerarMovimentacaoFinanceira(out erro, tipoMovimento, data, valor, numeroDocumento, observacao, unidadeDeTrabalho, tipoDocumento, tipoServicoMultisoftware, codigoMotorista, planoContaCredito, planoContaDebito, codigoTitulo,
                tipoMovimentoEntidade, pessoaFavorecida, grupoPessoaFavorecida, dataBase, centro, configuracoesExportacao, objetoExportacao, tipoMovimentoExportacao, moedaCotacaoBancoCentral, dataBaseCRT, valorMoedaCotacao,
                valorOriginalMoedaEstrangeira, tipoDespesaFinanceira, produto, formaTitulo))
                throw new Exception(erro);
        }

        public bool GerarMovimentacao(out string erro, Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, DateTime data, decimal valor, string numeroDocumento, string observacao, Repositorio.UnitOfWork unidadeDeTrabalho,
            TipoDocumentoMovimento tipoDocumento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoMotorista = 0, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaCredito = null,
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaDebito = null, int codigoTitulo = 0, TipoMovimentoEntidade? tipoMovimentoEntidade = null, Dominio.Entidades.Cliente pessoaFavorecida = null,
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaFavorecida = null, DateTime? dataBase = null, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centro = null,
            List<Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao> configuracoesExportacao = null, object objetoExportacao = null, TipoMovimentoExportacao? tipoMovimentoExportacao = null,
            MoedaCotacaoBancoCentral? moedaCotacaoBancoCentral = null, DateTime? dataBaseCRT = null, decimal valorMoedaCotacao = 0, decimal valorOriginalMoedaEstrangeira = 0,
            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = null, Dominio.Entidades.Produto produto = null, FormaTitulo? formaTitulo = null)
        {
            return GerarMovimentacaoFinanceira(out erro, tipoMovimento, data, valor, numeroDocumento, observacao, unidadeDeTrabalho, tipoDocumento, tipoServicoMultisoftware, codigoMotorista, planoContaCredito, planoContaDebito, codigoTitulo,
                tipoMovimentoEntidade, pessoaFavorecida, grupoPessoaFavorecida, dataBase, centro, configuracoesExportacao, objetoExportacao, tipoMovimentoExportacao, moedaCotacaoBancoCentral, dataBaseCRT, valorMoedaCotacao,
                valorOriginalMoedaEstrangeira, tipoDespesaFinanceira, produto, formaTitulo);
        }

        public bool MovimentacaoFinanceiraJaConciliada(Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, decimal valor, string numeroDocumento, Repositorio.UnitOfWork unidadeDeTrabalho, TipoDocumentoMovimento tipoDocumento,
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaCredito = null, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaDebito = null, int codigoTitulo = 0)
        {
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unidadeDeTrabalho);

            return repMovimentoFinanceiroDebitoCredito.MovimentacaoConcilidada(tipoMovimento?.PlanoDeContaDebito.Codigo ?? planoContaDebito?.Codigo ?? 0, tipoMovimento?.PlanoDeContaCredito.Codigo ?? planoContaCredito?.Codigo ?? 0, valor, numeroDocumento, codigoTitulo, tipoDocumento);
        }

        #endregion

        #region Métodos Privados

        private bool GerarMovimentacaoFinanceira(out string erro, Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, DateTime data, decimal valor, string numeroDocumento, string observacao,
            Repositorio.UnitOfWork unidadeDeTrabalho, TipoDocumentoMovimento tipoDocumento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            int codigoMotorista = 0, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaCredito = null, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoContaDebito = null, int codigoTitulo = 0,
            TipoMovimentoEntidade? tipoMovimentoEntidade = null, Dominio.Entidades.Cliente pessoaFavorecida = null, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaFavorecida = null,
            DateTime? dataBase = null, Dominio.Entidades.Embarcador.Financeiro.CentroResultado centro = null, List<Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao> configuracoesExportacao = null,
            object objetoExportacao = null, TipoMovimentoExportacao? tipoMovimentoExportacao = null,
            MoedaCotacaoBancoCentral? moedaCotacaoBancoCentral = null, DateTime? dataBaseCRT = null, decimal valorMoedaCotacao = 0, decimal valorOriginalMoedaEstrangeira = 0,
            Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesaFinanceira = null, Dominio.Entidades.Produto produto = null, FormaTitulo? formaTitulo = null)
        {
            if ((tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe) || valor <= 0m)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = codigoTitulo > 0 ? repTitulo.BuscarPorCodigo(codigoTitulo) : null;

            if (FechamentoDiario.VerificarSeExisteFechamento(titulo?.Empresa?.Codigo ?? 0, data, unidadeDeTrabalho, tipoDocumento, _FechamentosExistentes))
            {
                erro = "Já existe um fechamento diário igual ou posterior à data de " + data.ToString("dd/MM/yyyy") + ", não sendo possível gerar o movimento financeiro.";
                return false;
            }

            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroEntidade repMovimentoFinanceiroEntidade = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroEntidade(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

            DateTime dataGeracaoMovimento = DateTime.Now;

            Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimento = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro
            {
                DataMovimento = data,
                DataGeracaoMovimento = dataGeracaoMovimento,
                Documento = numeroDocumento,
                Observacao = Utilidades.String.Left(string.IsNullOrWhiteSpace(tipoMovimento?.Observacao) ? observacao : tipoMovimento?.Observacao + ' ' + observacao, 499),
                PlanoDeContaCredito = tipoMovimento?.PlanoDeContaCredito ?? planoContaCredito,
                PlanoDeContaDebito = tipoMovimento?.PlanoDeContaDebito ?? planoContaDebito,
                TipoMovimento = tipoMovimento,
                Valor = valor,
                TipoDocumentoMovimento = tipoDocumento,
                TipoGeracaoMovimento = TipoGeracaoMovimento.Automatica,
                CentroResultado = centro,
                MoedaCotacaoBancoCentral = moedaCotacaoBancoCentral,
                DataBaseCRT = dataBaseCRT,
                ValorMoedaCotacao = valorMoedaCotacao,
                ValorOriginalMoedaEstrangeira = valorOriginalMoedaEstrangeira,
                TipoDespesaFinanceira = tipoDespesaFinanceira,
                FormaTitulo = formaTitulo
            };

            if (!dataBase.HasValue || dataBase.Value == DateTime.MinValue)
                movimento.DataBase = data;
            else
                movimento.DataBase = dataBase.Value;

            if (grupoPessoaFavorecida != null)
                movimento.GrupoPessoas = grupoPessoaFavorecida;

            if (pessoaFavorecida != null)
                movimento.Pessoa = pessoaFavorecida;

            if (grupoPessoaFavorecida == null && pessoaFavorecida?.GrupoPessoas != null)
                movimento.GrupoPessoas = pessoaFavorecida.GrupoPessoas;

            if (codigoTitulo > 0)
            {
                movimento.Titulo = titulo;

                if (titulo.Provisao && titulo.Empresa != null && titulo.Empresa.UtilizaDataVencimentoNaEmissao)
                {
                    data = titulo.DataVencimento.Value;
                    movimento.DataMovimento = data;
                    movimento.DataBase = data;
                }
            }

            if (movimento.PlanoDeContaCredito != null && movimento.PlanoDeContaCredito.AnaliticoSintetico == AnaliticoSintetico.Sintetico)
            {
                erro = "Plano de conta de crédito está como Sintético. Tipo Movimento: '" + tipoMovimento?.Descricao + "'. Conta: " + movimento.PlanoDeContaCredito.Descricao + ".";
                return false;
            }

            if (movimento.PlanoDeContaDebito != null && movimento.PlanoDeContaDebito.AnaliticoSintetico == AnaliticoSintetico.Sintetico)
            {
                erro = "Plano de conta de débito está como Sintético. Tipo Movimento: '" + tipoMovimento?.Descricao + "'. Conta: " + movimento.PlanoDeContaDebito.Descricao + ".";
                return false;
            }

            if (movimento.PlanoDeContaDebito != null && movimento.PlanoDeContaCredito != null && movimento.PlanoDeContaDebito.Codigo == movimento.PlanoDeContaCredito.Codigo)
            {
                erro = "Plano de conta de débito e de crédito são os mesmos. Tipo Movimento: '" + tipoMovimento?.Descricao + "'. Conta Débito: " + movimento.PlanoDeContaDebito.Descricao + ". Conta Crédito: " + movimento.PlanoDeContaCredito.Descricao + ".";
                return false;
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (movimento.PlanoDeContaCredito?.Empresa != null)
                    movimento.Empresa = movimento.PlanoDeContaCredito.Empresa;
                else if (movimento.PlanoDeContaDebito?.Empresa != null)
                    movimento.Empresa = movimento.PlanoDeContaDebito.Empresa;

                if (tipoMovimento?.Empresa != null)
                    movimento.TipoAmbiente = tipoMovimento.Empresa.TipoAmbiente;
                else if (planoContaCredito?.Empresa != null)
                    movimento.TipoAmbiente = planoContaCredito.Empresa.TipoAmbiente;
                else if (planoContaDebito?.Empresa != null)
                    movimento.TipoAmbiente = planoContaDebito.Empresa.TipoAmbiente;
            }

            repMovimentoFinanceiro.Inserir(movimento);

            Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoCredito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito
            {
                DataMovimento = data,
                DataGeracaoMovimento = dataGeracaoMovimento,
                DebitoCredito = DebitoCredito.Credito,
                MovimentoFinanceiro = movimento,
                PlanoDeConta = movimento.PlanoDeContaCredito,
                Valor = valor
            };

            repMovimentoFinanceiroDebitoCredito.Inserir(movimentoCredito);

            Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito movimentoDebito = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito
            {
                DataMovimento = data,
                DataGeracaoMovimento = dataGeracaoMovimento,
                DebitoCredito = DebitoCredito.Debito,
                MovimentoFinanceiro = movimento,
                PlanoDeConta = movimento.PlanoDeContaDebito,
                Valor = valor
            };

            repMovimentoFinanceiroDebitoCredito.Inserir(movimentoDebito);

            if (tipoMovimentoEntidade.HasValue && tipoMovimentoEntidade != TipoMovimentoEntidade.Nenhum && codigoMotorista > 0)
            {
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);

                if (Transportadores.Motorista.GetHabilitarFichaMotorista(motorista, unidadeDeTrabalho))
                {
                    Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade movimentoEntidade = new Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade
                    {
                        Motorista = motorista,
                        MovimentoFinanceiro = movimento,
                        TipoMovimentoEntidade = tipoMovimentoEntidade.Value,
                        Produto = produto
                    };

                    repMovimentoFinanceiroEntidade.Inserir(movimentoEntidade);
                }
            }

            GerarRegistrosExportacao(movimento, configuracoesExportacao, objetoExportacao, tipoMovimentoExportacao, unidadeDeTrabalho);

            erro = string.Empty;
            return true;
        }

        private void GerarRegistrosExportacao(Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro, List<Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao> configuracoesContasExportacao, object objetoExportacao, TipoMovimentoExportacao? tipoMovimentoExportacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!tipoMovimentoExportacao.HasValue)
                return;

            if (objetoExportacao == null)
                return;

            if (configuracoesContasExportacao == null || configuracoesContasExportacao.Count == 0)
            {
                if (movimentoFinanceiro.TipoMovimento != null && movimentoFinanceiro.TipoMovimento.Exportar)
                    configuracoesContasExportacao = movimentoFinanceiro.TipoMovimento.ContasExportacao.ToList();

                if (configuracoesContasExportacao == null || configuracoesContasExportacao.Count == 0)
                    return;
            }

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = null;
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = null;
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = null;
            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = null;
            Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto tituloDocumentoAcrescimoDesconto = null;

            Type type = objetoExportacao.GetType();

            if (type == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || type.BaseType == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)objetoExportacao;
            else if (type == typeof(Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado) || type.BaseType == typeof(Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado))
                tituloBaixaAgrupado = (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado)objetoExportacao;
            else if (type == typeof(Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto) || type.BaseType == typeof(Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto))
                tituloBaixaAgrupadoDocumentoAcrescimoDesconto = (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto)objetoExportacao;
            else if (type == typeof(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete) || type.BaseType == typeof(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete))
                contratoFrete = (Dominio.Entidades.Embarcador.Terceiros.ContratoFrete)objetoExportacao;
            else if (type == typeof(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa) || type.BaseType == typeof(Dominio.Entidades.Embarcador.Financeiro.TituloBaixa))
                tituloBaixa = (Dominio.Entidades.Embarcador.Financeiro.TituloBaixa)objetoExportacao;
            else if (type == typeof(Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto) || type.BaseType == typeof(Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto))
                tituloDocumentoAcrescimoDesconto = (Dominio.Entidades.Embarcador.Financeiro.TituloDocumentoAcrescimoDesconto)objetoExportacao;
            else
                return;

            Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil repDocumentoExportacaoContabil = new Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabil(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabilConta repDocumentoExportacaoContabilConta = new Repositorio.Embarcador.Financeiro.DocumentoExportacaoContabilConta(unitOfWork);

            if (tipoMovimentoExportacao == TipoMovimentoExportacao.AcrescimoDescontoFatura || tipoMovimentoExportacao == TipoMovimentoExportacao.CancelamentoAcrescimoDescontoFatura)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil documentoExportacaoContabil = new Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil()
                {
                    CTe = tituloDocumentoAcrescimoDesconto.TituloDocumento.CTe,
                    TituloDocumentoAcrescimoDesconto = tituloDocumentoAcrescimoDesconto,
                    MovimentoFinanceiro = movimentoFinanceiro,
                    TipoMovimento = tipoMovimentoExportacao.Value,
                    Empresa = tituloDocumentoAcrescimoDesconto.TituloDocumento.CTe.Empresa,
                    Tomador = tituloDocumentoAcrescimoDesconto.TituloDocumento.CTe.TomadorPagador?.Cliente,
                    DataEmissao = tituloDocumentoAcrescimoDesconto.TituloDocumento.CTe.DataEmissao.Value,
                    Numero = tituloDocumentoAcrescimoDesconto.TituloDocumento.CTe.Numero.ToString(),
                    Valor = tituloDocumentoAcrescimoDesconto.Valor,
                    TipoDocumento = TipoDocumentoExportacaoContabil.CTe
                };

                repDocumentoExportacaoContabil.Inserir(documentoExportacaoContabil);

                foreach (Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao configuracaoContaExportacao in configuracoesContasExportacao)
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta documentoExportacaoContabilConta = new Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta()
                    {
                        ContaContabil = configuracaoContaExportacao.ContaContabil,
                        PlanoConta = configuracaoContaExportacao.PlanoConta,
                        Tipo = configuracaoContaExportacao.Tipo,
                        CentroResultado = configuracaoContaExportacao.CentroResultado,
                        CodigoCentroResultado = configuracaoContaExportacao.CodigoCentroResultado,
                        DocumentoExportacaoContabil = documentoExportacaoContabil
                    };

                    repDocumentoExportacaoContabilConta.Inserir(documentoExportacaoContabilConta);
                }

            }
            else if ((tipoMovimentoExportacao == TipoMovimentoExportacao.PagamentoContratoFrete || tipoMovimentoExportacao == TipoMovimentoExportacao.ReversaoPagamentoContratoFrete) && tituloBaixa != null)
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> titulosBaixaAgrupados = repTituloBaixaAgrupado.BuscarPorBaixaTitulo(tituloBaixa.Codigo);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado titulo in tituloBaixa.TitulosAgrupados)
                {
                    if (titulo.Titulo.ContratoFrete == null)
                        continue;

                    Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil documentoExportacaoContabil = new Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil()
                    {
                        TituloBaixaAgrupado = titulo,
                        MovimentoFinanceiro = movimentoFinanceiro,
                        TipoMovimento = tipoMovimentoExportacao.Value,
                        Valor = titulo.Titulo.ValorOriginal,
                        DataEmissao = titulo.Titulo.ContratoFrete.DataEmissaoContrato,
                        Numero = titulo.Titulo.ContratoFrete.NumeroContrato.ToString(),
                        Empresa = titulo.Titulo.ContratoFrete.Carga.Empresa,
                        Tomador = titulo.Titulo.ContratoFrete.TransportadorTerceiro,
                        ContratoFrete = titulo.Titulo.ContratoFrete,
                        TipoDocumento = TipoDocumentoExportacaoContabil.ContratoFrete
                    };

                    repDocumentoExportacaoContabil.Inserir(documentoExportacaoContabil);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao configuracaoContaExportacao in configuracoesContasExportacao)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta documentoExportacaoContabilConta = new Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta()
                        {
                            ContaContabil = configuracaoContaExportacao.ContaContabil,
                            PlanoConta = configuracaoContaExportacao.PlanoConta,
                            Tipo = configuracaoContaExportacao.Tipo,
                            CentroResultado = configuracaoContaExportacao.CentroResultado,
                            CodigoCentroResultado = configuracaoContaExportacao.CodigoCentroResultado,
                            DocumentoExportacaoContabil = documentoExportacaoContabil
                        };

                        repDocumentoExportacaoContabilConta.Inserir(documentoExportacaoContabilConta);
                    }
                }
            }
            else if (tipoMovimentoExportacao == TipoMovimentoExportacao.BaixaTituloReceber ||
                     tipoMovimentoExportacao == TipoMovimentoExportacao.CancelamentoBaixaTituloReceber)
            {
                Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento> tituloBaixaAgrupadoDocumentos = repTituloBaixaAgrupadoDocumento.BuscarPorTituloBaixaAgrupado(tituloBaixaAgrupado.Codigo);

                foreach (Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento in tituloBaixaAgrupadoDocumentos)
                {
                    if (tituloBaixaAgrupadoDocumento.TituloDocumento.CTe == null)
                        continue;

                    Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil documentoExportacaoContabil = new Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil()
                    {
                        CTe = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe,
                        TituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumento,
                        MovimentoFinanceiro = movimentoFinanceiro,
                        TipoMovimento = tipoMovimentoExportacao.Value,
                        Empresa = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Empresa,
                        Tomador = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.TomadorPagador?.Cliente,
                        DataEmissao = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.DataEmissao.Value,
                        Numero = tituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Numero.ToString(),
                        Valor = tituloBaixaAgrupadoDocumento.ValorPago,
                        TipoDocumento = TipoDocumentoExportacaoContabil.CTe
                    };

                    repDocumentoExportacaoContabil.Inserir(documentoExportacaoContabil);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao configuracaoContaExportacao in configuracoesContasExportacao)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta documentoExportacaoContabilConta = new Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta()
                        {
                            ContaContabil = configuracaoContaExportacao.ContaContabil,
                            PlanoConta = configuracaoContaExportacao.PlanoConta,
                            Tipo = configuracaoContaExportacao.Tipo,
                            CentroResultado = configuracaoContaExportacao.CentroResultado,
                            CodigoCentroResultado = configuracaoContaExportacao.CodigoCentroResultado,
                            DocumentoExportacaoContabil = documentoExportacaoContabil
                        };

                        repDocumentoExportacaoContabilConta.Inserir(documentoExportacaoContabilConta);
                    }
                }
            }
            else
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil documentoExportacaoContabil = new Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabil()
                {
                    CTe = cte,
                    TituloBaixaAgrupadoDocumentoAcrescimoDesconto = tituloBaixaAgrupadoDocumentoAcrescimoDesconto,
                    MovimentoFinanceiro = movimentoFinanceiro,
                    TipoMovimento = tipoMovimentoExportacao.Value,
                    Valor = movimentoFinanceiro.Valor,
                    TipoDocumento = TipoDocumentoExportacaoContabil.CTe
                };

                if (cte != null)
                {
                    documentoExportacaoContabil.DataEmissao = cte.DataEmissao.Value;
                    documentoExportacaoContabil.Numero = cte.Numero.ToString();
                    documentoExportacaoContabil.Empresa = cte.Empresa;
                    documentoExportacaoContabil.Tomador = cte.TomadorPagador?.Cliente;
                    documentoExportacaoContabil.TipoDocumento = TipoDocumentoExportacaoContabil.CTe;
                }
                else if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto != null)
                {
                    if (tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento.TituloDocumento.TipoDocumento == TipoDocumentoTitulo.CTe)
                    {
                        documentoExportacaoContabil.DataEmissao = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento.TituloDocumento.CTe.DataEmissao.Value;
                        documentoExportacaoContabil.Numero = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Numero.ToString();
                        documentoExportacaoContabil.Empresa = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento.TituloDocumento.CTe.Empresa;
                        documentoExportacaoContabil.Tomador = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento.TituloDocumento.CTe.TomadorPagador?.Cliente;
                        documentoExportacaoContabil.CTe = tituloBaixaAgrupadoDocumentoAcrescimoDesconto.TituloBaixaAgrupadoDocumento.TituloDocumento.CTe;
                        documentoExportacaoContabil.TipoDocumento = TipoDocumentoExportacaoContabil.CTe;
                    }
                    else
                    {
                        throw new Exception("Não é possível gerar documentos para exportação de contabilização por carga (TituloDocumento = Carga).");
                    }
                }
                else if (contratoFrete != null)
                {
                    documentoExportacaoContabil.DataEmissao = contratoFrete.DataEmissaoContrato;
                    documentoExportacaoContabil.Numero = contratoFrete.NumeroContrato.ToString();
                    documentoExportacaoContabil.Empresa = contratoFrete.Carga.Empresa;
                    documentoExportacaoContabil.Tomador = contratoFrete.TransportadorTerceiro;
                    documentoExportacaoContabil.ContratoFrete = contratoFrete;
                    documentoExportacaoContabil.TipoDocumento = TipoDocumentoExportacaoContabil.ContratoFrete;
                }

                repDocumentoExportacaoContabil.Inserir(documentoExportacaoContabil);

                foreach (Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao configuracaoContaExportacao in configuracoesContasExportacao)
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta documentoExportacaoContabilConta = new Dominio.Entidades.Embarcador.Financeiro.DocumentoExportacaoContabilConta()
                    {
                        ContaContabil = configuracaoContaExportacao.ContaContabil,
                        PlanoConta = configuracaoContaExportacao.PlanoConta,
                        Tipo = configuracaoContaExportacao.Tipo,
                        CentroResultado = configuracaoContaExportacao.CentroResultado,
                        CodigoCentroResultado = configuracaoContaExportacao.CodigoCentroResultado,
                        DocumentoExportacaoContabil = documentoExportacaoContabil
                    };

                    repDocumentoExportacaoContabilConta.Inserir(documentoExportacaoContabilConta);
                }
            }
        }

        #endregion
    }
}
