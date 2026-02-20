using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas.GestaoDadosColeta
{
    public class GestaoDadosColeta : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta>
    {
        #region Construtores

        public GestaoDadosColeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDadosColeta.GestaoDadosColeta> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            string sql = QueryGestaoDadosColeta(filtrosPesquisa, false, parametroConsulta);
            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDadosColeta.GestaoDadosColeta)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDadosColeta.GestaoDadosColeta>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            string sql = QueryGestaoDadosColeta(filtrosPesquisa, true, parametroConsulta);
            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta>();
            var result = from obj in query where obj.CargaEntrega.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        #endregion Métodos Públicos

        #region Métodos privados

        private string QueryGestaoDadosColeta(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else
                sql.Append($@"SELECT
                     gestaodadocoleta.GDC_CODIGO as Codigo,
                     gestaodadocoleta.GDC_TIPO as Tipo,
                     gestaodadocoleta.GDC_SITUACAO as Situacao,
                     gestaodadocoleta.GDC_ORIGEM as Origem,
                     carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador,
                     carga.CAR_DATA_CRIACAO as DataCriacaoCarga,
                     gestaodadocoleta.GDC_DATA_APROVACAO as DataAprovacao,
                     cliente.CLI_NOME as NomeCliente,
		             sumarizado.CDS_DESTINOS as DestinoCarga,
                     sumarizado.CDS_ORIGENS as OrigemCarga,   
                     localidade.LOC_DESCRICAO as DescricaoLocalidade,
                     usuario.FUN_NOME as NomeUsuario,
                     usuario.FUN_MOTORISTA_ESTRANGEIRO as MotoristaEstrangeiroUsuario,
                     usuario.FUN_CPF as CPFUsuario,
                     usuario.FUN_AMBIENTE as TipoAcessoUsuario,
                     filial.FIL_DESCRICAO as Filial,
                     gestaodadocoleta.GDC_DATA_CRIACAO as DataInicialCriacaoCarga,
                     gestaodadocoleta.GDC_DATA_RETORNO_CONFIRMACAO_COLETA DataRetornoConfirmacaoColeta,
                     gestaodadocoleta.GDC_ERRO_RETORNO_CONFIRMACAO_COLETA ErroRetornoConfirmacaoColeta,
                     gestaodadocoleta.GDC_IDEXTERNO_RETORNO_CONFIRMACAO_COLETA IdExternoRetornoConfirmacaoColeta,
                     gestaodadocoleta.GDC_OPERACAO_RETORNO_CONFIRMACAO_COLETA OperacaoRetornoConfirmacaoColeta
                ");

            sql.Append(" from T_GESTAO_DADOS_COLETA gestaodadocoleta ");
            sql.Append(@"
                 left outer join T_EMPRESA empresa on gestaodadocoleta.EMP_CODIGO = empresa.EMP_CODIGO
                 left outer join T_FUNCIONARIO usuario on gestaodadocoleta.FUN_USUARIO_APROVACAO=usuario.FUN_CODIGO 
                 left outer join T_CARGA_ENTREGA cargaentrega on gestaodadocoleta.CEN_CODIGO = cargaentrega.CEN_CODIGO 
                 left outer join T_CARGA carga on cargaentrega.CAR_CODIGO = carga.CAR_CODIGO 
 	             left outer join T_CARGA_DADOS_SUMARIZADOS sumarizado ON carga.CDS_CODIGO = sumarizado.CDS_CODIGO
                 left outer join T_LOCALIDADES localidade on cargaentrega.LOC_CODIGO = localidade.LOC_CODIGO 
                 left outer join T_CLIENTE cliente on cargaentrega.CLI_CODIGO_ENTREGA = cliente.CLI_CGCCPF 
                 left outer join T_FILIAL filial ON carga.FIL_CODIGO = filial.FIL_CODIGO
            ");

            sql.Append(" WHERE 1 = 1 ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                sql.Append($" AND carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");

            if (filtrosPesquisa.CodigoTransportador > 0)
                sql.Append($" AND empresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.DataInicialCriacaoCarga != DateTime.MinValue)
                sql.Append($" AND carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicialCriacaoCarga.Date:yyyyMMdd HH:mm:ss}'");

            if (filtrosPesquisa.DataFinalCriacaoCarga != DateTime.MinValue)
                sql.Append($" AND carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataFinalCriacaoCarga.AddDays(1).Date:yyyyMMdd HH:mm:ss}'");

            if (filtrosPesquisa.SituacaoGestaoDadosColeta != null)
                sql.Append($" AND gestaodadocoleta.GDC_SITUACAO = {(int)filtrosPesquisa.SituacaoGestaoDadosColeta}");

            if (filtrosPesquisa.CodigoFilialEmbarcador > 0)
                sql.Append($" AND carga.FIL_CODIGO = {filtrosPesquisa.CodigoFilialEmbarcador}");

            if (filtrosPesquisa.CodigoCliente > 0)
                sql.Append($" AND cargaentrega.CLI_CODIGO_ENTREGA = {filtrosPesquisa.CodigoCliente}");

            if (filtrosPesquisa.OrigemCarga > 0)
                sql.Append($" AND EXISTS (SELECT 1 FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CAR_CODIGO = carga.CAR_CODIGO AND cargaPedido.LOC_CODIGO_ORIGEM = {filtrosPesquisa.OrigemCarga})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DestinoCarga > 0)
                sql.Append($" AND EXISTS (SELECT 1 FROM T_CARGA_PEDIDO cargaPedido WHERE cargaPedido.CAR_CODIGO = carga.CAR_CODIGO AND cargaPedido.LOC_CODIGO_DESTINO = {filtrosPesquisa.DestinoCarga})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.RetornoConfirmacao != null)
            {
                if (filtrosPesquisa.RetornoConfirmacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoDadosColetaRetornoConfirmacao.ComErro)
                    sql.Append($" AND (gestaodadocoleta.GDC_ERRO_RETORNO_CONFIRMACAO_COLETA is not null AND gestaodadocoleta.GDC_ERRO_RETORNO_CONFIRMACAO_COLETA != '') ");
                else if (filtrosPesquisa.RetornoConfirmacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoDadosColetaRetornoConfirmacao.SemErro)
                    sql.Append($" AND (gestaodadocoleta.GDC_ERRO_RETORNO_CONFIRMACAO_COLETA is not null AND gestaodadocoleta.GDC_ERRO_RETORNO_CONFIRMACAO_COLETA = '') ");
            }

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
            {
                sql.Append($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametroConsulta.InicioRegistros} ROWS FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        #endregion Métodos Privados
    }
}
