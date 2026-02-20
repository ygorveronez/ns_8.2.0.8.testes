using System;

namespace Dominio.ObjetosDeValor.WebService.Devolucao
{
    public class AtualizarOcorrenciaDevolucao
    {
        public string NumeroOcorrencia { get; set; }

        public string OrigemOcorrecia { get; set; }

        public string CodCliente { get; set; }

        public string Assunto { get; set; }

        public string ChaveNFe { get; set; }

        public string ValorNFe { get; set; }

        public string AreaResponsavel { get; set; }

        public DateTime DataOcorrencia { get; set; }

        public string SituacaoOcorrencia { get; set; }

        public string CnpjTransportador { get; set; }

        public string Motivo { get; set; }

        public string Descricao { get; set; }

        public string UsuarioOcorrencia { get; set; }

        public string ChaveNFD { get; set; }

        public string ValorNFD { get; set; }

        public string FUP { get; set; }
    }
}
