using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class CargaOcorrenciaSumarizado : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado>
    {
        public CargaOcorrenciaSumarizado(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaOcorrenciaSumarizado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado BuscarPorCodigoVeiculoEOcorrencia(int veiculo, int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado>();
            var result = from obj in query where obj.Veiculo.Codigo == veiculo && obj.CargaOcorrencia.Codigo == ocorrencia select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado> BuscarCargasOcorrenciaPorOcorrencia(int ocorrencia)
        {
            var result = _ConsultarCargasOcorrenciaPorOcorrencia(ocorrencia);

            return result.ToList();
        }

        public int ContarCargasOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            var result = from obj in query where obj.Codigo == ocorrencia select obj.Cargas.Count();

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado> ConsultarCargasOcorrenciaPorOcorrencia(int ocorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarCargasOcorrenciaPorOcorrencia(ocorrencia);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result
                .Fetch(obj => obj.Veiculo)
                .ThenFetch(obj => obj.ModeloVeicularCarga)
                .ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado>> ConsultarCargasOcorrenciaPorOcorrenciaAsync(int ocorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarCargasOcorrenciaPorOcorrencia(ocorrencia);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return await result
                .Fetch(obj => obj.Veiculo)
                .ThenFetch(obj => obj.ModeloVeicularCarga)
                .ToListAsync();
        }
        public int ContarConsultaCargasOcorrenciaPorOcorrencia(int ocorrencia)
        {
            var result = _ConsultarCargasOcorrenciaPorOcorrencia(ocorrencia);

            return result.Count();
        }
        public async Task<int> ContarConsultaCargasOcorrenciaPorOcorrenciaAsync(int ocorrencia)
        {
            var result = _ConsultarCargasOcorrenciaPorOcorrencia(ocorrencia);

            return await result.CountAsync();
        }
        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado> _ConsultarCargasOcorrenciaPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado>();

            var result = from o in query where o.CargaOcorrencia.Codigo == ocorrencia select o;

            return result;
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> ConsultarCargasOcorrenciaPorPeriodo(DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, bool filtrarCargasPeriodo, int tipoOcorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string sql = ObterSelectConsultaCargasPeriodo(false, periodoInicial, periodoFim, transportador, filial, proprietario, filtrarCargasPeriodo, tipoOcorrencia, propOrdena, dirOrdena, inicioRegistros, maximoRegistros);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado>();
        }
        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado>> ConsultarCargasOcorrenciaPorPeriodoAsync(DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, bool filtrarCargasPeriodo, int tipoOcorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string sql = ObterSelectConsultaCargasPeriodo(false, periodoInicial, periodoFim, transportador, filial, proprietario, filtrarCargasPeriodo, tipoOcorrencia, propOrdena, dirOrdena, inicioRegistros, maximoRegistros);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado)));

            return (List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado>)await query.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado>();
        }
        public int ContarConsultaCargasOcorrenciaPorPeriodo(DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, bool filtrarCargasPeriodo, int tipoOcorrencia)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaCargasPeriodo(true, periodoInicial, periodoFim, transportador, filial, proprietario, filtrarCargasPeriodo, tipoOcorrencia, string.Empty, string.Empty, 0, 0));

            return query.UniqueResult<int>();
        }
        public async Task<int> ContarConsultaCargasOcorrenciaPorPeriodoAsync(DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, bool filtrarCargasPeriodo, int tipoOcorrencia)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaCargasPeriodo(true, periodoInicial, periodoFim, transportador, filial, proprietario, filtrarCargasPeriodo, tipoOcorrencia, string.Empty, string.Empty, 0, 0));

            return await query.UniqueResultAsync<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados> ConsultarDocumentosAgrupados(DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string placa, int contrato, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string sql = ObterSelectConsultaDocumentosAgrupados(false, periodoInicial, periodoFim, transportador, proprietario, placa, contrato, 0, propOrdena, dirOrdena, inicioRegistros, maximoRegistros);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>();
        }
        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>> ConsultarDocumentosAgrupadosAsync(DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string placa, int contrato, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string sql = ObterSelectConsultaDocumentosAgrupados(false, periodoInicial, periodoFim, transportador, proprietario, placa, contrato, 0, propOrdena, dirOrdena, inicioRegistros, maximoRegistros);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados)));

            return (List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>)await query.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados> ConsultarDocumentosAgrupadosDaOcorrencia(int ocorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string sql = ObterSelectConsultaDocumentosAgrupados(false, DateTime.MinValue, DateTime.MinValue, 0, "", "", 0, ocorrencia, propOrdena, dirOrdena, inicioRegistros, maximoRegistros);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>();
        }
        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>> ConsultarDocumentosAgrupadosDaOcorrenciaAsync(int ocorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string sql = ObterSelectConsultaDocumentosAgrupados(false, DateTime.MinValue, DateTime.MinValue, 0, "", "", 0, ocorrencia, propOrdena, dirOrdena, inicioRegistros, maximoRegistros);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados)));

            return (List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>)await query.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>();
        }

        public int ContarConsultaDocumentosAgrupados(DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string placa, int contrato)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaDocumentosAgrupados(true, periodoInicial, periodoFim, transportador, proprietario, placa, contrato, 0, string.Empty, string.Empty, 0, 0));

            return query.UniqueResult<int>();
        }

        public async Task<int> ContarConsultaDocumentosAgrupadosAsync(DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string placa, int contrato)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaDocumentosAgrupados(true, periodoInicial, periodoFim, transportador, proprietario, placa, contrato, 0, string.Empty, string.Empty, 0, 0));

            return await query.UniqueResultAsync<int>();
        }


        public int ContarConsultaDocumentosAgrupadosDaOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaDocumentosAgrupados(true, DateTime.MinValue, DateTime.MinValue, 0, "", "", 0, ocorrencia, string.Empty, string.Empty, 0, 0));

            return query.UniqueResult<int>();
        }
        public async Task<int> ContarConsultaDocumentosAgrupadosDaOcorrenciaAsync(int ocorrencia)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaDocumentosAgrupados(true, DateTime.MinValue, DateTime.MinValue, 0, "", "", 0, ocorrencia, string.Empty, string.Empty, 0, 0));

            return await query.UniqueResultAsync<int>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarDocumentosCargasOcorrenciaPorPeriodo(int ocorrencia, int veiculo, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var resultCargas = (from o in queryCargas where o.Codigo == ocorrencia select o).FirstOrDefault();
            /*var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var resultCargas = (from o in queryCargas where o.Codigo == ocorrencia select o).FirstOrDefault();
            var codigosCargas = (from o in resultCargas.Cargas select o.Codigo).ToArray();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from o in query where codigosCargas.Contains(o.Carga.Codigo) && o.CTe != null && o.CargaCTeComplementoInfo == null select o;*/

            var result = _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(ocorrencia, null, null, 0, 0, "", veiculo, resultCargas.TipoOcorrencia.FiltrarCargasPeriodo, resultCargas.TipoOcorrencia.Codigo);
            
            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> ConsultarDocumentosCargasOcorrenciaPorPeriodoAsync(int ocorrencia, int veiculo, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var resultCargas = (from o in queryCargas where o.Codigo == ocorrencia select o).FirstOrDefault();   

            var result = _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(ocorrencia, null, null, 0, 0, "", veiculo, resultCargas.TipoOcorrencia.FiltrarCargasPeriodo, resultCargas.TipoOcorrencia.Codigo);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return await result.ToListAsync();
        }

        public int ContarConsultaDocumentosCargasOcorrenciaPorPeriodo(int ocorrencia, int veiculo)
        {
            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var resultCargas = (from o in queryCargas where o.Codigo == ocorrencia select o).FirstOrDefault();
            /*var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var resultCargas = (from o in queryCargas where o.Codigo == ocorrencia select o).FirstOrDefault();
            var codigosCargas = (from o in resultCargas.Cargas select o.Codigo).ToArray();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from o in query where codigosCargas.Contains(o.Carga.Codigo) && o.CTe != null && o.CargaCTeComplementoInfo == null select o;*/

            var result = _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(ocorrencia, null, null, 0, 0, "", veiculo, resultCargas.TipoOcorrencia.FiltrarCargasPeriodo, resultCargas.TipoOcorrencia.Codigo);

            return result.Count();
        }

        public async Task<int> ContarConsultaDocumentosCargasOcorrenciaPorPeriodoAsync(int ocorrencia, int veiculo)
        {
            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var resultCargas = (from o in queryCargas where o.Codigo == ocorrencia select o).FirstOrDefault();           

            var result = _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(ocorrencia, null, null, 0, 0, "", veiculo, resultCargas.TipoOcorrencia.FiltrarCargasPeriodo, resultCargas.TipoOcorrencia.Codigo);

            return await result.CountAsync();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(int ocorrencia, DateTime? periodoInicial, DateTime? periodoFim, int transportador, int filial, string proprietario, int veiculo, bool filtrarCargasPeriodo, int tipoOcorrencia)
        {
            IList<int> codigosCargaCTe = CodigosCargaCTePorVeiculoEPeriodoOuOcorrencia(ocorrencia, periodoInicial.HasValue ? periodoInicial.Value : DateTime.MinValue, periodoFim.HasValue ? periodoFim.Value : DateTime.MinValue, transportador, filial, proprietario, veiculo, filtrarCargasPeriodo, tipoOcorrencia);

            codigosCargaCTe = codigosCargaCTe.Skip(0).Take(2000).ToList();

            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from o in queryCargas
                         where
                             codigosCargaCTe.Contains(o.Codigo)
                             && o.CTe != null
                             && o.CargaCTeComplementoInfo == null
                             && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal
                         select o;

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(int ocorrencia, DateTime? periodoInicial, DateTime? periodoFim, int transportador, string proprietario, string placa, int contrato, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento)
        {
            IList<int> codigosCargaCTe = CodigosCargaCTePorVeiculoEContratoOuOcorrencia(ocorrencia, periodoInicial ?? DateTime.MinValue, periodoFim ?? DateTime.MinValue, transportador, proprietario, placa, contrato, cnpjRemetente, cnpjDestinatario, modeloDocumento);
            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from o in queryCargas
                         where
                             codigosCargaCTe.Contains(o.Codigo)
                             && o.CTe != null
                             && o.CargaCTeComplementoInfo == null
                             && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal
                         select o;

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarDocumentosCargasPorPeriodo(DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, int veiculo, bool filtrarCargasPeriodo, int tipoOcorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            int ocorrencia = 0;
            var result = _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, filial, proprietario, veiculo, filtrarCargasPeriodo, tipoOcorrencia);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> ConsultarDocumentosCargasPorPeriodoAsync(DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, int veiculo, bool filtrarCargasPeriodo, int tipoOcorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            int ocorrencia = 0;
            var result = _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, filial, proprietario, veiculo, filtrarCargasPeriodo, tipoOcorrencia);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return await result.ToListAsync();
        }

        public int ContarConsultaDocumentosCargasPorPeriodo(DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, int veiculo, bool filtrarCargasPeriodo, int tipoOcorrencia)
        {
            int ocorrencia = 0;
            var result = _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, filial, proprietario, veiculo, filtrarCargasPeriodo, tipoOcorrencia);

            return result.Count();
        }
        public async Task<int> ContarConsultaDocumentosCargasPorPeriodoAsync(DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, int veiculo, bool filtrarCargasPeriodo, int tipoOcorrencia)
        {
            int ocorrencia = 0;
            var result = _ConsultaCargaCTesPorVeiculoPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, filial, proprietario, veiculo, filtrarCargasPeriodo, tipoOcorrencia);

            return await result.CountAsync();
        }

        public int ContarConsultaDocumentosCargasPorContratoEVeiculoDaOcorrencia(int ocorrencia, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento)
        {
            var result = _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(ocorrencia, DateTime.MinValue, DateTime.MinValue, 0, "", "", 0, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            return result.Count();
        }
        public async Task<int> ContarConsultaDocumentosCargasPorContratoEVeiculoDaOcorrenciaAsync(int ocorrencia, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento)
        {
            var result = _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(ocorrencia, DateTime.MinValue, DateTime.MinValue, 0, "", "", 0, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            return await result.CountAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarDocumentosCargasPorContratoEVeiculoDaOcorrencia(int ocorrencia, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(ocorrencia, DateTime.MinValue, DateTime.MinValue, 0, "", "", 0, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> ConsultarDocumentosCargasPorContratoEVeiculoDaOcorrenciaAsync(int ocorrencia, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(ocorrencia, DateTime.MinValue, DateTime.MinValue, 0, "", "", 0, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return await result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarDocumentosCargasPorContratoEVeiculo(DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string veiculo, int contrato, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            int ocorrencia = 0;
            var result = _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, proprietario, veiculo, contrato, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> ConsultarDocumentosCargasPorContratoEVeiculoAsync(DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string veiculo, int contrato, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            int ocorrencia = 0;
            var result = _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, proprietario, veiculo, contrato, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return await result.ToListAsync();
        }

        public int ContarConsultaDocumentosCargasPorContratoEVeiculo(DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string veiculo, int contrato, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento)
        {
            int ocorrencia = 0;
            var result = _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, proprietario, veiculo, contrato, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            return result.Count();
        }

        public async Task<int> ContarConsultaDocumentosCargasPorContratoEVeiculoAsync(DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string veiculo, int contrato, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento)
        {
            int ocorrencia = 0;
            var result = _ConsultaCargaCTesContratoVeiculoPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, proprietario, veiculo, contrato, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            return await result.CountAsync();
        }


        private IList<int> CodigosCargaCTePorVeiculoEPeriodoOuOcorrencia(int ocorrencia, DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, int veiculo, bool filtrarCargasPeriodo, int tipoOcorrencia)
        {
            string sql = ObterSelectCodigosCargaCTePorVeiculoEPeriodoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, filial, proprietario, veiculo, filtrarCargasPeriodo, tipoOcorrencia);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.List<int>();
        }

        private IList<int> CodigosCargaCTePorVeiculoEContratoOuOcorrencia(int ocorrencia, DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string placa, int contrato, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento)
        {
            string sql = ObterSelectCodigosCargaCTePorVeiculoEContratoOuOcorrencia(ocorrencia, periodoInicial, periodoFim, transportador, proprietario, placa, contrato, cnpjRemetente, cnpjDestinatario, modeloDocumento);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.List<int>();
        }

        private string ObterSelectCodigosCargaCTePorVeiculoEPeriodoOuOcorrencia(int ocorrencia, DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, int veiculo, bool filtrarCargasPeriodo, int tipoOcorrencia)
        {
            string select = @"CT.CCT_CODIGO Documento";
            string where = RetornaWhereConsultaCargasPeriodo(ocorrencia, periodoInicial, periodoFim, transportador, filial, proprietario, veiculo);

            string unionSelect = ObterSelectUnionCargasPeriodo(select, where, ocorrencia, filtrarCargasPeriodo, tipoOcorrencia);

            string query = @"
                SELECT DISTINCT CAST(Documento as int)
	            FROM ( " + unionSelect + @" ) CargaCTes";
            
            return query;
        }

        private string ObterSelectUnionCargasPeriodo(string select, string where, int ocorrencia, bool filtrarCargasPeriodo, int tipoOcorrencia)
        {
            string joins = @"JOIN T_CTE CTE
                                ON CT.CON_CODIGO = CTE.CON_CODIGO
                            JOIN T_CTE_PARTICIPANTE REM
                                ON CTE.CON_REMETENTE_CTE = REM.PCT_CODIGO
                            JOIN T_CTE_PARTICIPANTE DEST
                                ON CTE.CON_DESTINATARIO_CTE = DEST.PCT_CODIGO
                            JOIN T_MODDOCFISCAL MODE
                                ON CTE.CON_MODELODOC = MODE.MOD_CODIGO
                            JOIN T_CARGA CA
                                ON CA.CAR_CODIGO = CT.CAR_CODIGO
                            JOIN T_VEICULO VE
                                ON CA.CAR_VEICULO = VE.VEI_CODIGO
                            LEFT JOIN T_MODELO_VEICULAR_CARGA MV
                                ON VE.MVC_CODIGO = MV.MVC_CODIGO";

            if(ocorrencia > 0)
            {
                joins += @" JOIN T_CARGA_OCORRENCIA_CARGAS CargasOcorrencia
						        ON CA.CAR_CODIGO = CargasOcorrencia.CAR_CODIGO ";
            }

            string joinFiltro = @" JOIN T_OCORRENCIA_FILTRO_PERIODO OFP
                                        ON OFP.CLI_REMENTENTE = REM.PCT_CPF_CNPJ 
                                        AND OFP.CLI_DESTINATARIO = DEST.PCT_CPF_CNPJ 
							            AND OFP.OCO_CODIGO = " + tipoOcorrencia.ToString();
            
            string orderBy = string.Empty;

            // Select
            //string query = @"
            //        SELECT " + select + @" 
            //        FROM T_CARGA_CTE CT 
            //        " + joins + (filtrarCargasPeriodo ? joinFiltro : "") + @"
            //        WHERE " + (filtrarCargasPeriodo ? " MODE.MOD_NUM <> '39' AND " : "") + where +
            //        (filtrarCargasPeriodo ?
            //            @"
            //            UNION
            //            SELECT " + select + @"
            //            FROM T_CARGA_CTE CT
            //            " + joins + @"
            //            WHERE MODE.MOD_NUM = '39' AND " + where
            //        : "");        
            string query = @"
                    SELECT " + select + @" 
                    FROM T_CARGA_CTE CT 
                    " + joins + (filtrarCargasPeriodo ? joinFiltro : "") + @"
                    WHERE " + where;

            return query;
        }

        private string ObterSelectConsultaCargasPeriodo(bool count, DateTime periodoInicial, DateTime periodoFim, int transportador, int filial, string proprietario, bool filtrarCargasPeriodo, int tipoOcorrencia, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            int ocorrencia = 0;
            string select = @"CA.CAR_VEICULO CodigoVeiculo,
                            VE.VEI_PLACA Veiculo, 
                            MV.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR ModeloVeicular,
                            CA.CAR_CODIGO Carga,
                            (dateadd(dd, 0, datediff(dd, 0, CA.CAR_DATA_CRIACAO))) Dia,
                            CTE.CON_VALOR_TOTAL_MERC ValorNota, 
                            CT.CON_CODIGO Documento";

            string selectUnion = @"CodigoVeiculo, 
                                   Veiculo, 
	                               ModeloVeicular,
	                               COUNT(DISTINCT Carga) QuantidadeCargas,
	                               COUNT(DISTINCT Dia) QuantidadeDias,
	                               SUM(ValorNota) ValorNotas,
	                               COUNT(Documento) QuantidadeDocumentos ";

            string where = RetornaWhereConsultaCargasPeriodo(ocorrencia, periodoInicial, periodoFim, transportador, filial, proprietario, 0);
            string groupBy = "CodigoVeiculo, Veiculo, ModeloVeicular ";
            string orderBy = string.Empty;

            string unionSelect = ObterSelectUnionCargasPeriodo(select, where, ocorrencia, filtrarCargasPeriodo, tipoOcorrencia);

            string query = @"
                SELECT " + (count ? " DISTINCT(COUNT(0) OVER ())" : selectUnion) + @"
	            FROM ( " + unionSelect + @" ) CargasPeriodo
                GROUP BY " + groupBy;

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propOrdena) && selectUnion.Contains(propOrdena))
                    orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                query += " ORDER BY " + (orderBy.Length > 0 ? orderBy : "1 ASC ");

                if (inicio > 0 || limite > 0)
                    query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;";
            }

            return query;
        }

        private string RetornaWhereConsultaCargasPeriodo(int ocorrencia, DateTime periodoInicial, DateTime periodoFinal, int codigoTransportador, int codigoFilial, string proprietario, int veiculo)
        {
            // Se o filtro é por ocorrencia, ignora todos os outros filtros e tras apenas o relacionamento 
            // com a tablea de cargas da ocorrencia
            string where = " CT.CCC_CODIGO IS NULL ";

            if(ocorrencia > 0)
            {
                where += " AND CargasOcorrencia.COC_CODIGO = " + ocorrencia.ToString();
            }
            else
            {
                where += " AND (CAR_SITUACAO = 11 or CAR_SITUACAO = 9 or CAR_SITUACAO = 7 or CAR_SITUACAO = 15 or CAR_SITUACAO = 10) ";
                where += " AND CAR_DATA_CRIACAO BETWEEN '" + periodoInicial.ToString("yyyy-MM-dd") + "' AND '" + periodoFinal.ToString("yyyy-MM-dd") + " 23:59:59'";

                if (codigoTransportador > 0 && !string.IsNullOrWhiteSpace(proprietario))
                    where += " AND (CA.EMP_CODIGO = " + codigoTransportador.ToString() + " OR VE.VEI_PROPRIETARIO = '" + proprietario.ToString() + "' )";
                else if (codigoTransportador > 0 || !string.IsNullOrWhiteSpace(proprietario))
                    where += " AND " + (codigoTransportador > 0 ? "CA.EMP_CODIGO = " + codigoTransportador.ToString() : "") + (!string.IsNullOrWhiteSpace(proprietario) ? "VE.VEI_PROPRIETARIO = '" + proprietario.ToString() + "'" : "");
                if (codigoFilial > 0)
                    where += " AND CA.FIL_CODIGO = '" + codigoFilial.ToString() + "'";            
            }

            if (veiculo > 0)
                where += " AND VEI_CODIGO = " + veiculo.ToString();

            return where;
        }


        private string ObterSelectCodigosCargaCTePorVeiculoEContratoOuOcorrencia(int ocorrencia, DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string placa, int contrato, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento)
        {
            string select = @"CT.CCT_CODIGO Documento";
            string where = RetornaWhereConsultaDocumentosAgrupados(ocorrencia, periodoInicial, periodoFim, transportador, proprietario, placa, contrato, cnpjRemetente, cnpjDestinatario, modeloDocumento);

            string unionSelect = ObterSelectUnionDocumentosAgrupados(select, where, ocorrencia, contrato);

            string query = @"
                SELECT DISTINCT CAST(Documento as int)
	            FROM ( " + unionSelect + @" ) DocumentosAgrupados";

            return query;
        }

        private string ObterSelectConsultaDocumentosAgrupados(bool count, DateTime periodoInicial, DateTime periodoFim, int transportador, string proprietario, string placa, int contrato, int ocorrencia, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = @"MAX(Carga) Codigo, 
	                          CnpjRemetente, 
	                          CodigoRemetente, 
	                          NomeRemetente, 
                              CnpjDestinatario,
	                          CodigoDestinatario,
	                          NomeDestinatario,
	                          ModeloDocumento,
	                          DescricaoModeloDocumento,
	                          COUNT(DISTINCT Carga) QuantidadeCargas,
	                          SUM(ValorMercadoria) ValorMercadoria,
	                          COUNT(Documento) QuantidadeDocumentos ";

            string selectUnion = @"REM.PCT_CPF_CNPJ CnpjRemetente,
                                   CLI_REM.CLI_CODIGO_INTEGRACAO CodigoRemetente,
		                           REM.PCT_NOME NomeRemetente,
	                               DEST.PCT_CPF_CNPJ CnpjDestinatario,
                                   CLI_DEST.CLI_CODIGO_INTEGRACAO CodigoDestinatario,
		                           DEST.PCT_NOME NomeDestinatario,
		                           MODE.MOD_CODIGO ModeloDocumento,
		                           MODE.MOD_ABREVIACAO DescricaoModeloDocumento,
                                   CA.CAR_CODIGO Carga,
                                   CTE.CON_VALOR_TOTAL_MERC ValorMercadoria, 
                                   CT.CON_CODIGO Documento ";

            string where = RetornaWhereConsultaDocumentosAgrupados(ocorrencia, periodoInicial, periodoFim, transportador, proprietario, placa, contrato, "", "", 0);
            string groupBy = "CnpjRemetente, CodigoRemetente, NomeRemetente, CnpjDestinatario, CodigoDestinatario, NomeDestinatario, ModeloDocumento, DescricaoModeloDocumento ";
            string orderBy = string.Empty;

            string unionSelect = ObterSelectUnionDocumentosAgrupados(selectUnion, where, ocorrencia, contrato);

            string query = @"
                SELECT " + (count ? " DISTINCT(COUNT(0) OVER ())" : select) + @"
	            FROM ( " + unionSelect + @" ) DocumentosAgrupados
                GROUP BY " + groupBy;

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propOrdena) && selectUnion.Contains(propOrdena))
                    orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                query += " ORDER BY " + (orderBy.Length > 0 ? orderBy : "1 ASC ");

                if (inicio > 0 || limite > 0)
                    query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;";
            }

            return query;
        }

        private string ObterSelectUnionDocumentosAgrupados(string select, string where, int ocorrencia, int contrato)
        {
            string joins = @"JOIN T_CTE CTE
                                ON CT.CON_CODIGO = CTE.CON_CODIGO
                            JOIN T_CTE_PARTICIPANTE REM
                                ON CTE.CON_REMETENTE_CTE = REM.PCT_CODIGO
	                        LEFT JOIN T_CLIENTE CLI_REM 
	                            ON REM.CLI_CODIGO = CLI_REM.CLI_CGCCPF
                            JOIN T_CTE_PARTICIPANTE DEST
                                ON CTE.CON_DESTINATARIO_CTE = DEST.PCT_CODIGO
	                        LEFT JOIN T_CLIENTE CLI_DEST 
	                            ON DEST.CLI_CODIGO = CLI_REM.CLI_CGCCPF
                            JOIN T_MODDOCFISCAL MODE
                                ON CTE.CON_MODELODOC = MODE.MOD_CODIGO
                            JOIN T_CARGA CA
                                ON CA.CAR_CODIGO = CT.CAR_CODIGO
                            JOIN T_VEICULO VE
                                ON CA.CAR_VEICULO = VE.VEI_CODIGO";

            if(contrato > 0)
            {
                joins += @" JOIN T_CONTRATO_FRETE_TRANSPORTADOR_CLIENTE CO
	                            ON CO.CLI_CGCCPF = REM.PCT_CPF_CNPJ";
            }

            if (ocorrencia > 0)
            {
                joins += @" JOIN T_CARGA_OCORRENCIA_CARGAS CargasOcorrencia
						        ON CA.CAR_CODIGO = CargasOcorrencia.CAR_CODIGO ";
            }


            string orderBy = string.Empty;

            string query = @"
                    SELECT " + select + @" 
                    FROM T_CARGA_CTE CT 
                    " + joins + @"
                    WHERE " + where;

            return query;
        }

        private string RetornaWhereConsultaDocumentosAgrupados(int ocorrencia, DateTime periodoInicial, DateTime periodoFinal, int codigoTransportador, string proprietario, string placa, int contrato, string cnpjRemetente, string cnpjDestinatario, int modeloDocumento)
        {
            string where = " CT.CCC_CODIGO IS NULL AND CTE.CON_TIPO_CTE = 0 ";
            
            if (ocorrencia > 0)
            {
                where += " AND CargasOcorrencia.COC_CODIGO = " + ocorrencia.ToString();
            }
            else
            {
                where += " AND (CAR_SITUACAO = 11 or CAR_SITUACAO = 9 or CAR_SITUACAO = 7 or CAR_SITUACAO = 15 or CAR_SITUACAO = 10) ";
                where += " AND CAR_DATA_CRIACAO BETWEEN '" + periodoInicial.ToString("yyyy-MM-dd") + "' AND '" + periodoFinal.ToString("yyyy-MM-dd") + " 23:59:59'";

                if (codigoTransportador > 0 && !string.IsNullOrWhiteSpace(proprietario))
                    where += " AND (CA.EMP_CODIGO = " + codigoTransportador.ToString() + " OR VE.VEI_PROPRIETARIO = '" + proprietario.ToString() + "' )";
                else if (codigoTransportador > 0 || !string.IsNullOrWhiteSpace(proprietario))
                    where += " AND " + (codigoTransportador > 0 ? "CA.EMP_CODIGO = " + codigoTransportador.ToString() : "") + (!string.IsNullOrWhiteSpace(proprietario) ? "VE.VEI_PROPRIETARIO = '" + proprietario.ToString() + "'" : "");
            }

            if (!string.IsNullOrWhiteSpace(placa))
                where += " AND VE.VEI_PLACA = '" + placa + "'";

            if (contrato > 0)
                where += " AND CO.CFT_CODIGO = " + contrato.ToString();

            if (!string.IsNullOrWhiteSpace(cnpjRemetente))
                where += " AND REM.PCT_CPF_CNPJ = '" + cnpjRemetente + "'";

            if (!string.IsNullOrWhiteSpace(cnpjDestinatario))
                where += " AND DEST.PCT_CPF_CNPJ = '" + cnpjDestinatario + "'";

            if (modeloDocumento > 0)
                where += " AND MODE.MOD_CODIGO = " + modeloDocumento.ToString();

            return where;
        }
    }
}
