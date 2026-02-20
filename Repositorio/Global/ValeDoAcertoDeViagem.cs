using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ValeDoAcertoDeViagem : RepositorioBase<Dominio.Entidades.ValeDoAcertoDeViagem>, Dominio.Interfaces.Repositorios.ValeDoAcertoDeViagem
    {
        public ValeDoAcertoDeViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ValeDoAcertoDeViagem BuscarPorCodigoEAcertoDeViagem(int codigo, int codigoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValeDoAcertoDeViagem>();
            var result = from obj in query where obj.Codigo == codigo && obj.AcertoDeViagem.Codigo == codigoAcertoViagem select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ValeDoAcertoDeViagem> BuscarPorAcertoDeViagem(int codigoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValeDoAcertoDeViagem>();
            var result = from obj in query where obj.AcertoDeViagem.Codigo == codigoAcertoViagem orderby obj.Data select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ValeDoAcertoDeViagem> BuscarPorAcertos(List<int> codigosAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValeDoAcertoDeViagem>();
            var result = from obj in query where codigosAcertoViagem.Contains(obj.AcertoDeViagem.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ValeDoAcertoDeViagem> BuscarPorListaDeAcertosDeViagens(List<int> listaCodigosAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValeDoAcertoDeViagem>();
            var result = from obj in query where listaCodigosAcertoViagem.Contains(obj.AcertoDeViagem.Codigo) select obj;
            return result.ToList();
        }

        #region Registros
        // Registro para relatorio 
        // NAO APAGAR
        public List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcertoCabecalho> AcertoValeCabecalho()
        {
            return new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcertoCabecalho>();
        }
        public List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcerto> AcertoVale()
        {
            return new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcerto>();
        }
        #endregion
    }
}
