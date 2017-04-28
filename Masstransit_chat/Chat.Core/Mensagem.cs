using System;

namespace Chat.Core
{
    public class Mensagem
    {
        public Mensagem(Guid guid, string mensagem)
        {
            this.Guid = guid;
            this.Mensagem_Enviada = mensagem;
        }

        public Guid Guid { get; private set; }
        public string Mensagem_Enviada { get; private set; }
    }
}