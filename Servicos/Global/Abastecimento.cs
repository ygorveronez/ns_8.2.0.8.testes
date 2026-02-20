using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class Abastecimento
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public Abastecimento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void GerarMovimentoDoFinanceiro(int codigoAbastecimento)
        {
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
            Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento);

            if (abastecimento != null)
            {
                Dominio.Entidades.PlanoDeConta plano = abastecimento.Veiculo != null && abastecimento.Veiculo.Empresa.Configuracao != null ? abastecimento.Veiculo.Empresa.Configuracao.PlanoAbastecimento : null;
                if (plano != null)
                {
                    Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(_unitOfWork);
                    Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorAbastecimento(abastecimento.Veiculo?.Empresa.Codigo ?? 0, abastecimento.Codigo);

                    if ((abastecimento.Status == null || abastecimento.Status.Equals("A")) && abastecimento.Situacao.Equals("F"))
                    {
                        if (movimento == null)
                            movimento = new Dominio.Entidades.MovimentoDoFinanceiro();

                        movimento.Data = abastecimento.Data.HasValue ? abastecimento.Data.Value : DateTime.Now;
                        movimento.Empresa = abastecimento.Veiculo?.Empresa ?? null;
                        movimento.PlanoDeConta = plano;
                        movimento.Valor = abastecimento.ValorTotal;
                        movimento.Veiculo = abastecimento.Veiculo;
                        movimento.Pessoa = abastecimento.Posto;
                        movimento.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                        movimento.Observacao = string.Concat("Referente ao abastecimento de ", abastecimento.Data.HasValue ? abastecimento.Data.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"), " do veículo ", (abastecimento.Veiculo?.Placa ?? ""), ".");
                        if (abastecimento.AcertoDeViagem != null)
                        {
                            movimento.AcertoViagem = abastecimento.AcertoDeViagem;
                            movimento.Motorista = abastecimento.Motorista;
                        }

                        if (movimento.Abastecimentos != null && movimento.Abastecimentos.Count() > 0)
                            movimento.Abastecimentos.Clear();
                        else if (movimento.Abastecimentos == null)
                            movimento.Abastecimentos = new List<Dominio.Entidades.Abastecimento>();

                        movimento.Abastecimentos.Add(abastecimento);

                        if (movimento.Codigo > 0)
                            repMovimento.Atualizar(movimento);
                        else
                            repMovimento.Inserir(movimento);
                    }
                    else
                    {
                        if (movimento != null)
                            repMovimento.Deletar(movimento);
                    }
                }
            }
        }

        public void DeletarMovimentoDoFinanceiro(int codigoAbastecimento)
        {
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
            Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(codigoAbastecimento);

            if (abastecimento != null)
            {
                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(_unitOfWork);
                Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorAbastecimento(abastecimento.Veiculo?.Empresa.Codigo ?? 0, abastecimento.Codigo);
                if (movimento != null)
                    repMovimento.Deletar(movimento);
            }
        }

        #endregion Métodos Públicos
    }
}
