using Dominio.ObjetosDeValor.Embarcador.Imposto.OutrasAliquotas;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.OutrasAliquotas
{
    public class OutrasAliquotas : RepositorioBase<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas>
    {
        #region Construtores

        public OutrasAliquotas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public OutrasAliquotas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private string QueryConsultaOutrasAliquotas(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, bool somenteContarNumeroRegistros)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append(@"select 
                                OutrasAliquotas.TOA_CODIGO AS Codigo,  
                                TipoOperacao.TOP_CODIGO AS CodigoTipoOperacao,
                                ISNULL(OutrasAliquotas.TOA_ATIVO,0) AS StatusAtividade,                               
                                ISNULL(OutrasAliquotas.TOA_CALCULAR_IMPOSTO_DOCUMENTO,0) AS CalcularImpostoDocumento,
		                        OutrasAliquotas.TOA_CST As CST,
		                        OutrasAliquotas.TOA_CODIGO_INDICADOR_OPERACAO As CodigoIndicadorOperacao,
		                        OutrasAliquotas.TOA_CODIGO_CLASSE_TRIBUTARIA As CodigoClassificacaoTributaria,
		                        TipoOperacao.TOP_DESCRICAO As TipoOperacao,
                                OutrasAliquotas.TOA_ZERAR_BASE AS ZerarBase,
                                OutrasAliquotas.TOA_EXPORTACAO AS Exportacao
                            ");

            sql.Append(@" From T_OUTRAS_ALIQUOTAS OutrasAliquotas
	                       left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = OutrasAliquotas.TOP_CODIGO");

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
            {
                sql.Append($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametroConsulta.InicioRegistros} ROWS FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas BuscarPorCodigo(int codigoOutrasAliquotas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas>();

            var result = from obj in query where obj.Codigo == codigoOutrasAliquotas select obj;

            return result.FirstOrDefault();
        }

        public Task<int> ContarConsultaOutrasAliquotas()
        {
            string sql = QueryConsultaOutrasAliquotas(null, true);
            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas> BuscarTodosOuPorCST(string cst)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas>();
            query = query.Where(o => o.Ativo);

            if (!string.IsNullOrWhiteSpace(cst))
                query = query.Where(o => o.CST == cst);


            return query.ToList();
        }

        public Task<IList<DadosOutrasAliquotas>> ConsultaOutrasAliquotas(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            string sql = QueryConsultaOutrasAliquotas(parametroConsulta, false);
            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(DadosOutrasAliquotas)));

            return consulta.SetTimeout(600).ListAsync<DadosOutrasAliquotas>(CancellationToken);
        }

        public Task<bool> ExisteRegistroDuplicado(string cst, string codigoClassificacao, int codigoTipoOperacao, int codigoOutrasAliquotasDesconsiderar)
        {
            var consultaOutrasAliquotas = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas>()
                .Where(o => o.Ativo &&
                            o.CST == cst &&
                            o.CodigoClassificacaoTributaria == codigoClassificacao
                );

            if (codigoTipoOperacao > 0)
                consultaOutrasAliquotas = consultaOutrasAliquotas.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);
            else
                consultaOutrasAliquotas = consultaOutrasAliquotas.Where(o => o.TipoOperacao == null);

            if (codigoOutrasAliquotasDesconsiderar > 0)
                consultaOutrasAliquotas = consultaOutrasAliquotas.Where(o => o.Codigo != codigoOutrasAliquotasDesconsiderar);

            return consultaOutrasAliquotas.AnyAsync(CancellationToken);
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota>> BuscarTodasOutrasAliquotasAsync()
        {
            string sql = @"select 
                            outrasAliquotas.TOA_CODIGO Codigo,
                            outrasAliquotas.TOA_CST CST,
                            outrasAliquotas.TOA_CODIGO_CLASSE_TRIBUTARIA ClassificacaoTributaria,
                            outrasAliquotas.TOP_CODIGO CodigoTipoOperacao,
                            outrasAliquotas.TOA_ZERAR_BASE ZerarBase,
                            outrasAliquotas.TOA_EXPORTACAO Exportacao,
                            outrasAliquotas.TOA_CALCULAR_IMPOSTO_DOCUMENTO SomarImpostosDocumento,
                            outrasAliquotas.TOA_CODIGO_INDICADOR_OPERACAO CodigoIndicadorOperacao
                         from T_OUTRAS_ALIQUOTAS outrasAliquotas
                         where outrasAliquotas.TOA_ATIVO = 1";

            ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota)));

            return consulta.SetTimeout(600).ListAsync<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota>(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas> BuscarOutraAliquotaPorCodigoAsync(int codigo)
        {
            var consultaOutrasAliquotas = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas>()
                .Where(o => o.Codigo == codigo);

            return consultaOutrasAliquotas.FirstOrDefaultAsync(CancellationToken);
        }


        public Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas BuscarPorCSTClassificacaoTributaria(string cst, string classificaoTributaria)
        {
            var consultaOutrasAliquotas = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas>()
                .Where(o => o.CST == cst && o.CodigoClassificacaoTributaria == classificaoTributaria && o.Ativo);

            return consultaOutrasAliquotas.FirstOrDefault();
        }


        public bool ValidarExistenciaOutrasAliquotasEmDocumentos(int codigo)
        {

            var consultaCTeComplementar = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var consultaDocumentoContabil = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();
            var consultaDocumentoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            var consultaPedidoXMLNotaFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            var consultaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var consultaPreCTe = this.SessionNHiBernate.Query<Dominio.Entidades.PreConhecimentoDeTransporteEletronico>();

            var existeOutraAliquota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas>()
                .Where(x =>
                    x.Codigo == codigo &&
                    (
                        consultaCTeComplementar.Any(c => c.OutrasAliquotas.Codigo == x.Codigo) ||
                        consultaDocumentoContabil.Any(c => c.OutrasAliquotas.Codigo == x.Codigo) ||
                        consultaDocumentoProvisao.Any(c => c.OutrasAliquotas.Codigo == x.Codigo) ||
                        consultaPedidoXMLNotaFiscal.Any(c => c.OutrasAliquotas.Codigo == x.Codigo) ||
                        consultaCTe.Any(c => c.OutrasAliquotas.Codigo == x.Codigo) ||
                        consultaPreCTe.Any(c => c.OutrasAliquotas.Codigo == x.Codigo)
                    )
                );

            return existeOutraAliquota.Any();
        }

        #endregion
    }
}
