using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete.ImportacaoTabelaFrete
{

    public class ImportacaoTabelaFreteLinhaColuna : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna>
    {
        public ImportacaoTabelaFreteLinhaColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna> BuscarPorImportacaoPendentes(int codigoImportacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna>();

            query = query.Where(o => o.Linha.ImportacaoTabelaFrete.Codigo == codigoImportacao && o.Linha.TabelaFreteCliente == null);

            return query.Fetch(o => o.Linha).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna> BuscarPorLinha(int codigoLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinhaColuna>();

            query = query.Where(o => o.Linha.Codigo == codigoLinha);

            return query.Fetch(o => o.Linha).ToList();
        }

        public void InsertSQL(Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha Linha, List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna> ListaColunas)
        {
            string parameros = "( :ITL_CODIGO_[X], :ITC_NOME_CAMPO_[X], :ITC_VALOR_[X])";
            string sqlQuery = @"
                        INSERT INTO T_IMPORTACAO_TABELA_FRETE_LINHA_COLUNA ( ITL_CODIGO, ITC_NOME_CAMPO, ITC_VALOR ) values " + parameros.Replace("[X]", "0");

            for (int i = 1; i < ListaColunas.Count; i++)
                sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            object nome = DBNull.Value;
            if (!string.IsNullOrEmpty(ListaColunas[0].NomeCampo))
                nome = ListaColunas[0].NomeCampo;

            object valor = DBNull.Value;
            if (!string.IsNullOrEmpty(ListaColunas[0].Valor))
                valor = ListaColunas[0].Valor;

            query.SetParameter("ITL_CODIGO_0", Linha.Codigo);
            query.SetParameter("ITC_NOME_CAMPO_0", nome);
            query.SetParameter("ITC_VALOR_0", valor);

            for (int i = 1; i < ListaColunas.Count; i++)
            {
                nome = DBNull.Value;
                if (!string.IsNullOrEmpty(ListaColunas[i].NomeCampo))
                    nome = ListaColunas[i].NomeCampo;

                valor = DBNull.Value;
                if (!string.IsNullOrEmpty(ListaColunas[i].Valor))
                    valor = ListaColunas[i].Valor;

                query.SetParameter("ITL_CODIGO_" + i.ToString(), Linha.Codigo);
                query.SetParameter("ITC_NOME_CAMPO_" + i.ToString(), nome);
                query.SetParameter("ITC_VALOR_" + i.ToString(), valor);
            }

            query.ExecuteUpdate();

        }

    }
}
