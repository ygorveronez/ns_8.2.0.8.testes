using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BoletoConfiguracao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>
    {
        public BoletoConfiguracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao BuscarPorDescricaoBanco(string descricaoBanco)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>();
            var result = from obj in query where obj.DescricaoBanco == descricaoBanco select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao BuscarPrimeiraConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>();
            var result = from obj in query where !obj.UtilizaConfiguracaoPagamentoEletronico select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao> Consultar(bool? utilizaConfiguracaoPagamentoEletronico, string numeroBanco, int codigoEmpresa, string numeroAgencia, string numeroConta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco banco, int situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>();

            var result = from obj in query select obj;

            if (utilizaConfiguracaoPagamentoEletronico != null && utilizaConfiguracaoPagamentoEletronico.HasValue)
            {
                if (utilizaConfiguracaoPagamentoEletronico.Value == true)
                    result = result.Where(obj => obj.UtilizaConfiguracaoPagamentoEletronico == utilizaConfiguracaoPagamentoEletronico.Value);
                else
                    result = result.Where(obj => obj.UtilizaConfiguracaoPagamentoEletronico == utilizaConfiguracaoPagamentoEletronico.Value || obj.UtilizaConfiguracaoPagamentoEletronico == null);
            }

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(numeroAgencia))
                result = result.Where(obj => obj.NumeroAgencia.Contains(numeroAgencia));

            if (!string.IsNullOrWhiteSpace(numeroBanco))
                result = result.Where(obj => obj.NumeroBanco.Equals(numeroBanco));

            if (!string.IsNullOrWhiteSpace(numeroConta))
                result = result.Where(obj => obj.NumeroConta.Contains(numeroConta));

            if ((int)banco > 0)
                result = result.Where(obj => obj.BoletoBanco == banco);

            if (situacao > 0)
                result = result.Where(obj => obj.Situacao == (situacao == 1 ? true : false));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(bool? utilizaConfiguracaoPagamentoEletronico, string numeroBanco, int codigoEmpresa, string numeroAgencia, string numeroConta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoBanco banco, int situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>();

            var result = from obj in query select obj;

            if (utilizaConfiguracaoPagamentoEletronico != null && utilizaConfiguracaoPagamentoEletronico.HasValue)
            {
                if (utilizaConfiguracaoPagamentoEletronico.Value == true)
                    result = result.Where(obj => obj.UtilizaConfiguracaoPagamentoEletronico == utilizaConfiguracaoPagamentoEletronico.Value);
                else
                    result = result.Where(obj => obj.UtilizaConfiguracaoPagamentoEletronico == utilizaConfiguracaoPagamentoEletronico.Value || obj.UtilizaConfiguracaoPagamentoEletronico == null);
            }

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(numeroAgencia))
                result = result.Where(obj => obj.NumeroAgencia.Contains(numeroAgencia));

            if (!string.IsNullOrWhiteSpace(numeroBanco))
                result = result.Where(obj => obj.NumeroBanco.Equals(numeroBanco));

            if (!string.IsNullOrWhiteSpace(numeroConta))
                result = result.Where(obj => obj.NumeroConta.Contains(numeroConta));

            if ((int)banco > 0)
                result = result.Where(obj => obj.BoletoBanco == banco);

            if (situacao > 0)
                result = result.Where(obj => obj.Situacao == (situacao == 1 ? true : false));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao>();
            var result = from obj in query select obj;
            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            return result.ToList();
        }
    }
}
