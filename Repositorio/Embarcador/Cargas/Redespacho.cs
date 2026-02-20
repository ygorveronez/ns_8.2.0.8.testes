using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class Redespacho : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Redespacho>
    {
        public Redespacho(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Redespacho(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.Redespacho BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Redespacho>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Redespacho BuscarPorCargaGerada(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Redespacho>();
            var result = from obj in query where obj.CargaGerada.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Redespacho BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Redespacho>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Redespacho> BuscarAtivasPorCargaOrigem(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Redespacho>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaGerada.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.CargaGerada.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj;
            return result.ToList();
        }

        public List<int> BuscarPorProtocoloIntegracaoCargaOrigem(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Redespacho>();
            var result = from obj in query where obj.Carga.Protocolo == codigo select obj.CargaGerada.Protocolo;
            return result.ToList();
        }

        public List<int> BuscarPorProtocoloIntegracaoCargaOrigem(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Redespacho>();
            var result = from obj in query where codigos.Contains(obj.Carga.Protocolo) select obj.CargaGerada.Protocolo;
            return result.ToList();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Redespacho>();

            int? retorno = query.Max(o => (int?)o.NumeroRedespacho);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Redespacho.FiltroPesquisaRedespacho filtrosPesquisa)
        {
            var sql = QueryPesquisaRedespacho(filtrosPesquisa, true, null);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Redespacho.Redespacho>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Redespacho.FiltroPesquisaRedespacho filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sql = QueryPesquisaRedespacho(filtrosPesquisa, false, parametrosConsulta);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Redespacho.Redespacho)));

            return consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.Redespacho.Redespacho>(CancellationToken);
        }

        private static string QueryPesquisaRedespacho(Dominio.ObjetosDeValor.Embarcador.Carga.Redespacho.FiltroPesquisaRedespacho filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append($@"SELECT redespacho.RED_CODIGO                  AS Codigo
                                , redespacho.RED_NUMERO                     AS NumeroRedespacho
                                , cargaGerada.CAR_CODIGO_CARGA_EMBARCADOR   AS CargaRedespacho
                                , SUBSTRING((SELECT DISTINCT ', ' + cargasUtilizadas.CAR_CODIGO_CARGA_EMBARCADOR 
                                                FROM T_CARGA cargasUtilizadas 
                                                LEFT JOIN T_REDESPACHO_CARGA ON CargasUtilizadas.Car_Codigo = T_REDESPACHO_CARGA.Car_Codigo
                                                WHERE T_REDESPACHO_CARGA.red_codigo = redespacho.RED_CODIGO
                                                for xml path('')
                                            ), 3, 1000)                     AS CargasUtilizadas
                                , carga.CAR_CODIGO_CARGA_EMBARCADOR         AS Carga
                                , redespacho.RED_DATA_REDESPACHO            AS DataRedespacho 
	                            , cliente.CLI_NOME                          AS Expedidor
                                , cliente.CLI_NOMEFANTASIA                  AS ExpedidorNomeFantasia
	                            , cliente.CLI_CODIGO_INTEGRACAO             AS ExpedidorCodigoIntegracao
	                            , cliente.CLI_CGCCPF                        AS ExpedidorCnpjCpf
	                            , cliente.CLI_PONTO_TRANSBORDO              AS ExpedidorPontoTransbordo
                                , cliente.CLI_FISJUR						AS ExpedidorTipoFisJur
                ");

            sql.Append(@"  FROM T_REDESPACHO    redespacho      
                            LEFT JOIN T_CARGA   cargaGerada	ON redespacho.CAR_CODIGO_GERADA     = cargaGerada.CAR_CODIGO
                            LEFT JOIN T_CARGA   carga		ON redespacho.CAR_CODIGO            = carga.CAR_CODIGO
                            LEFT JOIN T_CLIENTE cliente	    ON redespacho.CLI_CODIGO_EXPEDIDOR  = cliente.CLI_CGCCPF
            ");

            sql.Append("WHERE 1 = 1 ");

            if (filtrosPesquisa.NumeroRedespacho > 0)
                sql.Append($" AND Redespacho.RED_NUMERO = {filtrosPesquisa.NumeroRedespacho}");

            if (filtrosPesquisa.DataInicio.HasValue)
                sql.Append($" AND Redespacho.RED_DATA_REDESPACHO >= '{filtrosPesquisa.DataInicio.Value.Date:yyyyMMdd HH:mm:ss}'");

            if (filtrosPesquisa.DataFim.HasValue)
                sql.Append($" AND Redespacho.RED_DATA_REDESPACHO < '{filtrosPesquisa.DataFim.Value.AddDays(1).Date:yyyyMMdd HH:mm:ss}'");

            if (filtrosPesquisa.CodigoCarga > 0)
                sql.Append($" AND carga.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.CodigoExpedidor > 0)
                sql.Append($" AND Redespacho.CLI_CODIGO_EXPEDIDOR = {filtrosPesquisa.CodigoExpedidor}");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                sql.Append($" AND Redespacho.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");

            if (filtrosPesquisa.CodigosRecebedores.Count > 0)
                sql.Append($@" AND EXISTS(SELECT 
                                            cargaPedido.CPE_CODIGO
                                        FROM
                                            T_CARGA_PEDIDO cargaPedido
                                        WHERE
                                            carga.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                        AND ( cargaPedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(", ", filtrosPesquisa.CodigosRecebedores)})
                                              OR cargaPedido.CLI_CODIGO_RECEBEDOR IS NULL))
                ");

            if (filtrosPesquisa.CodigosFilial.Count > 0)
                sql.Append($@" AND carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeOrdenar))
            {
                sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }
    }
}