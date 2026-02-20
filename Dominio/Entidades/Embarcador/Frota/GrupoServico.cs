using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_SERVICO", EntityName = "GrupoServico", Name = "Dominio.Entidades.Embarcador.Frota.GrupoServico", NameType = typeof(GrupoServico))]
    public class GrupoServico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GSF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GSF_DESCRICAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GSF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "GSF_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KmInicial", Column = "GSF_KM_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KmInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KmFinal", Column = "GSF_KM_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KmFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaInicial", Column = "GSF_DIA_INICIAL", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaFinal", Column = "GSF_DIA_FINAL", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoVeiculoEquipamento", Column = "GSF_TIPO_VEICULO_EQUIPAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.VeiculoEquipamento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.VeiculoEquipamento TipoVeiculoEquipamento { get; set; }

        [Obsolete("Migrado para lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeiculo", Column = "VMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeiculo ModeloVeiculo { get; set; }

        [Obsolete("Migrado para lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaVeiculo", Column = "VMA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MarcaVeiculo MarcaVeiculo { get; set; }

        [Obsolete("Migrado para lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaEquipamento", Column = "EQM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculos.MarcaEquipamento MarcaEquipamento { get; set; }

        [Obsolete("Migrado para lista")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloEquipamento", Column = "EMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculos.ModeloEquipamento ModeloEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiModelosVeiculo", Column = "GSF_POSSUI_MODELOS_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiModelosVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiMarcasVeiculo", Column = "GSF_POSSUI_MARCAS_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiMarcasVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiMarcasEquipamento", Column = "GSF_POSSUI_MARCAS_EQUIPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiMarcasEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiModelosEquipamento", Column = "GSF_POSSUI_MODELOS_EQUIPAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiModelosEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "GSF_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrotaTipo", Column = "FOT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo TipoOrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ServicosVeiculo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_SERVICO_SERVICO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GSF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoServicoServicoVeiculo", Column = "GSV_CODIGO")]
        public virtual IList<GrupoServicoServicoVeiculo> ServicosVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "MarcasVeiculo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_SERVICO_MARCA_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GSF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MarcaVeiculo", Column = "VMA_CODIGO")]
        public virtual ICollection<MarcaVeiculo> MarcasVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeiculo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_SERVICO_MODELO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GSF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeiculo", Column = "VMO_CODIGO")]
        public virtual ICollection<ModeloVeiculo> ModelosVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "MarcasEquipamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_SERVICO_MARCA_EQUIPAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GSF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MarcaEquipamento", Column = "EQM_CODIGO")]
        public virtual ICollection<Veiculos.MarcaEquipamento> MarcasEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosEquipamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GRUPO_SERVICO_MODELO_EQUIPAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GSF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloEquipamento", Column = "EMO_CODIGO")]
        public virtual ICollection<Veiculos.ModeloEquipamento> ModelosEquipamento { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
