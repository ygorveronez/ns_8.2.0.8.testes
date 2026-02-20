using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.AcompanhamentoChecklist
{
    public sealed class AcompanhamentoChecklist
    {
        public int Codigo { get; set; }

        public int CodigoCarga { get; set; }

        public string Carga { get; set; }

        public string Filial { get; set; }

        public string TransportadorNome { get; set; }

        public string TransportadorCNPJ { get; set; }

        public bool Situacao { get; set; }

        public string Veiculos { get; set; }

        public string SituacaoFormatada
        {
            get { return Situacao ? "Lido" : "Não Lido"; }
        }

        public string Transportador
        {
            get
            {
                string descricao = "";

                if (!string.IsNullOrWhiteSpace(TransportadorNome))
                    descricao += TransportadorNome.Trim();
                if (!string.IsNullOrWhiteSpace(TransportadorCNPJ))
                    descricao += " - " + TransportadorCNPJ.ObterCnpjFormatado();

                return descricao;
            }
        }

        public string OrdemCklist { get; set; }
    }
}
