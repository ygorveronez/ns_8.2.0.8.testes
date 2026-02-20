using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO", EntityName = "PeriodoDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento", NameType = typeof(PeriodoDescarregamento))]
    public class PeriodoDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExcecaoCapacidadeDescarregamento", Column = "CEX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ExcecaoCapacidadeDescarregamento ExcecaoCapacidadeDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "PED_DIA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaDoMes", Column = "PED_DIA_DO_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaDoMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mes", Column = "PED_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeDescarregamentoSimultaneo", Column = "PED_CAPACIDADE_DESCARREGAMENTO_SIMULTANEO", TypeType = typeof(int), NotNull = true)]
        public virtual int CapacidadeDescarregamentoSimultaneo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeDescarregamentoSimultaneoAdicional", Column = "PED_CAPACIDADE_DESCARREGAMENTO_SIMULTANEO_ADICIONAL", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamentoSimultaneoAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_CAPACIDADE_DESCARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int CapacidadeDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaExcessoTempo", Column = "PED_TOLERANCIA_EXCESSO_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int ToleranciaExcessoTempo { get; set; }

        //Campos Não Utilizados
        //[NHibernate.Mapping.Attributes.Property(0, Column = "PED_VOLUMES_DE", TypeType = typeof(int), NotNull = false)]
        //public virtual int? VolumesDe { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "PED_VOLUMES_ATE", TypeType = typeof(int), NotNull = false)]
        //public virtual int? VolumesAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_SKU_DE", TypeType = typeof(int), NotNull = false)]
        public virtual int? SkuDe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_SKU_ATE", TypeType = typeof(int), NotNull = false)]
        public virtual int? SkuAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicio", Column = "PED_HORA_INICIO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraTermino", Column = "PED_HORA_TERMINO", Type = "NHibernate.Type.TimeAsTimeSpanType", NotNull = true)]
        public virtual TimeSpan HoraTermino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "GruposPessoas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_GRUPO_PESSOA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoDescarregamentoGrupoPessoa", Column = "PDG_CODIGO")]
        public virtual ICollection<PeriodoDescarregamentoGrupoPessoa> GruposPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Remetentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_REMETENTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoDescarregamentoRemetente", Column = "PDR_CODIGO")]
        public virtual ICollection<PeriodoDescarregamentoRemetente> Remetentes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TiposDeCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_TIPO_DE_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoDescarregamentoTipoDeCarga", Column = "PDT_CODIGO")]
        public virtual ICollection<PeriodoDescarregamentoTipoDeCarga> TiposDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CanaisVenda", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_CANAL_VENDA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoDescarregamentoCanalVenda", Column = "PCV_CODIGO")]
        public virtual ICollection<PeriodoDescarregamentoCanalVenda> CanaisVenda { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "GruposProdutos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO_GRUPO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PeriodoDescarregamentoGrupoProduto", Column = "PDG_CODIGO")]
        public virtual ICollection<PeriodoDescarregamentoGrupoProduto> GruposProdutos { get; set; }

        public virtual string Descricao
        {
            get
            {
                if (this.CentroDescarregamento?.CapacidadeDescaregamentoPorDia ?? false)
                    return $"{DiaDoMes.ToString().PadLeft(2, '0')}/{Mes.ToString().PadLeft(2, '0')} - {HoraInicio.ToString(@"hh\:mm")} até {HoraTermino.ToString(@"hh\:mm")}";
                else
                    return $"{Dia.ObterDescricaoResumida()} {HoraInicio.ToString(@"hh\:mm")} até {HoraTermino.ToString(@"hh\:mm")}";
            }
        }

        public virtual string DescricaoPeriodo
        {
            get { return $"Das {HoraInicio.ToString(@"hh\:mm")} as {HoraTermino.ToString(@"hh\:mm")}"; }
        }

        public virtual int CapacidadeDescarregamentoSimultaneoTotal
        {
            get { return CapacidadeDescarregamentoSimultaneo + CapacidadeDescarregamentoSimultaneoAdicional; }
        }
    }
}
