using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEmbarque
{
    public class RelacaoEmbarque
    {
        public int NumeroNota { get; set; }
        public string Serie { get; set; }
        public string Tomador { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string Destino { get; set; }
        public decimal Peso { get; set; }
        public int Volumes { get; set; }
        public decimal Pecas { get; set; }
        public decimal ValorTotal { get; set; }
        public ClassificacaoNFe ClassificacaoNF { get; set; }

        public string NumeroPedido { get; set; }

        public virtual string ClassificacaoNFDescricao
        {
            get
            {                
                    return ClassificacaoNFeHelper.ObterDescricao(ClassificacaoNF);
            }
        }
    }
}
