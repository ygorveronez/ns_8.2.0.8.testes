using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.AbastecimentoInterno
{
    public class MovimentacaoAbastecimentoSaida : RepositorioBase<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida>
    {
        public MovimentacaoAbastecimentoSaida(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida> Consultar(DateTime dataInicial, DateTime dataFinal, int codVeiculo, int codEmpresa, int codLocalArmazenamentoProduto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
           
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida>();

            var result = from obj in query select obj;

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataInicial && obj.Data <= dataFinal.AddDays(1));
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataInicial);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataFinal );

            if (codVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codVeiculo);

            if (codEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codEmpresa);

            if (codLocalArmazenamentoProduto > 0)
                result = result.Where(obj => obj.LocalArmazenamentoProduto.Codigo == codLocalArmazenamentoProduto);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();

        }

        public int ContarConsulta(DateTime dataInicial, DateTime dataFinal, int codVeiculo, int codEmpresa, int codLocalArmazenamentoProduto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            List<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida> result = Consultar(dataInicial, dataFinal, codVeiculo, codEmpresa, codLocalArmazenamentoProduto,propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}