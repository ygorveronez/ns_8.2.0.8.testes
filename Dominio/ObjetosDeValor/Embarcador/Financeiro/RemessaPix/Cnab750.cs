using Dominio.Entidades.Embarcador.Avarias;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro.RemessaPix
{
    public class Cnab750
    {
        public class Header
        {
            public string TipoRegistro { get; set; }
            public string Operacao { get; set; }
            public string LiteralRemessa { get; set; }
            public string CodigoServico { get; set; }
            public string LiteralServiço { get; set; }
            public string IspbParticipante { get; set; }
            public string TipoPessoaRecebedor { get; set; }
            public string CpfCnpj { get; set; }
            public string Agencia { get; set; }
            public string Conta { get; set; }
            public string TipoConta { get; set; }
            public string ChavePix { get; set; }
            public string DataGeracao { get; set; }
            public string CodigoConvenio { get; set; }
            public string ExclusivoPspRecebedor { get; set; }
            public string NomeRecebedor { get; set; }
            public string Brancos { get; set; }
            public string NumeroSequencialRemessa { get; set; }
            public string VersaoArquivo { get; set; }
            public string NumeroSequencialRegistro { get; set; }

            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', '0', 1, Operacao)}" +
                       $"{FormatCampo('R', ' ', 7, LiteralRemessa)}" +
                       $"{FormatCampo('L', '0', 2, CodigoServico)}" +
                       $"{FormatCampo('R', ' ', 15, LiteralServiço)}" +
                       $"{FormatCampo('R', ' ', 8, IspbParticipante)}" +
                       $"{FormatCampo('L', '0', 2, TipoPessoaRecebedor)}" +
                       $"{FormatCampo('L', '0', 14, CpfCnpj)}" +
                       $"{FormatCampo('L', ' ', 4, Agencia)}" +
                       $"{FormatCampo('L', ' ', 20, Conta)}" +
                       $"{FormatCampo('L', ' ', 4, TipoConta)}" +
                       $"{FormatCampo('L', ' ', 77, ChavePix)}" +
                       $"{FormatCampo('L', '0', 8, DataGeracao)}" +
                       $"{FormatCampo('R', ' ', 30, CodigoConvenio)}" +
                       $"{FormatCampo('R', ' ', 60, ExclusivoPspRecebedor)}" +
                       $"{FormatCampo('R', ' ', 100, NomeRecebedor)}" +
                       $"{FormatCampo('L', ' ', 378, Brancos)}" +
                       $"{FormatCampo('L', '0', 10, NumeroSequencialRemessa)}" +
                       $"{FormatCampo('L', '0', 3, VersaoArquivo)}" +
                       $"{FormatCampo('L', '0', 6, NumeroSequencialRegistro)}" +
                       $"";
            }
        }

        public class Detalhe
        {
            public string TipoRegistro { get; set; }
            public string Identificador { get; set; }
            public string TipoPessoarecebedor { get; set; }
            public string CpfCnpj { get; set; }
            public string Agencia { get; set; }
            public string Conta { get; set; }
            public string Tipo { get; set; }
            public string ChavePix { get; set; }
            public string TipoCobrança { get; set; }
            public string CodOcorrência { get; set; }
            public string TimestampExpiracao { get; set; }
            public string DataVencimento { get; set; }
            public string ValidadeAposVencimento { get; set; }
            public string ValorOriginal { get; set; }
            public string TipoPessoaDevedor { get; set; }
            public string CpfCnpjDevedor { get; set; }
            public string NomeDevedor { get; set; }
            public string SolicitacaoPagadorCampoTextoLivre { get; set; }
            public string ExclusivoPspRecebedor { get; set; }
            public string Brancos { get; set; }
            public string NumeroSequencialRegistro { get; set; }


            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', '0', 35, Identificador)}" +
                       $"{FormatCampo('L', '0', 2, TipoPessoarecebedor)}" +
                       $"{FormatCampo('L', '0', 14, CpfCnpj)}" +
                       $"{FormatCampo('L', '0', 4, Agencia)}" +
                       $"{FormatCampo('L', '0', 20, Conta)}" +
                       $"{FormatCampo('L', '0', 4, Tipo)}" +
                       $"{FormatCampo('L', '0', 77, ChavePix)}" +
                       $"{FormatCampo('L', '0', 1, TipoCobrança)}" +
                       $"{FormatCampo('L', '0', 2, CodOcorrência)}" +
                       $"{FormatCampo('L', '0', 14, TimestampExpiracao)}" +
                       $"{FormatCampo('L', '0', 8, DataVencimento)}" +
                       $"{FormatCampo('L', '0', 4, ValidadeAposVencimento)}" +
                       $"{FormatCampo('L', '0', 15, ValorOriginal)}" +
                       $"{FormatCampo('L', '0', 2, TipoPessoaDevedor)}" +
                       $"{FormatCampo('L', '0', 14, CpfCnpjDevedor)}" +
                       $"{FormatCampo('L', '0', 140, NomeDevedor)}" +
                       $"{FormatCampo('L', '0', 140, SolicitacaoPagadorCampoTextoLivre)}" +
                       $"{FormatCampo('L', '0', 60, ExclusivoPspRecebedor)}" +
                       $"{FormatCampo('L', '0', 185, Brancos)}" +
                       $"{FormatCampo('L', '0', 6, NumeroSequencialRegistro)}" +
                       $"";
            }
        }        

        public class DetalheInformacoesAdicionais
        {            
            public string TipoRegistro { get; set; }
            public string Identificador { get; set; }
            public string Nome1 { get; set; }
            public string Valor1 { get; set; }
            public string Nome2 { get; set; }
            public string Valor2 { get; set; }
            public string Brancos { get; set; }
            public string NumeroSequencial { get; set; }


            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', '0', 35, Identificador)}" +
                       $"{FormatCampo('L', '0', 50, Nome1)}" +
                       $"{FormatCampo('L', '0', 200, Valor1)}" +
                       $"{FormatCampo('L', '0', 50, Nome2)}" +
                       $"{FormatCampo('L', '0', 200, Valor2)}" +
                       $"{FormatCampo('L', '0', 208, Brancos)}" +
                       $"{FormatCampo('L', '0', 6, NumeroSequencial)}" +                    
                       $"";                                                                                                                                                                        
            }
        }

        public class DetalheDadosEspecificosCobrancaVencimento
        {
            public string TipoRegistro { get; set; }
            public string Identificador { get; set; }
            public string EmailDevedor { get; set; }
            public string LogradouroDevedor { get; set; }
            public string CidadeDevedor { get; set; }
            public string EstadoDevedorUF { get; set; }
            public string CepDevedor { get; set; }
            public string ModalidadeAbatimento { get; set; }
            public string ValorAbatimento { get; set; }
            public string ModalidadeDesconto { get; set; }
            public string DataDesconto1 { get; set; }
            public string ValorDesconto1 { get; set; }
            public string DataDesconto2 { get; set; }
            public string ValorDesconto2 { get; set; }
            public string DataDesconto3 { get; set; }
            public string ValorDesconto3 { get; set; }
            public string ModalidadeJuros { get; set; }
            public string ValorJuros { get; set; }
            public string ModalidadeMulta { get; set; }
            public string ValorMulta { get; set; }
            public string Brancos { get; set; }
            public string NumeroSequencialRegistro { get; set; }



            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', '0', 35, Identificador)}" +
                       $"{FormatCampo('L', '0', 77, EmailDevedor)}" +
                       $"{FormatCampo('L', '0', 200, LogradouroDevedor)}" +
                       $"{FormatCampo('L', '0', 200, CidadeDevedor)}" +
                       $"{FormatCampo('L', '0', 2, EstadoDevedorUF)}" +
                       $"{FormatCampo('L', '0', 8, CepDevedor)}" +
                       $"{FormatCampo('L', '0', 1, ModalidadeAbatimento)}" +
                       $"{FormatCampo('L', '0', 17, ValorAbatimento)}" +
                       $"{FormatCampo('L', '0', 1, ModalidadeDesconto)}" +
                       $"{FormatCampo('L', '0', 8, DataDesconto1)}" +
                       $"{FormatCampo('L', '0', 17, ValorDesconto1)}" +
                       $"{FormatCampo('L', '0', 8, DataDesconto2)}" +
                       $"{FormatCampo('L', '0', 17, ValorDesconto2)}" +
                       $"{FormatCampo('L', '0', 8, DataDesconto3)}" +
                       $"{FormatCampo('L', '0', 17, ValorDesconto3)}" +
                       $"{FormatCampo('L', '0', 1, ModalidadeJuros)}" +
                       $"{FormatCampo('L', '0', 17, ValorJuros)}" +
                       $"{FormatCampo('L', '0', 1, ModalidadeMulta)}" +
                       $"{FormatCampo('L', '0', 17, ValorMulta)}" +
                       $"{FormatCampo('L', '0', 91, Brancos)}" +
                       $"{FormatCampo('L', '0', 6, NumeroSequencialRegistro)}" +
                       $"";
            }
        }

        public class Trailer
        {
            public string TipoRegistro { get; set; }
            public string Brancos { get; set; }
            public string ValorTotal { get; set; }
            public string QtdeDetalhes { get; set; }
            public string NumeroSequencial { get; set; }

            public string Formatar()
            {
                return $"{FormatCampo('L', '0', 1, TipoRegistro)}" +
                       $"{FormatCampo('L', '0', 711, Brancos)}" +
                       $"{FormatCampo('L', '0', 17, ValorTotal)}" +
                       $"{FormatCampo('L', '0', 15, QtdeDetalhes)}" +
                       $"{FormatCampo('L', '0', 6, NumeroSequencial)}" +                      
                       $"";
            }
        }

        private static string FormatCampo(char tipoformat, char caractere, int tamanho, string texto)
        {
            if (tipoformat == 'l')
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
