using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class RelatorioCheckListCarga
    {
        #region Propriedades

        public int Codigo { get; set; }

        public DateTime Data { get; set; }

        public DateTime DataChecklistAnterior { get; set; }

        public DateTime DataChecklistAtual { get; set; }

        public string TipoOperacao { get; set; }

        public string Carga { get; set; }

        public string ObservacoesGerais { get; set; }

        public string MotivoReprovaChecklistAnterior { get; set; }

        public SituacaoCheckList Status { get; set; }

        public SituacaoCheckList StatusChecklistAnterior { get; set; }

        public string Transportador { get; set; }

        public string Placa { get; set; }

        #endregion Propriedades

        #region Propriedades Dinâmicas

        public string ColunaDinamica0 { get; set; }
        public string ColunaDinamica1 { get; set; }
        public string ColunaDinamica2 { get; set; }
        public string ColunaDinamica3 { get; set; }
        public string ColunaDinamica4 { get; set; }
        public string ColunaDinamica5 { get; set; }
        public string ColunaDinamica6 { get; set; }
        public string ColunaDinamica7 { get; set; }
        public string ColunaDinamica8 { get; set; }
        public string ColunaDinamica9 { get; set; }
        public string ColunaDinamica10 { get; set; }
        public string ColunaDinamica11 { get; set; }
        public string ColunaDinamica12 { get; set; }
        public string ColunaDinamica13 { get; set; }
        public string ColunaDinamica14 { get; set; }
        public string ColunaDinamica15 { get; set; }
        public string ColunaDinamica16 { get; set; }
        public string ColunaDinamica17 { get; set; }
        public string ColunaDinamica18 { get; set; }
        public string ColunaDinamica19 { get; set; }
        public string ColunaDinamica20 { get; set; }
        public string ColunaDinamica21 { get; set; }
        public string ColunaDinamica22 { get; set; }
        public string ColunaDinamica23 { get; set; }
        public string ColunaDinamica24 { get; set; }
        public string ColunaDinamica25 { get; set; }
        public string ColunaDinamica26 { get; set; }
        public string ColunaDinamica27 { get; set; }
        public string ColunaDinamica28 { get; set; }
        public string ColunaDinamica29 { get; set; }
        public string ColunaDinamica30 { get; set; }
        public string ColunaDinamica31 { get; set; }
        public string ColunaDinamica32 { get; set; }
        public string ColunaDinamica33 { get; set; }
        public string ColunaDinamica34 { get; set; }
        public string ColunaDinamica35 { get; set; }
        public string ColunaDinamica36 { get; set; }
        public string ColunaDinamica37 { get; set; }
        public string ColunaDinamica38 { get; set; }
        public string ColunaDinamica39 { get; set; }
        public string ColunaDinamica40 { get; set; }
        public string ColunaDinamica41 { get; set; }
        public string ColunaDinamica42 { get; set; }
        public string ColunaDinamica43 { get; set; }
        public string ColunaDinamica44 { get; set; }
        public string ColunaDinamica45 { get; set; }
        public string ColunaDinamica46 { get; set; }
        public string ColunaDinamica47 { get; set; }
        public string ColunaDinamica48 { get; set; }
        public string ColunaDinamica49 { get; set; }
        public string ColunaDinamica50 { get; set; }
        public string ColunaDinamica51 { get; set; }
        public string ColunaDinamica52 { get; set; }
        public string ColunaDinamica53 { get; set; }
        public string ColunaDinamica54 { get; set; }
        public string ColunaDinamica55 { get; set; }
        public string ColunaDinamica56 { get; set; }
        public string ColunaDinamica57 { get; set; }
        public string ColunaDinamica58 { get; set; }
        public string ColunaDinamica59 { get; set; }

        #endregion Propriedades Dinâmicas

        #region Propriedades Com Regras

        public string DataFormatada
        {
            get { return this.Data != DateTime.MinValue ? this.Data.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataChecklistAnteriorFormatada
        {
            get { return this.DataChecklistAnterior != DateTime.MinValue ? this.DataChecklistAnterior.ToDateTimeString() : string.Empty; }
        }

        public string DataChecklistAtualFormatada
        {
            get { return this.DataChecklistAtual != DateTime.MinValue ? this.DataChecklistAtual.ToDateTimeString() : string.Empty; }
        }

        public string StatusDescricao
        {
            get { return this.Status.ObterDescricao(); }
        }

        public string StatusChecklistAnteriorFormatada
        {
            get { return this.StatusChecklistAnterior.ObterDescricao(); }
        }

        #endregion Propriedades Com Regras
    }
}
