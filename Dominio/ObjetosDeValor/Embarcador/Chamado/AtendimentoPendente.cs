using System;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public class AtendimentoPendente
    {

        public int Codigo { get; set; }

        public int CodigoOrdenacao { get; set; }

        public int Numero { get; set; }

        public string NumeroCarga { get; set; }

        public string GrupoMotivoAtendimento { get; set; }

        public string MotivoChamado { get; set; }

        public string Transportador { get; set; }

        public string Cliente { get; set; }

        public string Veiculo { get; set; }

        public string NotasFiscais { get; set; }

        public string Filial { get; set; }

        public DateTime DataCriacao { get; set; }

        public string DataCriacaoDescricao { get { return this.DataCriacao.ToString("g"); } }
    }
}