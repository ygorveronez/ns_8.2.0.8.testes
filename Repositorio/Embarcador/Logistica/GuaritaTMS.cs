using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class GuaritaTMS : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>
    {
        public GuaritaTMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.GuaritaTMS BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemRegistroGuaritaCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoEntradaSaida == tipoEntradaSaida);
            return query.Any();
        }

        public bool ContemRegistroGuaritaOrdemDeServico(int codigoOrdemDeServico, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            query = query.Where(obj => obj.OrdemServicoFrota.Codigo == codigoOrdemDeServico && obj.TipoEntradaSaida == tipoEntradaSaida);
            return query.Any();
        }

        public bool RegistroDuplicado(int codigoVeiculo, int km, int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            var result = from obj in query where obj.Codigo != codigo && obj.Veiculo.Codigo == codigoVeiculo && obj.KMAtual == km && obj.TipoEntradaSaida == tipoEntradaSaida select obj;
            return result.Count() > 0;
        }

        public bool RegistroEntradaSaidaDuplicadoCarga(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, int codigo, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            var result = from obj in query where obj.Codigo != codigo && obj.Veiculo.Codigo == codigoVeiculo && obj.TipoEntradaSaida == tipoEntradaSaida && obj.Carga.Codigo == codigoCarga select obj;
            return result.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Logistica.GuaritaTMS BuscarUltimoEntradaCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoEntradaSaida == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada select obj;
            return result.OrderBy("KMAtual descending, Codigo descending").FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.GuaritaTMS BuscarUltimoEntradaMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            query = query.Where(obj => obj.Motorista.Codigo == codigoMotorista && obj.TipoEntradaSaida == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada);
            return query.OrderBy("DataSaidaEntrada descending, HoraSaidaEntrada descending, Codigo descending").FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.GuaritaTMS BuscarUltimoRegistro(int codigoVeiculo, int codigoGuarita)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            var result = from obj in query where obj.Codigo != codigoGuarita && obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.OrderBy("KMAtual descending, Codigo descending").FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.GuaritaTMS BuscarRegistroVeiculo(int codigoVeiculo, DateTime dataRegistro, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();
            var result = from obj in query where obj.DataSaidaEntrada.Date == dataRegistro.Date && obj.Veiculo.Codigo == codigoVeiculo && obj.TipoEntradaSaida == tipoEntradaSaida select obj;
            return result.OrderBy("KMAtual descending").FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS> Consultar(int codigoVeiculo, int codigoCarga, int codigoOrdemServico, int codigoMotorista, int kmAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, DateTime dataInicial, DateTime dataFinal, int codigoEmpresa, List<int> reboques, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();

            var result = from obj in query select obj;

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (codigoCarga > 0)
                result = result.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (codigoOrdemServico > 0)
                result = result.Where(obj => obj.OrdemServicoFrota.Codigo == codigoOrdemServico);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (kmAtual > 0)
                result = result.Where(obj => obj.KMAtual == kmAtual);

            if (tipoEntradaSaida > 0)
                result = result.Where(obj => obj.TipoEntradaSaida == tipoEntradaSaida);

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataSaidaEntrada.Date >= dataInicial);

            if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataSaidaEntrada.Date <= dataFinal);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (reboques.Count > 0)
                result = result.Where(obj => obj.Reboques.Any(r => reboques.Contains(r.Codigo)));
            

            return result.Fetch(o => o.Veiculo).Fetch(o => o.Carga).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoVeiculo, int codigoCarga, int codigoOrdemServico, int codigoMotorista, int kmAtual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, DateTime dataInicial, DateTime dataFinal, int codigoEmpresa, List<int> reboques)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();

            var result = from obj in query select obj;

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (codigoCarga > 0)
                result = result.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (codigoOrdemServico > 0)
                result = result.Where(obj => obj.OrdemServicoFrota.Codigo == codigoOrdemServico);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (kmAtual > 0)
                result = result.Where(obj => obj.KMAtual == kmAtual);

            if (tipoEntradaSaida > 0)
                result = result.Where(obj => obj.TipoEntradaSaida == tipoEntradaSaida);

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataSaidaEntrada.Date >= dataInicial);

            if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataSaidaEntrada.Date <= dataFinal);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (reboques.Count > 0)
                result = result.Where(obj => obj.Reboques.Any(r => reboques.Contains(r.Codigo)));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS> Consultar(int codigoCTe, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();

            var result = from obj in query select obj;

            if (codigoCTe > 0)
                result = result.Where(obj => obj.Carga.CargaCTes.Any(o => o.CTe.Codigo == codigoCTe));

            return result.Fetch(o => o.Veiculo).Fetch(o => o.Carga).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>();

            var result = from obj in query select obj;

            if (codigoCTe > 0)
                result = result.Where(obj => obj.Carga.CargaCTes.Any(o => o.CTe.Codigo == codigoCTe));

            return result.Count();
        }

        public void DeletarReboques(int codigoGuarita)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_GUARITA_TMS_REBOQUE WHERE GUA_CODIGO = :codigoGuarita").SetInt32("codigoGuarita", codigoGuarita).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_GUARITA_TMS_REBOQUE WHERE GUA_CODIGO = :codigoGuarita").SetInt32("codigoGuarita", codigoGuarita).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        #endregion

        #region Relatório de Guarita

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.GuaritaTMS> ConsultarRelatorioGuaritaTMS(int empresa, DateTime dataInicial, DateTime dataFinal, int veiculo, int carga, int ordemServico, int motorista, int operador, int kmInicial, int kmFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioGuaritaTMS(empresa, dataInicial, dataFinal, veiculo, carga, ordemServico, motorista, operador, kmInicial, kmFinal, tipoEntradaSaida, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.GuaritaTMS)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.GuaritaTMS>();
        }

        public int ContarConsultaRelatorioGuaritaTMS(int empresa, DateTime dataInicial, DateTime dataFinal, int veiculo, int carga, int ordemServico, int motorista, int operador, int kmInicial, int kmFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioGuaritaTMS(empresa, dataInicial, dataFinal, veiculo, carga, ordemServico, motorista, operador, kmInicial, kmFinal, tipoEntradaSaida, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioGuaritaTMS(int empresa, DateTime dataInicial, DateTime dataFinal, int veiculo, int carga, int ordemServico, int motorista, int operador, int kmInicial, int kmFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaGuaritaTMS(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaGuaritaTMS(ref where, ref groupBy, ref joins, dataInicial, dataFinal, empresa, veiculo, carga, ordemServico, motorista, operador, kmInicial, kmFinal, tipoEntradaSaida);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaGuaritaTMS(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_GUARITA_TMS GuaritaTMS ";

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

        private void SetarSelectRelatorioConsultaGuaritaTMS(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = GuaritaTMS.VEI_CODIGO";

                        select += "Veiculo.VEI_PLACA Veiculo, ";
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        if (!joins.Contains(" Empresa "))
                            joins += " JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = GuaritaTMS.EMP_CODIGO";

                        select += "Empresa.EMP_RAZAO Empresa, ";
                    }
                    break;
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += " LEFT OUTER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = GuaritaTMS.CAR_CODIGO";

                        select += "ISNULL(Carga.CAR_CODIGO_CARGA_EMBARCADOR, '') Carga, ";
                    }
                    break;
                case "OrdemServico":
                    if (!select.Contains(" OrdemServico, "))
                    {
                        if (!joins.Contains(" OrdemServico "))
                            joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO OrdemServico ON OrdemServico.OSE_CODIGO = GuaritaTMS.OSE_CODIGO";

                        select += "ISNULL(OrdemServico.OSE_NUMERO, 0) OrdemServico, ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        if (!joins.Contains(" Motorista "))
                            joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO =  GuaritaTMS.FUN_CODIGO_MOTORISTA";

                        select += "Motorista.FUN_NOME Motorista, ";
                    }
                    break;
                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        if (!joins.Contains(" Operador "))
                            joins += " JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO =  GuaritaTMS.FUN_CODIGO_OPERADOR";

                        select += "Operador.FUN_NOME Operador, ";
                    }
                    break;
                case "KMLancamento":
                    if (!select.Contains(" KMLancamento, "))
                    {
                        select += "GuaritaTMS.GUA_KM_ATUAL KMLancamento, ";
                    }
                    break;
                case "DataPassagem":
                    if (!select.Contains(" DataPassagem, "))
                    {
                        select += "CONVERT(VARCHAR(10), GuaritaTMS.GUA_DATA_SAIDA_ENTRADA , 103) + ' '  + convert(VARCHAR(8), GuaritaTMS.GUA_HORA_SAIDA_ENTRADA, 14) DataPassagem, ";
                    }
                    break;
                case "EntradaSaida":
                    if (!select.Contains(" EntradaSaida, "))
                    {
                        select += @"CASE
	                        WHEN GuaritaTMS.GUA_ENTRADA_SAIDA = 2 THEN 'Saída' 
	                        ELSE 'Entrada'
                        END EntradaSaida, ";
                    }
                    break;
                case "FinalizouViagemFormatado":
                    if (!select.Contains(" FinalizouViagem, "))
                    {
                        select += "GuaritaTMS.GUA_FINALIZOU_VIAGEM FinalizouViagem, ";
                    }
                    break;
                case "RetornouComReboqueFormatado":
                    if (!select.Contains(" RetornouComReboque, "))
                    {
                        select += "GuaritaTMS.GUA_RETORNOU_COM_REBOQUE RetornouComReboque, ";
                    }
                    break;
                case "VeiculoVazioFormatado":
                    if (!select.Contains(" VeiculoVazio, "))
                    {
                        select += "GuaritaTMS.GUA_VEICULO_VAZIO VeiculoVazio, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "GuaritaTMS.GUA_OBSERVACAO Observacao, ";
                    }
                    break;
                case "NumeroFrota":
                    if (!select.Contains(" NumeroFrota, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = GuaritaTMS.VEI_CODIGO";

                        select += "Veiculo.VEI_NUMERO_FROTA NumeroFrota, ";
                    }
                    break;
                case "StatusVeiculoOS":
                    if (!select.Contains(" StatusVeiculoOS, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + CAST(OSE_NUMERO AS NVARCHAR(160)) + 
                                    ' (' + CASE 
	                                    WHEN OSE_SITUACAO = 1 THEN 'Ag. autorização'
	                                    WHEN OSE_SITUACAO = 3 THEN 'Em manutenção'
	                                    WHEN OSE_SITUACAO = 4 THEN 'Divergência'
	                                    ELSE 'Em digitação' END + ')'
		                            FROM T_FROTA_ORDEM_SERVICO FOS
		                            WHERE FOS.OSE_SITUACAO IN (0, 1, 3, 4) AND FOS.VEI_CODIGO = GuaritaTMS.VEI_CODIGO FOR XML PATH('')), 3, 1000) AS StatusVeiculoOS, ";
                    }
                    break;

                case "Reboques":
                    if (!select.Contains(" Reboques, "))
                    {
                        select += @"SUBSTRING((select ', ' + VeiculoReboque.VEI_PLACA
                                                  from T_GUARITA_TMS_REBOQUE Reboque
                                                  join T_VEICULO VeiculoReboque on VeiculoReboque.VEI_CODIGO = Reboque.VEI_CODIGO
                                                  where Reboque.GUA_CODIGO = GuaritaTMS.GUA_CODIGO for XML PATH('')), 3, 1000) as Reboques, ";
                    }
                    break;
                case "NumeroFrotaReboques":
                    if (!select.Contains(" NumeroFrotaReboques, "))
                    {
                        select += @"SUBSTRING((select ', ' + VeiculoReboque.VEI_NUMERO_FROTA
                                                  from T_GUARITA_TMS_REBOQUE Reboque
                                                  join T_VEICULO VeiculoReboque on VeiculoReboque.VEI_CODIGO = Reboque.VEI_CODIGO
                                                  where Reboque.GUA_CODIGO = GuaritaTMS.GUA_CODIGO and VEI_NUMERO_FROTA is not null and VEI_NUMERO_FROTA <> ''
                                               for XML PATH('')), 3, 1000) as NumeroFrotaReboques, ";
                    }
                    break;
                case "SegmentoTracao":
                    if (!select.Contains(" SegmentoTracao, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = GuaritaTMS.VEI_CODIGO";

                        if (!joins.Contains(" SegmentoVeiculo "))
                            joins += " LEFT OUTER JOIN T_VEICULO_SEGMENTO SegmentoVeiculo on SegmentoVeiculo.VSE_CODIGO = Veiculo.VSE_CODIGO";

                        select += @"CASE 
                                    WHEN Veiculo.VEI_TIPOVEICULO = '0' THEN SegmentoVeiculo.VSE_DESCRICAO END as SegmentoTracao, ";
                    }
                    break;
                case "SegmentoReboque":
                    if (!select.Contains(" SegmentoReboque, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = GuaritaTMS.VEI_CODIGO";

                        if (!joins.Contains(" SegmentoVeiculo "))
                            joins += " LEFT OUTER JOIN T_VEICULO_SEGMENTO SegmentoVeiculo on SegmentoVeiculo.VSE_CODIGO = Veiculo.VSE_CODIGO";

                        select += @"CASE 
                                    WHEN Veiculo.VEI_TIPOVEICULO = '1' THEN SegmentoVeiculo.VSE_DESCRICAO END as SegmentoReboque, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaGuaritaTMS(ref string where, ref string groupBy, ref string joins, DateTime dataInicial, DateTime dataFinal, int empresa, int veiculo, int carga, int ordemServico, int motorista, int operador, int kmInicial, int kmFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida)
        {
            string pattern = "yyyy-MM-dd";

            if (dataInicial != DateTime.MinValue)
                where += " AND CAST(GuaritaTMS.GUA_DATA_SAIDA_ENTRADA AS DATE) >= '" + dataInicial.ToString(pattern) + "' ";
            if (dataFinal != DateTime.MinValue)
                where += " AND CAST(GuaritaTMS.GUA_DATA_SAIDA_ENTRADA AS DATE) <= '" + dataFinal.ToString(pattern) + "' ";

            if (veiculo > 0)
            {
                if (!joins.Contains(" Veiculo "))
                    joins += " JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = GuaritaTMS.VEI_CODIGO";

                where += " AND GuaritaTMS.VEI_CODIGO = " + veiculo.ToString();
            }

            if (carga > 0)
            {
                if (!joins.Contains(" Carga "))
                    joins += " LEFT OUTER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = GuaritaTMS.CAR_CODIGO";

                where += " AND GuaritaTMS.CAR_CODIGO = " + carga.ToString();
            }

            if (empresa > 0)
            {
                if (!joins.Contains(" Empresa "))
                    joins += " JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = GuaritaTMS.EMP_CODIGO";

                where += " AND GuaritaTMS.EMP_CODIGO = " + empresa.ToString();
            }            

            if (ordemServico > 0)
            {
                if (!joins.Contains(" OrdemServico "))
                    joins += " LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO OrdemServico ON OrdemServico.OSE_CODIGO = GuaritaTMS.OSE_CODIGO";

                where += " AND GuaritaTMS.OSE_CODIGO = " + ordemServico.ToString();
            }

            if (motorista > 0)
            {
                if (!joins.Contains(" Motorista "))
                    joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO =  GuaritaTMS.FUN_CODIGO_MOTORISTA";

                where += " AND GuaritaTMS.FUN_CODIGO_MOTORISTA = " + motorista.ToString();
            }

            if (operador > 0)
            {
                if (!joins.Contains(" Operador "))
                    joins += " JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO =  GuaritaTMS.FUN_CODIGO_OPERADOR";

                where += " AND GuaritaTMS.FUN_CODIGO_OPERADOR = " + operador.ToString();
            }

            if (kmInicial > 0)
                where += " AND GuaritaTMS.GUA_KM_ATUAL >= " + kmInicial.ToString();
            if (kmFinal > 0)
                where += " AND GuaritaTMS.GUA_KM_ATUAL <= " + kmFinal.ToString();

            if (tipoEntradaSaida == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Saida)
                where += " AND GuaritaTMS.GUA_ENTRADA_SAIDA = 2";
            else if (tipoEntradaSaida == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Entrada)
                where += " AND GuaritaTMS.GUA_ENTRADA_SAIDA = 1";
        }

        #endregion
    }
}
