using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_PAIS", EntityName = "PaisMercosul", Name = "Dominio.Entidades.Embarcador.Configuracoes.PaisMercosul", NameType = typeof(PaisMercosul))]
    public class PaisMercosul : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pais", Column = "CPA_PAIS", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Pais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Empresa", Column = "CPA_EMPRESA_FILIAL", TypeType = typeof(string), Length = 250, NotNull = true)]
        public virtual string Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoCrt", Column = "CPA_ULTIMO_CRT", TypeType = typeof(int), NotNull = true)]
        public virtual int UltimoCrt { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoMicDta", Column = "CPA_ULTIMO_MICDTA", TypeType = typeof(int), NotNull = true)]
        public virtual int UltimoMicDta { get; set; }
        public virtual string Descricao
        {
            get
            {
                return "Configuração País Mercosul";
            }
        }
    }
}
