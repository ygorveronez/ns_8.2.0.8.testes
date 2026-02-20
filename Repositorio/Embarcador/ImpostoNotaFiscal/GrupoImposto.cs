using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ImpostoNotaFiscal
{
    public class GrupoImposto : RepositorioBase<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto>
    {
        public GrupoImposto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto> Consultar(string ncm, int codigoEmpresaPai, int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0 && codigoEmpresaPai > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai);

            if (!string.IsNullOrWhiteSpace(ncm))
                result = result.Where(obj => obj.NCM.Contains(ncm) || obj.NCM == "" || obj.NCM == null);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string ncm, int codigoEmpresaPai, int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0 && codigoEmpresaPai > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai);

            if (!string.IsNullOrWhiteSpace(ncm))
                result = result.Where(obj => obj.NCM.Contains(ncm) || obj.NCM == "" || obj.NCM == null);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }
    }
}
