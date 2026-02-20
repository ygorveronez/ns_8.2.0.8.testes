using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.CheckListsUsuario
{
    public class CheckListUsuario
    {
        public int Codigo { get; set; }
        public DateTime DataPreenchimento { get; set; }
        public string Usuario { get; set; }
        public string TipoGROT { get; set; }
        public SituacaoCheckList Situacao { get; set; }
        public string Observacao { get; set; }

        public virtual string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

        public virtual string DataPreenchimentoFormatada
        {
            get
            {
                return DataPreenchimento.ToString("dd/MM/yyyy");
            }
        }

    }
}
