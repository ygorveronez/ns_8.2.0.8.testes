using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class ColetaNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ColetaNotaFiscal>
    {
        public ColetaNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ColetaNotaFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaNotaFiscal>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaNotaFiscal> BuscarColetaNotaFiscalPendenteVinculo(bool habilitarFuncionalidadesProjetoGollum)
        {
            string sql = @"select ColetaNotaFiscal.CNF_CODIGO CodigoColetaNotaFiscal, Carga.CAR_CODIGO CodigoCarga, CargaPedido.CPE_CODIGO CodigoCargaPedido,  Container.CTR_CODIGO CodigoContainer
                    from T_COLETA_NOTA_FISCAL ColetaNotaFiscal 
                    JOIN T_CONTAINER Container on Container.CTR_NUMERO = ColetaNotaFiscal.CNF_NUMERO_CONTAINER
                    JOIN T_PEDIDO Pedido on Pedido.CTR_CODIGO = Container.CTR_CODIGO AND Pedido.PED_NUMERO_OS = ColetaNotaFiscal.CNF_NUMERO_OS
                    JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO
                    JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
                    JOIN T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                    JOIN T_CONFIGURACAO_TIPO_OPERACAO_CARGA ConfiguracaoTipoOperacao on ConfiguracaoTipoOperacao.CCG_CODIGO = TipoOperacao.CCG_CODIGO
                    WHERE ColetaNotaFiscal.CNF_COLETA_PROCESSADA = 0 and Carga.CAR_SITUACAO in (1,5) and Carga.CAR_CARGA_FECHADA = 1";

            if (habilitarFuncionalidadesProjetoGollum)
                sql += " AND ConfiguracaoTipoOperacao.CCG_UTILIZA_INTEGRACAO_OK_COLETA = 1";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaNotaFiscal)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaNotaFiscal>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaNotaFiscal> BuscarColetaNotaFiscalPendenteVinculoSemContainer()
        {
            string sql = @"select ColetaNotaFiscal.CNF_CODIGO CodigoColetaNotaFiscal, Carga.CAR_CODIGO CodigoCarga, CargaPedido.CPE_CODIGO CodigoCargaPedido,  Container.CTR_CODIGO CodigoContainer
                from T_COLETA_NOTA_FISCAL ColetaNotaFiscal 
                JOIN T_CONTAINER Container on Container.CTR_NUMERO = ColetaNotaFiscal.CNF_NUMERO_CONTAINER
                JOIN T_PEDIDO Pedido on Pedido.CTR_CODIGO IS NULL AND Pedido.PED_NUMERO_OS = ColetaNotaFiscal.CNF_NUMERO_OS
                JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO
                JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
                WHERE ColetaNotaFiscal.CNF_COLETA_PROCESSADA = 0 and Carga.CAR_SITUACAO in (1,5) and Carga.CAR_CARGA_FECHADA = 1";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaNotaFiscal)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaNotaFiscal>();
        }

        public void ExcluirPorNotaFiscal(int xmlNotaFiscal)
        {
            var querySql = $@"DELETE FROM T_COLETA_NOTA_XML_NOTA_FISCAL WHERE NFX_CODIGO = {xmlNotaFiscal}";

            var query = this.SessionNHiBernate.CreateSQLQuery(querySql);

            query.ExecuteUpdate();
        }
    }
}
