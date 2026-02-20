using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class Subcontratacao
    {
        #region Propriedades

        public int Codigo { get; set; }

        public int NumeroCTe { get; set; }
        public string SerieCTe { get; set; }
        public int CFOP { get; set; }
        public decimal ValorPrestacao { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal Peso { get; set; }
        public decimal ValorMercadoria { get; set; }
        public decimal ValorICMS { get; set; }
        public string ChaveCTe { get; set; }

        public string CodigoDestinatario { get; set; }
        public string Destinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string TelefoneDestinatario { get; set; }
        public string EmailDestinatario { get; set; }
        private string TipoPessoaDestinatario { get; set; }
        private double CNPJDestinatarioSemFormato { get; set; }
        public string GrupoDestinatario { get; set; }
        public string CategoriaDestinatario { get; set; }
        public string Destino { get; set; }

        public string Origem { get; set; }
        public string CodigoRemetente { get; set; }
        public string Remetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string TipoOperacao { get; set; }
        private string TipoPessoaRemetente { get; set; }
        private double CNPJRemetenteSemFormato { get; set; }
        public string GrupoRemetente { get; set; }
        public string CategoriaRemetente { get; set; }

        private string TipoPessoaExpedidor { get; set; }
        private double CNPJExpedidorSemFormato { get; set; }
        private string Expedidor { get; set; }

        private string TipoPessoaRecebedor { get; set; }
        private double CNPJRecebedorSemFormato { get; set; }
        private string Recebedor { get; set; }

        private string TipoPessoaOutroTomador { get; set; }
        private double CNPJOutroTomadorSemFormato { get; set; }
        private string OutroTomador { get; set; }

        public decimal AliquotaICMS { get; set; }
        public string CST { get; set; }
        public string CodigoEmpresa { get; set; }
        private string CNPJEmpresaSemFormato { get; set; }
        public string Empresa { get; set; }
        public string Filial { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataCarga { get; set; }

        public decimal Volumes { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroCargaEmbarcador { get; set; }
        public string Motorista { get; set; }
        public string Placa { get; set; }
        public double CapacidadeVeiculo { get; set; }
        public string GrupoPessoa { get; set; }
        private Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }
        private Dominio.Enumeradores.TipoServico TipoServico { get; set; }
        private Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }
        public string Observacao { get; set; }
        public string Ocorrencia { get; set; }
        private SituacaoCarga SituacaoCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz SituacaoSEFAZ { get; set; }
        public string DescricaoSituacaoSEFAZ
        {
            get
            {
                return SituacaoSEFAZ.ObterDescricao();
            }
        }

        #endregion

        #region Propriedades com Regras

        public string CNPJDestinatario
        {
            get
            {
                if (TipoPessoaDestinatario == "F")
                    return string.Format(@"{0:000\.000\.000\-00}", CNPJDestinatarioSemFormato);
                else
                    return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJDestinatarioSemFormato);
            }
        }

        public string CNPJRemetente
        {
            get
            {
                if (TipoPessoaRemetente == "F")
                    return string.Format(@"{0:000\.000\.000\-00}", CNPJRemetenteSemFormato);
                else
                    return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJRemetenteSemFormato);
            }
        }

        public string CNPJEmpresa
        {
            get
            {
                return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJEmpresaSemFormato);
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataCargaFormatada
        {
            get { return DataCarga != DateTime.MinValue ? DataCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoTipoCTe
        {
            get { return TipoCTe.ObterDescricao(); }
        }

        public string DescricaoTipoServico
        {
            get { return TipoServico.ObterDescricao(); }
        }

        public string DescricaoTipoTomador
        {
            get { return TipoTomador.ObterDescricao(); }
        }

        public string CNPJTomador
        {
            get
            {
                if (TipoTomador == TipoTomador.Remetente)
                {
                    if (TipoPessoaRemetente == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CNPJRemetenteSemFormato);
                    else
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJRemetenteSemFormato);
                }
                else if (TipoTomador == TipoTomador.Destinatario)
                {
                    if (TipoPessoaDestinatario == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CNPJDestinatarioSemFormato);
                    else
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJDestinatarioSemFormato);
                }
                else if (TipoTomador == TipoTomador.Expedidor)
                {
                    if (TipoPessoaExpedidor == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CNPJExpedidorSemFormato);
                    else
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJExpedidorSemFormato);
                }
                else if (TipoTomador == TipoTomador.Recebedor)
                {
                    if (TipoPessoaRecebedor == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CNPJRecebedorSemFormato);
                    else
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJRecebedorSemFormato);
                }
                else
                {
                    if (TipoPessoaOutroTomador == "F")
                        return string.Format(@"{0:000\.000\.000\-00}", CNPJOutroTomadorSemFormato);
                    else
                        return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJOutroTomadorSemFormato);
                }
            }
        }

        public string Tomador
        {
            get
            {
                if (TipoTomador == TipoTomador.Remetente)
                    return Remetente;
                else if (TipoTomador == TipoTomador.Destinatario)
                    return Destinatario;
                else if (TipoTomador == TipoTomador.Expedidor)
                    return Expedidor;
                else if (TipoTomador == TipoTomador.Recebedor)
                    return Recebedor;
                else
                    return OutroTomador;
            }
        }

        public string SituacaoCargaFormatada
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }

        #endregion
    }
}
