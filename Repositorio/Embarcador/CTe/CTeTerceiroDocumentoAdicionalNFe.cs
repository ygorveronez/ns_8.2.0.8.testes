using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroDocumentoAdicionalNFe : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe>
    {
        public CTeTerceiroDocumentoAdicionalNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe> BuscarPorCTeTerceiroEChaveAcessoCTe(int codigoCTeTerceiro, string chaveAcessoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe>();
            var result = from obj in query where obj.CTeTerceiro.Codigo == codigoCTeTerceiro && obj.CTeTerceiroDocumentoAdicional.Chave == chaveAcessoCTe select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe BuscarPorCTeTerceiroEChaveAcessoNFe(int codigoCTeTerceiro, string chaveAcessoNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe>();
            var result = from obj in query where obj.CTeTerceiro.Codigo == codigoCTeTerceiro && obj.Chave == chaveAcessoNFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe> BuscarPorCTeTerceiro(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicionalNFe>();
            var result = from obj in query where obj.CTeTerceiro.Codigo == codigoCTeTerceiro select obj;
            return result.ToList();
        }
    }
}
