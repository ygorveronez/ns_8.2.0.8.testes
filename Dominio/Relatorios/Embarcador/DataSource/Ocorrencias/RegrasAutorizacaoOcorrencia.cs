using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Ocorrencias
{
    public sealed class RegrasAutorizacaoOcorrencia
    {
        public int Codigo { get; set; }

        private bool Ativo { get; set; }

        public string AtivoDescricao
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

        private DateTime DataVigencia { get; set; }

        public string DataVigenciaFormatada
        {
            get { return DataVigencia != DateTime.MinValue ? DataVigencia.ToString("dd/MM/yyyy") : ""; }
        }

        public string Descricao { get; set; }

        private int DiasPrazoAprovacao { get; set; }

        public string DiasPrazoAprovacaoFormatado
        {
            get { return DiasPrazoAprovacao > 0 ? DiasPrazoAprovacao.ToString() : ""; }
        }

        private EtapaAutorizacaoOcorrencia EtapaAutorizacao { get; set; }

        public string EtapaAutorizacaoDescricao
        {
            get { return EtapaAutorizacao.ObterDescricao(); }
        }

        public int NumeroAprovadores { get; set; }
        public int NumeroReprovadores { get; set; }
        public string Observacao { get; set; }
        public int PrioridadeAprovacao { get; set; }
        public string TipoOcorrencia { get; set; }
        public string ValorOcorrencia { get; set; }
        public string TipoOperacao { get; set; }
        public string Aprovadores { get; set; }
    }
}
