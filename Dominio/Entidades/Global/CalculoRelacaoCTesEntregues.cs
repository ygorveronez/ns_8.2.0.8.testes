using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CALCULO_RELACAO_CTES_ENTREGUES", EntityName = "CalculoRelacaoCTesEntregues", Name = "Dominio.Entidades.CalculoRelacaoCTesEntregues", NameType = typeof(CalculoRelacaoCTesEntregues))]
    public class CalculoRelacaoCTesEntregues : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_EMISSOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Emissor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_VALOR_DIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_VALOR_MEIADIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorMeiaDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_PERCENTUAL_POR_CTE", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal PercentualPorCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_VALOR_MINIMO_POR_CTE", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorMinimoPorCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_VALOR_MINIMO_CTE_MESMO_DESTINO", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorMinimoCTeMesmoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_FRACAO_KG", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal FracaoKG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_VALOR_POR_FRACAO", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorPorFracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_VALOR_POR_FRACAO_EM_ENTREGAS_IGUAIS", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorPorFracaoEmEntregasIguais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_FRANQUIA_KM", TypeType = typeof(int), NotNull = false)]
        public virtual int FranquiaKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_VALOR_KM_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorKMExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_COLETA_VALOR_POR_EVENTO", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ColetaValorPorEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_COLETA_FRACAO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ColetaFracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_COLETA_VALOR_POR_FRACAO", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ColetaValorPorFracao { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "PercentualCidades", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CALCULO_RELACAO_CTES_ENTREGUES_CIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CalculoRelacaoCTesEntreguesCidade", Column = "CCC_CODIGO")]
        public virtual IList<Dominio.Entidades.CalculoRelacaoCTesEntreguesCidade> PercentualCidades { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Cliente?.Descricao ?? string.Empty;
            }
        }

        public virtual Dominio.ObjetosDeValor.CalculoRelacaoCTesEntregues ObjetoCalculo() 
        {
            return new ObjetosDeValor.CalculoRelacaoCTesEntregues()
            {
                ValorDiaria = this.ValorDiaria,
                ValorMeiaDiaria = this.ValorMeiaDiaria,
                PercentualPorCTe = this.PercentualPorCTe,
                ValorMinimoPorCTe = this.ValorMinimoPorCTe,
                ValorMinimoCTeMesmoDestino = this.ValorMinimoCTeMesmoDestino,
                FracaoKG = this.FracaoKG,
                ValorPorFracao = this.ValorPorFracao,
                ValorPorFracaoEmEntregasIguais = this.ValorPorFracaoEmEntregasIguais,
                ValorKMExcedente = this.ValorKMExcedente,
                ColetaValorPorEvento = this.ColetaValorPorEvento,
                ColetaFracao = this.ColetaFracao,
                ColetaValorPorFracao = this.ColetaValorPorFracao,
                FranquiaKM = this.FranquiaKM,

                Cidades = (from obj in this.PercentualCidades
                           select new Dominio.ObjetosDeValor.CalculoRelacaoCTesEntreguesCidade
                           {
                               Cidade = obj.Cidade.Codigo,
                               Percentual = obj.Percentual,
                           }).ToList()
            };
        }
    }
}
