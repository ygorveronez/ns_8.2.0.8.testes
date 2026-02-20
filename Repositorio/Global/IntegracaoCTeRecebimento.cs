using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class IntegracaoCTeRecebimento : RepositorioBase<Dominio.Entidades.IntegracaoCTeRecebimento>, Dominio.Interfaces.Repositorios.IntegracaoCTeRecebimento
    {
        public IntegracaoCTeRecebimento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.IntegracaoCTeRecebimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTeRecebimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.IntegracaoCTeRecebimento BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTeRecebimento>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.IntegracaoCTeRecebimento> Consultar(Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe? status, int numeroCTe, DateTime data, string empresa, string usuario,  int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTeRecebimento>();

            var result = from obj in query select obj;

            result = result.Where(o => o.Tipo == Dominio.Enumeradores.TipoIntegracaoEnvioCTe.Autorizacao);

            if (status != null)
                result = result.Where(o => o.Status == status);

            if (numeroCTe > 0)
                result = result.Where(o => (o.CTe.Numero == numeroCTe));

            if (data > DateTime.MinValue)
                result = result.Where(o => (o.CTe.DataEmissao >= data.Date && o.CTe.DataEmissao < data.AddDays(1).Date));

            if (!string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => (o.CTe.Empresa.RazaoSocial.Contains(empresa)));

            if (!string.IsNullOrWhiteSpace(usuario))
                result = result.Where(o => (o.Usuario.Nome.Contains(usuario)));

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.Enumeradores.StatusIntegracaoRecebimentoCTe? status, int numeroCTe, DateTime data, string empresa, string usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTeRecebimento>();
            
            var result = from obj in query select obj;

            result = result.Where(o => o.Tipo == Dominio.Enumeradores.TipoIntegracaoEnvioCTe.Autorizacao);

            if (status != null)
                result = result.Where(o => o.Status == status);

            if (numeroCTe > 0)
                result = result.Where(o => (o.CTe.Numero == numeroCTe));

            if (data > DateTime.MinValue)
                result = result.Where(o => (o.CTe.DataEmissao >= data.Date && o.Data < data.AddDays(1).Date));

            if (!string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => (o.CTe.Empresa.RazaoSocial.Contains(empresa)));

            if (!string.IsNullOrWhiteSpace(usuario))
                result = result.Where(o => (o.Usuario.Nome.Contains(usuario)));

            return result.Count();
        }
    }
}
