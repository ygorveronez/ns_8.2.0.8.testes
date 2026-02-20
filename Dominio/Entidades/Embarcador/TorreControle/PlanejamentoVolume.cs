using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.TorreControle
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PLANEJAMENTO_VOLUME", EntityName = "PlanejamentoVolume", Name = "Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume", NameType = typeof(PlanejamentoVolume))]
    public class PlanejamentoVolume : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PLV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramacaoCargaInicial", Column = "PLV_DATA_PROGRAMACAO_CARGA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataProgramacaoCargaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProgramacaoCargaFinal", Column = "PLV_DATA_PROGRAMACAO_CARGA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataProgramacaoCargaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalToneladasMes", Column = "PLV_TOTAL_TONELADAS_MES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalToneladasMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalTransferenciaEntrePlantas", Column = "PLV_TOTAL_TRANSFERENCIA_ENTRE_PLANTAS", TypeType = typeof(int), NotNull = false)]
        public virtual int TotalTransferenciaEntrePlantas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "PLV_PESO_BRUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilidadePlacas", Column = "PLV_DISPONIBILIDADE_PLACAS", TypeType = typeof(int), NotNull = false)]
        public virtual int DisponibilidadePlacas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContrato", Column = "PLV_NUMERO_CONTRATO", TypeType = typeof(string), NotNull = false)]
        public virtual string NumeroContrato { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Origens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PLANEJAMENTO_VOLUME_ORIGENS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PLV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Localidade> Origens { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PLANEJAMENTO_VOLUME_DESTINOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PLV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Localidade> Destinos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Remetentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PLANEJAMENTO_VOLUME_REMETENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PLV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Remetentes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Destinatarios", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PLANEJAMENTO_VOLUME_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PLV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Destinatarios { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
