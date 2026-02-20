using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Localidades
{
    public class RegiaoPrazoEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega>
    {
        public RegiaoPrazoEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega> BuscarPorRegiao(int codigoRegiao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega>();

            var result = from obj in query where obj.Regiao.Codigo == codigoRegiao select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega BuscarPorFilialTipoOperacaoTipoCarga(int codigoRegiao, int codigoFilial, int codigoTipoOperacao, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega>();

            var result = from obj in query
                         where obj.Regiao.Codigo == codigoRegiao && obj.TempoDeViagemEmMinutos > 0
                         && obj.Filial.Codigo == codigoFilial
                         && (obj.TipoOperacao.Codigo == codigoTipoOperacao || obj.TipoOperacao == null)
                         && (obj.TipoDeCarga.Codigo == codigoTipoCarga || obj.TipoDeCarga == null)
                         select obj;

            return result.FirstOrDefault();
        }

        public bool ExisteRegraDuplicada(string codigoIntegracaoRegiao, string codigoIntegracaoFilial, string codigoIntegracaoTipoDeCarga, string codigoIntegracaoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega>();

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoRegiao))
                query = query.Where(o => o.Regiao.CodigoIntegracao == codigoIntegracaoRegiao);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoTipoDeCarga))
                query = query.Where(o => o.TipoDeCarga.CodigoTipoCargaEmbarcador == codigoIntegracaoTipoDeCarga);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoFilial))
                query = query.Where(o => o.Filial.CodigoFilialEmbarcador == codigoIntegracaoFilial);

            if (!string.IsNullOrWhiteSpace(codigoIntegracaoTipoOperacao))
                query = query.Where(o => o.TipoOperacao.CodigoIntegracao == codigoIntegracaoTipoOperacao);

            return query.Any();
        }

        #endregion
    }
}
