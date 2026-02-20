using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AVON_MANIFESTO", EntityName = "ManifestoAvon", Name = "Dominio.Entidades.ManifestoAvon", NameType = typeof(ManifestoAvon))]
    public class ManifestoAvon : EntidadeBase, IEquatable<ManifestoAvon>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FretePorTipoDeVeiculo", Column = "FTV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FretePorTipoDeVeiculo TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MAV_NUMERO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "MAV_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "MAV_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAReceber", Column = "MAV_VALOR_RECEBER", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "MAV_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MAV_STATUS", TypeType = typeof(Enumeradores.StatusManifestoAvon), NotNull = true)]
        public virtual Enumeradores.StatusManifestoAvon Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegradora", Column = "MAV_TIPO_INTEGRADORA", TypeType = typeof(ObjetosDeValor.Enumerador.TipoIntegradoraManifesto), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.TipoIntegradoraManifesto TipoIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Faturas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AVON_FATURA_MANIFESTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaAvon", Column = "FAV_CODIGO")]
        public virtual ICollection<Dominio.Entidades.FaturaAvon> Faturas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AVON_MANIFESTO_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoManifestoAvon", Column = "MAD_CODIGO")]
        public virtual IList<Dominio.Entidades.DocumentoManifestoAvon> Documentos { get; set; }

        public virtual bool Equals(ManifestoAvon other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusManifestoAvon.Finalizado:
                        return "Finalizado";
                    case Enumeradores.StatusManifestoAvon.Emitido:
                        return "Emitido";
                    case Enumeradores.StatusManifestoAvon.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusManifestoAvon.FalhaNoRetorno:
                        return "Falha ao enviar Retorno";
                    default:
                        return "";
                }
            }
        }

    }
}
