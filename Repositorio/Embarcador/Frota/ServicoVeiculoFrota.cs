using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frota
{
    public class ServicoVeiculoFrota : RepositorioBase<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>
    {
        public ServicoVeiculoFrota(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ServicoVeiculoFrota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();

            query = query.Where(o => o.Descricao.Contains(descricao));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> BuscarPorCodigo(int[] codigosServicos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();

            query = query.Where(o => codigosServicos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> BuscarPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> Consultar(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa? ativo, int codigoGrupoServico, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(codigoEmpresa, descricao, ativo, codigoGrupoServico);

            return result.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa? ativo, int codigoGrupoServico)
        {
            var result = Consultar(codigoEmpresa, descricao, ativo, codigoGrupoServico);

            return result.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> BuscarManutencoesObrigatoriasParaCargaPorVeiculo(int codigoVeiculo, bool utilizarValidadePeloGrupoServico)
        {
            string joinGrupoServico = "";
            string whereGrupoServico = "";

            string selectServico = @"   servico.SEV_VALIDADE_KM ValidadeQuilometrosServico, 
                                        servico.SEV_TOLERANCIA_KM ToleranciaQuilometrosServico, 
                                        servico.SEV_VALIDADE_DIAS ValidadeDiasServico, 
                                        servico.SEV_TOLERANCIA_DIAS ToleranciaDiasServico, 
                                        servico.SEV_TIPO TipoServico,
                                        servico.SEV_VALIDADE_HORIMETRO ValidadeHorimetroServico, 
                                        servico.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetroServico,";

            if (utilizarValidadePeloGrupoServico)
            {
                selectServico = @"  GrupoServicoVeiculo.GSV_VALIDADE_KM ValidadeQuilometrosServico, 
                                    GrupoServicoVeiculo.GSV_TOLERANCIA_KM ToleranciaQuilometrosServico, 
                                    GrupoServicoVeiculo.GSV_VALIDADE_DIAS ValidadeDiasServico, 
                                    GrupoServicoVeiculo.GSV_TOLERANCIA_DIAS ToleranciaDiasServico, 
                                    GrupoServicoVeiculo.GSV_TIPO TipoServico,
				                    GrupoServicoVeiculo.GSV_VALIDADE_HORIMETRO ValidadeHorimetroServico,
                                    GrupoServicoVeiculo.GSV_TOLERANCIA_HORIMETRO ToleranciaHorimetroServico,";

                joinGrupoServico = @"join T_GRUPO_SERVICO_SERVICO_VEICULO GrupoServicoVeiculo on GrupoServicoVeiculo.SEV_CODIGO = servico.SEV_CODIGO
                                     join T_GRUPO_SERVICO Grupo on Grupo.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO ";

                whereGrupoServico = @" AND Grupo.GSF_ATIVO = 1 
                                    AND ((Grupo.GSF_KM_FINAL > 0 AND veiculo.VEI_KMATUAL >= Grupo.GSF_KM_INICIAL AND veiculo.VEI_KMATUAL <= Grupo.GSF_KM_FINAL) 
                                        or (veiculo.VEI_DATACOMPRA is not null) or (Grupo.GSF_DIA_FINAL > 0 AND Grupo.GSF_DIA_INICIAL <= DATEDIFF(day, veiculo.VEI_DATACOMPRA, getdate()) AND Grupo.GSF_DIA_FINAL >= DATEDIFF(day, veiculo.VEI_DATACOMPRA, getdate())))
				                    AND (
                                    ((veiculo.VMO_CODIGO IN (SELECT MoEq.VMO_CODIGO FROM T_GRUPO_SERVICO_MODELO_VEICULO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.VMO_CODIGO = veiculo.VMO_CODIGO))
                                        or (Grupo.GSF_POSSUI_MODELOS_VEICULO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 1)))
				                    and ((veiculo.VMA_CODIGO in (SELECT MoEq.VMA_CODIGO FROM T_GRUPO_SERVICO_MARCA_VEICULO MoEq where MoEq.GSF_CODIGO = GrupoServicoVeiculo.GSF_CODIGO and MoEq.VMA_CODIGO = veiculo.VMA_CODIGO))
                                        or (Grupo.GSF_POSSUI_MARCAS_VEICULO = 0 AND (Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 0 OR Grupo.GSF_TIPO_VEICULO_EQUIPAMENTO = 1)))
                                        )";
            }

            string sqlQuery = $@"WITH summary AS (
                                SELECT
                                    TT.CodigoServico,
		                            OSS.CodigoOrdemServico,
		                            OSS.CodigoManutencaoOrdemServico,
		                            OSS.DataUltimaExecucao,
		                            ISNULL(OSS.QuilometragemUltimaExecucao, 0) QuilometragemUltimaExecucao,
		                            ISNULL(OSS.HorimetroUltimaExecucao, 0) HorimetroUltimaExecucao,
		                            TT.QuilometragemAtual,
		                            0 HorimetroAtual,
		                            TT.Placa,
                                    TT.DescricaoServico,
                                    TT.ValidadeQuilometrosServico,
                                    TT.ToleranciaQuilometrosServico,
                                    TT.ValidadeDiasServico,
                                    TT.ToleranciaDiasServico,
                                    TT.TipoServico,
                                    TT.ValidadeHorimetroServico,
                                    TT.ToleranciaHorimetroServico,
		                            ISNULL(OSS.RowNumber, ROW_NUMBER() OVER(PARTITION BY TT.DescricaoServico ORDER BY TT.DescricaoServico desc)) RowNumber
                                FROM
                                (SELECT
			                            veiculo.VEI_CODIGO CodigoVeiculo,
			                            servico.SEV_CODIGO CodigoServico,
                                        servico.SEV_DESCRICAO DescricaoServico,
			                            veiculo.VEI_KMATUAL QuilometragemAtual,
                                        veiculo.VEI_PLACA Placa,
                                        { selectServico }
			                            0 HorimetroAtual
                                    FROM
                                        T_VEICULO veiculo,
                                        T_FROTA_SERVICO_VEICULO servico
                                    { joinGrupoServico }
                                    WHERE servico.SEV_TIPO <> 6 and servico.SEV_OBRIGATORIO_PARA_REALIZAR_CARGA = 1 AND servico.SEV_ATIVO = 1 and (servico.SEV_SERVICO_PARA_EQUIPAMENTO is null OR servico.SEV_SERVICO_PARA_EQUIPAMENTO = 0)
                                    and veiculo.VEI_CODIGO = { codigoVeiculo }
                                    { whereGrupoServico }
                                ) AS TT                    
                                    LEFT OUTER JOIN
                                        (
                                            SELECT
                                                os.OSE_CODIGO CodigoOrdemServico,
                                                manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                                                os.OSE_DATA_PROGRAMADA DataUltimaExecucao,
                                                os.OSE_QUILOMETRAGEM_VEICULO QuilometragemUltimaExecucao,
                                                os.OSE_HORIMETRO HorimetroUltimaExecucao,
                                                manutencao.SEV_CODIGO,
                                                os.VEI_CODIGO,
                                                ROW_NUMBER() OVER(PARTITION 
                                            BY
                                                manutencao.SEV_CODIGO,
                                                os.VEI_CODIGO 
                                            ORDER BY
                                                os.OSE_DATA_PROGRAMADA DESC,
                                                os.OSE_CODIGO DESC) AS RowNumber                                      
                                            FROM
                                                T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao                                      
                                            JOIN
                                                T_FROTA_ORDEM_SERVICO os 
                                                    ON manutencao.OSE_CODIGO = os.OSE_CODIGO                                      
                                            WHERE
                                                (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) 
                                                AND os.VEI_CODIGO = { codigoVeiculo }
                                        ) AS OSS 
                                            ON OSS.SEV_CODIGO = TT.CodigoServico 
                                            AND OSS.VEI_CODIGO = TT.CodigoVeiculo 
                                            AND RowNumber = 1 
		                                )
                             SELECT s.*
                             FROM summary s
                             WHERE s.RowNumber = 1 
                             AND 
                             (((s.TipoServico = 0 OR s.TipoServico = 1) AND ((s.QuilometragemUltimaExecucao + s.ValidadeQuilometrosServico - s.ToleranciaQuilometrosServico) <= s.QuilometragemAtual))
                             OR
                             ((s.TipoServico = 0 OR s.TipoServico = 2) AND ((DATEADD(day, (s.ValidadeDiasServico - s.ToleranciaDiasServico), s.DataUltimaExecucao)) <= GETDATE())))";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> BuscarPendentesPorVeiculo(int codigoVeiculo, int kmAtualOS, int codigoGrupoServico = 0, bool utilizarValidadePeloGrupoServico = false)
        {
            var queryCondicao = "";
            var queryCondicaoServico = "";

            var selectServico = @"   servico.SEV_VALIDADE_KM ValidadeQuilometrosServico, 
                                     servico.SEV_TOLERANCIA_KM ToleranciaQuilometrosServico, 
                                     servico.SEV_VALIDADE_DIAS ValidadeDiasServico, 
                                     servico.SEV_TOLERANCIA_DIAS ToleranciaDiasServico, 
                                     servico.SEV_VALIDADE_HORIMETRO ValidadeHorimetroServico, 
                                     servico.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetroServico,
                                     servico.SEV_TIPO TipoServico,";

            if (codigoGrupoServico > 0)
            {
                var queryValidade = "";
                if (utilizarValidadePeloGrupoServico)
                {
                    selectServico = $@"  (select servicoVeiculoGrupo.GSV_VALIDADE_KM from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ValidadeQuilometrosServico, 
                                     (select servicoVeiculoGrupo.GSV_TOLERANCIA_KM from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ToleranciaQuilometrosServico, 
                                     (select servicoVeiculoGrupo.GSV_VALIDADE_DIAS from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ValidadeDiasServico, 
                                     (select servicoVeiculoGrupo.GSV_TOLERANCIA_DIAS from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ToleranciaDiasServico, 
                                     (select servicoVeiculoGrupo.GSV_VALIDADE_HORIMETRO from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ValidadeHorimetroServico, 
                                     (select servicoVeiculoGrupo.GSV_TOLERANCIA_HORIMETRO from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ToleranciaHorimetroServico,
                                     (select servicoVeiculoGrupo.GSV_TIPO from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) TipoServico,";

                    queryValidade = " and (servicoVeiculoGrupo.GSV_VALIDADE_KM > 0 or servicoVeiculoGrupo.GSV_VALIDADE_DIAS > 0)";
                }

                queryCondicao += $@" and servico.SEV_CODIGO in (
                                        select servicoVeiculoGrupo.SEV_CODIGO from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        join T_GRUPO_SERVICO grupoServico on grupoServico.GSF_CODIGO = servicoVeiculoGrupo.GSF_CODIGO
                                        where grupoServico.GSF_CODIGO = { codigoGrupoServico }
                                        and ((({ kmAtualOS } between grupoServico.GSF_KM_INICIAL and grupoServico.GSF_KM_FINAL) or (grupoServico.GSF_KM_INICIAL = 0 and grupoServico.GSF_KM_FINAL = 0 and grupoServico.GSF_DIA_FINAL > 0))
                                            or ((ISNULL(DATEDIFF(day, veiculo.VEI_DATACOMPRA, getdate()), 0) between grupoServico.GSF_DIA_INICIAL and grupoServico.GSF_DIA_FINAL) 
                                                    or (grupoServico.GSF_DIA_INICIAL = 0 and grupoServico.GSF_DIA_FINAL = 0 and grupoServico.GSF_KM_FINAL > 0)))
                                        { queryValidade }
                                    )";
            }

            if (!utilizarValidadePeloGrupoServico || codigoGrupoServico == 0)
                queryCondicaoServico = " and (servico.SEV_VALIDADE_KM > 0 or servico.SEV_VALIDADE_DIAS > 0)";

            var sqlQuery = $@"WITH summary AS (
                             SELECT servico.SEV_CODIGO CodigoServico, 
                             os.OSE_CODIGO CodigoOrdemServico, 
                             manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                             os.OSE_DATA_PROGRAMADA DataUltimaExecucao, 
                             ISNULL(os.OSE_QUILOMETRAGEM_VEICULO, 0) QuilometragemUltimaExecucao, 
                             ISNULL(os.OSE_HORIMETRO, 0) HorimetroUltimaExecucao, 
                             { kmAtualOS.ToString("D") } QuilometragemAtual, 
                             0 HorimetroAtual, 
                             { selectServico }
                             servico.SEV_TEMPO_ESTIMADO TempoEstimado,
                             ROW_NUMBER() OVER(PARTITION BY servico.SEV_CODIGO ORDER BY os.OSE_DATA_PROGRAMADA DESC, os.OSE_CODIGO DESC) AS RowNumber
                             FROM T_FROTA_SERVICO_VEICULO servico
                             LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao ON servico.SEV_CODIGO = manutencao.SEV_CODIGO 
                             LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO os ON manutencao.OSE_CODIGO = os.OSE_CODIGO AND (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) AND os.VEI_CODIGO = { codigoVeiculo }
                             LEFT OUTER JOIN T_VEICULO veiculo ON veiculo.VEI_CODIGO = os.VEI_CODIGO 
                             WHERE servico.SEV_TIPO <> 6 and servico.SEV_EXECUCAO_UNICA <> 1 AND servico.SEV_ATIVO = 1 and (servico.SEV_SERVICO_PARA_EQUIPAMENTO is null OR servico.SEV_SERVICO_PARA_EQUIPAMENTO = 0)
                             { queryCondicaoServico }
                             { queryCondicao }
                             )
                             SELECT s.*
                             FROM summary s
                             WHERE s.RowNumber = 1 
                             AND 
                             (((s.TipoServico = 0 OR s.TipoServico = 1) AND ((s.QuilometragemUltimaExecucao + s.ValidadeQuilometrosServico - s.ToleranciaQuilometrosServico) <= s.QuilometragemAtual))
                             OR
                             ((s.TipoServico = 0 OR s.TipoServico = 2) AND ((DATEADD(day, (s.ValidadeDiasServico - s.ToleranciaDiasServico), s.DataUltimaExecucao)) <= GETDATE())))";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico> BuscarPendentesPorEquipamento(int codigoEquipamento, int kmAtualOS, int horimetroAtual, int codigoGrupoServico = 0, bool utilizarValidadePeloGrupoServico = false)
        {
            var queryCondicao = "";
            var queryCondicaoServico = "";

            var selectServico = @"   servico.SEV_VALIDADE_KM ValidadeQuilometrosServico, 
                                     servico.SEV_TOLERANCIA_KM ToleranciaQuilometrosServico, 
                                     servico.SEV_VALIDADE_DIAS ValidadeDiasServico, 
                                     servico.SEV_TOLERANCIA_DIAS ToleranciaDiasServico, 
                                     servico.SEV_VALIDADE_HORIMETRO ValidadeHorimetroServico, 
                                     servico.SEV_TOLERANCIA_HORIMETRO ToleranciaHorimetroServico,
                                     servico.SEV_TIPO TipoServico,";

            if (codigoGrupoServico > 0)
            {
                var queryValidade = "";
                if (utilizarValidadePeloGrupoServico)
                {
                    selectServico = $@"  (select servicoVeiculoGrupo.GSV_VALIDADE_KM from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ValidadeQuilometrosServico, 
                                     (select servicoVeiculoGrupo.GSV_TOLERANCIA_KM from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ToleranciaQuilometrosServico, 
                                     (select servicoVeiculoGrupo.GSV_VALIDADE_DIAS from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ValidadeDiasServico, 
                                     (select servicoVeiculoGrupo.GSV_TOLERANCIA_DIAS from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ToleranciaDiasServico, 
                                     (select servicoVeiculoGrupo.GSV_VALIDADE_HORIMETRO from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ValidadeHorimetroServico, 
                                     (select servicoVeiculoGrupo.GSV_TOLERANCIA_HORIMETRO from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) ToleranciaHorimetroServico,
                                     (select servicoVeiculoGrupo.GSV_TIPO from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        where servicoVeiculoGrupo.SEV_CODIGO = servico.SEV_CODIGO and servicoVeiculoGrupo.GSF_CODIGO = { codigoGrupoServico }) TipoServico,";

                    queryValidade = " and (servicoVeiculoGrupo.GSV_VALIDADE_KM > 0 or servicoVeiculoGrupo.GSV_VALIDADE_DIAS > 0 or servicoVeiculoGrupo.GSV_VALIDADE_HORIMETRO >= 0)";
                }

                queryCondicao += $@" and servico.SEV_CODIGO in (
                                        select servicoVeiculoGrupo.SEV_CODIGO from T_GRUPO_SERVICO_SERVICO_VEICULO servicoVeiculoGrupo 
                                        join T_GRUPO_SERVICO grupoServico on grupoServico.GSF_CODIGO = servicoVeiculoGrupo.GSF_CODIGO
                                        where grupoServico.GSF_CODIGO = { codigoGrupoServico }
                                        and ((({ horimetroAtual } between grupoServico.GSF_KM_INICIAL and grupoServico.GSF_KM_FINAL) or (grupoServico.GSF_KM_INICIAL = 0 and grupoServico.GSF_KM_FINAL = 0 and grupoServico.GSF_DIA_FINAL > 0))
                                            or ((ISNULL(DATEDIFF(day, equipamento.EQP_DATA_AQUISICAO, getdate()), 0) between grupoServico.GSF_DIA_INICIAL and grupoServico.GSF_DIA_FINAL) 
                                                    or (grupoServico.GSF_DIA_INICIAL = 0 and grupoServico.GSF_DIA_FINAL = 0 and grupoServico.GSF_KM_FINAL > 0)))
                                        { queryValidade }
                                    )";
            }

            if (!utilizarValidadePeloGrupoServico || codigoGrupoServico == 0)
                queryCondicaoServico = " and (servico.SEV_VALIDADE_KM > 0 or servico.SEV_VALIDADE_DIAS > 0 or servico.SEV_VALIDADE_HORIMETRO >= 0)";

            var sqlQuery = $@"WITH summary AS (
                             SELECT servico.SEV_CODIGO CodigoServico, 
                             os.OSE_CODIGO CodigoOrdemServico, 
                             manutencao.OSS_CODIGO CodigoManutencaoOrdemServico,
                             os.OSE_DATA_PROGRAMADA DataUltimaExecucao,
                             ISNULL(os.OSE_QUILOMETRAGEM_VEICULO, 0) QuilometragemUltimaExecucao,
                             ISNULL(os.OSE_HORIMETRO, 0) HorimetroUltimaExecucao,
                             { kmAtualOS.ToString("D") } QuilometragemAtual, 
                             { horimetroAtual.ToString("D") } HorimetroAtual, 
                             { selectServico }
                             servico.SEV_TEMPO_ESTIMADO TempoEstimado,
                             ROW_NUMBER() OVER(PARTITION BY servico.SEV_CODIGO ORDER BY os.OSE_DATA_PROGRAMADA DESC, os.OSE_CODIGO DESC) AS RowNumber
                             FROM T_FROTA_SERVICO_VEICULO servico 
                             LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO_SERVICO_VEICULO manutencao ON servico.SEV_CODIGO = manutencao.SEV_CODIGO 
                             LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO os ON manutencao.OSE_CODIGO = os.OSE_CODIGO AND (os.OSE_SITUACAO = 5 OR os.OSE_SITUACAO = 7) AND os.EQP_CODIGO = { codigoEquipamento }                                 
                             LEFT OUTER JOIN T_EQUIPAMENTO equipamento ON equipamento.EQP_CODIGO = os.EQP_CODIGO 
                             WHERE servico.SEV_TIPO <> 6 and servico.SEV_EXECUCAO_UNICA <> 1 and servico.SEV_ATIVO = 1 AND servico.SEV_SERVICO_PARA_EQUIPAMENTO = 1
                             { queryCondicaoServico }
                             { queryCondicao }
                             )
                             SELECT s.*
                             FROM summary s
                             WHERE s.RowNumber = 1 
                             AND 
                             (((s.TipoServico = 0 OR s.TipoServico = 1 OR s.TipoServico = 4) AND ((s.QuilometragemUltimaExecucao + s.ValidadeQuilometrosServico - s.ToleranciaQuilometrosServico) <= s.QuilometragemAtual))
                             OR
                             ((s.TipoServico = 3 OR s.TipoServico = 4) AND ((s.HorimetroUltimaExecucao + s.ValidadeHorimetroServico - s.ToleranciaHorimetroServico) <= s.HorimetroAtual))
                             OR
                             ((s.TipoServico = 0 OR s.TipoServico = 2 OR s.TipoServico = 4) AND ((DATEADD(day, (s.ValidadeDiasServico - s.ToleranciaDiasServico), s.DataUltimaExecucao)) <= GETDATE())))";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Frota.UltimaExecucaoServico>();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> BuscarPendentesExecucaoUnicaPorVeiculo(Dominio.Entidades.Veiculo veiculo, int kmAtualVeiculo, int codigoGrupoServico = 0, bool utilizarValidadePeloGrupoServico = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();

            query = query.Where(o => !o.ServicoParaEquipamento && o.ExecucaoUnica && o.Ativo);

            if (!utilizarValidadePeloGrupoServico || codigoGrupoServico == 0)
                query = query.Where(o => (o.ValidadeDias >= 0 || o.ValidadeKM >= 0) && ((o.ValidadeKM - o.ToleranciaKM) <= kmAtualVeiculo));

            if (codigoGrupoServico > 0)
            {
                var queryGrupoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServico>();
                var resultQueryGrupoServico = from obj in queryGrupoServico select obj;

                query = query.Where(o => resultQueryGrupoServico.Where(g => g.Codigo == codigoGrupoServico && g.ServicosVeiculo.Any(s => s.ServicoVeiculoFrota.Codigo == o.Codigo && s.ServicoVeiculoFrota.Ativo)).Any());

                double qtdDias = veiculo.DataCompra.HasValue ? (DateTime.Now.Date - veiculo.DataCompra.Value.Date).TotalDays : 0;
                query = query.Where(o => resultQueryGrupoServico.Where(g => ((kmAtualVeiculo >= g.KmInicial && kmAtualVeiculo <= g.KmFinal) || (g.KmInicial == 0 && g.KmFinal == 0 && g.DiaFinal > 0)) ||
                                                                            ((qtdDias >= g.DiaInicial && qtdDias <= g.DiaFinal) || (g.DiaInicial == 0 && g.DiaFinal == 0 && g.KmFinal > 0))).Any());

                if (utilizarValidadePeloGrupoServico)
                    query = query.Where(o => resultQueryGrupoServico.Where(g => g.ServicosVeiculo.Any(s => (s.ValidadeDias >= 0 || s.ValidadeKM >= 0) && ((s.ValidadeKM - s.ToleranciaKM) <= kmAtualVeiculo))).Any());
            }

            var subQueryServicosExecutados = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            subQueryServicosExecutados = subQueryServicosExecutados.Where(o => o.OrdemServico.Veiculo.Codigo == veiculo.Codigo && (o.OrdemServico.Situacao == SituacaoOrdemServicoFrota.Finalizada || o.OrdemServico.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal));

            query = query.Where(o => !subQueryServicosExecutados.Select(s => s.Servico.Codigo).Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> BuscarPendentesExecucaoUnicaPorEquipamento(Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento, int kmAtual, int horimetroAtual, int codigoGrupoServico = 0, bool utilizarValidadePeloGrupoServico = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();

            query = query.Where(o => o.ServicoParaEquipamento && o.ExecucaoUnica && o.Ativo);

            if (!utilizarValidadePeloGrupoServico || codigoGrupoServico == 0)
                query = query.Where(o => (o.ValidadeDias >= 0 || o.ValidadeKM >= 0 || o.ValidadeHorimetro >= 0) && (((o.ValidadeKM - o.ToleranciaKM) <= kmAtual) || ((o.ValidadeHorimetro - o.ToleranciaHorimetro) <= horimetroAtual)));

            if (codigoGrupoServico > 0)
            {
                var queryGrupoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServico>();
                var resultQueryGrupoServico = from obj in queryGrupoServico select obj;

                query = query.Where(o => resultQueryGrupoServico.Where(g => g.Codigo == codigoGrupoServico && g.ServicosVeiculo.Any(s => s.ServicoVeiculoFrota.Codigo == o.Codigo && s.ServicoVeiculoFrota.Ativo)).Any());

                double qtdDias = equipamento.DataAquisicao.HasValue ? (DateTime.Now.Date - equipamento.DataAquisicao.Value.Date).TotalDays : 0;
                query = query.Where(o => resultQueryGrupoServico.Where(g => ((horimetroAtual >= g.KmInicial && horimetroAtual <= g.KmFinal) || (g.KmInicial == 0 && g.KmFinal == 0 && g.DiaFinal > 0)) ||
                                                                            ((qtdDias >= g.DiaInicial && qtdDias <= g.DiaFinal) || (g.DiaInicial == 0 && g.DiaFinal == 0 && g.KmFinal > 0))).Any());

                if (utilizarValidadePeloGrupoServico)
                    query = query.Where(o => resultQueryGrupoServico.Where(g => g.ServicosVeiculo.Any(s => (s.ValidadeDias >= 0 || s.ValidadeKM >= 0 || s.ValidadeHorimetro >= 0) &&
                    (((s.ValidadeKM - s.ToleranciaKM) <= kmAtual) || ((s.ValidadeHorimetro - s.ToleranciaHorimetro) <= horimetroAtual)))).Any());
            }

            var subQueryServicosExecutados = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo>();

            subQueryServicosExecutados = subQueryServicosExecutados.Where(o => o.OrdemServico.Equipamento.Codigo == equipamento.Codigo && (o.OrdemServico.Situacao == SituacaoOrdemServicoFrota.Finalizada || o.OrdemServico.Situacao == SituacaoOrdemServicoFrota.AgNotaFiscal));

            query = query.Where(o => !subQueryServicosExecutados.Select(s => s.Servico.Codigo).Contains(o.Codigo));

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> Consultar(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa? ativo, int codigoGrupoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (ativo.HasValue)
            {
                if (ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(obj => obj.Ativo);
                else if (ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    result = result.Where(obj => !obj.Ativo);
            }

            if (codigoGrupoServico > 0)
            {
                var queryGrupoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.GrupoServicoServicoVeiculo>();
                var resultQueryGrupoServico = from obj in queryGrupoServico select obj;
                result = result.Where(obj => resultQueryGrupoServico.Where(g => g.GrupoServico.Codigo == codigoGrupoServico && g.ServicoVeiculoFrota.Codigo == obj.Codigo).Any());
            }

            return result;
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo> ConsultarRelatorioServicoVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaServicoVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo>> ConsultarRelatorioServicoVeiculoAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaServicoVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.ServicoVeiculo>();
        }

        public int ContarConsultaRelatorioServicoVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaServicoVeiculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
