using Repositorio;
using System;

namespace Servicos.Embarcador.Financeiro
{
    public class PlanoOrcamentario : ServicoBase
    {
        public PlanoOrcamentario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public static String ObterConfiguracaoPlanoOrcamentario(Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Empresa empresa, DateTime dataEmissao, int codigoTipoMovimento, int codigoBaixaTitulo, int codigoNaturezaOperacao, int codigoNaturezaOperacaoNFSe, bool nfce)
        {
            Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unidadeTrabalho);
            Repositorio.NaturezaDaOperacao repNaturezaOperacao = new Repositorio.NaturezaDaOperacao(unidadeTrabalho);

            if (empresa == null)
                return "";

            if (empresa.TipoLancamentoFinanceiroSemOrcamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoFinanceiroSemOrcamento.Liberar)
                return "";

            int planos = 0;
            if (codigoTipoMovimento > 0 && empresa != null)
                planos = repPlanoOrcamentario.QtdPlanoOrcamentarioSobreTipoMovimento(empresa.Codigo, dataEmissao, codigoTipoMovimento);
            else if (codigoBaixaTitulo > 0 && empresa != null)
                planos = repPlanoOrcamentario.QtdPlanoOrcamentarioSobreBaixaTitulo(empresa.Codigo, codigoBaixaTitulo);
            else if (codigoNaturezaOperacao > 0 || codigoNaturezaOperacaoNFSe > 0)
            {
                Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao = null;

                if (codigoNaturezaOperacaoNFSe > 0)
                    naturezaDaOperacao = repNaturezaOperacao.BuscarPorIdNFSe(codigoNaturezaOperacaoNFSe);
                else
                    naturezaDaOperacao = repNaturezaOperacao.BuscarPorId(codigoNaturezaOperacao);

                if (naturezaDaOperacao != null && naturezaDaOperacao.TipoMovimento != null && empresa != null)
                {
                    codigoTipoMovimento = naturezaDaOperacao.TipoMovimento.Codigo;
                    planos = repPlanoOrcamentario.QtdPlanoOrcamentarioSobreTipoMovimento(empresa.Codigo, dataEmissao, codigoTipoMovimento);
                }
            }
            else if (nfce && empresa != null)
            {
                if (empresa.NaturezaDaOperacaoNFCe != null && empresa.NaturezaDaOperacaoNFCe.TipoMovimento != null)
                {
                    codigoTipoMovimento = empresa.NaturezaDaOperacaoNFCe.TipoMovimento.Codigo;
                    planos = repPlanoOrcamentario.QtdPlanoOrcamentarioSobreTipoMovimento(empresa.Codigo, dataEmissao, codigoTipoMovimento);
                }
            }

            if (planos > 0)
                return "";
            else if (empresa != null)
            {
                switch (empresa.TipoLancamentoFinanceiroSemOrcamento)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoFinanceiroSemOrcamento.Bloquear:
                        return "Não existe plano orçamentário ativo para a competência do(s) título(s). Verifique";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoFinanceiroSemOrcamento.Avisar:
                        return "Não existe plano orçamentário ativo para a competência do(s) título(s). Deseja continuar?";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoFinanceiroSemOrcamento.Liberar:
                        return "";
                    default:
                        return "";
                }
            }
            else
                return "";
        }
    }
}
