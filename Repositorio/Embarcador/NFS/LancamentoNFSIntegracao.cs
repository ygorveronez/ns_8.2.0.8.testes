using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class LancamentoNFSIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao>
    {

        public LancamentoNFSIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao> BuscarPorLancamentoNFSManual(int lancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao>();
            var resut = from obj in query where obj.LancamentoNFSManual.Codigo == lancamentoNFSManual select obj;
            return resut.ToList();
        }

        //public Dominio.Entidades.Embarcador.Cargas.CargaIntegracao BuscarPorCodigo(int codigo)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao>();
        //    var resut = from obj in query where obj.Codigo == codigo select obj;
        //    return resut.FirstOrDefault();
        //}

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposPorLancamentoNFSManual(int lancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao>();
            var resut = from obj in query where obj.LancamentoNFSManual.Codigo == lancamentoNFSManual select obj.TipoIntegracao.Tipo;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao> BuscarPorCarga(int lancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao>();
            var resut = from obj in query where obj.LancamentoNFSManual.Codigo == lancamentoNFSManual select obj;
            return resut.ToList();
        }

        public int ContarPorLancamentoNFSManual(int lancamentoNFSManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao>();

            var resut = from obj in query where obj.LancamentoNFSManual.Codigo == lancamentoNFSManual select obj;

            return resut.Count();
        }

        public int ContarPorLancamentoNFSManualETipoIntegracao(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSIntegracao>();

            var resut = from obj in query where obj.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return resut.Count();
        }

    }
}
