using System;
using System.Collections.Generic;

namespace Servicos.WebService.Financeiro
{
    public class Titulo : ServicoBase
    {        
        public Titulo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado ConverterObjetoTituloQuitado(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);

            Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Empresa.Empresa serEmpresa = new Empresa.Empresa(unitOfWork);
            TipoMovimento serTipoMovimento = new TipoMovimento(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro movimentoFinanceiro = repMovimentoFinanceiro.BuscarPorBaixaTitulo(tituloBaixa.Codigo, tituloBaixa.DataBaixa.Value, tituloBaixa.Valor, titulo.ValorPago, titulo.Codigo);

            string atribuicao = string.Empty;
            if (!string.IsNullOrWhiteSpace(titulo.TipoDocumentoTituloOriginal))
                atribuicao = titulo.TipoDocumentoTituloOriginal;
            if (!string.IsNullOrWhiteSpace(atribuicao))
                atribuicao += ", ";
            atribuicao += titulo.DataVencimento.Value.ToDateString();
            if (titulo.DataProgramacaoPagamento.HasValue)
                atribuicao += ", " + titulo.DataProgramacaoPagamento.Value.ToDateString();
            atribuicao += ", " + titulo.DataLiquidacao.Value.ToDateString();

            Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado tituloQuitado = new Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado()
            {
                Codigo = titulo.Codigo,
                DataEmissao = titulo.DataEmissao.Value,
                Empresa = serEmpresa.ConverterObjetoEmpresa(titulo.Empresa),
                NumeroDocumento = titulo.NumeroDocumentoTituloOriginal,
                ObservacaoMovimento = movimentoFinanceiro?.Observacao ?? string.Empty,
                Atribuicao = atribuicao,
                Conta = titulo.Pessoa.CPF_CNPJ_SemFormato,
                Pessoa = serPessoa.ConverterObjetoPessoa(titulo.Pessoa),
                TipoTitulo = titulo.TipoTitulo,
                ValorOriginal = titulo.ValorOriginal,
                ValorPago = titulo.ValorPago,
                ObservacaoBaixa = tituloBaixa.Observacao,
                PlanoRecebimento = serTipoMovimento.ConverterObjetoPlanoDeConta(tituloBaixa.TipoPagamentoRecebimento?.PlanoConta, unitOfWork),
                CTes = RetornarCTesPorTitulo(titulo.Codigo, unitOfWork)
            };

            return tituloQuitado;
        }

        private List<Dominio.ObjetosDeValor.WebService.CTe.CTe> RetornarCTesPorTitulo(int codigoTitulo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            CTe.CTe serCTe = new CTe.CTe(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repTituloDocumento.BuscarCTesPorTitulo(codigoTitulo);

            if (listaCTe.Count == 0)
                return null;

            List<Dominio.ObjetosDeValor.WebService.CTe.CTe> ctesRetornar = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCTe)
                ctesRetornar.Add(serCTe.ConverterObjetoCTe(cte, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, unitOfWork, false));

            return ctesRetornar;
        }

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo ConverterObjetoTituloAPagar(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Empresa.Empresa serEmpresa = new Empresa.Empresa(unitOfWork);
            TipoMovimento serTipoMovimento = new TipoMovimento(unitOfWork);

            string referencia = string.Empty;
            if (!string.IsNullOrWhiteSpace(titulo.NumeroDocumentoTituloOriginal))
                referencia = titulo.NumeroDocumentoTituloOriginal;
            if (!string.IsNullOrWhiteSpace(referencia) && !string.IsNullOrWhiteSpace(titulo.TipoDocumentoTituloOriginal))
                referencia += ", ";
            if (!string.IsNullOrWhiteSpace(titulo.TipoDocumentoTituloOriginal))
                referencia += titulo.TipoDocumentoTituloOriginal;

            string observacao = titulo.Observacao ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(observacao) && !string.IsNullOrWhiteSpace(referencia))
                observacao += ", ";
            if (!string.IsNullOrWhiteSpace(referencia))
                observacao += referencia;

            Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo tituloRetornar = new Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo()
            {
                CodigoAbastecimento = titulo.Abastecimento?.Codigo ?? 0,
                Protocolo = titulo.Codigo,
                DataEmissao = titulo.DataEmissao.Value,
                DataVencimento = titulo.DataVencimento.Value,
                Pessoa = serPessoa.ConverterObjetoPessoa(titulo.Pessoa),
                Empresa = serEmpresa.ConverterObjetoEmpresa(titulo.Empresa),
                TipoMovimento = serTipoMovimento.ConverterObjetoTipoMovimento(titulo.TipoMovimento, unitOfWork),
                Moeda = titulo.MoedaCotacaoBancoCentral ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real,
                FormaTitulo = titulo.FormaTitulo,
                Situacao = titulo.StatusTitulo,
                ValorOriginal = titulo.ValorOriginal,
                Observacao = observacao,
                Referencia = referencia,
                Parcelas = repTitulo.ObterSequenciaComParcelaTitulo(titulo.Codigo),
                CodigoIntegracaoPagamento = titulo.Pessoa?.CodigoIntegracaoDuplicataNotaEntrada ?? string.Empty
            };

            return tituloRetornar;
        }

        #endregion
    }
}
