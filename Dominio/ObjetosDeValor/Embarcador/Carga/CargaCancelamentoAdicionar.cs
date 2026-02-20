namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CargaCancelamentoAdicionar
    {
        public Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public bool DefinirSituacaoEmCancelamento { get; set; }

        public bool DuplicarCarga { get; set; }

        public bool GerarIntegracoes { get; set; } = true;

        public bool LiberarPedidosParaMontagemCarga { get; set; }

        public string MotivoCancelamento { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        public Entidades.Usuario Usuario { get; set; }

        public string UsuarioERPSolicitouCancelamento { get; set; } = "";

        public string ControleIntegracaoEmbarcador { get; set; }
    }
}
