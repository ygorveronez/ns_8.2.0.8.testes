using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class UsuarioCTe : RepositorioBase<Dominio.Entidades.UsuarioCTe>, Dominio.Interfaces.Repositorios.UsuarioCTe
    {
        public UsuarioCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarPorUsuario(int codigoEmpresa, int codigoUsuario, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UsuarioCTe>();

            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario && obj.Usuario.Empresa.Codigo == codigoEmpresa select obj.CTe;

            return result.OrderByDescending(o => o.DataEmissao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorUsuario(int codigoEmpresa, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UsuarioCTe>();

            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario && obj.Usuario.Empresa.Codigo == codigoEmpresa select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.UsuarioCTe> ConsultarPorUsuario(int codigoEmpresa, int codigoUsuario, int numeroCTe, int serieCTe, DateTime data, string empresa,  int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UsuarioCTe>();

            var result = from obj in query where obj.Usuario.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroCTe == 0 && serieCTe == 0 && data == DateTime.MinValue && string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);

            if (numeroCTe > 0)
                result = result.Where(o => (o.CTe.Numero == numeroCTe) || (o.NFSe.Numero == numeroCTe));

            if (serieCTe > 0)
                result = result.Where(o => (o.CTe.Serie.Numero == serieCTe) || (o.NFSe.Serie.Numero == serieCTe));

            if (data > DateTime.MinValue)
                result = result.Where(o => (o.CTe.DataEmissao >= data.Date && o.CTe.DataEmissao < data.AddDays(1).Date) || (o.NFSe.DataEmissao >= data.Date && o.NFSe.DataEmissao < data.AddDays(1).Date));

            if (!string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => (o.CTe.Empresa.RazaoSocial.Contains(empresa)) || (o.NFSe.Empresa.RazaoSocial.Contains(empresa)));

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorUsuario(int codigoEmpresa, int codigoUsuario, int numeroCTe, int serieCTe, DateTime data, string empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UsuarioCTe>();

            var result = from obj in query where obj.Usuario.Empresa.Codigo == codigoEmpresa select obj;

            if (numeroCTe == 0 && serieCTe == 0 && data == DateTime.MinValue && string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);

            if (numeroCTe > 0)
                result = result.Where(o => o.CTe.Numero == numeroCTe);

            if (serieCTe > 0)
                result = result.Where(o => o.CTe.Serie.Numero == serieCTe);

            if (data > DateTime.MinValue)
                result = result.Where(o => o.CTe.DataEmissao >= data.Date && o.CTe.DataEmissao < data.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(empresa))
                result = result.Where(o => o.CTe.Empresa.RazaoSocial.Contains(empresa));

            return result.Count();
        }

    }
}
