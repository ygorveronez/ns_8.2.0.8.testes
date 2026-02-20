using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class ConsolidacaoSolicitacaoAbastecimentoGas : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>
    {
        public ConsolidacaoSolicitacaoAbastecimentoGas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        
        public List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> BuscarPorSolicitacao(int codigoSolicitacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>()
                .Where(obj => obj.SolicitacaoAbastecimentoGas.Codigo == codigoSolicitacao);

            return query.ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> BuscarPorSolicitacoesCanceladas(List<int> codigosSolicitacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>()
                .Where(obj => codigosSolicitacoes.Contains(obj.SolicitacaoAbastecimentoGas.Codigo))
                .Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                .Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);
            
            return query.ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> BuscarPorDataMedicao(DateTime dataMedicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>()
                .Where(obj => obj.SolicitacaoAbastecimentoGas.DataMedicao.Date == dataMedicao.Date);
            
            return query.ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> BuscarPorDataMedicaoProduto(DateTime dataMedicao, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>()
                .Where(obj => obj.SolicitacaoAbastecimentoGas.DataMedicao.Date == dataMedicao.Date)
                .Where(obj => obj.SolicitacaoAbastecimentoGas.Produto.Codigo == codigoProduto);
            
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas BuscarConsolidacaoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);
            
            return query.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
