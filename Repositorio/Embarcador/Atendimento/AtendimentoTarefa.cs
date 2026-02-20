using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Atendimento
{
    public class AtendimentoTarefa : RepositorioBase<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa>
    {
        public AtendimentoTarefa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa BuscarPorAtendimento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa>();
            var result = from obj in query where obj.Atendimento.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa> Consultar(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, string motivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusAberto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusCancelado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusFinalizado, int codigoSistema, int codigoModulo, int codigoTela, int empresa, int empresaPai, int numeroInicial, int numeroFinal, string titulo, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade, int codigoSolicitante, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(motivo))
                result = result.Where(obj => obj.MotivoProblema.Contains(motivo));

            if ((int)statusAberto > 0 && (int)statusCancelado > 0 && (int)statusFinalizado > 0)
                result = result.Where(obj => obj.Status == statusAberto || obj.Status == statusCancelado || obj.Status == statusFinalizado);
            else if ((int)statusAberto > 0 && (int)statusCancelado > 0)
                result = result.Where(obj => obj.Status == statusAberto || obj.Status == statusCancelado);
            else if ((int)statusAberto > 0 && (int)statusFinalizado > 0)
                result = result.Where(obj => obj.Status == statusAberto || obj.Status == statusFinalizado);
            else if ((int)statusCancelado > 0 && (int)statusFinalizado > 0)
                result = result.Where(obj => obj.Status == statusCancelado || obj.Status == statusFinalizado);
            else if ((int)statusAberto > 0)
                result = result.Where(obj => obj.Status == statusAberto);
            else if ((int)statusCancelado > 0)
                result = result.Where(obj => obj.Status == statusCancelado);
            else if ((int)statusFinalizado > 0)
                result = result.Where(obj => obj.Status == statusFinalizado);

            if (codigoSistema > 0)
                result = result.Where(obj => obj.AtendimentoSistema.Codigo == codigoSistema);

            if (codigoModulo > 0)
                result = result.Where(obj => obj.AtendimentoModulo.Codigo == codigoModulo);

            if (codigoTela > 0)
                result = result.Where(obj => obj.AtendimentoTela.Codigo == codigoTela);

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Atendimento.Numero >= numeroInicial && obj.Atendimento.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Atendimento.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Atendimento.Numero == numeroFinal);

            if (!string.IsNullOrWhiteSpace(titulo))
                result = result.Where(obj => obj.Titulo.Contains(titulo));

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataInicial && obj.Data <= dataFinal.AddDays(1));
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataInicial && obj.Data <= dataInicial.AddDays(1));
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataFinal && obj.Data <= dataFinal.AddDays(1));

            if ((int)prioridade > 0)
                result = result.Where(obj => obj.Prioridade == prioridade);

            if (codigoSolicitante > 0)
                result = result.Where(obj => obj.Solicitante.Codigo == codigoSolicitante);

            if (tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                if (empresa == 0 && tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    result = result.Where(obj => obj.Atendimento.Empresa.Codigo == empresaPai);
                else if (empresaPai > 0 && tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    result = result.Where(obj => obj.Atendimento.Empresa.Codigo == empresaPai);
                else
                    result = result.Where(obj => obj.Atendimento.EmpresaFilho.Codigo == empresa);
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, string motivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusAberto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusCancelado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusFinalizado, int codigoSistema, int codigoModulo, int codigoTela, int empresa, int empresaPai, int numeroInicial, int numeroFinal, string titulo, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade, int codigoSolicitante)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(motivo))
                result = result.Where(obj => obj.MotivoProblema.Contains(motivo));

            if ((int)statusAberto > 0 && (int)statusCancelado > 0 && (int)statusFinalizado > 0)
                result = result.Where(obj => obj.Status == statusAberto || obj.Status == statusCancelado || obj.Status == statusFinalizado);
            else if ((int)statusAberto > 0 && (int)statusCancelado > 0)
                result = result.Where(obj => obj.Status == statusAberto || obj.Status == statusCancelado);
            else if ((int)statusAberto > 0 && (int)statusFinalizado > 0)
                result = result.Where(obj => obj.Status == statusAberto || obj.Status == statusFinalizado);
            else if ((int)statusCancelado > 0 && (int)statusFinalizado > 0)
                result = result.Where(obj => obj.Status == statusCancelado || obj.Status == statusFinalizado);
            else if ((int)statusAberto > 0)
                result = result.Where(obj => obj.Status == statusAberto);
            else if ((int)statusCancelado > 0)
                result = result.Where(obj => obj.Status == statusCancelado);
            else if ((int)statusFinalizado > 0)
                result = result.Where(obj => obj.Status == statusFinalizado);

            if (codigoSistema > 0)
                result = result.Where(obj => obj.AtendimentoSistema.Codigo == codigoSistema);

            if (codigoModulo > 0)
                result = result.Where(obj => obj.AtendimentoModulo.Codigo == codigoModulo);

            if (codigoTela > 0)
                result = result.Where(obj => obj.AtendimentoTela.Codigo == codigoTela);

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Atendimento.Numero >= numeroInicial && obj.Atendimento.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Atendimento.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Atendimento.Numero == numeroFinal);

            if (!string.IsNullOrWhiteSpace(titulo))
                result = result.Where(obj => obj.Titulo.Contains(titulo));

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataInicial && obj.Data <= dataFinal.AddDays(1));
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataInicial && obj.Data <= dataInicial.AddDays(1));
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataFinal && obj.Data <= dataFinal.AddDays(1));

            if ((int)prioridade > 0)
                result = result.Where(obj => obj.Prioridade == prioridade);

            if (codigoSolicitante > 0)
                result = result.Where(obj => obj.Solicitante.Codigo == codigoSolicitante);

            if (tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                if (empresa == 0 && tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    result = result.Where(obj => obj.Atendimento.Empresa.Codigo == empresaPai);
                else if (empresaPai > 0 && tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    result = result.Where(obj => obj.Atendimento.Empresa.Codigo == empresaPai);
                else
                    result = result.Where(obj => obj.Atendimento.EmpresaFilho.Codigo == empresa);
            }

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Atendimentos.RelatorioChamado> RelatorioChamado(int codigoEmpresa, int codigoEmpresaFilho, int codigoTela, int codigoModulo, int codigoSistema, int codigoTipo, int codigoFuncionario, string titulo, DateTime dataChamadoInicial, DateTime dataChamadoFinal, DateTime dataAtendimentoInicial, DateTime dataAtendimentoFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade, int codigoSolicitante, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = false, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"   SELECT AT.ATC_DATA DataChamado, 
                                A.ATE_DATA_INICIAL DataAtendimento, 
                                CASE  
	                                WHEN AT.ATC_STATUS = 1 THEN 'Aberto'  
	                                WHEN AT.ATC_STATUS = 2 THEN 'Cancelado' 
	                                WHEN AT.ATC_STATUS = 3 THEN 'Finalizado' 
	                                ELSE 'Indefinido'  
                                END DescricaoStatus,
                                CASE  
	                                WHEN AT.ATC_PRIORIDADE = 1 THEN 'Baixa'  
	                                WHEN AT.ATC_PRIORIDADE = 2 THEN 'Normal' 
	                                WHEN AT.ATC_PRIORIDADE = 3 THEN 'Alta' 
	                                ELSE 'Indefinido'  
                                END DescricaoPrioridade,
                                ATL.ATL_DESCRICAO Tela, 
                                ATM.ATM_DESCRICAO Modulo, 
                                ATS.ATS_DESCRICAO Sistema,
                                ATT.ATT_DESCRICAO Tipo,
                                EF.EMP_RAZAO EmpresaFilho, 
                                E.EMP_RAZAO Empresa,
                                AT.ATC_TITULO Titulo,
                                AT.ATC_MOTIVO_PROBLEMA Motivo, 
                                AT.ATC_JUSTIFICATIVA_SOLUCAO_OBSERVACAO Observacao, 
                                F.FUN_NOME Funcionario, 
                                A.ATE_CONTATO_ATENDIMENTO Pessoa, 
                                A.ATE_OBSERVACAO ObservacaoSuporte, 
                                A.ATE_OBSERVACAO_SUPORTE Justificativa,
                                A.PEV_NUMERO Numero,
                                FS.FUN_NOME Solicitante

                                FROM T_ATENDIMENTO_TAREFA AT
                                JOIN T_ATENDIMENTO A ON A.ATE_CODIGO = AT.ATE_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO
                                LEFT OUTER JOIN T_ATENDIMENTO_TELA ATL ON ATL.ATL_CODIGO = AT.ATL_CODIGO
                                LEFT OUTER JOIN T_ATENDIMENTO_TIPO ATT ON ATT.ATT_CODIGO = AT.ATT_CODIGO
                                LEFT OUTER JOIN T_ATENDIMENTO_MODULO ATM ON ATM.ATM_CODIGO = AT.ATM_CODIGO
                                JOIN T_ATENDIMENTO_SISTEMA ATS ON ATS.ATS_CODIGO = AT.ATS_CODIGO
                                JOIN T_EMPRESA EF ON EF.EMP_CODIGO = A.EMP_CODIGO_FILHO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = A.EMP_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO FS ON FS.FUN_CODIGO = AT.FUN_CODIGO_SOLICITANTE
                                WHERE 1 = 1 ";

            if (codigoEmpresa > 0)
                query += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoEmpresaFilho > 0)
                query += " AND EF.EMP_CODIGO = " + codigoEmpresaFilho.ToString();

            if (codigoTela > 0)
                query += " AND ATL.ATL_CODIGO = " + codigoTela.ToString();

            if (codigoModulo > 0)
                query += " AND ATM.ATM_CODIGO = " + codigoModulo.ToString();

            if (codigoSistema > 0)
                query += " AND ATS.ATS_CODIGO = " + codigoSistema.ToString();

            if (codigoTipo > 0)
                query += " AND ATT.ATT_CODIGO = " + codigoTipo.ToString();

            if (codigoFuncionario > 0)
                query += " AND F.FUN_CODIGO = " + codigoFuncionario.ToString();

            if (!string.IsNullOrWhiteSpace(titulo))
                query += " AND AT.ATC_TITULO LIKE '%" + titulo + "%'";

            if (dataChamadoInicial != DateTime.MinValue)
                query += " AND AT.ATC_DATA >= '" + dataChamadoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataChamadoFinal != DateTime.MinValue)
                query += " AND AT.ATC_DATA <= '" + dataChamadoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (dataAtendimentoInicial != DateTime.MinValue)
                query += " AND A.ATE_DATA_INICIAL >= '" + dataAtendimentoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataAtendimentoFinal != DateTime.MinValue)
                query += " AND A.ATE_DATA_INICIAL <= '" + dataAtendimentoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if ((int)status > 0)
                query += " AND AT.ATC_STATUS = '" + (int)status + "'";

            if ((int)prioridade > 0)
                query += " AND AT.ATC_PRIORIDADE = '" + (int)prioridade + "'";

            if (codigoSolicitante > 0)
                query += " AND FS.FUN_CODIGO = " + codigoSolicitante.ToString();

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

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Atendimentos.RelatorioChamado)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Atendimentos.RelatorioChamado>();
        }

        public int ContarRelatorioChamado(int codigoEmpresa, int codigoEmpresaFilho, int codigoTela, int codigoModulo, int codigoSistema, int codigoTipo, int codigoFuncionario, string titulo, DateTime dataChamadoInicial, DateTime dataChamadoFinal, DateTime dataAtendimentoInicial, DateTime dataAtendimentoFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade, int codigoSolicitante)
        {
            string query = @"   SELECT COUNT(0) as CONTADOR
                                FROM T_ATENDIMENTO_TAREFA AT
                                JOIN T_ATENDIMENTO A ON A.ATE_CODIGO = AT.ATE_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO
                                LEFT OUTER JOIN T_ATENDIMENTO_TELA ATL ON ATL.ATL_CODIGO = AT.ATL_CODIGO
                                LEFT OUTER JOIN T_ATENDIMENTO_TIPO ATT ON ATT.ATT_CODIGO = AT.ATT_CODIGO
                                LEFT OUTER JOIN T_ATENDIMENTO_MODULO ATM ON ATM.ATM_CODIGO = AT.ATM_CODIGO
                                JOIN T_ATENDIMENTO_SISTEMA ATS ON ATS.ATS_CODIGO = AT.ATS_CODIGO
                                JOIN T_EMPRESA EF ON EF.EMP_CODIGO = A.EMP_CODIGO_FILHO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = A.EMP_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO FS ON FS.FUN_CODIGO = AT.FUN_CODIGO_SOLICITANTE
                                WHERE 1 = 1 ";

            if (codigoEmpresa > 0)
                query += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoEmpresaFilho > 0)
                query += " AND EF.EMP_CODIGO = " + codigoEmpresaFilho.ToString();

            if (codigoTela > 0)
                query += " AND ATL.ATL_CODIGO = " + codigoTela.ToString();

            if (codigoModulo > 0)
                query += " AND ATM.ATM_CODIGO = " + codigoModulo.ToString();

            if (codigoSistema > 0)
                query += " AND ATS.ATS_CODIGO = " + codigoSistema.ToString();

            if (codigoTipo > 0)
                query += " AND ATT.ATT_CODIGO = " + codigoTipo.ToString();

            if (codigoFuncionario > 0)
                query += " AND F.FUN_CODIGO = " + codigoFuncionario.ToString();

            if (!string.IsNullOrWhiteSpace(titulo))
                query += " AND AT.ATC_TITULO LIKE '%" + titulo + "%'";

            if (dataChamadoInicial != DateTime.MinValue)
                query += " AND AT.ATC_DATA >= '" + dataChamadoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataChamadoFinal != DateTime.MinValue)
                query += " AND AT.ATC_DATA <= '" + dataChamadoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (dataAtendimentoInicial != DateTime.MinValue)
                query += " AND A.ATE_DATA_INICIAL >= '" + dataAtendimentoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataAtendimentoFinal != DateTime.MinValue)
                query += " AND A.ATE_DATA_INICIAL <= '" + dataAtendimentoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if ((int)status > 0)
                query += " AND AT.ATC_STATUS = '" + (int)status + "'";

            if ((int)prioridade > 0)
                query += " AND AT.ATC_PRIORIDADE = '" + (int)prioridade + "'";

            if (codigoSolicitante > 0)
                query += " AND FS.FUN_CODIGO = " + codigoSolicitante.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }
    }
}
