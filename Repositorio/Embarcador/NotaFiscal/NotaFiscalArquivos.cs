using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalArquivos : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>
    {
        public NotaFiscalArquivos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> BuscarPorNotas(List<int> codigosCTes, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>();

            var result = from obj in query where codigosCTes.Contains(obj.NotaFiscal.Codigo) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.NotaFiscal.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos BuscarPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> BuscarArquivosParaAssinar(string cnpjEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>();
            var result = from obj in query where obj.NotaFiscal.Empresa.CNPJ.StartsWith(cnpjEmpresa) && obj.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.AguardandoAssinar && obj.XMLNaoAssinado != null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> BuscarArquivosParaInutilizar(string cnpjEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>();
            var result = from obj in query where obj.NotaFiscal.Empresa.CNPJ.StartsWith(cnpjEmpresa) && obj.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.AguardandoInutilizarAssinar && obj.XMLInutilizacaoNaoAssinado != null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> BuscarArquivosParaCancelar(string cnpjEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>();
            var result = from obj in query where obj.NotaFiscal.Empresa.CNPJ.StartsWith(cnpjEmpresa) && obj.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.AguardandoCancelarAssinar && obj.XMLCancelamentoNaoAssinado != null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> BuscarArquivosParaCartaCorrecao(string cnpjEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>();
            var result = from obj in query where obj.NotaFiscal.Empresa.CNPJ.StartsWith(cnpjEmpresa) && obj.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.AguardandoCartaCorrecaoAssinar && obj.XMLCartaCorrecaoNaoAssinado != null select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos BuscarUltimoPorNota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>();
            var result = from obj in query where obj.NotaFiscal.Codigo == codigo select obj;
            result = result.OrderByDescending(obj => obj.Codigo);
            return result.FirstOrDefault();
        }

    }
}
