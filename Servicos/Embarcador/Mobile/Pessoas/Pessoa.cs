using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Mobile.Pessoas
{
    public class Pessoa
    {
        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa> BuscarClientesDigitalizamCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa srvPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa> objClientes = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>();

            List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarClientesDigitalizamCanhoto();
            for (int i = 0, s = clientes.Count; i < s; i++)
                objClientes.Add(srvPessoa.ConverterObjetoPessoa(clientes[i]));

            return objClientes;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa> BuscarGrupoClientesDigitalizamCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa srvPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa> objGruposPessoa = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>();

            List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarClientesDigitalizamCanhoto();

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = (from obj in clientes where obj.GrupoPessoas != null select obj.GrupoPessoas).Distinct().ToList();

            for (int i = 0, s = grupoPessoas.Count; i < s; i++)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = grupoPessoas[i];
                Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa objGrupoPessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa();
                objGrupoPessoa.Descricao = grupoPessoa.Descricao;
                objGrupoPessoa.CodigoIntegracao = grupoPessoa.Codigo.ToString();
                objGruposPessoa.Add(objGrupoPessoa);
            }
                

            return objGruposPessoa;
        }
    }
}
