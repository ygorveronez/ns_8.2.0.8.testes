using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Transportadores
{
    public class MotoristaLicenca : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>
    {
        public MotoristaLicenca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> BuscarLicencasParaAlerta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();
            var result = from obj in query where obj.Motorista.Status == "A" select obj;

            var queryAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Alerta>();
            var resultAlerta = from obj in queryAlerta where obj.TelaAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaTela.Motorista select obj;

            result = result.Where(o => !resultAlerta.Any(c => c.CodigoEntidade == o.Codigo));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> BuscarLicencasParaBloqueioPedido(List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();
            var result = from obj in query where codigosMotorista.Contains(obj.Motorista.Codigo) && obj.DataVencimento.Value.Date < DateTime.Now.Date && obj.BloquearCriacaoPedidoLicencaVencida select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca BuscarLicencaParaBloqueioPedido(List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();
            var result = from obj in query where codigosMotorista.Contains(obj.Motorista.Codigo) && obj.DataVencimento.Value.Date < DateTime.Now.Date && obj.BloquearCriacaoPedidoLicencaVencida select obj;
            return result.FirstOrDefault();
        }

        public bool ContemLicencaValida(int codigoTipoLicenca, DateTime dataAtual, string cpf, bool pedido = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();
            var result = from obj in query 
                        where obj.Motorista.CPF == cpf 
                           && obj.Licenca.Codigo == codigoTipoLicenca 
                           && ( (!obj.DataVencimento.HasValue) 
                              || (obj.DataVencimento >= dataAtual) 
                              || (obj.DataVencimento < dataAtual && ( !pedido || !obj.BloquearCriacaoPedidoLicencaVencida)) 
                              )
                         select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca BuscarLicencaParaBloqueioPlanejamentoPedido(List<int> codigosMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();
            var result = from obj in query where codigosMotorista.Contains(obj.Motorista.Codigo) && obj.DataVencimento.Value.Date < DateTime.Now.Date && obj.BloquearCriacaoPlanejamentoPedidoLicencaVencida select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> BuscarLicencasMotoristaPorUsuarioMobile(int codigoUsuarioMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>()
                .Where(o => o.Motorista.CodigoMobile == codigoUsuarioMobile);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> BuscarLicencasPorDataVencimentoParaMobile(DateTime dataVencimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>()
                .Where(o => o.DataVencimento.Value.Date == dataVencimento && o.Motorista.Status == "A" && o.Motorista.CodigoMobile > 0);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca BuscarLicencasMotoristaPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>()
                .Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Administrativo.Licenca> RelatorioLicenca(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string query = "", queryVeiculo = "", queryFuncionario = "", queryMotorista = "", queryPessoa = "", queryNaoMostra = " AND 1 = 0";
            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                queryVeiculo += " AND V.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                queryFuncionario += " AND F.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                queryPessoa += " AND C.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
            }
            if (filtrosPesquisa.StatusLicenca > 0)
            {
                queryPessoa += $" AND L.PLI_STATUS = {filtrosPesquisa.StatusLicenca.Value.ToString("d")}";
                queryFuncionario += $" AND L.MLI_STATUS = {filtrosPesquisa.StatusLicenca.Value.ToString("d")}";
                queryVeiculo += $" AND L.VLI_STATUS = {filtrosPesquisa.StatusLicenca.Value.ToString("d")}";
            }
            if (filtrosPesquisa.CodigoTipoLicenca > 0)
            {
                queryVeiculo += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
                queryFuncionario += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
                queryPessoa += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
            }

            if (filtrosPesquisa.CodigoPessoa > 0)
            {
                queryPessoa += $" AND L.CLI_CGCCPF = {filtrosPesquisa.CodigoPessoa.ToString()}";
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                queryFuncionario += $" AND L.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista.ToString()}";
            }

            if (filtrosPesquisa.CodigoFuncionario > 0)
            {
                queryFuncionario += $" AND L.FUN_CODIGO = {filtrosPesquisa.CodigoFuncionario.ToString()}";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                queryVeiculo += $" AND L.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Entidade))
            {
                queryVeiculo += " AND VEI_PLACA LIKE '%" + filtrosPesquisa.Entidade + "%'";
                queryFuncionario += " AND FUN_NOME LIKE '%" + filtrosPesquisa.Entidade + "%'";
                queryPessoa += " AND CLI_NOME LIKE '%" + filtrosPesquisa.Entidade + "%'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
            {
                queryVeiculo += " AND VLI_DESCRICAO LIKE '%" + filtrosPesquisa.Descricao + "%'";
                queryFuncionario += " AND MLI_DESCRICAO LIKE '%" + filtrosPesquisa.Descricao + "%'";
                queryPessoa += " AND PLI_DESCRICAO LIKE '%" + filtrosPesquisa.Descricao + "%'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroLicenca))
            {
                queryVeiculo += " AND VLI_NUMERO LIKE '%" + filtrosPesquisa.NumeroLicenca + "%'";
                queryFuncionario += " AND MLI_NUMERO LIKE '%" + filtrosPesquisa.NumeroLicenca + "%'";
                queryPessoa += " AND PLI_NUMERO LIKE '%" + filtrosPesquisa.NumeroLicenca + "%'";
            }

            string datePattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                queryVeiculo += " AND CAST(VLI_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
                queryFuncionario += " AND CAST(MLI_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
                queryPessoa += " AND CAST(PLI_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                queryVeiculo += " AND CAST(VLI_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
                queryFuncionario += " AND CAST(MLI_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
                queryPessoa += " AND CAST(PLI_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
            }

            queryMotorista = queryFuncionario;

            if (filtrosPesquisa.TipoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca.Veiculo)
            {
                queryFuncionario = queryNaoMostra;
                queryPessoa = queryNaoMostra;
                queryMotorista = queryNaoMostra;
            }
            else if (filtrosPesquisa.TipoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca.Pessoa)
            {
                queryFuncionario = queryNaoMostra;
                queryVeiculo = queryNaoMostra;
                queryMotorista = queryNaoMostra;
            }
            else if (filtrosPesquisa.TipoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca.Funcionario)
            {
                queryPessoa = queryNaoMostra;
                queryVeiculo = queryNaoMostra;
                queryMotorista = queryNaoMostra;
            }
            else if (filtrosPesquisa.TipoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca.Motorista)
            {
                queryPessoa = queryNaoMostra;
                queryVeiculo = queryNaoMostra;
                queryFuncionario = queryNaoMostra;
            }

            if (filtrosPesquisa.StatusEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
            {
                queryFuncionario += " AND F.FUN_STATUS = 'A' ";
                queryMotorista += " AND F.FUN_STATUS = 'A' ";
                queryPessoa += " AND CLI_ATIVO = 1 ";
                queryVeiculo += " AND VEI_ATIVO = 1 ";
            }
            else if (filtrosPesquisa.StatusEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
            {
                queryFuncionario += " AND F.FUN_STATUS = 'I' ";
                queryMotorista += " AND F.FUN_STATUS = 'I' ";
                queryPessoa += " AND C.CLI_ATIVO = 0 ";
                queryVeiculo += " AND V.VEI_ATIVO = 0 ";
            }

            query = @"   SELECT MLI_CODIGO Codigo,
                        'Funcionário' Identificador,
                        FUN_NOME Entidade,
                        MLI_DESCRICAO Descricao,
                        MLI_NUMERO NumeroLicenca,
                        MLI_DATA_EMISSAO DataEmissao,
                        MLI_DATA_VENCIMENTO DataVencimento,
                        ISNULL(LL.LIC_DESCRICAO, '') TipoLicenca,
                        '' NumeroFrota,
                        CASE 
                            WHEN F.FUN_STATUS = 'A' THEN CAST(1 AS BIT)
							ELSE CAST(0 AS BIT)
						END StatusEntidade 
                        FROM T_MOTORISTA_LICENCA L
                        JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = L.FUN_CODIGO
                        LEFT OUTER JOIN T_LICENCA LL ON LL.LIC_CODIGO = L.LIC_CODIGO
                        WHERE FUN_TIPO = 'U' " + queryFuncionario + @"

                        UNION

                        SELECT MLI_CODIGO Codigo,
                        'Motorista' Identificador,
                        FUN_NOME Entidade,
                        MLI_DESCRICAO Descricao,
                        MLI_NUMERO NumeroLicenca,
                        MLI_DATA_EMISSAO DataEmissao,
                        MLI_DATA_VENCIMENTO DataVencimento,
                        ISNULL(LL.LIC_DESCRICAO, '') TipoLicenca,
                        '' NumeroFrota,
                        CASE 
                            WHEN F.FUN_STATUS = 'A' THEN CAST(1 AS BIT)
							ELSE CAST(0 AS BIT)
						END StatusEntidade 
                        FROM T_MOTORISTA_LICENCA L
                        JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = L.FUN_CODIGO
                        LEFT OUTER JOIN T_LICENCA LL ON LL.LIC_CODIGO = L.LIC_CODIGO
                        WHERE FUN_TIPO = 'M' " + queryMotorista + @"

                        UNION

                        SELECT PLI_CODIGO Codigo,
                        'Pessoa' Identificador,
                        CLI_NOME Entidade,
                        PLI_DESCRICAO Descricao,
                        PLI_NUMERO NumeroLicenca,
                        PLI_DATA_EMISSAO DataEmissao,
                        PLI_DATA_VENCIMENTO DataVencimento,
                        ISNULL(LL.LIC_DESCRICAO, '') TipoLicenca,
                        '' NumeroFrota,
                        C.CLI_ATIVO StatusEntidade 
                        FROM T_PESSOA_LICENCA L
                        JOIN T_CLIENTE C ON C.CLI_CGCCPF = L.CLI_CGCCPF
                        LEFT OUTER JOIN T_LICENCA LL ON LL.LIC_CODIGO = L.LIC_CODIGO
                        WHERE 1 = 1 " + queryPessoa + @"

                        UNION

                        SELECT VLI_CODIGO Codigo,
                        'Veículo' Identificador,
                        VEI_PLACA Entidade,
                        VLI_DESCRICAO Descricao,
                        VLI_NUMERO NumeroLicenca,
                        VLI_DATA_EMISSAO DataEmissao,
                        VLI_DATA_VENCIMENTO DataVencimento,
                        ISNULL(LL.LIC_DESCRICAO, '') TipoLicenca,
                        V.VEI_NUMERO_FROTA NumeroFrota,
                        V.VEI_ATIVO StatusEntidade 
                        FROM T_VEICULO_LICENCA L
                        JOIN T_VEICULO V ON V.VEI_CODIGO = L.VEI_CODIGO
                        LEFT OUTER JOIN T_LICENCA LL ON LL.LIC_CODIGO = L.LIC_CODIGO
                        WHERE 1 = 1 " + queryVeiculo;

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoOrdenar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.DirecaoOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Administrativo.Licenca)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.Licenca>();
        }

        public int ContarRelatorioLicenca(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string query = "", queryVeiculo = "", queryFuncionario = "", queryMotorista = "", queryPessoa = "", queryNaoMostra = " AND 1 = 0";
            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                queryVeiculo += " AND V.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                queryFuncionario += " AND F.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                queryPessoa += " AND C.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
            }

            if (filtrosPesquisa.StatusLicenca > 0)
            {
                queryPessoa += $" AND L.PLI_STATUS = {filtrosPesquisa.StatusLicenca.Value.ToString("d")}";
                queryFuncionario += $" AND L.MLI_STATUS = {filtrosPesquisa.StatusLicenca.Value.ToString("d")}";
                queryVeiculo += $" AND L.VLI_STATUS = {filtrosPesquisa.StatusLicenca.Value.ToString("d")}";
            }

            if (filtrosPesquisa.CodigoTipoLicenca > 0)
            {
                queryVeiculo += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
                queryFuncionario += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
                queryPessoa += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
            }

            if (filtrosPesquisa.CodigoPessoa > 0)
            {
                queryPessoa += $" AND L.CLI_CGCCPF = {filtrosPesquisa.CodigoPessoa.ToString()}";
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                queryFuncionario += $" AND L.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista.ToString()}";
            }

            if (filtrosPesquisa.CodigoFuncionario > 0)
            {
                queryFuncionario += $" AND L.FUN_CODIGO = {filtrosPesquisa.CodigoFuncionario.ToString()}";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                queryVeiculo += $" AND L.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}";
            }

            if (filtrosPesquisa.CodigoTipoLicenca > 0)
            {
                queryVeiculo += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
                queryFuncionario += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
                queryPessoa += " AND L.LIC_CODIGO = " + filtrosPesquisa.CodigoTipoLicenca;
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Entidade))
            {
                queryVeiculo += " AND VEI_PLACA LIKE '%" + filtrosPesquisa.Entidade + "%'";
                queryFuncionario += " AND FUN_NOME LIKE '%" + filtrosPesquisa.Entidade + "%'";
                queryPessoa += " AND CLI_NOME LIKE '%" + filtrosPesquisa.Entidade + "%'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
            {
                queryVeiculo += " AND VLI_DESCRICAO LIKE '%" + filtrosPesquisa.Descricao + "%'";
                queryFuncionario += " AND MLI_DESCRICAO LIKE '%" + filtrosPesquisa.Descricao + "%'";
                queryPessoa += " AND PLI_DESCRICAO LIKE '%" + filtrosPesquisa.Descricao + "%'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroLicenca))
            {
                queryVeiculo += " AND VLI_NUMERO LIKE '%" + filtrosPesquisa.NumeroLicenca + "%'";
                queryFuncionario += " AND MLI_NUMERO LIKE '%" + filtrosPesquisa.NumeroLicenca + "%'";
                queryPessoa += " AND PLI_NUMERO LIKE '%" + filtrosPesquisa.NumeroLicenca + "%'";
            }

            string datePattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                queryVeiculo += " AND CAST(VLI_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
                queryFuncionario += " AND CAST(MLI_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
                queryPessoa += " AND CAST(PLI_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                queryVeiculo += " AND CAST(VLI_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
                queryFuncionario += " AND CAST(MLI_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
                queryPessoa += " AND CAST(PLI_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
            }

            queryMotorista = queryFuncionario;

            if (filtrosPesquisa.TipoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca.Veiculo)
            {
                queryFuncionario = queryNaoMostra;
                queryPessoa = queryNaoMostra;
                queryMotorista = queryNaoMostra;
            }
            else if (filtrosPesquisa.TipoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca.Pessoa)
            {
                queryFuncionario = queryNaoMostra;
                queryVeiculo = queryNaoMostra;
                queryMotorista = queryNaoMostra;
            }
            else if (filtrosPesquisa.TipoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca.Funcionario)
            {
                queryPessoa = queryNaoMostra;
                queryVeiculo = queryNaoMostra;
                queryMotorista = queryNaoMostra;
            }
            else if (filtrosPesquisa.TipoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTelaLicenca.Motorista)
            {
                queryPessoa = queryNaoMostra;
                queryVeiculo = queryNaoMostra;
                queryFuncionario = queryNaoMostra;
            }

            if (filtrosPesquisa.StatusEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
            {
                queryFuncionario += " AND F.FUN_STATUS = 'A' ";
                queryMotorista += " AND F.FUN_STATUS = 'A' ";
                queryPessoa += " AND CLI_ATIVO = 1 ";
                queryVeiculo += " AND VEI_ATIVO = 1 ";
            }
            else if (filtrosPesquisa.StatusEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
            {
                queryFuncionario += " AND F.FUN_STATUS = 'I' ";
                queryMotorista += " AND F.FUN_STATUS = 'I' ";
                queryPessoa += " AND C.CLI_ATIVO = 0 ";
                queryVeiculo += " AND V.VEI_ATIVO = 0 ";
            }

            query = @"  SELECT COUNT(0) as CONTADOR FROM(

                            SELECT MLI_CODIGO Codigo
                            FROM T_MOTORISTA_LICENCA L
                            JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = L.FUN_CODIGO
                            WHERE FUN_TIPO = 'U' " + queryFuncionario + @"
                            UNION
                            SELECT MLI_CODIGO Codigo
                            FROM T_MOTORISTA_LICENCA L
                            JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = L.FUN_CODIGO
                            WHERE FUN_TIPO = 'M' " + queryMotorista + @"
                            UNION
                            SELECT PLI_CODIGO Codigo
                            FROM T_PESSOA_LICENCA L
                            JOIN T_CLIENTE C ON C.CLI_CGCCPF = L.CLI_CGCCPF
                            WHERE 1 = 1 " + queryPessoa + @"
                            UNION
                            SELECT VLI_CODIGO Codigo
                            FROM T_VEICULO_LICENCA L
                            JOIN T_VEICULO V ON V.VEI_CODIGO = L.VEI_CODIGO
                            WHERE 1 = 1 " + queryVeiculo + ") AS T";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LicencaVeiculo> RelatorioLicencaVeiculo(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaLicencaVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Administrativo.LicencaVeiculo)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LicencaVeiculo>();
        }

        public int ContarRelatorioLicencaVeiculo(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaLicencaVeiculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
