namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AVON_MANIFESTO_DOCUMENTO", EntityName = "DocumentoManifestoAvon", Name = "Dominio.Entidades.DocumentoManifestoAvon", NameType = typeof(DocumentoManifestoAvon))]
    public class DocumentoManifestoAvon : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoAvon", Column = "MAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoAvon Manifesto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MAD_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "MAD_SERIE", TypeType = typeof(int), NotNull = true)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "MAD_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "MAD_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Documento", Column = "MAD_DOCUMENTO", Type = "StringClob", NotNull = true)]
        public virtual string Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MAD_STATUS", TypeType = typeof(Enumeradores.StatusDocumentoManifestoAvon), NotNull = true)]
        public virtual Enumeradores.StatusDocumentoManifestoAvon Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "MAD_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "MAD_PROBLEMA_INTEGRACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ProblemaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidEnvio", Column = "MAD_GUID_ENVIO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string GuidEnvio { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusDocumentoManifestoAvon.Finalizado:
                        return "Finalizado";
                    case Enumeradores.StatusDocumentoManifestoAvon.Emitido:
                        return "Emitido";
                    case Enumeradores.StatusDocumentoManifestoAvon.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusDocumentoManifestoAvon.FalhaNoRetorno:
                        return "Falha ao enviar retorno";
                    default:
                        return "";
                }
            }
        }

        public virtual string MesagemAvon
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusDocumentoManifestoAvon.Finalizado:
                        return "Conhecimento processado com êxito";
                    case Enumeradores.StatusDocumentoManifestoAvon.Emitido:
                        return "Aguardando o envio para Avon";
                    case Enumeradores.StatusDocumentoManifestoAvon.Enviado:
                        return "Aguardando a autorização do CT-e";
                    case Enumeradores.StatusDocumentoManifestoAvon.FalhaNoRetorno:
                        return this.ProblemaIntegracao;
                    default:
                        return "";
                }
            }
        }
    }
}
