using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class HistoricoDeVeiculo
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public HistoricoDeVeiculo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void GerarMovimentoDoFinanceiro(int codigoHistoricoVeiculo, int codigoEmpresa)
        {
            Repositorio.HistoricoVeiculo repHistoricoVeiculo = new Repositorio.HistoricoVeiculo(_unitOfWork);
            Dominio.Entidades.HistoricoVeiculo historico = repHistoricoVeiculo.BuscarPorCodigo(codigoHistoricoVeiculo, codigoEmpresa);

            if (historico != null)
            {
                if (historico.Servico.PlanoDeConta != null)
                {
                    Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(_unitOfWork);
                    Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorHistoricoDeVeiculo(historico.Veiculo.Empresa.Codigo, historico.Codigo);

                    if (historico.Status.Equals("A"))
                    {
                        if (movimento == null)
                            movimento = new Dominio.Entidades.MovimentoDoFinanceiro();

                        movimento.Data = historico.Data.HasValue ? historico.Data.Value : DateTime.Now;
                        movimento.Empresa = historico.Veiculo.Empresa;
                        movimento.PlanoDeConta = historico.Servico.PlanoDeConta;
                        movimento.Valor = historico.Valor;
                        movimento.Veiculo = historico.Veiculo;
                        movimento.Observacao = string.Concat("Referente ao histórico de veículo de ", historico.Data.HasValue ? historico.Data.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"), " do veículo ", historico.Veiculo.Placa, ".");
                        movimento.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                        
                        if (movimento.HistoricosVeiculos != null && movimento.HistoricosVeiculos.Count() > 0)
                            movimento.HistoricosVeiculos.Clear();
                        else if (movimento.HistoricosVeiculos == null)
                            movimento.HistoricosVeiculos = new List<Dominio.Entidades.HistoricoVeiculo>();

                        movimento.HistoricosVeiculos.Add(historico);

                        if (movimento.Codigo > 0)
                            repMovimento.Atualizar(movimento);
                        else
                            repMovimento.Inserir(movimento);
                    }
                    else
                    {
                        if (movimento != null)
                        {
                            movimento.HistoricosVeiculos.Clear();
                            repMovimento.Atualizar(movimento);
                            repMovimento.Deletar(movimento);
                        }                      
                            
                    }
                }
            }
        }

        public void DeletarMovimentoDoFinanceiro(int codigoHistoricoVeiculo, int codigoEmpresa)
        {
            Repositorio.HistoricoVeiculo repHistoricoVeiculo = new Repositorio.HistoricoVeiculo(_unitOfWork);
            Dominio.Entidades.HistoricoVeiculo historico = repHistoricoVeiculo.BuscarPorCodigo(codigoHistoricoVeiculo, codigoEmpresa);

            if (historico != null)
            {
                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(_unitOfWork);
                Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorHistoricoDeVeiculo(historico.Veiculo.Empresa.Codigo, historico.Codigo);
                if (movimento != null)
                    repMovimento.Deletar(movimento);
            }
        }

        #endregion Métodos Públicos
    }
}
