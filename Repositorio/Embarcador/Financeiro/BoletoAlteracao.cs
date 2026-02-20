using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BoletoAlteracao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao>
    {
        public BoletoAlteracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao> Consulta(double cnpjPessoa, int codigoEmpresa, DateTime vencimentoInicial, DateTime vencimentoFinal, DateTime emissaoInicial, DateTime emissaoFinal, int codigoBoletoConfiguracao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao>();
            var result = from obj in query where obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoStatus.Aberto select obj;

            if (cnpjPessoa > 0)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == cnpjPessoa);

            if (codigoBoletoConfiguracao > 0)
                result = result.Where(obj => obj.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (vencimentoInicial > DateTime.MinValue && vencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoInicial >= vencimentoInicial && obj.DataVencimentoFinal <= vencimentoFinal);
            else if (vencimentoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoInicial >= vencimentoInicial);
            else if (vencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoFinal <= vencimentoFinal);

            if (emissaoInicial > DateTime.MinValue && emissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoInicial >= emissaoInicial && obj.DataEmissaoFinal <= emissaoFinal);
            else if (emissaoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoInicial >= emissaoInicial);
            else if (emissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoFinal <= emissaoFinal);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(double cnpjPessoa, int codigoEmpresa, DateTime vencimentoInicial, DateTime vencimentoFinal, DateTime emissaoInicial, DateTime emissaoFinal, int codigoBoletoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracao>();
            var result = from obj in query where obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoAlteracaoStatus.Aberto select obj;

            if (cnpjPessoa > 0)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == cnpjPessoa);

            if (codigoBoletoConfiguracao > 0)
                result = result.Where(obj => obj.BoletoConfiguracao.Codigo == codigoBoletoConfiguracao);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (vencimentoInicial > DateTime.MinValue && vencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoInicial >= vencimentoInicial && obj.DataVencimentoFinal <= vencimentoFinal);
            else if (vencimentoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoInicial >= vencimentoInicial);
            else if (vencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoFinal <= vencimentoFinal);

            if (emissaoInicial > DateTime.MinValue && emissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoInicial >= emissaoInicial && obj.DataEmissaoFinal <= emissaoFinal);
            else if (emissaoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoInicial >= emissaoInicial);
            else if (emissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoFinal <= emissaoFinal);

            return result.Count();
        }

    }
}
