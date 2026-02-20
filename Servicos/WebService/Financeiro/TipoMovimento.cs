using System.Collections.Generic;

namespace Servicos.WebService.Financeiro
{
    public class TipoMovimento : ServicoBase
    {        
        public TipoMovimento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento ConverterObjetoTipoMovimento(Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoMovimento == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento retornoTipoMovimento = new Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento()
            {
                Codigo = tipoMovimento.Codigo,
                Descricao = tipoMovimento.Descricao,
                Situacao = tipoMovimento.Ativo ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo,
                PlanoEntrada = ConverterObjetoPlanoDeConta(tipoMovimento.PlanoDeContaDebito, unitOfWork),
                PlanoSaida = ConverterObjetoPlanoDeConta(tipoMovimento.PlanoDeContaCredito, unitOfWork),
                CentrosResultados = ConverterObjetoCentrosResultados(tipoMovimento.CentrosResultados)
            };

            return retornoTipoMovimento;
        }

        public Dominio.ObjetosDeValor.Embarcador.Financeiro.PlanoDeConta ConverterObjetoPlanoDeConta(Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta, Repositorio.UnitOfWork unitOfWork)
        {
            if (planoConta == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Financeiro.PlanoDeConta retornoPlanoDeConta = new Dominio.ObjetosDeValor.Embarcador.Financeiro.PlanoDeConta()
            {
                Codigo = planoConta.Codigo,
                CodigoIntegracao = planoConta.PlanoContabilidade,
                Descricao = planoConta.Descricao,
                Plano = planoConta.Plano
            };

            return retornoPlanoDeConta;
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado> ConverterObjetoCentrosResultados(IList<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado> centrosResultados)
        {
            if (centrosResultados == null || centrosResultados.Count == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado> listaCentroResultado = new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado>();

            foreach (Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado tipoMovimentoCentroResultado in centrosResultados)
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado centroResultado = new Dominio.ObjetosDeValor.Embarcador.Financeiro.CentroResultado()
                {
                    Descricao = tipoMovimentoCentroResultado.CentroResultado.Descricao,
                    Codigo = tipoMovimentoCentroResultado.CentroResultado.Codigo
                };

                listaCentroResultado.Add(centroResultado);
            }

            return listaCentroResultado;
        }

        #endregion
    }
}

