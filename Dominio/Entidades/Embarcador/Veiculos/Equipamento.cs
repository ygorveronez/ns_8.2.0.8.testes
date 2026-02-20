using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EQUIPAMENTO", EntityName = "Equipamento", Name = "Dominio.Entidades.Embarcador.Veiculos.Equipamento", NameType = typeof(Equipamento))]
    public class Equipamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Veiculos.Equipamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EQP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "EQP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "EQP_NUMERO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chassi", Column = "EQP_CHASSI", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Chassi { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "EQP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MarcaEquipamento", Column = "EQM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.MarcaEquipamento MarcaEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloEquipamento", Column = "EMO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.ModeloEquipamento ModeloEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SegmentoVeiculo", Column = "VSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Veiculos.SegmentoVeiculo SegmentoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Hodometro", Column = "EQP_HODOMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Hodometro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Horimetro", Column = "EQP_HORIMETRO", TypeType = typeof(int), NotNull = false)]
        public virtual int Horimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAquisicao", Column = "EQP_DATA_AQUISICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAquisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnoFabricacao", Column = "EQP_ANO_FABRICACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int AnoFabricacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnoModelo", Column = "EQP_ANO_MODELO", TypeType = typeof(int), NotNull = false)]
        public virtual int AnoModelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "EQP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EquipamentoAceitaAbastecimento", Column = "EQP_EQUIPAMENTO_ACEITA_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EquipamentoAceitaAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ViradaHodometro", Column = "EQP_VIRADA_HODOMETRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ViradaHodometro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroVirada", Column = "EQP_KM_VIRADA", TypeType = typeof(int), NotNull = false)]
        public virtual int HorimetroVirada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizaTanqueCompartilhado", Column = "EQP_UTILIZA_TANQUE_COMPARTILHADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizaTanqueCompartilhado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EQP_MEDIA_PADRAO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MediaPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeTanque", Column = "EQP_CAPACIDADE_TANQUE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal CapacidadeTanque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeMaximaTanque", Column = "EQP_CAPACIDADE_MAXIMA_TANQUE", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal CapacidadeMaximaTanque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Neokohm", Column = "EQP_NEOKOHM", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao Neokohm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "EQP_COR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Renavam", Column = "EQP_RENAVAM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Renavam { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "EQP_INTEGRADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TrocaHorimetro", Column = "EQP_TROCA_HORIMETRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TrocaHorimetro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorimetroAtual", Column = "EQP_HORIMETRO_ATUAL", TypeType = typeof(int), NotNull = false)]
        public virtual int HorimetroAtual { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "HistoricoHorimetros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True)]
        [NHibernate.Mapping.Attributes.Key(1, Column = "EQP_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "HistoricoHorimetro")]
        public virtual IList<HistoricoHorimetro> HistoricoHorimetros { get; set; }

        public virtual int HorimetroAtualHistoricoHorimetro
        {
            get
            {
                return HistoricoHorimetros?.OrderByDescending(h => h.DataAlteracao).OrderByDescending(h => h.Codigo)?.FirstOrDefault()?.HorimetroAtual ?? 0;
            }
        }

        public virtual string DescricaoComMarcaModelo
        {
            get
            {
                return Descricao + (MarcaEquipamento != null && ModeloEquipamento != null ? " (" + MarcaEquipamento.Descricao + " " + ModeloEquipamento.Descricao + ")" : string.Empty);
            }
        }

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

        public virtual bool Equals(Equipamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
