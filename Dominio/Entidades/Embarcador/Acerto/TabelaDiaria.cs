using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_DIARIA", EntityName = "TabelaDiaria", Name = "Dominio.Entidades.Embarcador.Acerto.TabelaDiaria", NameType = typeof(TabelaDiaria))]
    public class TabelaDiaria : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TAD_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TAD_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SegmentoVeiculo", Column = "VSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.SegmentoVeiculo SegmentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Periodos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_DIARIA_PERIODO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaDiaria", Column = "TDP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo> Periodos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarMovimentoSaidaFixaMotorista", Column = "TAD_GERAR_MOVIMENTO_SAIDA_FIXA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarMovimentoSaidaFixaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaInicial", Column = "TAD_DATA_VIGENCIA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigenciaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaFinal", Column = "TAD_DATA_VIGENCIA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigenciaFinal { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual bool Equals(TabelaDiaria other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
