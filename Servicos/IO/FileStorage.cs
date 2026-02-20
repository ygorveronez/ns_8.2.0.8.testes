namespace Servicos.IO
{
    public static class FileStorage
    {
        private static readonly object _locker = new object();
        private static AdminMultisoftware.Dominio.Enumeradores.FileStorageType? _fileStorageType;

        public static void ConfigureApplicationFileStorage(string adminMultisoftwareConnectionString, string host)
        {
            if (_fileStorageType.HasValue)
                return; //storage already configured

            lock (_locker)
            {
                if (_fileStorageType.HasValue)
                    return; //storage already configured

                using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminMultisoftwareConnectionString))
                {
                    AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWork);
                    AdminMultisoftware.Repositorio.Pessoas.ClienteConfiguracaoFileStorage repClienteConfiguracaoFileStorage = new AdminMultisoftware.Repositorio.Pessoas.ClienteConfiguracaoFileStorage(unitOfWork);

                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorURL(host);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracaoFileStorage clienteConfiguracaoFileStorage = repClienteConfiguracaoFileStorage.BuscarPorCliente(clienteURLAcesso.Cliente.Codigo, clienteURLAcesso.URLHomologacao);

                    _fileStorageType = clienteConfiguracaoFileStorage?.TipoFileStorage ?? AdminMultisoftware.Dominio.Enumeradores.FileStorageType.Local;

                    switch (_fileStorageType)
                    {
                        case AdminMultisoftware.Dominio.Enumeradores.FileStorageType.Azure:
                            Utilidades.IO.FileStorageService.ConfigureWithAzureDefault(clienteConfiguracaoFileStorage.AzureConnectionString, clienteConfiguracaoFileStorage.AzureContainerName);
                            break;
                        case AdminMultisoftware.Dominio.Enumeradores.FileStorageType.Local:
                            Utilidades.IO.FileStorageService.ConfigureWithLocalDefault();
                            break;
                    }
                }
            }
            }
    }
}
