using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUARITA_CHECK_LIST", EntityName = "GuaritaCheckList", Name = "Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList", NameType = typeof(GuaritaCheckList))]
    public class GuaritaCheckList : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_OPERADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLC_DATA_ABERTURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GuaritaTMS", Column = "GUA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.GuaritaTMS Guarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMAtual", Column = "GCL_KM_ATUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GCL_ENTRADA_SAIDA", TypeType = typeof(TipoEntradaSaida), NotNull = false)]
        public virtual TipoEntradaSaida TipoEntradaSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "GCL_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrota OrdemServicoFrota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Perguntas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GUARITA_CHECK_LIST_PERGUNTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GuaritaCheckListPerguntas", Column = "GPE_CODIGO")]
        public virtual IList<GuaritaCheckListPerguntas> Perguntas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GUARITA_CHECK_LIST_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GuaritaCheckListAnexo", Column = "GLA_CODIGO")]
        public virtual IList<GuaritaCheckListAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GCL_TIPO_CHECK", TypeType = typeof(TipoCheckListGuarita), NotNull = false)]
        public virtual TipoCheckListGuarita TipoCheckListGuarita { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CheckListTipo", Column = "CLT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CheckListTipo CheckListTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "GCL_SITUACAO", TypeType = typeof(SituacaoGuaritaCheckList), NotNull = false)]
        public virtual SituacaoGuaritaCheckList Situacao { get; set; }

        #region Manutenção Veículo

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarOS", Column = "GCL_GERAR_OS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramada", Column = "GCL_DATA_PROGRAMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProgramada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipo", Column = "FOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrotaTipo TipoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente LocalManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOS", Column = "GCL_OBSERVACAO_OS", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoOS { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Servicos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GUARITA_CHECK_LIST_SERVICO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GuaritaCheckListServicoVeiculo", Column = "GSV_CODIGO")]
        public virtual IList<GuaritaCheckListServicoVeiculo> Servicos { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        #region Abastecimento

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarAbastecimento", Column = "GCL_GERAR_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAbastecimento", Column = "GCL_TIPO_ABASTECIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento? TipoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Litros", Column = "GCL_LITROS_ABASTECIDO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Litros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "GCL_VALOR_UNITARIO_ABASTECIMENTO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_POSTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Posto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Abastecimento", Column = "ABA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Abastecimento Abastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Horimetro", Column = "GCL_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Horimetro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }

        public virtual decimal ValorTotal
        {
            get { return this.Litros * this.ValorUnitario; }
        }

        #endregion

        #region Manutenção Equipamento

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO_EQUIPAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrota OrdemServicoFrotaEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarOSEquipamento", Column = "GCL_GERAR_OS_EQUIPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOSEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramadaEquipamento", Column = "GCL_DATA_PROGRAMADA_EQUIPAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProgramadaEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipo", Column = "FOT_CODIGO_EQUIPAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrotaTipo TipoOrdemServicoEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_EQUIPAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente LocalManutencaoEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Equipamento", Column = "EQP_CODIGO_SERVICO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Equipamento EquipamentoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOSEquipamento", Column = "GCL_OBSERVACAO_OS_EQUIPAMENTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ObservacaoOSEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ServicosEquipamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GUARITA_CHECK_LIST_SERVICO_EQUIPAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GuaritaCheckListServicoEquipamento", Column = "GSE_CODIGO")]
        public virtual IList<GuaritaCheckListServicoEquipamento> ServicosEquipamento { get; set; }

        #endregion

        public virtual string Descricao
        {
            get
            {
                return this.Guarita?.Carga?.CodigoCargaEmbarcador ?? string.Empty;
            }
        }

        public virtual string DescricaoTipoEntradaSaida
        {
            get { return TipoEntradaSaida.ObterDescricao(); }
        }
    }
}