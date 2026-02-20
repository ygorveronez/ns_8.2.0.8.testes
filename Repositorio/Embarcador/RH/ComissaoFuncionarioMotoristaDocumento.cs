using Dominio.ObjetosDeValor.Embarcador.RH;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using NHibernate.Linq;
using Repositorio.Embarcador.ICMS;
using Repositorio.Embarcador.RH.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Zen.Barcode;

namespace Repositorio.Embarcador.RH
{
    public class ComissaoFuncionarioMotoristaDocumento : RepositorioBase<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>
    {
        public ComissaoFuncionarioMotoristaDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemCargaDuplicada(int codigoCarga, int codigoComissaoFuncionarioMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>();
            query = query.Where(c => c.ComissaoFuncionarioMotorista.Codigo == codigoComissaoFuncionarioMotorista && c.Carga.Codigo == codigoCarga);

            return query?.Any() ?? false;
        }

        public bool ContemCargaEmOutraComissao(int codigoCarga, int codigoComissaoFuncionario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>();
            query = query.Where(c => c.ComissaoFuncionarioMotorista.ComissaoFuncionario.Codigo != codigoComissaoFuncionario && c.Carga.Codigo == codigoCarga && c.ComissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada);

            return query?.Any() ?? false;
        }

        public decimal TotalExecucaoPorCarga(int codigoCarga, int codigoComissaoFuncionarioMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>();
            query = query.Where(c => c.ComissaoFuncionarioMotorista.Codigo != codigoComissaoFuncionarioMotorista && c.Carga.Codigo == codigoCarga && c.ComissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada);
            return query?.Sum(c => (decimal?)c.PercentualExecucao) ?? 0m;
        }
        public decimal TotalExecucaoPorOcorrencia(int codigoOcorrencia, int codigoComissaoFuncionarioMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>();
            query = query.Where(c => c.ComissaoFuncionarioMotorista.Codigo != codigoComissaoFuncionarioMotorista && c.CargaOcorrencia.Codigo == codigoOcorrencia && c.ComissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada);
            return query?.Sum(c => (decimal?)c.PercentualExecucao) ?? 0m;
        }

        public IQueryable<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento> CriarQueryFiltro(FiltroPesquisaDocumenotosComissaoFuncionario FiltroPesquisa) {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>();
            if (FiltroPesquisa.codigoComissaoMotorista > 0) {
                query = query.Where(q => q.ComissaoFuncionarioMotorista.Codigo == FiltroPesquisa.codigoComissaoMotorista);
            }
            if (FiltroPesquisa.Motorista != null && FiltroPesquisa.Motorista.Count > 0)
            {
                query = query.Where(q => FiltroPesquisa.Motorista.Contains(q.ComissaoFuncionarioMotorista.Motorista.Codigo));
            }

            if (FiltroPesquisa.DataInicial != DateTime.MinValue) {
                query = query.Where(q => q.ComissaoFuncionarioMotorista.ComissaoFuncionario.DataInicio >= FiltroPesquisa.DataInicial);
            }

            if (FiltroPesquisa.DataFinal != DateTime.MinValue) {
                query = query.Where(q => q.ComissaoFuncionarioMotorista.ComissaoFuncionario.DataFim <= FiltroPesquisa.DataFinal);
            }

            return query;
        
        }

        public List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento> Consultar(FiltroPesquisaDocumenotosComissaoFuncionario filtroPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {

            var result = CriarQueryFiltro(filtroPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(FiltroPesquisaDocumenotosComissaoFuncionario filtroPesquisa)
        {
            var result = CriarQueryFiltro(filtroPesquisa);
            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionario> ConsultarRelatorioComissaoFuncionario(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Consulta.ConsultaComissaoFuncionario().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionario)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionario>();


            //var result = CriarQueryFiltro(filtrosPesquisa);
            //Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista repositorioConfiguracaoComissaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista(UnitOfWork);
            //Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista configuracaoComissaoMotorista = repositorioConfiguracaoComissaoMotorista.BuscarConfiguracaoPadrao();

            //List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento> listaComissaoFuncionarioMotoristaDocumento = 
            //    result
            //    .Fetch(o=>o.ComissaoFuncionarioMotorista).ThenFetch(obj => obj.Motorista)
            //    .Fetch(o=>o.CargaDadosSumarizados)                
            //    .Fetch(o=>o.ComissaoFuncionarioMotorista).ThenFetch(obj => obj.ComissaoFuncionario)          
            //    .Fetch(o=>o.Carga).ThenFetchMany(o => o.CargaCTes).ThenFetch(o=>o.CTe)
            //    .Fetch(o=>o.Carga).ThenFetchMany(o => o.CargaCTes).ThenFetch(o=>o.PreCTe)
            //    .Fetch(o=>o.CargaOcorrencia).ThenFetch(o=> o.Carga)
            //    .ToList();
            //return (from p in listaComissaoFuncionarioMotoristaDocumento
            // select new
            // Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionario {
            //     Numero = p.Numero,
            //     DataCarregaementoEmissao = configuracaoComissaoMotorista.DataBase == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseComissaoMotorista.DataEmissao ? p.CurrentCarga?.CargaCTes.Count > 0 ? p.CurrentCarga?.CargaCTes[0].CTe?.DataEmissao?.ToString("dd/MM/yyy") ?? "" : "" : p.CurrentCarga?.DataCarregamentoCarga?.ToString("dd/MM/yyy") ?? "",
            //     Documentos = String.Join(",", (from cte in p.CurrentCarga.CargaCTes select !string.IsNullOrWhiteSpace(cte.CTe.NumeroCRT) ? cte.CTe.NumeroCRT : cte.CTe.Numero.ToString()).ToArray<string>()),
            //     Motorista = p.ComissaoFuncionarioMotorista.Motorista.Nome,
            //     CPF = p.ComissaoFuncionarioMotorista.Motorista.CPF_CNPJ_Formatado,
            //     Origem = p.CargaDadosSumarizados != null ? p.CargaDadosSumarizados.Origens : "",
            //     Destino = p.CargaDadosSumarizados != null ? p.CargaDadosSumarizados.Destinos : "",
            //     ValoFreteLiquido = p.ValoFreteLiquido,
            //     ValorComissao = p.ValorComissao,
            //     Periodo = p.ComissaoFuncionarioMotorista.ComissaoFuncionario.DataInicio.ToString("dd/MM/yyy") + " - "+ p.ComissaoFuncionarioMotorista.ComissaoFuncionario.DataFim.ToString("dd/MM/yyy"),
            // }).ToList();
        }

        public int ContarConsultaRelatorioComissaoFuncionario(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Consulta.ConsultaComissaoFuncionario().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ConsultarSemComissaoMotorista(int numeroOcorrencia, int codigoComissaoFuncionario, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada && obj.ValorOcorrenciaLiquida > 0 select obj;

            if (numeroOcorrencia > 0)
                result = result.Where(obj => obj.NumeroOcorrencia == numeroOcorrencia);

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>();
            var resultAcerto = from obj in queryAcerto where obj.CargaOcorrencia != null && obj.ComissaoFuncionarioMotorista.Codigo == codigoComissaoFuncionario select obj;

            result = result.Where(obj => !resultAcerto.Select(a => a.CargaOcorrencia).Contains(obj));

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Distinct().ToList();
        }

        public int ContarConsultaSemComissaoMotorista(int numeroOcorrencia, int codigoComissaoFuncionario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada && obj.ValorOcorrenciaLiquida > 0 select obj;

            if (numeroOcorrencia > 0)
                result = result.Where(obj => obj.NumeroOcorrencia == numeroOcorrencia);

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>();
            var resultAcerto = from obj in queryAcerto where obj.CargaOcorrencia != null && obj.ComissaoFuncionarioMotorista.Codigo == codigoComissaoFuncionario select obj;

            result = result.Where(obj => !resultAcerto.Select(a => a.CargaOcorrencia).Contains(obj));
            return result.Distinct().Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista> ConsultaRelatorio(int codigoComissao, int comissaoFuncionario)
        {
            string sqlQuery = @"select 
                                ComissaoFuncionarioMotorista.CFM_CODIGO CodigoComissaoFuncionarioMotorista,
                                ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_BASE_CALCULO BaseCalculo,
                                Motorista.FUN_CPF CPF,
                                isnull(Motorista.FUN_CODIGO_INTEGRACAO, '') CodigoIntegracao,
                                convert(varchar(10), ComissaoFuncionarioMotoristaDocumento.CFM_DATA_EMISSAO, 103) DataEmissao,
                                CargaDadosSumarizados.CDS_UF_DESTINOS + ' - ' + CargaDadosSumarizados.CDS_DESTINATARIOS Destinatario,
                                ComissaoFuncionarioMotorista.CFM_NUMERO_DIAS_EM_VIAGEM DiasEmViagem,
                                ((select vei.VEI_NUMERO_FROTA from T_VEICULO vei where vei.VEI_CODIGO = Carga.CAR_VEICULO) +ISNULL((SELECT ', ' + veiculo1.VEI_NUMERO_FROTA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')) Frota,
                                Motorista.FUN_NOME + ' (' + Motorista.FUN_CPF + ')'  Funcionario,
                                ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_ICMS ICMS,
                                CASE ComissaoFuncionarioMotoristaDocumento.CFM_TIPO_DOCUMENTO_COMISSAO WHEN 1 THEN 'Carga' WHEN 2 THEN 'Ocorrência' ELSE 'Nenhum' END Modelo,
                                ComissaoFuncionarioMotoristaDocumento.CFM_NUMERO Numero,
                                ComissaoFuncionarioMotoristaDocumento.CMD_OUTROS_VALORES Outros,
                                ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_PEDAGIO Pedagio,
                                ComissaoFuncionarioMotorista.CFM_PERCENTUAL_COMISSAO PercentualComissao,
                                CargaDadosSumarizados.CDS_UF_ORIGENS + ' - ' + CargaDadosSumarizados.CDS_REMETENTES Remetente,
                                ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_TOTAL_FRETE TotalFrete,
                                CargaDadosSumarizados.CDS_UF_DESTINOS UFDestino,
                                CargaDadosSumarizados.CDS_UF_ORIGENS UFOrigem,
                                ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_COMISSAO ValorComissao,
                                ComissaoFuncionario.CMF_PERCENTUAL_BASE_CALCULO_COMISSAO PercentualBaseCalculo,
                                ComissaoFuncionario.CMF_VALOR_DIARIA ValorDiaria,
                                ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_FRETE_LIQUIDO ValorFreteLiquido,
                                ISNULL(Valores.TPV_VALOR, 0) ValorProdutividade,
								CASE WHEN ComissaoFuncionarioMotorista.CFM_ATINGIU_MEDIA = 1 THEN (((ISNULL(ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_FRETE_LIQUIDO, 0) * (100 + COALESCE(ComissaoFuncionarioMotorista.CFM_PERCENTUAL_ATINGIR_MEDIA,1.5))) / 100) - ISNULL(ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_FRETE_LIQUIDO, 0)) ELSE 0 END ValorFreteLiquidoComMeta,
                                CASE WHEN ComissaoFuncionarioMotorista.CFM_ATINGIU_MEDIA = 1 then 'Sim' else 'Não' END AtingiuMedia,
								CASE WHEN ComissaoFuncionarioMotorista.CFM_NAO_HOUVE_SINISTRO = 1 then 'Não' else 'Sim' END NaoHouveSinitro,
								CASE WHEN ComissaoFuncionarioMotorista.CFM_NAO_HOUVE_ADVERTENCIA = 1 then 'Não' else 'Sim' END NaoHouveAdvertencia,
								CASE WHEN ComissaoFuncionarioMotorista.CFM_CONTEM_DESLOCAMENTO_VAZIO = 1 then 'Sim' else 'Não' END ContemDeslocamentoVazio,
								ComissaoFuncionarioMotorista.CFM_FATURAMENTO_MINIMO FaturamentoMinimo,
								Cargo.CRG_DESCRICAO CargoMotorista,
								ISNULL(ComissaoFuncionarioMotorista.CFM_VALOR_BONIFICACAO, 0) ValorBonificacao,
								ComissaoFuncionarioMotorista.CFM_VALOR_BASE_CALCULO ValorBaseComissao,
								ComissaoFuncionarioMotorista.CFM_PERCENTUAL_MEDIA PercentualMedia,
								ComissaoFuncionarioMotorista.CFM_PERCENTUAL_SINISTRO PercentualSinistro,
								ISNULL(ComissaoFuncionarioMotorista.CFM_VALOR_COMISSAO, 0) PercentualAdvertencia,
								ComissaoFuncionarioMotorista.CFM_KM_FINAL KMFinal,
								ComissaoFuncionarioMotorista.CFM_KM_INICIAL KMInicial,
								ComissaoFuncionarioMotorista.CFM_KM_TOTAL KMTotal,
								ComissaoFuncionarioMotorista.CFM_LITROS_TOTAL_ABASTECIMENTO LitrosTotalAbastecimento,
								ComissaoFuncionarioMotorista.CFM_MEDIA_FINAL MediaFinal,
								ComissaoFuncionarioMotorista.CFM_MEDIA_IDEAL MediaIdeal,
                                ComissaoFuncionarioMotoristaDocumento.CMD_VALOR_COMISSAO ValorComissaoDocumento,
                                ComissaoFuncionarioMotoristaDocumento.CMD_PERCENTUAL_COMISSAO_PARCIAL PercentualComissaoDocumento,
                                ComissaoFuncionarioMotoristaDocumento.CMD_PESO_CARGA PesoCarga,
                                ComissaoFuncionarioMotoristaDocumento.CMD_MEDIA_IDEAL MediaIdealCarga,
                                CentroResultado.CRE_DESCRICAO CentroResultadoDescricao       
                                from T_COMISSAO_FUNCIONARIO ComissaoFuncionario
                                inner join T_COMISSAO_FUNCIONARIO_MOTORISTA ComissaoFuncionarioMotorista on ComissaoFuncionario.CMF_CODIGO = ComissaoFuncionarioMotorista.CMF_CODIGO
                                inner join T_FUNCIONARIO Motorista on ComissaoFuncionarioMotorista.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO
                                left outer join T_CARGO_PESSOA Cargo on Cargo.CRG_CODIGO = Motorista.CRG_CODIGO
                                left outer join T_COMISSAO_FUNCIONARIO_MOTORISTA_DOCUMENTO ComissaoFuncionarioMotoristaDocumento on ComissaoFuncionarioMotorista.CFM_CODIGO = ComissaoFuncionarioMotoristaDocumento.CFM_CODIGO
                                left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = ComissaoFuncionarioMotoristaDocumento.CDS_CODIGO
                                left outer join T_CARGA Carga on Carga.CAR_CODIGO = ComissaoFuncionarioMotoristaDocumento.CAR_CODIGO
                                left outer join T_TABELA_PRODUTIVIDADE_VALORES Valores on Valores.TPV_CODIGO = ComissaoFuncionarioMotorista.TPV_CODIGO
                                left outer join T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = Motorista.CRE_CODIGO
                                where ComissaoFuncionarioMotorista.CFM_GERAR_COMISSAO = 1 and ComissaoFuncionario.CMF_CODIGO = " + codigoComissao.ToString();

            if (comissaoFuncionario > 0)
                sqlQuery += " and ComissaoFuncionarioMotorista.CFM_CODIGO = " + comissaoFuncionario.ToString();

            sqlQuery += " ORDER BY ComissaoFuncionarioMotoristaDocumento.CFM_DATA_EMISSAO";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista>();

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento> ConsultaRelatorioAbastecimento(int codigoComissao, int comissaoFuncionario)
        {
            string sqlQuery = @"select ComissaoFuncionarioMotorista.CFM_CODIGO CodigoComissaoFuncionarioMotorista,
                                Veiculo.VEI_PLACA Veiculo,
                                Posto.CLI_NOME Posto,
                                Abastecimento.ABA_DATA DataAbatecimento,
                                Abastecimento.ABA_KM KM,
                                ISNULL((SELECT TOP(1) ISNULL(AV.ACV_NUMERO, 0) 
                                FROM T_ACERTO_ABASTECIMENTO BV 
                                JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO AND AV.ACV_SITUACAO <> 3
                                WHERE BV.ABA_CODIGO = Abastecimento.ABA_CODIGO), 0)  Acerto,
                                Abastecimento.ABA_LITROS Litros,
                                Abastecimento.ABA_TIPO TipoAbastecimento,
                                Abastecimento.ABA_SITUACAO Situacao,
                                ((Abastecimento.ABA_KM - CASE
                                WHEN Combustivel.PRO_DESCRICAO LIKE '%ARLA%' THEN
	                                ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE Abastecimento.ABA_KM > AA.ABA_KM AND Abastecimento.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), Abastecimento.ABA_KM)
                                ELSE
	                                ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE Abastecimento.ABA_KM > AA.ABA_KM AND Abastecimento.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), Abastecimento.ABA_KM)
                                END) / CASE WHEN Abastecimento.ABA_LITROS <= 0 THEN 1 ELSE Abastecimento.ABA_LITROS END) Media, 
                                Abastecimento.ABA_VALOR_UN * Abastecimento.ABA_LITROS ValorTotal
                                from T_COMISSAO_FUNCIONARIO ComissaoFuncionario
                                inner join T_COMISSAO_FUNCIONARIO_MOTORISTA ComissaoFuncionarioMotorista on ComissaoFuncionario.CMF_CODIGO = ComissaoFuncionarioMotorista.CMF_CODIGO
                                join T_COMISSAO_FUNCIONARIO_MOTORISTA_ABASTECIMENTO ComissaoFuncionarioMotoristaAbastecimento on ComissaoFuncionarioMotoristaAbastecimento.CFM_CODIGO = ComissaoFuncionarioMotorista.CFM_CODIGO
                                join T_ABASTECIMENTO Abastecimento on Abastecimento.ABA_CODIGO = ComissaoFuncionarioMotoristaAbastecimento.ABA_CODIGO
                                left outer join T_PRODUTO Combustivel on Combustivel.PRO_CODIGO = Abastecimento.PRO_CODIGO
                                left outer join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Abastecimento.VEI_CODIGO
                                left outer join T_CLIENTE Posto on Posto.CLI_CGCCPF = Abastecimento.CLI_CGCCPF
                                where ComissaoFuncionarioMotorista.CFM_GERAR_COMISSAO = 1 and ComissaoFuncionario.CMF_CODIGO = " + codigoComissao.ToString();

            if (comissaoFuncionario > 0)
                sqlQuery += " and ComissaoFuncionarioMotorista.CFM_CODIGO = " + comissaoFuncionario.ToString();

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento>();

        }

    }
}
