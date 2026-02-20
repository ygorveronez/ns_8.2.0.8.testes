using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_CONTRATO", EntityName = "EmpresaContrato", Name = "Dominio.Entidades.EmpresaContrato", NameType = typeof(EmpresaContrato))]
    public class EmpresaContrato : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ECO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "ECO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Contrato", Column = "ECO_CONTRATO", Type = "StringClob", NotNull = false)]
        public virtual string Contrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RecorrenciaEmDias", Column = "ECO_RECORRENCIA_EM_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int RecorrenciaEmDias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarPorEmail", Column = "ECO_NOTIFICAR_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarPorEmail { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "ECO_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaAlteracao", Column = "ECO_DATA_ULTIMA_ALTERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaAlteracao { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_CONTRATO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ECO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "EmpresaContratoAnexo", Column = "ECA_CODIGO")]
        public virtual IList<EmpresaContratoAnexo> Anexos { get; set; }

        public virtual string ContratoFormatado
        {
            get { return System.Text.RegularExpressions.Regex.Replace(Contrato, "<.*?>", string.Empty); }
        }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
