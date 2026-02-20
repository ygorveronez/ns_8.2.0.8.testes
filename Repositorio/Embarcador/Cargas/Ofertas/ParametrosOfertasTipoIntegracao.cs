using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertasTipoIntegracao : RepositorioRelacionamentoParametrosOfertas<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao>, Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRepositorioRelacionamentoParametrosOfertas
    {
        #region Construtores

        public ParametrosOfertasTipoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        public async Task DeletarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, int codigoParametrosOfertas)
        {
            var op = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao>();

            await op.Where(o => o.TipoIntegracao == tipo && o.ParametrosOfertas.Codigo == codigoParametrosOfertas).DeleteAsync();

        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>> BuscarPorOfertaAsync(List<long> codigosOfertas, CancellationToken cancellationToken)
        {
            string sql = @$"select 
                        tpi.TPI_CODIGO as Codigo,
                        tpi.TPI_TIPO as Tipo,
                        tpi.TPI_GRUPO as Grupo,
                        tpi.TPI_TIPO_ENVIO as TipoEnvio,
                        tpi.TPI_QTD_MAXIMA_ENVIO_LOTE as QuantidadeMaximaEnvioLote,
                        tpi.TPI_GERAR_INTEGRACAO_NAS_OCORRENCIAS as GerarIntegracaoNasOcorrencias,
                        tpi.TPI_DESCRICAO as Descricao,
                        tpi.TPI_CONTROLE_POR_LOTE as ControlePorLote,
                        tpi.TPI_INTEGRACAO_TRANSPORTADOR as IntegracaoTransportador,
                        tpi.TPI_INTEGRAR_CARGA_TRANSBORDO as IntegrarCargaTransbordo,
                        tpi.TPI_GERAR_INTEGRACAO_FECHAMENTO_CARGA as GerarIntegracaoFechamentoCarga,
                        tpi.TPI_GERAR_INTEGRACAO_DADOS_TRANSPORTE_CARGA as GerarIntegracaoDadosTransporteCarga,
                        tpi.TPI_ATIVO as Ativo,
                        tpi.TPI_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO as NaoSubtrairValePedagioDoContrato,
                        tpi.TPI_TENTATIVAS as Tentativas,
                        tpi.TPI_INTEGRAR_VEICULO_TROCA_MOTORISTA as IntegrarVeiculoTrocaMotorista,
                        tpi.TPI_INTEGRAR_PEDIDOS as IntegrarPedidos,
                        tpi.TPI_INTEGRAR_COM_PLATAFORMA_NSTECH as IntegrarComPlataformaNstech,
                        CAST (tpi.TPI_TEMPO_CONSULTA_INTEGRACAO as INT) as TempoConsultaIntegracao,
                        tpi.TPI_RASTREADOR as Rastreador,
                        tpi.TPI_BLOQUEAR_ETAPA_TRANSPORTE_SE_REJEITAR as BloquearEtapaTransporteSeRejeitar,
                        tpi.TPI_PERMITIR_REENVIO_EXCECAO as PermitirReenvioExcecao
                        from 
                        T_carga_oferta cao inner join t_parametros_ofertas pof on cao.pof_codigo = pof.pof_codigo
                        inner join t_parametros_ofertas_tipo_integracao pti on pof.pof_codigo = pti.pof_codigo 
                        inner join t_tipo_integracao tpi on pti.pti_tipo_integracao = tpi.tpi_tipo
                        where cao.cao_codigo in ({string.Join(",", codigosOfertas)})";

            var query = SessionNHiBernate.CreateSQLQuery(sql).
                SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao)));

            return query.List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>()
                .ToDynamicListAsync<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>(cancellationToken);
        }


        public Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao>> BuscarPorParametrosOfertasAsync(List<int> codigosParametrosOfertas, CancellationToken cancellationToken)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasTipoIntegracao>()
                .Where(obj => codigosParametrosOfertas.Contains(obj.ParametrosOfertas.Codigo));

            return query.ToListAsync(cancellationToken);
        }

        public Task<int> PossuiTipoIntegracaoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, int codigoParametrosOfertas)
        {
            string query = @$"select PTI_CODIGO from T_PARAMETROS_OFERTAS_TIPO_INTEGRACAO where PTI_TIPO_INTEGRACAO = {(int)tipo} AND POF_CODIGO = {codigoParametrosOfertas}";
            var result = this.SessionNHiBernate.CreateSQLQuery(query);

            return result.SetTimeout(600).UniqueResultAsync<int>();
        }
    }
}
