namespace Chat.Core
{
    public class MensagemEntrada
    {
        public MensagemEntrada(string mensagem)
        {
            Mensagem = mensagem;
        }

        public string Mensagem { get; private set; }
    }
}