using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class ConfiguracaoToleranciaPesagem : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem>
    {
        public ConfiguracaoToleranciaPesagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem> _Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroToleranciaPesagem filtroToleranciaPesagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem>();

            var result = from obj in query select obj;

            if (filtroToleranciaPesagem == null) return result;

            if (filtroToleranciaPesagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoToleranciaPesagem.Todos)
                result = result.Where(o => o.Ativo == (filtroToleranciaPesagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoToleranciaPesagem.Ativo));

            if (filtroToleranciaPesagem.CodigosTipoCarga.Count > 0)
                result = result.Where(o => o.TiposCarga.Any(x => filtroToleranciaPesagem.CodigosTipoCarga.Contains(x.Codigo)));

            if (filtroToleranciaPesagem.CodigosTipoOperacao.Count > 0)
                result = result.Where(o => o.TiposOperacao.Any(to => filtroToleranciaPesagem.CodigosTipoOperacao.Contains(to.Codigo)));

            if (filtroToleranciaPesagem.CodigosModeloVeicular.Count > 0)
                result = result.Where(o => o.ModelosVeicularesCarga.Any(mvc => filtroToleranciaPesagem.CodigosModeloVeicular.Contains(mvc.Codigo)));

            if (filtroToleranciaPesagem.CodigosFiliais.Count > 0)
                result = result.Where(o => o.Filials.Any(f => filtroToleranciaPesagem.CodigosFiliais.Contains(f.Codigo)));

            if (filtroToleranciaPesagem.Codigo > 0)
                result.Where(x => x.Codigo == filtroToleranciaPesagem.Codigo);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroToleranciaPesagem filtroToleranciaPesagem = null, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem> consulta = _Consultar(filtroToleranciaPesagem);

            return ObterLista(consulta, parametrosConsulta);
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem> consulta = _Consultar(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroToleranciaPesagem() { Codigo = codigo });

            return consulta.FirstOrDefault();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroToleranciaPesagem filtroToleranciaPesagem)
        {
            var result = _Consultar(filtroToleranciaPesagem);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem ConfiguracoesCompativel(int codigoFilial, int codigoModeloVeicular, int codigoTipoDeCarga, int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem>();

            var result = from obj in query
                         where (obj.ValidaFilial && obj.Filials.Any(f => f.Codigo == codigoFilial)) &&
                           (obj.ValidaModeloVeicularCarga && obj.ModelosVeicularesCarga.Any(m => m.Codigo == codigoModeloVeicular)) &&
                           (obj.ValidaTipoCarga && obj.TiposCarga.Any(tc => tc.Codigo == codigoTipoDeCarga)) &&
                           (obj.ValidaTipoOperacao && obj.TiposOperacao.Any(to => to.Codigo == codigoTipoOperacao))
                         select obj;

            var listaResult = result.FirstOrDefault();
            if (listaResult != null)
                return listaResult;

            var combinacoes = new List<Func<IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem>,
              IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem>>>() {
                q => from obj in q
                     where(obj.ValidaFilial && obj.Filials.Any(f => f.Codigo == codigoFilial)) &&
                     (obj.ValidaModeloVeicularCarga && obj.ModelosVeicularesCarga.Any(m => m.Codigo == codigoModeloVeicular)) &&
                     (obj.ValidaTipoCarga && obj.TiposCarga.Any(tc => tc.Codigo == codigoTipoDeCarga))
                     select obj,

                q => from obj in q
                     where(obj.ValidaFilial && obj.Filials.Any(f => f.Codigo == codigoFilial)) &&
                     (obj.ValidaModeloVeicularCarga && obj.ModelosVeicularesCarga.Any(m => m.Codigo == codigoModeloVeicular)) &&
                     (obj.ValidaTipoOperacao && obj.TiposOperacao.Any(to => to.Codigo == codigoTipoOperacao))
                     select obj,

                q => from obj in q
                     where(obj.ValidaFilial && obj.Filials.Any(f => f.Codigo == codigoFilial)) &&
                     (obj.ValidaTipoCarga && obj.TiposCarga.Any(tc => tc.Codigo == codigoTipoDeCarga)) &&
                     (obj.ValidaTipoOperacao && obj.TiposOperacao.Any(to => to.Codigo == codigoTipoOperacao))
                     select obj,

                q => from obj in q
                     where(obj.ValidaModeloVeicularCarga && obj.ModelosVeicularesCarga.Any(m => m.Codigo == codigoModeloVeicular)) &&
                     (obj.ValidaTipoCarga && obj.TiposCarga.Any(tc => tc.Codigo == codigoTipoDeCarga)) &&
                     (obj.ValidaTipoOperacao && obj.TiposOperacao.Any(to => to.Codigo == codigoTipoOperacao))
                     select obj,

                q => from obj in q
                     where(obj.ValidaFilial && obj.Filials.Any(f => f.Codigo == codigoFilial)) &&
                     (obj.ValidaModeloVeicularCarga && obj.ModelosVeicularesCarga.Any(m => m.Codigo == codigoModeloVeicular))
                     select obj,

                q => from obj in q
                     where(obj.ValidaFilial && obj.Filials.Any(f => f.Codigo == codigoFilial)) &&
                     (obj.ValidaTipoCarga && obj.TiposCarga.Any(tc => tc.Codigo == codigoTipoDeCarga))
                     select obj,

                q => from obj in q
                     where(obj.ValidaFilial && obj.Filials.Any(f => f.Codigo == codigoFilial)) &&
                     (obj.ValidaTipoOperacao && obj.TiposOperacao.Any(to => to.Codigo == codigoTipoOperacao))
                     select obj,

                q => from obj in q
                     where(obj.ValidaModeloVeicularCarga && obj.ModelosVeicularesCarga.Any(m => m.Codigo == codigoModeloVeicular)) &&
                     (obj.ValidaTipoCarga && obj.TiposCarga.Any(tc => tc.Codigo == codigoTipoDeCarga))
                     select obj,

                q => from obj in q
                     where(obj.ValidaModeloVeicularCarga && obj.ModelosVeicularesCarga.Any(m => m.Codigo == codigoModeloVeicular)) &&
                     (obj.ValidaTipoOperacao && obj.TiposOperacao.Any(to => to.Codigo == codigoTipoOperacao))
                     select obj,

                q => from obj in q
                     where(obj.ValidaTipoCarga && obj.TiposCarga.Any(tc => tc.Codigo == codigoTipoDeCarga)) &&
                     (obj.ValidaTipoOperacao && obj.TiposOperacao.Any(to => to.Codigo == codigoTipoOperacao))
                     select obj,

                q => from obj in q
                     where obj.ValidaFilial && obj.Filials.Any(f => f.Codigo == codigoFilial)
                     select obj,

                q => from obj in q
                     where obj.ValidaModeloVeicularCarga && obj.ModelosVeicularesCarga.Any(m => m.Codigo == codigoModeloVeicular)
                     select obj,

                q => from obj in q
                     where obj.ValidaTipoCarga && obj.TiposCarga.Any(tc => tc.Codigo == codigoTipoDeCarga)
                     select obj,

                q => from obj in q
                     where obj.ValidaTipoOperacao && obj.TiposOperacao.Any(to => to.Codigo == codigoTipoOperacao)
                     select obj
            };

            foreach (var combinacao in combinacoes)
            {
                result = combinacao(query);
                listaResult = result.FirstOrDefault();
                if (listaResult != null)
                    return listaResult;
            }

            return query.Where(x => !x.ValidaTipoCarga && !x.ValidaTipoOperacao && !x.ValidaModeloVeicularCarga && !x.ValidaFilial).FirstOrDefault();
        }

    }
}