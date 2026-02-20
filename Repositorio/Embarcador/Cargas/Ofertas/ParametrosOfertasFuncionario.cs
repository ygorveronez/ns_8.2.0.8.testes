using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertasFuncionario : RepositorioRelacionamentoParametrosOfertas<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario>, Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRepositorioRelacionamentoParametrosOfertas
    {
        #region Construtores

        public ParametrosOfertasFuncionario(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        public override Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario>> BuscarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario>();

            consulta = AplicarFiltros(consulta, filtro);
            consulta.Fetch(o => o.ParametrosOfertas);
            consulta.Fetch(o => o.Funcionario);

            return ObterListaAsync(consulta, parametrosConsulta);
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>> BuscarCodigosDescricaoAsync(int codigoParametrosOfertas)
        {
            var consulta = UnitOfWork.Sessao.CreateSQLQuery($"""
                select 
                    tpof.PFU_CODIGO as CodigoRelacionamento,
                    tpof.FUN_CODIGO as Codigo,
                    tf.FUN_NOME as Descricao,
                    tf.FUN_CODIGO as CodigoEntidadeFraca
                from T_PARAMETROS_OFERTAS_FUNCIONARIO tpof 
                inner join T_FUNCIONARIO tf on tpof.FUN_CODIGO = tf.FUN_CODIGO 
                where tpof.POF_CODIGO = {codigoParametrosOfertas};
            """);

            consulta.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>());

            return consulta.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>();
        }

        public Task<Dominio.Entidades.Usuario> BuscarPrimeiroMotoristaPorCodigoParametroOfertaAsync(int codigoParametroOferta, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasFuncionario>()
                .Where(p => p.ParametrosOfertas.Codigo == codigoParametroOferta)
                .Fetch(p => p.Funcionario);

            return query.Select(p => p.Funcionario).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
