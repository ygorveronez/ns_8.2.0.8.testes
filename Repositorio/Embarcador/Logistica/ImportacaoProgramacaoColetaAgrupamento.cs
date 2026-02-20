using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class ImportacaoProgramacaoColetaAgrupamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento>
    {
        public ImportacaoProgramacaoColetaAgrupamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento> BuscarPorImportacaoProgramacaoColeta(int codigoImportacaoProgramacaoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento>();
            var result = from obj in query where obj.ImportacaoProgramacaoColeta.Codigo == codigoImportacaoProgramacaoColeta select obj;
            return result.ToList();
        }

        public bool PossuiAgrupamentoComFalha(int codigoImportacaoProgramacaoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento>();
            var result = from obj in query where obj.ImportacaoProgramacaoColeta.Codigo == codigoImportacaoProgramacaoColeta && obj.Carga == null select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento> Consultar(int codigoImportacaoProgramacaoColeta, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento>();

            var result = from obj in query where obj.ImportacaoProgramacaoColeta.Codigo == codigoImportacaoProgramacaoColeta select obj;

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(int codigoImportacaoProgramacaoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento>();

            var result = from obj in query where obj.ImportacaoProgramacaoColeta.Codigo == codigoImportacaoProgramacaoColeta select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento> BuscarProgramacaoColetaAnteriorComMesmoVeiculo(int codigoImportacaoProgramacaoColeta, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento>();
            var result = from obj in query
                         where obj.ImportacaoProgramacaoColeta.Codigo != codigoImportacaoProgramacaoColeta && obj.Carga.Veiculo.Codigo == codigoVeiculo &&
                               obj.ImportacaoProgramacaoColeta.SituacaoImportacaoProgramacaoColeta != SituacaoImportacaoProgramacaoColeta.Finalizado && obj.AgrupamentoNovaProgramacao == null
                         select obj;
            return result.ToList();
        }

        public bool TodosOsAgrupamentosEstaoEmNovasProgramacoes(int codigoImportacaoProgramacaoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColetaAgrupamento>();
            var result = from obj in query where obj.ImportacaoProgramacaoColeta.Codigo == codigoImportacaoProgramacaoColeta && obj.AgrupamentoNovaProgramacao == null select obj;
            return !result.Any();
        }
    }
}
