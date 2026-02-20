using System.Collections.Generic;

namespace Dominio.Interfaces.Repositorios
{
    public interface Empresa : Base<Dominio.Entidades.Empresa>
    {
        Dominio.Entidades.Empresa BuscarPorCodigo(int codigo);
        Dominio.Entidades.Empresa BuscarPorCodigoEEmpresaPai(int codigo, int codigoEmpresaPai);
        List<Dominio.Entidades.Empresa> BuscarTodos(int codigoEmpresa, int maximoRegistros, int inicioRegistros);
        int ContarTodos(int codigoEmpresa);
        List<Dominio.Entidades.Empresa> Consultar(int codigoEmpresa, string nome, string cnpj, string placaVeiculo, string status, int inicioRegistros, int maximoRegistros, string propOrdenacao = "Codigo", string dirOrdenacao = "desc", bool SomenteProducao = false, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao, bool semEmpresaPai = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor sistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos, int filial = 0, string nomeFantasia = "", List<int> codigos = null, string ufFilialTransportador = "");
        int ContarConsulta(int codigoEmpresa, string nome, string cnpj, string placaVeiculo, string status, bool SomenteProducao = false, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao, bool semEmpresaPai = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor sistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos, int filial = 0, string nomeFantasia = "", List<int> codigos = null, string ufFilialTransportador = "");
    }
}
