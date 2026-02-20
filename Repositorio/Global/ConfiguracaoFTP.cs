using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;


namespace Repositorio
{
    public class ConfiguracaoFTP : RepositorioBase<Dominio.Entidades.ConfiguracaoFTP>, Dominio.Interfaces.Repositorios.ConfiguracaoFTP
    {
        public ConfiguracaoFTP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoFTP BuscarPorCodigo(int codigo, int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoFTP>();
            var result = from obj in query where obj.Codigo == codigo && obj.Configuracao.Codigo == codigoConfiguracao select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.ConfiguracaoFTP> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoFTP>();
            var result = from obj in query where obj.Configuracao.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ConfiguracaoFTP> BuscarTodasImportacao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoFTP>();
            var result = from obj in query
                         where (obj.Tipo == Dominio.Enumeradores.TipoArquivoFTP.ImportacaoNOTFIS ||
                                obj.Tipo == Dominio.Enumeradores.TipoArquivoFTP.ImportacaoXMLNFe)
                         select obj;

            return result.Fetch(o => o.Configuracao).Fetch(o => o.Cliente).ToList();
        }

        public List<Dominio.Entidades.ConfiguracaoFTP> BuscarTodasEnvio()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoFTP>();
            var result = from obj in query
                         where (obj.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioCONEMB ||
                                obj.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioOCORENCTe ||
                                obj.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioOCORENNFSe ||
                                obj.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioOCORENNFe ||
                                obj.Tipo == Dominio.Enumeradores.TipoArquivoFTP.EnvioXMLCTe)
                         select obj;

            return result.Fetch(o => o.Configuracao).Fetch(o => o.Cliente).ToList();
        }

        public Dominio.Entidades.ConfiguracaoFTP BuscarPorConfiguracaoClienteTipo(int codigoConfiguracao, double cliente, Dominio.Enumeradores.TipoArquivoFTP tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoFTP>();
            var result = from obj in query where obj.Configuracao.Codigo == codigoConfiguracao && obj.Cliente.CPF_CNPJ == cliente && obj.Tipo == tipo select obj;
            return result.FirstOrDefault();
        }

    }
}
