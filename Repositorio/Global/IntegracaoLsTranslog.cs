using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio
{
    public class IntegracaoLsTranslog : RepositorioBase<Dominio.Entidades.IntegracaoLsTranslog>, Dominio.Interfaces.Repositorios.IntegracaoLsTranslog
    {
        public IntegracaoLsTranslog(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.IntegracaoLsTranslog BuscaPorCodigo(int codigo, int empresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslog>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            if (empresa > 0)
                result = result.Where(o => o.Empresa.Codigo == empresa);

            return result.FirstOrDefault();
        }

        public List<int> BuscarIntegracoesPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslog>();

            var result = from obj in query where obj.StatusEnvio == Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente select obj;

            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarIntegracoesPendentesConsulta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslog>();

            var result = from obj in query where obj.StatusEnvio == Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso && obj.StatusConsulta == Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente select obj;

            return result.OrderBy(o => o.Codigo).Select(o => o.Codigo).ToList();
        }

        private IQueryable<Dominio.Entidades.IntegracaoLsTranslog> _Consultar(int empresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog? tipoDocumento, Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog? statusEnvio, Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog? statusConsulta, string identificador, int numeroNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslog>();

            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;

            if (!string.IsNullOrWhiteSpace(identificador))
            {
                var query2 = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslogLog>();

                result = result.Where(o => (from obj in query2 where obj.Identificador.Equals(identificador) select obj.IntegracaoLsTranslog.Codigo).Contains(o.Codigo));
            }

            if (numeroNota > 0)
            {
                var query2 = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoLsTranslogLog>();

                result = result.Where(o => (from obj in query2 where obj.NumeroNFe == numeroNota select obj.IntegracaoLsTranslog.Codigo).Contains(o.Codigo));
            }

            if (numeroInicial > 0)
                result = result.Where(o => o.CTe != null ? o.CTe.Numero >= numeroInicial : o.NFSe != null ? o.NFSe.Numero >= numeroInicial : int.Parse(o.NFe.Numero) >= numeroInicial);
            if (numeroFinal > 0) 
                result = result.Where(o => o.CTe != null ? o.CTe.Numero <= numeroFinal : o.NFSe != null ? o.NFSe.Numero <= numeroFinal : int.Parse(o.NFe.Numero) <= numeroFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.Data.Date >= dataInicial);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.Data.Date <= dataFinal);

            if (tipoDocumento.HasValue && tipoDocumento.Value == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.CTe)
                result = result.Where(o => o.CTe != null);
            else if (tipoDocumento.HasValue && tipoDocumento.Value == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.NFSe)
                result = result.Where(o => o.NFSe != null);
            else if (tipoDocumento.HasValue && tipoDocumento.Value == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.NFe)
                result = result.Where(o => o.NFe != null);

            if (statusEnvio.HasValue)
                result = result.Where(o => o.StatusEnvio == statusEnvio.Value);
            if (statusConsulta.HasValue)
                result = result.Where(o => o.StatusConsulta == statusConsulta.Value);

            return result;
        }

        public List<Dominio.Entidades.IntegracaoLsTranslog> Consultar(int empresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog? tipoDocumento, Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog? statusEnvio, Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog? statusConsulta, string identificador, int numeroNota, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(empresa, numeroInicial, numeroFinal, dataInicial, dataFinal, tipoDocumento, statusEnvio, statusConsulta, identificador, numeroNota);

            result = result.OrderBy("Codigo descending");

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int empresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog? tipoDocumento, Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog? statusEnvio, Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog? statusConsulta, string identificador, int numeroNota)
        {
            var result = _Consultar(empresa, numeroInicial, numeroFinal, dataInicial, dataFinal, tipoDocumento, statusEnvio, statusConsulta, identificador, numeroNota);

            return result.Count();
        }
    }
}
