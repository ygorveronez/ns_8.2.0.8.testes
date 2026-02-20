using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaCheckList : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList>
    {
        public GuaritaCheckList(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList BuscarPorCargaVeiculo(int codigoCarga, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Veiculo.Codigo == codigoVeiculo && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList.Cancelado select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList> BuscarPorGuarita(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList>();
            var result = from obj in query where obj.Guarita.Codigo == codigo select obj;
            return result.ToList();
        }

        public bool ContemCheckListPorGuarita(int codigoGuarita)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList>();
            var result = from obj in query where obj.Guarita.Codigo == codigoGuarita select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList> Consultar(int ordemServico, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipo, int carga, int operador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList situacao, int veiculo, int codigoEmpresa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(ordemServico, tipo, carga, operador, situacao, veiculo, codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(o => o.Operador)
                .Fetch(o => o.CheckListTipo)
                .Fetch(o => o.Veiculo)
                .Fetch(o => o.Motorista)
                .Fetch(o => o.OrdemServicoFrota)
                .Fetch(o => o.Carga)
                .ToList();
        }

        public int ContarConsulta(int ordemServico, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipo, int carga, int operador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList situacao, int veiculo, int codigoEmpresa)
        {
            var result = _Consultar(ordemServico, tipo, carga, operador, situacao, veiculo, codigoEmpresa);

            return result.Count();
        }

        public bool BuscarCheckListVencida(int codigoVeiculo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.DataProgramada < data.Date && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList.Aberto select obj;
            return result.Any();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CheckListVistoria> BuscarCheckListParaAlerta()
        {
            string sqlQuery = @"SELECT 
                                GuaritaCheckList.GCL_CODIGO Codigo,                                
                                GuaritaCheckList.CLT_CODIGO CodigoTipoChecklist,
                                VistoriaCheckList.VCH_PERIOCIDADE_VENCIMENTO DiasVencimento,
                                Veiculo.VEI_PLACA Placa,
                                Veiculo.VEI_KMATUAL KmAtual,
                                GuaritaCheckList.GCL_DATA_PROGRAMADA DataProgramada,
                                TiposCheckList.CLT_DESCRICAO DescricaoTipoCheckList
                               FROM
                                T_GUARITA_CHECK_LIST GuaritaCheckList
                               JOIN T_CHECK_LIST_TIPOS TiposCheckList on TiposCheckList.CLT_CODIGO = GuaritaCheckList.CLT_CODIGO
                               JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = GuaritaCheckList.VEI_CODIGO
                               JOIN T_VISTORIA_CHECKLIST VistoriaCheckList ON VistoriaCheckList.VCH_CODIGO = 
		                                (SELECT TOP(1) VistoriaCheckListSubSelect.VCH_CODIGO FROM T_VISTORIA_CHECKLIST VistoriaCheckListSubSelect 
		                                 JOIN T_VISTORIA_CHECKLIST_MODELO_VEICULAR ChecklistVistoriaModeloVeicular ON ChecklistVistoriaModeloVeicular.VCH_CODIGO = VistoriaCheckListSubSelect.VCH_CODIGO 		 
		                                    WHERE VistoriaCheckListSubSelect.CLT_CODIGO = GuaritaCheckList.CLT_CODIGO AND ChecklistVistoriaModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO AND VistoriaCheckListSubSelect.VCH_STATUS = 1)
                               WHERE GuaritaCheckList.GCL_DATA_PROGRAMADA IS NOT NULL AND Veiculo.VEI_ATIVO = 1 AND NOT EXISTS (SELECT ALE_CODIGO FROM T_ALERTA Alerta WHERE Alerta.ALE_TELA_ALERTA = 11 AND Alerta.ALE_CODIGO_ENTIDADE = GuaritaCheckList.GCL_CODIGO); ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CheckListVistoria)));
            return query.List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CheckListVistoria>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList> _Consultar(int ordemServico, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipo, int carga, int operador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList situacao, int veiculo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList>();

            var result = from obj in query select obj;

            // Filtros
            if (carga > 0)
                result = result.Where(o => o.Carga.Codigo == carga);

            if (ordemServico > 0)
                result = result.Where(o => o.OrdemServicoFrota.Codigo == ordemServico);

            if (tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Todos)
                result = result.Where(o => o.TipoEntradaSaida == tipo);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList.Todos)
                result = result.Where(o => o.Situacao == situacao);

            if (operador > 0)
                result = result.Where(o => o.Operador.Codigo == operador);

            if (veiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == veiculo);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result;
        }

        #endregion

        #region Relatório Check List Guarita

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListGuarita> RelatorioCheckListGuarita(int codigoCheckListGuarita)
        {
            string query = @"select G.GCL_CODIGO CodigoCheck,
                G.CLC_DATA_ABERTURA DataAbertura,
                G.GCL_KM_ATUAL KMAtual,
                CASE
	                WHEN G.GCL_ENTRADA_SAIDA = 1 THEN 'Entrada'
	                ELSE 'Saída'
                END EntradaSaida,
                G.GCL_OBSERVACAO Observacao,
                P.GPE_CODIGO CodigoPergunta,
                P.GPE_DESCRICAO DescricaoPergunta,
                CASE
	                WHEN P.GPE_CATEGORIA = 1 THEN 'Tração'
	                WHEN P.GPE_CATEGORIA = 2 THEN 'Reboque'
	                WHEN P.GPE_CATEGORIA = 3 THEN 'Motorista'
	                ELSE 'Manutenção'
                END DescricaoCategoria,
                CASE
	                WHEN P.GPE_TIPO = 0 THEN 'Aprovação'
	                WHEN P.GPE_TIPO = 1 THEN 'Sim/Não'
	                WHEN P.GPE_TIPO = 2 THEN 'Opções'
                    WHEN P.GPE_TIPO = 4 THEN 'Seleções'
	                ELSE 'Informativo'
                END TipoCategoria,
                P.GPE_TIPO CodigoTipo,
                P.GPE_REPSOSTA RespostaPergunta,
                P.GPE_OPCAO OpcaoPergunta,
                A.GAL_CODIGO CodigoAlternativa,
                A.GAL_DESCRICAO DescricaoAlternativa,
                A.GAL_ORDEM OrdemAlternativa,
                A.GAL_MARCADO MarcadoAlternativa,
                F.FUN_NOME Operador,
                GA.CAR_CODIGO_CARGA_EMBARCADOR Carga,
                O.OSE_NUMERO OrdemServico,
                V.VEI_PLACA Veiculo,
                (SELECT F.FUN_NOME FROM T_FUNCIONARIO F
			            where F.FUN_CODIGO = G.FUN_MOTORISTA) Motorista,
			    (SELECT F.FUN_CPF FROM T_FUNCIONARIO F
			            where F.FUN_CODIGO = G.FUN_MOTORISTA) CPF
                from T_GUARITA_CHECK_LIST G 
                JOIN T_GUARITA_CHECK_LIST_PERGUNTA P ON G.GCL_CODIGO = P.GCL_CODIGO
                LEFT OUTER JOIN T_GUARITA_CHECK_LIST_PERGUNTA_ALTERNATIVA A ON A.GPE_CODIGO = P.GPE_CODIGO
                LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = G.FUN_OPERADOR
                LEFT OUTER JOIN T_CARGA GA ON GA.CAR_CODIGO = G.CAR_CODIGO
                LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO O ON O.OSE_CODIGO = G.OSE_CODIGO
                LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = G.VEI_CODIGO
                WHERE P.GPE_TIPO > 0 AND G.GCL_CODIGO = " + codigoCheckListGuarita.ToString() + @"
                ORDER BY P.GPE_CODIGO, P.GPE_TIPO, A.GAL_ORDEM";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListGuarita)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.CheckListGuarita>();
        }

        #endregion

        #region Relatório de Guarita Check List

        public IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.GuaritaCheckList> ConsultarRelatorioGuaritaCheckList(DateTime dataInicial, DateTime dataFinal, int carga, int ordemServico, int veiculo, int tipoCheck, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipo, int codigoEmpresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioGuaritaCheckList(dataInicial, dataFinal, carga, ordemServico, veiculo, tipoCheck, tipo, codigoEmpresa, tipoServicoMultisoftware, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.GuaritaCheckList)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.GuaritaCheckList>();
        }

        public int ContarConsultaRelatorioGuaritaCheckList(DateTime dataInicial, DateTime dataFinal, int carga, int ordemServico, int veiculo, int tipoCheck, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipo, int codigoEmpresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioGuaritaCheckList(dataInicial, dataFinal, carga, ordemServico, veiculo, tipoCheck, tipo, codigoEmpresa, tipoServicoMultisoftware, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioGuaritaCheckList(DateTime dataInicial, DateTime dataFinal, int carga, int ordemServico, int veiculo, int tipoCheck, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipo, int codigoEmpresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioGuaritaCheckList(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, ref where, count, tipoServicoMultisoftware);

            SetarWhereRelatorioConsultaRelatorioGuaritaCheckList(ref where, ref groupBy, ref joins, dataInicial, dataFinal, carga, ordemServico, veiculo, tipoCheck, tipo, codigoEmpresa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioGuaritaCheckList(propAgrupa, 0, ref select, ref groupBy, ref joins, ref where, count, tipoServicoMultisoftware);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_GUARITA_CHECK_LIST CheckList ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioGuaritaCheckList(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, ref string where, bool count, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string selecionado = " [Selecionado] ";
            string marcado = " [X] ";
            string naoMarcado = " [  ] ";

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                marcado = "[Aprovado]";
                naoMarcado = "[Com inconformidade]";
            }

            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "CheckList.GCL_CODIGO Codigo, ";
                        groupBy += "CheckList.GCL_CODIGO, ";
                    }
                    break;
                case "DataAbertura":
                    if (!select.Contains(" DataAbertura, "))
                    {
                        select += "CheckList.CLC_DATA_ABERTURA DataAbertura, ";
                        groupBy += "CheckList.CLC_DATA_ABERTURA, ";
                    }
                    break;
                case "KMAtual":
                    if (!select.Contains(" KMAtual, "))
                    {
                        select += "CheckList.GCL_KM_ATUAL KMAtual, ";
                        groupBy += "CheckList.GCL_KM_ATUAL, ";
                    }
                    break;
                case "Tipo":
                    if (!select.Contains(" Tipo, "))
                    {
                        select += @"CASE WHEN CheckList.GCL_ENTRADA_SAIDA = 1 THEN 'Entrada' 
                                        WHEN CheckList.GCL_ENTRADA_SAIDA = 2 THEN 'Saída' 
                                        ELSE 'Não definido' 
                                    END Tipo, ";
                        groupBy += "CheckList.GCL_ENTRADA_SAIDA, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "CheckList.GCL_OBSERVACAO Observacao, ";
                        groupBy += "CheckList.GCL_OBSERVACAO, ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = CheckList.VEI_CODIGO";

                        select += "Veiculo.VEI_PLACA Veiculo, ";
                        groupBy += "Veiculo.VEI_PLACA, ";
                    }
                    break;
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CheckList.CAR_CODIGO";

                        select += "Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ";
                        groupBy += "Carga.CAR_CODIGO_CARGA_EMBARCADOR, ";
                    }
                    break;
                case "OrdemServico":
                    if (!select.Contains(" OrdemServico, "))
                    {
                        if (!joins.Contains(" OrdemServico "))
                            joins += " LEFT JOIN T_FROTA_ORDEM_SERVICO OrdemServico ON OrdemServico.OSE_CODIGO = CheckList.OSE_CODIGO";

                        select += "OrdemServico.OSE_NUMERO OrdemServico, ";
                        groupBy += "OrdemServico.OSE_NUMERO, ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        if (!joins.Contains(" OrdemServico "))
                            joins += " LEFT JOIN T_FROTA_ORDEM_SERVICO OrdemServico ON OrdemServico.OSE_CODIGO = CheckList.OSE_CODIGO";

                        if (!joins.Contains(" MotoristaOrdemServico "))
                            joins += " LEFT JOIN T_FUNCIONARIO MotoristaOrdemServico ON MotoristaOrdemServico.FUN_CODIGO = OrdemServico.FUN_MOTORISTA";

                        if (!joins.Contains(" Carga "))
                            joins += " LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CheckList.CAR_CODIGO";

                        /*if (!joins.Contains(" CargaMotorista "))
                            joins += " LEFT JOIN T_CARGA_MOTORISTA CargaMotorista ON CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO";

                        if (!joins.Contains(" Motorista "))
                            joins += " LEFT JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA";*/

                        select += @"ISNULL(SUBSTRING((SELECT ', ' + Motorista.FUN_NOME
                                    FROM T_CARGA_MOTORISTA CargaMotorista 
                                    INNER JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
                                    WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000), 
                                    MotoristaOrdemServico.FUN_NOME
                                    ) AS Motorista, ";
                        groupBy += "Carga.CAR_CODIGO, MotoristaOrdemServico.FUN_NOME, ";
                    }
                    break;
                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        if (!joins.Contains(" Operador "))
                            joins += " LEFT JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = CheckList.FUN_OPERADOR";

                        select += "Operador.FUN_NOME Operador, ";
                        groupBy += "Operador.FUN_NOME, ";
                    }
                    break;
                case "TipoCheck":
                    if (!select.Contains(" TipoCheck, "))
                    {
                        if (!joins.Contains(" TipoCheck "))
                            joins += " LEFT JOIN T_CHECK_LIST_TIPOS TipoCheck ON TipoCheck.CLT_CODIGO = CheckList.CLT_CODIGO";

                        select += "TipoCheck.CLT_DESCRICAO TipoCheck, ";
                        groupBy += "TipoCheck.CLT_DESCRICAO, ";
                    }
                    break;
                case "Vistoria":
                    if (!select.Contains(" Vistoria, ")) //Categorias diferente de Motorista
                    {
                        if (!joins.Contains(" CheckListPergunta "))
                            joins += " LEFT JOIN T_GUARITA_CHECK_LIST_PERGUNTA CheckListPergunta ON CheckListPergunta.GCL_CODIGO = CheckList.GCL_CODIGO";

                        if (!joins.Contains(" CheckListPerguntaAlternativa "))
                            joins += " LEFT JOIN T_GUARITA_CHECK_LIST_PERGUNTA_ALTERNATIVA CheckListPerguntaAlternativa ON CheckListPerguntaAlternativa.GPE_CODIGO = CheckListPergunta.GPE_CODIGO";

                        select += @"CASE WHEN CheckListPergunta.GPE_CATEGORIA <> 3 THEN  
                                        CASE WHEN CheckListPergunta.GPE_CATEGORIA = 2 THEN 'Reboque'
                                            WHEN CheckListPergunta.GPE_CATEGORIA = 4 THEN 'Manutenção'
                                            ELSE 'Tração' END + ' | ' + 
                                        CheckListPergunta.GPE_DESCRICAO + ' - ' + 
                                        CASE WHEN CheckListPergunta.GPE_TIPO = 1 THEN CASE WHEN CheckListPergunta.GPE_OPCAO = 1 THEN 'Sim' ELSE 'Não' END
                                            WHEN CheckListPergunta.GPE_TIPO = 3 THEN CheckListPergunta.GPE_REPSOSTA
                                            WHEN CheckListPergunta.GPE_TIPO = 4 THEN CheckListPerguntaAlternativa.GAL_DESCRICAO + ' - ' + 
                                                CASE WHEN CheckListPerguntaAlternativa.GAL_MARCADO = 1 THEN '" + selecionado + @"' ELSE '" + naoMarcado + @"' END
                                            ELSE CheckListPerguntaAlternativa.GAL_DESCRICAO + ' - ' + 
                                                CASE WHEN CheckListPerguntaAlternativa.GAL_MARCADO = 1 THEN '" + marcado + @"' ELSE '" + naoMarcado + @"' END
                                        END
                                    ELSE '' END Vistoria, ";
                        groupBy += @"CheckListPergunta.GPE_CATEGORIA, CheckListPergunta.GPE_DESCRICAO, CheckListPergunta.GPE_TIPO, CheckListPergunta.GPE_REPSOSTA, CheckListPergunta.GPE_OPCAO, 
                                        CheckListPerguntaAlternativa.GAL_DESCRICAO, CheckListPerguntaAlternativa.GAL_MARCADO, ";

                        //Somente assinalados 
                        where += " AND (CheckListPerguntaAlternativa.GAL_MARCADO = 1 OR CheckListPergunta.GPE_REPSOSTA <> '' OR CheckListPergunta.GPE_OPCAO = 1) AND CheckListPergunta.GPE_TIPO > 0 ";
                        if (!where.Contains(" CheckListPergunta.GPE_CATEGORIA = 3 "))
                            where += " AND CheckListPergunta.GPE_CATEGORIA <> 3 ";
                        else
                            where = where.Replace("AND CheckListPergunta.GPE_CATEGORIA = 3", string.Empty);
                    }
                    break;
                case "Croquis":
                    if (!select.Contains(" Croquis, ")) //Somente Categoria Motorista
                    {
                        if (!joins.Contains(" CheckListPergunta "))
                            joins += " LEFT JOIN T_GUARITA_CHECK_LIST_PERGUNTA CheckListPergunta ON CheckListPergunta.GCL_CODIGO = CheckList.GCL_CODIGO";

                        if (!joins.Contains(" CheckListPerguntaAlternativa "))
                            joins += " LEFT JOIN T_GUARITA_CHECK_LIST_PERGUNTA_ALTERNATIVA CheckListPerguntaAlternativa ON CheckListPerguntaAlternativa.GPE_CODIGO = CheckListPergunta.GPE_CODIGO";

                        select += @"CASE WHEN CheckListPergunta.GPE_CATEGORIA = 3 THEN  
                                        'Motorista' + ' | ' + 
                                        CheckListPergunta.GPE_DESCRICAO + ' - ' + 
                                        CASE WHEN CheckListPergunta.GPE_TIPO = 1 THEN CASE WHEN CheckListPergunta.GPE_OPCAO = 1 THEN 'Sim' ELSE 'Não' END
                                            WHEN CheckListPergunta.GPE_TIPO = 3 THEN CheckListPergunta.GPE_REPSOSTA
                                            WHEN CheckListPergunta.GPE_TIPO = 4 THEN CheckListPerguntaAlternativa.GAL_DESCRICAO + ' - ' + 
                                                CASE WHEN CheckListPerguntaAlternativa.GAL_MARCADO = 1 THEN '" + selecionado + @"' ELSE '" + naoMarcado + @"' END
                                            ELSE CheckListPerguntaAlternativa.GAL_DESCRICAO + ' - ' + 
                                                CASE WHEN CheckListPerguntaAlternativa.GAL_MARCADO = 1 THEN '" + marcado + @"' ELSE '" + naoMarcado + @"' END
                                        END
                                    ELSE '' END Croquis, ";
                        groupBy += @"CheckListPergunta.GPE_CATEGORIA, CheckListPergunta.GPE_DESCRICAO, CheckListPergunta.GPE_TIPO, CheckListPergunta.GPE_REPSOSTA, CheckListPergunta.GPE_OPCAO, 
                                        CheckListPerguntaAlternativa.GAL_DESCRICAO, CheckListPerguntaAlternativa.GAL_MARCADO, ";

                        //Somente assinalados 
                        where += " AND (CheckListPerguntaAlternativa.GAL_MARCADO = 1 OR CheckListPergunta.GPE_REPSOSTA <> '' OR CheckListPergunta.GPE_OPCAO = 1) AND CheckListPergunta.GPE_TIPO > 0 ";
                        if (!where.Contains(" CheckListPergunta.GPE_CATEGORIA <> 3 "))
                            where += " AND CheckListPergunta.GPE_CATEGORIA = 3 ";
                        else
                            where = where.Replace("AND CheckListPergunta.GPE_CATEGORIA <> 3", string.Empty);
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioGuaritaCheckList(ref string where, ref string groupBy, ref string joins, DateTime dataInicial, DateTime dataFinal, int carga, int ordemServico, int veiculo, int tipoCheck, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipo, int codigoEmpresa)
        {
            string pattern = "yyyy-MM-dd";

            if (dataInicial != DateTime.MinValue)
                where += " AND CheckList.CLC_DATA_ABERTURA >= '" + dataInicial.ToString(pattern) + "' ";

            if (dataFinal != DateTime.MinValue)
                where += " AND CheckList.CLC_DATA_ABERTURA <= '" + dataFinal.AddDays(1).ToString(pattern) + "'";

            if ((int)tipo > 0)
                where += " AND CheckList.GCL_ENTRADA_SAIDA = " + tipo.ToString("D");

            if (carga > 0)
                where += " AND CheckList.CAR_CODIGO = " + carga.ToString();

            if (ordemServico > 0)
                where += " AND CheckList.OSE_CODIGO = " + ordemServico.ToString();

            if (veiculo > 0)
                where += " AND CheckList.VEI_CODIGO = " + veiculo.ToString();

            if (tipoCheck > 0)
                where += " AND CheckList.CLT_CODIGO = " + tipoCheck.ToString();

            if (codigoEmpresa > 0)
                where += " AND CheckList.EMP_CODIGO = " + codigoEmpresa.ToString();
        }

        #endregion
    }
}
