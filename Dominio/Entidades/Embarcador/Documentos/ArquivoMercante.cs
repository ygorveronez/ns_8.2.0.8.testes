using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_MERCANTE", EntityName = "ArquivoMercante", Name = "Dominio.Entidades.Embarcador.Documentos.ArquivoMercante", NameType = typeof(ArquivoMercante))]

    public class ArquivoMercante : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "AME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAbsorcao", Column = "AME_DATA_ABSORCAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAbsorcao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusRetorno", Column = "AME_STATUS_ARQUIVO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string StatusRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "AME_NOME_ARQUIVO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIMO", Column = "AME_CODIGO_IMO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIMO { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPortoOrigem", Column = "AME_CODIGO_PORTO_ORIGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoPortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPortoDestino", Column = "AME_CODIGO_PORTO_DESTINO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoPortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagem", Column = "AME_NUMERO_VIAGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DirecaoViagem", Column = "AME_DIRECAO_VIAGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string DirecaoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArquivo", Column = "AME_TIPO_ARQUIVO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "AME_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "AME_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "IMO: " + CodigoIMO + " Nº " + NumeroViagem + " D: " + DirecaoViagem + " PO: " + CodigoPortoOrigem + " PD: " + CodigoPortoDestino;
            }
        }
    }
}
