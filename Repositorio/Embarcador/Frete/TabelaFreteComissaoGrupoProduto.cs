using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Relatorios.Embarcador.DataSource.Fretes;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteComissaoGrupoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>
    {
        public TabelaFreteComissaoGrupoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TabelaFreteComissaoGrupoProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto> Consultar(int codigoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoTransportador, int codigoGrupoProduto, int codigoContratoFreteTransportador, double codigoPessoa, int codigoGrupoPessoas, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete);

            if (codigoTransportador > 0)
                query = query.Where(obj => obj.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador);

            if (codigoGrupoProduto > 0)
                query = query.Where(obj => obj.GrupoProduto.Codigo == codigoGrupoProduto);

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

        public int ContarConsulta(int codigoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoTransportador, int codigoGrupoProduto, int codigoContratoFreteTransportador, double codigoPessoa, int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(obj => obj.TabelaFrete.Codigo == codigoTabelaFrete);

            if (codigoTransportador > 0)
                query = query.Where(obj => obj.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador);

            if (codigoGrupoProduto > 0)
                query = query.Where(obj => obj.GrupoProduto.Codigo == codigoGrupoProduto);

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

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto Buscar(int codigoTabelaFrete, int codigoContratoFrete, int codigoGrupoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && o.ContratoFreteTransportador.Codigo == codigoContratoFrete && o.GrupoProduto.Codigo == codigoGrupoProduto);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);
            else
                query = query.Where(o => o.GrupoPessoas == null);

            if (cpfCnpjPessoa > 0d)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cpfCnpjPessoa);
            else
                query = query.Where(o => o.Pessoa == null);

            return query.FirstOrDefault();
        }

        public bool VerificarSeExiste(int codigo, int codigoTabelaFrete, int codigoContratoFrete, int codigoGrupoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && o.ContratoFreteTransportador.Codigo == codigoContratoFrete && o.GrupoProduto.Codigo == codigoGrupoProduto);

            if (codigo > 0)
                query = query.Where(o => o.Codigo != codigo);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);
            else if (cpfCnpjPessoa > 0d)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cpfCnpjPessoa);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto BuscarPorGrupoProdutoEPessoa(int codigoTabelaFrete, int codigoGrupoProduto, double cpfCnpjPessoa, int codigoTransportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(o => o.Ativo &&
                                     o.ContratoFreteTransportador.Ativo &&
                                     (o.ContratoFreteTransportador.Transportador == null || o.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador) &&
                                     o.TabelaFrete.Codigo == codigoTabelaFrete &&
                                     o.GrupoProduto.Codigo == codigoGrupoProduto &&
                                     o.Pessoa.CPF_CNPJ == cpfCnpjPessoa &&
                                     o.GrupoPessoas == null &&
                                     o.ContratoFreteTransportador.DataInicial <= data.Date &&
                                     o.ContratoFreteTransportador.DataFinal >= data.Date);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto BuscarPorGrupoProdutoEGrupoPessoas(int codigoTabelaFrete, int codigoGrupoProduto, int codigoGrupoPessoas, int codigoTransportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(o => o.Ativo &&
                                     o.ContratoFreteTransportador.Ativo &&
                                     (o.ContratoFreteTransportador.Transportador == null || o.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador) &&
                                     o.TabelaFrete.Codigo == codigoTabelaFrete &&
                                     o.GrupoProduto.Codigo == codigoGrupoProduto &&
                                     o.GrupoPessoas.Codigo == codigoGrupoPessoas &&
                                     o.Pessoa == null &&
                                     o.ContratoFreteTransportador.DataInicial <= data.Date &&
                                     o.ContratoFreteTransportador.DataFinal >= data.Date);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto BuscarPorGrupoProduto(int codigoTabelaFrete, int codigoGrupoProduto, int codigoTransportador, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(o => o.Ativo &&
                                     o.ContratoFreteTransportador.Ativo &&
                                     (o.ContratoFreteTransportador.Transportador == null || o.ContratoFreteTransportador.Transportador.Codigo == codigoTransportador) &&
                                     o.TabelaFrete.Codigo == codigoTabelaFrete &&
                                     o.GrupoProduto.Codigo == codigoGrupoProduto &&
                                     o.GrupoPessoas == null &&
                                     o.Pessoa == null &&
                                     o.ContratoFreteTransportador.DataInicial <= data.Date &&
                                     o.ContratoFreteTransportador.DataFinal >= data.Date);

            return query.FirstOrDefault();
        }

        public int ContarPorContratoFrete(int codigoContratoFreteTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(o => o.ContratoFreteTransportador.Codigo == codigoContratoFreteTransportador);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto> BuscarPorContratoFrete(int codigoContratoFreteTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto>();

            query = query.Where(o => o.ContratoFreteTransportador.Codigo == codigoContratoFreteTransportador);

            return query.ToList();
        }

        public int ContarConsultaRelatorioComissaoGrupoProduto(List<PropriedadeAgrupamento> agrupamentos, int codigoTransportador, int codigoContratoFreteTransportador, int codigoGrupoProdutos, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioComissaoGrupoProduto(true, agrupamentos, codigoTransportador, codigoContratoFreteTransportador, codigoGrupoProdutos, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            return query.UniqueResult<int>();
        }

        public IList<ComissaoGrupoProduto> ConsultarRelatorioComissaoGrupoProduto(List<PropriedadeAgrupamento> agrupamentos, int codigoTransportador, int codigoContratoFreteTransportador, int codigoGrupoProdutos, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioComissaoGrupoProduto(false, agrupamentos, codigoTransportador, codigoContratoFreteTransportador, codigoGrupoProdutos, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComissaoGrupoProduto)));

            return query.List<ComissaoGrupoProduto>();
        }

        public async Task<IList<ComissaoGrupoProduto>> ConsultarRelatorioComissaoGrupoProdutoAsync(List<PropriedadeAgrupamento> agrupamentos, int codigoTransportador, int codigoContratoFreteTransportador, int codigoGrupoProdutos, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectRelatorioComissaoGrupoProduto(false, agrupamentos, codigoTransportador, codigoContratoFreteTransportador, codigoGrupoProdutos, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(ComissaoGrupoProduto)));

            return await query.ListAsync<ComissaoGrupoProduto>();
        }

        #endregion

        #region Métodos Privados

        private string ObterSelectRelatorioComissaoGrupoProduto(bool count, List<PropriedadeAgrupamento> propriedades, int codigoTransportador, int codigoContratoFreteTransportador, int codigoGrupoProdutos, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioComissaoGrupoProduto(propriedades[i].Propriedade, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioComissaoGrupoProduto(ref where, ref joins, codigoTransportador, codigoContratoFreteTransportador, codigoGrupoProdutos, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioComissaoGrupoProduto(propAgrupa, ref select, ref groupBy, ref joins, count);

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
                   " FROM T_TABELA_FRETE_COMISSAO_GRUPO_PRODUTO tgp INNER JOIN T_CONTRATO_FRETE_TRANSPORTADOR cft ON cft.CFT_CODIGO = tgp.CFT_CODIGO " + joins +
                   " WHERE cft.CFT_ATIVO = 1 AND tgp.TCG_ATIVO = 1 " + where +
                   (groupBy.Length > 0 ? " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " ORDER BY " + orderBy : " ORDER BY 1 ASC ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;");
        }

        private void SetarSelectRelatorioComissaoGrupoProduto(string propriedade, ref string select, ref string groupBy, ref string joins, bool count)
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
                case "GrupoProdutos":
                    if (!select.Contains("GrupoProdutos"))
                    {
                        select += "gpr.GRP_DESCRICAO GrupoProdutos, ";

                        if (!joins.Contains("T_GRUPO_PRODUTO gpr"))
                            joins += "INNER JOIN T_GRUPO_PRODUTO gpr ON gpr.GPR_CODIGO = tgp.GPR_CODIGO ";
                    }
                    break;
                case "GrupoPessoas":
                    if (!select.Contains("GrupoPessoas"))
                    {
                        select += "gpe.GRP_DESCRICAO GrupoPessoas, ";

                        if (!joins.Contains("T_GRUPO_PESSOAS gpe"))
                            joins += "LEFT OUTER JOIN T_GRUPO_PESSOAS gpe ON gpe.GRP_CODIGO = tgp.GRP_CODIGO ";
                    }
                    break;
                case "Pessoa":
                    if (!select.Contains("Pessoa"))
                    {
                        select += "cli.CLI_NOME Pessoa, ";

                        if (!joins.Contains("T_CLIENTE cli"))
                            joins += "LEFT OUTER JOIN T_CLIENTE cli ON cli.CLI_CGCCPF = tgp.CLI_CGCCPF ";
                    }
                    break;
                case "PercentualComissao":
                    if (!select.Contains("PercentualComissao"))
                    {
                        select += "tgp.TCG_PERCENTUAL_VALOR_PRODUTO PercentualComissao, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioComissaoGrupoProduto(ref string where, ref string joins, int codigoTransportador, int codigoContratoFreteTransportador, int codigoGrupoProdutos, int codigoProduto, int codigoGrupoPessoas, double cpfCnpjPessoa)
        {
            if (codigoTransportador > 0)
                where += " and cft.EMP_CODIGO = " + codigoTransportador.ToString();

            if (codigoContratoFreteTransportador > 0)
                where += " and tgp.CFT_CODIGO = " + codigoContratoFreteTransportador.ToString();

            if (codigoGrupoProdutos > 0)
                where += " and tgp.GPR_CODIGO = " + codigoGrupoProdutos.ToString();

            if (codigoGrupoPessoas > 0)
                where += " and tgp.GRP_CODIGO = " + codigoGrupoPessoas.ToString();

            if (cpfCnpjPessoa > 0d)
                where += " and tgp.CLI_CGCCPF = " + cpfCnpjPessoa.ToString("F0");

            if (codigoProduto > 0)
                where += " and tgp.GPR_CODIGO in (select GRP_CODIGO from T_PRODUTO_EMBARCADOR where PRO_CODIGO = " + codigoProduto.ToString() + ")"; // SQL-INJECTION-SAFE
        }

        #endregion
    }
}
