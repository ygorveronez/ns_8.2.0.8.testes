using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class GradeCarregamento
    {
        public GradeCarregamento(Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo periodo) : this(periodo.CapacidadeCarregamentoSimultaneo)
        {
            CodigoTipoOperacao = periodo.TipoOperacao.Codigo;
            TipoOperacao = periodo.TipoOperacao.Descricao;
        }

        public GradeCarregamento(Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao tipoOperacaoParada) : this(tipoOperacaoParada.Quantidade)
        {
            CodigoTipoOperacao = tipoOperacaoParada.TipoOperacao.Codigo;
            TipoOperacao = tipoOperacaoParada.TipoOperacao.Descricao;
        }

        public GradeCarregamento(int quantidade)
        {
            Disponiveis = quantidade;
            Exclusividades = new List<GradeCarregamentoExclusividade>();
        }

        public int CodigoTipoOperacao { get; set; }

        public string TipoOperacao { get; set; }

        public int Quantidade => Disponiveis - Exclusivas - Ocupadas;

        public int Ocupadas { get; set; }

        public int Disponiveis { get; set; }

        public int Exclusivas => Exclusividades.Sum(o => o.Quantidade);

        public List<GradeCarregamentoExclusividade> Exclusividades { get; set; }

        public bool PossuiDisponbilidadeTipoOperacao(int codigoTransportador, double codigoCliente, int codigoModeloVeicular)
        {
            var vagaExclusiva = (from obj in Exclusividades
                                 where
                                      (obj.Transportador == 0 || obj.Transportador == codigoTransportador)
                                      && (obj.Cliente == 0 || obj.Cliente == codigoCliente)
                                      && (obj.ModeloVeicular == 0 || obj.ModeloVeicular == codigoModeloVeicular)
                                 select obj).FirstOrDefault();

            if (vagaExclusiva == null)
                return Quantidade > 0;

            vagaExclusiva.Quantidade--;
            if (vagaExclusiva.Quantidade == 0)
                Exclusividades.Remove(vagaExclusiva);

            return true;
        }

        public bool PossuiDisponbilidadeTipoOperacao(JanelaCarregamento.CargaPeriodo cargaPeriodo)
        {
            return PossuiDisponbilidadeTipoOperacao(cargaPeriodo.Transportador ?? 0, cargaPeriodo.Destinatario ?? 0, cargaPeriodo.ModeloVeicularCarga ?? 0);
        }

        public void AddExclusividade(int codigoTransportador, double codigoCliente, int codigoModeloVeicular, int quantidadeExclusivas)
        {
            Exclusividades.Add(new GradeCarregamentoExclusividade(codigoTransportador, codigoCliente, codigoModeloVeicular, quantidadeExclusivas));
        }

        public void AddExclusividade(Entidades.Embarcador.Logistica.ExclusividadeCarregamento exclusividadeCarregamento, int quantidadeExclusivas)
        {
            AddExclusividade(exclusividadeCarregamento.Transportador?.Codigo ?? 0, exclusividadeCarregamento.Cliente?.CPF_CNPJ ?? 0, exclusividadeCarregamento.ModeloVeicularCarga?.Codigo ?? 0, quantidadeExclusivas);
        }
    }
}
