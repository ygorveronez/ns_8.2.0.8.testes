using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NotasFiscais
{
    public class ItemNaoConformidade
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        private bool Situacao { get; set; }
        public string NotaFiscal { get; set; }
        private GrupoNC Grupo { get; set; }
        private SubGrupoNC SubGrupo { get; set; }
        private AreaNC Area { get; set; }
        private bool IrrelevanteNaoConformidade { get; set; }
        private bool PermiteContingencia { get; set; }
        private TipoRegraNaoConformidade TipoRegra { get; set; }
        private TipoParticipante Participante { get; set; }
        public string TipoOperacao { get; set; }
        public string Filial { get; set; }
        public string Fornecedor { get; set; }
        public int CFOP { get; set; }

        #region Props com regras

        public string SituacaoFormatada
        {
            get { return this.Situacao.ObterDescricaoAtivo(); }
        }

        public string GrupoFormatado
        {
            get { return this.Grupo.ObterDescricao(); }
        }

        public string SubGrupoFormatado
        {
            get { return this.SubGrupo.ObterDescricao(); }
        }

        public string AreaFormatada
        {
            get { return this.Area.ObterDescricao(); }
        }

        public string IrrelevanteNaoConformidadeFormatado
        {
            get { return this.IrrelevanteNaoConformidade.ObterDescricao(); }
        }

        public string PermiteContingenciaFormatado
        {
            get { return this.PermiteContingencia.ObterDescricao(); }
        }

        public string TipoRegraFormatado
        {
            get { return this.TipoRegra.ObterDescricao(); }
        }

        public string ParticipanteFormatado
        {
            get { return this.Participante.ObterDescricao(); }
        }

        #endregion
    }
}
