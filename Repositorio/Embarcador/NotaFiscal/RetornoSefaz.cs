using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class RetornoSefaz : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz>
    {
        public RetornoSefaz(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz> Consultar(int codigo, string mensagemRetornoSefaz, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(mensagemRetornoSefaz))
                result = result.Where(obj => obj.MensagemRetornoSefaz.Contains(mensagemRetornoSefaz));

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigo, string mensagemRetornoSefaz, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(mensagemRetornoSefaz))
                result = result.Where(obj => obj.MensagemRetornoSefaz.Contains(mensagemRetornoSefaz));

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);


            return result.Count();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz ContemRetornoSefazAtivo(string mensagemRetornoSefaz)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(mensagemRetornoSefaz))
                result = result.Where(obj => mensagemRetornoSefaz.Contains(obj.MensagemRetornoSefaz));

            result = result.Where(o => o.Status);

            return result.FirstOrDefault();
        }

        #endregion
    }
}

