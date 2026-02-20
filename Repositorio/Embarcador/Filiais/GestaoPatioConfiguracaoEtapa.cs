using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Filiais
{
    public class GestaoPatioConfiguracaoEtapa : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.GestaoPatioConfiguracaoEtapa>
    {
        #region Construtores

        public GestaoPatioConfiguracaoEtapa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.Filial.GestaoPatioConfiguracaoEtapa> BuscarPorCarga(int codigoCarga)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select Etapas.FGE_ETAPA_FLUXO_GESTAO as Etapa, ");
            sql.Append("       Etapas.FGE_ORDEM as Ordem, ");
            sql.Append("       Configuracao.GCE_POSSUI_FILIAIS_EXCLUSIVAS as PossuiFiliaisExclusivas, ");
            sql.Append("       Configuracao.GCE_SITUACAO_CONFIRMACAO as SituacaoConfirmacao, ");
            sql.Append("       Configuracao.GCE_BLOQUEAR_EDICAO_DADOS_TRANSPORTE_JANELA_TRANSPORTADOR as BloquearEdicaoDadosTransporteJanelaTransportador, ");
            sql.Append("       Configuracao.GCE_BLOQUEAR_EDICAO_VEICULOS_CARGA as BloquearEdicaoVeiculosCarga ");
            sql.Append("  from T_FLUXO_GESTAO_PATIO FluxoPatio ");
            sql.Append("  join T_FLUXO_GESTAO_PATIO_ETAPAS Etapas on Etapas.FGP_CODIGO = FluxoPatio.FGP_CODIGO ");
            sql.Append("  join T_GESTAO_PATIO_CONFIGURACAO_ETAPA Configuracao on Configuracao.GCE_ETAPA = Etapas.FGE_ETAPA_FLUXO_GESTAO ");
            sql.Append($"where FluxoPatio.CAR_CODIGO = {codigoCarga} ");
            sql.Append($"  and FluxoPatio.FGP_SITUACAO_ETAPA_FLUXO_GESTAO in ({(int)SituacaoEtapaFluxoGestaoPatio.Aguardando}, {(int)SituacaoEtapaFluxoGestaoPatio.Aprovado}) ");
            sql.Append("   and (Configuracao.GCE_TIPO is null or Configuracao.GCE_TIPO = FluxoPatio.FGE_TIPO) ");
            sql.Append("   and Configuracao.GCE_SITUACAO_CONFIRMACAO = ( ");
            sql.Append("           case ");
            sql.Append($"              when Etapas.FGE_ORDEM > FluxoPatio.FGP_ETAPA_ATUAL then {(int)SituacaoConfirmacaoEtapaFluxoGestaoPatio.AguardandoLiberacao} ");
            sql.Append($"              when Etapas.FGE_ORDEM = FluxoPatio.FGP_ETAPA_ATUAL and FluxoPatio.FGP_SITUACAO_ETAPA_FLUXO_GESTAO = {(int)SituacaoEtapaFluxoGestaoPatio.Aguardando} then {(int)SituacaoConfirmacaoEtapaFluxoGestaoPatio.Liberada} ");
            sql.Append($"              else {(int)SituacaoConfirmacaoEtapaFluxoGestaoPatio.Confirmada} ");
            sql.Append("           end ");
            sql.Append("       ) ");
            sql.Append("   and ( ");
            sql.Append("           Configuracao.GCE_POSSUI_FILIAIS_EXCLUSIVAS = 0 or ");
            sql.Append("           exists( ");
            sql.Append("               select top(1) FilialExclusiva.FIL_CODIGO ");
            sql.Append("                 from T_GESTAO_PATIO_CONFIGURACAO_ETAPA_FILIAL_EXCLUSIVA FilialExclusiva ");
            sql.Append("                where FilialExclusiva.GCE_CODIGO = Configuracao.GCE_CODIGO ");
            sql.Append("                  and FilialExclusiva.FIL_CODIGO = FluxoPatio.FIL_CODIGO ");
            sql.Append("           ) ");
            sql.Append("       ) ");

            var consultaGestaoPatioConfiguracaoEtapa = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consultaGestaoPatioConfiguracaoEtapa.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Filial.GestaoPatioConfiguracaoEtapa)));

            return consultaGestaoPatioConfiguracaoEtapa.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Filial.GestaoPatioConfiguracaoEtapa>();
        }

        #endregion
    }
}
