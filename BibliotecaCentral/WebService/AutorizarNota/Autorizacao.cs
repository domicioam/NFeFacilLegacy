﻿using BibliotecaCentral.IBGE;
using BibliotecaCentral.ModeloXML.PartesProcesso;
using System.Threading.Tasks;

namespace BibliotecaCentral.WebService.AutorizarNota
{
    public struct Autorizacao
    {
        Estado UF { get; }

        public Autorizacao(Estado estado)
        {
            UF = estado;
        }

        public Autorizacao(string siglaOuNome)
        {
            UF = Estados.Buscar(siglaOuNome);
        }

        public Autorizacao(ushort codigo)
        {
            UF = Estados.Buscar(codigo);
        }

        public async Task<CorpoResponse> AutorizarAsync(bool teste, params NFe[] xmls)
        {
            return await new GerenciadorGeral<CorpoRequest, CorpoResponse>(UF, Operacoes.Autorizar, teste)
                .EnviarAsync(new CorpoRequest(xmls, xmls[0].Informações.identificação.Numero));
        }
    }
}
