using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertas : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas>
    {
        #region Construtores

        public ParametrosOfertas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ParametrosOfertas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion


        public async Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas>> BuscarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertas filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaParametrosOfertas = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas>();

            consultaParametrosOfertas = AplicarFiltros(consultaParametrosOfertas, filtro);

            return await ObterListaAsync(consultaParametrosOfertas, parametrosConsulta);
        }

        public async Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertas filtro)
        {
            var consultaParametrosOfertas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas>();

            consultaParametrosOfertas = AplicarFiltros(consultaParametrosOfertas, filtro);

            return await consultaParametrosOfertas.CountAsync(CancellationToken);
        }

        private static IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas> AplicarFiltros(IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertas> consulta, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertas filtro)
        {
            if (filtro.Descricao != null && filtro.Descricao != string.Empty)
            {
                consulta = consulta.Where(po => po.Descricao.Contains(filtro.Descricao));
            }

            if (filtro.CodigoIntegracao != null && filtro.CodigoIntegracao != string.Empty)
            {
                consulta = consulta.Where(po => po.CodigoIntegracao.Equals(filtro.CodigoIntegracao));
            }

            if (filtro.Ativo != null)
            {
                consulta = consulta.Where(po => po.Ativo.Equals((bool)filtro.Ativo));
            }

            return consulta;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertas> BuscarPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string select = @$"
        select parametrosOfertas.POF_CODIGO as {nameof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta.Codigo)}
        , parametrosOfertas.POF_DESCRICAO as {nameof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta.Descricao)}
        , parametrosOfertas.POF_CODIGO_INTEGRACAO as {nameof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta.CodigoIntegracao)}
        , parametrosOfertas.POF_ATIVO as {nameof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta.Ativo)}
        , STUFF((SELECT ',' + CAST(parametrosOfertasEmpresa.EMP_CODIGO AS VARCHAR)
                 FROM T_PARAMETROS_OFERTAS_EMPRESA parametrosOfertasEmpresa
                 WHERE parametrosOfertasEmpresa.POF_CODIGO = parametrosOfertas.POF_CODIGO
                 FOR XML PATH('')), 1, 1, '') as {nameof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta.CodigosEmpresasString)}
        , STUFF((SELECT ',' + CAST(parametrosOfertasFilial.FIL_CODIGO AS VARCHAR)
                 FROM T_PARAMETROS_OFERTAS_FILIAL parametrosOfertasFilial
                 WHERE parametrosOfertasFilial.POF_CODIGO = parametrosOfertas.POF_CODIGO
                 FOR XML PATH('')), 1, 1, '') as {nameof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta.CodigosFiliaisString)}
        , STUFF((SELECT ',' + CAST(parametrosOfertasTipoCarga.TCG_CODIGO AS VARCHAR)
                 FROM T_PARAMETROS_OFERTAS_TIPO_CARGA parametrosOfertasTipoCarga
                 WHERE parametrosOfertasTipoCarga.POF_CODIGO = parametrosOfertas.POF_CODIGO
                 FOR XML PATH('')), 1, 1, '') as {nameof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta.CodigosTiposCargasString)}
        , STUFF((SELECT ',' + CAST(parametrosOfertasTipoOperacao.TOP_CODIGO AS VARCHAR)
                 FROM T_PARAMETROS_OFERTAS_TIPO_OPERACAO parametrosOfertasTipoOperacao
                 WHERE parametrosOfertasTipoOperacao.POF_CODIGO = parametrosOfertas.POF_CODIGO
                 FOR XML PATH('')), 1, 1, '') as {nameof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta.CodigosTiposOperacoesString)}
    from T_PARAMETROS_OFERTAS as parametrosOfertas
    ";

            string joins = " ";
            string where = " where parametrosOfertas.POF_ATIVO = 1";

            if (carga.Empresa != null)
            {
                joins += " left join T_PARAMETROS_OFERTAS_EMPRESA parametrosOfertasEmpresa on parametrosOfertasEmpresa.POF_CODIGO = parametrosOfertas.POF_CODIGO";
                where += $" and (parametrosOfertasEmpresa.EMP_CODIGO = {carga.Empresa.Codigo} or parametrosOfertasEmpresa.EMP_CODIGO is null)";
            }

            if (carga.Filial != null)
            {
                joins += " left join T_PARAMETROS_OFERTAS_FILIAL parametrosOfertasFilial on parametrosOfertasFilial.POF_CODIGO = parametrosOfertas.POF_CODIGO";
                where += $" and (parametrosOfertasFilial.FIL_CODIGO = {carga.Filial.Codigo} or parametrosOfertasFilial.FIL_CODIGO is null)";
            }

            if (carga.TipoDeCarga != null)
            {
                joins += " left join T_PARAMETROS_OFERTAS_TIPO_CARGA parametrosOfertasTipoCarga on parametrosOfertasTipoCarga.POF_CODIGO = parametrosOfertas.POF_CODIGO";
                where += $" and (parametrosOfertasTipoCarga.TCG_CODIGO = {carga.TipoDeCarga.Codigo} or parametrosOfertasTipoCarga.TCG_CODIGO is null)";
            }

            if (carga.TipoOperacao != null)
            {
                joins += " left join T_PARAMETROS_OFERTAS_TIPO_OPERACAO parametrosOfertasTipoOperacao on parametrosOfertasTipoOperacao.POF_CODIGO = parametrosOfertas.POF_CODIGO";
                where += $" and (parametrosOfertasTipoOperacao.TOP_CODIGO = {carga.TipoOperacao.Codigo} or parametrosOfertasTipoOperacao.TOP_CODIGO is null)";
            }

            string groupBy = @" 
            group by parametrosOfertas.POF_CODIGO
                , parametrosOfertas.POF_DESCRICAO
                , parametrosOfertas.POF_CODIGO_INTEGRACAO
                , parametrosOfertas.POF_ATIVO
            ";

            ISQLQuery sqlQuery = SessionNHiBernate.CreateSQLQuery(select + joins + where + groupBy);

            sqlQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta)));

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta> parametrosOfertasResultadoConsulta = sqlQuery.List<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertasResultadoConsulta>();

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertas> parametrosOfertas = parametrosOfertasResultadoConsulta
                .Select(x => new Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.ParametrosOfertas(x))
                .ToList();

            return parametrosOfertas;
        }

        public Task<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao> BuscarCodigosDescricaoAsync(int codigoParametrosOfertas, CancellationToken cancellationToken)
        {
            var consulta = UnitOfWork.Sessao.CreateSQLQuery(@$"
                select top 1
                    tpof.POF_CODIGO as CodigoRelacionamento,
                    tgmo.GMO_CODIGO as Codigo,
                    tgmo.GMO_DESCRICAO as Descricao
                from T_PARAMETROS_OFERTAS tpof 
                inner join T_GRUPO_MOTORISTAS tgmo on tpof.GMO_CODIGO = tgmo.GMO_CODIGO
                where tpof.POF_CODIGO = {codigoParametrosOfertas};
            ");

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao)));

            return consulta.UniqueResultAsync<Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao>(cancellationToken);
        }
    }
}
