using Dominio.Interfaces.Embarcador.Integracao;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANHOTO_INTEGRACAO", EntityName = "CanhotoIntegracao", Name = "Dominio.Entidades.Embarcador.Veiculos.CanhotoIntegracao", NameType = typeof(CanhotoIntegracao))]
    public class CanhotoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Canhoto", Column = "CNF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Canhoto Canhoto { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CANHOTO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        //Removido o campo pois será criada entidade nova. Removido o enumerado TipoRegistroIntegracaoCanhoto e criado o enumerador TipoRegistroIntegracaoCTe
        //[NHibernate.Mapping.Attributes.Property(0, Name = "TipoRegistro", Column = "CAI_TIPO_REGISTRO", TypeType = typeof(TipoRegistroIntegracaoCanhoto), NotNull = true)]
        //public virtual TipoRegistroIntegracaoCanhoto TipoRegistro { get; set; }

        #region Validação Canhoto IA Comprovei Devops: #7910
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoCanhoto", Column = "INT_VALIDACAO_CANHOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoCanhoto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoNumero", Column = "INT_VALIDACAO_NUMERO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoEncontrouData", Column = "INT_VALIDACAO_ENCONTROU_DATA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoEncontrouData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidacaoAssinatura", Column = "INT_VALIDACAO_ASSINATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidacaoAssinatura { get; set; }
        #endregion

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoServicoMultisoftware", Column = "INT_TIPO_SERVICO_MULTISOFTWARE", TypeType = typeof(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware), NotNull = false)]
        public virtual AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IniciouConexaoExterna", Column = "INT_INICIOU_CONEXAO_EXTERNA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciouConexaoExterna { get; set; }

        public virtual string Descricao
        {
            get { return this.Canhoto.Numero.ToString(); }
        }
    }
}

