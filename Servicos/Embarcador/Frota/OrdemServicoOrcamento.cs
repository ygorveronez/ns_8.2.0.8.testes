using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frota
{
    public class OrdemServicoOrcamento
    {
        #region Métodos Públicos

        public static void GerarOrcamentoInicial(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicos, Repositorio.UnitOfWork unidadeTrabalho)
        {
            GerarOrcamentoInicial(ordemServico, servicos, 0, 0, null, unidadeTrabalho);
        }

        public static void GerarOrcamentoInicial(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicos, decimal valorProdutos, decimal valorMaoObra, Dominio.Entidades.Produto produtoOrcado, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto repOrcamentoServicoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento orcamento = repOrcamento.BuscarPorOrdemServico(ordemServico.Codigo);

            if (orcamento != null)
            {
                AtualizarServicosOrcamento(orcamento, servicos, unidadeTrabalho);
                AtualizarValoresOrcamento(orcamento, unidadeTrabalho);
                return;
            }

            orcamento = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento();
            orcamento.OrdemServico = ordemServico;
            orcamento.Parcelas = 1;
            orcamento.ValorTotalPreAprovado = servicos?.Sum(o => o.CustoEstimado) ?? 0m;

            repOrcamento.Inserir(orcamento);

            if (servicos != null)
            {
                foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico in servicos)
                {
                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico();

                    orcamentoServico.Orcamento = orcamento;
                    orcamentoServico.Manutencao = servico;
                    orcamentoServico.ValorProdutos = valorProdutos;
                    orcamentoServico.ValorMaoObra = valorMaoObra;

                    repOrcamentoServico.Inserir(orcamentoServico);

                    if (produtoOrcado != null)
                    {
                        Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto orcamentoServicoProduto = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServicoProduto();

                        orcamentoServicoProduto.OrcamentoServico = orcamentoServico;
                        orcamentoServicoProduto.Produto = produtoOrcado;
                        orcamentoServicoProduto.Quantidade = 1;
                        orcamentoServicoProduto.Valor = valorProdutos;

                        repOrcamentoServicoProduto.Inserir(orcamentoServicoProduto);
                    }
                }

                if (valorProdutos > 0 || valorMaoObra > 0)
                    AtualizarValoresOrcamento(orcamento, unidadeTrabalho);
            }
        }

        public static void AtualizarValoresOrcamento(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento orcamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento repOrcamento = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamento(unidadeTrabalho);

            orcamento.ValorTotalMaoObra = repOrcamentoServico.BuscarValorTotalMaoObra(orcamento.Codigo);
            orcamento.ValorTotalProdutos = repOrcamentoServico.BuscarValorTotalProdutos(orcamento.Codigo);
            orcamento.ValorTotalOrcado = orcamento.ValorTotalMaoObra + orcamento.ValorTotalProdutos;

            repOrcamento.Atualizar(orcamento);
        }

        #endregion

        #region Métodos Privados

        private static void AtualizarServicosOrcamento(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamento orcamento, List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicos, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico repOrcamentoServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico(unidadeTrabalho);

            if (servicos != null)
            {
                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico> servicosJaOrcados = repOrcamentoServico.BuscarPorOrcamento(orcamento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servico in servicos)
                {
                    Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico orcamentoServico = (from obj in servicosJaOrcados where obj.Manutencao.Servico.Codigo == servico.Servico.Codigo select obj).FirstOrDefault();
                    if (orcamentoServico == null)
                    {
                        orcamentoServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaOrcamentoServico();
                        orcamentoServico.Orcamento = orcamento;
                        orcamentoServico.Manutencao = servico;
                        orcamentoServico.ValorMaoObra = servico.OrdemServico.Pneu != null && servico.CustoEstimado > 0 ? servico.CustoEstimado : 0;

                        repOrcamentoServico.Inserir(orcamentoServico);
                    }
                }
            }
        }

        #endregion
    }
}
