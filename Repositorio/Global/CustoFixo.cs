using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class CustoFixo : RepositorioBase<Dominio.Entidades.CustoFixo>, Dominio.Interfaces.Repositorios.CustoFixo
    {
        public CustoFixo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CustoFixo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CustoFixo>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.CustoFixo> Consultar(int codigoEmpresa, string descricao, string placaVeiculo, string nomeMorotista, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CustoFixo>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa.Contains(placaVeiculo));

            if (!string.IsNullOrWhiteSpace(nomeMorotista))
                result = result.Where(o => o.Funcionario.Nome.Contains(nomeMorotista));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string placaVeiculo, string nomeMorotista, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CustoFixo>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa.Contains(placaVeiculo));

            if (!string.IsNullOrWhiteSpace(nomeMorotista))
                result = result.Where(o => o.Funcionario.Nome.Contains(nomeMorotista));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.Entidades.CustoFixo> Relatorio(int codigoEmpresa, int codigoVeiculo, int codigoTipoCustoFixo, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CustoFixo>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") select obj;

            if (codigoTipoCustoFixo > 0)
                result = result.Where(o => o.TipoDeCustoFixo.Codigo == codigoTipoCustoFixo);

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => dataInicial <= o.DataFinal); //result = result.Where(o => o.DataInicial >= dataInicial && o.DataInicial <= dataFinal);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => dataFinal >= o.DataInicial); //result = result.Where(o => o.DataFinal >= dataInicial && o.DataFinal <= dataFinal);

            return result.ToList();
        }

        public List<Dominio.Entidades.CustoFixo> BuscarPorPeriodoEVeiculo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo, int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CustoFixo>();
            //var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") && obj.DataInicial >= dataInicial && obj.DataInicial <= dataFinal select obj;
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") && dataInicial <= obj.DataFinal && dataFinal >= obj.DataInicial select obj;

            if (codigoVeiculo > 0 && codigoMotorista > 0)
                result = result.Where(o => (o.Veiculo.Codigo == codigoVeiculo || o.Funcionario.Codigo == codigoMotorista));
            else if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);
            else if(codigoMotorista > 0)
                result = result.Where(o => o.Funcionario.Codigo == codigoMotorista);

            return result.ToList(); 
        }



        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCustoFixoAgrupado> ObterDadosSumarizados(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo)
        {
            throw new NotImplementedException();
        }
    }
}
