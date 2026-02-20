using System.Collections.Generic;

namespace Servicos
{
    public static class Atividade
    {
        public static Dominio.Entidades.Atividade ObterAtividade(int codigoEmpresa, string tipoCliente, string stringConexao, int codigoAtividade = 0, Repositorio.UnitOfWork unitOfWork = null, List<Dominio.Entidades.Atividade> lstAtividade = null, List<Dominio.Entidades.Empresa> lstEmpresas = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Atividade repositorioAtividade = new Repositorio.Atividade(unitOfWork);

            if (codigoAtividade > 0)
                return repositorioAtividade.BuscarPorCodigo(codigoAtividade, lstAtividade);

            if (tipoCliente == "F" || tipoCliente == "E")
                return repositorioAtividade.BuscarPorCodigo(7, lstAtividade);

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(codigoEmpresa, lstEmpresas);

            if (empresa != null)
            {
                if (empresa.Configuracao?.Atividade != null)
                    return empresa.Configuracao.Atividade;

                if (empresa.EmpresaPai?.Configuracao?.Atividade != null)
                    return empresa.EmpresaPai.Configuracao.Atividade;
            }

            return repositorioAtividade.BuscarPorCodigo(3, lstAtividade);
        }
    }
}
