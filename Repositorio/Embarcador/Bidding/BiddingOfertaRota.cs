using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingOfertaRota : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>
    {
        public BiddingOfertaRota(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public BiddingOfertaRota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> BuscarRotas(int codigoBiddingOferta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>()
                .Where(o => o.BiddingOferta.Codigo == codigoBiddingOferta);

            return query
                .Fetch(x => x.TiposCarga)
                .Fetch(x => x.ModelosVeiculares)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>> BuscarRotasAsync(int codigoBiddingOferta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>()
                .Where(o => o.BiddingOferta.Codigo == codigoBiddingOferta);

            return query
                .Fetch(x => x.TiposCarga)
                .Fetch(x => x.ModelosVeiculares)
                .ToListAsync(CancellationToken);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados> BuscarRotasProcessadas(int codigoBiddingOferta)
        {
            if (codigoBiddingOferta == 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados>();


            string sql = $@"WITH Filiais AS (
                                SELECT DISTINCT 
                                    fp.TBR_CODIGO, 
                                    f.FIL_CODIGO AS Codigo, 
                                    f.FIL_DESCRICAO + ' - ' + f.FIL_CNPJ AS Descricao
                                FROM T_BIDDING_OFERTA_FILIAL_PARTICIPANTE fp
                                JOIN T_FILIAL f ON fp.FIL_CODIGO = f.FIL_CODIGO
                            ),
                            Origens AS (
                                SELECT DISTINCT 
                                    ro.TBR_CODIGO, 
                                    l.LOC_CODIGO AS Codigo, 
                                    l.LOC_DESCRICAO AS Descricao,
		                            uf.RBR_CODIGO as OrigensRegiaoCodigo,
		                            regiao.RBR_DESCRICAO AS OrigensRegiaoDescricao
                                FROM T_BIDDING_OFERTA_ROTA_ORIGEM ro
                                JOIN T_LOCALIDADES l ON ro.LOC_CODIGO = l.LOC_CODIGO
	                            JOIN T_UF uf on uf.UF_SIGLA = l.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            ),
                            Destinos AS (
                                SELECT DISTINCT 
                                    rd.TBR_CODIGO, 
                                    l.LOC_CODIGO AS Codigo, 
                                    l.LOC_DESCRICAO AS Descricao,
		                            uf.RBR_CODIGO as DestinosRegiaoCodigo,
		                            regiao.RBR_DESCRICAO AS DestinosRegiaoDescricao
                                FROM T_BIDDING_OFERTA_ROTA_DESTINO rd
                                JOIN T_LOCALIDADES l ON rd.LOC_CODIGO = l.LOC_CODIGO
	                            JOIN T_UF uf on uf.UF_SIGLA = l.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            ),
                            RegioesDestino AS ( 
                                SELECT DISTINCT 
                                    ro.TBR_CODIGO, 
                                    regiao.RBR_CODIGO AS Codigo, 
                                    regiao.RBR_DESCRICAO AS Descricao
                                FROM T_BIDDING_OFERTA_ROTA_REGIAO_DESTINO ro
                                JOIN T_LOCALIDADES r ON ro.REG_CODIGO = r.REG_CODIGO
	                            JOIN T_UF uf on uf.UF_SIGLA = r.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            ),
                            RegioesOrigem AS ( 
                                SELECT DISTINCT 
                                    ro.TBR_CODIGO, 
                                    regiao.RBR_CODIGO AS Codigo, 
                                    regiao.RBR_DESCRICAO AS Descricao
                                FROM T_BIDDING_OFERTA_ROTA_REGIAO_ORIGEM ro
                                JOIN T_LOCALIDADES r ON ro.REG_CODIGO = r.REG_CODIGO
	                            JOIN T_UF uf on uf.UF_SIGLA = r.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            ),
                            ModelosVeiculares AS (
                                SELECT DISTINCT 
                                    mv.TBR_CODIGO, 
                                    mv.VEI_CODIGO AS Codigo, 
                                    modelo.MVC_DESCRICAO AS Descricao
                                FROM T_BIDDING_OFERTA_ROTA_MODELO_VEICULAR mv
	                            join T_MODELO_VEICULAR_CARGA modelo on mv.VEI_CODIGO = modelo.MVC_CODIGO 
                            ),
                            PossuiCEPDestino AS (
                                SELECT 
                                    TBR_CODIGO, 
                                    CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS Existe
                                FROM T_BIDDING_OFERTA_ROTA_CEP_DESTINO
                                WHERE TCD_CODIGO > 0 
                                GROUP BY TBR_CODIGO
                            ),
                            PossuiCEPOrigem AS (
                                SELECT 
                                    TBR_CODIGO, 
                                    CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS Existe
                                FROM T_BIDDING_OFERTA_ROTA_CEP_ORIGEM
                                WHERE TCO_CODIGO > 0 
                                GROUP BY TBR_CODIGO
                            ),
                            MesorregioesDestino AS (
                                SELECT DISTINCT 
                                    md.TBR_CODIGO, 
                                    m.REG_CODIGO AS Codigo, 
                                    m.REG_DESCRICAO AS Descricao
                                FROM T_BIDDING_OFERTA_ROTA_REGIAO_DESTINO md
                                JOIN T_REGIAO m ON md.REG_CODIGO = m.REG_CODIGO
                            ),
                            MesorregioesOrigem AS (
                                SELECT DISTINCT 
                                    mo.TBR_CODIGO, 
                                    m.REG_CODIGO AS Codigo, 
                                    m.REG_DESCRICAO AS Descricao
                                FROM T_BIDDING_OFERTA_ROTA_REGIAO_ORIGEM mo
                                JOIN T_REGIAO m ON mo.REG_CODIGO = m.REG_CODIGO
                            ),
                            ClienteDestino AS (
                                SELECT DISTINCT 
                                    cli.TBR_CODIGO, 
                                    c.CLI_CGCCPF AS Codigo, 
                                    c.CLI_NOMEFANTASIA AS Descricao,
		                            uf.RBR_CODIGO as ClienteDestinoRegiaoCodigo,
		                            regiao.RBR_DESCRICAO as ClienteDestinoRegiaoDescricao
                                FROM T_BIDDING_OFERTA_ROTA_CLIENTE_DESTINO cli
                                JOIN T_CLIENTE c ON c.CLI_CGCCPF = c.CLI_CGCCPF
	                            JOIN T_LOCALIDADES r ON c.LOC_CODIGO = r.LOC_CODIGO
	                            JOIN T_UF uf on uf.UF_SIGLA = r.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            ),
                            ClienteOrigem AS (
                                SELECT DISTINCT 
                                    cli.TBR_CODIGO, 
                                    c.CLI_CGCCPF AS Codigo, 
                                    c.CLI_NOMEFANTASIA AS Descricao,
		                            uf.RBR_CODIGO as ClienteOrigemRegiaoCodigo,
		                            regiao.RBR_DESCRICAO as ClienteOrigemRegiaoDescricao
                                FROM T_BIDDING_OFERTA_ROTA_CLIENTE_ORIGEM cli
                                JOIN T_CLIENTE c ON c.CLI_CGCCPF = cli.CLI_CGCCPF
	                            JOIN T_LOCALIDADES r ON c.LOC_CODIGO = r.LOC_CODIGO
	                            JOIN T_UF uf on uf.UF_SIGLA = r.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            ),
                            RotaDestino AS (
                                SELECT DISTINCT 
                                    rotaDestino.TBR_CODIGO, 
                                    rotaFrete.CLI_CGCCPF AS Codigo, 
                                    rotaFrete.ROF_DESCRICAO AS Descricao,
		                            uf.RBR_CODIGO as RotaDestinoRegiaoCodigo,
		                            regiao.RBR_DESCRICAO as RotaDestinoRegiaoDescricao
                                FROM T_BIDDING_OFERTA_ROTA_ROTA_DESTINO rotaDestino
                                JOIN T_ROTA_FRETE rotaFrete ON rotaFrete.ROF_CODIGO = rotaDestino.ROF_CODIGO
	                            JOIN T_ROTA_FRETE_LOCALIDADE_ORIGEM rotaLocalidade ON rotaFrete.ROF_CODIGO = rotaLocalidade.ROF_CODIGO
	                            JOIN T_LOCALIDADES r ON rotaLocalidade.LOC_CODIGO = r.LOC_CODIGO
	                            JOIN T_UF uf on uf.UF_SIGLA = r.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
	                            ),
                            RotaOrigem AS (
                                SELECT DISTINCT 
                                    rotaDestino.TBR_CODIGO, 
                                    rotaFrete.CLI_CGCCPF AS Codigo, 
                                    rotaFrete.ROF_DESCRICAO AS Descricao,
		                            uf.RBR_CODIGO as RotaOrigemRegiaoCodigo,		
		                            regiao.RBR_DESCRICAO as RotaOrigemRegiaoDescricao
                                FROM T_BIDDING_OFERTA_ROTA_ROTA_ORIGEM rotaDestino
                                JOIN T_ROTA_FRETE rotaFrete ON rotaFrete.ROF_CODIGO = rotaDestino.ROF_CODIGO
	                            JOIN T_ROTA_FRETE_LOCALIDADE_ORIGEM rotaLocalidade ON rotaFrete.ROF_CODIGO = rotaLocalidade.ROF_CODIGO
	                            JOIN T_LOCALIDADES r ON rotaLocalidade.LOC_CODIGO = r.LOC_CODIGO
	                            JOIN T_UF uf on uf.UF_SIGLA = r.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            ),
                            EstadoDestino AS (
                                SELECT DISTINCT 
                                    estadoDestino.TBR_CODIGO, 
                                    uf.UF_SIGLA AS Codigo, 
                                    uf.UF_NOME AS Descricao,
		                            uf.RBR_CODIGO as EstadoDestinoRegiaoCodigo,
		                            regiao.RBR_DESCRICAO as EstadoDestinoRegiaoDescricao
                                FROM T_BIDDING_OFERTA_ROTA_ESTADO_DESTINO estadoDestino
                                JOIN T_UF uf ON uf.UF_SIGLA = estadoDestino.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            ),
                            EstadoOrigem AS (
                                SELECT DISTINCT 
                                    estadoOrigem.TBR_CODIGO, 
                                    uf.UF_SIGLA AS Codigo, 
                                    uf.UF_NOME AS Descricao,
		                            uf.RBR_CODIGO as EstadoOrigemRegiaoCodigo,
		                            regiao.RBR_DESCRICAO as EstadoOrigemRegiaoDescricao
                                FROM T_BIDDING_OFERTA_ROTA_ESTADO_ORIGEM estadoOrigem
                                JOIN T_UF uf ON uf.UF_SIGLA = estadoOrigem.UF_SIGLA
	                            JOIN T_REGIAO_BRASIL regiao on uf.RBR_CODIGO = regiao.RBR_CODIGO
                            )
                            ,
                            PaisOrigem AS (
                                SELECT DISTINCT 
                                    PaisOrigem.TBR_CODIGO, 
                                    pais.PAI_CODIGO AS Codigo, 
                                    pais.PAI_NOME AS Descricao
                                FROM T_BIDDING_OFERTA_ROTA_PAIS_ORIGEM PaisOrigem
                                JOIN T_PAIS pais ON pais.PAI_CODIGO = PaisOrigem.PAI_CODIGO
                            ),
                            PaisDestino AS (
                                SELECT DISTINCT 
                                    PaisDestino.TBR_CODIGO, 
                                    pais.PAI_CODIGO AS Codigo, 
                                    pais.PAI_NOME AS Descricao
                                FROM T_BIDDING_OFERTA_ROTA_PAIS_DESTINO PaisDestino
                                JOIN T_PAIS pais ON pais.PAI_CODIGO = PaisDestino.PAI_CODIGO
                            )

                            SELECT 
                                r.TBR_CODIGO AS Codigo,
                                r.TBR_DESCRICAO AS Descricao,

                                -- Filiais
                            (SELECT COALESCE(STRING_AGG(CAST(f.Codigo AS VARCHAR), ', '), '') FROM Filiais f WHERE f.TBR_CODIGO = r.TBR_CODIGO) AS FiliaisCodigos,
                            (SELECT COALESCE(STRING_AGG(f.Descricao, ', '), '') FROM Filiais f WHERE f.TBR_CODIGO = r.TBR_CODIGO) AS FiliaisDescricoes,

                            -- Origens
                            (SELECT COALESCE(STRING_AGG(CAST(o.Codigo AS VARCHAR), ', '), '') FROM Origens o WHERE o.TBR_CODIGO = r.TBR_CODIGO) AS OrigensCodigos,
                            (SELECT COALESCE(STRING_AGG(CAST(o.OrigensRegiaoCodigo AS VARCHAR), ', '), '') FROM Origens o WHERE o.TBR_CODIGO = r.TBR_CODIGO) AS OrigensRegiaoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(o.OrigensRegiaoDescricao AS VARCHAR), ', '), '') FROM Origens o WHERE o.TBR_CODIGO = r.TBR_CODIGO) AS OrigensRegiaoDescricao,
                            (SELECT COALESCE(STRING_AGG(o.Descricao, ', '), '') FROM Origens o WHERE o.TBR_CODIGO = r.TBR_CODIGO) AS OrigensDescricoes,

                            -- Destinos
                            (SELECT COALESCE(STRING_AGG(CAST(d.Codigo AS VARCHAR), ', '), '') FROM Destinos d WHERE d.TBR_CODIGO = r.TBR_CODIGO) AS DestinosCodigos,
                            (SELECT COALESCE(STRING_AGG(CAST(d.DestinosRegiaoCodigo AS VARCHAR), ', '), '') FROM Destinos d WHERE d.TBR_CODIGO = r.TBR_CODIGO) AS DestinosRegiaoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(d.DestinosRegiaoDescricao AS VARCHAR), ', '), '') FROM Destinos d WHERE d.TBR_CODIGO = r.TBR_CODIGO) AS DestinosRegiaoDescricao,
                            (SELECT COALESCE(STRING_AGG(d.Descricao, ', '), '') FROM Destinos d WHERE d.TBR_CODIGO = r.TBR_CODIGO) AS DestinosDescricoes,

                            -- Regiões Destino
                            (SELECT COALESCE(STRING_AGG(CAST(rd.Codigo AS VARCHAR), ', '), '') FROM RegioesDestino rd WHERE rd.TBR_CODIGO = r.TBR_CODIGO) AS RegioesDestinoCodigos,
                            (SELECT COALESCE(STRING_AGG(rd.Descricao, ', '), '') FROM RegioesDestino rd WHERE rd.TBR_CODIGO = r.TBR_CODIGO) AS RegioesDestinoDescricoes,

                            -- Regiões Origem
                            (SELECT COALESCE(STRING_AGG(CAST(ro.Codigo AS VARCHAR), ', '), '') FROM RegioesOrigem ro WHERE ro.TBR_CODIGO = r.TBR_CODIGO) AS RegioesOrigemCodigos,
                            (SELECT COALESCE(STRING_AGG(ro.Descricao, ', '), '') FROM RegioesOrigem ro WHERE ro.TBR_CODIGO = r.TBR_CODIGO) AS RegioesOrigemDescricoes,

                            -- Modelos Veiculares
                            (SELECT COALESCE(STRING_AGG(CAST(mv.Codigo AS VARCHAR), ', '), '') FROM ModelosVeiculares mv WHERE mv.TBR_CODIGO = r.TBR_CODIGO) AS ModelosVeicularesCodigos,
                            (SELECT COALESCE(STRING_AGG(mv.Descricao, ', '), '') FROM ModelosVeiculares mv WHERE mv.TBR_CODIGO = r.TBR_CODIGO) AS ModelosVeicularesDescricoes,

                            -- Mesorregiões Origem
                            (SELECT COALESCE(STRING_AGG(CAST(mo.Codigo AS VARCHAR), ', '), '') FROM MesorregioesOrigem mo WHERE mo.TBR_CODIGO = r.TBR_CODIGO) AS MesorregioesOrigemCodigo,
                            (SELECT COALESCE(STRING_AGG(mo.Descricao, ', '), '') FROM MesorregioesOrigem mo WHERE mo.TBR_CODIGO = r.TBR_CODIGO) AS MesorregioesOrigem,

                            -- Mesorregiões Destino
                            (SELECT COALESCE(STRING_AGG(CAST(md.Codigo AS VARCHAR), ', '), '') FROM MesorregioesDestino md WHERE md.TBR_CODIGO = r.TBR_CODIGO) AS MesorregioesDestinoCodigo,
                            (SELECT COALESCE(STRING_AGG(md.Descricao, ', '), '') FROM MesorregioesDestino md WHERE md.TBR_CODIGO = r.TBR_CODIGO) AS MesorregioesDestino,

                            -- Cliente Destino
                            (SELECT COALESCE(STRING_AGG(CAST(cli.Codigo AS VARCHAR), ', '), '') FROM ClienteDestino cli WHERE cli.TBR_CODIGO = r.TBR_CODIGO) AS ClienteDestinoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(cli.ClienteDestinoRegiaoCodigo AS VARCHAR), ', '), '') FROM ClienteDestino cli WHERE cli.TBR_CODIGO = r.TBR_CODIGO) AS ClienteDestinoRegiaoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(cli.ClienteDestinoRegiaoDescricao AS VARCHAR), ', '), '') FROM ClienteDestino cli WHERE cli.TBR_CODIGO = r.TBR_CODIGO) AS ClienteDestinoRegiaoDescricao,
                            (SELECT COALESCE(STRING_AGG(cli.Descricao, ', '), '') FROM ClienteDestino cli WHERE cli.TBR_CODIGO = r.TBR_CODIGO) AS ClienteDestino,

                            -- Cliente Origem
                            (SELECT COALESCE(STRING_AGG(CAST(cli.Codigo AS VARCHAR), ', '), '') FROM ClienteOrigem cli WHERE cli.TBR_CODIGO = r.TBR_CODIGO) AS ClienteOrigemCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(cli.ClienteOrigemRegiaoCodigo AS VARCHAR), ', '), '') FROM ClienteOrigem cli WHERE cli.TBR_CODIGO = r.TBR_CODIGO) AS ClienteOrigemRegiaoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(cli.ClienteOrigemRegiaoDescricao AS VARCHAR), ', '), '') FROM ClienteOrigem cli WHERE cli.TBR_CODIGO = r.TBR_CODIGO) AS ClienteOrigemRegiaoDescricao,
                            (SELECT COALESCE(STRING_AGG(cli.Descricao, ', '), '') FROM ClienteOrigem cli WHERE cli.TBR_CODIGO = r.TBR_CODIGO) AS ClienteOrigem,

                            -- Rota Origem
                            (SELECT COALESCE(STRING_AGG(CAST(rotaOrigem.Codigo AS VARCHAR), ', '), '') FROM RotaOrigem rotaOrigem WHERE rotaOrigem.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigemCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(rotaOrigem.RotaOrigemRegiaoCodigo AS VARCHAR), ', '), '') FROM RotaOrigem rotaOrigem WHERE rotaOrigem.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigemRegiaoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(rotaOrigem.RotaOrigemRegiaoDescricao AS VARCHAR), ', '), '') FROM RotaOrigem rotaOrigem WHERE rotaOrigem.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigemRegiaoDescricao,
                            (SELECT COALESCE(STRING_AGG(rotaOrigem.Descricao, ', '), '') FROM RotaOrigem rotaOrigem WHERE rotaOrigem.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigem,

                            -- Rota Destino
                            (SELECT COALESCE(STRING_AGG(CAST(rotaDestino.Codigo AS VARCHAR), ', '), '') FROM RotaDestino rotaDestino WHERE rotaDestino.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigemCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(rotaDestino.RotaDestinoRegiaoCodigo AS VARCHAR), ', '), '') FROM RotaDestino rotaDestino WHERE rotaDestino.TBR_CODIGO = r.TBR_CODIGO) AS RotaDestinoRegiaoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(rotaDestino.RotaDestinoRegiaoDescricao AS VARCHAR), ', '), '') FROM RotaDestino rotaDestino WHERE rotaDestino.TBR_CODIGO = r.TBR_CODIGO) AS RotaDestinoRegiaoDescricao,
                            (SELECT COALESCE(STRING_AGG(rotaDestino.Descricao, ', '), '') FROM RotaDestino rotaDestino WHERE rotaDestino.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigem,

                            -- Estado Destino
                            (SELECT COALESCE(STRING_AGG(CAST(estadoDestino.Codigo AS VARCHAR), ', '), '') FROM EstadoDestino estadoDestino WHERE estadoDestino.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigemCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(estadoDestino.EstadoDestinoRegiaoCodigo AS VARCHAR), ', '), '') FROM EstadoDestino estadoDestino WHERE estadoDestino.TBR_CODIGO = r.TBR_CODIGO) AS EstadoDestinoRegiaoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(estadoDestino.EstadoDestinoRegiaoDescricao AS VARCHAR), ', '), '') FROM EstadoDestino estadoDestino WHERE estadoDestino.TBR_CODIGO = r.TBR_CODIGO) AS EstadoDestinoRegiaoDescricao,
                            (SELECT COALESCE(STRING_AGG(estadoDestino.Descricao, ', '), '') FROM EstadoDestino estadoDestino WHERE estadoDestino.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigem,

                            -- Estado Origem
                            (SELECT COALESCE(STRING_AGG(CAST(estadoOrigem.Codigo AS VARCHAR), ', '), '') FROM EstadoOrigem estadoOrigem WHERE estadoOrigem.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigemCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(estadoOrigem.EstadoOrigemRegiaoCodigo AS VARCHAR), ', '), '') FROM EstadoOrigem estadoOrigem WHERE estadoOrigem.TBR_CODIGO = r.TBR_CODIGO) AS EstadoOrigemRegiaoCodigo,
                            (SELECT COALESCE(STRING_AGG(CAST(estadoOrigem.EstadoOrigemRegiaoDescricao AS VARCHAR), ', '), '') FROM EstadoOrigem estadoOrigem WHERE estadoOrigem.TBR_CODIGO = r.TBR_CODIGO) AS EstadoOrigemRegiaoDescricao,
                            (SELECT COALESCE(STRING_AGG(estadoOrigem.Descricao, ', '), '') FROM EstadoOrigem estadoOrigem WHERE estadoOrigem.TBR_CODIGO = r.TBR_CODIGO) AS RotaOrigem,

                            -- Pais Origem
                            (SELECT COALESCE(STRING_AGG(CAST(paisOrigem.Codigo AS VARCHAR), ', '), '') FROM PaisOrigem paisOrigem WHERE paisOrigem.TBR_CODIGO = r.TBR_CODIGO) AS PaisOrigemCodigo,
                            (SELECT COALESCE(STRING_AGG(paisOrigem.Descricao, ', '), '') FROM PaisOrigem paisOrigem WHERE paisOrigem.TBR_CODIGO = r.TBR_CODIGO) AS PaisOrigem,

                            -- Pais Destino
                            (SELECT COALESCE(STRING_AGG(CAST(paisDestino.Codigo AS VARCHAR), ', '), '') FROM PaisDestino paisDestino WHERE paisDestino.TBR_CODIGO = r.TBR_CODIGO) AS PaisDestinoCodigo,
                            (SELECT COALESCE(STRING_AGG(paisDestino.Descricao, ', '), '') FROM PaisDestino paisDestino WHERE paisDestino.TBR_CODIGO = r.TBR_CODIGO) AS PaisDestino,

	                            COALESCE(pc.Existe, 0) AS PossuiCEPDestino,
                                COALESCE(po.Existe, 0) AS PossuiCEPOrigem,
                                -- Outras Informações
                                r.TBR_NUMERO_ENTREGA AS QuantidadeEntregas,
                                r.TBR_QUANTIDADE_AJUDANTE_POR_VEICULO AS QuantidadeAjudantes,
                               r.TBR_QUANTIDADE_VIAGENS_POR_ANO AS QuantidadeViagensAno
                            FROM T_BIDDING_OFERTA_ROTA r
                            LEFT JOIN PossuiCEPDestino pc ON r.TBR_CODIGO = pc.TBR_CODIGO
                            LEFT JOIN PossuiCEPOrigem po ON r.TBR_CODIGO = po.TBR_CODIGO
                            WHERE r.TBO_CODIGO = :codigoBidding;";

            return this.SessionNHiBernate
                .CreateSQLQuery(sql)
                .SetInt64("codigoBidding", codigoBiddingOferta)
                .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados)))
                .List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingOfertaRotaDados>();

        }


        public List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> BuscarRotasPorCodigo(List<int> codigosRota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>()
                .Where(o => codigosRota.Contains(o.Codigo));

            return query.Fetch(x => x.ModelosVeiculares).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> BuscarRotasPorBidding(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeConvite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>()
                .Where(o => o.BiddingOferta.BiddingConvite == entidadeConvite);

            return query.ToList();
        }

        public async Task<int> BuscarUltimoProtocoloImportacaoAsync()
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>();

            int? maxProtocolo = await query.MaxAsync(o => (int?)o.ProtocoloImportacao);
            return maxProtocolo ?? 0;
        }

        public Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota BuscarPorProtocoloImportacao(int protocoloImportacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>();
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> result = from obj in query where obj.ProtocoloImportacao == protocoloImportacao select obj;
            return result.FirstOrDefault();
        }
    }
}
