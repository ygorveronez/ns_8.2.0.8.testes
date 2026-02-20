using Dominio.Enumeradores;
using NHibernate.Mapping.Attributes;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [Class(0, Table = "T_EDI_LAYOUT", EntityName = "LayoutEDI", Name = "Dominio.Entidades.LayoutEDI", NameType = typeof(LayoutEDI))]
    public class LayoutEDI : EntidadeBase
    {
        [Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAY_CODIGO")]
        [Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [Property(0, Name = "Descricao", Column = "LAY_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = true)]
        public virtual string Descricao { get; set; }

        [Property(0, Name = "Tipo", Column = "LAY_TIPO", TypeType = typeof(Enumeradores.TipoLayoutEDI), NotNull = true)]
        public virtual Enumeradores.TipoLayoutEDI Tipo { get; set; }

        [Property(0, Name = "Status", Column = "LAY_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [Property(0, Name = "Separador", Column = "LAY_SEPARADOR_CAMPO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Separador { get; set; }

        [Property(0, Name = "SeparadorInicialFinal", Column = "LAY_SEPARADOR_INICIAL_FINAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparadorInicialFinal { get; set; }

        [Property(0, Name = "SeparadorDecimal", Column = "LAY_SEPARADOR_DECIMAL", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string SeparadorDecimal { get; set; }

        [Property(0, Name = "ValidarRota", Column = "LAY_VALIDAR_ROTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarRota { get; set; }

        [Property(0, Column = "LAY_VALIDAR_NUMERO_REFERENCIA_EDI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarNumeroReferenciaEDI { get; set; }

        [Property(0, Column = "LAY_CAMPO_POR_INDICES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CamposPorIndices { get; set; }

        [Property(0, Column = "LAY_GERAR_EDI_EM_OCORRENCIAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarEDIEmOcorrencias { get; set; }

        [Property(0, Column = "LAY_INCLUIR_CNPJ_EMITENTE_ARQUIVO_EDI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirCNPJEmitenteArquivoEDI { get; set; }


        [Obsolete("Utilize a propriedade ConsiderarDadosExpedidorECTe")]
        [Property(0, Column = "LAY_ENVIAR_DADOS_EXPEDIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarDadosExpedidor { get; set; }

        [Property(0, Column = "LAY_CONSIDERAR_DADOS_EXPEDIDOR_E_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarDadosExpedidorECTe { get; set; }

        [Property(0, Name = "Nomenclatura", Column = "LAY_NOMENCLATURA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Nomenclatura { get; set; }

        [Property(0, Name = "ExtensaoArquivo", Column = "LAY_EXTENSAO_ARQUIVO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ExtensaoArquivo { get; set; }

        [Property(0, Name = "EmailLeitura", Column = "LAY_EMAIL_LEITURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EmailLeitura { get; set; }

        [ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [Property(0, Column = "LAY_REMOVER_DIACRITICOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RemoverDiacriticos { get; set; }

        [Property(0, Column = "LAY_UTILIZAR_INFORMACOES_CTE_ORIGINAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarInformacoesCTeOriginal { get; set; }

        [Property(0, Column = "LAY_UTILIZAR_TOMADOR_COMO_EXPEDIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarTomadorComoExpedidor { get; set; }

        [Property(0, Column = "LAY_UTILIZAR_EMITENTE_CHAVE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarEmitenteDaChave { get; set; }

        /// <summary>
        /// Utilizada para exibir como coluna no relatório de CT-e.
        /// </summary>
        [Property(0, Column = "LAY_VINCULAR_NOME_ARQUIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularNomeArquivo { get; set; }

        /// <summary>
        /// Utiliza apenas o CPF/CNPJ do tomador e carrega as informações do banco de dados.
        /// </summary>
        [Property(0, Column = "LAY_UTILIZAR_TOMADOR_EXISTENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarTomadorExistente { get; set; }

        [Property(0, Column = "LAY_AGRUPAR_POR_REMETENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparPorRemetente { get; set; }

        [Property(0, Column = "LAY_BUSCAR_NOTA_SEM_CHAVE_DOS_DOCUMENTOS_DESTINADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BuscarNotaSemChaveDosDocumentosDestinados { get; set; }

        [Set(0, Name = "Campos", Cascade = "all", Lazy = CollectionLazy.True, Table = "T_EDI_LAYOUT_CAMPO")]
        [Key(1, Column = "LAY_CODIGO")]
        [ManyToMany(2, Class = "CampoEDI", Column = "CAM_CODIGO", NotFound = NotFoundMode.Ignore)]
        public virtual ICollection<Dominio.Entidades.CampoEDI> Campos { get; set; }

        [Property(0, Name = "QuantidadeNotasSequencia", Column = "LAY_QUANTIDADE_NOTAS_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeNotasSequencia { get; set; }

        /// <summary>
        /// Numero de tentativas padrão de envio para integração do layout EDI
        /// </summary>
        [Property(0, Name = "NumeroTentativasAutomaticasIntegracao", Column = "LAY_NUMERO_TENTATIVAS_AUTOMATICAS_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativasAutomaticasIntegracao { get; set; }

        [Set(0, Name = "ModeloDocumentoFiscais", Cascade = "all", Lazy = CollectionLazy.True, Table = "T_EDI_LAYOUT_MODELOS_DOCUMENTOS_FISCAIS")]
        [Key(1, Column = "LAY_CODIGO")]
        [ManyToMany(2, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO")]
        public virtual ICollection<ModeloDocumentoFiscal> ModeloDocumentoFiscais { get; set; }

        [Property(0, Column = "LAY_ENCODING", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Encoding { get; set; }

        [Property(0, Column = "LAY_AGRUPAR_NOTAS_FISCAIS_DOS_CTES_PARA_SUBCONTRATACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AgruparNotasFiscaisDosCTesParaSubcontratacao { get; set; }

        [Property(0, Column = "lAY_GERAR_EDI_POR_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarEDIPorNotaFiscal { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                return this.Tipo.ToString("G");
            }
        }

        public virtual string DescricaoTipoFormatado
        {
            get { return Tipo.ObterDescricao(); }
        }

    }
}
