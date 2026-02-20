using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Mobile.Cargas
{
    public class Carga
    {

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> BuscarCargasAgNotas(DateTime dataUltimaVerificacao, int usuarioAPP, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftare, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> cargasMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Usuario> motoristas = repUsuario.BuscarPorTodosCodigoMobile(usuarioAPP);
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                if (motorista.Cliente == null)
                    cargas = repCarga.BuscarCargaAgNotaPorMotorista(motorista.Codigo, dataUltimaVerificacao);
                else
                    cargas = repCarga.BuscarCargaAgNotaPorCliente(motorista.Codigo, dataUltimaVerificacao);

                for (int i = 0; i < cargas.Count; i++)
                    cargasMob.Add(ConverterCarga(cargas[i], clienteMultisoftare, true, unitOfWork));

            }
            return cargasMob;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> BuscarCargas(DateTime dataUltimaVerificacao, int usuarioAPP, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftare, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> cargasMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Usuario> motoristas = repUsuario.BuscarPorTodosCodigoMobile(usuarioAPP);
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                if (motorista.Cliente == null)
                    cargas = repCarga.BuscarCargaPorMotorista(motorista.Codigo, dataUltimaVerificacao);
                else
                    cargas = repCarga.BuscarCargaPorClienteDestino(motorista.Cliente.CPF_CNPJ, dataUltimaVerificacao);

                for (int i = 0; i < cargas.Count; i++)
                    cargasMob.Add(ConverterCarga(cargas[i], clienteMultisoftare, true, unitOfWork));
            }

            return cargasMob;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> BuscarCargasCanceladas(DateTime dataUltimaVerificacao, int usuarioAPP, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftare, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> cargasMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Usuario> motoristas = repUsuario.BuscarPorTodosCodigoMobile(usuarioAPP);
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                if (motorista.Cliente == null)
                    cargas = repCarga.BuscarCargaPorMotoristaCanceladas(motorista.Codigo, dataUltimaVerificacao);
                else
                    cargas = repCarga.BuscarCargaPorClienteDestinoCanceladas(motorista.Cliente.CPF_CNPJ, dataUltimaVerificacao);

                for (int i = 0; i < cargas.Count; i++)
                    cargasMob.Add(ConverterCarga(cargas[i], clienteMultisoftare, false, unitOfWork));

            }
            return cargasMob;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> BuscarCargasEncerradas(DateTime dataUltimaVerificacao, int usuarioAPP, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftare, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> cargasMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Usuario> motoristas = repUsuario.BuscarPorTodosCodigoMobile(usuarioAPP);
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                if (motorista.Cliente == null)
                    cargas = repCarga.BuscarCargaPorMotoristaEncerradas(motorista.Codigo, dataUltimaVerificacao);
                else
                    cargas = repCarga.BuscarCargaPorClienteDestinoEncerradas(motorista.Cliente.CPF_CNPJ, dataUltimaVerificacao);

                for (int i = 0; i < cargas.Count; i++)
                    cargasMob.Add(ConverterCarga(cargas[i], clienteMultisoftare, false, unitOfWork));

            }
            return cargasMob;
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga ConverterCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftare, bool buscarCanhoto, Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Canhotos.Canhotos();
            Servicos.Embarcador.Mobile.Multisoftware.ClienteMultisoftware serClienteMultisoftware = new Multisoftware.ClienteMultisoftware();
            Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga cargaMob = new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga();
            cargaMob.ClienteMultisoftware = serClienteMultisoftware.ConverterClienteMultisoftware(clienteMultisoftare);
            cargaMob.CodigoIntegracao = carga.Codigo;
            cargaMob.NumeroCargaEmbarcador = carga.CodigoCargaEmbarcador;
            cargaMob.DataCarga = carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss");
            cargaMob.Destino = carga?.DadosSumarizados.Destinos ?? "";
            cargaMob.Origem = carga?.DadosSumarizados.Origens ?? "";
            cargaMob.SituacaoCarga = carga.SituacaoCarga;
            cargaMob.Canhotos = buscarCanhoto ? serCanhoto.ObterCanhotosPorCarga(carga, clienteMultisoftare, unitOfWork) : null;
            return cargaMob;
        }


    }
}
