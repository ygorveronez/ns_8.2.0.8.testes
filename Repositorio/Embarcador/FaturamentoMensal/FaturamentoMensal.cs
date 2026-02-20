using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.FaturamentoMensal
{
    public class FaturamentoMensal : RepositorioBase<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal>
    {
        #region Construtores

        public FaturamentoMensal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal> BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal>();
            var result = from obj in query where obj.StatusFaturamentoMensal == status select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal> Consultar(double cnpjPessoa, int codigoGrupoFaturamento, int codigoServico, DateTime dataVencimento, StatusFaturamentoMensal status, int codigoEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaFaturamentoMensal = Consultar(cnpjPessoa, codigoGrupoFaturamento, codigoServico, dataVencimento, status, codigoEmpresa);

            return ObterLista(consultaFaturamentoMensal, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(double cnpjPessoa, int codigoGrupoFaturamento, int codigoServico, DateTime dataVencimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal status, int codigoEmpresa)
        {
            var consultaFaturamentoMensal = Consultar(cnpjPessoa, codigoGrupoFaturamento, codigoServico, dataVencimento, status, codigoEmpresa);

            return consultaFaturamentoMensal.Count();
        }

        public List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> ConsultarPorCodigoFaturamento(int codigo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.FaturamentoMensal.Codigo == codigo select obj;

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();
        }

        public List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> ConsultarPorCodigoFaturamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.FaturamentoMensal.Codigo == codigo select obj;

            return result.ToList();
        }

        public int ContarConsultarPorCodigoFaturamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.FaturamentoMensal.Codigo == codigo select obj;

            return result.Count();
        }

        public int ContarBoletosGeradosPorCodigoFaturamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();
            var result = from obj in query where obj.FaturamentoMensal.Codigo == codigo && obj.Titulo.BoletoConfiguracao != null && obj.Titulo.NossoNumero != null && obj.Titulo.NossoNumero != "" select obj;

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.CobrancaMensal> RelatorioCobrancaMensal(int codigoEmpresa, int codigoGrupoFaturamento, int codigoServico, int dia, int codigoConfiguracaoBoleto, double cnpjPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal status, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"   SELECT FCS_DATA_VENCIMENTO DataVencimento,
                            FME_DATA_FINALIZACAO DataFinalizacao,
                            FME_DATA_PROCESSAMENTO DataFatura,
                            FMC_DIA_FATURA DiaFatura,
                            FCS_VALOR_FATURA ValorFatura,
                            FMCS.TIT_CODIGO CodigoTitulo,
                            T.TIT_NOSSO_NUMERO Boleto,
                            N.NFI_NUMERO Nota,
                            NFS.CON_NUM NotaServico,
                            C.CLI_NOME Pessoa,
                            FMG.FMG_DESCRICAO GrupoFaturamento,
                            FCS_OBSERVACAO_FATURA Observacao,

                            CASE
	                            WHEN FME_STATUS = 1 THEN 'Iniciada'
	                            WHEN FME_STATUS = 2 THEN 'Gerado Documentos'
	                            WHEN FME_STATUS = 3 THEN 'Documentos Autorizados'
	                            WHEN FME_STATUS = 4 THEN 'Gerado Boletos'
	                            WHEN FME_STATUS = 5 THEN 'Finalizado'
	                            WHEN FME_STATUS = 6 THEN 'Cancelado'
	                            WHEN FME_STATUS = 7 THEN 'Aguardando Envio dos E-mails'
	                            WHEN FME_STATUS = 8 THEN 'Aguardando Autorização dos Documentos'
	                            WHEN FME_STATUS = 9 THEN 'Enviando E-mails'
	                            WHEN FME_STATUS = 10 THEN 'Autorizando Documentos'
	                            ELSE ''
                            END DescricaoStatus

                            FROM T_FATURAMENTO_MENSAL_CLIENTE_SERVICO FMCS
                            JOIN T_TITULO T ON T.TIT_CODIGO = FMCS.TIT_CODIGO
                            LEFT OUTER JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = FMCS.NFI_CODIGO
                            LEFT OUTER JOIN T_CTE NFS ON NFS.CON_CODIGO = FMCS.CON_CODIGO
                            JOIN T_FATURAMENTO_MENSAL FM ON FM.FME_CODIGO = FMCS.FME_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = FM.EMP_CODIGO
                            JOIN T_FATURAMENTO_MENSAL_CLIENTE FMC ON FMC.FMC_CODIGO = FMCS.FMC_CODIGO
                            JOIN T_CLIENTE C ON C.CLI_CGCCPF = FMC.CLI_CGCCPF
                            JOIN T_FATURAMENTO_MENSAL_GRUPO FMG ON FMG.FMG_CODIGO = FMC.FMG_CODIGO 
                            WHERE 1 = 1 ";

            if (codigoEmpresa > 0)
                query += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoGrupoFaturamento > 0)
                query += " AND FMG.FMG_CODIGO = " + codigoGrupoFaturamento.ToString();

            if (codigoServico > 0)
                query += " AND FMC.SER_CODIGO = " + codigoServico.ToString();

            if (dia > 0)
                query += " AND FMC.FMC_DIA_FATURA = " + dia.ToString();

            if (codigoConfiguracaoBoleto > 0)
                query += " AND FMC.BCF_CODIGO = " + codigoConfiguracaoBoleto.ToString();

            if (cnpjPessoa > 0)
                query += " AND FMC.CLI_CGCCPF = " + cnpjPessoa.ToString();

            if ((int)status > 0)
                query += " AND FME_STATUS = " + (int)status;

            if (dataEmissaoInicial != DateTime.MinValue)
            {
                query += " AND FME_DATA_PROCESSAMENTO >= '" + dataEmissaoInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataEmissaoFinal != DateTime.MinValue)
            {
                query += " AND FME_DATA_PROCESSAMENTO <= '" + dataEmissaoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if (dataVencimentoInicial != DateTime.MinValue)
            {
                query += " AND FCS_DATA_VENCIMENTO >= '" + dataVencimentoInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataVencimentoFinal != DateTime.MinValue)
            {
                query += " AND FCS_DATA_VENCIMENTO <= '" + dataVencimentoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.CobrancaMensal)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.CobrancaMensal>();
        }

        public int ContarRelatorioCobrancaMensal(int codigoEmpresa, int codigoGrupoFaturamento, int codigoServico, int dia, int codigoConfiguracaoBoleto, double cnpjPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal status, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataVencimentoInicial, DateTime dataVencimentoFinal)
        {
            string query = @"   SELECT COUNT(0) as CONTADOR
                                FROM T_FATURAMENTO_MENSAL_CLIENTE_SERVICO FMCS
                                JOIN T_TITULO T ON T.TIT_CODIGO = FMCS.TIT_CODIGO
                                LEFT OUTER JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = FMCS.NFI_CODIGO
                                LEFT OUTER JOIN T_CTE NFS ON NFS.CON_CODIGO = FMCS.CON_CODIGO
                                JOIN T_FATURAMENTO_MENSAL FM ON FM.FME_CODIGO = FMCS.FME_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = FM.EMP_CODIGO
                                JOIN T_FATURAMENTO_MENSAL_CLIENTE FMC ON FMC.FMC_CODIGO = FMCS.FMC_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = FMC.CLI_CGCCPF
                                JOIN T_FATURAMENTO_MENSAL_GRUPO FMG ON FMG.FMG_CODIGO = FMC.FMG_CODIGO 
                                WHERE 1 = 1 ";

            if (codigoEmpresa > 0)
                query += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoGrupoFaturamento > 0)
                query += " AND FMG.FMG_CODIGO = " + codigoGrupoFaturamento.ToString();

            if (codigoServico > 0)
                query += " AND FMC.SER_CODIGO = " + codigoServico.ToString();

            if (dia > 0)
                query += " AND FMC.FMC_DIA_FATURA = " + dia.ToString();

            if (codigoConfiguracaoBoleto > 0)
                query += " AND FMC.BCF_CODIGO = " + codigoConfiguracaoBoleto.ToString();

            if (cnpjPessoa > 0)
                query += " AND FMC.CLI_CGCCPF = " + cnpjPessoa.ToString();

            if ((int)status > 0)
                query += " AND FME_STATUS = " + (int)status;

            if (dataEmissaoInicial != DateTime.MinValue)
            {
                query += " AND FME_DATA_PROCESSAMENTO >= '" + dataEmissaoInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataEmissaoFinal != DateTime.MinValue)
            {
                query += " AND FME_DATA_PROCESSAMENTO <= '" + dataEmissaoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if (dataVencimentoInicial != DateTime.MinValue)
            {
                query += " AND FCS_DATA_VENCIMENTO >= '" + dataVencimentoInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataVencimentoFinal != DateTime.MinValue)
            {
                query += " AND FCS_DATA_VENCIMENTO <= '" + dataVencimentoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal> Consultar(double cnpjPessoa, int codigoGrupoFaturamento, int codigoServico, DateTime dataVencimento, StatusFaturamentoMensal status, int codigoEmpresa)
        {
            var consultaFaturamentoMensalClienteServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico>();

            if (cnpjPessoa > 0)
                consultaFaturamentoMensalClienteServico = consultaFaturamentoMensalClienteServico.Where(clienteServico => clienteServico.FaturamentoMensalCliente.Pessoa.CPF_CNPJ == cnpjPessoa);

            if (codigoGrupoFaturamento > 0)
                consultaFaturamentoMensalClienteServico = consultaFaturamentoMensalClienteServico.Where(clienteServico => clienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Codigo == codigoGrupoFaturamento);

            if (dataVencimento > DateTime.MinValue)
                consultaFaturamentoMensalClienteServico = consultaFaturamentoMensalClienteServico.Where(clienteServico => clienteServico.DataVencimento == dataVencimento);

            var consultaFaturamentoMensal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal>()
                .Where(faturamentoMensal => consultaFaturamentoMensalClienteServico.Any(clienteServico => clienteServico.FaturamentoMensal.Codigo == faturamentoMensal.Codigo));

            if ((int)status > 0)
                consultaFaturamentoMensal = consultaFaturamentoMensal.Where(faturamentoMensal => faturamentoMensal.StatusFaturamentoMensal == status);
            
            if ((int)status == 0)
                consultaFaturamentoMensal = consultaFaturamentoMensal.Where(faturamentoMensal => faturamentoMensal.StatusFaturamentoMensal != StatusFaturamentoMensal.Finalizado && faturamentoMensal.StatusFaturamentoMensal != StatusFaturamentoMensal.Cancelado);
            
            if (codigoEmpresa > 0)
                consultaFaturamentoMensal = consultaFaturamentoMensal.Where(faturamentoMensal => faturamentoMensal.Empresa.Codigo == codigoEmpresa);

            return consultaFaturamentoMensal;
        }

        #endregion Métodos Privados
    }
}
