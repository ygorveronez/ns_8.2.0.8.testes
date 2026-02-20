using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Documentos
{
    public class ControleDocumento
    {

        public int Codigo { get; set; }
        public int CodigoCargaCTe { get; set; }
        public int CodigoCTe { get; set; }
        public string MotivoParqueamento { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public int CodigoHistorico { get; set; }
        public int CodigoEmpresaCarga { get; set; }
        public int SequenciaTratativa { get; set; }
        public string Carga { get; set; }
        public int CodigoCarga { get; set; }
        public string NFes { get; set; }
        public string SituacaoControleDocumento { get { return Situacao >= 0 ? Situacao.ToString().ToEnum<SituacaoControleDocumento>().ObterDescricao() : string.Empty; } }
        private DateTime? DataEnvioAprovacao { get; set; }
        public int Situacao { get; set; }

        public int ServicoResponsavel { get; set; }
        public string ServicoResponsavelFormatado { get { return ServicoResponsavel >= 0 ? ServicoResponsavel.ToString().ToEnum<ServicoResponsavel>().ObterDescricao() : string.Empty; } }
        public string DiasEmAprovacao
        {
            get
            {
                return (this.Situacao > 0 && this.Situacao.ToString().ToEnum<SituacaoControleDocumento>() == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleDocumento.AguardandoAprovacao && this.DataEnvioAprovacao.HasValue) ? string.Concat((DateTime.Today - this.DataEnvioAprovacao.Value.ToString("d").ToDateTime()).TotalDays.ToString(), " Dia(s)") : string.Empty;
            }
        }
        public string Transportador { get; set; }
        public string Portfolio { get; set; }
        public int CodigoSetor { get; set; }
        public int CodigoIrregularidade { get; set; }
        public int CodigoEntidadeIrregularidade { get; set; }
        public string Irregularidade { get; set; }
        public DateTime? DataGeracaoIrregularidade { get; set; }
        public string ResponsavelPelaIrregularidade { get; set; }
        public string Analise { get; set; }
        public string Setor { get; set; }
        public string Tratativas { get; set; }
        private int PossuiPreCTeInt { get; set; }
        public bool PossuiPreCTe
        {
            get { return PossuiPreCTeInt == 1; }
        }
        private int TipoDocumentoEmissaoInt { get; set; }
        public Dominio.Enumeradores.TipoDocumento TipoDocumentoEmissao 
        {
             get { return (Dominio.Enumeradores.TipoDocumento)TipoDocumentoEmissaoInt; }
        }
        private DateTime DataGeracaoDocumento { get; set; }
        public bool DentroDoMes
        {
            get { return DataGeracaoDocumento.Month == DateTime.Now.Month && DataGeracaoDocumento.Year == DateTime.Now.Year; }
        }
        public string ModeloDocumentoFiscal { get; set; }
        private int SituacaoCCeInt { get; set; }
        public bool CCeRejeitada
        {
            get { return (SituacaoAprovacaoCartaDeCorrecao)SituacaoCCeInt == SituacaoAprovacaoCartaDeCorrecao.Rejeitada; }
        }
        public string MotivoRejeicaoCCe { get; set; }

    }
}