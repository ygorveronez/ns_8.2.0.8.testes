using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class ContaBancaria : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria>
    {
        public ContaBancaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ContarConsulta(double codigoPortador, int codigoBanco, string agencia, string numeroConta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? tipoConta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria>();

            var result = from obj in query select obj;

            if (codigoPortador > 0)
                result = result.Where(o => o.ClientePortadorConta.CPF_CNPJ == codigoPortador);

            if (codigoBanco > 0)
                result = result.Where(o => o.Banco.Codigo == codigoBanco);

            if (!string.IsNullOrWhiteSpace(agencia))
                result = result.Where(o => o.Agencia.Contains(agencia));

            if (!string.IsNullOrWhiteSpace(numeroConta))
                result = result.Where(o => o.NumeroConta.Contains(numeroConta));

            if (tipoConta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Nenhum)
                result = result.Where(o => o.TipoContaBanco == tipoConta);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria> Consultar(double codigoPortador, int codigoBanco, string agencia, string numeroConta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? tipoConta, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria>();

            var result = from obj in query select obj;

            if (codigoPortador > 0)
                result = result.Where(o => o.ClientePortadorConta.CPF_CNPJ == codigoPortador);

            if (codigoBanco > 0)
                result = result.Where(o => o.Banco.Codigo == codigoBanco);

            if (!string.IsNullOrWhiteSpace(agencia))
                result = result.Where(o => o.Agencia.Contains(agencia));

            if (!string.IsNullOrWhiteSpace(numeroConta))
                result = result.Where(o => o.NumeroConta.Contains(numeroConta));

            if (tipoConta != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Nenhum)
                result = result.Where(o => o.TipoContaBanco == tipoConta);

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }
    }
}
