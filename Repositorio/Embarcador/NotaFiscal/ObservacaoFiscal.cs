using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ObservacaoFiscal : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal>
    {
        public ObservacaoFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal> Consultar(string descricao, int empresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Observacao.Contains(descricao));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Observacao.Contains(descricao));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal> BuscarPorDadosNota(int codigoEmpresa, string siglaDestinatario, int codigoAtividade, int codigoNaturezaDaOperacao, List<string> ncms, List<int> cfops, List<int> csts)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ObservacaoFiscal>();

            var result = from obj in query
                         where
                            (obj.Estado != null || obj.Atividade != null || obj.NaturezaDaOperacao != null || obj.CFOP != null || obj.CSTICMS != null || obj.NCMProduto != string.Empty) &&
                            (obj.Estado.Sigla == siglaDestinatario || obj.Estado == null) &&
                            (obj.Atividade.Codigo == codigoAtividade || obj.Atividade == null) &&
                            (obj.NaturezaDaOperacao.Codigo == codigoNaturezaDaOperacao || obj.NaturezaDaOperacao == null)
                         select obj;

            if (cfops.Count > 0)
                result = result.Where(obj => cfops.Contains(obj.CFOP.Codigo) || obj.CFOP == null);

            if (csts.Count > 0)
                result = result.Where(obj => csts.Contains((int)obj.CSTICMS) || obj.CSTICMS == null);

            if (ncms.Count > 0)
                result = result.Where(obj => ncms.Contains(obj.NCMProduto) || obj.NCMProduto == string.Empty);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }
    }
}
