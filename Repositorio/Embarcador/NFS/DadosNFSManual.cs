using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class DadosNFSManual : RepositorioBase<Dominio.Entidades.Embarcador.NFS.DadosNFSManual>
    {

        public DadosNFSManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NFS.DadosNFSManual BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.DadosNFSManual>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.DadosNFSManual BuscarPorNumeroSerieEmpresa(int numero, int serie, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.DadosNFS.Numero == numero && obj.DadosNFS.Serie.Numero == serie && obj.Transportador.Codigo == empresa && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Cancelada select obj.DadosNFS;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NFS.DadosNFSManual BuscarPorNumeroRPSSerieEmpresa(int numeroRPS, int serie, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();
            var result = from obj in query where obj.DadosNFS.NumeroRPS == numeroRPS && obj.DadosNFS.Serie.Numero == serie && obj.Transportador.Codigo == empresa && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Cancelada select obj.DadosNFS;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.NFS.DadosNFSManual> _Consultar(int transportador, int filial, int numeroInicio, int numeroFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.DadosNFSManual>();

            var result = from obj in query select obj;

            // Filtros
            //if (transportador > 0)
            //    result = result.Where(o => o..Codigo == transportador);

            //if (filial > 0)
            //    result = result.Where(o => o.Filial.Codigo == filial);

            if (numeroInicio > 0)
                result = result.Where(o => o.Numero >= numeroInicio);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.NFS.DadosNFSManual> Consultar(int transportador, int filial, int numeroInicio, int numeroFinal, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(transportador, filial, numeroInicio, numeroFinal);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int transportador, int filial, int numeroInicio, int numeroFinal)
        {
            var result = _Consultar(transportador, filial, numeroInicio, numeroFinal);

            return result.Count();
        }

        public int BuscarProximoNumeroRPS()
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.DadosNFSManual>();

            int? retorno = result.Max(o => (int?)o.NumeroRPS);

            return retorno.HasValue ? (retorno.Value + 1) : 1;
        }

        public int BuscarProximoNumero(int codigoSerie, int codigoModeloDocumentoFiscal, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            query = query.Where(o =>
                o.CTe != null &&
                o.DadosNFS.Serie.Codigo == codigoSerie &&
                o.DadosNFS.ModeloDocumentoFiscal.Codigo == codigoModeloDocumentoFiscal &&
                o.Transportador.Codigo == codigoEmpresa &&
                (!o.CTe.Desabilitado.HasValue || !o.CTe.Desabilitado.Value) &&
                o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Cancelada &&
                o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Reprovada
            );

            int? retorno = query.Max(o => (int?)o.DadosNFS.Numero);

            return retorno.HasValue ? (retorno.Value + 1) : 1;
        }
    }
}
