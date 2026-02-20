using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Relatorios.Embarcador.DataSource.Fretes;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteComissaoProduto : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>
    {
        public TabelaFreteComissaoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TabelaFreteComissaoProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #region Métodos Globais

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto BuscarPorTabelaProdutoEmpresa(int tabelaFrete, int produto, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();
            var result = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete && obj.Empresa.Codigo == empresa && obj.ProdutoEmbarcador.Codigo == produto select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        
        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto BuscarPorEmpresaProduto(int codigoEmpresa, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.ProdutoEmbarcador.Codigo == codigoProduto && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto> Consultar(int codigoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoTransportador, int codigoProduto, int codigoContratoFreteTransportador, double codigoPessoa, int codigoGrupoPessoas, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();

            query = query.Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete);

            if (codigoTransportador > 0)
                query = query.Where(obj => obj.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador);

            if (codigoProduto > 0)
                query = query.Where(obj => obj.ProdutoEmbarcador.Codigo == codigoProduto);

            if (codigoPessoa > 0d)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == codigoPessoa);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (codigoContratoFreteTransportador > 0)
                query = query.Where(o => o.ContratoFreteTransportador.Codigo == codigoContratoFreteTransportador);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoTransportador, int codigoProduto, int codigoContratoFreteTransportador, double codigoPessoa, int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();

            query = query.Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete);

            if (codigoTransportador > 0)
                query = query.Where(obj => obj.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador);

            if (codigoProduto > 0)
                query = query.Where(obj => obj.ProdutoEmbarcador.Codigo == codigoProduto);

            if (codigoPessoa > 0d)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == codigoPessoa);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            if (codigoContratoFreteTransportador > 0)
                query = query.Where(o => o.ContratoFreteTransportador.Codigo == codigoContratoFreteTransportador);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Ativo == false);

            return query.Count();
        }

        public bool VerificarSeExiste(int codigo, int codigoTabelaFrete, int codigoContratoFrete, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();

            query = query.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && o.ContratoFreteTransportador.Codigo == codigoContratoFrete && o.ProdutoEmbarcador.Codigo == codigoProduto);

            if (codigo > 0)
                query = query.Where(o => o.Codigo != codigo);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);
            else
                query = query.Where(o => o.GrupoPessoas == null);

            if (cpfCnpjPessoa > 0d)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cpfCnpjPessoa);
            else
                query = query.Where(o => o.Pessoa == null);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto BuscarPorProdutoEPessoa(int codigoTabelaFrete, int codigoProduto, double cpfCnpjPessoa, int codigoTransportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();

            query = query.Where(o => o.Ativo && 
                                     o.ContratoFreteTransportador.Ativo && 
                                     (o.ContratoFreteTransportador.Transportador == null || o.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador) &&
                                     o.TabelaFrete.Codigo == codigoTabelaFrete &&
                                     o.ProdutoEmbarcador.Codigo == codigoProduto && 
                                     o.Pessoa.CPF_CNPJ == cpfCnpjPessoa && 
                                     o.GrupoPessoas == null &&
                                     o.ContratoFreteTransportador.DataInicial<= data.Date &&
                                     o.ContratoFreteTransportador.DataFinal >= data.Date);
            
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto BuscarPorProdutoEGrupoPessoas(int codigoTabelaFrete, int codigoProduto, int codigoGrupoPessoas, int codigoTransportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();

            query = query.Where(o => o.Ativo &&
                                     o.ContratoFreteTransportador.Ativo &&
                                     (o.ContratoFreteTransportador.Transportador == null || o.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador) &&
                                     o.TabelaFrete.Codigo == codigoTabelaFrete &&
                                     o.ProdutoEmbarcador.Codigo == codigoProduto &&
                                     o.GrupoPessoas.Codigo == codigoGrupoPessoas &&
                                     o.Pessoa == null &&
                                     o.ContratoFreteTransportador.DataInicial <= data.Date &&
                                     o.ContratoFreteTransportador.DataFinal >= data.Date);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto BuscarPorProduto(int codigoTabelaFrete, int codigoProduto, int codigoTransportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();

            query = query.Where(o => o.Ativo &&
                                     o.ContratoFreteTransportador.Ativo &&
                                     (o.ContratoFreteTransportador.Transportador == null || o.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador) &&
                                     o.TabelaFrete.Codigo == codigoTabelaFrete &&
                                     o.ProdutoEmbarcador.Codigo == codigoProduto &&
                                     o.GrupoPessoas == null &&
                                     o.Pessoa == null &&
                                     o.ContratoFreteTransportador.DataInicial <= data.Date &&
                                     o.ContratoFreteTransportador.DataFinal >= data.Date);

            return query.FirstOrDefault();
        }

        public int ContarPorContratoFrete(int codigoContratoFreteTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();

            query = query.Where(o => o.ContratoFreteTransportador.Codigo == codigoContratoFreteTransportador);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto> BuscarPorContratoFrete(int codigoContratoFreteTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto>();

            query = query.Where(o => o.ContratoFreteTransportador.Codigo == codigoContratoFreteTransportador);

            return query.ToList();
        }

        public int ContarConsultaRelatorioComissaoProduto(List<PropriedadeAgrupamento> agrupamentos, int codigoTransportador, int codigoContratoFreteTransportador, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioComissaoProduto(true, agrupamentos, codigoTransportador, codigoContratoFreteTransportador, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            return query.UniqueResult<int>();
        }

        public async Task<IList<ComissaoProduto>> ConsultarRelatorioComissaoProduto(List<PropriedadeAgrupamento> agrupamentos, int codigoTransportador, int codigoContratoFreteTransportador, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioComissaoProduto(false, agrupamentos, codigoTransportador, codigoContratoFreteTransportador, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComissaoProduto)));
            return await query.ListAsync<ComissaoProduto>();
        }

        #endregion

        #region Métodos Privados

        private string ObterSelectRelatorioComissaoProduto(bool count, List<PropriedadeAgrupamento> propriedades, int codigoTransportador, int codigoContratoFreteTransportador, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioComissaoProduto(propriedades[i].Propriedade, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioComissaoProduto(ref where, ref joins, codigoTransportador, codigoContratoFreteTransportador, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioComissaoProduto(propAgrupa, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            return (count ? "SELECT DISTINCT(COUNT(0) OVER ())" : "SELECT " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " FROM T_TABELA_FRETE_COMISSAO_PRODUTO tcp INNER JOIN T_CONTRATO_FRETE_TRANSPORTADOR cft ON cft.CFT_CODIGO = tcp.CFT_CODIGO " + joins +
                   " WHERE cft.CFT_ATIVO = 1 AND tcp.TFC_ATIVO = 1 " + where +
                   (groupBy.Length > 0 ? " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " ORDER BY " + orderBy : " ORDER BY 1 ASC ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;");
        }

        private void SetarSelectRelatorioComissaoProduto(string propriedade, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "ContratoFrete":
                    if (!select.Contains(" ContratoFrete"))
                    {
                        select += "cft.CFT_DESCRICAO ContratoFrete, ";
                    }
                    break;
                case "DataInicialContratoFrete":
                    if (!select.Contains("DataInicialContratoFrete"))
                    {
                        select += "cft.CFT_DATA_INICIAL DataInicialContratoFrete, ";
                    }
                    break;
                case "DataFinalContratoFrete":
                    if (!select.Contains("DataFinalContratoFrete"))
                    {
                        select += "cft.CFT_DATA_FINAL DataFinalContratoFrete, ";
                    }
                    break;
                case "Transportador":
                    if (!select.Contains("Transportador"))
                    {
                        select += "emp.EMP_RAZAO Transportador, ";

                        if (!joins.Contains("T_EMPRESA emp"))
                            joins += "INNER JOIN T_EMPRESA emp ON emp.EMP_CODIGO = cft.EMP_CODIGO ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains("Produto"))
                    {
                        select += "pro.GRP_DESCRICAO Produto, ";

                        if (!joins.Contains("T_PRODUTO_EMBARCADOR pro"))
                            joins += "INNER JOIN T_PRODUTO_EMBARCADOR pro ON pro.PRO_CODIGO = tcp.PRO_CODIGO ";
                    }
                    break;
                case "CodigoProduto":
                    if (!select.Contains("CodigoProduto"))
                    {
                        select += "pro.PRO_CODIGO_PRODUTO_EMBARCADOR CodigoProduto, ";

                        if (!joins.Contains("T_PRODUTO_EMBARCADOR pro"))
                            joins += "INNER JOIN T_PRODUTO_EMBARCADOR pro ON pro.PRO_CODIGO = tcp.PRO_CODIGO ";
                    }
                    break;
                case "GrupoPessoas":
                    if (!select.Contains("GrupoPessoas"))
                    {
                        select += "gpe.GRP_DESCRICAO GrupoPessoas, ";

                        if (!joins.Contains("T_GRUPO_PESSOAS gpe"))
                            joins += "LEFT OUTER JOIN T_GRUPO_PESSOAS gpe ON gpe.GRP_CODIGO = tcp.GRP_CODIGO ";
                    }
                    break;
                case "Pessoa":
                    if (!select.Contains("Pessoa"))
                    {
                        select += "cli.CLI_NOME Pessoa, ";

                        if (!joins.Contains("T_CLIENTE cli"))
                            joins += "LEFT OUTER JOIN T_CLIENTE cli ON cli.CLI_CGCCPF = tcp.CLI_CGCCPF ";
                    }
                    break;
                case "PercentualComissao":
                    if (!select.Contains("PercentualComissao"))
                    {
                        select += "tcp.TFC_PERCENTUAL_VALOR_PRODUTO PercentualComissao, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioComissaoProduto(ref string where, ref string joins, int codigoTransportador, int codigoContratoFreteTransportador, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa)
        {
            if (codigoTransportador > 0)
                where += " and cft.EMP_CODIGO = " + codigoTransportador.ToString();

            if (codigoContratoFreteTransportador > 0)
                where += " and tcp.CFT_CODIGO = " + codigoContratoFreteTransportador.ToString();

            if (codigoProduto > 0)
                where += " and tcp.PRO_CODIGO = " + codigoProduto.ToString();

            if (codigoGrupoPessoas > 0)
                where += " and tcp.GRP_CODIGO = " + codigoGrupoPessoas.ToString();

            if (cpfCnpjPessoa > 0d)
                where += " and tcp.CLI_CGCCPF = " + cpfCnpjPessoa.ToString("F0");
        }

        #endregion
    }
}
