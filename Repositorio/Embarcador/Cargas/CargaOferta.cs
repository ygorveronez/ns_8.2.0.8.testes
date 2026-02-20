using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaOferta : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaOferta>
    {
        public CargaOferta(UnitOfWork unitOfWork) : this(unitOfWork, default) { }
        public CargaOferta(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoCargaOferta.CargaOferta>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOferta.FiltroPesquisaCargaOferta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sql = QueryGestaoCargaOferta(filtrosPesquisa, false, parametrosConsulta);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoCargaOferta.CargaOferta)));

            return consulta.ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoCargaOferta.CargaOferta>(CancellationToken);
        }

        public Task<int> ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOferta.FiltroPesquisaCargaOferta filtrosPesquisa)
        {
            var sql = QueryGestaoCargaOferta(filtrosPesquisa, true, null);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public async Task AtualizarSituacaoIntegracaoAsync(List<int> codigos, CancellationToken cancellationToken)
        {
            if (codigos.Count == 0)
                return;

            string sql = @$"declare @codigosCargaOferta table (codigoCargaoferta INT);
                            insert into @codigosCargaOferta (codigoCargaoferta)
                            select distinct CAO_CODIGO
                            from 
	                            T_CARGA_OFERTA_INTEGRACAO cargaOfertaIntegracao 
                            where
	                            cargaOfertaIntegracao.COI_CODIGO in ({string.Join(", ", codigos)}) and
	                            not exists (
                                    select top 1 1 
                                    from T_CARGA_OFERTA_INTEGRACAO _cargaOfertaIntegracao 
                                    where _cargaOfertaIntegracao.CAO_CODIGO = cargaOfertaIntegracao.CAO_CODIGO 
                                    and _cargaOfertaIntegracao.INT_SITUACAO_INTEGRACAO != {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado}
                                )
                            IF EXISTS (SELECT 1 FROM @codigosCargaOferta)
                            BEGIN
                                UPDATE T
                                SET T.CAO_SITUACAO_INTEGRACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado}
                                FROM T_CARGA_OFERTA T
                                JOIN @codigosCargaOferta C ON T.CAO_CODIGO = C.codigoCargaoferta
                            END"; // SQL-INJECTION-SAFE

            await SessionNHiBernate.CreateSQLQuery(sql).ExecuteUpdateAsync(cancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaOferta> BuscarPorCargaAsync(int codigoCarga)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOferta>()
           .FirstOrDefaultAsync(oferta => oferta.Carga.Codigo == codigoCarga);
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaOferta>> BuscarPorCodigosAsync(List<long> codigosCargaOferta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOferta>().Where(cargaOferta => codigosCargaOferta.Contains(cargaOferta.Codigo));
            return query.ToListAsync();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private string QueryGestaoCargaOferta(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOferta.FiltroPesquisaCargaOferta filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool selecionarTodos = false, List<int> codigosCargaOferta = null)
        {
            StringBuilder query = new StringBuilder();
            string pattern = "yyyy-MM-dd";

            query.Append($@"WITH CTE_Placas AS (
                            SELECT 
                                carga.CAR_CODIGO,
                                Placas = (
                                    SELECT vei.VEI_PLACA
                                    FROM T_VEICULO vei
                                    WHERE vei.VEI_CODIGO = carga.CAR_VEICULO
                                ) + ISNULL((
                                    SELECT ', ' + veiculo.VEI_PLACA
                                    FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga
                                    INNER JOIN T_VEICULO veiculo ON veiculoVinculadoCarga.VEI_CODIGO = veiculo.VEI_CODIGO
                                    WHERE veiculoVinculadoCarga.CAR_CODIGO = carga.CAR_CODIGO
                                    FOR XML PATH('')
                                ), '')
                            FROM T_CARGA carga),

                        CTE_Motoristas AS (
				            SELECT 
					            carga.CAR_CODIGO,
					            Motoristas = STUFF((
							            SELECT 
							            ', ' + motorista.FUN_NOME +
							            CASE 
							            WHEN motorista.FUN_FONE IS NULL OR motorista.FUN_FONE = '' 
							            THEN '' 
							            ELSE ' (' + motorista.FUN_FONE + ')' 
							            END
							            FROM T_CARGA_MOTORISTA motoristaCarga
							            INNER JOIN T_FUNCIONARIO motorista
								            ON motoristaCarga.CAR_MOTORISTA = motorista.FUN_CODIGO
							            WHERE motoristaCarga.CAR_CODIGO = carga.CAR_CODIGO
							            FOR XML PATH(''), TYPE)
							            .value('.', 'NVARCHAR(MAX)')
					            , 1, 2, '')
				             FROM T_CARGA carga)");

            if (somenteContarNumeroRegistros)
                query.Append(@" SELECT DISTINCT(count(0) over ()) ");
            else
                query.Append(@" SELECT CAO.CAO_CODIGO AS Codigo,
                            CAR.CAR_CODIGO AS CodigoCarga, 
                            CAR_CODIGO_CARGA_EMBARCADOR AS NumeroCarga, 
                            CAO.CAO_SITUACAO AS SituacaoCargaOferta,
                            CAO.CAO_SITUACAO_INTEGRACAO AS SituacaoIntegracao,
                            CAR.CAR_SITUACAO AS SituacaoCarga,
                            CAO.CAO_DATA_HORA AS DataOferta,
                            CAO.CAO_DATA_HORA_ACEITE AS DataOfertaAceite,
                            CDS.CDS_REMETENTES AS Remetente,
                            CDS_DESTINATARIOS AS Destinatario,
                            EMP_RAZAO + ' (' + EMP_CNPJ + ')' AS Transportadores,
                            CDS_ORIGENS AS Origem,
                            CDS_DESTINOS AS Destino,
                            CDS.CDS_DATA_PREVISAO_ENTREGA AS DataPrevisaoEntrega,
                            CAR.CAR_DATA_CARREGAMENTO AS DataCarregamento,
                            Placas.Placas AS Placa,
                            Motoristas.Motoristas AS Motorista,
                            CAR.CAR_VALOR_FRETE AS ValorFrete, 
                            CAO.CAO_DATA_FIM_OFERTA AS DataFimOferta,
							CDS.CDS_DISTANCIA AS Quilometragem"
            );

            query.Append(@" FROM T_CARGA_OFERTA CAO
                LEFT JOIN T_CARGA CAR ON CAR.CAR_CODIGO = CAO.CAR_CODIGO
                LEFT JOIN T_CARGA_DADOS_SUMARIZADOS CDS ON CAR.CDS_CODIGO = CDS.CDS_CODIGO
                LEFT JOIN T_EMPRESA EMP ON EMP.EMP_CODIGO = CAR.EMP_CODIGO
                LEFT JOIN CTE_Placas Placas ON Placas.CAR_CODIGO = CAR.CAR_CODIGO 
                LEFT JOIN CTE_Motoristas Motoristas ON Motoristas.CAR_CODIGO = CAR.CAR_CODIGO");

            /* Filtros */

            query.Append(" WHERE 1 = 1");

            if (filtrosPesquisa.DataInicial.HasValue)
                query = query.Append($" AND CAR.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFim.HasValue)
                query = query.Append($" AND CAR.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataFim.Value.ToString(pattern)}'");

            if (filtrosPesquisa.CodigosCarga.Count > 0)
                query = query.Append($" AND CAR.CAR_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosCarga)})");

            if (filtrosPesquisa.TipoOperacao > 0)
                query = query.Append($" AND CAR.TOP_CODIGO = {filtrosPesquisa.TipoOperacao}");

            if (filtrosPesquisa.CodigosTiposCarga.Count > 0)
                query = query.Append($" AND CAR.TCG_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTiposCarga)})");

            if (filtrosPesquisa.CodigosFiliais.Count > 0)
                query = query.Append($" AND CAR.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosFiliais)})");

            if (filtrosPesquisa.CodigosTransportadores.Count > 0)
                query = query.Append($" AND EMP.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTransportadores)})");

            if (filtrosPesquisa.SituacaoOferta.HasValue && filtrosPesquisa.SituacaoOferta.Value > 0)
                query = query.Append($" AND CAO.CAO_SITUACAO = {(int)filtrosPesquisa.SituacaoOferta.Value}");

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                query = query.Append($" AND CAO.CAO_SITUACAO_INTEGRACAO = {(int)filtrosPesquisa.SituacaoIntegracao.Value}");

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeOrdenar))
            {
                query.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    query.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return query.ToString();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaOferta>> BuscarCargasOfertasExpiradasAsync(DateTime now, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOferta>()
                .Where(co => co.DataFimOferta.HasValue && co.DataFimOferta < now && co.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaOferta.EmOferta);
            return await query.Take(15).ToListAsync(cancellationToken);
        }

        #endregion Métodos Privados
    }
}