using NHibernate.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertasTipoOperacao : RepositorioRelacionamentoParametrosOfertas<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoOperacao>, Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRepositorioRelacionamentoParametrosOfertas
    {
        #region Construtores

        public ParametrosOfertasTipoOperacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        public override async Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoOperacao>> BuscarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoOperacao>();

            consulta = AplicarFiltros(consulta, filtro);
            consulta.Fetch(to => to.ParametrosOfertas);
            consulta.Fetch(to => to.TipoOperacao);

            return await ObterListaAsync(consulta, parametrosConsulta);
        }

        public async Task<IList<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>> BuscarCodigosDescricaoAsync(int codigoParametrosOfertas)
        {
            var consulta = UnitOfWork.Sessao.CreateSQLQuery($"""
                select 
                    tpoto.PTO_CODIGO as CodigoRelacionamento,
                    tpoto.TOP_CODIGO as Codigo,
                    tto.TOP_DESCRICAO as Descricao,
                    tto.TOP_CODIGO as CodigoEntidadeFraca
                from T_PARAMETROS_OFERTAS_TIPO_OPERACAO tpoto 
                inner join T_TIPO_OPERACAO tto on tpoto.TOP_CODIGO = tto.TOP_CODIGO
                where tpoto.POF_CODIGO = {codigoParametrosOfertas};
            """);

            consulta.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>());

            return await consulta.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>();
        }
    }
}
