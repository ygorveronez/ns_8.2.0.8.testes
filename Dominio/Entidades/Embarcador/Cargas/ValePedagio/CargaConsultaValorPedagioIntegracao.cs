using System;
using System.Collections.Generic;
using Dominio.Interfaces.Embarcador.Integracao;

namespace Dominio.Entidades.Embarcador.Cargas.ValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CONSULTA_VALOR_PEDAGIO_INTEGRACAO ", EntityName = "CargaConsultaValorPedagioIntegracao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaConsultaValorPedagioIntegracao", NameType = typeof(CargaConsultaValorPedagioIntegracao))]
    public class CargaConsultaValorPedagioIntegracao : Integracao.Integracao, IEquatable<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao>, IIntegracaoComArquivo<CargaValePedagioIntegracaoArquivo>
    {
        public CargaConsultaValorPedagioIntegracao()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroValePedagio", Column = "CCP_NUMERO_VALE_PEDAGIO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorValePedagio", Column = "CCP_VALOR_VALE_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRoteiro", Column = "CCP_CODIGO_ROTEIRO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoRoteiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPercurso", Column = "CCP_CODIGO_PERCURSO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoPercurso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEixos", Column = "CCP_QUANTIDADE_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEixos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CCP_TIPO_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoValePedagio", Column = "CCP_CODIGO_INTEGRACAO_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CONSULTA_VALOR_PEDAGIO_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaValePedagioIntegracaoArquivo", Column = "CVI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(CargaConsultaValorPedagioIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}

