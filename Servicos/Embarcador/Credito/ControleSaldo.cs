using System.Collections.Generic;

namespace Servicos.Embarcador.Credito
{
    public class ControleSaldo : ServicoBase
    {                
        public ControleSaldo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public dynamic BuscarInformacoesSaldoCredito(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
            Repositorio.Embarcador.Creditos.CreditoDisponivel repCreditoDisponivel = new Repositorio.Embarcador.Creditos.CreditoDisponivel(unitOfWork);


            unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

            dynamic retorno;
            List<dynamic> dynCredito = new List<dynamic>();
            List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> hierarquiasCredito = repHierarquiaSolicitacaoCredito.BuscarPorRecebedor(usuario.Codigo);
            decimal valorSaldoTotal = 0;
            if (hierarquiasCredito.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito hierarquiaSolicitacaoCredito in hierarquiasCredito)
                {
                    Dominio.Entidades.Embarcador.Creditos.CreditoDisponivel creditoDisponivel = repCreditoDisponivel.BuscarRecebedorCredito(hierarquiaSolicitacaoCredito.Creditor.Codigo, hierarquiaSolicitacaoCredito.Solicitante.Codigo);
                    if (creditoDisponivel != null)
                    {
                        var dadosCredito = new
                        {
                            creditoDisponivel.Codigo,
                            Creditor = new { Descricao = creditoDisponivel.Creditor.Nome, Codigo = creditoDisponivel.Creditor.Codigo },
                            Credito = creditoDisponivel.ValorCredito,
                            Comprometido = creditoDisponivel.ValorComprometido,
                            Obtido = creditoDisponivel.ValorObtido,
                            DataFimCredito = creditoDisponivel.DataFimCredito.ToString("dd/MM/yyyy"),
                            Utilizado = (creditoDisponivel.ValorCredito - creditoDisponivel.ValorSaldo - creditoDisponivel.ValorComprometido).ToString("n2"),
                            Saldo = creditoDisponivel.ValorSaldo.ToString("n2")
                        };
                        valorSaldoTotal += creditoDisponivel.ValorSaldo;
                        dynCredito.Add(dadosCredito);
                    }
                }
                retorno = new
                {
                    PossuiSuperior = true,
                    Saldo = valorSaldoTotal,
                    Creditos = dynCredito
                };
            }
            else
            {
                retorno = new
                {
                    PossuiSuperior = false,
                    Saldo = valorSaldoTotal,
                    Creditos = dynCredito
                };
            }

            return retorno;
        }
    }
}
