using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PORTO", EntityName = "Porto", Name = "Dominio.Entidades.Embarcador.Pedidos.Porto", NameType = typeof(Porto))]
    public class Porto : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.Porto>
    
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "POT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "POT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "POT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "POT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIATA", Column = "POR_CODIGO_IATA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIATA { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaEmissaoSVM", Column = "POT_FORMA_EMISSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaEmissaoSVM), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaEmissaoSVM FormaEmissaoSVM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumento", Column = "POT_CODIGO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMercante", Column = "POT_CODIGO_MERCANTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoMercante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeHorasFaturamentoAutomatico", Column = "POT_QUANTIDADE_HORAS_FATURAMENTO_AUTOMATICO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeHorasFaturamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtivarDespachanteComoConsignatario", Column = "POT_ATIVAR_DESPACHANTE_COMO_CONSIGNATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtivarDespachanteComoConsignatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DividirCargasAcordoComQuantidadeContainerRecebidoPortoDestino", Column = "POT_DIVIDIR_CARGAS_ACORDO_COM_QUANTIDADE_CONTAINER_RECEBIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DividirCargasAcordoComQuantidadeContainerRecebidoPortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DividirCargasAcordoComQuantidadeContainerRecebidoPortoOrigem", Column = "POT_DIVIDIR_CARGAS_ACORDO_COM_QUANTIDADE_CONTAINER_RECEBIDO_PORTO_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DividirCargasAcordoComQuantidadeContainerRecebidoPortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "POT_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAntesDoPodParaEnvioDaDocumentacao", Column = "POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasAntesDoPodParaEnvioDaDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RKST", Column = "POT_RKST", NotNull = false)]
        public virtual string RKST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CriarSequenciaCargasMesmoComPedidoExistente", Column = "POT_CRIAR_SEQUENCIA_CARGAS_MESMO_COM_PEDIDO_EXISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CriarSequenciaCargasMesmoComPedidoExistente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarUmaCargaSVMPorCargaMTLQuandoPortoDestino", Column = "POT_GEARAR_UMA_CARGA_SVM_POR_CARGA_MTL_QUANDO_PORTO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarUmaCargaSVMPorCargaMTLQuandoPortoDestino { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarUmaCargaSVMPorCargaMTLQuandoPortoOrigem", Column = "POT_GEARAR_UMA_CARGA_SVM_POR_CARGA_MTL_QUANDO_PORTO_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarUmaCargaSVMPorCargaMTLQuandoPortoOrigem { get; set; }

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

        public virtual bool Equals(Porto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
