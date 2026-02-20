namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class GestaoPatioEtapa
    {
        public string Descricao { get; set; }

        public Enumeradores.EtapaFluxoGestaoPatio Etapa { get; set; }

        public bool PermiteQRCode { get; set; }

        public int Ordem { get; set; }

        public bool CheckListPermiteSalvarSemPreencher { get; set; }

        public string CodigoIntegracao { get; set; }
    }
}
