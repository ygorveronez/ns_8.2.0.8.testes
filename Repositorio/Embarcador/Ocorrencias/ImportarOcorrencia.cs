using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Ocorrencias
{
    public class ImportarOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia>
    {
        public ImportarOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia> BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia>();
            var result = from obj in query where obj.SituacaoImportarOcorrencia == situacao select obj;
            return result.ToList();
        }

        public bool RegistroDuplicado(string numeroCarga, string codigoTipoOcorrencia, string codigoComponenteFrete, int numeroCTe, int numeroSerie, decimal valorOcorrencia, DateTime? dataOcorrencia, decimal aliquotaICMS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia>();
            var result = from obj in query where obj.SituacaoImportarOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia.Falha && obj.NumeroCarga == numeroCarga && obj.CodigoTipoOcorrencia == codigoTipoOcorrencia && obj.CodigoComponenteFrete == codigoComponenteFrete && obj.NumeroCTe == numeroCTe && obj.SerieCTe == numeroSerie && obj.ValorOcorrencia == valorOcorrencia && obj.DataOcorrencia == dataOcorrencia select obj;

            if (aliquotaICMS > 0)
                result = result.Where(obj => obj.AliquotaICMS == aliquotaICMS);

            return result.Any();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia> _Consultar(string numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia>();

            var result = from obj in query select obj;

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia.Todos)
                result = result.Where(o => o.SituacaoImportarOcorrencia == situacao);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                result = result.Where(o => o.NumeroCarga == numeroCarga || o.Carga.CodigoCargaEmbarcador == numeroCarga);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia> Consultar(string numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(numeroCarga, situacao);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(string numeroCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportarOcorrencia situacao)
        {
            var result = _Consultar(numeroCarga, situacao);

            return result.Count();
        }
    }
}
