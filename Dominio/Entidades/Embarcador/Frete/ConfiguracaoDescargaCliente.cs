using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_DESCARGA_CLIENTE", EntityName = "ConfiguracaoDescargaCliente", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ConfiguracaoDescargaCliente", NameType = typeof(ConfiguracaoDescargaCliente))]
    public class ConfiguracaoDescargaCliente : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoDeCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDC_VALOR_TONELADA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTonelada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDC_VALOR_UNIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDC_VALOR_PALLET", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDC_VALOR_AJUDANTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAjudante { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "CDC_VALOR_PRE_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorDePreDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioVigencia", Column = "CDC_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? InicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FimVigencia", Column = "CDC_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? FimVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "CDC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAprovacao", Column = "CDC_DATA_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInativacao", Column = "CDC_DATA_INATIVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInativacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaAlteracao", Column = "CDC_DATA_ULTIMA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFretePreDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CFR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteConfiguracaoDescargaCliente), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteConfiguracaoDescargaCliente Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Clientes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_DESCARGA_CLIENTE_CLIENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Cliente", Column = "CLI_CGCCPF")]
        public virtual ICollection<Dominio.Entidades.Cliente> Clientes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "GrupoPessoas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_DESCARGA_CLIENTE_GRUPO_PESSOA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GrupoPessoas", Column = "GRP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_DESCARGA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TiposOperacoes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_DESCARGA_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Empresa> Transportadores { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Filial - {this.Filial?.Descricao ?? string.Empty}";
            }
        }

        public virtual string DescricaoVigencia
        {
            get
            {
                if (InicioVigencia.HasValue && FimVigencia.HasValue)
                    return $"De {InicioVigencia.Value.ToDateString()} até {FimVigencia.Value.ToDateString()}";

                if (InicioVigencia.HasValue)
                    return $"À partir de {InicioVigencia.Value.ToDateString()}";

                if (FimVigencia.HasValue)
                    return $"Até {FimVigencia.Value.ToDateString()}";

                return string.Empty;
            }
        }
    }
}