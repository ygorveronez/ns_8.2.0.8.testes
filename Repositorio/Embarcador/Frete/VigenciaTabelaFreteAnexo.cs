using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public sealed class VigenciaTabelaFreteAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFreteAnexo>
    {
        #region Construtores

        public VigenciaTabelaFreteAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private string ObterSqlPorAutorizacaoTabelaFrete(int codigoTabelaFrete, int inicioRegistros, int limiteRegistros, bool somenteContarNumeroRegistros)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append("select distinct(count(0) over ()) ");
            else
            {
                sql.Append("select VigenciaAnexo.ANX_CODIGO as Codigo, ");
                sql.Append("       VigenciaAnexo.ANX_DESCRICAO as Descricao, ");
                sql.Append("       VigenciaAnexo.ANX_NOME_ARQUIVO as NomeArquivo, ");
                sql.Append("       Vigencia.TFV_DATA_INICIAL as DataInicialVigencia, ");
                sql.Append("       Vigencia.TFV_DATA_FINAL as DataFinalVigencia ");
            }

            sql.Append("  from T_TABELA_FRETE_CLIENTE_ALTERACAO TabelaFreteClienteAlteracao ");
            sql.Append("  join T_TABELA_FRETE_CLIENTE TabelaFreteCliente ON TabelaFreteCliente.TFC_CODIGO = TabelaFreteClienteAlteracao.TFC_CODIGO ");
            sql.Append("  join T_TABELA_FRETE_VIGENCIA Vigencia on Vigencia.TFV_CODIGO = TabelaFreteCliente.TFV_CODIGO ");
            sql.Append("  join T_TABELA_FRETE_VIGENCIA_ANEXOS VigenciaAnexo on VigenciaAnexo.TFV_CODIGO = Vigencia.TFV_CODIGO ");
            sql.Append(" where TabelaFreteCliente.TFC_ATIVO = 1 ");
            sql.Append($"  and TabelaFreteCliente.TBF_CODIGO = {codigoTabelaFrete} ");
            sql.Append(" group by VigenciaAnexo.ANX_CODIGO, ");
            sql.Append("       VigenciaAnexo.ANX_DESCRICAO, ");
            sql.Append("       VigenciaAnexo.ANX_NOME_ARQUIVO, ");
            sql.Append("       Vigencia.TFV_DATA_INICIAL, ");
            sql.Append("       Vigencia.TFV_DATA_FINAL ");

            if (!somenteContarNumeroRegistros && ((inicioRegistros > 0) || (limiteRegistros > 0)))
                sql.Append($" order by DataInicialVigencia offset {inicioRegistros} rows fetch next {limiteRegistros} rows only;");

            return sql.ToString();
        }

        private string ObterSqlPorReajusteTabelaFrete(int codigoAjuste, int inicioRegistros, int limiteRegistros, bool somenteContarNumeroRegistros)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();
            if (somenteContarNumeroRegistros)
                sql.Append("select distinct(count(0) over ()) ");
            else
            {
                sql.Append("select distinct VigenciaAnexo.ANX_CODIGO as Codigo, ");
                sql.Append("       VigenciaAnexo.ANX_DESCRICAO as Descricao, ");
                sql.Append("       VigenciaAnexo.ANX_NOME_ARQUIVO as NomeArquivo, ");
                sql.Append("       Vigencia.TFV_DATA_INICIAL as DataInicialVigencia, ");
                sql.Append("       Vigencia.TFV_DATA_FINAL as DataFinalVigencia ");
            }

            sql.Append("  from T_TABELA_FRETE_AJUSTE TabelaFreteAjuste ");
            sql.Append("  join T_TABELA_FRETE_CLIENTE TabelaFreteCliente on TabelaFreteCliente.TFA_CODIGO = TabelaFreteAjuste.TFA_CODIGO ");
            sql.Append("  join T_TABELA_FRETE_VIGENCIA Vigencia on Vigencia.TFV_CODIGO = TabelaFreteCliente.TFV_CODIGO ");
            sql.Append("  join T_TABELA_FRETE_VIGENCIA_ANEXOS VigenciaAnexo on VigenciaAnexo.TFV_CODIGO = Vigencia.TFV_CODIGO ");
            sql.Append(" where TabelaFreteCliente.TFC_ATIVO = 1 ");
            sql.Append("   and TabelaFreteCliente.TFC_APLICAR_ALTERACOES_DO_AJUSTE = 1 ");
            sql.Append($"  and TabelaFreteCliente.TFA_CODIGO = {codigoAjuste} ");
            sql.Append("   and TabelaFreteAjuste.TFA_SITUACAO not in (10, 11) ");

            if (!somenteContarNumeroRegistros && ((inicioRegistros > 0) || (limiteRegistros > 0)))
                sql.Append($" order by DataInicialVigencia offset {inicioRegistros} rows fetch next {limiteRegistros} rows only;");

            return sql.ToString();
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFreteAnexo> BuscarPorTabela(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.VigenciaTabelaFreteAnexo>();

            var result = from obj in query where obj.EntidadeAnexo.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            return result.Fetch(obj => obj.EntidadeAnexo).ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo> ConsultarPorAutorizacaoTabelaFrete(int codigoTabelaFrete, int inicioRegistros, int limiteRegistros)
        {
            string sql = ObterSqlPorAutorizacaoTabelaFrete(codigoTabelaFrete, inicioRegistros, limiteRegistros, somenteContarNumeroRegistros: false);
            var consultaVigenciaAnexo = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaVigenciaAnexo.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo)));

            return consultaVigenciaAnexo.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo> ConsultarPorReajusteTabelaFrete(int codigoAjuste, int inicioRegistros, int limiteRegistros)
        {
            string sql = ObterSqlPorReajusteTabelaFrete(codigoAjuste, inicioRegistros, limiteRegistros, somenteContarNumeroRegistros: false);
            var consultaVigenciaAnexo = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaVigenciaAnexo.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo)));

            return consultaVigenciaAnexo.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.VigenciaAnexo>();
        }

        public int ContarConsultaPorAutorizacaoTabelaFrete(int codigoTabelaFrete)
        {
            string sql = ObterSqlPorAutorizacaoTabelaFrete(codigoTabelaFrete, inicioRegistros: 0, limiteRegistros: 0, somenteContarNumeroRegistros: true);
            var consultaVigenciaAnexo = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consultaVigenciaAnexo.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaPorReajusteTabelaFrete(int codigoAjuste)
        {
            string sql = ObterSqlPorReajusteTabelaFrete(codigoAjuste, inicioRegistros: 0, limiteRegistros: 0, somenteContarNumeroRegistros: true);
            var consultaVigenciaAnexo = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consultaVigenciaAnexo.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
