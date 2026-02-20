using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO", EntityName = "ContratoFreteTransportadorValorFreteMinimo", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo", NameType = typeof(ContratoFreteTransportadorValorFreteMinimo))]
    public class ContratoFreteTransportadorValorFreteMinimo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFreteTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVM_VALOR_MINIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorMinimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiClientesDestino", Column = "CVM_POSSUI_CLIENTES_DESTINO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiClientesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiClientesOrigem", Column = "CVM_POSSUI_CLIENTES_ORIGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiClientesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiEstadosDestino", Column = "CVM_POSSUI_ESTADOS_DESTINO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiEstadosDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiEstadosOrigem", Column = "CVM_POSSUI_ESTADOS_ORIGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiEstadosOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiLocalidadesDestino", Column = "CVM_POSSUI_LOCALIDADES_DESTINO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiLocalidadesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiLocalidadesOrigem", Column = "CVM_POSSUI_LOCALIDADES_ORIGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiLocalidadesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiModelosVeicularesCarga", Column = "CVM_POSSUI_MODELOS_VEICULARES_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiModelosVeicularesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiTiposCarga", Column = "CVM_POSSUI_TIPOS_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PossuiTiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO_CLIENTE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ClientesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO_CLIENTE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Cliente> ClientesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO_ESTADO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "EstadosOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO_ESTADO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Estado", Column = "UF_SIGLA")]
        public virtual ICollection<Estado> EstadosOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LocalidadesDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO_LOCALIDADE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> LocalidadesDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LocalidadesOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO_LOCALIDADE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Localidade", Column = "LOC_CODIGO")]
        public virtual ICollection<Localidade> LocalidadesOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeicularesCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO_MODELO_VEICULAR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosVeicularesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALOR_FRETE_MINIMO_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Cargas.TipoDeCarga> TiposCarga { get; set; }
    }
}
