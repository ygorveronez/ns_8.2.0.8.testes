using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.IntegracaoERP
{
    public class IntegracaoERP<TEntidade> : RepositorioBase<TEntidade>
        where TEntidade : Dominio.Entidades.EntidadeBase
    {
        #region Propiedades Protegidas
        readonly private int _quantidadePadraoRetornoRegistro = 50;
        readonly private NHibernate.Criterion.SimpleExpression _integradoERPFalso = NHibernate.Criterion.Expression.Eq("IntegradoERP", false);
        readonly private NHibernate.Criterion.SimpleExpression _integradoERPNulo = NHibernate.Criterion.Expression.Eq("IntegradoERP", null);
        #endregion

        #region Construtores

        public IntegracaoERP(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos
        public IList<TEntidade> BuscarRegitrosPendenteIntegracao(int quatidade)
        {
            int totalRegistros = quatidade > 0 ? quatidade : _quantidadePadraoRetornoRegistro;

            var criteria = this.SessionNHiBernate.CreateCriteria<TEntidade>();
            criteria.Add(NHibernate.Criterion.Expression.Or(_integradoERPFalso, _integradoERPNulo));
            criteria.SetMaxResults(totalRegistros);
            return criteria.List<TEntidade>();
        }

        public int ContarRegistroPendenteIntegracao()
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<TEntidade>();
            criteria.Add(NHibernate.Criterion.Expression.Or(_integradoERPFalso, _integradoERPNulo));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public IList<TEntidade> BuscarRegitrosPendentesIntegracaoPeloProtocolos(List<int> listaProtocolos)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<TEntidade>();
            criteria.Add(NHibernate.Criterion.Expression.In("Codigo", listaProtocolos));
            criteria.Add(NHibernate.Criterion.Expression.Or(_integradoERPFalso, _integradoERPNulo));
            return criteria.List<TEntidade>();
        }

        public IList<TEntidade> BuscarRegitrosPendentesIntegracaoPeloProtocolos(List<long> listaProtocolos)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<TEntidade>();
            criteria.Add(NHibernate.Criterion.Expression.In("CPF_CNPJ", listaProtocolos));
            criteria.Add(NHibernate.Criterion.Expression.Or(_integradoERPFalso, _integradoERPNulo));
            return criteria.List<TEntidade>();
        }
        #endregion
    }
}
