using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{

    public class SolicitacaoEmissao : RepositorioBase<Dominio.Entidades.SolicitacaoEmissao>, Dominio.Interfaces.Repositorios.SolicitacaoEmissao
    {
        public SolicitacaoEmissao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.SolicitacaoEmissao BuscaPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SolicitacaoEmissao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public int ContasSolicitacoesPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SolicitacaoEmissao>();

            var result = from obj in query where obj.Status == Dominio.Enumeradores.StatusSolicitacaoEmissao.Pendente select obj;

            return result.Count();
        }


        public List<Dominio.Entidades.SolicitacaoEmissao> Consultar(int codigo, int codigoUsuarioEnvio, int codigoUsuarioRetorno, string assunto, string texto, DateTime dataEnvio, DateTime dataRetorno, Dominio.Enumeradores.StatusSolicitacaoEmissao? status, string usuarioEnvio, string nomeTransportador, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SolicitacaoEmissao>();

            var result = from obj in query where 1 == 1 select obj;

            if (codigo > 0)
                result = result.Where(o => o.Codigo == codigo);

            if (codigoUsuarioEnvio > 0)
                result = result.Where(o => o.UsuarioEnvio.Codigo == codigoUsuarioEnvio);

            if (codigoUsuarioRetorno > 0)
                result = result.Where(o => o.UsuarioRetorno.Codigo == codigoUsuarioRetorno);

            if (!string.IsNullOrWhiteSpace(assunto))
                result = result.Where(o => o.Assunto.Contains(assunto));

            if (!string.IsNullOrWhiteSpace(texto))
                result = result.Where(o => o.Assunto.Contains(texto));

            if (dataEnvio != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio == dataEnvio.Date);

            if (dataRetorno != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio == dataRetorno.Date);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio <= dataFinal.Date);

            if (status != null)
                result = result.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(usuarioEnvio))
                result = result.Where(o => o.UsuarioEnvio.Nome.Contains(usuarioEnvio));

            if (!string.IsNullOrWhiteSpace(nomeTransportador))
                result = result.Where(o => o.Empresa.RazaoSocial.Contains(nomeTransportador));

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigo, int codigoUsuarioEnvio, int codigoUsuarioRetorno, string assunto, string texto, DateTime dataEnvio, DateTime dataRetorno, Dominio.Enumeradores.StatusSolicitacaoEmissao? status, string usuarioEnvio, string nomeTransportador, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SolicitacaoEmissao>();

            var result = from obj in query where 1 == 1 select obj;

            if (codigo > 0)
                result = result.Where(o => o.Codigo == codigo);

            if (codigoUsuarioEnvio > 0)
                result = result.Where(o => o.UsuarioEnvio.Codigo == codigoUsuarioEnvio);

            if (codigoUsuarioRetorno > 0)
                result = result.Where(o => o.UsuarioRetorno.Codigo == codigoUsuarioRetorno);

            if (!string.IsNullOrWhiteSpace(assunto))
                result = result.Where(o => o.Assunto.Contains(assunto));

            if (!string.IsNullOrWhiteSpace(texto))
                result = result.Where(o => o.Assunto.Contains(texto));

            if (dataEnvio != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio == dataEnvio.Date);

            if (dataRetorno != DateTime.MinValue)
                result = result.Where(o => o.DataRetorno == dataRetorno);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEnvio <= dataFinal.Date);

            if (status != null)
                result = result.Where(o => o.Status == status);

            if (!string.IsNullOrWhiteSpace(usuarioEnvio))
                result = result.Where(o => o.UsuarioEnvio.Nome.Contains(usuarioEnvio));

            if (!string.IsNullOrWhiteSpace(nomeTransportador))
                result = result.Where(o => o.Empresa.RazaoSocial.Contains(nomeTransportador));

            return result.Count();
        }

    }
}
