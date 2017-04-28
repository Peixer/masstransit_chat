namespace Chat
{
    internal class ConfiguracaoRabbitMQ
    {
        public string Servidor { get; private set; }
        public string Usuario { get; private set; }
        public string Senha { get; private set; }

        public ConfiguracaoRabbitMQ(string servidor, string usuario, string senha)
        {
            Servidor = servidor;
            Usuario = usuario;
            Senha = senha;
        }
    }
}