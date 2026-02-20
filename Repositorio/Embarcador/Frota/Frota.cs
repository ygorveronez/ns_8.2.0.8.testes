using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class Frota : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Frota>
    {
        public Frota(UnitOfWork unitOfWork) : base(unitOfWork) { }


        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Frota.Frota BuscarPorVeiculoTracao(int codigoVeiculoTracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.Frota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Frota>();
            query = query.Where(o => o.Veiculo.Codigo == codigoVeiculoTracao && o.Ativo && ((o.VigenciaFim.HasValue && o.VigenciaFim <= DateTime.Now) || !o.VigenciaFim.HasValue));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.Frota BuscarPorVeiculoReboque(int codigoVeiculoReboque)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.Frota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Frota>();
            query = query.Where(o => (o.Reboque1.Codigo == codigoVeiculoReboque || o.Reboque2.Codigo == codigoVeiculoReboque) && o.Ativo && ((o.VigenciaFim.HasValue && o.VigenciaFim <= DateTime.Now) || !o.VigenciaFim.HasValue));

            return query.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Frota.Frota> BuscarListaPorVeiculoReboqueSemTracao(int codigoVeiculoReboque)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.Frota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Frota>();
            query = query.Where(o => (o.Reboque1.Codigo == codigoVeiculoReboque || o.Reboque2.Codigo == codigoVeiculoReboque) && o.Ativo && o.Veiculo == null && ((o.VigenciaFim.HasValue && o.VigenciaFim <= DateTime.Now) || !o.VigenciaFim.HasValue));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.Frota> BuscarPorMotorista(int codigoMotorista)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.Frota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Frota>();
            query = query.Where(o => (o.Motorista.Codigo == codigoMotorista || o.MotoristaAuxiliar.Codigo == codigoMotorista) && o.Ativo && ((o.VigenciaFim.HasValue && o.VigenciaFim <= DateTime.Now) || !o.VigenciaFim.HasValue));

            return query.ToList();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Frota.PlanejamentoFrota> ConsultarPlanejamentoFrota(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrota filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string filtro = MontarPesquisaViewPlanejamentoFrota(filtrosPesquisa);
            string dataConsulta = filtrosPesquisa.DataConsultaVigencia.ToString("yyyy-MM-dd");

            var sql = "";
            if (filtrosPesquisa.longitudeOrigem != 0 && filtrosPesquisa.latitudeOrigem != 0)
            {
                sql = $@"
                            drop table if exists #TempUltimaProgramacaoCarga;

                            Select	 max(Frota.FRT_CODIGO) CodigoFrota
                            ,max(FrotaCarga.FRC_DATA_PREVISTA_FIM_VIAGEM) DataFimViagem
                            ,max(LocalidadefimVeiculo.POA_LONGITUDE) LongitudeCidade
                            ,max(LocalidadefimVeiculo.POA_LATITUDE) LatitudeCidade

                            into #TempUltimaProgramacaoCarga

                            FROM T_FROTA Frota
                            LEFT JOIN T_FROTA_CARGA as FrotaCarga on Frota.FRT_CODIGO = FrotaCarga.FRT_CODIGO and Frota.FRT_ATIVO = 1
                            LEFT JOIN T_VEICULO as Veiculo on Veiculo.VEI_CODIGO = Frota.VEI_CODIGO_TRACAO AND Veiculo.VEI_ATIVO = 1
                            LEFT JOIN T_POSICAO_ATUAL as LocalidadefimVeiculo on Veiculo.VEI_CODIGO = LocalidadefimVeiculo.VEI_CODIGO
                            WHERE (FrotaCarga.FRC_DATA_PREVISTA_FIM_VIAGEM is null or FrotaCarga.FRC_DATA_PREVISTA_FIM_VIAGEM <= '{dataConsulta}' or FrotaCarga.FRT_CODIGO = {filtrosPesquisa.CodigoFrota}) 
                            group by Frota.FRT_CODIGO
                               
                            select   viw.[VeiculoTracao]
		                            ,viw.[VeiculoReboque1]
		                            ,viw.[VeiculoReboque2]
		                            ,viw.[MotoristaPrincipal]
		                            ,viw.[MotoristaAuxiliar]
		                            ,viw.[DataInicioVigencia]
		                            ,viw.[DataFimVigencia]
		                            ,viw.[Ativo]
		                            ,viw.[CodigoFrota]
		                            ,viw.[LocalVeiculoFrota]
		                            ,viw.[LatitudeFrota]
		                            ,viw.[LongitudeFrota]
		                            ,viw.[NomeMotoristaPrincipal]
		                            ,viw.[NomeMotoristaAuxiliar]
		                            ,viw.[ModeloVeicularVeiculoTracao]
                                    ,viw.[ModeloVeicularReboque1]
		                            ,viw.[ModeloVeicularReboque2]
		                            ,viw.[PlacaVeiculoTracao]
		                            ,viw.[PlacaVeiculoReboque1]
		                            ,viw.[PlacaVeiculoReboque2]
                                    ,viw.[DescricaoLocalVeiculoFrota]                      
                                    ,viw.[UFDescricaoLocalVeiculoFrota]                             
                                    ,viw.[PaisLocalVeiculoFrota]                                    
		                            ,viw.[DescricaoLocalVeiculoFrotaReboque1]                           
                                    ,viw.[UFDescricaoLocalVeiculoFrotaReboque1]                         
                                    ,viw.[PaisLocalVeiculoFrotaReboque1]                                
		                            ,viw.[DescricaoLocalVeiculoFrotaReboque2]                           
                                    ,viw.[UFDescricaoLocalVeiculoFrotaReboque2]                         
                                    ,viw.[PaisLocalVeiculoFrotaReboque2]
		                            ,viw.[DadosPlanejamento]
		                            ,viw.[DataManutencaoTracao]
		                            ,viw.[DataManutencaoReboque1]
		                            ,viw.[DataManutencaoReboque2]
                                    ,viw.[DataPosicao]
		                            ,viw.[PosicaoAtual]
                                    ,CAST(temp.[LatitudeCidade] AS float) [LatitudeCidade]
                                    ,CAST(temp.[LongitudeCidade] AS float) [LongitudeCidade]
		                            ,CAST(dbo.fncCalcula_Distancia_Coordenada(temp.LatitudeCidade,temp.LongitudeCidade, { filtrosPesquisa.latitudeOrigem.ToString().Replace(",", ".")  } , { filtrosPesquisa.longitudeOrigem.ToString().Replace(",", ".")}) AS float) Distancia
    	                            from VIEW_PLANEJAMENTO_FROTA viw
                                    inner join #TempUltimaProgramacaoCarga temp on temp.CodigoFrota = viw.CodigoFrota  
                                    WHERE viw.Ativo = 1 and temp.LatitudeCidade is not null and temp.LongitudeCidade is not null  ";

                if (parametrosConsulta != null)
                {
                    sql = sql + $"  order by Distancia asc, temp.DataFimViagem desc ";
                    if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                        sql = sql + $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
                }
            }
            else
            {
                sql = @"select [VeiculoTracao]
                              ,[VeiculoReboque1]
                              ,[VeiculoReboque2]
                              ,[MotoristaPrincipal]
                              ,[MotoristaAuxiliar]
                              ,[DataInicioVigencia]
                              ,[DataFimVigencia]
                              ,[Ativo]
                              ,[CodigoFrota]
                              ,[LocalVeiculoFrota]
                              ,[LatitudeFrota]
                              ,[LongitudeFrota]
                              ,[NomeMotoristaPrincipal]
                              ,[NomeMotoristaAuxiliar]
                              ,[ModeloVeicularVeiculoTracao]
                              ,[ModeloVeicularReboque1]
		                      ,[ModeloVeicularReboque2]
                              ,[PlacaVeiculoTracao]
                              ,[PlacaVeiculoReboque1]
                              ,[PlacaVeiculoReboque2]
                              ,[DescricaoLocalVeiculoFrota]                               
                              ,[UFDescricaoLocalVeiculoFrota]                             
                              ,[PaisLocalVeiculoFrota]                                    
		                      ,[DescricaoLocalVeiculoFrotaReboque1]                           
                              ,[UFDescricaoLocalVeiculoFrotaReboque1]                         
                              ,[PaisLocalVeiculoFrotaReboque1]                                
		                      ,[DescricaoLocalVeiculoFrotaReboque2]                           
                              ,[UFDescricaoLocalVeiculoFrotaReboque2]                         
                              ,[PaisLocalVeiculoFrotaReboque2]                                
                              ,[DadosPlanejamento]
                              ,[DataManutencaoTracao]
							  ,[DataManutencaoReboque1]
							  ,[DataManutencaoReboque2]
                              ,[DataPosicao]
		                      ,[PosicaoAtual]
                              ,CAST(0 AS float) [LatitudeCidade]
                              ,CAST(0 AS float) [LongitudeCidade]
                              ,CAST(0 AS float) [Distancia]          
                               from VIEW_PLANEJAMENTO_FROTA ";
                sql += !string.IsNullOrWhiteSpace(filtro) ? " WHERE Ativo = 1 " + filtro : " WHERE Ativo = 1 ";

                if (parametrosConsulta != null)
                {
                    sql = sql + $"  order by PlacaVeiculoTracao asc";
                    if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                        sql = sql + $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
                }

            }

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Frota.PlanejamentoFrota)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Frota.PlanejamentoFrota>();
        }

        public int ContarConsultarPlanejamentoFrota(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrota filtrosPesquisa)
        {
            string filtro = MontarPesquisaViewPlanejamentoFrota(filtrosPesquisa);
            var sql = "";

            string dataConsulta = filtrosPesquisa.DataConsultaVigencia.ToString("yyyy-MM-dd");
            if (filtrosPesquisa.longitudeOrigem != 0 && filtrosPesquisa.latitudeOrigem != 0)
            {
                sql = $@"
                            drop table if exists #TempUltimaProgramacaoCarga;

                            Select	 max(Frota.FRT_CODIGO) CodigoFrota
                            ,max(FrotaCarga.FRC_DATA_PREVISTA_FIM_VIAGEM) DataFimViagem
                            ,max(LocalidadefimVeiculo.POA_LONGITUDE) LongitudeCidade
                            ,max(LocalidadefimVeiculo.POA_LATITUDE) LatitudeCidade

                            into #TempUltimaProgramacaoCarga

                            FROM T_FROTA Frota
                            LEFT JOIN T_FROTA_CARGA as FrotaCarga on Frota.FRT_CODIGO = FrotaCarga.FRT_CODIGO and Frota.FRT_ATIVO = 1
                            LEFT JOIN T_VEICULO as Veiculo on Veiculo.VEI_CODIGO = Frota.VEI_CODIGO_TRACAO
                            LEFT JOIN T_POSICAO_ATUAL as LocalidadefimVeiculo on Veiculo.VEI_CODIGO = LocalidadefimVeiculo.VEI_CODIGO
                            WHERE (FrotaCarga.FRC_DATA_PREVISTA_FIM_VIAGEM is null or FrotaCarga.FRC_DATA_PREVISTA_FIM_VIAGEM <= '{dataConsulta}' or FrotaCarga.FRT_CODIGO = {filtrosPesquisa.CodigoFrota}) 
                            group by Frota.FRT_CODIGO
                               
                            select count(1) from VIEW_PLANEJAMENTO_FROTA viw
                                    inner join #TempUltimaProgramacaoCarga temp on temp.CodigoFrota = viw.CodigoFrota  
                                    where Ativo = 1 and temp.LatitudeCidade is not null and temp.LongitudeCidade is not null  ";
            }
            else
            {
                sql = "select count(1) from VIEW_PLANEJAMENTO_FROTA";
                sql += !string.IsNullOrWhiteSpace(filtro) ? " WHERE Ativo = 1 " + filtro : " WHERE Ativo = 1 ";
            }

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Metodos Privados

        private string MontarPesquisaViewPlanejamentoFrota(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPlanejamentoFrota filtrosPesquisa)
        {
            string WhereGeral = " "; //where direto na view;
            string dateFormat = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoMotorista > 0)
                WhereGeral += $" AND (MotoristaPrincipal = {filtrosPesquisa.CodigoMotorista} OR MotoristaAuxiliar = { filtrosPesquisa.CodigoMotorista}) ";

            if (filtrosPesquisa.CodigoVeiculo > 0)
                WhereGeral += $" AND (VeiculoTracao = { filtrosPesquisa.CodigoVeiculo} OR VeiculoReboque1 = { filtrosPesquisa.CodigoVeiculo} OR VeiculoReboque2 = { filtrosPesquisa.CodigoVeiculo} ) ";

            if (filtrosPesquisa.CodigoOrigem > 0)
            {
                WhereGeral += $@" and CodigoFrota in (select FrotaCarga.FRT_CODIGO from T_FROTA_CARGA FrotaCarga
						  			LEFT JOIN T_CARGA AS Carga on Carga.CAR_CODIGO = FrotaCarga.CAR_CODIGO
									LEFT JOIN T_CARGA_LOCAIS_PRESTACAO AS locaisCarga on Carga.CAR_CODIGO = locaisCarga.CAR_CODIGO
									LEFT JOIN T_LOCALIDADES AS LocalidadeInicio on LocalidadeInicio.LOC_CODIGO = locaisCarga.LOC_INICIO_PRESTACAO
									where LocalidadeInicio.LOC_CODIGO =  { filtrosPesquisa.CodigoDestino} )";
            }

            if (filtrosPesquisa.CodigoDestino > 0)
            {
                WhereGeral += $@" and CodigoFrota in (select FrotaCarga.FRT_CODIGO from T_FROTA_CARGA FrotaCarga
						  			LEFT JOIN T_CARGA AS Carga on Carga.CAR_CODIGO = FrotaCarga.CAR_CODIGO
									LEFT JOIN T_CARGA_LOCAIS_PRESTACAO AS locaisCarga on Carga.CAR_CODIGO = locaisCarga.CAR_CODIGO
									LEFT JOIN T_LOCALIDADES AS Localidadefim on Localidadefim.LOC_CODIGO = locaisCarga.LOC_TERMINO_PRESTACAO
									where Localidadefim.LOC_CODIGO =  { filtrosPesquisa.CodigoDestino} )";
            }


            if (filtrosPesquisa.SituacaoDaFrota.HasValue)
            {
                if (filtrosPesquisa.SituacaoDaFrota.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFrota.Disponivel)
                {
                    WhereGeral += $@" AND CodigoFrota not in (select FrotaCarga.FRT_CODIGO from T_FROTA_CARGA FrotaCarga
						  			LEFT JOIN T_CARGA AS Carga on Carga.CAR_CODIGO = FrotaCarga.CAR_CODIGO
									where CAST(FrotaCarga.FRC_DATA_CARREGAMENTO AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat) }' )";
                }
                else if (filtrosPesquisa.SituacaoDaFrota.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFrota.EmCarregamento)
                {
                    WhereGeral += $@" AND CodigoFrota in (select FrotaCarga.FRT_CODIGO from T_FROTA_CARGA FrotaCarga
						  			LEFT JOIN T_CARGA AS Carga on Carga.CAR_CODIGO = FrotaCarga.CAR_CODIGO
									where CAST(FrotaCarga.FRC_DATA_CARREGAMENTO AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' )";

                }
                else if (filtrosPesquisa.SituacaoDaFrota.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFrota.EmManutencao)
                {
                    WhereGeral += $@" AND ( VeiculoTracao in (select OrdemServico.vei_codigo from T_FROTA_ORDEM_SERVICO OrdemServico
									where OrdemServico.OSE_SITUACAO = 0 AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' )
                                       OR VeiculoReboque1 in (select OrdemServico.vei_codigo from T_FROTA_ORDEM_SERVICO OrdemServico
									where OrdemServico.OSE_SITUACAO = 0 AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE)= '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' )
                                       OR VeiculoReboque2 in (select OrdemServico.vei_codigo from T_FROTA_ORDEM_SERVICO OrdemServico
									where OrdemServico.OSE_SITUACAO = 0 AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' ))";

                }
                else if (filtrosPesquisa.SituacaoDaFrota.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFrota.EmViagem)
                {
                    WhereGeral += $@" AND
							        CodigoFrota in (select FrotaCarga.FRT_CODIGO from T_FROTA_CARGA FrotaCarga
						  			LEFT JOIN T_CARGA AS Carga on Carga.CAR_CODIGO = FrotaCarga.CAR_CODIGO 
                                    LEFT JOIN T_FROTA Frota ON Frota.FRT_CODIGO = FrotaCarga.FRT_CODIGO
									WHERE
									(CAST(Carga.CAR_DATA_INICIO_VIAGEM_PREVISTA AS DATE) <= '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)} ') 
									AND (CAST(Carga.CAR_DATA_FIM_VIAGEM_PREVISTA AS DATE) >= '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)} ')) ";
                }
                else if (filtrosPesquisa.SituacaoDaFrota.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFrota.EmDescarregamento)
                {
                    WhereGeral += $@" AND
							        CodigoFrota in (select FrotaCarga.FRT_CODIGO from T_FROTA_CARGA FrotaCarga
						  			LEFT JOIN T_CARGA AS Carga on Carga.CAR_CODIGO = FrotaCarga.CAR_CODIGO
                                    WHERE
									(CAST(FrotaCarga.FRC_DATA_PREVISTA_FIM_VIAGEM AS DATE) >= '{ filtrosPesquisa.DataConsultaVigencia.AddDays(-1).ToString(dateFormat)}') 
									AND (CAST(FrotaCarga.FRC_DATA_PREVISTA_FIM_VIAGEM AS DATE) <= '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)} ')) ";
                }

            }

            if (filtrosPesquisa.SituacaoDoConjunto.HasValue)
            {
                if (filtrosPesquisa.SituacaoDoConjunto.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDoConjuntoFrota.ConjuntoCompleto)
                    WhereGeral += " AND VeiculoTracao is not null AND VeiculoReboque1 is not null";
                else if (filtrosPesquisa.SituacaoDoConjunto.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDoConjuntoFrota.SemReboque)
                    WhereGeral += "  AND VeiculoTracao is not null AND VeiculoReboque1 is null";
                else if (filtrosPesquisa.SituacaoDoConjunto.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDoConjuntoFrota.SemTracao)
                    WhereGeral += "  AND VeiculoTracao is null ";
            }


            if (filtrosPesquisa.VeiculoNecessitaManutencao)
            {
                WhereGeral += $@" AND ( VeiculoTracao in (select OrdemServico.vei_codigo from T_FROTA_ORDEM_SERVICO OrdemServico
									where OrdemServico.OSE_SITUACAO = 0 AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' )
                                       OR VeiculoReboque1 in (select OrdemServico.vei_codigo from T_FROTA_ORDEM_SERVICO OrdemServico
									where OrdemServico.OSE_SITUACAO = 0 AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' )
                                       OR VeiculoReboque2 in (select OrdemServico.vei_codigo from T_FROTA_ORDEM_SERVICO OrdemServico
									where OrdemServico.OSE_SITUACAO = 0 AND CAST(OrdemServico.OSE_DATA_PROGRAMADA AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' ))";
            }


            if (filtrosPesquisa.VeiculoComCarga)
            {
                WhereGeral += $@" AND VeiculoTracao in (select Frota.VEI_CODIGO_TRACAO from T_FROTA_CARGA FrotaCarga
						  			LEFT JOIN T_CARGA AS Carga on Carga.CAR_CODIGO = FrotaCarga.CAR_CODIGO 		
									LEFT JOIN T_FROTA Frota ON Frota.FRT_CODIGO = FrotaCarga.FRT_CODIGO
									WHERE
								    CAST(Carga.CAR_DATA_CARREGAMENTO AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' OR CAST(FrotaCarga.FRC_DATA_PREVISTA_INICIO_VIAGEM AS DATE) = '{ filtrosPesquisa.DataConsultaVigencia.ToString(dateFormat)}' )";
            }

            if (filtrosPesquisa.CodigoFrota > 0)
                WhereGeral += $" AND CodigoFrota = {filtrosPesquisa.CodigoFrota } ";

            if (filtrosPesquisa.MotoristaNecessitaIrCasa)// implementação posterior
                WhereGeral += "";


            string filtro = WhereGeral;

            return filtro;
        }

        #endregion

    }
}
