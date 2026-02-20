using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class LinhaSeparacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>
    {

        public LinhaSeparacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao BuscarPorTodasCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> BuscarPorTodasCodigosIntegracoes(List<string> codigosIntegracoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();

            var result = from obj in query where codigosIntegracoes.Contains(obj.CodigoIntegracao) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> Consultar(string descricao, string codigoIntegracao, int codigoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (codigoFilial > 0)
                result = result.Where(x => x.Filial.Codigo == codigoFilial);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();

        }

        public int ContarConsulta(string descricao, string codigoIntegracao, int codigoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (codigoFilial > 0)
                result = result.Where(x => x.Filial.Codigo == codigoFilial);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao> ValidadoAgrupamentos(int codigoFilial, bool validadoAgrupamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao>();

            var result = from obj in query where obj.ValidadoAgrupamentos == validadoAgrupamento select obj;

            if (codigoFilial > 0)
                result = result.Where(x => x.Filial.Codigo == codigoFilial);

            return result.ToList();
        }

        public IList<(int CodigoCarga, string DescricaoLinhaSeparacao)> BuscarDescricaoPorCodigosCargas(List<int> codigosCargas)
        {
            List<string> codigosCargasString = new List<string>();

            foreach (int codigoCarga in codigosCargas)
                codigosCargasString.Add(codigoCarga.ToString());

            string sql = $@"
                            SELECT CargaPedido.CAR_CODIGO AS CodigoCarga,
                            SUBSTRING((
                              SELECT ', ' +  LinhaSeparacao.CLS_DESCRICAO
                              FROM T_LINHA_SEPARACAO LinhaSeparacao
                              INNER JOIN T_PEDIDO_PRODUTO PedidoProduto ON PedidoProduto.CLS_CODIGO = LinhaSeparacao.CLS_CODIGO
                              WHERE PedidoProduto.PED_CODIGO = CargaPedido.PED_CODIGO
                              group by LinhaSeparacao.CLS_DESCRICAO
                              FOR XML PATH('')), 3, 1000) AS DescricaoLinhaSeparacao
                            FROM T_CARGA_PEDIDO CargaPedido
							WHERE CargaPedido.CAR_CODIGO IN ({string.Join(", ", codigosCargasString)})";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoCarga, string DescricaoLinhaSeparacao)).GetConstructors().FirstOrDefault()));

            return consulta.SetTimeout(600).List<(int CodigoCarga, string DescricaoLinhaSeparacao)>();
        }
    }
}