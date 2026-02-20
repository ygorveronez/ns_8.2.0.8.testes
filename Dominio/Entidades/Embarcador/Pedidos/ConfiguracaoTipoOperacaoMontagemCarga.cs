using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TIPO_OPERACAO_MONTAGEM_CARGA", DynamicUpdate = true, EntityName = "ConfiguracaoTipoOperacaoMontagemCarga", Name = "Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMontagemCarga", NameType = typeof(ConfiguracaoTipoOperacaoMontagemCarga))]
    public class ConfiguracaoTipoOperacaoMontagemCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarPedidosMontagemAoFinalizarTransporte", Column = "CMC_DISPONIBILIZAR_PEDIDO_AO_FINALIZAR_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarPedidosMontagemAoFinalizarTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExibirPedidosMontagemIntegracao", Column = "CMC_EXIBIR_PEDIDOS_AO_INTEGRAR_TRANSPORTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirPedidosMontagemIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisponibilizarPedidosMontagemDeterminadosTransportadores", Column = "CMC_DISPONIBILIZAR_PEDIDOS_DETERMINADOS_TRANSPORTADORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DisponibilizarPedidosMontagemDeterminadosTransportadores { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "TransportadoresMontagemCarga", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_TIPO_OPERACAO_MONTAGEM_CARGA_DETERMINADOS_TRANSPORTADORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Empresa> TransportadoresMontagemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_OCULTAR_TIPO_DE_OPERACAO_NA_MONTAGEM_DA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarTipoDeOperacaoNaMontagemDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlarCapacidadePorUnidade", Column = "CMC_CONTROLAR_CAPACIDADE_POR_UNIDADE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarCapacidadePorUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigirInformarDataPrevisaoInicioViagem", Column = "CMC_EXIGIR_INFORMAR_DATA_PREVISAO_INICIO_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirInformarDataPrevisaoInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RoteirizarNovamenteAoConfirmarDocumentos", Column = "CMC_ROTEIRIZAR_NOVAMENTE_AO_CONFIRMAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RoteirizarNovamenteAoConfirmarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_MONTAGEM_COM_RECEBEDOR_NAO_GERAR_CARGA_COMO_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemComRecebedorNaoGerarCargaComoColeta { get; set; }

        public virtual string Descricao
        {
            get { return "Configurações tipo operação da montagem de carga."; }
        }
    }
}