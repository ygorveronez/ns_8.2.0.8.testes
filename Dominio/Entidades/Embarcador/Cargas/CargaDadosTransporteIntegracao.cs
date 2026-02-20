using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_DADOS_TRANSPORTE_INTEGRACAO", EntityName = "CargaDadosTransporteIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao", NameType = typeof(CargaDadosTransporteIntegracao))]
    public class CargaDadosTransporteIntegracao : Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<CargaDadosTransporteIntegracao>
    {
        public CargaDadosTransporteIntegracao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_DADOS_TRANSPORTE_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CDI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDI_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDI_PRE_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PreProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoColeta", Column = "CDI_INTEGRACAO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoColeta { get; set; }

        [Obsolete("Não é mais usado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "MeioPagamentoDigitalCom", Column = "CDI_MEIO_PAGAMENTO_DIGITALCOM", TypeType = typeof(MeiosPagamentoDigitalCom), NotNull = false)]
        public virtual MeiosPagamentoDigitalCom? MeioPagamentoDigitalCom { get; set; }

        [Obsolete("Não é mais usado")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "IDRetornoDigitalCom", Column = "CDI_ID_RETORNO_DIGITALCOM", TypeType = typeof(int), NotNull = false)]
        public virtual int IDRetornoDigitalCom { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AguardarFinalizarCargaAnterior", Column = "CDI_AGUARDAR_FINALIZAR_CARGA_ANTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AguardarFinalizarCargaAnterior { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_CARGA_PENDENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga CargaPendente { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Protocolo;
            }
        }

        public virtual bool Equals(CargaDadosTransporteIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
