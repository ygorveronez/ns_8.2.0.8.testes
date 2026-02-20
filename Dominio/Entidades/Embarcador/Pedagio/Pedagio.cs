using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDAGIO", EntityName = "Pedagio", Name = "Dominio.Entidades.Embarcador.Pedagio.Pedagio", NameType = typeof(Pedagio))]
    public class Pedagio : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedagio.Pedagio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculoNaoCadastrado", Column = "PED_PLACA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string PlacaVeiculoNaoCadastrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tag", Column = "PED_TAG", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Tag { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prefixo", Column = "PED_PREFIXO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Prefixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MarcaVeiculo", Column = "PED_MARCA_VEICULO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string MarcaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Categoria", Column = "PED_CATEGORIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Categoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PED_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        /// <summary>
        /// Utilizada para controlar a sumarização dos dados para análise de resultados
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PED_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rodovia", Column = "PED_RODOVIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Rodovia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Praca", Column = "PED_PRACA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Praca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PED_VALOR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImportadoDeSemParar", Column = "PED_IMPORTADO_SEM_PARAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImportadoDeSemParar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPedagio", Column = "PED_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio SituacaoPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPedagio", Column = "PED_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio TipoPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoPedagio", Column = "FPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frotas.FechamentoPedagio FechamentoPedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "PED_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "PED_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "PED_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "PED_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "PED_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PED_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AcertosViagem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACERTO_PEDAGIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcertoPedagio", Column = "ACP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> AcertosViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContagemDuplicado", Formula = @"ISNULL((SELECT Count(*) 
                        FROM T_PEDAGIO A
                        WHERE A.PED_DATA = PED_DATA AND A.VEI_CODIGO = VEI_CODIGO 
                        GROUP BY A.PED_DATA, A.VEI_CODIGO HAVING Count(*) > 1), 0)", TypeType = typeof(long), Lazy = true)]
        public virtual long ContagemDuplicado { get; set; }

        public virtual string Descricao
        {
            get { return this.Veiculo?.Placa; }
        }

        public virtual string DescricaoSituacaoPedagio
        {
            get { return SituacaoPedagio.ObterDescricao(); }
        }

        public virtual bool Equals(Pedagio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}