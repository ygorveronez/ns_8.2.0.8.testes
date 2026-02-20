using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SETOR", EntityName = "Setor", Name = "Dominio.Entidades.Setor", NameType = typeof(Setor))]
    public class Setor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SET_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SET_DESCRICAO", TypeType = typeof(string), Length = 80, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "SET_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirAssumirChamadosDoMesmoSetor", Column = "SET_PERMITIR_ASSUMIR_CHAMADOS_DO_MESMO_SETOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirAssumirChamadosDoMesmoSetor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermitirCancelarAtendimento", Column = "SET_PERMITIR_CANCELAR_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirCancelarAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSetorFuncionario", Column = "SET_TIPO_SETOR_FUNCIONARIO", TypeType = typeof(TipoSetorFuncionario), NotNull = false)]
        public virtual TipoSetorFuncionario TipoSetorFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Checklist", Column = "CKL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Checklist.Checklist Checklist { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TipoCargoFuncionario", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SETOR_CARGO_FUNCIONARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SET_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "SCF_CARGO_FUNCIONARIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCargoFuncionario), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoCargoFuncionario> TipoCargoFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificarCenarioPosEntregaImprocedenteGestaoDevolucao", Column = "SET_NOTIFICAR_CENARIO_POS_ENTREGA_IMPROCEDENTE_GESTAO_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificarCenarioPosEntregaImprocedenteGestaoDevolucao { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                if (this.Status == "A")
                    return "Ativo";
                else if (this.Status == "I")
                    return "Inativo";
                else
                    return "";
            }
        }
    }
}
