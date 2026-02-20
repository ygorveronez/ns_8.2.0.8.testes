using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class RecebimentoMercadoria : RepositorioBase<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>
    {
        public RecebimentoMercadoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria BuscarRecebimentoComSaldo(int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();
            var result = from obj in query where obj.Produto.Codigo == codigoProduto && obj.QuantidadeLote > 0 select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarValorUnitario(int codigoProduto, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();
            var result = from obj in query where obj.Produto.Codigo == codigoProduto && codigos.Contains(obj.Codigo) select obj;
            return result.Average(p => (decimal?)p.ValorUnitario) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> Consultar(int codigoRecebimento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();

            var result = from obj in query select obj;

            result = result.Where(o => o.Recebimento.Codigo == codigoRecebimento);

            return result.OrderBy(propOrdena + " " + dirOrdena).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoRecebimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();

            var result = from obj in query select obj;

            result = result.Where(o => o.Recebimento.Codigo == codigoRecebimento);

            return result.Count();
        }

        public bool NumeroSolicitacaoJaRecebido(string codigoBarras)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();
            var result = from obj in query where obj.Descricao == codigoBarras && obj.Recebimento.SituacaoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Finalizada select obj;
            return result.Count() > 0;
        }

        public bool NumeroSolicitacaoPendenteDeConferencia(string codigoBarras)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();
            var result = from obj in query where obj.Descricao == codigoBarras && obj.Recebimento.SituacaoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Finalizada && obj.QuantidadeConferida < obj.QuantidadeFaltante select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> BuscarPorRecebimento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();
            var result = from obj in query where obj.Recebimento.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> BuscarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();
            var result = from obj in query where obj.ChaveNFe == chave && obj.Produto != null && obj.Recebimento.SituacaoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Finalizada select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> BuscarPorXMLNotasFiscais(List<int> codigosNotasFiscais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>()
                .Where(obj => codigosNotasFiscais.Contains(obj.XMLNotaFiscal.Codigo));

            return query
                .Fetch(obj => obj.XMLNotaFiscal)
                .Fetch(obj => obj.Recebimento)
                .ToList();
        }

        public bool ContemRegistroDupliacdo(int codigoRecebimento, string codigoBarras, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();
            var result = from obj in query where obj.Recebimento.Codigo == codigoRecebimento && obj.CodigoBarras == codigoBarras && obj.Descricao == descricao select obj;
            return result.Count() > 0;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.WMS.EstoqueProdutoCarga> ProdutosSemEstoqueDaCarga(int codigoCarga)
        {
            string sql = @"select T.PRO_CODIGO Codigo, T.PRO_DESCRICAO Descricao, T.Quantidade, T.XFP_UNIDADE_MEDIDA Unidade, Est.QtdEstoque 
                from (SELECT Produto.PRO_CODIGO, Produto.PRO_DESCRICAO, SUM(XmlProduto.XFP_QUANTIDADE) Quantidade, XmlProduto.XFP_UNIDADE_MEDIDA
                FROM T_XML_NOTA_FISCAL_PRODUTO XmlProduto
                JOIN T_PEDIDO_XML_NOTA_FISCAL Pedido on Pedido.NFX_CODIGO = XmlProduto.NFX_CODIGO
                JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = XmlProduto.PRO_CODIGO_INTERNO
                JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = Pedido.CPE_CODIGO
                WHERE CargaPedido.CAR_CODIGO = " + codigoCarga + @"
                GROUP BY Produto.PRO_CODIGO, Produto.PRO_DESCRICAO, XmlProduto.XFP_UNIDADE_MEDIDA) as T
                JOIN (SELECT Estoque.PRO_CODIGO Codigo, SUM(Estoque.PRE_QUANTIDADE) QtdEstoque FROM T_PRODUTO_ESTOQUE Estoque group by Estoque.PRO_CODIGO) Est on Est.Codigo = T.PRO_CODIGO
                where T.Quantidade > Est.QtdEstoque";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(sql);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.WMS.EstoqueProdutoCarga)));

            return nhQuery.List<Dominio.ObjetosDeValor.Embarcador.WMS.EstoqueProdutoCarga>();
        }
        
        public IList<Dominio.Relatorios.Embarcador.DataSource.WMS.Armazenagem> RelatorioArmazenagem(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaArmazenagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var nhQuery = new ConsultaArmazenagem().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.WMS.Armazenagem)));

            return nhQuery.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.WMS.Armazenagem>();
        }

        public int ContarRelatorioArmazenagem(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaArmazenagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var nhQuery = new ConsultaArmazenagem().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return nhQuery.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.WMS.ConferenciaVolume> RelatorioConferenciaVolume(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioConferenciaVolume filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = new ConsultaConferenciaVolume().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.WMS.ConferenciaVolume)));

            return result.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.WMS.ConferenciaVolume>();
        }

        public int ContarRelatorioConferenciaVolume(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioConferenciaVolume filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var result = new ConsultaConferenciaVolume().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return result.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.WMS.ExpedicaoVolume> RelatorioExpedicaoVolume(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = new ConsultaExpedicaoVolume().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.WMS.ExpedicaoVolume)));

            return result.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.WMS.ExpedicaoVolume>();
        }

        public int ContarRelatorioExpedicaoVolume(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioExpedicaoVolume filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var result = new ConsultaExpedicaoVolume().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return result.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.WMS.SaldoArmazenamento> RelatorioSaldoArmazenamento(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = new ConsultaSaldoArmazenamento().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.WMS.SaldoArmazenamento)));

            return result.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.WMS.SaldoArmazenamento>();
        }

        public int ContarRelatorioSaldoArmazenamento(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var result = new ConsultaSaldoArmazenamento().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return result.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.WMS.RastreamentoVolumes> ConsultarRelatorioRastreabilidadeVolumes(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)//string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            var result = new ConsultaRastreabilidadeVolumes().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.WMS.RastreamentoVolumes)));

            return result.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.WMS.RastreamentoVolumes>();
        }

        public int ContarConsultaRelatorioRastreabilidadeVolumes(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioRastreabilidadeVolumes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var result = new ConsultaRastreabilidadeVolumes().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return result.SetTimeout(600).UniqueResult<int>();
        }
    }
}