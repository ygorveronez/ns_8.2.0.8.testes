using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class LogLeituraArquivoOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia>
    {
        public LogLeituraArquivoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia> _Consultar(string arquivo, string ocorrencia, int empresa, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioArquivo tipoEnvioArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia>();

            if (!string.IsNullOrWhiteSpace(arquivo))
                query = query.Where(obj => obj.NomeArquivo.Contains(arquivo));

            if (!string.IsNullOrWhiteSpace(ocorrencia))
                query = query.Where(obj => obj.OcorrenciasGeradas.Contains(ocorrencia));

            if (empresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == empresa);

            if (dataInicio != DateTime.MinValue)
                query = query.Where(obj => obj.DataRecebimento.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                query = query.Where(obj => obj.DataRecebimento.Date <= dataFim);

            if (tipoEnvioArquivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioArquivo.Todos)
                query = query.Where(obj => obj.TipoEnvioArquivo == tipoEnvioArquivo);

            return query;
        }


        public List<Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia> Consultar(string arquivo, string ocorrencia, int empresa, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioArquivo tipoEnvioArquivo, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(arquivo, ocorrencia, empresa, dataInicio, dataFim, tipoEnvioArquivo);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Usuario)
                .ToList();
        }

        public int ContarConsulta(string arquivo, string ocorrencia, int empresa, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioArquivo tipoEnvioArquivo)
        {
            var result = _Consultar(arquivo, ocorrencia, empresa, dataInicio, dataFim, tipoEnvioArquivo);

            return result.Count();
        }


    }
}
