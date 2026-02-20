using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class Servico : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.Servico>
    {
        public Servico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.Servico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Servico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.NotaFiscal.Servico BuscarPorCodigoServico(string codigoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Servico>();
            var result = from obj in query where obj.CodigoServico == obj.CodigoServicoParaEnum(codigoServico) select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.NotaFiscal.Servico BuscarPorCodigoIntegracao(string codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Servico>();
            var result = from obj in query where obj.CodigoIntegracao == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.Servico BuscarPorCodigoServicoNFSe(int codigoServicoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Servico>();
            var result = from obj in query where obj.ServicoNFSe.Codigo == codigoServicoNFSe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.Servico> BuscarPorCodigosServicoNFSe(List<int> codigoServicoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Servico>();
            var result = from obj in query where codigoServicoNFSe.Contains(obj.ServicoNFSe.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.Servico> Consultar(int codigo, string descricao, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Servico>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigo, string descricao, int empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Servico>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);

            return result.Count();
        }

        public bool ContemServicoMesmoCodigoIntegracao(string codigoIntegracao, int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.Servico>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;

            if (codigo > 0)
                result = result.Where(o => o.Codigo != codigo);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.Any();
        }

        #endregion
    }
}
