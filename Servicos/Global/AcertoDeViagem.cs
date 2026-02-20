using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class AcertoDeViagem
    {
        private Dominio.Entidades.AcertoDeViagem AcertoViagem { get; set; }

        private string NumeroAcertoViagem
        {
            get
            {
                return AcertoViagem?.Numero.ToString() ?? "";
            }
        }

        private Repositorio.UnitOfWork UnitOfWork { get; set; }

        private DateTime DataMovimentos { get; set; }

        private List<Dominio.Entidades.MovimentoDoFinanceiro> MovimentosFinanceiros { get; set; }

        public AcertoDeViagem(Dominio.Entidades.AcertoDeViagem acertoDeViagem, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            this.AcertoViagem = acertoDeViagem;
            this.UnitOfWork = unitOfWork;
            this.DataMovimentos = data;
            this.MovimentosFinanceiros = new List<Dominio.Entidades.MovimentoDoFinanceiro>();
        }

        private Dominio.Entidades.MovimentoDoFinanceiro BaseEntidadeMovimento(Dominio.Entidades.PlanoDeConta plano, Dominio.Enumeradores.TipoMovimento tipoMovimento)
        {
            return new Dominio.Entidades.MovimentoDoFinanceiro()
            {
                AcertoViagem = AcertoViagem,
                Data = DataMovimentos,
                Documento = NumeroAcertoViagem,
                Empresa = AcertoViagem.Empresa,
                Veiculo = AcertoViagem.Veiculo,
                Motorista = AcertoViagem.Motorista,
                PlanoDeConta = plano,
                Tipo = tipoMovimento
            };
        }

        public void GerarMovimentoReceitas(Dominio.Entidades.PlanoDeConta plano, Dominio.Enumeradores.TipoMovimentoAcerto tipoMovimentoAcerto, List<Dominio.Entidades.DestinoDoAcertoDeViagem> destinos)
        {
            if (tipoMovimentoAcerto == Dominio.Enumeradores.TipoMovimentoAcerto.PorAcerto)
            {
                if (AcertoViagem.TotalReceitas > 0)
                {
                    Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                    movimentoDoFinanceiro.Valor = AcertoViagem.TotalReceitas;
                    movimentoDoFinanceiro.Observacao = "Receitas Acerto Viagem " + NumeroAcertoViagem;

                    MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                }
            }
            else
            {
                int index = 0;
                foreach (Dominio.Entidades.DestinoDoAcertoDeViagem destino in destinos)
                {
                    Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                    if (destino.ValorFrete > 0)
                    {
                        if (destino.DataInicial.HasValue)
                            movimentoDoFinanceiro.Data = destino.DataInicial.Value;
                        movimentoDoFinanceiro.Valor = destino.ValorFrete - destino.OutrosDescontos;
                        if (destino.CTe != null)
                            movimentoDoFinanceiro.Observacao = "Receita CT-e " + destino.CTe.Descricao + " - Acerto Viagem " + NumeroAcertoViagem;
                        else
                            movimentoDoFinanceiro.Observacao = "Receita Destino " + (++index).ToString() + " - Acerto Viagem " + NumeroAcertoViagem;

                        MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                    }
                }
            }
        }

        public void GerarMovimentoDespesas(Dominio.Entidades.PlanoDeConta plano, Dominio.Enumeradores.TipoMovimentoAcerto tipoMovimentoAcerto, List<Dominio.Entidades.DespesaDoAcertoDeViagem> despesas)
        {
            if (tipoMovimentoAcerto == Dominio.Enumeradores.TipoMovimentoAcerto.PorAcerto)
            {
                decimal valorTotalDespesas = (from a in despesas select a.ValorTotal).Sum();

                if (valorTotalDespesas > 0)
                {
                    Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                    movimentoDoFinanceiro.Valor = valorTotalDespesas;
                    movimentoDoFinanceiro.Observacao = "Despesas Acerto Viagem " + NumeroAcertoViagem;

                    MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                }
            }
            else
            {
                foreach (Dominio.Entidades.DespesaDoAcertoDeViagem despesa in despesas)
                {
                    if (despesa.ValorTotal > 0)
                    {
                        Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                        if (despesa.Data.HasValue)
                            movimentoDoFinanceiro.Data = despesa.Data.Value;
                        movimentoDoFinanceiro.Valor = despesa.ValorTotal;
                        movimentoDoFinanceiro.Observacao = "Despesa " + despesa.Descricao + " / Tipo " + despesa.DescricaoDespesa + " - Acerto Viagem " + NumeroAcertoViagem;

                        MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                    }
                }
            }
        }

        public void GerarMovimentoAbastecimentos(Dominio.Entidades.PlanoDeConta plano, Dominio.Enumeradores.TipoMovimentoAcerto tipoMovimentoAcerto, List<Dominio.Entidades.Abastecimento> abastecimentos)
        {
            if (tipoMovimentoAcerto == Dominio.Enumeradores.TipoMovimentoAcerto.PorAcerto)
            {
                decimal valorTotalAbastecimentos = (from a in abastecimentos select a.ValorTotal).Sum();

                if (valorTotalAbastecimentos > 0)
                {
                    Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                    movimentoDoFinanceiro.Valor = valorTotalAbastecimentos;
                    movimentoDoFinanceiro.Observacao = "Abastecimentos Acerto Viagem " + NumeroAcertoViagem;

                    MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                }
            }
            else
            {
                int index = 0;
                foreach (Dominio.Entidades.Abastecimento abastecimento in abastecimentos)
                {
                    if (abastecimento.ValorTotal > 0)
                    {
                        Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                        if (abastecimento.Data.HasValue)
                            movimentoDoFinanceiro.Data = abastecimento.Data.Value;
                        movimentoDoFinanceiro.Valor = abastecimento.ValorTotal;
                        movimentoDoFinanceiro.Observacao = "Abastecimento " + (++index).ToString() + " - Acerto Viagem " + NumeroAcertoViagem;

                        MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                    }
                }
            }
        }

        public void GerarMovimentoPagamentosMotorista(Dominio.Entidades.PlanoDeConta plano)
        {
            decimal valorMovimento = 0;

            if (AcertoViagem.TipoComissao == Dominio.Enumeradores.TipoComissao.ValorBruto)
                valorMovimento = AcertoViagem.TotalReceitas * (AcertoViagem.PercentualComissao / 100);
            else if (AcertoViagem.TipoComissao == Dominio.Enumeradores.TipoComissao.ValorLiquido)
                valorMovimento = (AcertoViagem.TotalReceitas - AcertoViagem.TotalDespesas) * (AcertoViagem.PercentualComissao / 100);

            if (valorMovimento > 0)
            {
                Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                movimentoDoFinanceiro.Valor = Math.Round(valorMovimento, 2, MidpointRounding.ToEven); ;
                movimentoDoFinanceiro.Observacao = "Pagamento Motorista Acerto Viagem " + NumeroAcertoViagem;

                MovimentosFinanceiros.Add(movimentoDoFinanceiro);
            }
        }

        public void GerarMovimentoAdiantamentosMotorista(Dominio.Entidades.PlanoDeConta plano, Dominio.Enumeradores.TipoMovimentoAcerto tipoMovimentoAcerto, List<Dominio.Entidades.ValeDoAcertoDeViagem> vales)
        {
            if (tipoMovimentoAcerto == Dominio.Enumeradores.TipoMovimentoAcerto.PorAcerto)
            {
                if (AcertoViagem.Adiantamento > 0)
                {
                    Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                    movimentoDoFinanceiro.Valor = AcertoViagem.Adiantamento;
                    movimentoDoFinanceiro.Observacao = "Adiantamentos Acerto Viagem " + NumeroAcertoViagem;

                    MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                }
            }
            else
            {
                foreach (Dominio.Entidades.ValeDoAcertoDeViagem vale in (from v in vales where v.Tipo == Dominio.Enumeradores.TipoValeAcertoViagem.Vale select v))
                {
                    if (vale.Valor > 0)
                    {
                        Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                        movimentoDoFinanceiro.Data = vale.Data;
                        movimentoDoFinanceiro.Valor = vale.Valor;
                        movimentoDoFinanceiro.Observacao = "Adiantamento " + vale.Numero.ToString() + " / " + vale.Descricao + " - Acerto Viagem " + NumeroAcertoViagem;

                        MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                    }
                }
            }
        }

        public void GerarMovimentoDevolucoesMotorista(Dominio.Entidades.PlanoDeConta plano, Dominio.Enumeradores.TipoMovimentoAcerto tipoMovimentoAcerto, List<Dominio.Entidades.ValeDoAcertoDeViagem> vales)
        {
            List<Dominio.Entidades.ValeDoAcertoDeViagem> valesFiltrados = (from v in vales where v.Tipo == Dominio.Enumeradores.TipoValeAcertoViagem.Devolucao select v).ToList();
            if (tipoMovimentoAcerto == Dominio.Enumeradores.TipoMovimentoAcerto.PorAcerto)
            {
                decimal valorTotalVales = valesFiltrados.Sum(o => o.Valor);

                if (valorTotalVales > 0)
                {
                    Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                    movimentoDoFinanceiro.Valor = valorTotalVales;
                    movimentoDoFinanceiro.Observacao = "Devoluções Acerto Viagem " + NumeroAcertoViagem;

                    MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                }
            }
            else
            {
                foreach (Dominio.Entidades.ValeDoAcertoDeViagem vale in valesFiltrados)
                {
                    if (vale.Valor > 0)
                    {
                        Dominio.Entidades.MovimentoDoFinanceiro movimentoDoFinanceiro = BaseEntidadeMovimento(plano, Dominio.Enumeradores.TipoMovimento.Entrada);

                        movimentoDoFinanceiro.Data = vale.Data;
                        movimentoDoFinanceiro.Valor = vale.Valor;
                        movimentoDoFinanceiro.Observacao = "Devolução " + vale.Numero.ToString() + " / " + vale.Descricao + " - Acerto Viagem " + NumeroAcertoViagem;

                        MovimentosFinanceiros.Add(movimentoDoFinanceiro);
                    }
                }
            }
        }

        public void ProcessarMovimentos()
        {
            Repositorio.MovimentoDoFinanceiro repMovimentoDoFinanceiro = new Repositorio.MovimentoDoFinanceiro(UnitOfWork);
            for (var i = 0; i < MovimentosFinanceiros.Count; i++)
                repMovimentoDoFinanceiro.Inserir(MovimentosFinanceiros[i]);
        }
    }
}
