using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    public class PagamentoProvedorCarga : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>
    {
        #region Construtores

        public PagamentoProvedorCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga BuscarPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.FirstOrDefault();
        }

        public bool VerificarSeExisteCargaEmPagamentoProvedor(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && !obj.PagamentoProvedor.NovoFluxoIniciado && (obj.PagamentoProvedor.SituacaoLiberacaoPagamentoProvedor == SituacaoLiberacaoPagamentoProvedor.Rejeitada || obj.PagamentoProvedor.SituacaoLiberacaoPagamentoProvedor == SituacaoLiberacaoPagamentoProvedor.Finalizada || obj.PagamentoProvedor.SituacaoLiberacaoPagamentoProvedor == SituacaoLiberacaoPagamentoProvedor.Aberto) select obj;

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> BuscarPagamentosRejeitadosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.PagamentoProvedor.SituacaoLiberacaoPagamentoProvedor == SituacaoLiberacaoPagamentoProvedor.Rejeitada select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> BuscarProvedoresPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasProvedor(int codigoPagamentoProvedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.PagamentoProvedor.Codigo == codigoPagamentoProvedor select obj.Carga;
            return result.ToList();
        }

        public List<string> BuscarNumerosCargasPorPagamentoProvedor(int codigoPagamentoProvedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.PagamentoProvedor.Codigo == codigoPagamentoProvedor select obj.Carga;
            return result.Select(o => o.CodigoCargaEmbarcador).ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga BuscarPorCodigoPagamentoProvedor(int codigoPagamentoProvedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.PagamentoProvedor.Codigo == codigoPagamentoProvedor select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> BuscarPorCodigosPagamentoProvedor(int codigoPagamentoProvedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.PagamentoProvedor.Codigo == codigoPagamentoProvedor select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> BuscarCodigoPagamentoProvedorCargas(int codigoPagamentoProvedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
            var result = from obj in query where obj.PagamentoProvedor.Codigo == codigoPagamentoProvedor select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoProvedor filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaPagamentoProvedorCarga = Consultar(filtrosPesquisa);

            return ObterLista(consultaPagamentoProvedorCarga, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoProvedor filtrosPesquisa)
        {
            var consultaPagamentoProvedorCarga = Consultar(filtrosPesquisa);

            return consultaPagamentoProvedorCarga.Count();
        }

        public int ContarConsultaCargasPedidos(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDetalhesCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ConsultarCargasPedidosPagamentoProvedor(filtrosPesquisa, parametroConsulta, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.LiberacaoPagamentoProvedorCargaPedido> BuscarCargasPedidosPagamentoProvedor(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDetalhesCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ConsultarCargasPedidosPagamentoProvedor(filtrosPesquisa, parametroConsulta, false));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.LiberacaoPagamentoProvedorCargaPedido)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.LiberacaoPagamentoProvedorCargaPedido>();
        }

        #endregion

        #region Métodos Privados

        private string ConsultarCargasPedidosPagamentoProvedor(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDetalhesCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, bool somenteContarNumeroRegistros)
        {
            string sql;

            if (somenteContarNumeroRegistros)
            {
                sql = "select distinct(count(0) over ()) ";
            }
            else
            {
                sql = $@"SELECT CargaPedido.CPE_CODIGO Codigo,
                    Provedor.CLI_NOME Provedor,
                    ConfiguracaoTipoOperacao.CCG_DOCUMENTO_PROVEDOR DocumentoProvedor, 
                    EmpresaCarga.EMP_RAZAO + ' - ' + EmpresaCarga.EMP_CNPJ EmpresaTomador,
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga,
                    Carga.CAR_CODIGO CodigoCarga,
                    Carga.CAR_VALOR_TOTAL_PROVEDOR ValorTotalPrestacao,
                    Localidade.LOC_DESCRICAO + ' - ' + Localidade.UF_SIGLA Localidade";
            }

            sql += @" FROM T_CARGA_PEDIDO CargaPedido
                JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO
                JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                LEFT JOIN T_TIPO_OPERACAO TipoOperacao on Carga.TOP_CODIGO = TipoOperacao.TOP_CODIGO
                LEFT JOIN T_CONFIGURACAO_TIPO_OPERACAO_CARGA ConfiguracaoTipoOperacao on ConfiguracaoTipoOperacao.CCG_CODIGO = TipoOperacao.CCG_CODIGO
                LEFT JOIN T_CLIENTE Provedor on Pedido.CLI_CODIGO_PROVEDOR_OS = Provedor.CLI_CGCCPF
                LEFT JOIN T_EMPRESA EmpresaCarga on EmpresaCarga.EMP_CODIGO = Carga.EMP_CODIGO
                LEFT JOIN T_PEDIDO_ADICIONAL pedidoAdicional on pedidoAdicional.PED_CODIGO = CargaPedido.PED_CODIGO
                LEFT JOIN T_LOCALIDADES Localidade ON (
                    CASE 
                        WHEN pedidoAdicional.PAD_INDICATIVO_COLETA_ENTREGA = 2 THEN Pedido.LOC_CODIGO_DESTINO 
                        WHEN pedidoAdicional.PAD_INDICATIVO_COLETA_ENTREGA = 1 THEN Pedido.LOC_CODIGO_ORIGEM 
                        ELSE NULL 
                    END
                ) = Localidade.LOC_CODIGO";

            sql += ObterWhereConsultaDirecionamentoPorOperador(filtrosPesquisa);

            if (parametroConsulta != null && !somenteContarNumeroRegistros)
            {
                sql += $" order by {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}";

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametroConsulta.InicioRegistros} rows fetch next {parametroConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        private string ObterWhereConsultaDirecionamentoPorOperador(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaDetalhesCarga filtrosPesquisa)
        {
            StringBuilder where = new StringBuilder(@" WHERE 1 = 1 ");

            if (filtrosPesquisa.Codigo.Count > 0)
            {
                where.Append($@" AND Carga.CAR_CODIGO IN (
                                SELECT DISTINCT pagamentoProvedorCarga.CAR_CODIGO 
                                FROM T_PAGAMENTO_PROVEDOR_CARGA pagamentoProvedorCarga 
                                WHERE 
                                    (pagamentoProvedorCarga.PRC_CODIGO in ({string.Join(", ", filtrosPesquisa.Codigo)})))");
            }
            else
            {
                where.Append($@" AND Carga.CAR_CATEGORIA_OS = 2 AND Carga.CAR_INDIC_LIBERACAO_OK = 1 AND Carga.CAR_LIBERAR_PAGAMENTO = 1 AND Carga.CAR_SITUACAO <> 13 AND Carga.CAR_SITUACAO <> 18 AND
                                         NOT EXISTS (
                                         SELECT 1
                                         FROM T_PAGAMENTO_PROVEDOR_CARGA PagamentoProvedorCarga
                                         JOIN T_PAGAMENTO_PROVEDOR PagamentoProvedor ON PagamentoProvedor.PRO_CODIGO = PagamentoProvedorCarga.PRO_CODIGO
                                         WHERE PagamentoProvedorCarga.CAR_CODIGO = Carga.CAR_CODIGO
                                         AND PagamentoProvedor.PRO_SITUACAO_PROVEDOR != 2 AND PagamentoProvedor.PRO_SITUACAO_PROVEDOR != 4)");
            }

            if (filtrosPesquisa.Carga > 0)
                where.Append($" and Carga.CAR_CODIGO = {filtrosPesquisa.Carga}");

            if (filtrosPesquisa.EmpresaTomador > 0)
                where.Append($" and EmpresaCarga.EMP_CODIGO = {filtrosPesquisa.EmpresaTomador}");

            if (filtrosPesquisa.Localidade.Count > 0)
            {
                where.Append($@" AND CargaPedido.CAR_CODIGO IN (
                                SELECT DISTINCT cp.CAR_CODIGO 
                                FROM T_CARGA_PEDIDO cp 
                                LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = cp.PED_CODIGO
                                LEFT JOIN T_PEDIDO_ADICIONAL pedidoAdicional ON pedidoAdicional.PED_CODIGO = cp.PED_CODIGO
                                LEFT JOIN T_CLIENTE Provedor ON Pedido.CLI_CODIGO_PROVEDOR_OS = Provedor.CLI_CGCCPF
                                LEFT JOIN T_LOCALIDADES Destino ON Destino.LOC_CODIGO = Provedor.LOC_CODIGO
                                WHERE 
                                    (pedidoAdicional.PAD_INDICATIVO_COLETA_ENTREGA = 2 AND Pedido.LOC_CODIGO_DESTINO in ({string.Join(", ", filtrosPesquisa.Localidade)}))
                                    OR 
                                    (pedidoAdicional.PAD_INDICATIVO_COLETA_ENTREGA = 1 AND Pedido.LOC_CODIGO_ORIGEM in ({string.Join(", ", filtrosPesquisa.Localidade)}))
                                )");
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && filtrosPesquisa.CodigoProvedor > 0)
                where.Append($@" AND Provedor.CLI_CGCCPF = {filtrosPesquisa.CodigoProvedor}");

            if (filtrosPesquisa.Provedor > 0)
                where.Append($" and Provedor.CLI_CGCCPF = {filtrosPesquisa.Provedor}");

            if (filtrosPesquisa.DocumentoProvedor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoProvedor.Nenhum)
                where.Append($" and ConfiguracaoTipoOperacao.CCG_DOCUMENTO_PROVEDOR = {(int)filtrosPesquisa.DocumentoProvedor}");

            return where.ToString();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoProvedor filtrosPesquisa)
        {
            var consultaPagamentoProvedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>();

            if (filtrosPesquisa.CodigoProvedor > 0)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var consultaPagamentoProvedorCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();

                List<int> cargas = query.Where(cp => cp.Pedido.ProvedorOS.CPF_CNPJ == filtrosPesquisa.CodigoProvedor).Select(o => o.Carga.Codigo).Distinct().ToList();
                var codigoProvedores = consultaPagamentoProvedorCarga.Where(obj => cargas.Contains(obj.Carga.Codigo)).Select(o => o.PagamentoProvedor.Codigo).ToList();

                consultaPagamentoProvedor = consultaPagamentoProvedor.Where(obj => codigoProvedores.Contains(obj.Codigo));
            }

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                var consultaPagamentoProvedorCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga>();
                List<int> codigoProvedores = consultaPagamentoProvedorCarga.Where(obj => obj.Carga.Codigo == filtrosPesquisa.CodigoCarga).Select(obj => obj.PagamentoProvedor.Codigo).ToList();

                consultaPagamentoProvedor = consultaPagamentoProvedor.Where(obj => codigoProvedores.Contains(obj.Codigo));
            }

            if (filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor.Count > 0 && !filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor.Contains(SituacaoLiberacaoPagamentoProvedor.Todos))
                consultaPagamentoProvedor = consultaPagamentoProvedor.Where(o => filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor.Contains(o.SituacaoLiberacaoPagamentoProvedor));

            if (filtrosPesquisa.EtapaLiberacaoPagamentoProvedor.Count > 0 && !filtrosPesquisa.EtapaLiberacaoPagamentoProvedor.Contains(EtapaLiberacaoPagamentoProvedor.Todos))
                consultaPagamentoProvedor = consultaPagamentoProvedor.Where(o => filtrosPesquisa.EtapaLiberacaoPagamentoProvedor.Contains(o.EtapaLiberacaoPagamentoProvedor));

            consultaPagamentoProvedor = consultaPagamentoProvedor.Where(p => !p.NovoFluxoIniciado);

            return consultaPagamentoProvedor;
        }

        #endregion
    }
}
