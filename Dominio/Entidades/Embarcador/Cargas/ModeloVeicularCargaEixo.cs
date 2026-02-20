using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODELO_VEICULAR_CARGA_EIXO", EntityName = "ModeloVeicularCargaEixo", Name = "Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo", NameType = typeof(ModeloVeicularCargaEixo))]
    public class ModeloVeicularCargaEixo : EntidadeBase, IEquatable<ModeloVeicularCargaEixo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MEX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MEX_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoPneuNovoKm", Column = "MEX_PREVISAO_PNEU_NOVO_KM", TypeType = typeof(int), NotNull = true)]
        public virtual int PrevisaoPneuNovoKm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoReformaKm", Column = "MEX_PREVISAO_REFORMA_KM", TypeType = typeof(int), NotNull = true)]
        public virtual int PrevisaoReformaKm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoRodizioKm", Column = "MEX_PREVISAO_RODIZIO_KM", TypeType = typeof(int), NotNull = true)]
        public virtual int PrevisaoRodizioKm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePneu", Column = "MEX_QUANTIDADE_PNEU", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.QuantidadePneuEixo QuantidadePneu { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MEX_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEixo), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoEixo Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaPneuNovoKm", Column = "MEX_TOLERANCIA_PNEU_NOVO_KM", TypeType = typeof(int), NotNull = true)]
        public virtual int ToleranciaPneuNovoKm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaReformaKm", Column = "MEX_TOLERANCIA_REFORMA_KM", TypeType = typeof(int), NotNull = true)]
        public virtual int ToleranciaReformaKm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaRodizioKm", Column = "MEX_TOLERANCIA_RODIZIO_KM", TypeType = typeof(int), NotNull = true)]
        public virtual int ToleranciaRodizioKm { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoPneuNovoDia", Column = "MEX_PREVISAO_PNEU_NOVO_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrevisaoPneuNovoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoReformaDia", Column = "MEX_PREVISAO_REFORMA_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrevisaoReformaDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrevisaoRodizioDia", Column = "MEX_PREVISAO_RODIZIO_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrevisaoRodizioDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaPneuNovoDia", Column = "MEX_TOLERANCIA_PNEU_NOVO_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaPneuNovoDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaReformaDia", Column = "MEX_TOLERANCIA_REFORMA_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaReformaDia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaRodizioDia", Column = "MEX_TOLERANCIA_RODIZIO_DIA", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaRodizioDia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Pneus", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MODELO_VEICULAR_CARGA_EIXO_PNEU")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MEX_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCargaEixoPneu", Column = "MEP_CODIGO")]
        public virtual ICollection<ModeloVeicularCargaEixoPneu> Pneus { get; set; }

        public virtual string Descricao
        {
            get { return $"Eixo {Numero}"; }
        }

        public virtual bool Equals(ModeloVeicularCargaEixo other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
