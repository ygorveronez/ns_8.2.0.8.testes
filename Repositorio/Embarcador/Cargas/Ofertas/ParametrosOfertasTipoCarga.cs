using NHibernate.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertasTipoCarga : RepositorioRelacionamentoParametrosOfertas<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoCarga>, Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRepositorioRelacionamentoParametrosOfertas
    {
        #region Construtores

        public ParametrosOfertasTipoCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        public override async Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoCarga>> BuscarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoCarga>();

            consulta = AplicarFiltros(consulta, filtro);
            consulta.Fetch(to => to.ParametrosOfertas);
            consulta.Fetch(to => to.TipoDeCarga.Pessoa);

            return await ObterListaAsync(consulta, parametrosConsulta);
        }

        public async Task<IList<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>> BuscarCodigosDescricaoAsync(int codigoParametrosOfertas)
        {
            var consulta = UnitOfWork.Sessao.CreateSQLQuery($"""
                select 
                    tpotc.PTC_CODIGO as CodigoRelacionamento,
                    tpotc.TCG_CODIGO as Codigo,
                    ttc.TCG_DESCRICAO as Descricao,
                    ttc.TCG_CODIGO as CodigoEntidadeFraca
                from T_PARAMETROS_OFERTAS_TIPO_CARGA tpotc 
                inner join T_TIPO_DE_CARGA ttc on tpotc.TCG_CODIGO = ttc.TCG_CODIGO
                where tpotc.POF_CODIGO = {codigoParametrosOfertas};
            """);

            consulta.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>());

            return await consulta.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>();
        }
    }
}
