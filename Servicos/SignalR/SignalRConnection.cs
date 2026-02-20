namespace Servicos.SignalR
{
    public sealed class SignalRConnection
    {
        private static SignalRConnection _Instancia;

        public string ConnectionString { get; private set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; private set; }

        public int CodigoClienteMultisoftware { get; private set; }

        private SignalRConnection()
        {

        }

        public static SignalRConnection GetInstance()
        {
            if (_Instancia == null)
                _Instancia = new SignalRConnection();

            return _Instancia;
        }

        public void SetInstance(string connectionString, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoClienteMultisoftware)
        {
            GetInstance().setInstaceData(connectionString, tipoServicoMultisoftware, codigoClienteMultisoftware);
        }

        private void setInstaceData(string connectionString, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoClienteMultisoftware)
        {
            ConnectionString = connectionString;
            TipoServicoMultisoftware = tipoServicoMultisoftware;
            CodigoClienteMultisoftware = codigoClienteMultisoftware;
        }
    }
}
