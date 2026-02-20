using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.AbastecimentoInterno
{
    public class MovimentoEntradaTanque : RepositorioBase<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque>
    {
        public MovimentoEntradaTanque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque> Consultar(string descricao, DateTime dataInicialEntrada, DateTime dataFinalEntrada, int codTipoOleo, int codEmpresa,int codNotaFiscalEntrada, int codLocalArmazenamentoProduto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (dataInicialEntrada > DateTime.MinValue && dataFinalEntrada > DateTime.MinValue)
                result = result.Where(obj => obj.DataHoraEntrada >= dataInicialEntrada && obj.DataHoraEntrada <= dataFinalEntrada);
            else if (dataInicialEntrada > DateTime.MinValue)
                result = result.Where(obj => obj.DataHoraEntrada >= dataInicialEntrada);
            else if (dataFinalEntrada > DateTime.MinValue)
                result = result.Where(obj => obj.DataHoraEntrada >= dataFinalEntrada);

            if (codTipoOleo > 0)
                result = result.Where(obj => obj.TipoOleo.Codigo == codTipoOleo);

            if (codEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codEmpresa);

            if (codNotaFiscalEntrada > 0)
                result = result.Where(obj => obj.DocumentoEntrada.Codigo == codNotaFiscalEntrada);

            if (codLocalArmazenamentoProduto > 0)
                result = result.Where(obj => obj.LocalArmazenamentoProduto.Codigo == codLocalArmazenamentoProduto);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();

        }

        public int ContarConsulta(string descricao, DateTime dataInicial, DateTime dataFinal, int codTipoOleo, int codEmpresa, int codNotaFiscalEntrada, int codLocalArmazenamentoProduto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            List<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque> result = Consultar(descricao, dataInicial, dataFinal, codTipoOleo, codEmpresa, codNotaFiscalEntrada, codLocalArmazenamentoProduto, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}