using Dominio.Entidades.Embarcador.Avarias;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix
{
    public class Cnab240
    {
        public class Header
        {
            public string CodigoBancoCompensacao { get; set; }
            public string LoteServico { get; set; }
            public string TipoRegistro { get; set; }
            public string UsoExclusivoFebraban1 {  get; set; }
            public string TipoInscricaoEmpresa { get; set; }
            public string NumeroInscricaoEmpresa { get; set; }
            public string CodigoConvenioBanco { get; set; }
            public string AgenciaMantenedoraConta { get; set; }
            public string DigitoVerificadorAgencia { get; set; }
            public string NumeroContaCorrente { get; set; }
            public string DigitoVerificadorConta { get; set; }
            public string DigitoVerificadorAgConta { get; set; }
            public string NomeEmpresa { get; set; }
            public string NomeBanco { get; set; }
            public string UsoExclusivoFebraban2 { get; set; }
            public string CodigoRemessaRetorno { get; set; }
            public string DataGeracaoArquivo { get; set; }
            public string HoraGeracaoArquivo { get; set; }
            public string NumeroSequencialArquivo { get; set; }
            public string NumeroVersaoLayoutArquivo { get; set; }
            public string DensidadeGravacao { get; set; }
            public string IdentificacaoRemessaPIX { get; set; }
            public string UsoReservadoBanco { get; set; }
            public string UsoReservadoEmpresa { get; set; }
            public string UsoExclusivoFebraban3 { get; set; }

            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 3, CodigoBancoCompensacao)}" +
                       $"{FormatCampo('L', '0', 4, LoteServico)}" +
                       $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('R', ' ', 9, UsoExclusivoFebraban1)}" +
                       $"{FormatCampo('L', '0', 1, TipoInscricaoEmpresa)}" +
                       $"{FormatCampo('L', '0', 14, NumeroInscricaoEmpresa)}" +
                       $"{FormatCampo('R', ' ', 20, CodigoConvenioBanco)}" +
                       $"{FormatCampo('L', '0', 5, AgenciaMantenedoraConta)}" +
                       $"{FormatCampo('R', ' ', 1, DigitoVerificadorAgencia)}" +
                       $"{FormatCampo('L', '0', 12, NumeroContaCorrente)}" +
                       $"{FormatCampo('R', ' ', 1, DigitoVerificadorConta)}" +
                       $"{FormatCampo('R', ' ', 1, DigitoVerificadorAgConta)}" +
                       $"{FormatCampo('R', ' ', 30, NomeEmpresa)}" +
                       $"{FormatCampo('R', ' ', 30, NomeBanco)}" +
                       $"{FormatCampo('R', ' ', 10, UsoExclusivoFebraban2)}" +
                       $"{FormatCampo('L', '0', 1, CodigoRemessaRetorno)}" +
                       $"{FormatCampo('L', '0', 8, DataGeracaoArquivo)}" +
                       $"{FormatCampo('L', '0', 6, HoraGeracaoArquivo)}" +
                       $"{FormatCampo('L', '0', 6, NumeroSequencialArquivo)}" +
                       $"{FormatCampo('L', '0', 3, NumeroVersaoLayoutArquivo)}" +
                       $"{FormatCampo('L', '0', 5, DensidadeGravacao)}" +
                       $"{FormatCampo('R', ' ', 3, IdentificacaoRemessaPIX)}" +
                       $"{FormatCampo('R', ' ', 17, UsoReservadoBanco)}" +
                       $"{FormatCampo('R', ' ', 20, UsoReservadoEmpresa)}" +
                       $"{FormatCampo('R', ' ', 29, UsoExclusivoFebraban3)}" +
                       $"";
            }
        }

        public class HeaderLote
        {
            public string CodigoBancoCompensacao { get; set; }
            public string LoteServico { get; set; }
            public string TipoRegistro { get; set; }
            public string TipoOperacao { get; set; }
            public string TipoServico { get; set; }
            public string FormaLancamento { get; set; }
            public string NumeroVersaoLayoutLote { get; set; }
            public string UsoExclusivoFebraban1 { get; set; }
            public string TipoInscricaoEmpresa { get; set; }
            public string NumeroInscricaoEmpresa { get; set; }
            public string CodigoConvenioBanco { get; set; }
            public string AgenciaMantenedoraConta { get; set; }
            public string DigitoVerificadorAgencia { get; set; }
            public string NumeroContaCorrente { get; set; }
            public string DigitoVerificadorConta { get; set; }
            public string DigitoVerificadorAgConta { get; set; }
            public string NomeEmpresa { get; set; }
            public string Mensagem { get; set; }
            public string NomeRuaAvPcaEtc { get; set; }
            public string NumeroLocal { get; set; }
            public string CasaAptoSalaEtc { get; set; }
            public string NomeCidade { get; set; }
            public string CEP { get; set; }
            public string ComplementoCEP { get; set; }
            public string SiglaEstado { get; set; }
            public string IndicativoFormaPagamentoServico { get; set; }
            public string UsoExclusivoFebraban2 { get; set; }
            public string CodigosOcorrenciasRetorno { get; set; }

            public string Formatar()
            {                
                return $"{FormatCampo('L', '0', 3, CodigoBancoCompensacao)}" +
                       $"{FormatCampo('L', '0', 4, LoteServico)}" +
                       $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', ' ', 1, TipoOperacao)}" +
                       $"{FormatCampo('L', '0', 2, TipoServico)}" +
                       $"{FormatCampo('L', '0', 2, FormaLancamento)}" +
                       $"{FormatCampo('L', '0', 3, NumeroVersaoLayoutLote)}" +
                       $"{FormatCampo('L', ' ', 1, UsoExclusivoFebraban1)}" +
                       $"{FormatCampo('L', '0', 1, TipoInscricaoEmpresa)}" +
                       $"{FormatCampo('L', '0', 14, NumeroInscricaoEmpresa)}" +
                       $"{FormatCampo('R', ' ', 20, CodigoConvenioBanco)}" +
                       $"{FormatCampo('L', '0', 5, AgenciaMantenedoraConta)}" +
                       $"{FormatCampo('L', ' ', 1, DigitoVerificadorAgencia)}" +
                       $"{FormatCampo('L', '0', 12, NumeroContaCorrente)}" +
                       $"{FormatCampo('L', ' ', 1, DigitoVerificadorConta)}" +
                       $"{FormatCampo('L', ' ', 1, DigitoVerificadorAgConta)}" +
                       $"{FormatCampo('R', ' ', 30, NomeEmpresa)}" +
                       $"{FormatCampo('R', ' ', 40, Mensagem)}" +
                       $"{FormatCampo('R', ' ', 30, NomeRuaAvPcaEtc)}" +
                       $"{FormatCampo('R', ' ', 5, NumeroLocal)}" +
                       $"{FormatCampo('R', ' ', 15, CasaAptoSalaEtc)}" +
                       $"{FormatCampo('R', ' ', 20, NomeCidade)}" +
                       $"{FormatCampo('R', '0', 5, CEP)}" +
                       $"{FormatCampo('R', ' ', 3, ComplementoCEP)}" +
                       $"{FormatCampo('R', ' ', 2, SiglaEstado)}" +
                       $"{FormatCampo('L', '0', 2, IndicativoFormaPagamentoServico)}" +
                       $"{FormatCampo('R', ' ', 6, UsoExclusivoFebraban2)}" +
                       $"{FormatCampo('R', ' ', 10, CodigosOcorrenciasRetorno)}" +
                       $"";
            }           
        }

        public class DetalheSegmentoA
        {
            public string CodigoBancoCompensacao { get; set; }
            public string LoteServico { get; set; }
            public string TipoRegistro { get; set; }
            public string NumeroSequencialRegistroLote { get; set; }
            public string CodigoSegmentoRegDetalhe { get; set; }
            public string TipoMovimento { get; set; }
            public string CodigoInstrucaoMovimento { get; set; }
            public string CodigoCamaraCentralizadora { get; set; }
            public string CodigoBancoFavorecido { get; set; }
            public string AgMantenedoraCtaFavor { get; set; }
            public string DigitoVerificadorAgencia { get; set; }
            public string NumeroContaCorrente { get; set; }
            public string DigitoVerificadorConta { get; set; }
            public string DigitoVerificadorAgConta { get; set; }
            public string NomeFavorecido { get; set; }
            public string NumeroDocumAtribuidoEmpresa { get; set; }
            public string DataPagamento { get; set; }
            public string TipoMoeda { get; set; }
            public string QuantidadeMoeda { get; set; }
            public string ValorPagamento { get; set; }
            public string NumeroDocumAtribuidoBanco { get; set; }
            public string DataRealEfetivacaoPagto { get; set; }
            public string ValorRealEfetivacaoPagto { get; set; }
            public string OutrasInformacoes { get; set; }
            public string ComplTipoServico { get; set; }
            public string CodigoFinalidadeTED { get; set; }
            public string ComplFinalidadePagamento { get; set; }
            public string UsoExclusivoFebraban1 { get; set; }
            public string AvisoFavorecido { get; set; }
            public string CodigosOcorrenciasRetorno { get; set; }

            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 3, CodigoBancoCompensacao)}" +
                       $"{FormatCampo('L', '0', 4, LoteServico)}" +
                       $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', '0', 5, NumeroSequencialRegistroLote)}" +
                       $"{FormatCampo('L', ' ', 1, CodigoSegmentoRegDetalhe)}" +
                       $"{FormatCampo('L', '0', 1, TipoMovimento)}" +
                       $"{FormatCampo('L', '0', 2, CodigoInstrucaoMovimento)}" +
                       $"{FormatCampo('L', '0', 3, CodigoCamaraCentralizadora)}" +
                       $"{FormatCampo('L', ' ', 3, CodigoBancoFavorecido)}" +
                       $"{FormatCampo('L', ' ', 5, AgMantenedoraCtaFavor)}" +
                       $"{FormatCampo('L', ' ', 1, DigitoVerificadorAgencia)}" +
                       $"{FormatCampo('L', ' ', 12, NumeroContaCorrente)}" +
                       $"{FormatCampo('L', ' ', 1, DigitoVerificadorConta)}" +
                       $"{FormatCampo('L', ' ', 1, DigitoVerificadorAgConta)}" +
                       $"{FormatCampo('R', ' ', 30, NomeFavorecido)}" +
                       $"{FormatCampo('L', '0', 20, NumeroDocumAtribuidoEmpresa)}" +
                       $"{FormatCampo('L', '0', 8, DataPagamento)}" +
                       $"{FormatCampo('L', ' ', 3, TipoMoeda)}" +
                       $"{FormatCampo('R', '0', 15, QuantidadeMoeda)}" +
                       $"{FormatCampo('L', '0', 15, ValorPagamento)}" +
                       $"{FormatCampo('R', ' ', 20, NumeroDocumAtribuidoBanco)}" +
                       $"{FormatCampo('R', '0', 8, DataRealEfetivacaoPagto)}" +
                       $"{FormatCampo('R', '0', 15, ValorRealEfetivacaoPagto)}" +
                       $"{FormatCampo('L', ' ', 40, OutrasInformacoes)}" +
                       $"{FormatCampo('L', ' ', 2, ComplTipoServico)}" +
                       $"{FormatCampo('R', ' ', 5, CodigoFinalidadeTED)}" +
                       $"{FormatCampo('R', ' ', 2, ComplFinalidadePagamento)}" +
                       $"{FormatCampo('R', ' ', 3, UsoExclusivoFebraban1)}" +
                       $"{FormatCampo('L', '0', 1, AvisoFavorecido)}" +
                       $"{FormatCampo('R', ' ', 10, CodigosOcorrenciasRetorno)}" +
                       $"";
            }
        }

        public class DetalheSegmentoB
        {
            public string CodigoBancoCompensacao { get; set; }
            public string LoteServico { get; set; }
            public string TipoRegistro { get; set; }
            public string NumeroSequencialRegistroLote { get; set; }
            public string CodigoSegmentoRegDetalhe { get; set; }
            public string FormaIniciacao { get; set; }
            public string TipoInscricaoFavorecido { get; set; }
            public string NumeroInscricaoFavorecido { get; set; }
            public string Informacao10 { get; set; }
            public string Informacao11 { get; set; }
            public string Informacao12 { get; set; }
            public string UsoExclusivoSIAPE { get; set; }
            public string CodigoISPB { get; set; }


            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 3, CodigoBancoCompensacao)}" +
                       $"{FormatCampo('L', '0', 4, LoteServico)}" +
                       $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', '0', 5, NumeroSequencialRegistroLote)}" +
                       $"{FormatCampo('L', ' ', 1, CodigoSegmentoRegDetalhe)}" +
                       $"{FormatCampo('R', ' ', 3, FormaIniciacao)}" +
                       $"{FormatCampo('L', '0', 1, TipoInscricaoFavorecido)}" +
                       $"{FormatCampo('L', '0', 14, NumeroInscricaoFavorecido)}" +
                       $"{FormatCampo('R', ' ', 35, Informacao10)}" +
                       $"{FormatCampo('R', ' ', 60, Informacao11)}" +
                       $"{FormatCampo('R', ' ', 99, Informacao12)}" +
                       $"{FormatCampo('R', '0', 6, UsoExclusivoSIAPE)}" +
                       $"{FormatCampo('R', '0', 8, CodigoISPB)}" +
                       $"";
            }
        }

        public class DetalheSegmentoJ52Pix
        {
            public string CodigoBancoCompensacao { get; set; }
            public string LoteServico { get; set; }
            public string TipoRegistro { get; set; }
            public string NumeroSequencialRegistroLote { get; set; }
            public string CodigoSegmentoRegDetalhe { get; set; }
            public string UsoExclusivoFebraban1 { get; set; }
            public string CodigoMovimentoRemessa { get; set; }
            public string IdentificacaoRegistroOpcional { get; set; }
            public string TipoInscricaoDevedor { get; set; }
            public string NumeroInscricaoDevedor { get; set; }
            public string NomeDevedor { get; set; }
            public string TipoInscricaoFavorecido { get; set; }
            public string NumeroInscricaoFavorecido { get; set; }
            public string NomeFavorecido { get; set; }
            public string UrlChaveEnderecamento { get; set; }
            public string CodigoIdentificacaoQrCode { get; set; }

            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 3, CodigoBancoCompensacao)}" +
                       $"{FormatCampo('L', '0', 4, LoteServico)}" +
                       $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', '0', 5, NumeroSequencialRegistroLote)}" +
                       $"{FormatCampo('L', ' ', 1, CodigoSegmentoRegDetalhe)}" +
                       $"{FormatCampo('R', ' ', 1, UsoExclusivoFebraban1)}" +
                       $"{FormatCampo('L', '0', 2, CodigoMovimentoRemessa)}" +
                       $"{FormatCampo('L', ' ', 2, IdentificacaoRegistroOpcional)}" +
                       $"{FormatCampo('L', ' ', 1, TipoInscricaoDevedor)}" +
                       $"{FormatCampo('L', '0', 15, NumeroInscricaoDevedor)}" +
                       $"{FormatCampo('R', ' ', 40, NomeDevedor)}" +
                       $"{FormatCampo('L', '0', 1, TipoInscricaoFavorecido)}" +
                       $"{FormatCampo('L', '0', 15, NumeroInscricaoFavorecido)}" +
                       $"{FormatCampo('R', ' ', 40, NomeFavorecido)}" +
                       $"{FormatCampo('R', ' ', 79, UrlChaveEnderecamento)}" +
                       $"{FormatCampo('R', ' ', 30, CodigoIdentificacaoQrCode)}" +
                       $"";

            }
        }

        public class TrailerLote
        {
            public string CodigoBancoCompensacao { get; set; }
            public string LoteServico { get; set; }
            public string TipoRegistro { get; set; }
            public string UsoExclusivoFebraban1 { get; set; }
            public string QuantidadeRegistrosLote { get; set; }
            public string SomatoriaValores { get; set; }
            public string SomatoriaQuantidadeMoedas { get; set; }
            public string NumeroAvisoDebito { get; set; }
            public string UsoExclusivoFebraban2 { get; set; }
            public string CodigosOcorrenciasRetorno { get; set; }

            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 3, CodigoBancoCompensacao)}" +
                       $"{FormatCampo('L', '0', 4, LoteServico)}" +
                       $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('R', ' ', 9, UsoExclusivoFebraban1)}" +
                       $"{FormatCampo('L', '0', 6, QuantidadeRegistrosLote)}" +
                       $"{FormatCampo('L', '0', 18, SomatoriaValores)}" +
                       $"{FormatCampo('L', '0', 18, SomatoriaQuantidadeMoedas)}" +
                       $"{FormatCampo('L', '0', 6, NumeroAvisoDebito)}" +                       
                       $"{FormatCampo('R', ' ', 165, UsoExclusivoFebraban2)}" +
                       $"{FormatCampo('L', ' ', 10, CodigosOcorrenciasRetorno)}" +
                       $"";               
            }
        }

        public class Trailer
        {
            public string CodigoBancoCompensacao { get; set; }
            public string LoteServico { get; set; }
            public string TipoRegistro { get; set; }
            public string UsoExclusivoFebraban1 { get; set; }
            public string QuantidadeLotesArquivo { get; set; }
            public string QuantidadeRegistrosArquivo { get; set; }
            public string QtdeContasConcil { get; set; }
            public string UsoExclusivoFebraban2 { get; set; }

            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 3, CodigoBancoCompensacao)}" +
                       $"{FormatCampo('L', '0', 4, LoteServico)}" +
                       $"{FormatCampo('L', '0', 1, TipoRegistro)}" +                       
                       $"{FormatCampo('R', ' ', 9, UsoExclusivoFebraban1)}" +
                       $"{FormatCampo('L', '0', 6, QuantidadeLotesArquivo)}" +
                       $"{FormatCampo('L', '0', 6, QuantidadeRegistrosArquivo)}" +
                       $"{FormatCampo('L', '0', 6, QtdeContasConcil)}" +
                       $"{FormatCampo('R', ' ', 205, UsoExclusivoFebraban2)}" +
                       $"";                                                                                                                                                                        
            }
        }

        private static string FormatCampo(char tipoformat, char caractere, int tamanho, string texto)
        {
            if (tipoformat == 'L')
                return PadLeft(texto, tamanho, caractere);
            else
                return PadRight(texto, tamanho, caractere);
        }

        private static string PadRight(string texto, int tamanho, char caractere)
        {
            if (texto.Length >= tamanho)
                return texto.Substring(0, tamanho);
            else
                return texto.PadRight(tamanho, caractere);
        }

        private static string PadLeft(string texto, int tamanho, char caractere)
        {
            if (texto.Length >= tamanho)
                return texto.Substring(0, tamanho);
            else
                return texto.PadLeft(tamanho, caractere);
        }
    }
}
