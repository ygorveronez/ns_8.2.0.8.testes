using Dominio.Enumeradores;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class CFOP
    {
        #region Propriedades

        public int NumeroCFOP { get; set; }
        public string Extensao { get; set; }
        public string Descricao { get; set; }
        private TipoCFOP Tipo { get; set; }

        public string USO { get; set; }
        public string REVERSAO { get; set; }
        public string USODESCONTO { get; set; }
        public string REVERSAODESCONTO { get; set; }
        public string USOOUTRASDESPESAS { get; set; }
        public string REVERSAOOUTRASDESPESAS { get; set; }
        public string USOFRETE { get; set; }
        public string REVERSAOFRETE { get; set; }
        public string USOICMS { get; set; }
        public string REVERSAOICMS { get; set; }
        public string USOPIS { get; set; }
        public string REVERSAOPIS { get; set; }
        public string USOCOFINS { get; set; }
        public string REVERSAOCOFINS { get; set; }
        public string USOIPI { get; set; }
        public string REVERSAOIPI { get; set; }
        public string USOICMSST { get; set; }
        public string REVERSAOICMSST { get; set; }
        public string USODIFERENCIAL { get; set; }
        public string REVERSAODIFERENCIAL { get; set; }
        public string USOSEGURO { get; set; }
        public string REVERSAOSEGURO { get; set; }
        public string USOFRETEFORA { get; set; }
        public string REVERSAOFRETEFORA { get; set; }
        public string USOOUTRASFORA { get; set; }
        public string REVERSAOOUTRASFORA { get; set; }
        public string USODESCONTOFORA { get; set; }
        public string REVERSAODESCONTOFORA { get; set; }
        public string USOIMPOSTOFORA { get; set; }
        public string REVERSAOIMPOSTOFORA { get; set; }
        public string USODIFERENCIALFRETEFORA { get; set; }
        public string REVERSAODIFERENCIALFRETEFORA { get; set; }
        public string USOICMSFRETEFORA { get; set; }
        public string REVERSAOICMSFRETEFORA { get; set; }
        public string USOCUSTO { get; set; }
        public string REVERSAOCUSTO { get; set; }
        public string USORETENCAOPIS { get; set; }
        public string REVERSAORETENCAOPIS { get; set; }
        public string USORETENCAOCOFINS { get; set; }
        public string REVERSAORETENCAOCOFINS { get; set; }
        public string USORETENCAOINSS { get; set; }
        public string REVERSAORETENCAOINSS { get; set; }
        public string USORETENCAOIPI { get; set; }
        public string REVERSAORETENCAOIPI { get; set; }
        public string USORETENCAOCSLL { get; set; }
        public string REVERSAORETENCAOCSLL { get; set; }
        public string USORETENCAOOUTRAS { get; set; }
        public string REVERSAORETENCAOOUTRAS { get; set; }
        public string USOTITULORETENCAOPIS { get; set; }
        public string REVERSAOTITULORETENCAOPIS { get; set; }
        public string USOTITULORETENCAOCOFINS { get; set; }
        public string REVERSAOTITULORETENCAOCOFINS { get; set; }
        public string USOTITULORETENCAOINSS { get; set; }
        public string REVERSAOTITULORETENCAOINSS { get; set; }
        public string USOTITULORETENCAOIPI { get; set; }
        public string REVERSAOTITULORETENCAOIPI { get; set; }
        public string USOTITULORETENCAOCSLL { get; set; }
        public string REVERSAOTITULORETENCAOCSLL { get; set; }
        public string USOTITULORETENCAOOUTRAS { get; set; }
        public string REVERSAOTITULORETENCAOOUTRAS { get; set; }
        public string USOTITULORETENCAOISS { get; set; }
        public string REVERSAOTITULORETENCAOISS { get; set; }
        public string USOTITULORETENCAOIR { get; set; }
        public string REVERSAOTITULORETENCAOIR { get; set; }
        public string USORETENCAOISS { get; set; }
        public string REVERSAORETENCAOISS { get; set; }
        public string USORETENCAOIR { get; set; }
        public string REVERSAORETENCAOIR { get; set; }
        private bool GeraEstoque { get; set; }
        private CSTICMS CSTICMS { get; set; }
        public decimal AliquotaInterna { get; set; }
        public decimal AliquotaInterestadual { get; set; }
        public decimal AliquotaDiferencial { get; set; }
        private BloqueioDocumentoEntrada BloqueioDocumentoEntrada { get; set; }
        public decimal AliquotaRetencaoPIS { get; set; }
        public decimal AliquotaRetencaoCOFINS { get; set; }
        public decimal AliquotaRetencaoINSS { get; set; }
        public decimal AliquotaRetencaoIPI { get; set; }
        public decimal AliquotaRetencaoCSLL { get; set; }
        public decimal AliquotaOutrasRetencoes { get; set; }
        public decimal AliquotaRetencaoIR { get; set; }
        public decimal AliquotaRetençãoISS { get; set; }
        private bool RealizarRateioDespesaVeiculo { get; set; }

        #endregion

        #region Propriedades com Regras

        public string TipoFormatado
        {
            get { return Tipo.ObterDescricao(); }
        }

        public string BloqueioDocumentoEntradaFormatado
        {
            get { return BloqueioDocumentoEntrada.ObterDescricao(); }
        }

        public string CSTICMSFormatado
        {
            get { return CSTICMS.ObterDescricao(); }
        }
        public string GeraEstoqueFormatado
        {
            get { return GeraEstoque ? "Sim" : "Não"; }
        }

        public string RealizarRateioDespesaVeiculoFormatado
        {
            get { return RealizarRateioDespesaVeiculo ? "Sim" : "Não"; }
        }

        #endregion
    }
}
