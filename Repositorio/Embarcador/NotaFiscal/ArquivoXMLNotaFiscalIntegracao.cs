using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ArquivoXMLNotaFiscalIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao>
    {
        public ArquivoXMLNotaFiscalIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao BuscarPendentePorCodigo(int codigoXmlArquivoXMLNotaFiscalIntegracao)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao>()
            .Where(obj => obj.Codigo == codigoXmlArquivoXMLNotaFiscalIntegracao && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Pendente)
            .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao BuscarProcessadoPorChave(string chave)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao>()
            .Where(obj => obj.Chave == chave && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Sucesso)
            .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao> BuscarPendentesLiberacao(int limiteTentativas, int limite, double segundosACadaTentativa)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao>()
            .Where(obj => obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Sucesso && obj.TentativasLiberacao <= limiteTentativas && obj.DataTentativa <= DateTime.Now.AddSeconds(-segundosACadaTentativa))
            .Take(limite).ToList();
        }

        public List<int> BuscarCodigosPendentes(int limiteTentativas, int limite, double minutosACadaTentativa)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao>()
            .Where(obj => obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Pendente ||
                (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Falha && obj.Tentativas < limiteTentativas && obj.DataTentativa <= DateTime.Now.AddMinutes(-minutosACadaTentativa)))
            .Take(limite).Select(x => x.Codigo).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsulta(filtrosPesquisa, null, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsulta(filtrosPesquisa, parametrosConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe)));

            return consulta.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe>();
        }

        private string ObterConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null, bool somenteContar = false)
        {
            string sql;

            sql = (somenteContar ? "select count(0) " : ObterSqlSelect()) + ObterSqlFrom() + ObterSqlWhere(filtrosPesquisa);

            if (parametrosConsulta != null)
            {
                sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")} ";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        private string ObterSqlSelect()
        {
            string select =
            @"select 
                Requisicao.AXI_CODIGO Codigo,
                Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,
                Requisicao.AXI_MENSAGEM Retorno,
                Requisicao.AXI_DATA_RECEBIMENTO DataIntegracao,
                Requisicao.AXI_SITUACAO SituacaoProcessamentoRegistro
";

            return select;
        }

        private string ObterSqlFrom()
        {
            string select =
            @"from T_ARQUIVO_XML_NOTA_FISCAL_IMPORTACAO Requisicao
                left join T_CARGA Carga on Carga.CAR_CODIGO = Requisicao.CAR_CODIGO";

            return select;
        }

        private string ObterSqlWhere(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd HH:mm";
            string where = " where 1=1 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where += $"and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                where += $@"and Requisicao.AXI_CHAVE = '{filtrosPesquisa.Chave}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                where += $@"and exists (select top 1 1 from T_XML_NOTA_FISCAL _xmlNotaFiscal
			                        left join T_PEDIDO_XML_NOTA_FISCAL _pedidoNota on _pedidoNota.NFX_CODIGO = _xmlNotaFiscal.NFX_CODIGO
			                        left join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.CPE_CODIGO = _pedidoNota.CPE_CODIGO
			                        left join T_PEDIDO _pedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
				                    where _xmlNotaFiscal.NF_CHAVE = Requisicao.AXI_CHAVE and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}') ";

            if (filtrosPesquisa.DataFinal.HasValue)
                where += $"and Requisicao.AXI_DATA_RECEBIMENTO <= '{filtrosPesquisa.DataFinal.Value.ToString(pattern)}' ";

            if (filtrosPesquisa.DataInicial.HasValue)
                where += $"and Requisicao.AXI_DATA_RECEBIMENTO >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' ";

            if (filtrosPesquisa.SituacaoProcessamentoRegistro.Count > 0)
                where += $"and Requisicao.AXI_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacaoProcessamentoRegistro.Select(x => (int)x).ToList())}) ";

            return where;
        }
    }
}
