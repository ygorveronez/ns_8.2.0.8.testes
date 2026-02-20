using NHibernate.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertasFilial : RepositorioRelacionamentoParametrosOfertas<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasFilial>, Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRepositorioRelacionamentoParametrosOfertas
    {
        #region Construtores

        public ParametrosOfertasFilial(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        public override async Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasFilial>> BuscarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasFilial>();

            consulta = AplicarFiltros(consulta, filtro);
            consulta.Fetch(to => to.ParametrosOfertas);
            consulta.Fetch(to => to.Filial);

            return await ObterListaAsync(consulta, parametrosConsulta);
        }

        public async Task<IList<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>> BuscarCodigosDescricaoAsync(int codigoParametrosOfertas)
        {
            var consulta = UnitOfWork.Sessao.CreateSQLQuery($"""
                select 
                    tpof.PFI_CODIGO as CodigoRelacionamento,
                    tpof.FIL_CODIGO as Codigo,
                    tf.FIL_DESCRICAO as Descricao,
                    TF.FIL_CODIGO as CodigoEntidadeFraca
                from T_PARAMETROS_OFERTAS_FILIAL tpof 
                inner join T_FILIAL tf on tpof.FIL_CODIGO = tf.FIL_CODIGO
                where tpof.POF_CODIGO = {codigoParametrosOfertas};
            """);

            consulta.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>());

            return await consulta.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>();
        }
    }
}
