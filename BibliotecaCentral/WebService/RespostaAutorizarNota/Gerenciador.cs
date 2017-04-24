﻿using System.Threading.Tasks;

namespace BibliotecaCentral.WebService.RespostaAutorizarNota
{
    public static class Gerenciador
    {
        public static async Task<Response> ObterRespostaAutorizacao(AutorizarNota.CorpoResponse recibo)
        {
            using (var conexao = new Conexao<IRespostaAutorizaNFe>(ConjuntoServicos.RespostaAutorizar.Endereco))
            {
                var procura = conexao.EstabelecerConexão();
                return await new GerenciadorGeral<Request, Response>(
                    procura.nfeRetAutorizacaoLote, procura.nfeRetAutorizacaoLoteAsync,
                    ConjuntoServicos.RespostaAutorizar).EnviarAsync(new Request
                    {
                        consReciNFe = new CorpoRequest(recibo.tpAmb, recibo.infRec.nRec)
                    }, recibo.cUF);
            }
        }
    }
}
