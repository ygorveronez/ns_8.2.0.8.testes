using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pedidos
{
    public class ColetaContainer : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>
    {
        #region Construtores

        public ColetaContainer(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Metodos Publicos

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarPorCargaDeColeta(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.CargaDeColeta.Codigo == codigoCarga select obj;

            return result.FirstOrDefault();
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarPorCargaAtual(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.CargaAtual.Codigo == codigoCarga select obj;

            return result.FirstOrDefault();
        }

        public virtual List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer> BuscarPorCargaAgrupadaAtual(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.CargaAtual.CargaAgrupamento.Codigo == codigoCarga select obj;

            return result.ToList();
        }

        public virtual int ContarContainersPesquisa(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer filtroPesquisa)
        {
            var sql = GetSQLSelectConsultaContainersPesquisa(true) + GetSQLWhereConsultaContaineresPesquisa(filtroPesquisa);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarPorCargaAtualComContainer(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.CargaAtual.Codigo == codigoCarga && obj.Container != null select obj;

            return result.FirstOrDefault();
        }

        public virtual IList<Dominio.ObjetosDeValor.Embarcador.Container.ControleContainer> BuscarContainersPesquisa(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string order = @" ORDER BY DataColeta desc ";

            if (parametrosConsulta.LimiteRegistros > 0)
                order += $@"OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH NEXT { parametrosConsulta.LimiteRegistros} ROWS ONLY; ";

            var sql = GetSQLSelectConsultaContainersPesquisa(false) + GetSQLWhereConsultaContaineresPesquisa(filtroPesquisa) + order;


            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Container.ControleContainer)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Container.ControleContainer>();
        }

        //usado na thread diaria para alertar containers excedidos.
        public virtual IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaContainerRetornoCalculoDiaria> BuscarContainersEmPosseExcedido()
        {
            var sql = GetSqlSelectConsultaContainerEmPosseNotificacao();
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaContainerRetornoCalculoDiaria)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaContainerRetornoCalculoDiaria>();
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarPorContainerEmEsperaCarregando(int codigoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.Container.Codigo == codigoContainer && obj.Status == StatusColetaContainer.EmAreaEsperaCarregado select obj;

            return result.FirstOrDefault();
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarAtivoPorContainer(int codigoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.Container.Codigo == codigoContainer && obj.Status != StatusColetaContainer.EmbarcadoNavio && obj.Status != StatusColetaContainer.Cancelado && obj.Status != StatusColetaContainer.Porto select obj;

            return result.FirstOrDefault();
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarPorContainerNaoEmbarcadoENaoCancelado(int codigoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.Container.Codigo == codigoContainer && obj.Status != StatusColetaContainer.EmbarcadoNavio && obj.Status != StatusColetaContainer.Cancelado select obj;

            return result.FirstOrDefault();
        }

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarPorLocalAtualEStatusEContainer(double CodigoLocalAtual, StatusColetaContainer status, int codigoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.Container.Codigo == codigoContainer && obj.LocalAtual.CPF_CNPJ == CodigoLocalAtual && obj.Status == status select obj;

            return result.FirstOrDefault();
        }

        public int DeletarPorCodigo(int codigo)
        {
            if (codigo <= 0)
                return 0;

            string sql = $" DELETE FROM T_COLETA_CONTAINER_CARGA_ENTREGA WHERE CCR_CODIGO = {codigo}; " + // SQL-INJECTION-SAFE
                        $" DELETE FROM T_COLETA_CONTAINER_HISTORICO WHERE CCR_CODIGO = {codigo}; " + // SQL-INJECTION-SAFE
                        $" DELETE FROM T_COLETA_CONTAINER WHERE CCR_CODIGO = {codigo}; "; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.ExecuteUpdate();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.LocalRetiradaContainer> BuscarLocaisRetiradaContainer(double cpfCnpjLocal, int codigoContainerTipo, bool mostrarTodos)
        {
            StringBuilder where = new StringBuilder();

            where.Append($" and Local.CLI_CGCCPF = {cpfCnpjLocal}", cpfCnpjLocal > 0d);
            where.Append($" and ContainerTipo.CTI_CODIGO = {codigoContainerTipo}", codigoContainerTipo > 0);

            string sql = $@"
                select Local.CLI_CGCCPF as LocalCpfCnpj,
                       Local.CLI_NOME as LocalNome,
                       Local.CLI_NOMEFANTASIA LocalNomeFantasia,
                       Local.CLI_CODIGO_INTEGRACAO LocalCodigoIntegracao,
                       Local.CLI_FISJUR LocalTipo,
                       Local.CLI_PONTO_TRANSBORDO LocalPontoTransbordo,
                       ContainerTipo.CTI_DESCRICAO TipoContainer,
                       ContainerTipo.CTI_CODIGO CodigoTipoContainer,
                       sum(LocalRetiradaContainer.Quantidade) as Quantidade,
                       sum(LocalRetiradaContainer.QuantidadeReservada) as QuantidadeReservada,
                       sum(LocalRetiradaContainer.Quantidade) - sum(LocalRetiradaContainer.QuantidadeReservada) as QuantidadeDisponivel
                  from (
                           select ColetaContainer.CLI_CODIGO_ATUAL as CpfCnpjLocal,
                                  Container.CTI_CODIGO as CodigoContainerTipo,
                                  1 Quantidade,
                                  0 as QuantidadeReservada
                             from T_COLETA_CONTAINER ColetaContainer
                             join T_CONTAINER Container on Container.CTR_CODIGO = ColetaContainer.CTR_CODIGO
                            where ColetaContainer.CCR_STATUS = {(int)StatusColetaContainer.EmAreaEsperaVazio}
                            union all
                           select RetiradaContainer.CLI_CODIGO_LOCAL as CpfCnpjLocal,
                                  RetiradaContainer.CTI_CODIGO as CodigoContainerTipo,
                                  0 Quantidade,
                                  1 as QuantidadeReservada
                             from T_RETIRADA_CONTAINER RetiradaContainer
                            join T_CARGA carga on carga.CAR_CODIGO = RetiradaContainer.CAR_CODIGO
                            where RetiradaContainer.CCR_CODIGO is null and carga.CAR_SITUACAO != {(int)SituacaoCarga.Cancelada} and carga.CAR_SITUACAO != {(int)SituacaoCarga.Anulada}
                       ) as LocalRetiradaContainer
                  join T_CLIENTE Local on Local.CLI_CGCCPF = LocalRetiradaContainer.CpfCnpjLocal
                  join T_CONTAINER_TIPO ContainerTipo on ContainerTipo.CTI_CODIGO = LocalRetiradaContainer.CodigoContainerTipo
                  {(where.Length > 0 ? $" where {where.ToString().Trim().Substring(4)} " : "")}
                 group by Local.CLI_CGCCPF, Local.CLI_NOME, Local.CLI_NOMEFANTASIA, Local.CLI_CODIGO_INTEGRACAO,
                       Local.CLI_FISJUR, Local.CLI_PONTO_TRANSBORDO, ContainerTipo.CTI_DESCRICAO, ContainerTipo.CTI_CODIGO
                {(mostrarTodos ? "" : " having sum(LocalRetiradaContainer.Quantidade) - sum(LocalRetiradaContainer.QuantidadeReservada) > 0 ")} order by Local.CLI_NOME";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.LocalRetiradaContainer)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Pedido.LocalRetiradaContainer>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Pedido.DetalheLocalRetiradaContainer> BuscarDetalhesLocaisRetiradaContainer(double local, int codigoContainer)
        {
            StringBuilder where = new StringBuilder();

            string sql = $@"
                   select RetiradaContainer.CLI_CODIGO_LOCAL as CpfCnpjLocal,
                        ContainerTipo.CTI_DESCRICAO as ContainerTipo,
			            ContainerTipo.CTI_CODIGO as CodigoContainerTipo,
			            Carga.CAR_CODIGO_CARGA_EMBARCADOR cargaEmbarcador,
			            Carga.CAR_CODIGO Codigo,
			            SUBSTRING((SELECT ', ' + Pedido.PED_NUMERO_EXP AS [text()]  
                                            FROM T_PEDIDO Pedido
                                            JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.Ped_codigo = Pedido.Ped_codigo
                                            WHERE CargaPedido.CAR_CODIGO = carga.CAR_CODIGO
                                            FOR XML PATH ('')), 3, 1000) NumeroEXP
                    from
                        T_RETIRADA_CONTAINER RetiradaContainer                             
                    join
                        T_CARGA carga 
                            on carga.CAR_CODIGO = RetiradaContainer.CAR_CODIGO
	                join
			            T_CONTAINER_TIPO ContainerTipo 
				            on ContainerTipo.CTI_CODIGO = RetiradaContainer.CTI_CODIGO  
                    where
                        RetiradaContainer.CCR_CODIGO is null 
                        and carga.CAR_SITUACAO != 13 
                        and carga.CAR_SITUACAO != 18
			            and RetiradaContainer.CLI_CODIGO_LOCAL = {local}
			            and RetiradaContainer.CTI_CODIGO = {codigoContainer}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Pedido.DetalheLocalRetiradaContainer)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Pedido.DetalheLocalRetiradaContainer>();
        }


        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarPorContainerEmEsperaVazio(int codigoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

            var result = from obj in query where obj.Container.Codigo == codigoContainer && obj.Status == StatusColetaContainer.EmAreaEsperaVazio select obj;

            return result.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Containers.ControleContainer.RelatorioControleContainer> ConsultarRelatorioControleContainer(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaControleContainer = new ConsultaControleContainer().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaControleContainer.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Containers.ControleContainer.RelatorioControleContainer)));

            return consultaControleContainer.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Containers.ControleContainer.RelatorioControleContainer>();
        }

        public int ContarConsultaRelatorioControleContainer(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaControleContainer = new ConsultaControleContainer().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaControleContainer.SetTimeout(1200).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Containers.HistoricoMovimentacaoContainers.HistoricoMovimentacaoContainers> ConsultarRelatorioHistoricoMovimentacaoContainer(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = new ConsultaHistoricoMovimentacaoContainer().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCarga.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Containers.HistoricoMovimentacaoContainers.HistoricoMovimentacaoContainers)));

            return consultaCarga.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Containers.HistoricoMovimentacaoContainers.HistoricoMovimentacaoContainers>();
        }

        public int ContarConsultaRelatorioHistoricoMovimentacaoContainer(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCarga = new ConsultaHistoricoMovimentacaoContainer().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCarga.SetTimeout(1200).UniqueResult<int>();
        }

        #endregion

        #region Metodos Privados

        private string GetSqlSelectConsultaContainerEmPosseNotificacao()
        {
            List<StatusColetaContainer> situacoesEmPosse = StatusColetaContainerHelper.ObterStatusEmPosse();

            string sql;
            sql = $@"
                SELECT 
                    ContainerColeta.CCR_CODIGO Codigo,
                    Container.CTR_NUMERO NumeroContainer,
                    ContainerColeta.CCR_DATA_COLETA DataColeta,
                    ContainerColeta.CCR_STATUS Status,
                    ContainerColeta.CCR_DATA_ULTIMA_MOVIMENTACAO DataUltimaMovimentacao,
                    ContainerColeta.CLI_CODIGO_COLETA LocalColeta,
		            ClienteColeta.CLI_NOME ClienteLocalColeta,
                    ContainerColeta.CLI_CODIGO_ATUAL LocalAtual,
		            ClienteAtual.CLI_NOME ClienteLocalAtual,
                    ContainerColeta.CLI_CODIGO_EMBARQUE LocalEmbarque,
		            ClienteEmbarque.CLI_NOME ClienteLocalEmbarque,
                    ContainerColeta.CCR_FREETIME FreeTime,
                    ContainerColeta.CCR_VALOR_DIARIA ValorDiaria,
                    ContainerColeta.FIL_CODIGO Filial,
                    (DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}') + 1) DiasEmPosse
                 ";

            sql += $@"from T_COLETA_CONTAINER as ContainerColeta
	                inner join T_CLIENTE ClienteColeta on ClienteColeta.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_COLETA
                    inner join T_CARGA Carga on ContainerColeta.CAR_CODIGO = Carga.CAR_CODIGO
                    inner join T_CONTAINER Container on Container.CTR_CODIGO = ContainerColeta.CTR_CODIGO
                    left join T_CLIENTE ClienteAtual on ClienteAtual.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_ATUAL
                    left join T_CLIENTE ClienteEmbarque on ClienteEmbarque.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_EMBARQUE
                    left join T_CARGA CargaAtual on ContainerColeta.CAR_CODIGO_ATUAL = CargaAtual.CAR_CODIGO
                where ContainerColeta.CCR_STATUS in ({string.Join(", ", situacoesEmPosse.Select(o => (int)o))})
                  and ContainerColeta.CCR_FREETIME > 0
                  and DATEADD(day, ContainerColeta.CCR_FREETIME, cast(ContainerColeta.CCR_DATA_COLETA as date)) <= '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}'";

            return sql;

        }

        private string GetSQLSelectConsultaContainersPesquisa(bool contar)
        {
            string sql;
            if (contar)
            {
                sql = @"select distinct(count(0) over ()) ";
            }
            else
            {
                sql = $@"
                SELECT 
                    ContainerColeta.CCR_CODIGO Codigo,
                    ContainerTipo.CTI_CODIGO CodigoTipoContainer,
                    Container.CTR_NUMERO NumeroContainer,
                    ContainerTipo.CTI_DESCRICAO TipoContainer,
                    ContainerColeta.CCR_DATA_COLETA DataColeta,
                    ContainerColeta.CCR_STATUS Status,
                    Carga.CAR_CODIGO Carga,
                    CargaAtual.CAR_CODIGO CargaAtual,
                    case when CargaAtual.CAR_CODIGO is null then Carga.CAR_CODIGO_CARGA_EMBARCADOR else CargaAtual.CAR_CODIGO_CARGA_EMBARCADOR end CargaEmbarcador,
                    ContainerColeta.CCR_DATA_ULTIMA_MOVIMENTACAO DataUltimaMovimentacao,
                    ContainerColeta.CLI_CODIGO_COLETA LocalColeta,
		            ClienteColeta.CLI_NOME ClienteLocalColeta,
                    ContainerColeta.CLI_CODIGO_ATUAL LocalAtual,
		            ClienteAtual.CLI_NOME ClienteLocalAtual,
                    ContainerColeta.CLI_CODIGO_EMBARQUE LocalEmbarque,
		            ClienteEmbarque.CLI_NOME ClienteLocalEmbarque,
                    ContainerColeta.CCR_FREETIME FreeTime,
                    ContainerColeta.CCR_VALOR_DIARIA ValorDiaria,
                    ContainerColeta.FIL_CODIGO Filial,
                    ContainerColeta.CCR_DATA_EMBARQUE DataEmbarque,
                    ContainerColeta.CCR_DATA_EMBARQUE_NAVIO DataEmbarqueNavio,
                    FilialCargaAtual.FIL_DESCRICAO FilialCargaAtual,
                    FilialCargaAtual.FIL_CNPJ CNPJFilialCargaAtual,
                    FilialOrigemCargaAtual.FIL_DESCRICAO FilialCargaOrigem,
                    FilialOrigemCargaAtual.FIL_CNPJ CNPJFilialCargaOrigem,
                    CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR NumeroCargaAgrupada,
                    (DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}')) + 1) DiasEmPosse,
                    ClienteAreaEsperaVazio.CLI_NOME AreaEsperaVazio,
                    (
                        select top 1 Justificativa.JSC_JUSTIFICATIVA_DESCRITIVA
                          from T_COLETA_CONTAINER_JUSTIFICATIVA Justificativa 
                          join T_JUSTIFICATIVA_CONTAINER JustificativaContainer on JustificativaContainer.JSC_CODIGO = Justificativa.JSC_CODIGO 
                         where Justificativa.CCR_CODIGO = ContainerColeta.CCR_CODIGO and Justificativa.CCR_STATUS = ContainerColeta.CCR_STATUS
                         order by CCR_DATA_JUSTIFICATIVA desc
                    ) JustificativaDescritiva,
				    (
                        select top 1 JustificativaContainer.JSC_DESCRICAO 
                          from T_COLETA_CONTAINER_JUSTIFICATIVA Justificativa 
                          join T_JUSTIFICATIVA_CONTAINER JustificativaContainer on JustificativaContainer.JSC_CODIGO = Justificativa.JSC_CODIGO 
                         where Justificativa.CCR_CODIGO = ContainerColeta.CCR_CODIGO and Justificativa.CCR_STATUS = ContainerColeta.CCR_STATUS
                         order by CCR_DATA_JUSTIFICATIVA desc
                    ) Justificativa,
		            substring(( 
                        select distinct ', ' + Pedido.PED_NUMERO_BOOKING 
                          from T_PEDIDO Pedido 
                          join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                         where CargaPedido.CAR_CODIGO_ORIGEM = CargaAtual.CAR_CODIGO
                           and isnull(Pedido.PED_NUMERO_BOOKING, '') <> ''
                           for xml path('')
                    ), 3, 1000) NumeroBookingAgrupada,
		            substring(( 
                        select distinct ', ' + Pedido.PED_NUMERO_EXP 
                          from T_PEDIDO Pedido 
                          join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                         where CargaPedido.CAR_CODIGO_ORIGEM = CargaAtual.CAR_CODIGO
                           and isnull(Pedido.PED_NUMERO_EXP, '') <> ''
                           for xml path('')
                    ), 3, 1000) NumeroEXPAgrupada,
		            substring(( 
                        select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR 
                          from T_PEDIDO Pedido 
                          join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                         where CargaPedido.CAR_CODIGO_ORIGEM = CargaAtual.CAR_CODIGO
                           and isnull(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, '') <> ''
                           for xml path('')
                    ), 3, 1000) PedidoAgrupado,
                    substring(( 
                        select distinct ', ' + Pedido.PED_NUMERO_BOOKING 
                          from T_PEDIDO Pedido 
                          join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                         where CargaPedido.CAR_CODIGO = CargaAtual.CAR_CODIGO
                           and isnull(Pedido.PED_NUMERO_BOOKING, '') <> ''
                           for xml path('')
                    ), 3, 1000) NumeroBooking,
		            substring(( 
                        select distinct ', ' + Pedido.PED_NUMERO_EXP 
                          from T_PEDIDO Pedido 
                          join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                         where CargaPedido.CAR_CODIGO = CargaAtual.CAR_CODIGO
                           and isnull(Pedido.PED_NUMERO_EXP, '') <> ''
                           for xml path('')
                    ), 3, 1000) NumeroEXP,
		            substring(( 
                        select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR 
                          from T_PEDIDO Pedido 
                          join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                         where CargaPedido.CAR_CODIGO = CargaAtual.CAR_CODIGO
                           and isnull(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, '') <> ''
                           for xml path('')
                    ), 3, 1000) Pedido
                 ";
            }

            sql += @"from T_COLETA_CONTAINER as ContainerColeta
                    inner join T_CARGA Carga on ContainerColeta.CAR_CODIGO = Carga.CAR_CODIGO
                    inner join T_CONTAINER Container on Container.CTR_CODIGO = ContainerColeta.CTR_CODIGO
                    left join T_CLIENTE ClienteColeta on ClienteColeta.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_COLETA
                    left join T_CARGA CargaAtual on ContainerColeta.CAR_CODIGO_ATUAL = CargaAtual.CAR_CODIGO
                    left join T_FILIAL FilialCargaAtual on FilialCargaAtual.FIL_CODIGO = CargaAtual.FIL_CODIGO
                    left join T_CLIENTE ClienteAtual on ClienteAtual.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_ATUAL
                    left join T_CLIENTE ClienteEmbarque on ClienteEmbarque.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_EMBARQUE
                    join T_CONTAINER_TIPO ContainerTipo on ContainerTipo.CTI_CODIGO = Container.CTI_CODIGO
                    left join T_CARGA CargaAgrupada on CargaAgrupada.CAR_CODIGO = CargaAtual.CAR_CODIGO_AGRUPAMENTO
                    left join T_CLIENTE ClienteAreaEsperaVazio ON ClienteAreaEsperaVazio.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_AREA_ESPERA_VAZIO
                    left join T_FILIAL FilialOrigemCargaAtual on FilialOrigemCargaAtual.FIL_CODIGO = CargaAtual.FIL_CODIGO_ORIGEM ";


            return sql;
        }

        private string GetSQLWhereConsultaContaineresPesquisa(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer filtroPesquisa)
        {
            string filtro = " where 1=1 ";


            if (!string.IsNullOrWhiteSpace(filtroPesquisa.CodigoCargaEmbarcador))
                filtro += $" AND (CargaAtual.CAR_CODIGO_CARGA_EMBARCADOR = '{filtroPesquisa.CodigoCargaEmbarcador}' or CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR = '{filtroPesquisa.CodigoCargaEmbarcador}' or Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtroPesquisa.CodigoCargaEmbarcador}')";

            if (filtroPesquisa.DataInicialColeta != DateTime.MinValue)
                filtro += $" AND ContainerColeta.CCR_DATA_COLETA >= '{filtroPesquisa.DataInicialColeta.ToString("yyyy-MM-dd")}' ";

            if (filtroPesquisa.DataFinalColeta != DateTime.MinValue)
                filtro += $" AND ContainerColeta.CCR_DATA_COLETA < '{filtroPesquisa.DataFinalColeta.AddDays(1).ToString("yyyy-MM-dd")}'";

            if (filtroPesquisa.DataUltimaMovimentacao != DateTime.MinValue)
                filtro += $" AND convert(date,ContainerColeta.CCR_DATA_ULTIMA_MOVIMENTACAO, 102) = convert(datetime, '{filtroPesquisa.DataUltimaMovimentacao.ToString("yyyy-MM-dd")}', 102) ";

            if (filtroPesquisa.DiasPosse > 0)
                filtro += $" AND (DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}')) + 1) >= {filtroPesquisa.DiasPosse}";

            if (filtroPesquisa.DiasPosseFim > 0)
                filtro += $" AND (DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}')) + 1) <= {filtroPesquisa.DiasPosseFim}";

            if (filtroPesquisa.AreaDeRedex > 0)
                filtro += $" AND ContainerColeta.CLI_CODIGO_ATUAL = '{filtroPesquisa.AreaDeRedex}'";

            if (filtroPesquisa.DataEmbarque != DateTime.MinValue)
                filtro += $" AND convert(date,ContainerColeta.CCR_DATA_EMBARQUE, 102) = convert(datetime, '{filtroPesquisa.DataEmbarque.ToString("yyyy-MM-dd")}', 102) ";

            if (!string.IsNullOrEmpty(filtroPesquisa.NumeroContainer))
                filtro += $" AND Container.CTR_NUMERO = '{filtroPesquisa.NumeroContainer}'";

            if (filtroPesquisa.LocalEsperaVazio > 0)
                filtro += $" AND ContainerColeta.CLI_CODIGO_ATUAL = '{filtroPesquisa.LocalEsperaVazio}'";

            if (filtroPesquisa.LocalAtual > 0)
                filtro += $" AND ContainerColeta.CLI_CODIGO_ATUAL = '{filtroPesquisa.LocalAtual}'";

            if (filtroPesquisa.LocalColeta > 0)
                filtro += $" AND ContainerColeta.CLI_CODIGO_COLETA = '{filtroPesquisa.LocalColeta}'";

            if (filtroPesquisa.SomenteExcedidos)
            {
                if (filtroPesquisa.DataInicialColeta == DateTime.MinValue)
                {
                    filtro += $"AND (DATEDIFF(day,DATEADD(day, ContainerColeta.CCR_FREETIME, ContainerColeta.CCR_DATA_COLETA),'{DateTime.Now.ToString("yyyy-MM-dd")}'))> 1 ";
                }
                else
                {
                    filtro += $"AND (DATEDIFF(day,DATEADD(day, ContainerColeta.CCR_FREETIME, ContainerColeta.CCR_DATA_COLETA),'{filtroPesquisa.DataInicialColeta.ToString("yyyy-MM-dd")}'))> 1 ";
                }
            }


            if (!string.IsNullOrEmpty(filtroPesquisa.NumeroPedido) || !string.IsNullOrEmpty(filtroPesquisa.NumeroEXP) || !string.IsNullOrEmpty(filtroPesquisa.NumeroBooking))
            {
                filtro += @" and (exists(
                    select top(1) _pedido.PED_CODIGO
                      from T_PEDIDO _pedido
                      join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
                     where _cargaPedido.CAR_CODIGO = CargaAtual.CAR_CODIGO ";

                if (!string.IsNullOrEmpty(filtroPesquisa.NumeroPedido))
                    filtro += $" and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtroPesquisa.NumeroPedido}'";

                if (!string.IsNullOrEmpty(filtroPesquisa.NumeroEXP))
                    filtro += $" and _pedido.PED_NUMERO_EXP = '{filtroPesquisa.NumeroEXP}'";

                if (!string.IsNullOrEmpty(filtroPesquisa.NumeroBooking))
                    filtro += $" and _pedido.PED_NUMERO_BOOKING = '{filtroPesquisa.NumeroBooking}'";

                filtro += @") or exists (  select top(1) _pedido.PED_CODIGO
                      from T_PEDIDO _pedido
                      join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
                     where _cargaPedido.CAR_CODIGO_ORIGEM = CargaAtual.CAR_CODIGO ";

                if (!string.IsNullOrEmpty(filtroPesquisa.NumeroPedido))
                    filtro += $" and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtroPesquisa.NumeroPedido}'";

                if (!string.IsNullOrEmpty(filtroPesquisa.NumeroEXP))
                    filtro += $" and _pedido.PED_NUMERO_EXP = '{filtroPesquisa.NumeroEXP}'";

                if (!string.IsNullOrEmpty(filtroPesquisa.NumeroBooking))
                    filtro += $" and _pedido.PED_NUMERO_BOOKING = '{filtroPesquisa.NumeroBooking}'";

                filtro += " ))";
            }

            if (filtroPesquisa.TipoContainer > 0)
                filtro += $" AND ContainerTipo.CTI_CODIGO = '{filtroPesquisa.TipoContainer}'";

            if (filtroPesquisa.FilialAtual > 0)
                filtro += $" AND CargaAtual.FIL_CODIGO = '{filtroPesquisa.FilialAtual}'";


            if (filtroPesquisa.StatusContainer.HasValue)
                filtro += $" AND ContainerColeta.CCR_STATUS = {(int)filtroPesquisa.StatusContainer.Value} ";
            else
                filtro += $" AND ContainerColeta.CCR_STATUS <> {(int)StatusColetaContainer.Cancelado} ";

            if (filtroPesquisa.DataEmbarqueNavioInicial != DateTime.MinValue && filtroPesquisa.DataEmbarqueNavioFinal != DateTime.MinValue)
            {
                filtro += $"AND ((CONVERT(VARCHAR, ContainerColeta.CCR_DATA_EMBARQUE_NAVIO, 120) >= '{filtroPesquisa.DataEmbarqueNavioInicial:yyyy-MM-dd}' AND " +
                          $"CONVERT(VARCHAR, ContainerColeta.CCR_DATA_EMBARQUE_NAVIO, 120) < '{filtroPesquisa.DataEmbarqueNavioFinal.AddDays(1):yyyy-MM-dd}') OR " +
                          $"ContainerColeta.CCR_DATA_EMBARQUE_NAVIO is null)";
            }
            else
            {
                if (filtroPesquisa.DataEmbarqueNavioInicial != DateTime.MinValue) { }
                filtro += $" AND (CONVERT(VARCHAR, ContainerColeta.CCR_DATA_EMBARQUE_NAVIO, 120) >= '{filtroPesquisa.DataEmbarqueNavioInicial:yyyy-MM-dd}' OR ContainerColeta.CCR_DATA_EMBARQUE_NAVIO is null) ";

                if (filtroPesquisa.DataEmbarqueNavioFinal != DateTime.MinValue)
                    filtro += $" AND (CONVERT(VARCHAR, ContainerColeta.CCR_DATA_EMBARQUE_NAVIO, 120) < '{filtroPesquisa.DataEmbarqueNavioFinal.AddDays(1):yyyy-MM-dd}' OR ContainerColeta.CCR_DATA_EMBARQUE_NAVIO is null) ";
            }
            return filtro;
        }

        #endregion Metodos Privados
    }
}
