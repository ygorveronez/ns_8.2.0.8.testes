using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class ContratoFreteAcrescimoDesconto
    {
        public string NumCiot { get; set; }

        public int NumContratoFrete { get; set; }

        public string NumCarga { get; set; }

        public string Justificativa { get; set; }

        public decimal Valor { get; set; }

        public string Observacao { get; set; }

        private DateTime DataLancamento { get; set; }

        private SituacaoContratoFreteAcrescimoDesconto Situacao { get; set; }

        private int RetornoIntegracao { get; set; }

        public string CPFAutonomo { get; set; }

        public string NomeAutonomo { get; set; }

        public string OperadorSolicitante{ get; set; }

        public string DataLancamentoFormatada
        {
            get { return DataLancamento != DateTime.MinValue ? DataLancamento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string SituacaoFormatada
        {
            get{ return Situacao.ObterDescricao(); }
        }
        public string RetornoIntegracaoFormatada
        {
            get{ 
            switch (RetornoIntegracao)
                {
                    case 1:
                        return "sucesso";
                        
                    case 2:
                        return "Erro";
                    
                    case 3:
                        return "Rejeicao";
                    
                    case 4:
                        return "Aguardando";
                        
                    default:
                        return " ";
            
                }

            }
        }

        
    }
}
