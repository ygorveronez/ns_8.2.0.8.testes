using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class Despesa
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public Despesa(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void GerarMovimentoDoFinanceiro(int codigoDespesa)
        {
            Repositorio.DespesaDoAcertoDeViagem repDespesa = new Repositorio.DespesaDoAcertoDeViagem(_unitOfWork);
            Dominio.Entidades.DespesaDoAcertoDeViagem despesa = repDespesa.BuscarPorCodigo(codigoDespesa);

            if (despesa != null && despesa.TipoDespesa != null && despesa.TipoDespesa.PlanoDeConta != null)
            {

                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(_unitOfWork);

                Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorDespesa(despesa.AcertoDeViagem.Empresa.Codigo, despesa.Codigo);

                if (despesa.Paga && despesa.AcertoDeViagem.Situacao == "F")
                {
                    if (movimento == null)
                        movimento = new Dominio.Entidades.MovimentoDoFinanceiro();

                    movimento.Documento = despesa.AcertoDeViagem.Numero.ToString();
                    movimento.Data = despesa.Data.HasValue ? despesa.Data.Value : DateTime.Now;
                    movimento.Empresa = despesa.AcertoDeViagem.Empresa;
                    movimento.PlanoDeConta = despesa.TipoDespesa.PlanoDeConta;
                    movimento.Valor = despesa.ValorTotal;
                    movimento.Veiculo = despesa.AcertoDeViagem.Veiculo;
                    movimento.Pessoa = despesa.Fornecedor;
                    movimento.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                    movimento.Observacao = string.Concat("Referente à despesa de ", despesa.Data.HasValue ? despesa.Data.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"), " do veículo ", despesa.AcertoDeViagem.Veiculo.Placa, ".");
                    if (despesa.AcertoDeViagem != null)
                        movimento.AcertoViagem = despesa.AcertoDeViagem;
                    if (despesa.AcertoDeViagem.Motorista != null)
                        movimento.Motorista = despesa.AcertoDeViagem.Motorista;

                    if (movimento.Despesas != null && movimento.Despesas.Count() > 0)
                        movimento.Despesas.Clear();
                    else if (movimento.Despesas == null)
                        movimento.Despesas = new List<Dominio.Entidades.DespesaDoAcertoDeViagem>();

                    movimento.Despesas.Add(despesa);

                    if (movimento.Codigo > 0)
                        repMovimento.Atualizar(movimento);
                    else
                        repMovimento.Inserir(movimento);
                }
                else if (movimento != null)
                {
                    repMovimento.Deletar(movimento);
                }
            }
        }

        public void DeletarMovimentoDoFinanceiro(int codigoDespesa)
        {
            Repositorio.DespesaDoAcertoDeViagem repDespesa = new Repositorio.DespesaDoAcertoDeViagem(_unitOfWork);
            Dominio.Entidades.DespesaDoAcertoDeViagem despesa = repDespesa.BuscarPorCodigo(codigoDespesa);

            if (despesa != null)
            {
                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(_unitOfWork);

                Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorDespesa(despesa.AcertoDeViagem.Empresa.Codigo, despesa.Codigo);

                if (movimento != null)
                    repMovimento.Deletar(movimento);
            }
        }

        #endregion Métodos Públicos
    }
}
