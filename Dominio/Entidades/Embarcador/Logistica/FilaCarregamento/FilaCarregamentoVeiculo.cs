using Dominio.Entidades.Embarcador.Veiculos;
using System;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILA_CARREGAMENTO_VEICULO", EntityName = "FilaCarregamentoVeiculo", Name = "Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo", NameType = typeof(FilaCarregamentoVeiculo))]
    public class FilaCarregamentoVeiculo : EntidadeBase, IEquatable<FilaCarregamentoVeiculo>
    {
        #region Propriedades Públicas

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FLV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrada", Column = "FLV_DATA_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramada", Column = "FLV_DATA_PROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProgramada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramadaInicial", Column = "FLV_DATA_PROGRAMADA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProgramadaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramadaAlteradaAutomaticamente", Column = "FLV_DATA_PROGRAMADA_ALTERADA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DataProgramadaAlteradaAutomaticamente { get; set; }

        [Obsolete("Migrado o campo para carga.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardandoSalvarDadosTransporteCarga", Column = "FLV_AGUARDANDO_SALVAR_DADOS_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardandoSalvarDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "FLV_POSICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FLV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FLV_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoConjuntoMotorista", Column = "FCM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoConjuntoMotorista ConjuntoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilaCarregamentoConjuntoVeiculo", Column = "FCV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FilaCarregamentoConjuntoVeiculo ConjuntoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConjuntoVeiculoDedicado", Column = "FLV_CONJUNTO_VEICULO_DEDICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConjuntoVeiculoDedicado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoRetornoCarga", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Retornos.TipoRetornoCarga TipoRetornoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AreaVeiculo", Column = "ARV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AreaVeiculo AreaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Equipamento Equipamento { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (CentroCarregamento != null)
                    return $"Fila do centro de carregamento {CentroCarregamento.Descricao}";

                if (Filial != null)
                    return $"Fila de carregamento da filial {Filial.Descricao}";

                return "Fila de carregamento";
            }
        }

        #endregion

        #region Métodos Públicos

        public virtual bool IsConjuntosCompletos()
        {
            return ConjuntoMotorista.IsCompleto() && ConjuntoVeiculo.IsCompleto();
        }

        public virtual string ObterCorLinha()
        {
            switch (Situacao)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado:
                    return "#ff99ff";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga:
                    return "#ffff99";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga:
                    return "#c2ff66";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.AguardandoCarga:
                    return "#ffd966";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo:
                    return "#c2ff66";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao:
                    return "#ffd966";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos:
                    return "#ffcc99";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.CargaCancelada:
                    return "#ff6666";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.Disponivel:
                    if (ConjuntoMotorista.FilaCarregamentoMotorista == null)
                        return Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculo.Vazio ? "#80ff80" : "#91a8ee";
                    else
                    {
                        if (ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoMotorista.Disponivel)
                            return Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculo.Vazio ? "#80ff80" : "#91a8ee";

                        if (ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoMotorista.SenhaPerdida)
                            return "#cccccc";

                        if (ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoMotorista.CargaRecusada)
                            return "#ff99ff";
                    }
                    break;

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.EmChecklist:
                    return "#92ede8";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.EmRemocao:
                    return "#ffa280";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.EmTransicao:
                    return Tipo == ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculo.Vazio ? "#80ff80" : "#91a8ee";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado:
                    return "#53c685";

                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo.EmViagem:
                    return "#9fc69f";
            }

            return "";
        }

        public virtual string ObterPosicao()
        {
            return Posicao > 0 ? Posicao.ToString() : "";
        }

        public virtual Empresa ObterTransportador()
        {
            Empresa transportadorMotorista = ConjuntoMotorista.Motorista?.Empresa;
            Empresa transportadorTracao = ConjuntoVeiculo.Tracao?.Empresa;
            Empresa transportadorReboques = ConjuntoVeiculo.Reboques?.FirstOrDefault()?.Empresa;

            return transportadorTracao ?? transportadorReboques ?? transportadorMotorista;
        }

        public virtual bool Equals(FilaCarregamentoVeiculo other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion
    }
}
