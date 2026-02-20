using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Documentos
{
    public class ControleDocumento
    {
        public int Codigo { get; set; }
        public int CodigoCargaCTe { get; set; }
        public int CodigoCTe { get; set; }
        public string MotivoParqueamento { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string Carga { get; set; }
        public string NFes { get; set; }
        public SituacaoControleDocumento SituacaoControleDocumento { get; set; }
        public int DiasEmAprovacao { get; set; }
        public string Transportador { get; set; }
        public string Portfolio { get; set; }
        public string Irregularidade { get; set; }
        private DateTime DataGeracaoIrregularidade { get; set; }
        public string ResponsavelPelaIrregularidade { get; set; }
    
        #region Propriedades com Regras

        public string DataGeracaoIrregularidadeFormatada
        {
            get { return DataGeracaoIrregularidade != DateTime.MinValue ? DataGeracaoIrregularidade.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string Situacao
        {
            get { return SituacaoControleDocumento.ObterDescricao(); }
        }
        #endregion
    }
}
