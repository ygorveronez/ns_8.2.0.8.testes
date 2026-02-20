using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BoletoRemessa : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa>
    {
        public BoletoRemessa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa> BuscarPorNotas(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public int BuscarProximaNumereracao(int codigoConfiguracaoBoleto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa>();
            var result = from obj in query where obj.BoletoConfiguracao.Codigo == codigoConfiguracaoBoleto select obj;
            if (result.Count() > 0)
            {
                result = result.OrderBy("NumeroSequencial descending");
                return result.FirstOrDefault().NumeroSequencial + 1;
            }
            else
                return 1;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa> Consultar(int codigoEmpresa, int numeroSequencial, int boletoConfiguracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DownloadRealizado statusDownload, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (boletoConfiguracao > 0)
                result = result.Where(obj => obj.BoletoConfiguracao.Codigo == boletoConfiguracao);

            if (numeroSequencial > 0)
                result = result.Where(obj => obj.NumeroSequencial == numeroSequencial);

            if ((int)statusDownload > 0)
            {
                if (statusDownload == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DownloadRealizado.Pendente)
                    result = result.Where(obj => !obj.DownloadRealizado);
                else if (statusDownload == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DownloadRealizado.Realizado)
                    result = result.Where(obj => obj.DownloadRealizado);
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int numeroSequencial, int boletoConfiguracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DownloadRealizado statusDownload)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (boletoConfiguracao > 0)
                result = result.Where(obj => obj.BoletoConfiguracao.Codigo == boletoConfiguracao);

            if (numeroSequencial > 0)
                result = result.Where(obj => obj.NumeroSequencial == numeroSequencial);

            if ((int)statusDownload > 0)
            {
                if (statusDownload == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DownloadRealizado.Pendente)
                    result = result.Where(obj => !obj.DownloadRealizado);
                else if (statusDownload == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DownloadRealizado.Realizado)
                    result = result.Where(obj => obj.DownloadRealizado);
            }

            return result.Count();
        }
    }
}