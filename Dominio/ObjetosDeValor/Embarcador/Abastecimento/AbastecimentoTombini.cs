namespace Dominio.ObjetosDeValor.Embarcador.Abastecimento
{
    public class Raiz
    {
        public Header Header { get; set; }

        public Transacoes Transacoes { get; set; }
    }

    public class Header
    {
        public string NomeArquivo { get; set; }
        public string DataGeracao { get; set; }
        public string HoraGeracao { get; set; }
    }

    public class Transacoes
    {
        public string CodigoTransacao { get; set; }
        public string DataTransacao { get; set; }
        public string NumeroCartao { get; set; }
        public string Placa { get; set; }
        public string Fabricante { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public string Ano { get; set; }
        public string NumeroFrota { get; set; }
        public string NumeroMatricula { get; set; }
        public string NomeMotorista { get; set; }
        public string RazaoSocial { get; set; }
        public string EstabelecimentoCNPJ { get; set; }
        public string NomeReduzido { get; set; }
        public string NomeCidade { get; set; }
        public string UF { get; set; }
        public string Quilometragem { get; set; }
        public string TipoCombustivel { get; set; }
        public string Litros { get; set; }
        public string ValorTransacao { get; set; }
        public int Odometro { get; set; }
    }
}
