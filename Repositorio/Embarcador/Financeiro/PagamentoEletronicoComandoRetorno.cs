using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class PagamentoEletronicoComandoRetorno : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno>
    {
        public PagamentoEletronicoComandoRetorno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno BuscarComando(int codigoBoletoConfiguracao, string comando)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno>();
            var result = from obj in query where obj.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao && obj.Ativo == true &&  obj.Comando == comando select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno> Consultar(int codigoEmpresa, int codigoBoletoConfiguracao, string comando, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoBoletoConfiguracao > 0)
                result = result.Where(obj => obj.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(comando))
                result = result.Where(obj => obj.Comando.Contains(comando));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));
            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();

        }

        public int ContarConsulta(int codigoEmpresa, int codigoBoletoConfiguracao, string comando, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoBoletoConfiguracao > 0)
                result = result.Where(obj => obj.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(comando))
                result = result.Where(obj => obj.Comando.Contains(comando));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }


    }
}
