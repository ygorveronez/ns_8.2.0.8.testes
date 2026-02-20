using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ImportacaoPedidoLinhaColuna : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna>
    {
        public ImportacaoPedidoLinhaColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> BuscarPorImportacaoPendentesGeracaoPedido(int codigoImportacaoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna>();

            query = query.Where(o => o.Linha.ImportacaoPedido.Codigo == codigoImportacaoPedido && o.Linha.Pedido == null);

            return query.Fetch(o => o.Linha).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> BuscarPorLinha(int codigoLinha)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna>();

            query = query.Where(o => o.Linha.Codigo == codigoLinha);

            return query.Fetch(o => o.Linha).ToList();
        }

        public void InsertSQL(Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha linha, List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna> ListaColunas)
        {
            string parameros = "( :IML_CODIGO_[X], :IMC_NOME_CAMPO_[X], :IMC_VALOR_[X])";
            string sqlQuery = @"INSERT INTO T_IMPORTACAO_PEDIDO_LINHA_COLUNA ( IML_CODIGO, IMC_NOME_CAMPO, IMC_VALOR ) values " + parameros.Replace("[X]", "0"); // SQL-INJECTION-SAFE

            for (int i = 1; i < ListaColunas.Count; i++)
                sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            for (int i = 0; i < ListaColunas.Count; i++)
            {
                string nome = ListaColunas[i].NomeCampo;
                string valor = (string)ListaColunas[i].Valor;

                query.SetParameter("IML_CODIGO_" + i.ToString(), linha.Codigo);
                query.SetParameter("IMC_NOME_CAMPO_" + i.ToString(), nome);
                query.SetParameter("IMC_VALOR_" + i.ToString(), valor ?? string.Empty);
            }

            query.ExecuteUpdate();
        }

    }
}
