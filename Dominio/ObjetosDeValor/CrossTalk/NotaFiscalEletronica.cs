using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.CrossTalk
{
    public class NotaFiscalEletronica
    {
        public NotaFiscalEletronica(string dados)
        {
            this.Documento = dados;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            string[] dado = dados.Split('|');

            this.Emitente = new Pessoa();

            this.Emitente.CPFCNPJ = dado[0];
            this.Emitente.IE = dado[1];
            this.Emitente.Nome = dado[2];
            this.Emitente.Logradouro = dado[3];
            this.Emitente.Numero = dado[4];
            this.Emitente.Complemento = dado[5];
            this.Emitente.Bairro = dado[6];
            this.Emitente.CodigoMunicipio = string.IsNullOrWhiteSpace(dado[7]) ? 0 : int.Parse(dado[7]);
            this.Emitente.DescricaoMunicipio = dado[8];
            this.Emitente.UF = dado[9];
            this.Emitente.CEP = dado[10];
            this.Emitente.CodigoPais = string.IsNullOrWhiteSpace(dado[11]) ? 0 : int.Parse(dado[11]);
            this.Emitente.DescricaoPais = dado[12];
            this.Emitente.Fone = dado[13];

            this.ModalidadeFrete = string.IsNullOrWhiteSpace(dado[14]) ? 0 : int.Parse(dado[14]);
            this.Serie = string.IsNullOrWhiteSpace(dado[15]) ? 0 : int.Parse(dado[15]);
            this.Numero = string.IsNullOrWhiteSpace(dado[16]) ? 0 : int.Parse(dado[16]);
            this.DataEmissao = string.IsNullOrWhiteSpace(dado[17]) ? DateTime.Now : DateTime.ParseExact(dado[17], "yyyy-MM-dd", null);
            this.Chave = dado[18];
            this.Valor = string.IsNullOrWhiteSpace(dado[19]) ? 0m : decimal.Parse(dado[19], cultura);
            this.Peso = string.IsNullOrWhiteSpace(dado[20]) ? 0m : decimal.Parse(dado[20], cultura);
            this.Quantidade = string.IsNullOrWhiteSpace(dado[21]) ? 0 : int.Parse(dado[21]);

            this.Destinatario = new Pessoa();
            this.Destinatario.CPFCNPJ = dado[22];
            this.Destinatario.IE = dado[23];
            this.Destinatario.Nome = dado[24];
            this.Destinatario.Fone = dado[25];
            this.Destinatario.Suframa = dado[26];
            this.Destinatario.Logradouro = dado[27];
            this.Destinatario.Numero = dado[28];
            this.Destinatario.Complemento = dado[29];
            this.Destinatario.Bairro = dado[30];
            this.Destinatario.CodigoMunicipio = string.IsNullOrWhiteSpace(dado[31]) ? 0 : int.Parse(dado[31]);
            this.Destinatario.DescricaoMunicipio = dado[32];
            this.Destinatario.UF = dado[33];
            this.Destinatario.CEP = dado[34];
            this.Destinatario.CodigoPais = string.IsNullOrWhiteSpace(dado[35]) ? 0 : int.Parse(dado[35]);
            this.Destinatario.DescricaoPais = dado[36];

            this.Canhoto = new Canhoto();
            this.Canhoto.ADAnterior = string.IsNullOrWhiteSpace(dado[37]) ? 0 : int.Parse(dado[37]);
            this.Canhoto.ADEntrega = string.IsNullOrWhiteSpace(dado[38]) ? 0 : int.Parse(dado[38]);
            this.Canhoto.ADCobranca = string.IsNullOrWhiteSpace(dado[39]) ? 0 : int.Parse(dado[39]);
            this.Canhoto.RegistroRevendedora = string.IsNullOrWhiteSpace(dado[40]) ? 0 : int.Parse(dado[40]);
            this.Canhoto.CampanhaEntrega = string.IsNullOrWhiteSpace(dado[41]) ? 0 : int.Parse(dado[41]);
            this.Canhoto.CampanhaCobranca = string.IsNullOrWhiteSpace(dado[42]) ? 0 : int.Parse(dado[42]);
            this.Canhoto.ValorEntrega = string.IsNullOrWhiteSpace(dado[43]) ? 0m : decimal.Parse(dado[43], new System.Globalization.CultureInfo("pt-BR"));
            this.Canhoto.ValorCobranca = dado[44];
            this.Canhoto.Setor = string.IsNullOrWhiteSpace(dado[45]) ? 0 : int.Parse(dado[45]);
            this.Canhoto.Rota = dado[46];
            this.Canhoto.SequenciaEntrega = dado[47];

            this.LocalRetirada = new Pessoa();
            this.LocalRetirada.CPFCNPJ = dado[48];
            this.LocalRetirada.Logradouro = dado[49];
            this.LocalRetirada.Numero = dado[50];
            this.LocalRetirada.Complemento = dado[51];
            this.LocalRetirada.Bairro = dado[52];
            this.LocalRetirada.CodigoMunicipio = string.IsNullOrWhiteSpace(dado[53]) ? 0 : int.Parse(dado[53]);
            this.LocalRetirada.DescricaoMunicipio = dado[54];
            this.LocalRetirada.UF = dado[55];

            this.LocalEntrega = new Pessoa();
            this.LocalEntrega.CPFCNPJ = dado[56];
            this.LocalEntrega.Logradouro = dado[57];
            this.LocalEntrega.Numero = dado[58];
            this.LocalEntrega.Complemento = dado[59];
            this.LocalEntrega.Bairro = dado[60];
            this.LocalEntrega.CodigoMunicipio = string.IsNullOrWhiteSpace(dado[61]) ? 0 : int.Parse(dado[61]);
            this.LocalEntrega.DescricaoMunicipio = dado[62];
            this.LocalEntrega.UF = dado[63];
        }

        public NotaFiscalEletronica() { }

        public string Documento { get; set; }

        public Pessoa Emitente { get; set; }

        public Pessoa Destinatario { get; set; }

        public Canhoto Canhoto { get; set; }

        public Pessoa LocalRetirada { get; set; }

        public Pessoa LocalEntrega { get; set; }

        public int ModalidadeFrete { get; set; }

        public int Serie { get; set; }

        public int Numero { get; set; }

        public DateTime DataEmissao { get; set; }

        public string Chave { get; set; }

        public decimal Valor { get; set; }

        public decimal Peso { get; set; }

        public int Quantidade { get; set; }

        public void SetarDocumento()
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            List<string> dados = new List<string>();

            dados.Add(this.Emitente.CPFCNPJ);
            dados.Add(this.Emitente.IE);
            dados.Add(this.Emitente.Nome);
            dados.Add(this.Emitente.Logradouro);
            dados.Add(this.Emitente.Numero);
            dados.Add(this.Emitente.Complemento);
            dados.Add(this.Emitente.Bairro);
            dados.Add(this.Emitente.CodigoMunicipio.ToString());
            dados.Add(this.Emitente.DescricaoMunicipio);
            dados.Add(this.Emitente.UF);
            dados.Add(this.Emitente.CEP);
            dados.Add(this.Emitente.CodigoPais.ToString());
            dados.Add(this.Emitente.DescricaoPais);
            dados.Add(this.Emitente.Fone);
            dados.Add(this.ModalidadeFrete.ToString());
            dados.Add(this.Serie.ToString());
            dados.Add(this.Numero.ToString());
            dados.Add(this.DataEmissao.ToString("yyyy-MM-dd"));
            dados.Add(this.Chave);
            dados.Add(this.Valor.ToString(cultura));
            dados.Add(this.Peso.ToString(cultura));
            dados.Add(this.Quantidade.ToString());
            dados.Add(this.Destinatario.CPFCNPJ);
            dados.Add(this.Destinatario.IE);
            dados.Add(this.Destinatario.Nome);
            dados.Add(this.Destinatario.Fone);
            dados.Add(this.Destinatario.Suframa);
            dados.Add(this.Destinatario.Logradouro);
            dados.Add(this.Destinatario.Numero);
            dados.Add(this.Destinatario.Complemento);
            dados.Add(this.Destinatario.Bairro);
            dados.Add(this.Destinatario.CodigoMunicipio.ToString());
            dados.Add(this.Destinatario.DescricaoMunicipio);
            dados.Add(this.Destinatario.UF);
            dados.Add(this.Destinatario.CEP);
            dados.Add(this.Destinatario.CodigoPais.ToString());
            dados.Add(this.Destinatario.DescricaoPais);
            dados.Add(this.Canhoto.ADAnterior.ToString());
            dados.Add(this.Canhoto.ADEntrega.ToString());
            dados.Add(this.Canhoto.ADCobranca.ToString());
            dados.Add(this.Canhoto.RegistroRevendedora.ToString());
            dados.Add(this.Canhoto.CampanhaEntrega.ToString());
            dados.Add(this.Canhoto.CampanhaCobranca.ToString());
            dados.Add(this.Canhoto.ValorEntrega.ToString(cultura));
            dados.Add(this.Canhoto.ValorCobranca);
            dados.Add(this.Canhoto.Setor.ToString());
            dados.Add(this.Canhoto.Rota);
            dados.Add(this.Canhoto.SequenciaEntrega);
            dados.Add(this.LocalRetirada.CPFCNPJ);
            dados.Add(this.LocalRetirada.Logradouro);
            dados.Add(this.LocalRetirada.Numero);
            dados.Add(this.LocalRetirada.Complemento);
            dados.Add(this.LocalRetirada.Bairro);
            dados.Add(this.LocalRetirada.CodigoMunicipio.ToString());
            dados.Add(this.LocalRetirada.DescricaoMunicipio); 
            dados.Add(this.LocalRetirada.UF);
            dados.Add(this.LocalEntrega.CPFCNPJ);
            dados.Add(this.LocalEntrega.Logradouro);
            dados.Add(this.LocalEntrega.Numero);
            dados.Add(this.LocalEntrega.Complemento);
            dados.Add(this.LocalEntrega.Bairro);
            dados.Add(this.LocalEntrega.CodigoMunicipio.ToString());
            dados.Add(this.LocalEntrega.DescricaoMunicipio);
            dados.Add(this.LocalEntrega.UF);

            this.Documento = string.Join("|", dados);
        }
    }

    public class Canhoto
    {
        public int ADAnterior { get; set; }
        public int ADEntrega { get; set; }
        public int ADCobranca { get; set; }
        public int RegistroRevendedora { get; set; }
        public int CampanhaEntrega { get; set; }
        public int CampanhaCobranca { get; set; }
        public decimal ValorEntrega { get; set; }
        public string ValorCobranca { get; set; }
        public int Setor { get; set; }
        public string Rota { get; set; }
        public string SequenciaEntrega { get; set; }
    }

    public class Pessoa
    {
        public string CPFCNPJ { get; set; }
        public string IE { get; set; }
        public string Suframa { get; set; }
        public string Nome { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public int CodigoMunicipio { get; set; }
        public string DescricaoMunicipio { get; set; }
        public string UF { get; set; }
        public string CEP { get; set; }
        public int CodigoPais { get; set; }
        public string DescricaoPais { get; set; }
        public string Fone { get; set; }
    }
}
