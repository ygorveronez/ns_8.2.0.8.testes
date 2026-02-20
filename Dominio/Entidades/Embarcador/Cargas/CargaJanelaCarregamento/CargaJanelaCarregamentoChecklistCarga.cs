using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_CHECKLIST_CARGA", EntityName = "CargaJanelaCarregamentoChecklistCarga", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoChecklistCarga", NameType = typeof(CargaJanelaCarregamentoChecklistCarga))]
    public class CargaJanelaCarregamentoChecklistCarga : EntidadeBase, IEquatable<CargaJanelaCarregamentoChecklistCarga>
    {
        public CargaJanelaCarregamentoChecklistCarga() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoChecklist", Column = "CHE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamentoChecklist CargaJanelaCarregamentoChecklist { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataChecklist", Column = "CCC_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataChecklist { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Reboques", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_JANELA_CARREGAMENTO_CHECKLIST_CARGA_REBOQUES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Reboques { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoProduto", Column = "GPR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.GrupoProduto GrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegimeLimpeza", Column = "REGIME_LIMPEZA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumRegimeLimpeza), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumRegimeLimpeza RegimeLimpeza { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_JANELA_CARREGAMENTO_CHECKLIST_CARGA_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHC_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "CargaJanelaCarregamentoChecklistCargaAnexos")]
        public virtual ICollection<CargaJanelaCarregamentoChecklistCargaAnexos> Anexos { get; set; }


        public virtual bool Equals(CargaJanelaCarregamentoChecklistCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_CHECKLIST_CARGA_ANEXOS", EntityName = "CargaJanelaCarregamentoChecklistCargaAnexos", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaJanelaCarregamentoChecklistCargaAnexos", NameType = typeof(CargaJanelaCarregamentoChecklistCargaAnexos))]
    public class CargaJanelaCarregamentoChecklistCargaAnexos : EntidadeBase, IEquatable<CargaJanelaCarregamentoChecklistCargaAnexos>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoChecklistCarga", Column = "CHC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamentoChecklistCarga CargaJanelaCarregamentoChecklistCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CHA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "CHA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "CHA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

        public virtual bool Equals(CargaJanelaCarregamentoChecklistCargaAnexos other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
