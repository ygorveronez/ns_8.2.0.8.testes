using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Text;

namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>
    {
        #region Construtores

        public FechamentoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFrete filtrosPesquisa)
        {
            var consultaFechamentoFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>();

            if (filtrosPesquisa.Numero > 0)
                consultaFechamentoFrete = consultaFechamentoFrete.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaFechamentoFrete = consultaFechamentoFrete.Where(o => o.Contrato.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoContratoFrete > 0)
                consultaFechamentoFrete = consultaFechamentoFrete.Where(o => o.Contrato.Codigo == filtrosPesquisa.CodigoContratoFrete);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaFechamentoFrete = consultaFechamentoFrete.Where(o => o.DataFechamento.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaFechamentoFrete = consultaFechamentoFrete.Where(o => o.DataFechamento.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaFechamentoFrete = consultaFechamentoFrete.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consultaFechamentoFrete;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete BuscarFechamentoExistente(int codigoContrato, int ano, int mes, int periodo)
        {
            var consultaFechamentoFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>()
                .Where(o =>
                    o.Contrato.Codigo == codigoContrato &&
                    o.Situacao != SituacaoFechamentoFrete.Cancelado &&
                    o.Periodo == periodo &&
                    o.Mes == mes &&
                    o.Ano == ano
                );

            return consultaFechamentoFrete
                .Fetch(o => o.Contrato)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete BuscarFechamentoExistente(int codigoContrato, DateTime data)
        {
            var consultaFechamentoFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>()
                .Where(o =>
                    o.Contrato.Codigo == codigoContrato &&
                    o.Situacao != SituacaoFechamentoFrete.Cancelado &&
                    o.DataInicio >= data.Date &&
                    o.DataFim <= data.Date.Add(DateTime.MaxValue.TimeOfDay)
                );

            return consultaFechamentoFrete.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete situacaoFechamentoFrete, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>();
            var result = from obj in query where obj.Situacao == situacaoFechamentoFrete select obj;


            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>();
            var result = from obj in query select obj.Numero;

            int maiorNumero = 0;
            if (result.Count() > 0)
                maiorNumero = result.Max();

            return maiorNumero + 1;
        }

        public decimal BuscarValorFranquiaPagoPorContrato(DateTime dataInicio, DateTime dataFim, int codigoContrato)
        {
            var consultaFechamentoFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>()
                .Where(o => (o.Contrato.Codigo == codigoContrato) 
                && o.DataInicio >= dataInicio.Date && o.DataFim <= dataFim.Date.Add(DateTime.MaxValue.TimeOfDay)
                && (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.Fechado));

            return consultaFechamentoFrete.Sum(o => (decimal?)(o.ValorPagar + o.ValorComplementos)) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFrete filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFechamentoFrete = Consultar(filtrosPesquisa);

            consultaFechamentoFrete = consultaFechamentoFrete
                .Fetch(o => o.Contrato);

            return ObterLista(consultaFechamentoFrete, parametrosConsulta);
        }

        public Dominio.Relatorios.Embarcador.DataSource.Fechamento.Fechamento ConsultarRelatorio(int codigoFechamento)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select FechamentoFrete.FEF_DATA_INICIO as DataInicio, ");
            sql.Append("       FechamentoFrete.FEF_DATA_FIM as DataFim, ");
            sql.Append("       Contrato.CFT_FRANQUIA_CONTRATO_MENSAL ContratoMensal, ");
            sql.Append("       Contrato.CFT_FRANQUIA_VALOR_EXCEDENTE as ValorKmExcedido, ");
            sql.Append("       Contrato.CFT_FRANQUIA_TOTAL_KM as TotalFranquiaKmMes, ");
            sql.Append("       FechamentoFreteSaldo.TotalValorCTe, ");
            sql.Append("       cast(FechamentoFreteSaldo.KmConsumo as int) as KmConsumoPeriodo, ");
            sql.Append("       cast(( ");
            sql.Append("           select sum(ContratoSaldoMes.CSM_DISTANCIA) ");
            sql.Append("             from T_CONTRATO_SALDO_MES ContratoSaldoMes ");
            sql.Append("             join T_CARGA Carga on Carga.CAR_CODIGO = ContratoSaldoMes.CAR_CODIGO ");
            sql.Append("            where ContratoSaldoMes.CFT_CODIGO = Contrato.CFT_CODIGO ");
            sql.Append($"             and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Cancelada} ");
            sql.Append($"             and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Anulada} ");
            sql.Append("              and ( ");
            sql.Append("                       datepart(year, ContratoSaldoMes.CSM_DATA_REGISTRO) = datepart(year, FechamentoFrete.FEF_DATA_FIM) and ");
            sql.Append("                       datepart(month, ContratoSaldoMes.CSM_DATA_REGISTRO) = datepart(month, FechamentoFrete.FEF_DATA_FIM) and ");
            sql.Append("                       datepart(day, ContratoSaldoMes.CSM_DATA_REGISTRO) <= datepart(day, FechamentoFrete.FEF_DATA_FIM) ");
            sql.Append("                  ) ");
            sql.Append("       ) as int) as KmConsumoMes, ");
            sql.Append("      ( ");
            sql.Append("           ContratoTransportador.EMP_RAZAO + ");
            sql.Append("           case  ");
            sql.Append("               when ContratoTransportador.LOC_CODIGO is null then '' ");
            sql.Append("               else ");
            sql.Append("                   ' (' + TransportadorLocalidade.LOC_DESCRICAO + ' - ' +  ");
            sql.Append("                   case  ");
            sql.Append("                       when (TransportadorLocalidade.LOC_IBGE <> 9999999 and TransportadorLocalidade.PAI_CODIGO is null) then isnull(LocalidadeEstado.UF_SIGLA, '') ");
            sql.Append("                       when (LocalidadePais.PAI_ABREVIACAO is null) then isnull(LocalidadePais.PAI_NOME, '') ");
            sql.Append("                       else isnull(LocalidadePais.PAI_ABREVIACAO, '') ");
            sql.Append("                   end + ')' ");
            sql.Append("           end ");
            sql.Append("       ) as Transportador, ");
            sql.Append("       isnull(( ");
            sql.Append("           select count(Veiculo.VEI_CODIGO) ");
            sql.Append("             from T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoVeiculo ");
            sql.Append("             join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = ContratoVeiculo.VEI_CODIGO ");
            sql.Append("            where ContratoVeiculo.CFT_CODIGO = Contrato.CFT_CODIGO ");
            sql.Append("              and Veiculo.VEI_TIPOVEICULO = '0' ");
            sql.Append("       ), 0) as QuantidadeCavalos ");
            sql.Append("  from T_FECHAMENTO_FRETE FechamentoFrete ");
            sql.Append("  join T_CONTRATO_FRETE_TRANSPORTADOR Contrato on Contrato.CFT_CODIGO = FechamentoFrete.CFT_CODIGO ");
            sql.Append("  join T_EMPRESA ContratoTransportador on ContratoTransportador.EMP_CODIGO = Contrato.EMP_CODIGO ");
            sql.Append("  left join T_LOCALIDADES TransportadorLocalidade on TransportadorLocalidade.LOC_CODIGO = ContratoTransportador.LOC_CODIGO ");
            sql.Append("  left join T_UF LocalidadeEstado on LocalidadeEstado.UF_SIGLA = TransportadorLocalidade.UF_SIGLA ");
            sql.Append("  left join T_PAIS LocalidadePais on LocalidadePais.PAI_CODIGO = TransportadorLocalidade.PAI_CODIGO ");
            sql.Append("  left join ( ");
            sql.Append("           select FechamentoFreteCarga.FEF_CODIGO CodigoFechamentoFrete, ");
            sql.Append("                  sum(ContratoSaldoMes.CSM_DISTANCIA) as KmConsumo, ");
            sql.Append("                  sum(ContratoSaldoMes.CSM_VALOR) as TotalValorCTe ");
            sql.Append("             from T_FECHAMENTO_FRETE_CARGA FechamentoFreteCarga ");
            sql.Append("             join T_CONTRATO_SALDO_MES ContratoSaldoMes on ContratoSaldoMes.CAR_CODIGO = FechamentoFreteCarga.CAR_CODIGO ");
            sql.Append("            group by FechamentoFreteCarga.FEF_CODIGO ");
            sql.Append("       ) FechamentoFreteSaldo on FechamentoFreteSaldo.CodigoFechamentoFrete = FechamentoFrete.FEF_CODIGO ");
            sql.Append($"where FechamentoFrete.FEF_CODIGO = {codigoFechamento} ");

            var consultaAcompanhamentoReversa = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consultaAcompanhamentoReversa.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fechamento.Fechamento)));

            return consultaAcompanhamentoReversa.UniqueResult<Dominio.Relatorios.Embarcador.DataSource.Fechamento.Fechamento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoComplemento> ConsultarRelatorioComplemento(int codigoFechamento)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select ComplementoInfo.CCC_VALOR_OCORRENCIA as Valor, ");
            sql.Append("       Cte.CON_NUM as CTe, ");
            sql.Append("       CteDestinatario.PCT_CODIGO_INTEGRACAO as Destinatario ");
            sql.Append("  from T_CARGA_CTE_COMPLEMENTO_INFO ComplementoInfo ");
            sql.Append("  join T_CTE Cte on Cte.CON_CODIGO = ComplementoInfo.CON_CODIGO ");
            sql.Append("  join T_CTE_PARTICIPANTE CteDestinatario on CteDestinatario.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
            sql.Append($"where ComplementoInfo.FEF_CODIGO = {codigoFechamento} ");

            var consultaAcompanhamentoReversa = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consultaAcompanhamentoReversa.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoComplemento)));

            return consultaAcompanhamentoReversa.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoComplemento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoDestinatario> ConsultarRelatorioDestinatario(int codigoFechamento)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select SaldoFechamento.Destinatario, ");
            sql.Append("       cast(sum(SaldoFechamento.KmDistancia) as int) as KmDistancia, ");
            sql.Append("       sum(SaldoFechamento.ValorCTe) as ValorCTe ");
            sql.Append("  from ( ");
            sql.Append("           select CteDestinatario.PCT_CODIGO_INTEGRACAO as Destinatario, ");
            sql.Append("                  CteDestinatario.PCT_CPF_CNPJ as CpfCnpjDestinatario, ");
            sql.Append("                  ContratoSaldoMes.CSM_DISTANCIA as KmDistancia, ");
            sql.Append("                  ContratoSaldoMes.CSM_VALOR as ValorCTe ");
            sql.Append("             from T_FECHAMENTO_FRETE_CARGA FechamentoFreteCarga ");
            sql.Append("             join T_CONTRATO_SALDO_MES ContratoSaldoMes on ContratoSaldoMes.CAR_CODIGO = FechamentoFreteCarga.CAR_CODIGO ");
            sql.Append("             join T_CARGA_CTE CargaCte on CargaCte.CAR_CODIGO = ContratoSaldoMes.CAR_CODIGO ");
            sql.Append("             join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
            sql.Append("             join T_CTE_PARTICIPANTE CteDestinatario on CteDestinatario.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
            sql.Append($"           where FechamentoFreteCarga.FEF_CODIGO = {codigoFechamento} ");
            sql.Append("            group by ContratoSaldoMes.CAR_CODIGO, ContratoSaldoMes.CSM_DISTANCIA, ContratoSaldoMes.CSM_VALOR, ");
            sql.Append("                  CteDestinatario.PCT_CODIGO_INTEGRACAO, CteDestinatario.PCT_CPF_CNPJ ");
            sql.Append("       ) as SaldoFechamento ");
            sql.Append(" group by SaldoFechamento.CpfCnpjDestinatario, SaldoFechamento.Destinatario ");
            sql.Append(" order by SaldoFechamento.Destinatario ");

            var consultaAcompanhamentoReversa = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consultaAcompanhamentoReversa.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoDestinatario)));

            return consultaAcompanhamentoReversa.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoDestinatario>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoDestinatarioDetalhado> ConsultarRelatorioDestinatarioDetalhado(int codigoFechamento, bool consultarPorCompomenteFrete)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select CteDestinatario.PCT_CODIGO_INTEGRACAO as Destinatario, ");
            sql.Append("       Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
            sql.Append("       cast(ContratoSaldoMes.CSM_DISTANCIA as int) as KmDistancia, ");
            sql.Append("       Cte.CON_NUM as CTe, ");
            sql.Append("       Veiculo.VEI_PLACA as Placa, ");
            sql.Append($"      {(consultarPorCompomenteFrete ? "ComponenteFrete.CCC_VALOR_COMPONENTE" : "Cte.CON_VALOR_FRETE")} as Valor ");
            sql.Append("  from T_FECHAMENTO_FRETE_CARGA FechamentoFreteCarga ");
            sql.Append("  join T_CONTRATO_SALDO_MES ContratoSaldoMes on ContratoSaldoMes.CAR_CODIGO = FechamentoFreteCarga.CAR_CODIGO ");
            sql.Append("  join T_CARGA Carga on Carga.CAR_CODIGO = ContratoSaldoMes.CAR_CODIGO ");
            sql.Append("  join T_CARGA_CTE CargaCte on CargaCte.CAR_CODIGO = Carga.CAR_CODIGO ");
            sql.Append("  join T_CTE Cte on Cte.CON_CODIGO = CargaCte.CON_CODIGO ");
            sql.Append("  join T_CTE_PARTICIPANTE CteDestinatario on CteDestinatario.PCT_CODIGO = Cte.CON_DESTINATARIO_CTE ");
            sql.Append("  join T_CARGA_CTE_COMPONENTES_FRETE ComponenteFrete on ComponenteFrete.CCT_CODIGO = CargaCte.CCT_CODIGO ", consultarPorCompomenteFrete);
            sql.Append("  left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
            sql.Append($"where FechamentoFreteCarga.FEF_CODIGO = {codigoFechamento} ");
            sql.Append("   and CargaCte.CCC_CODIGO is null ");
            sql.Append("   and Cte.CON_STATUS = 'A' ");
            sql.Append(" order by CteDestinatario.PCT_CODIGO_INTEGRACAO ");

            var consultaAcompanhamentoReversa = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consultaAcompanhamentoReversa.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoDestinatarioDetalhado)));

            return consultaAcompanhamentoReversa.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fechamento.FechamentoDestinatarioDetalhado>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFrete filtrosPesquisa)
        {
            var consultaFechamentoFrete = Consultar(filtrosPesquisa);

            return consultaFechamentoFrete.Count();
        }

        public bool VerificarExistemFechamento()
        {
            IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>();

            query = query.Where(obj => obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.Cancelado);

            return query.Select(o => o.Codigo).Any();
        }

        #endregion
    }
}
