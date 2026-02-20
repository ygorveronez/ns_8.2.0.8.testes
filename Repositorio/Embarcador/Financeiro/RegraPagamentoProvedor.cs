using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    public class RegraPagamentoProvedor : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor>
    {
        #region Construtores

        public RegraPagamentoProvedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> BuscarPorAtivaOuConfiguracao(TipoDocumentoProvedor tipoDocumentoNFSe)
        {
            System.DateTime dataAtual = System.DateTime.Today;

            var listaRegrasAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor>()
                .Where(regra => regra.Ativo && (regra.Vigencia == null || regra.Vigencia >= dataAtual))
                .ToList();

            if (tipoDocumentoNFSe != TipoDocumentoProvedor.NFSe)
            {
                listaRegrasAutorizacao = listaRegrasAutorizacao.Where(regra => (regra.IsAlcadaAtiva() || regra.ValidarTodosCamposAuditoriaDocumentoProvedor))
                    .ToList();
            }
            else
                listaRegrasAutorizacao = listaRegrasAutorizacao.Where(regra => regra.IsAlcadaAtiva()).ToList();

            return listaRegrasAutorizacao;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor> BuscarMultiplosCTe()
        {
            System.DateTime dataAtual = System.DateTime.Today;

            var listaRegrasAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor>()
                .Where(regra => regra.Ativo && (regra.Vigencia == null || regra.Vigencia >= dataAtual))
                .ToList();

            listaRegrasAutorizacao = listaRegrasAutorizacao.Where(regra => regra.BloquearPagamentoMultiplosCTe)
                .ToList();

            return listaRegrasAutorizacao;
        }

        public Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor BuscarPorCodigoRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor>();
            var result = from obj in query where obj.OrigemAprovacao.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
