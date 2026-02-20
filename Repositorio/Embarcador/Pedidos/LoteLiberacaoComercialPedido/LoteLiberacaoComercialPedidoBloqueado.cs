using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido
{
    public class LoteLiberacaoComercialPedidoBloqueado : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado>
    {
        public LoteLiberacaoComercialPedidoBloqueado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<int> BuscarLoteBloqueadoPedidosPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado>();
            var result = from obj in query where obj.LoteLiberacaoComercialPedido.Codigo == codigo select obj.Pedido.Codigo;
            return result.ToList<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado> ConsultarPedidos(Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedidoBloqueado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarPedidos(filtrosPesquisa, parametrosConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado> ConsultarPedidosPorCodigo(List<int> codigos, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarPedidos(null, parametrosConsulta, false, codigos));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado>();
        }


        public int ContarConsultarPedidos(Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedidoBloqueado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarPedidos(filtrosPesquisa, parametrosConsulta, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public List<int> ObterPedidos(Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedidoBloqueado filtrosPesquisa, bool selecionarTodos, List<int> codigosPedidos)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarPedidos(filtrosPesquisa, null, false, codigosPedidos, selecionarTodos));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado)));

            var dados = consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado>();

            return dados.Select(x => x.DT_RowId).ToList();
        }

        #endregion

        #region Métodos Privados       

        private string QueryConsultarPedidos(Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedidoBloqueado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool somenteContarNumeroRegistros = false, List<int> codigos = null, bool selecionarTodos = false)
        {
            string sql;

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
                sql = @"select 
                        pedido.PED_CODIGO DT_RowId,
                        filial.FIL_DESCRICAO Filial,
                        pedido.PED_NUMERO_PEDIDO_EMBARCADOR Pedido,
                        situacaoComercial.SCP_DESCRICAO SituacaoComercialPedido,
                        cliente.CLI_NOME Destinatario,
                        canalEntrega.CNE_DESCRICAO CanalEntrega,
                        regiao.REG_DESCRICAO Regiao,
                        vendedor.FUN_NOME Vendedor,
                        gerente.FUN_NOME Gerente,
                        supervisor.FUN_NOME Supervisor, 
                        grupoPessoas.GRP_DESCRICAO GrupoPessoa,
						categoria.CTP_DESCRICAO Categoria,
						situacaoEstoquePedido.SEP_DESCRICAO situacaoEstoquePedido,
                        SUBSTRING((SELECT DISTINCT ', ' + produto.GRP_DESCRICAO
                            from T_PEDIDO_PRODUTO pedidoProduto
						    left join T_PRODUTO_EMBARCADOR produto on produto.PRO_CODIGO = pedidoProduto.PRO_CODIGO 
                            where pedidoProduto.PED_CODIGO = pedido.PED_CODIGO   for xml path('')), 3, 1000) Produto";

            sql += @"   from T_PEDIDO pedido
                        left join T_FILIAL filial on pedido.FIL_CODIGO = filial.FIL_CODIGO
                        left join T_CLIENTE cliente on cliente.CLI_CGCCPF = pedido.CLI_CODIGO
                        left join T_CANAL_ENTREGA canalEntrega on canalEntrega.CNE_CODIGO = pedido.CNE_CODIGO
                        left join T_REGIAO regiao on regiao.REG_CODIGO = pedido.REG_CODIGO_DESTINO
                        left join T_FUNCIONARIO vendedor on vendedor.FUN_CODIGO = pedido.FUN_CODIGO_VENDEDOR
                        left join T_FUNCIONARIO gerente on gerente.FUN_CODIGO = pedido.FUN_CODIGO_GERENTE
                        left join T_FUNCIONARIO supervisor on supervisor.FUN_CODIGO = pedido.FUN_CODIGO_SUPERVISOR
						left join T_CATEGORIA_PESSOA categoria on categoria.CTP_CODIGO = cliente.CTP_CODIGO
						left join T_SITUACAO_COMERCIAL_PEDIDO situacaoComercial on situacaoComercial.SCP_CODIGO = pedido.SCP_CODIGO
                        left join T_GRUPO_PESSOAS grupoPessoas on grupoPessoas.GRP_CODIGO = cliente.GRP_CODIGO
                        left join T_PEDIDO_ADICIONAL pedidoAdicional on pedidoAdicional.PED_CODIGO = pedido.PED_CODIGO
                        left join T_SITUACAO_ESTOQUE_PEDIDO situacaoEstoquePedido on situacaoEstoquePedido.SEP_CODIGO = pedidoAdicional.SEP_CODIGO";

            sql += @" where ((situacaoComercial.SCP_CODIGO is not null and situacaoComercial.SCP_BLOQUEIA_PEDIDO = 1) or (situacaoEstoquePedido.SEP_CODIGO is not null and situacaoEstoquePedido.SEP_BLOQUEIA_PEDIDO = 1))";

            if (filtrosPesquisa != null)
            {
                if (filtrosPesquisa.CodigoFilial > 0)
                    sql += $" and filial.FIL_CODIGO = {filtrosPesquisa.CodigoFilial}";

                if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                    sql += $" and cast(pedido.PED_DATA_CRIACAO as date) >= '{filtrosPesquisa.DataInicial.ToString("yyyy-MM-dd")}'";

                if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                    sql += $" and cast(pedido.PED_DATA_CRIACAO as date) <= '{filtrosPesquisa.DataFinal.ToString("yyyy-MM-dd")}'";

                if (filtrosPesquisa.CodigosPedidos?.Count > 0)
                    sql += " and pedido.PED_CODIGO in ('" + string.Join("', '", filtrosPesquisa.CodigosPedidos) + "')";

                if (filtrosPesquisa.CodigosDestinatarios?.Count > 0)
                    sql += " and pedido.CLI_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosDestinatarios) + ")";

                if (filtrosPesquisa.CodigosCanalEntregas?.Count > 0)
                    sql += " and pedido.CNE_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosCanalEntregas) + ")";

                if (filtrosPesquisa.CodigosRegioes?.Count > 0)
                    sql += " and pedido.REG_CODIGO_DESTINO in (" + string.Join(", ", filtrosPesquisa.CodigosRegioes) + ")";

                if (filtrosPesquisa.CodigosVendedores?.Count > 0)
                    sql += " and pedido.FUN_CODIGO_VENDEDOR in (" + string.Join(", ", filtrosPesquisa.CodigosVendedores) + ")";

                if (filtrosPesquisa.CodigosGerentes?.Count > 0)
                    sql += " and pedido.FUN_CODIGO_GERENTE in (" + string.Join(", ", filtrosPesquisa.CodigosGerentes) + ")";

                if (filtrosPesquisa.CodigosSupervisores?.Count > 0)
                    sql += " and pedido.FUN_CODIGO_SUPERVISOR in (" + string.Join(", ", filtrosPesquisa.CodigosSupervisores) + ")";

                if (filtrosPesquisa.CodigosGrupoPessoas?.Count > 0)
                    sql += " and cliente.GRP_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosGrupoPessoas) + ")";

                if (filtrosPesquisa.CodigosCategorias?.Count > 0)
                    sql += " and categoria.CTP_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosCategorias) + ")";

                if (filtrosPesquisa.CodigosSituacaoComercialPedido?.Count > 0)
                    sql += " and situacaoComercial.SCP_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosSituacaoComercialPedido) + ")";
            }



            if (codigos?.Count > 0)
                if (!selecionarTodos)
                    sql += " and pedido.PED_CODIGO in (" + string.Join(", ", codigos) + ")";
                else
                    sql += " and pedido.PED_CODIGO not in (" + string.Join(", ", codigos) + ")";
            else
                sql += $@" and pedido.PED_CODIGO NOT IN(
                                      select PED_CODIGO from T_LOTE_LIBERACAO_COMERCIAL_PEDIDO_BLOQUEADO
                                    )";


            if (parametrosConsulta != null && !somenteContarNumeroRegistros)
            {
                sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        #endregion

    }
}