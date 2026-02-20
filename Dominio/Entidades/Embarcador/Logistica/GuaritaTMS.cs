using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUARITA_TMS", EntityName = "GuaritaTMS", Name = "Dominio.Entidades.Embarcador.Logistica.GuaritaTMS", NameType = typeof(GuaritaTMS))]
    public class GuaritaTMS : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Logistica.GuaritaTMS>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GUA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMAtual", Column = "GUA_KM_ATUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "GUA_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaidaEntrada", Column = "GUA_DATA_SAIDA_ENTRADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataSaidaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraSaidaEntrada", Column = "GUA_HORA_SAIDA_ENTRADA", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = false)]
        public virtual TimeSpan HoraSaidaEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GUA_ENTRADA_SAIDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida TipoEntradaSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GUA_HORARIO_INFORMADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HorarioInformadoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizouViagem", Column = "GUA_FINALIZOU_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizouViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornouComReboque", Column = "GUA_RETORNOU_COM_REBOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornouComReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VeiculoVazio", Column = "GUA_VEICULO_VAZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VeiculoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "GUA_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_OPERADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrota OrdemServicoFrota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GUA_TIPO_VEICULO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculoTerceiro", Column = "GUA_PLACA_VEICULO_TERCEIRO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string PlacaVeiculoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarCheckList", Column = "GUA_GERAR_CHECKLIST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarCheckList { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoPatio.CheckListTipo CheckListTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EntrouCarregado", Column = "GUA_ENTROU_CARREGADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntrouCarregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlterarSituacaoVeiculoParaLiberado", Column = "GUA_ALTERAR_SITUACAO_VEICULO_PARA_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarSituacaoVeiculoParaLiberado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AlterarReboquesVeiculo", Column = "GUA_ALTERAR_REBOQUES_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlterarReboquesVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotoristaTerceiro", Column = "GUA_MOTORISTA_TERCEIRO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MotoristaTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Reboques", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GUARITA_TMS_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GUA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Reboques { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual string VeiculoComReboque
        {
            get
            {
                return Veiculo?.Placa + (Reboques.Count > 0 ? " (" + string.Join(", ", Reboques.Select(o => o.Placa)) + ")" : string.Empty) ?? string.Empty;
            }
        }

        public virtual bool Equals(GuaritaTMS other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
