using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public sealed class ConfiguracaoDisponibilidadeCarregamento
    {
        #region Propriedades

        public bool BloquearJanelaCarregamentoExcedente { get; set; }

        public int CodigoCarga { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public double CpfCnpjCliente { get; set; }

        public int DiasLimiteParaDefinicaoHorarioCarregamento { get; set; }

        public bool NotificarAlteracaoHorarioCarregamento { get; set; }

        public bool PermitirCapacidadeCarregamentoExcedida { get; set; }

        public bool PermitirHorarioCarregamentoComLimiteAtingido { get; set; }

        public bool PermitirHorarioCarregamentoInferiorAoAtual { get; set; }

        public bool ValidarSomenteEmResevas { get; set; }

        #endregion Propriedades

        #region Construtores

        public ConfiguracaoDisponibilidadeCarregamento() : this(cargaJanelaCarregamento: null) { }

        public ConfiguracaoDisponibilidadeCarregamento(Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            CodigoCarga = cargaJanelaCarregamento?.Carga?.Codigo ?? 0;
            CodigoModeloVeicularCarga = cargaJanelaCarregamento?.Carga?.ModeloVeicularCarga?.Codigo ?? 0;
            CodigoTipoOperacao = cargaJanelaCarregamento?.Carga?.TipoOperacao?.Codigo ?? 0;
            CodigoTransportador = cargaJanelaCarregamento?.Carga?.Empresa?.Codigo ?? 0;
            CpfCnpjCliente = cargaJanelaCarregamento?.Carga?.Pedidos?.FirstOrDefault()?.Pedido?.Destinatario?.CPF_CNPJ ?? 0;
            NotificarAlteracaoHorarioCarregamento = true;
        }

        #endregion Construtores
    }
}
